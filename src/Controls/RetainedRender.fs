namespace FS.Skia.UI.Controls

open FS.Skia.UI.Scene

// Feature 091 (E2) — wiring the parked keyed reconciler (feature 067) onto the live render path.
// This is NOT a new algorithm: it consumes `Reconcile.diff`'s patch and drives the next frame
// from `ControlInternals.evaluateLayout` + `paintNode` (the SAME measure/paint `Control.renderTree`
// uses), reusing cached fragments for unchanged + unshifted subtrees. The render output is
// therefore byte-for-byte identical to a full rebuild of `next` BY CONSTRUCTION (FR-005, C2):
// a reused fragment is reused only when its paint inputs (the node's own data + its computed box)
// are provably unchanged, so it equals what re-painting would have produced.

type internal RetainedId = RetainedId of uint64

type internal RenderFragment =
    { OwnScene: FS.Skia.UI.Scene.Scene list
      SubtreeScene: FS.Skia.UI.Scene.Scene list
      Box: FS.Skia.UI.Scene.Rect option }

type internal RetainedNode<'msg> =
    { Identity: RetainedId
      Control: Control<'msg>
      Fragment: RenderFragment
      Children: RetainedNode<'msg> list }

// Feature 099 (R4): the per-identity animation clock — the feature-073 multi-channel paint carrier
// (opacity/transform/color) plus the accumulated injected `Elapsed` and the `VisualState` the clock
// is animating toward. Generalizes the 091 transform-only carried slot.
type internal AnimationClock =
    { Anim: FS.Skia.UI.Scene.Animation
      Elapsed: System.TimeSpan
      Target: VisualState }

type internal RetainedUiState =
    { Animation: AnimationClock option
      Text: TextInputModel option }

type internal RetainedRender<'msg> =
    { Root: RetainedNode<'msg>
      NextId: uint64
      StateByIdentity: Map<RetainedId, RetainedUiState>
      Theme: Theme
      // Feature 097 (R2): previous frame's full LayoutResult — the measure/bounds cache (FR-002).
      Layout: FS.Skia.UI.Layout.LayoutResult }

type internal WorkReductionRecord =
    { BaselineNodeCount: int
      RecomputedNodeCount: int
      ChangedSubtreeBound: int
      ShiftedNodeCount: int
      // Feature 097 (R2, FR-006): nodes actually re-measured this frame (post-propagation dirty set).
      RemeasuredNodeCount: int }

type internal RetainedRenderStep<'msg> =
    { Retained: RetainedRender<'msg>
      Render: ControlRenderResult<'msg>
      Diagnostics: ControlDiagnostic list
      WorkReduction: WorkReductionRecord }

type internal RetainedInit<'msg> =
    { Retained: RetainedRender<'msg>
      Render: ControlRenderResult<'msg>
      Diagnostics: ControlDiagnostic list }

module internal RetainedRender =

    let private childPath (path: string) (index: int) = path + "." + string index

    // ---------------------------------------------------------------------------------------------
    // Feature 099 (R4) — the per-identity animation clock core. Pure + total + deterministic: every
    // function below depends ONLY on its arguments (no `Date.now`, no randomness, resume-safe). The
    // feature-073 `Animation`/`applyAt`/`isSettled` primitives are REUSED, not re-implemented.
    // ---------------------------------------------------------------------------------------------

    /// The single pinned framework default transition (research §R4 / data-model constant): a short
    /// 150 ms `EaseOut` settle on the opacity channel. A fixed value, not a per-control knob, so the
    /// determinism goldens reach the settled end after the same fixed frame count.
    let defaultTransitionDuration = System.TimeSpan.FromMilliseconds 150.0

    // The longest tween duration carried by an animation (the point past which it is settled).
    let private clockDuration (anim: FS.Skia.UI.Scene.Animation) : System.TimeSpan =
        [ anim.Opacity |> Option.map (fun t -> t.Duration)
          anim.Transform |> Option.map (fun t -> t.Duration)
          anim.Color |> Option.map (fun t -> t.Duration) ]
        |> List.choose id
        |> function
            | [] -> System.TimeSpan.Zero
            | ds -> List.max ds

    // The default fade-in animation: opacity travels from `startOpacity` to fully-shown (1.0) over
    // the framework default, eased out. End = 1.0 means a settled clock samples to opacity 1.0, so
    // `applyAt`'s identity-at-rest lowering makes the converged frame byte-identical to the static
    // render of the (now-stamped) state — FR-005 holds by construction.
    let private fadeAnimation (startOpacity: float) : FS.Skia.UI.Scene.Animation =
        { FS.Skia.UI.Scene.Animation.empty with
            Opacity =
                Some
                    { Start = startOpacity
                      End = 1.0
                      Duration = defaultTransitionDuration
                      Easing = FS.Skia.UI.Scene.EaseOut } }

    // The clock's current sampled opacity (the displayed value a mid-flight retarget continues from).
    let private currentOpacity (clock: AnimationClock) : float =
        match clock.Anim.Opacity with
        | Some tween -> FS.Skia.UI.Scene.Tween.sample FS.Skia.UI.Scene.Animation.lerpFloat clock.Elapsed tween
        | None -> 1.0

    let clockActive (clock: AnimationClock) : bool =
        not (FS.Skia.UI.Scene.Animation.isSettled clock.Elapsed clock.Anim)

    let advance (delta: System.TimeSpan) (clock: AnimationClock) : AnimationClock =
        // Non-positive delta is a designed no-op — never rewinds (the host never emits these). A
        // positive delta accumulates Elapsed CLAMPED to the duration, so a very-large delta settles
        // at the end (no overshoot) and the settled state is canonical (determinism of state, FR-006).
        if delta <= System.TimeSpan.Zero then
            clock
        else
            let dur = clockDuration clock.Anim
            let e = clock.Elapsed + delta
            { clock with Elapsed = (if e > dur then dur else e) }

    let updateClockForState (desired: VisualState) (carried: AnimationClock option) : AnimationClock option =
        // Compare the desired (stamped) VisualState against the carried clock's Target (contract C2).
        let triggered =
            match carried, desired with
            // At rest and staying at rest: no clock.
            | None, Normal -> None
            // Same state as the clock is already animating toward: advance-only (no retarget). A
            // settled same-state clock is KEPT (Target ≠ Normal) so a held state does not re-fire.
            | Some c, d when d = c.Target -> Some c
            // The state changed (or first entry into a non-Normal state). Mid-flight ⇒ retarget from
            // the current sampled value (no snap to start); a settled/absent clock ⇒ a fresh fade-in.
            | _ ->
                let startOpacity =
                    match carried with
                    | Some c when clockActive c -> currentOpacity c
                    | _ -> 0.0

                Some
                    { Anim = fadeAnimation startOpacity
                      Elapsed = System.TimeSpan.Zero
                      Target = desired }

        // A settled return-to-Normal clock is DROPPED so the identity returns to byte-identical
        // at-rest output (resolves the FR-003 vs FR-005 interaction); a settled non-Normal clock is
        // kept (its sampled opacity 1.0 lowers byte-identically via `applyAt`, and keeping it
        // suppresses a spurious re-fire while the state is held).
        match triggered with
        | Some c when (not (clockActive c)) && c.Target = Normal -> None
        | other -> other

    let sampleOnPaint (clock: AnimationClock) (ownScene: FS.Skia.UI.Scene.Scene list) : FS.Skia.UI.Scene.Scene list =
        // Paint-level only: wrap the identity's STATIC own paint through the feature-073 sampler at
        // the clock's current Elapsed. A node with no own paint (no box) contributes nothing.
        match ownScene with
        | [] -> []
        | _ ->
            [ { Nodes =
                  [ FS.Skia.UI.Scene.Animation.applyAt clock.Elapsed clock.Anim (FS.Skia.UI.Scene.Scene.group ownScene) ] } ]

    // FR-009: detect duplicate sibling keys present in the FIRST tree, mirroring the collision the
    // 067 `Reconcile.diff` reports from frame 1 (same shape/message), so a malformed first frame is
    // reported on frame 0 instead of a frame late. First occurrence wins; later dups are collisions.
    let private firstFrameCollisions (control: Control<'msg>) : ControlDiagnostic list =
        let diags = ResizeArray<ControlDiagnostic>()

        let rec walk (c: Control<'msg>) =
            let seen = System.Collections.Generic.HashSet<ControlId>()

            for child in c.Children do
                match child.Key with
                | Some k ->
                    if not (seen.Add k) then
                        diags.Add
                            { ControlId = Some k
                              ControlKind = c.Kind
                              Code = KeyCollision
                              Severity = ControlDiagnosticSeverity.Warning
                              Message =
                                sprintf "Duplicate key '%s' within the children of a '%s' node; first occurrence wins." k c.Kind
                              EvidencePath = None }
                | None -> ()

            for child in c.Children do
                walk child

        walk control
        List.ofSeq diags

    let init (theme: Theme) (size: FS.Skia.UI.Scene.Size) (control: Control<'msg>) : RetainedInit<'msg> =
        let layoutRoot, boundsById, layoutResult = ControlInternals.evaluateLayout size control

        let mutable nextId = 0UL

        let mint () =
            let id = RetainedId nextId
            nextId <- nextId + 1UL
            id

        let rec build (path: string) (nc: Control<'msg>) : RetainedNode<'msg> =
            let own = ControlInternals.paintNode theme boundsById path nc
            let children = nc.Children |> List.mapi (fun i child -> build (childPath path i) child)
            let subtree = own @ (children |> List.collect (fun c -> c.Fragment.SubtreeScene))

            { Identity = mint ()
              Control = nc
              Fragment =
                { OwnScene = own
                  SubtreeScene = subtree
                  Box = ControlInternals.nodeBox boundsById path nc }
              Children = children }

        let root = build "0" control

        // Paint the first frame ONCE: the Scene IS the root's pre-order SubtreeScene (the same list
        // `Control.renderTree`'s `paint "0"` builds), so this `Render` is byte-identical to a full
        // rebuild — the adapter reuses it instead of calling `Control.renderTree` a second time.
        let render: ControlRenderResult<'msg> =
            { Scene = root.Fragment.SubtreeScene |> Scene.group
              Layout = layoutRoot
              Bounds = ControlInternals.collectBoundsWith boundsById control
              Diagnostics = Control.diagnostics control
              EventBindings = ControlInternals.eventBindingsOf control
              BoundIds = ControlInternals.boundIdsOf control
              NodeCount = Control.count control }

        { Retained =
            { Root = root
              NextId = nextId
              StateByIdentity = Map.empty
              Theme = theme
              Layout = layoutResult }
          Render = render
          Diagnostics = firstFrameCollisions control }

    /// Feature 097 (R2, contract C2/C3): derive the layout-dirty set from the reconcile patch, in the
    /// `LayoutNodeId` (`Key |> defaultValue path`) domain `toLayout`/`evaluateIncremental` use. A node
    /// is self-dirty iff its `Update` sets/removes an `AttrCategory.Layout` attribute, sets/removes a
    /// geometry-driving NAME in `ControlInternals.layoutAffectingAttrNames`, OR carries a non-`Keep`
    /// child op (`ChildInsert`/`ChildRemove`/`ChildMove`); a `Replace` re-measures fresh. That name set
    /// is a SEPARATE hot-path `Set` from the names `toLayout` actually reads — not auto-derived from
    /// them (feature 101 / R7): the two are kept in lock-step by the behavioral-probe equality gate in
    /// `tests/Controls.Tests/Feature101LayoutDriftGuardTests.fs`, which fails the build the instant they
    /// drift in either direction. The `AttrCategory.Layout` channel here is honoured independently of
    /// the name set. Pure walk over (prev, patch, next) in parallel; conservative flex-line /
    /// fixed-size-ancestor propagation then happens inside `Layout.evaluateIncremental` (FR-004).
    let internal layoutDirtySet (prev: Control<'msg>) (patch: Reconcile.NodePatch<'msg>) (next: Control<'msg>) : Set<string> =
        let acc = System.Collections.Generic.HashSet<string>()

        let isLayout (c: AttrCategory) = c = AttrCategory.Layout

        let rec walk (path: string) (p: Control<'msg>) (patch: Reconcile.NodePatch<'msg>) (n: Control<'msg>) =
            let id = n.Key |> Option.defaultValue path

            match patch with
            | Reconcile.NodePatch.Keep -> ()
            | Reconcile.NodePatch.Replace _ ->
                // A Kind/Key change replaces the subtree: re-measure it fresh under its boundary.
                acc.Add id |> ignore
            | Reconcile.NodePatch.Update u ->
                let isGeometry (name: string) =
                    Set.contains name ControlInternals.layoutAffectingAttrNames

                let attrDirty =
                    u.AttrChanges
                    |> List.exists (fun ch ->
                        match ch with
                        // Geometry-driving NAME (the single source `toLayout` reads) OR a Layout-tagged
                        // category (future-proof). A content/style/state/visual-state change is neither,
                        // so it does not dirty measure (SC-004).
                        | Reconcile.AttrSet attr -> isLayout attr.Category || isGeometry attr.Name
                        | Reconcile.AttrRemoved name ->
                            isGeometry name
                            // Category recovered from the PREV node's attribute (the removed one).
                            || (p.Attributes |> List.exists (fun a -> a.Name = name && isLayout a.Category)))

                let childOpDirty =
                    u.Children
                    |> List.exists (fun op ->
                        match op with
                        | Reconcile.ChildKeep _ -> false
                        | _ -> true)

                if attrDirty || childOpDirty then
                    acc.Add id |> ignore

                // Recurse into the producing ops (every op except ChildRemove), zipped with next order.
                let producing =
                    u.Children
                    |> List.filter (fun op ->
                        match op with
                        | Reconcile.ChildRemove _ -> false
                        | _ -> true)

                List.map2 (fun op c -> op, c) producing n.Children
                |> List.iteri (fun i (op, c) ->
                    let cp = childPath path i

                    match op with
                    | Reconcile.ChildKeep(j, cpatch) -> walk cp p.Children.[j] cpatch c
                    | Reconcile.ChildMove(f, _, cpatch) -> walk cp p.Children.[f] cpatch c
                    | Reconcile.ChildInsert(_, _) -> acc.Add(n.Key |> Option.defaultValue path) |> ignore
                    | Reconcile.ChildRemove _ -> ())

        walk "0" prev patch next
        Set.ofSeq acc

    let step
        (theme: Theme)
        (size: FS.Skia.UI.Scene.Size)
        (prev: RetainedRender<'msg>)
        (next: Control<'msg>)
        : RetainedRenderStep<'msg> =
        // (1) the diff — total; never throws; duplicate keys -> KeyCollision diagnostic (C1/C4).
        let result = Reconcile.diff prev.Root.Control next

        // (2) layout of `next` via the INCREMENTAL evaluator (R2, FR-005): re-measure only the
        //     patch-derived dirty set (conservatively propagated to its flex line / fixed-size
        //     ancestor) and reuse the previous frame's cached bounds for everything else. The result
        //     `Bounds` are byte-identical to a full `evaluateLayout` (INV-1), so the reuse-driven paint
        //     walk below (`box = pr.Fragment.Box`) and the surfaced Bounds are unchanged.
        let dirty = layoutDirtySet prev.Root.Control result.Patch next
        let root, boundsById, layoutResult = ControlInternals.evaluateLayoutIncremental size next prev.Layout dirty
        // FR-006: nodes actually re-measured this frame = the honest post-propagation set.
        let remeasured = layoutResult.Invalidated |> List.length

        // FR-008: a fragment caches paint produced under a specific theme. When the per-loop theme
        // changes between frames, NO cached fragment may be reused (it would show stale-theme
        // paint); every node repaints under the new theme. Theme is uniform per frame, so one
        // top-level comparison suffices — no per-fragment theme storage.
        let themeChanged = prev.Theme <> theme

        // Mutation confined to this interpreter-edge step (constitution III): a monotonic id
        // counter and the measured work counters. The consumer `view`/`update` stay pure.
        let mutable nextId = prev.NextId
        let mutable recomputed = 0
        let mutable changedBound = 0
        // FR-007: nodes recomputed ONLY because an upstream change relaid a structurally-unchanged
        // subtree out (a shifted `Keep`) or a theme change forced a repaint — counted distinctly
        // from genuinely-changed work so `RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount`.
        let mutable shifted = 0

        let mint () =
            let id = RetainedId nextId
            nextId <- nextId + 1UL
            id

        let paintFresh (path: string) (nc: Control<'msg>) : FS.Skia.UI.Scene.Scene list =
            recomputed <- recomputed + 1
            ControlInternals.paintNode theme boundsById path nc

        // Build a brand-new subtree (Replace / ChildInsert / fallback): mint fresh ids, paint
        // every node. Used where there is no matched prev node — so no false identity is retained.
        let rec buildFresh (path: string) (nc: Control<'msg>) : RetainedNode<'msg> =
            let own = paintFresh path nc
            let children = nc.Children |> List.mapi (fun i child -> buildFresh (childPath path i) child)
            let subtree = own @ (children |> List.collect (fun c -> c.Fragment.SubtreeScene))

            { Identity = mint ()
              Control = nc
              Fragment =
                { OwnScene = own
                  SubtreeScene = subtree
                  Box = ControlInternals.nodeBox boundsById path nc }
              Children = children }

        // Recompute a structurally-identical subtree whose box SHIFTED (a `Keep` relaid out by an
        // upstream change) while CARRYING every node's prior identity — it is the same node.
        let rec carry (path: string) (pr: RetainedNode<'msg>) (nc: Control<'msg>) : RetainedNode<'msg> =
            shifted <- shifted + 1
            let own = paintFresh path nc

            let children =
                List.map2 (fun p c -> p, c) pr.Children nc.Children
                |> List.mapi (fun i (p, c) -> carry (childPath path i) p c)

            let subtree = own @ (children |> List.collect (fun c -> c.Fragment.SubtreeScene))

            { Identity = pr.Identity
              Control = nc
              Fragment =
                { OwnScene = own
                  SubtreeScene = subtree
                  Box = ControlInternals.nodeBox boundsById path nc }
              Children = children }

        // The reuse-driven walk: produce the next retained node for `nc` under `patch`, matched
        // against the prev retained node `pr`.
        let rec build (path: string) (pr: RetainedNode<'msg>) (patch: Reconcile.NodePatch<'msg>) (nc: Control<'msg>) : RetainedNode<'msg> =
            match patch with
            | Reconcile.NodePatch.Keep ->
                let box = ControlInternals.nodeBox boundsById path nc

                if box = pr.Fragment.Box && not themeChanged then
                    // unchanged AND unshifted AND same theme: reuse the cached subtree verbatim
                    // (identity-at-rest: zero re-measure/re-paint, zero id churn, same RetainedId).
                    { pr with Control = nc }
                else
                    // an upstream layout change shifted this subtree, or the theme changed (FR-008):
                    // recompute under the new theme/box, carrying identities (the node is the same).
                    carry path pr nc

            | Reconcile.NodePatch.Replace _ ->
                // Kind/Key changed -> a different node. Mint a fresh identity; the old identity
                // (and its UI state) is dropped — no false identity across a Replace (SC-001 -).
                changedBound <- changedBound + Control.count nc
                buildFresh path nc

            | Reconcile.NodePatch.Update u ->
                let box = ControlInternals.nodeBox boundsById path nc

                // This node's OWN paint is unchanged when its own data (attrs/content) did not
                // change, its leaf/container shape did not flip, and its box did not move — then
                // `paintNode` would reproduce the cached `OwnScene` exactly, so reuse it.
                let ownUnchanged =
                    List.isEmpty u.AttrChanges
                    && u.ContentChange = Reconcile.Unchanged
                    && (List.isEmpty nc.Children = List.isEmpty pr.Control.Children)
                    && box = pr.Fragment.Box
                    && not themeChanged

                let own =
                    if ownUnchanged then
                        pr.Fragment.OwnScene
                    else
                        changedBound <- changedBound + 1
                        paintFresh path nc

                // Producing ops (every op except ChildRemove) are emitted one-per-next-child in
                // next order, so they zip with `nc.Children`.
                let producing =
                    u.Children
                    |> List.filter (fun op ->
                        match op with
                        | Reconcile.ChildRemove _ -> false
                        | _ -> true)

                let children =
                    List.map2 (fun op c -> op, c) producing nc.Children
                    |> List.mapi (fun i (op, c) ->
                        let cp = childPath path i

                        match op with
                        | Reconcile.ChildKeep (j, p) -> build cp pr.Children.[j] p c
                        | Reconcile.ChildMove (f, _, p) -> build cp pr.Children.[f] p c
                        | Reconcile.ChildInsert (_, node) ->
                            changedBound <- changedBound + Control.count node
                            buildFresh cp node
                        // Unreachable (ChildRemove is filtered out of `producing`); kept total —
                        // paint the next child fresh rather than throw.
                        | Reconcile.ChildRemove _ -> buildFresh cp c)

                let subtree = own @ (children |> List.collect (fun c -> c.Fragment.SubtreeScene))

                { Identity = pr.Identity
                  Control = nc
                  Fragment =
                    { OwnScene = own
                      SubtreeScene = subtree
                      Box = box }
                  Children = children }

        let newRoot = build "0" prev.Root result.Patch next

        // Re-key UI state to the STABLE identities still live this frame AND compute this frame's
        // animation clocks (R4). Walking `newRoot` is the GC: only live identities carry state, so a
        // removed identity's clock/text is dropped with the rest of its state (FR-007, no new GC
        // code). For each live identity, the carried clock (already advanced by the host Tick wrapper)
        // is started/retargeted/dropped from the stamped `VisualState` via `updateClockForState`
        // (R1 → R4 trigger); carried text is preserved unchanged.
        let rec collect (n: RetainedNode<'msg>) (acc: Map<RetainedId, RetainedUiState>) : Map<RetainedId, RetainedUiState> =
            let carried = Map.tryFind n.Identity prev.StateByIdentity
            let carriedClock = carried |> Option.bind (fun s -> s.Animation)
            let carriedText = carried |> Option.bind (fun s -> s.Text)
            let desired = ControlInternals.visualStateOf n.Control.Attributes
            let clock = updateClockForState desired carriedClock

            let acc =
                match clock, carriedText with
                | None, None -> acc
                | _ -> Map.add n.Identity { Animation = clock; Text = carriedText } acc

            n.Children |> List.fold (fun a c -> collect c a) acc

        let stateById = collect newRoot Map.empty

        // Assemble the painted scene, overlaying any ACTIVE animation clock onto its identity's own
        // (static) paint — paint-level only, scoped to that subtree (FR-002/FR-010). When NO clock is
        // active the fast path returns the cached `SubtreeScene` verbatim, so an at-rest frame is
        // byte-identical to the pre-R4 golden and costs nothing extra (FR-005, identity-at-rest). The
        // overlay always wraps the cached STATIC `OwnScene` (fragments never store animated paint), so
        // the reuse/caching invariants are untouched and a settled/absent clock paints unchanged.
        let anyActive =
            stateById |> Map.exists (fun _ s -> s.Animation |> Option.exists clockActive)

        let sceneList =
            if not anyActive then
                newRoot.Fragment.SubtreeScene
            else
                let rec assemble (n: RetainedNode<'msg>) : Scene list =
                    let ownStatic = n.Fragment.OwnScene

                    let own =
                        match Map.tryFind n.Identity stateById |> Option.bind (fun s -> s.Animation) with
                        | Some c when clockActive c -> sampleOnPaint c ownStatic
                        | _ -> ownStatic

                    own @ (n.Children |> List.collect assemble)

                assemble newRoot

        // Byte-identical to `Control.renderTree theme size next` AT REST: `SubtreeScene` is the
        // pre-order concatenation of `paintNode` over every node — the same list `renderTree`'s paint
        // builds. An active clock contributes a paint-level overlay scoped to its own identity.
        let render: ControlRenderResult<'msg> =
            { Scene = sceneList |> Scene.group
              Layout = root
              Bounds = ControlInternals.collectBoundsWith boundsById next
              Diagnostics = Control.diagnostics next
              EventBindings = ControlInternals.eventBindingsOf next
              BoundIds = ControlInternals.boundIdsOf next
              NodeCount = Control.count next }

        { Retained =
            { Root = newRoot
              NextId = nextId
              StateByIdentity = stateById
              Theme = theme
              Layout = layoutResult }
          Render = render
          Diagnostics = result.Diagnostics
          WorkReduction =
            { BaselineNodeCount = Control.count next
              RecomputedNodeCount = recomputed
              ChangedSubtreeBound = changedBound
              ShiftedNodeCount = shifted
              RemeasuredNodeCount = remeasured } }

    let retainedHitTest (x: float) (y: float) (retained: RetainedRender<'msg>) : RetainedId option =
        // The deepest node whose cached box contains the point. Each node — including unkeyed
        // same-kind siblings — carries a distinct identity and its own box, so this resolves to a
        // per-node identity with no collision (the defect the `ControlId` hitTest path has).
        let contains (box: Rect option) =
            match box with
            | Some(b: Rect) -> x >= b.X && x <= b.X + b.Width && y >= b.Y && y <= b.Y + b.Height
            | None -> false

        let rec go (n: RetainedNode<'msg>) : RetainedId option =
            // children first (deepest-wins); fall back to self when the point is in this node's own
            // area but in a gap between its children.
            match n.Children |> List.tryPick go with
            | Some _ as hit -> hit
            | None -> if contains n.Fragment.Box then Some n.Identity else None

        go retained.Root

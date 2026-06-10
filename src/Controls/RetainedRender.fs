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

type internal RetainedUiState =
    { Animation: FS.Skia.UI.Scene.AnimationState<FS.Skia.UI.Scene.Transform> option
      Text: TextInputModel option }

type internal RetainedRender<'msg> =
    { Root: RetainedNode<'msg>
      NextId: uint64
      StateByIdentity: Map<RetainedId, RetainedUiState>
      Theme: Theme }

type internal WorkReductionRecord =
    { BaselineNodeCount: int
      RecomputedNodeCount: int
      ChangedSubtreeBound: int
      ShiftedNodeCount: int }

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
        let layoutRoot, boundsById = ControlInternals.evaluateLayout size control

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
              NodeCount = Control.count control }

        { Retained =
            { Root = root
              NextId = nextId
              StateByIdentity = Map.empty
              Theme = theme }
          Render = render
          Diagnostics = firstFrameCollisions control }

    let step
        (theme: Theme)
        (size: FS.Skia.UI.Scene.Size)
        (prev: RetainedRender<'msg>)
        (next: Control<'msg>)
        : RetainedRenderStep<'msg> =
        // (1) the diff — total; never throws; duplicate keys -> KeyCollision diagnostic (C1/C4).
        let result = Reconcile.diff prev.Root.Control next

        // (2) the global layout of `next` (required for byte-identity: a node's box depends on the
        //     whole tree). Reused for both the Scene assembly and the surfaced Bounds.
        let root, boundsById = ControlInternals.evaluateLayout size next

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

        // Re-key UI state to the STABLE identities still live this frame: carried identities keep
        // their state (focus/animation/text survive a positional shift); dropped identities lose it.
        let rec liveIds (n: RetainedNode<'msg>) : RetainedId seq =
            seq {
                yield n.Identity
                for c in n.Children do
                    yield! liveIds c
            }

        let live = liveIds newRoot |> Set.ofSeq
        let stateById = prev.StateByIdentity |> Map.filter (fun id _ -> Set.contains id live)

        // Byte-identical to `Control.renderTree theme size next`: SubtreeScene is the pre-order
        // concatenation of `paintNode` over every node — the same list `renderTree`'s paint builds.
        let render: ControlRenderResult<'msg> =
            { Scene = newRoot.Fragment.SubtreeScene |> Scene.group
              Layout = root
              Bounds = ControlInternals.collectBoundsWith boundsById next
              Diagnostics = Control.diagnostics next
              EventBindings = ControlInternals.eventBindingsOf next
              NodeCount = Control.count next }

        { Retained =
            { Root = newRoot
              NextId = nextId
              StateByIdentity = stateById
              Theme = theme }
          Render = render
          Diagnostics = result.Diagnostics
          WorkReduction =
            { BaselineNodeCount = Control.count next
              RecomputedNodeCount = recomputed
              ChangedSubtreeBound = changedBound
              ShiftedNodeCount = shifted } }

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

namespace FS.Skia.UI.Controls

/// Feature 091 (E2) — the retained render structure that wires the parked keyed reconciler
/// (`module internal Reconcile`, feature 067) onto the live render path. Each frame holds the
/// previous lowered `Control<'msg>` tree paired with its cached render fragments and a stable,
/// diff-conferred identity per node; the next frame is produced by `Reconcile.diff`-ing against
/// it and reusing the unchanged subtrees' cached fragments.
///
/// This whole surface is `internal` — assembly-internal accessibility, genuinely unreachable
/// from package consumers (mirrors `module internal Reconcile` / `module internal SceneRenderer`;
/// zero public-surface baseline delta, 067 SC-005). The Expecto/FsCheck property tests reach it
/// via `[<assembly: InternalsVisibleTo("Controls.Tests")>]`. It is a contract between framework
/// internals and the property tests, NOT a consumer API: it exposes no mutable view-model, no
/// data binding, and no dependency/attached property (permanent roadmap non-goals).

/// The stable identity the diff confers on a matched node. Monotonic within a host loop; NOT the
/// path-derived `ControlId` (which is unstable across a positional shift — the very reason
/// focus/text state resets today). Per-control state (focus, animation clock, text model) re-keys
/// to this so it survives an unrelated re-render. Minted deterministically from a per-host
/// counter (no clock/randomness), so identical frame sequences mint identical ids (SC-005).
type internal RetainedId = RetainedId of uint64

/// The cached, reusable unit of measure + paint for one retained node. `OwnScene` is the node's
/// own painted contribution (`Control.renderTree`'s per-node `here`); `SubtreeScene` is the
/// pre-order painted scene of the node AND its descendants (reused verbatim when the whole subtree
/// is unchanged AND unshifted); `Box` is the node's evaluated absolute box (the reuse key).
type internal RenderFragment =
    { OwnScene: FS.Skia.UI.Scene.Scene list
      SubtreeScene: FS.Skia.UI.Scene.Scene list
      Box: FS.Skia.UI.Scene.Rect option }

/// One retained control node: its stable identity, the lowered control it was built from, its
/// cached render fragment, and its retained children (mirroring `Control.Children` order).
type internal RetainedNode<'msg> =
    { Identity: RetainedId
      Control: Control<'msg>
      Fragment: RenderFragment
      Children: RetainedNode<'msg> list }

/// Feature 099 (R4) — the per-identity animation clock. Generalizes the feature-091 carried slot
/// (which was `AnimationState<Transform> option`, transform-only and never written by the host) to
/// the feature-073 multi-channel paint carrier so a focus-ring fade (opacity) / press tint (color)
/// can be expressed, not just a transform. `Anim` is the reused feature-073 `Animation` shape
/// (opacity/transform/color tweens, sampled by `Animation.applyAt`); `Elapsed` is the accumulated
/// INJECTED delta (the sole time coordinate — no wall-clock); `Target` is the `VisualState` this
/// clock is animating toward (used to detect a retarget when the stamped state flips). `None` on the
/// slot ⇒ the identity is at rest and paints byte-identically to the pre-R4 static render (FR-005).
type internal AnimationClock =
    { Anim: FS.Skia.UI.Scene.Animation
      Elapsed: System.TimeSpan
      Target: VisualState }

/// Per-control UI state keyed by the STABLE `RetainedId` rather than the path-derived `ControlId`,
/// so it survives a positional shift (FR-003). `Animation` is the per-control clock proving
/// FR-003 survival; under feature 099 (R4) it is the live `AnimationClock` advanced by the host
/// tick and sampled on paint (091 only carried it; nothing wrote it). `Text` is re-keyed text-input
/// state. Focus itself stays in the consumer model's `ControlRuntime.FocusedControl`; 091 only
/// remaps the lookup to `RetainedId`.
type internal RetainedUiState =
    { Animation: AnimationClock option
      Text: TextInputModel option }

/// The per-frame retained root plus the monotonic identity counter, the identity-keyed UI
/// state map, and the theme this structure was painted under. Lives in the host loop's existing
/// mutable-ref state (the interpreter edge). 092: `Theme` is the fragment-reuse key — a theme
/// change between `step` calls invalidates all cached fragments so they repaint (FR-008), and
/// the live host now READS/WRITES `StateByIdentity` (091 only carried it; the host ignored it).
type internal RetainedRender<'msg> =
    { Root: RetainedNode<'msg>
      NextId: uint64
      StateByIdentity: Map<RetainedId, RetainedUiState>
      Theme: Theme
      /// Feature 097 (R2): the previous frame's full `LayoutResult` — the per-frame measure/bounds
      /// cache (FR-002). `step` threads it into `Layout.evaluateIncremental` so an unchanged subtree's
      /// bounds survive across frames and are reused without re-measuring. Seeded by `init` with a full
      /// `evaluate`; advanced each `step` to the incremental result.
      Layout: FS.Skia.UI.Layout.LayoutResult }

/// Measured per-frame work reduction (SC-003). `BaselineNodeCount` is what a full rebuild
/// re-measures/re-paints (== N); `RecomputedNodeCount` is what the wired path actually
/// recomputed; `ChangedSubtreeBound` is the genuinely-changed work (Replace/own-change/insert);
/// `ShiftedNodeCount` (092) is work recomputed ONLY because an upstream change relaid a
/// structurally-unchanged subtree out (a `Keep` whose box moved, or a theme repaint). For any
/// localized change:
///   `RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount`
///   `RecomputedNodeCount < BaselineNodeCount`
/// (091 documented `RecomputedNodeCount ≤ ChangedSubtreeBound`, which a sibling-shifting change
/// violates — the shifted work was recomputed but uncounted; FR-007 splits it out.)
type internal WorkReductionRecord =
    { BaselineNodeCount: int
      RecomputedNodeCount: int
      ChangedSubtreeBound: int
      ShiftedNodeCount: int
      /// Feature 097 (R2, FR-006): nodes actually RE-MEASURED this frame (the post-propagation dirty
      /// set `Layout.evaluateIncremental` reports in `Invalidated`). For a localized update this is
      /// strictly below `BaselineNodeCount`; for a genuine whole-tree relayout it equals it; for an
      /// empty patch it is 0. Measures partial MEASURE work, distinct from partial PAINT above.
      RemeasuredNodeCount: int }

/// The result of one wired frame: the next retained structure, the render result (byte-identical
/// to a full rebuild of `next`), the diagnostics surfaced from the diff (e.g. `KeyCollision`), and
/// the measured work reduction.
type internal RetainedRenderStep<'msg> =
    { Retained: RetainedRender<'msg>
      Render: ControlRenderResult<'msg>
      Diagnostics: ControlDiagnostic list
      WorkReduction: WorkReductionRecord }

/// The first-frame result (092, FR-009): the seeded retained structure, the render result it
/// painted (so the adapter paints the first frame ONCE instead of also calling
/// `Control.renderTree`), and any first-frame diagnostics (e.g. a duplicate-key `KeyCollision`
/// present in the very first tree — 091 only diffed from frame 1, so it surfaced a frame late).
type internal RetainedInit<'msg> =
    { Retained: RetainedRender<'msg>
      Render: ControlRenderResult<'msg>
      Diagnostics: ControlDiagnostic list }

module internal RetainedRender =

    /// Feature 099 (R4): the single pinned framework default transition — exactly 150 ms, `EaseOut`,
    /// on the opacity channel — applied when a tween is started/retargeted. A fixed constant (not a
    /// per-control consumer knob) so the determinism goldens reach the settled end after the same
    /// fixed frame count for the same injected-delta sequence. Reached by the test assemblies.
    val internal defaultTransitionDuration: System.TimeSpan

    /// Feature 099 (R4): advance a clock by an INJECTED delta. Total + pure (no wall-clock): a
    /// non-positive delta is a no-op (never rewinds); a positive delta accumulates `Elapsed`,
    /// CLAMPED to the animation's duration (so a very-large delta settles at the end with no
    /// overshoot, and replaying an identical delta sequence reproduces identical state — FR-006).
    val internal advance: delta: System.TimeSpan -> clock: AnimationClock -> AnimationClock

    /// Feature 099 (R4): true while the clock is still in flight (not every present tween has
    /// reached its `Duration`). A settled clock is NOT sampled — it paints byte-identically to the
    /// static render (FR-005), so only active clocks contribute a per-frame change.
    val internal clockActive: clock: AnimationClock -> bool

    /// Feature 099 (R4): the pure transition trigger (contract C2). Given the `desired` VisualState
    /// stamped by `ControlRuntime.applyRuntimeVisualState` (R1) and the carried (already-advanced)
    /// clock, decide the frame's clock: START a fade-in for a fresh state change (from a settled/no
    /// clock), RETARGET from the current sampled value for a mid-flight change (no snap to start),
    /// advance-only when the state is unchanged, and DROP a settled return-to-`Normal` clock so the
    /// identity is byte-identical at rest (FR-003/FR-005).
    val internal updateClockForState: desired: VisualState -> carried: AnimationClock option -> AnimationClock option

    /// Feature 099 (R4): sample an ACTIVE clock onto an identity's own painted scene (paint-level
    /// only — opacity/transform/color, never layout), reusing feature-073 `Animation.applyAt`. The
    /// static `ownScene` is the cached fragment paint; the result wraps it at the clock's current
    /// `Elapsed`. Used only for active clocks — a settled/absent clock paints `ownScene` unchanged.
    val internal sampleOnPaint: clock: AnimationClock -> ownScene: FS.Skia.UI.Scene.Scene list -> FS.Skia.UI.Scene.Scene list

    /// Build the initial retained structure from the first frame's lowered tree, painting it
    /// ONCE. The returned `Render` is byte-identical to `Control.renderTree theme size control`
    /// (so the adapter reuses it rather than re-painting), and `Diagnostics` carries any
    /// first-frame duplicate-key `KeyCollision` (FR-009). Total; never throws.
    val init: theme: Theme -> size: FS.Skia.UI.Scene.Size -> control: Control<'msg> -> RetainedInit<'msg>

    /// Produce the next frame from the retained `prev` and the next lowered tree, by
    /// `Reconcile.diff`-ing and reusing/recomputing fragments under the patch.
    ///
    /// Guarantees (asserted by the promoted 067 suite on the WIRED path):
    ///   - totality:         never throws for any (prev, next); duplicate keys -> KeyCollision diagnostic
    ///   - determinism:      identical (prev, next) -> identical Render + identical minted RetainedIds
    ///   - identity-at-rest: next structurally equal to prev.Root.Control -> Keep no-op, no re-measure
    ///   - round-trip:       Render is byte-identical to `Control.renderTree theme size next`
    val step:
        theme: Theme ->
        size: FS.Skia.UI.Scene.Size ->
        prev: RetainedRender<'msg> ->
        next: Control<'msg> ->
            RetainedRenderStep<'msg>

    /// Resolve a point to the stable identity of the control under it (092, FR-004): the deepest
    /// retained node whose cached `Fragment.Box` contains `(x, y)`, else `None` (a true gap /
    /// outside the root). Because every node — INCLUDING unkeyed same-kind siblings — carries a
    /// distinct `RetainedId` and its own evaluated box, this returns a per-node identity with no
    /// collision, unlike the `ControlId` `hitTest`/`nearestAuthored` path (which collapses unkeyed
    /// same-kind siblings). Focus-on-click resolves through this. Reuses the boxes already computed
    /// by `init`/`step`; total and deterministic.
    val retainedHitTest: x: float -> y: float -> retained: RetainedRender<'msg> -> RetainedId option

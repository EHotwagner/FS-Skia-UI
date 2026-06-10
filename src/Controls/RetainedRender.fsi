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

/// Per-control UI state keyed by the STABLE `RetainedId` rather than the path-derived `ControlId`,
/// so it survives a positional shift (FR-003). `Animation` is the per-control clock proving
/// FR-003 survival (Scene `Animation`, feature 073); `Text` is re-keyed text-input state. Focus
/// itself stays in the consumer model's `ControlRuntime.FocusedControl`; 091 only remaps the
/// lookup to `RetainedId`.
type internal RetainedUiState =
    { Animation: FS.Skia.UI.Scene.AnimationState<FS.Skia.UI.Scene.Transform> option
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
      Theme: Theme }

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
      ShiftedNodeCount: int }

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

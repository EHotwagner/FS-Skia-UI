# Phase 0 Research: Mouse Input & Pointer Events

All NEEDS CLARIFICATION items from Technical Context are resolved below. Each
decision records what was chosen, why, and the alternatives weighed against the
existing codebase.

## R1. Where does the coordination layer live? (package placement)

**Decision**: A new pure module `FS.Skia.UI.Controls.Pointer` (files
`src/Controls/Pointer.fsi` + `Pointer.fs`) is the front door, layered over the
existing `ControlRuntime` and consuming `Layout.hitTestComputed`. The MVU bridge
is a new `interpretPointerEffect` in `FS.Skia.UI.Controls.Elmish`. The host
contract extension lives in `FS.Skia.UI.SkiaViewer`.

**Rationale**: This matches the spec's Framework Governance Prompts exactly ("New
public surface … primarily in `FS.Skia.UI.Controls` … and its MVU adapter
`FS.Skia.UI.Controls.Elmish`"). `Controls` already references `Layout` (so
`hitTestComputed` is reachable) and already hosts `ControlRuntime` (hover/press/
drag/focus state). No new package identity is added; the existing acyclic project
graph is preserved.

**Alternatives considered**:
- *Host-coupled helper in a package that sees both `ViewerEvent` and hit-testing*
  (parity with `FS.Skia.UI.Input.updateFromViewerEvent`). Rejected: `Controls`
  does not (and should not) depend on `SkiaViewer`, and `FS.Skia.UI.Input` does
  not depend on `Controls`/`Layout`. Bridging them would require a new dependency
  edge (e.g. `Controls.Elmish → SkiaViewer`) or a brand-new package — the spec
  says no package identities are added, and pulling the host into the MVU adapter
  expands its footprint for marginal convenience.
- *Extend `FS.Skia.UI.Input` (the host-coupled YAML keyboard package)*. Rejected:
  that package's model (`InputRuntime`, YAML bindings, bigram analysis) is about
  keyboard command resolution, not control-addressed pointer hit-testing; the
  spec scopes pointer surface to `Controls`/`Controls.Elmish`.

## R2. How is a pointer coordinate mapped to a control? (LayoutNodeId ↔ ControlId)

**Decision**: Reuse the existing convention — no new mapping table.
`Layout.hitTestComputed policy result x y : LayoutNodeId option` returns the
front-most visible node id, and `LayoutNodeId = ControlId = string` by
construction: `Control.layoutNode` derives the node id from
`control.Key |> Option.defaultValue control.Kind`, the same identity used in
`ControlEventBinding.ControlId`. The hit-test result is therefore directly
usable as a `ControlId`.

**Rationale**: Both are `string` aliases (`src/Layout/Types.fsi:13`,
`src/Controls/Types.fsi:7`) and the lowering already establishes the equivalence.
A separate node→control dictionary would be redundant state to keep in sync.

**Caveat / follow-up**: the `Pointer.update` reducer takes the current
`LayoutResult` as an explicit argument and does not assume it is fresh; FR-010
stale-target handling covers the case where a hit id no longer corresponds to a
live control (the consumer rebuilt the tree). Keys must be unique within a view
for unambiguous addressing — documented in the quickstart as a consumer
obligation (duplicate keys already risk layout-id collisions today).

## R3. Hover enter/leave transition semantics (FR-003)

**Decision**: `Pointer.update` computes ordered transitions from the *prior*
hover target and the *new* hit-test result: on a `Move`, if the hit id differs
from `PointerState.Hover`, emit `HoverLeave(prior)` (when there was one) **then**
`HoverEnter(next)` (when there is one), and update the stored hover. A move whose
hit id equals the stored hover emits nothing (no redundant transitions). Moving
to empty space emits `HoverLeave(prior)` only; leaving the window
(`WindowExited`) does the same.

**Rationale**: `ControlRuntimeModel` stores only the *current* `HoveredControl`
and `HoverChanged` carries a single `ControlId option`, so enter/leave ordering
must be derived by the coordination layer rather than read off the runtime. Doing
it in the pure reducer makes ordering directly assertable (SC-001) and keeps the
runtime unchanged for this concern. `Pointer.update` still dispatches
`ControlRuntimeMsg.HoverControl next` so the runtime's `HoveredControl` (used for
visual affordances) stays consistent.

**Alternative**: add explicit `HoverEnter`/`HoverLeave` effects to
`ControlRuntime`. Rejected as the heavier option — it would move the `Controls`
runtime baseline for state the coordination layer can compute, and enter/leave is
fundamentally a *transition* concern, not a *resting-state* concern.

## R4. Click vs drag resolution and the threshold (FR-005/FR-006)

**Decision**: A press records a per-button `PressCandidate { Control; StartX;
StartY }`. On each held `Move`, if the Manhattan/Euclidean distance from the
candidate's start exceeds the configured `DragThreshold` (a `float` in
`PointerState`, default chosen in data-model), the sequence commits to a **drag**
(emit `DragBegin` once, then `DragMove` per subsequent move) and the candidate is
flagged dragging so release emits `DragEnd`, **not** a click. If movement stays at
or below the threshold, release over the same control emits `Click`; release over
a different control or empty space emits no click and clears the candidate. A
sequence yields a click XOR a drag, never both (resolving the spec's noted
FR-005/FR-006 conflict). The drag path drives
`ControlRuntimeMsg.StartDrag/MoveDrag/EndDrag`.

**Rationale**: Mirrors `ControlDrag { StartX; StartY; CurrentX; CurrentY }`
already in `ControlRuntime`; the threshold is the single tunable separating the
two interactions, kept in the model so it is testable and overridable.

**Alternative**: time-based click/drag disambiguation. Rejected — the spec
defines the separator as a movement threshold, and a distance test is
deterministic without a clock (supports replay, SC-005).

## R5. Per-button tracking and secondary/middle buttons (FR-013)

**Decision**: Introduce `PointerButton = Primary | Secondary | Middle`.
`PointerState` holds press candidates as `Map<PointerButton, PressCandidate>` so
overlapping presses (US4 scenario 2: primary held while secondary clicks) are
tracked and resolved independently with zero cross-button misattribution
(SC-008). `Click` and the drag effects carry the originating `PointerButton`; a
secondary press+release over a control surfaces as `Click` with `Secondary`
(consumers drive context actions; the framework does **not** render a menu).
`ControlRuntime`'s primary press/drag remain the source of visual affordances; the
coordination layer owns the per-button bookkeeping.

**Rationale**: `ControlRuntimeModel.PressedControls : Set<ControlId>` has no button
dimension, so per-button state belongs in the new `PointerState`. The host already
receives `MouseButton` at the press/release handler (`Vulkan.fs`, currently
discarded as `_`), so the identity is available with no new dependency.

**Follow-up for task generation**: decide whether `ControlRuntime` press effects
gain a button field or whether the coordination layer alone carries button
identity (leaning to the latter to minimise runtime baseline churn).

## R6. Host contract extension — button + wheel + window-exit (FR-013/FR-014/FR-007)

**Decision**: Extend `ViewerEvent` (`src/SkiaViewer/Host/Diagnostics.fsi`):
- add a `button` field to `PointerPressed`/`PointerReleased`
  (`PointerPressed of x: float * y: float * button: ViewerPointerButton`),
- add `PointerScrolled of x: float * y: float * deltaX: float * deltaY: float`
  for the wheel (FR-014),
- add a window-exit signal (`PointerExited` or reuse a focus-lost/blur path) so
  the cancel rule (FR-007) has a deterministic host trigger.

In `Vulkan.fs`: bind the `MouseButton` parameter (replace the `_` discard) and map
Silk.NET `MouseButton` → `ViewerPointerButton`; register a new `IMouse.Scroll`
handler (`mouse.add_Scroll`, `Action<IMouse, ScrollWheel>`) and dispose it
symmetrically; wire a mouse-leave/window-blur to the exit signal.

**Rationale**: Silk.NET already delivers `MouseButton` to the down/up handlers and
exposes `IMouse.Scroll`; only the wiring is missing. Changing the
`PointerPressed`/`PointerReleased` case arity is source-breaking in principle, but
the sole existing matcher returns `None` (`SkiaViewer.fs`), so blast radius is
contained — documented as a compatibility note in the contract.

**Verification item for implementation**: confirm Silk.NET's `ScrollWheel` axis
sign convention and the exact mouse-leave event name on the target Silk.NET
version at code time (kept out of the contract — only the framework's signed
delta per axis is contractual, FR-014).

## R7. Determinism and recorded-event replay (FR-009/SC-005)

**Decision**: `Pointer.update` is a pure `(PointerMsg, LayoutResult,
PointerState) -> PointerState * PointerInteraction list`, and a `Pointer.replay`
folds an initial state + a `PointerMsg list` (against a supplied `LayoutResult`)
to a final state + accumulated effects — directly mirroring
`Keyboard`/`InputKeyboard.replay`. No clock, RNG, or hidden mutable state; the
same recorded sequence replays to identical outcomes.

**Rationale**: The keyboard system already proves this pattern
(`KeyboardInput.replay`, `Keyboard.update`). Re-using it gives reproducible
evidence capture for free and satisfies the project's evidence/readiness model.

## R8. Opt-out / keyboard-only parity (FR-012/SC-006)

**Decision**: Pointer support is purely additive and consumer-initiated — an
application that never constructs a `PointerState`, never translates
`ViewerEvent.Pointer*`, and never calls `interpretPointerEffect` behaves exactly
as before. The host already returns `None` for pointer events today; the new
host case fields are inert unless the consumer maps them. SC-006 is verified by
re-running an existing keyboard-only sample unchanged.

**Rationale**: No global pump or implicit subscription is introduced; the
front door is a value the consumer chooses to thread through its `update`,
identical to how keyboard is opt-in via `interpretKeyboardEffect`.

## R9. Pointer origin distinction (FR-011)

**Decision**: `PointerInteraction` is a distinct effect type from
`ControlRuntimeEffect`/`KeyboardEffect`, and click carries `PointerButton` +
position + `Origin = Pointer`, so a consumer can always tell a mouse click from a
keyboard activation (which arrives as `CommandResolved`/`FocusChanged`). The MVU
bridge keeps them in separate `interpret*` functions.

**Rationale**: Type-level separation is the simplest honest discriminator and
matches the existing split between `interpretKeyboardEffect` and
`interpretControlEffect`.

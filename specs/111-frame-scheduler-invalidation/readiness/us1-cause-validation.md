# US1 / US2 independent validation path (feature 111, T010 / T013)

## US1 — FrameCause (T010)

Run a mixed `FrameInput` script through `ControlsElmish.Perf.runScript` and assert each produced frame's
`FrameCause`:

- `FrameInput.Idle` -> `FrameCause.Idle`
- a coalesced hover/drag move burst -> ONE `FrameCause.PointerMove` frame
- a discrete `PressedDown`/`Click`/`ReleasedUp`/`Scroll` -> `FrameCause.PointerDiscrete`
- `FrameInput.Key` -> `FrameCause.Key`
- `FrameInput.Tick` over a live animation clock -> `FrameCause.Tick`

`Resize`/`Theme` are live-scheduler causes (a window resize / theme switch between paints); the
deterministic corpus produces none. The cause names the trigger, not the effect — a key that changes the
model is `FrameCause.Key` with `ProductModelChanged = true`. Evidence: `Feature111FrameCauseTests`
(byte-stable cause sequence across repeated runs).

## US2 — phase record (T013)

The phase record is FOUR booleans: `ViewCalled` (the VIEW phase), `DiffRan`, `LayoutRan`, `PaintRan`.

- `ViewCalled` = `host.View size model` ran (the existing field reused as the view phase).
- `DiffRan` = a newly-produced view tree was reconciled against the retained tree (the retained step ran
  on a fresh `host.View`); an animation-only overlay re-sample does NOT count.
- `LayoutRan` = >=1 node re-measured this frame (set explicitly, consistent with `RemeasuredNodeCount > 0`).
- `PaintRan` = the painted scene (a model render) or the animation overlay was (re)assembled.

**Hit-test is intentionally NOT a phase field** (clarified 2026-06-12): the deterministic
`Perf.runScript` path does not hit-test coalesced hover/drag moves (only discrete clicks hit-test,
feature 110), so a hit-test bool would read `false` across the move-burst corpus — a misleading
always-false field. Hit-test work stays covered by `PointerSamplesReceived` / `PointerMovesProcessed` /
`FullRenderFallbackCount`. Evidence: `Feature111PhaseRecordTests`.

## US3 — view-skip + frame-rate work (recorded here for completeness)

A frame whose cause did not change the product model skips `host.View` (reuses the already-produced view
tree): an animation-only tick and a pointer-move frame are view-free (`ViewCalled = false`,
`FullRenderCount = 0`) while rendered output is byte-identical; continuous drag + continuous animation are
frame-rate work (`PointerMovesProcessed <= 1`, zero per-sample `host.View`). Evidence:
`Feature111ViewSkipTests` + [view-free-delta.md](./view-free-delta.md).

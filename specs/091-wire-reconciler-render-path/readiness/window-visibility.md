# Window visibility — feature 091

status=deferred
mode=render-only-offscreen
window-visible=deferred
accessible-window=deferred
first-frame-presented=deferred
self-closed-for-evidence=false

Feature 091 is **internal render-path wiring** (the parked 067 keyed reconciler is wired onto
the live render path via `module internal RetainedRender`). It ships **no new interactive
surface** and changes only how each frame is produced internally — the consumer
`view`/`update`/`Init`/`Subscriptions` contract is unchanged (FR-008). Per the plan and
[[fs-skia-evidence-mode]], **all** correctness evidence is capturable **headless/offscreen** and
**no live Vulkan window is required**: golden-diff parity is a pure structural equality of the
produced `ControlRenderResult.Scene` (wired == full rebuild, zero diff); the focus/animation
survives-proof is the pure carry of the stable `RetainedId` through `RetainedRender.StateByIdentity`;
the work-reduction metric is a node count. A live persistent-window launch is therefore **deferred**
(render-only honesty) — not claimed and not required. See `interactive-visible-window.md` and
`real-image-evidence.md`. No taskbar-only / process-only substitution is claimed.

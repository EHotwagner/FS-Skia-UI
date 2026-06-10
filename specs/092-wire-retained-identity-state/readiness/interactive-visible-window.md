# Interactive Visible Window Evidence (092)

status=deferred
mode=render-only-offscreen
window-visible=deferred
accessible-window=deferred
first-frame-presented=deferred
self-closed-for-evidence=false

## Render-only / offscreen posture (no live window required)

Feature 092 wires the retained **identity** (091's `RetainedRender.StateByIdentity`, keyed by the
stable `RetainedId`) into the live interactive state of `ControlsElmish.runInteractiveApp`'s closure
(focus, in-progress text, the per-control clock), and folds the 090/091 render-path defects. It adds
**no** new interactive surface — the consumer `Init`/`Update`/`View`/`MapPointer`/`Tick`/
`Diagnostics` contract is unchanged; the only signature move is `InteractiveViewerHost.MapKey :
'msg list` (FR-006) — so the interactive-UI run-and-use gate does **not** apply to a new surface.

Per the plan and [[fs-skia-evidence-mode]], every success criterion is proven **render-only /
offscreen** with **no live Vulkan window**:

- SC-001 live survival — focus + draft text (+ the carried clock) survive a positional shift,
  driven through the REAL `resolveFocus`/`routeFocusedText`/`RetainedRender.step` seam (no
  hand-seeded state); a rebuild-every-frame baseline fails the same proof.
- SC-002 focus resolution — keyed/unkeyed/wrapped fields resolve to distinct `RetainedId`s; a
  pre-filled multi-line first keystroke appends.
- SC-003/SC-004/SC-006 — node-count work reduction, multi-frame structural parity, theme-reuse
  byte-identity — pure value comparisons.

A live persistent-window launch is **deferred** (not performed — internal-state wiring, no new
interactive surface) and is **not required**. The environment HAS a GPU and a live Vulkan/Skia
window can open (via a compiled exe on the X11 path); the deferral is by scope, not a hardware
limitation. No taskbar-only / process-only success is claimed.

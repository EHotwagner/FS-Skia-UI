# Window visibility — feature 092

status=deferred
mode=render-only-offscreen
window-visible=deferred
accessible-window=deferred
first-frame-presented=deferred
self-closed-for-evidence=false

Feature 092 wires the retained **identity** computed by 091 into the live interactive state
(focus, in-progress text, the per-control animation clock) and folds the 090/091 render-path
defects. It ships **no new interactive surface**: the consumer
`Init`/`Update`/`View`/`MapPointer`/`Tick`/`Diagnostics` contract is unchanged; the only
signature move is `InteractiveViewerHost.MapKey : 'msg list` (FR-006) and the re-keyed internal
focus seam. Per the plan and [[fs-skia-evidence-mode]], **all** correctness evidence is capturable
**headless/offscreen** and **no live Vulkan window is required**: the live-survival proof drives the
real `resolveFocus`/`routeFocusedText`/`RetainedRender.step` seam and checks the carried
`RetainedId`-keyed state (focus + draft text continued, clock advanced not reset); the theme-reuse
and multi-frame proofs are pure structural `Scene` equality; the work-reduction proof is a node
count. A live persistent-window launch is therefore **deferred** (render-only honesty) — not claimed
and not required. See `real-image-evidence.md` and `visual-evidence-honesty.md`. No taskbar-only /
process-only substitution is claimed.

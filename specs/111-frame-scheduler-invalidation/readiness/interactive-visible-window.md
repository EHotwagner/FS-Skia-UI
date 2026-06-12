# Interactive visible window — applicability (feature 111, T002)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=not-applicable

Feature 111 ships no NEW persistent/graphical entry point — it is a per-frame scheduling/observability
change proven through deterministic, headless evidence. Its user-facing surface is the public
`FrameMetrics`/`FrameCause` observability contract proven through `ControlsElmish.Perf.runScript` and the
regenerated corpus goldens, plus the internal `RetainedRender` step exercised via InternalsVisibleTo. A
live Vulkan window is NOT required; the existing `runInteractiveApp` window launch contract is unchanged
(its signature is untouched; the scheduler skips a redundant `host.View` on model-unchanged frames with
at-rest output byte-identical, FR-008/SC-007).

# Interactive visible window — applicability (feature 109, T001/T028)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=not-applicable

Feature 109 ships no NEW persistent/graphical entry point — it is observation-and-evidence only. Its
user-facing surface is the public `FrameMetrics` observability contract proven through the
deterministic, headless `ControlsElmish.Perf.runScript` driver (per-frame counts + booleans) and the
committed corpus goldens. A live Vulkan window is NOT required; the existing `runInteractiveApp` window
is unchanged in its launch contract (the `FrameMetrics` field change touches only the observability
surface, default `OnFrameMetrics = ignore` host path stays byte-identical, FR-020/SC-008).

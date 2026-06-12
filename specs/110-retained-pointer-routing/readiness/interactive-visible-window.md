# Interactive visible window — applicability (feature 110, T002)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=not-applicable

Feature 110 ships no NEW persistent/graphical entry point — it is a hot-path routing MECHANISM change
proven through deterministic, headless evidence. Its user-facing surface is the public `FrameMetrics`
observability contract (the new `FullRenderFallbackCount` field + narrowed routing counts) proven through
the deterministic `ControlsElmish.Perf.runScript` driver and the regenerated corpus goldens, plus the
internal retained-route seams compared against the preserved oracle. A live Vulkan window is NOT
required; the existing `runInteractiveApp` window launch contract is unchanged (its signature is
untouched; the route it wires changes from the full-render oracle to the retained route, with at-rest
output byte-identical, FR-011/SC-008).

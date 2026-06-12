# Interactive visible window — applicability (feature 108, T002/T003/T040)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=not-applicable

Feature 108 ships no NEW persistent/graphical entry point. Its user-facing surfaces are proven through
the deterministic, headless `ControlsElmish.Perf.runScript` driver (per-frame `FrameMetrics`), the
structural-Scene focus-ring diff over the real `Control.renderTree`, and the interactive responds-proof
(`captureRespondsProof` / `routeInteractivePointer`) — the documented evidence path (spec Assumptions /
[runtime-limitations.md](./runtime-limitations.md)). A live Vulkan window is **not required**; the
existing `runInteractiveApp` window is unchanged in its launch contract (the two additive host fields
carry inert defaults, so at-rest behaviour is byte-identical).

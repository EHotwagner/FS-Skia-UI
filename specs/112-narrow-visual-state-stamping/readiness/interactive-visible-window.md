# interactive-visible-window — applicability (feature 112, T002)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=not-applicable

Feature 112 ships no NEW persistent/graphical entry point — it is a per-frame visual-state stamp
mechanism change proven through deterministic, headless evidence (the internal ControlRuntime targeted
stamp via InternalsVisibleTo from Controls.Tests + the standing Scene-parity suite under Dev). A live
Vulkan window is NOT required; the existing runInteractiveApp window launch contract is unchanged (its
signature is untouched; the live renderRetained narrows the runtime-state stamp on model-unchanged frames
with at-rest output byte-identical, FR-008/SC-005).

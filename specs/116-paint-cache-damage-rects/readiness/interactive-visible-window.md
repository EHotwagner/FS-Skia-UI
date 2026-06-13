# interactive-visible-window — applicability (feature 116, T002)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=not-applicable

Feature 116 ships no new persistent/graphical entry point — it is an internal damage-set + bounded
picture-cache + advisory offscreen-effect diagnostic + six additive `FrameMetrics` fields. It is proven
through deterministic, headless evidence (the `RetainedRender.step` internal-seam tests + the
`ControlsElmish.Perf.runScript` metrics + the standing Scene-parity suite under `Dev`, all green). A live
Vulkan window is NOT required; the existing `runInteractiveApp` window-launch contract is unchanged (its
signature is untouched and the launch seam is not edited).

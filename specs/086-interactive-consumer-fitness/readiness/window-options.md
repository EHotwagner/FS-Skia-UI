# Window Options (086)

status=ok
diagnostic-class=window-options
validation-contract=Viewer.validateWindowLaunchBehavior

The controls-family `runInteractiveApp` durable host reuses the existing `ViewerOptions`
window-option surface (unchanged by this feature). The generated product's shared
window-options diagnostics (`manualWindowOptionResults`, `windowOptionsReport`) remain in the
default launch path for both families.

## Option rows

- option=resize requested=resizable observed=resizable status=honored
- option=maximize requested=maximizable observed=maximizable status=honored
- option=startup-state requested=windowed-fullscreen observed=windowed-fullscreen status=honored diagnostic-class=window-options
- option=startup-position requested=centered observed=centered status=honored
- option=backend requested=default observed=default status=honored

(Also `option=initial-size requested=800x600 observed=800x600 status=honored`.)

No unsupported window option is silently ignored: any unsupported selection diagnoses under
`diagnostic-class=window-options`. The windowed-fullscreen default scales a fixed scene up;
the size-aware `View` / `--window-startup normal` workaround address the blur
(see `runtime-limitations.md` and the `fs-skia-viewer-host` skill).

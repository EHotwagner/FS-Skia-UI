# Window Options (085)

status=ok
diagnostic-class=window-options
validation-contract=Viewer.validateWindowLaunchBehavior

The `runInteractiveApp` durable host reuses the existing `ViewerOptions` window-option surface
(unchanged by this feature). Captured from the real
`Viewer.validateWindowLaunchBehavior { Width=800; Height=600 } Viewer.defaultWindowBehavior`
for the launch in `interactive-visible-window.md` — every option Honored.

## Option rows

- option=resize requested=resizable observed=resizable status=honored
- option=maximize requested=maximizable observed=maximizable status=honored
- option=startup-state requested=windowed-fullscreen observed=windowed-fullscreen status=honored diagnostic-class=window-options
- option=startup-position requested=centered observed=centered status=honored
- option=backend requested=default observed=default status=honored

(Also `option=initial-size requested=800x600 observed=800x600 status=honored`.)

No unsupported window option is silently ignored: any unsupported selection would diagnose
under `diagnostic-class=window-options`. The windowed-fullscreen default is what scales a fixed
scene up; US4's size-aware `View` / the `--window-startup normal` workaround address the blur
(see `runtime-limitations.md`).

# Window Options (084)

status=ok
diagnostic-class=window-options
validation-contract=Viewer.validateWindowLaunchBehavior

The window-option families and their classification under the new default
(windowed fullscreen) and the reclassified fullscreen states. Captured from the real
`validateWindowBehavior` / `validateWindowLaunchBehavior` surface
(`readiness/fsi-session.txt`, `tests/SkiaViewer.Tests`). An explicit `--window-startup`
selection overrides the default; the explicit, last-specified value wins on conflict.

## Option rows

- option=resize requested=resizable observed=resizable status=honored
- option=maximize requested=maximizable observed=maximizable status=honored
- option=startup-state requested=windowed-fullscreen observed=windowed-fullscreen status=honored diagnostic-class=window-options
- option=startup-position requested=centered observed=centered status=honored
- option=backend requested=default observed=default status=honored

## Startup-state classification (US1, FR-002/FR-003)

| startup-state | window mechanics | status |
|---------------|------------------|--------|
| normal | `WindowState.Normal` | honored |
| maximized | `WindowState.Maximized` | honored |
| minimized | not a visible interactive launch state | unsupported |
| fullscreen | `WindowState.Fullscreen` (exclusive) | honored (reclassified from unsupported) |
| windowed-fullscreen | `WindowBorder.Hidden` + work-area geometry + `WindowState.Normal` | honored (new default) |

No unsupported window option is silently ignored: any unsupported selection diagnoses
under `diagnostic-class=window-options`, never hidden under app-lifecycle. Fullscreen
and windowed fullscreen are distinct selectable states, never aliases.

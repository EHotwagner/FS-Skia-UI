# Public Surface

Status: reviewed.

This feature used existing public surfaces for screenshot evidence, generated host wiring, Scene evidence, Layout evidence, and Testing validators. No new public `.fsi` additions were required.

Reviewed surfaces:

- `src/SkiaViewer/SkiaViewer.fsi`
- `src/Testing/Testing.fsi`
- `src/Elmish/Elmish.fsi`
- `src/Scene/Scene.fsi`
- `src/Layout/*.fsi`

Validation:

- FSI surface exercise: `readiness/fsi-session.txt`
- Initial feature-local baselines: `readiness/package-surface-baseline.md`
- Package surface check: `readiness/package-surface-check.log`

# Surface Baseline Expectations

## FS.Skia.UI.SkiaViewer

Expected public surface changes are centered on persistent-launch evidence:

- request/result data for persistent launch and bounded run evidence
- viewer-owned window facts and observed-value diagnostics
- first-frame, input-dispatch, close-reason, blocked-stage, classification, category, and message fields
- pure `Viewer.init`, `Viewer.update`, `Viewer.initRun`, and `Viewer.updateRun` paths that emit interpreter effects
- generated host and evidence entry points that keep native window and filesystem work at the viewer edge

## FS.Skia.UI.Testing

Expected public surface changes are centered on generated validation and audit
support:

- host warning classification with raw message, warning class, fatal flag, evidence path, supporting facts, and diagnostics
- persistent-launch artifact validation with required-field and contradictory-pass diagnostics
- readiness-file discovery for the five contracted readiness artifacts

Final integration must refresh `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt`
and `readiness/surface-baselines/FS.Skia.UI.Testing.txt` with
`./fake.sh build -t RefreshSurfaceBaselines`, then verify with
`./fake.sh build -t PackageSurfaceCheck`.

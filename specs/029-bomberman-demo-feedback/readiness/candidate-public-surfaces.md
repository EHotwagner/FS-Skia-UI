# Candidate Public Surfaces

Task: T006
Captured: 2026-05-29T11:48:32+02:00

## Screenshot Evidence

Current `src/SkiaViewer/SkiaViewer.fsi` already exposes screenshot workflow records and discriminated unions:

- `ScreenshotEvidenceRequest`
- `ScreenshotEvidenceStatus`
- `ScreenshotCaptureMode`
- `ViewerOpenStatus`
- `FirstFrameStatus`
- `ScreenshotCaptureAvailability`
- `ScreenshotCaptureSource`

Candidate refinement: keep the real capture probe in `SkiaViewer` and expose only stable report facts needed by generated commands. Unsupported classifications must preserve capture availability, source, blocked stage, category, and diagnostics.

## Report Validation

Current `src/Testing/Testing.fsi` already exposes:

- `EvidenceReport`
- `EvidenceReportRequest`
- `EvidenceReportValidationResult`
- `ScreenshotEvidenceReportCheck`

Candidate refinement: report validation should accept stable `key=value` fields and reject unsupported reports without real probe details, ok reports without readiness-local nonblank artifacts, and app-command implementation errors mislabeled as host unsupported.

## Generated Host Wiring

Current `src/Elmish/Elmish.fsi` exposes `ElmishAdapterModel`, `ElmishAdapterMsg`, `ElmishAdapterEffect`, and pure `ElmishAdapter.init/update`.

Candidate refinement: generated app host shape may remain in `SkiaViewer` if existing generated host support is sufficient. Any new public helper must keep app-owned effects separate from viewer effects until the host adapter boundary.

## Scene/Layout Construction

Current `src/Scene/Scene.fsi` exposes record-heavy primitives (`Size`, `Point`, `Rect`, diagnostics and layout evidence records). Current `src/Layout/*.fsi` exposes layout records (`LayoutBounds`, `LayoutSize`, `LayoutDiagnostic`, workflow types).

Candidate refinement: prefer helper constructors or module-qualified examples only where ambiguity remains. Do not add viewer, keyboard, controls, or native host dependencies to Scene/Layout.

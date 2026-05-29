# US2 Implementation Evidence

Tasks: T027, T028, T029, T030
Captured: 2026-05-29T12:13:00+02:00

## Implementation Paths

- `src/SkiaViewer/SkiaViewer.fs`: `Viewer.captureScreenshotEvidence` writes a real PNG through the viewer render-target path before any failed/unsupported outcome and records viewer-open, first-frame, capture availability, capture source, pixel validation, and proof fields.
- `src/Testing/Testing.fs`: `EvidenceReports.validateScreenshotEvidence` validates stable screenshot report fields, rejects unsupported reports that hide capture probe details, rejects success reports without live nonblank screenshot proof, and classifies app/product defects as failed evidence fields rather than unsupported host capability.
- `template/base/src/Product/EvidenceCommands.fs`: `--screenshot-evidence` calls `Viewer.captureScreenshotEvidence` and writes status, command, output, mode, evidence kind, app/sample, host facts, capture mode, viewer-open status, first-frame status, capture availability, capture source, screenshot proof, blocked stage, classification, category, fallback, unsupported-host reason, timestamp, diagnostics, and artifact facts.

## Verification

```text
dotnet test tests/Testing.Tests/Testing.Tests.fsproj --filter "ScreenshotEvidenceReport" --logger "console;verbosity=minimal"
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --filter "screenshot evidence|evidence workflow" --logger "console;verbosity=minimal"
dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "screenshot evidence" --logger "console;verbosity=minimal"
```

Results:

- Testing.Tests: 4 passed, 0 failed.
- SkiaViewer.Tests: 3 passed, 0 failed.
- Governance.Tests: 1 passed, 0 failed.

Note: T027/T028/T029/T030 have direct real-code verification, but the graph may compute synthetic propagation from T024's approved malformed-report fixture dependency.

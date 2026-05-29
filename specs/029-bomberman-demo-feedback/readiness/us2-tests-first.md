# US2 Tests First Evidence

Tasks: T024, T025, T026
Captured: 2026-05-29T12:10:00+02:00

## Synthetic Error-Handling Coverage

T024 uses design-approved malformed report fixtures. Test names include `Synthetic`, and fixture comments disclose the approved synthetic reason and real-evidence path.

Executed:

```text
dotnet test tests/Testing.Tests/Testing.Tests.fsproj --filter "ScreenshotEvidenceReport" --logger "console;verbosity=minimal"
```

Result: 4 passed, 0 failed.

## Real Screenshot Success Coverage

Executed:

```text
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --filter "screenshot evidence" --logger "console;verbosity=minimal"
```

Result: 1 passed, 0 failed.

The SkiaViewer test exercises `Viewer.captureScreenshotEvidence`, writes a PNG artifact through the real render-target path, and asserts live capture source, first-frame status, nonblank pixel validation, and screenshot proof.

## Classification Coverage

Testing and SkiaViewer coverage distinguishes:

- accepted live-window screenshot reports
- unsupported reports missing capture probe detail
- unsupported capture facts with viewer-open and first-frame facts preserved
- app/product defects as failed screenshot outcomes rather than unsupported host capability

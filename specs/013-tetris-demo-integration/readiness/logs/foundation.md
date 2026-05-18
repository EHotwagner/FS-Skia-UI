# Foundation Readiness Notes

## Public Surface Transcript Expectations

The following public contracts require FSI or packed-library transcript
evidence before story tasks are marked complete:

- `FS.Skia.UI.KeyboardInput.ViewerKeyboard.normalize`,
  `normalizeEvent`, and `toKeyId` for normalized viewer input.
- `FS.Skia.UI.SkiaViewer.Viewer.initRun`, `updateRun`,
  `runUntilFirstFrame`, and `runForFrames` for bounded viewer smoke.
- `FS.Skia.UI.SkiaViewer.ViewerDiagnosticEvent` and
  `ViewerDiagnosticsOptions` for categorized diagnostics and capturable sinks.
- `FS.Skia.UI.Scene.SceneEvidence.render`, `renderHash`, and `renderPng` for
  deterministic non-window scene evidence.
- `FS.Skia.UI.SkiaViewer.GeneratedAppHost.dispatchKey` and `smoke` for the
  optional generated app-host convenience path.
- `FS.Skia.UI.Testing.LocalConsumerPackages.report`,
  `classifyDrift`, and `GeneratedConsumerValidation.summarize` for local
  package guidance and generated consumer validation.

Expected readiness transcript locations:

- `readiness/normalized-viewer-input.md`
- `readiness/bounded-viewer-smoke.md`
- `readiness/diagnostics.md`
- `readiness/headless-scene-evidence.md`
- `readiness/generated-template-input-flows.md`
- `readiness/local-consumer-packages.md`
- `readiness/generated-consumer-validation.md`

## Surface Baseline Expectations

The changed Tier 1 public surfaces require refreshed baselines before final
integration:

- `readiness/surface-baselines/FS.Skia.UI.KeyboardInput.txt`
- `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt`
- `readiness/surface-baselines/FS.Skia.UI.Scene.txt`
- `readiness/surface-baselines/FS.Skia.UI.Testing.txt`

Initial failing-first evidence is captured in
`readiness/logs/t011-failing-contract-tests.txt`.

## Foundation Verification

Focused verification logs:

- `readiness/logs/t015-keyboardinput-tests.txt`: normalized viewer input and
  key event conversion tests passed.
- `readiness/logs/t015-skiaviewer-tests.txt`: bounded run `initRun`/`updateRun`
  and diagnostic/key update effects passed.
- `readiness/logs/t015-scene-tests.txt`: deterministic scene evidence hash
  helper passed.
- `readiness/logs/t015-testing-tests.txt`: local consumer package report and
  generated validation summary helpers passed.
- `readiness/logs/t015-governance-fixture-build.txt`: shared diagnostic
  fixture module compiled under `Governance.Tests`.

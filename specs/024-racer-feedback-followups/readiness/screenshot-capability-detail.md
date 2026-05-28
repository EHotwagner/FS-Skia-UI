# Screenshot Capability Detail

## T005 Public Contract Draft

Recorded at: 2026-05-28T08:05:10+02:00

Changed public signatures:

- `src/SkiaViewer/SkiaViewer.fsi`
  - Added `ViewerOpenStatus`, `FirstFrameStatus`,
    `ScreenshotCaptureAvailability`, and `ScreenshotCaptureSource`.
  - Extended `ScreenshotEvidenceResult` with viewer-open status, first-frame
    status, capture availability, capture source, deterministic fallback kind,
    and explicit screenshot-proof boolean.
  - Added `EvidenceWorkflowModel`, `EvidenceWorkflowMsg`,
    `EvidenceWorkflowEffect`, `Viewer.initEvidenceWorkflow`, and
    `Viewer.updateEvidenceWorkflow` for the pure workflow boundary.
- `src/Testing/Testing.fsi`
  - Added `ScreenshotEvidenceReportCheck` and
    `ScreenshotEvidenceReportValidationResult`.
  - Added `EvidenceReports.validateScreenshotEvidence` for screenshot report
    field validation.

Verification:

| Command | Result | Evidence |
|---------|--------|----------|
| `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --logger "console;verbosity=minimal"` | PASS, 46 tests | `readiness/logs/t005-skiaviewer-tests.txt` |
| `dotnet test tests/Testing.Tests/Testing.Tests.fsproj --logger "console;verbosity=minimal"` | PASS, 28 tests | `readiness/logs/t005-testing-tests.txt` |

## T006 Semantic Test Coverage

Recorded at: 2026-05-28T08:11:30+02:00

Added public-contract tests covering:

- Screenshot unsupported records expose viewer-open status, first-frame status,
  capture source, fallback kind, and `ProvesScreenshot=false`.
- `Viewer.initEvidenceWorkflow` emits `LaunchViewerForEvidence`.
- `Viewer.updateEvidenceWorkflow` preserves model facts and emits capture/report
  effects for success and unsupported capture paths.
- `EvidenceReports.validateScreenshotEvidence` accepts live-window screenshot
  proof fields and rejects unsupported records that hide capture details.
- `HostWarningClassification.classify` treats known GTK module warnings as
  benign only when launch/render/layout/package facts pass and preserves raw
  warning text.

Verification:

| Command | Result | Evidence |
|---------|--------|----------|
| `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --logger "console;verbosity=minimal"` | PASS, 48 tests | `readiness/logs/t006-skiaviewer-tests.txt` |
| `dotnet test tests/Testing.Tests/Testing.Tests.fsproj --logger "console;verbosity=minimal"` | PASS, 31 tests | `readiness/logs/t006-testing-tests.txt` |

## T008 Synthetic Error-Handling Red Tests

Recorded at: 2026-05-28T08:18:10+02:00

Added design-approved `[SEH]` tests with `Synthetic` names and code-level
`// SYNTHETIC:` disclosures for malformed screenshot success fields, hidden
warning diagnostics, and hostile artifact paths.

Red evidence:

| Command | Expected result | Evidence |
|---------|-----------------|----------|
| `dotnet test tests/Testing.Tests/Testing.Tests.fsproj --logger "console;verbosity=minimal"` | FAIL, hostile artifact path still accepted | `readiness/logs/t008-testing-tests-red.txt` |

Synthetic input class: malformed readiness report fields, hidden-warning
fixtures, and hostile artifact paths. Expected behavior: validators reject the
report with visible diagnostics and no screenshot-success claim.

## T009 Public FSI Exercise

Recorded at: 2026-05-28T08:20:10+02:00

FSI transcript source: `readiness/fsi/t009-screenshot-workflow.fsx`

Public compiled assemblies exercised:

- `FS.Skia.UI.Scene.dll`
- `FS.Skia.UI.KeyboardInput.dll`
- `FS.Skia.UI.SkiaViewer.dll`

Covered public paths:

- `Viewer.initEvidenceWorkflow`
- `Viewer.updateEvidenceWorkflow`
- `EvidenceWorkflowModel`
- `EvidenceWorkflowMsg`
- `EvidenceWorkflowEffect`
- `ScreenshotEvidenceResult`

Verification:

| Command | Result | Evidence |
|---------|--------|----------|
| `dotnet fsi specs/024-racer-feedback-followups/readiness/fsi/t009-screenshot-workflow.fsx` | PASS | `readiness/logs/t009-fsi-screenshot-workflow.txt` |

## T015/T016 US2 Semantic Test Coverage

Recorded at: 2026-05-28T08:31:00+02:00

T015 successful screenshot record coverage:

- `status=ok`
- `evidence-kind=screenshot`
- PNG artifact path
- positive width and height
- first-frame presentation status
- `capture-source=live-viewer-window`
- `ProvesScreenshot=true`

T016 unsupported screenshot record coverage:

- viewer-open status is separate from capture availability
- first-frame status is preserved when known
- capture unavailable reason is explicit
- deterministic fallback kind is explicit
- unsupported records do not claim screenshot proof

Evidence:

- `readiness/logs/t006-skiaviewer-tests.txt`
- `readiness/logs/t006-testing-tests.txt`

## T017 Generated Product Red Test

Recorded at: 2026-05-28T08:32:30+02:00

Added generated product/template tests requiring `--screenshot-evidence` to
use `Viewer.captureScreenshotEvidence` and report:

- `viewer-open-status`
- `first-frame-status`
- `capture-availability`
- `capture-source`
- `deterministic-fallback-kind`
- `proves-screenshot`

Red evidence:

| Command | Expected result | Evidence |
|---------|-----------------|----------|
| `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --logger "console;verbosity=minimal"` | FAIL, generated screenshot evidence command is missing capability detail fields | `readiness/logs/t017-governance-tests-red.txt` |
| `./fake.sh build -t TemplateCheck` | PASS; template target does not yet catch the missing field scan directly | `readiness/logs/t017-template-check-red.txt` |

## T018 Screenshot Validator Implementation

Recorded at: 2026-05-28T08:35:00+02:00

Implemented:

- Additive SkiaViewer screenshot capability fields and pure evidence workflow
  records/effects.
- Testing screenshot evidence validation for missing fields, invalid success
  proof, hidden warning diagnostics, and hostile artifact paths.

Verification:

| Command | Result | Evidence |
|---------|--------|----------|
| `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --logger "console;verbosity=minimal"` | PASS, 48 tests | `readiness/logs/t018-skiaviewer-tests.txt` |
| `dotnet test tests/Testing.Tests/Testing.Tests.fsproj --logger "console;verbosity=minimal"` | PASS, 34 tests | `readiness/logs/t018-testing-tests.txt` |

## T019 Generated Screenshot Output Wiring

Recorded at: 2026-05-28T08:37:00+02:00

Updated `template/base/src/Product/EvidenceCommands.fs` so
`--screenshot-evidence` writes:

- `viewer-open-status`
- `first-frame-status`
- `capture-availability`
- `capture-source`
- `deterministic-fallback-kind`
- `proves-screenshot`

Verification:

| Command | Result | Evidence |
|---------|--------|----------|
| `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --logger "console;verbosity=minimal"` | Generated screenshot field test passes; aggregate still fails on later guidance wording | `readiness/logs/t019-governance-tests.txt` |
| `./fake.sh build -t TemplateCheck` | PASS | `readiness/logs/t019-template-check.txt` |

## T021 Unsupported Capability Detail

Recorded at: 2026-05-28T08:41:00+02:00

The local live screenshot attempt returned an explicit unsupported capture
result rather than screenshot proof.

Observed fields from `readiness/logs/t020-live-screenshot-attempt.txt`:

- viewer open status: `ViewerOpenUnknown`
- first frame status: `FirstFrameUnknownStatus`
- capture availability:
  `CaptureUnavailable "screenshot capture is unavailable for this viewer host"`
- capture source: `DeterministicSceneRender`
- screenshot path: `None`
- proves screenshot: `false`
- fallback: `deterministic-scene-evidence`

This is real negative host/capability evidence. It is not screenshot proof and
does not close T020.

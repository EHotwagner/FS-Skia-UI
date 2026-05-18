# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `build.fsx` | `command-surface` |
| `docs/build.md` | `documentation` |
| `docs/dependencies.md` | `documentation` |
| `docs/evidence.md` | `documentation` |
| `src/KeyboardInput/KeyboardInput.fs` | `source-code` |
| `src/KeyboardInput/KeyboardInput.fsi` | `source-code` |
| `src/Scene/Scene.fs` | `source-code` |
| `src/Scene/Scene.fsi` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fs` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fsi` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fsproj` | `source-code` |
| `src/Testing/Testing.fs` | `source-code` |
| `src/Testing/Testing.fsi` | `source-code` |
| `tests/Elmish.Tests/Tests.fs` | `test-code` |
| `tests/Governance.Tests/ControlsBoundaryCompositionTests.fs` | `test-code` |
| `tests/Governance.Tests/GeneratedProjectValidationTests.fs` | `test-code` |
| `tests/Governance.Tests/Governance.Tests.fsproj` | `test-code` |
| `tests/Governance.Tests/PublicRecordInvariantTests.fs` | `test-code` |
| `tests/Governance.Tests/TestSupport.fs` | `test-code` |
| `tests/KeyboardInput.Tests/Tests.fs` | `test-code` |
| `tests/Scene.Tests/Tests.fs` | `test-code` |
| `tests/SkiaViewer.Tests/Tests.fs` | `test-code` |
| `tests/Testing.Tests/Tests.fs` | `test-code` |
| `docs/generated-apps.md` | `documentation` |
| `tests/Governance.Tests/IntegrationDiagnosticFixtures.fs` | `test-code` |

## Required Alignment Classes

- `build.fsx` requires `command-docs`
- `build.fsx` requires `active-feature-evidence`
- `docs/build.md` requires `docs-alignment`
- `docs/build.md` requires `active-feature-evidence`
- `docs/dependencies.md` requires `docs-alignment`
- `docs/dependencies.md` requires `active-feature-evidence`
- `docs/evidence.md` requires `docs-alignment`
- `docs/evidence.md` requires `active-feature-evidence`
- `src/KeyboardInput/KeyboardInput.fs` requires `source-contract`
- `src/KeyboardInput/KeyboardInput.fs` requires `active-feature-evidence`
- `src/KeyboardInput/KeyboardInput.fsi` requires `source-contract`
- `src/KeyboardInput/KeyboardInput.fsi` requires `active-feature-evidence`
- `src/Scene/Scene.fs` requires `source-contract`
- `src/Scene/Scene.fs` requires `active-feature-evidence`
- `src/Scene/Scene.fsi` requires `source-contract`
- `src/Scene/Scene.fsi` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fs` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fs` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fsi` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fsi` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fsproj` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fsproj` requires `active-feature-evidence`
- `src/Testing/Testing.fs` requires `source-contract`
- `src/Testing/Testing.fs` requires `active-feature-evidence`
- `src/Testing/Testing.fsi` requires `source-contract`
- `src/Testing/Testing.fsi` requires `active-feature-evidence`
- `tests/Elmish.Tests/Tests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Tests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/ControlsBoundaryCompositionTests.fs` requires `test-evidence`
- `tests/Governance.Tests/ControlsBoundaryCompositionTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/GeneratedProjectValidationTests.fs` requires `test-evidence`
- `tests/Governance.Tests/GeneratedProjectValidationTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `test-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `active-feature-evidence`
- `tests/Governance.Tests/PublicRecordInvariantTests.fs` requires `test-evidence`
- `tests/Governance.Tests/PublicRecordInvariantTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/TestSupport.fs` requires `test-evidence`
- `tests/Governance.Tests/TestSupport.fs` requires `active-feature-evidence`
- `tests/KeyboardInput.Tests/Tests.fs` requires `test-evidence`
- `tests/KeyboardInput.Tests/Tests.fs` requires `active-feature-evidence`
- `tests/Scene.Tests/Tests.fs` requires `test-evidence`
- `tests/Scene.Tests/Tests.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Tests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Tests.fs` requires `active-feature-evidence`
- `tests/Testing.Tests/Tests.fs` requires `test-evidence`
- `tests/Testing.Tests/Tests.fs` requires `active-feature-evidence`
- `docs/generated-apps.md` requires `docs-alignment`
- `docs/generated-apps.md` requires `active-feature-evidence`
- `tests/Governance.Tests/IntegrationDiagnosticFixtures.fs` requires `test-evidence`
- `tests/Governance.Tests/IntegrationDiagnosticFixtures.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, command-docs, dependency-docs, docs-alignment, sample-contract, source-contract, template-drift-docs, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/013-tetris-demo-integration`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Diagnostics

- No drift blockers.

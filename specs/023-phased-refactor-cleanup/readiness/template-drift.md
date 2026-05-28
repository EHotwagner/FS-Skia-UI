# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `build.fsx` | `command-surface` |
| `src/SkiaViewer/SkiaViewer.fs` | `source-code` |
| `tests/Governance.Tests/CommandContractTests.fs` | `test-code` |
| `tests/Governance.Tests/ControlsBoundaryCompositionTests.fs` | `test-code` |
| `tests/Governance.Tests/GeneratedGuidanceTests.fs` | `test-code` |
| `tests/Governance.Tests/GeneratedProjectValidationTests.fs` | `test-code` |
| `tests/Governance.Tests/PersistentViewerEvidenceTests.fs` | `test-code` |
| `tests/Governance.Tests/ProcessReliabilityContractTests.fs` | `test-code` |
| `tests/Governance.Tests/TestSupport.fs` | `test-code` |
| `tests/SkiaViewer.Tests/Tests.fs` | `test-code` |

## Required Alignment Classes

- `build.fsx` requires `command-docs`
- `build.fsx` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fs` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/CommandContractTests.fs` requires `test-evidence`
- `tests/Governance.Tests/CommandContractTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/ControlsBoundaryCompositionTests.fs` requires `test-evidence`
- `tests/Governance.Tests/ControlsBoundaryCompositionTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/GeneratedGuidanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/GeneratedGuidanceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/GeneratedProjectValidationTests.fs` requires `test-evidence`
- `tests/Governance.Tests/GeneratedProjectValidationTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/PersistentViewerEvidenceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/PersistentViewerEvidenceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/ProcessReliabilityContractTests.fs` requires `test-evidence`
- `tests/Governance.Tests/ProcessReliabilityContractTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/TestSupport.fs` requires `test-evidence`
- `tests/Governance.Tests/TestSupport.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Tests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Tests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, command-docs, dependency-docs, docs-alignment, sample-contract, source-contract, template-drift-docs, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/023-phased-refactor-cleanup`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Diagnostics

- No drift blockers.

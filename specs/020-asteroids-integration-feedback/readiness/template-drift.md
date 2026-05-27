# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `build.fsx` | `command-surface` |
| `docs/generated-apps.md` | `documentation` |
| `docs/testing.md` | `documentation` |
| `src/Scene/Scene.fs` | `source-code` |
| `src/Scene/Scene.fsi` | `source-code` |
| `src/Scene/Scene.fsproj` | `source-code` |
| `src/Testing/Testing.fs` | `source-code` |
| `src/Testing/Testing.fsi` | `source-code` |
| `src/Testing/Testing.fsproj` | `source-code` |
| `tests/Governance.Tests/GeneratedGuidanceTests.fs` | `test-code` |
| `tests/Governance.Tests/PersistentViewerEvidenceTests.fs` | `test-code` |
| `tests/Governance.Tests/ProcessReliabilityContractTests.fs` | `test-code` |
| `tests/Governance.Tests/SkillValidationTests.fs` | `test-code` |
| `tests/Scene.Tests/Tests.fs` | `test-code` |
| `tests/Testing.Tests/Tests.fs` | `test-code` |

## Required Alignment Classes

- `build.fsx` requires `command-docs`
- `build.fsx` requires `active-feature-evidence`
- `docs/generated-apps.md` requires `docs-alignment`
- `docs/generated-apps.md` requires `active-feature-evidence`
- `docs/testing.md` requires `docs-alignment`
- `docs/testing.md` requires `active-feature-evidence`
- `src/Scene/Scene.fs` requires `source-contract`
- `src/Scene/Scene.fs` requires `active-feature-evidence`
- `src/Scene/Scene.fsi` requires `source-contract`
- `src/Scene/Scene.fsi` requires `active-feature-evidence`
- `src/Scene/Scene.fsproj` requires `source-contract`
- `src/Scene/Scene.fsproj` requires `active-feature-evidence`
- `src/Testing/Testing.fs` requires `source-contract`
- `src/Testing/Testing.fs` requires `active-feature-evidence`
- `src/Testing/Testing.fsi` requires `source-contract`
- `src/Testing/Testing.fsi` requires `active-feature-evidence`
- `src/Testing/Testing.fsproj` requires `source-contract`
- `src/Testing/Testing.fsproj` requires `active-feature-evidence`
- `tests/Governance.Tests/GeneratedGuidanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/GeneratedGuidanceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/PersistentViewerEvidenceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/PersistentViewerEvidenceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/ProcessReliabilityContractTests.fs` requires `test-evidence`
- `tests/Governance.Tests/ProcessReliabilityContractTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/SkillValidationTests.fs` requires `test-evidence`
- `tests/Governance.Tests/SkillValidationTests.fs` requires `active-feature-evidence`
- `tests/Scene.Tests/Tests.fs` requires `test-evidence`
- `tests/Scene.Tests/Tests.fs` requires `active-feature-evidence`
- `tests/Testing.Tests/Tests.fs` requires `test-evidence`
- `tests/Testing.Tests/Tests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, command-docs, dependency-docs, docs-alignment, sample-contract, source-contract, template-drift-docs, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/020-asteroids-integration-feedback`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Diagnostics

- No drift blockers.

# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `samples/InteractiveViewer/InteractiveViewer.fsproj` | `sample-code` |
| `samples/InteractiveViewer/Program.fs` | `sample-code` |
| `src/Input/KeyboardInput.fs` | `source-code` |
| `src/Input/KeyboardInput.fsi` | `source-code` |
| `src/Lib/Lib.fsproj` | `source-code` |
| `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs` | `test-code` |
| `tests/Governance.Tests/PerPackageSurfaceTests.fs` | `test-code` |
| `tests/Governance.Tests/PublicRecordInvariantTests.fs` | `test-code` |
| `tests/Input.Tests/KeyboardInputTests.fs` | `test-code` |
| `tests/Lib.Tests/Lib.Tests.fsproj` | `test-code` |
| `tests/Parity.Tests/Parity.Tests.fsproj` | `test-code` |
| `tests/Parity.Tests/Tests.fs` | `test-code` |
| `src/Input/Input.fsproj` | `source-code` |
| `tests/Input.Tests/Input.Tests.fsproj` | `test-code` |
| `tests/Input.Tests/Program.fs` | `test-code` |

## Required Alignment Classes

- `samples/InteractiveViewer/InteractiveViewer.fsproj` requires `sample-contract`
- `samples/InteractiveViewer/InteractiveViewer.fsproj` requires `active-feature-evidence`
- `samples/InteractiveViewer/Program.fs` requires `sample-contract`
- `samples/InteractiveViewer/Program.fs` requires `active-feature-evidence`
- `src/Input/KeyboardInput.fs` requires `source-contract`
- `src/Input/KeyboardInput.fs` requires `active-feature-evidence`
- `src/Input/KeyboardInput.fsi` requires `source-contract`
- `src/Input/KeyboardInput.fsi` requires `active-feature-evidence`
- `src/Lib/Lib.fsproj` requires `source-contract`
- `src/Lib/Lib.fsproj` requires `active-feature-evidence`
- `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/PerPackageSurfaceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/PerPackageSurfaceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/PublicRecordInvariantTests.fs` requires `test-evidence`
- `tests/Governance.Tests/PublicRecordInvariantTests.fs` requires `active-feature-evidence`
- `tests/Input.Tests/KeyboardInputTests.fs` requires `test-evidence`
- `tests/Input.Tests/KeyboardInputTests.fs` requires `active-feature-evidence`
- `tests/Lib.Tests/Lib.Tests.fsproj` requires `test-evidence`
- `tests/Lib.Tests/Lib.Tests.fsproj` requires `active-feature-evidence`
- `tests/Parity.Tests/Parity.Tests.fsproj` requires `test-evidence`
- `tests/Parity.Tests/Parity.Tests.fsproj` requires `active-feature-evidence`
- `tests/Parity.Tests/Tests.fs` requires `test-evidence`
- `tests/Parity.Tests/Tests.fs` requires `active-feature-evidence`
- `src/Input/Input.fsproj` requires `source-contract`
- `src/Input/Input.fsproj` requires `active-feature-evidence`
- `tests/Input.Tests/Input.Tests.fsproj` requires `test-evidence`
- `tests/Input.Tests/Input.Tests.fsproj` requires `active-feature-evidence`
- `tests/Input.Tests/Program.fs` requires `test-evidence`
- `tests/Input.Tests/Program.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/052-v3-lib-decoupling`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.

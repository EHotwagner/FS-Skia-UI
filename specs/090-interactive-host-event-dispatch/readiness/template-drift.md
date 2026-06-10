# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Controls.Elmish/ControlsElmish.fs` | `source-code` |
| `src/Controls.Elmish/ControlsElmish.fsi` | `source-code` |
| `src/Controls/Control.fs` | `source-code` |
| `src/Controls/Control.fsi` | `source-code` |
| `tests/Controls.Tests/Controls.Tests.fsproj` | `test-code` |
| `tests/Elmish.Tests/Elmish.Tests.fsproj` | `test-code` |
| `tests/Governance.Tests/Governance.Tests.fsproj` | `test-code` |
| `tests/Controls.Tests/Feature090RecoveryTests.fs` | `test-code` |
| `tests/Elmish.Tests/Feature090DispatchTests.fs` | `test-code` |
| `tests/Governance.Tests/Feature090GovernanceTests.fs` | `test-code` |

## Required Alignment Classes

- `src/Controls.Elmish/ControlsElmish.fs` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fs` requires `active-feature-evidence`
- `src/Controls.Elmish/ControlsElmish.fsi` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fsi` requires `active-feature-evidence`
- `src/Controls/Control.fs` requires `source-contract`
- `src/Controls/Control.fs` requires `active-feature-evidence`
- `src/Controls/Control.fsi` requires `source-contract`
- `src/Controls/Control.fsi` requires `active-feature-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `test-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `active-feature-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `test-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `active-feature-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `test-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature090RecoveryTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature090RecoveryTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature090DispatchTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature090DispatchTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/Feature090GovernanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/Feature090GovernanceTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/090-interactive-host-event-dispatch`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.

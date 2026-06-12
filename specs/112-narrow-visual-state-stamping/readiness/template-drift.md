# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Controls.Elmish/ControlsElmish.fs` | `source-code` |
| `src/Controls/ControlRuntime.fs` | `source-code` |
| `src/Controls/ControlRuntime.fsi` | `source-code` |
| `tests/Controls.Tests/Controls.Tests.fsproj` | `test-code` |
| `tests/Controls.Tests/Feature112PrecedenceTests.fs` | `test-code` |
| `tests/Controls.Tests/Feature112TargetedStampParityTests.fs` | `test-code` |
| `tests/Controls.Tests/Feature112TouchedCountTests.fs` | `test-code` |

## Required Alignment Classes

- `src/Controls.Elmish/ControlsElmish.fs` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fs` requires `active-feature-evidence`
- `src/Controls/ControlRuntime.fs` requires `source-contract`
- `src/Controls/ControlRuntime.fs` requires `active-feature-evidence`
- `src/Controls/ControlRuntime.fsi` requires `source-contract`
- `src/Controls/ControlRuntime.fsi` requires `active-feature-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `test-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature112PrecedenceTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature112PrecedenceTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature112TargetedStampParityTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature112TargetedStampParityTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature112TouchedCountTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature112TouchedCountTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/112-narrow-visual-state-stamping`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.

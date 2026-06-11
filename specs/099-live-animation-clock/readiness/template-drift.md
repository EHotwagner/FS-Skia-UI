# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Controls.Elmish/ControlsElmish.fs` | `source-code` |
| `src/Controls/Reconcile.fs` | `source-code` |
| `src/Controls/RetainedRender.fs` | `source-code` |
| `src/Controls/RetainedRender.fsi` | `source-code` |
| `tests/Controls.Tests/Controls.Tests.fsproj` | `test-code` |
| `tests/Controls.Tests/Feature091RetainedRenderTests.fs` | `test-code` |
| `tests/Elmish.Tests/Elmish.Tests.fsproj` | `test-code` |
| `tests/Elmish.Tests/Feature092LiveSurvivalTests.fs` | `test-code` |
| `tests/Controls.Tests/Feature099AnimationClockTests.fs` | `test-code` |
| `tests/Elmish.Tests/Feature099AnimationSeamTests.fs` | `test-code` |

## Required Alignment Classes

- `src/Controls.Elmish/ControlsElmish.fs` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fs` requires `active-feature-evidence`
- `src/Controls/Reconcile.fs` requires `source-contract`
- `src/Controls/Reconcile.fs` requires `active-feature-evidence`
- `src/Controls/RetainedRender.fs` requires `source-contract`
- `src/Controls/RetainedRender.fs` requires `active-feature-evidence`
- `src/Controls/RetainedRender.fsi` requires `source-contract`
- `src/Controls/RetainedRender.fsi` requires `active-feature-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `test-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature091RetainedRenderTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature091RetainedRenderTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `test-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature092LiveSurvivalTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature092LiveSurvivalTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature099AnimationClockTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature099AnimationClockTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature099AnimationSeamTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature099AnimationSeamTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/099-live-animation-clock`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.

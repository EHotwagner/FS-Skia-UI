# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Controls.Elmish/ControlsElmish.fs` | `source-code` |
| `src/Controls.Elmish/ControlsElmish.fsi` | `source-code` |
| `src/Controls/RetainedRender.fs` | `source-code` |
| `src/Controls/RetainedRender.fsi` | `source-code` |
| `tests/Elmish.Tests/Elmish.Tests.fsproj` | `test-code` |
| `tests/Elmish.Tests/Feature108MetricsTests.fs` | `test-code` |
| `tests/Elmish.Tests/Feature109CorpusTests.fs` | `test-code` |
| `tests/Elmish.Tests/Feature109MetricsHonestyTests.fs` | `test-code` |
| `tests/Elmish.Tests/Feature110FallbackTests.fs` | `test-code` |
| `tests/Elmish.Tests/Feature110RetainedRoutingParityTests.fs` | `test-code` |
| `tests/Elmish.Tests/Feature110RetainedRoutingTests.fs` | `test-code` |

## Required Alignment Classes

- `src/Controls.Elmish/ControlsElmish.fs` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fs` requires `active-feature-evidence`
- `src/Controls.Elmish/ControlsElmish.fsi` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fsi` requires `active-feature-evidence`
- `src/Controls/RetainedRender.fs` requires `source-contract`
- `src/Controls/RetainedRender.fs` requires `active-feature-evidence`
- `src/Controls/RetainedRender.fsi` requires `source-contract`
- `src/Controls/RetainedRender.fsi` requires `active-feature-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `test-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature108MetricsTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature108MetricsTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature109CorpusTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature109CorpusTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature109MetricsHonestyTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature109MetricsHonestyTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature110FallbackTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature110FallbackTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature110RetainedRoutingParityTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature110RetainedRoutingParityTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature110RetainedRoutingTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature110RetainedRoutingTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/110-retained-pointer-routing`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.

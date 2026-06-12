# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Controls.Elmish/ControlsElmish.fs` | `source-code` |
| `src/Controls.Elmish/ControlsElmish.fsi` | `source-code` |
| `tests/Elmish.Tests/Elmish.Tests.fsproj` | `test-code` |
| `tests/Elmish.Tests/Feature108MetricsTests.fs` | `test-code` |
| `docs/reports/_baselines/2026-06-12-controls-corpus-after.md` | `documentation` |
| `docs/reports/_baselines/2026-06-12-controls-corpus-before.md` | `documentation` |
| `tests/Elmish.Tests/Feature109BaselineReportTests.fs` | `test-code` |
| `tests/Elmish.Tests/Feature109CorpusTests.fs` | `test-code` |
| `tests/Elmish.Tests/Feature109MetricsHonestyTests.fs` | `test-code` |

## Required Alignment Classes

- `src/Controls.Elmish/ControlsElmish.fs` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fs` requires `active-feature-evidence`
- `src/Controls.Elmish/ControlsElmish.fsi` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fsi` requires `active-feature-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `test-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature108MetricsTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature108MetricsTests.fs` requires `active-feature-evidence`
- `docs/reports/_baselines/2026-06-12-controls-corpus-after.md` requires `docs-alignment`
- `docs/reports/_baselines/2026-06-12-controls-corpus-after.md` requires `active-feature-evidence`
- `docs/reports/_baselines/2026-06-12-controls-corpus-before.md` requires `docs-alignment`
- `docs/reports/_baselines/2026-06-12-controls-corpus-before.md` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature109BaselineReportTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature109BaselineReportTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature109CorpusTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature109CorpusTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature109MetricsHonestyTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature109MetricsHonestyTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/109-perf-metrics-baseline`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.

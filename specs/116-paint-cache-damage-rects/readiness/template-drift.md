# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Controls.Elmish/ControlsElmish.fs` | `source-code` |
| `src/Controls.Elmish/ControlsElmish.fsi` | `source-code` |
| `src/Controls/RetainedRender.fs` | `source-code` |
| `src/Controls/RetainedRender.fsi` | `source-code` |
| `src/Controls/Types.fs` | `source-code` |
| `src/Controls/Types.fsi` | `source-code` |
| `tests/Controls.Tests/Controls.Tests.fsproj` | `test-code` |
| `tests/Elmish.Tests/Elmish.Tests.fsproj` | `test-code` |
| `tests/Elmish.Tests/Feature109CorpusTests.fs` | `test-code` |
| `tests/Controls.Tests/Feature116CacheBoundTests.fs` | `test-code` |
| `tests/Controls.Tests/Feature116DamageTests.fs` | `test-code` |
| `tests/Controls.Tests/Feature116OffscreenDiagTests.fs` | `test-code` |
| `tests/Controls.Tests/Feature116PictureCacheTests.fs` | `test-code` |
| `tests/Elmish.Tests/Feature116MetricsTests.fs` | `test-code` |

## Required Alignment Classes

- `src/Controls.Elmish/ControlsElmish.fs` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fs` requires `active-feature-evidence`
- `src/Controls.Elmish/ControlsElmish.fsi` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fsi` requires `active-feature-evidence`
- `src/Controls/RetainedRender.fs` requires `source-contract`
- `src/Controls/RetainedRender.fs` requires `active-feature-evidence`
- `src/Controls/RetainedRender.fsi` requires `source-contract`
- `src/Controls/RetainedRender.fsi` requires `active-feature-evidence`
- `src/Controls/Types.fs` requires `source-contract`
- `src/Controls/Types.fs` requires `active-feature-evidence`
- `src/Controls/Types.fsi` requires `source-contract`
- `src/Controls/Types.fsi` requires `active-feature-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `test-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `active-feature-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `test-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature109CorpusTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature109CorpusTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature116CacheBoundTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature116CacheBoundTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature116DamageTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature116DamageTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature116OffscreenDiagTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature116OffscreenDiagTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature116PictureCacheTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature116PictureCacheTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature116MetricsTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature116MetricsTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/116-paint-cache-damage-rects`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.

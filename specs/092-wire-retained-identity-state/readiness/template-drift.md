# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Controls.Elmish/Controls.Elmish.fsproj` | `source-code` |
| `src/Controls.Elmish/ControlsElmish.fs` | `source-code` |
| `src/Controls.Elmish/ControlsElmish.fsi` | `source-code` |
| `src/Controls/Controls.fsproj` | `source-code` |
| `src/Controls/RetainedRender.fs` | `source-code` |
| `src/Controls/RetainedRender.fsi` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fs` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fsi` | `source-code` |
| `tests/Controls.Tests/Controls.Tests.fsproj` | `test-code` |
| `tests/Controls.Tests/Feature091RetainedRenderTests.fs` | `test-code` |
| `tests/Elmish.Tests/Elmish.Tests.fsproj` | `test-code` |
| `tests/Elmish.Tests/Feature090DispatchTests.fs` | `test-code` |
| `tests/SkiaViewer.Tests/Feature085InteractiveHostTests.fs` | `test-code` |
| `tests/Controls.Tests/Feature092RetainedRenderTests.fs` | `test-code` |
| `tests/Elmish.Tests/Feature092LiveSurvivalTests.fs` | `test-code` |

## Required Alignment Classes

- `src/Controls.Elmish/Controls.Elmish.fsproj` requires `source-contract`
- `src/Controls.Elmish/Controls.Elmish.fsproj` requires `active-feature-evidence`
- `src/Controls.Elmish/ControlsElmish.fs` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fs` requires `active-feature-evidence`
- `src/Controls.Elmish/ControlsElmish.fsi` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fsi` requires `active-feature-evidence`
- `src/Controls/Controls.fsproj` requires `source-contract`
- `src/Controls/Controls.fsproj` requires `active-feature-evidence`
- `src/Controls/RetainedRender.fs` requires `source-contract`
- `src/Controls/RetainedRender.fs` requires `active-feature-evidence`
- `src/Controls/RetainedRender.fsi` requires `source-contract`
- `src/Controls/RetainedRender.fsi` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fs` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fs` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fsi` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fsi` requires `active-feature-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `test-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature091RetainedRenderTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature091RetainedRenderTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `test-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature090DispatchTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature090DispatchTests.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Feature085InteractiveHostTests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Feature085InteractiveHostTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature092RetainedRenderTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature092RetainedRenderTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature092LiveSurvivalTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature092LiveSurvivalTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/092-wire-retained-identity-state`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.

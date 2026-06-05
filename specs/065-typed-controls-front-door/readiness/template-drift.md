# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `samples/ControlsGallery/Program.fs` | `sample-code` |
| `src/Controls/Controls.fsproj` | `source-code` |
| `tests/Controls.Tests/Controls.Tests.fsproj` | `test-code` |
| `tests/Controls.Tests/InteractionTests.fs` | `test-code` |
| `tests/Controls.Tests/PublicSurfaceTests.fs` | `test-code` |
| `tests/Controls.Tests/RenderingTests.fs` | `test-code` |
| `tests/Controls.Tests/TypedControlContractTests.fs` | `test-code` |
| `tests/Elmish.Tests/Elmish.Tests.fsproj` | `test-code` |
| `src/Controls/Widget.fs` | `source-code` |
| `src/Controls/Widget.fsi` | `source-code` |
| `src/Controls/Widgets/DataGridWidget.fs` | `source-code` |
| `src/Controls/Widgets/DataGridWidget.fsi` | `source-code` |
| `src/Controls/Widgets/Primitives.fs` | `source-code` |
| `src/Controls/Widgets/Primitives.fsi` | `source-code` |
| `src/Controls/Widgets/TextBoxWidget.fs` | `source-code` |
| `src/Controls/Widgets/TextBoxWidget.fsi` | `source-code` |
| `tests/Controls.Tests/TypedLoweringTests.fs` | `test-code` |
| `tests/Elmish.Tests/TypedControlsAdapterTests.fs` | `test-code` |

## Required Alignment Classes

- `samples/ControlsGallery/Program.fs` requires `sample-contract`
- `samples/ControlsGallery/Program.fs` requires `active-feature-evidence`
- `src/Controls/Controls.fsproj` requires `source-contract`
- `src/Controls/Controls.fsproj` requires `active-feature-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `test-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `active-feature-evidence`
- `tests/Controls.Tests/InteractionTests.fs` requires `test-evidence`
- `tests/Controls.Tests/InteractionTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/PublicSurfaceTests.fs` requires `test-evidence`
- `tests/Controls.Tests/PublicSurfaceTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/RenderingTests.fs` requires `test-evidence`
- `tests/Controls.Tests/RenderingTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/TypedControlContractTests.fs` requires `test-evidence`
- `tests/Controls.Tests/TypedControlContractTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `test-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `active-feature-evidence`
- `src/Controls/Widget.fs` requires `source-contract`
- `src/Controls/Widget.fs` requires `active-feature-evidence`
- `src/Controls/Widget.fsi` requires `source-contract`
- `src/Controls/Widget.fsi` requires `active-feature-evidence`
- `src/Controls/Widgets/DataGridWidget.fs` requires `source-contract`
- `src/Controls/Widgets/DataGridWidget.fs` requires `active-feature-evidence`
- `src/Controls/Widgets/DataGridWidget.fsi` requires `source-contract`
- `src/Controls/Widgets/DataGridWidget.fsi` requires `active-feature-evidence`
- `src/Controls/Widgets/Primitives.fs` requires `source-contract`
- `src/Controls/Widgets/Primitives.fs` requires `active-feature-evidence`
- `src/Controls/Widgets/Primitives.fsi` requires `source-contract`
- `src/Controls/Widgets/Primitives.fsi` requires `active-feature-evidence`
- `src/Controls/Widgets/TextBoxWidget.fs` requires `source-contract`
- `src/Controls/Widgets/TextBoxWidget.fs` requires `active-feature-evidence`
- `src/Controls/Widgets/TextBoxWidget.fsi` requires `source-contract`
- `src/Controls/Widgets/TextBoxWidget.fsi` requires `active-feature-evidence`
- `tests/Controls.Tests/TypedLoweringTests.fs` requires `test-evidence`
- `tests/Controls.Tests/TypedLoweringTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/TypedControlsAdapterTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/TypedControlsAdapterTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/065-typed-controls-front-door`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.

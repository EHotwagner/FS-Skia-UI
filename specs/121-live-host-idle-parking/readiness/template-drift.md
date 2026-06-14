# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `samples/DemoReel/Program.fs` | `sample-code` |
| `src/Controls.Elmish/ControlsElmish.fs` | `source-code` |
| `src/Controls/RetainedRender.fs` | `source-code` |
| `src/Controls/RetainedRender.fsi` | `source-code` |
| `src/SkiaViewer/Host/OpenGl.fs` | `source-code` |
| `src/SkiaViewer/Host/OpenGl.fsi` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fs` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fsi` | `source-code` |
| `tests/Controls.Tests/Controls.Tests.fsproj` | `test-code` |
| `tests/ControlsPreview.Harness/PreviewRender.fs` | `test-code` |
| `tests/Elmish.Tests/Tests.fs` | `test-code` |
| `tests/SkiaViewer.Tests/Feature063RendererTests.fs` | `test-code` |
| `tests/SkiaViewer.Tests/Feature086SceneTranslateTests.fs` | `test-code` |
| `tests/SkiaViewer.Tests/Feature118PresentModeTests.fs` | `test-code` |
| `tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` | `test-code` |
| `tests/SkiaViewer.Tests/Tests.fs` | `test-code` |
| `tests/Controls.Tests/Feature121IdleTickTests.fs` | `test-code` |
| `tests/SkiaViewer.Tests/Feature121LiveHostPacingTests.fs` | `test-code` |

## Required Alignment Classes

- `samples/DemoReel/Program.fs` requires `sample-contract`
- `samples/DemoReel/Program.fs` requires `active-feature-evidence`
- `src/Controls.Elmish/ControlsElmish.fs` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fs` requires `active-feature-evidence`
- `src/Controls/RetainedRender.fs` requires `source-contract`
- `src/Controls/RetainedRender.fs` requires `active-feature-evidence`
- `src/Controls/RetainedRender.fsi` requires `source-contract`
- `src/Controls/RetainedRender.fsi` requires `active-feature-evidence`
- `src/SkiaViewer/Host/OpenGl.fs` requires `source-contract`
- `src/SkiaViewer/Host/OpenGl.fs` requires `active-feature-evidence`
- `src/SkiaViewer/Host/OpenGl.fsi` requires `source-contract`
- `src/SkiaViewer/Host/OpenGl.fsi` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fs` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fs` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fsi` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fsi` requires `active-feature-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `test-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `active-feature-evidence`
- `tests/ControlsPreview.Harness/PreviewRender.fs` requires `test-evidence`
- `tests/ControlsPreview.Harness/PreviewRender.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Tests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Tests.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Feature063RendererTests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Feature063RendererTests.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Feature086SceneTranslateTests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Feature086SceneTranslateTests.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Feature118PresentModeTests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Feature118PresentModeTests.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` requires `test-evidence`
- `tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Tests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Tests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature121IdleTickTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature121IdleTickTests.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Feature121LiveHostPacingTests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Feature121LiveHostPacingTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, template-drift-docs, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/121-live-host-idle-parking`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.

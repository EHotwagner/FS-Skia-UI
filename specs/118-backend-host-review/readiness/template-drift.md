# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `samples/DemoReel/Program.fs` | `sample-code` |
| `src/SkiaViewer/Host/Diagnostics.fs` | `source-code` |
| `src/SkiaViewer/Host/Diagnostics.fsi` | `source-code` |
| `src/SkiaViewer/Host/Viewer.fs` | `source-code` |
| `src/SkiaViewer/Host/Vulkan.fs` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fs` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fsi` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fsproj` | `source-code` |
| `tests/ControlsPreview.Harness/PreviewRender.fs` | `test-code` |
| `tests/Elmish.Tests/Tests.fs` | `test-code` |
| `tests/SkiaViewer.Tests/Feature063RendererTests.fs` | `test-code` |
| `tests/SkiaViewer.Tests/Feature086SceneTranslateTests.fs` | `test-code` |
| `tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` | `test-code` |
| `tests/SkiaViewer.Tests/Tests.fs` | `test-code` |
| `src/SkiaViewer/PresentMode.fs` | `source-code` |
| `src/SkiaViewer/PresentMode.fsi` | `source-code` |
| `tests/SkiaViewer.Tests/Feature118PresentModeTests.fs` | `test-code` |

## Required Alignment Classes

- `samples/DemoReel/Program.fs` requires `sample-contract`
- `samples/DemoReel/Program.fs` requires `active-feature-evidence`
- `src/SkiaViewer/Host/Diagnostics.fs` requires `source-contract`
- `src/SkiaViewer/Host/Diagnostics.fs` requires `active-feature-evidence`
- `src/SkiaViewer/Host/Diagnostics.fsi` requires `source-contract`
- `src/SkiaViewer/Host/Diagnostics.fsi` requires `active-feature-evidence`
- `src/SkiaViewer/Host/Viewer.fs` requires `source-contract`
- `src/SkiaViewer/Host/Viewer.fs` requires `active-feature-evidence`
- `src/SkiaViewer/Host/Vulkan.fs` requires `source-contract`
- `src/SkiaViewer/Host/Vulkan.fs` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fs` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fs` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fsi` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fsi` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fsproj` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fsproj` requires `active-feature-evidence`
- `tests/ControlsPreview.Harness/PreviewRender.fs` requires `test-evidence`
- `tests/ControlsPreview.Harness/PreviewRender.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Tests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Tests.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Feature063RendererTests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Feature063RendererTests.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Feature086SceneTranslateTests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Feature086SceneTranslateTests.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` requires `test-evidence`
- `tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Tests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Tests.fs` requires `active-feature-evidence`
- `src/SkiaViewer/PresentMode.fs` requires `source-contract`
- `src/SkiaViewer/PresentMode.fs` requires `active-feature-evidence`
- `src/SkiaViewer/PresentMode.fsi` requires `source-contract`
- `src/SkiaViewer/PresentMode.fsi` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Feature118PresentModeTests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Feature118PresentModeTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/118-backend-host-review`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.

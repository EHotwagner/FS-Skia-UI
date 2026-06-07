# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `samples/InteractiveViewer/Program.fs` | `sample-code` |
| `samples/ScreenshotGallery/Program.fs` | `sample-code` |
| `src/Controls.Elmish/ControlsElmish.fs` | `source-code` |
| `src/Controls.Elmish/ControlsElmish.fsi` | `source-code` |
| `src/Controls/Controls.fsproj` | `source-code` |
| `src/SkiaViewer/Host/Diagnostics.fs` | `source-code` |
| `src/SkiaViewer/Host/Diagnostics.fsi` | `source-code` |
| `src/SkiaViewer/Host/Vulkan.fs` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fs` | `source-code` |
| `tests/Controls.Tests/Controls.Tests.fsproj` | `test-code` |
| `tests/Lib.Tests/Tests.fs` | `test-code` |
| `samples/PointerInteractionGallery/PointerInteractionGallery.fsproj` | `sample-code` |
| `samples/PointerInteractionGallery/Program.fs` | `sample-code` |
| `src/Controls/Pointer.fs` | `source-code` |
| `src/Controls/Pointer.fsi` | `source-code` |
| `tests/Controls.Tests/PointerInteractionTests.fs` | `test-code` |

## Required Alignment Classes

- `samples/InteractiveViewer/Program.fs` requires `sample-contract`
- `samples/InteractiveViewer/Program.fs` requires `active-feature-evidence`
- `samples/ScreenshotGallery/Program.fs` requires `sample-contract`
- `samples/ScreenshotGallery/Program.fs` requires `active-feature-evidence`
- `src/Controls.Elmish/ControlsElmish.fs` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fs` requires `active-feature-evidence`
- `src/Controls.Elmish/ControlsElmish.fsi` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fsi` requires `active-feature-evidence`
- `src/Controls/Controls.fsproj` requires `source-contract`
- `src/Controls/Controls.fsproj` requires `active-feature-evidence`
- `src/SkiaViewer/Host/Diagnostics.fs` requires `source-contract`
- `src/SkiaViewer/Host/Diagnostics.fs` requires `active-feature-evidence`
- `src/SkiaViewer/Host/Diagnostics.fsi` requires `source-contract`
- `src/SkiaViewer/Host/Diagnostics.fsi` requires `active-feature-evidence`
- `src/SkiaViewer/Host/Vulkan.fs` requires `source-contract`
- `src/SkiaViewer/Host/Vulkan.fs` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fs` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `test-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `active-feature-evidence`
- `tests/Lib.Tests/Tests.fs` requires `test-evidence`
- `tests/Lib.Tests/Tests.fs` requires `active-feature-evidence`
- `samples/PointerInteractionGallery/PointerInteractionGallery.fsproj` requires `sample-contract`
- `samples/PointerInteractionGallery/PointerInteractionGallery.fsproj` requires `active-feature-evidence`
- `samples/PointerInteractionGallery/Program.fs` requires `sample-contract`
- `samples/PointerInteractionGallery/Program.fs` requires `active-feature-evidence`
- `src/Controls/Pointer.fs` requires `source-contract`
- `src/Controls/Pointer.fs` requires `active-feature-evidence`
- `src/Controls/Pointer.fsi` requires `source-contract`
- `src/Controls/Pointer.fsi` requires `active-feature-evidence`
- `tests/Controls.Tests/PointerInteractionTests.fs` requires `test-evidence`
- `tests/Controls.Tests/PointerInteractionTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, template-drift-docs, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/075-mouse-input-events`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.

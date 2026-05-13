# Starting Repository Inventory

Feature: `002-skia-feature-parity`  
Captured: 2026-05-13

## Projects

| Path | Role |
|------|------|
| `src/Lib/Lib.fsproj` | Existing core `FS.Skia.UI` library |
| `tests/Lib.Tests/Lib.Tests.fsproj` | Existing core library tests |
| `samples/BasicViewer/BasicViewer.fsproj` | Existing basic viewer sample |
| `samples/InteractiveViewer/InteractiveViewer.fsproj` | Existing interactive viewer sample |

## Current Public Surface Files

| Path | Notes |
|------|-------|
| `src/Lib/Library.fsi` | Existing core signature with size/color/configuration, diagnostics, minimal scene constructors, screenshots, viewer program/effects, and run helpers |

## Current Implementation Files

| Path | Notes |
|------|-------|
| `src/Lib/Library.fs` | Existing Vulkan/Skia implementation and public module bodies |
| `scripts/prelude.fsx` | Existing core prelude workflow |
| `scripts/us1-vulkan-smoke.sh` | Existing Vulkan smoke helper from previous feature |
| `tests/Lib.Tests/Tests.fs` | Existing public surface, MVU transition, diagnostics, and sample/package-style tests |
| `tests/Lib.Tests/Program.fs` | Existing test entry point |

## Current Samples

| Path | Notes |
|------|-------|
| `samples/BasicViewer/Program.fs` | Existing basic Vulkan viewer sample |
| `samples/InteractiveViewer/Program.fs` | Existing interactive viewer sample |

## Gaps Against Current Feature Plan

- `src/Charts` and `src/Layout` do not exist yet.
- Planned test projects `Charts.Tests`, `Layout.Tests`, `Parity.Tests`, `Package.Tests`, and `Smoke.Tests` do not exist yet.
- Planned parity samples `ParityGallery`, `EffectsGallery`, `ChartsGallery`, `DataGridGallery`, `LayoutGraphGallery`, `ScreenshotGallery`, and `DemoReel` do not exist yet.
- Only the core package has an `.fsi` surface today.
- Readiness scaffolding for this feature existed only as task graph files before setup.

## Capture Commands

```bash
rg --files
find . -maxdepth 3 -type f \( -name '*.fsproj' -o -name '*.sln' -o -name '*.fs' -o -name '*.fsi' -o -name '*.fsx' \) | sort
find . -maxdepth 4 -type f -path '*readiness*' | sort
```

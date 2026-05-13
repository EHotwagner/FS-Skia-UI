# Pinned Baseline Capability Inventory

Feature: `002-skia-feature-parity`  
Inspected repository: `https://github.com/EHotwagner/SkiaViewer.git`  
Pinned commit: `7aac43dd12903f93004d0c2bf7c6254318a366dc`  
Commit date: `2026-04-13 11:09:47 +0000`  
Commit subject: `feat(charts): add configurable text colors to DataGrid`

## Verification

Commands run from this repository on 2026-05-13:

```bash
git ls-remote https://github.com/EHotwagner/SkiaViewer.git HEAD refs/heads/* refs/tags/*
git clone --depth 1 https://github.com/EHotwagner/SkiaViewer.git /tmp/SkiaViewer-baseline
git -C /tmp/SkiaViewer-baseline rev-parse HEAD
git -C /tmp/SkiaViewer-baseline log -1 --format='%H%n%ci%n%s'
find /tmp/SkiaViewer-baseline -maxdepth 3 -type f
```

The upstream `HEAD` and `refs/heads/master` both resolved to the pinned commit.

## Capability Areas Observed

| Area | Baseline evidence | Capabilities to map into FS-Skia-UI |
|------|-------------------|--------------------------------------|
| Core viewer | `src/SkiaViewer/Viewer.fsi`, `src/SkiaViewer/VulkanBackend.fs`, README quick start | Windowed viewer, scene stream input, input stream output, background lifetime, screenshot capture, Vulkan startup, GL fallback behavior to adapt/exclude |
| Declarative scene DSL | `src/SkiaViewer/Scene.fsi`, `docs/declarative-scene-dsl.fsx`, `docs/drawing-primitives.fsx` | Scene creation, grouping, primitives, text, images, paths, transforms, clipping, paint helpers, reusable scene elements |
| Rendering and Skia translation | `src/SkiaViewer/SceneRenderer.fsi`, `src/SkiaViewer/CachedRenderer.fsi` | Primitive rendering, frame recovery, caching/diff behavior, Skia paint/shader/filter/path effect support |
| Shaders and effects | `docs/shaders-and-effects.fsx`, `scripts/examples/04-effects-showcase.fsx` | Radial/sweep/conical gradients, Perlin noise, SkSL shader usage, color/mask/image filters, blend modes, drop shadows/glow-style effects |
| Screenshots | `docs/screenshots.fsx`, `scripts/examples/01-screenshot.fsx` | PNG/JPEG screenshot workflow, output path handling, rendered-frame capture evidence |
| Performance evidence | `tests/SkiaViewer.PerfTests/*`, `scripts/examples/03-perf-suite.fsx` | Render performance scenarios, large-scene generators, metrics/report output |
| Charts | `src/SkiaViewer.Charts/*.fsi`, chart tests, README chart section | Line, bar, pie/donut, scatter, area, histogram, candlestick, radar, axes, legends, palettes, scale helpers |
| DataGrid | `src/SkiaViewer.Charts/DataGrid.fsi`, `docs/datagrid.fsx`, DataGrid tests | Columns, typed cell values, sorting, scrolling/viewport behavior, fixed headers, configurable colors/text formatting |
| Layout | `src/SkiaViewer.Layout/Layout.fsi`, `Defaults.fsi`, layout tests | HStack, VStack, Dock, sizing, spacing, padding, alignment, nested layout composition |
| Graphs | `src/SkiaViewer.Layout/Graph.fsi`, `GraphValidation.fsi`, graph tests | Directed DAG rendering, undirected weighted graph rendering, cycle detection, duplicate/missing endpoint validation, graph layout helpers |
| Examples and demos | `scripts/examples/*.fsx`, README demo reel scene list | Screenshot, declarative scene, charts gallery, performance suite, DataGrid, effects showcase, layouts, graphs, layout graph window, demo reel, render reel |
| Documentation | `docs/*.fsx`, `docs/index.md` | Getting started, architecture, drawing, shaders/effects, input, screenshots, charting, DataGrid, tests, known issues |

## Adaptation Notes

- Baseline advertises Vulkan GPU backend with GL raster fallback. This feature is constrained to Vulkan-only; fallback behavior must be documented as excluded or adapted, not reintroduced.
- Baseline public integration uses `IObservable<Scene>` and `IObservable<InputEvent>`. This feature is constrained to Elmish-only public flow; reactive-stream APIs must be mapped to Elmish `Model` / `Msg` / `Effect` evidence instead of copied.
- Baseline package names are `SkiaViewer`, `SkiaViewer.Charts`, and `SkiaViewer.Layout`. This feature targets independently referenceable packages `FS.Skia.UI`, `FS.Skia.UI.Charts`, and `FS.Skia.UI.Layout`.

# Skia Feature Parity Matrix

Pinned baseline: `EHotwagner/SkiaViewer` commit `7aac43dd12903f93004d0c2bf7c6254318a366dc`.

| Baseline area | FS-Skia-UI status | Evidence | Adaptation |
|---------------|-------------------|----------|------------|
| Core viewer | Adapted | `readiness/smoke/t061-interactiveviewer-contract.txt` | Observable scene/input streams are represented as Elmish `Model`, `Msg`, and `ViewerEffect`. |
| Declarative scene DSL | Supported | `readiness/logs/t026-lib-tests.txt` | Immutable scene constructors cover primitives, grouping, text, images, paths, clipping, transforms, and pictures. |
| Rendering and Skia translation | Supported | `readiness/screenshots/us1-render-readback.txt` | Vulkan/Skia rendering remains the only renderer path. |
| Shaders and effects | Adapted | `readiness/smoke/t029-effectsgallery-contract.txt` | Unsupported device capabilities are reported as diagnostics instead of fallback rendering. |
| Screenshots | Supported | `readiness/smoke/t061-screenshotgallery-contract.txt` | Screenshot capture is requested through Elmish effects and interpreted at the viewer edge. |
| Performance evidence | Supported | `readiness/logs/final-pass-dotnet-test.txt` | Scale tests and smoke logs cover chart, DataGrid, graph, and viewer paths. |
| Charts | Supported | `readiness/logs/t038-charts-tests-rerun.txt` | Charts are pure view-layer scene builders. |
| DataGrid | Supported | `readiness/logs/t038-charts-tests-rerun.txt` | Sort and viewport state are owned by the consumer model. |
| Layout | Supported | `readiness/logs/t049-layout-tests-rerun.txt` | Layout helpers return pure scene composition. |
| Graphs | Supported | `readiness/logs/t049-layout-tests-rerun.txt` | Validation and layout are deterministic pure helpers. |
| Examples and demos | Supported | `readiness/smoke/` | Samples cover BasicViewer, InteractiveViewer, parity/effects/charts/grid/layout/screenshot galleries, and DemoReel. |
| Documentation | Adapted | `quickstart.md`, this matrix | Package and sample commands are documented for the three FS.Skia.UI packages. |

## Excluded Or Adapted Baseline Behaviors

- Vulkan-only: this project does not provide a GL or raster fallback renderer. Baseline fallback renderer behavior is excluded by feature constraint and replaced by structured startup diagnostics.
- Elmish-only: baseline reactive stream integration is adapted to Elmish `init`, `update`, `view`, event mapping, and effect interpretation.
- Package names: baseline `SkiaViewer*` packages map to `FS.Skia.UI`, `FS.Skia.UI.Charts`, and `FS.Skia.UI.Layout`.

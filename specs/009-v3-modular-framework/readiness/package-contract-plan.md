# Package Contract Plan

Date: 2026-05-16

| Capability | Package/project | Public contract | Workflow boundary |
|------------|-----------------|-----------------|-------------------|
| Scene | `FS.Skia.UI.Scene`, `src/Scene/Scene.fsproj` | `src/Scene/Scene.fsi` | Pure scene data and helpers; Principle IV not applicable except consumers. |
| SkiaViewer | `FS.Skia.UI.SkiaViewer`, `src/SkiaViewer/SkiaViewer.fsproj` | `src/SkiaViewer/SkiaViewer.fsi` | `ViewerModel`, `ViewerMsg`, `ViewerEffect`, `Viewer.init`, and pure `Viewer.update`; native I/O belongs at interpreter edge. |
| Elmish | `FS.Skia.UI.Elmish`, `src/Elmish/Elmish.fsproj` | `src/Elmish/Elmish.fsi` | `ElmishAdapterModel`, `ElmishAdapterMsg`, `ElmishAdapterEffect`, `init`, and pure `update`. |
| KeyboardInput | `FS.Skia.UI.KeyboardInput`, `src/KeyboardInput/KeyboardInput.fsproj` | `src/KeyboardInput/KeyboardInput.fsi` | `KeyboardModel`, `KeyboardMsg`, `KeyboardEffect`, `Keyboard.init`, and pure `Keyboard.update`. |
| Layout | `FS.Skia.UI.Layout`, `src/Layout/Layout.fsproj` | `src/Layout/*.fsi` | Pure layout calculation surface; Yoga remains implementation dependency. |
| Charts | `FS.Skia.UI.Charts`, `src/Charts/Charts.fsproj` | `src/Charts/*.fsi` | Pure chart/DataGrid scene-builder surface. |
| Testing | `FS.Skia.UI.Testing`, `src/Testing/Testing.fsproj` | `src/Testing/Testing.fsi` | Pure generated-product validation helper surface. |

## Compile Order

1. Scene
2. SkiaViewer, KeyboardInput, Layout, Charts, Testing
3. Elmish
4. Capability tests
5. Governance and package tests

## Compatibility Note

The existing broad `FS.Skia.UI` package remains in the repository during this
stage so current samples and tests continue to build while V3 capability
packages become independently reviewable.

# Capability Catalog

PASS: capability catalog metadata, dependency closure, default app set, contracts, tests, skills, fragments, evidence, and surface baselines are valid.

| Capability | Package | Project | Dependencies | Default app |
|------------|---------|---------|--------------|-------------|
| Scene | `FS.Skia.UI.Scene` | `src/Scene/Scene.fsproj` | (none) | True |
| SkiaViewer | `FS.Skia.UI.SkiaViewer` | `src/SkiaViewer/SkiaViewer.fsproj` | scene | True |
| Elmish | `FS.Skia.UI.Elmish` | `src/Elmish/Elmish.fsproj` | scene, skiaviewer | True |
| KeyboardInput | `FS.Skia.UI.KeyboardInput` | `src/KeyboardInput/KeyboardInput.fsproj` | scene | True |
| Layout | `FS.Skia.UI.Layout` | `src/Layout/Layout.fsproj` | scene | True |
| Charts | `FS.Skia.UI.Charts` | `src/Charts/Charts.fsproj` | scene | True |
| Testing | `FS.Skia.UI.Testing` | `src/Testing/Testing.fsproj` | scene | False |
| Samples | `non-runtime` | `non-runtime` | scene, skiaviewer, elmish | False |

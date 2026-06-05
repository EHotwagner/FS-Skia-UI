# FS.Skia.UI.Build

Compiled-F# governance engine (evidence graph + merge-gate audit) consumed in-process by FS.Skia.UI build tooling and generated products.

`FS.Skia.UI.Build` is one of the **FS.Skia.UI** distribution packages — an F# / Elmish UI and 2D
scene-graph framework for .NET 10 desktop, rendered through Vulkan + SkiaSharp.

## Install

```bash
dotnet add package FS.Skia.UI.Build
```

Or scaffold a full governed project that wires the FS.Skia.UI packages together:

```bash
dotnet new install FS.Skia.UI.Template
dotnet new fs-skia-ui -o MyApp
```

## Versioning

All `FS.Skia.UI.*` libraries share one version and move together. In a generated project a
single `<FsSkiaUiVersion>` in `Directory.Packages.props` pins every package — upgrading is one
edit; see `docs/UPGRADING.md`. Pre-release versions use a `-preview.N` suffix.

## Links

- Repository & issues: https://github.com/FS-Skia-UI/FS-Skia-UI
- License: MIT

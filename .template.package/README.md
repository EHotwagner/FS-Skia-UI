# FS.Skia.UI.Template

The `dotnet new` project template for **FS.Skia.UI** — an F# / Elmish UI and 2D scene-graph
framework for .NET 10 desktop, rendered through Vulkan + SkiaSharp.

## Install & scaffold

```bash
dotnet new install FS.Skia.UI.Template
dotnet new fs-skia-ui -o MyApp        # profiles: app, headless-scene, governed, sample-pack
cd MyApp
dotnet restore                        # resolves FS.Skia.UI.* from nuget.org only
dotnet build
dotnet test
```

The generated project restores entirely from the **public nuget.org feed** — no machine-local
path — so it works on any machine without a repository checkout.

## Single-source versioning

Every generated project pins all `FS.Skia.UI.*` packages **and** the in-process build engine to
one `<FsSkiaUiVersion>` value in `Directory.Packages.props`. Upgrading is a single edit + `dotnet
restore`; see the generated `docs/UPGRADING.md`. Preview vs stable is explicit in the value
(`-preview.N` ⇒ preview channel).

## Links

- Repository & issues: https://github.com/FS-Skia-UI/FS-Skia-UI
- License: MIT

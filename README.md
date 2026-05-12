# FS.Skia.UI

Elmish-first F# viewer library for declarative SkiaSharp scenes rendered through a Vulkan-only desktop path.

## Requirements

- .NET SDK with `net10.0` support.
- Windows or Linux desktop.
- Vulkan-capable GPU, driver, and presentation surface.
- NuGet access for SkiaSharp 4 preview and Silk.NET dependencies.

macOS, mobile, browser, and headless production targets are out of scope for this first version. The public API does not expose renderer selection and does not provide an OpenGL, CPU, software, or fallback renderer.

## Build And Test

```bash
dotnet restore
dotnet build
dotnet test
```

## Run The Basic Viewer Smoke Test

```bash
scripts/us1-vulkan-smoke.sh specs/001-vulkan-elmish-viewer/readiness/us1-vulkan-smoke.txt
```

Successful output records `renderer=Vulkan`, `fallback-used=false`, startup timing, and first-frame timing.

## Samples

Run the examples from source:

```bash
dotnet run --project samples/BasicViewer/BasicViewer.fsproj
dotnet run --project samples/InteractiveViewer/InteractiveViewer.fsproj
```

Run their public-API contract smoke paths without opening a live window:

```bash
dotnet run --project samples/BasicViewer/BasicViewer.fsproj -- --contract-smoke
dotnet run --project samples/InteractiveViewer/InteractiveViewer.fsproj -- --contract-smoke
```

After packing, verify the samples against the NuGet package surface:

```bash
dotnet pack src/Lib/Lib.fsproj -c Release -o specs/001-vulkan-elmish-viewer/readiness/package
dotnet run --project samples/BasicViewer/BasicViewer.fsproj -p:UsePackedPackage=true -p:RestoreAdditionalProjectSources=/absolute/path/to/readiness/package -- --contract-smoke
dotnet run --project samples/InteractiveViewer/InteractiveViewer.fsproj -p:UsePackedPackage=true -p:RestoreAdditionalProjectSources=/absolute/path/to/readiness/package -- --contract-smoke
```

The samples use the Elmish public API for configuration, scene composition, input state, subscriptions, diagnostics, and screenshot effects. `BasicViewer` demonstrates shapes, text, image placeholders, chart data, and PNG screenshot requests. `InteractiveViewer` demonstrates keyboard and pointer state, timer-style updates, diagnostics, and JPEG screenshot requests.

If a local validation package uses a unique version, pass
`-p:FsSkiaUiPackageVersion=<version>` with `-p:UsePackedPackage=true`.

Screenshot smoke can write PNG and JPEG artifacts from the last successful Vulkan/Skia frame:

```bash
dotnet run --project samples/BasicViewer/BasicViewer.fsproj -- --screenshot-smoke --output=specs/001-vulkan-elmish-viewer/readiness/screenshots/basic-viewer.png
dotnet run --project samples/BasicViewer/BasicViewer.fsproj -- --screenshot-smoke --jpeg --output=specs/001-vulkan-elmish-viewer/readiness/screenshots/basic-viewer.jpg
```

## Package Compatibility

This is a first-version package. There is no previous public FS.Skia.UI API to migrate from. Package consumers should treat the SkiaSharp 4 preview dependency as preview-risk: Vulkan driver behavior, native assets, and package shape may change before SkiaSharp 4 reaches stable release.

## Unsupported Environment Diagnostics

Startup validates the supported OS, presentation surface, Vulkan instance, physical device, swapchain, and Skia Vulkan context before rendering. Failures return `Result<unit, RenderDiagnostic>` from `Viewer.run` and include a diagnostic stage such as `PlatformCheck`, `VulkanInstance`, `VulkanDevice`, `VulkanSurface`, `VulkanSwapchain`, or `SkiaContext`.

Expected failure output identifies Vulkan initialization and states that no fallback renderer is used.

Screenshot capture is available through `ViewerEffect.CaptureScreenshot` and writes PNG or JPEG from the last successful Vulkan/Skia frame. A capture requested before any successful frame returns a `ScreenshotCapture` diagnostic.

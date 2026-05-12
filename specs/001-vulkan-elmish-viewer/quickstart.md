# Quickstart: Vulkan Elmish Viewer

## Prerequisites

- .NET SDK with `net10.0` support.
- Windows or Linux desktop.
- Vulkan-capable GPU, driver, and presentation surface for runtime smoke tests.
- NuGet access for preview package restore.

## Restore and Build

```bash
dotnet restore
dotnet build
```

## Run Tests

```bash
dotnet test
```

Expected coverage:

- Public `.fsi` surface compiles.
- Pure Elmish `update` behavior is tested without Vulkan I/O.
- Diagnostics tests verify unsupported Vulkan initialization fails clearly without fallback.

## Pack Library

```bash
dotnet pack src/Lib/Lib.fsproj -c Release -o ~/.local/share/nuget-local
```

## FSI Contract Check

```bash
dotnet fsi
```

Then load the project prelude:

```fsharp
#load "scripts/prelude.fsx"
```

The prelude should allow constructing viewer configuration, scenes, and a minimal Elmish viewer program through the public package surface.

## Run Samples

Basic scene sample:

```bash
dotnet run --project samples/BasicViewer/BasicViewer.fsproj
```

Basic public-API contract smoke:

```bash
dotnet run --project samples/BasicViewer/BasicViewer.fsproj -- --contract-smoke
```

US1 Vulkan-only validation:

```bash
scripts/us1-vulkan-smoke.sh specs/001-vulkan-elmish-viewer/readiness/us1-vulkan-smoke.txt
```

The smoke output records `renderer=Vulkan`, `fallback-used=false`, startup
timing, and first-frame timing. For frame failure diagnostics, run:

```bash
dotnet run --project samples/BasicViewer/BasicViewer.fsproj -- --smoke --fail-frame
```

Expected frame-failure output includes `diagnostic-stage=FrameRender`,
`renderer=Vulkan`, and `fallback-used=false`.

Screenshot smoke:

```bash
dotnet run --project samples/BasicViewer/BasicViewer.fsproj -- --screenshot-smoke --output=specs/001-vulkan-elmish-viewer/readiness/screenshots/basic-viewer.png
dotnet run --project samples/BasicViewer/BasicViewer.fsproj -- --screenshot-smoke --jpeg --output=specs/001-vulkan-elmish-viewer/readiness/screenshots/basic-viewer.jpg
```

Interactive sample:

```bash
dotnet run --project samples/InteractiveViewer/InteractiveViewer.fsproj
```

Interactive public-API contract smoke:

```bash
dotnet run --project samples/InteractiveViewer/InteractiveViewer.fsproj -- --contract-smoke
```

US3 controlled Elmish-flow smoke:

```bash
dotnet run --project samples/InteractiveViewer/InteractiveViewer.fsproj -- --smoke --seconds=60
```

This records input-driven model changes, an input latency summary, and 240
timer ticks for a 60-second subscription horizon. This command is suitable
for automated readiness capture, but it is synthetic because it does not
open a live Vulkan window or wait 60 wall-clock seconds.

US3 real interactive validation:

```bash
dotnet run --project samples/InteractiveViewer/InteractiveViewer.fsproj
```

On a supported Vulkan workstation, press a key, click or move the pointer,
and leave the sample running for at least 60 wall-clock seconds. Capture the
log, screenshot, or transcript under
`specs/001-vulkan-elmish-viewer/readiness/us3-interactive-smoke.txt`.

Expected behavior:

- On supported Vulkan systems, a visible window renders through Vulkan only.
- Keyboard or pointer input causes a visible model-driven scene change.
- Subscription-driven sample updates continue for at least 60 seconds.
- On unsupported systems, startup fails before rendering and reports a Vulkan-specific diagnostic.

## Verify Samples Against The Packed Package

```bash
dotnet pack src/Lib/Lib.fsproj -c Release -o specs/001-vulkan-elmish-viewer/readiness/package
dotnet run --project samples/BasicViewer/BasicViewer.fsproj -p:UsePackedPackage=true -p:RestoreAdditionalProjectSources=/home/developer/projects/FS-Skia-UI/specs/001-vulkan-elmish-viewer/readiness/package -- --contract-smoke
dotnet run --project samples/InteractiveViewer/InteractiveViewer.fsproj -p:UsePackedPackage=true -p:RestoreAdditionalProjectSources=/home/developer/projects/FS-Skia-UI/specs/001-vulkan-elmish-viewer/readiness/package -- --contract-smoke
```

The packed-package smoke paths verify that sample code consumes only the
published public API. Use a fresh `RestorePackagesPath` when validating a
new local package with the same preview version, or set
`FsSkiaUiPackageVersion` when validating a uniquely versioned local package.

## Screenshot Capture

Samples request screenshots through `ViewerEffect.CaptureScreenshot`.
`BasicViewer` requests PNG output and `InteractiveViewer` requests JPEG
output. The host writes screenshots from the last successful Vulkan/Skia
frame. If capture is requested before the first successful frame, the host
reports a `ScreenshotCapture` diagnostic.

## Compatibility And Migration

FS.Skia.UI is a first-version package with no older public API migration
path. Consumers should pin the package version and treat the SkiaSharp 4
preview dependency as preview-risk until that dependency stabilizes.

## Runtime Environment Notes

- The first version supports Windows and Linux desktop only.
- macOS, mobile, browser, and headless production targets are out of scope.
- No fallback renderer is available or acceptable.
- Startup validates the OS, presentation surface, Vulkan instance, physical
  device, swapchain, and Skia Vulkan context before rendering.
- Unsupported environments return `Result.Error` from `Viewer.run` with a
  `RenderDiagnostic` stage such as `PlatformCheck`, `VulkanInstance`,
  `VulkanDevice`, `VulkanSurface`, `VulkanSwapchain`, or `SkiaContext`.
- Failure diagnostics identify Vulkan initialization and must not suggest
  switching to OpenGL, CPU, software, or fallback rendering.

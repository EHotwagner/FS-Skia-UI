# FS.Skia.UI

FS.Skia.UI is an experimental F# desktop UI toolkit and governed project
template for building SkiaSharp-rendered applications with an Elmish/MVU
application model.

Application code owns the model, messages, update function, and view function.
The view function returns immutable `Scene` values. `Viewer.run` owns the host
edge: window creation, input events, Vulkan/Skia setup, frame rendering,
screenshots, diagnostics, and shutdown.

The repository currently contains:

- `FS.Skia.UI`: core scene primitives, paint/path/text/image declarations,
  viewer runtime contracts, structured diagnostics, screenshot effects, and
  keyboard input helpers.
- `FS.Skia.UI.Charts`: pure chart and DataGrid builders that return core
  `Scene` values.
- `FS.Skia.UI.Layout`: Yoga-backed layout, graph validation, graph layout,
  rendering, and hit testing.
- Runnable sample applications with non-visual contract smoke modes.
- A governed `dotnet new fs-skia-ui` template plus build, dependency,
  documentation, and evidence workflows.

It is not a general renderer abstraction, a browser/mobile UI framework, or a
traditional retained widget toolkit that owns application state. The current
runtime host is deliberately narrow: desktop, SkiaSharp, Silk.NET, and Vulkan.

## Minimal App Shape

```fsharp
open Elmish
open FS.Skia.UI

type Model = { Title: string }

type Msg =
    | NoOp

let init () =
    { Title = "Hello FS.Skia.UI" }, Cmd.none

let update msg model =
    match msg with
    | NoOp -> model, Cmd.none

let view model =
    Scene.group [
        Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgba 18uy 24uy 32uy 255uy)
        Scene.text (48.0, 88.0) model.Title Colors.white
    ]

let config =
    Viewer.defaultConfiguration "Hello" { Width = 640; Height = 480 }

let program =
    Viewer.create config init update view

[<EntryPoint>]
let main _ =
    match Viewer.run program with
    | Ok() -> 0
    | Error diagnostic ->
        eprintfn "%s" diagnostic.Message
        1
```

Higher-level packages follow the same rule: they produce data, diagnostics,
hit-test results, or `Scene` output. They do not create windows or take over the
application loop.

## Current Runtime Boundary

The live desktop host currently requires:

- .NET SDK with `net10.0` support.
- Windows or Linux desktop.
- Vulkan-capable GPU, driver, and presentation surface.
- NuGet access for SkiaSharp 4 preview and Silk.NET dependencies.

macOS, mobile, browser, and headless production targets are out of scope for
this first version. The public API does not expose renderer selection and does
not provide an OpenGL, CPU, software, or fallback renderer.

## Build And Test

```bash
./fake.sh build -t Dev
./fake.sh build -t Verify
./fake.sh build -t TemplateCheck
./fake.sh build -t DependencyReport
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateDrift
```

Use `fake.cmd build -t Dev` or `fake.cmd build -t Verify` from Windows command
prompts. See [docs/build.md](docs/build.md), [docs/testing.md](docs/testing.md),
and [docs/evidence.md](docs/evidence.md) for target responsibilities, evidence
paths, and deferred roadmap items.

## Technical Design

Start with [docs/technical-design.md](docs/technical-design.md) for the
architecture overview, runtime design, subsystem design, design decisions, and
links to the operational governance documents.

## Project Template

Install the governed template from the source directory:

```bash
dotnet new install .
dotnet new fs-skia-ui --name MyProduct --profile default
dotnet new fs-skia-ui --name MyProduct.Minimal --profile minimal
```

Maintainers validate source and packaged template paths with:

```bash
./fake.sh build -t TemplateCheck
```

See [docs/template-profile.md](docs/template-profile.md),
[docs/dependencies.md](docs/dependencies.md), and
[docs/speckit.md](docs/speckit.md).

## Run The Basic Viewer Smoke Test

```bash
scripts/us1-vulkan-smoke.sh specs/001-vulkan-elmish-viewer/readiness/us1-vulkan-smoke.txt
```

Successful output records `renderer=Vulkan`, `fallback-used=false`, startup timing, and first-frame timing.

## Samples

The repository includes these runnable samples:

| Sample | Project | Focus |
|--------|---------|-------|
| `BasicViewer` | `samples/BasicViewer/BasicViewer.fsproj` | Core scene composition, chart placeholder, image placeholder, diagnostics, and screenshot requests. |
| `ChartsGallery` | `samples/ChartsGallery/ChartsGallery.fsproj` | Line, bar, scatter, area, pie/donut, and histogram chart widgets. |
| `DataGridGallery` | `samples/DataGridGallery/DataGridGallery.fsproj` | Data grid sorting, viewport state, fixed headers, and hit testing. |
| `DemoReel` | `samples/DemoReel/DemoReel.fsproj` | Animated combined showcase for geometry, shaders, layout, charts, data grid, graphs, and effects. |
| `EffectsGallery` | `samples/EffectsGallery/EffectsGallery.fsproj` | Paint effects, gradients, path effects, blend modes, clipping, perspective, and color spaces. |
| `InteractiveViewer` | `samples/InteractiveViewer/InteractiveViewer.fsproj` | Keyboard and pointer state, timer-style updates, diagnostics, and JPEG screenshot requests. |
| `KeyboardInputGallery` | `samples/KeyboardInputGallery/KeyboardInputGallery.fsproj` | Keyboard input layouts, command resolution, and keyboard state display. |
| `LayoutGraphGallery` | `samples/LayoutGraphGallery/LayoutGraphGallery.fsproj` | Automatic layout, graph rendering, chart/data-grid composition, validation, and hit testing. |
| `ParityGallery` | `samples/ParityGallery/ParityGallery.fsproj` | Skia feature parity coverage for shapes, paths, vertices, clips, regions, pictures, images, and charts. |
| `ScreenshotGallery` | `samples/ScreenshotGallery/ScreenshotGallery.fsproj` | Screenshot effects, render effects, recoverable diagnostics, and shutdown effects. |

Run any sample from source with its project path:

```bash
dotnet run --project samples/BasicViewer/BasicViewer.fsproj
dotnet run --project samples/DemoReel/DemoReel.fsproj
```

Every current sample exposes a public-API contract smoke path that does not
open a live window:

```bash
for project in \
  samples/BasicViewer/BasicViewer.fsproj \
  samples/ChartsGallery/ChartsGallery.fsproj \
  samples/DataGridGallery/DataGridGallery.fsproj \
  samples/DemoReel/DemoReel.fsproj \
  samples/EffectsGallery/EffectsGallery.fsproj \
  samples/InteractiveViewer/InteractiveViewer.fsproj \
  samples/KeyboardInputGallery/KeyboardInputGallery.fsproj \
  samples/LayoutGraphGallery/LayoutGraphGallery.fsproj \
  samples/ParityGallery/ParityGallery.fsproj \
  samples/ScreenshotGallery/ScreenshotGallery.fsproj
do
  dotnet run --project "$project" -- --contract-smoke
done
```

After packing, verify package-aware samples against the NuGet package surface:

```bash
PACKAGE_DIR="$(pwd)/specs/001-vulkan-elmish-viewer/readiness/package"
dotnet pack src/Lib/Lib.fsproj -c Release -o "$PACKAGE_DIR"

for project in \
  samples/BasicViewer/BasicViewer.fsproj \
  samples/EffectsGallery/EffectsGallery.fsproj \
  samples/InteractiveViewer/InteractiveViewer.fsproj \
  samples/ParityGallery/ParityGallery.fsproj \
  samples/ScreenshotGallery/ScreenshotGallery.fsproj
do
  dotnet run --project "$project" \
    -p:UsePackedPackage=true \
    -p:RestoreAdditionalProjectSources="$PACKAGE_DIR" \
    -- --contract-smoke
done
```

The samples use the Elmish public API for configuration, scene composition,
input state, subscriptions, diagnostics, layout, charting, graph/data-grid
widgets, keyboard input, and screenshot effects.

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

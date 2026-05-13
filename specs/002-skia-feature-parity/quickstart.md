# Quickstart: Skia Feature Parity

This quickstart defines the expected developer and verification workflow for the parity feature.

## Prerequisites

- .NET SDK with `net10.0` support.
- Windows or Linux desktop with Vulkan-capable GPU, driver, and presentation surface for positive smoke tests.
- A headless or Vulkan-disabled environment, or a controlled failure mode, for startup diagnostics tests.
- Local NuGet package output path: `~/.local/share/nuget-local/`.

## Restore and Build

```bash
dotnet restore
dotnet build
```

## FSI / Prelude Contract Checks

The implementation must provide prelude scripts that exercise public package surfaces through packed or referenced libraries:

```bash
dotnet fsi scripts/prelude.fsx
dotnet fsi scripts/charts-prelude.fsx
dotnet fsi scripts/layout-prelude.fsx
```

Expected result:

- Core scene/viewer constructors are usable from FSI.
- Chart and DataGrid components are usable as pure view-layer scene builders.
- Layout and graph components are usable as pure view-layer scene builders.
- No private helper APIs are required by the examples.

## Run Tests

```bash
dotnet test
```

Expected evidence:

- Public surface baseline tests pass for all public modules.
- Semantic tests cover pure `update` paths and view projection helpers.
- Chart tests cover empty, small, and 100,000-point datasets.
- DataGrid tests cover empty, small, and 10,000-row datasets.
- Layout tests cover resize behavior and nested layouts.
- Graph tests cover validation, a 100-node DAG, and a 50-node weighted undirected graph.
- Screenshot tests write and reload PNG and JPEG output.
- Vulkan-unavailable tests fail fast with structured diagnostics.

## Run Samples

```bash
dotnet run --project samples/BasicViewer/BasicViewer.fsproj
dotnet run --project samples/InteractiveViewer/InteractiveViewer.fsproj
dotnet run --project samples/ParityGallery/ParityGallery.fsproj
dotnet run --project samples/EffectsGallery/EffectsGallery.fsproj
dotnet run --project samples/ChartsGallery/ChartsGallery.fsproj
dotnet run --project samples/DataGridGallery/DataGridGallery.fsproj
dotnet run --project samples/LayoutGraphGallery/LayoutGraphGallery.fsproj
dotnet run --project samples/ScreenshotGallery/ScreenshotGallery.fsproj
dotnet run --project samples/DemoReel/DemoReel.fsproj
```

Expected result:

- Viewer starts through the Vulkan path only.
- The first frame appears within the configured performance target on a supported workstation.
- Input-driven samples visibly respond within 1 second.
- Galleries demonstrate drawing, effects, charts, data grid, layout, graph, screenshots, and demo reel behavior.
- No sample exposes fallback-renderer selection.

## Package Verification

```bash
dotnet pack src/Lib/Lib.fsproj --output ~/.local/share/nuget-local/
dotnet pack src/Charts/Charts.fsproj --output ~/.local/share/nuget-local/
dotnet pack src/Layout/Layout.fsproj --output ~/.local/share/nuget-local/
```

Expected result:

- `FS.Skia.UI` package is produced.
- `FS.Skia.UI.Charts` package is produced.
- `FS.Skia.UI.Layout` package is produced.
- A clean consumer can reference packages independently.

## Parity Evidence Report

```bash
dotnet fsi scripts/parity-evidence.fsx
```

Expected output:

- `readiness/parity-evidence.json`
- One record per pinned-baseline capability.
- Every non-conflicting capability is `Supported` or `Adapted`.
- `NotYetSupported` does not appear for merge-ready non-conflicting capabilities.
- Manual visual review appears only where deterministic graphics comparison is impractical.

## Elmish View Component Pattern

Consumers should keep domain state in `Model`, derive view props with pure functions, and call components from `view`:

```fsharp
let chartProps model =
    model.Sales
    |> SalesProjection.monthlySeries model.SelectedPoint model.VisibleRange

let gridProps model =
    OrdersProjection.rows model.GridSort model.GridScroll model.Orders

let graphProps model =
    DependencyProjection.graph model.GraphFocus model.Dependencies

let view model =
    Scene.group [
        Layout.dock Defaults.dockConfig [
            Charts.LineChart.lineChart (chartProps model)
            Charts.DataGrid.dataGrid (gridProps model)
            Layout.Graph.graph (graphProps model)
        ]
    ]
```

The component packages do not own selection, sort, scroll, zoom, or focus state. Those values live in the Elmish model and change only through messages handled by `update`.

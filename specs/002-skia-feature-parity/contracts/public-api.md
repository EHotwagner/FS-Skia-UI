# Public API Contract: Skia Feature Parity

This contract describes the planned `.fsi` surface. Exact names may be refined during FSI-first implementation, but the boundaries and responsibilities are fixed by the spec and plan.

## Package: FS.Skia.UI

Core viewer, scene DSL, Skia feature coverage, diagnostics, screenshots, and Vulkan host.

### Module: FS.Skia.UI.Scene

Public surface categories:

- Core data: `Color`, `Point`, `Size`, `Rect`, `Matrix`, `Scene`, `Element`.
- Paint data: `Paint`, `StrokeCap`, `StrokeJoin`, `BlendMode`, `Shader`, `ColorFilter`, `MaskFilter`, `ImageFilter`, `PathEffect`.
- Text data: `FontSpec`, text measurement request/result, text run.
- Path data: path commands, fill types, path operations, path measurement, segment extraction.
- Clip and region data: rectangular, path, and region clips.
- Transform data: 2D transforms and perspective-style transforms.
- Constructors for every baseline element category: rectangles, ellipses, lines, text, images, paths, groups, points, vertices, arcs, pictures, text runs.

Contract rules:

- Constructors return immutable scene values.
- Group and layout composition preserve declaration order.
- The public scene DSL has no imperative canvas callback entry point.
- Device-specific unsupported features return diagnostics at render/evidence time rather than adding fallback rendering.

### Module: FS.Skia.UI.Diagnostics

Public surface categories:

- `DiagnosticSeverity`
- `DiagnosticStage`
- `RenderDiagnostic`
- Helpers for unsupported platform, invalid configuration, Vulkan unavailable, missing capability, frame recovery, screenshot failure, and shutdown failure.

Contract rules:

- Diagnostics are structured and testable.
- Startup failures identify Vulkan/surface/device/capability stage where possible.

### Module: FS.Skia.UI.Viewer

Public surface categories:

- `ViewerConfiguration`
- `ViewerEvent`
- `ScreenshotFormat`
- `ScreenshotRequest`
- `ViewerEffect<'msg>`
- `ViewerProgram<'model,'msg>`
- `defaultConfiguration`
- `create`
- `withSubscription`
- `withEventMapping`
- `withEffectMapping`
- `run`

Contract rules:

- `View : 'model -> Scene` is the only public rendering projection.
- `Update` remains pure; I/O is represented as commands/effects and interpreted at the edge.
- The viewer is Vulkan-only and exposes no fallback backend selector.
- Screenshot capture returns clear success/failure information.

### Module: FS.Skia.UI.Parity

Public or test-support surface categories:

- `ParityStatus`
- `EvidenceType`
- `ParityEvidenceItem`
- `ParityReport`
- report serialization/deserialization helpers
- baseline capability IDs

Contract rules:

- Reports include the pinned upstream commit.
- Merge-ready reports contain no `NotYetSupported` non-conflicting capabilities.

## Package: FS.Skia.UI.Charts

Charts and DataGrid as pure view-layer components.

### Module: FS.Skia.UI.Charts.Types

Public surface categories:

- Shared chart types: `ChartConfig`, `AxisConfig`, `LegendConfig`, `Palette`, `DataPoint`, `DataSeries`.
- Interaction projection types: selected point, highlighted series, visible range.
- DataGrid types: `ColumnDef`, `ColumnType`, `CellValue`, `SortDirection`, `SortState`, `DataGridConfig`, `DataGridData`, `DataGridViewport`.

Contract rules:

- Types represent view props, not hidden component state.
- Consumer Elmish `Model` owns selection, sorting, scrolling, filtering, hover, and zoom.

### Modules: LineChart, BarChart, PieChart, ScatterPlot, AreaChart, Histogram, Candlestick, RadarChart

Public surface categories:

- default config builders
- chart builder functions returning core `Element` or `Scene`
- optional pure scaling/layout helpers exposed only when they are useful in FSI
- optional pure hit-test helpers returning semantic chart targets

Contract rules:

- Chart builders are pure.
- Empty, invalid, and large datasets are handled without unhandled exceptions.
- 100,000-point scale tests are part of the contract.

### Module: FS.Skia.UI.Charts.DataGrid

Public surface categories:

- default config builder
- grid builder function returning core `Element` or `Scene`
- pure visible-row calculation
- pure sorting helper
- optional hit-test helper for column/row/cell targets

Contract rules:

- DataGrid builder is pure.
- Scroll and sort state are inputs from the Elmish model.
- 10,000-row scale tests are part of the contract.

## Package: FS.Skia.UI.Layout

Layout and graph visualization as pure view-layer components.

### Module: FS.Skia.UI.Layout.Types

Public surface categories:

- `HorizontalAlignment`
- `VerticalAlignment`
- `DockPosition`
- `LayoutPadding`
- `LayoutSizing`
- `StackConfig`
- `DockConfig`
- `LayoutChild`
- `GraphKind`
- `GraphNode`
- `GraphEdge`
- `GraphConfig`
- `GraphDefinition`
- graph style and layout result records

### Module: FS.Skia.UI.Layout.Layout

Public surface categories:

- horizontal stack builder
- vertical stack builder
- dock builder
- pure layout measurement/allocation helpers

Contract rules:

- Layout builders return core scene elements.
- Resize behavior is deterministic.
- Layouts must not crash on empty, zero-size, or constrained bounds.

### Module: FS.Skia.UI.Layout.GraphValidation

Public surface categories:

- graph validation result
- cycle detection
- duplicate/missing endpoint validation
- disconnected component reporting

Contract rules:

- Invalid directed acyclic graph inputs return clear validation results.
- Validation is pure and independently testable.

### Module: FS.Skia.UI.Layout.Graph

Public surface categories:

- directed graph builder
- undirected graph builder
- pure layout result helper
- graph hit-test helper

Contract rules:

- Graph builders return core scene elements or validation errors.
- 100-node DAG and 50-node weighted undirected graph scale targets are part of the contract.

## Cross-Package Composition Contract

Consumer code should follow this shape:

```fsharp
type Model =
    { Sales: Sale list
      SelectedPoint: string option
      GridSort: SortState option
      GridScroll: int
      GraphFocus: string option }

type Msg =
    | ViewerEvent of ViewerEvent
    | SelectPoint of string
    | SortGrid of string
    | ScrollGrid of int
    | FocusNode of string

let salesChartProps model =
    // Pure model-to-view projection.
    model.Sales
    |> Sales.toMonthlySeries model.SelectedPoint

let view model =
    Scene.group [
        Layout.dock Defaults.dockConfig [
            Charts.LineChart.lineChart (salesChartProps model)
            Charts.DataGrid.dataGrid (ordersGridProps model)
            Layout.Graph.graph (graphProps model)
        ]
    ]
```

Rules:

- Components are called from `view`.
- Components receive props derived from `Model`.
- Components return scene elements.
- Component interaction helpers may produce semantic targets; consumer `update` decides how those targets change `Model`.
- No chart/layout/graph package owns hidden long-lived state.

## Compatibility Contract

- This is a Tier 1 public API expansion.
- Existing core viewer names may be revised only through `.fsi` first and with migration notes.
- The plan may add new packages but must keep the existing core package packable.
- The upstream baseline is behavioral/reference material only; source reuse requires explicit license attribution and plan update.

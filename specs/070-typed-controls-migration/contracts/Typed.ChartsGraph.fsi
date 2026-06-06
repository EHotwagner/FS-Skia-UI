// CONTRACT SKETCH (Phase 1) — charts/graph. Data-class fields REUSE the existing
// chart/graph data types (ChartSeries/ChartPoint from Charts.fsi, graph node/edge
// types); the façade does NOT redefine the data model (spec edge case). Lowers to
// the dedicated legacy *.create in Charts.fsi.
namespace FS.Skia.UI.Controls.Typed

open FS.Skia.UI.Controls

type LineChartProps<'msg> = { Id: ControlId option; Series: ChartSeries list; OnSelected: (string -> 'msg) option }
type BarChartProps<'msg> = { Id: ControlId option; Series: ChartSeries list; OnSelected: (string -> 'msg) option }
type PieChartProps<'msg> = { Id: ControlId option; Values: ChartPoint list; OnSelected: (string -> 'msg) option }
type ScatterPlotProps<'msg> = { Id: ControlId option; Series: ChartSeries list; OnSelected: (string -> 'msg) option }
type GraphViewProps<'msg> = { Id: ControlId option; Nodes: GraphNode list; Edges: GraphEdge list; OnSelected: (string -> 'msg) option }

module LineChart =
    val defaults: LineChartProps<'msg>
    /// Lowers ≡ `LineChart.create [ LineChart.series props.Series ]`.
    val view: props: LineChartProps<'msg> -> Widget<'msg>

module BarChart =
    val defaults: BarChartProps<'msg>
    val view: props: BarChartProps<'msg> -> Widget<'msg>

module PieChart =
    val defaults: PieChartProps<'msg>
    val view: props: PieChartProps<'msg> -> Widget<'msg>

module ScatterPlot =
    val defaults: ScatterPlotProps<'msg>
    val view: props: ScatterPlotProps<'msg> -> Widget<'msg>

module GraphView =
    val defaults: GraphViewProps<'msg>
    val view: props: GraphViewProps<'msg> -> Widget<'msg>

// custom-control: NO Props schema. Typed via the existing public bridge
//   Widget.ofControl : Control<'msg> -> Widget<'msg>
// over a legacy `CustomControl.create definition attrs` (FR-006, research R4).

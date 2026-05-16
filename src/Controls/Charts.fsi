namespace FS.Skia.UI.Controls

type ChartPoint =
    { X: float
      Y: float
      Label: string option }

type ChartSeries =
    { Name: string
      Points: ChartPoint list }

module LineChart =
    val create: Attr<'msg> list -> Control<'msg>
    val series: ChartSeries list -> Attr<'msg>

module BarChart =
    val create: Attr<'msg> list -> Control<'msg>
    val series: ChartSeries list -> Attr<'msg>

module PieChart =
    val create: Attr<'msg> list -> Control<'msg>
    val values: ChartPoint list -> Attr<'msg>

module ScatterPlot =
    val create: Attr<'msg> list -> Control<'msg>
    val series: ChartSeries list -> Attr<'msg>

module GraphView =
    val create: Attr<'msg> list -> Control<'msg>
    val nodes: string list -> Attr<'msg>

namespace FS.Skia.UI.Controls

type ChartPoint =
    { X: float
      Y: float
      Label: string option }

type ChartSeries =
    { Name: string
      Points: ChartPoint list }

module ChartAttrs =
    let series (values: ChartSeries list) = Attr.create "series" Data (UntypedValue values)
    let points (values: ChartPoint list) = Attr.create "values" Data (UntypedValue values)
    let nodes (values: string list) = Attr.create "nodes" Data (StringListValue values)

module LineChart =
    let create attrs = Control.create "line-chart" attrs
    let series values = ChartAttrs.series values

module BarChart =
    let create attrs = Control.create "bar-chart" attrs
    let series values = ChartAttrs.series values

module PieChart =
    let create attrs = Control.create "pie-chart" attrs
    let values values = ChartAttrs.points values

module ScatterPlot =
    let create attrs = Control.create "scatter-plot" attrs
    let series values = ChartAttrs.series values

module GraphView =
    let create attrs = Control.create "graph-view" attrs
    let nodes values = ChartAttrs.nodes values

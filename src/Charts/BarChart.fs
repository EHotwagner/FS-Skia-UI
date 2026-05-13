namespace FS.Skia.UI.Charts

open FS.Skia.UI

module BarChart =
    let defaultConfig width height = Defaults.chartConfig width height

    let barChart (_: ChartConfig) (series: DataSeries list) =
        series |> ChartHelpers.yValues |> Scene.chart

    let hitTest config series x y = ChartHelpers.hitTestSeries config series x y

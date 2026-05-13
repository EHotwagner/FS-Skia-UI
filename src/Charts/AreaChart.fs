namespace FS.Skia.UI.Charts

open FS.Skia.UI

module AreaChart =
    let defaultConfig width height = Defaults.chartConfig width height

    let areaChart (_: ChartConfig) (_: bool) (series: DataSeries list) =
        series |> ChartHelpers.yValues |> Scene.chart

    let hitTest config series x y = ChartHelpers.hitTestSeries config series x y

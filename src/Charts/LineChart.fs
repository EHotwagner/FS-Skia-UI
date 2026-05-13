namespace FS.Skia.UI.Charts

open FS.Skia.UI

module LineChart =
    let defaultConfig width height = Defaults.chartConfig width height

    let lineChart (config: ChartConfig) (series: DataSeries list) =
        let values = series |> ChartHelpers.yValues
        Scene.chart values

    let hitTest config series x y = ChartHelpers.hitTestSeries config series x y

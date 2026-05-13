namespace FS.Skia.UI.Charts

open FS.Skia.UI

module PieChart =
    let defaultConfig width height = Defaults.chartConfig width height
    let pieChart (_: ChartConfig) (values: DataPoint list) = values |> List.map _.Y |> ChartHelpers.finiteValues |> Scene.chart
    let donutChart (config: ChartConfig) (_: float) (values: DataPoint list) = pieChart config values
    let hitTest config values x y = ChartHelpers.hitTestPie config values x y

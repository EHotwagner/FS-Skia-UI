namespace FS.Skia.UI.Charts

open FS.Skia.UI

module PieChart =
    val defaultConfig : width: float -> height: float -> ChartConfig
    val pieChart : config: ChartConfig -> values: DataPoint list -> Scene
    val donutChart : config: ChartConfig -> holeRadius: float -> values: DataPoint list -> Scene
    val hitTest : config: ChartConfig -> values: DataPoint list -> x: float -> y: float -> ChartTarget option

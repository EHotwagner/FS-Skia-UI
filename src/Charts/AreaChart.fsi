namespace FS.Skia.UI.Charts

open FS.Skia.UI

module AreaChart =
    val defaultConfig : width: float -> height: float -> ChartConfig
    val areaChart : config: ChartConfig -> stacked: bool -> series: DataSeries list -> Scene
    val hitTest : config: ChartConfig -> series: DataSeries list -> x: float -> y: float -> ChartTarget option

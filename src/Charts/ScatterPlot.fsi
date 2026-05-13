namespace FS.Skia.UI.Charts

open FS.Skia.UI

module ScatterPlot =
    val defaultConfig : width: float -> height: float -> ChartConfig
    val scatterPlot : config: ChartConfig -> series: DataSeries list -> Scene
    val hitTest : config: ChartConfig -> series: DataSeries list -> x: float -> y: float -> ChartTarget option

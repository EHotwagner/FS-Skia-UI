namespace FS.Skia.UI.Charts

open FS.Skia.UI

module Histogram =
    val defaultConfig : width: float -> height: float -> ChartConfig
    val histogram : config: ChartConfig -> binCount: int -> values: float list -> Scene
    val bins : binCount: int -> values: float list -> (float * float * int) list

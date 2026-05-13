namespace FS.Skia.UI.Charts

open FS.Skia.UI

type OhlcPoint =
    { X: float
      Open: float
      High: float
      Low: float
      Close: float }

module Candlestick =
    val defaultConfig : width: float -> height: float -> ChartConfig
    val candlestick : config: ChartConfig -> values: OhlcPoint list -> Scene

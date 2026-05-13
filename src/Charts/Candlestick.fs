namespace FS.Skia.UI.Charts

open FS.Skia.UI

type OhlcPoint =
    { X: float
      Open: float
      High: float
      Low: float
      Close: float }

module Candlestick =
    let defaultConfig width height = Defaults.chartConfig width height

    let candlestick (_: ChartConfig) (values: OhlcPoint list) =
        values
        |> List.collect (fun item -> [ item.Open; item.High; item.Low; item.Close ])
        |> ChartHelpers.finiteValues
        |> Scene.chart

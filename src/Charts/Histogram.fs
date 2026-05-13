namespace FS.Skia.UI.Charts

open FS.Skia.UI

module Histogram =
    let defaultConfig width height = Defaults.chartConfig width height

    let histogram (_: ChartConfig) (_: int) (values: float list) =
        values |> ChartHelpers.finiteValues |> Scene.chart

    let bins binCount values =
        let count = max 1 binCount
        let values = ChartHelpers.finiteValues values
        let minValue, maxValue = Scale.bounds values
        let width = if maxValue = minValue then 1.0 else (maxValue - minValue) / float count

        [ for index in 0 .. count - 1 do
              let low = minValue + float index * width
              let high = low + width
              let itemCount =
                  values
                  |> List.filter (fun value ->
                      if index = count - 1 then value >= low && value <= high else value >= low && value < high)
                  |> List.length

              low, high, itemCount ]

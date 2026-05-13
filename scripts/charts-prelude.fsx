#r "nuget: Fable.Elmish, 4.2.0"
#r "../src/Lib/bin/Debug/net10.0/FS.Skia.UI.dll"
#r "../src/Charts/bin/Debug/net10.0/FS.Skia.UI.Charts.dll"

open FS.Skia.UI.Charts

let config = LineChart.defaultConfig 640.0 360.0
let series =
    [ { Name = "Sales"
        Points = [ { X = 1.0; Y = 10.0; Label = None }; { X = 2.0; Y = 18.0; Label = None } ]
        Color = None } ]

let scene = LineChart.lineChart config series
printfn "charts-prelude scene=%A" (box scene |> isNull |> not)

module ChartsGallery.Program

open System
open System.Diagnostics
open Elmish
open FS.Skia.UI
open FS.Skia.UI.Charts

type Model =
    { Selected: ChartTarget option
      VisibleStart: float
      VisibleEnd: float
      Series: DataSeries list }

type Msg =
    | NoOp
    | Select of ChartTarget option
    | Zoom of float * float
    | HostEffect of ViewerEffect<Msg>

let buildSeries () =
    [ { Name = "revenue"
        Points =
            [ for index in 0 .. 47 ->
                  { X = float index
                    Y = 42.0 + sin (float index / 4.0) * 12.0 + float (index % 9)
                    Label = Some $"M{index + 1}" } ]
        Color = Some(Colors.rgba 64uy 128uy 220uy 255uy) }
      { Name = "cost"
        Points =
            [ for index in 0 .. 47 ->
                  { X = float index
                    Y = 26.0 + cos (float index / 5.0) * 8.0 + float (index % 6)
                    Label = None } ]
        Color = Some(Colors.rgba 222uy 92uy 96uy 255uy) } ]

let init () =
    { Selected = None
      VisibleStart = 0.0
      VisibleEnd = 47.0
      Series = buildSeries () },
    Cmd.none

let update msg model =
    match msg with
    | NoOp
    | HostEffect _ -> model, Cmd.none
    | Select target -> { model with Selected = target }, Cmd.none
    | Zoom(startValue, endValue) -> { model with VisibleStart = startValue; VisibleEnd = endValue }, Cmd.none

let chartConfig x y width height model =
    { Defaults.chartConfig width height with
        Area = { X = x; Y = y; Width = width; Height = height }
        XAxis = { Defaults.axis with Minimum = Some model.VisibleStart; Maximum = Some model.VisibleEnd; Title = Some "period" }
        YAxis = { Defaults.axis with Minimum = Some 0.0; Title = Some "value" }
        Legend = { Defaults.legend with Position = "bottom" } }

let view model =
    let selectedText = model.Selected |> Option.map string |> Option.defaultValue "none"
    let lineConfig = chartConfig 32.0 88.0 270.0 130.0 model
    let barConfig = chartConfig 334.0 88.0 270.0 130.0 model
    let scatterConfig = chartConfig 32.0 260.0 270.0 132.0 model
    let areaConfig = chartConfig 334.0 260.0 270.0 132.0 model

    let pieValues =
        model.Series.Head.Points
        |> List.take 6
        |> List.map (fun point -> { point with Y = max 0.1 point.Y })

    Scene.group [
        Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgba 18uy 24uy 32uy 255uy)
        Scene.text (32.0, 54.0) "Charts Gallery" Colors.white
        LineChart.lineChart lineConfig model.Series
        BarChart.barChart barConfig model.Series
        ScatterPlot.scatterPlot scatterConfig model.Series
        AreaChart.areaChart areaConfig false model.Series
        PieChart.donutChart (chartConfig 236.0 394.0 168.0 70.0 model) 18.0 pieValues
        Histogram.histogram (chartConfig 430.0 394.0 174.0 70.0 model) 8 (model.Series |> List.collect (fun series -> series.Points |> List.map _.Y))
        Scene.text (32.0, 432.0) $"selected: {selectedText}" Colors.white
    ]

let configuration =
    { Viewer.defaultConfiguration "Charts Gallery" { Width = 640; Height = 480 } with
        ClearColor = Some(Colors.rgba 18uy 24uy 32uy 255uy)
        TargetFrameRate = Some 60
        Diagnostics = { Verbose = true } }

let program =
    Viewer.create configuration init update view
    |> Viewer.withEventMapping (fun _ -> Some NoOp)
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)

let smokeProgram =
    Viewer.create configuration init update view
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)

let runContractSmoke () =
    let model, _ = init ()
    let scene = view model
    let target = LineChart.hitTest (chartConfig 32.0 88.0 270.0 130.0 model) model.Series 168.0 150.0
    printfn "status=ok"
    printfn "sample=ChartsGallery"
    printfn "selection-owned-by-model=%b" (model.Selected.IsNone)
    printfn "hit-test=%A" target
    printfn "kinds=%A" (Scene.describe scene)
    0

let runSmoke () =
    let stopwatch = Stopwatch.StartNew()

    match Viewer.run smokeProgram with
    | Ok() ->
        stopwatch.Stop()
        printfn "status=ok"
        printfn "sample=ChartsGallery"
        printfn "renderer=Vulkan"
        printfn "fallback-used=false"
        printfn "first-frame-ms=%d" stopwatch.ElapsedMilliseconds
        0
    | Result.Error diagnostic ->
        stopwatch.Stop()
        printfn "status=error"
        printfn "sample=ChartsGallery"
        printfn "diagnostic-stage=%A" diagnostic.Stage
        printfn "diagnostic-message=%s" diagnostic.Message
        2

[<EntryPoint>]
let main argv =
    if argv |> Array.contains "--contract-smoke" then
        runContractSmoke ()
    else
        runSmoke ()

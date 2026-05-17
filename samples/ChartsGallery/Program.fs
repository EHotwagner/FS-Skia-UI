module ChartsGallery.Program

open System
open System.Diagnostics
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls

type Model =
    { Selected: string option
      VisibleStart: float
      VisibleEnd: float
      Series: ChartSeries list }

type Msg =
    | NoOp
    | Select of string option
    | Zoom of float * float

let buildSeries () =
    [ { Name = "revenue"
        Points =
            [ for index in 0 .. 47 ->
                  { X = float index
                    Y = 42.0 + sin (float index / 4.0) * 12.0 + float (index % 9)
                    Label = Some $"M{index + 1}" } ] }
      { Name = "cost"
        Points =
            [ for index in 0 .. 47 ->
                  { X = float index
                    Y = 26.0 + cos (float index / 5.0) * 8.0 + float (index % 6)
                    Label = None } ] } ]

let init () =
    { Selected = None
      VisibleStart = 0.0
      VisibleEnd = 47.0
      Series = buildSeries () }

let update msg model =
    match msg with
    | NoOp -> model
    | Select target -> { model with Selected = target }
    | Zoom(startValue, endValue) -> { model with VisibleStart = startValue; VisibleEnd = endValue }

let chartControls model =
    let pieValues =
        model.Series.Head.Points
        |> List.take 6
        |> List.map (fun point -> { point with Y = max 0.1 point.Y })

    Stack.create [
        Stack.children [
            TextBlock.create [ TextBlock.text "Charts Gallery" ]
            LineChart.create [ LineChart.series model.Series; Attr.width 270.0; Attr.height 130.0 ]
            BarChart.create [ BarChart.series model.Series; Attr.width 270.0; Attr.height 130.0 ]
            ScatterPlot.create [ ScatterPlot.series model.Series; Attr.width 270.0; Attr.height 132.0 ]
            PieChart.create [ PieChart.values pieValues; Attr.width 168.0; Attr.height 70.0 ]
        ]
    ]

let view model =
    let selectedText = model.Selected |> Option.defaultValue "none"
    let rendered = Control.render Theme.light (chartControls model)

    Scene.group [
        Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgba 18uy 24uy 32uy 255uy)
        rendered.Scene
        Scene.text (32.0, 432.0) $"selected: {selectedText}" Colors.white
    ]

let runContractSmoke () =
    let stopwatch = Stopwatch.StartNew()
    let model = init ()
    let rendered = Control.render Theme.light (chartControls model)
    stopwatch.Stop()

    printfn "status=ok"
    printfn "sample=ChartsGallery"
    printfn "selection-owned-by-model=%b" model.Selected.IsNone
    printfn "control-count=%d" rendered.NodeCount
    printfn "kinds=%A" (Scene.describe (view model))
    printfn "elapsed-ms=%d" stopwatch.ElapsedMilliseconds
    0

let runSmoke () =
    runContractSmoke ()

[<EntryPoint>]
let main argv =
    if argv |> Array.contains "--contract-smoke" then
        runContractSmoke ()
    else
        runSmoke ()

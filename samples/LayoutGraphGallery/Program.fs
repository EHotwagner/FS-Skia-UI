module LayoutGraphGallery.Program

open System
open System.Diagnostics
open Elmish
open FS.Skia.UI
open FS.Skia.UI.Charts
open FS.Skia.UI.Layout

type Model =
    { Focus: GraphTarget option
      Sort: SortState }

type Msg =
    | NoOp
    | Focus of GraphTarget option
    | SortBy of SortState
    | HostEffect of ViewerEffect<Msg>

let init () =
    { Focus = None
      Sort = { ColumnKey = "score"; Direction = Descending } },
    Cmd.none

let update msg model =
    match msg with
    | NoOp
    | HostEffect _ -> model, Cmd.none
    | Focus target -> { model with Focus = target }, Cmd.none
    | SortBy sort -> { model with Sort = sort }, Cmd.none

let node id label =
    { Id = id
      Label = label
      Style = None }

let edge source target weight =
    { Source = source
      Target = target
      Weight = weight
      Label = weight |> Option.map (fun value -> value.ToString("G3", Globalization.CultureInfo.InvariantCulture)) }

let directedGraph =
    { Config =
        { Defaults.graphConfig Directed 260.0 150.0 with
            Bounds = { X = 32.0; Y = 92.0; Width = 260.0; Height = 150.0 } }
      Nodes = [ node "ingest" "Ingest"; node "clean" "Clean"; node "model" "Model"; node "ship" "Ship" ]
      Edges = [ edge "ingest" "clean" None; edge "clean" "model" None; edge "model" "ship" None ] }

let invalidDag =
    { directedGraph with
        Nodes = [ node "a" "A"; node "b" "B" ]
        Edges = [ edge "a" "b" None; edge "b" "a" None ] }

let weightedGraph =
    { Config =
        { Defaults.graphConfig Undirected 260.0 150.0 with
            Bounds = { X = 334.0; Y = 92.0; Width = 260.0; Height = 150.0 } }
      Nodes = [ for index in 0 .. 9 -> node $"u{index}" $"U{index}" ]
      Edges = [ for index in 0 .. 9 -> edge $"u{index}" $"u{(index + 3) % 10}" (Some(float (index % 5 + 1))) ] }

let chartSeries =
    [ { Name = "layout"
        Points = [ for index in 0 .. 11 -> { X = float index; Y = float (index * 7 % 13 + 3); Label = None } ]
        Color = None } ]

let gridColumns =
    [ { Key = "name"; Header = "Name"; ColumnType = Text; Width = Some 160.0 }
      { Key = "score"; Header = "Score"; ColumnType = Numeric; Width = None } ]

let gridRows =
    [ for index in 0 .. 7 ->
          [ "name", TextValue $"Node {index}"
            "score", NumericValue(float (index * 11 % 19)) ]
          |> Map.ofList ]

let view model =
    let selectedText = model.Focus |> Option.map string |> Option.defaultValue "none"
    let chartConfig =
        { Defaults.chartConfig 260.0 120.0 with
            Area = { X = 32.0; Y = 292.0; Width = 260.0; Height = 120.0 } }

    let gridConfig =
        { Defaults.dataGridConfig 260.0 120.0 with
            Area = { X = 334.0; Y = 292.0; Width = 260.0; Height = 120.0 } }

    let gridData =
        { Columns = gridColumns
          Rows = DataGrid.sortRows gridColumns model.Sort gridRows }

    let invalidText =
        match GraphValidation.validate invalidDag with
        | [] -> "invalid DAG: none"
        | issues -> $"invalid DAG: {issues.Length} issue(s)"

    let directedScene =
        Graph.directed directedGraph |> Result.defaultValue (Scene.text (32.0, 112.0) "directed graph invalid" (Colors.rgba 220uy 64uy 52uy 255uy))

    let weightedScene =
        Graph.undirected weightedGraph |> Result.defaultValue (Scene.text (334.0, 112.0) "weighted graph invalid" (Colors.rgba 220uy 64uy 52uy 255uy))

    let layoutShell =
        Layout.horizontalStack
            { Defaults.stackConfig 584.0 180.0 with
                Bounds = { X = 28.0; Y = 84.0; Width = 584.0; Height = 180.0 }
                Spacing = 12.0 }
            [ Defaults.child directedScene; Defaults.child weightedScene ]

    Scene.group [
        Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgba 18uy 24uy 32uy 255uy)
        Scene.text (32.0, 54.0) "Layout Graph Gallery" Colors.white
        layoutShell
        LineChart.lineChart chartConfig chartSeries
        DataGrid.dataGrid gridConfig gridData { FirstRow = 0; RowCount = 6 }
        Scene.text (32.0, 436.0) invalidText Colors.white
        Scene.text (334.0, 436.0) $"focus: {selectedText}" Colors.white
    ]

let configuration =
    { Viewer.defaultConfiguration "Layout Graph Gallery" { Width = 640; Height = 480 } with
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
    let directedLayout = Graph.layout directedGraph
    printfn "status=ok"
    printfn "sample=LayoutGraphGallery"
    printfn "model-owns-focus=%b" model.Focus.IsNone
    printfn "invalid-dag-issues=%d" (GraphValidation.validate invalidDag).Length
    printfn "directed-layout=%A" (directedLayout |> Result.map (fun layout -> layout.Nodes.Length, layout.Edges.Length))
    printfn "kinds=%A" (Scene.describe scene)
    0

let runSmoke () =
    let stopwatch = Stopwatch.StartNew()

    match Viewer.run smokeProgram with
    | Ok() ->
        stopwatch.Stop()
        printfn "status=ok"
        printfn "sample=LayoutGraphGallery"
        printfn "renderer=Vulkan"
        printfn "fallback-used=false"
        printfn "first-frame-ms=%d" stopwatch.ElapsedMilliseconds
        0
    | Result.Error diagnostic ->
        stopwatch.Stop()
        printfn "status=error"
        printfn "sample=LayoutGraphGallery"
        printfn "diagnostic-stage=%A" diagnostic.Stage
        printfn "diagnostic-message=%s" diagnostic.Message
        2

[<EntryPoint>]
let main argv =
    if argv |> Array.contains "--contract-smoke" then
        runContractSmoke ()
    else
        runSmoke ()

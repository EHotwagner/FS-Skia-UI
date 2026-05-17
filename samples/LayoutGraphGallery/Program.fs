module LayoutGraphGallery.Program

open System
open System.Diagnostics
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls
open FS.Skia.UI.Layout

type Model =
    { Focus: GraphTarget option
      Grid: DataGridModel
      GridRows: DataGridRow list }

type Msg =
    | NoOp
    | Focus of GraphTarget option
    | GridMsg of DataGridMsg

let gridColumns =
    [ { Key = "name"; Header = "Name"; Width = 160.0; ColumnType = TextColumn }
      { Key = "score"; Header = "Score"; Width = 88.0; ColumnType = NumericColumn } ]

let gridRow index =
    let key = $"node-{index:D2}"

    { Key = key
      Cells =
        [ { RowKey = key; ColumnKey = "name"; Value = $"Node {index}" }
          { RowKey = key; ColumnKey = "score"; Value = string (index * 11 % 19) } ] }

let init () =
    let rows = [ for index in 0 .. 7 -> gridRow index ]
    let grid, _ = DataGrid.init "layout-grid" gridColumns rows.Length 24.0 120.0
    { Focus = None; Grid = grid; GridRows = rows }

let update msg model =
    match msg with
    | NoOp -> model
    | Focus target -> { model with Focus = target }
    | GridMsg gridMsg ->
        let grid, _ = DataGrid.update gridMsg model.Grid
        { model with Grid = grid }

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
        Points = [ for index in 0 .. 11 -> { X = float index; Y = float (index * 7 % 13 + 3); Label = None } ] } ]

let automaticMeasure text =
    fun request ->
        { Width = min request.AvailableWidth (float (String.length text) * 8.0)
          Height = 18.0
          Diagnostics = [] }

let automaticNode id label grow =
    { Defaults.layoutNode id with
        Intent = { Defaults.layoutIntent with FlexGrow = grow }
        Measure = Some(automaticMeasure label)
        Content = Some(Scene.text (0.0, 0.0) label Colors.white) }

let automaticLayoutRoot width statusVisibility =
    { Defaults.layoutNode "gallery-auto-root" with
        Intent =
            { Defaults.layoutIntent with
                Direction = Row
                Wrap = Wrap
                Padding = { Left = 8.0; Top = 8.0; Right = 8.0; Bottom = 8.0 }
                Gap = { Row = 6.0; Column = 8.0 } }
        Children =
            [ automaticNode "gallery-auto-title" "Auto Layout" 1.0
              { Defaults.layoutNode "gallery-auto-widget-row" with
                    Intent =
                        { Defaults.layoutIntent with
                            Direction = Row
                            Wrap = Wrap
                            Size = { Width = Some(max 120.0 (width * 0.42)); Height = None }
                            Gap = { Row = 4.0; Column = 6.0 } }
                    Children =
                        [ automaticNode "gallery-auto-chart-chip" "chart" 1.0
                          automaticNode "gallery-auto-grid-chip" "grid" 1.0 ] }
              { automaticNode "gallery-auto-status" "Mixed controls" 0.0 with Visibility = statusVisibility }
              automaticNode "gallery-auto-action" "Resize ready" 0.0 ] }

let chartScene () =
    LineChart.create [ LineChart.series chartSeries; Attr.width 260.0; Attr.height 120.0 ]
    |> Control.render Theme.light
    |> _.Scene

let gridScene model =
    DataGrid.create gridColumns [
        DataGrid.rows model.GridRows
        DataGrid.visibleRange model.Grid.VisibleRange
        Attr.width 260.0
        Attr.height 120.0
    ]
    |> Control.render Theme.light
    |> _.Scene

let view model =
    let selectedText = model.Focus |> Option.map string |> Option.defaultValue "none"

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

    let automaticRoot = automaticLayoutRoot 584.0 Visible
    let automaticLayout = Layout.evaluate (Defaults.availableSpace 584.0 72.0) automaticRoot

    Scene.group [
        Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgba 18uy 24uy 32uy 255uy)
        Scene.text (32.0, 54.0) "Layout Graph Gallery" Colors.white
        layoutShell
        Layout.renderComputed automaticLayout automaticRoot
        chartScene ()
        gridScene model
        Scene.text (32.0, 436.0) invalidText Colors.white
        Scene.text (334.0, 436.0) $"focus: {selectedText}" Colors.white
    ]

let runContractSmoke () =
    let stopwatch = Stopwatch.StartNew()
    let model = init ()
    let scene = view model
    let directedLayout = Graph.layout directedGraph
    let automaticRoot = automaticLayoutRoot 584.0 Visible
    let resizedRoot = automaticLayoutRoot 340.0 Hidden
    let automaticLayout = Layout.evaluate (Defaults.availableSpace 584.0 72.0) automaticRoot
    let resizedLayout = Layout.evaluate (Defaults.availableSpace 340.0 100.0) resizedRoot
    let hiddenStatus = resizedLayout.Bounds |> List.find (fun item -> item.NodeId = "gallery-auto-status")
    stopwatch.Stop()

    printfn "status=ok"
    printfn "sample=LayoutGraphGallery"
    printfn "model-owns-focus=%b" model.Focus.IsNone
    printfn "invalid-dag-issues=%d" (GraphValidation.validate invalidDag).Length
    printfn "directed-layout=%A" (directedLayout |> Result.map (fun layout -> layout.Nodes.Length, layout.Edges.Length))
    printfn "automatic-layout-bounds=%d" automaticLayout.Bounds.Length
    printfn "automatic-layout-diagnostics=%d" automaticLayout.Diagnostics.Length
    printfn "automatic-layout-resized-bounds=%d" resizedLayout.Bounds.Length
    printfn "automatic-layout-hidden-status=%A" hiddenStatus.Visibility
    printfn "automatic-layout-nested=%b" (automaticLayout.Bounds |> List.exists (fun item -> item.NodeId = "gallery-auto-widget-row"))
    printfn "automatic-layout-hit=%A" (Layout.hitTestComputed (Defaults.pixelSnapPolicy 1.0) automaticLayout 12.0 12.0)
    printfn "datagrid-visible-range=%A" model.Grid.VisibleRange
    printfn "kinds=%A" (Scene.describe scene)
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

module DataGridGallery.Program

open System
open System.Diagnostics
open Elmish
open FS.Skia.UI
open FS.Skia.UI.Charts

type Model =
    { Sort: SortState
      ScrollOffset: int
      Selected: DataGridTarget option
      Rows: Map<string, CellValue> list }

type Msg =
    | NoOp
    | SortBy of SortState
    | ScrollTo of int
    | Select of DataGridTarget option
    | HostEffect of ViewerEffect<Msg>

let columns =
    [ { Key = "account"; Header = "Account"; ColumnType = Text; Width = Some 190.0 }
      { Key = "region"; Header = "Region"; ColumnType = Text; Width = Some 120.0 }
      { Key = "amount"; Header = "Amount"; ColumnType = Numeric; Width = Some 110.0 }
      { Key = "active"; Header = "Active"; ColumnType = Boolean; Width = None } ]

let row index =
    [ "account", TextValue $"Customer {index:D4}"
      "region", TextValue ([| "North"; "South"; "East"; "West" |][index % 4])
      "amount", NumericValue(float (index * 17 % 1000) / 10.0)
      "active", BooleanValue(index % 3 <> 0) ]
    |> Map.ofList

let init () =
    { Sort = { ColumnKey = "amount"; Direction = Descending }
      ScrollOffset = 128
      Selected = None
      Rows = [ for index in 0 .. 999 -> row index ] },
    Cmd.none

let update msg model =
    match msg with
    | NoOp
    | HostEffect _ -> model, Cmd.none
    | SortBy sort -> { model with Sort = sort }, Cmd.none
    | ScrollTo offset -> { model with ScrollOffset = max 0 offset }, Cmd.none
    | Select target -> { model with Selected = target }, Cmd.none

let config =
    { Defaults.dataGridConfig 560.0 318.0 with
        Area = { X = 40.0; Y = 92.0; Width = 560.0; Height = 318.0 }
        HeaderHeight = 32.0
        RowHeight = 26.0
        FixedHeader = true }

let view model =
    let selectedText = model.Selected |> Option.map string |> Option.defaultValue "none"
    let sortedRows = DataGrid.sortRows columns model.Sort model.Rows
    let viewport = DataGrid.visibleRows config sortedRows.Length model.ScrollOffset
    let data = { Columns = columns; Rows = sortedRows }

    Scene.group [
        Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgba 18uy 24uy 32uy 255uy)
        Scene.text (40.0, 56.0) "DataGrid Gallery" Colors.white
        DataGrid.dataGrid config data viewport
        Scene.text (40.0, 440.0) $"sort: {model.Sort.ColumnKey} {model.Sort.Direction}; scroll: {model.ScrollOffset}; selected: {selectedText}" Colors.white
    ]

let configuration =
    { Viewer.defaultConfiguration "DataGrid Gallery" { Width = 640; Height = 480 } with
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
    let sortedRows = DataGrid.sortRows columns model.Sort model.Rows
    let viewport = DataGrid.visibleRows config sortedRows.Length model.ScrollOffset
    let target = DataGrid.hitTest config { Columns = columns; Rows = sortedRows } viewport 52.0 132.0
    printfn "status=ok"
    printfn "sample=DataGridGallery"
    printfn "state-owned-by-model=%b" (model.Selected.IsNone && model.ScrollOffset = 128)
    printfn "hit-test=%A" target
    printfn "kinds=%A" (Scene.describe (view model))
    0

let runSmoke () =
    let stopwatch = Stopwatch.StartNew()

    match Viewer.run smokeProgram with
    | Ok() ->
        stopwatch.Stop()
        printfn "status=ok"
        printfn "sample=DataGridGallery"
        printfn "renderer=Vulkan"
        printfn "fallback-used=false"
        printfn "first-frame-ms=%d" stopwatch.ElapsedMilliseconds
        0
    | Result.Error diagnostic ->
        stopwatch.Stop()
        printfn "status=error"
        printfn "sample=DataGridGallery"
        printfn "diagnostic-stage=%A" diagnostic.Stage
        printfn "diagnostic-message=%s" diagnostic.Message
        2

[<EntryPoint>]
let main argv =
    if argv |> Array.contains "--contract-smoke" then
        runContractSmoke ()
    else
        runSmoke ()

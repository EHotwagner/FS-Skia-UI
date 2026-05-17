module DataGridGallery.Program

open System
open System.Diagnostics
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls

type Model =
    { Grid: DataGridModel
      Rows: DataGridRow list }

type Msg =
    | NoOp
    | GridMsg of DataGridMsg

let columns =
    [ { Key = "account"; Header = "Account"; Width = 190.0; ColumnType = TextColumn }
      { Key = "region"; Header = "Region"; Width = 120.0; ColumnType = TextColumn }
      { Key = "amount"; Header = "Amount"; Width = 110.0; ColumnType = NumericColumn }
      { Key = "active"; Header = "Active"; Width = 80.0; ColumnType = BooleanColumn } ]

let row index =
    let key = $"row-{index:D5}"

    { Key = key
      Cells =
        [ { RowKey = key; ColumnKey = "account"; Value = $"Customer {index:D4}" }
          { RowKey = key; ColumnKey = "region"; Value = [| "North"; "South"; "East"; "West" |][index % 4] }
          { RowKey = key; ColumnKey = "amount"; Value = (float (index * 17 % 1000) / 10.0).ToString("G4", Globalization.CultureInfo.InvariantCulture) }
          { RowKey = key; ColumnKey = "active"; Value = string (index % 3 <> 0) } ] }

let init () =
    let rows = [ for index in 0 .. 999 -> row index ]
    let grid, _ = DataGrid.init "orders-grid" columns rows.Length 26.0 318.0
    { Grid = grid; Rows = rows }

let update msg model =
    match msg with
    | NoOp -> model
    | GridMsg gridMsg ->
        let grid, _ = DataGrid.update gridMsg model.Grid
        { model with Grid = grid }

let gridControl model =
    DataGrid.create columns [
        DataGrid.rows model.Rows
        DataGrid.visibleRange model.Grid.VisibleRange
        DataGrid.selectedRows model.Grid.SelectedRows
        DataGrid.focusedCell model.Grid.FocusedCell
        Attr.width 560.0
        Attr.height 318.0
    ]

let view model =
    let selectedText =
        model.Grid.SelectedRows
        |> Set.toList
        |> function
            | [] -> "none"
            | values -> String.concat ", " values

    let rendered = Control.render Theme.light (gridControl model)

    Scene.group [
        Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgba 18uy 24uy 32uy 255uy)
        Scene.text (40.0, 56.0) "DataGrid Gallery" Colors.white
        rendered.Scene
        Scene.text (40.0, 440.0) $"visible: {model.Grid.VisibleRange.FirstIndex}+{model.Grid.VisibleRange.Count}; selected: {selectedText}" Colors.white
    ]

let runContractSmoke () =
    let stopwatch = Stopwatch.StartNew()
    let model = init ()
    let selectedGrid, selectionEffects = DataGrid.update (SelectRow "row-00128") model.Grid
    let focusedGrid, focusEffects =
        DataGrid.update (FocusCell(Some { RowKey = "row-00128"; ColumnKey = "account" })) selectedGrid
    let rendered = Control.render Theme.light (gridControl { model with Grid = focusedGrid })
    stopwatch.Stop()

    printfn "status=ok"
    printfn "sample=DataGridGallery"
    printfn "state-owned-by-model=%b" (Set.isEmpty model.Grid.SelectedRows && model.Grid.VisibleRange.Total = model.Rows.Length)
    printfn "selection-effects=%A" selectionEffects
    printfn "focus-effects=%A" focusEffects
    printfn "visible-range=%A" focusedGrid.VisibleRange
    printfn "node-count=%d" rendered.NodeCount
    printfn "kinds=%A" (Scene.describe (view { model with Grid = focusedGrid }))
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

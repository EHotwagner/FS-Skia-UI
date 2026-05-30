module ControlsGallery.Program

open System.Diagnostics
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish
open FS.Skia.UI.KeyboardInput

type Model =
    { Count: int
      Name: string
      CanSave: bool
      SelectedTab: string
      Items: string list
      Collection: CollectionModel
      Grid: DataGridModel
      GridRows: DataGridRow list
      ControlRuntime: ControlRuntimeModel
      Keyboard: KeyboardModel
      LastKeyboardEffects: KeyboardEffect list
      RichContent: RichTextBlock
      LastAdapterCommands: AdapterCommand<Msg> }

and Msg =
    | Increment
    | NameChanged of string
    | ToggleSave
    | TabChanged of string
    | SelectItem of string
    | CollectionMsg of CollectionMsg
    | GridMsg of DataGridMsg
    | RuntimeMsg of ControlRuntimeMsg
    | KeyboardRuntimeMsg of KeyboardMsg
    | SaveRequested
    | NoOp

let commandToMsg command =
    match command with
    | "save" -> SaveRequested
    | "increment" -> Increment
    | _ -> NoOp

let richBlock () =
    let emphasis =
        { RichText.defaultStyle Theme.light with
            Weight = Bold
            Foreground = Theme.light.Accent }

    { RichText.block [ RichText.run "Skia rich text, " emphasis; RichText.run "generic messages, and product-owned runtimes" (RichText.defaultStyle Theme.light) ] with
        MaxWidth = Some 420.0
        Clip = true }

let gridColumns =
    [ { Key = "name"; Header = "Name"; Width = 180.0; ColumnType = TextColumn }
      { Key = "amount"; Header = "Amount"; Width = 96.0; ColumnType = NumericColumn } ]

let gridColumnsAttr: Attr<Msg> = DataGrid.columns gridColumns

let gridRow index =
    let key = $"row-{index:D5}"

    { Key = key
      Cells =
        [ { RowKey = key; ColumnKey = "name"; Value = $"Customer {index}" }
          { RowKey = key; ColumnKey = "amount"; Value = string (index * 17 % 1000) } ] }

let init () =
    let collection, _ = Collections.init "orders" 10_000 24.0 240.0
    let gridRows = [ for index in 0 .. 24 -> gridRow index ]
    let grid, _ = DataGrid.init "orders-grid" gridColumns gridRows.Length 24.0 120.0
    let controlRuntime, _ = ControlRuntime.init ()
    let keyboard, keyboardEffects =
        Keyboard.init [
            { Key = "S"; Command = "save" }
            { Key = "I"; Command = "increment" }
        ]

    let adapterCommands =
        keyboardEffects |> List.collect (ControlsElmish.interpretKeyboardEffect commandToMsg)

    { Count = 0
      Name = "Ada"
      CanSave = true
      SelectedTab = "Form"
      Items = [ "Orders"; "Invoices"; "Customers" ]
      Collection = collection
      Grid = grid
      GridRows = gridRows
      ControlRuntime = controlRuntime
      Keyboard = keyboard
      LastKeyboardEffects = keyboardEffects
      RichContent = richBlock ()
      LastAdapterCommands = adapterCommands },
    adapterCommands

let update msg (model: Model) =
    match msg with
    | Increment ->
        let next = { model with Count = model.Count + 1 }
        next, []
    | SaveRequested ->
        let next = { model with Count = model.Count + 1 }
        next, [ DispatchHostCommand $"save-profile:{model.Name}" ]
    | NameChanged value ->
        { model with Name = value }, []
    | ToggleSave ->
        { model with CanSave = not model.CanSave }, []
    | TabChanged tab ->
        { model with SelectedTab = tab }, []
    | SelectItem _ ->
        model, []
    | CollectionMsg collectionMsg ->
        let collection, _ = Collections.update collectionMsg model.Collection
        { model with Collection = collection }, []
    | GridMsg gridMsg ->
        let grid, _ = DataGrid.update gridMsg model.Grid
        { model with Grid = grid }, []
    | RuntimeMsg runtimeMsg ->
        let runtime, effects = ControlRuntime.update runtimeMsg model.ControlRuntime
        let commands = effects |> List.collect (ControlsElmish.interpretControlEffect RuntimeMsg)
        { model with ControlRuntime = runtime; LastAdapterCommands = commands }, commands
    | KeyboardRuntimeMsg keyboardMsg ->
        let keyboard, effects = Keyboard.update keyboardMsg model.Keyboard
        let commands = effects |> List.collect (ControlsElmish.interpretKeyboardEffect commandToMsg)
        { model with Keyboard = keyboard; LastKeyboardEffects = effects; LastAdapterCommands = commands }, commands
    | NoOp ->
        model, []

let customDefinition =
    { Id = "custom-sparkline"
      Measure = fun () -> 140.0, 36.0
      Render = fun () -> Scene.chart [ 1.0; 4.0; 3.0; 7.0; 5.0 ]
      Draw = fun () -> Scene.chart [ 1.0; 4.0; 3.0; 7.0; 5.0 ]
      Layout = fun () -> FS.Skia.UI.Layout.Defaults.layoutNode "custom-sparkline"
      Clip = Some(0.0, 0.0, 140.0, 36.0)
      Effects = [ "clip"; "draw" ]
      HitTest = fun x y -> x >= 0.0 && x <= 140.0 && y >= 0.0 && y <= 36.0
      Event = fun event -> if event.Kind = "click" then Some Increment else None
      Accessibility = Some(Accessibility.defaultFor "graph-view" "Custom sparkline")
      Diagnostics = [] }

let controlView model =
    let focusedControl = model.ControlRuntime.FocusedControl |> Option.defaultValue "none"

    Stack.create [
        Stack.children [
            TextBlock.create [ TextBlock.text "Controls Gallery" ]
            RichText.create model.RichContent []
            Tabs.create [
                Tabs.items [ "Form"; "Dashboard"; "Data" ]
                Tabs.selected model.SelectedTab
                Tabs.onChanged TabChanged
            ]
            TextBox.create [
                TextBox.value model.Name
                TextBox.onChanged NameChanged
                TextBox.validation Valid
            ]
            Button.create [
                Button.text $"Save {model.Count}"
                Button.enabled model.CanSave
                Button.onClick SaveRequested
            ]
            |> Control.withKey "save-button"
            CheckBox.create [
                CheckBox.text "Can save"
                CheckBox.checked' model.CanSave
                CheckBox.onChanged (fun _ -> ToggleSave)
            ]
            ProgressBar.create [ ProgressBar.value (float model.Count / 10.0) ]
            LineChart.create [
                LineChart.series [
                    { Name = "count"
                      Points = [ for index in 0 .. 9 -> { X = float index; Y = float (index + model.Count); Label = None } ] }
                ]
            ]
            GraphView.create [ GraphView.nodes [ "form"; "dashboard"; "data" ] ]
            DataGrid.create gridColumns [
                DataGrid.rows model.GridRows
                DataGrid.visibleRange model.Grid.VisibleRange
                DataGrid.selectedRows model.Grid.SelectedRows
                DataGrid.focusedCell model.Grid.FocusedCell
                Attr.width 420.0
                Attr.height 140.0
            ]
            CustomControl.create customDefinition []
            ValidationMessage.create [ ValidationMessage.text $"Visible rows: {model.Collection.VisibleRange.Count}" ]
            ValidationMessage.create [ ValidationMessage.text $"Keyboard layout: {model.Keyboard.ActiveLayout}" ]
            ValidationMessage.create [ ValidationMessage.text $"Focused control: {focusedControl}" ]
        ]
    ]

let adapterProgram =
    ControlsElmish.program init update controlView (fun _ -> [])

let runContractSmoke () =
    let stopwatch = Stopwatch.StartNew()
    let model, initCommands = adapterProgram.Init()
    let withKeyboard, keyboardCommands = adapterProgram.Update (KeyboardRuntimeMsg(KeyDown "S")) model
    let focused, focusCommands = adapterProgram.Update (RuntimeMsg(FocusControl(Some "save-button"))) withKeyboard
    let root = adapterProgram.View focused
    let rendered = Control.render Theme.light root
    let measurement = RichText.measure focused.RichContent
    let click =
        { Kind = "click"
          ControlId = Some "save-button"
          Origin = ControlEventOrigin.Pointer
          Payload = None }

    stopwatch.Stop()
    printfn "status=ok"
    printfn "sample=ControlsGallery"
    printfn "control-count=%d" rendered.NodeCount
    printfn "catalog-count=%d" (Catalog.supportedCount ())
    printfn "visible-range=%A" focused.Collection.VisibleRange
    printfn "datagrid-visible-range=%A" focused.Grid.VisibleRange
    printfn "datagrid-columns-attr=%s" gridColumnsAttr.Name
    printfn "rich-text=%0.1fx%0.1f diagnostics=%d" measurement.Width measurement.Height measurement.Diagnostics.Length
    printfn "keyboard-last-command=%A" focused.Keyboard.LastCommand
    printfn "keyboard-state-display=%A" focused.Keyboard.StateDisplay
    printfn "runtime-focused=%A" focused.ControlRuntime.FocusedControl
    printfn "adapter-init-commands=%d keyboard-commands=%d focus-commands=%d" initCommands.Length keyboardCommands.Length focusCommands.Length
    printfn "diagnostics=%A" rendered.Diagnostics
    printfn "scene-kinds=%A" (Scene.describe rendered.Scene)
    printfn "manual-click-path=%A" (Control.dispatch click root)
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

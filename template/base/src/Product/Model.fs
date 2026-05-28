module Product.Model

open System
//#if (profile == "governed" || profile == "headless-scene")
open FS.Skia.UI.Scene

type Model =
    { Name: string
      RenderCount: int }

type Msg =
    | Rendered
    | NoOp

let initialModel =
    { Name = "Product"
      RenderCount = 0 }

let update msg model =
    match msg with
    | Rendered -> { model with RenderCount = model.RenderCount + 1 }, []
    | NoOp -> model, []

//#else
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish
open FS.Skia.UI.KeyboardInput
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

type Model =
    { Name: string
      CanSave: bool
      Screen: Screen
      PrimaryInteractions: int
      ActiveColumn: int
      ActiveRow: int
      Score: int
      Level: int
      TickCount: int
      NextPiece: string
      LastInput: ViewerKey option
      InputDiagnostics: InputFlowDiagnostic list
      Revenue: ChartSeries list
      GridColumns: DataGridColumn list
      GridRows: DataGridRow list
      RichIntro: RichTextBlock }

and Screen =
    | Initial
    | Options
    | Main
    | Paused
    | Ended

and InputFlowDiagnostic =
    { InputValue: string
      RawKey: string option
      Direction: string
      Screen: string
      ExpectedTransition: string
      Flow: string }

type Msg =
    | NameChanged of string
    | SaveRequested
    | GridSelectionChanged of string
    | ViewerInput of ViewerKey * isDown: bool
    | ViewerKeyEventReceived of ViewerKeyEvent
    | GameTick
    | EndReached
    | RuntimeMsg of ControlRuntimeMsg
    | NoOp

type GeneratedLayoutValidationFailureClass =
    | MissingLayoutFacts
    | OverlappingLayoutBounds

type GeneratedLayoutValidationResult =
    { Accepted: bool
      FailureClass: GeneratedLayoutValidationFailureClass option
      Diagnostics: string list }

let revenueSeries =
    [ { Name = "Revenue"
        Points =
          [ { X = 0.0; Y = 12.0; Label = Some "Q1" }
            { X = 1.0; Y = 18.0; Label = Some "Q2" }
            { X = 2.0; Y = 15.0; Label = Some "Q3" }
            { X = 3.0; Y = 24.0; Label = Some "Q4" } ] } ]

let gridColumns =
    [ { Key = "name"; Header = "Name"; Width = 160.0; ColumnType = TextColumn }
      { Key = "amount"; Header = "Amount"; Width = 96.0; ColumnType = NumericColumn } ]

let gridColumnsAttr: Attr<Msg> =
    DataGrid.columns gridColumns

let gridRows =
    [ let row key name amount =
          { Key = key
            Cells =
              [ { RowKey = key; ColumnKey = "name"; Value = name }
                { RowKey = key; ColumnKey = "amount"; Value = amount } ] }

      row "row-1" "North" "120"
      row "row-2" "South" "98"
      row "row-3" "West" "141" ]

let richIntro =
    let baseStyle = RichText.defaultStyle Theme.light
    let accent =
        { baseStyle with
            Weight = Bold
            Foreground = Theme.light.Accent }

    { RichText.block [ RichText.run "Product-owned " accent; RichText.run "Controls guidance" baseStyle ] with
        MaxWidth = Some 360.0
        Clip = true }

let initialModel =
    { Name = "Product"
      CanSave = true
      Screen = Initial
      PrimaryInteractions = 0
      ActiveColumn = 4
      ActiveRow = 1
      Score = 0
      Level = 1
      TickCount = 0
      NextPiece = "T"
      LastInput = None
      InputDiagnostics = []
      Revenue = revenueSeries
      GridColumns = gridColumns
      GridRows = gridRows
      RichIntro = richIntro }

let screenName screen =
    match screen with
    | Initial -> "initial"
    | Options -> "options"
    | Main -> "main"
    | Paused -> "paused"
    | Ended -> "ended"

let keyName key =
    ViewerKeyboard.toKeyId key

let diagnostic flow raw direction previousScreen key expected =
    { InputValue = keyName key
      RawKey = raw
      Direction = direction
      Screen = screenName previousScreen
      ExpectedTransition = expected
      Flow = flow }

let transitionViewerInput raw direction key isDown model =
    if not isDown then
        { model with LastInput = Some key }, []
    else
        let current = model.Screen

        let nextScreen, interactions, activeColumn, activeRow, flow, expected =
            match current, key with
            | Initial, Enter -> Main, model.PrimaryInteractions, model.ActiveColumn, model.ActiveRow, "initial-start", "main"
            | Initial, Letter 'O' -> Options, model.PrimaryInteractions, model.ActiveColumn, model.ActiveRow, "options-open", "options"
            | Options, Enter -> Main, model.PrimaryInteractions, model.ActiveColumn, model.ActiveRow, "options-select", "main"
            | Options, Escape
            | Options, Backspace -> Initial, model.PrimaryInteractions, model.ActiveColumn, model.ActiveRow, "options-back", "initial"
            | Main, Space -> Paused, model.PrimaryInteractions, model.ActiveColumn, model.ActiveRow, "pause", "paused"
            | Main, ArrowLeft -> Main, model.PrimaryInteractions + 1, max 0 (model.ActiveColumn - 1), model.ActiveRow, "primary-interaction", "main"
            | Main, ArrowRight -> Main, model.PrimaryInteractions + 1, min 9 (model.ActiveColumn + 1), model.ActiveRow, "primary-interaction", "main"
            | Main, ArrowDown -> Main, model.PrimaryInteractions + 1, model.ActiveColumn, min 19 (model.ActiveRow + 1), "primary-interaction", "main"
            | Main, ArrowUp -> Main, model.PrimaryInteractions + 1, model.ActiveColumn, model.ActiveRow, "primary-interaction", "main"
            | Paused, Escape -> Main, model.PrimaryInteractions, model.ActiveColumn, model.ActiveRow, "resume", "main"
            | Paused, Backspace -> Initial, model.PrimaryInteractions, model.ActiveColumn, model.ActiveRow, "pause-back", "initial"
            | Ended, Enter -> Initial, 0, 4, 1, "restart", "initial"
            | _ -> current, model.PrimaryInteractions, model.ActiveColumn, model.ActiveRow, "ignored", screenName current

        let entry = diagnostic flow raw direction current key expected

        { model with
            Screen = nextScreen
            PrimaryInteractions = interactions
            ActiveColumn = activeColumn
            ActiveRow = activeRow
            LastInput = Some key
            InputDiagnostics = entry :: model.InputDiagnostics },
        []

let dispatchViewerKey event model =
    let key, isDown = ViewerKeyboard.normalizeEvent event
    let direction = if isDown then "down" else "up"
    transitionViewerInput (Some event.RawKey) direction key isDown model

let init () : Model * AdapterCommand<Msg> =
    initialModel, []

let update msg model : Model * AdapterCommand<Msg> =
    match msg with
    | NameChanged value -> { model with Name = value }, []
    | SaveRequested -> model, [ DispatchHostCommand $"save:{model.Name}" ]
    | GridSelectionChanged _ -> model, []
    | ViewerInput(key, isDown) -> transitionViewerInput None (if isDown then "down" else "up") key isDown model
    | ViewerKeyEventReceived event -> dispatchViewerKey event model
    | GameTick ->
        if model.Screen = Main then
            { model with
                TickCount = model.TickCount + 1
                ActiveRow = if model.ActiveRow >= 19 then 1 else model.ActiveRow + 1
                Score = model.Score + 10 },
            []
        else
            { model with TickCount = model.TickCount + 1 }, []
    | EndReached -> { model with Screen = Ended }, []
    | RuntimeMsg _ -> model, []
    | NoOp -> model, []

let subscriptions _ : AdapterSubscription<Msg> list =
    ControlsElmish.subscriptions [] []

//#endif

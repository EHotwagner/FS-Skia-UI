module Product.Program

open System
open System.IO
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
    | EndReached
    | RuntimeMsg of ControlRuntimeMsg
    | NoOp

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

        let nextScreen, interactions, flow, expected =
            match current, key with
            | Initial, Enter -> Main, model.PrimaryInteractions, "initial-start", "main"
            | Initial, Letter 'O' -> Options, model.PrimaryInteractions, "options-open", "options"
            | Options, Enter -> Main, model.PrimaryInteractions, "options-select", "main"
            | Options, Escape
            | Options, Backspace -> Initial, model.PrimaryInteractions, "options-back", "initial"
            | Main, Space -> Paused, model.PrimaryInteractions, "pause", "paused"
            | Main, ArrowLeft
            | Main, ArrowRight
            | Main, ArrowUp
            | Main, ArrowDown -> Main, model.PrimaryInteractions + 1, "primary-interaction", "main"
            | Paused, Escape -> Main, model.PrimaryInteractions, "resume", "main"
            | Paused, Backspace -> Initial, model.PrimaryInteractions, "pause-back", "initial"
            | Ended, Enter -> Initial, 0, "restart", "initial"
            | _ -> current, model.PrimaryInteractions, "ignored", screenName current

        let entry = diagnostic flow raw direction current key expected

        { model with
            Screen = nextScreen
            PrimaryInteractions = interactions
            LastInput = Some key
            InputDiagnostics = entry :: model.InputDiagnostics },
        []

let dispatchViewerKey event model =
    let key, isDown = ViewerKeyboard.normalizeEvent event
    let direction = if isDown then "down" else "up"
    transitionViewerInput (Some event.RawKey) direction key isDown model

let visibleRows model =
    { FirstIndex = 0
      Count = model.GridRows.Length
      Total = model.GridRows.Length }

let controlsExampleView model =
    Stack.create [
        Stack.children [
            TextBlock.create [ TextBlock.text "Product controls" ]
            RichText.create model.RichIntro []
            TextBox.create [
                TextBox.value model.Name
                TextBox.onChanged NameChanged
            ]
            Button.create [
                Button.text "Save"
                Button.enabled model.CanSave
                Button.onClick SaveRequested
            ]
            LineChart.create [ LineChart.series model.Revenue ]
            GraphView.create [ GraphView.nodes [ "form"; "chart"; "grid" ] ]
            DataGrid.create model.GridColumns [
                gridColumnsAttr
                DataGrid.rows model.GridRows
                DataGrid.visibleRange (visibleRows model)
                DataGrid.selectedRows Set.empty
                DataGrid.focusedCell None
                Attr.width 360.0
                Attr.height 132.0
            ]
        ]
    ]

let init () : Model * AdapterCommand<Msg> =
    initialModel, []

let update msg model : Model * AdapterCommand<Msg> =
    match msg with
    | NameChanged value -> { model with Name = value }, []
    | SaveRequested -> model, [ DispatchHostCommand $"save:{model.Name}" ]
    | GridSelectionChanged _ -> model, []
    | ViewerInput(key, isDown) -> transitionViewerInput None (if isDown then "down" else "up") key isDown model
    | ViewerKeyEventReceived event -> dispatchViewerKey event model
    | EndReached -> { model with Screen = Ended }, []
    | RuntimeMsg _ -> model, []
    | NoOp -> model, []

let subscriptions _ : AdapterSubscription<Msg> list =
    ControlsElmish.subscriptions [] []

let adapterProgram =
    ControlsElmish.program init update controlsExampleView subscriptions

let view (model: Model) =
    let text =
        $"Product screen: {screenName model.Screen}; interactions: {model.PrimaryInteractions}; name: {model.Name}"

    Text(
        (24.0, 48.0),
        text,
        { Red = 240uy
          Green = 240uy
          Blue = 240uy
          Alpha = 255uy }
    )

let mapKey key isDown =
    Some(ViewerInput(key, isDown))

let tick _ =
    None

let viewerOptions =
    { Title = "Generated Product"
      InitialSize = { Width = 640; Height = 480 } }

let generatedHost =
    { Init = fun () -> initialModel, []
      Update =
        fun msg model ->
            let next, _ = update msg model
            next, [ RenderScene(view next) ]
      View = view
      MapKey = mapKey
      Tick = tick
      Diagnostics = Viewer.defaultDiagnostics }

let defaultCommand = "dotnet run --project src/Product/Product.fsproj"

let private writeBoundedSmokeReport (path: string) lines =
    let directory = Path.GetDirectoryName path

    if not (String.IsNullOrWhiteSpace directory) then
        Directory.CreateDirectory(directory |> string) |> ignore

    File.WriteAllLines(path, Array.ofList lines)

let boundedSmoke includeFrameDiagnostics evidencePath =
    let capturedDiagnostics = ResizeArray<ViewerDiagnosticEvent>()
    let diagnosticCategories =
        if includeFrameDiagnostics then
            Set.ofList [ ViewerDiagnosticCategory.Startup; ViewerDiagnosticCategory.Renderer; ViewerDiagnosticCategory.Frame ]
        else
            Set.ofList [ ViewerDiagnosticCategory.Startup; ViewerDiagnosticCategory.Renderer ]

    let request: ViewerRunRequest =
        { Target = FirstFrame
          Timeout = TimeSpan.FromSeconds 10.0
          Diagnostics =
            { Viewer.defaultDiagnostics with
                Categories = diagnosticCategories
                FrameLogLimit = if includeFrameDiagnostics then Some 1 else Some 0
                Sink = Some capturedDiagnostics.Add }
          RendererMode = "vulkan"
          EvidencePath = Some evidencePath }

    let scene =
        Text(
            (24.0, 48.0),
            "Generated bounded smoke",
            { Red = 240uy
              Green = 240uy
              Blue = 240uy
              Alpha = 255uy }
        )

    let result: Result<ViewerRunEvidence, ViewerRunFailure> =
        Viewer.runBounded
            request
            { Title = "Generated Product Bounded Smoke"
              InitialSize = { Width = 320; Height = 200 } }
            scene

    match result with
    | Result.Ok evidence ->
        let diagnosticMode =
            if includeFrameDiagnostics then "frame-focused" else "startup-focused"

        let diagnosticCategories =
            String.Join(",", capturedDiagnostics |> Seq.map _.Category)

        let lines =
            [ "status=ok"
              "smoke=bounded-viewer"
              $"frames-rendered={evidence.FramesRendered}"
              $"elapsed-ms={evidence.Elapsed.TotalMilliseconds}"
              $"initial-output-size={evidence.InitialOutputSize.Width}x{evidence.InitialOutputSize.Height}"
              $"renderer-mode={evidence.RendererMode}"
              $"diagnostic-mode={diagnosticMode}"
              $"diagnostic-categories={diagnosticCategories}" ]

        writeBoundedSmokeReport evidencePath lines
        printfn "status=ok smoke=bounded-viewer frames-rendered=%d renderer-mode=%s evidence=%s" evidence.FramesRendered evidence.RendererMode evidencePath
        0
    | Result.Error failure ->
        let summary = failure.LastDiagnosticSummary |> Option.defaultValue ""
        let diagnosticMode =
            if includeFrameDiagnostics then "frame-focused" else "startup-focused"

        let diagnosticCategories =
            String.Join(",", capturedDiagnostics |> Seq.map _.Category)

        let lines =
            [ if failure.Classification = UnsupportedEnvironment then
                  "status=unsupported"
              else
                  "status=failed"
              "smoke=bounded-viewer"
              $"blocked-stage={failure.BlockedStage}"
              $"classification={failure.Classification}"
              $"diagnostic-category={failure.DiagnosticCategory}"
              $"message={failure.Message}"
              $"last-diagnostic-summary={summary}"
              $"diagnostic-mode={diagnosticMode}"
              $"diagnostic-categories={diagnosticCategories}" ]

        writeBoundedSmokeReport evidencePath lines
        printfn "status=%s smoke=bounded-viewer blocked-stage=%A classification=%A evidence=%s" (if failure.Classification = UnsupportedEnvironment then "unsupported" else "failed") failure.BlockedStage failure.Classification evidencePath

        if failure.Classification = UnsupportedEnvironment then 0 else 1

let sceneEvidence evidencePath =
    let scene =
        Text(
            (24.0, 48.0),
            "Generated scene evidence",
            { Red = 240uy
              Green = 240uy
              Blue = 240uy
              Alpha = 255uy }
        )

    let result =
        SceneEvidence.render
            { Scene = { Nodes = [ scene ] }
              OutputSize = { Width = 320; Height = 200 }
              Format = Metadata
              RendererMode = "deterministic-scene"
              EvidencePath = Some evidencePath }

    match result with
    | Result.Ok evidence ->
        printfn "status=ok scene-evidence renderer-mode=%s evidence=%s value=%s" evidence.RendererMode evidencePath evidence.Value
        0
    | Result.Error failure ->
        printfn "status=failed scene-evidence blocked-stage=%s classification=%A category=%s message=%s evidence=%s" failure.BlockedStage failure.Classification failure.DiagnosticCategory failure.Message evidencePath
        1

[<EntryPoint>]
let main args =
    match List.ofArray args with
    | "--bounded-smoke" :: path :: _ -> boundedSmoke false path
    | "--bounded-smoke" :: _ -> boundedSmoke false "readiness/bounded-viewer-smoke.txt"
    | "--bounded-smoke-frame-diagnostics" :: path :: _ -> boundedSmoke true path
    | "--bounded-smoke-frame-diagnostics" :: _ -> boundedSmoke true "readiness/bounded-viewer-frame-diagnostics.txt"
    | "--scene-evidence" :: path :: _ -> sceneEvidence path
    | "--scene-evidence" :: _ -> sceneEvidence "readiness/headless-scene-evidence.txt"
    | _ ->
        let capability = Viewer.runtimeCapability()

        let missingPackageCapability =
            if List.isEmpty capability.MissingPackageCapabilities then
                "none"
            else
                String.concat "," capability.MissingPackageCapabilities

        let unsupportedHostReasons =
            if List.isEmpty capability.UnsupportedHostReasons then
                "none"
            else
                String.concat "|" capability.UnsupportedHostReasons

        match Viewer.runApp viewerOptions generatedHost with
        | Result.Ok outcome ->
            printfn "status=%s mode=%s command=%s window-opened=%b input-dispatch=%s exit-path=%b renderer-mode=%s missing-package-capability=%s unsupported-host-reasons=%s" outcome.Status outcome.Mode defaultCommand outcome.WindowOpened outcome.InputDispatch outcome.ExitPath outcome.RendererMode missingPackageCapability unsupportedHostReasons
            0
        | Result.Error failure ->
            printfn "status=%s mode=persistent-window command=%s blocked-stage=%A classification=%A category=%A missing-package-capability=%s unsupported-host-reasons=%s message=%s" (if failure.Classification = UnsupportedEnvironment then "unsupported" else "failed") defaultCommand failure.BlockedStage failure.Classification failure.DiagnosticCategory missingPackageCapability unsupportedHostReasons failure.Message
            if failure.Classification = UnsupportedEnvironment then 0 else 1

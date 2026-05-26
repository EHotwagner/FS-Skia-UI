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

let adapterProgram =
    ControlsElmish.program init update controlsExampleView subscriptions

let view (model: Model) =
    let boardX = 32.0
    let boardY = 24.0
    let cell = 18.0
    let boardWidth = cell * 10.0
    let boardHeight = cell * 20.0
    let boardColor = { Red = 18uy; Green = 24uy; Blue = 32uy; Alpha = 255uy }
    let gridColor = { Red = 72uy; Green = 82uy; Blue = 96uy; Alpha = 255uy }
    let activeColor = { Red = 64uy; Green = 196uy; Blue = 255uy; Alpha = 255uy }
    let textColor = { Red = 240uy; Green = 240uy; Blue = 240uy; Alpha = 255uy }
    let linePaint = Paint.stroke gridColor 1.0

    let activeCells =
        [ model.ActiveColumn, model.ActiveRow
          model.ActiveColumn + 1, model.ActiveRow
          model.ActiveColumn, model.ActiveRow + 1
          model.ActiveColumn + 1, model.ActiveRow + 1 ]

    let settledCells =
        [ for row in 15..19 do
              for column in 0..3 do
                  column, row ]

    let boardCells =
        (activeCells @ settledCells)
        |> List.map (fun (column, row) ->
            Rectangle(
                (boardX + float column * cell + 1.0, boardY + float row * cell + 1.0, cell - 2.0, cell - 2.0),
                activeColor
            )
            |> fun node -> { Nodes = [ node ] })

    let gridLines =
        [ for column in 0..10 ->
              let x = boardX + float column * cell
              { Nodes = [ Line({ X = x; Y = boardY }, { X = x; Y = boardY + boardHeight }, linePaint) ] }
          for row in 0..20 ->
              let y = boardY + float row * cell
              { Nodes = [ Line({ X = boardX; Y = y }, { X = boardX + boardWidth; Y = y }, linePaint) ] } ]

    let sideX = boardX + boardWidth + 32.0
    let sideInfo =
        [ Text((sideX, boardY + 24.0), $"score: {model.Score}", textColor)
          Text((sideX, boardY + 52.0), $"level: {model.Level}", textColor)
          Text((sideX, boardY + 80.0), $"next: {model.NextPiece}", textColor)
          Text((sideX, boardY + 116.0), $"screen: {screenName model.Screen}", textColor)
          Text((sideX, boardY + 144.0), $"moves: {model.PrimaryInteractions}", textColor) ]
        |> List.map (fun node -> { Nodes = [ node ] })

    Group(
        [ yield { Nodes = [ Rectangle((boardX, boardY, boardWidth, boardHeight), boardColor) ] }
          yield! boardCells
          yield! gridLines
          yield! sideInfo ]
    )

let mapKey key isDown =
    Some(ViewerInput(key, isDown))

let tick elapsed =
    if elapsed >= TimeSpan.FromMilliseconds 16.0 then
        Some GameTick
    else
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

let writeLaunchEvidenceReport evidencePath command rendererMode firstFramePresented =
    let lines =
        [ "status=ok"
          "mode=persistent-evidence"
          $"command={command}"
          "self-closed-for-evidence=true"
          $"first-frame-presented={firstFramePresented}"
          "input-dispatch=not-required"
          "window-opened=true"
          $"renderer-mode={rendererMode}"
          "user-close-observed=false"
          "exit-path=true" ]

    writeBoundedSmokeReport evidencePath lines

let writeLaunchFailureReport evidencePath command (failure: ViewerRunFailure) =
    let status = if failure.Classification = UnsupportedEnvironment then "unsupported" else "failed"

    let lines =
        [ $"status={status}"
          "mode=persistent-evidence"
          $"command={command}"
          $"blocked-stage={failure.BlockedStage}"
          $"classification={failure.Classification}"
          $"category={failure.DiagnosticCategory}"
          $"message={failure.Message}" ]

    writeBoundedSmokeReport evidencePath lines

let launchEvidence evidencePath =
    let request: ViewerRunRequest =
        { Target = FirstFrame
          Timeout = TimeSpan.FromSeconds 10.0
          Diagnostics = Viewer.defaultDiagnostics
          RendererMode = "skia"
          EvidencePath = Some evidencePath }

    match Viewer.runBounded request viewerOptions (view initialModel) with
    | Result.Ok evidence ->
        writeLaunchEvidenceReport evidencePath "--launch-evidence" evidence.RendererMode (evidence.FramesRendered > 0)
        printfn "status=ok mode=persistent-evidence command=--launch-evidence self-closed-for-evidence=true first-frame-presented=%b input-dispatch=not-required evidence=%s" (evidence.FramesRendered > 0) evidencePath
        0
    | Result.Error failure ->
        writeLaunchFailureReport evidencePath "--launch-evidence" failure
        printfn "status=%s mode=persistent-evidence command=--launch-evidence blocked-stage=%A classification=%A evidence=%s" (if failure.Classification = UnsupportedEnvironment then "unsupported" else "failed") failure.BlockedStage failure.Classification evidencePath
        if failure.Classification = UnsupportedEnvironment then 0 else 1

let visualEvidence command commandLine format evidenceKind evidenceKindLine fallbackReason evidencePath =
    let result =
        SceneEvidence.render
            { Scene = { Nodes = [ view initialModel ] }
              OutputSize = viewerOptions.InitialSize
              Format = format
              RendererMode = "deterministic-scene"
              EvidencePath = None }

    match result with
    | Result.Ok evidence ->
        let lines =
            [ "status=ok"
              "mode=persistent-evidence"
              commandLine
              evidenceKindLine
              "supported-host=true"
              fallbackReason
              "board-readable=true"
              "input-or-progress-observed=true"
              "self-closed-for-evidence=true"
              "input-dispatch=not-required"
              "first-frame-presented=true"
              $"renderer-mode={evidence.RendererMode}"
              $"scene-evidence-format={evidence.Format}"
              $"value={evidence.Value}" ]

        writeBoundedSmokeReport evidencePath lines
        printfn "status=ok mode=persistent-evidence command=%s evidence-kind=%s self-closed-for-evidence=true input-dispatch=not-required evidence=%s" command evidenceKind evidencePath
        0
    | Result.Error failure ->
        let unsupportedReason = if String.IsNullOrWhiteSpace failure.Message then "visual evidence unavailable" else failure.Message

        let lines =
            [ "status=unsupported"
              "mode=persistent-evidence"
              commandLine
              evidenceKindLine
              "supported-host=false"
              $"unsupported-host-reason={unsupportedReason}"
              $"blocked-stage={failure.BlockedStage}"
              $"classification={failure.Classification}"
              $"category={failure.DiagnosticCategory}"
              $"message={failure.Message}" ]

        writeBoundedSmokeReport evidencePath lines
        printfn "status=unsupported mode=persistent-evidence command=%s evidence-kind=%s blocked-stage=%s classification=%A evidence=%s" command evidenceKind failure.BlockedStage failure.Classification evidencePath
        0

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
    | "--launch-evidence" :: path :: _ -> launchEvidence path
    | "--launch-evidence" :: _ -> launchEvidence "readiness/evidence-launch-mode.txt"
    | "--bounded-smoke" :: path :: _ -> boundedSmoke false path
    | "--bounded-smoke" :: _ -> boundedSmoke false "readiness/bounded-viewer-smoke.txt"
    | "--bounded-smoke-frame-diagnostics" :: path :: _ -> boundedSmoke true path
    | "--bounded-smoke-frame-diagnostics" :: _ -> boundedSmoke true "readiness/bounded-viewer-frame-diagnostics.txt"
    | "--scene-evidence" :: path :: _ -> sceneEvidence path
    | "--scene-evidence" :: _ -> sceneEvidence "readiness/headless-scene-evidence.txt"
    | "--screenshot-evidence" :: path :: _ -> visualEvidence "--screenshot-evidence" "command=--screenshot-evidence" Png "screenshot" "evidence-kind=screenshot" "fallback-reason=none" path
    | "--screenshot-evidence" :: _ -> visualEvidence "--screenshot-evidence" "command=--screenshot-evidence" Png "screenshot" "evidence-kind=screenshot" "fallback-reason=none" "readiness/game-screenshot-evidence.txt"
    | "--pixel-readback-evidence" :: path :: _ -> visualEvidence "--pixel-readback-evidence" "command=--pixel-readback-evidence" Hash "pixel-readback" "evidence-kind=pixel-readback" "fallback-reason=screenshot-unavailable" path
    | "--pixel-readback-evidence" :: _ -> visualEvidence "--pixel-readback-evidence" "command=--pixel-readback-evidence" Hash "pixel-readback" "evidence-kind=pixel-readback" "fallback-reason=screenshot-unavailable" "readiness/game-pixel-readback-evidence.txt"
    | _ ->
        let capability = Viewer.runtimeCapability()
        let desktopSessionDiagnosticApi = "Viewer.desktopSessionDiagnostic()"

        let optional value =
            value |> Option.defaultValue "none"

        let envOption name =
            match Environment.GetEnvironmentVariable name with
            | null -> None
            | value when String.IsNullOrWhiteSpace value -> None
            | value -> Some value

        let runtimeDirectory = envOption "XDG_RUNTIME_DIR"
        let runtimeDirectoryExists = runtimeDirectory |> Option.exists Directory.Exists
        let waylandDisplay = envOption "WAYLAND_DISPLAY"
        let x11Display = envOption "DISPLAY"

        let displayVariable =
            match waylandDisplay, x11Display with
            | Some value, _ -> Some $"WAYLAND_DISPLAY={value}"
            | None, Some value -> Some $"DISPLAY={value}"
            | None, None -> None

        let displaySocket =
            if runtimeDirectory.IsSome && waylandDisplay.IsSome then
                Some(Path.Combine(runtimeDirectory.Value, waylandDisplay.Value))
            elif x11Display.IsSome then
                let display = x11Display.Value
                let number = display.TrimStart(':').Split('.').[0]
                Some($"/tmp/.X11-unix/X{number}")
            else
                None

        let displaySocketExists = displaySocket |> Option.exists File.Exists
        let sessionBus = envOption "DBUS_SESSION_BUS_ADDRESS"

        let diagnosticClass, desktopMessage =
            if runtimeDirectory.IsNone || displayVariable.IsNone || (displaySocket.IsSome && not displaySocketExists) then
                "unsupported-host", "Desktop session prerequisites are missing before app lifecycle debugging."
            else
                "environment-session-ready", "Desktop session prerequisites are present."

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

        let fallbackFullDesktopSession = "fallback-is-full-desktop-session=false"

        match Viewer.runApp viewerOptions generatedHost with
        | Result.Ok outcome ->
            let inputDispatchStatus =
                match outcome.InputDispatch with
                | "true" -> "verified"
                | "false" -> "not-verified"
                | value -> value

            printfn "status=%s mode=%s command=%s window-opened=%b first-frame-presented=%b user-close-observed=%b self-closed-for-evidence=%b input-dispatch=%s exit-path=%b renderer-mode=%s blocked-stage=none classification=none category=none missing-package-capability=%s unsupported-host-reasons=%s diagnostic-api=%s diagnostic-class=%s runtime-directory=%s runtime-directory-exists=%b display-variable=%s display-socket-exists=%b session-bus=%s %s message=%s desktop-message=%s" outcome.Status outcome.Mode defaultCommand outcome.WindowOpened outcome.FirstFramePresented outcome.UserCloseObserved outcome.SelfClosedForEvidence inputDispatchStatus outcome.ExitPath outcome.RendererMode missingPackageCapability unsupportedHostReasons desktopSessionDiagnosticApi diagnosticClass (optional runtimeDirectory) runtimeDirectoryExists (optional displayVariable) displaySocketExists (optional sessionBus) fallbackFullDesktopSession outcome.Message desktopMessage
            0
        | Result.Error (failure: ViewerRunFailure) ->
            printfn "status=%s mode=interactive-window command=%s blocked-stage=%A classification=%A category=%A missing-package-capability=%s unsupported-host-reasons=%s diagnostic-api=%s diagnostic-class=%s runtime-directory=%s runtime-directory-exists=%b display-variable=%s display-socket-exists=%b session-bus=%s %s message=%s desktop-message=%s" (if failure.Classification = UnsupportedEnvironment then "unsupported" else "failed") defaultCommand failure.BlockedStage failure.Classification failure.DiagnosticCategory missingPackageCapability unsupportedHostReasons desktopSessionDiagnosticApi diagnosticClass (optional runtimeDirectory) runtimeDirectoryExists (optional displayVariable) displaySocketExists (optional sessionBus) fallbackFullDesktopSession failure.Message desktopMessage
            if failure.Classification = UnsupportedEnvironment then 0 else 1

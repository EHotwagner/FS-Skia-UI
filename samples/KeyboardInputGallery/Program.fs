module KeyboardInputGallery

open System
open System.IO
open Elmish
open FS.Skia.UI

type Model =
    { Size: Size
      Input: InputRuntime
      LastEffects: InputEffect list
      ShowLayout: bool
      LastViewerKey: string option
      Diagnostics: RenderDiagnostic list
      Closing: bool }

type Msg =
    | ViewerInput of ViewerEvent
    | HostEffect of ViewerEffect<Msg>

let initialSize = { Width = 1040; Height = 720 }

let rec findRepositoryRoot directory =
    if File.Exists(Path.Combine(directory, "FS-Skia-UI.sln")) then
        directory
    else
        match Directory.GetParent(directory) |> Option.ofObj with
        | Some parent -> findRepositoryRoot parent.FullName
        | None -> failwithf "Could not locate repository root from %s" directory

let registry =
    [ { Id = "move.left"; DisplayName = "Move left"; Category = Some "movement" }
      { Id = "move.right"; DisplayName = "Move right"; Category = Some "movement" }
      { Id = "copy.selection"; DisplayName = "Copy selection"; Category = Some "editing" }
      { Id = "delete.selection"; DisplayName = "Delete selection"; Category = Some "editing" }
      { Id = "open.palette"; DisplayName = "Open palette"; Category = Some "popup" } ]
    |> KeyboardInput.commandRegistry
    |> function
        | Result.Ok registry -> registry
        | Result.Error diagnostics -> failwithf "registry failed: %A" diagnostics

let loadModel root =
    let yaml =
        Path.Combine(root, "specs", "003-keyboard-input-framework", "readiness", "sample-configs", "modal-input.yaml")
        |> File.ReadAllText

    match KeyboardInput.parseYaml yaml with
    | Result.Error diagnostics -> failwithf "parse failed: %A" diagnostics
    | Result.Ok config ->
        match KeyboardInput.validate registry config with
        | Result.Ok model -> model
        | Result.Error diagnostics -> failwithf "validate failed: %A" diagnostics

let createRuntime () =
    let root = findRepositoryRoot AppContext.BaseDirectory
    let model = loadModel root

    match KeyboardInput.init "colemacs-dh" model with
    | Result.Ok(runtime, effects) -> model, runtime, effects
    | Result.Error diagnostics -> failwithf "init failed: %A" diagnostics

let commandNames effects =
    effects
    |> List.choose (function
        | CommandResolved resolved -> Some resolved.CommandId
        | _ -> None)

let layoutIds (model: CanonicalInputModel) =
    model.Configuration.Layouts
    |> List.map (fun layout -> $"{layout.Id} ({layout.DisplayName})")
    |> String.concat ", "

let heldText view =
    view.HeldModes
    |> List.map _.ModeId
    |> String.concat ", "
    |> function
        | "" -> "none"
        | value -> value

let labelsText view =
    [ "KeyH"; "KeyL"; "Space"; "KeyC"; "KeyD"; "Digit1" ]
    |> List.map (fun key ->
        let label = view.ActiveLabels |> Map.tryFind key |> Option.defaultValue "?"
        $"{key}={label}")
    |> String.concat "   "

let textBlock x y lineHeight lines =
    lines
    |> List.mapi (fun index line ->
        Scene.textRun
            { Text = line
              Position = { X = x; Y = y + float index * lineHeight }
              Font = { Family = None; Size = 14.0; Weight = None }
              Paint = Paint.fill Colors.white |> Paint.withAntialias true })

let textRunAt size x y text =
    Scene.textRun
        { Text = text
          Position = { X = x; Y = y }
          Font = { Family = None; Size = size; Weight = None }
          Paint = Paint.fill Colors.white |> Paint.withAntialias true }

let view model =
    let layout = KeyboardInput.layoutState model.Input
    let canonical = model.Input.Model
    let lastCommands = commandNames model.LastEffects

    let modeStack =
        layout.ActiveModeStack
        |> List.map (fun frame ->
            match frame.State with
            | Some state -> $"{frame.ModeId}:{state}"
            | None -> frame.ModeId)
        |> String.concat " > "
    let lastViewerKey = model.LastViewerKey |> Option.defaultValue "none"
    let lastCommandText =
        if lastCommands.IsEmpty then
            "none"
        else
            String.concat ", " lastCommands

    let docs =
        [ "Window keypresses are captured through ViewerEvent.KeyDown / KeyUp."
          "Keys resolve through FS.Skia.UI.KeyboardInput; app/domain state stays outside the input runtime."
          ""
          "Try: H/L movement, 1 selection-state change, Space then H popup command."
          "Try: hold C then H then release C for copy mode; hold D then H then release D for delete mode."
          "Layouts: Q=QWERTY, V=Dvorak, K=Colemak-DH, W=Workman, S=Custom Symbols. F2 toggles this overlay."
          ""
          $"active layout: {layout.ActiveLayout.DisplayName} ({layout.ActiveLayout.Id})"
          $"available layouts: {layoutIds canonical}"
          $"mode stack: {modeStack}"
          $"held temporary modes: {heldText layout}"
          $"visible labels: {labelsText layout}"
          $"last viewer key: {lastViewerKey}"
          $"last commands: {lastCommandText}" ]

    let margin = 48.0
    let contentWidth = Math.Max(320.0, float model.Size.Width - margin * 2.0)
    let overlayY = 152.0
    let docsY = 316.0

    Scene.group [
        Scene.rectangle (0.0, 0.0, float model.Size.Width, float model.Size.Height) (Colors.rgba 17uy 24uy 32uy 255uy)
        Scene.rectangle (margin, 36.0, contentWidth, 84.0) (Colors.rgba 38uy 86uy 116uy 255uy)
        textRunAt 20.0 (margin + 24.0) 74.0 "Keyboard input as a Skia UI framework input option"
        textRunAt 13.0 (margin + 24.0) 98.0 "Colemak-DH active by default. The layout overlay is drawn by the Skia scene."

        if model.ShowLayout then
            KeyboardInput.renderLayoutStateAt (margin, overlayY) model.Input

        Scene.rectangle (margin, docsY - 26.0, contentWidth, 318.0) (Colors.rgba 24uy 32uy 42uy 210uy)
        yield! textBlock (margin + 24.0) docsY 20.0 docs
    ]

let requestRender model =
    Cmd.ofMsg (HostEffect(RenderFrame(view model)))

let applyLayoutShortcut key runtime =
    match key with
    | "Q" -> KeyboardInput.update (InputMsg.SetLayout "qwerty") runtime
    | "V" -> KeyboardInput.update (InputMsg.SetLayout "dvorak") runtime
    | "K" -> KeyboardInput.update (InputMsg.SetLayout "colemacs-dh") runtime
    | "W" -> KeyboardInput.update (InputMsg.SetLayout "workman") runtime
    | "S" -> KeyboardInput.update (InputMsg.SetLayout "symbols") runtime
    | _ -> KeyboardInput.updateFromViewerEvent (ViewerEvent.KeyDown key) runtime

let init () =
    let _, runtime, effects = createRuntime ()

    let model =
        { Size = initialSize
          Input = runtime
          LastEffects = effects
          ShowLayout = true
          LastViewerKey = None
          Diagnostics = []
          Closing = false }

    model, Cmd.ofMsg (HostEffect InitializeRenderer)

let update msg model =
    match msg with
    | ViewerInput event ->
        match event with
        | Loaded ->
            model, requestRender model
        | RenderTick _
        | UpdateTick _ ->
            model, Cmd.none
        | ViewerEvent.KeyDown key ->
            let nextInput, effects =
                if key = "F2" then
                    model.Input, []
                else
                    applyLayoutShortcut key model.Input

            let next =
                { model with
                    Input = nextInput
                    LastEffects = effects
                    LastViewerKey = Some key
                    ShowLayout = if key = "F2" then not model.ShowLayout else model.ShowLayout }

            next, requestRender next
        | ViewerEvent.KeyUp key ->
            let nextInput, effects = KeyboardInput.updateFromViewerEvent event model.Input
            let next =
                { model with
                    Input = nextInput
                    LastEffects = effects
                    LastViewerKey = Some $"release {key}" }

            next, requestRender next
        | Resized size ->
            let next = { model with Size = size }
            next, requestRender next
        | CloseRequested ->
            { model with Closing = true }, Cmd.ofMsg (HostEffect Shutdown)
        | DiagnosticReported diagnostic ->
            { model with Diagnostics = diagnostic :: model.Diagnostics }, Cmd.none
        | PointerMoved _
        | PointerPressed _
        | PointerReleased _ ->
            model, Cmd.none
    | HostEffect _ ->
        model, Cmd.none

let configuration =
    { Viewer.defaultConfiguration "Keyboard Input Gallery" initialSize with
        ClearColor = Some(Colors.rgba 17uy 24uy 32uy 255uy)
        Diagnostics = { Verbose = true } }

let program =
    Viewer.create configuration init update view
    |> Viewer.withEventMapping (ViewerInput >> Some)
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)

let runContractSmoke () =
    let canonical, runtime, _ = createRuntime ()

    let replayed, effects =
        KeyboardInput.replay
            runtime
            [ InputMsg.KeyDown "Space"
              InputMsg.KeyDown "KeyH"
              InputMsg.KeyDown "KeyC"
              InputMsg.KeyDown "KeyH"
              InputMsg.KeyUp "KeyC"
              InputMsg.SetLayout "symbols"
              InputMsg.SetLayout "colemacs-dh" ]

    let report = KeyboardInput.analyzeBigrams canonical "colemacs-dh"
    let layout = KeyboardInput.layoutState replayed
    let sceneKinds = KeyboardInput.renderLayoutState replayed |> Scene.describe

    printfn "status=ok sample=KeyboardInputGallery active-layout=%s available-layouts=%s mode-stack=%A held=%A top-pairs=%d effects=%d scene-kinds=%A"
        layout.ActiveLayout.Id
        (canonical.Configuration.Layouts |> List.map _.Id |> String.concat ",")
        (layout.ActiveModeStack |> List.map _.ModeId)
        (layout.HeldModes |> List.map _.ModeId)
        report.TopPairs.Length
        effects.Length
        sceneKinds
    0

[<EntryPoint>]
let main argv =
    if argv |> Array.contains "--contract-smoke" then
        runContractSmoke ()
    else
        match Viewer.run program with
        | Result.Ok() -> 0
        | Result.Error diagnostic ->
            eprintfn "diagnostic-stage=%A" diagnostic.Stage
            eprintfn "diagnostic-severity=%A" diagnostic.Severity
            eprintfn "diagnostic-message=%s" diagnostic.Message
            eprintfn "diagnostic-cause=%A" diagnostic.Cause
            2

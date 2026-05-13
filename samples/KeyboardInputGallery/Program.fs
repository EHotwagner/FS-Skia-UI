module KeyboardInputGallery

open System
open System.IO
open Elmish
open FS.Skia.UI

type Model =
    { Size: Size
      Input: InputRuntime
      LastEffects: InputEffect list
      StateDisplay: StateDisplayMode
      LastViewerKey: string option
      Diagnostics: RenderDiagnostic list
      Closing: bool }

and StateDisplayMode =
    | CompactDisplay
    | ExpandedDisplay
    | HiddenDisplay

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
    [ { Id = "move.left"; DisplayName = "move-left"; Category = Some "movement" }
      { Id = "move.right"; DisplayName = "move-right"; Category = Some "movement" }
      { Id = "copy.selection"; DisplayName = "copy-left"; Category = Some "editing" }
      { Id = "delete.selection"; DisplayName = "delete-left"; Category = Some "editing" }
      { Id = "open.palette"; DisplayName = "open-palette"; Category = Some "popup" } ]
    |> KeyboardInput.commandRegistry
    |> function
        | Result.Ok registry -> registry
        | Result.Error diagnostics -> failwithf "registry failed: %A" diagnostics

let loadModel root =
    let yaml =
        Path.Combine(root, "specs", "003-keyboard-input-framework", "readiness", "sample-configs", "modal-input.yaml")
        |> File.ReadAllText

    let keyId label =
        match label with
        | ";" -> "Semicolon"
        | "," -> "Comma"
        | "." -> "Period"
        | "/" -> "Slash"
        | value -> $"Key{value.ToUpperInvariant()}"

    let position row column label =
        { Id = keyId label
          Hand = if column < 5 then LeftHand else RightHand
          Finger =
            match column with
            | 0 | 9 -> Pinky
            | 1 | 8 -> Ring
            | 2 | 7 -> Middle
            | 3 | 4 | 5 | 6 -> Index
            | _ -> UnknownFinger
          Row = row
          Column = column }

    let qwertyRows =
        [ [ "q"; "w"; "e"; "r"; "t"; "y"; "u"; "i"; "o"; "p" ]
          [ "a"; "s"; "d"; "f"; "g"; "h"; "j"; "k"; "l"; ";" ]
          [ "z"; "x"; "c"; "v"; "b"; "n"; "m"; ","; "."; "/" ] ]

    let letterPositions =
        qwertyRows
        |> List.mapi (fun row labels -> labels |> List.mapi (position row))
        |> List.concat

    let labeledLayout layoutId displayName labels =
        let labelMap =
            List.zip qwertyRows labels
            |> List.collect (fun (physicalRow, activeRow) ->
                List.zip physicalRow activeRow
                |> List.map (fun (physicalLabel, activeLabel) -> keyId physicalLabel, activeLabel))
            |> Map.ofList

        { Id = layoutId
          DisplayName = displayName
          Positions =
            letterPositions
            @ [ { Id = "Digit1"; Hand = LeftHand; Finger = Pinky; Row = -1; Column = 0 }
                { Id = "Space"; Hand = EitherHand; Finger = Thumb; Row = 4; Column = 4 } ]
          Labels = labelMap |> Map.add "Digit1" "1" |> Map.add "Space" "space" }

    let demoLayouts =
        [ labeledLayout "qwerty" "QWERTY" [ [ "q"; "w"; "e"; "r"; "t"; "y"; "u"; "i"; "o"; "p" ]; [ "a"; "s"; "d"; "f"; "g"; "h"; "j"; "k"; "l"; ";" ]; [ "z"; "x"; "c"; "v"; "b"; "n"; "m"; ","; "."; "/" ] ]
          labeledLayout "dvorak" "Dvorak" [ [ "'"; ","; "."; "p"; "y"; "f"; "g"; "c"; "r"; "l" ]; [ "a"; "o"; "e"; "u"; "i"; "d"; "h"; "t"; "n"; "s" ]; [ ";"; "q"; "j"; "k"; "x"; "b"; "m"; "w"; "v"; "z" ] ]
          labeledLayout "colemacs-dh" "Colemak-DH" [ [ "q"; "w"; "f"; "p"; "b"; "j"; "l"; "u"; "y"; ";" ]; [ "a"; "r"; "s"; "t"; "g"; "m"; "n"; "e"; "i"; "o" ]; [ "z"; "x"; "c"; "d"; "v"; "k"; "h"; ","; "."; "/" ] ]
          labeledLayout "workman" "Workman" [ [ "q"; "d"; "r"; "w"; "b"; "j"; "f"; "u"; "p"; ";" ]; [ "a"; "s"; "h"; "t"; "g"; "y"; "n"; "e"; "o"; "i" ]; [ "z"; "x"; "m"; "c"; "v"; "k"; "l"; ","; "."; "/" ] ]
          labeledLayout "symbols" "Custom Symbols" [ [ "!"; "@"; "#"; "$"; "%"; "^"; "&"; "*"; "("; ")" ]; [ "<"; ">"; "{"; "}"; "["; "]"; "-"; "_"; "="; "+" ]; [ "copy"; "cut"; "paste"; "undo"; "redo"; "layer"; "menu"; ","; "."; "/" ] ] ]

    match KeyboardInput.parseYaml yaml with
    | Result.Error diagnostics -> failwithf "parse failed: %A" diagnostics
    | Result.Ok config ->
        let config =
            { config with
                DefaultLayout = "qwerty"
                Layouts = demoLayouts }

        match KeyboardInput.validate registry config with
        | Result.Ok model -> model
        | Result.Error diagnostics -> failwithf "validate failed: %A" diagnostics

let createRuntime () =
    let root = findRepositoryRoot AppContext.BaseDirectory
    let model = loadModel root

    match KeyboardInput.init "qwerty" model with
    | Result.Ok(runtime, effects) -> model, runtime, effects
    | Result.Error diagnostics -> failwithf "init failed: %A" diagnostics

let commandNames effects =
    effects
    |> List.choose (function
        | CommandResolved resolved -> Some resolved.CommandId
        | _ -> None)

let stateDisplayOptions mode =
    match mode with
    | CompactDisplay -> KeyboardInput.compactStateDisplayOptions
    | ExpandedDisplay -> KeyboardInput.expandedStateDisplayOptions
    | HiddenDisplay ->
        { KeyboardInput.compactStateDisplayOptions with
            Visibility = KeyboardStateDisplayHidden }

let nextStateDisplay mode =
    match mode with
    | CompactDisplay -> ExpandedDisplay
    | ExpandedDisplay -> HiddenDisplay
    | HiddenDisplay -> CompactDisplay

let stateDisplayName mode =
    match mode with
    | CompactDisplay -> "compact"
    | ExpandedDisplay -> "expanded"
    | HiddenDisplay -> "hidden"

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
    let lastCommands = commandNames model.LastEffects
    let lastViewerKey = model.LastViewerKey |> Option.defaultValue "none"
    let lastCommandText =
        if lastCommands.IsEmpty then
            "none"
        else
            String.concat ", " lastCommands

    let docs =
        [ "Window keypresses are captured through ViewerEvent.KeyDown / KeyUp."
          ""
          "Selection layer labels: H move-left, L move-right, 1 line-state, Space space-menu."
          "Held layer labels: hold C for copy-layer, hold D for delete-layer, then press H for the layer action."
          "Layouts: Q=QWERTY, V=Dvorak, K=Colemak-DH, W=Workman, S=Custom Symbols. F2 cycles state display."
          ""
          $"state display: {stateDisplayName model.StateDisplay}"
          $"last viewer key: {lastViewerKey}"
          $"last commands: {lastCommandText}" ]

    let margin = 48.0
    let contentWidth = Math.Max(320.0, float model.Size.Width - margin * 2.0)
    let overlayY = 136.0
    let docsY = 568.0
    let docsHeight = Math.Max(132.0, float model.Size.Height - docsY - 32.0)

    Scene.group [
        Scene.rectangle (0.0, 0.0, float model.Size.Width, float model.Size.Height) (Colors.rgba 17uy 24uy 32uy 255uy)
        Scene.rectangle (margin, 36.0, contentWidth, 84.0) (Colors.rgba 38uy 86uy 116uy 255uy)
        textRunAt 20.0 (margin + 24.0) 74.0 "Keyboard input as a Skia UI framework input option"
        textRunAt 13.0 (margin + 24.0) 98.0 "QWERTY active by default. The state display draws physical key positions with active-layout labels."

        KeyboardInput.renderKeyboardStateDisplayAt (margin, overlayY) (stateDisplayOptions model.StateDisplay) model.LastEffects model.Input

        Scene.rectangle (margin, docsY - 26.0, contentWidth, docsHeight) (Colors.rgba 24uy 32uy 42uy 210uy)
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
          StateDisplay = CompactDisplay
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
                    StateDisplay = if key = "F2" then nextStateDisplay model.StateDisplay else model.StateDisplay }

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
              InputMsg.SetLayout "qwerty" ]

    let report = KeyboardInput.analyzeBigrams canonical "qwerty"
    let layout = KeyboardInput.layoutState replayed
    let compact = KeyboardInput.keyboardStateDisplay KeyboardInput.compactStateDisplayOptions effects replayed
    let expanded = KeyboardInput.keyboardStateDisplay KeyboardInput.expandedStateDisplayOptions effects replayed
    let hidden =
        KeyboardInput.keyboardStateDisplay
            { KeyboardInput.compactStateDisplayOptions with Visibility = KeyboardStateDisplayHidden }
            effects
            replayed
    let sceneKinds = KeyboardInput.renderKeyboardStateDisplay KeyboardInput.compactStateDisplayOptions effects replayed |> Scene.describe

    printfn "status=ok sample=KeyboardInputGallery active-layout=%s available-layouts=%s mode-stack=%A held=%A top-pairs=%d effects=%d compact-labels=%d expanded-stack=%d hidden=%A scene-kinds=%A"
        layout.ActiveLayout.Id
        (canonical.Configuration.Layouts |> List.map _.Id |> String.concat ",")
        (layout.ActiveModeStack |> List.map _.ModeId)
        (layout.HeldModes |> List.map _.ModeId)
        report.TopPairs.Length
        effects.Length
        compact.Labels.Length
        expanded.Stack.Length
        hidden.Visibility
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

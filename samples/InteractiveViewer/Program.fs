module InteractiveViewer.Program

open System
open System.Diagnostics
open System.IO
open System.Threading
open Elmish
open FS.Skia.UI

type Model =
    { Size: Size
      Pointer: (float * float) option
      PointerDown: bool
      ActiveKey: string option
      Keyboard: InputRuntime option
      KeyboardEffects: InputEffect list
      ShowKeyboardStateDisplay: bool
      Ticks: int
      Diagnostics: RenderDiagnostic list
      Closing: bool }

type Msg =
    | ViewerInput of ViewerEvent
    | Tick
    | ScreenshotRequested of ScreenshotRequest
    | HostEffect of ViewerEffect<Msg>

let initialSize = { Width = 800; Height = 520 }

let rec findRepositoryRoot directory =
    if File.Exists(Path.Combine(directory, "FS-Skia-UI.sln")) then
        directory
    else
        match Directory.GetParent directory |> Option.ofObj with
        | Some parent -> findRepositoryRoot parent.FullName
        | None -> failwithf "Could not locate repository root from %s" directory

let keyboardRegistry =
    [ { Id = "move.left"; DisplayName = "Move left"; Category = Some "movement" }
      { Id = "move.right"; DisplayName = "Move right"; Category = Some "movement" }
      { Id = "copy.selection"; DisplayName = "Copy selection"; Category = Some "editing" }
      { Id = "delete.selection"; DisplayName = "Delete selection"; Category = Some "editing" }
      { Id = "open.palette"; DisplayName = "Open palette"; Category = Some "popup" } ]
    |> KeyboardInput.commandRegistry
    |> function
        | Result.Ok registry -> registry
        | Result.Error diagnostics -> failwithf "keyboard registry failed: %A" diagnostics

let createKeyboardRuntime () =
    let root = findRepositoryRoot AppContext.BaseDirectory
    let yaml =
        Path.Combine(root, "samples", "KeyboardInput", "configs", "modal-input.yaml")
        |> File.ReadAllText

    match KeyboardInput.parseYaml yaml with
    | Result.Error diagnostics -> failwithf "keyboard YAML parse failed: %A" diagnostics
    | Result.Ok config ->
        match KeyboardInput.validate keyboardRegistry config with
        | Result.Error diagnostics -> failwithf "keyboard validation failed: %A" diagnostics
        | Result.Ok model ->
            match KeyboardInput.init "colemacs-dh" model with
            | Result.Ok(runtime, effects) -> Some runtime, effects
            | Result.Error diagnostics -> failwithf "keyboard init failed: %A" diagnostics

let init () =
    let keyboard, keyboardEffects = createKeyboardRuntime ()

    { Size = initialSize
      Pointer = None
      PointerDown = false
      ActiveKey = None
      Keyboard = keyboard
      KeyboardEffects = keyboardEffects
      ShowKeyboardStateDisplay = true
      Ticks = 0
      Diagnostics = []
      Closing = false },
    Cmd.ofMsg (HostEffect InitializeRenderer)

let backgroundColor model =
    match model.ActiveKey, model.PointerDown with
    | Some _, _ -> Colors.rgba 34uy 72uy 132uy 255uy
    | None, true -> Colors.rgba 95uy 55uy 96uy 255uy
    | None, false -> Colors.rgba 20uy 26uy 34uy 255uy

let accentColor model =
    let phase = byte ((model.Ticks * 11) % 120)

    if model.PointerDown then
        Colors.rgba 235uy 118uy 76uy 255uy
    else
        Colors.rgba (80uy + phase) 190uy 135uy 255uy

let view model =
    let width = float model.Size.Width
    let height = float model.Size.Height

    let pointerX, pointerY =
        model.Pointer
        |> Option.defaultValue (width * 0.5, height * 0.5)

    let pulse = float (model.Ticks % 90)
    let activeKey = model.ActiveKey |> Option.defaultValue "none"
    let keyboardScene =
        match model.ShowKeyboardStateDisplay, model.Keyboard with
        | true, Some runtime ->
            KeyboardInput.renderKeyboardStateDisplayAt
                (56.0, 252.0)
                KeyboardInput.compactStateDisplayOptions
                model.KeyboardEffects
                runtime
        | _ -> Scene.empty

    Scene.group [
        Scene.rectangle (0.0, 0.0, width, height) (backgroundColor model)
        Scene.rectangle (40.0 + pulse, 54.0, 180.0, 96.0) (accentColor model)
        Scene.rectangle (pointerX - 28.0, pointerY - 28.0, 56.0, 56.0) (Colors.rgba 240uy 235uy 185uy 235uy)
        Scene.text (56.0, 112.0) "Interactive Vulkan Viewer" Colors.white
        Scene.text (56.0, 176.0) $"key: {activeKey}" Colors.white
        Scene.text (56.0, 216.0) $"ticks: {model.Ticks}" Colors.white
        Scene.text (56.0, 244.0) "F2 toggles keyboard state display. Press H/L/Space/C/D/1 to drive input." Colors.white
        keyboardScene
        Scene.chart [ 2.0 + float (model.Ticks % 5); 4.0; 3.0 + float (model.Ticks % 7); 7.0 ]
    ]

let requestRender model =
    Cmd.ofMsg (HostEffect(RenderFrame(view model)))

let update msg model =
    match msg with
    | ViewerInput event ->
        match event with
        | Loaded ->
            model, requestRender model
        | UpdateTick _ ->
            model, Cmd.none
        | RenderTick _ ->
            model, Cmd.none
        | ViewerEvent.KeyDown key ->
            let keyboard, effects =
                model.Keyboard
                |> Option.map (KeyboardInput.updateFromViewerEvent event)
                |> Option.map (fun (runtime, effects) -> Some runtime, effects)
                |> Option.defaultValue (None, [])

            let next =
                { model with
                    ActiveKey = Some key
                    Keyboard = keyboard
                    KeyboardEffects = effects
                    ShowKeyboardStateDisplay =
                        if key = "F2" then
                            not model.ShowKeyboardStateDisplay
                        else
                            model.ShowKeyboardStateDisplay }

            next, requestRender next
        | ViewerEvent.KeyUp key ->
            let keyboard, effects =
                model.Keyboard
                |> Option.map (KeyboardInput.updateFromViewerEvent event)
                |> Option.map (fun (runtime, effects) -> Some runtime, effects)
                |> Option.defaultValue (None, [])

            let next =
                if model.ActiveKey = Some key then
                    { model with
                        ActiveKey = None
                        Keyboard = keyboard
                        KeyboardEffects = effects }
                else
                    { model with
                        Keyboard = keyboard
                        KeyboardEffects = effects }

            next, requestRender next
        | PointerMoved(x, y) ->
            let next = { model with Pointer = Some(x, y) }
            next, requestRender next
        | PointerPressed(x, y) ->
            let next =
                { model with
                    Pointer = Some(x, y)
                    PointerDown = true }

            next, requestRender next
        | PointerReleased(x, y) ->
            let next =
                { model with
                    Pointer = Some(x, y)
                    PointerDown = false }

            next, requestRender next
        | Resized size ->
            let next = { model with Size = size }
            next, requestRender next
        | CloseRequested ->
            { model with Closing = true },
            Cmd.ofMsg (HostEffect Shutdown)
        | DiagnosticReported diagnostic ->
            { model with Diagnostics = diagnostic :: model.Diagnostics },
            Cmd.none
    | Tick ->
        let next = { model with Ticks = model.Ticks + 1 }
        next, requestRender next
    | ScreenshotRequested request ->
        model, Cmd.ofMsg (HostEffect(CaptureScreenshot request))
    | HostEffect _ ->
        model, Cmd.none

let subscriptions _ =
    [ [ "interactive"; "timer" ],
      fun dispatch ->
          let timer =
              new Timer(
                  TimerCallback(fun _ -> dispatch Tick),
                  null,
                  TimeSpan.FromMilliseconds 250.0,
                  TimeSpan.FromMilliseconds 250.0
              )

          { new IDisposable with
              member _.Dispose() = timer.Dispose() } ]

let configuration =
    { Viewer.defaultConfiguration "Interactive Viewer" initialSize with
        ClearColor = Some(Colors.rgba 20uy 26uy 34uy 255uy)
        Diagnostics = { Verbose = true } }

let program =
    Viewer.create configuration init update view
    |> Viewer.withEventMapping (ViewerInput >> Some)
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)
    |> Viewer.withSubscription subscriptions

let runSmoke seconds =
    // SYNTHETIC: smoke mode drives sample messages without a live Vulkan window or wall-clock timer; real evidence is the interactive window smoke in T039 follow-up.
    let stopwatch = Stopwatch.StartNew()
    let model, _ = init ()
    let afterKey, _ = update (ViewerInput(ViewerEvent.KeyDown "Space")) model
    let afterPointer, _ = update (ViewerInput(PointerPressed(240.0, 180.0))) afterKey
    let rec tick count state =
        if count <= 0 then
            state
        else
            let next, _ = update Tick state
            tick (count - 1) next

    let ticked = tick (seconds * 4) afterPointer
    stopwatch.Stop()
    printfn "status=ok"
    printfn "input-latency-ms=%d" stopwatch.ElapsedMilliseconds
    printfn "subscription-seconds=%d" seconds
    printfn "subscription-ticks=%d" ticked.Ticks
    printfn "active-key=%A" ticked.ActiveKey
    printfn "keyboard-layout=%A" (ticked.Keyboard |> Option.map _.ActiveLayout)
    printfn "keyboard-state-display=%b" ticked.ShowKeyboardStateDisplay
    printfn "pointer=%A" ticked.Pointer
    0

let runContractSmoke () =
    let screenshot =
        { Destination = "specs/002-skia-feature-parity/readiness/screenshots/interactive-viewer.jpg"
          Format = Jpeg }

    let model, initCmd = init ()
    let afterKey, _ = update (ViewerInput(ViewerEvent.KeyDown "Space")) model
    let afterPointer, _ = update (ViewerInput(PointerPressed(320.0, 210.0))) afterKey
    let afterResize, _ = update (ViewerInput(Resized { Width = 1024; Height = 640 })) afterPointer
    let afterTick, _ = update Tick afterResize
    let _, screenshotCmd = update (ScreenshotRequested screenshot) afterTick
    let initEffects = ResizeArray<Msg>()
    let screenshotEffects = ResizeArray<Msg>()

    initCmd |> List.iter (fun effect -> effect initEffects.Add)
    screenshotCmd |> List.iter (fun effect -> effect screenshotEffects.Add)

    printfn "status=ok"
    printfn "sample=InteractiveViewer"
    printfn "active-key=%A" afterTick.ActiveKey
    printfn "keyboard-layout=%A" (afterTick.Keyboard |> Option.map _.ActiveLayout)
    printfn "keyboard-state-display=%b" afterTick.ShowKeyboardStateDisplay
    printfn "keyboard-effects=%d" afterTick.KeyboardEffects.Length
    match afterTick.Keyboard with
    | Some runtime ->
        let display = KeyboardInput.keyboardStateDisplay KeyboardInput.compactStateDisplayOptions afterTick.KeyboardEffects runtime
        printfn "keyboard-display-layout=%A" (display.Layout |> Option.map _.Id)
        printfn "keyboard-display-labels=%d" display.Labels.Length
        printfn "keyboard-display-scene=%A" (KeyboardInput.renderKeyboardStateDisplay KeyboardInput.compactStateDisplayOptions afterTick.KeyboardEffects runtime |> Scene.describe)
    | None ->
        printfn "keyboard-display-layout=None"
        printfn "keyboard-display-labels=0"
        printfn "keyboard-display-scene=[]"
    printfn "pointer=%A" afterTick.Pointer
    printfn "ticks=%d" afterTick.Ticks
    printfn "size=%dx%d" afterTick.Size.Width afterTick.Size.Height
    printfn "scene=%A" (view afterTick)

    match List.ofSeq initEffects, List.ofSeq screenshotEffects with
    | [ HostEffect InitializeRenderer ], [ HostEffect(CaptureScreenshot request) ] ->
        printfn "initialize-effect=true"
        printfn "screenshot-destination=%s" request.Destination
        printfn "screenshot-format=%A" request.Format
        0
    | otherInit, otherScreenshot ->
        eprintfn "unexpected-init-effects=%A" otherInit
        eprintfn "unexpected-screenshot-effects=%A" otherScreenshot
        4

[<EntryPoint>]
let main argv =
    if argv |> Array.contains "--smoke" then
        let seconds =
            argv
            |> Array.tryFind (fun arg -> arg.StartsWith("--seconds=", StringComparison.Ordinal))
            |> Option.map (fun arg -> arg.Substring("--seconds=".Length))
            |> Option.bind (fun value ->
                match Int32.TryParse value with
                | true, parsed -> Some parsed
                | false, _ -> None)
            |> Option.defaultValue 60

        runSmoke seconds
    elif argv |> Array.contains "--contract-smoke" then
        runContractSmoke ()
    else
        match Viewer.run program with
        | Ok() -> 0
        | Result.Error diagnostic ->
            eprintfn "diagnostic-stage=%A" diagnostic.Stage
            eprintfn "diagnostic-severity=%A" diagnostic.Severity
            eprintfn "diagnostic-message=%s" diagnostic.Message
            eprintfn "diagnostic-cause=%A" diagnostic.Cause
            2

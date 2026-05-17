module KeyboardInputGallery

open System.Diagnostics
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish
open FS.Skia.UI.KeyboardInput

type Model =
    { Keyboard: KeyboardModel
      LastEffects: KeyboardEffect list
      LastAdapterCommands: AdapterCommand<Msg> }

and Msg =
    | KeyboardRuntimeMsg of KeyboardMsg
    | CommandInvoked of CommandId
    | NoOp

let commandToMsg command =
    CommandInvoked command

let init () =
    let keyboard, effects =
        Keyboard.init [
            { Key = "H"; Command = "move.left" }
            { Key = "L"; Command = "move.right" }
            { Key = "C"; Command = "copy.selection" }
        ]

    let commands = effects |> List.collect (ControlsElmish.interpretKeyboardEffect commandToMsg)

    { Keyboard = keyboard
      LastEffects = effects
      LastAdapterCommands = commands },
    commands

let update msg model =
    match msg with
    | KeyboardRuntimeMsg keyboardMsg ->
        let keyboard, effects = Keyboard.update keyboardMsg model.Keyboard
        let commands = effects |> List.collect (ControlsElmish.interpretKeyboardEffect commandToMsg)

        { model with
            Keyboard = keyboard
            LastEffects = effects
            LastAdapterCommands = commands },
        commands
    | CommandInvoked _
    | NoOp ->
        model, []

let displayView model =
    let display = Keyboard.stateDisplay model.Keyboard
    let pressed = String.concat "," display.PressedKeys
    let modeStack = String.concat "," display.ActiveModeStack
    let lastCommand = display.LastCommand |> Option.defaultValue "none"

    Stack.create [
        Stack.children [
            TextBlock.create [ TextBlock.text "Keyboard Input Gallery" ]
            TextBlock.create [ TextBlock.text $"active-layout: {display.ActiveLayout}" ]
            TextBlock.create [ TextBlock.text $"pressed: {pressed}" ]
            TextBlock.create [ TextBlock.text $"mode-stack: {modeStack}" ]
            TextBlock.create [ TextBlock.text $"last-command: {lastCommand}" ]
        ]
    ]

let adapterProgram =
    ControlsElmish.program init update displayView (fun _ -> [])

let runContractSmoke () =
    let stopwatch = Stopwatch.StartNew()
    let model, initCommands = adapterProgram.Init()
    let withMode, modeCommands = adapterProgram.Update (KeyboardRuntimeMsg(PushTemporaryMode "symbols")) model
    let withKey, keyCommands = adapterProgram.Update (KeyboardRuntimeMsg(KeyDown "H")) withMode
    let recovered, recoveryCommands = adapterProgram.Update (KeyboardRuntimeMsg FocusLost) withKey
    let displayBeforeRecovery = Keyboard.stateDisplay withKey.Keyboard
    let displayAfterRecovery = Keyboard.stateDisplay recovered.Keyboard
    let rendered = Control.render Theme.light (adapterProgram.View recovered)
    stopwatch.Stop()

    printfn "status=ok"
    printfn "sample=KeyboardInputGallery"
    printfn "active-layout=%s" displayAfterRecovery.ActiveLayout
    printfn "available-layouts=default"
    printfn "mode-stack=%A" displayBeforeRecovery.ActiveModeStack
    printfn "held=%A" displayBeforeRecovery.PressedKeys
    printfn "effects=%d" recovered.LastEffects.Length
    printfn "compact-labels=%d" displayBeforeRecovery.PressedKeys.Length
    printfn "expanded-stack=%d" displayBeforeRecovery.ActiveModeStack.Length
    printfn "hidden=KeyboardStateDisplayHidden"
    printfn "last-command=%A" displayBeforeRecovery.LastCommand
    printfn "recovered-pressed=%A" displayAfterRecovery.PressedKeys
    printfn "adapter-init-commands=%d mode-commands=%d key-commands=%d recovery-commands=%d" initCommands.Length modeCommands.Length keyCommands.Length recoveryCommands.Length
    printfn "scene-kinds=%A" (Scene.describe rendered.Scene)
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

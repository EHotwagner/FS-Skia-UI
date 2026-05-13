#r "nuget: YamlDotNet, 17.1.0"
#r "../src/Lib/bin/Debug/net10.0/FS.Skia.UI.dll"

open System
open System.IO
open FS.Skia.UI

let root =
    let rec find directory =
        if File.Exists(Path.Combine(directory, "FS-Skia-UI.sln")) then directory
        else find (Directory.GetParent(directory).FullName)
    find __SOURCE_DIRECTORY__

let fixture path =
    File.ReadAllText(Path.Combine(root, path))

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

let config =
    fixture "specs/003-keyboard-input-framework/readiness/sample-configs/modal-input.yaml"
    |> KeyboardInput.parseYaml
    |> function
        | Result.Ok config -> config
        | Result.Error diagnostics -> failwithf "parse failed: %A" diagnostics

let model =
    KeyboardInput.validate registry config
    |> function
        | Result.Ok model -> model
        | Result.Error diagnostics -> failwithf "validate failed: %A" diagnostics

let runtime, initEffects =
    KeyboardInput.init "qwerty" model
    |> function
        | Result.Ok value -> value
        | Result.Error diagnostics -> failwithf "init failed: %A" diagnostics

let replayed, replayEffects =
    KeyboardInput.replay runtime [ InputMsg.KeyDown "Space"; InputMsg.KeyDown "KeyH"; InputMsg.KeyDown "KeyC"; InputMsg.KeyDown "KeyH"; InputMsg.KeyUp "KeyC"; InputMsg.FocusLost ]

let report = KeyboardInput.analyzeBigrams model "qwerty"
let view = KeyboardInput.layoutState replayed

printfn "init-effects=%d" initEffects.Length
printfn "replay-effects=%d" replayEffects.Length
printfn "mode-stack=%A" (view.ActiveModeStack |> List.map _.ModeId)
printfn "held-modes=%A" (view.HeldModes |> List.map _.ModeId)
printfn "active-layout=%s" view.ActiveLayout.Id
printfn "top-pairs=%d risks=%d suggestions=%d" report.TopPairs.Length report.Risks.Length report.Suggestions.Length

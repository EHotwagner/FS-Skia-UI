#r "nuget: YamlDotNet, 17.1.0"
#r "../src/Lib/bin/Debug/net10.0/FS.Skia.UI.dll"

open System
open System.Diagnostics
open System.IO
open FS.Skia.UI

let rec findRoot directory =
    if File.Exists(Path.Combine(directory, "FS-Skia-UI.sln")) then directory
    else findRoot (Directory.GetParent(directory).FullName)

let root = findRoot __SOURCE_DIRECTORY__

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

let model =
    File.ReadAllText(Path.Combine(root, "specs/003-keyboard-input-framework/readiness/sample-configs/modal-input.yaml"))
    |> KeyboardInput.parseYaml
    |> function
        | Result.Ok config -> KeyboardInput.validate registry config
        | Result.Error diagnostics -> Result.Error diagnostics
    |> function
        | Result.Ok model -> model
        | Result.Error diagnostics -> failwithf "model failed: %A" diagnostics

let runtime =
    KeyboardInput.init "qwerty" model
    |> function
        | Result.Ok(runtime, _) -> runtime
        | Result.Error diagnostics -> failwithf "init failed: %A" diagnostics

let messages =
    [ for i in 1 .. 10000 do
          if i % 5 = 0 then InputMsg.KeyDown "Space"
          elif i % 5 = 1 then InputMsg.KeyDown "KeyH"
          elif i % 5 = 2 then InputMsg.KeyDown "KeyC"
          elif i % 5 = 3 then InputMsg.KeyUp "KeyC"
          else InputMsg.Timeout ]

let watch = Stopwatch.StartNew()
let _, effects = KeyboardInput.replay runtime messages
watch.Stop()

let eventMilliseconds = float watch.ElapsedMilliseconds / float messages.Length
let under16 = eventMilliseconds < 16.0

let bigramWatch = Stopwatch.StartNew()
for _ in 1 .. 500 do
    KeyboardInput.analyzeBigrams model "qwerty" |> ignore
bigramWatch.Stop()

printfn "events=%d replay-ms=%d avg-event-ms=%.4f under-16ms=%b effects=%d" messages.Length watch.ElapsedMilliseconds eventMilliseconds under16 effects.Length
printfn "bigram-runs=500 total-ms=%d under-1000ms=%b" bigramWatch.ElapsedMilliseconds (bigramWatch.ElapsedMilliseconds < 1000L)

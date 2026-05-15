#r "nuget: YamlDotNet, 17.1.0"
#r "../src/Lib/bin/Debug/net10.0/FS.Skia.UI.dll"

open System
open System.Diagnostics
open System.IO
open FS.Skia.UI

let root =
    let rec find directory =
        if File.Exists(Path.Combine(directory, "FS-Skia-UI.sln")) then directory
        else find (Directory.GetParent(directory).FullName)

    find __SOURCE_DIRECTORY__

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
    File.ReadAllText(Path.Combine(root, "samples", "KeyboardInput", "configs", "modal-input.yaml"))
    |> KeyboardInput.parseYaml
    |> function
        | Result.Ok config -> config
        | Result.Error diagnostics -> failwithf "parse failed: %A" diagnostics
    |> KeyboardInput.validate registry
    |> function
        | Result.Ok model -> model
        | Result.Error diagnostics -> failwithf "validate failed: %A" diagnostics

let runtime, initEffects =
    KeyboardInput.init "qwerty" model
    |> function
        | Result.Ok value -> value
        | Result.Error diagnostics -> failwithf "init failed: %A" diagnostics

let replayed, effects =
    KeyboardInput.replay runtime [ InputMsg.KeyDown "Space"; InputMsg.KeyDown "KeyH"; InputMsg.KeyDown "KeyC"; InputMsg.KeyDown "KeyH"; InputMsg.KeyUp "KeyC"; InputMsg.FocusLost ]

let pendingRuntime =
    { replayed with
        PendingSequence =
            Some
                { StartedAt = DateTimeOffset.UnixEpoch
                  Chords = [ { Position = "Space"; RequiredHeld = [] } ] } }

let nestedRuntime =
    { replayed with
        ModeStack =
            [ { ModeId = "selection"; State = Some "character"; EnteredBy = None }
              { ModeId = "space"; State = None; EnteredBy = Some "Space" }
              { ModeId = "copy"; State = None; EnteredBy = Some "KeyC" }
              { ModeId = "delete"; State = None; EnteredBy = Some "KeyD" }
              { ModeId = "selection"; State = Some "line"; EnteredBy = None } ] }

let twelveLabelRuntime =
    let layout = model.Configuration.Layouts.Head
    let extraPositions =
        [ 1..12 ]
        |> List.map (fun index ->
            { Id = $"Extra{index}"
              Hand = UnknownHand
              Finger = UnknownFinger
              Row = index / 4
              Column = index % 4 })

    let labels =
        extraPositions
        |> List.map (fun position -> position.Id, position.Id.ToLowerInvariant())
        |> Map.ofList

    let bindings =
        extraPositions
        |> List.map (fun position ->
            { ModeId = "selection"
              Sequence = [ { Position = position.Id; RequiredHeld = [] } ]
              WhenState = Some "character"
              Outcome = NoInputOp
              Weight = None })

    { runtime with
        Model =
            { runtime.Model with
                Configuration =
                    { runtime.Model.Configuration with
                        Layouts = { layout with Positions = layout.Positions @ extraPositions; Labels = Map.fold (fun acc key value -> Map.add key value acc) layout.Labels labels } :: model.Configuration.Layouts.Tail
                        Bindings = runtime.Model.Configuration.Bindings @ bindings } } }

let partialRuntime = { runtime with ActiveLayout = "missing-layout" }

let measure name options recentEffects value =
    let iterations = 5000
    let sw = Stopwatch.StartNew()

    for _ in 1..iterations do
        KeyboardInput.keyboardStateDisplay options recentEffects value |> ignore

    sw.Stop()
    let averageMs = sw.Elapsed.TotalMilliseconds / float iterations
    printfn "%s average-ms=%.6f pass-under-1ms=%b" name averageMs (averageMs < 1.0)

measure "compact" KeyboardInput.compactStateDisplayOptions initEffects runtime
measure "expanded" KeyboardInput.expandedStateDisplayOptions effects replayed
measure "nested-stack" KeyboardInput.compactStateDisplayOptions effects nestedRuntime
measure "twelve-label" KeyboardInput.expandedStateDisplayOptions effects twelveLabelRuntime
measure "pending-sequence" KeyboardInput.expandedStateDisplayOptions effects pendingRuntime
measure "recent-command" KeyboardInput.compactStateDisplayOptions effects replayed
measure "diagnostic" KeyboardInput.expandedStateDisplayOptions effects replayed
measure "partial-layout" KeyboardInput.expandedStateDisplayOptions effects partialRuntime

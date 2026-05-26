#i "nuget: /home/developer/projects/FS-Skia-UI/specs/016-persistent-viewer-contract/readiness/package"
#r "nuget: FS.Skia.UI.SkiaViewer, 0.1.16-persistent.1"

open System
open FS.Skia.UI.KeyboardInput
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

type Model =
    { Count: int
      Closed: bool }

type Msg =
    | Increment
    | Close

let update msg model =
    match msg with
    | Increment -> { model with Count = model.Count + 1 }, [ RenderScene(Group []) ]
    | Close -> { model with Closed = true }, [ CloseWindow ]

let host =
    { Init = fun () -> { Count = 0; Closed = false }, []
      Update = update
      View = fun model -> Text((0.0, 0.0), $"count {model.Count}", { Red = 255uy; Green = 255uy; Blue = 255uy; Alpha = 255uy })
      MapKey = fun key isDown -> if isDown && key = Space then Some Increment else None
      Tick = fun elapsed -> if elapsed > TimeSpan.Zero then Some Increment else None
      Diagnostics = Viewer.defaultDiagnostics }

let transitioned, effects =
    GeneratedAppHost.dispatchKey
        host
        { RawKey = "Space"
          Direction = ViewerKeyDirection.KeyDown }
        { Count = 0; Closed = false }

printfn "model-count=%d" transitioned.Count
printfn "render-effect=%b" (effects |> List.exists (function RenderScene _ -> true | _ -> false))
printfn "tick-dispatch=%b" (host.Tick(TimeSpan.FromMilliseconds 16.0) = Some Increment)

match Viewer.runApp { Title = "Packed Generated Host"; InitialSize = { Width = 320; Height = 200 } } host with
| Result.Ok outcome ->
    printfn "status=%s" outcome.Status
    printfn "mode=%s" outcome.Mode
    printfn "window-opened=%b" outcome.WindowOpened
    printfn "exit-path=%b" outcome.ExitPath
| Result.Error failure ->
    printfn "status=unsupported-or-failed"
    printfn "classification=%A" failure.Classification
    printfn "message=%s" failure.Message

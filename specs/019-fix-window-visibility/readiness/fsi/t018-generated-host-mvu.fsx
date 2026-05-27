#r "../../../../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../../../../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../../../../src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.SkiaViewer.dll"

open System
open FS.Skia.UI.KeyboardInput
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

type HostModel = { Count: int; Closed: bool }
type HostMsg = Increment | Close

let host =
    { Init = fun () -> { Count = 0; Closed = false }, []
      Update =
        fun msg model ->
            match msg with
            | Increment -> { model with Count = model.Count + 1 }, [ RenderScene(Group []) ]
            | Close -> { model with Closed = true }, [ CloseWindow ]
      View = fun model -> Text((0.0, 0.0), $"count {model.Count}", { Red = 255uy; Green = 255uy; Blue = 255uy; Alpha = 255uy })
      MapKey = fun key isDown -> if isDown && key = Space then Some Increment else None
      Tick = fun elapsed -> if elapsed > TimeSpan.Zero then Some Increment else None
      Diagnostics = Viewer.defaultDiagnostics }

let initial, initEffects = host.Init()
let afterKey, keyEffects =
    GeneratedAppHost.dispatchKey
        host
        { RawKey = "Space"; Direction = ViewerKeyDirection.KeyDown }
        initial
let tickMsg = host.Tick(TimeSpan.FromMilliseconds 16.0)
let closed, closeEffects = host.Update Close afterKey

printfn "init-count=%i init-effects=%i" initial.Count initEffects.Length
printfn "manual-input-count=%i render-effect=%b" afterKey.Count (keyEffects |> List.exists (function RenderScene _ -> true | _ -> false))
printfn "tick-message=%A" tickMsg
printfn "explicit-close-closed=%b close-effect=%b" closed.Closed (closeEffects |> List.exists (function CloseWindow -> true | _ -> false))

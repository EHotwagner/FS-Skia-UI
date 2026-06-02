// prelude.fsx - public API construction smoke for the packed library.
//
// Usage:
//   dotnet fsi scripts/prelude.fsx

#r "nuget: Fable.Elmish, 4.2.0"
#r "../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.SkiaViewer.dll"

open Elmish
open FS.Skia.UI.Scene
// Open the viewer host last so its diagnostic/effect types resolve over the Scene vocabulary.
open FS.Skia.UI.SkiaViewer.Host

type Msg =
    | Tick

let configuration =
    Viewer.defaultConfiguration "Prelude Viewer" { Width = 640; Height = 480 }

let scene =
    Scene.group [
        Scene.rectangle (0.0, 0.0, 180.0, 80.0) (Colors.rgba 28uy 88uy 140uy 255uy)
        Scene.text (16.0, 42.0) "FS.Skia.UI" Colors.white
        Scene.chart [ 2.0; 4.0; 3.0; 8.0 ]
    ]

let init () = 0, Cmd.none

let update msg model =
    match msg with
    | Tick -> model + 1, Cmd.none

let view _ = scene

let subscriptions _ =
    [ [ "prelude"; "timer" ],
      fun _ -> { new System.IDisposable with member _.Dispose() = () } ]

let program =
    Viewer.create configuration init update view
    |> Viewer.withSubscription subscriptions

let model, _ = program.Init()
let next, _ = program.Update Tick model
let renderEffect: ViewerEffect<Msg> = RenderFrame(program.View next)

let screenshot =
    { Destination = "prelude.png"
      Format = Png }

let effect: ViewerEffect<Msg> = CaptureScreenshot screenshot

printfn "prelude: %s model=%d next=%d screenshot=%A render=%A capture=%A" program.Configuration.Title model next screenshot.Format renderEffect effect

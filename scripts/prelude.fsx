// prelude.fsx - public API construction smoke for the packed library.
//
// Usage:
//   dotnet fsi scripts/prelude.fsx

#i "nuget: file:///home/developer/.local/share/nuget-local/"
#r "nuget: FS.Skia.UI, 0.1.0-preview.1"

open Elmish
open FS.Skia.UI

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

let screenshot =
    { Destination = "prelude.png"
      Format = Png }

let effect = CaptureScreenshot screenshot

printfn "prelude: %s %A %A" program.Configuration.Title screenshot.Format effect

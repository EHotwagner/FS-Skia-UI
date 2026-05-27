#r "../../../../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../../../../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../../../../src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.SkiaViewer.dll"

open System
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

type HostModel = { Closed: bool }
type HostMsg = Close

let host =
    { Init = fun () -> { Closed = false }, []
      Update = fun Close model -> { model with Closed = true }, [ CloseWindow ]
      View = fun _ -> Group []
      MapKey = fun _ _ -> None
      Tick = fun _ -> None
      Diagnostics = Viewer.defaultDiagnostics }

let options = { Title = "Product"; InitialSize = { Width = 640; Height = 480 } }

Environment.SetEnvironmentVariable("DISPLAY", null)
Environment.SetEnvironmentVariable("WAYLAND_DISPLAY", null)

let desktop = Viewer.desktopSessionDiagnostic()
let capability = Viewer.runtimeCapability()

printfn "desktop-diagnostic-class=%s" desktop.DiagnosticClass
printfn "persistent-window-capability=%b" capability.PersistentWindow

match Viewer.runApp options host with
| Result.Ok outcome ->
    printfn "status=%s mode=%s first-frame-presented=%b self-closed-for-evidence=%b close-reason=%A user-close-observed=%b app-close-observed=%b exit-path=%b" outcome.Status outcome.Mode outcome.FirstFramePresented outcome.SelfClosedForEvidence outcome.CloseReason outcome.UserCloseObserved outcome.AppCloseObserved outcome.ExitPath
| Result.Error failure ->
    printfn "status=failed blocked-stage=%A classification=%A category=%A message=%s" failure.BlockedStage failure.Classification failure.DiagnosticCategory failure.Message

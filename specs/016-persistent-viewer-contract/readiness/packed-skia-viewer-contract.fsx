#i "nuget: /home/developer/projects/FS-Skia-UI/specs/016-persistent-viewer-contract/readiness/package"
#r "nuget: FS.Skia.UI.SkiaViewer, 0.1.16-persistent.1"

open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

let capability = Viewer.runtimeCapability()
printfn "persistent-window=%b" capability.PersistentWindow
printfn "bounded-smoke=%b" capability.BoundedSmoke
printfn "keyboard-input=%b" capability.KeyboardInput
printfn "renderer-mode=%s" capability.RendererMode

let result =
    Viewer.run
        { Title = "Packed Contract"
          InitialSize = { Width = 320; Height = 200 } }
        (Group [])

match result with
| Result.Ok outcome ->
    printfn "status=%s" outcome.Status
    printfn "mode=%s" outcome.Mode
    printfn "window-opened=%b" outcome.WindowOpened
    printfn "exit-path=%b" outcome.ExitPath
| Result.Error failure ->
    printfn "status=unsupported-or-failed"
    printfn "blocked-stage=%A" failure.BlockedStage
    printfn "classification=%A" failure.Classification
    printfn "message=%s" failure.Message

#r "../../../../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../../../../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../../../../src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.SkiaViewer.dll"

open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

let options = { Title = "Product"; InitialSize = { Width = 640; Height = 480 } }
let model, _ = Viewer.init options
let running, _ = Viewer.update StartInteractive model
let firstFrame, frameEffects = Viewer.update (FramePresented { Width = 640; Height = 480 }) running
let userClosed, _ = Viewer.update ViewerMsg.UserCloseObserved firstFrame
let appClosed, _ = Viewer.update AppCloseRequested firstFrame
let hostClosed, _ = Viewer.update HostCloseObserved firstFrame

printfn "mode=interactive-window"
printfn "first-frame-state=%A" firstFrame.LifecycleState
printfn "first-frame-running=%b" firstFrame.IsRunning
printfn "self-closed-for-evidence=%b" (frameEffects |> List.exists (function CloseWindow -> true | _ -> false))
printfn "user-close-state=%A user-close-observed=%b" userClosed.LifecycleState userClosed.UserCloseObserved
printfn "app-close-state=%A user-close-observed=%b" appClosed.LifecycleState appClosed.UserCloseObserved
printfn "host-close-state=%A user-close-observed=%b" hostClosed.LifecycleState hostClosed.UserCloseObserved

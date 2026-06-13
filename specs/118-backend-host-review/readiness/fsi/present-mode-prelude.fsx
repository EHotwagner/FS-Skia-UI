// Feature 118 — exercise the new public present-mode surface through FSI, as a consumer would.
// References the locally-built FS.Skia.UI.SkiaViewer (pre-merge; published pins lag until the
// speckit-merge version bump). Surface: ViewerPresentMode DU + ViewerOptions.PresentMode field.
#r "/home/developer/projects/FS-Skia-UI/src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "/home/developer/projects/FS-Skia-UI/src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "/home/developer/projects/FS-Skia-UI/src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.SkiaViewer.dll"

open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

let offscreen = ViewerPresentMode.OffscreenReadback
let direct = ViewerPresentMode.DirectToSwapchain
printfn "present-modes: %A | %A" offscreen direct

let options =
    { Title = "My App"
      InitialSize = { Width = 1280; Height = 720 }
      PresentMode = ViewerPresentMode.OffscreenReadback }
printfn "default options present-mode: %A" options.PresentMode

let fastOptions = { options with PresentMode = ViewerPresentMode.DirectToSwapchain }
printfn "opted-in present-mode: %A" fastOptions.PresentMode
printfn "FSI surface exercise: OK"

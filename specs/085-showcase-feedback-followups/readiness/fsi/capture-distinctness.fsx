// T013 — capture per-page render-distinctness PNGs from Control.renderTree (SC-001).
#I "/home/developer/projects/FS-Skia-UI/src/Controls.Elmish/bin/Debug/net10.0"
#I "/home/developer/projects/FS-Skia-UI/samples/PointerInteractionGallery/bin/Debug/net10.0"
#r "Yoga.Net.dll"
#r "SkiaSharp.dll"
#r "FS.Skia.UI.Scene.dll"
#r "FS.Skia.UI.Layout.dll"
#r "FS.Skia.UI.KeyboardInput.dll"
#r "FS.Skia.UI.Controls.dll"
#r "FS.Skia.UI.SkiaViewer.dll"

open System
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls
open FS.Skia.UI.SkiaViewer

type Msg = Clicked

let theme = Theme.light
let size = { Width = 640; Height = 480 }
let outDir = "/home/developer/projects/FS-Skia-UI/specs/085-showcase-feedback-followups/evidence/render-distinctness"

let pageA: Control<Msg> =
    Stack.create
        [ Stack.children
              [ TextBlock.create [ TextBlock.text "Alpha" ]
                Button.create [ Button.text "Go"; Button.onClick Clicked ]
                Slider.create [ Slider.value 0.3 ] ] ]

let pageB: Control<Msg> =
    Stack.create
        [ Stack.children
              [ TextBlock.create [ TextBlock.text "Beta" ]
                Stack.create [ Stack.children [ TextBlock.create [ TextBlock.text "Nested-1" ]
                                                TextBlock.create [ TextBlock.text "Nested-2" ] ] ]
                Button.create [ Button.text "Stop"; Button.onClick Clicked ] ] ]

let capture name (page: Control<Msg>) =
    let rendered = Control.renderTree theme size page
    let sceneNode = SceneNode.Group [ rendered.Scene ]
    let path = sprintf "%s/%s.png" outDir name
    let request: ScreenshotEvidenceRequest =
        { Command = sprintf "Control.renderTree (page %s)" name
          AppOrSample = "feature-085-render-distinctness"
          OutputPath = path
          Width = size.Width
          Height = size.Height
          RendererMode = "skia"
          CaptureMode = ViewerRenderTargetPng
          HostFacts = [ "feature=085"; "evidence=render-distinctness" ]
          Timeout = TimeSpan.FromSeconds 30.0 }
    let options: ViewerOptions = { Title = sprintf "page-%s" name; InitialSize = size; PresentMode = ViewerPresentMode.OffscreenReadback }
    let result = Viewer.captureScreenshotEvidence request options sceneNode
    printfn "page %s: status=%A path=%A dims=%Ax%A pixelContent=%A proves=%b"
        name result.Status result.ScreenshotPath result.Width result.Height result.PixelContentValidation result.ProvesScreenshot
    path

let pathA = capture "page-a" pageA
let pathB = capture "page-b" pageB

let bytesA = IO.File.ReadAllBytes pathA
let bytesB = IO.File.ReadAllBytes pathB
printfn "page-a.png bytes=%d  page-b.png bytes=%d" bytesA.Length bytesB.Length
printfn "PNG signature A ok=%b" (bytesA.Length > 8 && bytesA.[1] = 0x50uy && bytesA.[2] = 0x4Euy && bytesA.[3] = 0x47uy)
printfn "SC-001 per-page diff non-empty (bytes differ) = %b" (bytesA <> bytesB)
printfn "capture complete."

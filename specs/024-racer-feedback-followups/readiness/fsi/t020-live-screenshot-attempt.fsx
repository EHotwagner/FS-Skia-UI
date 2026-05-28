#r "../../../../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../../../../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../../../../src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.SkiaViewer.dll"

open System
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

let result =
    Viewer.captureScreenshotEvidence
        { Command = "--screenshot-evidence"
          OutputPath = "specs/024-racer-feedback-followups/readiness/artifacts/t020-live-screenshot.png"
          Width = 320
          Height = 200
          RendererMode = "skia"
          Timeout = TimeSpan.FromSeconds 10.0 }
        { Title = "T020 Screenshot Evidence"; InitialSize = { Width = 320; Height = 200 } }
        (Rectangle((0.0, 0.0, 320.0, 200.0), Colors.white))

printfn "status=%A" result.Status
printfn "evidence-kind=%s" result.EvidenceKind
printfn "screenshot-path=%A" result.ScreenshotPath
printfn "viewer-open-status=%A" result.ViewerOpenStatus
printfn "first-frame-status=%A" result.FirstFrameStatus
printfn "capture-availability=%A" result.CaptureAvailability
printfn "capture-source=%A" result.CaptureSource
printfn "proves-screenshot=%b" result.ProvesScreenshot
printfn "unsupported-host-reason=%A" result.UnsupportedHostReason
printfn "fallback=%A" result.Fallback

if result.Status <> ScreenshotOk || result.ScreenshotPath.IsNone || not result.ProvesScreenshot then
    failwith "T020 did not produce real live-window PNG screenshot evidence"

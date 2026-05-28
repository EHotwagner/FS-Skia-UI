#I "../../../../src/Scene/bin/Debug/net10.0"
#I "../../../../src/KeyboardInput/bin/Debug/net10.0"
#I "../../../../src/SkiaViewer/bin/Debug/net10.0"
#I "/home/developer/.nuget/packages/skiasharp/4.147.0-preview.3.1/lib/net10.0"

#r "SkiaSharp.dll"
#r "FS.Skia.UI.Scene.dll"
#r "FS.Skia.UI.KeyboardInput.dll"
#r "FS.Skia.UI.SkiaViewer.dll"

open System
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

let request: ScreenshotEvidenceRequest =
    { Command = "dotnet fsi specs/026-working-screenshot-taking/readiness/fsi/screenshot-failure-smoke.fsx"
      AppOrSample = "working-screenshot-taking-failure-smoke"
      OutputPath = "specs/026-working-screenshot-taking/readiness/artifacts/invalid-screenshot-record.txt"
      Width = 0
      Height = 200
      RendererMode = "skia"
      CaptureMode = ViewerRenderTargetPng
      HostFacts = [ $"os={Environment.OSVersion.Platform}"; $"machine={Environment.MachineName}" ]
      Timeout = TimeSpan.FromSeconds 5.0 }

let result =
    Viewer.captureScreenshotEvidence request { Title = "Failure Smoke"; InitialSize = { Width = 320; Height = 200 } } (Rectangle((0.0, 0.0, 24.0, 24.0), Colors.white))

printfn "status=%A" result.Status
printfn "command=%s" result.Command
printfn "app-or-sample=%s" result.AppOrSample
printfn "capture-mode=%A" result.CaptureMode
printfn "evidence-kind=%s" result.EvidenceKind
printfn "artifact-path=%A" result.ScreenshotPath
printfn "pixel-content-validation=%A" result.PixelContentValidation
printfn "capture-source=%A" result.CaptureSource
printfn "proves-screenshot=%b" result.ProvesScreenshot
printfn "blocked-stage=%A" result.BlockedStage
printfn "classification=%A" result.Classification
printfn "category=%A" result.Category
printfn "message=%s" result.Message
printfn "diagnostics=%A" result.Diagnostics

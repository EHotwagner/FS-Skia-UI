#r "../../../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../../../src/Lib/bin/Debug/net10.0/FS.Skia.UI.dll"
#r "../../../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../../../src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.SkiaViewer.dll"
#r "../../../src/Testing/bin/Debug/net10.0/FS.Skia.UI.Testing.dll"

open System
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer
open FS.Skia.UI.Testing

let scene =
    Group
        [ Scene.circle { X = 32.0; Y = 32.0 } 12.0 (Colors.rgb 40uy 120uy 220uy)
          Scene.filledEllipse { X = 42.0; Y = 16.0; Width = 18.0; Height = 10.0 } (Colors.rgb 240uy 180uy 40uy) ]

let request =
    { Command = "--screenshot-evidence"
      OutputPath = "readiness/game-screenshot-evidence.txt"
      Width = 96
      Height = 64
      RendererMode = "skia"
      Timeout = TimeSpan.FromSeconds 2.0 }

let result =
    Viewer.captureScreenshotEvidence
        request
        { Title = "Screenshot Evidence"; InitialSize = { Width = 96; Height = 64 } }
        scene

let reportStatus =
    match result.Status with
    | ScreenshotOk -> EvidenceOk
    | ScreenshotUnsupported -> EvidenceUnsupported
    | ScreenshotFailed -> EvidenceFailed

let report =
    EvidenceReports.build
        { Status = reportStatus
          Command = result.Command
          OutputPath = result.OutputPath
          Fields =
            [ EvidenceReports.field "evidence-kind" result.EvidenceKind
              EvidenceReports.field "renderer-mode" result.RendererMode
              EvidenceReports.field "unsupported-host-reason" (result.UnsupportedHostReason |> Option.defaultValue "none")
              EvidenceReports.field "fallback" (result.Fallback |> Option.defaultValue "none")
              EvidenceReports.field "screenshot-path" (result.ScreenshotPath |> Option.defaultValue "none")
              EvidenceReports.field "width" (result.Width |> Option.map string |> Option.defaultValue "none")
              EvidenceReports.field "height" (result.Height |> Option.map string |> Option.defaultValue "none")
              EvidenceReports.field "frames-rendered" (result.FramesRendered |> Option.map string |> Option.defaultValue "none")
              EvidenceReports.field "diagnostics" (String.concat "|" result.Diagnostics) ] }

let validation = EvidenceReports.validate report

printfn "# Screenshot Evidence"
printfn ""
printfn "## Public API Transcript"
printfn ""
printfn "command=dotnet fsi specs/022-breakout-demo-feedback/readiness/screenshot-evidence.fsx"
printfn "status=%s" (EvidenceReports.statusText report.Status)
printfn "exit-code=%d" report.ExitCode
for line in report.Lines do
    printfn "%s" line
printfn "validation-accepted=%b" validation.Accepted
if validation.Diagnostics.IsEmpty then
    printfn "validation-diagnostics=none"
else
    printfn "validation-diagnostics=%s" (String.concat "|" validation.Diagnostics)
printfn ""
printfn "## Matrix"
printfn ""
printfn "| Host capability | Status | Screenshot proof | Fallback | Owner |"
printfn "|-----------------|--------|------------------|----------|-------|"
match result.Status with
| ScreenshotOk ->
    printfn "| current host | ok | %s | none | SkiaViewer |" (result.ScreenshotPath |> Option.defaultValue "missing-path")
    printfn "| unsupported host | unsupported | none | deterministic-scene-evidence | SkiaViewer |"
| ScreenshotUnsupported ->
    printfn "| supported host | blocked: capture capability missing | none | n/a | SkiaViewer host screenshot capture |"
    printfn "| current unsupported host | unsupported | none | deterministic-scene-evidence | SkiaViewer |"
| ScreenshotFailed ->
    printfn "| current host | failed | none | %s | SkiaViewer |" (result.Fallback |> Option.defaultValue "none")

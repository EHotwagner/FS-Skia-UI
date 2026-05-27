#r "../../../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../../../src/Lib/bin/Debug/net10.0/FS.Skia.UI.dll"
#r "../../../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../../../src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.SkiaViewer.dll"
#r "../../../src/Testing/bin/Debug/net10.0/FS.Skia.UI.Testing.dll"

open System
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer
open FS.Skia.UI.Testing

let fill = Colors.rgb 10uy 20uy 30uy
let circle = Scene.circle { X = 12.0; Y = 16.0 } 8.0 fill
let circleFacts = Scene.circleEvidence { Width = 64; Height = 64 } { X = 12.0; Y = 16.0 } 8.0 fill
let ellipse = Scene.filledEllipse { X = 2.0; Y = 4.0; Width = 24.0; Height = 12.0 } fill
let ellipseFacts = Scene.ellipseEvidence { Width = 64; Height = 64 } { X = 2.0; Y = 4.0; Width = 24.0; Height = 12.0 } fill

printfn "circle-kind=%A" (Scene.describe circle)
printfn "circle-bounds=%A placement=%A" circleFacts.Bounds circleFacts.Placement
printfn "ellipse-kind=%A" (Scene.describe ellipse)
printfn "ellipse-bounds=%A placement=%A" ellipseFacts.Bounds ellipseFacts.Placement

let screenshotRequest =
    { Command = "fsi"
      OutputPath = "readiness/screenshot-evidence.md"
      Width = 64
      Height = 64
      RendererMode = "skia"
      Timeout = TimeSpan.FromSeconds 1.0 }

let screenshot =
    Viewer.captureScreenshotEvidence
        screenshotRequest
        { Title = "FSI"; InitialSize = { Width = 64; Height = 64 } }
        (Group [])

printfn "screenshot-status=%A fallback=%A" screenshot.Status screenshot.Fallback

let report =
    EvidenceReports.build
        { Status = EvidenceUnsupported
          Command = "fsi"
          OutputPath = Some "readiness/report.txt"
          Fields =
            [ EvidenceReports.field "unsupported-host-reason" "capture unavailable"
              EvidenceReports.field "fallback" "deterministic-scene-evidence" ] }

printfn "report-lines=%A exit=%d" report.Lines report.ExitCode


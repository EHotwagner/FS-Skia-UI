#I "../../../../src/Scene/bin/Debug/net10.0"
#I "../../../../src/KeyboardInput/bin/Debug/net10.0"
#I "../../../../src/SkiaViewer/bin/Debug/net10.0"
#I "/home/developer/.nuget/packages/skiasharp/4.147.0-preview.3.1/lib/net10.0"

#r "SkiaSharp.dll"
#r "FS.Skia.UI.Scene.dll"
#r "FS.Skia.UI.KeyboardInput.dll"
#r "FS.Skia.UI.SkiaViewer.dll"

open System
open System.IO
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

let readiness = "specs/026-working-screenshot-taking/readiness"
let recordPath = Path.Combine(readiness, "artifacts", "working-screenshot-record.txt")

let request: ScreenshotEvidenceRequest =
    { Command = "dotnet fsi specs/026-working-screenshot-taking/readiness/fsi/screenshot-smoke.fsx"
      AppOrSample = "working-screenshot-taking-smoke"
      OutputPath = recordPath
      Width = 320
      Height = 200
      RendererMode = "skia"
      CaptureMode = ViewerRenderTargetPng
      HostFacts = [ $"os={Environment.OSVersion.Platform}"; $"machine={Environment.MachineName}" ]
      Timeout = TimeSpan.FromSeconds 5.0 }

let scene =
    Group
        [ { Nodes =
                [ Rectangle((24.0, 24.0, 180.0, 96.0), Colors.rgb 82uy 184uy 136uy)
                  Text((36.0, 88.0), "screenshot", Colors.white) ] } ]

let result =
    Viewer.captureScreenshotEvidence request { Title = "Screenshot Smoke"; InitialSize = { Width = 320; Height = 200 } } scene

Directory.CreateDirectory(Path.GetDirectoryName(recordPath)) |> ignore

let valueOrNone value =
    value |> Option.map string |> Option.defaultValue "none"

let hostFacts = String.concat "," result.HostFacts

let lines =
    [ $"status={result.Status}"
      $"command={result.Command}"
      $"app-or-sample={result.AppOrSample}"
      $"host-facts={hostFacts}"
      $"capture-mode={result.CaptureMode}"
      $"evidence-kind={result.EvidenceKind}"
      $"artifact-path={valueOrNone result.ScreenshotPath}"
      $"image-width={valueOrNone result.Width}"
      $"image-height={valueOrNone result.Height}"
      $"pixel-content-validation={result.PixelContentValidation}"
      $"capture-source={result.CaptureSource}"
      $"proves-screenshot={result.ProvesScreenshot}"
      $"blocked-stage={valueOrNone result.BlockedStage}"
      $"classification={valueOrNone result.Classification}"
      $"category={valueOrNone result.Category}"
      $"message={result.Message}"
      $"timestamp={result.Timestamp:O}" ]

File.WriteAllLines(recordPath, lines)
lines |> List.iter (printfn "%s")

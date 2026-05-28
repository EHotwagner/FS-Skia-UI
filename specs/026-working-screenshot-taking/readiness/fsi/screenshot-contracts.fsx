#I "../../../../src/Scene/bin/Debug/net10.0"
#I "../../../../src/KeyboardInput/bin/Debug/net10.0"
#I "../../../../src/SkiaViewer/bin/Debug/net10.0"
#I "../../../../src/Testing/bin/Debug/net10.0"

#r "FS.Skia.UI.Scene.dll"
#r "FS.Skia.UI.KeyboardInput.dll"
#r "FS.Skia.UI.SkiaViewer.dll"
#r "FS.Skia.UI.Testing.dll"

open System
open FS.Skia.UI.SkiaViewer
open FS.Skia.UI.Testing

let request: ScreenshotEvidenceRequest =
    { Command = "dotnet run -- --screenshot-evidence readiness/screenshot-evidence.md"
      AppOrSample = "fsi-contract-sample"
      OutputPath = "readiness/screenshot-evidence.md"
      Width = 64
      Height = 64
      RendererMode = "skia"
      CaptureMode = ViewerRenderTargetPng
      HostFacts = [ "host=fsi"; "desktop-session=not-launched" ]
      Timeout = TimeSpan.FromSeconds 2.0 }

let model, effects = Viewer.initEvidenceWorkflow request
printfn "init-viewer-open=%A effects=%A" model.ViewerOpenStatus effects

let launched, launchEffects = Viewer.updateEvidenceWorkflow (LaunchCompleted ViewerOpenConfirmed) model
printfn "launch-viewer-open=%A effects=%A" launched.ViewerOpenStatus launchEffects

let firstFrame, captureEffects = Viewer.updateEvidenceWorkflow (FirstFrameObserved FirstFramePresentedStatus) launched
printfn "first-frame=%A effects=%A" firstFrame.FirstFrameStatus captureEffects

let completed, writeEffects =
    Viewer.updateEvidenceWorkflow
        (CaptureSucceeded("readiness/artifacts/fsi-screenshot.png", 64, 64, LiveViewerWindow))
        firstFrame

let result =
    match completed.Result with
    | Some value -> value
    | None -> failwith "expected screenshot workflow result"

printfn "result-status=%A proves-screenshot=%b pixel-validation=%A effects=%A" result.Status result.ProvesScreenshot result.PixelContentValidation writeEffects

let validation =
    EvidenceReports.validateScreenshotEvidence
        { Status = "ok"
          Command = Some request.Command
          AppOrSample = Some request.AppOrSample
          HostFacts = request.HostFacts
          CaptureMode = Some "viewer-render-target-png"
          EvidenceKind = Some "screenshot"
          ArtifactPath = Some "readiness/artifacts/fsi-screenshot.png"
          ScreenshotPath = Some "readiness/artifacts/fsi-screenshot.png"
          Width = Some 64
          Height = Some 64
          PixelContentValidation = Some "non-blank"
          CaptureSource = Some "live-viewer-window"
          ProvesScreenshot = Some true
          BlockedStage = Some "none"
          Classification = Some "none"
          Category = Some "none"
          Message = Some "validated by FSI contract transcript"
          Timestamp = Some DateTimeOffset.UnixEpoch
          ViewerOpenStatus = Some "confirmed"
          FirstFrameStatus = Some "presented"
          CaptureAvailability = Some "available"
          UnsupportedHostReason = None
          Fallback = None
          Diagnostics = [] }

if not validation.Accepted then
    failwithf "expected accepted screenshot evidence validation: %A" validation

printfn "testing-validation-accepted=%b missing=%A" validation.Accepted validation.MissingFields

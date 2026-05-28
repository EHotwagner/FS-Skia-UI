#r "../../../../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../../../../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../../../../src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.SkiaViewer.dll"

open System
open FS.Skia.UI.SkiaViewer

let assertEqual expected actual message =
    if expected <> actual then
        failwithf "%s: expected %A, got %A" message expected actual

let request =
    { Command = "--screenshot-evidence"
      OutputPath = "readiness/screenshot-evidence.md"
      Width = 320
      Height = 200
      RendererMode = "skia"
      Timeout = TimeSpan.FromSeconds 2.0 }

let model, effects = Viewer.initEvidenceWorkflow request
assertEqual ViewerOpenUnknown model.ViewerOpenStatus "initial viewer-open status"

match effects with
| [ LaunchViewerForEvidence launched ] -> assertEqual request launched "initial launch effect"
| other -> failwithf "expected launch effect, got %A" other

let opened, openEffects = Viewer.updateEvidenceWorkflow (LaunchCompleted ViewerOpenConfirmed) model
assertEqual ViewerOpenConfirmed opened.ViewerOpenStatus "launch status"
assertEqual [] openEffects "launch fact effects"

let firstFrame, captureEffects =
    Viewer.updateEvidenceWorkflow (FirstFrameObserved FirstFramePresentedStatus) opened

assertEqual FirstFramePresentedStatus firstFrame.FirstFrameStatus "first-frame status"

match captureEffects with
| [ CaptureViewerScreenshot output ] -> assertEqual request.OutputPath output "capture output path"
| other -> failwithf "expected capture effect, got %A" other

let completed, writeEffects =
    Viewer.updateEvidenceWorkflow
        (CaptureSucceeded("readiness/artifacts/screenshot.png", 320, 200, LiveViewerWindow))
        firstFrame

match completed.Result with
| Some result ->
    assertEqual ScreenshotOk result.Status "result status"
    assertEqual (Some "readiness/artifacts/screenshot.png") result.ScreenshotPath "screenshot artifact path"
    assertEqual CaptureAvailable result.CaptureAvailability "capture availability"
    assertEqual LiveViewerWindow result.CaptureSource "capture source"
    assertEqual true result.ProvesScreenshot "screenshot proof"

    match writeEffects with
    | [ WriteScreenshotEvidenceReport written ] -> assertEqual result written "write report effect"
    | other -> failwithf "expected write effect, got %A" other
| None -> failwith "expected workflow result"

printfn "T009 public evidence workflow FSI exercise passed"

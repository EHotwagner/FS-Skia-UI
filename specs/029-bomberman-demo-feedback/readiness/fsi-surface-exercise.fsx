#r "../../../src/Scene/bin/Release/net10.0/FS.Skia.UI.Scene.dll"
#r "../../../src/KeyboardInput/bin/Release/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../../../src/SkiaViewer/bin/Release/net10.0/FS.Skia.UI.dll"
#r "../../../src/SkiaViewer/bin/Release/net10.0/FS.Skia.UI.SkiaViewer.dll"
#r "../../../src/Elmish/bin/Release/net10.0/FS.Skia.UI.Elmish.dll"
#r "../../../src/Layout/bin/Release/net10.0/FS.Skia.UI.Layout.dll"
#r "../../../src/Testing/bin/Release/net10.0/FS.Skia.UI.Testing.dll"

open System
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer
open FS.Skia.UI.Elmish
open FS.Skia.UI.Layout
open FS.Skia.UI.Testing

let sceneSize: FS.Skia.UI.Scene.Size = { Width = 320; Height = 240 }
let scenePoint: FS.Skia.UI.Scene.Point = { X = 16.0; Y = 24.0 }
let sceneRect: FS.Skia.UI.Scene.Rect = { X = 0.0; Y = 0.0; Width = 320.0; Height = 240.0 }
let layoutBounds: FS.Skia.UI.Layout.LayoutBounds = { X = 0.0; Y = 32.0; Width = 320.0; Height = 208.0 }
let layoutSize: FS.Skia.UI.Layout.LayoutSize = { Width = Some 320.0; Height = Some 240.0 }

let diagnostics: ViewerDiagnosticsOptions =
    { MinimumLevel = ViewerDiagnosticLevel.Info
      Categories = Set.ofList [ ViewerDiagnosticCategory.Startup; ViewerDiagnosticCategory.Screenshot ]
      FrameLogLimit = Some 4
      Sink = None
      Verbose = false }

let viewerOptions: ViewerOptions =
    { Title = "Bomberman feedback FSI surface exercise"
      InitialSize = sceneSize }

let screenshotRequest: ScreenshotEvidenceRequest =
    { Command = "fsi-surface-exercise"
      AppOrSample = "foundation"
      OutputPath = "specs/029-bomberman-demo-feedback/readiness/fsi-surface-exercise.png"
      Width = sceneSize.Width
      Height = sceneSize.Height
      RendererMode = "software"
      CaptureMode = ScreenshotCaptureMode.ViewerRenderTargetPng
      HostFacts = [ "fsi=true" ]
      Timeout = TimeSpan.FromSeconds 1.0 }

let screenshotCheck: ScreenshotEvidenceReportCheck =
    { Status = "unsupported"
      Command = Some screenshotRequest.Command
      AppOrSample = Some screenshotRequest.AppOrSample
      HostFacts = screenshotRequest.HostFacts
      CaptureMode = Some "ViewerRenderTargetPng"
      EvidenceKind = Some "UnsupportedHost"
      ArtifactPath = None
      ScreenshotPath = None
      Width = None
      Height = None
      PixelContentValidation = None
      CaptureSource = Some "NoCaptureSource"
      ProvesScreenshot = Some false
      BlockedStage = Some "Capture"
      Classification = Some "UnsupportedEnvironment"
      Category = Some "Screenshot"
      Message = Some "surface exercise only"
      Timestamp = Some DateTimeOffset.UnixEpoch
      ViewerOpenStatus = Some "ViewerOpenUnknown"
      FirstFrameStatus = Some "FirstFrameUnknownStatus"
      CaptureAvailability = Some "CaptureAvailabilityUnknown:surface exercise"
      UnsupportedHostReason = Some "surface exercise"
      Fallback = Some "none"
      Diagnostics = [ "public shape constructed" ] }

let adapterModel, adapterEffects: ElmishAdapterModel<int> * ElmishAdapterEffect<int> list =
    ElmishAdapter.init viewerOptions 0 SceneNode.Empty

let layoutDiagnostic: FS.Skia.UI.Layout.LayoutDiagnostic =
    { NodeId = Some "root"
      Code = LayoutDiagnosticCode.FallbackBoundsApplied
      Severity = FS.Skia.UI.Layout.DiagnosticSeverity.Info
      Message = "surface exercise"
      Constraint = None
      FallbackApplied = true }

printfn "scene-size=%dx%d point=(%.1f,%.1f)" sceneSize.Width sceneSize.Height scenePoint.X scenePoint.Y
printfn "scene-rect=%.1fx%.1f layout-bounds=%.1fx%.1f" sceneRect.Width sceneRect.Height layoutBounds.Width layoutBounds.Height
printfn "layout-size=%A diagnostic=%A" layoutSize layoutDiagnostic.Code
printfn "viewer-title=%s diagnostics=%d" viewerOptions.Title diagnostics.Categories.Count
printfn "screenshot-command=%s capture=%A" screenshotRequest.Command screenshotRequest.CaptureMode
printfn "screenshot-check-status=%s source=%A" screenshotCheck.Status screenshotCheck.CaptureSource
printfn "elmish-model=%A effects=%d" adapterModel.UserModel adapterEffects.Length

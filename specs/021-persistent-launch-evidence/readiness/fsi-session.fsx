#r "../../../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../../../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../../../src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.SkiaViewer.dll"
#r "../../../src/Testing/bin/Debug/net10.0/FS.Skia.UI.Testing.dll"

open System
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer
open FS.Skia.UI.Testing

let options: ViewerOptions =
    { Title = "FSI Persistent Evidence"
      InitialSize = { Width = 640; Height = 480 } }

let request: ViewerRunRequest =
    { Target = FirstFrame
      Timeout = TimeSpan.FromSeconds 5.0
      Diagnostics = Viewer.defaultDiagnostics
      RendererMode = "skia"
      EvidencePath = Some "readiness/persistent-launch-evidence.md" }

let model, initEffects = Viewer.init options
printfn "viewer-init-state=%A effects=%A" model.LifecycleState initEffects

let evidenceModel, evidenceEffects = Viewer.update (StartEvidence request) model
printfn "viewer-evidence-state=%A effects=%A" evidenceModel.LifecycleState evidenceEffects

let frameModel, frameEffects = Viewer.update (FramePresented { Width = 640; Height = 480 }) evidenceModel
printfn "viewer-frame-presented=%b state=%A effects=%A" frameModel.FirstFramePresented frameModel.LifecycleState frameEffects

let closeModel, closeEffects = Viewer.update EvidenceCloseRequested frameModel
printfn "viewer-close-state=%A effects=%A" closeModel.LifecycleState closeEffects

let artifact =
    PersistentLaunchArtifactValidation.validate
        { ArtifactPath = "readiness/persistent-launch-evidence.md"
          Lines =
            [ "status=ok"
              "mode=interactive-window"
              "command=--launch-evidence"
              "window-opened=true"
              "first-frame-presented=true"
              "input-dispatch=not-required"
              "exit-path=true"
              "blocked-stage=none"
              "classification=ok"
              "category=none"
              "message=ok" ]
          SyntheticFixture = false
          SupportedHostPassClaimed = true }

printfn "artifact-accepted=%b missing=%A contradictions=%A" artifact.Accepted artifact.MissingFields artifact.Contradictions

let warning =
    HostWarningClassification.classify
        { RawMessage = "Failed to load module \"colorreload-gtk-module\""
          KnownBenignMarkers = [ "colorreload-gtk-module" ]
          LaunchSucceeded = true
          RenderingSucceeded = true
          LayoutReadable = Some true
          ExplicitlyUnsupportedWithoutReadabilityClaim = false
          PackageSucceeded = true
          EvidencePath = Some "readiness/host-warning-classification.md" }

printfn "warning-class=%A fatal=%b facts=%A" warning.WarningClass warning.Fatal warning.SupportingFacts

let discovery =
    ReadinessFileDiscovery.validate
        { ReadinessDirectory = "specs/021-persistent-launch-evidence/readiness"
          RequiredFiles =
            [ "persistent-launch-evidence.md"
              "window-observation-diagnostics.md"
              "host-warning-classification.md"
              "generated-guidance.md"
              "evidence-audit.md" ]
          ExistingFiles =
            [ "persistent-launch-evidence.md"
              "window-observation-diagnostics.md"
              "host-warning-classification.md"
              "generated-guidance.md"
              "evidence-audit.md" ] }

printfn "readiness-complete=%b missing=%A" discovery.Complete discovery.MissingFiles

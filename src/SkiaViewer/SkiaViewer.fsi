namespace FS.Skia.UI.SkiaViewer

open System
open FS.Skia.UI.KeyboardInput
open FS.Skia.UI.Scene

type ViewerOptions =
    { Title: string
      InitialSize: Size }

type ViewerLaunchMode =
    | InteractiveWindow
    | PersistentEvidence

type ViewerInputDispatchStatus =
    | Verified
    | NotVerified
    | NotRequired

type ViewerDiagnosticLevel =
    | Error
    | Warning
    | Info
    | Debug
    | Trace

type ViewerDiagnosticCategory =
    | Startup
    | EnvironmentSession
    | Input
    | Frame
    | Renderer
    | Vulkan
    | Skia
    | Swapchain
    | Scene
    | Screenshot

type ViewerRunBlockedStage =
    | Window
    | Surface
    | Renderer
    | Swapchain
    | Scene
    | Readback
    | App
    | Timeout
    | Unknown

type ViewerRunFailureClassification =
    | UnsupportedEnvironment
    | PackageResolution
    | VerificationDepth
    | AppLifecycle
    | ProductDefect

type ViewerDiagnosticEvent =
    { Level: ViewerDiagnosticLevel
      Category: ViewerDiagnosticCategory
      Message: string
      FrameIndex: int option
      Stage: ViewerRunBlockedStage option
      Elapsed: TimeSpan option }

type ViewerDiagnosticsOptions =
    { MinimumLevel: ViewerDiagnosticLevel
      Categories: Set<ViewerDiagnosticCategory>
      FrameLogLimit: int option
      Sink: (ViewerDiagnosticEvent -> unit) option
      Verbose: bool }

type ViewerEvidenceTarget =
    | FirstFrame
    | FrameCount of int
    | Duration of TimeSpan

type ViewerRunRequest =
    { Target: ViewerEvidenceTarget
      Timeout: TimeSpan
      Diagnostics: ViewerDiagnosticsOptions
      RendererMode: string
      EvidencePath: string option }

type ViewerRunEvidence =
    { FramesRendered: int
      Elapsed: TimeSpan
      InitialOutputSize: Size
      RendererMode: string
      LastDiagnosticSummary: string option
      EvidencePath: string option }

type ViewerRunFailure =
    { BlockedStage: ViewerRunBlockedStage
      Classification: ViewerRunFailureClassification
      DiagnosticCategory: ViewerDiagnosticCategory
      Message: string
      LastDiagnosticSummary: string option }

type ViewerRuntimeCapability =
    { PersistentWindow: bool
      BoundedSmoke: bool
      KeyboardInput: bool
      RendererMode: string
      UnsupportedHostReasons: string list
      MissingPackageCapabilities: string list }

type ViewerDesktopSessionDiagnostic =
    { RuntimeDirectory: string option
      RuntimeDirectoryExists: bool
      RuntimeDirectoryOwnerSuitable: bool
      RuntimeDirectoryPermissionsSuitable: bool
      DisplayVariable: string option
      DisplaySocket: string option
      DisplaySocketExists: bool
      SessionBus: string option
      FallbackRuntimeDirectory: string option
      FallbackIsFullDesktopSession: bool
      DiagnosticClass: string
      Message: string }

type ViewerLaunchOutcome =
    { Status: string
      Mode: string
      Command: string option
      RendererMode: string
      WindowOpened: bool
      FirstFramePresented: bool
      UserCloseObserved: bool
      SelfClosedForEvidence: bool
      InputDispatch: string
      ExitPath: bool
      BlockedStage: ViewerRunBlockedStage option
      Classification: ViewerRunFailureClassification option
      Category: ViewerDiagnosticCategory option
      Message: string }

type ViewerLifecycleState =
    | NotStarted
    | CheckingDesktopSession
    | StartingWindow
    | InteractiveRunning
    | EvidenceRunning
    | FirstFramePresented
    | Closing
    | Failed
    | Unsupported

type ViewerModel =
    { Options: ViewerOptions
      IsRunning: bool
      LifecycleState: ViewerLifecycleState
      FirstFramePresented: bool
      UserCloseObserved: bool
      InputDispatch: ViewerInputDispatchStatus
      LastScene: SceneNode option }

type ViewerRunModel =
    { Request: ViewerRunRequest
      FramesRendered: int
      StartedAt: DateTimeOffset option
      LastDiagnostic: ViewerDiagnosticEvent option
      Completed: Result<ViewerRunEvidence, ViewerRunFailure> option }

type ViewerMsg =
    | Start
    | StartInteractive
    | StartEvidence of ViewerRunRequest
    | Stop
    | Render of SceneNode
    | KeyEvent of ViewerKeyEvent
    | DiagnosticCaptured of ViewerDiagnosticEvent
    | FramePresented of Size
    | UserCloseObserved
    | EvidenceTargetReached
    | RunFailed of ViewerRunFailure
    | RunTimedOut

type ViewerRunMsg =
    | BeginRun
    | RunStarted of DateTimeOffset
    | RecordFrame of Size
    | RecordDiagnostic of ViewerDiagnosticEvent
    | CompleteRun
    | FailRun of ViewerRunFailure
    | TimeoutRun

type ViewerEffect =
    | OpenWindow of title: string * size: Size
    | RenderScene of SceneNode
    | CloseWindow
    | DispatchInput of ViewerKey * isDown: bool
    | EmitDiagnostic of ViewerDiagnosticEvent
    | CheckDesktopSession
    | StartBoundedRun of ViewerRunRequest
    | CaptureScreenshot of path: string
    | ReadPixels
    | WriteRunEvidence of path: string * evidence: ViewerRunEvidence

type ViewerRunEffect =
    | OpenBoundedWindow of ViewerRunRequest
    | RequestFrame
    | CaptureOutputSize
    | StopBoundedRun
    | PersistRunEvidence of ViewerRunEvidence

type GeneratedAppHost<'model,'msg> =
    { Init: unit -> 'model * ViewerEffect list
      Update: 'msg -> 'model -> 'model * ViewerEffect list
      View: 'model -> SceneNode
      MapKey: ViewerKey -> bool -> 'msg option
      Tick: TimeSpan -> 'msg option
      Diagnostics: ViewerDiagnosticsOptions }

module Viewer =
    val init: options: ViewerOptions -> ViewerModel * ViewerEffect list
    val update: msg: ViewerMsg -> model: ViewerModel -> ViewerModel * ViewerEffect list
    val initRun: request: ViewerRunRequest -> ViewerRunModel * ViewerRunEffect list
    val updateRun: msg: ViewerRunMsg -> model: ViewerRunModel -> ViewerRunModel * ViewerRunEffect list
    val defaultDiagnostics: ViewerDiagnosticsOptions
    val shouldCaptureDiagnostic: options: ViewerDiagnosticsOptions -> diagnostic: ViewerDiagnosticEvent -> bool
    val captureDiagnostic: options: ViewerDiagnosticsOptions -> diagnostic: ViewerDiagnosticEvent -> ViewerDiagnosticEvent option
    val failureFromDiagnostic: diagnostic: ViewerDiagnosticEvent -> ViewerRunFailure
    val desktopSessionDiagnostic: unit -> ViewerDesktopSessionDiagnostic
    val runtimeCapability: unit -> ViewerRuntimeCapability
    val run: options: ViewerOptions -> scene: SceneNode -> Result<ViewerLaunchOutcome, ViewerRunFailure>
    val runApp: options: ViewerOptions -> host: GeneratedAppHost<'model,'msg> -> Result<ViewerLaunchOutcome, ViewerRunFailure>
    val runAppEvidence: request: ViewerRunRequest -> options: ViewerOptions -> host: GeneratedAppHost<'model,'msg> -> Result<ViewerLaunchOutcome, ViewerRunFailure>
    val runBounded: request: ViewerRunRequest -> options: ViewerOptions -> scene: SceneNode -> Result<ViewerRunEvidence, ViewerRunFailure>
    val runUntilFirstFrame: options: ViewerOptions -> scene: SceneNode -> Result<ViewerRunEvidence, ViewerRunFailure>
    val runForFrames: frameCount: int -> options: ViewerOptions -> scene: SceneNode -> Result<ViewerRunEvidence, ViewerRunFailure>

module GeneratedAppHost =
    val dispatchKey: host: GeneratedAppHost<'model,'msg> -> raw: ViewerKeyEvent -> model: 'model -> 'model * ViewerEffect list
    val smoke: host: GeneratedAppHost<'model,'msg> -> request: ViewerRunRequest -> Result<ViewerRunEvidence, ViewerRunFailure>

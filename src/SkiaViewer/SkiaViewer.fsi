namespace FS.Skia.UI.SkiaViewer

open System
open FS.Skia.UI.KeyboardInput
open FS.Skia.UI.Scene

type ViewerOptions =
    { Title: string
      InitialSize: Size }

type ViewerDiagnosticLevel =
    | Error
    | Warning
    | Info
    | Debug
    | Trace

type ViewerDiagnosticCategory =
    | Startup
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

type ViewerModel =
    { Options: ViewerOptions
      IsRunning: bool
      LastScene: SceneNode option }

type ViewerRunModel =
    { Request: ViewerRunRequest
      FramesRendered: int
      StartedAt: DateTimeOffset option
      LastDiagnostic: ViewerDiagnosticEvent option
      Completed: Result<ViewerRunEvidence, ViewerRunFailure> option }

type ViewerMsg =
    | Start
    | Stop
    | Render of SceneNode
    | KeyEvent of ViewerKeyEvent
    | DiagnosticCaptured of ViewerDiagnosticEvent
    | FramePresented of Size
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
    | StartBoundedRun of ViewerRunRequest
    | WriteRunEvidence of path: string * evidence: ViewerRunEvidence

type ViewerRunEffect =
    | OpenBoundedWindow of ViewerRunRequest
    | RequestFrame
    | CaptureOutputSize
    | StopBoundedRun
    | PersistRunEvidence of ViewerRunEvidence

module Viewer =
    val init: options: ViewerOptions -> ViewerModel * ViewerEffect list
    val update: msg: ViewerMsg -> model: ViewerModel -> ViewerModel * ViewerEffect list
    val initRun: request: ViewerRunRequest -> ViewerRunModel * ViewerRunEffect list
    val updateRun: msg: ViewerRunMsg -> model: ViewerRunModel -> ViewerRunModel * ViewerRunEffect list
    val defaultDiagnostics: ViewerDiagnosticsOptions
    val shouldCaptureDiagnostic: options: ViewerDiagnosticsOptions -> diagnostic: ViewerDiagnosticEvent -> bool
    val captureDiagnostic: options: ViewerDiagnosticsOptions -> diagnostic: ViewerDiagnosticEvent -> ViewerDiagnosticEvent option
    val failureFromDiagnostic: diagnostic: ViewerDiagnosticEvent -> ViewerRunFailure
    val runBounded: request: ViewerRunRequest -> options: ViewerOptions -> scene: SceneNode -> Result<ViewerRunEvidence, ViewerRunFailure>
    val runUntilFirstFrame: options: ViewerOptions -> scene: SceneNode -> Result<ViewerRunEvidence, ViewerRunFailure>
    val runForFrames: frameCount: int -> options: ViewerOptions -> scene: SceneNode -> Result<ViewerRunEvidence, ViewerRunFailure>

type GeneratedAppHost<'model,'msg> =
    { Init: unit -> 'model * ViewerEffect list
      Update: 'msg -> 'model -> 'model * ViewerEffect list
      View: 'model -> SceneNode
      MapKey: ViewerKey -> bool -> 'msg option
      Tick: TimeSpan -> 'msg option
      Diagnostics: ViewerDiagnosticsOptions }

module GeneratedAppHost =
    val dispatchKey: host: GeneratedAppHost<'model,'msg> -> raw: ViewerKeyEvent -> model: 'model -> 'model * ViewerEffect list
    val smoke: host: GeneratedAppHost<'model,'msg> -> request: ViewerRunRequest -> Result<ViewerRunEvidence, ViewerRunFailure>

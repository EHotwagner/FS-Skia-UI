namespace FS.Skia.UI.SkiaViewer.Host

open System
open Elmish
open FS.Skia.UI.Scene

type DiagnosticOptions =
    { Verbose: bool }

type ViewerConfiguration =
    { Title: string
      InitialSize: Size
      ClearColor: Color option
      TargetFrameRate: int option
      Diagnostics: DiagnosticOptions }

type DiagnosticSeverity =
    | Info
    | Warning
    | Error
    | Fatal

type DiagnosticStage =
    | PlatformCheck
    | VulkanInstance
    | VulkanDevice
    | VulkanSurface
    | VulkanSwapchain
    | SkiaContext
    | FrameRender
    | ScreenshotCapture
    | Shutdown

type RenderDiagnostic =
    { Severity: DiagnosticSeverity
      Stage: DiagnosticStage
      Message: string
      Cause: string option }

type ViewerEvent =
    | Loaded
    | UpdateTick of elapsedSeconds: float
    | RenderTick of elapsedSeconds: float
    | KeyDown of key: string
    | KeyUp of key: string
    | PointerMoved of x: float * y: float
    | PointerPressed of x: float * y: float
    | PointerReleased of x: float * y: float
    | Resized of Size
    | CloseRequested
    | DiagnosticReported of RenderDiagnostic

type ScreenshotFormat =
    | Png
    | Jpeg

type ScreenshotRequest =
    { Destination: string
      Format: ScreenshotFormat }

type ViewerEffect<'msg> =
    | InitializeRenderer
    | RenderFrame of Scene
    | CaptureScreenshot of ScreenshotRequest
    | Shutdown
    | ReportDiagnostic of RenderDiagnostic
    | Dispatch of 'msg

type ViewerProgram<'model, 'msg> =
    { Configuration: ViewerConfiguration
      Init: unit -> 'model * Cmd<'msg>
      Update: 'msg -> 'model -> 'model * Cmd<'msg>
      View: 'model -> Scene
      EventMapper: ViewerEvent -> 'msg option
      EffectMapper: 'msg -> ViewerEffect<'msg> option
      Subscriptions: 'model -> (string list * (Dispatch<'msg> -> IDisposable)) list }

module Diagnostics =
    let create severity stage message cause : RenderDiagnostic =
        { Severity = severity
          Stage = stage
          Message = message
          Cause = cause }

    let unsupportedPlatform (platform: string) =
        create Fatal PlatformCheck $"Unsupported platform '{platform}'. Vulkan desktop support is limited to Windows and Linux." None

    let invalidConfiguration message =
        create DiagnosticSeverity.Error DiagnosticStage.PlatformCheck message None

    let vulkanUnavailable detail =
        create DiagnosticSeverity.Fatal DiagnosticStage.VulkanInstance "Vulkan initialization is unavailable. The viewer has no fallback renderer." (Some detail)

    let missingCapability (capability: string) detail =
        create DiagnosticSeverity.Warning DiagnosticStage.SkiaContext $"Skia capability '{capability}' is unavailable on the active Vulkan device." (Some detail)

    let invalidPath detail =
        create DiagnosticSeverity.Error DiagnosticStage.FrameRender "Invalid path declaration." (Some detail)

    let unavailableFont (family: string) =
        create DiagnosticSeverity.Warning DiagnosticStage.FrameRender $"Font family '{family}' is unavailable; platform font resolution will choose a substitute." None

    let frameRenderFailed detail =
        create DiagnosticSeverity.Error DiagnosticStage.FrameRender "Vulkan/Skia frame rendering failed. The viewer has no fallback renderer." (Some detail)

    let screenshotFailed detail =
        create DiagnosticSeverity.Error DiagnosticStage.ScreenshotCapture "Screenshot capture failed." (Some detail)

    let shutdownFailed detail =
        create DiagnosticSeverity.Warning DiagnosticStage.Shutdown "Renderer shutdown failed." (Some detail)

    let startupFailed stage detail =
        create DiagnosticSeverity.Fatal stage "Vulkan initialization failed. The viewer has no fallback renderer." (Some detail)

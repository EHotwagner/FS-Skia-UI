# Public API Contract: Vulkan Elmish Viewer

This contract guides the `.fsi` public surface. Names may be refined during FSI sketching, but the capability boundaries must remain intact.

## Module: `FS.Skia.UI`

### Types

```fsharp
type Size = { Width: int; Height: int }

type Color = { Red: byte; Green: byte; Blue: byte; Alpha: byte }

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

type Scene

type ScreenshotFormat =
  | Png
  | Jpeg

type ScreenshotRequest =
  { Destination: string
    Format: ScreenshotFormat }
```

### Scene Construction

```fsharp
module Scene =
  val empty : Scene
  val group : Scene list -> Scene
  val rectangle : bounds: float * float * float * float -> fill: Color -> Scene
  val text : position: float * float -> text: string -> color: Color -> Scene
  val image : bounds: float * float * float * float -> source: string -> Scene
  val chart : values: float list -> Scene
```

### Elmish Viewer Program

```fsharp
type ViewerEffect<'msg> =
  | InitializeRenderer
  | RenderFrame of Scene
  | CaptureScreenshot of ScreenshotRequest
  | Shutdown
  | ReportDiagnostic of RenderDiagnostic
  | Dispatch of 'msg

type ViewerProgram<'model, 'msg> =
  { Init: unit -> 'model * Cmd<'msg>
    Update: 'msg -> 'model -> 'model * Cmd<'msg>
    View: 'model -> Scene
    EventMapper: ViewerEvent -> 'msg option
    EffectMapper: 'msg -> ViewerEffect<'msg> option
    Subscriptions: 'model -> (string list * (Dispatch<'msg> -> IDisposable)) list }

module Viewer =
  val create :
    configuration: ViewerConfiguration ->
    init: (unit -> 'model * Cmd<'msg>) ->
    update: ('msg -> 'model -> 'model * Cmd<'msg>) ->
    view: ('model -> Scene) ->
    ViewerProgram<'model, 'msg>

  val withSubscription :
    subscription: ('model -> (string list * (Dispatch<'msg> -> IDisposable)) list) ->
    program: ViewerProgram<'model, 'msg> ->
    ViewerProgram<'model, 'msg>

  val withEventMapping :
    mapper: (ViewerEvent -> 'msg option) ->
    program: ViewerProgram<'model, 'msg> ->
    ViewerProgram<'model, 'msg>

  val withEffectMapping :
    mapper: ('msg -> ViewerEffect<'msg> option) ->
    program: ViewerProgram<'model, 'msg> ->
    ViewerProgram<'model, 'msg>

  val run : program: ViewerProgram<'model, 'msg> -> Result<unit, RenderDiagnostic>
```

## Contract Rules

- No public API exposes renderer selection, fallback selection, OpenGL, CPU renderer, or legacy integration mode.
- `update` remains pure. Vulkan, filesystem, windowing, and clock work are executed only by the viewer host/interpreter.
- Startup failure returns or reports `RenderDiagnostic` before a partially functional window is shown.
- Public modules must be declared in `.fsi` before `.fs` implementation.
- Sample applications must consume the same public API as external users.

## Semantic Test Expectations

- FSI/prelude can construct `ViewerConfiguration`, `Scene`, and a minimal `ViewerProgram`.
- Pure `update` tests assert model transitions and emitted commands/effects.
- Unsupported renderer or platform checks produce `RenderDiagnostic` without fallback.
- Sample apps compile and run from documented commands.

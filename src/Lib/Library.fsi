namespace FS.Skia.UI

open System
open Elmish

/// Public contract type exposed by this FS.Skia.UI package.
type Size =
    { Width: int
      Height: int }

/// Public contract type exposed by this FS.Skia.UI package.
type Color =
    { Red: byte
      Green: byte
      Blue: byte
      Alpha: byte }

/// Public contract type exposed by this FS.Skia.UI package.
type Point =
    { X: float
      Y: float }

/// Public contract type exposed by this FS.Skia.UI package.
type Rect =
    { X: float
      Y: float
      Width: float
      Height: float }

/// Public contract type exposed by this FS.Skia.UI package.
type Matrix =
    { M11: float
      M12: float
      M21: float
      M22: float
      OffsetX: float
      OffsetY: float }

/// Public contract type exposed by this FS.Skia.UI package.
type StrokeCap =
    | Butt
    | Round
    | Square

/// Public contract type exposed by this FS.Skia.UI package.
type StrokeJoin =
    | Miter
    | RoundJoin
    | Bevel

/// Public contract type exposed by this FS.Skia.UI package.
type BlendMode =
    | SrcOver
    | Multiply
    | Screen
    | Overlay
    | Darken
    | Lighten
    | ColorDodge
    | ColorBurn
    | Difference
    | Exclusion

/// Public contract type exposed by this FS.Skia.UI package.
type Stroke =
    { Width: float
      Cap: StrokeCap
      Join: StrokeJoin
      Miter: float }

/// Public contract type exposed by this FS.Skia.UI package.
type Shader =
    | SolidColor of Color
    | LinearGradient of startPoint: Point * endPoint: Point * colors: Color list
    | RadialGradient of center: Point * radius: float * colors: Color list
    | SweepGradient of center: Point * colors: Color list

/// Public contract type exposed by this FS.Skia.UI package.
type ColorFilter =
    | NoColorFilter
    | BlendColor of Color * BlendMode

/// Public contract type exposed by this FS.Skia.UI package.
type MaskFilter =
    | NoMaskFilter
    | Blur of sigma: float

/// Public contract type exposed by this FS.Skia.UI package.
type ImageFilter =
    | NoImageFilter
    | DropShadow of dx: float * dy: float * blur: float * color: Color

/// Public contract type exposed by this FS.Skia.UI package.
type PathEffect =
    | NoPathEffect
    | Dash of intervals: float list * phase: float
    | Discrete of segmentLength: float * deviation: float
    | Corner of radius: float

/// Public contract type exposed by this FS.Skia.UI package.
type Paint =
    { Fill: Color option
      Stroke: Stroke option
      Opacity: float
      Antialias: bool
      BlendMode: BlendMode
      Shader: Shader option
      ColorFilter: ColorFilter
      MaskFilter: MaskFilter
      ImageFilter: ImageFilter
      PathEffect: PathEffect }

/// Public contract type exposed by this FS.Skia.UI package.
type PathFillType =
    | Winding
    | EvenOdd

/// Public contract type exposed by this FS.Skia.UI package.
type PathCommand =
    | MoveTo of Point
    | LineTo of Point
    | QuadTo of control: Point * point: Point
    | CubicTo of control1: Point * control2: Point * point: Point
    | ArcTo of bounds: Rect * startAngle: float * sweepAngle: float
    | Close

/// Public contract type exposed by this FS.Skia.UI package.
type PathSpec =
    { Commands: PathCommand list
      FillType: PathFillType }

/// Public contract type exposed by this FS.Skia.UI package.
type Clip =
    | RectClip of Rect
    | PathClip of PathSpec

/// Public contract type exposed by this FS.Skia.UI package.
type RegionOperation =
    | Replace
    | RegionUnion
    | RegionIntersect
    | RegionDifference

/// Public contract type exposed by this FS.Skia.UI package.
type Region =
    { Bounds: Rect list
      Operation: RegionOperation }

/// Public contract type exposed by this FS.Skia.UI package.
type ColorSpace =
    | Srgb
    | DisplayP3
    | AdobeRgb

/// Public contract type exposed by this FS.Skia.UI package.
type PerspectiveTransform =
    { M11: float
      M12: float
      M13: float
      M21: float
      M22: float
      M23: float
      M31: float
      M32: float
      M33: float }

/// Public contract type exposed by this FS.Skia.UI package.
type PathOperation =
    | Union
    | Intersect
    | Difference
    | Xor

/// Public contract type exposed by this FS.Skia.UI package.
type PathMeasure =
    { Length: float
      IsClosed: bool }

/// Public contract type exposed by this FS.Skia.UI package.
type FontSpec =
    { Family: string option
      Size: float
      Weight: int option }

/// Public contract type exposed by this FS.Skia.UI package.
type TextRun =
    { Text: string
      Position: Point
      Font: FontSpec
      Paint: Paint }

/// Public contract type exposed by this FS.Skia.UI package.
type TextMetrics =
    { Width: float
      Height: float
      Baseline: float }

/// Public contract type exposed by this FS.Skia.UI package.
type Vertex =
    { Position: Point
      Color: Color option }

/// Public contract type exposed by this FS.Skia.UI package.
type VertexMode =
    | Triangles
    | TriangleStrip
    | TriangleFan

/// Public contract type exposed by this FS.Skia.UI package.
type SceneElementKind =
    | EmptyElement
    | GroupElement
    | RectangleElement
    | EllipseElement
    | LineElement
    | PathElement
    | PointsElement
    | VerticesElement
    | ArcElement
    | TextElement
    | TextRunElement
    | ImageElement
    | ClipElement
    | RegionElement
    | ColorSpaceElement
    | PerspectiveElement
    | PictureElement
    | ChartElement

/// Public contract type exposed by this FS.Skia.UI package.
type RenderReadbackEvidence =
    { Size: Size
      CapabilityCount: int
      Capabilities: string list
      DeterministicHash: string }

/// Public contract type exposed by this FS.Skia.UI package.
type DiagnosticOptions =
    { Verbose: bool }

/// Public contract type exposed by this FS.Skia.UI package.
type ViewerConfiguration =
    { Title: string
      InitialSize: Size
      ClearColor: Color option
      TargetFrameRate: int option
      Diagnostics: DiagnosticOptions }

/// Public contract type exposed by this FS.Skia.UI package.
type DiagnosticSeverity =
    | Info
    | Warning
    | Error
    | Fatal

/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract type exposed by this FS.Skia.UI package.
type RenderDiagnostic =
    { Severity: DiagnosticSeverity
      Stage: DiagnosticStage
      Message: string
      Cause: string option }

/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract type exposed by this FS.Skia.UI package.
type Scene

/// Public contract type exposed by this FS.Skia.UI package.
type Picture =
    { Name: string
      Scene: Scene }

/// Public contract type exposed by this FS.Skia.UI package.
type ScreenshotFormat =
    | Png
    | Jpeg

/// Public contract type exposed by this FS.Skia.UI package.
type ScreenshotRequest =
    { Destination: string
      Format: ScreenshotFormat }

/// Public contract type exposed by this FS.Skia.UI package.
type ViewerEffect<'msg> =
    | InitializeRenderer
    | RenderFrame of Scene
    | CaptureScreenshot of ScreenshotRequest
    | Shutdown
    | ReportDiagnostic of RenderDiagnostic
    | Dispatch of 'msg

/// Public contract type exposed by this FS.Skia.UI package.
type ViewerProgram<'model, 'msg> =
    { Configuration: ViewerConfiguration
      Init: unit -> 'model * Cmd<'msg>
      Update: 'msg -> 'model -> 'model * Cmd<'msg>
      View: 'model -> Scene
      EventMapper: ViewerEvent -> 'msg option
      EffectMapper: 'msg -> ViewerEffect<'msg> option
      Subscriptions: 'model -> (string list * (Dispatch<'msg> -> IDisposable)) list }

/// Public contract type exposed by this FS.Skia.UI package.
type ParityStatus =
    | Supported
    | Adapted
    | Excluded
    | NotYetSupported

/// Public contract type exposed by this FS.Skia.UI package.
type EvidenceType =
    | SemanticTest
    | Screenshot
    | Smoke
    | Package
    | Documentation
    | ManualReview

/// Public contract type exposed by this FS.Skia.UI package.
type ParityEvidenceItem =
    { CapabilityId: string
      Capability: string
      Status: ParityStatus
      EvidenceType: EvidenceType
      Command: string
      Path: string
      AdaptationNotes: string
      ConflictsWithConstraints: bool }

/// Public contract type exposed by this FS.Skia.UI package.
type ParityReport =
    { Feature: string
      BaselineCommit: string
      Items: ParityEvidenceItem list }

/// Public contract module exposed by this FS.Skia.UI package.
module Colors =
    /// Public contract function exposed by this FS.Skia.UI package.
    val rgba : red: byte -> green: byte -> blue: byte -> alpha: byte -> Color
    /// Public contract function exposed by this FS.Skia.UI package.
    val black : Color
    /// Public contract function exposed by this FS.Skia.UI package.
    val white : Color
    /// Public contract function exposed by this FS.Skia.UI package.
    val transparent : Color

/// Public contract module exposed by this FS.Skia.UI package.
module Diagnostics =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create : severity: DiagnosticSeverity -> stage: DiagnosticStage -> message: string -> cause: string option -> RenderDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val unsupportedPlatform : platform: string -> RenderDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val invalidConfiguration : message: string -> RenderDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val vulkanUnavailable : detail: string -> RenderDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val missingCapability : capability: string -> detail: string -> RenderDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val invalidPath : detail: string -> RenderDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val unavailableFont : family: string -> RenderDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val frameRenderFailed : detail: string -> RenderDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val screenshotFailed : detail: string -> RenderDiagnostic
    /// Public contract function exposed by this FS.Skia.UI package.
    val shutdownFailed : detail: string -> RenderDiagnostic

/// Public contract module exposed by this FS.Skia.UI package.
module Paint =
    /// Public contract function exposed by this FS.Skia.UI package.
    val fill : color: Color -> Paint
    /// Public contract function exposed by this FS.Skia.UI package.
    val stroke : color: Color -> width: float -> Paint
    /// Public contract function exposed by this FS.Skia.UI package.
    val withOpacity : opacity: float -> paint: Paint -> Paint
    /// Public contract function exposed by this FS.Skia.UI package.
    val withAntialias : antialias: bool -> paint: Paint -> Paint
    /// Public contract function exposed by this FS.Skia.UI package.
    val withBlendMode : blendMode: BlendMode -> paint: Paint -> Paint
    /// Public contract function exposed by this FS.Skia.UI package.
    val withStrokeCap : cap: StrokeCap -> paint: Paint -> Paint
    /// Public contract function exposed by this FS.Skia.UI package.
    val withStrokeJoin : join: StrokeJoin -> paint: Paint -> Paint
    /// Public contract function exposed by this FS.Skia.UI package.
    val withMiter : miter: float -> paint: Paint -> Paint
    /// Public contract function exposed by this FS.Skia.UI package.
    val withShader : shader: Shader -> paint: Paint -> Paint
    /// Public contract function exposed by this FS.Skia.UI package.
    val withColorFilter : filter: ColorFilter -> paint: Paint -> Paint
    /// Public contract function exposed by this FS.Skia.UI package.
    val withMaskFilter : filter: MaskFilter -> paint: Paint -> Paint
    /// Public contract function exposed by this FS.Skia.UI package.
    val withImageFilter : filter: ImageFilter -> paint: Paint -> Paint
    /// Public contract function exposed by this FS.Skia.UI package.
    val withPathEffect : effect: PathEffect -> paint: Paint -> Paint

/// Public contract module exposed by this FS.Skia.UI package.
module Path =
    /// Public contract function exposed by this FS.Skia.UI package.
    val create : fillType: PathFillType -> commands: PathCommand list -> PathSpec
    /// Public contract function exposed by this FS.Skia.UI package.
    val moveTo : x: float -> y: float -> PathCommand
    /// Public contract function exposed by this FS.Skia.UI package.
    val lineTo : x: float -> y: float -> PathCommand
    /// Public contract function exposed by this FS.Skia.UI package.
    val quadTo : control: Point -> point: Point -> PathCommand
    /// Public contract function exposed by this FS.Skia.UI package.
    val cubicTo : control1: Point -> control2: Point -> point: Point -> PathCommand
    /// Public contract function exposed by this FS.Skia.UI package.
    val close : PathCommand
    /// Public contract function exposed by this FS.Skia.UI package.
    val bounds : path: PathSpec -> Rect option
    /// Public contract function exposed by this FS.Skia.UI package.
    val measure : path: PathSpec -> PathMeasure
    /// Public contract function exposed by this FS.Skia.UI package.
    val segment : startDistance: float -> endDistance: float -> path: PathSpec -> PathSpec
    /// Public contract function exposed by this FS.Skia.UI package.
    val combine : operation: PathOperation -> left: PathSpec -> right: PathSpec -> PathSpec

/// Public contract module exposed by this FS.Skia.UI package.
module Scene =
    /// Public contract function exposed by this FS.Skia.UI package.
    val empty : Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val group : Scene list -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val rectangle : bounds: float * float * float * float -> fill: Color -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val rectangleWithPaint : bounds: Rect -> paint: Paint -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val ellipse : bounds: Rect -> paint: Paint -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val line : startPoint: Point -> endPoint: Point -> paint: Paint -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val path : path: PathSpec -> paint: Paint -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val points : points: Point list -> paint: Paint -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val vertices : mode: VertexMode -> vertices: Vertex list -> paint: Paint -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val arc : bounds: Rect -> startAngle: float -> sweepAngle: float -> paint: Paint -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val text : position: float * float -> text: string -> color: Color -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val textRun : run: TextRun -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val measureText : text: string -> font: FontSpec -> TextMetrics
    /// Public contract function exposed by this FS.Skia.UI package.
    val image : bounds: float * float * float * float -> source: string -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val clipped : clip: Clip -> scene: Scene -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val region : region: Region -> paint: Paint -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val withColorSpace : colorSpace: ColorSpace -> scene: Scene -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val withPerspective : transform: PerspectiveTransform -> scene: Scene -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val picture : picture: Picture -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val chart : values: float list -> Scene
    /// Public contract function exposed by this FS.Skia.UI package.
    val describe : scene: Scene -> SceneElementKind list
    /// Public contract function exposed by this FS.Skia.UI package.
    val diagnostics : scene: Scene -> RenderDiagnostic list
    /// Public contract function exposed by this FS.Skia.UI package.
    val renderReadbackEvidence : size: Size -> scene: Scene -> RenderReadbackEvidence

/// Public contract module exposed by this FS.Skia.UI package.
module Parity =
    /// Public contract function exposed by this FS.Skia.UI package.
    val baselineCommit : string
    /// Public contract function exposed by this FS.Skia.UI package.
    val capabilityIds : string list
    /// Public contract function exposed by this FS.Skia.UI package.
    val createItem :
        capabilityId: string ->
        capability: string ->
        status: ParityStatus ->
        evidenceType: EvidenceType ->
        command: string ->
        path: string ->
        adaptationNotes: string ->
        conflictsWithConstraints: bool ->
            ParityEvidenceItem
    /// Public contract function exposed by this FS.Skia.UI package.
    val createReport : feature: string -> items: ParityEvidenceItem list -> ParityReport
    /// Public contract function exposed by this FS.Skia.UI package.
    val validateMergeReady : report: ParityReport -> string list
    /// Public contract function exposed by this FS.Skia.UI package.
    val toJson : report: ParityReport -> string
    /// Public contract function exposed by this FS.Skia.UI package.
    val writeJson : path: string -> report: ParityReport -> unit

/// Public contract module exposed by this FS.Skia.UI package.
module Viewer =
    /// Public contract function exposed by this FS.Skia.UI package.
    val defaultConfiguration : title: string -> initialSize: Size -> ViewerConfiguration

    /// Public contract function exposed by this FS.Skia.UI package.
    val create :
        configuration: ViewerConfiguration ->
        init: (unit -> 'model * Cmd<'msg>) ->
        update: ('msg -> 'model -> 'model * Cmd<'msg>) ->
        view: ('model -> Scene) ->
        ViewerProgram<'model, 'msg>

    /// Public contract function exposed by this FS.Skia.UI package.
    val withSubscription :
        subscription: ('model -> (string list * (Dispatch<'msg> -> IDisposable)) list) ->
        program: ViewerProgram<'model, 'msg> ->
        ViewerProgram<'model, 'msg>

    /// Public contract function exposed by this FS.Skia.UI package.
    val withEventMapping :
        mapper: (ViewerEvent -> 'msg option) ->
        program: ViewerProgram<'model, 'msg> ->
        ViewerProgram<'model, 'msg>

    /// Public contract function exposed by this FS.Skia.UI package.
    val withEffectMapping :
        mapper: ('msg -> ViewerEffect<'msg> option) ->
        program: ViewerProgram<'model, 'msg> ->
        ViewerProgram<'model, 'msg>

    /// Public contract function exposed by this FS.Skia.UI package.
    val run : program: ViewerProgram<'model, 'msg> -> Result<unit, RenderDiagnostic>

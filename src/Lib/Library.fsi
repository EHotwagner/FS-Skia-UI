namespace FS.Skia.UI

open System
open Elmish

type Size =
    { Width: int
      Height: int }

type Color =
    { Red: byte
      Green: byte
      Blue: byte
      Alpha: byte }

type Point =
    { X: float
      Y: float }

type Rect =
    { X: float
      Y: float
      Width: float
      Height: float }

type Matrix =
    { M11: float
      M12: float
      M21: float
      M22: float
      OffsetX: float
      OffsetY: float }

type StrokeCap =
    | Butt
    | Round
    | Square

type StrokeJoin =
    | Miter
    | RoundJoin
    | Bevel

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

type Stroke =
    { Width: float
      Cap: StrokeCap
      Join: StrokeJoin
      Miter: float }

type Shader =
    | SolidColor of Color
    | LinearGradient of startPoint: Point * endPoint: Point * colors: Color list
    | RadialGradient of center: Point * radius: float * colors: Color list
    | SweepGradient of center: Point * colors: Color list

type ColorFilter =
    | NoColorFilter
    | BlendColor of Color * BlendMode

type MaskFilter =
    | NoMaskFilter
    | Blur of sigma: float

type ImageFilter =
    | NoImageFilter
    | DropShadow of dx: float * dy: float * blur: float * color: Color

type PathEffect =
    | NoPathEffect
    | Dash of intervals: float list * phase: float
    | Discrete of segmentLength: float * deviation: float
    | Corner of radius: float

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

type PathFillType =
    | Winding
    | EvenOdd

type PathCommand =
    | MoveTo of Point
    | LineTo of Point
    | QuadTo of control: Point * point: Point
    | CubicTo of control1: Point * control2: Point * point: Point
    | ArcTo of bounds: Rect * startAngle: float * sweepAngle: float
    | Close

type PathSpec =
    { Commands: PathCommand list
      FillType: PathFillType }

type Clip =
    | RectClip of Rect
    | PathClip of PathSpec

type RegionOperation =
    | Replace
    | RegionUnion
    | RegionIntersect
    | RegionDifference

type Region =
    { Bounds: Rect list
      Operation: RegionOperation }

type ColorSpace =
    | Srgb
    | DisplayP3
    | AdobeRgb

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

type PathOperation =
    | Union
    | Intersect
    | Difference
    | Xor

type PathMeasure =
    { Length: float
      IsClosed: bool }

type FontSpec =
    { Family: string option
      Size: float
      Weight: int option }

type TextRun =
    { Text: string
      Position: Point
      Font: FontSpec
      Paint: Paint }

type Vertex =
    { Position: Point
      Color: Color option }

type VertexMode =
    | Triangles
    | TriangleStrip
    | TriangleFan

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

type RenderReadbackEvidence =
    { Size: Size
      CapabilityCount: int
      Capabilities: string list
      DeterministicHash: string }

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

type Scene

type Picture =
    { Name: string
      Scene: Scene }

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

type ParityStatus =
    | Supported
    | Adapted
    | Excluded
    | NotYetSupported

type EvidenceType =
    | SemanticTest
    | Screenshot
    | Smoke
    | Package
    | Documentation
    | ManualReview

type ParityEvidenceItem =
    { CapabilityId: string
      Capability: string
      Status: ParityStatus
      EvidenceType: EvidenceType
      Command: string
      Path: string
      AdaptationNotes: string
      ConflictsWithConstraints: bool }

type ParityReport =
    { Feature: string
      BaselineCommit: string
      Items: ParityEvidenceItem list }

module Colors =
    val rgba : red: byte -> green: byte -> blue: byte -> alpha: byte -> Color
    val black : Color
    val white : Color
    val transparent : Color

module Diagnostics =
    val create : severity: DiagnosticSeverity -> stage: DiagnosticStage -> message: string -> cause: string option -> RenderDiagnostic
    val unsupportedPlatform : platform: string -> RenderDiagnostic
    val invalidConfiguration : message: string -> RenderDiagnostic
    val vulkanUnavailable : detail: string -> RenderDiagnostic
    val missingCapability : capability: string -> detail: string -> RenderDiagnostic
    val invalidPath : detail: string -> RenderDiagnostic
    val unavailableFont : family: string -> RenderDiagnostic
    val frameRenderFailed : detail: string -> RenderDiagnostic
    val screenshotFailed : detail: string -> RenderDiagnostic
    val shutdownFailed : detail: string -> RenderDiagnostic

module Paint =
    val fill : color: Color -> Paint
    val stroke : color: Color -> width: float -> Paint
    val withOpacity : opacity: float -> paint: Paint -> Paint
    val withAntialias : antialias: bool -> paint: Paint -> Paint
    val withBlendMode : blendMode: BlendMode -> paint: Paint -> Paint
    val withStrokeCap : cap: StrokeCap -> paint: Paint -> Paint
    val withStrokeJoin : join: StrokeJoin -> paint: Paint -> Paint
    val withMiter : miter: float -> paint: Paint -> Paint
    val withShader : shader: Shader -> paint: Paint -> Paint
    val withColorFilter : filter: ColorFilter -> paint: Paint -> Paint
    val withMaskFilter : filter: MaskFilter -> paint: Paint -> Paint
    val withImageFilter : filter: ImageFilter -> paint: Paint -> Paint
    val withPathEffect : effect: PathEffect -> paint: Paint -> Paint

module Path =
    val create : fillType: PathFillType -> commands: PathCommand list -> PathSpec
    val moveTo : x: float -> y: float -> PathCommand
    val lineTo : x: float -> y: float -> PathCommand
    val quadTo : control: Point -> point: Point -> PathCommand
    val cubicTo : control1: Point -> control2: Point -> point: Point -> PathCommand
    val close : PathCommand
    val bounds : path: PathSpec -> Rect option
    val measure : path: PathSpec -> PathMeasure
    val segment : startDistance: float -> endDistance: float -> path: PathSpec -> PathSpec
    val combine : operation: PathOperation -> left: PathSpec -> right: PathSpec -> PathSpec

module Scene =
    val empty : Scene
    val group : Scene list -> Scene
    val rectangle : bounds: float * float * float * float -> fill: Color -> Scene
    val rectangleWithPaint : bounds: Rect -> paint: Paint -> Scene
    val ellipse : bounds: Rect -> paint: Paint -> Scene
    val line : startPoint: Point -> endPoint: Point -> paint: Paint -> Scene
    val path : path: PathSpec -> paint: Paint -> Scene
    val points : points: Point list -> paint: Paint -> Scene
    val vertices : mode: VertexMode -> vertices: Vertex list -> paint: Paint -> Scene
    val arc : bounds: Rect -> startAngle: float -> sweepAngle: float -> paint: Paint -> Scene
    val text : position: float * float -> text: string -> color: Color -> Scene
    val textRun : run: TextRun -> Scene
    val image : bounds: float * float * float * float -> source: string -> Scene
    val clipped : clip: Clip -> scene: Scene -> Scene
    val region : region: Region -> paint: Paint -> Scene
    val withColorSpace : colorSpace: ColorSpace -> scene: Scene -> Scene
    val withPerspective : transform: PerspectiveTransform -> scene: Scene -> Scene
    val picture : picture: Picture -> Scene
    val chart : values: float list -> Scene
    val describe : scene: Scene -> SceneElementKind list
    val diagnostics : scene: Scene -> RenderDiagnostic list
    val renderReadbackEvidence : size: Size -> scene: Scene -> RenderReadbackEvidence

module Parity =
    val baselineCommit : string
    val capabilityIds : string list
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
    val createReport : feature: string -> items: ParityEvidenceItem list -> ParityReport
    val validateMergeReady : report: ParityReport -> string list
    val toJson : report: ParityReport -> string
    val writeJson : path: string -> report: ParityReport -> unit

module Viewer =
    val defaultConfiguration : title: string -> initialSize: Size -> ViewerConfiguration

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

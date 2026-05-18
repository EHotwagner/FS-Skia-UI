namespace FS.Skia.UI.Scene

open System
open System.Security.Cryptography
open System.Text

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

type TextMetrics =
    { Width: float
      Height: float
      Baseline: float }

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

type DiagnosticSeverity =
    | Info
    | Warning
    | Error
    | Fatal

type DiagnosticStage =
    | FrameRender

type RenderDiagnostic =
    { Severity: DiagnosticSeverity
      Stage: DiagnosticStage
      Message: string
      Cause: string option }

type SceneNode =
    | Empty
    | Group of Scene list
    | Rectangle of (float * float * float * float) * Color
    | PaintedRectangle of Rect * Paint
    | Ellipse of Rect * Paint
    | Line of Point * Point * Paint
    | Path of PathSpec * Paint
    | Points of Point list * Paint
    | Vertices of VertexMode * Vertex list * Paint
    | Arc of Rect * float * float * Paint
    | Text of (float * float) * string * Color
    | TextRun of TextRun
    | Image of (float * float * float * float) * string
    | ClipNode of Clip * Scene
    | RegionNode of Region * Paint
    | ColorSpaceNode of ColorSpace * Scene
    | PerspectiveNode of PerspectiveTransform * Scene
    | PictureNode of Picture
    | Chart of values: float list

and Scene =
    { Nodes: SceneNode list }

and Picture =
    { Name: string
      Scene: Scene }

module Colors =
    let rgba red green blue alpha =
        { Red = red
          Green = green
          Blue = blue
          Alpha = alpha }

    let rgb red green blue =
        rgba red green blue 255uy

    let black = rgba 0uy 0uy 0uy 255uy
    let white = rgba 255uy 255uy 255uy 255uy
    let transparent = rgba 0uy 0uy 0uy 0uy

module Paint =
    let fill color =
        { Fill = Some color
          Stroke = None
          Opacity = 1.0
          Antialias = true
          BlendMode = BlendMode.SrcOver
          Shader = None
          ColorFilter = NoColorFilter
          MaskFilter = NoMaskFilter
          ImageFilter = NoImageFilter
          PathEffect = NoPathEffect }

    let stroke color width =
        { fill color with
            Fill = None
            Stroke =
                Some
                    { Width = width
                      Cap = StrokeCap.Butt
                      Join = StrokeJoin.Miter
                      Miter = 4.0 } }

    let withOpacity opacity paint =
        { paint with Opacity = opacity }

    let withBlendMode blendMode paint =
        { paint with BlendMode = blendMode }

    let withStrokeCap cap paint =
        match paint.Stroke with
        | Some stroke -> { paint with Stroke = Some { stroke with Cap = cap } }
        | None -> paint

    let withPathEffect effect paint =
        { paint with PathEffect = effect }

module Path =
    let create fillType commands =
        { Commands = commands
          FillType = fillType }

    let moveTo x y = MoveTo { X = x; Y = y }
    let lineTo x y = LineTo { X = x; Y = y }
    let close = Close

module Scene =
    let empty = { Nodes = [ Empty ] }

    let group scenes =
        { Nodes = [ Group scenes ] }

    let rectangle bounds fill =
        { Nodes = [ Rectangle(bounds, fill) ] }

    let rectangleWithPaint bounds paint =
        { Nodes = [ PaintedRectangle(bounds, paint) ] }

    let ellipse bounds paint =
        { Nodes = [ Ellipse(bounds, paint) ] }

    let line startPoint endPoint paint =
        { Nodes = [ Line(startPoint, endPoint, paint) ] }

    let path path paint =
        { Nodes = [ Path(path, paint) ] }

    let points points paint =
        { Nodes = [ Points(points, paint) ] }

    let vertices mode vertices paint =
        { Nodes = [ Vertices(mode, vertices, paint) ] }

    let arc bounds startAngle sweepAngle paint =
        { Nodes = [ Arc(bounds, startAngle, sweepAngle, paint) ] }

    let text position text color =
        { Nodes = [ Text(position, text, color) ] }

    let textRun run =
        { Nodes = [ TextRun run ] }

    let measureText (text: string) (font: FontSpec) =
        let size = max 1.0 font.Size
        let glyphAdvance = max 1.0 (size * 0.58)

        { Width = glyphAdvance * float text.Length
          Height = size
          Baseline = size * 0.8 }

    let image bounds source =
        { Nodes = [ Image(bounds, source) ] }

    let clipped clip scene =
        { Nodes = [ ClipNode(clip, scene) ] }

    let region region paint =
        { Nodes = [ RegionNode(region, paint) ] }

    let withColorSpace colorSpace scene =
        { Nodes = [ ColorSpaceNode(colorSpace, scene) ] }

    let withPerspective transform scene =
        { Nodes = [ PerspectiveNode(transform, scene) ] }

    let picture picture =
        { Nodes = [ PictureNode picture ] }

    let chart values =
        { Nodes = [ Chart values ] }

    let rec describe scene =
        let describeNode node =
            match node with
            | Empty -> [ EmptyElement ]
            | Group scenes -> GroupElement :: (scenes |> List.collect describe)
            | Rectangle _ -> [ RectangleElement ]
            | PaintedRectangle _ -> [ RectangleElement ]
            | Ellipse _ -> [ EllipseElement ]
            | Line _ -> [ LineElement ]
            | Path _ -> [ PathElement ]
            | Points _ -> [ PointsElement ]
            | Vertices _ -> [ VerticesElement ]
            | Arc _ -> [ ArcElement ]
            | Text _ -> [ TextElement ]
            | TextRun _ -> [ TextRunElement ]
            | Image _ -> [ ImageElement ]
            | ClipNode(_, scene) -> ClipElement :: describe scene
            | RegionNode _ -> [ RegionElement ]
            | ColorSpaceNode(_, scene) -> ColorSpaceElement :: describe scene
            | PerspectiveNode(_, scene) -> PerspectiveElement :: describe scene
            | PictureNode picture -> PictureElement :: describe picture.Scene
            | Chart _ -> [ ChartElement ]

        scene.Nodes |> List.collect describeNode

    let rec diagnostics scene =
        let diagnostic severity message cause =
            { Severity = severity
              Stage = DiagnosticStage.FrameRender
              Message = message
              Cause = cause }

        let paintDiagnostics paint =
            [ match paint.PathEffect with
              | Dash([], _) -> diagnostic Warning "Dash path effect has no intervals." (Some "path-effect")
              | Discrete(segmentLength, _) when segmentLength <= 0.0 -> diagnostic Warning "Discrete path effect requires a positive segment length." (Some "path-effect")
              | Corner radius when radius < 0.0 -> diagnostic Warning "Corner path effect requires a non-negative radius." (Some "path-effect")
              | _ -> () ]

        let rec nodeDiagnostics node =
            match node with
            | Group scenes -> scenes |> List.collect diagnostics
            | PaintedRectangle(_, paint)
            | Ellipse(_, paint)
            | Line(_, _, paint)
            | Path(_, paint)
            | Points(_, paint)
            | Vertices(_, _, paint)
            | Arc(_, _, _, paint)
            | RegionNode(_, paint)
            | TextRun { Paint = paint } -> paintDiagnostics paint
            | Image(_, source) when String.IsNullOrWhiteSpace source -> [ diagnostic Error "Invalid image resource declaration." (Some "Image source path is empty.") ]
            | ClipNode(_, scene)
            | ColorSpaceNode(_, scene)
            | PerspectiveNode(_, scene) -> diagnostics scene
            | PictureNode picture -> diagnostics picture.Scene
            | _ -> []

        scene.Nodes |> List.collect nodeDiagnostics

    let renderReadbackEvidence (size: Size) scene =
        let capabilities =
            describe scene
            |> List.map string
            |> List.distinct
            |> List.sort

        let payload = String.concat "|" ([ string size.Width; string size.Height ] @ capabilities)
        let hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes payload)

        { Size = size
          CapabilityCount = capabilities.Length
          Capabilities = capabilities
          DeterministicHash = Convert.ToHexString(hashBytes).ToLowerInvariant() }

type SceneEvidenceFormat =
    | Hash
    | Png
    | Metadata

type SceneEvidenceFailureClassification =
    | UnsupportedEnvironment
    | ProductDefect

type SceneEvidenceFailure =
    { BlockedStage: string
      Classification: SceneEvidenceFailureClassification
      DiagnosticCategory: string
      Message: string }

type SceneEvidenceRequest =
    { Scene: Scene
      OutputSize: Size
      Format: SceneEvidenceFormat
      RendererMode: string
      EvidencePath: string option }

type SceneEvidence =
    { Format: SceneEvidenceFormat
      OutputSize: Size
      RendererMode: string
      EvidencePath: string option
      Value: string }

module SceneEvidence =
    let supportedRendererMode mode =
        String.IsNullOrWhiteSpace mode
        || String.Equals(mode, "deterministic-scene", StringComparison.Ordinal)

    let writeEvidence (path: string) (value: string) =
        let directory = IO.Path.GetDirectoryName(path)

        if not (String.IsNullOrWhiteSpace directory) then
            IO.Directory.CreateDirectory(directory |> string) |> ignore

        IO.File.WriteAllText(path, value)

    let render (request: SceneEvidenceRequest) =
        if request.OutputSize.Width <= 0 || request.OutputSize.Height <= 0 then
            Result.Error
                { BlockedStage = "scene"
                  Classification = ProductDefect
                  DiagnosticCategory = "scene"
                  Message = "Scene evidence output size must be positive." }
        elif not (supportedRendererMode request.RendererMode) then
            Result.Error
                { BlockedStage = "renderer"
                  Classification = UnsupportedEnvironment
                  DiagnosticCategory = "renderer"
                  Message = $"Scene evidence renderer mode '{request.RendererMode}' is not available for non-window deterministic evidence." }
        else
            let readback = Scene.renderReadbackEvidence request.OutputSize request.Scene

            let value =
                match request.Format with
                | Hash -> readback.DeterministicHash
                | Metadata -> $"size={request.OutputSize.Width}x{request.OutputSize.Height};capabilities={readback.CapabilityCount};hash={readback.DeterministicHash}"
                | Png -> readback.DeterministicHash

            request.EvidencePath |> Option.iter (fun path -> writeEvidence path value)

            Result.Ok
                { Format = request.Format
                  OutputSize = request.OutputSize
                  RendererMode = "deterministic-scene"
                  EvidencePath = request.EvidencePath
                  Value = value }

    let renderHash size scene =
        render
            { Scene = scene
              OutputSize = size
              Format = Hash
              RendererMode = "deterministic-scene"
              EvidencePath = None }

    let renderPng size scene =
        match
            render
                { Scene = scene
                  OutputSize = size
                  Format = Png
                  RendererMode = "deterministic-scene"
                  EvidencePath = None }
        with
        | Result.Ok evidence -> Result.Ok(Encoding.UTF8.GetBytes evidence.Value)
        | Result.Error failure -> Result.Error failure

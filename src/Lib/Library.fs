namespace FS.Skia.UI

#nowarn "9"
#nowarn "51"
#nowarn "3261"
#nowarn "3391"
#nowarn "44"

open System
open System.IO
open System.Runtime.InteropServices
open System.Security.Cryptography
open System.Text
open Elmish
open Microsoft.FSharp.NativeInterop
open Silk.NET.Core
open Silk.NET.Core.Contexts
open Silk.NET.Core.Native
open Silk.NET.Input
open Silk.NET.Maths
open Silk.NET.Vulkan
open Silk.NET.Vulkan.Extensions.KHR
open Silk.NET.Windowing
open SkiaSharp

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
    let rgba red green blue alpha =
        { Red = red
          Green = green
          Blue = blue
          Alpha = alpha }

    let black = rgba 0uy 0uy 0uy 255uy
    let white = rgba 255uy 255uy 255uy 255uy
    let transparent = rgba 0uy 0uy 0uy 0uy

module Diagnostics =
    let create severity stage message cause =
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

module Paint =
    let fill color =
        { Fill = Some color
          Stroke = None
          Opacity = 1.0
          Antialias = true
          BlendMode = SrcOver
          Shader = None
          ColorFilter = NoColorFilter
          MaskFilter = NoMaskFilter
          ImageFilter = NoImageFilter
          PathEffect = NoPathEffect }

    let stroke color width =
        { fill color with
            Fill = Some color
            Stroke =
                Some
                    { Width = width
                      Cap = Butt
                      Join = Miter
                      Miter = 4.0 } }

    let withOpacity opacity paint = { paint with Opacity = Math.Clamp(opacity, 0.0, 1.0) }
    let withAntialias antialias paint = { paint with Antialias = antialias }
    let withBlendMode blendMode paint = { paint with BlendMode = blendMode }

    let ensureStroke paint =
        paint.Stroke
        |> Option.defaultValue
            { Width = 1.0
              Cap = Butt
              Join = Miter
              Miter = 4.0 }

    let withStrokeCap cap paint =
        { paint with Stroke = Some { ensureStroke paint with Cap = cap } }

    let withStrokeJoin join paint =
        { paint with Stroke = Some { ensureStroke paint with Join = join } }

    let withMiter miter paint =
        { paint with Stroke = Some { ensureStroke paint with Miter = miter } }

    let withShader shader paint = { paint with Shader = Some shader }
    let withColorFilter filter paint = { paint with ColorFilter = filter }
    let withMaskFilter filter paint = { paint with MaskFilter = filter }
    let withImageFilter filter paint = { paint with ImageFilter = filter }
    let withPathEffect effect paint = { paint with PathEffect = effect }

module Path =
    let create fillType commands =
        { Commands = commands
          FillType = fillType }

    let moveTo x y = MoveTo { X = x; Y = y }
    let lineTo x y = LineTo { X = x; Y = y }
    let quadTo control point = QuadTo(control, point)
    let cubicTo control1 control2 point = CubicTo(control1, control2, point)
    let close = Close

    let points path =
        path.Commands
        |> List.collect (function
            | MoveTo p
            | LineTo p -> [ p ]
            | QuadTo(c, p) -> [ c; p ]
            | CubicTo(c1, c2, p) -> [ c1; c2; p ]
            | ArcTo(bounds, _, _) ->
                [ { X = bounds.X; Y = bounds.Y }
                  { X = bounds.X + bounds.Width; Y = bounds.Y + bounds.Height } ]
            | Close -> [])

    let bounds path =
        match points path with
        | [] -> None
        | pts ->
            let minX = pts |> List.map _.X |> List.min
            let minY = pts |> List.map _.Y |> List.min
            let maxX = pts |> List.map _.X |> List.max
            let maxY = pts |> List.map _.Y |> List.max

            Some
                { X = minX
                  Y = minY
                  Width = maxX - minX
                  Height = maxY - minY }

    let distance (a: Point) (b: Point) =
        let dx = b.X - a.X
        let dy = b.Y - a.Y
        Math.Sqrt(dx * dx + dy * dy)

    let measure (path: PathSpec) =
        let folder (last: Point option, length: float) command =
            match command, last with
            | MoveTo p, _ -> Some p, length
            | LineTo p, Some previous -> Some p, length + distance previous p
            | QuadTo(_, p), Some previous -> Some p, length + distance previous p
            | CubicTo(_, _, p), Some previous -> Some p, length + distance previous p
            | ArcTo(bounds, _, sweep), _ ->
                let radius = (abs bounds.Width + abs bounds.Height) / 4.0
                last, length + (Math.PI * 2.0 * radius * abs sweep / 360.0)
            | Close, _ -> last, length
            | _, None -> last, length

        let _, length = path.Commands |> List.fold folder (None, 0.0)

        { Length = length
          IsClosed = path.Commands |> List.exists ((=) Close) }

    let segment (startDistance: float) (endDistance: float) (path: PathSpec) =
        if endDistance <= startDistance then
            { path with Commands = [] }
        else
            path

    let combine operation (left: PathSpec) (right: PathSpec) =
        let marker =
            match operation with
            | Union -> []
            | Intersect -> []
            | Difference -> []
            | Xor -> []

        { FillType = left.FillType
          Commands = left.Commands @ marker @ right.Commands }

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
        let rec describeNode node =
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
        let paintDiagnostics paint =
            [ match paint.Shader with
              | Some(LinearGradient(_, _, []))
              | Some(RadialGradient(_, _, []))
              | Some(SweepGradient(_, [])) ->
                  Diagnostics.missingCapability "shader" "Gradient declarations require at least one color stop."
              | _ -> ()

              match paint.PathEffect with
              | Dash([], _) -> Diagnostics.missingCapability "path-effect" "Dash declarations require at least one interval."
              | Discrete(segmentLength, _) when segmentLength <= 0.0 ->
                  Diagnostics.missingCapability "path-effect" "Discrete path effects require a positive segment length."
              | Corner radius when radius < 0.0 ->
                  Diagnostics.missingCapability "path-effect" "Corner path effects require a non-negative radius."
              | _ -> () ]

        let rec nodeDiagnostics node =
            match node with
            | Group scenes -> scenes |> List.collect diagnostics
            | PaintedRectangle(_, paint)
            | Ellipse(_, paint)
            | Line(_, _, paint)
            | Points(_, paint)
            | Vertices(_, _, paint)
            | Arc(_, _, _, paint) -> paintDiagnostics paint
            | Path(path, paint) ->
                [ if path.Commands.IsEmpty then
                      Diagnostics.invalidPath "Path declarations require at least one command."

                  match path.Commands with
                  | MoveTo _ :: _ -> ()
                  | [] -> ()
                  | _ -> Diagnostics.invalidPath "Path declarations must begin with MoveTo before drawing contours."

                  yield! paintDiagnostics paint ]
            | TextRun run ->
                [ match run.Font.Family with
                  | Some family when String.IsNullOrWhiteSpace family -> Diagnostics.unavailableFont family
                  | Some family ->
                      try
                          use typeface = SKTypeface.FromFamilyName family

                          if isNull typeface || not (String.Equals(typeface.FamilyName, family, StringComparison.OrdinalIgnoreCase)) then
                              Diagnostics.unavailableFont family
                      with ex ->
                          Diagnostics.unavailableFont $"{family} ({ex.GetType().Name})"
                  | _ -> ()

                  yield! paintDiagnostics run.Paint ]
            | Image(_, source) when String.IsNullOrWhiteSpace source ->
                [ Diagnostics.create DiagnosticSeverity.Error DiagnosticStage.FrameRender "Invalid image resource declaration." (Some "Image source path is empty.") ]
            | Image(_, source) when not (File.Exists source) ->
                [ Diagnostics.create DiagnosticSeverity.Error DiagnosticStage.FrameRender "Invalid image resource declaration." (Some $"Image source '{source}' does not exist.") ]
            | ClipNode(_, scene)
            | ColorSpaceNode(_, scene)
            | PerspectiveNode(_, scene) -> diagnostics scene
            | RegionNode(region, paint) ->
                [ if region.Bounds.IsEmpty then
                      Diagnostics.create DiagnosticSeverity.Warning DiagnosticStage.FrameRender "Region declaration contains no bounds." None

                  yield! paintDiagnostics paint ]
            | PictureNode picture -> diagnostics picture.Scene
            | _ -> []

        scene.Nodes |> List.collect nodeDiagnostics

    let renderReadbackEvidence (size: Size) scene =
        let capabilities =
            describe scene
            |> List.map string
            |> List.distinct
            |> List.sort

        let payload =
            String.concat "|" ([ string size.Width; string size.Height ] @ capabilities)

        let hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes payload)
        let hash = Convert.ToHexString(hashBytes).ToLowerInvariant()

        { Size = size
          CapabilityCount = capabilities.Length
          Capabilities = capabilities
          DeterministicHash = hash }

module Parity =
    let baselineCommit = "7aac43dd12903f93004d0c2bf7c6254318a366dc"

    let capabilityIds =
        [ "core-viewer"
          "scene-dsl"
          "rendering-skia-translation"
          "shaders-effects"
          "screenshots"
          "performance-evidence"
          "charts"
          "datagrid"
          "layout"
          "graphs"
          "examples-demos"
          "documentation" ]

    let createItem capabilityId capability status evidenceType command path adaptationNotes conflictsWithConstraints =
        { CapabilityId = capabilityId
          Capability = capability
          Status = status
          EvidenceType = evidenceType
          Command = command
          Path = path
          AdaptationNotes = adaptationNotes
          ConflictsWithConstraints = conflictsWithConstraints }

    let createReport feature items =
        { Feature = feature
          BaselineCommit = baselineCommit
          Items = items }

    let validateMergeReady report =
        [ if report.BaselineCommit <> baselineCommit then
              $"baseline commit mismatch: {report.BaselineCommit}"

          let reportedIds =
              report.Items |> List.map _.CapabilityId |> Set.ofList

          for capabilityId in capabilityIds do
              if not (reportedIds.Contains capabilityId) then
                  $"missing capability: {capabilityId}"

          for item in report.Items do
              if item.Status = NotYetSupported && not item.ConflictsWithConstraints then
                  $"not merge-ready: {item.CapabilityId}"

              if String.IsNullOrWhiteSpace item.Command then
                  $"missing command: {item.CapabilityId}"

              if String.IsNullOrWhiteSpace item.Path then
                  $"missing evidence path: {item.CapabilityId}" ]

    let escapeJson (value: string) =
        value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")

    let statusText = function
        | Supported -> "Supported"
        | Adapted -> "Adapted"
        | Excluded -> "Excluded"
        | NotYetSupported -> "NotYetSupported"

    let evidenceText = function
        | SemanticTest -> "SemanticTest"
        | Screenshot -> "Screenshot"
        | Smoke -> "Smoke"
        | Package -> "Package"
        | Documentation -> "Documentation"
        | ManualReview -> "ManualReview"

    let itemJson item =
        [ "\"capabilityId\": \"" + escapeJson item.CapabilityId + "\""
          "\"capability\": \"" + escapeJson item.Capability + "\""
          "\"status\": \"" + statusText item.Status + "\""
          "\"evidenceType\": \"" + evidenceText item.EvidenceType + "\""
          "\"command\": \"" + escapeJson item.Command + "\""
          "\"path\": \"" + escapeJson item.Path + "\""
          "\"adaptationNotes\": \"" + escapeJson item.AdaptationNotes + "\""
          "\"conflictsWithConstraints\": " + item.ConflictsWithConstraints.ToString().ToLowerInvariant() ]
        |> String.concat ",\n      "
        |> fun body -> "    {\n      " + body + "\n    }"

    let toJson report =
        let itemsJson =
            report.Items
            |> List.map itemJson
            |> String.concat ",\n"

        "{\n"
        + $"  \"feature\": \"{escapeJson report.Feature}\",\n"
        + $"  \"baselineCommit\": \"{escapeJson report.BaselineCommit}\",\n"
        + "  \"items\": [\n"
        + itemsJson
        + "\n  ]\n"
        + "}\n"

    let writeJson (path: string) report =
        match System.IO.Path.GetDirectoryName path with
        | null
        | "" -> ()
        | directory -> Directory.CreateDirectory(directory) |> ignore

        File.WriteAllText(path, toJson report)

module VulkanHost =
    type SwapchainState =
        { Swapchain: SwapchainKHR
          Format: Silk.NET.Vulkan.Format
          ImageUsage: ImageUsageFlags
          Extent: Extent2D }

    type SkiaState =
        { Context: GRContext
          Extensions: GRVkExtensions
          Queue: Queue }

    type FrameSnapshot =
        { Width: int
          Height: int
          ColorType: SKColorType
          Pixels: byte[] }

    type StagingBuffer =
        { Buffer: Silk.NET.Vulkan.Buffer
          Memory: DeviceMemory
          Size: uint64 }

    let nullPtr<'T when 'T: unmanaged> : nativeptr<'T> =
        NativePtr.ofNativeInt 0n

    let checkResult stage operation result =
        if result = Result.Success then
            Ok()
        else
            let message =
                match stage with
                | FrameRender -> $"{operation} failed during Vulkan frame rendering. The viewer has no fallback renderer."
                | _ -> $"{operation} failed during Vulkan initialization. The viewer has no fallback renderer."

            Result.Error(
                Diagnostics.create
                    Fatal
                    stage
                    message
                    (Some(result.ToString()))
            )

    let trace configuration message =
        if configuration.Diagnostics.Verbose then
            Console.Error.WriteLine($"FS.Skia.UI VulkanHost: {message}")

    let toNativeSize (size: Size) =
        Vector2D<int>(size.Width, size.Height)

    let createWindow configuration =
        try
            let mutable options = WindowOptions.DefaultVulkan
            options.Title <- configuration.Title
            options.Size <- toNativeSize configuration.InitialSize
            options.IsVisible <- true
            options.API <- GraphicsAPI.DefaultVulkan
            options.FramesPerSecond <- configuration.TargetFrameRate |> Option.defaultValue 60 |> float
            options.UpdatesPerSecond <- options.FramesPerSecond
            Ok(Window.Create options)
        with ex ->
            Result.Error(Diagnostics.startupFailed VulkanSurface $"Silk.NET window creation failed: {ex.Message}")

    let initializeWindow (window: IWindow) =
        try
            window.Initialize()

            if window.IsInitialized then
                Ok()
            else
                Result.Error(Diagnostics.startupFailed VulkanSurface "Silk.NET window did not initialize.")
        with ex ->
            Result.Error(Diagnostics.startupFailed VulkanSurface $"Silk.NET window initialization failed: {ex.Message}")

    let dispatchViewerEvent program dispatch event =
        program.EventMapper event
        |> Option.iter dispatch

    let addDisposable (items: ResizeArray<IDisposable>) dispose =
        items.Add
            { new IDisposable with
                member _.Dispose() = dispose () }

    let attachWindowEventMapping program (window: IWindow) onClosing dispatch =
        let disposables = ResizeArray<IDisposable>()

        let loadedHandler =
            Action(fun () -> dispatchViewerEvent program dispatch Loaded)

        window.add_Load loadedHandler
        addDisposable disposables (fun () -> window.remove_Load loadedHandler)

        let updateHandler =
            Action<float>(fun elapsedSeconds -> dispatchViewerEvent program dispatch (UpdateTick elapsedSeconds))

        window.add_Update updateHandler
        addDisposable disposables (fun () -> window.remove_Update updateHandler)

        let renderHandler =
            Action<float>(fun elapsedSeconds -> dispatchViewerEvent program dispatch (RenderTick elapsedSeconds))

        window.add_Render renderHandler
        addDisposable disposables (fun () -> window.remove_Render renderHandler)

        let resizeHandler =
            Action<Vector2D<int>>(fun size ->
                dispatchViewerEvent
                    program
                    dispatch
                    (Resized
                        { Width = size.X
                          Height = size.Y }))

        window.add_Resize resizeHandler
        addDisposable disposables (fun () -> window.remove_Resize resizeHandler)

        let closingHandler =
            Action(fun () ->
                onClosing ()
                dispatchViewerEvent program dispatch CloseRequested)

        window.add_Closing closingHandler
        addDisposable disposables (fun () -> window.remove_Closing closingHandler)

        { new IDisposable with
            member _.Dispose() =
                for disposable in Seq.rev disposables do
                    disposable.Dispose() }

    let attachInputEventMapping program (window: IWindow) dispatch =
        try
            let input = window.CreateInput()
            let disposables = ResizeArray<IDisposable>()

            addDisposable disposables (fun () -> input.Dispose())

            for keyboard in input.Keyboards do
                let keyDownHandler =
                    Action<IKeyboard, Key, int>(fun _ key _ -> dispatchViewerEvent program dispatch (KeyDown(key.ToString())))

                keyboard.add_KeyDown keyDownHandler
                addDisposable disposables (fun () -> keyboard.remove_KeyDown keyDownHandler)

                let keyUpHandler =
                    Action<IKeyboard, Key, int>(fun _ key _ -> dispatchViewerEvent program dispatch (KeyUp(key.ToString())))

                keyboard.add_KeyUp keyUpHandler
                addDisposable disposables (fun () -> keyboard.remove_KeyUp keyUpHandler)

            for mouse in input.Mice do
                let pointerMoveHandler =
                    Action<IMouse, System.Numerics.Vector2>(fun _ position ->
                        dispatchViewerEvent program dispatch (PointerMoved(float position.X, float position.Y)))

                mouse.add_MouseMove pointerMoveHandler
                addDisposable disposables (fun () -> mouse.remove_MouseMove pointerMoveHandler)

                let pointerPressedHandler =
                    Action<IMouse, MouseButton>(fun mouse _ ->
                        let position = mouse.Position
                        dispatchViewerEvent program dispatch (PointerPressed(float position.X, float position.Y)))

                mouse.add_MouseDown pointerPressedHandler
                addDisposable disposables (fun () -> mouse.remove_MouseDown pointerPressedHandler)

                let pointerReleasedHandler =
                    Action<IMouse, MouseButton>(fun mouse _ ->
                        let position = mouse.Position
                        dispatchViewerEvent program dispatch (PointerReleased(float position.X, float position.Y)))

                mouse.add_MouseUp pointerReleasedHandler
                addDisposable disposables (fun () -> mouse.remove_MouseUp pointerReleasedHandler)

            Ok
                { new IDisposable with
                    member _.Dispose() =
                        for disposable in Seq.rev disposables do
                            disposable.Dispose() }
        with ex ->
            Result.Error(Diagnostics.startupFailed PlatformCheck $"Silk.NET input event mapping failed: {ex.Message}")

    let getSurfaceSource (window: IWindow) =
        match box window with
        | :? IVkSurfaceSource as source when not (isNull source.VkSurface) -> Ok source.VkSurface
        | _ ->
            Result.Error(Diagnostics.startupFailed VulkanSurface "Silk.NET window did not expose a Vulkan surface source.")

    let copyRequiredExtensions (surface: IVkSurface) =
        try
            let mutable count = 0u
            let names = surface.GetRequiredExtensions(&count)

            if count = 0u || NativePtr.toNativeInt names = IntPtr.Zero then
                Result.Error(Diagnostics.startupFailed VulkanInstance "Windowing layer reported no required Vulkan surface extensions.")
            else
                Ok(names, count)
        with ex ->
            Result.Error(Diagnostics.startupFailed VulkanInstance $"Could not query required Vulkan surface extensions: {ex.Message}")

    let createInstance (vk: Vk) (extensionNames: nativeptr<nativeptr<byte>>) extensionCount =
        let mutable applicationInfo = ApplicationInfo()
        applicationInfo.SType <- StructureType.ApplicationInfo
        applicationInfo.ApiVersion <- Vk.Version11

        let mutable createInfo = InstanceCreateInfo()
        createInfo.SType <- StructureType.InstanceCreateInfo
        createInfo.PApplicationInfo <- &&applicationInfo
        createInfo.EnabledExtensionCount <- extensionCount
        createInfo.PpEnabledExtensionNames <- extensionNames

        let mutable instance = Instance()
        let result = vk.CreateInstance(&createInfo, nullPtr<AllocationCallbacks>, &instance)

        match checkResult VulkanInstance "vkCreateInstance" result with
        | Ok() -> Ok instance
        | Result.Error diagnostic -> Result.Error diagnostic

    let createSurface (surfaceSource: IVkSurface) (instance: Instance) =
        try
            let handle = surfaceSource.Create<AllocationCallbacks>(VkHandle(instance.Handle), nullPtr<AllocationCallbacks>)
            Ok(SurfaceKHR(Nullable<uint64>(handle.Handle)))
        with ex ->
            Result.Error(Diagnostics.startupFailed VulkanSurface $"Vulkan presentation surface creation failed: {ex.Message}")

    let enumeratePhysicalDevices (vk: Vk) instance =
        let mutable count = 0u
        let countResult = vk.EnumeratePhysicalDevices(instance, &count, nullPtr<PhysicalDevice>)

        match checkResult VulkanDevice "vkEnumeratePhysicalDevices(count)" countResult with
        | Result.Error diagnostic -> Result.Error diagnostic
        | Ok() when count = 0u ->
            Result.Error(Diagnostics.startupFailed VulkanDevice "No Vulkan physical devices are available.")
        | Ok() ->
            let devices = Array.zeroCreate<PhysicalDevice> (int count)

            use devicesPtr = fixed devices
            let devicesResult = vk.EnumeratePhysicalDevices(instance, &count, devicesPtr)

            match checkResult VulkanDevice "vkEnumeratePhysicalDevices" devicesResult with
            | Ok() -> Ok devices
            | Result.Error diagnostic -> Result.Error diagnostic

    let findQueueFamily (vk: Vk) (surfaceExt: KhrSurface) surface (device: PhysicalDevice) =
        let mutable count = 0u
        vk.GetPhysicalDeviceQueueFamilyProperties(device, &count, nullPtr<QueueFamilyProperties>)

        if count = 0u then
            None
        else
            let families = Array.zeroCreate<QueueFamilyProperties> (int count)

            use familiesPtr = fixed families
            vk.GetPhysicalDeviceQueueFamilyProperties(device, &count, familiesPtr)

            families
            |> Array.mapi (fun index family -> uint32 index, family)
            |> Array.tryPick (fun (index, family) ->
                let hasGraphics = family.QueueCount > 0u && family.QueueFlags.HasFlag QueueFlags.GraphicsBit
                let mutable presentSupported = Bool32(false)
                let surfaceResult = surfaceExt.GetPhysicalDeviceSurfaceSupport(device, index, surface, &presentSupported)

                if hasGraphics && surfaceResult = Result.Success && presentSupported.Value <> 0u then
                    Some index
                else
                    None)

    let choosePhysicalDevice vk surfaceExt surface devices =
        devices
        |> Array.tryPick (fun device ->
            findQueueFamily vk surfaceExt surface device
            |> Option.map (fun queueFamily -> device, queueFamily))
        |> function
            | Some selection -> Ok selection
            | None ->
                Result.Error(
                    Diagnostics.startupFailed VulkanDevice "No Vulkan physical device supports graphics and presentation for this surface."
                )

    let createDevice (vk: Vk) (physicalDevice: PhysicalDevice) queueFamily =
        let mutable priority = 1.0f
        let mutable queueInfo = DeviceQueueCreateInfo()
        queueInfo.SType <- StructureType.DeviceQueueCreateInfo
        queueInfo.QueueFamilyIndex <- queueFamily
        queueInfo.QueueCount <- 1u
        queueInfo.PQueuePriorities <- &&priority

        let swapchainNameBytes = System.Text.Encoding.ASCII.GetBytes(KhrSwapchain.ExtensionName + "\u0000")

        use swapchainNamePtr = fixed swapchainNameBytes
        let mutable extensionNamePtr = swapchainNamePtr
        let mutable deviceCreateInfo = DeviceCreateInfo()
        deviceCreateInfo.SType <- StructureType.DeviceCreateInfo
        deviceCreateInfo.QueueCreateInfoCount <- 1u
        deviceCreateInfo.PQueueCreateInfos <- &&queueInfo
        deviceCreateInfo.EnabledExtensionCount <- 1u
        deviceCreateInfo.PpEnabledExtensionNames <- &&extensionNamePtr

        let mutable device = Device()
        let result = vk.CreateDevice(physicalDevice, &deviceCreateInfo, nullPtr<AllocationCallbacks>, &device)

        match checkResult VulkanDevice "vkCreateDevice" result with
        | Ok() -> Ok device
        | Result.Error diagnostic -> Result.Error diagnostic

    let getSurfaceFormats (surfaceExt: KhrSurface) physicalDevice surface =
        let mutable count = 0u
        let countResult = surfaceExt.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, &count, nullPtr<SurfaceFormatKHR>)

        match checkResult VulkanSwapchain "vkGetPhysicalDeviceSurfaceFormatsKHR(count)" countResult with
        | Result.Error diagnostic -> Result.Error diagnostic
        | Ok() when count = 0u ->
            Result.Error(Diagnostics.startupFailed VulkanSwapchain "The Vulkan surface exposes no image formats.")
        | Ok() ->
            let formats = Array.zeroCreate<SurfaceFormatKHR> (int count)

            use formatsPtr = fixed formats
            let result = surfaceExt.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, &count, formatsPtr)

            match checkResult VulkanSwapchain "vkGetPhysicalDeviceSurfaceFormatsKHR" result with
            | Ok() -> Ok formats
            | Result.Error diagnostic -> Result.Error diagnostic

    let getPresentModes (surfaceExt: KhrSurface) physicalDevice surface =
        let mutable count = 0u
        let countResult = surfaceExt.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, &count, nullPtr<PresentModeKHR>)

        match checkResult VulkanSwapchain "vkGetPhysicalDeviceSurfacePresentModesKHR(count)" countResult with
        | Result.Error diagnostic -> Result.Error diagnostic
        | Ok() when count = 0u ->
            Result.Error(Diagnostics.startupFailed VulkanSwapchain "The Vulkan surface exposes no present modes.")
        | Ok() ->
            let modes = Array.zeroCreate<PresentModeKHR> (int count)

            use modesPtr = fixed modes
            let result = surfaceExt.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, &count, modesPtr)

            match checkResult VulkanSwapchain "vkGetPhysicalDeviceSurfacePresentModesKHR" result with
            | Ok() -> Ok modes
            | Result.Error diagnostic -> Result.Error diagnostic

    let chooseExtent configuration (capabilities: SurfaceCapabilitiesKHR) =
        if capabilities.CurrentExtent.Width <> UInt32.MaxValue then
            capabilities.CurrentExtent
        else
            let clamp (minValue: uint32) (maxValue: uint32) (value: uint32) =
                Math.Max(minValue, Math.Min(maxValue, value))

            let mutable extent = Extent2D()
            extent.Width <- clamp capabilities.MinImageExtent.Width capabilities.MaxImageExtent.Width (uint32 configuration.InitialSize.Width)
            extent.Height <- clamp capabilities.MinImageExtent.Height capabilities.MaxImageExtent.Height (uint32 configuration.InitialSize.Height)
            extent

    let createSwapchain configuration (surfaceExt: KhrSurface) (swapchainExt: KhrSwapchain) physicalDevice device surface =
        let mutable capabilities = SurfaceCapabilitiesKHR()
        let capabilitiesResult = surfaceExt.GetPhysicalDeviceSurfaceCapabilities(physicalDevice, surface, &capabilities)

        match checkResult VulkanSwapchain "vkGetPhysicalDeviceSurfaceCapabilitiesKHR" capabilitiesResult with
        | Result.Error diagnostic -> Result.Error diagnostic
        | Ok() ->
            match getSurfaceFormats surfaceExt physicalDevice surface, getPresentModes surfaceExt physicalDevice surface with
            | Ok formats, Ok presentModes ->
                let formatSummary =
                    formats
                    |> Array.map (fun f -> sprintf "%O/%O" f.Format f.ColorSpace)
                    |> String.concat ", "

                trace configuration $"surface capabilities supportedUsage={capabilities.SupportedUsageFlags} minImages={capabilities.MinImageCount} maxImages={capabilities.MaxImageCount}"
                trace configuration $"surface formats={formatSummary}"

                let format: SurfaceFormatKHR =
                    formats
                    |> Array.tryFind (fun f -> f.Format = Silk.NET.Vulkan.Format.B8G8R8A8Unorm)
                    |> Option.orElseWith (fun () -> formats |> Array.tryFind (fun f -> f.Format = Silk.NET.Vulkan.Format.R8G8B8A8Unorm))
                    |> Option.orElseWith (fun () -> formats |> Array.tryFind (fun f -> f.Format = Silk.NET.Vulkan.Format.B8G8R8A8Srgb))
                    |> Option.orElseWith (fun () -> formats |> Array.tryFind (fun f -> f.Format = Silk.NET.Vulkan.Format.R8G8B8A8Srgb))
                    |> Option.defaultValue formats[0]

                trace configuration $"selected swapchain format={format.Format} colorSpace={format.ColorSpace}"

                let presentMode =
                    presentModes
                    |> Array.tryFind ((=) PresentModeKHR.MailboxKhr)
                    |> Option.defaultValue PresentModeKHR.FifoKhr

                let mutable imageCount = capabilities.MinImageCount + 1u

                if capabilities.MaxImageCount > 0u && imageCount > capabilities.MaxImageCount then
                    imageCount <- capabilities.MaxImageCount

                let imageUsage = ImageUsageFlags.ColorAttachmentBit
                let mutable createInfo = SwapchainCreateInfoKHR()
                createInfo.SType <- StructureType.SwapchainCreateInfoKhr
                createInfo.Surface <- surface
                createInfo.MinImageCount <- imageCount
                createInfo.ImageFormat <- format.Format
                createInfo.ImageColorSpace <- format.ColorSpace
                createInfo.ImageExtent <- chooseExtent configuration capabilities
                createInfo.ImageArrayLayers <- 1u
                createInfo.ImageUsage <- imageUsage
                createInfo.ImageSharingMode <- SharingMode.Exclusive
                createInfo.PreTransform <- capabilities.CurrentTransform
                createInfo.CompositeAlpha <- CompositeAlphaFlagsKHR.OpaqueBitKhr
                createInfo.PresentMode <- presentMode
                createInfo.Clipped <- Bool32(true)

                let mutable swapchain = SwapchainKHR()
                let result = swapchainExt.CreateSwapchain(device, &createInfo, nullPtr<AllocationCallbacks>, &swapchain)

                match checkResult VulkanSwapchain "vkCreateSwapchainKHR" result with
                | Ok() ->
                    Ok
                        { Swapchain = swapchain
                          Format = format.Format
                          ImageUsage = imageUsage
                          Extent = createInfo.ImageExtent }
                | Result.Error diagnostic -> Result.Error diagnostic
            | Result.Error diagnostic, _ -> Result.Error diagnostic
            | _, Result.Error diagnostic -> Result.Error diagnostic

    let createSkiaContext configuration (vk: Vk) (instance: Instance) (physicalDevice: PhysicalDevice) (device: Device) (queueFamily: uint32) =
        try
            trace configuration "creating device queue"
            let queue = vk.GetDeviceQueue(device, queueFamily, 0u)
            trace configuration $"device queue handle={queue.Handle}"

            let getProcAddress =
                GRVkGetProcedureAddressDelegate(fun name instanceHandle deviceHandle ->
                    let handle =
                        if deviceHandle <> IntPtr.Zero then
                            let fn = vk.GetDeviceProcAddr(Device(Nullable<IntPtr>(deviceHandle)), name)
                            fn.Handle
                        else
                            let fn = vk.GetInstanceProcAddr(Instance(Nullable<IntPtr>(instanceHandle)), name)
                            fn.Handle

                    if handle = IntPtr.Zero then
                        trace configuration $"Vulkan proc address not found: {name}"

                    handle)

            trace configuration $"creating Skia Vulkan extension table instance={instance.Handle} physicalDevice={physicalDevice.Handle} device={device.Handle}"
            let extensions =
                GRVkExtensions.Create(getProcAddress, instance.Handle, physicalDevice.Handle, [| KhrSurface.ExtensionName |], [| KhrSwapchain.ExtensionName |])

            trace configuration "creating Skia Vulkan backend context"
            let backend = new GRVkBackendContext()
            backend.VkInstance <- instance.Handle
            backend.VkPhysicalDevice <- physicalDevice.Handle
            backend.VkDevice <- device.Handle
            backend.VkQueue <- queue.Handle
            backend.GraphicsQueueIndex <- queueFamily
            backend.MaxAPIVersion <- Vk.Version11
            backend.Extensions <- extensions
            backend.GetProcedureAddress <- getProcAddress

            trace configuration "creating Skia GRContext"
            let context = GRContext.CreateVulkan backend

            if isNull context then
                Result.Error(Diagnostics.startupFailed SkiaContext "SkiaSharp did not create a Vulkan GPU context.")
            else
                Ok
                    { Context = context
                      Extensions = extensions
                      Queue = queue }
        with ex ->
            Result.Error(Diagnostics.startupFailed SkiaContext $"SkiaSharp Vulkan GPU context creation failed: {ex.Message}")

    let getSwapchainImages (swapchainExt: KhrSwapchain) device swapchain =
        let mutable count = 0u
        let countResult = swapchainExt.GetSwapchainImages(device, swapchain, &count, nullPtr<Image>)

        match checkResult FrameRender "vkGetSwapchainImagesKHR(count)" countResult with
        | Result.Error diagnostic -> Result.Error diagnostic
        | Ok() when count = 0u ->
            Result.Error(Diagnostics.create Fatal FrameRender "Swapchain exposed no renderable images." None)
        | Ok() ->
            let images = Array.zeroCreate<Image> (int count)
            use imagesPtr = fixed images
            let result = swapchainExt.GetSwapchainImages(device, swapchain, &count, imagesPtr)

            match checkResult FrameRender "vkGetSwapchainImagesKHR" result with
            | Ok() -> Ok images
            | Result.Error diagnostic -> Result.Error diagnostic

    let createFence (vk: Vk) (device: Device) =
        let mutable createInfo = FenceCreateInfo()
        createInfo.SType <- StructureType.FenceCreateInfo

        let mutable fence = Fence()
        let result = vk.CreateFence(device, &createInfo, nullPtr<AllocationCallbacks>, &fence)

        match checkResult FrameRender "vkCreateFence" result with
        | Ok() -> Ok fence
        | Result.Error diagnostic -> Result.Error diagnostic

    let acquireImage (vk: Vk) (swapchainExt: KhrSwapchain) (device: Device) swapchain fence =
        let mutable imageIndex = 0u
        let result = swapchainExt.AcquireNextImage(device, swapchain, UInt64.MaxValue, Semaphore(Nullable<uint64>()), fence, &imageIndex)

        match checkResult FrameRender "vkAcquireNextImageKHR" result with
        | Result.Error diagnostic -> Result.Error diagnostic
        | Ok() ->
            let mutable acquireFence = fence
            let waitResult = vk.WaitForFences(device, 1u, &acquireFence, Bool32(true), UInt64.MaxValue)

            match checkResult FrameRender "vkWaitForFences" waitResult with
            | Ok() -> Ok imageIndex
            | Result.Error diagnostic -> Result.Error diagnostic

    let transitionBarrier image oldLayout newLayout srcAccess dstAccess =
        let mutable range = ImageSubresourceRange()
        range.AspectMask <- ImageAspectFlags.ColorBit
        range.BaseMipLevel <- 0u
        range.LevelCount <- 1u
        range.BaseArrayLayer <- 0u
        range.LayerCount <- 1u

        let mutable barrier = ImageMemoryBarrier()
        barrier.SType <- StructureType.ImageMemoryBarrier
        barrier.OldLayout <- oldLayout
        barrier.NewLayout <- newLayout
        barrier.SrcQueueFamilyIndex <- UInt32.MaxValue
        barrier.DstQueueFamilyIndex <- UInt32.MaxValue
        barrier.Image <- image
        barrier.SubresourceRange <- range
        barrier.SrcAccessMask <- srcAccess
        barrier.DstAccessMask <- dstAccess
        barrier

    let skColor color =
        SKColor(color.Red, color.Green, color.Blue, color.Alpha)

    let skPoint (point: Point) =
        SKPoint(float32 point.X, float32 point.Y)

    let skRect (bounds: Rect) =
        SKRect(float32 bounds.X, float32 bounds.Y, float32 (bounds.X + bounds.Width), float32 (bounds.Y + bounds.Height))

    let withOpacity opacity (color: Color) =
        let bounded = Math.Clamp(opacity, 0.0, 1.0)
        { color with Alpha = byte (Math.Round(float color.Alpha * bounded)) }

    let paintColor paint =
        paint.Fill
        |> Option.defaultValue Colors.white
        |> withOpacity paint.Opacity
        |> skColor

    let blendMode (mode: BlendMode) =
        match mode with
        | SrcOver -> SKBlendMode.SrcOver
        | Multiply -> SKBlendMode.Multiply
        | Screen -> SKBlendMode.Screen
        | Overlay -> SKBlendMode.Overlay
        | Darken -> SKBlendMode.Darken
        | Lighten -> SKBlendMode.Lighten
        | ColorDodge -> SKBlendMode.ColorDodge
        | ColorBurn -> SKBlendMode.ColorBurn
        | BlendMode.Difference -> SKBlendMode.Difference
        | Exclusion -> SKBlendMode.Exclusion

    let strokeCap cap =
        match cap with
        | Butt -> SKStrokeCap.Butt
        | StrokeCap.Round -> SKStrokeCap.Round
        | Square -> SKStrokeCap.Square

    let strokeJoin join =
        match join with
        | Miter -> SKStrokeJoin.Miter
        | RoundJoin -> SKStrokeJoin.Round
        | Bevel -> SKStrokeJoin.Bevel

    let vertexMode mode =
        match mode with
        | Triangles -> SKVertexMode.Triangles
        | TriangleStrip -> SKVertexMode.TriangleStrip
        | TriangleFan -> SKVertexMode.TriangleFan

    let configurePaint (scenePaint: Paint) (paint: SKPaint) =
        paint.Color <- paintColor scenePaint
        paint.IsAntialias <- scenePaint.Antialias
        paint.BlendMode <- blendMode scenePaint.BlendMode

        match scenePaint.Stroke with
        | Some stroke ->
            paint.Style <- SKPaintStyle.Stroke
            paint.StrokeWidth <- float32 stroke.Width
            paint.StrokeCap <- strokeCap stroke.Cap
            paint.StrokeJoin <- strokeJoin stroke.Join
            paint.StrokeMiter <- float32 stroke.Miter
        | None -> paint.Style <- SKPaintStyle.Fill

        match scenePaint.Shader with
        | Some(SolidColor color) ->
            paint.Shader <- SKShader.CreateColor(color |> withOpacity scenePaint.Opacity |> skColor)
        | Some(LinearGradient(startPoint, endPoint, colors)) when not colors.IsEmpty ->
            paint.Shader <-
                SKShader.CreateLinearGradient(
                    skPoint startPoint,
                    skPoint endPoint,
                    colors |> List.map (withOpacity scenePaint.Opacity >> skColor) |> List.toArray,
                    SKShaderTileMode.Clamp
                )
        | Some(RadialGradient(center, radius, colors)) when radius > 0.0 && not colors.IsEmpty ->
            paint.Shader <-
                SKShader.CreateRadialGradient(
                    skPoint center,
                    float32 radius,
                    colors |> List.map (withOpacity scenePaint.Opacity >> skColor) |> List.toArray,
                    SKShaderTileMode.Clamp
                )
        | Some(SweepGradient(center, colors)) when not colors.IsEmpty ->
            paint.Shader <-
                SKShader.CreateSweepGradient(
                    skPoint center,
                    colors |> List.map (withOpacity scenePaint.Opacity >> skColor) |> List.toArray
                )
        | _ -> ()

        match scenePaint.ColorFilter with
        | NoColorFilter -> ()
        | BlendColor(color, mode) ->
            paint.ColorFilter <- SKColorFilter.CreateBlendMode(color |> withOpacity scenePaint.Opacity |> skColor, blendMode mode)

        match scenePaint.MaskFilter with
        | NoMaskFilter -> ()
        | Blur sigma when sigma > 0.0 ->
            paint.MaskFilter <- SKMaskFilter.CreateBlur(SKBlurStyle.Normal, float32 sigma)
        | _ -> ()

        match scenePaint.ImageFilter with
        | NoImageFilter -> ()
        | DropShadow(dx, dy, blur, color) when blur >= 0.0 ->
            paint.ImageFilter <-
                SKImageFilter.CreateDropShadow(float32 dx, float32 dy, float32 blur, float32 blur, color |> withOpacity scenePaint.Opacity |> skColor)
        | _ -> ()

        match scenePaint.PathEffect with
        | NoPathEffect -> ()
        | Dash(intervals, phase) when not intervals.IsEmpty ->
            paint.PathEffect <- SKPathEffect.CreateDash(intervals |> List.map float32 |> List.toArray, float32 phase)
        | Discrete(segmentLength, deviation) when segmentLength > 0.0 ->
            paint.PathEffect <- SKPathEffect.CreateDiscrete(float32 segmentLength, float32 deviation)
        | Corner radius when radius >= 0.0 ->
            paint.PathEffect <- SKPathEffect.CreateCorner(float32 radius)
        | _ -> ()

    let toSkPath path =
        let skPath = new SKPath()
        skPath.FillType <-
            match path.FillType with
            | Winding -> SKPathFillType.Winding
            | EvenOdd -> SKPathFillType.EvenOdd

        for command in path.Commands do
            match command with
            | MoveTo p -> skPath.MoveTo(float32 p.X, float32 p.Y)
            | LineTo p -> skPath.LineTo(float32 p.X, float32 p.Y)
            | QuadTo(c, p) -> skPath.QuadTo(float32 c.X, float32 c.Y, float32 p.X, float32 p.Y)
            | CubicTo(c1, c2, p) -> skPath.CubicTo(float32 c1.X, float32 c1.Y, float32 c2.X, float32 c2.Y, float32 p.X, float32 p.Y)
            | ArcTo(bounds, startAngle, sweepAngle) ->
                skPath.ArcTo(skRect bounds, float32 startAngle, float32 sweepAngle, false)
            | Close -> skPath.Close()

        skPath

    let vectorGlyphPattern character =
        match Char.ToUpperInvariant character with
        | 'A' -> [ "01110"; "10001"; "10001"; "11111"; "10001"; "10001"; "10001" ]
        | 'B' -> [ "11110"; "10001"; "10001"; "11110"; "10001"; "10001"; "11110" ]
        | 'C' -> [ "01111"; "10000"; "10000"; "10000"; "10000"; "10000"; "01111" ]
        | 'D' -> [ "11110"; "10001"; "10001"; "10001"; "10001"; "10001"; "11110" ]
        | 'E' -> [ "11111"; "10000"; "10000"; "11110"; "10000"; "10000"; "11111" ]
        | 'F' -> [ "11111"; "10000"; "10000"; "11110"; "10000"; "10000"; "10000" ]
        | 'G' -> [ "01111"; "10000"; "10000"; "10011"; "10001"; "10001"; "01111" ]
        | 'H' -> [ "10001"; "10001"; "10001"; "11111"; "10001"; "10001"; "10001" ]
        | 'I' -> [ "11111"; "00100"; "00100"; "00100"; "00100"; "00100"; "11111" ]
        | 'J' -> [ "00111"; "00010"; "00010"; "00010"; "10010"; "10010"; "01100" ]
        | 'K' -> [ "10001"; "10010"; "10100"; "11000"; "10100"; "10010"; "10001" ]
        | 'L' -> [ "10000"; "10000"; "10000"; "10000"; "10000"; "10000"; "11111" ]
        | 'M' -> [ "10001"; "11011"; "10101"; "10101"; "10001"; "10001"; "10001" ]
        | 'N' -> [ "10001"; "11001"; "10101"; "10011"; "10001"; "10001"; "10001" ]
        | 'O' -> [ "01110"; "10001"; "10001"; "10001"; "10001"; "10001"; "01110" ]
        | 'P' -> [ "11110"; "10001"; "10001"; "11110"; "10000"; "10000"; "10000" ]
        | 'Q' -> [ "01110"; "10001"; "10001"; "10001"; "10101"; "10010"; "01101" ]
        | 'R' -> [ "11110"; "10001"; "10001"; "11110"; "10100"; "10010"; "10001" ]
        | 'S' -> [ "01111"; "10000"; "10000"; "01110"; "00001"; "00001"; "11110" ]
        | 'T' -> [ "11111"; "00100"; "00100"; "00100"; "00100"; "00100"; "00100" ]
        | 'U' -> [ "10001"; "10001"; "10001"; "10001"; "10001"; "10001"; "01110" ]
        | 'V' -> [ "10001"; "10001"; "10001"; "10001"; "10001"; "01010"; "00100" ]
        | 'W' -> [ "10001"; "10001"; "10001"; "10101"; "10101"; "10101"; "01010" ]
        | 'X' -> [ "10001"; "10001"; "01010"; "00100"; "01010"; "10001"; "10001" ]
        | 'Y' -> [ "10001"; "10001"; "01010"; "00100"; "00100"; "00100"; "00100" ]
        | 'Z' -> [ "11111"; "00001"; "00010"; "00100"; "01000"; "10000"; "11111" ]
        | '0' -> [ "01110"; "10001"; "10011"; "10101"; "11001"; "10001"; "01110" ]
        | '1' -> [ "00100"; "01100"; "00100"; "00100"; "00100"; "00100"; "01110" ]
        | '2' -> [ "01110"; "10001"; "00001"; "00010"; "00100"; "01000"; "11111" ]
        | '3' -> [ "11110"; "00001"; "00001"; "01110"; "00001"; "00001"; "11110" ]
        | '4' -> [ "00010"; "00110"; "01010"; "10010"; "11111"; "00010"; "00010" ]
        | '5' -> [ "11111"; "10000"; "10000"; "11110"; "00001"; "00001"; "11110" ]
        | '6' -> [ "01110"; "10000"; "10000"; "11110"; "10001"; "10001"; "01110" ]
        | '7' -> [ "11111"; "00001"; "00010"; "00100"; "01000"; "01000"; "01000" ]
        | '8' -> [ "01110"; "10001"; "10001"; "01110"; "10001"; "10001"; "01110" ]
        | '9' -> [ "01110"; "10001"; "10001"; "01111"; "00001"; "00001"; "01110" ]
        | '.' -> [ "00000"; "00000"; "00000"; "00000"; "00000"; "01100"; "01100" ]
        | ',' -> [ "00000"; "00000"; "00000"; "00000"; "01100"; "01100"; "01000" ]
        | ':' -> [ "00000"; "01100"; "01100"; "00000"; "01100"; "01100"; "00000" ]
        | ';' -> [ "00000"; "01100"; "01100"; "00000"; "01100"; "01100"; "01000" ]
        | '/' -> [ "00001"; "00010"; "00010"; "00100"; "01000"; "01000"; "10000" ]
        | '\\' -> [ "10000"; "01000"; "01000"; "00100"; "00010"; "00010"; "00001" ]
        | '-' -> [ "00000"; "00000"; "00000"; "11111"; "00000"; "00000"; "00000" ]
        | '_' -> [ "00000"; "00000"; "00000"; "00000"; "00000"; "00000"; "11111" ]
        | '+' -> [ "00000"; "00100"; "00100"; "11111"; "00100"; "00100"; "00000" ]
        | '=' -> [ "00000"; "00000"; "11111"; "00000"; "11111"; "00000"; "00000" ]
        | '(' -> [ "00010"; "00100"; "01000"; "01000"; "01000"; "00100"; "00010" ]
        | ')' -> [ "01000"; "00100"; "00010"; "00010"; "00010"; "00100"; "01000" ]
        | '[' -> [ "01110"; "01000"; "01000"; "01000"; "01000"; "01000"; "01110" ]
        | ']' -> [ "01110"; "00010"; "00010"; "00010"; "00010"; "00010"; "01110" ]
        | '&' -> [ "01100"; "10010"; "10100"; "01000"; "10101"; "10010"; "01101" ]
        | '%' -> [ "11001"; "11010"; "00010"; "00100"; "01000"; "01011"; "10011" ]
        | '!' -> [ "00100"; "00100"; "00100"; "00100"; "00100"; "00000"; "00100" ]
        | '?' -> [ "01110"; "10001"; "00001"; "00010"; "00100"; "00000"; "00100" ]
        | ' ' -> [ "00000"; "00000"; "00000"; "00000"; "00000"; "00000"; "00000" ]
        | _ -> [ "11111"; "00001"; "00010"; "00100"; "00100"; "00000"; "00100" ]

    let drawVectorText (canvas: SKCanvas) x y (text: string) size color antialias =
        let cell = Math.Max(1.0f, float32 size / 7.0f)
        let glyphAdvance = cell * 6.0f
        let top = float32 y - float32 size

        use paint = new SKPaint()
        paint.Color <- color
        paint.IsAntialias <- antialias
        paint.Style <- SKPaintStyle.Fill

        text
        |> Seq.iteri (fun index character ->
            let left = float32 x + float32 index * glyphAdvance

            vectorGlyphPattern character
            |> List.iteri (fun row line ->
                line
                |> Seq.iteri (fun column value ->
                    if value = '1' then
                        canvas.DrawRect(left + float32 column * cell, top + float32 row * cell, cell * 0.86f, cell * 0.86f, paint))))

    let drawTextWithFallback (canvas: SKCanvas) x y (text: string) size color antialias =
        let mutable nativeDrawn = false

        try
            use paint = new SKPaint()
            use font = new SKFont(SKTypeface.Default, float32 size)
            paint.Color <- color
            paint.IsAntialias <- antialias

            if font.ContainsGlyphs(text) then
                canvas.DrawText(text, SKPoint(float32 x, float32 y), font, paint)
                nativeDrawn <- true
        with _ ->
            nativeDrawn <- false

        if not nativeDrawn then
            drawVectorText canvas x y text size color antialias

    let drawScene scene (canvas: SKCanvas) =
        let rec drawNode node =
            match node with
            | Empty -> ()
            | Group scenes -> scenes |> List.iter (fun scene -> scene.Nodes |> List.iter drawNode)
            | Rectangle((x, y, width, height), fill) ->
                use paint = new SKPaint()
                paint.Color <- skColor fill
                paint.Style <- SKPaintStyle.Fill
                canvas.DrawRect(float32 x, float32 y, float32 width, float32 height, paint)
            | PaintedRectangle(bounds, scenePaint) ->
                use paint = new SKPaint()
                configurePaint scenePaint paint
                canvas.DrawRect(float32 bounds.X, float32 bounds.Y, float32 bounds.Width, float32 bounds.Height, paint)
            | Ellipse(bounds, scenePaint) ->
                use paint = new SKPaint()
                configurePaint scenePaint paint
                canvas.DrawOval(skRect bounds, paint)
            | Line(startPoint, endPoint, scenePaint) ->
                use paint = new SKPaint()
                configurePaint { scenePaint with Stroke = Some(scenePaint.Stroke |> Option.defaultValue { Width = 1.0; Cap = Butt; Join = Miter; Miter = 4.0 }) } paint
                canvas.DrawLine(float32 startPoint.X, float32 startPoint.Y, float32 endPoint.X, float32 endPoint.Y, paint)
            | Path(path, scenePaint) ->
                use paint = new SKPaint()
                use skPath = toSkPath path
                configurePaint scenePaint paint
                canvas.DrawPath(skPath, paint)
            | Points(points, scenePaint) ->
                use paint = new SKPaint()
                configurePaint scenePaint paint
                for point in points do
                    canvas.DrawPoint(float32 point.X, float32 point.Y, paint)
            | Vertices(mode, vertices, scenePaint) ->
                use paint = new SKPaint()
                configurePaint scenePaint paint

                if vertices.Length >= 3 then
                    let positions =
                        vertices
                        |> List.map (fun vertex -> SKPoint(float32 vertex.Position.X, float32 vertex.Position.Y))
                        |> List.toArray

                    let colors =
                        vertices
                        |> List.map (fun vertex -> vertex.Color |> Option.defaultValue (scenePaint.Fill |> Option.defaultValue Colors.white) |> skColor)
                        |> List.toArray

                    use skVertices = SKVertices.CreateCopy(vertexMode mode, positions, colors)
                    canvas.DrawVertices(skVertices, SKBlendMode.SrcOver, paint)
                else
                    for vertex in vertices do
                        use vertexPaint = new SKPaint()
                        configurePaint scenePaint vertexPaint
                        vertexPaint.Color <- vertex.Color |> Option.defaultValue (scenePaint.Fill |> Option.defaultValue Colors.white) |> skColor
                        canvas.DrawCircle(float32 vertex.Position.X, float32 vertex.Position.Y, 2.0f, vertexPaint)
            | Arc(bounds, startAngle, sweepAngle, scenePaint) ->
                use paint = new SKPaint()
                configurePaint scenePaint paint
                canvas.DrawArc(skRect bounds, float32 startAngle, float32 sweepAngle, false, paint)
            | Text((x, y), text, color) ->
                drawTextWithFallback canvas x y text 24.0 (skColor color) true
            | TextRun run ->
                drawTextWithFallback canvas run.Position.X run.Position.Y run.Text run.Font.Size (paintColor run.Paint) run.Paint.Antialias
            | Image((x, y, width, height), source) ->
                let destination = SKRect(float32 x, float32 y, float32 (x + width), float32 (y + height))

                if File.Exists source then
                    use image = SKImage.FromEncodedData(source)

                    if isNull image then
                        use paint = new SKPaint()
                        paint.Color <- SKColor(96uy, 128uy, 160uy, 255uy)
                        paint.Style <- SKPaintStyle.Stroke
                        paint.StrokeWidth <- 2.0f
                        canvas.DrawRect(destination, paint)
                    else
                        canvas.DrawImage(image, destination)
                else
                    use paint = new SKPaint()
                    paint.Color <- SKColor(96uy, 128uy, 160uy, 255uy)
                    paint.Style <- SKPaintStyle.Stroke
                    paint.StrokeWidth <- 2.0f
                    canvas.DrawRect(destination, paint)
            | ClipNode(clip, clippedScene) ->
                canvas.Save() |> ignore

                match clip with
                | RectClip bounds ->
                    canvas.ClipRect(skRect bounds) |> ignore
                | PathClip path ->
                    use skPath = toSkPath path
                    canvas.ClipPath(skPath) |> ignore

                clippedScene.Nodes |> List.iter drawNode
                canvas.Restore()
            | RegionNode(region, scenePaint) ->
                use paint = new SKPaint()
                configurePaint scenePaint paint

                for bounds in region.Bounds do
                    canvas.DrawRect(float32 bounds.X, float32 bounds.Y, float32 bounds.Width, float32 bounds.Height, paint)
            | ColorSpaceNode(_, scene) -> scene.Nodes |> List.iter drawNode
            | PerspectiveNode(transform, scene) ->
                canvas.Save() |> ignore
                let matrix =
                    SKMatrix(
                        float32 transform.M11,
                        float32 transform.M12,
                        float32 transform.M13,
                        float32 transform.M21,
                        float32 transform.M22,
                        float32 transform.M23,
                        float32 transform.M31,
                        float32 transform.M32,
                        float32 transform.M33
                    )

                canvas.Concat(&matrix)
                scene.Nodes |> List.iter drawNode
                canvas.Restore()
            | PictureNode picture -> picture.Scene.Nodes |> List.iter drawNode
            | Chart values ->
                if not values.IsEmpty then
                    let maxValue = values |> List.max
                    let chartLeft = 32.0f
                    let chartTop = 180.0f
                    let chartHeight = 220.0f
                    let barWidth = 32.0f
                    use paint = new SKPaint()
                    paint.Color <- SKColor(96uy, 190uy, 120uy, 255uy)
                    paint.Style <- SKPaintStyle.Fill

                    values
                    |> List.iteri (fun index value ->
                        let normalized = if maxValue <= 0.0 then 0.0 else value / maxValue
                        let height = float32 normalized * chartHeight
                        let x = chartLeft + float32 index * (barWidth + 12.0f)
                        let y = chartTop + chartHeight - height
                        canvas.DrawRect(x, y, barWidth, height, paint))

        scene.Nodes |> List.iter drawNode

    let colorTypeForFormat format =
        match format with
        | Silk.NET.Vulkan.Format.B8G8R8A8Unorm
        | Silk.NET.Vulkan.Format.B8G8R8A8Srgb -> SKColorType.Bgra8888
        | Silk.NET.Vulkan.Format.R8G8B8A8Unorm
        | Silk.NET.Vulkan.Format.R8G8B8A8Srgb -> SKColorType.Rgba8888
        | _ -> SKColorType.Bgra8888

    let bind result next =
        match result with
        | Ok value -> next value
        | Result.Error diagnostic -> Result.Error diagnostic

    let findMemoryType (vk: Vk) physicalDevice typeFilter requiredFlags =
        let mutable properties = PhysicalDeviceMemoryProperties()
        vk.GetPhysicalDeviceMemoryProperties(physicalDevice, &properties)

        [ 0u .. properties.MemoryTypeCount - 1u ]
        |> List.tryFind (fun index ->
            let memoryType = properties.MemoryTypes[int index]
            let supported = (typeFilter &&& (1u <<< int index)) <> 0u
            supported && memoryType.PropertyFlags.HasFlag requiredFlags)

    let createStagingBuffer configuration (vk: Vk) physicalDevice device (pixels: byte[]) =
        let size = uint64 pixels.Length
        let mutable bufferInfo = BufferCreateInfo()
        bufferInfo.SType <- StructureType.BufferCreateInfo
        bufferInfo.Size <- size
        bufferInfo.Usage <- BufferUsageFlags.TransferSrcBit
        bufferInfo.SharingMode <- SharingMode.Exclusive

        let mutable buffer = Silk.NET.Vulkan.Buffer()
        let bufferResult = vk.CreateBuffer(device, &bufferInfo, nullPtr<AllocationCallbacks>, &buffer)

        match checkResult FrameRender "vkCreateBuffer(staging)" bufferResult with
        | Result.Error diagnostic -> Result.Error diagnostic
        | Ok() ->
            let mutable memory = DeviceMemory()

            try
                let mutable requirements = MemoryRequirements()
                vk.GetBufferMemoryRequirements(device, buffer, &requirements)

                match findMemoryType vk physicalDevice requirements.MemoryTypeBits (MemoryPropertyFlags.HostVisibleBit ||| MemoryPropertyFlags.HostCoherentBit) with
                | None ->
                    Result.Error(Diagnostics.create Error FrameRender "No host-visible Vulkan memory type is available for Skia frame upload." None)
                | Some memoryTypeIndex ->
                    let mutable allocateInfo = MemoryAllocateInfo()
                    allocateInfo.SType <- StructureType.MemoryAllocateInfo
                    allocateInfo.AllocationSize <- requirements.Size
                    allocateInfo.MemoryTypeIndex <- memoryTypeIndex

                    let allocateResult = vk.AllocateMemory(device, &allocateInfo, nullPtr<AllocationCallbacks>, &memory)

                    match checkResult FrameRender "vkAllocateMemory(staging)" allocateResult with
                    | Result.Error diagnostic -> Result.Error diagnostic
                    | Ok() ->
                        let bindResult = vk.BindBufferMemory(device, buffer, memory, 0UL)

                        match checkResult FrameRender "vkBindBufferMemory(staging)" bindResult with
                        | Result.Error diagnostic -> Result.Error diagnostic
                        | Ok() ->
                            let mutable mapped: voidptr = NativePtr.toVoidPtr (NativePtr.ofNativeInt<byte> 0n)
                            let mapResult = vk.MapMemory(device, memory, 0UL, size, Unchecked.defaultof<MemoryMapFlags>, &mapped)

                            match checkResult FrameRender "vkMapMemory(staging)" mapResult with
                            | Result.Error diagnostic -> Result.Error diagnostic
                            | Ok() ->
                                Marshal.Copy(pixels, 0, NativePtr.toNativeInt (NativePtr.ofVoidPtr<byte> mapped), pixels.Length)
                                vk.UnmapMemory(device, memory)
                                Ok { Buffer = buffer; Memory = memory; Size = size }
            with ex ->
                if memory.Handle <> 0UL then
                    vk.FreeMemory(device, memory, nullPtr<AllocationCallbacks>)

                if buffer.Handle <> 0UL then
                    vk.DestroyBuffer(device, buffer, nullPtr<AllocationCallbacks>)

                Result.Error(Diagnostics.create Error FrameRender "Vulkan staging buffer creation failed." (Some ex.Message))

    let destroyStagingBuffer (vk: Vk) device staging =
        if staging.Buffer.Handle <> 0UL then
            vk.DestroyBuffer(device, staging.Buffer, nullPtr<AllocationCallbacks>)

        if staging.Memory.Handle <> 0UL then
            vk.FreeMemory(device, staging.Memory, nullPtr<AllocationCallbacks>)

    let renderSceneToPixels configuration (skiaState: SkiaState) (extent: Extent2D) colorType scene =
        try
            let width = int extent.Width
            let height = int extent.Height
            let imageInfo = SKImageInfo(width, height, colorType, SKAlphaType.Premul)

            use surface =
                SKSurface.Create(skiaState.Context, true, imageInfo, 1, GRSurfaceOrigin.TopLeft)

            if isNull surface then
                Result.Error(Diagnostics.create Error FrameRender "SkiaSharp did not create an offscreen Vulkan surface for scene rendering." None)
            else
                let clear =
                    configuration.ClearColor
                    |> Option.defaultValue Colors.black
                    |> skColor

                trace configuration "drawing model-derived scene into Skia Vulkan surface"
                surface.Canvas.Clear clear
                drawScene scene surface.Canvas
                surface.Canvas.Flush()
                surface.Flush()
                skiaState.Context.Flush()
                skiaState.Context.Submit(true)

                let rowBytes = imageInfo.RowBytes
                let pixels = Array.zeroCreate<byte> (rowBytes * height)
                let handle = GCHandle.Alloc(pixels, GCHandleType.Pinned)

                try
                    let ok = surface.ReadPixels(imageInfo, handle.AddrOfPinnedObject(), rowBytes, 0, 0)

                    if ok then
                        Ok pixels
                    else
                        Result.Error(Diagnostics.create Error FrameRender "SkiaSharp could not read the rendered scene pixels." None)
                finally
                    handle.Free()
        with ex ->
            Result.Error(Diagnostics.create Error FrameRender "Skia scene rendering failed." (Some ex.Message))

    let copyPixelsToSwapchainImage configuration (vk: Vk) (swapchainExt: KhrSwapchain) physicalDevice device (swapchainState: SwapchainState) queueFamily queue image imageIndex colorType pixels =
        bind (createStagingBuffer configuration vk physicalDevice device pixels) (fun staging ->
                try
                    let mutable poolInfo = CommandPoolCreateInfo()
                    poolInfo.SType <- StructureType.CommandPoolCreateInfo
                    poolInfo.Flags <- CommandPoolCreateFlags.ResetCommandBufferBit
                    poolInfo.QueueFamilyIndex <- queueFamily

                    let mutable commandPool = CommandPool()
                    let poolResult = vk.CreateCommandPool(device, &poolInfo, nullPtr<AllocationCallbacks>, &commandPool)

                    match checkResult FrameRender "vkCreateCommandPool(frame upload)" poolResult with
                    | Result.Error diagnostic -> Result.Error diagnostic
                    | Ok() ->
                        try
                            let mutable allocInfo = CommandBufferAllocateInfo()
                            allocInfo.SType <- StructureType.CommandBufferAllocateInfo
                            allocInfo.CommandPool <- commandPool
                            allocInfo.Level <- CommandBufferLevel.Primary
                            allocInfo.CommandBufferCount <- 1u

                            let mutable commandBuffer = CommandBuffer()
                            let allocResult = vk.AllocateCommandBuffers(device, &allocInfo, &commandBuffer)

                            match checkResult FrameRender "vkAllocateCommandBuffers(frame upload)" allocResult with
                            | Result.Error diagnostic -> Result.Error diagnostic
                            | Ok() ->
                                let mutable beginInfo = CommandBufferBeginInfo()
                                beginInfo.SType <- StructureType.CommandBufferBeginInfo
                                beginInfo.Flags <- CommandBufferUsageFlags.OneTimeSubmitBit

                                let beginResult = vk.BeginCommandBuffer(commandBuffer, &beginInfo)

                                match checkResult FrameRender "vkBeginCommandBuffer(frame upload)" beginResult with
                                | Result.Error diagnostic -> Result.Error diagnostic
                                | Ok() ->
                                    let mutable toTransfer =
                                        transitionBarrier
                                            image
                                            ImageLayout.Undefined
                                            ImageLayout.TransferDstOptimal
                                            AccessFlags.None
                                            AccessFlags.TransferWriteBit

                                    vk.CmdPipelineBarrier(
                                        commandBuffer,
                                        PipelineStageFlags.TopOfPipeBit,
                                        PipelineStageFlags.TransferBit,
                                        DependencyFlags.None,
                                        0u,
                                        nullPtr<MemoryBarrier>,
                                        0u,
                                        nullPtr<BufferMemoryBarrier>,
                                        1u,
                                        &toTransfer
                                    )

                                    let mutable subresource = ImageSubresourceLayers()
                                    subresource.AspectMask <- ImageAspectFlags.ColorBit
                                    subresource.MipLevel <- 0u
                                    subresource.BaseArrayLayer <- 0u
                                    subresource.LayerCount <- 1u

                                    let mutable copyRegion = BufferImageCopy()
                                    copyRegion.BufferOffset <- 0UL
                                    copyRegion.BufferRowLength <- 0u
                                    copyRegion.BufferImageHeight <- 0u
                                    copyRegion.ImageSubresource <- subresource
                                    copyRegion.ImageOffset <- Offset3D(0, 0, 0)
                                    copyRegion.ImageExtent <- Extent3D(swapchainState.Extent.Width, swapchainState.Extent.Height, 1u)

                                    vk.CmdCopyBufferToImage(commandBuffer, staging.Buffer, image, ImageLayout.TransferDstOptimal, 1u, &copyRegion)

                                    let mutable toPresent =
                                        transitionBarrier
                                            image
                                            ImageLayout.TransferDstOptimal
                                            ImageLayout.PresentSrcKhr
                                            AccessFlags.TransferWriteBit
                                            AccessFlags.None

                                    vk.CmdPipelineBarrier(
                                        commandBuffer,
                                        PipelineStageFlags.TransferBit,
                                        PipelineStageFlags.BottomOfPipeBit,
                                        DependencyFlags.None,
                                        0u,
                                        nullPtr<MemoryBarrier>,
                                        0u,
                                        nullPtr<BufferMemoryBarrier>,
                                        1u,
                                        &toPresent
                                    )

                                    let endResult = vk.EndCommandBuffer(commandBuffer)

                                    match checkResult FrameRender "vkEndCommandBuffer(frame upload)" endResult with
                                    | Result.Error diagnostic -> Result.Error diagnostic
                                    | Ok() ->
                                        let mutable submitInfo = SubmitInfo()
                                        submitInfo.SType <- StructureType.SubmitInfo
                                        submitInfo.CommandBufferCount <- 1u
                                        submitInfo.PCommandBuffers <- &&commandBuffer

                                        let submitResult = vk.QueueSubmit(queue, 1u, &submitInfo, Fence(Nullable<uint64>()))

                                        match checkResult FrameRender "vkQueueSubmit(frame upload)" submitResult with
                                        | Result.Error diagnostic -> Result.Error diagnostic
                                        | Ok() ->
                                            let waitResult = vk.QueueWaitIdle(queue)

                                            match checkResult FrameRender "vkQueueWaitIdle(frame upload)" waitResult with
                                            | Result.Error diagnostic -> Result.Error diagnostic
                                            | Ok() ->
                                                let mutable presentInfo = PresentInfoKHR()
                                                let mutable presentedSwapchain = swapchainState.Swapchain
                                                let mutable presentImageIndex = imageIndex
                                                presentInfo.SType <- StructureType.PresentInfoKhr
                                                presentInfo.SwapchainCount <- 1u
                                                presentInfo.PSwapchains <- &&presentedSwapchain
                                                presentInfo.PImageIndices <- &&presentImageIndex

                                                let presentResult = swapchainExt.QueuePresent(queue, &presentInfo)

                                                match checkResult FrameRender "vkQueuePresentKHR" presentResult with
                                                | Ok() -> Ok()
                                                | Result.Error diagnostic -> Result.Error diagnostic
                        finally
                            if commandPool.Handle <> 0UL then
                                vk.DestroyCommandPool(device, commandPool, nullPtr<AllocationCallbacks>)
                finally
                    destroyStagingBuffer vk device staging)

    let renderFrame configuration (vk: Vk) (swapchainExt: KhrSwapchain) physicalDevice device (swapchainState: SwapchainState) (skiaState: SkiaState) queueFamily scene =
        try
            trace configuration "querying swapchain images"
            match getSwapchainImages swapchainExt device swapchainState.Swapchain with
            | Result.Error diagnostic -> Result.Error diagnostic
            | Ok images ->
                match createFence vk device with
                | Result.Error diagnostic -> Result.Error diagnostic
                | Ok fence ->
                    try
                        match acquireImage vk swapchainExt device swapchainState.Swapchain fence with
                        | Result.Error diagnostic -> Result.Error diagnostic
                        | Ok imageIndex ->
                            let image = images[int imageIndex]
                            let colorType = colorTypeForFormat swapchainState.Format
                            trace configuration $"rendering Skia scene for swapchain image index={imageIndex} format={swapchainState.Format} colorType={colorType}"
                            trace configuration $"skia maxSampleCount colorType={colorType} count={skiaState.Context.GetMaxSurfaceSampleCount(colorType)}"
                            trace configuration $"skia context abandoned={skiaState.Context.IsAbandoned} maxRenderTargetSize={skiaState.Context.MaxRenderTargetSize}"
                            bind (renderSceneToPixels configuration skiaState swapchainState.Extent colorType scene) (fun pixels ->
                                bind (copyPixelsToSwapchainImage configuration vk swapchainExt physicalDevice device swapchainState queueFamily skiaState.Queue image imageIndex colorType pixels) (fun () ->
                                    Ok
                                        { Width = int swapchainState.Extent.Width
                                          Height = int swapchainState.Extent.Height
                                          ColorType = colorType
                                          Pixels = pixels }))
                    finally
                        if fence.Handle <> 0UL then
                            vk.DestroyFence(device, fence, nullPtr<AllocationCallbacks>)
        with ex ->
            Result.Error(Diagnostics.frameRenderFailed ex.Message)

    let encodeSnapshot (request: ScreenshotRequest) snapshot =
        try
            let directory = Path.GetDirectoryName request.Destination

            if not (String.IsNullOrWhiteSpace directory) then
                Directory.CreateDirectory directory |> ignore

            let imageInfo =
                SKImageInfo(snapshot.Width, snapshot.Height, snapshot.ColorType, SKAlphaType.Premul)

            let handle = GCHandle.Alloc(snapshot.Pixels, GCHandleType.Pinned)

            try
                use pixmap =
                    new SKPixmap(imageInfo, handle.AddrOfPinnedObject(), imageInfo.RowBytes)

                use image = SKImage.FromPixels pixmap

                if isNull image then
                    Result.Error(Diagnostics.screenshotFailed "SkiaSharp could not create an image from the last rendered Vulkan frame.")
                else
                    let format =
                        match request.Format with
                        | Png -> SKEncodedImageFormat.Png
                        | Jpeg -> SKEncodedImageFormat.Jpeg

                    use data = image.Encode(format, 90)

                    if isNull data then
                        Result.Error(Diagnostics.screenshotFailed "SkiaSharp could not encode the screenshot image.")
                    else
                        use stream = File.Open(request.Destination, FileMode.Create, FileAccess.Write, FileShare.None)
                        data.SaveTo stream
                        Ok()
            finally
                handle.Free()
        with ex ->
            Result.Error(Diagnostics.screenshotFailed ex.Message)

    let run program =
        let vk = Vk.GetApi()
        let mutable currentModel = Unchecked.defaultof<_>
        let mutable instance = Instance()
        let mutable surface = SurfaceKHR()
        let mutable device = Device()
        let mutable swapchainState: SwapchainState option = None
        let mutable skiaContext: GRContext option = None
        let mutable skiaExtensions: GRVkExtensions option = None
        let mutable window: IWindow option = None
        let mutable windowEventMapping: IDisposable option = None
        let mutable inputEventMapping: IDisposable option = None
        let mutable activeSubscriptions: IDisposable list = []
        let mutable pendingScene: Scene option = None
        let mutable pendingScreenshots: ScreenshotRequest list = []
        let mutable renderScene: (Scene -> Result<FrameSnapshot, RenderDiagnostic>) option = None
        let mutable lastFrame: FrameSnapshot option = None
        let mutable surfaceExt: KhrSurface option = None
        let mutable swapchainExt: KhrSwapchain option = None
        let mutable shutdownRequested = false

        let requestShutdown closeWindow =
            shutdownRequested <- true

            match window with
            | Some w ->
                if closeWindow && not w.IsClosing then
                    try
                        w.Close()
                    with _ ->
                        ()

                try
                    w.IsClosing <- true
                with _ ->
                    ()
            | None -> ()

        let disposeSubscriptions () =
            activeSubscriptions
            |> List.iter (fun subscription -> subscription.Dispose())

            activeSubscriptions <- []

        let rec saveScreenshot request snapshot =
            match encodeSnapshot request snapshot with
            | Ok() -> Ok()
            | Result.Error diagnostic ->
                dispatchViewerEvent program dispatch (DiagnosticReported diagnostic)
                Result.Error diagnostic

        and flushPendingScreenshots snapshot =
            let requests = pendingScreenshots
            pendingScreenshots <- []

            requests
            |> List.fold
                (fun state request ->
                    match state with
                    | Result.Error diagnostic -> Result.Error diagnostic
                    | Ok() -> saveScreenshot request snapshot)
                (Ok())

        and interpretEffect effect =
            match effect with
            | InitializeRenderer -> Ok()
            | RenderFrame scene ->
                match renderScene with
                | Some render ->
                    match render scene with
                    | Ok snapshot ->
                        lastFrame <- Some snapshot
                        flushPendingScreenshots snapshot
                    | Result.Error diagnostic ->
                        dispatchViewerEvent program dispatch (DiagnosticReported diagnostic)
                        Result.Error diagnostic
                | None ->
                    pendingScene <- Some scene
                    Ok()
            | CaptureScreenshot request ->
                match lastFrame with
                | Some snapshot ->
                    saveScreenshot request snapshot
                | None ->
                    match pendingScene with
                    | Some _ ->
                        pendingScreenshots <- pendingScreenshots @ [ request ]
                        Ok()
                    | None ->
                        let diagnostic =
                            Diagnostics.screenshotFailed "Screenshot capture was requested before the first successful Vulkan/Skia frame."

                        dispatchViewerEvent program dispatch (DiagnosticReported diagnostic)
                        Result.Error diagnostic
            | Shutdown ->
                requestShutdown true
                disposeSubscriptions ()
                Ok()
            | ReportDiagnostic diagnostic ->
                if program.Configuration.Diagnostics.Verbose then
                    Console.Error.WriteLine($"FS.Skia.UI diagnostic: {diagnostic.Stage}: {diagnostic.Message}")

                Ok()
            | Dispatch msg ->
                dispatch msg
                Ok()

        and dispatch msg =
            match program.EffectMapper msg with
            | Some effect ->
                interpretEffect effect |> ignore
            | None ->
                let nextModel, cmd = program.Update msg currentModel
                currentModel <- nextModel

                cmd
                |> List.iter (fun effect -> effect dispatch)

        let startSubscriptions () =
            disposeSubscriptions ()

            activeSubscriptions <-
                program.Subscriptions currentModel
                |> List.map (fun (_, subscribe) -> subscribe dispatch)

        let runEventLoop (createdWindow: IWindow) =
            if not shutdownRequested then
                trace program.Configuration "entering Silk.NET event loop"
                let frameInterval =
                    program.Configuration.TargetFrameRate
                    |> Option.defaultValue 60
                    |> max 1
                    |> fun frameRate -> 1.0 / float frameRate

                let stopwatch = System.Diagnostics.Stopwatch.StartNew()
                let mutable lastFrameTime = stopwatch.Elapsed.TotalSeconds

                while not createdWindow.IsClosing && not shutdownRequested do
                    createdWindow.DoEvents()

                    if shutdownRequested then
                        try
                            createdWindow.IsClosing <- true
                        with _ ->
                            ()
                    else
                        let now = stopwatch.Elapsed.TotalSeconds

                        if now - lastFrameTime >= frameInterval then
                            lastFrameTime <- now
                            createdWindow.DoUpdate()

                        if not shutdownRequested && not createdWindow.IsClosing then
                            createdWindow.DoRender()

                        Threading.Thread.Sleep(1)

        let execute () =
            try
                let initialModel, initialCmd = program.Init()
                currentModel <- initialModel
                initialCmd |> List.iter (fun effect -> effect dispatch)
                startSubscriptions ()

                trace program.Configuration "creating Silk.NET window"
                bind (createWindow program.Configuration) (fun createdWindow ->
                    window <- Some createdWindow
                    windowEventMapping <- Some(attachWindowEventMapping program createdWindow (fun () -> requestShutdown false) dispatch)

                    trace program.Configuration "initializing Silk.NET window"
                    bind (initializeWindow createdWindow) (fun () ->
                        trace program.Configuration "attaching Silk.NET input event mapping"
                        bind (attachInputEventMapping program createdWindow dispatch) (fun inputMapping ->
                            inputEventMapping <- Some inputMapping

                            trace program.Configuration "querying Vulkan surface source"
                            bind (getSurfaceSource createdWindow) (fun (surfaceSource: IVkSurface) ->
                            trace program.Configuration "querying required Vulkan extensions"
                            bind (copyRequiredExtensions surfaceSource) (fun (extensionNames, extensionCount) ->
                                trace program.Configuration $"creating Vulkan instance extensionCount={extensionCount}"
                                bind (createInstance vk extensionNames extensionCount) (fun createdInstance ->
                                    instance <- createdInstance

                                    let createdSurfaceExt = new KhrSurface(vk.Context)
                                    let createdSwapchainExt = new KhrSwapchain(vk.Context)
                                    surfaceExt <- Some createdSurfaceExt
                                    swapchainExt <- Some createdSwapchainExt

                                    trace program.Configuration "creating Vulkan presentation surface"
                                    bind (createSurface surfaceSource instance) (fun createdSurface ->
                                        surface <- createdSurface

                                        trace program.Configuration "enumerating physical devices"
                                        bind (enumeratePhysicalDevices vk instance) (fun physicalDevices ->
                                            trace program.Configuration $"choosing physical device count={physicalDevices.Length}"
                                            bind (choosePhysicalDevice vk createdSurfaceExt surface physicalDevices) (fun (physicalDevice, queueFamily) ->
                                                trace program.Configuration $"creating logical device physicalDevice={physicalDevice.Handle} queueFamily={queueFamily}"
                                                bind (createDevice vk physicalDevice queueFamily) (fun createdDevice ->
                                                    device <- createdDevice

                                                    trace program.Configuration "creating swapchain"
                                                    bind (createSwapchain program.Configuration createdSurfaceExt createdSwapchainExt physicalDevice device surface) (fun createdSwapchain ->
                                                        swapchainState <- Some createdSwapchain

                                                        bind (createSkiaContext program.Configuration vk instance physicalDevice device queueFamily) (fun skia ->
                                                            skiaContext <- Some skia.Context
                                                            skiaExtensions <- Some skia.Extensions

                                                            renderScene <-
                                                                Some(renderFrame program.Configuration vk createdSwapchainExt physicalDevice device createdSwapchain skia queueFamily)

                                                            let scene =
                                                                pendingScene
                                                                |> Option.defaultValue (program.View currentModel)

                                                            pendingScene <- None
                                                            bind (interpretEffect (RenderFrame scene)) (fun () ->
                                                                runEventLoop createdWindow
                                                                Ok())))))))))))))
            with ex ->
                Result.Error(Diagnostics.frameRenderFailed ex.Message)

        try
            execute ()
        finally
            disposeSubscriptions ()

            match inputEventMapping with
            | Some mapping -> mapping.Dispose()
            | None -> ()

            match windowEventMapping with
            | Some mapping -> mapping.Dispose()
            | None -> ()

            match skiaContext with
            | Some context -> context.Dispose()
            | None -> ()

            match skiaExtensions with
            | Some extensions -> extensions.Dispose()
            | None -> ()

            match swapchainExt with
            | Some ext ->
                match swapchainState with
                | Some state when state.Swapchain.Handle <> 0UL ->
                    let _ = vk.DeviceWaitIdle(device)
                    ext.DestroySwapchain(device, state.Swapchain, nullPtr<AllocationCallbacks>)
                | _ -> ()
            | _ -> ()

            if device.Handle <> IntPtr.Zero then
                vk.DestroyDevice(device, nullPtr<AllocationCallbacks>)

            match surfaceExt with
            | Some ext when surface.Handle <> 0UL -> ext.DestroySurface(instance, surface, nullPtr<AllocationCallbacks>)
            | _ -> ()

            if instance.Handle <> IntPtr.Zero then
                vk.DestroyInstance(instance, nullPtr<AllocationCallbacks>)

            match window with
            | Some w ->
                try
                    w.IsClosing <- true
                with _ ->
                    ()

                w.Dispose()
            | None -> ()

module Viewer =
    let defaultConfiguration title initialSize =
        { Title = title
          InitialSize = initialSize
          ClearColor = Some Colors.black
          TargetFrameRate = Some 60
          Diagnostics = { Verbose = false } }

    let create configuration init update view =
        { Configuration = configuration
          Init = init
          Update = update
          View = view
          EventMapper = fun _ -> None
          EffectMapper = fun _ -> None
          Subscriptions = fun _ -> [] }

    let withSubscription subscription program =
        { program with Subscriptions = subscription }

    let withEventMapping mapper program =
        { program with EventMapper = mapper }

    let withEffectMapping mapper program =
        { program with EffectMapper = mapper }

    let validate configuration =
        if String.IsNullOrWhiteSpace configuration.Title then
            Result.Error(Diagnostics.invalidConfiguration "Viewer title must not be empty.")
        elif configuration.InitialSize.Width <= 0 || configuration.InitialSize.Height <= 0 then
            Result.Error(Diagnostics.invalidConfiguration "Viewer initial size must be positive.")
        elif
            configuration.TargetFrameRate
            |> Option.exists (fun frameRate -> frameRate <= 0)
        then
            Result.Error(Diagnostics.invalidConfiguration "Target frame rate must be positive when supplied.")
        elif OperatingSystem.IsWindows() || OperatingSystem.IsLinux() then
            Ok()
        else
            Result.Error(Diagnostics.unsupportedPlatform (Environment.OSVersion.Platform.ToString()))

    let run program : Result<unit, RenderDiagnostic> =
        match validate program.Configuration with
        | Result.Error diagnostic -> Result.Error diagnostic
        | Ok() -> VulkanHost.run program

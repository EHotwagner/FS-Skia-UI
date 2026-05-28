namespace FS.Skia.UI.SkiaViewer

open FS.Skia.UI.Scene

module SceneConversion =
    let rec toLegacyScene (scene: Scene) : FS.Skia.UI.Scene =
        let legacyScenes = scene.Nodes |> List.map toLegacyNode
        FS.Skia.UI.Scene.group legacyScenes

    and private toLegacyNode node : FS.Skia.UI.Scene =
        match node with
        | Empty -> FS.Skia.UI.Scene.empty
        | Group scenes -> scenes |> List.map toLegacyScene |> FS.Skia.UI.Scene.group
        | Rectangle(bounds, color) -> FS.Skia.UI.Scene.rectangle bounds (toLegacyColor color)
        | PaintedRectangle(bounds, paint) -> FS.Skia.UI.Scene.rectangleWithPaint (toLegacyRect bounds) (toLegacyPaint paint)
        | Circle(center, radius, fill) ->
            let bounds =
                { X = center.X - radius
                  Y = center.Y - radius
                  Width = radius * 2.0
                  Height = radius * 2.0 }

            FS.Skia.UI.Scene.ellipse (toLegacyRect bounds) (FS.Skia.UI.Paint.fill (toLegacyColor fill))
        | FilledEllipse(bounds, fill) -> FS.Skia.UI.Scene.ellipse (toLegacyRect bounds) (FS.Skia.UI.Paint.fill (toLegacyColor fill))
        | Ellipse(bounds, paint) -> FS.Skia.UI.Scene.ellipse (toLegacyRect bounds) (toLegacyPaint paint)
        | Line(startPoint, endPoint, paint) -> FS.Skia.UI.Scene.line (toLegacyPoint startPoint) (toLegacyPoint endPoint) (toLegacyPaint paint)
        | Path(path, paint) -> FS.Skia.UI.Scene.path (toLegacyPath path) (toLegacyPaint paint)
        | Points(points, paint) -> FS.Skia.UI.Scene.points (points |> List.map toLegacyPoint) (toLegacyPaint paint)
        | Vertices(mode, vertices, paint) -> FS.Skia.UI.Scene.vertices (toLegacyVertexMode mode) (vertices |> List.map toLegacyVertex) (toLegacyPaint paint)
        | Arc(bounds, startAngle, sweepAngle, paint) -> FS.Skia.UI.Scene.arc (toLegacyRect bounds) startAngle sweepAngle (toLegacyPaint paint)
        | Text(position, text, color) -> FS.Skia.UI.Scene.text position text (toLegacyColor color)
        | TextRun run -> FS.Skia.UI.Scene.textRun (toLegacyTextRun run)
        | SceneNode.Image(bounds, source) -> FS.Skia.UI.Scene.image bounds source
        | ClipNode(clip, clippedScene) -> FS.Skia.UI.Scene.clipped (toLegacyClip clip) (toLegacyScene clippedScene)
        | RegionNode(region, paint) -> FS.Skia.UI.Scene.region (toLegacyRegion region) (toLegacyPaint paint)
        | ColorSpaceNode(colorSpace, child) -> FS.Skia.UI.Scene.withColorSpace (toLegacyColorSpace colorSpace) (toLegacyScene child)
        | PerspectiveNode(transform, child) -> FS.Skia.UI.Scene.withPerspective (toLegacyPerspective transform) (toLegacyScene child)
        | PictureNode picture ->
            let legacyPicture: FS.Skia.UI.Picture =
                { Name = picture.Name
                  Scene = toLegacyScene picture.Scene }

            FS.Skia.UI.Scene.picture legacyPicture
        | Chart values -> FS.Skia.UI.Scene.chart values

    and private toLegacyColor (color: Color) : FS.Skia.UI.Color =
        { Red = color.Red
          Green = color.Green
          Blue = color.Blue
          Alpha = color.Alpha }

    and private toLegacyPoint (point: Point) : FS.Skia.UI.Point =
        { X = point.X
          Y = point.Y }

    and private toLegacyRect (rect: Rect) : FS.Skia.UI.Rect =
        { X = rect.X
          Y = rect.Y
          Width = rect.Width
          Height = rect.Height }

    and private toLegacyStrokeCap cap : FS.Skia.UI.StrokeCap =
        match cap with
        | Butt -> FS.Skia.UI.StrokeCap.Butt
        | Round -> FS.Skia.UI.StrokeCap.Round
        | Square -> FS.Skia.UI.StrokeCap.Square

    and private toLegacyStrokeJoin join : FS.Skia.UI.StrokeJoin =
        match join with
        | Miter -> FS.Skia.UI.StrokeJoin.Miter
        | RoundJoin -> FS.Skia.UI.StrokeJoin.RoundJoin
        | Bevel -> FS.Skia.UI.StrokeJoin.Bevel

    and private toLegacyBlendMode mode : FS.Skia.UI.BlendMode =
        match mode with
        | SrcOver -> FS.Skia.UI.BlendMode.SrcOver
        | Multiply -> FS.Skia.UI.BlendMode.Multiply
        | Screen -> FS.Skia.UI.BlendMode.Screen
        | Overlay -> FS.Skia.UI.BlendMode.Overlay
        | Darken -> FS.Skia.UI.BlendMode.Darken
        | Lighten -> FS.Skia.UI.BlendMode.Lighten
        | ColorDodge -> FS.Skia.UI.BlendMode.ColorDodge
        | ColorBurn -> FS.Skia.UI.BlendMode.ColorBurn
        | BlendMode.Difference -> FS.Skia.UI.BlendMode.Difference
        | Exclusion -> FS.Skia.UI.BlendMode.Exclusion

    and private toLegacyStroke (stroke: Stroke) : FS.Skia.UI.Stroke =
        { Width = stroke.Width
          Cap = toLegacyStrokeCap stroke.Cap
          Join = toLegacyStrokeJoin stroke.Join
          Miter = stroke.Miter }

    and private toLegacyShader shader : FS.Skia.UI.Shader =
        match shader with
        | SolidColor color -> FS.Skia.UI.Shader.SolidColor(toLegacyColor color)
        | LinearGradient(startPoint, endPoint, colors) ->
            FS.Skia.UI.Shader.LinearGradient(toLegacyPoint startPoint, toLegacyPoint endPoint, colors |> List.map toLegacyColor)
        | RadialGradient(center, radius, colors) ->
            FS.Skia.UI.Shader.RadialGradient(toLegacyPoint center, radius, colors |> List.map toLegacyColor)
        | SweepGradient(center, colors) -> FS.Skia.UI.Shader.SweepGradient(toLegacyPoint center, colors |> List.map toLegacyColor)

    and private toLegacyColorFilter filter : FS.Skia.UI.ColorFilter =
        match filter with
        | NoColorFilter -> FS.Skia.UI.ColorFilter.NoColorFilter
        | BlendColor(color, mode) -> FS.Skia.UI.ColorFilter.BlendColor(toLegacyColor color, toLegacyBlendMode mode)

    and private toLegacyMaskFilter filter : FS.Skia.UI.MaskFilter =
        match filter with
        | NoMaskFilter -> FS.Skia.UI.MaskFilter.NoMaskFilter
        | Blur sigma -> FS.Skia.UI.MaskFilter.Blur sigma

    and private toLegacyImageFilter filter : FS.Skia.UI.ImageFilter =
        match filter with
        | NoImageFilter -> FS.Skia.UI.ImageFilter.NoImageFilter
        | DropShadow(dx, dy, blur, color) -> FS.Skia.UI.ImageFilter.DropShadow(dx, dy, blur, toLegacyColor color)

    and private toLegacyPathEffect effect : FS.Skia.UI.PathEffect =
        match effect with
        | NoPathEffect -> FS.Skia.UI.PathEffect.NoPathEffect
        | Dash(intervals, phase) -> FS.Skia.UI.PathEffect.Dash(intervals, phase)
        | Discrete(segmentLength, deviation) -> FS.Skia.UI.PathEffect.Discrete(segmentLength, deviation)
        | Corner radius -> FS.Skia.UI.PathEffect.Corner radius

    and private toLegacyPaint (paint: Paint) : FS.Skia.UI.Paint =
        { Fill = paint.Fill |> Option.map toLegacyColor
          Stroke = paint.Stroke |> Option.map toLegacyStroke
          Opacity = paint.Opacity
          Antialias = paint.Antialias
          BlendMode = toLegacyBlendMode paint.BlendMode
          Shader = paint.Shader |> Option.map toLegacyShader
          ColorFilter = toLegacyColorFilter paint.ColorFilter
          MaskFilter = toLegacyMaskFilter paint.MaskFilter
          ImageFilter = toLegacyImageFilter paint.ImageFilter
          PathEffect = toLegacyPathEffect paint.PathEffect }

    and private toLegacyPathCommand command : FS.Skia.UI.PathCommand =
        match command with
        | MoveTo point -> FS.Skia.UI.PathCommand.MoveTo(toLegacyPoint point)
        | LineTo point -> FS.Skia.UI.PathCommand.LineTo(toLegacyPoint point)
        | QuadTo(control, point) -> FS.Skia.UI.PathCommand.QuadTo(toLegacyPoint control, toLegacyPoint point)
        | CubicTo(control1, control2, point) ->
            FS.Skia.UI.PathCommand.CubicTo(toLegacyPoint control1, toLegacyPoint control2, toLegacyPoint point)
        | ArcTo(bounds, startAngle, sweepAngle) -> FS.Skia.UI.PathCommand.ArcTo(toLegacyRect bounds, startAngle, sweepAngle)
        | Close -> FS.Skia.UI.PathCommand.Close

    and private toLegacyPathFillType fillType : FS.Skia.UI.PathFillType =
        match fillType with
        | Winding -> FS.Skia.UI.PathFillType.Winding
        | EvenOdd -> FS.Skia.UI.PathFillType.EvenOdd

    and private toLegacyPath (path: PathSpec) : FS.Skia.UI.PathSpec =
        { Commands = path.Commands |> List.map toLegacyPathCommand
          FillType = toLegacyPathFillType path.FillType }

    and private toLegacyClip clip : FS.Skia.UI.Clip =
        match clip with
        | RectClip rect -> FS.Skia.UI.Clip.RectClip(toLegacyRect rect)
        | PathClip path -> FS.Skia.UI.Clip.PathClip(toLegacyPath path)

    and private toLegacyRegionOperation operation : FS.Skia.UI.RegionOperation =
        match operation with
        | Replace -> FS.Skia.UI.RegionOperation.Replace
        | RegionUnion -> FS.Skia.UI.RegionOperation.RegionUnion
        | RegionIntersect -> FS.Skia.UI.RegionOperation.RegionIntersect
        | RegionDifference -> FS.Skia.UI.RegionOperation.RegionDifference

    and private toLegacyRegion (region: Region) : FS.Skia.UI.Region =
        { Bounds = region.Bounds |> List.map toLegacyRect
          Operation = toLegacyRegionOperation region.Operation }

    and private toLegacyColorSpace colorSpace : FS.Skia.UI.ColorSpace =
        match colorSpace with
        | Srgb -> FS.Skia.UI.ColorSpace.Srgb
        | DisplayP3 -> FS.Skia.UI.ColorSpace.DisplayP3
        | AdobeRgb -> FS.Skia.UI.ColorSpace.AdobeRgb

    and private toLegacyPerspective (transform: PerspectiveTransform) : FS.Skia.UI.PerspectiveTransform =
        { M11 = transform.M11
          M12 = transform.M12
          M13 = transform.M13
          M21 = transform.M21
          M22 = transform.M22
          M23 = transform.M23
          M31 = transform.M31
          M32 = transform.M32
          M33 = transform.M33 }

    and private toLegacyFontSpec (font: FontSpec) : FS.Skia.UI.FontSpec =
        { Family = font.Family
          Size = font.Size
          Weight = font.Weight }

    and private toLegacyTextRun (run: TextRun) : FS.Skia.UI.TextRun =
        { Text = run.Text
          Position = toLegacyPoint run.Position
          Font = toLegacyFontSpec run.Font
          Paint = toLegacyPaint run.Paint }

    and private toLegacyVertex (vertex: Vertex) : FS.Skia.UI.Vertex =
        { Position = toLegacyPoint vertex.Position
          Color = vertex.Color |> Option.map toLegacyColor }

    and private toLegacyVertexMode mode : FS.Skia.UI.VertexMode =
        match mode with
        | Triangles -> FS.Skia.UI.VertexMode.Triangles
        | TriangleStrip -> FS.Skia.UI.VertexMode.TriangleStrip
        | TriangleFan -> FS.Skia.UI.VertexMode.TriangleFan

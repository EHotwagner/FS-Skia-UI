#r "nuget: FS.Skia.UI.Scene, 0.1.35-preview.1"

open FS.Skia.UI.Scene

let bounds: Rect = { X = 0.0; Y = 0.0; Width = 320.0; Height = 180.0 }
let stroke: Stroke = { Width = 2.0; Cap = StrokeCap.Round; Join = StrokeJoin.RoundJoin; Miter = 4.0 }
let paint: Paint =
    { Fill = Some { Red = 32uy; Green = 96uy; Blue = 160uy; Alpha = 255uy }
      Stroke = Some stroke
      Opacity = 1.0
      Antialias = true
      BlendMode = BlendMode.SrcOver
      Shader = None
      ColorFilter = ColorFilter.NoColorFilter
      MaskFilter = MaskFilter.NoMaskFilter
      ImageFilter = ImageFilter.NoImageFilter
      PathEffect = PathEffect.NoPathEffect }
let textRun: TextRun =
    { Text = "Hello"
      Position = { X = 16.0; Y = 32.0 }
      Font = { Family = Some "Inter"; Size = 14.0; Weight = Some 400 }
      Paint = paint }
let kind = SceneElementKind.RectangleElement

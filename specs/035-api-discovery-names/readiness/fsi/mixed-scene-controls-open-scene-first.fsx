#r "nuget: FS.Skia.UI.Scene, 0.1.35-preview.1"
#r "nuget: FS.Skia.UI.Controls, 0.1.34-preview.1"

open FS.Skia.UI.Scene
open FS.Skia.UI.Controls

type Msg =
    | Changed of string

let sceneBounds: FS.Skia.UI.Scene.Rect =
    { X = 0.0
      Y = 0.0
      Width = 240.0
      Height = 80.0 }

let scenePaint: FS.Skia.UI.Scene.Paint =
    { Fill = Some { Red = 24uy; Green = 88uy; Blue = 128uy; Alpha = 255uy }
      Stroke = None
      Opacity = 1.0
      Antialias = true
      BlendMode = FS.Skia.UI.Scene.BlendMode.SrcOver
      Shader = None
      ColorFilter = FS.Skia.UI.Scene.ColorFilter.NoColorFilter
      MaskFilter = FS.Skia.UI.Scene.MaskFilter.NoMaskFilter
      ImageFilter = FS.Skia.UI.Scene.ImageFilter.NoImageFilter
      PathEffect = FS.Skia.UI.Scene.PathEffect.NoPathEffect }

let control: FS.Skia.UI.Controls.Control<Msg> =
    FS.Skia.UI.Controls.Stack.create [
        FS.Skia.UI.Controls.Stack.children [
            FS.Skia.UI.Controls.TextBlock.create [
                FS.Skia.UI.Controls.TextBlock.text "Scene first"
            ]
            FS.Skia.UI.Controls.TextBox.create [
                FS.Skia.UI.Controls.TextBox.onChanged Changed
            ]
        ]
    ]

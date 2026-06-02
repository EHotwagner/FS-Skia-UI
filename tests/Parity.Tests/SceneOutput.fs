module ParitySceneOutput

// Feature 048 (US1, FR-004): a deterministic, environment-independent scene-output
// encoder over the CURRENT HOST's `Scene` values. The monolith host's `Scene` is opaque;
// the host exposes deterministic introspection (`Scene.describe`, `Scene.diagnostics`,
// `Scene.renderReadbackEvidence` with its `DeterministicHash`). The encoding is fixed and
// versioned with the fixtures (header `format-version`), uses stable ordering, and emits
// no timestamps or environment-dependent fields, so re-derivation is byte-identical
// (SC-003). It reads the host's scene values and writes text — no runtime behaviour
// (FR-010/SC-007).
//
// The three seed scenes reproduce the deterministic, non-interactive initial `view
// (init())` of the host galleries (`BasicViewer`, `EffectsGallery`, `ScreenshotGallery`),
// built through the host's public `Scene` vocabulary.

open System.Text
open FS.Skia.UI

let formatVersion = "scene-output/v1"

// --- seed scenes (mirror the galleries' deterministic initial view) ---

let private basicViewerScene () =
    let title = "Basic Vulkan Viewer"
    let values = [ 2.0; 4.0; 3.0; 8.0; 5.0 ]
    let red = Colors.rgba 220uy 64uy 52uy 230uy
    let blue = Colors.rgba 52uy 112uy 220uy 255uy
    let green = Colors.rgba 68uy 168uy 96uy 255uy

    let effectPaint =
        Paint.fill red
        |> Paint.withShader (LinearGradient({ X = 32.0; Y = 150.0 }, { X = 260.0; Y = 230.0 }, [ red; blue; green ]))
        |> Paint.withColorFilter (BlendColor(Colors.rgba 255uy 255uy 255uy 48uy, Screen))
        |> Paint.withMaskFilter (Blur 0.5)
        |> Paint.withImageFilter (DropShadow(4.0, 4.0, 3.0, Colors.rgba 0uy 0uy 0uy 120uy))
        |> Paint.withPathEffect (Corner 6.0)

    let strokePaint =
        Paint.stroke (Colors.rgba 248uy 220uy 92uy 255uy) 3.0
        |> Paint.withStrokeCap Round
        |> Paint.withStrokeJoin RoundJoin
        |> Paint.withPathEffect (Dash([ 8.0; 4.0 ], 0.0))

    let samplePath =
        Path.create EvenOdd [
            Path.moveTo 42.0 168.0
            Path.lineTo 148.0 150.0
            Path.quadTo { X = 230.0; Y = 178.0 } { X = 190.0; Y = 232.0 }
            Path.lineTo 64.0 232.0
            Path.close
        ]

    let perspective =
        { M11 = 1.0; M12 = 0.08; M13 = 0.0
          M21 = -0.04; M22 = 1.0; M23 = 0.0
          M31 = 0.0002; M32 = 0.0002; M33 = 1.0 }

    Scene.group [
        Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgba 18uy 24uy 32uy 255uy)
        Scene.rectangle (32.0, 32.0, 220.0, 96.0) (Colors.rgba 36uy 118uy 160uy 255uy)
        Scene.text (48.0, 88.0) title Colors.white
        Scene.clipped
            (RectClip { X = 28.0; Y = 146.0; Width = 240.0; Height = 112.0 })
            (Scene.group [
                Scene.path samplePath effectPaint
                Scene.line { X = 44.0; Y = 244.0 } { X = 250.0; Y = 158.0 } strokePaint
                Scene.withPerspective perspective (Scene.rectangleWithPaint { X = 184.0; Y = 174.0; Width = 42.0; Height = 42.0 } effectPaint)
            ])
        Scene.image (280.0, 40.0, 96.0, 96.0) "assets/sample.png"
        Scene.chart values
    ]

let private effectsGalleryScene () =
    let title = "Effects Gallery"
    let red = Colors.rgba 220uy 64uy 52uy 230uy
    let blue = Colors.rgba 52uy 112uy 220uy 255uy
    let green = Colors.rgba 68uy 168uy 96uy 255uy

    let gradientPaint =
        Paint.fill red
        |> Paint.withShader (LinearGradient({ X = 44.0; Y = 112.0 }, { X = 266.0; Y = 220.0 }, [ red; blue; green ]))
        |> Paint.withBlendMode SrcOver
        |> Paint.withColorFilter (BlendColor(Colors.rgba 255uy 255uy 255uy 48uy, Screen))
        |> Paint.withMaskFilter (Blur 0.75)
        |> Paint.withImageFilter (DropShadow(5.0, 5.0, 4.0, Colors.rgba 0uy 0uy 0uy 150uy))

    let radialPaint =
        Paint.fill blue
        |> Paint.withShader (RadialGradient({ X = 430.0; Y = 174.0 }, 72.0, [ Colors.white; blue; green ]))
        |> Paint.withOpacity 0.92

    let dashedPaint =
        Paint.stroke (Colors.rgba 248uy 220uy 92uy 255uy) 4.0
        |> Paint.withStrokeCap Round
        |> Paint.withStrokeJoin RoundJoin
        |> Paint.withPathEffect (Dash([ 10.0; 5.0; 3.0; 5.0 ], 0.0))

    let cornerPaint =
        Paint.stroke green 8.0
        |> Paint.withPathEffect (Corner 18.0)
        |> Paint.withBlendMode Screen

    let path =
        Path.create EvenOdd [
            Path.moveTo 54.0 156.0
            Path.lineTo 168.0 108.0
            Path.cubicTo { X = 248.0; Y = 116.0 } { X = 276.0; Y = 214.0 } { X = 186.0; Y = 256.0 }
            Path.lineTo 74.0 238.0
            Path.close
        ]

    let perspective =
        { M11 = 1.0; M12 = 0.12; M13 = 0.0
          M21 = -0.08; M22 = 1.0; M23 = 0.0
          M31 = 0.00025; M32 = 0.00018; M33 = 1.0 }

    Scene.group [
        Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgba 16uy 20uy 28uy 255uy)
        Scene.text (38.0, 66.0) title Colors.white
        Scene.path path gradientPaint
        Scene.ellipse { X = 350.0; Y = 104.0; Width = 160.0; Height = 128.0 } radialPaint
        Scene.line { X = 52.0; Y = 312.0 } { X = 570.0; Y = 312.0 } dashedPaint
        Scene.arc { X = 404.0; Y = 250.0; Width = 118.0; Height = 118.0 } 15.0 290.0 cornerPaint
        Scene.clipped
            (PathClip
                (Path.create Winding [
                    Path.moveTo 68.0 348.0
                    Path.lineTo 284.0 348.0
                    Path.lineTo 260.0 420.0
                    Path.lineTo 88.0 420.0
                    Path.close
                ]))
            (Scene.group [
                Scene.rectangleWithPaint { X = 58.0; Y = 338.0; Width = 236.0; Height = 96.0 } gradientPaint
                Scene.textRun
                    { Text = "clip + text"
                      Position = { X = 94.0; Y = 394.0 }
                      Font = { Family = None; Size = 24.0; Weight = Some 600 }
                      Paint = Paint.fill Colors.white }
            ])
        Scene.withPerspective perspective (Scene.rectangleWithPaint { X = 516.0; Y = 132.0; Width = 70.0; Height = 70.0 } gradientPaint)
        Scene.withColorSpace DisplayP3 (Scene.rectangleWithPaint { X = 536.0; Y = 376.0; Width = 50.0; Height = 50.0 } radialPaint)
    ]

let private screenshotGalleryScene () =
    // init() deterministic state: Size = 760x460, Frame = 0, Recovering = false,
    // LastScreenshot = None, Diagnostics = [].
    let width = 760.0
    let height = 460.0
    let frameOffset = 0.0
    let stateColor = Colors.rgba 64uy 168uy 126uy 255uy
    let screenshotLabel = "none"

    Scene.group [
        Scene.rectangle (0.0, 0.0, width, height) (Colors.rgba 18uy 22uy 30uy 255uy)
        Scene.rectangle (36.0, 42.0, width - 72.0, 92.0) (Colors.rgba 42uy 78uy 118uy 255uy)
        Scene.rectangle (64.0 + frameOffset, 172.0, 180.0, 72.0) stateColor
        Scene.text (56.0, 96.0) "Screenshot Gallery" Colors.white
        Scene.text (56.0, 286.0) "frame: 0" Colors.white
        Scene.text (56.0, 326.0) (sprintf "last screenshot: %s" screenshotLabel) Colors.white
        Scene.text (56.0, 366.0) "diagnostics: 0" Colors.white
    ]

/// The fixed, closed seed set: (seed id, output size, scene thunk). Adding/removing a seed
/// is a reviewed fixture change (data-model.md "Parity oracle").
let seeds: (string * Size * (unit -> Scene)) list =
    [ "basic-viewer", { Width = 640; Height = 480 }, basicViewerScene
      "effects-gallery", { Width = 640; Height = 480 }, effectsGalleryScene
      "screenshot-gallery", { Width = 760; Height = 460 }, screenshotGalleryScene ]

// --- deterministic encoder ---

let encode (seedId: string) (size: Size) (scene: Scene) =
    let sb = StringBuilder()
    let line (s: string) = sb.Append(s).Append('\n') |> ignore

    line (sprintf "format: %s" formatVersion)
    line (sprintf "seed: %s" seedId)
    line (sprintf "size: %dx%d" size.Width size.Height)

    line "elements:"
    for kind in Scene.describe scene do
        line (sprintf "  %A" kind)

    let diagnostics = Scene.diagnostics scene
    line (sprintf "diagnostics: %d" (List.length diagnostics))
    for d in diagnostics do
        line (sprintf "  %A/%A %s" d.Severity d.Stage d.Message)

    let readback = Scene.renderReadbackEvidence size scene
    line "readback:"
    line (sprintf "  capability-count: %d" readback.CapabilityCount)
    line "  capabilities:"
    for cap in readback.Capabilities do
        line (sprintf "    %s" cap)
    line (sprintf "  deterministic-hash: %s" readback.DeterministicHash)

    sb.ToString()

/// Encode one seed by id (re-runs the host scene construction + encoding).
let encodeSeed (seedId: string, size: Size, sceneThunk: unit -> Scene) =
    encode seedId size (sceneThunk ())

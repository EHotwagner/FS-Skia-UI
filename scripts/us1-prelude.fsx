#r "../src/Lib/bin/Debug/net10.0/FS.Skia.UI.dll"
#r "../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../src/SkiaViewer/bin/Debug/net10.0/FS.Skia.UI.SkiaViewer.dll"

open FS.Skia.UI
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer.Host

let red = Colors.rgba 220uy 64uy 52uy 255uy
let blue = Colors.rgba 52uy 112uy 220uy 255uy

let paint =
    Paint.stroke blue 2.0
    |> Paint.withBlendMode Multiply
    |> Paint.withStrokeCap Round
    |> Paint.withPathEffect (Dash([ 2.0; 2.0 ], 0.0))

let path =
    Path.create EvenOdd [
        Path.moveTo 0.0 0.0
        Path.lineTo 16.0 0.0
        Path.quadTo { X = 20.0; Y = 8.0 } { X = 16.0; Y = 16.0 }
        Path.close
    ]

let scene =
    Scene.group [
        Scene.rectangle (0.0, 0.0, 24.0, 24.0) red
        Scene.path path paint
        Scene.vertices Triangles [
            { Position = { X = 0.0; Y = 0.0 }; Color = Some red }
            { Position = { X = 12.0; Y = 0.0 }; Color = Some blue }
            { Position = { X = 0.0; Y = 12.0 }; Color = None }
        ] paint
        Scene.arc { X = 2.0; Y = 2.0; Width = 20.0; Height = 20.0 } 0.0 180.0 paint
        Scene.clipped (RectClip { X = 0.0; Y = 0.0; Width = 32.0; Height = 32.0 }) (Scene.text (4.0, 16.0) "clip" red)
        Scene.region { Bounds = [ { X = 2.0; Y = 2.0; Width = 8.0; Height = 8.0 } ]; Operation = RegionUnion } paint
        Scene.withColorSpace DisplayP3 (Scene.rectangle (1.0, 1.0, 3.0, 3.0) blue)
    ]

let readback =
    Scene.renderReadbackEvidence { Width = 128; Height = 96 } scene

printfn "us1-kinds=%A" (Scene.describe scene)
printfn "us1-path-length=%.3f" (Path.measure path).Length
printfn "us1-readback=%s capabilities=%d" readback.DeterministicHash readback.CapabilityCount

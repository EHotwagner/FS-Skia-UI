// US6 (FR-010, SC-006) scene-constructor fixture.
//
// The existing positional constructors keep working AND the new self-describing,
// Rect/Point-based forms exist. Before the additive change, `Scene.filledRectangle`
// and `Scene.textAt` do not exist, so this fails to compile (the failing-first
// state). After the change, both the positional and the self-describing forms
// compile — no removals.

#r "../../../../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"

open FS.Skia.UI.Scene

let red: Color = { Red = 220uy; Green = 40uy; Blue = 40uy; Alpha = 255uy }

// --- Existing positional forms (must keep compiling) ---
let positionalRect: Scene = Scene.rectangle (10.0, 20.0, 100.0, 40.0) red
let positionalText: Scene = Scene.text (12.0, 24.0) "score" red
let positionalDuRect: SceneNode = Rectangle((10.0, 20.0, 100.0, 40.0), red)
let positionalDuText: SceneNode = Text((12.0, 24.0), "score", red)

// --- New self-describing forms (added by FR-010) ---
// Rect-based rectangle, parallel to the existing `Scene.filledEllipse: Rect -> Color -> Scene`.
let selfDescribingRect: Scene =
    Scene.filledRectangle { X = 10.0; Y = 20.0; Width = 100.0; Height = 40.0 } red
// Point-based text, parallel to `Scene.circle: center: Point -> ...`.
let selfDescribingText: Scene =
    Scene.textAt { X = 12.0; Y = 24.0 } "score" red

printfn "us6 fixture compiled: %A %A %A %A %A %A"
    positionalRect positionalText positionalDuRect positionalDuText selfDescribingRect selfDescribingText

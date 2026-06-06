module ParityAnimationOutput

// Feature 073 (US3, FR-009, SC-003): a deterministic, value-aware encoder over a
// sampled animation frame. The repository's `SceneEvidence.render` hash is derived
// from element KINDS + size only (it is value-insensitive by design), so it proves
// the structural settled≡static / un-animated parity but cannot distinguish two
// frames that differ only in opacity / transform VALUES. This encoder fills that
// gap: it walks the sampled `SceneNode` and emits the animated property values
// (folded opacity / alpha and the lowered transform matrix) at fixed precision with
// stable ordering and no environment-dependent fields, so re-derivation is
// byte-identical in the same process and a fresh process. It is the value-aware
// progression oracle that complements the kind-hash.

open System
open System.Text
open FS.Skia.UI.Scene

let formatVersion = "animation-output/v1"
let outputSize: Size = { Width = 320; Height = 240 }

// --- reference target scene: a single-node panel (a Group), so the settled
//     animation unwraps byte-identically to the static render. ---

let panelScene () : Scene =
    let panelPaint =
        Paint.fill (Colors.rgba 36uy 118uy 160uy 255uy)

    Scene.group
        [ Scene.rectangleWithPaint { X = 16.0; Y = 16.0; Width = 288.0; Height = 120.0 } panelPaint
          Scene.textAt { X = 32.0; Y = 64.0 } "Panel" Colors.white ]

// --- reference animations ---

/// Story 1 entrance: opacity 0 → 1 and slide up 24px → 0 over 300 ms, ease-out.
let entrance: Animation =
    { Animation.empty with
        Opacity = Some { Start = 0.0; End = 1.0; Duration = TimeSpan.FromMilliseconds 300.0; Easing = EaseOut }
        Transform =
            Some
                { Start = { Transform.identity with TranslateY = 24.0 }
                  End = Transform.identity
                  Duration = TimeSpan.FromMilliseconds 300.0
                  Easing = EaseOut } }

/// Story 2 glide expressed as data: opacity 0.25 → 1 and scale-up 0.8 → 1 over
/// 200 ms, ease-in-out. Distinct from `entrance` so concurrent sampling shows no
/// interference.
let glide: Animation =
    { Animation.empty with
        Opacity = Some { Start = 0.25; End = 1.0; Duration = TimeSpan.FromMilliseconds 200.0; Easing = EaseInOut }
        Transform =
            Some
                { Start = { Transform.identity with ScaleX = 0.8; ScaleY = 0.8 }
                  End = Transform.identity
                  Duration = TimeSpan.FromMilliseconds 200.0
                  Easing = EaseInOut } }

/// (label, animation, elapsed) sample points captured as goldens. Each covers
/// start / midpoint / settle of its animation.
let samples: (string * Animation * TimeSpan) list =
    [ "entrance-start", entrance, TimeSpan.Zero
      "entrance-mid", entrance, TimeSpan.FromMilliseconds 150.0
      "entrance-settled", entrance, TimeSpan.FromMilliseconds 300.0
      "glide-start", glide, TimeSpan.Zero
      "glide-mid", glide, TimeSpan.FromMilliseconds 100.0
      "glide-settled", glide, TimeSpan.FromMilliseconds 200.0 ]

// --- deterministic value-aware encoder ---

let private f (v: float) = sprintf "%.4f" v // F# %f is InvariantCulture

let private encodeColor (c: Color) =
    sprintf "rgba(%d,%d,%d,%d)" (int c.Red) (int c.Green) (int c.Blue) (int c.Alpha)

let private encodePaint (p: Paint) =
    let fill = p.Fill |> Option.map encodeColor |> Option.defaultValue "none"
    sprintf "paint(opacity=%s,fill=%s)" (f p.Opacity) fill

let rec private encodeScene (indent: string) (scene: Scene) (sb: StringBuilder) =
    for node in scene.Nodes do
        encodeNode indent node sb

and private encodeNode (indent: string) (node: SceneNode) (sb: StringBuilder) =
    let line (s: string) =
        sb.Append(indent).Append(s).Append('\n') |> ignore

    let child = indent + "  "

    match node with
    | Group scenes ->
        line "group"
        for s in scenes do
            encodeScene child s sb
    | PaintedRectangle(rect, paint) ->
        line (sprintf "painted-rect x=%s y=%s w=%s h=%s %s" (f rect.X) (f rect.Y) (f rect.Width) (f rect.Height) (encodePaint paint))
    | Rectangle((x, y, w, h), color) -> line (sprintf "rect x=%s y=%s w=%s h=%s %s" (f x) (f y) (f w) (f h) (encodeColor color))
    | Text((x, y), text, color) -> line (sprintf "text x=%s y=%s %s %s" (f x) (f y) text (encodeColor color))
    | PerspectiveNode(m, scene) ->
        line (sprintf "perspective m11=%s m12=%s m13=%s m21=%s m22=%s m23=%s" (f m.M11) (f m.M12) (f m.M13) (f m.M21) (f m.M22) (f m.M23))
        encodeScene child scene sb
    | other ->
        // Fixtures here only use the cases above; fall back to the structural
        // kind label so an unexpected node is still encoded deterministically.
        line (sprintf "node:%A" (Scene.describe { Nodes = [ other ] }))

/// Encode one sampled frame deterministically (value-aware).
let encodeFrame (label: string) (animation: Animation) (elapsed: TimeSpan) : string =
    let node = Animation.applyAt elapsed animation (panelScene ())
    let sb = StringBuilder()
    sb.Append(sprintf "format: %s\n" formatVersion) |> ignore
    sb.Append(sprintf "sample: %s\n" label) |> ignore
    sb.Append(sprintf "elapsed-ms: %d\n" (int elapsed.TotalMilliseconds)) |> ignore
    sb.Append("frame:\n") |> ignore
    encodeScene "  " { Nodes = [ node ] } sb
    sb.ToString()

let encodeSample (label: string, animation: Animation, elapsed: TimeSpan) = encodeFrame label animation elapsed

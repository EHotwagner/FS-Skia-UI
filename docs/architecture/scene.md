---
title: Scene
category: Architecture
categoryindex: 3
index: 2
description: The Scene subsystem — the immutable scene primitives and drawing vocabulary that the renderer consumes.
---

# Scene

`FS.Skia.UI.Scene` is the framework's **drawing vocabulary**: a small, immutable,
data-only description of *what* to draw, with no knowledge of *how* it gets drawn.
A `Scene` is a tree of `SceneNode` values — rectangles, circles, paths, text,
images, clips, gradients, transforms — built with pure constructor functions and
consumed by the [SkiaViewer host](./host-skiaviewer.html), which is the only place
those nodes turn into Skia draw calls. Keeping the vocabulary in its own package
with no rendering dependency is what lets a `view : 'model -> Scene` function be a
pure value you can build, inspect, hash, and diff in tests without ever touching a
GPU. Per [ADR 0008](../adr/0008-scene-vocabulary-single-source.md), this package is
the **single canonical source** of the scene vocabulary — there is exactly one
`Scene` type, retyped throughout the host with no conversion shim.

See the API reference for the full surface:
[`FS.Skia.UI.Scene`](../reference/fs-skia-ui-scene.html),
[`Scene`](../reference/fs-skia-ui-scene-scene.html), and the
[reference index](../reference/index.html).

## The core types

A scene is built from three mutually-recursive types:

```fsharp
type SceneNode =
    | Empty
    | Group of Scene list
    | Rectangle of (float * float * float * float) * Color
    | PaintedRectangle of Rect * Paint
    | Circle of center: Point * radius: float * fill: Color
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
    // … (the full set is exhaustive in the .fsi)

and Scene = { Nodes: SceneNode list }

and Picture = { Name: string; Scene: Scene }
```

`SceneNode` is the unit of drawing; `Scene` is just an ordered list of nodes
(painted back-to-front); `Picture` is a named, reusable sub-scene. Several nodes —
`Group`, `ClipNode`, `ColorSpaceNode`, `PerspectiveNode`, `PictureNode` — nest a
child `Scene`, so the whole structure is a tree.

Around those sit the supporting value types, all plain records and unions:

- **Geometry:** `Size`, `Point`, `Rect`, and the `PathSpec` / `PathCommand` family
  (`MoveTo`, `LineTo`, `QuadTo`, `CubicTo`, `ArcTo`, `Close`) with a `PathFillType`.
- **Appearance:** `Color`, `Paint` (fill, stroke, opacity, antialias, blend mode,
  plus optional `Shader`, `ColorFilter`, `MaskFilter`, `ImageFilter`, `PathEffect`),
  and `Stroke`. Gradients live in `Shader` (`LinearGradient`, `RadialGradient`,
  `SweepGradient`).
- **Text:** `FontSpec`, `TextRun`, `TextMetrics`.

## The builder modules

You rarely construct `SceneNode` cases by hand. Five modules provide ergonomic,
self-describing constructors:

- **`Scene`** — the main builder: `empty`, `group`, `rectangle` /
  `filledRectangle` / `rectangleWithPaint`, `circle`, `ellipse` / `filledEllipse`,
  `line`, `path`, `points`, `vertices`, `arc`, `text` / `textAt`, `textRun`,
  `image`, `clipped`, `region`, `withColorSpace`, `withPerspective`, `picture`, and
  `chart`. Each returns a `Scene`, so they compose.
- **`Colors`** — `rgba`, `rgb`, and the `black` / `white` / `transparent` constants.
- **`Paint`** — `fill` and `stroke` plus a fluent `with…` family
  (`withOpacity`, `withBlendMode`, `withShader`, `withMaskFilter`, …).
- **`Path`** — path construction (`create`, `moveTo`, `lineTo`, `quadTo`,
  `cubicTo`, `close`) and pure geometry queries (`bounds`, `measure`, `segment`,
  `combine`).

Two of the `Scene` constructors deserve a note: `filledRectangle` (a `Rect`-based
sibling of the positional `rectangle`) and `textAt` (a `Point`-based sibling of the
positional `text`) exist specifically to avoid the silent arity slip that bare
`(float * float * float * float)` and `(float * float)` tuples invite — a small but
deliberate ergonomics-and-correctness choice baked into the public surface.

## Inspection without pixels

Because a scene is pure data, the package ships functions that interrogate it
*semantically*, which is how the framework's tests assert coverage without
comparing rendered images:

- `Scene.describe` returns the `SceneElementKind list` actually present.
- `Scene.diagnostics` returns `RenderDiagnostic list` for invalid inputs.
- `Scene.renderReadbackEvidence` produces a deterministic capability list + hash.
- `Scene.circleEvidence` / `Scene.ellipseEvidence` and the `LayoutEvidence` module
  classify whether shapes and HUD/gameplay regions sit inside the output bounds
  (`ShapePlacement`, `LayoutOverlapStatus`) — the data behind the
  [layout-readability](./layout.html) checks.
- `SceneEvidence.render` / `renderHash` / `renderPng` produce deterministic
  evidence in `Hash`, `Metadata`, or `Png` form, returning
  `Result<_, SceneEvidenceFailure>` so an unsupported environment is distinguished
  from a product defect.

## Animation: motion as data

`Animation.fsi` adds a bounded, **additive** motion slice on top of the static
vocabulary (feature 073). The design rule is that sampling is a *pure function of an
explicit `TimeSpan`* — identical inputs and identical time samples always produce
byte-identical output, and the framework owns no hidden mutable animation registry.

- An `Animation` declares optional `Tween`s over opacity, an affine `Transform`,
  and/or color; each `Tween<'a>` carries `Start`, `End`, `Duration`, and an
  `Easing` curve (`Linear`, `EaseIn`, `EaseOut`, `EaseInOut`).
- `Animation.applyAt elapsed animation target` samples the animation at a time and
  produces a `SceneNode`. The **identity-at-rest** rule (R5) is the key trick: when
  the sampled opacity is `1.0` and the sampled transform is identity, it returns the
  target scene's node *unwrapped* — byte-identical to the static render — so a
  settled animation costs nothing and proves equal to the still frame. A
  non-identity transform lowers to a `PerspectiveNode` (reusing the existing 3×3
  transform node rather than inventing a new primitive).
- `AnimationState<'a>` is the stateful, author-held value for interactive
  retargeting (`create`, `advance`, `retarget`, `value`, `isActive`). All
  transitions are pure; `retarget` continues from the currently displayed value
  rather than snapping back. Because a namespace cannot hold a bare value, the
  per-`'a` interpolants live as `Animation.lerpFloat` / `Animation.lerpColor` and
  `Transform.lerp`, and `AnimationState` stores the chosen interpolant in its
  `Interp` field.

## How it fits the rest of the framework

- **The host consumes it.** `SceneRenderer.paintNode` in the
  [SkiaViewer host](./host-skiaviewer.html) is the single, **exhaustive** `match`
  over every `SceneNode` case — no wildcard — so adding a primitive is a compile
  error until both the interactive and screenshot render paths handle it. Scene
  defines vocabulary; the host defines pixels.
- **`view` produces it.** In the [Elmish/MVU runtime](./elmish-mvu.html), the
  application's `view : 'model -> Scene` is the bridge from pure model to drawing;
  the host calls it each frame.
- **It depends on nothing visual.** `FS.Skia.UI.Scene` has no Skia or Vulkan
  dependency, which is exactly what makes it the safe, single canonical vocabulary
  mandated by [ADR 0008](../adr/0008-scene-vocabulary-single-source.md).

## Analysis

### Implementation strengths

- The renderer's `match` over `SceneNode` is exhaustive with **no wildcard**, so
  every primitive added to this package is forced into both the interactive and the
  screenshot render paths by the compiler — coverage is mechanically guaranteed,
  not remembered.
- Animation sampling is a pure function of an explicit `TimeSpan` with pinned easing
  endpoints and an identity-at-rest lowering, so a settled animation is provably
  byte-identical to the static frame and deterministic evidence (`sampleFrames`) is
  trivial to capture.
- The package gives tests real semantic hooks (`describe`, `diagnostics`,
  `renderReadbackEvidence`, the `LayoutEvidence` placement classifiers), so behaviour
  can be asserted on data structure rather than on fragile pixel comparison.
- Self-describing constructors (`filledRectangle`, `textAt`) and the `Paint` `with…`
  combinators make correct authoring the path of least resistance and head off the
  positional-tuple arity slips the raw union cases would otherwise allow.

### Implementation weaknesses

- `SceneNode` carries redundant, overlapping cases — `Rectangle` (positional tuple
  + `Color`) versus `PaintedRectangle` (`Rect` + `Paint`), and `Circle` /
  `FilledEllipse` versus the `Paint`-based `Ellipse` — so the same visual result has
  several spellings, enlarging the surface every consumer and the renderer must
  handle.
- A few primitives are special-purpose rather than general: `Chart` bakes fixed
  pixel offsets (chart left/top/height, bar width) directly into the renderer, which
  is convenient for one use case but an odd citizen in a general drawing vocabulary.
- Some appearance fields are silently ignored or only partially honoured by the
  renderer (e.g. `ColorSpaceNode` is drawn through without applying the color space),
  so the data model promises more than the painter currently delivers.
- The `AnimationState<'a>` record stores its interpolant as a function-valued
  `Interp` field, which keeps the `advance`/`value` signatures clean but means the
  state value is not a plain serializable record and carries a closure.

### Design pros

- A single canonical `Scene` type with no conversion shim (ADR 0008) eliminates the
  duplicate-vocabulary problem at the root: there is one source of truth, and the
  host is retyped directly onto it.
- Making drawing pure data with zero rendering dependency is the keystone of the
  whole architecture — `view` is a pure value, scenes are inspectable, hashable, and
  diffable, and the GPU is never needed to reason about *what* will be drawn.
- The vocabulary is deliberately small and immutable, which keeps it easy to learn,
  cheap to construct, and safe to share structurally across frames (important for
  the diff/reconciliation and animation paths).
- Animation is an additive layer that reuses existing nodes (a non-identity
  transform lowers to the existing `PerspectiveNode`) rather than a parallel motion
  system, so motion and static rendering share one lowering and one renderer.

### Design cons

- The flat `SceneNode` union with multiple ways to express the same shape trades
  type-level guidance for breadth; without the builder modules' guidance it is easy
  to pick a less-capable case (e.g. `Rectangle` when you wanted `Paint`).
- A `Scene` is an unindexed `SceneNode list` with no identity or keying at this
  layer, so anything wanting stable node identity across frames (diffing, hit
  testing) must impose it above the vocabulary rather than read it from the model.
- The package mixes pure drawing vocabulary with a sizeable body of
  evidence/layout-classification types (`LayoutEvidenceReport`, `SceneEvidence…`),
  which serve this project's governance needs but broaden what is nominally a
  "drawing primitives" package.
- Because the renderer — not the vocabulary — decides what each node actually
  produces, the `Scene` types alone do not fully specify rendered output; the real
  contract is split between this package's data and the host's painter.

---
name: fs-skia-scene
description: Build pure scene descriptions in a generated FS.Skia.UI product.
---

# Scene Capability

## Scope

Use this skill for product code that builds pure `Scene` / `SceneNode`
descriptions: HUD regions, gameplay geometry, markers, and text. Scene values are
plain data — they perform no window, render, or screenshot I/O themselves.

## Public Contract

The signatures you consume are bundled with this product at
`docs/api-surface/Scene/Scene.fsi`. Read them to confirm any union case's exact
field order locally — no DLL reflection needed. Prefer the self-describing
constructors (`Scene.filledRectangle`, `Scene.textAt`, `Scene.circle`) over the
positional tuple cases to avoid an arity slip.

## Usage

```fsharp
open FS.Skia.UI.Scene

let panel = { Red = 40uy; Green = 90uy; Blue = 200uy; Alpha = 255uy }
let ink = { Red = 255uy; Green = 255uy; Blue = 255uy; Alpha = 255uy }

// A pure scene: a HUD bar plus a label. No I/O happens here.
let hud : Scene =
    Scene.group
        [ Scene.filledRectangle { X = 0.0; Y = 0.0; Width = 320.0; Height = 48.0 } panel
          Scene.textAt { X = 12.0; Y = 30.0 } "tally: 0" ink ]
```

## Build Commands

Run `./fake.sh build -t Dev` then `./fake.sh build -t Verify` in this product.

## Test Commands

Run `./fake.sh build -t Test` to exercise product-owned scene examples.

## Evidence

Record scene and bounds evidence under this product's `readiness/` paths. Do not
copy framework readiness reports into the product.

## Package Boundary

Scene must not reference Elmish, the viewer host, layout, or widgets. Keep host
wiring in `fs-skia-skiaviewer` and control authoring in `fs-skia-ui-widgets`.

## Generated Product

Scene is the base capability in every profile; build product geometry from these
primitives and feed the resulting `SceneNode` to your `View`.

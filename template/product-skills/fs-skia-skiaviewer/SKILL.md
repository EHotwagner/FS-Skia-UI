---
name: fs-skia-skiaviewer
description: Wire a generated FS.Skia.UI product to the desktop viewer host.
---

# SkiaViewer Capability

## Scope

Use this skill for the host boundary of a generated product: opening the native
window, rendering scenes, routing keyboard input, advancing time, and
interpreting `ViewerEffect` values returned by your pure `update`.

## Public Contract

The signatures you consume are bundled with this product at
`docs/api-surface/SkiaViewer/SkiaViewer.fsi`. `Viewer.runApp` is the canonical
entry point and the only place that performs host-boundary I/O. See
`docs/effects-boundary.md` for the full effect-category description.

## Usage

```fsharp
open FS.Skia.UI.SkiaViewer

// Bundle your pure pieces into the host record.
let generatedHost =
    { Init = fun () -> initialModel, []   // initial model + startup effects
      Update = update                     // pure Msg -> Model -> Model * ViewerEffect list
      View = view                         // Model -> SceneNode
      MapKey = mapKey                     // ViewerKey -> bool -> Msg option
      Tick = tick                         // TimeSpan -> Msg option
      Diagnostics = Viewer.defaultDiagnostics }

match Viewer.runApp viewerOptions generatedHost with
| Ok _ -> 0          // window opened, scenes rendered, effects interpreted
| Error _ -> 1       // classified host/launch/verification failure
```

## Build Commands

Run `./fake.sh build -t Dev` then `./fake.sh build -t Verify` in this product.

## Test Commands

Run `./fake.sh build -t Test` for product host-wiring coverage.

## Evidence

Record window-visibility and screenshot evidence under this product's
`readiness/` paths. Do not copy framework readiness reports into the product.

## Package Boundary

Keep window, render, and screenshot I/O inside the `Viewer.runApp` interpreter.
Your `update` and `View` stay pure; never perform host I/O inside them.

## Generated Product

The app profile wires `Viewer.runApp viewerOptions generatedHost` as the default
launch path. Use `Viewer.runAppEvidence` with the same host for bounded evidence
runs.

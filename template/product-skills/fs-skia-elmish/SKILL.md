---
name: fs-skia-elmish
description: Drive a generated FS.Skia.UI product through the pure Elmish adapter.
---

# Elmish Capability

## Scope

Use this skill for the Elmish boundary of a generated product: wrapping your pure
user model/messages in the adapter so viewer messages and effects are threaded
through one pure `update`.

## Public Contract

The signatures you consume are bundled with this product at
`docs/api-surface/Elmish/Elmish.fsi`. `ElmishAdapter.init` and
`ElmishAdapter.update` are pure: they return the next model and a list of
requested effects as plain values, interpreted later at the host boundary.

## Usage

```fsharp
open FS.Skia.UI.Scene
open FS.Skia.UI.Elmish

// init wraps your user model and returns startup effects (values, not I/O).
let adapterModel, startupEffects =
    ElmishAdapter.init viewerOptions initialModel (view initialModel)

// update stays pure: next adapter model + requested effects.
let nextModel, effects =
    ElmishAdapter.update view (UserMsg productMsg) adapterModel
```

## Build Commands

Run `./fake.sh build -t Dev` then `./fake.sh build -t Verify` in this product.

## Test Commands

Run `./fake.sh build -t Test` to assert adapter transitions and effects.

## Evidence

Record transition and effect evidence under this product's `readiness/` paths. Do
not copy framework readiness reports into the product.

## Package Boundary

Keep `Model`, `Msg`, `Effect`, `init`, and `update` pure. Native viewer I/O
belongs to `fs-skia-skiaviewer` interpreter code, not the adapter.

## Generated Product

Products that select Elmish also receive Scene and SkiaViewer; wire the adapter
between your pure `update` and `Viewer.runApp`.

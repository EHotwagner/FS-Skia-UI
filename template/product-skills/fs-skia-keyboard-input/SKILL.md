---
name: fs-skia-keyboard-input
description: Map keyboard input to product commands in a generated FS.Skia.UI product.
---

# KeyboardInput Capability

## Scope

Use this skill for product keyboard handling: declaring bindings, normalizing raw
host keys to `ViewerKey`, and reducing key events to product commands through a
pure `Keyboard.update`.

## Public Contract

The signatures you consume are bundled with this product at
`docs/api-surface/KeyboardInput/KeyboardInput.fsi`. `Keyboard.init`/`update` are
pure and return `KeyboardEffect` values; `ViewerKeyboard.normalize` turns a raw
host key string into a typed `ViewerKey`.

## Usage

```fsharp
open FS.Skia.UI.KeyboardInput

// Declare product bindings and seed the pure keyboard model.
let bindings = [ { Key = "ArrowLeft"; Command = "move-left" }
                 { Key = "Space"; Command = "primary-action" } ]

let model, startupEffects = Keyboard.init bindings

// Turn a raw host key into a product Msg at your MapKey boundary.
let mapKey (key: ViewerKey) (isDown: bool) : Msg option =
    match key, isDown with
    | ArrowLeft, true -> Some MoveLeft
    | Space, true -> Some PrimaryAction
    | _ -> None
```

## Build Commands

Run `./fake.sh build -t Dev` then `./fake.sh build -t Verify` in this product.

## Test Commands

Run `./fake.sh build -t Test` to assert binding resolution and command effects.

## Evidence

Record keyboard command and state evidence under this product's `readiness/`
paths. Do not copy framework readiness reports into the product.

## Package Boundary

Keep key reduction pure; the host delivers raw key events and interprets
`RequestHostKeyCapture` through the viewer, not inside `Keyboard.update`.

## Generated Product

The app profile threads `mapKey` into `generatedHost` so the viewer routes input
through your pure reducer.

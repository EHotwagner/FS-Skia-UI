# Quickstart: pacing, quitting, and present modes in the live host

## Cap the live loop's frame rate (new)

```fsharp
open FS.Skia.UI.SkiaViewer

let options =
    { Title = "My App"
      InitialSize = Size(1280, 800)
      PresentMode = ViewerPresentMode.DirectToSwapchain   // live default
      FrameRateCap = Some 30 }                            // NEW: bound the loop to 30 FPS

// Or keep the default 60 by leaving the cap unset:
let defaultPaced = { options with FrameRateCap = None }
```

`FrameRateCap` bounds **both** the update and the render cadence of the native loop.
Combined with the already-shipped unchanged-frame paint-skip (feature 120,
`DirectToSwapchain`), a static page wakes at most `cap` times/second and does no draw
work per wake. `None` preserves the exact pre-121 behaviour (60). `Some n` with `n <= 0`
is rejected with a startup diagnostic.

> Environment note: on a host **without** a blocking compositor/vsync (e.g. a minimal
> headless GTK session), the loop cannot block on present and will free-run toward the
> cap regardless — that is an environment limitation, not a product defect. The frame-cap
> is the consumer-side lever to bound it.

## Quit gracefully from `update` (already available)

You do **not** need a new host effect to exit. Return the existing `CloseWindow`
`ViewerEffect` from your `update`:

```fsharp
let update msg model =
    match msg with
    | Quit -> model, [ ViewerEffect.CloseWindow ]   // graceful shutdown, no kill
    | _    -> model, []

// Wire a key to it (plain MapKey is the dependable path):
let mapKey key isDown =
    if isDown && key = ViewerKey.Q then Some Quit else None
```

The host interprets `CloseWindow` → `AppRequestedClose` → clean shutdown.

## Pick the right present mode

| Launch context | Present mode | Why |
|----------------|--------------|-----|
| Persistent interactive window | `DirectToSwapchain` | zero-readback live present; unchanged frames skip paint |
| Evidence / screenshot capture | `OffscreenReadback` | 640×480 readback for deterministic pixel capture |

**Do not** reuse your evidence `viewerOptions` (which set `OffscreenReadback`) for the
persistent interactive launch — that renders off-screen and shows a blank window. Give the
live launch its own `DirectToSwapchain` options (e.g. 1280×800).

## Discover the pointer surface

`PointerInteraction`, `PointerButton`, and `ViewerPointerPhaseKind` are published under
`docs/api-surface/` — consult them for the DU cases (`Click(control, button, x, y)`,
`DragBegin`, `DragCancelled of control option`, …) instead of reflecting over the
assemblies. Unrouted pointer interactions fall through to `host.MapPointer`.
</content>

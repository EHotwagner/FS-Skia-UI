---
name: fs-skia-viewer-host
description: Consumer-facing guide to hosting an interactive FS.Skia.UI app — the keyboard/pointer input surface, the preview-vs-tree render distinction, and the windowed-fullscreen blur caveat.
---

# FS Skia Viewer Host Capability

## Scope / when to use

Use this skill when authoring a **consumer** interactive host for an FS.Skia.UI app: wiring
keyboard and pointer input into an MVU loop, rendering a real nested control tree to a window,
and capturing visible-window evidence. This is the host-usage companion to the package-owned
`fs-skia-skiaviewer` skill (viewer host contracts) — it covers the **input surface** and the
**which-renderer** decisions a product makes, not the internal viewer plumbing.

## Public Contract (.fsi)

Two host front doors exist; pick by whether you need pointer routing:

- **`FS.Skia.UI.SkiaViewer.Viewer.runApp` + `GeneratedAppHost`** — keyboard + tick only;
  `View: 'model -> SceneNode`. Unchanged, durable (feature 084).
- **`FS.Skia.UI.Controls.Elmish.ControlsElmish.runInteractiveApp` + `InteractiveAppHost`**
  (feature 085) — adds a **pointer** seam and a **size-aware** view:

```fsharp
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish
open FS.Skia.UI.SkiaViewer

let host : InteractiveAppHost<Model, Msg> =
    { Init = fun () -> initModel, []
      Update = fun msg model -> update msg model       // pure: 'model -> 'model * ViewerEffect list
      View = fun (size: Size) model -> view size model // size-aware: returns Control<'msg>
      Theme = Theme.light
      MapKey = fun key isDown -> mapKey key isDown      // keyboard input surface
      MapPointer = fun interaction -> mapPointer interaction  // pointer input surface (Click/Drag/Scroll…)
      Tick = fun _ -> None
      Diagnostics = Viewer.defaultDiagnostics }

ControlsElmish.runInteractiveApp
    { Title = "App"
      InitialSize = { Width = 1024; Height = 768 }
      PresentMode = ViewerPresentMode.DirectToSwapchain   // live default (feature 119)
      FrameRateCap = None }                               // None = 60 FPS; Some n caps the loop (feature 121)
    host
```

### Input surface

- **Keyboard** flows through `MapKey: ViewerKey -> bool -> 'msg option` (same as `GeneratedAppHost`).
  Toolkit key names like `Number5`/`Digit5`/`Keypad5`/`Key5`/`KeyL` are normalized by
  `ViewerKeyboard.normalize` to `Digit n` / `Letter X` (feature 085); unknown names stay `Unknown raw`.
- **Pointer** flows through `MapPointer: PointerInteraction -> 'msg option`. `runInteractiveApp`
  hit-tests the rendered tree (`Control.renderTree` `Layout` × `EventBindings` by `ControlId`,
  with the shipped 4px click/drag fold via `Pointer.update`) and offers each `PointerInteraction`
  (`Click`/`PressedDown`/`ReleasedUp`/`DragBegin`/`Scroll`/…) to your `MapPointer`. Match on the
  `ControlId` to route the bound product message.

## Preview vs tree — `Control.render` is NOT `Control.renderTree`

This is the most common authoring mistake:

- **`Control.render theme control`** is the Feature-080 **single-control PREVIEW**: it flattens
  every descendant and stacks them at fixed y-offsets to show *one control's* faithful thumbnail.
  Use it for catalog/thumbnail rendering — **not** for a live app screen.
- **`Control.renderTree theme size control`** (feature 085) runs a **real recursive Yoga layout**
  at the output `size` and paints nested containers AND their children at their computed bounds.
  This is what an interactive host renders; `runInteractiveApp` uses it internally. Two
  structurally different trees produce visibly different scenes.

```fsharp
let live = Control.renderTree Theme.light { Width = 1024; Height = 768 } appTree  // real layout
let thumb = Control.render Theme.light singleControl                              // 080 preview
```

## Windowed-fullscreen blur caveat + workaround

The **no-flag default** is windowed fullscreen, which scales a fixed-resolution scene up to the
monitor work area and **blurs** it. Two fixes:

1. Use the **size-aware `View: Size -> 'model -> Control<'msg>`** so content is laid out to the
   actual swapchain extent (sharp at any size) — the preferred path.
2. Or pass exactly one flag — **`--window-startup normal`** — for a 1:1 normal window.

## Present mode: live vs evidence — do not reuse the evidence options (feature 121)

`ViewerOptions.PresentMode` picks the present mechanism; choose it by launch context:

| Launch context | `PresentMode` | Why |
|----------------|---------------|-----|
| Persistent interactive window | `DirectToSwapchain` | zero-readback live present (feature 119); unchanged frames skip paint (feature 120) |
| Evidence / screenshot capture | `OffscreenReadback` | small readback surface for deterministic pixel capture |

**Do NOT reuse your evidence `viewerOptions` for the persistent interactive launch.** The
evidence options set `OffscreenReadback` (640×480), which renders **off-screen** — a live window
built from them shows a **blank** frame. Give the interactive launch its own `DirectToSwapchain`
options (e.g. 1280×800). A generated scaffold that defaults the persistent launch to the evidence
`viewerOptions` ships a blank window — the exact defect the ControlsShowcase4 feedback hit.

## Loop pacing & the headless free-run environment limit (feature 121)

`ViewerOptions.FrameRateCap : int option` bounds the live persistent loop:

- `None` keeps the default **60 FPS** (the pre-121 behaviour).
- `Some n` (n > 0) caps **both** the update and the present cadence at `n` FPS — the consumer
  lever to keep the loop from free-running. `Some n` with `n <= 0` is rejected at startup.

**Desktop-session prerequisites for a responsive window**: a compositor with vsync. On such a
host the loop paces on present. On a **minimal/headless host with no compositor/vsync** (e.g. a
bare GTK session) the loop has nothing to block on and **free-runs toward the cap** — this is an
**environment limitation, not a product defect**. Lower the `FrameRateCap` to bound CPU there; a
truly responsive interactive window still requires a real desktop session.

## Already shipped — don't re-derive these (feature 121 reconciliation)

- **In-app quit needs no new effect.** `update` returns `ViewerEffect list`; return
  `[ ViewerEffect.CloseWindow ]` to request a graceful shutdown (it propagates to
  `AppRequestedClose` + `Shutdown`). Wire a plain key to it via `MapKey` — no `kill` required.
- **Unchanged frames already skip paint.** On `DirectToSwapchain`, a frame whose scene is
  reference/structurally unchanged at an unchanged size does no clear/scene-walk/draw (feature
  120). You do not need to add idle-skip yourself.

## Keyboard warm-up: no dropped keystrokes at focus (feature 086)

When a persistent window first gains focus there is a brief **warm-up window** before the
render pipeline signals ready (its first presented frame). Key events that fire in that gap
used to be dropped. The persistent host now buffers them in a **bounded pre-ready FIFO** and
flushes them **in capture order** the moment the pipeline is ready, so every keystroke issued
in the first seconds after focus still reaches your `MapKey`.

- The buffer is **bounded** (capacity 64). Past the cap it **drops the oldest** event and
  emits a structured `Input`/`Warning` diagnostic — explicit degradation, never silent loss
  (Principle VII). In practice the cap is never reached: the first frame arrives within a few
  frames of focus and the buffer drains.
- After the pipeline is ready, dispatch is **direct** (no buffering), and any residual
  buffered keys are drained in order before the live key.
- You do not opt in — it is automatic on the persistent host input path. This only closes the
  focus-time gap; there is no behavior change once the window is warm.

## Persistent problems

When a problem outlasts reasonable in-repo attempts, extensive external research is
**mandatory** — consult **official online docs first** (the F#/.NET docs and the driven
library's own documentation/API reference), then community sources (forums, Reddit, Q&A
sites, issue trackers and changelogs). Record the findings and resolving links in the
feature's `specs/<feature>/feedback/` folder and, for durable lessons, in this skill's
**Sources** line. Offline, the mandate degrades to recording "research blocked — <why>"
rather than hard-failing the phase.

## Related

- [[fs-skia-controls-host]] — the **maintainer-facing** counterpart: what `runInteractiveApp` does
  internally each frame (the retained structure, per-identity animation clocks, visual-state stamping,
  focus-first key routing). This skill is the consumer view; that one is the host-internals view.
- [[fs-skia-skiaviewer]] — the package-owned viewer host contracts this builds on.
- [[fs-skia-ui-widgets]] — building the `Control<'msg>` tree the host renders.
- [[fs-skia-elmish]] — the MVU wiring the interactive host follows.
- [[fs-skia-keyboard-input]] — `ViewerKeyboard.normalize` and the key-name families.

## Sources / links

- F#/.NET docs: https://learn.microsoft.com/en-us/dotnet/fsharp/
- SkiaSharp (the driven Skia rendering library): https://github.com/mono/SkiaSharp

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

ControlsElmish.runInteractiveApp { Title = "App"; InitialSize = { Width = 1024; Height = 768 } } host
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

## Persistent problems

When a problem outlasts reasonable in-repo attempts, extensive external research is
**mandatory** — consult **official online docs first** (the F#/.NET docs and the driven
library's own documentation/API reference), then community sources (forums, Reddit, Q&A
sites, issue trackers and changelogs). Record the findings and resolving links in the
feature's `specs/<feature>/feedback/` folder and, for durable lessons, in this skill's
**Sources** line. Offline, the mandate degrades to recording "research blocked — <why>"
rather than hard-failing the phase.

## Related

- [[fs-skia-skiaviewer]] — the package-owned viewer host contracts this builds on.
- [[fs-skia-ui-widgets]] — building the `Control<'msg>` tree the host renders.
- [[fs-skia-elmish]] — the MVU wiring the interactive host follows.
- [[fs-skia-keyboard-input]] — `ViewerKeyboard.normalize` and the key-name families.

## Sources / links

- F#/.NET docs: https://learn.microsoft.com/en-us/dotnet/fsharp/
- SkiaSharp (the driven Skia rendering library): https://github.com/mono/SkiaSharp

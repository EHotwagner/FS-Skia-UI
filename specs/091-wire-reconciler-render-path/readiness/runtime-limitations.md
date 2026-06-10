# Runtime Limitations — Feature 091

This feature changes the interactive host's **pointer/keyboard dispatch** and adds
a **responds-proof** capture over the existing render path. It reads existing
layout/hit-test/render data and introduces **no new rendering machinery**. A real
live Vulkan/Skia window **was launched** in this session via the production
`runInteractiveApp` path and presented the production render path
(`live-window-launch.md`); the responds-proof additionally captures the
input→visible-change headlessly so the dispatch evidence does not depend on a
display.

## Platform runtime envelope (unchanged by this feature)

- **.NET 10 desktop** is the supported host (`net10.0`, Windows + Linux desktop).
- Live windows render through **Vulkan** via the **SkiaSharp preview** native backend.
- **unsupported macOS/mobile/browser** — out of scope; no headed window path is
  validated there.
- **no software-renderer fallback** — a headless/over-SSH environment without a
  GPU/display cannot present a live window; this session HAS a display
  (`DISPLAY=:1`) and the live window opened (`live-window-launch.md`). The 090
  responds-proof additionally needs no live window — it is a **render-target-only**
  before/after capture over the production `Control.renderTree` path
  (`captureRespondsProof`) — so the dispatch evidence holds with or without a
  display. Dual Wayland/X11 sessions must force the X11 path to avoid the
  `libdecor-gtk` hazard (see `live-window-launch.md`).

## Unsupported-scope handling + failure diagnostics

- **FR-004a (`None` → `MapPointer` fallback).** When the nearest-keyed-ancestor
  recovery finds no keyed ancestor on a hit node's path, it returns `None`; the
  host then falls back to `MapPointer` with the **raw** positional interaction and
  **never invents a `Kind`/root id** the consumer did not author. An unroutable
  hit is visible, not silently mis-routed.
- **FR-008a (focus/tab-traversal & full editor UX deferred to E4).** The text seam
  delivers a keystroke to the **focused** text control only. Caret/selection
  gestures, IME UX beyond the existing `Composition` hooks, undo/redo, and a
  general focus/tab-traversal model across all control kinds are **out of scope**
  — trajectory item **E4**.
- **FR-005a (host mechanism + representative sample, no catalog-wide audit).** 090
  proves the dispatch path on a representative sample (leaf-keyed, container-keyed,
  text); it does **not** audit/retrofit the binding surface of the 52 typed
  `Widgets/*.fs` views. Any per-view "exposes no binding" gap is flagged to a
  separate fitness pass.
- **Non-authoritative `GeneratedProductCheck`.** `GeneratedProductCheck` drives a
  real consumer restore/build/`Verify`. Locally it can fail for **environment**
  reasons (the generated `Verify` cannot resolve an active feature: no template
  `.specify/feature.json` + a `Map.empty` environment) — a **non-authoritative
  environment-failure**, recorded in `logs/`, NOT a product defect.
- **Render-target-only responds-proof honesty.** The before/after frames are
  honest render-only artifacts; the proof states it captures an input→visible
  change on the running host's render path, not a live desktop-window screenshot.

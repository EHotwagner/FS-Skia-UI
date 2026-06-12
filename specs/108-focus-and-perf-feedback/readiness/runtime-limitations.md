# Runtime limitations & failure diagnostics (feature 108, T007)

## Documented evidence path

Feature 108's evidence path is **offscreen deterministic + responds-proof**; a live Vulkan window is
**not required** (spec Assumptions). The asserted surfaces are:

- the pure, headless `ControlsElmish.Perf.runScript` driver (byte-stable per-frame `FrameMetrics`),
- the structural-Scene focus-ring diff over the real `Control.renderTree`,
- the interactive responds-proof (`captureRespondsProof` / `routeInteractivePointer`),
- pure-transition tests for `markFocused`, `Control.map`/`Widget.map`, DataGrid tri-state, the
  modifier-aware key boundary, `Theming.resolve`/`toTheme`, and `EvidenceTour.run`.

A live window CAN open in this environment via the X11 path ([[live-vulkan-window-x11-path]]), but it
is not part of this feature's required evidence.

## Out-of-scope / deferred (spec Out of Scope)

The deeper repaint optimisations surfaced by the ControlsShowcase3 feedback are **explicitly deferred**:

- damage-rect / dirty-region repaint
- hover-as-local-invalidation re-stamp
- X11/Wayland backend **motion-event compression** (the live `runInteractiveApp` loop coalesces moves
  at the host edge via the per-frame accumulator + `Perf` predicate, but it cannot compress native
  motion events below the SkiaViewer per-sample callback model without rearchitecting SkiaViewer —
  out of scope. The authoritative, fully-asserted coalescing + metrics surface is `Perf.runScript`.)
- `speckit.snapshot-source-tree` tooling
- consumer-side ListView visible-window slicing

## Failure diagnostics

- A missing required evidence artifact fails `Route --enforce` (it names the artifact + requiring tier).
- A coalescing/metrics regression fails the `Perf.runScript` byte-stable golden (SC-003/004/005).
- `FrameDuration` is reported but EXCLUDED from determinism assertions (it varies run to run).

## Platform / runtime support boundary

Feature 108 touches the framework wherever it runs: a **.NET 10 desktop** host rendering through
**Vulkan** via the **SkiaSharp preview** native binding, on Windows and Linux desktop (`net10.0`).
**unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; those targets are
out of scope for this feature exactly as for the rest of the framework. The 108 evidence is
GPU-free deterministic assembly + responds-proof, so it does not depend on the live Vulkan surface.

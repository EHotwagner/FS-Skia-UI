# Runtime limitations & failure diagnostics (feature 110)

## Documented evidence path

Feature 110 is a **hot-path routing MECHANISM change** proven by deterministic, headless evidence; a
live Vulkan window is **not required** (spec *Unsupported scope* / Assumptions). The asserted surfaces:

- the pure `ControlsElmish.Perf.runScript` driver — the authoritative byte-stable per-frame
  `FrameMetrics` surface (counts + booleans), now showing zero routing full renders on pointer frames,
- the regenerated per-scenario pointer goldens under `specs/109-perf-metrics-baseline/readiness/perf-corpus/`
  (re-run byte-identically; routing `FullRenderCount` dropped to 0, SC-007),
- the parity tests (`Feature110RetainedRoutingParityTests`) comparing the retained route to the
  preserved full-render oracle, and the forced-fallback test (`Feature110FallbackTests`).

A live window CAN open in this environment via the X11 path ([[live-vulkan-window-x11-path]]), but it is
not part of this feature's required evidence — the live `runInteractiveApp` emit path is the BEST-EFFORT
observability sink, and the authoritative dispatch/metrics surface is `Perf.runScript` and the headless
internal seams reached via `InternalsVisibleTo`.

## Out-of-scope / deferred (spec *Unsupported scope*)

This feature is **Phase 2 only**. Explicitly deferred to Phase 3+ of the source report: the
`FrameCause`/`FrameInvalidation` frame scheduler, narrowed runtime visual-state stamping, view/control
memoization, viewport virtualization, damage rectangles / Skia picture / paint caches, text-measurement
/ layout-boundary caches, and any `SkiaViewer` backend / render-thread / compositor review. The
full-render path is **not removed** — it is preserved as the parity oracle and the counted last-resort
fallback (FR-007). The fallback degrades to **correct** dispatch (it runs the real oracle), never a
silent mis-dispatch; every fallback is made visible by `FullRenderFallbackCount` (FR-009).

## Failure diagnostics

- A missing required evidence artifact fails `Route --enforce` (it names the artifact + requiring tier).
- A dispatch-parity regression fails `Feature110RetainedRoutingParityTests` (retained route vs oracle).
- A routing-render regression (a reintroduced per-sample full render) fails the regenerated pointer
  goldens (`FullRenderCount` back above 0) and the `Feature110RetainedRoutingTests` zero-render asserts.
- A spurious fallback fails the `FullRenderFallbackCount = 0` corpus assertion (SC-005).
- `FrameDuration` is real wall-clock in the live loop but EXCLUDED from every deterministic golden
  (FR-012); `Perf.runScript` keeps it `TimeSpan.Zero` so the golden path never reads the clock.

## Platform / runtime support boundary

Feature 110 touches the framework wherever it runs: a **.NET 10 desktop** host rendering through
**Vulkan** via the **SkiaSharp preview** native binding, on Windows and Linux desktop (`net10.0`).
**unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; those targets are out
of scope exactly as for the rest of the framework. The 110 evidence is GPU-free deterministic metric +
parity assembly, so it does not depend on the live Vulkan surface.

# Runtime limitations & failure diagnostics (feature 111)

## Documented evidence path

Feature 111 is a per-frame **scheduling/observability** change proven by deterministic, headless
evidence; a live Vulkan window is **not required** (spec *Unsupported scope* / Assumptions). The
asserted surfaces:

- the pure `ControlsElmish.Perf.runScript` driver — the authoritative byte-stable per-frame
  `FrameMetrics` surface, now carrying `FrameCause` + the phase record and showing view-free
  animation/hover frames;
- the regenerated per-scenario goldens under `specs/109-perf-metrics-baseline/readiness/perf-corpus/`
  (re-run byte-identically; animation ticks now `ViewCalled=false`/`FullRenderCount=0`/`PaintRan=true`);
- the cause/phase/view-skip tests (`Feature111*`) and the updated `Feature109MetricsHonestyTests`.

A live window CAN open via the X11 path ([[live-vulkan-window-x11-path]]), but it is not part of this
feature's required evidence — the live `OnFrameMetrics` sink is best-effort, and the authoritative
cause/phase surface is `Perf.runScript`. The live-loop view-skip (`renderRetained` view cache) is a
real paint-cycle optimization whose correctness is byte-identity (the rendered scene is unchanged),
covered by the standing Scene-parity suite under `Dev`.

## Out-of-scope / deferred (spec *Unsupported scope*)

This feature is **Phase 3 only**. Explicitly deferred: **Phase 4** narrowed per-identity runtime
visual-state stamping (Phase 3 keeps the full-tree stamp, FR-009); view/control memoization (Phase 5);
viewport virtualization (Phase 6); damage rects / picture / paint caches (Phase 7); text / layout
caches (Phase 8); `SkiaViewer` backend / render-thread / compositor review (Phase 9). No granular
per-phase node-count fields beyond the ran/skipped record. The feature-110 retained routing +
full-render oracle/fallback are unchanged. The view-skip degrades to a re-view fallback on any
`(model, size)` mismatch (incl. value-type models), so it never paints a stale frame.

## Failure diagnostics

- A missing required evidence artifact fails `Route --enforce` (it names the artifact + requiring tier).
- A cause/phase classification regression fails `Feature111FrameCauseTests`/`Feature111PhaseRecordTests`.
- A re-introduced per-frame view rebuild on a model-unchanged frame fails the view-free golden
  (`ViewCalled` back true / `FullRenderCount` back above 0) and `Feature111ViewSkipTests`.
- `FrameDuration` is real wall-clock in the live loop but EXCLUDED from every deterministic golden
  (FR-012); `Perf.runScript` keeps it `TimeSpan.Zero`.

## Platform / runtime support boundary

Feature 111 touches the framework wherever it runs: a **.NET 10 desktop** host rendering through
**Vulkan** via the **SkiaSharp preview** native binding, on Windows and Linux desktop (`net10.0`).
**unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; those targets are
out of scope. The 111 evidence is GPU-free deterministic metric/phase assembly, so it does not depend
on the live Vulkan surface.

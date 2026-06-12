# Runtime limitations & failure diagnostics (feature 113)

## Documented evidence path

Feature 113 is a control-internal **memoization** + **stability-diagnostic** change proven by
deterministic, headless evidence; a live Vulkan window is **not required** (spec *Unsupported scope* /
Assumptions). The asserted surfaces:

- the internal `RetainedRender.memoize` seam + cache/entry types, exercised from `Controls.Tests` via
  `InternalsVisibleTo` — hit/miss/cold + reference-reuse (`Feature113MemoSeamTests`);
- the memo-on vs memo-off scene byte-identity + no-staleness over the wired `RetainedRender.step`
  (`Feature113MemoParityTests`);
- the deterministic `MemoHitCount`/`MemoMissCount` over `ControlsElmish.Perf.runScript`
  (`Feature113MemoMetricsTests`) + the regenerated 109 perf-corpus goldens;
- the public `Diagnostics.stabilityReport` report (`Feature113StabilityDiagTests`);
- the standing Scene-parity golden suite under `Dev` for at-rest rendered-output + geometry byte-identity.

A live window CAN open via the X11 path ([[live-vulkan-window-x11-path]]), but it is not part of this
feature's required evidence — memoization changes only *whether a pure subtree is recomputed or reused*,
observable via the deterministic `Perf.runScript` metrics and the internal seam tests, not a live window.
The live render staying byte-identical is covered by the Scene-parity suite under `Dev`.

## Out-of-scope / deferred (spec *Unsupported scope*)

This feature is **Phase 5 only**. Explicitly deferred: a public consumer `Control.memo`/`Widget.memo`
primitive; viewport virtualization (Phase 6); damage rects / picture / paint caches (Phase 7); text /
layout-boundary caches (Phase 8); `SkiaViewer` backend / render-thread / compositor review (Phase 9); any
enforced stability gate (the diagnostic is report-only this rung). Only a representative memoized site
(the DataGrid row/column projection) is wired — `Style.resolve` and the full 52-control migration are
OUT. Features 110/111/112 are unchanged (FR-015).

## Failure diagnostics

- A missing required evidence artifact fails `Route --enforce` (it names the artifact + requiring tier).
- A too-coarse dependency (a stale reuse) fails the memo-on/memo-off parity test
  (`Feature113MemoParityTests`) — never a stale render (the seam misses on an unequal/unknown dependency).
- A regression that defeats reuse (an always-new dependency) surfaces as misses in the
  `MemoHitCount`/`MemoMissCount` goldens instead of silent CPU.
- A stability-diagnostic regression fails `Feature113StabilityDiagTests` (a stable tree must report no
  findings; an injected always-new input must be flagged).

## Platform / runtime support boundary

Feature 113 touches the framework wherever it runs: a **.NET 10 desktop** host rendering through
**Vulkan** via the **SkiaSharp preview** native binding, on Windows and Linux desktop (`net10.0`).
**unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; those targets are out
of scope. The 113 evidence is GPU-free deterministic memo/parity/metrics assembly, so it does not depend
on the live Vulkan surface.

# Runtime limitations & failure diagnostics (feature 109)

## Documented evidence path

Feature 109 is **observation-and-evidence only**; a live Vulkan window is **not required** (spec
*Unsupported scope* / Assumptions). The asserted surfaces are all deterministic and headless:

- the pure `ControlsElmish.Perf.runScript` driver — the authoritative byte-stable per-frame
  `FrameMetrics` surface (counts + booleans),
- the per-scenario metrics goldens under `readiness/perf-corpus/` (re-run byte-identically, SC-005),
- the non-golden timing/allocation report under `docs/reports/_baselines/` (human-facing, NON-gating).

A live window CAN open in this environment via the X11 path ([[live-vulkan-window-x11-path]]), but it
is not part of this feature's required evidence — the live `runInteractiveApp` emit path is the
BEST-EFFORT observability sink and the authoritative coalescing/metrics surface is `Perf.runScript`
(consistent with feature 108: the SkiaViewer per-sample loop cannot truly coalesce below its callback
model without rearchitecting SkiaViewer, which is out of scope).

## Out-of-scope / deferred (spec *Unsupported scope*)

Phase 2+ of the source report is **explicitly deferred**: retained pointer routing, a frame scheduler,
narrowed visual-state stamping, view memoization, viewport virtualization, paint/damage caches, layout
caches, and the backend review. The DataGrid 10000-row scenario is intentionally run on the
**non-virtualized, fully-materialized** path — it is the pre-virtualization baseline, NOT a bug to fix
here. Paint / composite / hit-test counters do not exist until those phases land and are stated as
**not yet captured** rather than silently omitted (FR-015).

## Failure diagnostics

- A missing required evidence artifact fails `Route --enforce` (it names the artifact + requiring tier).
- A metric-honesty or coalescing regression fails the `Perf.runScript`-driven `Feature109` tests.
- A corpus regression fails the per-scenario byte-stable golden (SC-005); regression thresholds are
  **counts-first, timing-second** (FR-018).
- `FrameDuration` is real wall-clock in the live loop but EXCLUDED from every deterministic golden
  (FR-012); `Perf.runScript` keeps it `TimeSpan.Zero` so the golden path never reads the clock.

## Platform / runtime support boundary

Feature 109 touches the framework wherever it runs: a **.NET 10 desktop** host rendering through
**Vulkan** via the **SkiaSharp preview** native binding, on Windows and Linux desktop (`net10.0`).
**unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; those targets are
out of scope exactly as for the rest of the framework. The 109 evidence is GPU-free deterministic
metric assembly, so it does not depend on the live Vulkan surface.

# US3 independent validation — style-only and visual-state-only updates remeasure nothing

**Story**: A hover/focus/press/animation-tick (a style-only or runtime-visual-state-only update)
re-measures zero layout nodes, reports zero layout-invalidated nodes, and produces zero text-measure
cache misses for unchanged text, while staying byte-identical at rest.

## Path

Warm the text cache with one repaint frame, then drive a style-only / visual-state-only frame over the
SAME (unchanged) text through `RetainedRender.step` / `Perf.runScript`; assert `RemeasuredNodeCount = 0`,
`LayoutInvalidatedNodeCount = 0`, and `TextMeasureCacheMissCount = 0` (every measurement served from the
warm cache as a hit), with byte-identical rendered output.

## Evidence

- `tests/Controls.Tests/Feature117LayoutInvalidatedTests.fs` — a style-only/visual-state frame over warm
  text reports zero invalidated, zero re-measured, and ZERO text-cache misses with hits > 0.
- `tests/Elmish.Tests/Feature117MetricsTests.fs` — over `Perf.runScript`, the warm style-only frame
  reports `TextMeasureCacheMissCount = 0`, `LayoutInvalidatedNodeCount = 0`, `RemeasuredNodeCount = 0`.
- byte-identity at rest: the always-miss oracle (`Feature117TextCacheTests`) + the standing Scene-parity
  suite under `Dev`.

This is largely an assertion over behaviour 096/112/113 already produce, formalized here as a
deterministic gate (FR-007).

Result: PASS — a style-only / visual-state-only update is zero-work for layout and serves unchanged text
from the warm cache (SC-003), byte-identical at rest.

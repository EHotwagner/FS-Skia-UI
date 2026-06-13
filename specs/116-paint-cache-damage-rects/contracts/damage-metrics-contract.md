# Contract: Damage Metrics (`RepaintedNodeCount` / `DirtyRectCount` / `DirtyArea`)

Surface: public `FrameMetrics` fields (`ControlsElmish.fsi`), threaded from the internal
retained `step` via `WorkReductionRecord`. Deterministic, golden-asserted via
`Perf.runScript`. (FR-001, FR-002, FR-003, FR-004, FR-012; US1, US5.)

## Definitions

- **Damage set** — the set of nodes the retained `step` repainted this frame (own-scene
  repaint via `paintFresh`/`buildFresh`/`carry`-recompute) plus genuinely-shifted nodes.
- **`RepaintedNodeCount`** — count of nodes in the damage set.
- **`DirtyRectCount`** — count of **distinct** axis-aligned damage rectangles; default
  coalescing is one rectangle per repainted node's evaluated `Fragment.Box`, with identical
  boxes deduplicated. `None` boxes contribute no rectangle.
- **`DirtyArea`** — sum of `width * height` over the distinct rectangles, **integer-rounded**
  (control geometry is integer).

## Guarantees

1. **Localized → small.** A single control's visual-state change (e.g. hover) reports
   `RepaintedNodeCount` proportional to the changed control(s) and any genuinely-shifted
   ancestors/siblings, a small `DirtyRectCount`, and a `DirtyArea` covering only the changed
   box(es) — **not** frame-spanning.
2. **Whole-frame invalidation → frame-spanning.** A theme switch (all cached paint
   invalidated) reports every node repainted and `DirtyArea ≈` the frame area.
3. **Idle → zero.** A frame with no change reports `RepaintedNodeCount = 0`,
   `DirtyRectCount = 0`, `DirtyArea = 0`.
4. **Deterministic.** Integer geometry ⇒ integer rounding ⇒ identical values across runs;
   golden-assertable via `Perf.runScript`.
5. **Honest (never under-reports).** The damage set is the actual repaint set, including
   genuinely-shifted nodes; a localized change that shifts siblings reports those shifted
   boxes too (still bounded, not frame-spanning). Damage never claims less than was repainted.

## Concrete assertion bounds (resolves "small"/"proportional")

"Small" and "proportional" above are **not** the assertion form. Each scenario asserts a
concrete predicate, so a moderate regression (not just a whole-tree one) fails:

- **Localized hover (one leaf control changes state).** `RepaintedNodeCount` MUST equal the
  exact size of the honest repaint set — the changed node plus any genuinely-shifted
  ancestors/siblings — and MUST satisfy `RepaintedNodeCount <= 4` **and**
  `RepaintedNodeCount < TotalNodeCount` (a leaf hover with no sibling shift is exactly `1`).
  `DirtyArea` MUST equal the summed integer area of exactly those repainted boxes and MUST
  satisfy `DirtyArea < FrameArea`. `DirtyRectCount` MUST equal the count of distinct repainted
  boxes (`<= RepaintedNodeCount`). The `<= 4` ceiling is the regression tripwire: a change
  that repaints more than the changed control + its immediate shifted neighbours fails.
- **Theme switch (all paint invalidated).** `RepaintedNodeCount` MUST equal `TotalNodeCount`
  (every node) and `DirtyArea` MUST equal `FrameArea` (the union of all node boxes ≈ frame).
- **Idle.** `RepaintedNodeCount = DirtyRectCount = DirtyArea = 0` (exact).

The **exact frozen integers** for each corpus scene (the specific `TotalNodeCount`,
`FrameArea`, and per-scenario `DirtyArea`) are captured deterministically by the regenerated
golden file (`PERF_CORPUS_REGEN=1`) — the golden *is* the concrete-value pin, and the
predicates above are the run-to-run relationships `Feature116DamageTests`/`Feature116MetricsTests`
assert independently of the frozen numbers. Both `RepaintedNodeCount` and `TotalNodeCount`
are read from the same frame so the comparison is exact, not estimated.

## Counter-guarantees (negative tests)

- A regression that repaints the whole tree on a localized hover changes
  `RepaintedNodeCount`/`DirtyArea` from small to frame-spanning → **golden fails**.
- A regression that drops a genuinely-shifted node from the damage set under-reports →
  caught by the localized-shift scenario.

## Evidence

- `tests/Controls.Tests/Feature116DamageTests.fs` — small hover vs frame-spanning theme vs
  idle-zero; deterministic integer `DirtyArea`/`DirtyRectCount`.
- `tests/Elmish.Tests/Feature116MetricsTests.fs` + regenerated corpus goldens
  (`specs/109-perf-metrics-baseline/readiness/perf-corpus/*.golden.txt`,
  `PERF_CORPUS_REGEN=1`).

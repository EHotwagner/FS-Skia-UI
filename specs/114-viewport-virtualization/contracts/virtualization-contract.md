# Contract: Overscan + Bounded-Materialization

The observable, regression-proof virtualization contract for a DataGrid (the
representative virtualized surface this rung).

## C1 — Bounded materialization

For a DataGrid with `Total` logical rows, a viewport realizing `V` visible rows, and
overscan `N >= 0`:

```
VirtualItemsMaterialized <= V + 2*N        (symmetric overscan; tight at interior, tighter at edges)
VirtualItemsTotal        =  Total
```

`VirtualItemsMaterialized` equals the number of `data-grid-row` nodes actually built this
frame. The bound holds at **every** scroll position and **every** total.

## C2 — Non-scaling with total

With the same viewport and overscan, `VirtualItemsMaterialized` is **identical** at
`Total = 100`, `1000`, and `10000`; only `VirtualItemsTotal` changes. Materialization MUST
NOT grow with the logical total. (FR-003 / SC-001; proven by the corpus cross-scenario
assertion.)

## C3 — Transparent for small grids

When `Total <= V + 2*N`, the realized window is the whole set:
`VirtualItemsMaterialized = VirtualItemsTotal`. Virtualization is invisible. (FR-004.)

## C4 — Default-0 byte-identity

With overscan `0` (the default) and no other opt-in, for every existing corpus DataGrid
scenario: the realized rows, control geometry, and rendered scene are **byte-identical** to
the pre-feature baseline, and `VirtualItemsMaterialized` equals the prior realized-row
count. The only observable delta vs pre-feature is the two new metric fields existing on
`FrameMetrics`. (FR-002 / FR-006 / FR-016 / SC-002.)

## C5 — Opt-in overscan materializes only real, edge-clamped rows

With overscan `N > 0`: the realized window contains the visible rows **unchanged**, plus up
to `N` correct adjacent logical rows on each side. The window MUST NOT fabricate rows,
duplicate rows, or shift the visible rows. At the logical top/bottom edges overscan is
clamped (no index `< 0`, none `>= Total`); only real rows materialize. (FR-007 / SC-003.)

## C6 — Keyed row reuse on scroll preserved

Scrolling the realized window reuses row containers where the keyed diff permits (stable
`row.Key` → reuse), so a scroll does not rebuild unchanged rows. This feature MUST NOT
regress the existing keyed identity over `row.Key` (`DataGrid.fs:212`). (FR-008.)

## C7 — Metric determinism, idle, and aggregation

- `VirtualItemsMaterialized` / `VirtualItemsTotal` are deterministic on the
  `Perf.runScript` render path and **golden-asserted**.
- A frame that evaluates **no** virtualized control reports both as `0`. (SC-006.)
- Multiple virtualized controls in one frame **aggregate**: the materialized counts sum and
  the totals sum; per-control attribution is available in tests, not in the aggregate
  metric. (FR-014.)
- The live `OnFrameMetrics` sink reports the same fields.

## C8 — 113 memoization composes

Feature 113's DataGrid `gridGeom` memoization continues to work over the overscan-widened
realized row set: a steady-state overscan frame still records memo hits, and the projection
is byte-identical to the non-memoized build over the same realized rows. (FR-017 / SC-007.)

## Test mapping

| Contract | Test | Location |
|----------|------|----------|
| C1, C2 | bounded + non-scaling across 100/1000/10000 | `Feature114VirtualMetricsTests` (Elmish.Tests, corpus) + `Feature114OverscanTests` (Controls.Tests) |
| C3 | small grid `materialized = total` | `Feature114OverscanTests` |
| C4 | default-0 scene byte-identity + prior count | standing Scene-parity suite (`Dev`) + `Feature114VirtualMetricsTests` |
| C5 | opt-in overscan: real, edge-clamped, unshifted visible | `Feature114OverscanTests` |
| C6 | keyed reuse on scroll (no rebuild) | `Feature114OverscanTests` |
| C7 | determinism, idle 0/0, aggregate | `Feature114VirtualMetricsTests` |
| C8 | memo hit on steady overscan frame | `Feature114OverscanTests` (asserts 113 `MemoHitCount`) |

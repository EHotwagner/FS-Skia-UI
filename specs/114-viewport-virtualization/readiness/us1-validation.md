# US1 independent validation — bounded, non-scaling materialization

**Story**: A large repeated control materializes only its visible window plus overscan.

## Path

Render a 100-, 1000-, and 10000-row DataGrid scenario with the **same** bounded viewport and overscan,
then assert the materialized count (the number of `data-grid-row` nodes / the realized `VisibleRange.Count`)
is bounded by `visible + 2*overscan` and **identical** across the three totals while the logical total
scales.

## Evidence

- `tests/Controls.Tests/Feature114OverscanTests.fs` — overscan-0 Count identical (= V) across 100/1000/
  10000; with overscan N a mid-list window realizes exactly V + 2N real rows; the materialized
  `data-grid-row` count equals the realized Count; a grid whose total fits the window realizes the whole
  set (transparent); the window is edge-clamped at top and bottom.
- `tests/Elmish.Tests/Feature114VirtualMetricsTests.fs` — over `ControlsElmish.Perf.runScript`,
  `VirtualItemsMaterialized` is bounded and identical across 100/1000/10000 while `VirtualItemsTotal`
  scales (100 / 1000 / 10000).
- 109 perf-corpus goldens `datagrid-{100,1000,10000}.golden.txt` (regenerated) — `VirtualItemsMaterialized=30`
  at every total, `VirtualItemsTotal` = 100/1000/10000.

Result: PASS — materialized work does not grow with total logical row count (SC-001).

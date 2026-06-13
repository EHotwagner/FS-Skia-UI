# Virtual-Metrics Authority (FR-013 / FR-014 / FR-015 / SC-001 / SC-006)

**Purpose**: Records which evidence proves the deterministic `VirtualItemsMaterialized` /
`VirtualItemsTotal` `FrameMetrics` contract and the bounded / non-scaling materialization guarantee.

## Decomposition

| Clause | Authority | Task |
|--------|-----------|------|
| `VirtualItemsMaterialized <= visibleCount + 2*overscan` for any Total | `Feature114OverscanTests`, `Feature114VirtualMetricsTests` | T009/T018 |
| materialized does NOT scale with Total (identical at 100/1000/10000) | `Feature114VirtualMetricsTests` + the 109 datagrid-{100,1000,10000} goldens | T018/T020 |
| `VirtualItemsTotal = RowCount` (logical total scales with data) | `Feature114VirtualMetricsTests` + the regenerated goldens | T018/T020 |
| a frame with no virtualized control reports 0 / 0 | `Feature114VirtualMetricsTests` (button + idle cases) | T018 |
| counts aggregate across multiple virtualized controls in one frame | `Feature114VirtualMetricsTests` (two-grid case) | T018 |
| the 10000-row corpus scenario asserts bounded materialization while Total = 10000 | 109 `datagrid-10000.golden.txt` (regenerated) | T020 |

## Decision

`VirtualItemsMaterialized` / `VirtualItemsTotal` are populated from the internal
`WorkReductionRecord.VirtualMaterialized` / `VirtualTotal`, counted in the retained `step` by a read-only
walk of the lowered tree (count of `data-grid-row` nodes; sum of each `data-grid`'s `VisibleRange.Total`),
then threaded into `FrameMetrics` exactly as 113's `MemoHitCount`/`MemoMissCount` (the `zero` record, the
four `Perf.runScript` per-frame sites, and the live `OnFrameMetrics` sink). The counts are surfaced on the
deterministic `ControlsElmish.Perf.runScript` path, so they are reproducible and **golden-asserted**.

## Authoritative golden evidence (regenerated 2026-06-13, `PERF_CORPUS_REGEN=1`)

The 109 perf-corpus datagrid scenarios (frame 2, the retained-step frame) record:

```
datagrid-100   : VirtualItemsMaterialized=30 VirtualItemsTotal=100
datagrid-1000  : VirtualItemsMaterialized=30 VirtualItemsTotal=1000
datagrid-10000 : VirtualItemsMaterialized=30 VirtualItemsTotal=10000
```

Materialized is **identical (30)** across all three totals — proving materialization does not scale —
while the logical total scales with the data. `RemeasuredNodeCount=126` is unchanged from the
pre-feature golden (the counts are read-only). A regression that re-materializes every row, or an
overscan exceeding its bound, would move these goldens — a blocking change.

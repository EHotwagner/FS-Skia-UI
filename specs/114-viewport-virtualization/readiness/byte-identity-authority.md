# At-Rest Byte-Identity Authority (FR-006 / FR-016 / SC-002 / SC-007)

**Purpose**: Records, as a visible decision, which evidence proves the at-rest **rendered output**,
**control geometry**, **focus/keyboard routing for materialized rows**, and **dispatch-outcome**
byte-identity clauses of FR-006 / FR-016 / SC-002 / SC-007 — with default overscan (0) the virtualized
build must be byte-identical to the pre-feature build.

## Decomposition

| Clause | Authority | Task |
|--------|-----------|------|
| overscan-0 realized window == historic visible slice (same keys/geometry) | `Feature114OverscanTests` + `Feature114OverscanParityTests` | T009/T012/T013 |
| at-rest rendered scene + control geometry byte-identical | Standing Scene-parity / golden suite (091/092/096–103 + 109 corpus) under `Dev` | T023 (gate) |
| `VirtualItemsMaterialized` == prior realized-row count | `Feature114VirtualMetricsTests` + the regenerated 109 perf-corpus goldens | T018/T020 |
| dispatch outcome for an already-materialized (visible) row byte-identical | `Feature114OffscreenTests` (visible-row case) + existing `ControlsDataGridTests` | T014/T015 |
| opt-in overscan materializes only real, edge-clamped adjacent rows | `Feature114OverscanParityTests` | T012/T013 |

## Decision

Overscan defaults to **0**, and `Collections.visibleRange ... 0` returns the historic slice
(`first' = first`, `count' = count`) **by construction** — the widening arithmetic is a no-op at `n = 0`.
The materialization site is unchanged (`DataGrid.visibleRows rows visibleRange |> List.map (rowControl
columns)`); overscan only widens the `VisibleRange` the site consumes. The retained `step` adds a
**read-only** walk that counts `data-grid-row` nodes and sums each `data-grid`'s logical `Total` — it
emits no scene change. Offscreen focus/selection records logical state and relocates the window only via
`ScrollRowsTo` (index); `FocusCell`/`SelectRow`/`ToggleRow` keep their pre-feature outcomes, so a
visible-row dispatch is byte-identical (FR-016). At-rest rendered scene + per-control geometry are
therefore unchanged, and the **existing Scene-parity / golden suite** (run under `./fake.sh build -t
Dev`) is the standing authority for that clause.

Any unexpected scene/geometry golden movement during `Dev` is a **blocking regression**, not an accepted
change. The 109 perf-corpus goldens were regenerated to carry the two additive virtualization counts; all
prior fields are unchanged (virtualization counting does not alter layout/measure/diff/paint — the
datagrid goldens keep `RemeasuredNodeCount=126` and gain only `VirtualItemsMaterialized=30` /
`VirtualItemsTotal=100|1000|10000`).

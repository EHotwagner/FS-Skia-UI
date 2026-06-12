# US1 independent validation path (feature 113, T011)

Render the same model twice through `ControlsElmish.Perf.runScript` for a scenario with a memoizable
DataGrid whose data + theme are unchanged; the second frame records the **hit** and reuses the prior
projected subtree.

- A frame that re-evaluates a memoizable DataGrid leaf whose dependency (theme + box + cells) is
  unchanged → `MemoHitCount > 0`, `MemoMissCount = 0`, and the stored `Scene list` instance is reused
  (the thunk does not run) (SC-001, FR-001/FR-004).
- A changed dependency (the grid's cells change) or a cold first evaluation → a `Miss` + a fresh subtree
  (FR-005).
- An idle frame, or a host with no memoizable control → both counts `0` (C8/FR-009).

Evidence: `Feature113MemoSeamTests` (the seam directly, `Controls.Tests` via InternalsVisibleTo —
hit/miss/cold + reference-reuse with an instrumented thunk) and `Feature113MemoMetricsTests` (the same
behaviour observed end-to-end as deterministic `FrameMetrics` counts over `Perf.runScript`). The live
host (`runInteractiveApp`) threads the same retained-step memo outcomes into the `OnFrameMetrics` sink via
the carried `WorkReductionRecord` (the same channel `RemeasuredNodeCount` uses).

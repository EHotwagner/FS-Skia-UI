# Memo Metrics Authority (FR-009 / FR-010 / SC-004)

**Purpose**: Records, as a visible decision, which evidence proves the deterministic
`FrameMetrics.MemoHitCount` / `MemoMissCount` clause — memo work must be observable as deterministic,
golden-asserted counts.

## Decomposition

| Clause | Authority | Task |
|--------|-----------|------|
| Steady-state unchanged data → `MemoHitCount > 0`, `MemoMissCount = 0` | `Feature113MemoMetricsTests` over `Perf.runScript` | T014/T015 |
| Perturbed / cold inputs → `MemoMissCount` accrues | `Feature113MemoMetricsTests` | T014/T015 |
| Idle / no-memoizable frame → both counts `0` | `Feature113MemoMetricsTests` + the regenerated 109 corpus goldens | T014/T015/T016 |
| Counts thread from the retained step into `FrameMetrics` on the deterministic path AND the live `OnFrameMetrics` sink | `ControlsElmish.fs` threading (`WorkReductionRecord.MemoHits/MemoMisses` → `FrameMetrics.MemoHitCount/MemoMissCount`) | T015 |

## Decision

The retained step aggregates each frame's memo outcomes into `WorkReductionRecord.MemoHits`/`MemoMisses`
(summed over every memoized site evaluated that frame). `ControlsElmish` threads them into the two public
`FrameMetrics` fields: the `zero` record carries both `0`, every per-frame construction site sets them
from the last retained-step record (reset per frame so a render-free frame reports `0/0`), and the live
`emitFrameMetrics` reads them from the carried `lastWorkReduction`. The deterministic
`ControlsElmish.Perf.runScript` path is the **golden-asserted authority** (`Feature113MemoMetricsTests`
plus the regenerated 109 perf-corpus goldens, which now carry the two additive fields with all prior
fields unchanged). Both counts are `0` on an idle frame and on a host with no memoizable control — the
no-spurious-accounting clause (C8/FR-009).

# At-Rest Byte-Identity Authority (FR-014 / SC-002)

**Purpose**: Records, as a visible decision, which evidence proves the at-rest **rendered output** and
**control geometry** byte-identity clause of FR-014 / SC-002 — the memoized build must be byte-identical
to the non-memoized build.

## Decomposition

| Clause | Authority | Task |
|--------|-----------|------|
| memo-on rendered scene ≡ memo-off (always-miss oracle) over representative frame sequences | `Feature113MemoParityTests` (`Scene` equality) | T012/T013 |
| A real input change reflects the change (no stale reuse) | `Feature113MemoParityTests` (no-staleness case) | T012/T013 |
| The reuse is real (a forced rebuild with unchanged data hits) | `Feature113MemoParityTests` + `Feature113MemoMetricsTests` | T012/T014 |
| At-rest rendered output + control geometry | Standing Scene-parity / golden suite (091/092/096–103 + 109 corpus) run under `Dev` | T022 (gate) |

## Decision

This feature adds a memoization boundary inside the control-lowering / retained step; it does not change
layout evaluation, the diff, or any paint algorithm — it only decides *whether* a memoizable subtree (the
DataGrid projection) is recomputed or reused. A hit returns the exact `Scene list` the recompute would
produce because the dependency value captures every input the projection reads (theme, box, cells), so
an equal dependency guarantees an equal projection. The **always-miss oracle** (`MemoEnabled = false`) is
the authority that the dependency is not too coarse: memo-on ≡ memo-off for every frame, including a
real-input-change frame. At-rest rendered scene + per-control geometry are therefore unchanged **by
construction**, and the **existing Scene-parity / golden suite** (run under `./fake.sh build -t Dev`) is
the standing authority for that clause; the focused mechanism parity is `Feature113MemoParityTests`.

Any unexpected scene/geometry golden movement during `Dev` is a **blocking regression**, not an accepted
change. The 109 perf-corpus goldens were regenerated to carry the two additive memo counts; all prior
fields are unchanged (memoization does not alter layout/measure/diff).

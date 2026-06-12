# Controls corpus baseline — BEFORE feature-108 coalescing (hover/pointer-move burst)

Non-golden, human-facing, NON-GATING evidence (FR-016/017/019). Timing/allocation are
environment-dependent; the gating surface is the deterministic counts golden (see the
cross-linked `readiness/perf-corpus/*.golden.txt`). Regenerate with `PERF_BASELINE_REGEN=1`.

## Hover/pointer-move burst — coalescing OFF (each raw sample processed = N full renders)

- Scenario: hover-burst-300 (each of 300 raw moves processed individually)
- Phase: before
- TimingMs: 785.930 (median of measured iterations)
- AllocatedBytes: 970977600
- CounterSnapshot: PointerSamplesReceived=300 PointerMovesProcessed=300 FullRenderCount=300 (one render PER sample, un-coalesced)
- Cross-link: specs/109-perf-metrics-baseline/readiness/perf-corpus/hover-sweep-* (the count goldens)

## Regression threshold policy (FR-018)

Counts FIRST, timing SECOND: a regression is a change in the deterministic count/boolean
golden surface; timing/allocation only INFORM (they never gate, being environment-dependent).

## MissingCounters: paint, composite, hit-test, layout — NOT yet captured (paint/composite/hit-test arrive with Phase 2/7; the materialized DataGrid's per-row layout cost is not reflected in RemeasuredNodeCount because rows are not individual layout nodes — it shows as FullRenderCount). Silent omission is not acceptable (FR-015).


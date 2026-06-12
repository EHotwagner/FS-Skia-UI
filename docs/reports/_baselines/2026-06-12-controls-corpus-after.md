# Controls corpus baseline — AFTER (current path, feature-108 coalescing ON)

Non-golden, human-facing, NON-GATING evidence (FR-016/017). Timing/allocation are
environment-dependent; the gating surface is the deterministic counts goldens under
`specs/109-perf-metrics-baseline/readiness/perf-corpus/`. Regenerate with `PERF_BASELINE_REGEN=1`.

## Hover/pointer-move burst — coalescing ON (one processed move = one full render)

- Scenario: hover-burst-300 (the SAME 300 raw moves, coalesced to one processed move)
- Phase: after
- TimingMs: 2.711 (median of measured iterations)
- AllocatedBytes: 3287944
- CounterSnapshot: PointerSamplesReceived=300 PointerMovesProcessed=1 FullRenderCount=1 (coalesced)
- Observed coalescing speedup vs before: ~290.0x wall-clock (informational only)
- Cross-link: specs/109-perf-metrics-baseline/readiness/perf-corpus/hover-sweep-* (the count goldens)

## Corpus scenarios (current path)

Each corpus scenario's deterministic count/boolean snapshot is its committed golden; the
timing/allocation below is the non-gating wall-clock the report generator captured.


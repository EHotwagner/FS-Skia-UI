# Performance scenario corpus — counts evidence (feature 109, T019, SC-006)

The corpus drives a fixed set of representative interactions through the deterministic
`ControlsElmish.Perf.runScript` path over REAL control trees and the REAL fully-materialized DataGrid,
each producing a byte-stable per-frame metrics golden (counts + booleans only) under
`readiness/perf-corpus/<scenario>.golden.txt`. Evidence: `tests/Elmish.Tests/Feature109CorpusTests.fs`
(10 scenarios; each asserts its committed golden AND re-runs byte-identically, SC-005).

## What the counts answer, per scenario (SC-006)

- **hover-sweep-100/1000/5000** — a move burst over N controls is ONE coalesced frame:
  `PointerSamplesReceived = N`, `PointerMovesProcessed = 1`, `FullRenderCount = 1` (the routing render),
  `ProductModelChanged = false`. Coalescing collapses N raw samples to a single processed move.
- **datagrid-100/1000/10000** (non-virtualized, fully-materialized) — a layout-affecting interaction
  performs `FullRenderCount = 1` full materialization of all N rows. `RemeasuredNodeCount = 126`
  REGARDLESS of row count (100, 1000, or 10000) — a genuine finding: the materialized DataGrid does NOT
  expose per-row layout nodes, so the per-row cost is NOT reflected in `RemeasuredNodeCount`; it lives
  in the full materialization (`FullRenderCount`). This is exactly the cost Phase 2 virtualization
  targets, and the gap is stated explicitly (see MissingCounters below).
- **deep-nested-layout** — an orientation toggle re-measures `RemeasuredNodeCount = 64` nested nodes
  (proving the remeasure counter scales with genuine layout-node structure).
- **text-entry-while-animating** — text-entry frames report `ProductModelChanged = true`; the
  interleaved animation-only ticks report `ProductModelChanged = false, ViewCalled = true` (divergence).
- **theme-switch-dashboard** — a theme toggle re-renders the dashboard (`FullRenderCount = 1`).
- **continuous-drag-400** — 400 raw drag samples collapse to one processed move
  (`PointerSamplesReceived = 400`, `PointerMovesProcessed = 1`); the raw path stays reconstructable.

## MissingCounters (FR-015 — silent omission is not acceptable)

paint, composite, hit-test, and layout-per-row counters are **not yet captured** (paint/composite/
hit-test arrive with the deferred Phase 2/7; the materialized DataGrid's per-row cost is not a layout
node so it is not in `RemeasuredNodeCount`). The corpus is EXTENDED with these counters when those
phases land; until then they are stated, not silently dropped.

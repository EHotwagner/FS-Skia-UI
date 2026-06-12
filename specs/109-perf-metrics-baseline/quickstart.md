# Quickstart: feature 109 — honest frame metrics & perf baseline corpus

Observation-only. Implements report Phase 0 + Phase 1; Phase 2+ deferred.

## 1. Harden the metric (Phase 1)

Edit `src/Controls.Elmish/ControlsElmish.fsi` — in `FrameMetrics`, remove
`ViewRebuilt`; add `ProductModelChanged`, `ViewCalled`, `FullRenderCount` (with
XML-doc). See [contracts/frame-metrics.md](./contracts/frame-metrics.md).

Edit `src/Controls.Elmish/ControlsElmish.fs`:
- `emitFrameMetrics` (~L796): replace the single `viewRebuilt` arg with
  `productModelChanged` (model reference changed across `host.Update`) and
  `viewCalled`/`fullRenderCount` (did `renderStep` materialize a tree). Set real
  `FrameDuration` in the live loop.
- `Perf.runScript` `zero` (~L1048) + each per-frame branch (move-coalesced,
  `Idle`, `Tick`, `Key`, discrete `Pointer`): split `rebuilt` into
  `ProductModelChanged` (reference-compare model before/after `applyMessages`) and
  `ViewCalled`/`FullRenderCount` (count the `host.View`+`renderTree`
  materializations — the routing render in `routeInteraction` and the
  `renderStep`). Keep `FrameDuration = TimeSpan.Zero` here (golden path stays
  clock-free).

Update reading sites: `tests/Elmish.Tests/Feature108MetricsTests.fs`,
`Feature090DispatchTests.fs`, `Feature098DispatchTests.fs`. (The
`OnFrameMetrics = ignore` sites in `template/base/src/Product/EvidenceCommands.fs`
and `tests/SkiaViewer.Tests/Feature085InteractiveHostTests.fs` set a host *field*
and need **no** change.)

## 2. Add the scenario corpus (Phase 0)

In `tests/Elmish.Tests`, add corpus fixtures + golden tests (new
`Feature109CorpusTests.fs`). For each `PerformanceScenario` build the host from
existing control kinds / the current DataGrid path, drive its `FrameInput` script
through `Perf.runScript`, serialize counts+booleans, and assert against the
committed golden under `readiness/perf-corpus/<scenario>.golden.txt`. Re-run to
confirm byte-identity.

Required scenarios (FR-013): hover 100/1000/5000 controls; DataGrid
100/1000/10000 rows; deep nested layout; focused text entry while siblings
animate; theme switch dashboard; continuous drag of hundreds of samples.

## 3. Metric-honesty + coalescing tests (Phase 1)

New `tests/Elmish.Tests/Feature109MetricsHonestyTests.fs`:
- SC-001 three frames (no product message / model-change-no-visual / host
  visual-state change) + SC-004 idle → assert each field vs the code-path fact.
- SC-002 burst → received=N, processed≤1; SC-003 discrete press/release/click/
  scroll never dropped; FR-011 drag path retained.
- SC-010 `OnFrameMetrics` fires exactly once per produced frame.

These must fail first (won't compile against the old `ViewRebuilt` shape / assert
the old conflated fact) and pass after.

## 4. Non-golden baselines (Phase 0 / US4)

Run the non-golden report generator (an Expecto evidence test or small harness —
**not** a FAKE gate) over the corpus; write per-scenario timing + allocation to
`docs/reports/_baselines/2026-06-12-controls-corpus-{before,after}.md`, including
the hover-burst before/after-coalescing pair (FR-019), count-first regression
thresholds (FR-018), and an explicit `MissingCounters` line (FR-015).

## 5. Validate

```
./fake.sh build -t Route            # confirm escalation to controls-public-surface
./fake.sh build -t RefreshSurfaceBaselines   # regen surface + per-package baselines
```
Then run only the gates `Route` prints, sequentially (FAKE-backed, deterministic
order):
```
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

## Invariant

No render/layout/dispatch path is edited. At-rest output and the default host path
stay byte-identical (FR-020 / SC-008). The DataGrid 10000-row scenario runs on the
**non-virtualized** path on purpose — it is the pre-virtualization baseline, not a
bug to fix here.

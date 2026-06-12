# Implementation Plan: Controls Performance Baseline Corpus & Honest Frame Metrics (feature 109)

**Branch**: `109-perf-metrics-baseline` | **Date**: 2026-06-12 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/109-perf-metrics-baseline/spec.md`
**Source report**: `docs/reports/2026-06-12-1422-controls-performance-framework-research.md`
(Phase 0 + Phase 1 only; Phase 2+ deferred — see spec *Unsupported scope*)

## Summary

This feature implements the **first part** of the controls-performance research
report — **Phase 0 (Baseline and Guardrails)** and **Phase 1 (Finish and Correct
Feature 108 Metrics and Coalescing)**. It is an **observation-and-evidence
feature**: it changes **no** rendering, layout, hit-testing, dispatch, or
default (non-observing) host behavior, and at-rest output stays **byte-identical**
(FR-020 / SC-008). The work has two pillars:

1. **Make the per-frame metrics truthful (Phase 1, P1).** Feature 108's
   `FrameMetrics.ViewRebuilt` is a *semantic approximation* — it currently means
   "a product message changed the model" (`not (List.isEmpty msgs)`), not "`host.View`
   actually ran". Those are different facts and the report requires them
   separated before the metric hardens into public contract on the next bump.
   **Resolution (clarified 2026-06-12): replace `ViewRebuilt`** with two precise
   booleans `ProductModelChanged` + `ViewCalled`, and **add** a deterministic
   integer `FullRenderCount` (full `host.View` + `Control.renderTree` rebuilds per
   frame). This is a breaking public `.fsi` change to `FrameMetrics` in
   `ControlsElmish.fsi`; every construction/read site updates in the same change.
   Coalescing fidelity (FR-008..FR-011), once-per-frame emission (FR-007), and
   real `FrameDuration` timing kept out of goldens (FR-012) are verified and made
   load-bearing.

2. **Stand up a reproducible scenario corpus with honest baselines (Phase 0, P1/P2).**
   A fixed corpus of representative interactions (hover sweep 100/1000/5000
   controls; DataGrid 100/1000/10000 rows on the **current fully-materialized**
   path; deep nested layout; focused text-entry while siblings animate; theme
   switch across a dashboard; continuous drag of hundreds of raw samples) is
   driven through the deterministic `Perf.runScript` path, each producing a
   **byte-stable per-frame metrics golden** of counts+booleans only. A **non-golden**
   timing/allocation report generator captures real numbers, and **before/after**
   baselines (including the feature-108 coalescing hover burst) are stored in-repo
   under `docs/reports/_baselines/`. The corpus and its driver live in
   **test/evidence projects only** — **no new shipped `Controls.Elmish` API** is
   added beyond the `FrameMetrics` field change (clarified 2026-06-12).

The four user stories are prioritised P1 (US1 metric honesty, US2 corpus+goldens),
P2 (US3 coalescing fidelity, US4 before/after timing baselines).

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: none new. Exercises existing `FS.Skia.UI.Controls.Elmish`
(`FrameMetrics`, `InteractiveAppHost.OnFrameMetrics`, `Perf.runScript`,
`runInteractiveApp` coalescing) and the current `FS.Skia.UI.Controls` DataGrid
surface. Tests: Expecto + FsCheck (already referenced by `tests/Elmish.Tests`).
**Testing**: Expecto semantic tests in `tests/Elmish.Tests` (Perf-driver metric
facts + corpus goldens), FAKE targets, deterministic `Perf.runScript` goldens
committed under the feature evidence area, a non-golden report generator writing
to `docs/reports/_baselines/`.
**Target Platform**: Windows and Linux. Observation-only — no Vulkan/Skia/window
behavior change; no unsupported-environment diagnostic change.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification: Tier 1 (contracted change).** The public `FrameMetrics`
field set changes (remove `ViewRebuilt`; add `ProductModelChanged`, `ViewCalled`,
`FullRenderCount`). Requires the full artifact chain: spec, plan, `.fsi` update,
surface + per-package baseline updates, failing-first semantic tests, and doc
updates. The rendered scene, control geometry, and default host path stay
byte-identical (Tier-1 *observability* surface change, not a behavior change) —
FR-020 reconciles the breaking surface change against byte-identical default
behavior (the *shape* of the observability contract changes; no rendered pixel,
layout box, or dispatch outcome does).

### Repository Governance Decisions

- **Template ownership**: N/A to `.template.config/template.json` selection — no
  new capability, sample, or command is added to the generated product. The
  template's only `FrameMetrics` touchpoint is `template/base/src/Product/
  EvidenceCommands.fs:295` which sets `OnFrameMetrics = ignore` (a host *field*,
  not a `FrameMetrics` *construction*), so it is unaffected by the field rename
  and needs no edit. `TemplateCheck`/`TemplateDrift` still run because the `.fsi`
  change escalates `Route`; they must stay green (the template consumes the
  packed `Controls.Elmish`, so its pin advances on the post-merge bump per the
  standard cadence, not in this feature branch).
- **Dependency impact**: N/A — no new dependency. `Directory.Packages.props`,
  `docs/dependencies.md`, and `DependencyReport` are untouched (no package added,
  removed, or version-pinned). FsCheck/Expecto are already referenced by
  `tests/Elmish.Tests`.
- **Command-surface impact**: No `build.fsx` target is added or removed and **no
  new gate is introduced**. The non-golden benchmark/report generator is a
  test/evidence-project entry (an Expecto test or a small console/`dotnet run`
  evidence harness writing `docs/reports/_baselines/`), **not** a new FAKE target,
  so it never gates. `Route` escalates to the controls-public-surface tier once
  the `.fsi` edit exists; obey its printed list. FAKE-backed targets run
  sequentially in the deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
  (Run `Route` first and obey its printed minimal list; before the `.fsi` edit
  exists Route reports `agent-ready` = `Dev, GeneratedGuidanceCheck,
  TemplateDrift, EvidenceGraph, EvidenceAudit` — the escalation is the post-`.fsi`
  state the spec's Build-target impact records.)
- **Generated project impact**: None. Default/minimal generated contents,
  selected Controls guidance, local skills, validation logs, placeholder/
  excluded-history scans, and generated `Dev` behavior are unchanged. The corpus
  is repo-internal evidence, not generated-product content.
- **Evidence paths**:
  - Deterministic per-scenario metrics goldens: `specs/109-perf-metrics-baseline/readiness/perf-corpus/<scenario>.golden.txt`
    (counts + booleans only; `FrameDuration`/allocation excluded).
  - Non-golden timing/allocation report + before/after baselines:
    `docs/reports/_baselines/2026-06-12-controls-corpus-before.md` and
    `…-after.md` (FR-016/017/019), plus the count-first regression thresholds
    (FR-018).
  - Metric-honesty FSI transcript: `specs/109-perf-metrics-baseline/readiness/fsi-session.txt`.
  - Skill-loading evidence: `specs/109-perf-metrics-baseline/readiness/skill-loading-evidence.md`.
  - Window-visibility not-applicable set (if the audit fires on literal
    filenames): the standard 6-file set under `readiness/`.
  - `readiness/evidence-audit.md` (verdict token) and `readiness/generated-validation.md`
    (package-resolution=resolved, package-mismatch=false).
- **`.fsi` / contract impact**: **Yes — breaking.** `ControlsElmish.fsi`
  `FrameMetrics` record: remove `ViewRebuilt: bool`; add `ProductModelChanged:
  bool`, `ViewCalled: bool`, `FullRenderCount: int`. XML-doc required on every new
  field (doc-preservation gate; attribute-before-doc-before-type ordering as in
  108). `RefreshSurfaceBaselines` regenerates `readiness/surface-baselines/
  FS.Skia.UI.Controls.Elmish.txt` and `readiness/per-package-surface/
  FS.Skia.UI.Controls.Elmish.fsi.txt` after the change. `Perf.runScript` signature
  is unchanged (still `host -> size -> script -> FrameMetrics list`); only the
  returned record's field set changes.
- **MVU/effect boundary**: No change to MVU semantics. `Update`, effects,
  subscriptions, commands, and the interpreter are untouched (spec State-workflow
  impact = none). `Perf.runScript` already folds the pure `host.Update` +
  `RetainedRender.step`; this feature only changes which **facts** the fold
  records (`ProductModelChanged` = model identity changed across `host.Update`;
  `ViewCalled`/`FullRenderCount` = whether/how many times `renderStep` invoked
  `host.View` + `Control.renderTree`). No new effect or message type.
- **Synthetic evidence**: None expected. The corpus drives the **real**
  `Perf.runScript` pure path over **real** control trees and the **real**
  fully-materialized DataGrid; goldens are produced by real code, not literals.
  Timing/allocation baselines are real measurements (environment-dependent,
  human-facing, non-gating) — recorded evidence, not synthetic fixtures. No `[S]`/
  `[SEH]` task is anticipated; if a corpus scenario cannot drive a real path it
  returns to task review rather than being stubbed.
- **Test evidence**: Failing-first Expecto tests in `tests/Elmish.Tests`:
  (a) **metric-honesty** — the three scripted frames of SC-001 (no product
  message; product message with no visual change; host visual-state change with
  no product message) plus the idle frame (SC-004), asserting each
  `ProductModelChanged`/`ViewCalled`/`FullRenderCount`/`RemeasuredNodeCount` field
  against the code-path fact; (b) **coalescing fidelity** — burst → received=N,
  processed≤1 (SC-002), discrete press/release/click/scroll never dropped
  (SC-003), drag path retained (FR-011); (c) **corpus goldens** — each scenario
  re-runs byte-identically (SC-005) and the goldens contain no timing/allocation
  field (SC-009); (d) **once-per-frame** emission count (SC-010). These fail
  against today's `ViewRebuilt` shape (won't compile / wrong facts) and pass after.
- **Observability**: This feature *is* observability. `OnFrameMetrics` fires
  exactly once per produced frame (FR-007/SC-010). `FrameDuration` is real
  wall-clock for live diagnostics, excluded from goldens (FR-012). Baselines that
  cannot yet capture a counter (paint/composite/hit-test — introduced in deferred
  Phase 2/7) MUST state the omission explicitly (FR-015 resolution: no silent
  omission). No unsupported-environment diagnostic changes.
- **Deferred scope**: Phase 2+ is OUT (retained pointer routing, frame scheduler,
  narrowed visual-state stamping, view memoization, viewport virtualization,
  paint/damage caches, layout caches, backend review). The DataGrid 10000-row
  scenario is intentionally run on the **non-virtualized** path to capture the
  pre-virtualization baseline; it must not be "fixed" here. Paint/hit-test counters
  are added to the corpus only when their phases land.

**Gate result: PASS.** No principle violated; no complexity requiring
justification (plain records, a pure fold, golden comparison). Tier-1 obligations
(`.fsi` + baselines + tests + docs) are all enumerated above. Re-checked after
Phase 1 design below — still PASS (design adds no class/SRTP/reflection/CE; the
only new types are record fields and test fixtures).

## Project Structure

**Shipped surface (one file, breaking field change):**
- `src/Controls.Elmish/ControlsElmish.fsi` — `FrameMetrics`: remove `ViewRebuilt`;
  add `ProductModelChanged`, `ViewCalled`, `FullRenderCount` (with XML-doc).
- `src/Controls.Elmish/ControlsElmish.fs` — update the `FrameMetrics` record
  definition and **every** construction site:
  - `emitFrameMetrics` (the live `runInteractiveApp` sink, ~line 796) — compute
    `ProductModelChanged` (model identity changed) and `ViewCalled`/`FullRenderCount`
    (did `renderStep` run `host.View`) instead of the single `viewRebuilt` arg.
  - `Perf.runScript` `zero` record + each per-frame branch (move-coalesced,
    `Idle`, `Tick`, `Key`, discrete `Pointer`, ~lines 1048–1131) — split the
    existing `rebuilt` into `ProductModelChanged` and a separately-tracked
    `ViewCalled`/`FullRenderCount` (currently `renderStep` runs iff `rebuilt`/
    `hadAnimation`; make the metric report that fact rather than re-deriving it).

**Baselines regenerated (after the `.fsi` change):**
- `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt`
- `readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt`
  (both via `./fake.sh build -t RefreshSurfaceBaselines`).

**Tests / evidence projects (no shipped API):**
- `tests/Elmish.Tests/Feature109MetricsHonestyTests.fs` — US1/US3 metric-fact and
  coalescing-fidelity tests (new file, added to `Elmish.Tests.fsproj`).
- `tests/Elmish.Tests/Feature109CorpusTests.fs` (or a `PerfCorpus.fs` fixture +
  test) — the scenario corpus definitions, `Perf.runScript` golden runs, and
  golden re-run determinism (US2).
- Existing `tests/Elmish.Tests/Feature108MetricsTests.fs`,
  `Feature090DispatchTests.fs`, `Feature098DispatchTests.fs` read/construct
  `FrameMetrics` → update for the field rename (compile gate).
- Non-golden report generator: an Expecto evidence test (or small evidence
  harness) that runs the corpus and writes timing+allocation to
  `docs/reports/_baselines/` — never a FAKE gate.

**Evidence outputs:**
- `specs/109-perf-metrics-baseline/readiness/perf-corpus/*.golden.txt` (committed
  deterministic goldens).
- `docs/reports/_baselines/2026-06-12-controls-corpus-{before,after}.md` (timing/
  allocation + count-first thresholds; before/after hover-burst coalescing).
- Standard escalated `maintainer-verify` readiness set under
  `specs/109-perf-metrics-baseline/readiness/`.

## Phase 0 — Research (`research.md`)

Resolves: how to split `ViewRebuilt` truthfully against the existing
`Perf.runScript` control flow; what "full render" counts; how `ProductModelChanged`
is detected without an equality constraint on `'model`; corpus scenario shapes and
golden format; where the non-golden report lives. See [research.md](./research.md).
No `NEEDS CLARIFICATION` remain (all five spec ambiguities were resolved at
`/speckit-clarify` 2026-06-12).

## Phase 1 — Design & Contracts

- **Data model**: [data-model.md](./data-model.md) — the hardened `FrameMetrics`
  record, the `PerformanceScenario` corpus entity, the `BaselineRecord`, and the
  `Perf script` (`FrameInput` sequence).
- **Contract**: [contracts/frame-metrics.md](./contracts/frame-metrics.md) — the
  exact before/after `FrameMetrics` `.fsi` shape, per-field precise meaning
  (SC-011), and the golden serialization format (which fields are golden vs
  non-golden).
- **Quickstart**: [quickstart.md](./quickstart.md) — how to add a scenario, run
  the goldens, and regenerate baselines.
- **Agent context**: `AGENTS.md` SPECKIT marker updated to this plan.

## Re-evaluated Constitution Check (post-design): PASS

Design introduces no class hierarchy, SRTP, reflection, or non-trivial CE; the
hardened metric is a plain record, scenario corpus is a list of values, golden
comparison is string equality. Tier-1 `.fsi`/baseline/test/doc obligations are
all planned. Observation-only invariant (FR-020) holds — no render/layout/dispatch
path is edited.

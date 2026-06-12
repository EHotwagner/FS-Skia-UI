# Tasks: Controls Performance Baseline Corpus & Honest Frame Metrics

**Feature branch**: `109-perf-metrics-baseline`
**Spec**: `specs/109-perf-metrics-baseline/spec.md`
**Plan**: `specs/109-perf-metrics-baseline/plan.md` (with `research.md`,
`data-model.md`, `contracts/frame-metrics.md`, `quickstart.md`). Tasks are
derived from the plan, the detailed `spec.md` (FR-001..FR-020, SC-001..SC-011,
the Framework Governance Prompts), and the source report's Phase 0 + Phase 1
sections (`docs/reports/2026-06-12-1422-controls-performance-framework-research.md`).

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. **None planned for this feature** —
it is an observation-and-evidence feature; every metric, golden, and baseline
obligation has a real path through the deterministic `Perf.runScript` driver,
the real `RetainedRender.step`, and committed artifacts. No malformed-input or
forced error-path work is in scope.

## Vertical-slice rule (US phases)

A `[US*]` task may only be marked `[X]` when the change is reachable from a
user-facing entry point and that path was actually exercised. For this
feature the user-facing surface is the **observability contract**: the public
`FrameMetrics` record and the `Perf.runScript` evidence surface in
`FS.Skia.UI.Controls.Elmish`. `[X]` for a `[US*]` task therefore requires the
public `FrameMetrics` / `Perf.runScript` contract exercised (FSI against the
packed library or the test project loading it), the per-frame count/bool
metrics asserted against committed goldens, and — for the live-loop coalescing
and once-per-frame-emit claims (US3) — the real `runInteractiveApp` emit path
covered, not only `Perf.runScript`. Core/helper changes whose unit tests pass
green do **not** satisfy `[X]` for a `[US*]` task.

## Success-criterion → assertion mapping

Each headline SC is paired with a concrete enforcing assertion, noted on the
task line as `(SC-00x)`:

- SC-001 / SC-011 — three scripted frames assert `ProductModelChanged` /
  `ViewCalled` per code path; no surviving field conflates the two (T011/T015).
- SC-002 — K-sample burst asserts `PointerSamplesReceived = N` and
  `PointerMovesProcessed ≤ 1` incl. deferred (T021).
- SC-003 — burst interleaved with press/release/click/scroll: zero dropped (T022).
- SC-004 — idle frame asserts zero remeasure, zero processed moves,
  `ViewCalled = false` (T012).
- SC-005 — every corpus scenario has a byte-stable golden that re-runs
  identically, timing excluded (T017/T018).
- SC-006 — counts answer how many times `host.View` ran / full renders occurred
  per scenario (T019).
- SC-007 — hover/pointer-move burst has both before and after coalescing
  baselines in-repo (T026).
- SC-008 — at-rest rendered output + default host path byte-identical
  (FR-020) asserted (T030).
- SC-009 — non-golden timing/allocation report exists; none of its fields
  appears in any deterministic golden (T025).
- SC-010 — `OnFrameMetrics` fires exactly once per produced frame (T013).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US4]** — user-story scope
- **[T1]** — this whole feature is Tier 1 (contracted, breaking `ControlsElmish.fsi`
  change); per-task tier omitted (matches the spec's overall tier).

Every task has a matching entry in `tasks.deps.yml`; every line mirrors the
structured `skillist` via `[skillist: ...]` (`[skillist: []]` when empty).

## Canonical Verification Targets

`Route` is authoritative — run `./fake.sh build -t Route` against the real diff
and run only the gates it prints (`--enforce` for missing evidence). This
feature makes a **breaking public `.fsi` change** to `FrameMetrics` in
`FS.Skia.UI.Controls.Elmish`, so `Route` **escalates** to the
controls-public-surface (maintainer-verify) tier. The serialized,
non-concurrent FAKE-backed order for the escalated set is:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

`RefreshSurfaceBaselines` must regenerate the surface + per-package baselines
after the `FrameMetrics` field change, and every `FrameMetrics` construction
site (sample `EvidenceCommands.fs`, FSI preludes, tests) must be updated in the
same change or the build breaks. FAKE-backed targets share `.fake` state and
must run sequentially; non-FAKE file reads/checks may be parallel-safe.

## Governance risk levels

- **Small** — a test-only or corpus/golden edit that does not touch the public
  `.fsi`: focused validation is `Dev` plus the affected test list.
- **Medium** — the `FrameMetrics` `.fsi`/`.fs` field change and construction-site
  updates: focused validation is `RefreshSurfaceBaselines` + `Dev` +
  `GeneratedProductCheck`.
- **Broad** — full escalated six-target order above; required because the public
  `ControlsElmish.fsi` surface changes. Broad validation is mandatory before
  merge. Non-authoritative aggregate results (e.g. an `All`/aggregate run) are
  recorded as advisory only in `readiness/aggregate-hang-diagnostics.md`; the
  authoritative verdict is the focused per-target rerun.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold `specs/109-perf-metrics-baseline/readiness/` with the audit-enforced placeholder files discoverable before implementation: `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation.md`, `skill-loading-evidence.md`, `evidence-graph.md`, `evidence-audit.md`, plus `window-visibility.md` and `real-image-evidence.md` not-applicable stubs (observation-only feature, no window launch). Each names its authoritative command, artifact path, failure class, and next action.
- [X] T002 [P] [skillist: []] Record the feature Tier (Tier 1, breaking `ControlsElmish.fsi`), affected layer (`FS.Skia.UI.Controls.Elmish` observability surface only), public-API impact (remove `ViewRebuilt`; add `ProductModelChanged`, `ViewCalled`, `FullRenderCount`), Elmish/MVU applicability (MVU semantics unchanged — observation only), the small/medium/broad governance risk levels, and the required evidence obligations into `readiness/`.
- [X] T003 [P] [skillist: fs-skia-evidence-mode] Establish the in-repo baseline area `docs/reports/_baselines/` for this feature (a `109-` baseline record skeleton) and the deterministic-evidence honesty note that timing/allocation are human-facing only and never gate (counts gate, timing informs).

---

## Phase 2: Foundation — the `FrameMetrics` contract change (linchpin)

- [X] T004 [skillist: fs-skia-controls-host] Draft the reshaped public `FrameMetrics` record in `src/Controls.Elmish/ControlsElmish.fsi`: **remove** `ViewRebuilt`; **add** `ProductModelChanged: bool` and `ViewCalled: bool` (FR-001/FR-002), **add** `FullRenderCount: int` (FR-015); keep `RemeasuredNodeCount`, `PointerSamplesReceived`, `PointerMovesProcessed`, `FrameDuration`. Write a `///` XML-doc line on every changed/new field giving its single precise meaning (doc-preservation gate; SC-011), with the attribute-before-doc-before-type ordering the XML-doc gate requires.
- [X] T005 [skillist: fs-skia-controls-host, fs-skia-reconciliation] Update the `FrameMetrics` type in `src/Controls.Elmish/ControlsElmish.fs` to match the new `.fsi`, and thread the real facts through `Perf.runScript`: `ProductModelChanged` = a product message changed the model; `ViewCalled` = `host.View size model` actually ran for the frame (true on the animation-only tick path where it runs with no product message); `FullRenderCount` = count of full `host.View` + `Control.renderTree` rebuilds for the frame. Keep deterministic counts byte-stable; do not alter render/layout/dispatch behavior (FR-020).
- [X] T006 [skillist: fs-skia-controls-host] Thread the same three fields through the live `runInteractiveApp` emit path (`emitFrameMetrics`) so the live `OnFrameMetrics` sink reports the same code-path facts as `Perf.runScript`, preserving inert at-rest defaults.
- [X] T007 [skillist: fs-skia-template-update] Update **every** `FrameMetrics` record construction/read site in the same change so the build stays green: the existing tests that construct or read `ViewRebuilt` (`tests/Elmish.Tests/Feature108MetricsTests.fs`, `Feature090DispatchTests.fs`, `Feature098DispatchTests.fs`) — replacing `ViewRebuilt` with the new fields. Confirm (per plan research D8) that the `OnFrameMetrics = ignore` sites (`template/base/src/Product/EvidenceCommands.fs`, `tests/SkiaViewer.Tests/Feature085InteractiveHostTests.fs`) set a host *field*, **not** a `FrameMetrics` record, and therefore need no edit; and that no `scripts/*-prelude.fsx` FSI prelude constructs a `FrameMetrics` (grep-clean), so none needs updating.
- [X] T008 [skillist: fs-skia-template-update] Run `./fake.sh build -t RefreshSurfaceBaselines` to regenerate `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt` and `readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt` for the field change, and confirm the only shipped-surface delta is the `FrameMetrics` fields (no other API moved).
- [X] T009 [skillist: []] Exercise the reshaped `FrameMetrics` / `Perf.runScript` surface from FSI against the built library and capture the transcript to `readiness/fsi-session.txt`, showing the new fields populated for representative frames.

**Checkpoint**: Foundation ready — the new contract compiles, all sites updated, baselines regenerated. Story implementation may begin.

---

## Phase 3: User Story 1 — Per-frame metrics tell the truth (US1, P1)

### Tests First (Principle I, Principle VI)

- [X] T010 [P] [US1] [skillist: fs-skia-controls-host] Add deterministic `Perf.runScript` tests in `tests/Elmish.Tests/` for the three scripted frames of the Independent Test: (a) a frame with no product message → `ProductModelChanged = false`; (b) a product message that changes the model with no visual difference → `ProductModelChanged = true` with `RemeasuredNodeCount`/`FullRenderCount` reporting the *actual* work, no field implying more (FR-003/FR-004); (c) a host-owned hover/focus/animation change with no product message → `ProductModelChanged = false` while `ViewCalled` and the real per-frame work are reported truthfully (FR-005) (SC-001).
- [X] T011 [P] [US1] [skillist: fs-skia-controls-host] Add a test asserting that for every produced frame each field's meaning is single and precise — `ProductModelChanged` and `ViewCalled` can diverge (animation-only tick: `ProductModelChanged = false`, `ViewCalled = true`) — so no surviving field conflates "model changed" with "view ran" (SC-011).
- [X] T012 [P] [US1] [skillist: fs-skia-controls-host] Add an idle-frame test: zero remeasured nodes, zero pointer moves processed, `ViewCalled = false`, unless an active animation clock or explicit tick requires work (FR-006, SC-004).
- [X] T013 [P] [US1] [skillist: fs-skia-controls-host] Add a live-loop test that `OnFrameMetrics` fires **exactly once** per produced frame (not once per incidental flush boundary, not with ambiguous aggregated counts) (FR-007, SC-010).

### Implementation

- [X] T014 [US1] [skillist: fs-skia-controls-host, fs-skia-reconciliation] Make `ProductModelChanged` / `ViewCalled` / `FullRenderCount` truthful across all `Perf.runScript` frame arms (coalesced-move, idle, tick/animation, key, discrete-pointer) so each reports its real code path — in particular `ViewCalled = true` on the animation-only tick where `renderStep` runs with no product message, and the pointer-routing `host.View` call is accounted for honestly in `FullRenderCount` (FR-001..FR-006).
- [X] T015 [US1] [skillist: fs-skia-controls-host] Enforce once-per-frame `OnFrameMetrics` emission on the live `runInteractiveApp` loop (FR-007) and document the precise meaning of each `FrameMetrics` field (the reviewer-nameable single meaning, SC-011) in `readiness/`.
- [X] T016 [US1] [skillist: []] Document US1's independent validation path (drive the three scripted frames through `Perf.runScript`; assert view/model fields match the code path in every case) in `readiness/`.

**Checkpoint**: US1 is independently functional — metrics report code-path facts, verifiable through `Perf.runScript` and the live emit path.

---

## Phase 4: User Story 2 — Reproducible scenario corpus with deterministic goldens (US2, P1)

### Tests First

- [X] T017 [P] [US2] [skillist: fs-skia-evidence-mode, fs-skia-testing] Add the corpus golden harness in the test/evidence project: for each scenario, drive it through `Perf.runScript`, assert the per-frame count/boolean metrics against a committed golden, and re-run to confirm byte-for-byte identity (timing fields excluded) (FR-014, SC-005).

### Implementation

- [X] T018 [US2] [skillist: fs-skia-ui-widgets, fs-skia-testing] Author the scenario corpus driver and fixtures in **test/evidence projects only** (no new shipped `Controls.Elmish` API) covering FR-013: hover sweep across 100 / 1000 / 5000 simple controls; DataGrid at 100 / 1000 / 10000 rows against the **current fully-materialized** path (pre-virtualization baseline, not "fixed" here); deep nested layout of repeated labels and buttons; text entry in a focused field while unrelated controls animate; theme switch across a moderate dashboard; continuous drag/freehand path of hundreds of raw samples.
- [X] T019 [US2] [skillist: fs-skia-evidence-mode] Commit the per-scenario deterministic metrics goldens (counts + booleans only) under the feature evidence area, and make the evidence answer in counts, per scripted interaction, how many times `host.View` ran, how many full renders occurred, and how many nodes were remeasured (FR-015, SC-006). The baseline MUST explicitly state which phase counters are **not yet captured** (paint / composite / hit-test arrive in later phases — silent omission is not acceptable).

**Checkpoint**: US2 is independently functional — every corpus scenario has a byte-stable golden that re-runs identically.

---

## Phase 5: User Story 3 — Coalescing fidelity is verified and load-bearing (US3, P2)

### Tests First

- [X] T020 [P] [US3] [skillist: fs-skia-controls-host] Add a test that for N raw pointer-move samples in one frame (including any deferred/queued from a prior boundary) the reported `PointerSamplesReceived = N` and `PointerMovesProcessed ≤ 1` (FR-008/FR-009, SC-002).
- [X] T021 [P] [US3] [skillist: fs-skia-controls-host] Add a test that a move burst interleaved with a press, release, click, and scroll drops **none** of the discrete interactions (FR-010, SC-003), and a test that a continuous drag/freehand gesture of hundreds of samples keeps its raw path available to path-consuming consumers (FR-011).

### Implementation

- [X] T022 [US3] [skillist: fs-skia-controls-host] Verify and make load-bearing the feature-108 coalescing on both `Perf.runScript` and the live `runInteractiveApp` loop: `PointerSamplesReceived` counts raw native samples including deferred moves (FR-008), bursts collapse to ≤ 1 processed move (FR-009), discrete press/release/click/scroll are never coalesced or dropped (FR-010), and the raw drag path remains obtainable for path-consuming routing/repaint (FR-011) — without changing dispatch behavior.

**Checkpoint**: US3 is independently functional — coalescing fidelity is asserted, not assumed.

---

## Phase 6: User Story 4 — Honest before/after baselines and a non-golden timing report (US4, P2)

### Tests First

- [X] T023 [P] [US4] [skillist: fs-skia-evidence-mode] Add a test/assertion that `FrameDuration` is real wall-clock timing for live diagnostics and is **excluded** from every deterministic golden assertion (FR-012), and that timing/allocation fields are absent from the deterministic goldens (SC-009).

### Implementation

- [X] T024 [US4] [skillist: fs-skia-evidence-mode, fs-skia-testing] Add a **non-golden** benchmark/report generator (a local report command in the test/evidence project) that captures per-scenario timing and allocation fields, kept strictly separate from the deterministic goldens (FR-016).
- [X] T025 [US4] [skillist: fs-skia-evidence-mode] Store the captured "before" baseline numbers in-repo under `docs/reports/_baselines/` (FR-017) and define the regression thresholds in deterministic **counts first, timing second** (FR-018), recording that none of the timing/allocation fields appears in any golden (SC-009).
- [X] T026 [US4] [skillist: fs-skia-evidence-mode] Record **both** a before-coalescing and an after-coalescing feature-108 baseline for a hover/pointer-move burst under `docs/reports/_baselines/`, so the coalescing benefit is evidenced rather than asserted (FR-019, SC-007).

**Checkpoint**: US4 is independently functional — real timing/allocation captured and stored, strictly separate from gating counts.

---

## Phase 7: Integration & Polish

- [X] T027 [skillist: fs-skia-controls-host] Assert the observation-only invariant (FR-020, SC-008): at-rest rendered output, control geometry, dispatch behavior, and the default (non-observing) host path are byte-identical to the pre-feature state — the `FrameMetrics` field change and `FullRenderCount` addition change the observability surface only, no rendered pixel / layout box / dispatch outcome.
- [X] T028 [skillist: fs-skia-evidence-mode] Author the skill-loading evidence (`readiness/skill-loading-evidence.md`, one row per (task, skill)), the window-visibility not-applicable set (this feature launches no window), the `readiness/evidence-audit.md` verdict token, and the `readiness/generated-validation.md` package-resolution tokens (`package-resolution=resolved`, `package-mismatch=false`).
- [X] T029 [skillist: fsharp-build-orchestration] Run `Route` then the serialized escalated controls-public-surface gate order (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck`) sequentially (shared `.fake` state — never concurrent), recording focused per-target verdicts and any non-authoritative aggregate result as advisory in `readiness/aggregate-hang-diagnostics.md`.
- [X] T030 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, and no `[S*]` surprises; confirm the echoed `feature-directory=specs/109-perf-metrics-baseline` and `tasks=<n>` match this feature.
- [X] T031 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm the verdict is PASS (no `[S]`/`[S*]`, no diff-scan hits) or document every `--accept-synthetic` override.

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. **None planned**
— this feature's obligations all have real evidence paths (deterministic
`Perf.runScript` goldens, real `RetainedRender.step` counts, committed
baselines, live `runInteractiveApp` emit-path coverage).

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |

# Feature Specification: Controls Performance Baseline Corpus & Honest Frame Metrics

**Feature Branch**: `109-perf-metrics-baseline`
**Created**: 2026-06-12
**Status**: Draft
**Input**: User description: "@docs/reports/2026-06-12-1422-controls-performance-framework-research.md do first part."

**Source report** (local in-repo report, not a remote URL — no `source-spec.md`
snapshot per the specify FR-016 no-op rule):
`docs/reports/2026-06-12-1422-controls-performance-framework-research.md`. This
feature implements the **first part** of that report's staged implementation
plan — **Phase 0 (Baseline and Guardrails)** and **Phase 1 (Finish and Correct
Feature 108 Metrics and Coalescing)** — which are also the report's "Do first"
priorities #1 (verify feature 108 metrics semantics) and #3 (add phase-complete
metrics and before/after baselines). Everything from Phase 2 onward (retained
pointer routing, frame scheduler, narrowed visual-state stamping, memoization,
virtualization, paint/damage caches, layout caches, backend review) is **out of
scope** — see *Unsupported scope*.

## Why this feature (context)

Feature 108 (merged 2026-06-12) added the first slice of host-loop
observability: a `FrameMetrics` record, the deterministic `Perf.runScript`
driver, and pointer-move coalescing. The research report's central caution is
that **metrics names harden into contract quickly and must be precise before
they do**, and that no optimization should be accepted on anecdotal smoothness.
Today `FrameMetrics.ViewRebuilt` is a *semantic approximation* — it answers "did
a product message change the model?" rather than "did `host.View` actually run?"
Those are different facts, and the report requires them separated before the
metric becomes load-bearing. This feature makes the existing metrics **truthful
and load-bearing**, and establishes a **reproducible scenario corpus with honest
before/after baselines** so that the later optimization phases (Phase 2+) have a
trustworthy yardstick. It deliberately changes **no rendering, layout, or
dispatch behavior** — it is an observation-and-evidence feature.

## Clarifications

### Session 2026-06-12

- Q: Is "do first part" exactly Phase 0 + Phase 1 of the source report? → A: Yes —
  Phase 0 (baseline corpus + guardrails) + Phase 1 (correct feature 108
  metrics/coalescing); Phase 2+ deferred (confirmed, observation-only feature).
- Q: How should the ambiguous `FrameMetrics.ViewRebuilt` field be reshaped? → A:
  **Replace it** — remove `ViewRebuilt`; add two precise booleans
  `ProductModelChanged` and `ViewCalled`. Breaking surface change; no conflated
  name survives (satisfies SC-011).
- Q: Add a dedicated full-render counter to answer SC-006/FR-015? → A: Yes — add a
  deterministic int `FullRenderCount` field counting full `host.View`+`renderTree`
  rebuilds per frame.
- Q: Where do the scenario-corpus driver and timing/allocation baselines live? →
  A: Corpus driver in test/evidence projects; baselines under
  `docs/reports/_baselines/`. **No new shipped `Controls.Elmish` API** for the
  corpus (package surface stays minimal aside from the `FrameMetrics` field
  change).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Per-frame metrics tell the truth about what work ran (Priority: P1)

A framework maintainer (or the evidence tooling) reads the per-frame metrics
record to decide whether a frame did avoidable work. Today they cannot trust the
record to distinguish "the consumer's model changed" from "`host.View` actually
re-ran to produce a new tree for diffing" — `ViewRebuilt` conflates them. Before
any optimization phase can prove it removed work, the metric must report
**code-path facts**, not an approximation.

**Why this priority**: Per the report, metric *semantics* are the first "Do
first" item and a prerequisite for every later phase — an optimization cannot be
proven against a metric that does not mean what it says, and the name hardens
into public contract on the next version bump. Independently valuable: even with
no optimization landed, a precise metric turns an invisible regression into an
observable one.

**Independent Test**: Drive three deterministic scripted frames through the pure
host update path — (a) a frame that produces **no product message**, (b) a frame
whose product message changes the model but produces **no visual change**, and
(c) a **host-owned visual-state change** (hover/focus/animation) with **no
product message** — and assert that the metrics record's view/model fields match
the actual code path in every case.

**Acceptance Scenarios**:

1. **Given** a scripted frame that produces no product message, **When** the
   frame is processed, **Then** the metrics report that the product model did not
   change AND report truthfully whether `host.View` ran.
2. **Given** a product message that changes the model but yields no visual
   difference, **When** the frame is processed, **Then** the metrics report the
   model changed and report the actual remeasure/repaint work (which may be zero
   beyond what truly happened), with no field implying more work than occurred.
3. **Given** a host-owned hover/focus/animation change with no product message,
   **When** the frame is processed, **Then** the metrics report the product model
   did NOT change while still reporting the real per-frame work.
4. **Given** an idle frame with no input and no active animation, **When** it is
   processed, **Then** the metrics report zero remeasured nodes, zero pointer
   moves processed, and that `host.View` did not run.
5. **Given** any produced frame, **When** metrics are emitted, **Then**
   `OnFrameMetrics` fires **exactly once** for that frame (not once per incidental
   flush boundary with ambiguous counts).

---

### User Story 2 - A reproducible performance scenario corpus with deterministic goldens (Priority: P1)

A maintainer needs a fixed set of representative interaction scenarios that each
produce **byte-stable per-frame metric goldens**, so that "we made it faster" can
be expressed as "this scenario now does fewer counted operations" rather than a
feeling. The corpus is the yardstick every later optimization phase is measured
against.

**Why this priority**: The report's Phase 0 makes the corpus the guardrail that
precedes any behavior change ("no optimization is accepted solely on anecdotal
smoothness"). It is independently valuable: the corpus + goldens are a standing
regression net even before a single optimization lands.

**Independent Test**: For each corpus scenario, drive it through the
deterministic `Perf.runScript` path and assert the per-frame count/boolean
metrics against a committed golden; re-run and confirm the golden is identical.

**Acceptance Scenarios**:

1. **Given** the scenario corpus, **When** each scenario is run through
   `Perf.runScript`, **Then** each emits a deterministic per-frame metrics record
   that matches its committed golden byte-for-byte.
2. **Given** a corpus scenario, **When** it is re-run on the same inputs, **Then**
   the deterministic counts/booleans are identical run-to-run (timing fields
   excluded).
3. **Given** a scripted interaction in the corpus, **When** the evidence is
   inspected, **Then** it answers, in counts, how many times `host.View` ran and
   how many full renders occurred for that interaction.
4. **Given** the DataGrid scenarios at 100 / 1000 / 10000 rows, **When** they are
   run, **Then** they execute against the **current** (fully-materialized)
   rendering path and record its cost as the baseline that later virtualization
   (Phase 6, out of scope here) will be measured against.

---

### User Story 3 - Coalescing fidelity is verified and load-bearing (Priority: P2)

A maintainer must be sure feature 108's pointer-move coalescing is honest: that
the sample counter reflects raw native samples (including deferred ones), that a
burst collapses to at most one processed move per frame, and that coalescing
never silently drops a discrete press/release/click/scroll or corrupts a
drag/freehand path.

**Why this priority**: The report's Phase 1 requires the coalescing metrics to be
verified, not assumed; input fidelity is called out as a top risk. It is P2
because the mechanism already exists from 108 — this story hardens and proves it
rather than building it.

**Independent Test**: Script a burst of many pointer-move samples within one
frame interleaved with a discrete click and a scroll; assert the raw sample count
equals the samples sent, processed moves ≤ 1, the click and scroll are both
dispatched, and a scripted drag retains its raw path for path-consuming
consumers.

**Acceptance Scenarios**:

1. **Given** N raw pointer-move samples arriving for one frame (including any
   deferred from a prior boundary), **When** the frame is processed, **Then** the
   reported received-sample count equals N and the processed-move count is ≤ 1.
2. **Given** a move burst interleaved with a press, release, click, and scroll,
   **When** the frame is processed, **Then** none of the discrete interactions is
   dropped.
3. **Given** a continuous drag/freehand gesture of hundreds of samples, **When**
   it is coalesced for routing/repaint, **Then** the raw path remains available
   to consumers that need it (path fidelity preserved).

---

### User Story 4 - Honest before/after baselines and a non-golden timing report (Priority: P2)

A maintainer wants real timing and allocation numbers per scenario to guide
prioritization, captured **separately** from the deterministic goldens (timing
varies by machine and must never gate). They also want the "before" numbers
recorded in-repo so later phases can show a real delta, and regression thresholds
defined counts-first.

**Why this priority**: Phase 0 requires a non-golden benchmark/report generator
and stored baselines, and Phase 1 requires `FrameDuration` to be real timing kept
out of goldens. P2 because it supports — but does not block — the metric-honesty
core (US1) and the corpus (US2).

**Independent Test**: Run the non-golden report generator over the corpus; assert
it emits timing and allocation fields per scenario into the baselines area, that
those fields are absent from the deterministic goldens, and that a hover-burst
scenario has both a before and an after baseline recorded.

**Acceptance Scenarios**:

1. **Given** the corpus, **When** the non-golden report generator runs, **Then**
   each scenario gets recorded timing and allocation fields, stored in the in-repo
   baselines area, and none of those fields appears in any deterministic golden.
2. **Given** the feature 108 coalescing, **When** a hover/pointer-move burst is
   benchmarked, **Then** both a before-coalescing and an after-coalescing baseline
   exist in-repo so the benefit is evidenced rather than asserted.
3. **Given** regression thresholds, **When** they are defined, **Then** they are
   expressed in deterministic counts first and timing second.

---

## Requirements *(mandatory)*

### Functional Requirements

**Metric semantics (Phase 1)**

- **FR-001**: The per-frame metrics record MUST distinguish two facts that are
  currently conflated: (a) **a product message changed the model**, and (b)
  **`host.View` actually ran** to produce a new tree for diffing. These MUST be
  reported as **two separate booleans** — `ProductModelChanged` and `ViewCalled`
  — each individually testable.
- **FR-002**: The existing `ViewRebuilt` field MUST be **removed** from the
  `FrameMetrics` record (not retained as a deprecated alias) and replaced by the
  `ProductModelChanged` + `ViewCalled` booleans of FR-001, so no field whose name
  conflates "model changed" with "view ran" survives onto the public surface
  (clarified 2026-06-12). This is a breaking surface change: every `FrameMetrics`
  construction and read site (samples, FSI preludes, tests, surface/per-package
  baselines) MUST be updated in the same change.
- **FR-003**: A frame producing no product message MUST report model-changed =
  false.
- **FR-004**: A frame whose product message changes the model but yields no
  visual difference MUST report model-changed = true while reporting the *actual*
  remeasure/repaint work; no field may imply more work than the code path
  performed.
- **FR-005**: A host-owned visual-state change (hover / focus / press / animation
  clock) with no product message MUST report model-changed = false while still
  reporting the real per-frame work counts.
- **FR-006**: Idle frames MUST report zero remeasured nodes, zero pointer moves
  processed, and view-called = false, UNLESS an active animation clock or an
  explicit tick requires work.
- **FR-007**: The metrics sink (`OnFrameMetrics`) MUST fire **exactly once per
  produced frame**, not once per incidental flush boundary and not with ambiguous
  aggregated counts.

**Coalescing fidelity (Phase 1)**

- **FR-008**: `PointerSamplesReceived` MUST count the raw native pointer samples
  that arrived for the frame, including any deferred/queued moves carried from a
  prior boundary.
- **FR-009**: For a burst of pointer-move samples within a single frame,
  `PointerMovesProcessed` MUST be ≤ 1.
- **FR-010**: Coalescing MUST never drop a discrete pointer interaction —
  press, release, click, or scroll.
- **FR-011**: Drag/freehand **path fidelity** MUST be preserved: consumers that
  need the raw sample path MUST still be able to obtain it even though moves are
  coalesced for routing/repaint.

**Timing (Phase 1)**

- **FR-012**: `FrameDuration` MUST be real wall-clock timing for live diagnostics
  and MUST remain EXCLUDED from deterministic golden assertions.

**Scenario corpus & evidence (Phase 0)**

- **FR-013**: A controls performance scenario corpus MUST exist covering at
  least: a hover sweep across **100 / 1000 / 5000** simple controls; a DataGrid
  at **100 / 1000 / 10000** rows; a deep nested layout of repeated labels and
  buttons; text entry in a focused field while unrelated controls animate; a
  theme switch across a moderate dashboard; and a continuous drag/freehand path
  of hundreds of raw samples.
- **FR-014**: Each corpus scenario MUST drive the deterministic `Perf.runScript`
  path and produce a **byte-stable** per-frame metrics golden consisting of
  counts and booleans only.
- **FR-015**: For each scripted interaction the evidence MUST be able to answer,
  in counts, how many times `host.View` ran, how many **full renders** occurred,
  and how many nodes were remeasured. To make the full-render count first-class
  and golden-assertable, `FrameMetrics` MUST gain a deterministic integer
  `FullRenderCount` field that counts full `host.View` + `Control.renderTree`
  rebuilds for the frame (clarified 2026-06-12); the host.View-ran fact is the
  `ViewCalled` boolean (FR-001) and remeasure is `RemeasuredNodeCount`.
  > **Interacting requirement (resolution).** The source report's Phase 0
  > acceptance also lists *paint* and *hit-test* counts. Those phase counters do
  > not yet exist and are introduced in later phases (paint/damage in Phase 7,
  > retained hit-test routing in Phase 2 — both out of scope here). Resolution:
  > this feature's baselines report the counters that exist after Phase 1
  > (view-called, full-render, remeasured-node, pointer sample/move); the corpus
  > is **extended** with paint/composite/hit-test counters when those phases land.
  > Silent omission is not acceptable — the baseline MUST state which phase
  > counters are not yet captured.
- **FR-016**: A **non-golden** benchmark/report generator MUST capture timing and
  allocation fields per scenario, kept strictly separate from the deterministic
  goldens.
- **FR-017**: Captured "before" baseline numbers MUST be stored in-repo under
  `docs/reports/_baselines/` (clarified 2026-06-12).
- **FR-018**: Regression thresholds MUST be defined in deterministic counts
  first, timing second.
- **FR-019**: A hover/pointer-move burst MUST have both a **before** and an
  **after** feature-108-coalescing baseline recorded in-repo, so the coalescing
  benefit is evidenced rather than asserted.

**Behavior preservation (cross-cutting)**

- **FR-020**: All changes in this feature MUST be **observation-and-evidence
  only**. At-rest rendered output, control geometry, dispatch behavior, and the
  default (non-observing) host path MUST remain **byte-identical** to the
  pre-feature state. No control rendering, layout, hit-testing, or input
  semantics may change. Removing/replacing the `FrameMetrics` fields (FR-002) and
  adding `FullRenderCount` (FR-015) change the observability surface only — the
  rendered scene and default host behavior still MUST NOT change.

> Interacting / conflicting requirements: FR-002 (breaking change to the public
> `FrameMetrics` field set) vs FR-020 (byte-identical default behavior) —
> resolution: the *shape* of the `FrameMetrics` contract changes (it is
> observability surface, not render surface), but no rendered pixel, layout box, or
> dispatch outcome may change. A field set change is a surface/baseline update, not
> a behavior change. FR-013's
> 10000-row DataGrid scenario vs the absence of virtualization — resolution: the
> scenario runs against today's fully-materialized path on purpose, to record the
> pre-virtualization baseline; it must not be "fixed" by adding virtualization
> here (that is Phase 6, out of scope).

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Touches `FS.Skia.UI.Controls.Elmish` (the `FrameMetrics`
  record, `InteractiveAppHost.OnFrameMetrics`, the `Perf.runScript` driver, and
  the pointer coalescing in `runInteractiveApp`). The DataGrid corpus scenario
  authors against the **current active** `FS.Skia.UI.Controls` DataGrid surface
  (no Charts/legacy migration involved). No package identity changes; package
  **contents** of `Controls.Elmish` change and its version bumps on merge. The
  scenario-corpus driver and fixtures live in **test/evidence projects only**, not
  shipped packages — no reusable perf-corpus helper is added to `Controls.Elmish`
  (clarified 2026-06-12), so the only shipped-surface delta is the `FrameMetrics`
  field change below.
- **Public contract impact**: `FrameMetrics` field set changes — `ViewRebuilt` is
  **removed** and replaced by two booleans `ProductModelChanged` + `ViewCalled`,
  and the record **gains** an integer `FullRenderCount` field (FR-001/FR-002/
  FR-015). This is a breaking public `.fsi` change in `ControlsElmish.fsi`, so
  surface baselines and
  per-package baselines update and `Route` escalates to the
  controls-public-surface tier. XML-doc on every changed/new field is required
  (doc-preservation gate). `Perf.runScript` evidence surface gains the corpus
  scenarios.
- **State workflow impact**: None to MVU semantics — `Update`, effects,
  subscriptions, commands, and interpreter behavior are unchanged. The pointer
  queue/coalescing already exists (feature 108); this feature **verifies and
  reports** it (FR-008..FR-011) without changing its dispatch behavior.
- **Layout/rendering impact**: None functionally — observation-only, at-rest
  byte-identical (FR-020). The corpus drives the existing render/layout path;
  `RemeasuredNodeCount` and the render/view counters describe that path, they do
  not alter it. No Vulkan/Skia/visual-output change; no unsupported-environment
  diagnostic change.
- **Evidence obligations**: Per-scenario deterministic metrics goldens under the
  feature evidence area; a non-golden timing/allocation report and stored
  before/after baselines under `docs/reports/_baselines/` (FR-016/FR-017/FR-019);
  skill-loading evidence; window-visibility not-applicable set if the audit fires
  on literal filenames; `readiness/evidence-audit.md` with a verdict token; the
  generated-validation package-resolution tokens. The standard escalated
  `maintainer-verify` readiness set applies because of the `.fsi` change.
- **Unsupported scope**: This feature is **Phase 0 + Phase 1 only**. Explicitly
  OUT: retained-frame pointer routing / removing full-render pointer dispatch
  (Phase 2), the `FrameCause`/`FrameInvalidation` frame scheduler (Phase 3),
  narrowed runtime visual-state stamping (Phase 4), view/control memoization
  (Phase 5), viewport virtualization for DataGrid/list (Phase 6), damage
  rectangles / Skia picture / paint caches (Phase 7), text-measurement / layout
  boundary caches (Phase 8), and any `SkiaViewer` backend / render-thread /
  compositor review (Phase 9). No renderer rewrite, no Avalonia/WPF redesign, no
  new platform/release/distribution scope.
- **Build-target impact**: Escalated controls-public-surface set because of the
  `ControlsElmish.fsi` change: `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`,
  `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit` (run `Route` first and
  obey its printed list). `RefreshSurfaceBaselines` must regenerate the surface +
  per-package baselines after the `FrameMetrics` field change, and every
  `FrameMetrics` construction site (samples, FSI preludes) must be updated or the
  build breaks. No new gate is introduced.

## Success Criteria *(mandatory)*

- **SC-001**: For three scripted frames — (a) no product message, (b) a product
  message with no visual change, (c) a host visual-state change with no product
  message — the metrics record matches the code-path facts in **every** field.
- **SC-002**: A burst of N pointer-move samples in a single frame reports
  received-samples = N and processed-moves ≤ 1.
- **SC-003**: Across the scripted scenarios, **100%** of discrete interactions
  (press, release, click, scroll) are retained under coalescing — zero dropped.
- **SC-004**: Idle frames report zero remeasured nodes, zero processed moves, and
  view-called = false, unless an animation clock or explicit tick is active.
- **SC-005**: Every corpus scenario has a byte-stable deterministic metrics
  golden that re-runs identically (timing fields excluded).
- **SC-006**: For each scenario the evidence answers, in counts, how many times
  `host.View` ran and how many full renders occurred.
- **SC-007**: A hover/pointer-move burst has **both** a before and an after
  coalescing baseline recorded in-repo.
- **SC-008**: At-rest rendered output and the default (non-observing) host path
  are byte-identical to the pre-feature state (observation-only).
- **SC-009**: A non-golden timing/allocation report exists for every scenario and
  none of its timing/allocation fields appears in any deterministic golden.
- **SC-010**: `OnFrameMetrics` fires exactly once per produced frame across all
  scenarios.
- **SC-011**: The hardened `FrameMetrics` surface contains no field whose name
  conflates "model changed" with "view ran"; reviewers can name each field's
  single precise meaning.

## Key Entities

- **FrameMetrics record**: the per-frame work signal. Deterministic/golden
  fields — `ProductModelChanged` (bool), `ViewCalled` (bool), `FullRenderCount`
  (int), `RemeasuredNodeCount` (int), `PointerSamplesReceived` (int),
  `PointerMovesProcessed` (int). Removed: `ViewRebuilt`. Non-golden field —
  `FrameDuration` (and, via the report generator, allocation). The contract this
  feature makes precise.
- **Performance scenario**: a named, parameterized interaction (control count /
  row count / scripted `FrameInput` sequence) in the corpus, each with a
  committed deterministic golden.
- **Baseline record**: stored before/after timing + allocation numbers per
  scenario, plus the count-based regression thresholds, kept in-repo and out of
  the goldens.
- **Perf script**: an ordered `FrameInput` sequence (`Key` / `Pointer` / `Tick` /
  `Idle`) driven through the deterministic `Perf.runScript` path.

## Assumptions

- **"First part" = Phase 0 + Phase 1** (confirmed at `/speckit-clarify`
  2026-06-12). The report stages its plan as Phase 0 (Baseline and Guardrails)
  then Phase 1 (Finish and Correct Feature 108 Metrics and Coalescing); these are
  also "Do first" priorities #1 and #3. This feature scopes to exactly those two
  phases; Phase 2+ is out of scope.
- Feature 108's `FrameMetrics`, `Perf.runScript`, and pointer coalescing are
  already merged and are the foundation this feature corrects/extends — not
  rebuilt.
- The scenario corpus is authored against existing control kinds and the current
  DataGrid rendering path; no new control kinds are introduced.
- Baseline timing/allocation numbers are environment-dependent and are recorded as
  human-facing evidence only; they never gate (counts gate, timing informs).
- The DataGrid 10000-row scenario is intentionally run on the fully-materialized
  (non-virtualized) path to capture the pre-virtualization baseline.

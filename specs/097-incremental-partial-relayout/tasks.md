# Tasks: Incremental Measure / Partial Re-Layout (R2)

**Feature branch**: `097-incremental-partial-relayout`
**Spec**: `specs/097-incremental-partial-relayout/spec.md`
**Plan**: `specs/097-incremental-partial-relayout/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

`[SEH]` is an annotation for design-approved synthetic error-handling work; it
remains `[S]` when completed. **None planned for this feature** — `evaluateIncremental`
is a pure, total evaluator (a cache miss / unknown dirty id degrades to a full
re-measure of that subtree, never a throw, contract C1) and the dirty-set
derivation is a pure, total walk over the patch (an unrecognized node contributes
no dirt, never an exception). There is no runtime error path to fixture: all
equivalence, dirty-derivation, metric, and byte-identity evidence is real (FsCheck
real trees/edit-sequences, exact `Bounds`/`Scene` structural equality, the **live**
wired `RetainedRender.step` path, real surface baselines). Any `[S]` that appears
triggers the full Principle V disclosure regime.

## Tier & MVU posture

This is a **Tier 2 (internal change) with one public-behavior nuance**. R2 changes
only the **body** of the already-public `Layout.evaluateIncremental`
(`src/Layout/Layout.fsi:10`) — its **signature is unchanged** — and the observable
runtime **value** of `LayoutResult.Invalidated` (stub echo → honest post-propagation
set, FR-001a). No `.fsi` symbol is added or moved; `LayoutResult` keeps its shape
(`Revision`/`Invalidated` already present). The measure/bounds cache (on the internal
`RenderFragment`/`RetainedNode`), the previous-`LayoutResult` carry (internal
`RetainedRender<'msg>`), the internal incremental `ControlInternals.evaluateLayout`
seam, and the extended `WorkReductionRecord` field (`RemeasuredNodeCount`) are all
**internal**, reached by `Layout.Tests`/`Controls.Tests` via the existing
`InternalsVisibleTo`. Surface-area baselines (`FS.Skia.UI.Layout`, per-package,
cross-package) are committed **unchanged** (SC-006). Because the tier is uniform,
per-task `[T2]` marks are omitted.

**MVU/Elmish is untouched.** R2 adds **no** `Model`/`Msg`/`Effect`/`Cmd`/`init`/`update`.
The incremental evaluator, the dirty-set derivation, and the measure-cache reuse are
**pure** functions of `(previous LayoutResult, patch-derived dirty set, new tree)`;
they own no mutable state beyond the per-step re-measure counter / cache already
confined to the `RetainedRender.step` interpreter edge. The existing
`LayoutWorkflowModel`/`Msg`/`Effect` surface is unchanged — no new effect, command,
subscription, or interpreter behavior.

This is **not** a persistent graphical viewer feature. R2 is
performance-and-metric-only: the visible output never changes (FR-008), so the proof
is in the metric and the equivalence, **not** on screen. Parity is structural
`Bounds`/`Scene` equality (the `SceneEvidence` render functions are deterministic
capability-hash functions, not pixel encoders) plus the FsCheck equivalence invariant.
Recorded as a visible decision in T003: the viewer-launch task-generation rule does
not apply (no persistent-launch / screenshot obligation; no `real-image` claim).

## Vertical-slice rule (US phases)

A `[US*]` task is `[X]` only when the user-reachable surface — the public
`Layout.evaluateIncremental` through the packed library, or the **live** wired
`RetainedRender.step` path driving the incremental evaluator — was actually exercised.
Passing unit tests on the pure helpers alone do **not** satisfy `[X]`. Because the
runtime model is untouched, MVU evidence for these stories is the read of the existing
patch + previous `LayoutResult` driving the incremental re-measure on the live path;
no new transition is introduced to assert.

## Success-criterion → assertion mapping

- **SC-001** (localized leaf edit re-measures only its enclosing flex-line subtree,
  `Scene`-byte-identical to a full rebuild) → T008 failing-first re-measure-count test
  + T013 wiring (`partial-remeasure.md`).
- **SC-002** (`evaluateIncremental` byte-identical to full `evaluate` over ≥1000
  generated `(tree, edit-sequence)` cases incl. cumulative cache-staleness) → T014
  equivalence property + T011 body (`equivalence-property.md`).
- **SC-003** (`WorkReductionRecord.RemeasuredNodeCount`: localized < baseline,
  whole-tree = baseline, empty = 0) → T018 metric test + T006/T013 field+wiring
  (`remeasure-metric.md`).
- **SC-004** (an `AttrCategory.Layout` attr dirties the nearest flex line and climbs
  to/including the first fixed-`Size` ancestor and **stops**; content-sized chain
  reaches root; a non-layout attr dirties no measure) → T016 dirty-derivation unit
  cases + T009 derivation + T010 propagation (`dirty-derivation.md`).
- **SC-008** (post-incremental `Invalidated` = actual re-measured set, not the
  verbatim requested set; empty for an empty patch) → T015 `Invalidated`-honesty test +
  T011 body (`invalidated-honest.md`).
- **SC-005** (every per-frame render output byte-identical to the pre-R2
  full-re-measure build) → T020 at-rest + every-frame byte-identity
  (`byte-identity-at-rest.md`).
- **SC-006** (public `FS.Skia.UI.Layout` baseline unchanged; cache + metric internal) →
  T005 baseline reference + T023 baselines-committed-unchanged (`surface-baselines.md`).
- **SC-007** (all E2 determinism invariants hold on the incremental-layout-wired path) →
  T021 E2-invariant re-check (`e2-invariants.md`).

## Non-SC requirement traceability

- **FR-001** (genuine incremental evaluator; `Bounds` byte-identical to full
  `evaluate`) → T011.
- **FR-001a** (`Invalidated` = actual re-measured set; `Revision = previous.Revision + 1`)
  → T011 + T015.
- **FR-002** (per-node measure/bounds cache keyed by `RetainedId`; pure; translate-don't-
  re-measure) → T012.
- **FR-003** (dirty set derived from `ReconcileResult.Patch` via `attr.Category =
  AttrCategory.Layout` / any `ChildOp`; never a hand-maintained name list) → T009 + T016.
- **FR-004** (conservative whole-flex-line dirtying; climb to/incl. first fixed-`Size`
  ancestor and stop; content-chain reaches root) → T010 + T016.
- **FR-005** (`RetainedRender.step` drives the incremental evaluator via the internal
  `evaluateLayoutIncremental` seam, preserving the reuse-driven paint walk) → T006 + T013.
- **FR-006** (`WorkReductionRecord` extended with `RemeasuredNodeCount`) → T006 + T013 + T018.
- **FR-007** (equivalence property gate over generated trees + cumulative edit sequences)
  → T014.
- **FR-008** (output byte-identical for every frame; identity-at-rest preserved) → T020.
- **FR-009** (additive, non-goal-preserving; no new public type/algorithm/virtualization;
  consumer `view` contract unchanged) → T003 + T007 + T023.

## Governance risk levels

- **Small** — the pure `Layout.evaluateIncremental` body (propagation + cache reuse +
  honest `Invalidated`) and the `layoutDirtySet` derivation: focused validation is `Dev`
  + the targeted `Layout.Tests` equivalence/propagation/`Invalidated` suites.
- **Medium** — the `RetainedRender.step` wiring, the measure cache on
  `RenderFragment`/`RetainedNode`, and the extended `WorkReductionRecord`: `Dev` + the
  `Controls.Tests` metric / at-rest byte-identity / E2-invariant suites on the live path.
- **Broad** — escalation applies **only if** an `.fsi` is forced to move (not intended):
  then the serialized `Dev → GeneratedGuidanceCheck → TemplateCheck →
  GeneratedProductCheck → EvidenceGraph → EvidenceAudit` path applies. **`Route` is
  authoritative** — run `./fake.sh build -t Route` first and run exactly the gates it
  prints. FAKE-backed targets run **sequentially** (shared `.fake` state); aggregate
  results are recorded as **non-authoritative** unless re-confirmed sequentially.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- Every task has a matching `tasks.deps.yml` entry; every line mirrors the
  structured `skillist` via `[skillist: ...]`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory artifacts are present and linked (spec, plan, research, data-model, quickstart, `contracts/incremental-layout.md`, `checklists/requirements.md`) and that `.specify/feature.json` resolves `specs/097-incremental-partial-relayout`
- [X] T002 [P] [skillist: fs-skia-evidence-mode] Scaffold audit-discoverable readiness placeholders under `readiness/`: `partial-remeasure.md`, `equivalence-property.md`, `remeasure-metric.md`, `dirty-derivation.md`, `invalidated-honest.md`, `byte-identity-at-rest.md`, `e2-invariants.md`, `fsi-transcript.md`, `surface-baselines.md`, plus `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md`, `real-image-evidence.md`, `evidence-graph.md`, `evidence-audit.md` — each naming its authoritative command, artifact path, failure class, and next action (use `key=value` lines, not bare image-filename claims)
- [X] T003 [P] [skillist: []] Record feature Tier 2 (internal change; public-behavior nuance only — `evaluateIncremental` body + `Invalidated` value), affected layers (`FS.Skia.UI.Layout` evaluator body + propagation helper; `FS.Skia.UI.Controls` dirty-set derivation, retained measure cache, `RetainedRender.step` swap, extended `WorkReductionRecord`), public-API impact (signature/shape unchanged; cache + metric internal), MVU applicability (untouched — pure functions; no new `Msg`/`Effect`/`update`), and the evidence obligations from the plan; record as a **visible decision** that this is **not** a persistent graphical viewer feature (performance-and-metric-only; structural `Bounds`/`Scene` equality; no persistent-launch / screenshot / real-image obligation)
- [X] T004 [skillist: []] Run `./fake.sh build -t Route`; confirm the routed tier (inner-loop `Dev` + Layout/Controls determinism tests if no `.fsi` moves; the serialized six-target escalation only if an `.fsi` is forced to change) and record the authoritative gate list plus the small/medium/broad governance risk levels into `readiness/governance-risk-levels.md`

---

## Phase 2: Foundation

- [X] T005 [skillist: fs-skia-layout] Confirm the existing public `Layout.evaluateIncremental` signature (`previous -> changedNodeIds -> available -> root -> LayoutResult`, `src/Layout/Layout.fsi:10`) is the correct shape for genuine incremental layout (it already takes the dirty set) and `LayoutResult` already carries `Revision`/`Invalidated` — so this is a **body-only** change with no `.fsi` symbol added or moved; record the current `FS.Skia.UI.Layout` / per-package / cross-package surface-area baselines as the **unchanged** pre-change reference for the Phase 6 confirmation (SC-006)
- [X] T006 [P] [skillist: fs-skia-reconciliation, fs-skia-ui-widgets] Define the internal seams (no public `.fsi` move): extend the internal `WorkReductionRecord` with `RemeasuredNodeCount: int` (`src/Controls/RetainedRender.fsi`); extend the internal `RenderFragment`/`RetainedNode` to cache the per-node intrinsic measure + computed `ComputedBounds` keyed by `RetainedId`; carry the previous frame's `LayoutResult` on the internal `RetainedRender<'msg>`; declare the internal incremental `ControlInternals.evaluateLayoutIncremental` seam (`size -> control -> previous -> cache -> dirty -> LayoutNode * Map<LayoutNodeId, Rect> * LayoutResult`, contract C4) in `src/Controls/Control.fs` (NOT in any `.fsi` → automatically internal) — all reachable from `Controls.Tests` via the existing `InternalsVisibleTo`
- [X] T007 [P] [skillist: fs-skia-evidence-mode] Record unsupported-scope handling, permanent non-goals, and failure diagnostics into `readiness/runtime-limitations.md`: no virtualization/windowing (§6.2 deferred), no new layout algorithm, no new public layout type, no change to computed geometry; the evaluator is **total** (a cache miss / unrecognized dirty id degrades to a full re-measure of that subtree — conservative, never silent divergence, contract C1); `dirty` is a performance hint, never a correctness input; theme-only changes do **not** dirty measure (geometry is theme-independent, INV-7); no data-binding/observable/dependency-property/selector/lookless-template surface (permanent non-goals, FR-009)

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — a localized edit re-measures only its subtree, not the whole tree

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US1] [skillist: fs-skia-reconciliation, fs-skia-testing] Add a failing-first localized-re-measure test on the wired path: a next frame whose patch touches a single leaf (content-only — no `AttrCategory.Layout` attr, no `ChildOp`) yields `WorkReductionRecord.RemeasuredNodeCount` **strictly below** `BaselineNodeCount` and equal to the changed leaf's enclosing flex-line subtree, and the resolved `Scene` is byte-identical to a full-rebuild frame (fails against today's always-full-measure stub; SC-001)

### Implementation

- [X] T009 [US1] [skillist: fs-skia-reconciliation] Implement the pure `layoutDirtySet : prev:Control<'msg> -> patch:Reconcile.NodePatch<'msg> -> Set<LayoutNodeId>` derivation (Controls-side, `LayoutNodeId` layout-path domain, contract C2): a node is self-dirty iff its `Update u` has an `AttrSet` whose `attr.Category = AttrCategory.Layout`, or an `AttrRemoved` whose **prev** attr had `Category = AttrCategory.Layout`, or any `ChildInsert`/`ChildRemove`/`ChildMove`; `Keep`/`Replace`/non-layout `Update` contribute no self-dirt; classification reads `attr.Category` — never a hand-maintained name list (FR-003, INV-2)
- [X] T010 [US1] [skillist: fs-skia-layout, fs-skia-reconciliation] Implement the conservative propagation (contract C3, FR-004) over the `LayoutNode` tree: for each self-dirty node add its whole nearest flex container/line, then climb adding ancestors until (and including) the first ancestor whose `LayoutIntent.Size` is `Some` on the constraining axis and **stop**; a fully content-sized chain reaches the root; when a fixed-size determination is ambiguous, treat the ancestor as **not** fixed (keep climbing — never under-dirty) (INV-3)
- [X] T011 [US1] [skillist: fs-skia-layout] Replace the stub body of `Layout.evaluateIncremental` (`src/Layout/Layout.fs`) with the genuine evaluator: propagate `changedNodeIds` (T010), re-measure **only** the propagated set, reuse `previous.Bounds` for everything else (translating when an ancestor moved), and return a `LayoutResult` whose `Bounds` are **byte-identical** to `evaluate available root` (INV-1); set `Invalidated` = the actual re-measured set (post-propagation, not the verbatim input, FR-001a) and `Revision = previous.Revision + 1L`; preserve `Diagnostics` verbatim; total — never throws (FR-001, contract C1)
- [X] T012 [US1] [skillist: fs-skia-reconciliation] Maintain the per-node measure/bounds cache keyed by `RetainedId` on the internal `RenderFragment`/`RetainedNode` (FR-002, INV-6): an unchanged subtree's intrinsic measure + computed bounds survive across frames and are reused (or **translated** by the ancestor delta when an ancestor moved, never re-measured); the cache is **pure** — keyed on the node's content / layout-relevant attrs / available-axis only, no clock/randomness/escaping mutation (confined to the `RetainedRender.step` mutable-ref retained state, constitution III)
- [X] T013 [US1] [skillist: fs-skia-elmish, fs-skia-reconciliation] Wire `RetainedRender.step` (`src/Controls/RetainedRender.fs:141`) to drive layout through the internal `ControlInternals.evaluateLayoutIncremental` seam instead of the unconditional full `evaluateLayout`: thread the carried previous `LayoutResult` + measure cache + the `layoutDirtySet`-derived dirty set, seed the cache with a full `evaluate` on the first frame / when no previous exists, preserve the reuse-driven paint walk (`box = pr.Fragment.Box`) and the `themeChanged` full-repaint (measure stays clean), and count re-measured nodes into `RemeasuredNodeCount` (interpreter-edge mutable); capture US1 to `readiness/partial-remeasure.md` (SC-001)

**Checkpoint**: User Story 1 is functional and testable independently.

---

## Phase 4: User Story 2 (US2) — incremental layout is provably identical to full layout under any edit sequence

### Tests First (Principle I)

- [X] T014 [P] [US2] [skillist: fs-skia-layout, fs-skia-testing] Add the failing-first equivalence property suite (`tests/Layout.Tests`, FsCheck, contract C6 / FR-007): over **≥1000** generated `(tree, edit-sequence)` cases — attribute changes, inserts, removes, moves, in any order — apply each edit through both `evaluateIncremental` (carrying the cache forward) and full `evaluate`, and assert their computed `Bounds` are **byte-identical** at **every** step, including long cumulative sequences that stress cache staleness; any divergence fails the gate with no tolerance (SC-002)
- [X] T015 [P] [US2] [skillist: fs-skia-layout, fs-skia-testing] Add the failing-first `Invalidated`-honesty test (fails against the verbatim-echo stub): after a localized incremental call, `Invalidated` is the **actual re-measured set** (⊋ the single requested node, bounded by the fixed-size-ancestor subtree, post-propagation) and `Revision = previous.Revision + 1L`; for an empty (all-`Keep`) patch `Invalidated` is empty; only `Bounds` are constrained to byte-identity — `Invalidated`/`Revision` are incremental metadata (FR-001a, INV-4, SC-008)
- [X] T016 [P] [US2] [skillist: fs-skia-reconciliation, fs-skia-testing] Add the dirty-derivation unit cases (contract C2/C3, SC-004): an `AttrCategory.Layout` attr dirties the nearest flex line and climbs to/including the first fixed-`Size` ancestor and **stops** (a subtree under a fixed-`Size` container does not dirty that container's ancestors); a fully content-sized chain dirties up to the root; each `ChildInsert`/`ChildRemove`/`ChildMove` dirties its parent container; a non-layout attr (content/style/state/`visualState`) and a `Keep`/`Replace` dirty **no** measure. **Failing-first**: authored against the `layoutDirtySet`/propagation **signatures before their bodies land** (fails against the stub derivation), so this test does **not** depend on the T009/T010 implementations

### Implementation / Evidence

- [X] T017 [US2] [skillist: fs-skia-layout, fs-skia-evidence-mode] Capture the equivalence + honesty evidence: `readiness/equivalence-property.md` (≥1000 cases, zero divergences incl. cumulative cache-staleness, SC-002/FR-007), `readiness/invalidated-honest.md` (post-incremental `Invalidated` = actual re-measured set, empty for empty patch, SC-008), and `readiness/dirty-derivation.md` (flex-line / fixed-size-ancestor stop, content-chain-to-root, each `ChildOp`, non-layout no-dirt, SC-004)

**Checkpoint**: User Story 2 is functional and testable independently.

---

## Phase 5: User Story 3 (US3) — the partial-layout speedup is measured and reported, not assumed

### Tests First (Principle I)

- [X] T018 [P] [US3] [skillist: fs-skia-reconciliation, fs-skia-testing] Add the failing-first re-measure-metric test on the wired path (contract C5, FR-006): a localized leaf edit shows `RemeasuredNodeCount < BaselineNodeCount` (consistent with the dirty flex-line subtree) **and** a re-paint reduction (`RecomputedNodeCount < BaselineNodeCount`); a genuine whole-tree relayout (a root-level `AttrCategory.Layout` change) shows `RemeasuredNodeCount = BaselineNodeCount` (never under-reports); an empty (all-`Keep`) patch shows `RemeasuredNodeCount = 0` (SC-003)

### Implementation / Evidence

- [X] T019 [US3] [skillist: fs-skia-reconciliation, fs-skia-evidence-mode] Write `readiness/remeasure-metric.md`: the extended `WorkReductionRecord` reports both a re-measure reduction and a re-paint reduction for a localized update, a re-measure count **equal to baseline** for a genuine whole-tree relayout, and **zero** for an empty patch — read from the real wired `step`, not assumed (SC-003, US3)

**Checkpoint**: User Story 3 is functional and testable independently.

---

## Phase 6: Integration & Polish

- [X] T020 [P] [skillist: fs-skia-reconciliation, fs-skia-evidence-mode] Write `readiness/byte-identity-at-rest.md` (FR-008/SC-005): an at-rest frame (all-`Keep` patch) re-measures nothing (`RemeasuredNodeCount = 0`) and renders a `Scene` byte-identical to the un-incremental build; every tested frame (localized + whole-tree) is byte-identical to the pre-R2 full-re-measure build — R2 changes work and metrics, never geometry or pixels
- [X] T021 [P] [skillist: fs-skia-reconciliation, fs-skia-evidence-mode] Write `readiness/e2-invariants.md` (SC-007): on the incremental-layout-wired path all E2 determinism invariants still hold — `RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount`, `Keep → reuse`, first-frame full paint, and `KeyCollision` diagnostics — demonstrated on the live render seam
- [X] T022 [P] [skillist: fs-skia-layout] Exercise the real (no-longer-stub) public `Layout.evaluateIncremental` from FSI against the packed library per quickstart §1 — `Bounds` byte-identical to a full re-evaluate, `Invalidated` reporting the propagated set (not the verbatim input), `Revision` advancing — and capture the session transcript to `readiness/fsi-transcript.md`
- [X] T023 [P] [skillist: fs-skia-layout] Confirm the `FS.Skia.UI.Layout` / per-package / cross-package surface-area baselines are committed **unchanged** vs the T005 reference (the `evaluateIncremental` signature and `LayoutResult` shape are preserved; the measure cache and `RemeasuredNodeCount` remain internal); record to `readiness/surface-baselines.md` (SC-006)
- [X] T024 [skillist: fs-skia-testing] Run exactly the gates `Route` printed (T004) — the inner-loop `Dev` plus the Layout/Controls determinism suites if no `.fsi` moved; only the serialized `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck` prefix **sequentially** (shared `.fake` state) if an `.fsi` was forced to change — recording the aggregate results as **non-authoritative** into `readiness/generated-guidance-validation.md`
- [X] T025 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed `feature-directory` + `tasks=<n>` match this feature, no cycles, no dangling refs, no `[S*]` surprises; record to `readiness/evidence-graph.md`
- [X] T026 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (synthetic-propagation + diff-scan) or document every `--accept-synthetic` override; record to `readiness/evidence-audit.md`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. **None planned**
— see the Status Legend rationale (total evaluator with a conservative
full-re-measure fallback and a pure total dirty-set walk, no runtime error path;
real equivalence/dirty-derivation/metric/byte-identity evidence). For any `[SEH]`
rows, include the approval label, design-phase source, synthetic input class,
expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |

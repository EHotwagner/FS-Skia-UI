# Tasks: Governance Gate Hardening

**Feature branch**: `087-governance-gate-hardening`
**Spec**: `specs/087-governance-gate-hardening/spec.md`
**Plan**: `specs/087-governance-gate-hardening/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

`[SEH]` plus `synthetic-error-handling-approved` is an annotation, not a status,
assigned only at design/planning/task time. **None are approved for this
feature**: the seeded defect/skew/deferral/violation inputs are *real* inputs to
pure governance functions (exercised through `Governance.Tests` + FAKE runs),
not synthetic substitutes for a missing capability — so no `[S]`/`[SEH]` is
expected (plan "Synthetic evidence"). If a seeded error-path input proves
infeasible to produce really, it is disclosed per Principle V at task time, not
relabeled at implementation time.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase). Non-FAKE only.
- **[US1]**…**[US6]** — user-story scope (governance-gate behavior, reachable via
  the named FAKE target rather than a UI surface).
- Tier matches the feature overall (**Tier 1**, governance-contract level), so
  per-task `[T1]`/`[T2]` annotations are omitted.

FAKE-backed commands (`./fake.sh`, `fake.cmd`, `dotnet fake`) share repository
`.fake` state and are **not** safe to run concurrently — they are serialized in
the deterministic escalated order. Non-FAKE reads/checks may run `[P]`.

## Governance risk levels

- **Small** — a single pure-function change with a focused `Governance.Tests`
  case (e.g. T024 propagation, T020 verdict): focused validation = the targeted
  Expecto/FsCheck test, no broad rerun.
- **Medium** — a gate's effect shaping or schema/contract change (T009/T010,
  T013/T014, T017, T021, T027/T028): focused validation = the owning FAKE target
  (`GeneratedProductCheck`, `TemplateCheck`, `RefreshSurfaceBaselines`,
  `EvidenceAudit`) plus its `Governance.Tests`.
- **Broad** — `Routing.fs`/contract regeneration or the FR-011 cross-gate sweep
  (T030–T032): broad validation = the full serialized six-target order. Broad
  validation is required only when governance routing or multiple gates change in
  one run. Non-authoritative aggregate results (e.g. a known environment obstacle)
  are recorded as such in the readiness file with the per-step classification, not
  silently hand-waved.

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold the feature directory and link spec + plan; confirm `.specify/feature.json` resolves `specs/087-governance-gate-hardening`
- [X] T002 [P] [skillist: []] Create readiness scaffolding under `specs/087-governance-gate-hardening/readiness/` with audit-enforced placeholder files discoverable before implementation: `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation-authority.md`, `skill-loading-evidence-workflow.md`, `audit-diagnostics.md`, `evidence-graph.md`, `evidence-audit.md`, plus the feature evidence stubs named in plan.md (`generated-product-check-green.txt`, `generated-product-defect-classification.txt`, `package-skew-seeded.txt`, `package-skew-clean.txt`, `refresh-surface-baselines-idempotent.txt`, `audit-three-verdicts.txt`, `synthetic-propagation-no-phase-edge.txt`, `skill-loading-evidence-provenance.md`, `true-positive-gates-still-block.txt`). Each names its authoritative command, artifact path, failure class, and next action
- [X] T003 [P] [skillist: []] Record feature Tier 1 (governance-contract), affected layer (`build/Governance/**`, no `src/**/*.fsi` change), public-API impact (none), Elmish/MVU applicability (Principle IV satisfied by keeping verdict/propagation/skew **pure** and only FR-001 feature-context provisioning at the interpreter edge — no new public `Model`/`Msg`/`Effect` surface), and the evidence obligations from plan.md

---

## Phase 2: Foundation

- [X] T004 [skillist: fsharp-code-generation] Define the changed engine value types as the single source in `build/Governance/Evidence/EvidenceFormatSchema.fs` (three-state `AuditVerdict`, `AcceptedDeferral`, skill-loading `LoadProvenance`, `StepClassification`, `PackageSet`, `PackageSkewFinding`) per data-model.md
- [X] T005 [P] [skillist: fsharp-build-orchestration] Add failing-first `tests/Governance.Tests/` scaffolding (Expecto + FsCheck) covering verdict / propagation / skew / provenance / per-package idempotence — each test fails before its change and passes after
- [X] T006 [P] [skillist: fsharp-code-generation] Regenerate `template/base/docs/evidence-formats.md` from the schema single source (skill-loading `provenance` column + accepted-deferral record shape); record governance-internal contract baselines for the changed verdict schema

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) — trustworthy `GeneratedProductCheck` (FR-001/002, P1)

### Tests First (Principle I, Principle VI)

- [X] T007 [P] [US1] [skillist: fsharp-build-orchestration] Add `Governance.Tests` for per-step product-defect-vs-environment aggregation (FR-002): overall verdict fails iff any step is `ProductDefect`, and an `Environment` classification never suppresses a `ProductDefect` in the same run (SC-002)
- [X] T008 [P] [US1] [skillist: fsharp-build-orchestration, fs-skia-template-update] Add verification that `GeneratedProductCheck` reaches a real green on a clean tree once the generated `Verify` step can resolve a feature context (FR-001, SC-001)

### Implementation

- [X] T009 [US1] [skillist: fsharp-build-orchestration, fs-skia-template-update] Provision each generated product a resolvable feature context — primary: a generated `.specify/feature.json` carrying a usable `feature_directory`; documented fallback only: split the authoritative build/test step from the env-dependent `Verify` step (per research.md R1) — so `Engine/Model.fs` `activeFeatureId` resolves instead of hard-failing (FR-001)
- [X] T010 [US1] [skillist: fsharp-build-orchestration] Attach a `{ step; classification; packageSet }` result to each generated-product step in `Front/Governance.fs` and compute the overall verdict as max severity over `ProductDefect` steps, reporting `Environment` steps as non-authoritative (FR-002)
- [X] T011 [US1] [skillist: fs-skia-template-update, fsharp-build-orchestration] Capture `generated-product-check-green.txt` (clean-tree green) and `generated-product-defect-classification.txt` (seeded product defect + concurrent env obstacle → product-defect verdict) (SC-001/002)

**Checkpoint**: US1 independently validated via `GeneratedProductCheck`.

---

## Phase 4: User Story 2 (US2) — pinned-package skew caught before merge (FR-003/004, P1)

### Tests First

- [X] T012 [P] [US2] [skillist: fsharp-io-globbing, fsharp-build-orchestration] Add `Governance.Tests` for static skew detection: a symbol referenced in generated source/tests that is present in the local-packed surface but absent from the pinned surface yields a `PackageSkewFinding` naming symbol + file + pinned-vs-local version gap; the real tree yields none (SC-003)

### Implementation

- [X] T013 [US2] [skillist: fsharp-io-globbing, fs-skia-template-update] Implement the static `PackageSkewFinding` check comparing referenced symbols ∩ (local-packed surface − pinned surface) using existing captured surface baselines — no network restore (FR-003)
- [X] T014 [US2] [skillist: fsharp-build-orchestration] Tag every generated-product report with an explicit `PackageSet` (`LocalPacked` for `TemplateCheck`, `Pinned` for `GeneratedProductCheck`) so an operator can determine the package source of any pass/fail from the report alone (FR-004, SC-004)
- [X] T015 [US2] [skillist: fs-skia-template-update] Capture `package-skew-seeded.txt` (fails naming symbol/file/version gap on a seeded unpinned-API reference) and `package-skew-clean.txt` (real tree passes, no restore) (SC-003/004)

**Checkpoint**: US2 independently validated via `TemplateCheck` skew sub-check.

---

## Phase 5: User Story 3 (US3) — complete, idempotent surface-baseline refresh (FR-005/006, P2)

### Tests First

- [X] T016 [P] [US3] [skillist: fsharp-io-globbing] Add `Governance.Tests` for per-package baseline byte-idempotence: capturing twice on an unchanged tree produces byte-equal `readiness/per-package-surface/*.fsi.txt` (no trailing-newline/whitespace churn) (SC-006)

### Implementation

- [X] T017 [US3] [skillist: fsharp-io-globbing] Fold `PerPackageSurface.captureCurrent` into `RefreshSurfaceBaselines` with byte-idempotent writes so one refresh regenerates per-package baselines alongside cross-package/api-surface/skill baselines (FR-005/006)
- [X] T018 [US3] [skillist: fsharp-build-orchestration] Capture `refresh-surface-baselines-idempotent.txt`: run `RefreshSurfaceBaselines` twice, `git status` clean after the second (SC-005/006)

**Checkpoint**: US3 independently validated via `RefreshSurfaceBaselines`.

---

## Phase 6: User Story 4 (US4) — accepted-deferral PASS verdict (FR-007/008, P2)

### Tests First

- [X] T019 [P] [US4] [skillist: fsharp-build-orchestration] Add an FsCheck property (FR-011 invariant): `PassWithAcceptedDeferrals` requires `unacceptedSynthetic = 0` **and** every blocking-hit count `= 0`; an accepted deferral can never mask an unaccepted synthetic or any blocking hit
- [X] T020 [US4] [skillist: fsharp-parsing] Replace the binary `Audit.verdict` with the three-state `AuditVerdict` derived from `sehSummary` counts plus the accepted-deferral set (FR-007)

### Implementation

- [X] T021 [US4] [skillist: fsharp-code-generation] Record each `AcceptedDeferral` as durable structured data in `readiness/synthetic-evidence.json` and surface accepted-vs-unaccepted synthetic counts separately in `seh-audit-summary.json` via `Evidence/Render.fs` (FR-008)
- [X] T022 [US4] [skillist: speckit-evidence-audit] Capture `audit-three-verdicts.txt` + `seh-audit-summary.json` samples on three seeded inputs (clean PASS / PASS-with-accepted-deferrals / FAIL), recovering the accepted-deferral justification as structured data (SC-007)

**Checkpoint**: US4 independently validated via `EvidenceAudit`.

---

## Phase 7: User Story 5 (US5) — propagation over real dependencies only (FR-009, P3)

### Tests First

- [X] T023 [P] [US5] [skillist: fsharp-graph-algorithms] Add an FsCheck property: a phase-checkpoint-edge-only downstream of an `[S]` leaf is never recomputed `[S*]`; taint follows `ExplicitDeps` only (SC-008)

### Implementation

- [X] T024 [US5] [skillist: fsharp-graph-algorithms] Change `Graph.propagate` to filter taint over `ExplicitDeps` only, keeping `allDeps` (`ExplicitDeps @ PhaseDeps`) for toposort/cycle detection/ordering (FR-009)
- [X] T025 [US5] [skillist: speckit-evidence-graph] Capture `synthetic-propagation-no-phase-edge.txt`: a leaf `[S]` whose output nothing consumes propagates `[S*]` to zero phase-edge-only tasks (SC-008)

**Checkpoint**: US5 independently validated via `EvidenceGraph`.

---

## Phase 8: User Story 6 (US6) — captured-vs-asserted skill-loading provenance (FR-010, P3)

### Tests First

- [X] T026 [P] [US6] [skillist: fsharp-parsing] Add `Governance.Tests` for skill-loading provenance parse/validate (`captured` vs `asserted` 9th column, existing `loaded_at < work_started_at`/ISO-8601 rules unchanged) and for at-implementation gap detection (SC-009)

### Implementation

- [X] T027 [US6] [skillist: fsharp-parsing, fsharp-code-generation] Add the 9th `provenance` column (`captured` | `asserted`) to the skill-loading-evidence row in the `EvidenceFormatSchema` single source and mirror it into `docs/evidence-formats.md` (FR-010)
- [X] T028 [US6] [skillist: fsharp-parsing] Surface a declared-but-unloaded skill **at the point the declaring task is implemented**, not deferred to the `[X]` flip (FR-010, SC-009)
- [X] T029 [US6] [skillist: fsharp-parsing] Capture `skill-loading-evidence-provenance.md` + an at-implementation-time gap report distinguishing captured from asserted load times (SC-009)

**Checkpoint**: US6 independently validated via the skill-loading gap report.

---

## Phase 9: Integration & Polish — preserve true positives + validate (FR-011)

- [X] T030 [skillist: fsharp-code-generation] Regenerate `validation.contract.yml` from `Routing.fs` for the changed governance paths and confirm `TargetMetadataDrift` / `SkillSyncCheck` currency
- [X] T031 [skillist: fsharp-build-orchestration] Capture `true-positive-gates-still-block.txt`: seed a real violation of diff-scan, additive-surface enforcement, window-visibility, persistent-launch, and synthetic-honesty and confirm each still blocks (FR-011, SC-010)
- [X] T032 [skillist: fsharp-build-orchestration, fs-skia-template-update] Run the escalated serialized FAKE order (sequential, no concurrent `.fake`): `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck`; record results and any non-authoritative aggregate handling with its per-step classification
- [X] T033 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, and no phase-edge-only `[S*]` surprises
- [X] T034 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm a clean PASS or PASS-with-accepted-deferrals verdict and document every `--accept-synthetic` override as structured data

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section. For `[SEH]` rows,
include the approval label, design-phase source, synthetic input class, expected
error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |

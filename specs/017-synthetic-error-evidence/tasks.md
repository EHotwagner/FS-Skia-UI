# Tasks: Synthetic Error Evidence

**Feature branch**: `017-synthetic-error-evidence`
**Spec**: `specs/017-synthetic-error-evidence/spec.md`
**Plan**: `specs/017-synthetic-error-evidence/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus
`synthetic-error-handling-approved`; it still remains `[S]` when completed
with synthetic-only evidence. `[SEH]` classification is valid only when it is
assigned during design, planning, clarification, or task generation before
implementation work begins.

## Vertical-slice rule (US phases)

A task tagged `[US*]` may only be marked `[X]` when the change is reachable
from a user-facing entry point and that path was actually exercised. This
feature changes governance workflow behavior rather than product runtime UI;
the reachable surfaces are the Spec Kit prompts/templates, evidence scripts,
FAKE targets, governance tests, and generated readiness reports.

For stateful or I/O-bearing stories, `[X]` also requires workflow evidence:
classification state, provenance timing, audit verdict state, and fixture
acceptance/rejection transitions must be tested and reported. Product
Elmish/MVU is not applicable for this feature.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, … — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change
- **[SEH]** — design-approved synthetic error-handling task

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors
the structured `skillist` value using `[skillist: ...]`; `[skillist: []]`
means no local capability skill materially applies.

## Canonical Verification Targets

- `./fake.sh build -t Dev` for fast local verification.
- `./fake.sh build -t GeneratedGuidanceCheck` for generated prompt, task-skill,
  and implementation guidance governance.
- `./fake.sh build -t EvidenceGraph` for task graph and skill metadata checks.
- `./fake.sh build -t EvidenceAudit` for synthetic-evidence and diff-scan gates.
- `./fake.sh build -t Verify` for the full governed workflow when broad
  validation is required.

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm current spec, plan, data model, contracts, and quickstart describe the `[SEH]` governance contract without missing required readiness paths
- [X] T002 [P] [skillist: speckit-tasks] Create or refresh `specs/017-synthetic-error-evidence/readiness/` placeholders for classification rules, task-generation guidance, accepted audit, late rejection, non-eligible cases, generated guidance, graph, and audit evidence
- [X] T003 [P] [skillist: speckit-tasks, speckit-implement, speckit-constitution] Inventory affected files and test modules for constitution, task templates, task command guidance, implementation command guidance, docs, evidence scripts, FAKE targets, and governance tests
- [X] T004 [skillist: []] Record Tier 1 scope, no `.fsi` package surface impact, Product MVU non-applicability, governance workflow state obligations, and small/medium/broad validation rules in readiness notes

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: []] Add failing governance tests for the `[SEH]` classification data model fields, required label, design source, synthetic input class, expected error behavior, and rationale
- [X] T006 [P] [skillist: speckit-constitution] Add failing governance tests for constitution text that preserves Principle V disclosure while defining the narrow `[SEH]` exception
- [X] T007 [P] [skillist: speckit-tasks] Add failing guidance tests for canonical and preset `tasks-template.md` plus `/speckit.tasks` examples covering eligible and non-eligible `[SEH]` classifications
- [X] T008 [P] [skillist: speckit-tasks, speckit-implement] Add failing guidance tests for `/speckit.implement` requiring newly discovered synthetic error-handling needs to return to task/design work instead of implementation-time relabeling
- [X] T009 [P] [skillist: speckit-evidence-graph] Add failing graph/report tests for accepted `[SEH]` counting, ordinary `[S]` counting, `[S*]` propagation visibility, and structured metadata fields
- [X] T010 [P] [skillist: speckit-evidence-audit] Add failing audit tests for PASS when every synthetic task is valid design-approved `[SEH]` and FAIL when any ordinary synthetic task remains
- [X] T011 [skillist: []] Document focused validation expectations: small changes use targeted governance tests, medium changes add fixture script runs, and broad validation requires `Verify` with non-authoritative aggregate results recorded separately

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) - Classify Necessary Synthetic Error Tests During Design

### Tests First

- [S] T012 [P] [US1] [SEH] synthetic-error-handling-approved [skillist: []] Add valid malformed-input fixture task lists that disclose synthetic input class, expected rejection behavior, design-phase source, and infeasible real-input rationale
- [X] T013 [P] [US1] [skillist: speckit-tasks] Add task-generation tests that classify at least eight examples across malformed parser input, corrupt file content, invalid arguments, protocol violations, missing data, hostile payloads, forced error results, and non-eligible convenience fixtures

### Implementation

- [X] T014 [US1] [skillist: speckit-constitution] Update `.specify/memory/constitution.md` and preset/active constitution templates with the narrow design-approved `[SEH]` exception and unchanged disclosure requirements
- [X] T015 [US1] [skillist: speckit-tasks] Update active and preset task and plan templates to document `[SEH]`, `synthetic-error-handling-approved`, required inventory fields, eligibility examples, non-eligible examples, and split/rename preservation rules
- [X] T016 [US1] [skillist: speckit-tasks] Update `/speckit.tasks` guidance so generated tasks assign `[SEH]` only during design/task generation and mirror the approval label in task metadata or the Synthetic-Evidence Inventory
- [X] T017 [US1] [skillist: []] Update `docs/evidence.md` and `docs/speckit.md` so reviewers can identify accepted synthetic error-handling tasks, rationale, and audit acceptance status within the 2-minute SC-006 threshold
- [X] T018 [US1] [skillist: speckit-tasks] Capture `readiness/seh-classification-rules.md` and `readiness/task-generation-seh.md` with eligible/non-eligible examples, reviewer classification timing, and evidence that eight examples can be classified within the 10-minute SC-004 threshold

**Checkpoint**: US1 design-time classification guidance is independently reviewable.

---

## Phase 4: User Story 2 (US2) - Accept Approved Synthetic Error-Handling Evidence Without Hiding It

### Tests First

- [S] T019 [P] [US2] [SEH] synthetic-error-handling-approved [skillist: speckit-evidence-audit] Add audit fixture graphs where every synthetic task is valid `[SEH]` and assert PASS with accepted synthetic counts still reported as synthetic
- [X] T020 [P] [US2] [skillist: speckit-evidence-graph] Add evidence graph tests that render accepted `[SEH]`, unaccepted `[S]`, and `[S*]` states with separate counts and root-cause annotations

### Implementation

- [X] T021 [US2] [skillist: speckit-evidence-graph] Extend evidence graph parsing/report output for `[SEH]` annotation, approval label metadata, design source, synthetic input class, expected error behavior, and acceptance status
- [X] T022 [US2] [skillist: speckit-evidence-audit] Extend evidence audit logic and output contract with `accepted-seh-tasks`, `unaccepted-synthetic-tasks`, `auto-synthetic-tasks`, `late-seh-tasks`, and reviewer-facing diagnostics
- [X] T023 [US2] [skillist: speckit-evidence-graph, speckit-evidence-audit] Update FAKE readiness report text and focused gate summaries so `EvidenceGraph`, `EvidenceAudit`, and generated reports describe accepted `[SEH]` separately from real task evidence
- [S] T024 [US2] [SEH] synthetic-error-handling-approved [skillist: speckit-evidence-audit] Capture `readiness/audit-accepted-seh.md` with real command output proving approved malformed/error-path synthetic fixtures pass while remaining visibly `[S]`

**Checkpoint**: US2 audit acceptance path is independently testable.

---

## Phase 5: User Story 3 (US3) - Reject Late Synthetic Reclassification During Implementation

### Tests First

- [X] T025 [P] [US3] [skillist: speckit-evidence-audit] Add audit fixtures for late `[SEH]` tag addition, late approval label addition, after-failure cleanup, missing provenance, and split/rename without preserved rationale
- [X] T026 [P] [US3] [skillist: speckit-tasks, speckit-implement] Add guidance tests proving implementation instructions forbid applying `[SEH]` locally and direct contributors back to task/design updates

### Implementation

- [X] T027 [US3] [skillist: speckit-evidence-audit] Implement late reclassification diagnostics with task id, first synthetic sighting, first `[SEH]` sighting, implementation-start evidence, failure reason, and required planning action
- [X] T028 [US3] [skillist: speckit-tasks, speckit-implement] Update `/speckit.implement` guidance and related generated guidance checks with the implementation-time relabeling prohibition
- [X] T029 [US3] [skillist: []] Capture `readiness/audit-rejects-late-seh.md` with real command evidence for late classification rejection and actionable diagnostic text

**Checkpoint**: US3 late-reclassification rejection is independently testable.

---

## Phase 6: User Story 4 (US4) - Separate Error-Handling Synthetic Evidence From Convenience Fixtures

### Tests First

- [X] T030 [P] [US4] [skillist: speckit-evidence-audit] Add audit fixtures for convenience mocks, unavailable host substitutes, incomplete integrations, placeholder outputs, speed-only fixtures, and ordinary in-memory substitutes that must not pass under `[SEH]`
- [X] T031 [P] [US4] [skillist: speckit-tasks] Add guidance tests ensuring non-eligible examples are rejected during task generation review even when they include synthetic data

### Implementation

- [X] T032 [US4] [skillist: speckit-evidence-audit] Implement non-eligible synthetic case rejection and diagnostics without weakening the existing `--accept-synthetic` override path
- [X] T033 [US4] [skillist: speckit-tasks] Update task guidance examples and review language so convenience mocks, incomplete integrations, unavailable product capability, missing host support, placeholder outputs, and speed-only fixtures remain ordinary synthetic evidence
- [X] T034 [US4] [skillist: []] Capture `readiness/non-eligible-synthetic-cases.md` with real command evidence that non-eligible synthetic cases fail readiness

**Checkpoint**: US4 non-eligible synthetic rejection is independently testable.

---

## Phase 7: Integration & Polish

- [X] T035 [skillist: []] Run focused governance tests for classification, guidance, graph, and audit behavior; record command outputs in the relevant readiness files
- [X] T036 [skillist: []] Run `./fake.sh build -t GeneratedGuidanceCheck` and capture `readiness/generated-guidance-check.md`
- [X] T037 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` and `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/017-synthetic-error-evidence --graph-only`; capture `readiness/evidence-graph.md`
- [X] T038 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit`; capture `readiness/evidence-audit.md` with accepted `[SEH]` counts and no unaccepted synthetic blockers
- [X] T039 [skillist: []] Run `./fake.sh build -t Verify` when broad validation is required or any shared governance target changed; record aggregate results as supporting, non-authoritative evidence
- [X] T040 [skillist: []] Complete readiness notes and PR-ready summary covering changed governance surfaces, synthetic evidence disclosures, residual risks, and no package/API/runtime impact

---

## Skill Evaluation Notes

High-confidence matches were declared where task text names local capability
signals: task templates and `/speckit.tasks` use `speckit-tasks`;
implementation command guidance uses `speckit-tasks, speckit-implement` in
that prerequisite order; constitution text uses `speckit-constitution`; graph
validation/report work uses `speckit-evidence-graph`; audit verdict and
diff-scan readiness work uses `speckit-evidence-audit`. Tasks with no
capability-specific signal are valid-empty and use `[skillist: []]`.

## Synthetic-Evidence Inventory

List every completed `[S]` task here with its Principle V disclosures. Pending
tasks T012, T019, and T024 are design-approved `[SEH]` candidates for malformed
input/error-path fixture evidence; rows must be added here if they are later
completed with synthetic-only evidence.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T012 | Malformed task-list fixture validates impossible parser/error input | infeasible, see spec FR-004 | n/a | synthetic-error-handling-approved | specs/017-synthetic-error-evidence/tasks.md:T012 | malformed parser input | reject malformed task graph with reviewer diagnostics | accepted-seh |
| T019 | Accepted audit fixture uses corrupt/error-path input to prove PASS semantics | infeasible, see spec FR-007 | n/a | synthetic-error-handling-approved | specs/017-synthetic-error-evidence/tasks.md:T019 | corrupt file content | pass audit while reporting accepted synthetic count | accepted-seh |
| T024 | Readiness capture uses the accepted malformed/error-path fixture | infeasible, see spec FR-007 | n/a | synthetic-error-handling-approved | specs/017-synthetic-error-evidence/tasks.md:T024 | forced error-result fixture | preserve `[S]` visibility with accepted audit status | accepted-seh |

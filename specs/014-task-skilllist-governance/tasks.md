# Tasks: Task Skilllist Governance

**Feature branch**: `014-task-skilllist-governance`
**Spec**: `specs/014-task-skilllist-governance/spec.md`
**Plan**: `specs/014-task-skilllist-governance/plan.md`

## Status Legend

- `[ ]` - pending
- `[X]` - done with real evidence
- `[S]` - done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` - failed
- `[-]` - skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

## Vertical-slice rule (US phases)

A task tagged `[US*]` may only be marked `[X]` when the workflow path was
actually exercised through the governed command, generated guidance check, or
readiness validator that users will run. Static fixture or parser-only evidence
is not enough for story completion unless the task is explicitly scoped to a
foundation fixture.

This feature changes Spec Kit governance, validation scripts, templates, and
agent workflow guidance. Principle IV Elmish/MVU evidence is not applicable
because there is no stateful runtime or product MVU workflow change.

## Task Annotations

- **[P]** - parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** - user-story scope
- **[T1]** / **[T2]** - Tier 1 (contracted) vs Tier 2 (internal) change

Every task must have a matching entry in `tasks.deps.yml`. Every task line
mirrors the structured `skillist` value using `[skillist: ...]`.

## Phase 1: Setup

- [X] T001 [skillist: speckit-tasks] Review the spec, plan, contract, existing task templates, and active skill inventory for this feature
- [X] T002 [P] [skillist: []] Create readiness directory scaffolding for validation logs and task skilllist fixtures
- [X] T003 [P] [skillist: []] Record Tier 1 governance scope, affected workflow surfaces, no public API impact, no package impact, and Principle IV non-applicability
- [X] T004 [P] [skillist: speckit-tasks] Capture the canonical capability skill inventory used for post-task skill evaluation

---

## Phase 2: Foundation

- [X] T005 [T2] [skillist: []] Document that no `.fsi` contract or runtime public API surface is introduced by this feature
- [X] T006 [skillist: speckit-tasks, speckit-evidence-graph] Finalize the structured task metadata shape with `deps` and `skillist` fields plus the `tasks.md` mirror format
- [X] T007 [P] [skillist: speckit-evidence-graph] Add failing-first readiness fixtures for missing structured `skillist`, non-list `skillist`, missing task mirrors, and existing bare-list metadata that must be migrated or regenerated
- [X] T008 [P] [skillist: speckit-evidence-graph] Add failing-first readiness fixtures for mirror mismatch, omitted obviously applicable capability skills, excess non-minimal skills, and invalid multi-skill dependency order
- [X] T009 [P] [skillist: speckit-implement] Add implementation-loading fixtures for valid skill loads, missing skills, unreadable skills, and ambiguous skill ids
- [X] T010 [skillist: speckit-evidence-graph, speckit-evidence-audit] Define the diagnostics contract for task id, failing field, unresolved skill id, and readiness-blocking verdicts

**Checkpoint**: Foundation ready - story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) - Tasks Record Applicable Skills

### Tests First

- [X] T011 [P] [US1] [skillist: speckit-evidence-graph] Add validation coverage that rejects task lists missing structured `skillist` fields or `tasks.md` mirrors
- [X] T012 [P] [US1] [skillist: speckit-evidence-graph] Add validation coverage that rejects mirror mismatches and omitted obvious capability skills
- [X] T013 [P] [US1] [skillist: speckit-tasks] Add generated-guidance coverage requiring task templates and task-generation guidance to emit `skillist` values

### Implementation

- [X] T014 [US1] [skillist: speckit-evidence-graph] Extend task graph parsing to read object-form `tasks.deps.yml` entries with `deps` and `skillist`
- [X] T015 [US1] [skillist: speckit-evidence-graph] Implement readiness validation for required `skillist`, list typing, mirror presence, mirror equality, declared skill resolution, obvious capability omissions, non-minimal skill sets, multi-skill dependency order, and existing task-list migration blockers
- [X] T016 [US1] [skillist: speckit-tasks] Update root and preset task templates to include the `skillist` mirror on every task line and structured metadata for every task
- [X] T017 [US1] [skillist: speckit-tasks] Update `/speckit.tasks` command and skill guidance to require post-generation skill evaluation and minimal ordered skill selection
- [X] T018 [US1] [skillist: speckit-evidence-graph] Record readiness evidence for invalid fixtures, a valid task list containing explicit empty and non-empty `skillist` values, and under-30-second missing-`skillist` rejection timing

**Checkpoint**: US1 validates independently through task generation guidance and readiness validation.

---

## Phase 4: User Story 2 (US2) - Implementation Loads Task Skills

### Tests First

- [X] T019 [P] [US2] [skillist: speckit-implement] Add generated-guidance coverage that `/speckit.implement` must load each task's declared skills before implementation
- [X] T020 [P] [US2] [skillist: speckit-implement] Add blocking-path fixtures for missing, unreadable, ambiguous declared task skills, and existing task lists that must be migrated or regenerated before implementation

### Implementation

- [X] T021 [US2] [skillist: speckit-implement] Update root and preset implementation skill guidance to read `skillist`, resolve each skill, load skills in order, and stop on failures
- [X] T022 [US2] [skillist: speckit-implement] Update implementation command guidance to record per-task skill-load evidence before code changes begin
- [X] T023 [US2] [skillist: speckit-implement, speckit-evidence-audit] Record readiness evidence that valid non-empty `skillist` entries are loaded and invalid entries block implementation

**Checkpoint**: US2 validates independently through implementation guidance and loading diagnostics.

---

## Phase 5: User Story 3 (US3) - Constitution Makes the Rule Mandatory

### Tests First

- [X] T024 [P] [US3] [skillist: speckit-constitution] Add generated-guidance coverage that the constitution and constitution template require post-task skill evaluation and implementation-time loading

### Implementation

- [X] T025 [US3] [skillist: speckit-constitution] Update `.specify/memory/constitution.md` with the mandatory `skillist` governance gate
- [X] T026 [US3] [skillist: speckit-constitution] Update root and preset constitution templates so generated products inherit the mandatory `skillist` rule
- [X] T027 [US3] [skillist: speckit-constitution, speckit-tasks, speckit-implement] Verify contributors can find the task-generation and implementation-loading obligations from the constitution and generated guidance

**Checkpoint**: US3 validates independently through constitution and generated guidance review.

---

## Phase 6: Integration & Polish

- [X] T028 [skillist: []] Update `.specify/integrations/codex.manifest.json` if generated skill hashes or governed integration state changed
- [X] T029 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` and capture `readiness/logs/evidence-graph.txt`
- [X] T030 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` and capture `readiness/logs/evidence-audit.txt`
- [X] T031 [skillist: []] Run `./fake.sh build -t GeneratedGuidanceCheck` and capture `readiness/logs/generated-guidance-check.txt`
- [X] T032 [skillist: []] Run `./fake.sh build -t Dev` and capture the final governed verification result
- [X] T033 [skillist: []] Complete readiness notes, including fixture inventory, validator diagnostics, implementation-load evidence, and any synthetic-evidence disclosures

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| _(none yet)_ | | | |

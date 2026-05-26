# Tasks: Improve Governance Weaknesses

**Feature branch**: `015-improve-governance-weaknesses`
**Spec**: `specs/015-improve-governance-weaknesses/spec.md`
**Plan**: `specs/015-improve-governance-weaknesses/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

## Vertical-Slice Rule (US Phases)

A task tagged `[US*]` may only be marked `[X]` when the change is reachable
from the maintainer-facing governance entry point and that path was actually
exercised by the focused command, parser fixture, readiness artifact, or
manual transcript named in the task. Domain, parser, or build-script changes
alone do not satisfy `[X]` for a `[US*]` task unless their user-facing
diagnostic/report path is also exercised.

Product Elmish/MVU is not applicable for this feature: no runtime product
state, public `.fsi` contract, renderer behavior, package identity, or sample
contract changes are in scope. Governance state is represented by static
validation records, readiness reports, and existing FAKE target effects.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, … — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors
the structured `skillist` value using `[skillist: ...]`.

---

## Phase 1: Setup

- [X] T001 [skillist: speckit-evidence-graph, speckit-evidence-audit] Create readiness scaffolding for skill loading, skill detection calibration, governance risk levels, aggregate hang diagnostics, runtime limitations, evidence graph, and evidence audit
- [X] T002 [P] [skillist: []] Record Tier 1 governance-contract scope, no public F# API/package impact, no product MVU applicability, and required real evidence paths in `readiness/governance-risk-levels.md`
- [X] T003 [P] [skillist: speckit-tasks] Add this feature's task-generation assumptions and initial skillist rationale to the readiness notes
- [X] T004 [skillist: []] Add or identify governance test fixture locations for task skill evidence, skill-match assessment, risk-level evidence, and aggregate timeout verdict examples

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: speckit-evidence-graph, speckit-implement] Add failing-first graph/parser fixtures for structured task metadata, skillist mirrors, missing skill-loading evidence references, and ambiguous skill diagnostics
- [X] T006 [P] [skillist: speckit-evidence-audit, speckit-implement] Add failing-first audit fixtures for late skill-loading evidence, incomplete reviewer exceptions, synthetic disclosure separation, and readiness-blocking diagnostics
- [X] T007 [P] [skillist: speckit-tasks, speckit-implement] Update task-generation templates and preset command guidance to require skill-loading evidence obligations and confidence-based skill-match review in generated tasks
- [X] T008 [P] [skillist: speckit-implement] Update implementation guidance to require declared skills to be resolved, loaded in order before task work, and recorded in `readiness/skill-loading-evidence.md`
- [X] T009 [skillist: speckit-evidence-graph] Extend the evidence graph contract model or parser expectations for skill match confidence, matched signals, ambiguity, and reviewer disposition
- [X] T010 [skillist: speckit-evidence-audit] Extend the evidence audit expectations for risk-level evidence paths, timeout verdict records, and non-authoritative aggregate results
- [X] T011 [skillist: []] Document unsupported scope and Principle IV non-applicability for this governance-only feature in the readiness scaffold

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (P1) - Audit Skill Loading Evidence

### Tests First

- [X] T012 [P] [US1] [T1] [skillist: speckit-evidence-audit, speckit-implement] Add tests or fixtures that reject completed tasks with non-empty skillist when pre-work skill-loading evidence is missing, late, unreadable, ambiguous, or exception-incomplete
- [X] T013 [P] [US1] [T1] [skillist: speckit-implement] Add guidance validation that `speckit-implement` records task id, skill id, resolved path, load result, loaded_at, work_started_at, evidence path, and reviewer exception fields

### Implementation

- [X] T014 [US1] [T1] [skillist: speckit-implement] Implement the per-task skill-loading evidence workflow in implementation guidance and generated implementation command text
- [X] T015 [US1] [T1] [skillist: speckit-evidence-audit, speckit-implement] Implement audit diagnostics that block task completion when declared skill-loading evidence is absent, late, unresolved, or exception-incomplete
- [X] T016 [US1] [T1] [skillist: speckit-evidence-audit, speckit-implement] Write `readiness/skill-loading-evidence.md` with real validation evidence, sample accepted rows, rejection examples, and reviewer exception requirements

**Checkpoint**: US1 independently validates declared-skill load evidence before task completion.

---

## Phase 4: User Story 2 (P1) - Calibrate Applicable-Skill Detection

### Tests First

- [X] T017 [P] [US2] [T1] [skillist: speckit-evidence-graph] Add calibration fixtures covering obvious skill matches, ambiguous matches, indirect ownership matches, false positives, and valid empty skill lists
- [X] T018 [P] [US2] [T1] [skillist: speckit-evidence-graph] Add tests for confidence, matched signals, ambiguity, reviewer disposition, and diagnostics when no skill is selected for a capability-owned task

### Implementation

- [X] T019 [US2] [T1] [skillist: speckit-evidence-graph] Implement skill-match assessment reporting in evidence graph validation without treating heuristic matches as authoritative proof
- [X] T020 [US2] [T1] [skillist: speckit-evidence-graph] Implement reviewer-disposition handling for medium, low, ambiguous, indirect, false-positive, and valid-empty skill assessments
- [X] T021 [US2] [T1] [skillist: speckit-tasks] Update task-generation guidance so generated tasks disclose confidence review needs instead of presenting regex skill detection as certainty
- [X] T022 [US2] [T1] [skillist: speckit-evidence-graph] Write `readiness/skill-detection-calibration.md` with calibration cases, runtime under 30 seconds, accepted dispositions, and remaining uncertainty

**Checkpoint**: US2 independently reports skill-match confidence and reviewer decisions before implementation starts.

---

## Phase 5: User Story 3 (P2) - Make Governance Cost Visible and Proportionate

### Tests First

- [X] T023 [P] [US3] [T1] [skillist: speckit-evidence-audit] Add tests or fixtures for small, medium, and broad governance risk levels with required checks, broad_required, rationale, and missing-evidence failures
- [X] T024 [P] [US3] [T1] [skillist: speckit-tasks] Add generated guidance checks that explain focused validation, broad validation, and non-authoritative aggregate results for each risk level

### Implementation

- [X] T025 [US3] [T1] [skillist: speckit-evidence-audit] Implement governance risk-level evidence validation and final-readiness blocking when the selected evidence path is incomplete
- [X] T026 [US3] [T1] [skillist: speckit-tasks] Update templates, preset commands, and generated guidance to name minimum evidence paths for small, medium, and broad changes
- [X] T027 [US3] [T1] [skillist: speckit-evidence-audit] Write `readiness/governance-risk-levels.md` with representative classifications and the required focused or broad checks for this feature

**Checkpoint**: US3 independently shows the minimum required evidence path for each governance risk level.

---

## Phase 6: User Story 4 (P2) - Diagnose Aggregate Build Hangs

**Skill Rationale**: Phase 6 tasks currently declare `[skillist: []]` because no local capability skill owns FAKE build orchestration or aggregate timeout verdict reporting. This empty selection must be confirmed in `readiness/skill-detection-calibration.md`; if a governance/build capability skill is added or identified, regenerate these task skill lists before implementation.

### Tests First

- [X] T028 [P] [US4] [T1] [skillist: []] Add build-process tests or fixtures for aggregate `Dev` timeout verdicts including stage, elapsed duration, last observed command, focused rerun, and verdict category
- [X] T029 [P] [US4] [T1] [skillist: []] Add focused smoke rerun separation tests proving a passing direct smoke check is not reported as an aggregate product failure after a hang

### Implementation

- [X] T030 [US4] [T1] [skillist: []] Implement bounded aggregate hang diagnostics in `Dev` or readiness reporting with stage timing, last active command, timeout policy, and recommended focused rerun
- [X] T031 [US4] [T1] [skillist: []] Implement verdict classification for timeout, orchestration concern, non-authoritative aggregate result, product failure, and environment failure
- [X] T032 [US4] [T1] [skillist: []] Write `readiness/aggregate-hang-diagnostics.md` with simulated or reproduced hang evidence, focused rerun command, focused result, and final classification

**Checkpoint**: US4 independently separates aggregate timeout evidence from deterministic product failures.

---

## Phase 7: User Story 5 (P3) - State Runtime Portability Roadmap Boundaries

### Tests First

- [X] T033 [P] [US5] [skillist: []] Add documentation checks that require runtime limitation notes to name platform, renderer, dependency, fallback, and toolchain boundaries without claiming new support

### Implementation

- [X] T034 [US5] [skillist: []] Write `readiness/runtime-limitations.md` with current .NET 10 desktop, Vulkan/SkiaSharp preview, unsupported macOS/mobile/browser, and no software-renderer fallback boundaries
- [X] T035 [US5] [skillist: []] Update product readiness or roadmap documentation to distinguish current support from separate future platform-expansion features

**Checkpoint**: US5 independently documents runtime limitations without expanding product support.

---

## Phase 8: Integration & Polish

- [X] T036 [skillist: speckit-evidence-graph] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/015-improve-governance-weaknesses --graph-only` and capture clean graph output in `readiness/evidence-graph.md`
- [X] T037 [skillist: speckit-evidence-graph, speckit-evidence-audit] Run `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`, then capture PASS or explicit blocking diagnostics in `readiness/evidence-audit.md`
- [X] T038 [skillist: speckit-tasks, speckit-implement] Run `./fake.sh build -t GeneratedGuidanceCheck` and confirm generated task and implementation guidance preserve skillist, skill-loading evidence, confidence reporting, and risk-level rules
- [X] T039 [skillist: speckit-implement] Run focused governance tests for skill-loading evidence, skill-match calibration, risk levels, aggregate timeout verdicts, and runtime limitation docs
- [X] T040 [skillist: []] Run `./fake.sh build -t Dev` only if the final declared risk level remains broad, otherwise document why focused validation is sufficient
- [X] T041 [skillist: speckit-evidence-audit, speckit-implement] Validate this feature's completed non-empty skillist tasks against `readiness/skill-loading-evidence.md`, including loaded skill path, load timing, work start timing, and reviewer-visible exceptions
- [X] T042 [skillist: speckit-evidence-audit] Complete final readiness review with synthetic inventory, non-authoritative aggregate verdicts, unsupported scope, and no package/API/runtime support changes

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| _(none yet)_ | | | |

# Tasks: Task Validator Feedback Follow-ups

**Feature branch**: `033-fix-task-validator-feedback`
**Spec**: `specs/033-fix-task-validator-feedback/spec.md`
**Plan**: `specs/033-fix-task-validator-feedback/plan.md`

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
`synthetic-error-handling-approved` label. It still remains `[S]` when
completed with synthetic-only malformed-input or explicit error-path evidence.
The classification must be assigned during design, planning, clarification, or
task generation. implementation-time relabeling is forbidden; newly discovered
needs go back to task/design review.

## Vertical-slice rule (US phases)

A task tagged `[US*]` may only be marked `[X]` when the change is reachable
from the user-facing governance surface and that path was actually exercised:
validator execution, generated guidance scan, command output capture, or a
readiness artifact under `specs/033-fix-task-validator-feedback/readiness/`.
Runtime FS.Skia.UI API or rendering evidence is out of scope for this feature.

Principle IV is not applicable to runtime MVU because this feature changes
governance scripts, Markdown guidance, generated template text, and governance
tests only. Script I/O remains at the validator command edge; deterministic
matching and id-resolution helpers should be tested directly.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, ... — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change
- **[SEH]** — design-approved synthetic error-handling task paired with
  `synthetic-error-handling-approved`

Every task has a matching entry in `tasks.deps.yml` and mirrors the structured
`skillist` value using `[skillist: ...]`. `tasks.deps.yml` uses indented object
metadata with exactly one key per task id; inline maps, duplicate keys,
dangling dependency ids, and mirror mismatches are invalid.

Task graph validator pitfall guidance: titles that clearly request graph
validation, audit validation, task authoring, implementation command loading,
or constitution work imply specific Spec Kit skills. Setup or readiness tasks
that only cite mandated filenames should either use safe wording or the
`Complete readiness notes` prefix. Avoid unrelated trigger phrases such as
`window visibility validation fixture` unless the task actually owns viewer
window evidence.

## Canonical Verification Targets

FAKE-backed commands share repository `.fake` state and must run sequentially
when more than one is needed. Use this deterministic order for broad validation:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

Risk level: medium governance risk. Focused validation is required for changed
governance tests, direct validator fixtures, guidance scans, and graph-only
output capture. Broad validation is required after touching shared templates,
generated product guidance, or build command surfaces. Aggregate FAKE results
are non-authoritative until the named focused evidence files are refreshed.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Record governance scope, feature risk level, deferred runtime scope, and package impact in the readiness notes
- [X] T002 [P] [skillist: []] Complete readiness notes for required contract evidence file placeholders
- [X] T003 [P] [skillist: []] Classify each follow-up item by validator behavior, author guidance, registry guidance, advisory guidance, or output labeling
- [X] T004 [skillist: []] Record public API, package, and MVU non-applicability with the required evidence obligations

---

## Phase 2: Foundation

- [S] T005 [P] [SEH] synthetic-error-handling-approved [skillist: []] Add malformed-title fixture tests for filename-bound trigger tokens
- [S] T006 [P] [SEH] synthetic-error-handling-approved [skillist: []] Add fixture tests for directory-like skill declarations that resolve to a different declared id
- [X] T007 [P] [skillist: speckit-tasks] Add guidance scan tests for generated task guidance coverage and safe setup-title examples
- [X] T008 [P] [skillist: speckit-evidence-graph] Add regression tests that preserve graph checks for cycles, missing deps, mirror mismatches, unreadable skills, and skill ordering
- [X] T009 [skillist: []] Define shared trigger-token, filename-context, registry-entry, and run-label helper boundaries for the validator script

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 - Avoid False Skill Requirements From Required Filenames

### Tests First

- [S] T010 [P] [US1] [SEH] synthetic-error-handling-approved [skillist: []] Add a validator fixture where a setup title cites a mandated readiness filename but requests no implementation workflow
- [S] T011 [P] [US1] [SEH] synthetic-error-handling-approved [skillist: []] Add whole-word positive fixtures that still require the intended Spec Kit workflow skill

### Implementation

- [X] T012 [US1] [skillist: []] Implement token-aware title matching with filename and longer-word exclusions
- [X] T013 [US1] [skillist: []] Capture `title-trigger-validation.md` with failing-first and passing validator fixture output

**Checkpoint**: US1 validates independently through the title-trigger readiness evidence.

---

## Phase 4: User Story 2 - Discover Escape Hatches And Trigger Tokens

### Tests First

- [X] T014 [P] [US2] [skillist: speckit-tasks] Add generated task guidance tests for the readiness prefix, enforced trigger groups, and safe setup-title examples

### Implementation

- [X] T015 [US2] [skillist: speckit-tasks] Update repository and preset task templates with blocking rule documentation and safe setup wording
- [X] T016 [US2] [skillist: speckit-tasks] Capture `task-guidance-scan.md` showing all enforced groups, the readiness prefix, and three safe examples

**Checkpoint**: US2 validates independently through the real guidance scan.

---

## Phase 5: User Story 3 - Resolve Skill Registry Names Without Guesswork

### Tests First

- [S] T017 [P] [US3] [SEH] synthetic-error-handling-approved [skillist: []] Add a registry mismatch diagnostic fixture for a readable skill whose directory differs from its declared id

### Implementation

- [X] T018 [US3] [skillist: []] Improve skill registry diagnostics to report the accepted declared id and source path for directory-like declarations
- [X] T019 [US3] [skillist: speckit-tasks] Update task author guidance to identify the authoritative skill registry roots and declared-id rule
- [X] T020 [US3] [skillist: []] Capture `skill-registry-diagnostics.md` with the mismatch diagnostic and author-facing accepted id

**Checkpoint**: US3 validates independently through registry diagnostic evidence.

---

## Phase 6: User Story 4 - Keep Guidance Aligned With Enforced Rules

### Tests First

- [X] T021 [P] [US4] [skillist: speckit-tasks] Add a coverage test that compares enforced trigger groups against published guidance text

### Implementation

- [X] T022 [US4] [skillist: speckit-tasks] Centralize or mirror the enforced trigger-group vocabulary so guidance and validator expectations stay reviewable together
- [X] T023 [US4] [skillist: speckit-tasks] Remove obsolete-only enforced-failure examples or relabel them as advisory suggestions

**Checkpoint**: US4 validates independently through guidance and validator coverage tests.

---

## Phase 7: User Story 5 - Improve Advisory Capability And Mode Signals

### Tests First

- [X] T024 [P] [US5] [skillist: speckit-evidence-graph] Add graph-only command output tests for success and failure labels
- [X] T025 [P] [US5] [skillist: speckit-tasks] Add guidance tests proving FS.Skia.UI capability hints cover at least five categories without becoming blocking rules

### Implementation

- [X] T026 [US5] [skillist: speckit-evidence-graph, speckit-evidence-audit] Update graph-only command and report output to identify graph validation and direct merge-gate checks to EvidenceAudit
- [X] T027 [US5] [skillist: speckit-tasks] Add advisory FS.Skia.UI capability guidance for rendering, viewer, input, layout, testing, and evidence tasks
- [X] T028 [US5] [skillist: []] Capture `advisory-capability-guidance.md` and `graph-only-output-label.md` with non-blocking and output-label proof

**Checkpoint**: US5 validates independently through advisory guidance and output-label evidence.

---

## Phase 8: Integration & Polish

- [X] T029 [skillist: []] Run focused governance tests for validator behavior, guidance coverage, registry diagnostics, and output labels
- [X] T030 [skillist: speckit-evidence-graph] Run direct graph validation for `specs/033-fix-task-validator-feedback` and refresh `readiness/task-graph.md`
- [X] T031 [skillist: []] Run `./fake.sh build -t Dev` sequentially and record any non-authoritative aggregate result
- [X] T032 [skillist: speckit-tasks] Run `./fake.sh build -t GeneratedGuidanceCheck` sequentially after template and guidance edits
- [X] T033 [skillist: []] Run `./fake.sh build -t TemplateCheck` sequentially if template-owned files changed
- [X] T034 [skillist: []] Run `./fake.sh build -t GeneratedProductCheck` sequentially if generated product guidance changed
- [X] T035 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` sequentially and confirm the graph-only label evidence
- [X] T036 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` sequentially and document PASS or every accepted synthetic override
- [X] T037 [skillist: []] Reconcile all required readiness files, risk-level notes, and synthetic evidence disclosures before work handoff

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T005 | Token-boundary matching needs malformed title strings that reproduce validator false-positive behavior without depending on a real feature task. | `specs/033-fix-task-validator-feedback/readiness/title-trigger-validation.md` | N/A | synthetic-error-handling-approved | `specs/033-fix-task-validator-feedback/plan.md` synthetic evidence decision | malformed task title / filename-context fixture | Validator reports no required skill for filename-only token matches and still reports clear whole-token matches. | accepted-seh |
| T006 | Directory/id mismatch diagnostics need synthetic registry metadata to isolate the error path from current repository skill names. | `specs/033-fix-task-validator-feedback/readiness/skill-registry-diagnostics.md` | N/A | synthetic-error-handling-approved | `specs/033-fix-task-validator-feedback/contracts/skill-registry-diagnostics.md` | mismatched skill directory and declared `name:` fixture | Validator identifies the accepted declared id and source path. | accepted-seh |
| T010 | The setup-title false-positive case is a validator edge input, not a runtime workflow. | `specs/033-fix-task-validator-feedback/readiness/title-trigger-validation.md` | N/A | synthetic-error-handling-approved | `specs/033-fix-task-validator-feedback/contracts/title-trigger-validation.md` | mandated readiness filename embedded in task title | Validator accepts `skillist: []` when no workflow is requested. | accepted-seh |
| T011 | Positive token fixtures must prove the error fix does not remove existing required-skill protections. | `specs/033-fix-task-validator-feedback/readiness/title-trigger-validation.md` | N/A | synthetic-error-handling-approved | `specs/033-fix-task-validator-feedback/contracts/title-trigger-validation.md` | whole-word workflow trigger title fixture | Validator blocks omitted required Spec Kit skill and reports the matched group. | accepted-seh |
| T017 | The directory-like declaration failure is an explicit metadata error path best isolated with a fixture. | `specs/033-fix-task-validator-feedback/readiness/skill-registry-diagnostics.md` | N/A | synthetic-error-handling-approved | `specs/033-fix-task-validator-feedback/contracts/skill-registry-diagnostics.md` | invalid skill id declaration for a readable skill path | Validator failure includes the accepted declared id and next action. | accepted-seh |

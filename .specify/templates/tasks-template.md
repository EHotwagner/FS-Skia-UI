# Tasks: [FEATURE_NAME]

**Feature branch**: `[FEATURE_BRANCH]`
**Spec**: `specs/[FEATURE_ID]/spec.md`
**Plan**: `specs/[FEATURE_ID]/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

## Vertical-slice rule (US phases)

A task tagged `[US*]` may only be marked `[X]` when the change is
reachable from a user-facing entry point and that path was actually
exercised — an FSI session against the packed library, a smoke run of the
application, a manual walk-through with transcript, or a screenshot
captured under `readiness/`. Domain, model, or core-layer changes alone
do **not** satisfy `[X]` for a `[US*]` task, even if their unit tests
pass green. If the user-reachable surface is missing, stubbed, or not
yet wired, mark `[ ]` (work continues) or `[S]` with a disclosed reason
in the Synthetic-Evidence Inventory — never `[X]`.

For stateful or I/O-bearing stories, `[X]` also requires Elmish/MVU evidence:
the public `Model` / `Msg` / `Effect` or `Cmd<Msg>` contract was exercised,
pure `update` transitions were tested, emitted effects were asserted, and
the effect interpreter was run against real dependencies where safe.

This rule does not apply to Setup, Foundation, Integration, or Polish
phase tasks; those are evaluated against their own phase verification.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, … — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change

Every task must have a matching entry in `tasks.deps.yml` even if its
dependency list is empty. Every task line MUST mirror the structured
`skillist` value using `[skillist: ...]`; use `[skillist: []]` when no
capability skill applies. The `speckit.evidence.graph` command refuses to
proceed with dangling references or invalid task skill metadata.

## Canonical Verification Targets

Generated tasks should call repository targets instead of duplicating raw
restore/build/test/package/evidence command order:

- `./fake.sh build -t Dev` for fast local verification.
- `./fake.sh build -t Verify` for the full governed workflow.
- `./fake.sh build -t PackLocal` for local package output.
- `./fake.sh build -t RefreshSurfaceBaselines` for intentional current surface
  baseline refreshes.
- `./fake.sh build -t PackageSurfaceCheck` for package surface review.
- `./fake.sh build -t FsiTranscripts` for public FSI evidence.
- `./fake.sh build -t SampleContractSmoke` for sample smoke evidence.
- `./fake.sh build -t TemplateCheck` for source/package default/minimal
  generated project validation.
- `./fake.sh build -t DependencyReport` for central package governance.
- `./fake.sh build -t GeneratedGuidanceCheck` for generated prompt,
  task-skill, and implementation guidance governance.
- `./fake.sh build -t TemplateDrift` for template-owned drift and deferral
  validation.
- `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`
  for graph and synthetic-evidence gates.

After task generation, evaluate every task against available local capability
skills (`.agents/skills/*/SKILL.md`, `src/*/skill/SKILL.md`, and template
capability skills). Write the minimal ordered skill set to structured
`skillist` metadata and mirror it in `tasks.md`. Capability skills are
preferred over generic guidance for matching tasks.

Keep `tasks.deps.yml`, the `skillist` mirror, and the
`speckit.evidence.graph` status refresh requirements in generated task lists.

Template source: `.specify/presets/fsharp-opinionated/templates/tasks-template.md`.

---

## Phase 1: Setup

- [ ] T001 [skillist: []] Scaffold the feature directory and link spec + plan
- [ ] T002 [P] [skillist: []] Add baseline install or adoption documentation for the selected profile
- [ ] T003 [P] [skillist: []] Add readiness artifact scaffolding (`specs/[FEATURE_ID]/readiness/`)
- [ ] T004 [skillist: []] Record feature Tier, affected layer, public-API impact, Elmish/MVU applicability, and required evidence obligations

---

## Phase 2: Foundation

- [ ] T005 [skillist: []] Draft the public surface as `.fsi` signature(s), including `Model`, `Msg`, `Effect` or `Cmd<Msg>`, `init`, `update`, and interpreter boundary for stateful or I/O-bearing features
- [ ] T006 [P] [skillist: []] Add or update constitutional guidance that this feature touches
- [ ] T007 [P] [skillist: []] Define or update operational workflows, commands, reports, or scripts
- [ ] T008 [skillist: []] Exercise the draft `.fsi` from FSI (`scripts/prelude.fsx` or ad-hoc), including representative `init` / `update` paths when MVU applies, and capture the session transcript to `readiness/fsi-session.txt`
- [ ] T009 [skillist: []] Record surface-area baselines for the new / changed public modules
- [ ] T010 [skillist: []] Record unsupported-scope handling and failure diagnostics

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1)

### Tests First (Principle I, Principle VI)

- [ ] T011 [P] [US1] [skillist: []] Add semantic tests that load the packed library (or prelude), exercise the US1 surface, and assert MVU state transitions plus emitted effects when applicable
- [ ] T012 [P] [US1] [skillist: []] Add verification for the US1 outcome against the readiness artifact, including real interpreter evidence for effects where safe

### Implementation

- [ ] T013 [P] [US1] [skillist: []] Add story-specific contracts, docs, or fixtures
- [ ] T014 [P] [US1] [skillist: []] Add any required sample or schema artifacts
- [ ] T015 [US1] [skillist: []] Implement the primary user-facing behavior for the story, keeping MVU `update` pure when applicable
- [ ] T016 [US1] [skillist: []] Connect the story's effect interpreter to canonical readiness artifacts or workflows
- [ ] T017 [US1] [skillist: []] Add validation and actionable failure diagnostics
- [ ] T018 [US1] [skillist: []] Document the story's independent validation path

**Checkpoint**: User Story 1 is fully functional and testable independently.

---

## Phase 4: User Story 2 (US2)

### Tests First

- [ ] T019 [P] [US2] [skillist: []] Add semantic tests exercising the US2 surface through FSI, including MVU transitions and effects when applicable
- [ ] T020 [P] [US2] [skillist: []] Add validation for the US2 readiness outcome, including real interpreter evidence where safe

### Implementation

- [ ] T021 [P] [US2] [skillist: []] Add story-specific contracts, docs, or fixtures
- [ ] T022 [US2] [skillist: []] Implement the primary user-facing behavior for the story

**Checkpoint**: User Story 2 is fully functional and testable independently.

---

## Phase 5: Integration & Polish

- [ ] T023 [skillist: []] Surface-area baseline refresh (Tier 1 only)
- [ ] T024 [skillist: []] Run the packed library through the numbered example scripts and confirm none are broken
- [ ] T025 [skillist: speckit-evidence-graph] Run `speckit.evidence.graph` — confirm no cycles, no dangling refs, no `[S*]` surprises
- [ ] T026 [skillist: speckit-evidence-audit] Run `speckit.evidence.audit` — confirm verdict PASS or document every `--accept-synthetic` override

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| _(none yet)_ | | | |

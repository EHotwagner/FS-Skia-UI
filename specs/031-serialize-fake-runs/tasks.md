# Tasks: Serialize FAKE Runs

**Feature branch**: `031-serialize-fake-runs`
**Spec**: `specs/031-serialize-fake-runs/spec.md`
**Plan**: `specs/031-serialize-fake-runs/plan.md`

## Status Legend

- `[ ]` - pending
- `[X]` - done with real evidence
- `[S]` - done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` - failed
- `[-]` - skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. It still remains `[S]` when
completed with synthetic-only malformed-input or explicit error-path evidence.

Every task has matching structured metadata in `tasks.deps.yml`; every task
line mirrors the `skillist` value.

## Phase 1: Setup

- [X] T001 [P] [skillist: speckit-tasks, speckit-implement] Create `specs/031-serialize-fake-runs/readiness/` placeholders for `sequential-fake-validation.md`, `guidance-scan.md`, `fake-command-order.md`, `evidence-graph.md`, `evidence-audit.md`, `governance-risk-levels.md`, `generated-validation-authority.md`, `skill-loading-evidence-workflow.md`, `audit-diagnostics.md`, `readiness-contract-discovery.md`, `framework-guidance.md`, and `evidence-vocabulary.md`
- [X] T002 [P] [skillist: []] Record feature scope: Tier 2 governance/docs change, no package identity changes, no public F# API, no runtime UI/rendering change, and no MVU/effect boundary changes
- [X] T003 [P] [skillist: []] Inventory repository, agent, generated-template, and readiness guidance surfaces that mention `fake.sh`, `fake.cmd`, `dotnet fake`, FAKE-backed tests, or FAKE targets
- [X] T004 [skillist: []] Record governance risk levels: small for single guidance edits, medium for generated-template guidance or scanner changes, broad when `Verify` or generated package output is needed; note non-authoritative aggregate logs separately from command-order evidence

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: []] Add failing-first governance scanner expectations for required semantics: FAKE-backed command class, `.fake` race risk, sequential execution, deterministic order, and non-FAKE parallelism distinction
- [X] T006 [P] [skillist: fs-skia-template-update] Add failing-first generated guidance expectations that template source and generated product artifacts carry the sequential FAKE rule
- [S] T007 [P] [SEH] synthetic-error-handling-approved [skillist: []] Add negative scanner fixtures for malformed or unsafe FAKE guidance snippets that omit `.fake`, omit sequential order, or imply concurrent FAKE execution
- [X] T008 [skillist: []] Confirm Principle IV is not applicable: this feature changes guidance and validation scans only, with filesystem/process effects remaining at existing build/test command boundaries
- [X] T009 [skillist: []] Define readiness evidence field checks for command order, working directory, purpose, relative or timestamped start/end order, exit code, log path, and race-like failure triage classification

**Checkpoint**: Foundation ready - story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) - Run Repository Validation Reliably

### Tests First

- [X] T010 [P] [US1] [skillist: []] Extend repository guidance tests to fail when updated maintainer validation instructions list multiple FAKE-backed commands without deterministic sequential ordering
- [X] T011 [P] [US1] [skillist: []] Extend readiness contract tests to require command-order evidence whenever more than one FAKE-backed command supports a readiness claim

### Implementation

- [X] T012 [P] [US1] [skillist: []] Update maintainer-facing repository docs (`README.md`, `docs/build.md`, `docs/testing.md`, `docs/evidence.md`, and related validation docs) so FAKE-backed commands are listed one at a time and named as unsafe to run concurrently because of shared `.fake` state
- [X] T013 [US1] [skillist: []] Update build/readiness guidance text emitted by repository validation paths so race-like FAKE failures tell maintainers to rerun affected FAKE-backed commands sequentially before product debugging
- [X] T014 [US1] [skillist: []] Produce `readiness/guidance-scan.md` with the repository guidance paths checked, required concepts found or missing, and repairs completed
- [X] T015 [US1] [skillist: []] Document the independent US1 validation path in `readiness/sequential-fake-validation.md` with the exact serialized command order used for focused repository validation

**Checkpoint**: US1 is independently testable through repository guidance scan plus sequential FAKE command evidence.

---

## Phase 4: User Story 2 (US2) - Guide Agents Away From Unsafe Parallelism

### Tests First

- [X] T016 [P] [US2] [skillist: []] Extend agent-facing governance tests to fail when `AGENTS.md`, `CLAUDE.md`, `.agents/skills/*`, `.claude/skills/*`, or `.claude/commands/*` mention FAKE-backed validation without sequential execution guidance
- [X] T017 [P] [US2] [skillist: fs-skia-template-update] Extend generated agent guidance checks to fail when generated `.agents/skills/` or `.claude/skills/` output omits the sequential FAKE rule

### Implementation

- [X] T018 [P] [US2] [skillist: []] Update repository agent instructions so agents may parallelize safe non-FAKE reads/checks but must not run any FAKE-backed tests or FAKE targets concurrently
- [X] T019 [P] [US2] [skillist: fs-skia-template-update] Update template-generated agent skill and command guidance so generated products list development, test, verification, and evidence-gate FAKE-backed commands as serialized work when multiple are needed
- [X] T020 [US2] [skillist: []] Refresh guidance scan evidence showing every updated agent-facing FAKE-backed instruction names `.fake`, sequential execution, and the non-FAKE parallelism exception

**Checkpoint**: US2 is independently testable through agent guidance scans and generated guidance checks.

---

## Phase 5: User Story 3 (US3) - Diagnose Race-Like Failures Clearly

### Tests First

- [X] T021 [P] [US3] [skillist: []] Add failure-triage tests for readiness notes that require failed command, concurrent FAKE context, `.fake` race classification, sequential rerun order, and follow-up classification
- [X] T022 [P] [US3] [skillist: fs-skia-template-update] Add generated product documentation checks requiring the same race-like failure triage guidance in generated README/product docs

### Implementation

- [X] T023 [P] [US3] [skillist: []] Update readiness templates, quickstart guidance, and failure notes so suspected or unknown concurrent FAKE context requires a sequential rerun before product-regression claims
- [X] T024 [P] [US3] [skillist: fs-skia-template-update] Update `template/base/README.md`, `template/base/docs/product.md`, and generated product guidance sources with the sequential rerun triage rule
- [X] T025 [US3] [skillist: []] Complete `readiness/fake-command-order.md` with the focused command sequence, expected log paths, and the rule that aggregate `Verify` evidence is broad but not a substitute for ordered focused logs

**Checkpoint**: US3 is independently testable through triage guidance tests and readiness evidence review.

---

## Phase 6: Integration & Polish

- [X] T026 [skillist: []] Run `dotnet tool restore` and record it as non-FAKE setup before any FAKE-backed validation command
- [X] T027 [skillist: []] Run `./fake.sh build -t Dev` as the first focused FAKE-backed validation command and record start/end order, exit code, and log path
- [X] T028 [skillist: []] Run `./fake.sh build -t GeneratedGuidanceCheck` after `Dev` completes and record order, exit code, and log path
- [X] T029 [skillist: fs-skia-template-update] Run `./fake.sh build -t TemplateCheck` after `GeneratedGuidanceCheck` completes and record order, exit code, and log path
- [X] T030 [skillist: fs-skia-template-update] Run `./fake.sh build -t GeneratedProductCheck` after `TemplateCheck` completes and record order, exit code, and log path
- [X] T031 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` after generated product validation completes; refresh `readiness/evidence-graph.md`, `readiness/task-graph.md`, and `readiness/task-graph.json`
- [X] T032 [skillist: speckit-evidence-graph, speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` after `EvidenceGraph` completes; refresh `readiness/evidence-audit.md` and record synthetic propagation plus diff-scan verdict
- [X] T033 [skillist: []] Complete readiness notes with final command order, failure triage outcome, generated validation authority, skill-loading workflow notes, and any non-authoritative aggregate results
- [-] T034 [skillist: []] Optionally run `./fake.sh build -t Verify` as one final broad FAKE-backed command only after focused commands are clean; skipped because the optional broad aggregate is non-authoritative for this feature and the required focused FAKE-backed sequence passed sequentially

---

## Skill Evaluation Notes

| Task range | Capability review |
|------------|-------------------|
| T001 | `speckit-tasks` and `speckit-implement` selected with high confidence because the task creates the skill-loading workflow placeholder used by generated implementation batches. |
| T002-T005, T008-T016, T018, T020-T021, T023, T025-T028, T033-T034 | Valid empty: no local capability skill materially narrows generic docs, scanner, or readiness work. |
| T006, T017, T019, T022, T024, T029-T030 | `fs-skia-template-update` selected with medium confidence because the work touches repo-owned template/generated-product guidance and generated validation. Package-version steps from that skill are not required by this feature. |
| T007 | Valid empty with `[SEH]`: design-approved synthetic malformed guidance snippets for negative scanner behavior; no implementation capability skill applies. |
| T031 | `speckit-evidence-graph` selected with high confidence for graph/readiness validation. |
| T032 | `speckit-evidence-graph` then `speckit-evidence-audit` selected with required prerequisite order. |

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T007 | Negative scanner coverage needs malformed or unsafe guidance snippets that should not appear in real docs. | `specs/031-serialize-fake-runs/readiness/guidance-scan.md` plus focused governance test output | N/A | synthetic-error-handling-approved | `specs/031-serialize-fake-runs/research.md` decision "Validate guidance through focused text checks and generated artifact checks"; `contracts/guidance-contract.md` validation expectations | Malformed or unsafe validation guidance text that omits required `.fake` or sequential semantics, or implies concurrent FAKE execution | Scanner reports path/snippet context and rejects the unsafe guidance without changing production docs | accepted-seh |

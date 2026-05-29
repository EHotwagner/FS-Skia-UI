# Tasks: Claude Code Ready Spec Kit

**Feature branch**: `030-claude-code-ready`
**Spec**: `specs/030-claude-code-ready/spec.md`
**Plan**: `specs/030-claude-code-ready/plan.md`

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
from a user-facing entry point and that path was actually exercised by the
named validation target, generated product check, manual transcript, or
readiness artifact under `specs/030-claude-code-ready/readiness/`.

For this feature, Principle IV applies to repository and template I/O
workflows rather than app Elmish state: source records, render decisions, and
drift comparisons must be testable as pure data transformations where
practical, while file writes, process execution, hook validation, template
instantiation, and report emission stay at build/generator boundaries. No
public F# API is planned; if a reusable module is introduced, add `.fsi`,
semantic tests, and surface baselines before implementation work continues.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, … — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change
- **[SEH]** — design-approved synthetic error-handling task paired with
  `synthetic-error-handling-approved`

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors
the structured `skillist` value using `[skillist: ...]`; `[skillist: []]`
means no available capability skill materially applies.

## Canonical Verification Targets

- `./fake.sh build -t Dev` for fast local verification.
- `./fake.sh build -t Verify` for the full governed workflow.
- `./fake.sh build -t TemplateCheck` for generated source/package profile validation.
- `./fake.sh build -t GeneratedGuidanceCheck` for generated prompt, task-skill, and implementation guidance governance.
- `./fake.sh build -t TemplateDrift` for template-owned drift and deferral validation.
- `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit` for graph and synthetic-evidence gates.

Risk levels for this feature:

- **Small**: a single repository or generated artifact path changes; run the focused unit/governance test plus the owning focused target.
- **Medium**: source generation, settings, hooks, or one template profile changes; run `GeneratedGuidanceCheck`, `TemplateDrift`, and the relevant `TemplateCheck` slice.
- **Broad**: shared source model, profile coverage, package template content, or validation aggregation changes; run `TemplateCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`, and `Verify`. If broad validation is blocked by environment health, record non-authoritative aggregate results separately in readiness and do not treat them as final sign-off.

Skill evaluation summary:

- `fs-skia-template-update` applies to tasks that change or verify template package/profile output.
- `speckit-evidence-graph` applies only to graph metadata/`EvidenceGraph` tasks.
- `speckit-evidence-audit` applies only to final audit tasks and is ordered after `speckit-evidence-graph`.
- Runtime capability skills (`fs-skia-scene`, `fs-skia-skiaviewer`, `fs-skia-elmish`, `fs-skia-keyboard-input`, `fs-skia-layout`, `fs-skia-ui-widgets`, `fs-skia-testing`, `fs-skia-layout-evidence`, generated controls/samples/project skills) are valid-empty for this task list because product runtime behavior, visual output, and public capability APIs are out of scope.

---

## Phase 1: Setup

- [X] T001 [P] [skillist: []] Confirm branch, feature directory, current plan/spec/contracts, and existing agent/template artifact inventory
- [X] T002 [P] [skillist: []] Create or refresh readiness placeholders for Claude research, repository inventory, sync validation, generated template artifacts, generated project readiness, governance risk levels, runtime limitations, generated validation authority, capability loading workflow, audit diagnostics, readiness contract discovery, framework guidance, and evidence vocabulary
- [X] T003 [P] [skillist: []] Record Tier 1 classification, affected repository/template/build layers, no expected package identity/version change, and no expected public `.fsi` surface
- [X] T004 [P] [skillist: []] Record Principle IV workflow boundaries for shared source rendering, drift comparison, settings validation, hook validation, and template file-system effects
- [X] T005 [skillist: []] Document capability skill evaluation outcomes, valid-empty runtime skill disposition, and skill evidence recording expectations

---

## Phase 2: Foundation

- [X] T006 [P] [skillist: []] Add failing governance tests for repository Claude project instructions, project skills, settings, hooks, and command-alias rules
- [X] T007 [P] [skillist: fs-skia-template-update] Add failing generated-product tests proving every template profile that emits Codex artifacts also emits Claude artifacts
- [X] T008 [P] [skillist: []] Add failing drift tests for Codex/Claude source parity and actionable mismatch report fields
- [X] T009 [P] [skillist: []] Add failing validation tests for malformed `.claude/settings.json`, non-project-local hook paths, missing hook scripts, and user-local settings dependency leaks
- [X] T010 [skillist: []] Define shared synchronization source records for instructions, lifecycle workflows, git extension workflows, evidence extension workflows, settings, hooks, and optional command aliases
- [X] T011 [skillist: []] Implement shared render/validation helpers for Codex and Claude artifact pairs with file writes kept at build or generator edges
- [X] T012 [skillist: []] Wire focused build validation and report paths for repository sync, generated guidance, template drift, and Claude settings or hook diagnostics
- [X] T013 [skillist: []] Seed readiness report writers for repository inventory, config sync validation, generated template artifacts, generated project readiness, and Claude Code research mapping
- [X] T014 [skillist: []] Record foundation evidence obligations, unsupported scope handling, and the no-public-API decision in readiness
- [X] T049 [skillist: []] Validate and record whether planned code changes introduced any reusable public `src/*` F# module; if yes, add `.fsi`, semantic FSI tests, surface-area baseline updates, and compatibility notes before further work continues

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) - Repository Works In Claude Code

### Tests First

- [X] T015 [P] [US1] [skillist: []] Add tests that `CLAUDE.md` exists, imports `AGENTS.md`, avoids duplicated Codex instructions, and preserves active-plan guidance
- [X] T016 [P] [US1] [skillist: []] Add tests that repository `.claude/skills/<workflow>/SKILL.md` files exist for lifecycle, git extension, and evidence extension workflows with valid discovery metadata
- [X] T017 [P] [US1] [skillist: []] Add tests that repository `.claude/settings.json` is valid project-shareable JSON and references only supported project-local hooks

### Implementation

- [X] T018 [US1] [skillist: []] Generate repository `CLAUDE.md` from the shared instruction source with `AGENTS.md` import semantics
- [X] T019 [US1] [skillist: []] Generate repository Claude project skills and optional command aliases from the same workflow sources as Codex skills
- [X] T020 [US1] [skillist: []] Generate repository `.claude/settings.json` and any validated project-local hook scripts for supported workflows
- [X] T021 [US1] [skillist: []] Write repository-agent-inventory readiness evidence listing Codex and Claude artifacts by class, source id, path, and validation status
- [X] T022 [US1] [skillist: []] Run focused repository guidance validation and capture the independent US1 validation path in readiness

**Checkpoint**: User Story 1 is fully functional and testable independently.

---

## Phase 4: User Story 2 (US2) - Generated Projects Are Claude Code Ready

### Tests First

- [X] T023 [P] [US2] [skillist: fs-skia-template-update] Add source-template validation for generated `CLAUDE.md`, `.claude/settings.json`, and Claude skills in all supported profiles that emit Spec Kit agent artifacts
- [X] T024 [P] [US2] [skillist: fs-skia-template-update] Add package-template validation proving packed template contents include the same Claude artifacts as source template output
- [X] T025 [P] [US2] [skillist: fs-skia-template-update] Add generated-project validation proving selected capability skills copied into `.agents/skills` have matching `.claude/skills` peers

### Implementation

- [X] T026 [US2] [skillist: fs-skia-template-update] Update template base artifacts to emit generated-product `CLAUDE.md`, `.claude/settings.json`, project skill, and supported workflow skills
- [X] T027 [US2] [skillist: fs-skia-template-update] Update profile/capability copying so generated Claude skill coverage follows the same selected capabilities as Codex skill coverage
- [X] T028 [US2] [skillist: fs-skia-template-update] Update template package content checks and generated file-list reports for Claude artifact coverage
- [X] T029 [US2] [skillist: fs-skia-template-update] Run source and package generated-project validation and record generated-template-agent-artifacts plus generated-project-claude-code-ready readiness evidence

**Checkpoint**: User Story 2 is fully functional and testable independently.

---

## Phase 5: User Story 3 (US3) - Codex And Claude Stay Synchronized

### Tests First

- [X] T030 [P] [US3] [skillist: []] Add deliberate one-line drift fixture coverage for Codex skill changed without Claude peer update
- [X] T031 [P] [US3] [skillist: []] Add deliberate one-line drift fixture coverage for Claude skill changed without Codex peer update
- [X] T032 [P] [US3] [skillist: []] Add tests that drift diagnostics name scope, source id, workflow id, expected path, actual path, difference summary, and repair action

### Implementation

- [X] T033 [US3] [skillist: []] Implement source-id based drift comparison for repository instruction, workflow, settings, hook, and optional command-alias artifacts
- [X] T034 [US3] [skillist: fs-skia-template-update] Extend template drift validation to fail when generated template output omits or mismatches a required Claude peer
- [X] T035 [US3] [skillist: []] Wire repair actions or regeneration guidance into drift reports and focused build failures
- [X] T036 [US3] [skillist: []] Run the controlled drift validation, restore generated artifacts, and record passing plus failing diagnostics in config-sync-validation readiness evidence

**Checkpoint**: User Story 3 is fully functional and testable independently.

---

## Phase 6: User Story 4 (US4) - Current Claude Code Guidance Is Captured

### Tests First

- [X] T037 [P] [US4] [T2] [skillist: []] Add documentation checks that Claude Code research evidence cites official sources with retrieval dates and maps each source to implemented artifacts
- [X] T038 [P] [US4] [T2] [skillist: []] Add checks that generated guidance distinguishes project-shareable settings from user-local Claude settings and identifies supported hook limitations

### Implementation

- [X] T039 [US4] [T2] [skillist: []] Refresh Claude Code research readiness notes with official documentation links, retrieval date 2026-05-29, supported concepts, and limitations
- [X] T040 [US4] [T2] [skillist: []] Update framework and generated-product guidance so Claude users can discover project skills, settings, hooks, optional command aliases, and active-plan reading behavior
- [X] T041 [US4] [T2] [skillist: []] Record reviewer-facing limitations for unsupported hooks, user-local preferences, watcher/session refresh behavior, and out-of-scope product runtime changes

**Checkpoint**: User Story 4 is fully functional and testable independently.

---

## Phase 7: Integration & Polish

- [X] T042 [P] [skillist: []] Run `./fake.sh build -t GeneratedGuidanceCheck` and record focused report paths plus any non-authoritative aggregate notes
- [X] T043 [P] [skillist: fs-skia-template-update] Run `./fake.sh build -t TemplateCheck` and record source/package template validation evidence for every supported profile
- [X] T044 [P] [skillist: []] Run `./fake.sh build -t TemplateDrift` and confirm drift fixtures, deferrals, and repair diagnostics are governed
- [X] T045 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` and confirm no cycles, dangling refs, mirror mismatches, invalid skill ids, or unexpected computed status changes
- [X] T050 [skillist: speckit-evidence-audit] Add or update evidence audit pattern coverage proving `CLAUDE.md`, `.claude/**`, `AGENTS.md`, and `.agents/**` are all recognized where synthetic or readiness disclosure rules apply
- [X] T046 [skillist: speckit-evidence-graph, speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` and confirm PASS or document every accepted synthetic override
- [X] T051 [skillist: speckit-evidence-graph, speckit-evidence-audit] Confirm `Ci` aggregates the updated `Verify`, `TemplateCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` expectations, or update the build aggregation and record the evidence
- [X] T047 [skillist: []] Run `./fake.sh build -t Verify` for broad Tier 1 sign-off, or record environment-blocked aggregate results as non-authoritative
- [X] T048 [skillist: []] Finalize readiness index and reviewer summary linking repository readiness, generated project readiness, sync validation, Claude research, graph, audit, and broad verification evidence

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. For `[SEH]`
rows, include the approval label, design-phase source, synthetic input class,
expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |

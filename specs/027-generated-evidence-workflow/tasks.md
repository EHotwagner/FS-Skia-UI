# Tasks: Generated Evidence Workflow Authority

**Feature branch**: `027-generated-evidence-workflow`
**Spec**: `specs/027-generated-evidence-workflow/spec.md`
**Plan**: `specs/027-generated-evidence-workflow/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. It still remains `[S]` when
completed with synthetic-only malformed-input or explicit error-path evidence.

## Phase 1: Setup

- [X] T001 [P] [skillist: speckit-tasks, speckit-implement] Create `specs/027-generated-evidence-workflow/readiness/` and placeholder files for generated-validation-authority, skill-loading-evidence-workflow, audit-diagnostics, readiness-contract-discovery, framework-guidance, evidence-vocabulary, evidence-graph, and evidence-audit
- [X] T002 [P] [skillist: []] Record feature governance notes covering Tier 1 scope, no planned `.fsi` public API change, MVU non-applicability for normal generated runtime, synthetic evidence restrictions, and required readiness paths
- [X] T003 [skillist: speckit-evidence-graph] Capture baseline EvidenceGraph output and current task metadata status to `readiness/evidence-graph.md`
- [X] T004 [skillist: speckit-evidence-graph, speckit-evidence-audit] Capture baseline EvidenceAudit output, readiness-file discovery gaps, and current blocking diagnostics to `readiness/evidence-audit.md`

---

## Phase 2: Foundation

- [X] T005 [skillist: speckit-evidence-graph, speckit-evidence-audit] Add failing-first governance tests for generated `EvidenceGraph` and `EvidenceAudit` targets that reject placeholder-only completion logs and preserve normal generated launch separation
- [S] T006 [P] [SEH] synthetic-error-handling-approved [skillist: speckit-evidence-graph, speckit-evidence-audit] Add malformed generated graph and audit fixtures with cycles, dangling references, missing readiness files, and skipped authority states for rejection tests
- [X] T007 [P] [skillist: speckit-tasks, speckit-implement] Add failing-first tests for skill-loading row generation and validation, including one row per task/skill pairing, no duplicate masking, and timestamp ordering
- [S] T008 [P] [SEH] synthetic-error-handling-approved [skillist: speckit-tasks, speckit-implement] Add malformed skill-loading evidence fixtures for collapsed task ranges, multi-skill prose rows, duplicate rows, late rows, and equal timestamps
- [S] T009 [P] [SEH] synthetic-error-handling-approved [skillist: speckit-evidence-graph, speckit-evidence-audit] Add audit readiness diagnostic fixtures for missing readiness files and incomplete readiness files with omitted required terms
- [X] T010 [P] [skillist: fs-skia-layout-evidence] Add failing-first generated guidance tests for app message qualification, vector-to-scene-point conversion, semantic scene evidence, screenshot vocabulary, and pixel-readback fallback claims
- [X] T011 [skillist: speckit-tasks, speckit-implement, speckit-evidence-graph] Update task-generation guidance and templates so audit-enforced readiness files and visible `skillist` metadata are discoverable before implementation starts
- [X] T012 [skillist: []] Define focused validation scope as medium governance risk: run story-specific governance/template checks after each story, broaden to `Verify` when graph/audit commands, template output, or root target aggregation changes, and record non-authoritative aggregate failures separately from authoritative verdicts

**Checkpoint**: Foundation ready - tests and malformed-input contracts exist before implementation.

---

## Phase 3: User Story 1 - Trust Generated Evidence Commands (Priority: P1)

### Tests First

- [X] T013 [P] [US1] [skillist: speckit-evidence-graph] Extend generated product and governance tests proving generated `EvidenceGraph` delegates to authoritative graph validation and fails before any pass report on invalid generated evidence packages
- [X] T014 [P] [US1] [skillist: speckit-evidence-graph, speckit-evidence-audit] Extend generated product and governance tests proving generated `EvidenceAudit` depends on a valid graph result, runs authoritative audit validation, and reports failed validation areas
- [X] T015 [P] [US1] [skillist: []] Add tests proving normal generated interactive launch remains persistent and does not run evidence commands, close windows, or write evidence artifacts

### Implementation

- [X] T016 [US1] [skillist: speckit-evidence-graph] Update `template/base/build.fsx` generated `EvidenceGraph` target to invoke or delegate to authoritative graph validation for the selected generated feature/readiness package
- [X] T017 [US1] [skillist: speckit-evidence-graph, speckit-evidence-audit] Update `template/base/build.fsx` generated `EvidenceAudit` target to require a valid graph result and invoke authoritative audit validation
- [X] T018 [US1] [skillist: []] Update `template/base/src/Product/EvidenceCommands.fs` report records and wording with command, target, generated app identity, authority, status, exit code, validation area, report path, and diagnostics
- [X] T019 [US1] [skillist: speckit-evidence-graph, speckit-evidence-audit] Update root `build.fsx` target dependencies only where `GeneratedGuidanceCheck`, `TemplateCheck`, `EvidenceGraph`, `EvidenceAudit`, `Verify`, or `Ci` aggregation must reflect generated validation authority
- [X] T020 [US1] [skillist: speckit-evidence-graph, speckit-evidence-audit] Capture real command evidence for generated graph/audit pass and rejection behavior in `readiness/generated-validation-authority.md`

**Checkpoint**: Generated graph and audit commands are authoritative and independently testable.

---

## Phase 4: User Story 2 - Produce Required Skill-Loading Evidence (Priority: P1)

### Tests First

- [X] T021 [P] [US2] [skillist: speckit-tasks, speckit-implement, speckit-evidence-graph] Add tests for deriving required skill-loading evidence rows from every structured `skillist` pairing in `tasks.deps.yml`
- [S] T022 [P] [US2] [SEH] synthetic-error-handling-approved [skillist: speckit-tasks, speckit-implement] Add rejection tests for malformed skill-loading rows that collapse task ranges, omit exact skill ids, use unreadable skill paths, or start work before loading

### Implementation

- [X] T023 [US2] [skillist: speckit-tasks, speckit-implement, speckit-evidence-graph] Extend `.specify/extensions/evidence/scripts/python/compute-task-graph.py` or adjacent helpers to generate and validate one skill-loading evidence row per task/skill pairing
- [X] T024 [US2] [skillist: speckit-tasks, speckit-implement] Add generated helper output or documentation that implementers can use before each task to record resolved skill path, loaded timestamp, work-start timestamp, and source
- [X] T025 [US2] [skillist: speckit-tasks, speckit-implement, speckit-evidence-graph] Persist skill-loading coverage diagnostics in graph/audit artifacts, including missing, late, duplicate, collapsed, and ambiguous-path rows
- [X] T026 [US2] [skillist: speckit-tasks, speckit-implement, speckit-evidence-graph] Capture real row-generation coverage and malformed-row rejection evidence in `readiness/skill-loading-evidence-workflow.md`

**Checkpoint**: Skill-loading evidence can be generated and validated per required task/skill row.

---

## Phase 5: User Story 3 - Diagnose Audit Readiness Failures (Priority: P2)

### Tests First

- [S] T027 [P] [US3] [SEH] synthetic-error-handling-approved [skillist: speckit-evidence-graph, speckit-evidence-audit] Add audit diagnostic tests where required readiness files are missing and where existing readiness files omit required terms or sections

### Implementation

- [X] T028 [US3] [skillist: speckit-evidence-graph, speckit-evidence-audit] Extend `.specify/extensions/evidence/scripts/bash/run-audit.sh` diagnostics to name each missing readiness file and missing required term or section in console output and persisted artifacts
- [X] T029 [US3] [skillist: speckit-evidence-graph, speckit-evidence-audit] Update generated `EvidenceAudit` command output so graph failure, readiness contract failure, synthetic evidence failure, diff-scan failure, and unsupported host classification failures are distinct
- [X] T030 [US3] [skillist: speckit-evidence-graph, speckit-evidence-audit] Capture missing-file, missing-term, and passing diagnostic evidence in `readiness/audit-diagnostics.md`

**Checkpoint**: Audit failures identify exact readiness files and terms.

---

## Phase 6: User Story 4 - Follow Generated Framework Guidance Safely (Priority: P2)

### Tests First

- [X] T031 [P] [US4] [skillist: fs-skia-layout-evidence] Add generated guidance tests that require `CloseRequested` qualification examples, app vector to scene point conversion examples, semantic scene fact reporting, and strict screenshot/fallback vocabulary

### Implementation

- [X] T032 [US4] [skillist: fs-skia-layout-evidence] Update `template/base/docs/product.md` with generated app guidance for qualifying app-owned messages, converting domain vectors to scene points, and reporting semantic scene facts explicitly
- [X] T033 [US4] [skillist: fs-skia-layout-evidence] Update template fragments under `template/fragments/` with evidence-safe screenshot, pixel-readback fallback, deterministic scene evidence, unsupported host, and `proves-screenshot=false` wording
- [X] T034 [US4] [skillist: fs-skia-layout-evidence] Update `GeneratedGuidanceCheck` and generated product tests so guidance wording is enforced before generated app authors implement evidence
- [X] T035 [US4] [skillist: fs-skia-layout-evidence] Capture generated framework guidance and evidence vocabulary proof in `readiness/framework-guidance.md` and `readiness/evidence-vocabulary.md`

**Checkpoint**: Generated authors see safe FS.Skia.UI guidance before evidence work.

---

## Phase 7: Integration & Polish

- [X] T036 [skillist: []] Run focused validation for medium governance risk: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj`, `./fake.sh build -t GeneratedGuidanceCheck`, and `./fake.sh build -t TemplateCheck`; record command output paths and any non-authoritative aggregate failures
- [X] T037 [skillist: speckit-evidence-graph, speckit-evidence-audit] Capture real readiness contract discovery proof in `readiness/readiness-contract-discovery.md`, including the generated author-facing path list and the audit-enforced readiness contract source
- [X] T038 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` and capture authoritative graph output in `readiness/evidence-graph.md`
- [X] T039 [skillist: speckit-evidence-graph, speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` and capture final audit output in `readiness/evidence-audit.md`, documenting any accepted synthetic error-handling rows
- [X] T040 [skillist: []] Run broad validation `./fake.sh build -t Verify` because this feature changes generated command behavior, evidence scripts, template output, and root target aggregation; record non-authoritative aggregate results separately from graph/audit verdicts
- [X] T041 [skillist: []] Review package/public-surface impact and either confirm no `.fsi` baseline refresh is required or run the required surface checks if implementation added a public helper

---

## Capability Skill Evaluation Notes

- `fs-skia-layout-evidence`: high confidence for generated game guidance, semantic scene evidence, screenshot vocabulary, host fallback wording, and public scene/update naming guidance. Declared only on those guidance tasks.
- `speckit-evidence-graph`: high confidence for graph validation, `tasks.deps.yml`, visible `skillist` metadata, and `EvidenceGraph` target work. Declared where task text or implementation directly touches those areas.
- `speckit-evidence-audit`: high confidence for audit diagnostics, readiness-blocking checks, synthetic propagation, and `EvidenceAudit` target work. Declared with `speckit-evidence-graph` first where both apply.
- `speckit-tasks` and `speckit-implement`: high confidence for task-generation/skill-loading evidence workflow because the validator treats pre-task skill-loading bookkeeping as implementation-skill metadata. Declared together with prerequisite order preserved.
- Valid-empty tasks: setup scaffolding, report wording, root target aggregation, launch separation, focused validation, broad validation, and surface review have no materially helpful local capability skill beyond ordinary repo implementation.

## Approved Synthetic Error-Handling Classifications

| Task | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|-------|---------------|-----------------------|-------------------------|-------------------|
| T006 | synthetic-error-handling-approved | `plan.md` Synthetic Evidence and `generated-evidence-command-contract.md` Failure Conditions | Malformed graph/audit package with cycles, dangling refs, missing files, skipped authority | Generated command exits non-zero, names failed validation area, and writes no pass claim | Approved during task generation |
| T008 | synthetic-error-handling-approved | `skill-loading-evidence-contract.md` Validation Rules | Collapsed task ranges, prose batch rows, duplicate/late/equal timestamp rows | Skill-loading validation reports exact invalid row class and does not satisfy missing required rows | Approved during task generation |
| T009 | synthetic-error-handling-approved | `audit-diagnostics-contract.md` Required Behavior | Missing readiness files and incomplete readiness content | Audit reports exact path and missing terms/sections as blocking diagnostics | Approved during task generation |
| T022 | synthetic-error-handling-approved | `skill-loading-evidence-contract.md` Verification | Malformed skill-loading rows and unreadable/ambiguous skill paths | Graph/audit rejects malformed evidence and identifies task id plus skill id | Approved during task generation |
| T027 | synthetic-error-handling-approved | `audit-diagnostics-contract.md` Console Output Requirements | Missing-file and missing-term readiness fixtures | Audit output distinguishes missing from incomplete and names every required repair target | Approved during task generation |

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T006 | Design-approved malformed generated graph/audit fixtures for rejection tests; fixture files carry `SYNTHETIC FIXTURE` banners. | `readiness/logs/t006/cycle-graph.txt`, `readiness/logs/t006/dangling-graph.txt`, `readiness/logs/t006/missing-readiness-audit.txt` | n/a | synthetic-error-handling-approved | `plan.md` Synthetic Evidence and `generated-evidence-command-contract.md` Failure Conditions | Malformed graph/audit package with cycles, dangling refs, missing files, skipped authority | Generated command exits non-zero, names failed validation area, and writes no pass claim | accepted-seh |
| T008 | Design-approved malformed skill-loading evidence fixtures; fixture files carry `SYNTHETIC FIXTURE` banners. | `readiness/logs/t008/*.txt` | n/a | synthetic-error-handling-approved | `skill-loading-evidence-contract.md` Validation Rules | Collapsed task ranges, prose batch rows, duplicate/late/equal timestamp rows | Skill-loading validation reports exact invalid row class and does not satisfy missing required rows | accepted-seh |
| T009 | Design-approved audit diagnostic fixtures for missing and incomplete readiness files; fixture files carry `SYNTHETIC FIXTURE` banners. | `readiness/logs/t009/missing-files-audit.txt`, `readiness/logs/t009/incomplete-terms-audit.txt` | n/a | synthetic-error-handling-approved | `audit-diagnostics-contract.md` Required Behavior | Missing readiness files and incomplete readiness content | Audit reports exact path and missing terms/sections as blocking diagnostics | accepted-seh |
| T022 | Design-approved malformed skill-loading row rejection tests; fixture files carry `SYNTHETIC FIXTURE` banners and test name includes `Synthetic`. | `readiness/logs/t022/*.txt` | n/a | synthetic-error-handling-approved | `skill-loading-evidence-contract.md` Verification | Malformed skill-loading rows and unreadable/ambiguous skill paths | Graph/audit rejects malformed evidence and identifies task id plus skill id | accepted-seh |
| T027 | Design-approved missing/incomplete readiness diagnostics fixtures; fixture files carry `SYNTHETIC FIXTURE` banners. | `readiness/logs/t027/missing-files-audit.txt`, `readiness/logs/t027/incomplete-terms-audit.txt` | n/a | synthetic-error-handling-approved | `audit-diagnostics-contract.md` Console Output Requirements | Missing-file and missing-term readiness fixtures | Audit output distinguishes missing from incomplete and names every required repair target | accepted-seh |

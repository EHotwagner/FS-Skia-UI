# Tasks: Archive Readiness And API Docs

**Feature branch**: `036-archive-readiness-api-docs`
**Spec**: `specs/036-archive-readiness-api-docs/spec.md`
**Plan**: `specs/036-archive-readiness-api-docs/plan.md`

## Status Legend

- `[ ]` pending
- `[X]` done with real evidence
- `[S]` done with synthetic evidence only
- `[F]` failed
- `[-]` skipped with written rationale

`[S*]` is computed by the evidence audit and must not be written manually.
`[SEH]` marks design-approved synthetic error-handling work that remains `[S]`
when completed with malformed-input or explicit error-path synthetic evidence.

## Authoring Notes

Keep `tasks.deps.yml` in object shape with one key per task id, indented `deps`
and `skillist` fields, exact dependency ids, and a visible `[skillist: ...]`
mirror on every task line. Avoid title trigger phrases that imply unrelated
capabilities; record viewer-window examples only for work that owns viewer
window evidence. This feature is documentation and governance work, so runtime
rendering, state workflow, and visual demo capability skills are intentionally
not assigned.

## Phase 1: Setup

- [X] T001 [P] [skillist: []] Create `specs/036-archive-readiness-api-docs/readiness/` placeholders for archive inventory, current evidence map, stale scan, API generator evaluation, fsdocs spike, generated guidance check, and verification notes
- [X] T002 [P] [skillist: []] Complete readiness notes for governance risk levels, aggregate hang diagnostics, runtime limitations, command ordering, and non-authoritative aggregate result policy
- [X] T003 [P] [skillist: speckit-tasks] Record post-generation skill evaluation notes covering matched signals, confidence, ambiguity, and valid-empty dispositions for every task

## Phase 2: Foundation

- [X] T004 [P] [skillist: []] Add failing archive inventory contract tests in `tests/Governance.Tests/` for required sections, row fields, archival markers, preservation status, and current-gate exclusion
- [S] T005 [P] [SEH] synthetic-error-handling-approved [skillist: []] Add malformed stale-reference scanner fixture tests for missing scan-area, unresolved path classification, and actionable diagnostic rejection
- [X] T006 [P] [skillist: []] Add failing stale-reference scan tests for active-surface blocking and historical-surface informational classification
- [X] T007 [P] [skillist: []] Add failing generated guidance tests for archived readiness wording, source-shaped `.fsi` authority, and forbidden reflection or repository-source authoring advice
- [X] T008 [P] [skillist: []] Add failing package API reference decision tests for required packages, comparison dimensions, fsdocs blocker fields, and current generator authority
- [X] T009 [P] [skillist: []] Identify or extend repository scanner/generator entry points for archive inventory, stale reference scan, and guidance validation without adding committed fsdocs dependencies

## Phase 3: User Story 1 - Find Current Evidence Without Historical Noise (P1)

**Independent Test**: A reviewer can classify active, stable, and historical readiness files as current, archived, roadmap/deferred, retained, or removable using repository guidance alone, and active stale references are blocking while historical references are informational.

- [X] T010 [P] [US1] [skillist: []] Implement archive inventory classification over real repository paths, preserving feature id, original path, archival marker, rationale, owner, preservation status, and replacement path
- [X] T011 [P] [US1] [skillist: []] Implement current evidence map generation for authoritative gates, required readiness paths, supporting paths, verification commands, and archived path policy
- [X] T012 [US1] [skillist: []] Write `specs/036-archive-readiness-api-docs/readiness/archive-inventory.md` from real repository paths and classification policy
- [X] T013 [US1] [skillist: []] Write `specs/036-archive-readiness-api-docs/readiness/current-evidence-map.md` with current package, template, generated-product, and audit gate paths
- [X] T014 [US1] [skillist: []] Update reviewer-facing docs that explain where current evidence lives and how archived readiness may be cited as audit context only
- [X] T015 [US1] [skillist: []] Verify US1 with focused governance tests and record the command, artifact path, failure class, and next action in readiness notes

## Phase 4: User Story 2 - Preserve Auditability While Archiving (P1)

**Independent Test**: Historical evidence remains discoverable by feature id and purpose, while current validation commands and docs no longer present archived material as pass/fail evidence.

- [X] T016 [P] [US2] [skillist: []] Implement stale-reference scan over active docs, templates, generated guidance, build reports, and the active feature with historical specs reported informationally
- [X] T017 [P] [US2] [skillist: []] Update or add stale-reference scanner report output with source path, referenced path, scan area, severity, reason, replacement path, line, and next action
- [X] T018 [US2] [skillist: []] Write `specs/036-archive-readiness-api-docs/readiness/stale-reference-scan.md` and optional JSON findings from real active-surface inspection
- [X] T019 [US2] [skillist: fs-skia-layout-readability] Update `docs/generated-apps.md`, `docs/template-profile.md`, `template/base/README.md`, and `template/base/docs/product.md` with archive/current-evidence guidance
- [X] T020 [US2] [skillist: []] Write `specs/036-archive-readiness-api-docs/readiness/generated-guidance-check.md` with generated guidance validation results and replacement instructions for stale references
- [X] T021 [US2] [skillist: []] Verify US2 with focused governance tests and `./fake.sh build -t GeneratedGuidanceCheck` run sequentially, recording logs under readiness

## Phase 5: User Story 3 - Decide Whether API Reference Generation Should Use FSharp.Formatting (P2)

**Independent Test**: A reviewer can verify from the decision record and samples that the current source-shaped `.fsi` generator remains authoritative for agents, and fsdocs is secondary/hybrid only if it preserves the required authoring guarantees.

- [X] T022 [P] [US3] [skillist: []] Generate or refresh current source-shaped package API reference samples for `FS.Skia.UI.Scene`, `FS.Skia.UI.Controls`, and one host or adapter package
- [X] T023 [P] [US3] [skillist: []] Run the isolated fsdocs/FSharp.Formatting spike or record a blocker with command, log path, reason, and next action
- [X] T024 [US3] [skillist: []] Write `specs/036-archive-readiness-api-docs/readiness/fsharp-formatting-spike.md` with sample output paths or documented blocker details
- [X] T025 [US3] [skillist: []] Write `specs/036-archive-readiness-api-docs/readiness/api-reference-generator-evaluation.md` comparing all required dimensions and decision values
- [X] T026 [US3] [skillist: []] Update package reference tests or reports so clean consumer discovery guarantees remain source-shaped and do not depend on reflection or repository source inspection
- [X] T027 [US3] [skillist: []] Verify US3 with focused package tests and record whether optional package/template gates remain unnecessary

## Phase 6: Polish And Verification

- [X] T028 [P] [skillist: []] Run `./fake.sh build -t Dev` sequentially and record focused validation status in readiness logs
- [X] T029 [skillist: []] Run `./fake.sh build -t GeneratedGuidanceCheck` sequentially after Dev and record authoritative guidance validation
- [X] T030 [skillist: []] Run `./fake.sh build -t TemplateDrift` sequentially and record template guidance drift status
- [X] T031 [skillist: speckit-evidence-graph] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/036-archive-readiness-api-docs --graph-only` or `./fake.sh build -t EvidenceGraph` sequentially and write `readiness/evidence-graph.md`
- [X] T032 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` sequentially after graph validation and write `readiness/evidence-audit.md`
- [X] T033 [skillist: []] Review all readiness reports for small, medium, and broad risk-level classification, focused validation evidence, broad-validation triggers, and non-authoritative aggregate result notes

## Synthetic-Evidence Inventory

| Task | Synthetic evidence | Label | Design source | Reason | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------------------|-------|---------------|-----------|-----------------------|-------------------------|-------------------|
| T005 | Malformed stale-reference scanner fixtures | synthetic-error-handling-approved | `specs/036-archive-readiness-api-docs/plan.md` synthetic evidence policy and `contracts/stale-reference-scan-contract.md` failure conditions | Real positive evidence must inspect repository paths; malformed scan metadata is an error path that needs deterministic rejection coverage | malformed scanner input | Scanner rejects missing scan-area or unresolved classification with source path, reason, and next action | accepted-seh |

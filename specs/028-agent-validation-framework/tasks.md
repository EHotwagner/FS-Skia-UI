# Tasks: Agent Validation Framework

**Feature branch**: `028-agent-validation-framework`
**Spec**: `specs/028-agent-validation-framework/spec.md`
**Plan**: `specs/028-agent-validation-framework/plan.md`

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
The classification must be assigned during design, planning, clarification, or
task generation. implementation-time relabeling is forbidden; newly discovered
needs go back to task/design review.

## Vertical-slice rule (US phases)

A task tagged `[US*]` may only be marked `[X]` when the change is reachable
from a user-facing entry point and that path was actually exercised - an FSI
session against the packed library, a smoke run of the application, a manual
walk-through with transcript, or a screenshot captured under `readiness/`.
Domain, model, or core-layer changes alone do **not** satisfy `[X]` for a
`[US*]` task, even if their unit tests pass green. If the user-reachable
surface is missing, stubbed, or not yet wired, mark `[ ]` (work continues) or
`[S]` with a disclosed reason in the Synthetic-Evidence Inventory - never
`[X]`.

For stateful or I/O-bearing stories, `[X]` also requires Elmish/MVU evidence:
the public `Model` / `Msg` / `Effect` or `Cmd<Msg>` contract was exercised,
pure `update` transitions were tested, emitted effects were asserted, and the
effect interpreter was run against real dependencies where safe.

## Task Annotations

- **[P]** - parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, ... - user-story scope
- **[T1]** / **[T2]** - Tier 1 (contracted) vs Tier 2 (internal) change
- **[SEH]** - design-approved synthetic error-handling task paired with
  `synthetic-error-handling-approved`

Every task has a matching entry in `tasks.deps.yml`, including structured
`skillist` metadata mirrored on the task line. The feature is Tier 1 overall,
so no tier annotation is needed unless a later task is explicitly rescoped.

## Canonical Verification Targets

- `./fake.sh build -t Dev` for fast local verification.
- `./fake.sh build -t AgentReady` for focused agent-ready validation and consolidated verdict output.
- `./fake.sh build -t Verify` for the full governed workflow.
- `./fake.sh build -t PackLocal` for local package output.
- `./fake.sh build -t RefreshSurfaceBaselines` for intentional current surface baseline refreshes.
- `./fake.sh build -t PackageSurfaceCheck` for package surface review.
- `./fake.sh build -t FsiTranscripts` for public FSI evidence.
- `./fake.sh build -t TemplateCheck` for source/package default/minimal generated project validation.
- `./fake.sh build -t GeneratedGuidanceCheck` for generated prompt, task-skill, and implementation guidance governance.
- `./fake.sh build -t TemplateDrift` for template-owned drift and deferral validation.
- `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit` for graph and synthetic-evidence gates.

## Skill Evaluation Notes

Capability skill review:

- `fs-skia-template-update`: high confidence for generated template/package-pin validation tasks that modify or validate `dotnet new fs-skia-ui` output.
- `speckit-evidence-graph`: high confidence only for tasks that directly validate graph/DAG readiness or run `EvidenceGraph`.
- `speckit-evidence-audit`: high confidence only for tasks that directly run the final audit or record audit diagnostics.
- `fs-skia-layout-evidence`: false positive for this feature; host and generated evidence wording is in scope, but generated game HUD/readability layout is not.
- Spec Kit meta skills (`speckit-plan`, `speckit-specify`, `speckit-clarify`, `speckit-tasks`, `speckit-implement`, git helpers, checklist, task-to-issues): valid empty for implementation tasks unless the task explicitly invokes that command family.

Risk-level evidence:

- Small risk: docs-only or metadata-only routing edits; focused validation is the selected rule tests plus `EvidenceGraph`.
- Medium risk: controls, generated template, validation contract, or generated evidence workflow edits; focused validation is the selected rule gates plus `EvidenceGraph` and `EvidenceAudit`.
- Broad risk: native target migration, command aggregation, package surface changes, or multi-rule ambiguity; run `Verify` and record aggregate results as non-authoritative unless the verdict fields identify completed authoritative gates.

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm branch, prerequisite spec artifacts, and existing worktree state for `028-agent-validation-framework` — recorded in `readiness/setup-status.md`
- [X] T002 [P] [skillist: []] Create or refresh `specs/028-agent-validation-framework/readiness/` placeholders for all required real evidence files — required readiness files and capability evidence table initialized
- [X] T003 [P] [skillist: []] Record governance risk levels, selected focused validation, broad-validation triggers, and non-authoritative aggregate result handling in readiness notes — recorded in `readiness/governance-validation-notes.md`
- [X] T004 [skillist: []] Record Tier 1 scope, public API impact, command-surface impact, generated template impact, and MVU/effect applicability for this feature — recorded in `readiness/governance-scope.md`

**Checkpoint**: Setup complete - readiness paths and governance obligations are discoverable.

---

## Phase 2: Foundation

- [X] T005 [skillist: []] Add failing governance tests for `validation.contract.yml` shape, required rules, tier definitions, and unknown gate rejection — failing-first evidence in `readiness/logs/t005-validation-contract-tests.txt`
- [X] T006 [P] [skillist: []] Add failing governance tests for active-feature changed-path selection, git merge-base fallback, unavailable context degradation, and multi-rule gate union — failing-first evidence in `readiness/logs/t006-t008-agent-validation-tests.txt`
- [X] T007 [P] [skillist: []] Add failing verdict contract tests for required JSON fields, Markdown wording authority, missing gates, next command, and failure classes — failing-first evidence in `readiness/logs/t006-t008-agent-validation-tests.txt`
- [X] T008 [P] [skillist: []] Add failing target metadata drift tests for missing runnable target, missing metadata, missing outputs, missing failure owner, and dependency divergence — failing-first evidence in `readiness/logs/t006-t008-agent-validation-tests.txt`
- [X] T009 [P] [skillist: fs-skia-ui-widgets] Add failing controls contract tests and FSI transcript expectations for typed standard controls, custom escape hatches, and schema diagnostics — failing-first evidence in `readiness/logs/t009-controls-contract-tests.txt`
- [X] T010 [P] [skillist: fs-skia-template-update] Add failing generated template tests proving normal launch is evidence-free and explicit evidence commands are separately discoverable — failing-first evidence in `readiness/logs/t010-generated-template-tests.txt`
- [S] T011 [P] [SEH] [skillist: []] Add design-approved malformed validation contract and verdict fixture tests for rejection paths synthetic-error-handling-approved — synthetic failing-first evidence in `readiness/logs/t011-synthetic-malformed-fixtures.txt`
- [X] T012 [skillist: []] Draft `.fsi` contracts for validation selection `Model`, `Msg`, `Effect`, `init`, `update`, and interpreter boundary — `src/Lib/AgentValidation.fsi` added and `dotnet build src/Lib/Lib.fsproj --no-restore` passed
- [X] T013 [skillist: []] Draft `.fsi` contracts for target metadata, agent verdict values, environment failure classification, and report serialization — `AgentValidation.fsi` extended and `dotnet build src/Lib/Lib.fsproj --no-restore` passed
- [X] T014 [skillist: fs-skia-ui-widgets] Draft `.fsi` contracts for typed controls front doors, shared control schema, catalog access, diagnostics, and visibly custom extension APIs — controls `.fsi` contracts updated and `dotnet build src/Controls/Controls.fsproj --no-restore` passed
- [X] T015 [skillist: []] Capture FSI transcript evidence for the draft validation and controls signatures in `readiness/fsi-session.txt` — `dotnet fsi readiness/fsi-session.fsx` passed and wrote `readiness/fsi-session.txt`
- [X] T016 [skillist: []] Record initial package surface baseline expectations for changed public controls and validation modules — recorded in `readiness/package-surface-expectations.md`

**Checkpoint**: Foundation ready - failing tests and public contracts define the implementation boundary.

---

## Phase 3: User Story 1 - Route Agent Validation Deliberately

### Tests First

- [X] T017 [P] [US1] [skillist: []] Add representative routing scenarios for controls, templates, evidence governance, generated guidance, docs-only, package surface, and build-target contract paths — failing-first evidence in `readiness/logs/t017-t019-us1-routing-tests.txt`
- [X] T018 [P] [US1] [skillist: []] Add pure transition tests for validation selection `init` and `update`, including emitted effects for feature metadata, git diff, contract load, and degraded fallback — public MVU transition tests passed in `readiness/logs/t017-t019-us1-routing-tests.txt`
- [S] T019 [P] [US1] [SEH] [skillist: []] Add malformed contract, unknown target, duplicate rule, and invalid path fixture tests synthetic-error-handling-approved — synthetic malformed fixture evidence in `readiness/logs/t017-t019-us1-routing-tests.txt`

### Implementation

- [X] T020 [US1] [skillist: []] Add `validation.contract.yml` with required tiers, defaults, routing rules, expected artifacts, timeout classes, and failure owners — routing contract evidence in `readiness/logs/t020-validation-contract-routing.txt`
- [X] T021 [US1] [skillist: []] Implement validation contract parser and schema diagnostics with actionable unknown-gate and malformed-field errors — parser diagnostics evidence in `readiness/logs/t021-validation-contract-parser.txt`
- [X] T022 [US1] [skillist: []] Implement changed-path source selection from active feature metadata with git merge-base diff fallback and explicit unavailable state — public MVU source-selection evidence in `readiness/logs/t021-validation-contract-parser.txt`
- [X] T023 [US1] [skillist: []] Implement pure rule selection that unions gates, preserves selected rule ids, resolves risk categories, and avoids duplicate execution claims — rule selection evidence in `readiness/logs/t023-rule-selection.txt`
- [X] T024 [US1] [skillist: []] Implement interpreter wiring for feature metadata reads, git diff execution, contract loading, and readiness report inputs — real filesystem, real git merge-base diff, real contract load, report-write tests in `readiness/logs/t024-validation-interpreter.txt`, and public FSI transcript in `readiness/logs/t024-validation-interpreter-fsi.txt`
- [X] T025 [US1] [skillist: []] Write `readiness/validation-contract.md` with real routing evidence and the selected focused gates for representative scenarios — public FSI and real interpreter evidence summarized in `readiness/validation-contract.md`

**Checkpoint**: US1 can independently select authoritative focused validation from real repository context.

---

## Phase 4: User Story 2 - Produce One Agent Verdict

### Tests First

- [X] T026 [P] [US2] [skillist: []] Add verdict tests for passed, failed, unsupported, degraded, stale-prerequisite, and missing-evidence outcomes — verdict outcome tests passed in `readiness/logs/t026-t028-us2-verdict-tests.txt`
- [X] T027 [P] [US2] [skillist: []] Add pure transition tests for verdict aggregation, completed and missing gate calculation, next command selection, and emitted report effects — aggregate calculation and serializer tests passed in `readiness/logs/t026-t028-us2-verdict-tests.txt`
- [S] T028 [P] [US2] [SEH] [skillist: []] Add forced error-result fixture tests for environment, unsupported-host, stale-prerequisite, and missing-evidence classification synthetic-error-handling-approved — synthetic forced-outcome classification tests passed in `readiness/logs/t026-t028-us2-verdict-tests.txt`

### Implementation

- [X] T029 [US2] [skillist: []] Implement `AgentVerdict` JSON and Markdown serializers with status, authority, changed-path source, gates, artifacts, diagnostics, and timestamp — serializer tests in `readiness/logs/t026-t028-us2-verdict-tests.txt` and public FSI transcript in `readiness/logs/t029-t030-agent-verdict-fsi.txt`
- [X] T030 [US2] [skillist: []] Implement gate result aggregation and failure ownership classification for product, template, governance, environment, unsupported-host, stale-prerequisite, missing-evidence, and unknown outcomes — aggregation tests in `readiness/logs/t026-t028-us2-verdict-tests.txt` and public FSI transcript in `readiness/logs/t029-t030-agent-verdict-fsi.txt`
- [X] T031 [US2] [skillist: speckit-evidence-audit] Add `AgentReady` build path that runs selected focused gates plus final readiness obligations and writes consolidated verdict artifacts — `./fake.sh build -t AgentReady` passed in `readiness/logs/t031-agent-ready.txt` and wrote degraded consolidated verdict artifacts in `readiness/agent-verdict.json`, `readiness/agent-verdict.md`, and `readiness/agent-ready-verdict.md`
- [X] T032 [US2] [skillist: []] Add degraded fallback handling that names `./fake.sh build -t Verify` whenever focused authority cannot be selected confidently — `AgentReady` degraded verdict and JSON fallback assertion passed in `readiness/logs/t032-degraded-fallback.txt`
- [X] T033 [US2] [skillist: []] Write `readiness/agent-ready-verdict.md` and `readiness/environment-failure-classification.md` from real command or governed diagnostic evidence — readiness files and verdict classification verified in `readiness/logs/t033-agent-ready-environment-classification.txt`

**Checkpoint**: US2 emits one auditable verdict for focused, failed, unsupported, and degraded runs.

---

## Phase 5: User Story 3 - Separate Normal Product Launch From Evidence Policy

### Tests First

- [X] T034 [P] [US3] [skillist: fs-skia-template-update] Add generated product tests proving normal launch remains persistent, interactive, and does not write readiness artifacts — failing-first governance evidence recorded in `readiness/logs/t034-generated-normal-launch-tests.txt`
- [X] T035 [P] [US3] [skillist: fs-skia-template-update] Add generated evidence command tests proving explicit workflows produce governed reports and preserve product-owned versus policy-owned facts — failing-first explicit workflow evidence recorded in `readiness/logs/t035-generated-evidence-command-tests.txt`
- [S] T036 [P] [US3] [SEH] [skillist: fs-skia-template-update] Add missing generated artifact and unsupported host fixture tests for generated evidence command classification synthetic-error-handling-approved — synthetic failing-first fixture evidence recorded in `readiness/logs/t036-synthetic-generated-evidence-fixtures.txt`

### Implementation

- [X] T037 [US3] [skillist: fs-skia-template-update] Refactor `template/base/src/Product/Program.fs` so default launch stays product-only and evidence-free — focused source verification recorded in `readiness/logs/t037-program-evidence-free.txt`
- [X] T038 [US3] [skillist: fs-skia-template-update] Implement generated `EvidenceCommands` orchestration for governed reports, authority wording, skipped gates, unsupported outcomes, and next commands — workflow metadata and SEH fixture contract verified in `readiness/logs/t038-evidence-command-workflows.txt`
- [X] T039 [US3] [skillist: fs-skia-template-update] Update generated product tests, template build targets, and package membership for explicit evidence workflow files — package membership and generated governance tests verified in `readiness/logs/t039-template-membership-tests.txt`
- [X] T040 [US3] [skillist: fs-skia-template-update] Update generated guidance and docs to prefer explicit evidence commands and avoid stronger claims than completed gates support — generated docs verification recorded in `readiness/logs/t040-generated-guidance-docs.txt`
- [X] T041 [US3] [skillist: fs-skia-template-update] Write `readiness/evidence-policy-separation.md` with normal-launch inspection and explicit evidence command proof — readiness note verified in `readiness/logs/t041-evidence-policy-separation.txt`

**Checkpoint**: US3 separates generated product launch from governed evidence policy without weakening evidence reporting.

---

## Phase 6: User Story 4 - Add Typed Control Guardrails

### Tests First

- [X] T042 [P] [US4] [skillist: fs-skia-ui-widgets] Add compile-time or semantic rejection tests for misspelled standard control kinds and standard event kinds across existing controls modules — controls semantic tests passed in `readiness/logs/t042-t045-us4-typed-controls-tests.txt`
- [X] T043 [P] [US4] [skillist: fs-skia-ui-widgets] Add chart and data grid typed data tests for series, points, columns, rows, visible range, selected rows, and focused cell values — controls semantic tests passed in `readiness/logs/t042-t045-us4-typed-controls-tests.txt`
- [X] T044 [P] [US4] [skillist: fs-skia-ui-widgets] Add schema-backed diagnostics tests for missing required attributes, unsupported attributes, unsupported events, and visibly custom usage — controls semantic tests passed in `readiness/logs/t042-t045-us4-typed-controls-tests.txt`
- [X] T045 [P] [US4] [skillist: fs-skia-ui-widgets] Add FSI transcript scenarios for typed front doors and deliberate custom extension APIs — transcript regenerated in `readiness/fsi-session.txt` and controls tests passed in `readiness/logs/t042-t045-us4-typed-controls-tests.txt`

### Implementation

- [X] T046 [US4] [skillist: fs-skia-ui-widgets] Implement typed standard control kind, event kind, attribute name, and value primitives in `src/Controls/Types.fsi` and `.fs` — controls semantic tests passed in `readiness/logs/t042-t045-us4-typed-controls-tests.txt`
- [X] T047 [US4] [skillist: fs-skia-ui-widgets] Implement typed standard creation and lowering compatibility in `Control.fsi` and `.fs` — controls semantic tests passed in `readiness/logs/t042-t045-us4-typed-controls-tests.txt`
- [X] T048 [US4] [skillist: fs-skia-ui-widgets] Implement typed event, attribute, and custom extension front doors in `Attributes.fsi` and `.fs` — controls semantic tests passed in `readiness/logs/t042-t045-us4-typed-controls-tests.txt`
- [X] T049 [US4] [skillist: fs-skia-ui-widgets] Implement typed chart data and grid data front doors in `Charts.fsi` and `DataGrid.fsi` with compatibility lowering — controls semantic tests passed in `readiness/logs/t042-t045-us4-typed-controls-tests.txt`
- [X] T050 [US4] [skillist: fs-skia-ui-widgets] Implement shared control schema, catalog access, and schema-owned diagnostics in `Catalog.fsi`, `Catalog.fs`, `Diagnostics.fsi`, and `Diagnostics.fs` — controls semantic tests passed in `readiness/logs/t042-t045-us4-typed-controls-tests.txt`
- [X] T051 [US4] [skillist: fs-skia-ui-widgets, fs-skia-template-update] Update generated template controls guidance and examples to use typed standard paths by default and visibly custom APIs only for deliberate extensions — source guidance verification recorded in `readiness/logs/t051-generated-controls-guidance.txt`
- [X] T052 [US4] [skillist: fs-skia-ui-widgets] Refresh package surface baselines, FSI transcript evidence, and docs for additive public controls contracts — `PackageSurfaceCheck` and `FsiTranscripts` passed in `readiness/logs/t052-package-surface-check.txt` and `readiness/logs/t052-fsi-transcripts.txt`
- [X] T053 [US4] [skillist: fs-skia-ui-widgets] Write `readiness/typed-controls-front-door.md` with real rejection, custom extension, diagnostic, and transcript evidence — readiness evidence recorded in `readiness/typed-controls-front-door.md`

**Checkpoint**: US4 exposes typed guardrails for every existing standard controls module while preserving custom extension paths.

---

## Phase 7: User Story 5 - Align Build Targets With Discoverable Metadata

### Tests First

- [X] T054 [P] [US5] [skillist: []] Add target metadata parity tests comparing native FAKE targets, metadata entries, docs, and validation contract references — agent validation contract tests passed in `readiness/logs/t054-t056-target-metadata-tests.txt`
- [X] T055 [P] [US5] [skillist: []] Add seeded drift tests for missing target, missing metadata, missing expected output, wrong dependency, and missing failure owner cases — agent validation contract tests passed in `readiness/logs/t054-t056-target-metadata-tests.txt`
- [X] T056 [P] [US5] [skillist: []] Add command compatibility tests for the full in-scope stable validation target name set without changing existing command names — `BuildWorkflowCheck` and agent validation contract tests passed in `readiness/logs/t054-t056-target-metadata-tests.txt`

### Implementation

- [X] T057 [US5] [skillist: []] Migrate in-scope validation targets in `build.fsx` to native FAKE target registration while preserving stable command names — native FAKE target discovery, command compatibility, and metadata drift evidence passed in `readiness/logs/t057-build-workflow-check.txt` and `readiness/logs/t057-target-metadata-drift.txt`
- [X] T058 [US5] [skillist: []] Add pure target metadata records with dependencies, prerequisites, outputs, stale assumptions, timeout class, cost, authority, failure owner, and command fields — metadata records verified by `./fake.sh build -t TargetMetadataDrift` in `readiness/logs/t058-t060-target-metadata.txt`
- [X] T059 [US5] [skillist: []] Expose target metadata through a build target or generated report that external tooling can consume without prose inference — `TargetMetadata` wrote `readiness/target-metadata.json` via `TargetMetadataDrift` in `readiness/logs/t058-t060-target-metadata.txt`
- [X] T060 [US5] [skillist: []] Implement drift validation across native targets, metadata, docs, and `validation.contract.yml` — `TargetMetadataDrift` passed across native FAKE target registry, metadata, docs, and validation contract references in `readiness/logs/t060-target-metadata-drift.txt`
- [X] T061 [US5] [skillist: []] Update build, evidence, generated app, testing, and controls docs for target metadata and command compatibility — docs and validation contract references verified by `TargetMetadataDrift` in `readiness/logs/t061-target-metadata-docs.txt`
- [X] T062 [US5] [skillist: []] Write `readiness/target-metadata.md` with real target discovery, drift validation, and compatibility evidence — readiness summary verified by `TargetMetadataDrift` in `readiness/logs/t062-target-metadata-readiness.txt`

**Checkpoint**: US5 provides discoverable target metadata aligned with runnable native targets and docs.

---

## Phase 8: Integration & Polish

- [X] T063 [P] [skillist: fs-skia-template-update] Run `TemplateCheck` and record generated template validation results relevant to this feature — `./fake.sh build -t TemplateCheck` passed in `readiness/logs/t063-template-check.txt`
- [X] T064 [P] [skillist: []] Run `GeneratedGuidanceCheck`, `PackageSurfaceCheck`, and `FsiTranscripts`; refresh intentional baselines only through governed commands — checks passed in `readiness/logs/t064-generated-guidance-check.txt`, `readiness/logs/t064-package-surface-check.txt`, and `readiness/logs/t064-fsi-transcripts.txt`
- [X] T065 [P] [skillist: []] Run `AgentReady` for this feature and confirm verdict artifacts match the contract and required readiness paths — `AgentReady` passed and required verdict artifacts were present in `readiness/logs/t065-agent-ready.txt`
- [X] T066 [skillist: speckit-evidence-graph] Run `EvidenceGraph` and write `readiness/evidence-graph.md` with graph status, task metadata validation, and any propagation notes — graph passed in `readiness/logs/t066-evidence-graph.txt`
- [X] T067 [skillist: speckit-evidence-audit] Run `EvidenceAudit` and write `readiness/evidence-audit.md` with PASS status or every accepted synthetic/error-path disclosure — audit passed with accepted `[SEH]` disclosure counts and no blocking diff hits in `readiness/logs/t067-evidence-audit-fixed.txt`
- [X] T068 [skillist: []] Run `Verify` for broad validation when triggered by risk classification or target migration scope, recording aggregate output as non-authoritative unless gate verdicts prove authority — `Verify` passed end-to-end in `readiness/logs/t068-verify-complete.txt`
- [X] T069 [skillist: []] Complete final readiness notes, docs cross-links, package membership review, and implementation handoff summary — final handoff recorded in `readiness/implementation-handoff.md` and re-audited in `readiness/logs/t069-evidence-audit.txt`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. For `[SEH]`
rows, include the approval label, design-phase source, synthetic input class,
expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T011 | Malformed validation contract and verdict rejection paths require intentionally invalid contract shapes and cannot come from real successful repository state. | `specs/028-agent-validation-framework/readiness/validation-contract.md` | | synthetic-error-handling-approved | `specs/028-agent-validation-framework/plan.md` Synthetic Evidence and contracts | malformed YAML/JSON fields, duplicate ids, unknown gates | parser rejects invalid input with actionable diagnostics and no success verdict | accepted-seh |
| T019 | Contract routing error paths require malformed contract fixtures, duplicate rules, and invalid paths to prove safe rejection. | `specs/028-agent-validation-framework/readiness/validation-contract.md` | | synthetic-error-handling-approved | `specs/028-agent-validation-framework/contracts/validation-contract.md` Drift Validation | malformed contract input, unknown target, duplicate rule id, invalid path pattern | routing rejects the fixture and reports governance-owned diagnostics | accepted-seh |
| T028 | Failure classification needs forced error-result fixtures for rare environment and prerequisite states without misclassifying real host behavior. | `specs/028-agent-validation-framework/readiness/environment-failure-classification.md` | | synthetic-error-handling-approved | `specs/028-agent-validation-framework/contracts/agent-verdict-contract.md` Required Failure Classes | forced gate result errors for environment, unsupported host, stale prerequisite, missing evidence | verdict records the expected failure class, owner, diagnostics, and next command when required | accepted-seh |
| T036 | Generated evidence command error paths need missing-artifact and unsupported-host fixtures that are explicit negative cases, not normal launch proof. | `specs/028-agent-validation-framework/readiness/evidence-policy-separation.md` | | synthetic-error-handling-approved | `specs/028-agent-validation-framework/contracts/generated-evidence-policy-contract.md` Validation | missing generated artifact and unsupported host fixtures | explicit evidence command reports unsupported or stale prerequisite without changing product launch | accepted-seh |

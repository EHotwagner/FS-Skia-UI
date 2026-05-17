# Tasks: Fix Refactor Process Reliability

**Feature branch**: `012-fix-refactor-process`
**Spec**: `specs/012-fix-refactor-process/spec.md`
**Plan**: `specs/012-fix-refactor-process/plan.md`

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

A task tagged `[US*]` may only be marked `[X]` when the change is reachable
from a maintainer-facing command, report, or validation entry point and that
path was actually exercised by a focused FAKE target, broad aggregate run,
governance test, scanner fixture, or readiness transcript under
`specs/012-fix-refactor-process/readiness/`.

For this process feature, Principle IV applies to the build workflow rather
than product UI state: `BuildModel`, `BuildMsg`, `BuildEffect`, pure `update`,
and interpreter-side filesystem/process effects must remain explicit. No
product `.fsi` or public Controls API change is planned; if implementation
discovers one is required, pause and update the spec/plan before changing it.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, … — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change

Every task has a matching entry in `tasks.deps.yml` even when its dependency
list is empty.

## Canonical Verification Targets

Use repository targets instead of duplicating command order:

- `./fake.sh build -t Dev`
- `./fake.sh build -t Verify`
- `./fake.sh build -t Ci`
- `./fake.sh build -t PackageSurfaceCheck`
- `./fake.sh build -t FsiTranscripts`
- `./fake.sh build -t ControlsCatalogCheck`
- `./fake.sh build -t ControlsInteractionCheck`
- `./fake.sh build -t ControlsRenderingCheck`
- `./fake.sh build -t DependencyReport`
- `./fake.sh build -t TemplateCheck`
- `./fake.sh build -t GeneratedProductCheck`
- `./fake.sh build -t GeneratedGuidanceCheck`
- `./fake.sh build -t TemplateDrift`
- `./fake.sh build -t EvidenceGraph`
- `./fake.sh build -t EvidenceAudit`

---

## Phase 1: Setup

- [X] T001 Create readiness scaffolding under `specs/012-fix-refactor-process/readiness/`, including `logs/` and placeholders for the nine required evidence files.
- [X] T002 [P] Confirm `docs/controls-boundary-refactor-process-report.md`, `spec.md`, `plan.md`, `data-model.md`, `quickstart.md`, and contracts describe the same follow-up scope.
- [X] T003 [P] Record the Tier 2 classification, no-product-API-change constraint, and no package ownership change constraint in the setup evidence notes.
- [X] T004 [P] Record synthetic-evidence policy for this feature: scanner fixtures may be synthetic, final readiness evidence is expected to be real.
- [X] T005 Consolidate setup notes into the readiness scaffolds and list any missing prerequisite artifacts before implementation starts.

**Checkpoint**: Setup complete — foundation work may begin.

---

## Phase 2: Foundation

- [X] T006 [P] Add failing command-contract tests in `tests/Governance.Tests/CommandContractTests.fs` or a new governance test module for process-health, bootstrap, verdict, and focused-gate effect contracts.
- [X] T007 [P] Add failing target-graph tests that every required focused gate is directly invocable and is not recoupled to `Verify` or `Ci` without a documented, tested prerequisite.
- [X] T008 [P] Add scanner fixture helpers for temporary XML project files, capability/template metadata, generated product roots, generated guidance, stale-reference files, and readiness reports.
- [X] T009 Extend the build workflow contract in `build.fsx` with process-health snapshot, threshold, bootstrap, verification-verdict, focused-gate, stale-boundary, and readiness path data carried through `BuildModel`, `BuildMsg`, and `BuildEffect`.
- [X] T010 Extend interpreter-side helpers for structured markdown/JSON report writes, command log paths, unsupported health signals, warning classification, and actionable diagnostics.
- [X] T011 [P] Record `.fsi` applicability: no product `.fsi` surface is expected, and the implementation contract is the build workflow's MVU-shaped model/message/effect/update/interpreter boundary.
- [X] T012 Update `docs/build.md`, `docs/evidence.md`, and `docs/testing.md` with the new process-health, verdict, focused-gate, scanner, and stale-boundary evidence classes.
- [X] T013 Verify foundation with pure `update` tests and emitted-effect assertions for representative `Dev`, `Verify`, `Ci`, focused gate, scanner, and audit targets.

**Checkpoint**: Foundation ready — P1 user stories may begin in parallel.

---

## Phase 3: P1 User Stories

### User Story 1 - Trust Broad Verification Results

#### Tests First

- [X] T014 [P] [US1] Add failing tests for process-health snapshots covering timestamp, target, platform, memory, process count, zombie count, thread/file descriptor headroom, dotnet startup, FAKE bootstrap, unsupported signals, threshold decisions, and elapsed time from broad target start to preflight summary within 30 seconds.
- [X] T015 [P] [US1] Add pure `update` tests proving `Verify` and `Ci` emit process-health and bootstrap effects before high-pressure aggregate effects, and emit verdict effects for success, product failure, environment failure, and degraded outcomes.
- [X] T016 [P] [US1] Add interpreter fixture tests for missing runner dependencies, malformed threshold overrides, CoreCLR/startup failure, process exhaustion, and repeated bootstrap warnings.

#### Implementation

- [X] T017 [US1] Implement repository-owned process-health thresholds and override parsing with rule id, default value, override value, override source, and human-readable reason diagnostics.
- [X] T018 [US1] Implement process-health collection for broad aggregates, including explicit unsupported-signal reporting on platforms where a signal cannot be measured and preflight timing fields that prove the summary was written within the SC-001 30-second bound.
- [X] T019 [US1] Implement bootstrap validation and warning classification so missing runner dependencies fail as environment failures and warning noise does not hide later target failures.
- [X] T020 [US1] Wire `Verify` and `Ci` preflight fail-fast before high-pressure work while preserving product-failure classification when product checks actually run and fail.
- [X] T021 [US1] Write broad verification verdict reports to `process-health.md`, `bootstrap-runner.md`, `verification-verdicts.md`, and logs with failing stage, diagnostics, product-check status, authoritative flag, recommended rerun environment, and preflight elapsed-time evidence.
- [X] T022 [US1] Capture real broad-run evidence for healthy and fail-fast paths where safe, or record the explicit environment-failure verdict and fresh-run requirement when the local runner is unhealthy.
- [X] T023 [US1] Update `quickstart.md`, `docs/build.md`, and `docs/evidence.md` with broad verdict categories, threshold override rules, and fresh shell/container/CI rerun guidance.
- [X] T024 [US1] Document the US1 independent validation path and map evidence to FR-001 through FR-005, FR-014, FR-017, FR-018, SC-001, SC-002, and SC-009.

### User Story 2 - Keep Focused Gates Actionable

#### Tests First

- [X] T025 [P] [US2] Add command-contract tests proving `PackageSurfaceCheck`, `FsiTranscripts`, `ControlsCatalogCheck`, `ControlsInteractionCheck`, `ControlsRenderingCheck`, `DependencyReport`, `TemplateCheck`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` can be invoked directly.
- [X] T026 [P] [US2] Add tests that focused gates fail when they depend on `Verify`, `Ci`, or undocumented broad work, and pass only when direct prerequisites are documented and tested.
- [X] T027 [P] [US2] Add tests for stale `--no-build` or `--no-restore` assumptions that require diagnostics naming the affected gate and remediation command.

#### Implementation

- [X] T028 [US2] Refactor `targetDependencies` and target contracts so focused gates have only explicit small prerequisites such as restore, build, template pack, capability check, or evidence graph.
- [X] T029 [US2] Add focused-gate summary output with target, direct prerequisites, duration/timestamp, log path, readiness path, verdict category, and failure rule or artifact.
- [X] T030 [US2] Add stale build/restore assumption checks and diagnostics for focused gates that rely on generated, restored, built, or packed artifacts.
- [X] T031 [US2] Update `docs/build.md`, `docs/testing.md`, and `docs/evidence.md` with the focused gate matrix, prerequisites, outputs, and recoupling rules.
- [F] T032 [US2] Capture real direct-invocation evidence for the required focused gates and write `readiness/focused-gates.md`.
- [ ] T033 [US2] Document the US2 independent validation path and map evidence to FR-006 through FR-008 and SC-003.
- [ ] T034 Record the P1 checkpoint showing broad-verdict honesty and focused-gate independence are independently testable.

**Checkpoint**: P1 stories are functional and independently testable.

---

## Phase 4: P2 User Stories

### User Story 3 - Reduce Governance False Positives

#### Tests First

- [ ] T035 [P] [US3] Add dependency scanner fixtures for package/project substring false positives and real `PackageReference` or `ProjectReference` violations.
- [ ] T036 [P] [US3] Add generated product profile fixtures for allowed `sample-pack` content, forbidden ordinary-profile copied framework content, and stale package rejection.
- [ ] T037 [P] [US3] Add generated guidance and inventory tests requiring source markers and test markers for `RichText.create`, `LineChart.create`, `GraphView.create`, `DataGrid.create`, and `ControlsElmish.program`.

#### Implementation

- [ ] T038 [US3] Refactor `scripts/dependency-report.fsx` and matching governance checks to use project XML, central package XML, or anchored governed metadata scanning instead of arbitrary substring matching.
- [ ] T039 [US3] Make generated product scanning profile-aware so `sample-pack` allows intended generated `samples/` content while ordinary profiles reject copied framework samples, implementation projects, historical specs, readiness evidence, docs, and stale package references.
- [ ] T040 [US3] Extend generated product inventories to include source files, test files, package references, selected capabilities/skills, command logs, behavior markers, and framework-source exclusion results.
- [ ] T041 [US3] Add scanner diagnostics that name rule id, file path, generated profile, package/project reference, capability id, source/test marker, readiness path, and remediation hint.
- [ ] T042 [US3] Write scanner evidence to `readiness/governance-scanners.md` and `readiness/generated-product-validation.md`.
- [ ] T043 [US3] Update `docs/dependencies.md`, `docs/template-profile.md`, `docs/evidence.md`, and generated guidance docs with scanner accuracy rules.
- [ ] T044 [US3] Capture real evidence from `dotnet test tests/Governance.Tests/Governance.Tests.fsproj -m:1 --no-restore`, `DependencyReport`, `GeneratedProductCheck`, `TemplateCheck`, `GeneratedGuidanceCheck`, and `TemplateDrift`.
- [ ] T045 [US3] Document the US3 independent validation path and map evidence to FR-009 through FR-011, FR-015, SC-004, and SC-005.

### User Story 4 - Catch Stale Boundary Evidence Earlier

#### Tests First

- [ ] T046 [P] [US4] Add seeded stale-reference fixtures across governance memory, architecture docs, active source, active tests, package metadata, capability metadata, template fragments, generated guidance, and readiness evidence.
- [ ] T047 [P] [US4] Add tests proving historical or migration references are allowed only when context clearly states replacement, removal, history, or deletion guidance.
- [ ] T048 [P] [US4] Add removed-package evidence tests requiring one report to identify source deletion, test deletion, package reference status, capability entry status, and active ownership documentation status.
- [ ] T049 [P] [US4] Add command-contract tests proving stale-boundary scanning runs before final evidence audit completion is accepted.

#### Implementation

- [ ] T050 [US4] Implement stale-boundary scanning for active-tree ownership docs, `.specify/memory/constitution.md`, architecture docs, source, tests, package metadata, capability metadata, generated guidance, template fragments, and readiness evidence.
- [ ] T051 [US4] Implement stale reference classification that separates forbidden active ownership references from allowed historical or migration references.
- [ ] T052 [US4] Wire stale-boundary scanning into the final audit path, or a documented focused gate consumed by `EvidenceAudit`, so stale active ownership blocks final readiness before audit completion.
- [ ] T053 [US4] Implement removed package evidence aggregation for source, tests, package references, capability entries, active ownership claims, and migration guidance.
- [ ] T054 [US4] Add diagnostics naming stale term, file path, line or context, classification, rule id, evidence path, and remediation action.
- [ ] T055 [US4] Update `docs/architecture.md`, `docs/build.md`, `docs/evidence.md`, generated guidance, and any stale active references found in `.specify/memory/constitution.md`; do not introduce new constitution rules here unless a separate `/speckit-constitution` change is explicitly approved.
- [ ] T056 [US4] Capture real stale-boundary evidence in `readiness/stale-boundary-scan.md`.
- [ ] T057 [US4] Document the US4 independent validation path and map evidence to FR-012, FR-012a, FR-013, FR-015, SC-006, and SC-007.
- [ ] T058 Record the P2 checkpoint showing scanner accuracy and stale-boundary readiness are independently testable.

**Checkpoint**: P2 stories are functional and independently testable.

---

## Phase 5: Integration & Polish

- [ ] T059 [P] Add final-readiness blocking tests proving a broad aggregate `environment-failure` requires a later healthy broad aggregate pass, while focused passing evidence remains diagnostic but not final product proof.
- [ ] T060 Integrate final readiness reports so process-health, bootstrap, verification verdicts, focused gates, governance scanners, generated product validation, and stale-boundary scan status are consumed consistently.
- [ ] T061 Run `./fake.sh build -t BuildWorkflowCheck` and the governance test suite; save command logs under `readiness/logs/`.
- [ ] T062 Run required focused gates directly and refresh `focused-gates.md`, `governance-scanners.md`, `generated-product-validation.md`, and `stale-boundary-scan.md`.
- [ ] T063 Run broad `Verify` and `Ci` on a healthy runner, or record an environment-failure verdict plus the exact fresh-run requirement if the current runner is not authoritative.
- [ ] T064 Run `./fake.sh build -t PackageSurfaceCheck` or equivalent surface review and record that Controls product behavior, public APIs, and package ownership did not change.
- [ ] T065 Run `./fake.sh build -t EvidenceGraph` and update `readiness/evidence-graph.md` or link to `readiness/task-graph.md`.
- [ ] T066 Run `./fake.sh build -t EvidenceAudit` and update `readiness/evidence-audit.md`, resolving any synthetic propagation or diff-scan blockers before declaring completion.
- [ ] T067 Perform final documentation/readiness review: every required evidence path links to logs, final readiness states authoritative/waiting/environment-failure status, and the Synthetic-Evidence Inventory is accurate.

**Checkpoint**: Feature complete — final readiness can be reviewed.

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| _(none yet)_ | | | |

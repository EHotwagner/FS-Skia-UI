# Tasks: Template Packaging and Drift Governance

**Feature branch**: `007-v2-template-packaging`
**Spec**: `specs/007-v2-template-packaging/spec.md`
**Plan**: `specs/007-v2-template-packaging/plan.md`

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

A task tagged `[US*]` may only be marked `[X]` when the change is reachable
from an operator-facing or generated-product entry point and that path was
actually exercised. For this feature, those entry points are the canonical
workflow targets such as `./fake.sh build -t TemplateCheck`,
`DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`, `Verify`, and
the generated projects' `./fake.sh build -t Dev` workflow.

This feature does not add a runtime product `.fsi` API. Because template
validation, dependency reporting, guidance checks, and drift detection are
process/file I/O-bearing workflows, Principle IV is satisfied through the
local workflow effect algebra in `build.fsx`: `BuildModel`, `BuildMsg`,
`BuildEffect`, `init`, pure `update`, and an interpreter that executes effects
at the edge. `[X]` for story work requires pure transition tests, emitted
effect assertions, and real interpreter evidence where safe.

This rule does not apply to Setup, Foundation, Integration, or Polish phase
tasks; those are evaluated against their own phase verification.

## Task Annotations

- **[P]** - parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, ... - user-story scope
- **[T1]** / **[T2]** - Tier 1 (contracted) vs Tier 2 (internal) change

Every task must have a matching entry in `tasks.deps.yml` even if its
dependency list is empty. The evidence graph command refuses to proceed with
dangling references.

## Canonical Verification Targets

Implementation tasks should call repository targets instead of duplicating raw
restore/build/test/package/evidence command order:

- `./fake.sh build -t Dev` for fast local restore/build/default non-visual tests.
- `./fake.sh build -t TemplateCheck` for source and local-package template validation.
- `./fake.sh build -t DependencyReport` for central package governance.
- `./fake.sh build -t GeneratedGuidanceCheck` for Spec Kit spec/plan prompt checks.
- `./fake.sh build -t TemplateDrift` for template-owned drift and deferral validation.
- `./fake.sh build -t Verify` for existing V1 verification plus V2 gates.
- `./fake.sh build -t Ci` for automation delegation to `Verify`.
- `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`
  for graph and synthetic-evidence gates.

Keep `tasks.deps.yml` and the `speckit.evidence.graph` status refresh
requirements in generated task lists.

---

## Phase 1: Setup

- [X] T001 Create feature readiness scaffolding under `specs/007-v2-template-packaging/readiness/` for logs, template validation output, dependency reports, generated guidance, drift reports, task graph output, and audit output
- [X] T002 [P] Inventory template-owned source files, generated-product exclusions, optional profile scope, historical specs/readiness paths, and placeholder tokens in `specs/007-v2-template-packaging/readiness/template-source-inventory.md`
- [X] T003 [P] Inventory current direct package references and local package smoke/version exceptions in `specs/007-v2-template-packaging/readiness/dependency-inventory.md`
- [X] T004 [P] Record V2 evidence obligations in `specs/007-v2-template-packaging/readiness/evidence-obligations.md`, including Tier 1 scope, explicit no-op runtime `.fsi`/surface-baseline impact, `BuildModel` / `BuildMsg` / `BuildEffect` applicability, real evidence requirements, and deferred visual/release/external distribution boundaries
- [X] T005 [P] Create a traceability matrix mapping FR/SC/contract targets to planned tests, implementation files, docs, and readiness artifacts

**Checkpoint**: Setup complete.

---

## Phase 2: Foundation

### Tests First

- [X] T006 [P] Add governance test helpers in `tests/Governance.Tests/` for reading JSON, XML, YAML, Markdown, feature readiness paths, and focused FAKE target output
- [X] T007 [P] Add failing V2 command-contract tests for required target names, target dependencies, `007-v2-template-packaging` readiness paths, `Verify`/`Ci` extension, explicit `.fsi`/surface-baseline no-op assertions, pure `update` transitions, emitted effects, and interpreter boundaries
- [X] T008 [P] Add failing readiness artifact path tests for V2 package logs, install logs, generated project logs, placeholder/excluded-history scans, dependency report, generated guidance report, drift report, and local template package output
- [X] T009 [P] Add failing safe-operation tests for target-owned temp-root cleanup, source-preserving `Clean` behavior, missing tool/network diagnostics, and missing artifact-class diagnostics

### Implementation

- [X] T010 Add shared path/model fields to `build.fsx` for the 007 feature directory, template logs, generated project roots, `artifacts/templates/`, and root deferral records while preserving V1 target behavior
- [X] T011 Extend `BuildMsg`, `BuildEffect`, `update`, and interpreter helpers for template packaging, template installs, generated project commands, scan reports, structured report writing, and required artifact classes
- [X] T012 Extend target discovery and target dependencies with `TemplatePack`, `TemplateInstallSource`, `TemplateInstallPackage`, `TemplateInstantiate`, `TemplateSmoke`, `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, and `TemplateDrift`; keep `Dev` independent of template packaging
- [X] T013 Run foundation verification for command contracts, `.fsi`/surface-baseline no-op evidence, MVU/effect assertions, readiness artifact paths, and safe-operation diagnostics; store output under `specs/007-v2-template-packaging/readiness/logs/`

**Checkpoint**: Foundation ready - story implementation may begin.

---

## Phase 3: User Story 1 (US1) - Create a Clean Project From the Template

### Tests First

- [X] T014 [P] [US1] Add `TemplateProfileTests` requiring `.template.config/template.json` to define `fs-skia-ui`, `default` and `minimal` profile choices, product identity symbols, source/exclude modifiers, and historical feature/readiness exclusions
- [X] T015 [P] [US1] Add `TemplateWorkflowTests` for `TemplatePack`, source/package install targets, four-row artifact/profile instantiation, package artifact paths, target dependencies, emitted effects, and `TemplateCheck` verdict output
- [X] T016 [P] [US1] Add generated project validation tests or fixtures for unreplaced placeholder diagnostics, excluded-history diagnostics, minimal profile required contents, optional layout/charts/parity/visual exclusions, generated `Dev` logs, explicit non-visual/no-graphics support messaging, and broken-reference failures
- [X] T017 [P] [US1] Add a real-interpreter evidence plan for source install, package install, default/minimal generation, generated `Dev` execution, and logs under `specs/007-v2-template-packaging/readiness/template/`

### Implementation

- [X] T018 [US1] Add `.template.config/template.json` with template identity, `profile` choice symbols, product identity substitutions, include/exclude source modifiers, default profile contents, minimal profile contents, and source-only history exclusions
- [X] T019 [US1] Add `.template.package/FS.Skia.UI.Template.fsproj` with local NuGet template package metadata and pack output under `artifacts/templates/`
- [X] T020 [US1] Implement `TemplatePack` and package content verification so `FS.Skia.UI.Template.*.nupkg` contains template metadata and template-owned files while excluding source-only artifacts
- [X] T021 [US1] Implement `TemplateInstallSource` and `TemplateInstallPackage` with uninstall/reinstall-safe behavior, actionable diagnostics, and separate readiness logs
- [X] T022 [US1] Implement `TemplateInstantiate` to create source/default, source/minimal, package/default, and package/minimal generated projects in isolated target-owned temp roots
- [X] T023 [US1] Implement `TemplateSmoke` placeholder scans, excluded-history scans, optional profile reference checks, generated `./fake.sh build -t Dev` execution, per-project logs, and summary diagnostics
- [X] T024 [US1] Implement `TemplateCheck` as the full V2 template validation target, require all readiness artifact classes, and wire `Verify`/`Ci` to include it without changing `Dev` into a packaging target
- [X] T025 [US1] Document template profiles, generation options, artifact boundaries, validation commands, readiness paths, non-visual/no-graphics support messaging, and deferred visual/release/external distribution scope in `docs/template-profile.md`, `docs/build.md`, `docs/testing.md`, `docs/evidence.md`, README, and quickstart references
- [X] T026 [US1] Run focused `TemplateCheck` or staged source/package/default/minimal validation and store package, install, generation, placeholder, excluded-history, generated `Dev`, and verdict evidence

**Checkpoint**: US1 template generation is independently testable.

---

## Phase 4: User Story 2 (US2) - Govern Dependency Versions Centrally

### Tests First

- [X] T027 [P] [US2] Add `DependencyGovernanceTests` requiring `Directory.Packages.props` to enable Central Package Management, declare direct `<PackageVersion />` entries, and keep repo-owned `.fsproj` external `PackageReference` entries versionless
- [X] T028 [P] [US2] Add dependency metadata/report tests requiring `docs/dependencies.md` fields for package id, version, purpose, owner, license posture, upgrade expectation, preview risk where relevant, validation-only exceptions, and readiness output
- [X] T029 [P] [US2] Add a negative scan fixture or test proving an unmanaged inline external package version fails with project path, package id, and required remediation

### Implementation

- [X] T030 [US2] Add `Directory.Packages.props` with central versions for current direct external packages and Central Package Management enabled
- [X] T031 [US2] Remove inline external dependency versions from repo-owned project files while preserving only documented validation-only local package version properties
- [X] T032 [US2] Write `docs/dependencies.md` with required dependency metadata, preview-risk notes, and validation-only exception policy
- [X] T033 [US2] Implement `scripts/dependency-report.fsx` to scan project files, compare central policy and docs metadata, validate exceptions, and emit `specs/007-v2-template-packaging/readiness/dependencies.md`
- [X] T034 [US2] Add the `DependencyReport` FAKE target and include it in `Verify`
- [X] T035 [US2] Ensure generated default and minimal template profiles include central dependency policy files and dependency governance documentation expected by new products
- [X] T036 [US2] Run `./fake.sh build -t DependencyReport` and focused dependency governance tests; store readiness evidence and diagnostics

**Checkpoint**: US2 dependency governance is independently testable.

---

## Phase 5: User Story 3 (US3) - Harden Generated Feature Guidance

### Tests First

- [X] T037 [P] [US3] Add generated spec guidance tests requiring active and preset-owned `spec-template.md` files to prompt for package impact, public contract impact, state workflow impact, layout/rendering impact, evidence obligations, unsupported scope, and build-target impact
- [X] T038 [P] [US3] Add generated plan guidance tests requiring active and preset-owned `plan-template.md` files to decide template ownership, dependency impact, command-surface impact, generated project impact, evidence paths, `.fsi`/contract impact, MVU/effect boundary applicability, synthetic evidence, test evidence, observability, and deferred scope
- [X] T039 [P] [US3] Add generated-artifact tests proving V2 obligations are distinguished from deferred visual evidence, release validation, external repository split, and distribution automation, with no manual copying from historical feature directories

### Implementation

- [X] T040 [US3] Update `.specify/templates/spec-template.md` and `.specify/presets/fsharp-opinionated/templates/spec-template.md` with the required V2 spec prompts
- [X] T041 [US3] Update `.specify/templates/plan-template.md` and `.specify/presets/fsharp-opinionated/templates/plan-template.md` with the required V2 planning decisions and constitution checks
- [X] T042 [US3] Write `docs/speckit.md` documenting generated spec/plan governance, preset inheritance, evidence expectations, and deferred roadmap boundaries
- [X] T043 [US3] Implement a generated guidance checker and the `GeneratedGuidanceCheck` FAKE target that emits `specs/007-v2-template-packaging/readiness/generated-guidance.md`
- [X] T044 [US3] Include `GeneratedGuidanceCheck` in `Verify` and in generated project validation where the default and minimal profiles carry Spec Kit assets
- [X] T045 [US3] Run `./fake.sh build -t GeneratedGuidanceCheck` and focused generated guidance tests; store readiness evidence

**Checkpoint**: US3 generated guidance is independently testable.

---

## Phase 6: User Story 4 (US4) - Detect Template Drift

### Tests First

- [X] T046 [P] [US4] Add `TemplateDriftTests` proving changed template-owned source, docs, presets, dependency policy, samples, and command-surface paths require template, docs, dependency, guidance, command, or deferral alignment
- [X] T047 [P] [US4] Add deferral validation tests requiring every accepted record to include `id`, paths, rationale, owner, and target phase, and proving records only cover named paths
- [X] T048 [P] [US4] Add drift report tests for path-level diagnostics, accepted deferrals, missing alignment actions, missing artifact classes, and readiness output

### Implementation

- [X] T049 [US4] Define template ownership and drift classification rules in `docs/template-profile.md` and/or machine-readable configuration consumed by the drift check
- [X] T050 [US4] Create root-level `readiness/template-deferrals.yml` with schema comments and no accepted deferrals unless implementation discovers an intentional source-only or future-roadmap exception
- [X] T051 [US4] Implement `scripts/template-drift.fsx` to collect changed paths, classify required alignment, validate deferrals, reject missing fields, and emit `specs/007-v2-template-packaging/readiness/template-drift.md`
- [X] T052 [US4] Add the `TemplateDrift` FAKE target and include it in `Verify`
- [X] T053 [US4] Update `docs/build.md`, `docs/testing.md`, `docs/evidence.md`, README, and `.specify/workflows/speckit/workflow.yml` so V2 validation delegates to canonical targets and documents drift/deferral boundaries
- [X] T054 [US4] Run `./fake.sh build -t TemplateDrift` plus at least one negative fixture/test for missing alignment or invalid deferral; store readiness evidence

**Checkpoint**: US4 drift governance is independently testable.

---

## Phase 7: Integration & Polish

- [X] T055 [P] Run `./fake.sh build -t Dev` and confirm the V1 fast workflow still restores, builds, and runs default non-visual tests without template packaging
- [X] T056 [P] Run `./fake.sh build -t TemplateCheck` and confirm all four artifact/profile rows pass, no placeholders remain, no excluded history is present, generated `Dev` completes within the 15 minute target per project, elapsed time is recorded per artifact/profile row, and required artifact classes exist
- [X] T057 [P] Run `./fake.sh build -t DependencyReport`, `./fake.sh build -t GeneratedGuidanceCheck`, and `./fake.sh build -t TemplateDrift`; confirm readiness outputs and actionable diagnostics
- [X] T058 Run `./fake.sh build -t Verify` and `./fake.sh build -t Ci`; confirm V1 plus V2 gates pass and `Ci` delegates to `Verify`
- [X] T059 Validate `artifacts/templates/FS.Skia.UI.Template.*.nupkg` and generated output inventories directly enough to confirm packaged artifact shape matches source-directory validation
- [X] T060 Record a minimal profile review proving core library, one basic sample, core tests, package checks, docs, and Spec Kit governance assets are present while optional layout, charts, parity, and visual sample scope are absent
- [X] T061 [P] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/007-v2-template-packaging --graph-only` and confirm no cycles, dangling references, orphaned tasks, or unexpected propagated statuses
- [X] T062 Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/007-v2-template-packaging` and confirm PASS, or document every unresolved synthetic-evidence or diff-scan blocker
- [X] T063 Update quickstart, contracts, plan notes, and readiness final review only if final target names, artifact paths, or deferred boundaries changed during implementation
- [X] T064 Prepare the merge summary with command results, readiness evidence paths, template validation matrix, dependency governance verdict, drift verdict, synthetic-evidence inventory, and deferred roadmap boundaries

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| _(none yet)_ | | | |

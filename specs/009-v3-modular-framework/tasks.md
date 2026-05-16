# Tasks: V3 Modular Framework

**Feature branch**: `009-v3-modular-framework`
**Spec**: `specs/009-v3-modular-framework/spec.md`
**Plan**: `specs/009-v3-modular-framework/plan.md`

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
from a maintainer-facing, generated-product, or package-consumer entry point
and that path was actually exercised. For this feature, those entry points
include `./fake.sh build -t CapabilityCheck`, `SkillCheck`,
`GeneratedProductCheck`, `TemplateCheck`, `DependencyReport`,
`PackageSurfaceCheck`, generated products' `./fake.sh build -t Dev`, `Test`,
and `Verify`, generated file-list reports, selected-skill reports, packed
library checks, and FSI sessions through package-specific `.fsi` contracts.

This feature is stateful and I/O-bearing. Principle IV evidence applies to
generated-product and build-governance workflows through `BuildModel`,
`BuildMsg`, `BuildEffect`, `init`, pure `update`, emitted-effect assertions,
and the edge interpreter. Runtime capability ownership also requires explicit
public state/effect contracts where applicable: SkiaViewer, Elmish,
KeyboardInput, generated command workflows, and any package that exposes
`Model`, `Msg`, `Effect`, `Cmd<Msg>`, `init`, `update`, subscriptions, or
interpreter boundaries. `[X]` for story work requires pure transition tests,
emitted-effect assertions, and real interpreter or generated-product command
evidence where safe.

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
- `./fake.sh build -t Verify` for the full governed workflow.
- `./fake.sh build -t Ci` for automation delegation to `Verify`.
- `./fake.sh build -t PackLocal` for local package output.
- `./fake.sh build -t RefreshSurfaceBaselines` for intentional current surface
  baseline refreshes.
- `./fake.sh build -t PackageSurfaceCheck` for package surface review.
- `./fake.sh build -t FsiTranscripts` for public FSI evidence.
- `./fake.sh build -t TemplateCheck` for source and local-package template
  validation.
- `./fake.sh build -t CapabilityCheck` for V3 capability catalog validation.
- `./fake.sh build -t SkillCheck` for package-owned local skill validation.
- `./fake.sh build -t GeneratedProductCheck` for V3 generated product
  cleanliness, selected capability, and governance validation.
- `./fake.sh build -t DependencyReport` for central package governance.
- `./fake.sh build -t GeneratedGuidanceCheck` for generated spec/plan prompt
  governance.
- `./fake.sh build -t TemplateDrift` for template-owned drift and deferral
  validation.
- `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`
  for graph and synthetic-evidence gates.

Keep `tasks.deps.yml` and the evidence graph status refresh requirements in
this generated task list.

---

## Phase 1: Setup

- [X] T001 Create feature readiness scaffolding under `specs/009-v3-modular-framework/readiness/` for capability catalog, generated file lists, generated product verify logs, selected skills, package surfaces, dependency report, generated guidance, template drift, compatibility impact, logs, task graph, and audit output
- [X] T002 [P] Inventory current package, source, test, and surface-baseline assets in `specs/009-v3-modular-framework/readiness/package-boundary-inventory.md`, including `src/Lib`, `src/Layout`, `src/Charts`, current tests, and `readiness/surface-baselines/`
- [X] T003 [P] Inventory current template packaging, generated-product validation, command wrappers, and generated-output exclusions in `specs/009-v3-modular-framework/readiness/template-source-inventory.md`
- [X] T004 [P] Inventory current repository and package-local agent guidance candidates in `specs/009-v3-modular-framework/readiness/skill-inventory.md`
- [X] T005 [P] Record feature Tier 1 scope, affected package/template/governance layers, public-API impact, Elmish/MVU applicability, unsupported V2 migration scope, synthetic evidence policy, and required evidence obligations in `specs/009-v3-modular-framework/readiness/evidence-obligations.md`
- [X] T006 [P] Create a traceability matrix in `specs/009-v3-modular-framework/readiness/traceability.md` mapping FR/SC/contract targets to planned tests, implementation files, commands, and readiness artifacts

**Checkpoint**: Setup complete.

---

## Phase 2: Foundation

### Tests First

- [X] T007 [P] Add shared V3 governance test helpers in `tests/Governance.Tests/TestSupport.fs` for YAML catalog parsing, generated file-list assertions, selected-skill inventories, command output assertions, and readiness report checks
- [X] T008 [P] Add failing command-contract tests in `tests/Governance.Tests/CommandContractTests.fs` for `CapabilityCheck`, `SkillCheck`, `GeneratedProductCheck`, expanded `TemplateCheck`/`Verify`/`Ci` dependencies, `BuildModel`, `BuildMsg`, `BuildEffect`, `init`, pure `update`, emitted effects, and interpreter boundaries
- [X] T009 [P] Add failing package boundary and surface tests in `tests/Package.Tests/SurfaceAreaTests.fs` requiring package-specific `.fsi` contracts, package-specific surface baselines, no top-level visibility modifiers as contract substitutes, and Scene dependency exclusions
- [X] T010 [P] Add failing capability catalog schema and profile tests in `tests/Governance.Tests/TemplateProfileTests.fs` for `template/capabilities.yml`, the default app capability set, dependency closure, cycle diagnostics, and profile rows
- [X] T011 [P] Add failing generated-product cleanliness and governance fixture tests in `tests/Governance.Tests/GeneratedProjectValidationTests.fs` for unexpected framework paths, selected skills, generated docs, full product governance, and consumer-mode package references
- [X] T012 [P] Add failing local skill contract tests in `tests/Governance.Tests/GeneratedGuidanceTests.fs` or a new skill validation test module requiring required skill sections, valid command references, and generated destination rules

### Implementation

- [X] T013 Define V3 validation paths, schemas, and error record types in `build.fsx` and/or scripts for capability catalog rows, selected skills, generated product rows, package surface reports, dependency reports, and feature readiness outputs
- [X] T014 Extend `BuildModel`, `BuildMsg`, `BuildEffect`, `init`, pure `update`, interpreter handling, and target graph wiring for `CapabilityCheck`, `SkillCheck`, `GeneratedProductCheck`, default app/package profile rows, and `009-v3-modular-framework` readiness paths
- [X] T015 Draft or update the package-specific public contract and compile-order plan for Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Charts, and Testing, including `Model`/`Msg`/`Effect` or `Cmd<Msg>`, `init`, `update`, subscriptions, and interpreter boundaries where stateful behavior applies
- [X] T016 Run foundation command-contract, package-boundary, catalog-schema, generated-product fixture, and skill-contract checks; store failing-first or focused output under `specs/009-v3-modular-framework/readiness/logs/`

**Checkpoint**: Foundation ready - story implementation may begin in priority order.

---

## Phase 3: User Story 1 (US1) - Generate a Lean Product

**Goal**: Generate a default V3 product that is a framework consumer, not a copy of the framework repository.

**Independent Test**: Generate the default product profile from source and packaged template paths, inspect file lists, and run the generated product's `Dev`, `Test`, and `Verify` commands.

### Tests First

- [X] T017 [P] [US1] Add default generated product content tests requiring exactly one product app, exactly one product test suite, product README/docs, command wrappers, full Spec Kit governance, selected local skills, package references for Scene, SkiaViewer, Elmish, KeyboardInput, Layout, and Charts, and no framework samples, galleries, parity suite, historical specs, readiness evidence, framework docs, framework README copy, implementation projects, template package project, or generated validation roots
- [X] T018 [P] [US1] Add generated product governance workflow tests requiring generated `Dev`, `Test`, and `Verify` to run product evidence gates, drift checks, generated guidance checks, readiness workflow, and selected capability usage checks while excluding framework gallery, parity, package-surface maintenance, template packaging, and framework-source maintenance checks
- [X] T019 [P] [US1] Add a real-interpreter evidence plan for source and packaged default product generation, generated `Dev`/`Test`/`Verify` execution, file-list reports, selected-skill reports, and observed command durations

### Implementation

- [X] T020 [US1] Create `template/base/` with one product app, one product test suite, product README/docs, command wrappers, and product-level Spec Kit governance assets
- [X] T021 [US1] Add default capability fragments for Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Charts, and full governance without copying framework implementation source
- [X] T022 [US1] Implement default app generation from source and packaged template paths, using package references or equivalent generated consumer references for the default capabilities
- [X] T023 [US1] Implement `GeneratedProductCheck` file-list reports and diagnostics for missing required files, extra product projects, copied framework paths, missing governance, unrelated skills, and consumer-mode implementation references
- [X] T024 [US1] Implement the generated product `Dev`, `Test`, and `Verify` command surface so it checks product behavior, selected capability usage, evidence graph/audit, generated guidance, drift, and readiness workflow without framework-source maintenance targets
- [X] T025 [US1] Update generated product docs and guidance so the default app identifies Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Charts, and product commands without framework architecture, V2 analysis, subsystem design, or template framework analysis documents
- [X] T026 [US1] Run source and package default app generation plus `GeneratedProductCheck`; store file lists and diagnostics under `specs/009-v3-modular-framework/readiness/generated-file-lists/`
- [X] T027 [US1] Run generated default product `Dev`, `Test`, and `Verify`; store logs under `specs/009-v3-modular-framework/readiness/generated-product-verify/`
- [X] T028 [US1] Document the US1 validation path and readiness verdict, including zero default framework samples, galleries, historical specs, readiness directories, framework docs, framework README content, and framework implementation projects

**Checkpoint**: US1 default generated product is independently testable.

---

## Phase 4: User Story 2 (US2) - Select Framework Capabilities Explicitly

**Goal**: Resolve selected capabilities and prerequisites predictably across app, headless, governed, and sample-pack profiles.

**Independent Test**: Generate products with at least four representative capability selections and compare package references, copied skills, commands, and generated files against the resolved capability set.

### Tests First

- [X] T029 [P] [US2] Add capability resolver tests for selected prerequisites, missing dependency diagnostics, dependency cycle diagnostics, and scene-only, default app, governed, and sample-pack selections
- [X] T030 [P] [US2] Add template profile tests for `app`, `headless-scene`, `governed`, and `sample-pack` source/package rows, including sample inclusion only through explicit sample profile or sample selection
- [X] T031 [P] [US2] Add generated product matrix tests for at least four representative capability selections checking package references, copied skills, command surface, generated files, and absence of unrelated capabilities
- [X] T032 [P] [US2] Add generated product verification tests asserting each representative selection runs product governance while excluding framework-source checks

### Implementation

- [X] T033 [US2] Create `template/capabilities.yml` with entries for Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Charts, Testing, and Samples, including default app flags, dependencies, profiles, evidence classes, validation paths, and surface baseline paths
- [X] T034 [US2] Create `template/profiles/app.yml`, `template/profiles/headless-scene.yml`, `template/profiles/governed.yml`, and `template/profiles/sample-pack.yml` with deterministic capability closure, governance, sample, and source-framework mode rules
- [X] T035 [US2] Implement capability selection, dependency closure, prerequisite reporting, and actionable failure diagnostics in template generation and validation scripts
- [X] T036 [US2] Create capability template fragments for `scene`, `skiaviewer`, `elmish`, `keyboard-input`, `layout`, `charts`, `testing`, `full-governance`, and `samples` with deterministic include/exclude rules
- [X] T037 [US2] Update `TemplateCheck` to generate and validate app, headless-scene, governed, and sample-pack profiles from source and packaged template paths
- [X] T038 [US2] Update sample-pack handling so samples are excluded by default and supplied only when a sample-oriented profile or sample capability is selected
- [X] T039 [US2] Run the representative capability selection matrix and store file-list, selected-skill, package-reference, and generated command reports under `specs/009-v3-modular-framework/readiness/generated-file-lists/`
- [X] T040 [US2] Document selected capability closure, prerequisite inclusions, and generated output messages in `specs/009-v3-modular-framework/readiness/capability-selection.md` and generated product quickstart/guidance

**Checkpoint**: US2 capability selection is independently testable.

---

## Phase 5: User Story 3 (US3) - Maintain Capability Ownership

**Goal**: Make every reusable capability's package, public contract, tests, skill, docs, fragment, dependencies, and evidence path reviewable before approval.

**Independent Test**: Run `CapabilityCheck`, package tests, dependency reports, surface checks, and packed-library or FSI evidence for each public capability package.

### Tests First

- [X] T041 [P] [US3] Add `CapabilityCheck` tests requiring every selectable capability to declare owner notes, package/project or non-runtime marker, `.fsi` contracts or no-public-surface record, tests, docs, skill, template fragment, dependencies, validation path, evidence classes, and surface baseline
- [X] T042 [P] [US3] Add dependency report tests requiring Scene to have no Elmish, Silk.NET, SkiaSharp, Yoga.Net, or YamlDotNet dependency and requiring SkiaViewer, Elmish, KeyboardInput, Layout, Charts, and Testing dependencies to match the contract
- [X] T043 [P] [US3] Add package semantic tests per capability package exercising public `.fsi` contracts, including pure transition and effect-emission tests for SkiaViewer, Elmish, and KeyboardInput where applicable
- [X] T044 [P] [US3] Add package surface baseline tests requiring package-specific baselines or explicit no-public-surface records and actionable diagnostics for drift, missing `.fsi`, unapproved exports, and missing baselines
- [X] T045 [P] [US3] Add an FSI and packed-library smoke evidence plan for public contracts, representative package use, and package surface evidence under `specs/009-v3-modular-framework/readiness/package-surfaces/`

### Implementation

- [X] T046 [US3] Split or retarget projects toward `src/Scene`, `src/SkiaViewer`, `src/Elmish`, `src/KeyboardInput`, `src/Layout`, `src/Charts`, and `src/Testing` packable capability packages while preserving staged buildability
- [X] T047 [US3] Move or curate `.fsi` public contracts for each capability, including state/effect boundaries and no-public-surface records where needed
- [X] T048 [US3] Update solution files, project references, `Directory.Packages.props`, package metadata, and package references so capability packages own only their allowed dependencies
- [X] T049 [US3] Add or retarget tests under `tests/Scene.Tests`, `tests/SkiaViewer.Tests`, `tests/Elmish.Tests`, `tests/KeyboardInput.Tests`, `tests/Layout.Tests`, `tests/Charts.Tests`, `tests/Testing.Tests`, `tests/Package.Tests`, and `tests/Governance.Tests`
- [X] T050 [US3] Implement `CapabilityCheck`, package-owned validation paths, and diagnostics for missing catalog metadata, dependency cycles, missing contracts, missing tests, missing docs, missing skills, missing fragments, and default app mismatches
- [X] T051 [US3] Implement or update `DependencyReport`, `PackageSurfaceCheck`, `PackLocal`, and `RefreshSurfaceBaselines` for package-specific surfaces and V3 dependency ownership
- [X] T052 [US3] Capture package-specific surface baselines under `readiness/surface-baselines/` and feature evidence under `specs/009-v3-modular-framework/readiness/package-surfaces/`
- [X] T053 [US3] Run `CapabilityCheck`, `DependencyReport`, `PackLocal`, `PackageSurfaceCheck`, focused package tests, and FSI or packed-library checks; store readiness evidence and diagnostics
- [X] T054 [US3] Write `specs/009-v3-modular-framework/readiness/compatibility-impact.md` stating affected packages/generated products, public surface impact, package identity impact, reviewer notes, migration/non-migration guidance for existing consumers, and that V2 migration implementation support is out of scope

**Checkpoint**: US3 capability ownership is independently testable.

---

## Phase 6: User Story 4 (US4) - Guide Agents With Selected Local Skills

**Goal**: Copy only the project-level skill and selected or prerequisite capability skills into generated products.

**Independent Test**: Generate products with different capability selections and inspect selected local skills, skill content, command references, and absence of unrelated capability skills.

### Tests First

- [X] T055 [P] [US4] Add `SkillCheck` tests for required sections and command validity in each capability `skill/SKILL.md`
- [X] T056 [P] [US4] Add generated-product selected-skill copy tests for keyboard skill present when keyboard input is selected, charts skill absent when charts is unselected, sample skill only present for sample profile, project skill always present, and prerequisite skills included
- [X] T057 [P] [US4] Add generated skill content and readiness tests requiring scope, owned files, public contract guidance, verification commands, evidence rules, package boundary guidance, and generated product considerations

### Implementation

- [X] T058 [US4] Add the project-level generated product skill and package-owned skills under each capability source root with required scope, command, evidence, and package-boundary guidance
- [X] T059 [US4] Implement selected skill copy logic from resolved capabilities to generated product destinations, including prerequisite-derived skills
- [X] T060 [US4] Implement `SkillCheck` and selected-skill report output under `specs/009-v3-modular-framework/readiness/selected-skills.md` with diagnostics for missing sections, unrelated skills, missing generated destinations, and invalid command references
- [X] T061 [US4] Wire selected skills into default app and explicit capability selection profiles, including project-level skill plus Scene, SkiaViewer, Elmish, KeyboardInput, Layout, and Charts for the default app
- [X] T062 [US4] Update generated product docs, README, and Spec Kit guidance to refer to selected skills and product-owned evidence only
- [X] T063 [US4] Run `SkillCheck` and the generated product skill matrix; store selected-skills evidence and generated destination inventories
- [X] T064 [US4] Document the US4 independent validation path and any intentionally omitted framework-maintenance-only skills

**Checkpoint**: US4 selected local skills are independently testable.

---

## Phase 7: Integration & Polish

- [X] T065 [P] Run `./fake.sh build -t Dev` and store the log under `specs/009-v3-modular-framework/readiness/logs/`
- [X] T066 [P] Run `./fake.sh build -t CapabilityCheck` and `./fake.sh build -t SkillCheck`; store `capability-catalog.md` and `selected-skills.md` readiness reports
- [X] T067 [P] Run `./fake.sh build -t DependencyReport`, `./fake.sh build -t PackLocal`, `./fake.sh build -t PackageSurfaceCheck`, and FSI or packed-library public contract checks; store dependency and package surface evidence
- [X] T068 [P] Run `./fake.sh build -t TemplateCheck` and `./fake.sh build -t GeneratedProductCheck` for source/package app, headless-scene, governed, and sample-pack rows; store matrix results, file lists, and observed durations
- [X] T069 [P] Run generated product `Dev`, `Test`, and `Verify` for at least four representative capability selections; store logs under `specs/009-v3-modular-framework/readiness/generated-product-verify/`
- [X] T070 [P] Run `./fake.sh build -t GeneratedGuidanceCheck` and `./fake.sh build -t TemplateDrift`; confirm reports distinguish product governance from framework-source maintenance scope
- [X] T071 Run `./fake.sh build -t Verify` and `./fake.sh build -t Ci`; confirm full V3 gates pass and `Ci` delegates to `Verify`
- [X] T072 [P] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/009-v3-modular-framework --graph-only` and confirm no cycles, dangling references, orphaned tasks, or unexpected propagated statuses
- [X] T073 Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/009-v3-modular-framework` and confirm PASS, or document every unresolved synthetic-evidence or diff-scan blocker
- [X] T074 Complete the Synthetic-Evidence Inventory, final readiness review, and compatibility-impact cross-links so no synthetic-only evidence, V2 migration exclusion, or package-surface decision is hidden
- [X] T075 Update quickstart, contracts, docs, README, and workflow references only where final target names, artifact paths, profile names, diagnostics, or generated product boundaries changed during implementation
- [X] T076 Prepare the merge summary with command results, readiness evidence paths, generated product matrix, capability catalog verdict, selected-skill verdict, package surface/dependency verdict, compatibility-impact stance, and synthetic-evidence inventory

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| _(none)_ | Full evidence audit PASS; no task is marked `[S]`. | Not applicable. | Not applicable. |

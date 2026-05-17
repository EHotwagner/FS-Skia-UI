# Tasks: Controls Boundary Refactor

**Feature branch**: `011-controls-boundary-refactor`
**Spec**: `specs/011-controls-boundary-refactor/spec.md`
**Plan**: `specs/011-controls-boundary-refactor/plan.md`

## Status Legend

- `[ ]` - pending
- `[X]` - done with real evidence
- `[S]` - done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` - failed
- `[-]` - skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/evidence-graph.md` for the propagated view.

## Vertical-slice Rule (US Phases)

A task tagged `[US*]` may only be marked `[X]` when the change is reachable
from a user-facing or maintainer-facing entry point and that path was actually
exercised: an FSI session against packed packages, a generated product command,
a repository smoke run, a manual transcript, or render/readback evidence under
`readiness/`. Public contracts, model types, or core-layer changes alone do not
satisfy `[X]` for a `[US*]` task.

This feature is stateful and I/O-bearing. `[X]` for story work also requires
Elmish/MVU evidence where applicable: the public `Model`, `Msg`, `Effect` or
`Cmd<Msg>` contract was exercised, pure `init` / `update` transitions were
tested, emitted effects were asserted, and the effect interpreter was run
against real dependencies where safe. Base Controls must stay generic over
product messages; direct command, subscription, and program integration must be
proved through the adapter surface.

This rule does not apply to Setup, Foundation, Integration, or Polish phase
tasks; those are evaluated against their own phase verification.

## Task Annotations

- **[P]** - parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, ... - user-story scope
- **[T1]** / **[T2]** - Tier 1 (contracted) vs Tier 2 (internal) change

Every task must have a matching entry in `tasks.deps.yml` even if its
dependency list is empty. The evidence graph command refuses to proceed with
dangling references.

The feature is classified as Tier 1 overall, so task lines omit `[T1]` unless a
specific task differs from that classification.

## Canonical Verification Targets

Use repository targets instead of duplicating raw restore/build/test/package
command order:

- `./fake.sh build -t Dev` for fast local verification.
- `./fake.sh build -t Verify` for the full governed workflow.
- `./fake.sh build -t Ci` for CI-equivalent verification.
- `./fake.sh build -t PackLocal` for local package output.
- `./fake.sh build -t RefreshSurfaceBaselines` for intentional current surface baseline refreshes.
- `./fake.sh build -t PackageSurfaceCheck` for package surface review.
- `./fake.sh build -t FsiTranscripts` for public FSI evidence.
- `./fake.sh build -t SampleContractSmoke` for sample smoke evidence.
- `./fake.sh build -t TemplateCheck` for source/package generated project validation.
- `./fake.sh build -t CapabilityCheck` for generated capability catalog validation.
- `./fake.sh build -t SkillCheck` for package-local and generated skill validation.
- `./fake.sh build -t GeneratedProductCheck` for generated product validation.
- `./fake.sh build -t DependencyReport` for central package governance.
- `./fake.sh build -t GeneratedGuidanceCheck` for generated spec/plan prompt governance.
- `./fake.sh build -t TemplateDrift` for template-owned drift and deferral validation.
- `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit` for graph and synthetic-evidence gates.

Feature-specific targets may be added when they improve failure messages:
`ControlsRuntimeCheck`, `KeyboardInputCheck`, `ControlsBoundaryCheck`,
`ControlsCatalogCheck`, and `ControlsRenderingCheck`.

Template source: `.specify/presets/fsharp-opinionated/templates/tasks-template.md`.

---

## Phase 1: Setup

- [X] T001 Create `specs/011-controls-boundary-refactor/readiness/` placeholders for public surface, package boundary, Elmish adapter, KeyboardInput package, control catalog, control runtime, rich rendering, keyboard input Elmish flow, chart/DataGrid Controls ownership, generated product usage, dependency report, template drift, compatibility impact, evidence graph, and evidence audit
- [X] T002 [P] Inventory current Controls, Charts, KeyboardInput, Elmish, Layout, Scene, SkiaViewer, template, sample, test, package, and surface-baseline assets in `specs/011-controls-boundary-refactor/readiness/package-boundary.md`
- [X] T003 [P] Inventory stale Charts package/capability references, chart-only guidance, DataGrid chart terminology, renderer-neutral controls wording, and generated product copy risks in `specs/011-controls-boundary-refactor/readiness/template-drift.md`
- [X] T004 [P] Inventory command targets and readiness producers that must cover the refactor in `specs/011-controls-boundary-refactor/readiness/dependency-report.md`
- [X] T005 [P] Record Tier 1 scope, affected public contracts, package/capability impact, MVU applicability, unsupported scope, synthetic-evidence policy, and real-evidence obligations in `specs/011-controls-boundary-refactor/readiness/evidence-audit.md`
- [X] T006 [P] Create a traceability matrix mapping FR/SC/contract obligations to tests, implementation files, commands, and readiness artifacts
- [X] T007 Record setup checkpoint evidence and open risks before foundation work begins

**Checkpoint**: Setup ready - foundation tasks may begin.

---

## Phase 2: Foundation

### Tests First

- [X] T008 [P] Add failing public-surface tests requiring curated `.fsi` contracts for Controls, KeyboardInput, and the Elmish adapter, plus package surface baselines and FSI transcript coverage
- [X] T009 [P] Add failing package/capability boundary tests requiring Controls ownership for rich text, charts, graph views, and DataGrid while rejecting active `FS.Skia.UI.Charts`, `charts`, and `src/Lib`/viewer coupling
- [X] T010 [P] Add failing KeyboardInput runtime tests for `Model`, `Msg`, `Effect`, `init`, pure `update`, pressed keys, active layout, mode stack, persistent mode state, pending sequence, focus loss, diagnostics, emitted effects, and state display
- [X] T011 [P] Add failing ControlRuntime tests for product-owned transient focus, hover, pressed, caret/selection, composition, drag, stale target, recovery diagnostics, pure update, and emitted effects
- [X] T012 [P] Add failing Elmish adapter tests for interpreting keyboard/control effects into commands, subscriptions, program wiring, product messages, and diagnostics without moving `Cmd` into base Controls
- [X] T013 [P] Add failing template and generated-guidance tests for one Controls path, no renderer-neutral promise, no chart-only guidance, and generated generic-message plus adapter examples

### Implementation

- [X] T014 Draft or update Controls `.fsi` contracts for stable records, generic event attributes, explicit Skia escape hatches, rich rendering, chart controls, graph views, DataGrid, diagnostics, catalog metadata, and `ControlRuntime`
- [X] T015 Draft or update `FS.Skia.UI.KeyboardInput` `.fsi` contracts for rich runtime state, messages, effects, diagnostics, `init`, pure `update`, state display, and interpreter-facing effect data
- [X] T016 Draft or update the dedicated Elmish adapter `.fsi` surface in `FS.Skia.UI.Controls.Elmish` with keyboard/control effect interpreters, subscriptions, program helpers, and diagnostics
- [X] T017 Define `src/Controls/catalog.yml` schema updates and catalog validation rules for ordinary controls, rich rendering, charts, graph views, DataGrid, accessibility metadata, evidence links, and category diagnostics
- [X] T018 Add or update FSI transcript harnesses, package surface baseline expectations, and public-surface readiness output for the draft Controls, KeyboardInput, and adapter contracts
- [X] T019 Update build and governance wiring plans so `Dev`, `Verify`, `Ci`, `PackLocal`, `PackageSurfaceCheck`, `FsiTranscripts`, `TemplateCheck`, `CapabilityCheck`, `SkillCheck`, `GeneratedProductCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` include this refactor
- [X] T020 Define actionable diagnostics for stale package references, dependency leaks, catalog omissions, unsupported environment conditions, duplicate runtime definitions, stale event targets, and unsupported scope expansion
- [X] T021 Exercise the draft `.fsi` contracts from FSI, including representative `init` / `update` paths and emitted effects for ControlRuntime, KeyboardInput, and the adapter, then record transcript paths in `readiness/public-surface.md`
- [X] T022 Record foundation checkpoint evidence, unresolved decisions, and unsupported-scope diagnostics before story implementation begins

**Checkpoint**: Foundation ready - user-story implementation may begin.

---

## Phase 3: User Story 1 (US1) - Build Skia/Elmish Controls Directly

**Goal**: Product developers can declare ordinary controls, rich text, and advanced Skia rendering through Controls while keeping product state and effect interpretation explicit.

**Independent Test**: A representative product view declares form controls, rich text, a custom Skia escape hatch, ControlRuntime state, KeyboardInput runtime state, and optional Elmish adapter wiring through public contracts and real evidence.

### Tests First

- [X] T023 [P] [US1] Add semantic packed-package or FSI tests for stable record form controls, generic product messages, rich text, custom Skia escape hatches, and deterministic diagnostics
- [X] T024 [P] [US1] Add pure ControlRuntime transition tests and emitted-effect assertions for focus, hover, pressed controls, caret/selection, text composition, drag lifecycle, focus loss, removed controls, cancelled interactions, and stale targets
- [X] T025 [P] [US1] Add pure KeyboardInput transition tests and emitted-effect assertions for key down/up, pressed keys, active layout, active mode stack, persistent mode state, temporary held layers, pending sequence, focus loss, reset, diagnostics, and state display
- [X] T026 [P] [US1] Add Elmish adapter tests proving generic message-based Controls works without `Cmd` and adapter paths translate keyboard/control effects into commands, subscriptions, or program wiring
- [X] T027 [P] [US1] Add rich rendering visual evidence tests for Skia-specific rich text, measurement, drawing, clipping/effects, diagnostics, render/readback or screenshot evidence, and unsupported environment reporting

### Implementation

- [X] T028 [US1] Implement Controls stable records, generic event attributes, diagnostics, accessibility metadata hooks, and message-producing control declarations behind the curated `.fsi` surface
- [X] T029 [US1] Implement rich text/rich rendering declarations and advanced `CustomControl` Skia escape hatches for measurement, drawing, clipping, effects, hit testing, diagnostics, and accessibility metadata
- [X] T030 [US1] Implement product-owned `ControlRuntime` model, messages, pure update, effects, diagnostics, and stale/cancelled recovery helpers without storing product business values
- [X] T031 [US1] Implement the rich `FS.Skia.UI.KeyboardInput` runtime, pure updates, effects, diagnostics, mode behavior, focus recovery, state display, and package-owned public surface
- [X] T032 [US1] Implement the dedicated Elmish adapter interpreters and subscriptions for keyboard/control effects while keeping direct command/program types outside base Controls declarations
- [X] T033 [US1] Connect Controls to the KeyboardInput package and adapter contracts without duplicate runtime definitions, hidden mutable state, or viewer/host-loop ownership
- [X] T034 [US1] Update `samples/ControlsGallery/`, `samples/KeyboardInputGallery/`, and public examples to show stable records, rich rendering, product-owned runtimes, and adapter wiring
- [X] T035 [US1] Capture `readiness/public-surface.md`, `readiness/control-runtime.md`, `readiness/keyboardinput-package.md`, `readiness/keyboard-input-elmish.md`, `readiness/rich-rendering.md`, and `readiness/elmish-adapter.md` with command results and evidence paths
- [X] T036 [US1] Document the US1 independent validation path and remaining unsupported environment conditions

**Checkpoint**: US1 is independently functional and testable.

---

## Phase 4: User Story 2 (US2) - Use Charts and DataGrid as Controls

**Goal**: Charts, graph views, and DataGrid are discoverable, configurable, validated, and documented under Controls ownership without selecting a legacy Charts capability.

**Independent Test**: Controls catalog, package surface, examples, generated guidance, and samples expose chart, graph, and DataGrid entries through Controls; DataGrid is categorized as data or collection.

### Tests First

- [X] T037 [P] [US2] Add catalog contract tests requiring chart, graph, and DataGrid rows under Controls with category, required attributes, supported states, interaction metadata, accessibility metadata, examples, tests, and evidence links
- [X] T038 [P] [US2] Add public API and FSI tests proving chart, graph, and DataGrid authoring works through `FS.Skia.UI.Controls` without `FS.Skia.UI.Charts`
- [X] T039 [P] [US2] Add package, capability, generated product, and surface-baseline tests rejecting active Charts package/project/capability references and chart-specific generated skills
- [X] T040 [P] [US2] Add DataGrid large-row tests for 10,000 items, visible-range behavior, selection/focus interaction, bounded scene nodes, observed durations, and diagnostics
- [X] T041 [P] [US2] Add sample and generated-product composition tests combining form inputs, a chart, and a DataGrid through Controls only

### Implementation

- [X] T042 [US2] Move or adapt chart and graph public contracts, implementations, tests, examples, diagnostics, and catalog ownership into Controls modules
- [X] T043 [US2] Add or update `DataGrid.fsi` / `DataGrid.fs` under Controls as a data or collection control with product-owned data, selection, focus, sort/filter metadata, visible range, cell rendering, accessibility role, and diagnostics
- [X] T044 [US2] Remove or deactivate the legacy Charts package/project, active `charts` capability, generated package references, chart-specific skill, and chart surface-baseline participation while preserving migration documentation
- [X] T045 [US2] Populate `src/Controls/catalog.yml` and documentation with Controls-owned chart, graph, and DataGrid entries, including evidence links and DataGrid data/collection categorization
- [X] T046 [US2] Update `samples/ControlsGallery/`, `samples/DataGridGallery/`, and any chart gallery references so chart, graph, and DataGrid usage is Controls-owned
- [X] T047 [US2] Refresh package surface baselines and FSI transcripts for Controls-owned chart, graph, and DataGrid contracts and the removed Charts package surface
- [X] T048 [US2] Capture `readiness/control-catalog.md`, `readiness/chart-datagrid-controls.md`, `readiness/package-boundary.md`, and `readiness/compatibility-impact.md` with command results, stale-reference scans, and migration notes
- [X] T049 [US2] Document the US2 independent validation path and DataGrid category evidence

**Checkpoint**: US2 is independently functional and testable.

---

## Phase 5: User Story 3 (US3) - Generate Coherent Product Guidance

**Goal**: Generated product profiles describe one Controls path for forms, rich text, charts, graph views, and data controls.

**Independent Test**: Generated products with Controls include Controls package references, product-owned examples, generic message-based flow, adapter flow when Elmish integration is selected, and no stale Charts guidance or copied framework implementation source.

### Tests First

- [X] T050 [P] [US3] Add generated guidance tests rejecting stale chart-only active capability references, DataGrid-as-chart wording, renderer-neutral controls promises, host-loop ownership requirements, and missing Charts migration guidance
- [X] T051 [P] [US3] Add generated product tests requiring Controls package references, no `FS.Skia.UI.Charts`, form plus chart/DataGrid usage, product-owned source, product tests, and no copied framework samples/specs/readiness/docs/implementation projects
- [X] T052 [P] [US3] Add capability/profile/template drift tests requiring Controls as the active home for ordinary controls, rich text, charts, graph views, and DataGrid across generated profiles
- [X] T053 [P] [US3] Add generated adapter guidance tests requiring generic message-based Controls examples and Elmish adapter examples when program integration is selected

### Implementation

- [X] T054 [US3] Update `template/capabilities.yml`, profiles, controls fragments, keyboard-input fragments, Elmish fragments, and package references so generated products select Controls as the single controls authoring path
- [X] T055 [US3] Update generated local skills, generated spec/plan guidance, README/docs fragments, and migration notes to remove stale Charts guidance and renderer-neutral controls wording
- [X] T056 [US3] Add product-owned generated examples for ordinary form controls, rich text or rich rendering, chart or graph controls, DataGrid, generic message flow, and Elmish adapter integration
- [X] T057 [US3] Update `TemplateCheck`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, and `TemplateDrift` outputs and diagnostics for Controls ownership, copied-asset exclusions, stale references, and adapter guidance
- [X] T058 [US3] Capture `readiness/generated-product-usage.md`, `readiness/template-drift.md`, and generated guidance evidence with generated profile names, file paths, package references, stale pattern scans, and command results
- [X] T059 [US3] Document the US3 independent validation path for generated product consumers

**Checkpoint**: US3 is independently functional and testable.

---

## Phase 6: User Story 4 (US4) - Validate the Boundary as a Maintainer

**Goal**: Public contracts, package boundaries, examples, dependency reports, generated guidance, and readiness evidence make the refactored Controls boundary auditable.

**Independent Test**: Governance checks report public surface, package contents, generated guidance, dependency impact, compatibility impact, visual/control evidence, and actionable failures for stale or unsupported references.

### Tests First

- [X] T060 [P] [US4] Add package surface and governance tests for Controls, KeyboardInput, the Elmish adapter, removed Charts baseline participation, `.fsi` ownership, and no public visibility keywords as contract substitutes
- [X] T061 [P] [US4] Add dependency report tests proving Controls depends only on allowed direct packages, has no hidden `src/Lib`/viewer/runtime coupling, KeyboardInput owns one rich input runtime, and adapter dependency placement is explicit
- [X] T062 [P] [US4] Add command-contract tests requiring `Dev`, `Verify`, `Ci`, `PackLocal`, `PackageSurfaceCheck`, `FsiTranscripts`, `TemplateCheck`, `CapabilityCheck`, `SkillCheck`, `GeneratedProductCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` to include the refactor
- [X] T063 [P] [US4] Add compatibility and documentation tests requiring a Charts replacement path, no compatibility shim promise, no automated migration promise, no release publishing promise, and preserved lower-level Scene/Layout/KeyboardInput/SkiaViewer/Elmish paths
- [X] T064 [P] [US4] Add failure-diagnostic tests requiring stale reference, package, capability, control, catalog entry, generated profile, adapter contract, runtime state, unsupported environment, or migration gap names in validation failures

### Implementation

- [X] T065 [US4] Update `build.fsx`, scripts, and command target wiring so all governed targets produce or consume this feature's boundary evidence and actionable diagnostics
- [X] T066 [US4] Update `Directory.Packages.props`, project references, package metadata, docs/dependency references, and dependency report generation for Controls, KeyboardInput, adapter, and Charts removal
- [X] T067 [US4] Refresh surface baselines only for intentional public changes and remove Charts package baseline participation from active package surface checks
- [X] T068 [US4] Update docs and compatibility guidance to explain Controls versus lower-level Scene/Layout/KeyboardInput/SkiaViewer/Elmish paths and the supported Charts replacement path
- [X] T069 [US4] Update governance, package, smoke, and sample tests to cover boundary evidence, generated guidance, dependency impact, compatibility impact, and lower-level path preservation
- [X] T070 [US4] Generate validation roots and run generated product source/package checks proving Controls usage and absence of copied framework implementation source
- [X] T071 [US4] Capture `readiness/public-surface.md`, `readiness/package-boundary.md`, `readiness/dependency-report.md`, `readiness/template-drift.md`, `readiness/compatibility-impact.md`, and command logs with pass/fail verdicts
- [X] T072 [US4] Document the US4 independent validation path and maintainer review checklist

**Checkpoint**: US4 is independently functional and testable.

---

## Phase 7: Integration & Polish

- [X] T073 Run `./fake.sh build -t Dev` and focused Controls, KeyboardInput, Elmish adapter, package, and governance tests; update readiness reports with commands, durations, and failures
- [X] T074 Run `./fake.sh build -t PackLocal`, `./fake.sh build -t PackageSurfaceCheck`, and `./fake.sh build -t FsiTranscripts`; run `./fake.sh build -t RefreshSurfaceBaselines` only for intentional public changes and record approved baseline diffs
- [X] T075 Run `./fake.sh build -t Verify` plus `ControlsRuntimeCheck`, `KeyboardInputCheck`, and `ControlsBoundaryCheck` if split targets exist; update runtime and adapter evidence
- [X] T076 Run `ControlsCatalogCheck` and `ControlsRenderingCheck` if split targets exist, or equivalent `Verify` coverage; update catalog, rich rendering, chart/DataGrid, and unsupported environment evidence
- [X] T077 Run `./fake.sh build -t CapabilityCheck`, `./fake.sh build -t SkillCheck`, and `./fake.sh build -t DependencyReport`; update package/capability, skill, and dependency readiness reports
- [X] T078 Run `./fake.sh build -t TemplateCheck`, `./fake.sh build -t GeneratedProductCheck`, `./fake.sh build -t GeneratedGuidanceCheck`, and `./fake.sh build -t TemplateDrift`; update generated product and template evidence
- [X] T079 Run generated product `Dev`, `Test`, and `Verify` commands for representative Controls selections; store logs and generated file/package-reference inventories
- [X] T080 Run `./fake.sh build -t Verify` and `./fake.sh build -t Ci`; record final command verdicts and any environment-specific skips or failures
- [X] T081 Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/011-controls-boundary-refactor --graph-only` and update or link `readiness/evidence-graph.md`
- [X] T082 Run `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`; document PASS or every unresolved synthetic-evidence or diff-scan blocker
- [X] T083 Review the Synthetic-Evidence Inventory and ensure no `[S]` or propagated `[S*]` task remains without a documented real-evidence replacement or accepted override
- [X] T084 Search active source, templates, generated output, docs, skills, and readiness reports for stale Charts package/capability references, renderer-neutral controls promises, chart-only DataGrid wording, hidden host-loop coupling, and copied framework assets
- [X] T085 Produce the final readiness summary tying requirements, success criteria, contracts, commands, public-surface baselines, generated product obligations, compatibility impact, and evidence reports to completed tasks

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| _(none yet)_ | | | |

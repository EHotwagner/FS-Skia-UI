# Tasks: Skia Controls Library

**Feature branch**: `010-skia-controls-library`
**Spec**: `specs/010-skia-controls-library/spec.md`
**Plan**: `specs/010-skia-controls-library/plan.md`

## Status Legend

- `[ ]` - pending
- `[X]` - done with real evidence
- `[S]` - done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` - failed
- `[-]` - skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

## Vertical-slice Rule (US Phases)

A task tagged `[US*]` may only be marked `[X]` when the change is reachable
from a user-facing entry point and that path was actually exercised: an FSI
session against the packed library, a smoke run of the application, a manual
walk-through with transcript, or screenshot/render evidence captured under
`readiness/`. Domain, model, or core-layer changes alone do not satisfy `[X]`
for a `[US*]` task. If the user-reachable surface is missing, stubbed, or not
yet wired, mark `[ ]` or `[S]` with disclosure in the Synthetic-Evidence
Inventory, never `[X]`.

For stateful or I/O-bearing stories, `[X]` also requires Elmish/MVU evidence:
the public `Model` / `Msg` / `Effect` or `Cmd<Msg>` contract was exercised,
pure `update` transitions were tested, emitted effects were asserted, and the
effect interpreter was run against real dependencies where safe.

This rule does not apply to Setup, Foundation, Integration, or Polish phase
tasks; those are evaluated against their own phase verification.

## Task Annotations

- **[P]** - parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, ... - user-story scope
- **[T1]** / **[T2]** - Tier 1 (contracted) vs Tier 2 (internal) change

Every task must have a matching entry in `tasks.deps.yml` even if its
dependency list is empty. The `speckit.evidence.graph` command refuses to
proceed with dangling references.

The feature is classified as Tier 1 overall, so task lines omit `[T1]` unless a
specific task differs from that classification.

## Canonical Verification Targets

Use repository targets instead of duplicating raw restore/build/test/package
command order:

- `./fake.sh build -t Dev` for fast local verification.
- `./fake.sh build -t Verify` for the full governed workflow.
- `./fake.sh build -t PackLocal` for local package output.
- `./fake.sh build -t RefreshSurfaceBaselines` for intentional current surface baseline refreshes.
- `./fake.sh build -t PackageSurfaceCheck` for package surface review.
- `./fake.sh build -t FsiTranscripts` for public FSI evidence.
- `./fake.sh build -t SampleContractSmoke` for sample smoke evidence.
- `./fake.sh build -t TemplateCheck` for source/package default/minimal generated project validation.
- `./fake.sh build -t CapabilityCheck` for generated capability catalog validation.
- `./fake.sh build -t SkillCheck` for local and generated skill validation.
- `./fake.sh build -t GeneratedProductCheck` for generated product validation.
- `./fake.sh build -t DependencyReport` for central package governance.
- `./fake.sh build -t GeneratedGuidanceCheck` for generated spec/plan prompt governance.
- `./fake.sh build -t TemplateDrift` for template-owned drift and deferral validation.
- `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit` for graph and synthetic-evidence gates.

Controls-specific targets may be added when they improve failure messages:
`ControlsCatalogCheck`, `ControlsInteractionCheck`, and `ControlsRenderingCheck`.

Template source: `.specify/presets/fsharp-opinionated/templates/tasks-template.md`.

---

## Phase 1: Setup

- [X] T001 Create `specs/010-skia-controls-library/readiness/` with placeholders for control catalog, public surface, semantic tests, interaction tests, layout/rendering, generated product usage, local skills, dependency report, generated guidance, template drift, evidence graph, evidence audit, and compatibility impact
- [X] T002 [P] Inventory existing Charts package, chart samples, chart template fragment, chart skill, generated profile references, and surface baselines in `specs/010-skia-controls-library/readiness/compatibility-impact.md`
- [X] T003 [P] Inventory current build targets, generated product validation, template profiles, package outputs, and readiness report paths that Controls must join in `specs/010-skia-controls-library/readiness/template-drift.md`
- [X] T004 [P] Record Elmish/MVU applicability for Controls authoring, reference gallery, generated product example, text/clipboard/environment edges, and validation runners in `specs/010-skia-controls-library/readiness/semantic-tests.md`
- [X] T005 [P] Record the dependency baseline for Scene, Layout, KeyboardInput, SkiaViewer, Elmish, Charts, central package versions, and the planned `FS.Skia.UI.Controls` package in `specs/010-skia-controls-library/readiness/dependency-report.md`
- [X] T006 Record setup evidence obligations, unsupported scope, real-evidence paths, and initial synthetic-evidence policy in `specs/010-skia-controls-library/readiness/evidence-audit.md`

**Checkpoint**: Setup ready - foundation tasks may begin.

---

## Phase 2: Foundation

- [X] T007 Add `src/Controls/Controls.fsproj` with `FS.Skia.UI.Controls` package metadata and wire it into repository package/build discovery
- [X] T008 [P] Draft public `.fsi` contracts for `Types`, `Control`, `Attributes`, `Theme`, `Accessibility`, `Diagnostics`, `Catalog`, `TextInput`, `Collections`, `Charts`, and `CustomControl`
- [X] T009 [P] Draft the reference gallery MVU contract with `Model`, `Msg`, `Effect` or `Cmd<Msg>`, `init`, pure `update`, and interpreter boundary
- [X] T010 [P] Draft the generated product controls example MVU contract with `Model`, `Msg`, `Effect` or `Cmd<Msg>`, `init`, pure `update`, and interpreter boundary
- [X] T011 [P] Define the structured `src/Controls/catalog.yml` schema and planned supported catalog rows across display, input, selection, navigation, layout, feedback, data, chart, graph, and custom categories
- [X] T012 [P] Add `tests/Controls.Tests/Controls.Tests.fsproj` with empty test modules for catalog, public surface, semantic behavior, interaction, text input, accessibility, and rendering coverage
- [X] T013 [P] Add an FSI transcript harness for the packed or prelude-loaded Controls public surface
- [X] T014 [P] Add the package surface baseline path `readiness/surface-baselines/FS.Skia.UI.Controls.txt` and baseline refresh expectations
- [X] T015 [P] Update repository package/build inventory so Controls contracts, tests, samples, and local package output are discoverable by existing FAKE targets
- [X] T016 [P] Define structured diagnostics for missing attributes, unsupported state combinations, missing stable keys, hit-test failures, layout conflicts, missing accessibility metadata, contrast failures, and unsupported environments
- [X] T017 [P] Define the runtime boundary between Controls, Scene, Layout, KeyboardInput, SkiaViewer, and Elmish so persistent state remains model-owned and only transient interaction state is retained
- [X] T018 [P] Add failing-first governance tests for Controls default capability inclusion, generated product package references, and Charts removal from active selection
- [X] T019 [P] Add failing-first skill governance tests for `fs-skia-ui-widgets` selection and stale `fs-skia-charts` or generated `fs-skia-layout` exclusion
- [X] T020 Exercise the draft `.fsi` contracts from FSI and capture the session transcript in `specs/010-skia-controls-library/readiness/public-surface.md`
- [X] T021 Record foundation readiness, unsupported-scope diagnostics, and command-surface obligations before story implementation begins

**Checkpoint**: Foundation ready - user-story implementation may begin.

---

## Phase 3: User Story 1 (US1) - Compose Screens in the View Function

### Tests First

- [X] T022 [P] [US1] Add semantic tests that load the packed library or prelude and construct a representative counter/form screen through `Control<'msg>` and an Elmish-style view function
- [X] T023 [P] [US1] Add pure MVU transition tests for the representative screen `Model`, `Msg`, `update`, and emitted `Effect` or `Cmd<Msg>` values
- [X] T024 [P] [US1] Add interaction dispatch tests for pointer activation, keyboard activation, disabled/read-only suppression, exactly-once messages, and stale handler prevention after model changes
- [X] T025 [P] [US1] Add a real interpreter or smoke-run evidence path for the representative screen through the viewer/input edge, with unsupported GPU, font, clipboard, text-input, IME, or window diagnostics recorded when applicable

### Implementation

- [X] T026 [US1] Implement the core typed DSL: `Control<'msg>`, `Attr<'msg>`, `ControlId`, `ControlEvent`, `ControlDiagnostic`, `Control.withKey`, `Control.render`, and `Control.diagnostics`
- [X] T027 [US1] Implement content and children composition, stable child ordering, keyed control identity, and key-collision diagnostics
- [X] T028 [US1] Implement representative view-function controls for text display, button activation, editable text, toggle/checkbox state, and stack/panel composition
- [X] T029 [US1] Implement model-owned state reflection for displayed values, enabled state, visibility, validation, focus indicators, selection states, hover states, pressed states, and loading states
- [X] T030 [US1] Implement message-oriented event mapping so tested actions dispatch the current-view message exactly once without app-owned widget lifecycle objects
- [X] T031 [US1] Implement keyed transient interaction state for hover, pressed, focus, caret, active drag, and in-progress composition without storing durable application values
- [X] T032 [US1] Connect Controls render, layout, hit-test, focus, and keyboard input output to Scene, Layout, and KeyboardInput boundaries for the representative screen
- [X] T033 [US1] Capture `readiness/semantic-tests.md` and `readiness/interaction-tests.md` evidence for US1, including pure update assertions, emitted-effect assertions, and real interpreter evidence where safe
- [X] T034 [US1] Document the US1 independent validation path and concise authoring example in Controls documentation or readiness notes

**Checkpoint**: US1 is independently functional and testable.

---

## Phase 4: User Story 2 (US2) - Use a Comprehensive Control Catalog

### Tests First

- [X] T035 [P] [US2] Add catalog contract tests requiring at least 30 supported controls or variants with purpose, attributes, events where applicable, visual states, accessibility metadata, examples, tests, evidence, and `.fsi` members
- [X] T036 [P] [US2] Add reference gallery rendering tests covering every supported row, common states, three viewport sizes, and two DPI scale factors
- [X] T037 [P] [US2] Add accessibility validation tests for role, accessible name source, state metadata, focus order, keyboard operation, and contrast evidence
- [X] T038 [P] [US2] Add large data control tests for 10,000 items, bounded visible range, recorded visible-range recalculation threshold, observed duration evidence, empty state, selection, and item update behavior
- [X] T039 [P] [US2] Add chart and graph ownership tests proving catalog rows, generated examples, public modules, tests, and evidence are Controls-owned and not active Charts capability artifacts
- [X] T105 [P] [US2] Draft the text-input and collection-control MVU contracts with `Model`, `Msg`, `Effect` or `Cmd<Msg>`, `init`, pure `update`, and interpreter boundaries for clipboard, IME/composition, focus, and scrolling effects
- [X] T106 [P] [US2] Add pure transition tests for text input, selection, validation, focus traversal, clipboard requests, IME/composition diagnostics, and large-data viewport updates
- [X] T107 [P] [US2] Add emitted-effect assertions and real interpreter evidence for clipboard, text-input, IME/composition, focus, and scrolling effects where safe

### Implementation

- [X] T040 [US2] Populate `src/Controls/catalog.yml` with supported controls or variants across display, input, selection, navigation, layout, feedback, data, chart, graph, and custom categories
- [X] T041 [US2] Implement supported display, input, selection, navigation, layout, and feedback controls declared by the catalog
- [X] T042 [US2] Implement plain single-line and multi-line text entry with cursor movement, selection, clipboard commands, validation feedback, committed value changes, cancellation or rejection of invalid input, and IME/composition diagnostics
- [X] T043 [US2] Implement list and table-like controls with bounded rendering or visible-range behavior for 10,000 items, threshold-recorded visible-range recalculation, scrolling, empty state, single and multiple selection, and item updates
- [X] T044 [US2] Move or adapt chart and graph controls into Controls public modules, examples, tests, generated guidance, and evidence
- [X] T045 [US2] Build `samples/ControlsGallery/` as the reference gallery that renders every supported catalog row and common visual state
- [X] T046 [US2] Implement accessibility metadata, focus traversal, keyboard operation, and contrast validation for supported interactive controls
- [X] T047 [US2] Implement `ControlsCatalogCheck` or equivalent `Verify` coverage that fails on missing catalog fields, examples, tests, evidence, accessibility metadata, `.fsi` members, or stale Charts ownership
- [X] T048 [US2] Implement `ControlsInteractionCheck` and `ControlsRenderingCheck` or equivalent `Verify` coverage for interaction dispatch, text entry, large data, gallery rendering, viewport/DPI evidence, and environment diagnostics
- [X] T049 [US2] Capture `readiness/control-catalog.md` and `readiness/layout-rendering.md` with supported row counts, viewport/DPI results, item counts, visible-range thresholds, observed durations, accessibility findings, and environment diagnostics
- [X] T050 [US2] Document the US2 independent catalog validation path
- [-] T108 [US2] Run and record the first-time evaluator catalog walkthrough for the simple form task, including participant count, success count, failures, and documentation improvements (skipped: requires five external first-time evaluators unavailable in this workspace; deferred to release-readiness review)

**Checkpoint**: US2 is independently functional and testable.

---

## Phase 5: User Story 3 (US3) - Configure Controls Declaratively

### Tests First

- [X] T051 [P] [US3] Add tests that compose five unrelated controls from different categories using the same creation, value, children, layout, style, validation, accessibility, and event patterns
- [X] T052 [P] [US3] Add tests for duplicate attributes, missing required attributes, invalid combinations, documented precedence, and deterministic diagnostics
- [X] T053 [P] [US3] Add theme, style, and layout override tests across different containers and model updates

### Implementation

- [X] T054 [US3] Implement common attribute groups for content, children, layout, styling, theme, state, validation, accessibility, and message-oriented events
- [X] T055 [US3] Normalize module names and `create : Attr<'msg> list -> Control<'msg>` signatures across supported catalog controls
- [X] T056 [US3] Implement application-level themes, per-control overrides, density, typography, fills, strokes, corner treatment, state variants, and contrast policy hooks
- [X] T057 [US3] Implement validation diagnostics for missing attributes, duplicate or conflicting attributes, unsupported state combinations, missing stable keys, layout conflicts, and accessibility gaps
- [X] T058 [US3] Update catalog metadata, docs, and examples to demonstrate consistent declarative attribute patterns without special-case wiring
- [X] T059 [US3] Exercise a five-control declarative configuration screen from FSI against the packed or prelude-loaded surface and capture the transcript in `readiness/public-surface.md`
- [X] T060 [US3] Capture `readiness/semantic-tests.md` evidence for declarative configuration, theme overrides, diagnostics, and model-driven updates
- [X] T061 [US3] Document the US3 independent validation path

**Checkpoint**: US3 is independently functional and testable.

---

## Phase 6: User Story 4 (US4) - Maintain Quality Through Tests and Examples

### Tests First

- [X] T062 [P] [US4] Add package surface tests for Controls `.fsi` coverage, public baseline drift, and removed chart member compatibility records
- [X] T063 [P] [US4] Add generated product tests for default Controls capability inclusion, package references, product-owned example view, product test coverage, and no copied framework samples or implementation projects
- [X] T064 [P] [US4] Add capability and template tests rejecting active `charts` capability selection, `FS.Skia.UI.Charts` default references, chart fragments, and `fs-skia-charts` generated skills
- [X] T065 [P] [US4] Add skill validation tests for `fs-skia-ui-widgets` required sections, generated selection, stale generated layout-control skill exclusion, and related skill redirects
- [X] T066 [P] [US4] Add dependency governance tests proving Controls dependencies are declared, Layout remains a separate runtime capability, and no unexpected dependency leaks are introduced
- [X] T067 [P] [US4] Add generated guidance and template drift tests for controls guidance, chart/graph replacement guidance, stale Charts references, stale generated Layout skill references, and unsupported copied assets

### Implementation

- [X] T068 [US4] Update `template/capabilities.yml` and profiles so the default app resolves to Scene, SkiaViewer, Elmish, KeyboardInput, Layout, and Controls while Charts is no longer active
- [X] T069 [US4] Add `template/fragments/controls/` with Controls package reference guidance, concise product-owned example view, product test coverage, and widgets skill selection
- [X] T070 [US4] Add `src/Controls/skill/SKILL.md` and generated `fs-skia-ui-widgets` content with Scope, Public Contract, Build Commands, Test Commands, Evidence, Package Boundary, and Generated Product sections
- [X] T071 [US4] Update Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Testing, and project skills to route widget, control, chart, and graph authoring to `fs-skia-ui-widgets` where applicable
- [X] T072 [US4] Remove or deactivate Charts package, capability, template fragment, generated package reference, and chart-specific skill from active generated product selection while preserving compatibility notes where needed
- [X] T073 [US4] Update `build.fsx` command surface so `Dev`, `Verify`, `Ci`, `PackLocal`, `PackageSurfaceCheck`, `FsiTranscripts`, `CapabilityCheck`, `SkillCheck`, `GeneratedProductCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` include Controls validation or evidence
- [X] T074 [US4] Update `Directory.Packages.props`, package metadata, dependency documentation, and dependency reports for Controls ownership, Layout dependency behavior, and Charts removal from active defaults
- [X] T075 [US4] Update governance, package, smoke, and surface-baseline tests for the Controls package and generated product behavior
- [X] T076 [US4] Generate source/package validation roots and prove the default product builds with a product-owned Controls example and without copied framework samples, galleries, specs, readiness evidence, docs, README copy, or implementation projects
- [X] T077 [US4] Capture `readiness/generated-product-usage.md`, `readiness/local-skills.md`, `readiness/dependency-report.md`, `readiness/generated-guidance.md`, and `readiness/template-drift.md`
- [X] T078 [US4] Capture `readiness/public-surface.md` for Controls package surface review, FSI transcripts, and surface baseline status
- [X] T079 [US4] Capture `readiness/compatibility-impact.md` for Charts removal, Controls replacement paths, lower-level API composition, in-scope compatibility work, and out-of-scope migration or release automation
- [X] T080 [US4] Document the US4 independent validation path

**Checkpoint**: US4 is independently functional and testable.

---

## Phase 7: User Story 5 (US5) - Extend With Custom Controls

### Tests First

- [X] T081 [P] [US5] Add semantic tests for the `CustomControl` public API through `.fsi`, including render, layout, hit-test, event, accessibility, and diagnostics hooks
- [X] T082 [P] [US5] Add interaction, layout, and rendering tests for a custom control wrapper placed beside built-in controls in a reference screen
- [X] T083 [P] [US5] Add diagnostics tests proving missing layout, input, accessibility, or diagnostic metadata fails validation before the wrapper is treated as supported

### Implementation

- [X] T084 [US5] Implement `CustomControl.fsi` and `CustomControl.fs` wrapper APIs for render, layout, hit-test, event mapping, accessibility metadata, diagnostics, and supported state
- [X] T085 [US5] Integrate custom controls with the control tree, layout/render/input pipeline, catalog diagnostics, and test diagnostics
- [X] T086 [US5] Add custom control wrapper catalog row and ControlsGallery example beside built-in controls
- [X] T087 [US5] Capture custom wrapper evidence in `readiness/semantic-tests.md`, `readiness/interaction-tests.md`, and `readiness/layout-rendering.md`
- [X] T088 [US5] Document the US5 independent validation path and extension guidance

**Checkpoint**: US5 is independently functional and testable.

---

## Phase 8: Integration & Polish

- [X] T089 Run `./fake.sh build -t Dev` and targeted Controls tests, then update semantic and interaction readiness reports with commands, durations, and failures
- [X] T090 Run `./fake.sh build -t CapabilityCheck`, `./fake.sh build -t SkillCheck`, and `./fake.sh build -t DependencyReport`, then update generated capability, skills, and dependency readiness reports
- [X] T091 Run `./fake.sh build -t PackLocal`, `./fake.sh build -t PackageSurfaceCheck`, and `./fake.sh build -t FsiTranscripts`, then update public surface evidence
- [X] T092 Run `./fake.sh build -t ControlsCatalogCheck`, `./fake.sh build -t ControlsInteractionCheck`, and `./fake.sh build -t ControlsRenderingCheck` or the equivalent `Verify` coverage, then update catalog, interaction, text, accessibility, and layout/rendering evidence
- [X] T093 Run `./fake.sh build -t TemplateCheck` and `./fake.sh build -t GeneratedProductCheck`, then update generated product evidence
- [X] T094 Run `./fake.sh build -t GeneratedGuidanceCheck` and `./fake.sh build -t TemplateDrift`, then update generated guidance and template drift evidence
- [X] T095 Run generated product `Dev`, `Test`, and `Verify` commands in validation roots and record product-owned Controls example evidence
- [X] T096 Run `./fake.sh build -t Verify` and `./fake.sh build -t Ci`, then record final command verdicts and any environment-specific skips or failures
- [X] T097 Run `./fake.sh build -t RefreshSurfaceBaselines` only for intentional surface changes and record the approved baseline diff
- [X] T098 Update `docs/controls.md`, package notes, generated quickstart guidance, timed walkthrough guidance, and compatibility notes with the final supported Controls catalog and deferred scope
- [X] T099 Review active generated output and repository reports for stale Charts capability/package/template/skill references, stale generated Layout widget skill references, and copied framework sample/gallery/spec/readiness assets
- [X] T100 Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/010-skia-controls-library --graph-only` and update or link `readiness/evidence-graph.md`
- [X] T101 Run `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`; document PASS or every `--accept-synthetic` override with justification
- [X] T102 Review the Synthetic-Evidence Inventory and ensure no `[S]` or propagated `[S*]` task remains without documented real-evidence replacement or accepted override
- [X] T103 Review compatibility impact, dependency impact, unsupported scope, and deferred V2 migration/release automation boundaries before sign-off
- [X] T109 Run a timed form-and-dashboard walkthrough using catalog documentation, covering at least 10 controls, 3 nested layout regions, and 5 interactions within the 30-minute SC-001 target
- [X] T104 Produce the final readiness summary tying all feature requirements, success criteria, contracts, generated product obligations, and evidence reports to completed tasks

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| _(none yet)_ | | | |

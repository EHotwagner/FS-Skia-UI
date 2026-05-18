# Tasks: Tetris Demo Integration Improvements

**Feature branch**: `013-tetris-demo-integration`
**Spec**: `specs/013-tetris-demo-integration/spec.md`
**Plan**: `specs/013-tetris-demo-integration/plan.md`

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
from a user-facing entry point and that path was actually exercised: packed
library or FSI transcript, generated product test, bounded viewer smoke,
headless scene evidence, local consumer workflow, or readiness transcript under
`specs/013-tetris-demo-integration/readiness/`. Domain, model, or core-layer
tests alone do not satisfy `[X]` for a `[US*]` task.

For stateful or I/O-bearing stories, `[X]` also requires Elmish/MVU evidence:
the public or internal `Model`, `Msg`, `Effect` or `Cmd<Msg>` contract was
exercised, pure `update` transitions were tested, emitted effects were
asserted, and the interpreter boundary was run against real dependencies where
safe. This applies to viewer lifecycle, bounded run behavior, generated app
host flow, diagnostic capture, generated consumer validation, and local package
reporting.

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
- `./fake.sh build -t PackLocal`
- `./fake.sh build -t RefreshSurfaceBaselines`
- `./fake.sh build -t PackageSurfaceCheck`
- `./fake.sh build -t FsiTranscripts`
- `./fake.sh build -t TemplateCheck`
- `./fake.sh build -t GeneratedProductCheck`
- `./fake.sh build -t GeneratedGuidanceCheck`
- `./fake.sh build -t TemplateDrift`
- `./fake.sh build -t DependencyReport`
- `./fake.sh build -t EvidenceGraph`
- `./fake.sh build -t EvidenceAudit`

---

## Phase 1: Setup

- [X] T001 Create readiness scaffolding under `specs/013-tetris-demo-integration/readiness/`, including `logs/` and placeholders for normalized input, bounded smoke, diagnostics, headless scene evidence, generated template flows, local consumer packages, generated consumer validation, evidence graph, and evidence audit.
- [X] T002 [P] Confirm `spec.md`, `plan.md`, `data-model.md`, `quickstart.md`, and all contracts describe the same Tetris demo integration scope and no Tetris-specific game-rule changes.
- [X] T003 [P] Record Tier 1 classification, affected packages/modules, public `.fsi` impact, generated template impact, command-surface impact, and package identity stability constraints in setup readiness notes.
- [X] T004 [P] Record synthetic-evidence policy: synthetic fixtures may cover forced pre-frame failures, unsupported host classification, stale package feeds, scanner inputs, and deterministic non-window scenes, but final readiness needs real public-surface or generated-product evidence where supported.
- [X] T005 Consolidate setup notes into readiness scaffolds and list any missing prerequisite artifacts before foundation work starts.

**Checkpoint**: Setup complete — foundation work may begin.

---

## Phase 2: Foundation

- [X] T006 [P] Draft `.fsi` contracts for normalized viewer input in `src/KeyboardInput/` or `src/SkiaViewer/`, including `ViewerKey`, key down/up conversion, alternate raw-name handling, and unknown-key preservation.
- [X] T007 [P] Draft `.fsi` contracts for bounded viewer smoke and diagnostics in `src/SkiaViewer/`, including `ViewerRunRequest`, `ViewerRunEvidence`, `ViewerRunFailure`, diagnostic level/category/sampling, capturable sink, and MVU-shaped run model/message/effect/interpreter boundary.
- [X] T008 [P] Draft `.fsi` contracts for deterministic scene evidence in `src/Scene/` or `src/Testing/`, including scene evidence request/result, renderer mode, output format, unsupported-environment failure, and non-window guarantee.
- [X] T009 [P] Draft generated app host or template-facing contracts for normalized input, app lifecycle, ticking, diagnostics, bounded smoke, and optional lower-level viewer escape hatches.
- [X] T010 [P] Draft local consumer package report and generated validation workflow contracts in build/testing surfaces, including package identities, versions, feed path, snippets, restore command, drift diagnostics, and validation result categories.
- [X] T011 Add failing public-surface/semantic tests that compile against the drafted signatures and exercise representative `init`, `update`, emitted effects, and interpreter boundaries for stateful or I/O-bearing workflows.
- [X] T012 Add or update shared diagnostics and failure classification fixtures for blocked rendering stages, unsupported host capabilities, product defects, setup drift, app flow names, screens, input values, package identities, and evidence paths.
- [X] T013 Record FSI or packed-library transcript expectations for the new public surfaces and initial surface-area baseline expectations for all changed packages.
- [X] T014 Update foundation guidance in `docs/build.md`, `docs/evidence.md`, `docs/generated-apps.md`, and `docs/dependencies.md` so readiness, public contracts, local package feeds, generated validation, and unsupported-host diagnostics are named before story implementation.
- [X] T015 Verify foundation with focused signature, semantic, and pure `update` tests, then capture foundation notes under `readiness/logs/`.

**Checkpoint**: Foundation ready — user story implementation may begin in parallel.

---

## Phase 3: User Story 1 - Start and Control a Generated Graphical App

### Tests First

- [X] T016 [P] [US1] Add failing normalized input tests for arrows, enter, space, escape, backspace, letters, digits, function keys, common alternate raw names, unknown raw keys, key-down events, and key-up events.
- [X] T017 [P] [US1] Add failing generated template tests proving initial screen start, options navigation, primary interaction, pause/back where present, and end-screen restart through viewer key events rather than raw string comparisons.
- [X] T018 [P] [US1] Add pure generated-app transition tests for user-reachable screens, including MVU `Model`, `Msg`, `Effect` or `Cmd<Msg>`, `init`, `update`, and emitted-effect assertions for input-driven flows.

### Implementation

- [X] T019 [US1] Implement normalized viewer key values and raw-name normalization with documented alternate mappings, unknown-key preservation, and down/up conversion.
- [X] T020 [US1] Wire viewer keyboard events into generated graphical app input messages without backend-specific raw string comparisons in generated app code, including the optional generated app-host convenience path for initialization, update, view/scene production, normalized key mapping, ticking, diagnostics, bounded smoke, and lower-level viewer escape hatches.
- [X] T021 [US1] Extend generated graphical template flows and fixtures for initial, options, main interaction, pause/back, and restart/exit screens where generated.
- [X] T022 [US1] Add app-flow diagnostics that name input value, raw key, event direction, current screen, expected transition, and affected generated app flow.
- [X] T023 [US1] Capture packed-library/FSI evidence and generated product validation for the viewer-key start/options/interaction/restart path and optional app-host convenience path in `readiness/normalized-viewer-input.md` and `readiness/generated-template-input-flows.md`.
- [X] T024 [US1] Document the US1 independent validation path and map evidence to FR-001 through FR-006, FR-018, FR-019, SC-001, SC-002, and SC-008.

**Checkpoint**: US1 is fully functional and independently testable.

---

## Phase 4: User Story 2 - Prove Graphical Startup Without Manual Timeouts

### Tests First

- [X] T025 [P] [US2] Add failing bounded run contract tests for first-frame success, positive frame count, positive timeout, elapsed time, output size, renderer mode, frame count, and last diagnostic summary.
- [X] T026 [P] [US2] Add failing forced pre-frame failure tests for blocked window, surface, renderer, swapchain, scene, readback, app, timeout, and unknown stages with unsupported-environment versus product-defect classification.
- [X] T027 [P] [US2] Add pure viewer lifecycle tests for bounded run `Model`, `Msg`, `Effect` or `Cmd<Msg>`, `init`, `update`, emitted effects, and interpreter decisions without relying on shell timeouts.

### Implementation

- [X] T028 [US2] Implement bounded viewer run requests for first frame, exact frame count, and bounded duration with validation for positive frame counts and timeouts.
- [X] T029 [US2] Implement real viewer interpreter behavior that exits after evidence target completion or returns structured pre-frame failure without external process timeout or stderr scraping.
- [X] T030 [US2] Capture structured success evidence with frames rendered, elapsed time, initial output size, renderer mode, last diagnostic summary, and evidence path.
- [X] T031 [US2] Capture structured failure evidence with blocked stage, classification, diagnostic category, message, and last diagnostic summary.
- [X] T032 [US2] Add generated consumer graphical smoke command integration and readiness writing for supported-host success or explicit unsupported-host output.
- [X] T033 [US2] Capture bounded real-viewer smoke evidence in `readiness/bounded-viewer-smoke.md`, including logs under `readiness/logs/`.
- [X] T034 [US2] Document the US2 independent validation path and map evidence to FR-007 through FR-009, FR-014a, FR-019, SC-003, and SC-004.

**Checkpoint**: US2 is fully functional and independently testable.

---

## Phase 5: User Story 3 - Debug Startup Without Frame Log Noise

### Tests First

- [X] T035 [P] [US3] Add failing diagnostic filtering tests for startup, input, frame, renderer, Vulkan, Skia, swapchain, scene, screenshot/readback categories and level thresholds.
- [X] T036 [P] [US3] Add failing frame sampling tests proving startup-only diagnostics exclude repeated per-frame messages while frame-loop messages appear only when enabled or sampled.
- [X] T037 [P] [US3] Add failing diagnostic sink tests proving in-process capture can assert startup, input, renderer, and frame categories without process stderr scraping.

### Implementation

- [X] T038 [US3] Implement diagnostic event records with level, category, message, optional frame index, optional stage, elapsed timestamp, and capturable sink dispatch.
- [X] T039 [US3] Implement diagnostics options for independent level/category selection, frame log limits or sampling, compatibility verbose behavior, and readable last-diagnostic summaries.
- [X] T040 [US3] Wire viewer startup, input, renderer, swapchain/surface, scene drawing, screenshot/readback, and frame loop milestones through categorized diagnostics.
- [X] T041 [US3] Add tests and generated smoke evidence proving startup-focused runs are readable and frame-focused runs contain frame messages only by explicit configuration.
- [X] T042 [US3] Capture diagnostic readiness in `readiness/diagnostics.md`, including category examples and captured-sink assertions.
- [X] T043 [US3] Document the US3 independent validation path and map evidence to FR-010 through FR-012, FR-019, SC-005, and SC-006.

**Checkpoint**: US3 is fully functional and independently testable.

---

## Phase 6: User Story 4 - Collect Visual Evidence When Desktop Windows Are Unavailable

### Tests First

- [X] T044 [P] [US4] Add failing deterministic scene evidence tests for hash, PNG, or metadata output with stable output size, renderer mode, evidence value, and representative generated graphical app scene.
- [X] T045 [P] [US4] Add failing tests proving scene evidence does not open a native viewer/window path and remains separate from bounded real viewer startup evidence.
- [X] T046 [P] [US4] Add failing unsupported-environment tests for missing rendering/readback capabilities with explicit diagnostics rather than ambiguous app failures.

### Implementation

- [X] T047 [US4] Implement scene evidence request/result helpers in the appropriate Scene or Testing package using deterministic scene-level rendering rather than live viewer startup.
- [X] T048 [US4] Implement hash, PNG, or metadata evidence writing with output size, renderer mode, evidence path, and value suitable for generated app validation.
- [X] T049 [US4] Implement unsupported-host classification for scene evidence without treating unsupported hosts as successful product evidence.
- [X] T050 [US4] Wire generated product validation to collect deterministic scene-level evidence while retaining separate bounded real-viewer smoke status.
- [X] T051 [US4] Capture scene evidence readiness in `readiness/headless-scene-evidence.md`, including generated scene output or explicit unsupported-host diagnostics.
- [X] T052 [US4] Document the US4 independent validation path and map evidence to FR-013, FR-014, FR-014a, FR-019, and SC-007.

**Checkpoint**: US4 is fully functional and independently testable.

---

## Phase 7: User Story 5 - Reproduce Consumer Package Setup Reliably

### Tests First

- [X] T053 [P] [US5] Add failing local package report tests for feed path, package identities, versions, consumer package configuration snippet, optional `nuget.config` snippet, restore command, and generated consumer package set.
- [X] T054 [P] [US5] Add failing stale or missing local feed fixture tests proving package/feed drift is reported before generated consumer build, source, input, or rendering failures.
- [X] T055 [P] [US5] Add failing generated consumer validation tests for the path from fresh local package output to semantic tests, bounded real viewer smoke where supported, scene evidence or unsupported-host diagnostics, elapsed time, and reproducible command context.

### Implementation

- [X] T056 [US5] Implement the local consumer package report command or workflow in build scripts, including package identities, versions, feed path, snippets, restore command, drift diagnostics, and whether package inventory comes from `DependencyReport` or from the local consumer package report workflow directly.
- [X] T057 [US5] Implement stale/missing local feed classification as setup drift with package identity, expected version, actual version, feed path, and remediation command.
- [X] T058 [US5] Wire generated guidance and quickstarts to show interactive run, bounded smoke, headless scene evidence, unsupported-host expectations, and local package restore setup.
- [X] T059 [US5] Wire generated consumer validation from `PackLocal` output through restore, generated semantic tests, bounded smoke where available, scene evidence, and readiness writing.
- [X] T060 [US5] Capture local package guidance, `DependencyReport` output when used for package inventory, and generated consumer validation evidence in `readiness/local-consumer-packages.md` and `readiness/generated-consumer-validation.md`.
- [X] T061 [US5] Document the US5 independent validation path and map evidence to FR-015 through FR-017, FR-019, SC-009, SC-010, and SC-011.

**Checkpoint**: US5 is fully functional and independently testable.

---

## Phase 8: Integration & Polish

- [X] T062 [P] Refresh public surface baselines for all changed Tier 1 packages and run `./fake.sh build -t PackageSurfaceCheck`.
- [X] T063 [P] Run focused package and semantic checks for changed projects, including `KeyboardInput.Tests`, `SkiaViewer.Tests`, `Scene.Tests`, `Testing.Tests`, `Governance.Tests`, `Smoke.Tests`, and `Package.Tests` as applicable. Evidence: focused logs under `readiness/logs/t063-*.txt`; `Governance.Tests` passes 93/93 after governance fixture drift fixes.
- [X] T064 [P] Run `./fake.sh build -t TemplateCheck`, `./fake.sh build -t GeneratedGuidanceCheck`, `./fake.sh build -t TemplateDrift`, and `./fake.sh build -t GeneratedProductCheck`; save command logs under `readiness/logs/`.
- [X] T065 Run `./fake.sh build -t PackLocal` and confirm generated consumers use local packages rather than repository implementation source.
- [X] T066 Run generated consumer validation from fresh package output to first-frame or deterministic visual evidence in under 10 minutes on a supported local machine, or record explicit unsupported-host diagnostics.
- [X] T067 Run `./fake.sh build -t FsiTranscripts` and update FSI or packed-library transcript evidence for the new public contracts.
- [X] T068 Update `quickstart.md`, `docs/build.md`, `docs/evidence.md`, `docs/generated-apps.md`, and `docs/dependencies.md` with final command names, generated app flows, diagnostics, package setup, and evidence paths.
- [X] T069 Run `./fake.sh build -t Verify` and `./fake.sh build -t Ci`, or record a non-authoritative environment failure plus the exact required rerun environment.
- [X] T070 Run `./fake.sh build -t EvidenceGraph` and update `readiness/evidence-graph.md` with the current task DAG status.
- [X] T071 Run `./fake.sh build -t EvidenceAudit` and update `readiness/evidence-audit.md`, resolving synthetic propagation or diff-scan blockers before declaring completion.
- [X] T072 Perform final readiness review: every required evidence file links to real logs or disclosed synthetic evidence, all unsupported-host outcomes are explicit, and the Synthetic-Evidence Inventory is accurate.

**Checkpoint**: Feature complete — final readiness can be reviewed.

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| _(none yet)_ | | | |

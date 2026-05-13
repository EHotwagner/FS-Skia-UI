# Tasks: Skia Feature Parity

**Feature branch**: `002-skia-feature-parity`
**Spec**: `specs/002-skia-feature-parity/spec.md`
**Plan**: `specs/002-skia-feature-parity/plan.md`

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
from a user-facing entry point and that path was actually exercised: an FSI
session against the packed library, a smoke run of the application, a manual
walk-through with transcript, or a screenshot captured under `readiness/`.
Domain, model, or core-layer changes alone do **not** satisfy `[X]` for a
`[US*]` task, even if their unit tests pass green. If the user-reachable
surface is missing, stubbed, or not yet wired, mark `[ ]` or `[S]` with a
disclosed reason in the Synthetic-Evidence Inventory, never `[X]`.

For stateful or I/O-bearing stories, `[X]` also requires Elmish/MVU evidence:
the public `Model` / `Msg` / `Effect` or `Cmd<Msg>` contract was exercised,
pure `update` transitions were tested, emitted effects were asserted, and the
effect interpreter was run against real dependencies where safe.

This rule does not apply to Setup, Foundation, Integration, or Polish phase
tasks; those are evaluated against their own phase verification.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, ... — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change

Every task must have a matching entry in `tasks.deps.yml` even if its
dependency list is empty. The evidence graph command refuses to proceed with
dangling references.

---

## Phase 1: Setup

- [X] T001 Confirm the pinned upstream baseline commit and record the inspected capability-area inventory in `readiness/baseline-capabilities.md`
- [X] T002 [P] Create feature readiness scaffolding under `readiness/` for transcripts, screenshots, parity reports, surface baselines, package logs, and smoke logs
- [X] T003 [P] Add the local NuGet output, sample, and evidence command conventions to the feature readiness notes
- [X] T004 [P] Record feature Tier 1 obligations: public API impact, required `.fsi` files, Elmish/MVU applicability, Vulkan-only constraint, and real-vs-synthetic evidence rules
- [X] T005 [P] Capture current repository surface and sample/test inventory as the starting implementation baseline

**Checkpoint**: Setup complete.

---

## Phase 2: Foundation

- [X] T006 Draft `src/Lib` public `.fsi` contracts for scene, paint, shader, filter, path, clipping, text, picture, diagnostics, screenshots, parity reporting, and viewer MVU host APIs
- [X] T007 [P] Scaffold `src/Charts` and `src/Layout` packable projects with pinned dependencies and solution entries
- [X] T008 Draft `src/Charts` public `.fsi` contracts for shared chart props, every chart module, DataGrid props, pure helpers, and hit-test projections
- [X] T009 Draft `src/Layout` public `.fsi` contracts for layout types, stack/dock layout, graph validation, graph layout, graph builders, and hit-test projections
- [X] T010 [P] Create test projects `Charts.Tests`, `Layout.Tests`, `Parity.Tests`, `Package.Tests`, and `Smoke.Tests` and add them to the solution
- [X] T011 [P] Create or update FSI prelude scripts for core, charts, layout, and parity evidence workflows
- [X] T012 Add surface-area baseline generation and comparison tests for all public modules in the three packages
- [X] T013 Add shared deterministic rendering fixtures, screenshot tolerance metadata, large-data generators, and sample asset fixtures
- [S] T014 Add diagnostics test fixtures for unsupported Vulkan, missing capability, screenshot failure, frame recovery, and shutdown failure scenarios
- [X] T015 Exercise the draft `.fsi` surface from FSI and capture `readiness/fsi-session.txt`, including core `init`/`update`/effect paths and pure component construction
- [X] T016 Document unsupported-scope handling for fallback renderer and non-Elmish integration baseline behaviors
- [X] T017 Run foundation verification (`dotnet restore`, `dotnet build`, surface baseline tests, and FSI scripts) and store logs under `readiness/`

**Checkpoint**: Foundation ready; story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) - Render Baseline Drawing Coverage

### Tests First

- [X] T018 [P] [US1] Add packed-library/prelude semantic tests for primitive, group, image, arc, point, vertices, picture, and nested scene constructors
- [X] T019 [P] [US1] Add semantic tests for paint defaults and options: fill, stroke, opacity, antialiasing, caps, joins, miter, and blend modes
- [X] T020 [P] [US1] Add semantic tests for shader, color filter, mask filter, image filter, and path effect declarations with unsupported-capability diagnostics
- [X] T021 [P] [US1] Add semantic tests for path commands, fill types, boolean operations, path measurement, segment extraction, and construction helpers
- [X] T022 [P] [US1] Add screenshot or render-readback verification for the drawing parity gallery covering at least 60 visual capabilities

### Implementation

- [X] T023 [US1] Implement immutable scene, element, bounds, color, matrix, metadata, and composition data structures in `FS.Skia.UI`
- [X] T024 [US1] Implement paint, blend mode, stroke, shader, color filter, mask filter, image filter, and path effect declarations
- [X] T025 [US1] Implement path, clipping, region, text, font, text-run, picture, color-space, and perspective transform declarations
- [X] T026 [US1] Implement Skia translation/rendering for primitive elements, groups, images, points, vertices, arcs, reusable pictures, and nested scenes
- [X] T027 [US1] Implement Skia translation/rendering for paint options, blend modes, shaders, filters, path effects, clipping, regions, color handling, and transforms
- [X] T028 [US1] Add diagnostics for invalid resources, unavailable fonts, unsupported effects, invalid paths, and device-specific rendering capability gaps
- [X] T029 [US1] Build `samples/ParityGallery` and `samples/EffectsGallery` with representative drawing, styling, shader, filter, path, text, image, clipping, region, picture, color-space, and transform scenes
- [X] T030 [US1] Document the US1 independent validation path and record gallery screenshots or render evidence under `readiness/screenshots/`

**Checkpoint**: US1 drawing coverage is independently testable.

---

## Phase 4: User Story 2 (US2) - Build Rich Data Visuals

### Tests First

- [X] T031 [P] [US2] Add packed-library/prelude tests for chart props, axis/legend/palette config, pure scaling helpers, and empty/invalid data behavior
- [X] T032 [P] [US2] Add semantic and scale tests for line, bar, pie/donut, scatter, area, histogram, candlestick, and radar charts, including 100,000-point datasets
- [X] T033 [P] [US2] Add semantic and scale tests for DataGrid columns, cells, fixed headers, vertical viewport math, sorting, width management, and 10,000-row datasets
- [X] T034 [P] [US2] Add composition tests proving charts and DataGrid are pure scene elements embedded in larger core scenes

### Implementation

- [X] T035 [US2] Implement shared chart/DataGrid types, defaults, palettes, labels, legends, axes, viewport records, sort records, and projection helpers
- [X] T036 [US2] Implement chart scaling, invalid-value filtering, empty-state output, label layout, legend layout, and pure hit-test projection helpers
- [X] T037 [US2] Implement line, bar, pie/donut, scatter, area, histogram, candlestick, and radar chart builders returning core scene elements
- [X] T038 [US2] Implement DataGrid builder, visible-row calculation, sorting helper, fixed header rendering, cell formatting, width management, and hit-test projection helpers
- [X] T039 [US2] Build `samples/ChartsGallery` and `samples/DataGridGallery` with realistic datasets, resizing behavior, and Elmish-owned selection/sort/scroll state
- [X] T040 [US2] Document US2 validation and capture chart/DataGrid FSI transcripts, screenshots, and scale-test logs under `readiness/`

**Checkpoint**: US2 data visuals are independently testable.

---

## Phase 5: User Story 3 (US3) - Compose Layouts and Graphs

### Tests First

- [X] T041 [P] [US3] Add packed-library/prelude tests for layout props, horizontal stack, vertical stack, dock config, child sizing, padding, spacing, and zero/negative bounds
- [X] T042 [P] [US3] Add layout resize tests for nested layouts with at least 10 child elements at three window sizes and no overlap of required content
- [X] T043 [P] [US3] Add graph validation tests for cycles, duplicate identifiers, missing endpoints, disconnected components, self-loops, and dense edge sets
- [X] T044 [P] [US3] Add graph layout/render tests for a 100-node DAG within 2 seconds and a 50-node weighted undirected graph with visible components

### Implementation

- [X] T045 [US3] Implement layout shared types, sizing, measurement, allocation, bounds handling, and deterministic child placement helpers
- [X] T046 [US3] Implement horizontal stack, vertical stack, and dock builders returning core scene elements
- [X] T047 [US3] Implement graph shared types, style records, validation results, cycle detection, missing endpoint checks, duplicate checks, and component reporting
- [X] T048 [US3] Implement DAG layering and undirected weighted graph layout helpers with deterministic bounds and scale behavior
- [X] T049 [US3] Implement directed and undirected graph scene builders with node, edge, label, weight, validation diagnostic, and hit-test output
- [X] T050 [US3] Build `samples/LayoutGraphGallery` with nested layouts, chart/grid composition, directed graph, invalid DAG diagnostic, and weighted undirected graph views
- [X] T051 [US3] Document US3 validation and capture layout/graph FSI transcripts, screenshots, validation reports, and performance logs under `readiness/`

**Checkpoint**: US3 layouts and graphs are independently testable.

---

## Phase 6: User Story 4 (US4) - Operate Reliably Through Elmish Viewer Flow

### Tests First

- [X] T052 [P] [US4] Add `.fsi` contract tests for `ViewerProgram`, `ViewerEvent`, `ViewerEffect`, screenshot request/result types, diagnostics, `init`, `update`, and interpreter boundary
- [X] T053 [P] [US4] Add pure Elmish transition tests for keyboard, pointer, scroll, resize, close, lifecycle, frame tick, diagnostic, screenshot, and recoverable frame-error messages
- [X] T054 [P] [US4] Add emitted-effect assertion tests for initialize renderer, render frame, capture screenshot, report diagnostic, dispatch message, and shutdown commands
- [X] T055 [P] [US4] Add real interpreter evidence tests where safe for screenshot output, lifecycle disposal, recoverable frame errors, and Vulkan-unavailable startup diagnostics

### Implementation

- [X] T056 [US4] Extend viewer contracts and implementation for keyboard, pointer, scroll, resize, close, lifecycle, frame tick, diagnostic, and recoverable frame-error events
- [X] T057 [US4] Implement screenshot capture as Elmish edge effects with PNG/JPEG output, post-frame gating, file failure diagnostics, and current-frame verification hooks
- [X] T058 [US4] Implement Vulkan-only startup capability checks and structured diagnostics for unsupported hardware, driver, surface, swapchain, Skia context, and effect capabilities
- [X] T059 [US4] Implement frame-level recovery flow that reports recoverable errors and renders the next valid frame without crashing the application
- [X] T060 [US4] Implement thread-safe lifecycle shutdown and disposal with documented timeout behavior and failure diagnostics
- [X] T061 [US4] Build or update `samples/InteractiveViewer` and `samples/ScreenshotGallery` to exercise input, lifecycle, screenshots, diagnostics, recovery, and shutdown from Elmish state
- [X] T062 [US4] Document US4 validation and capture MVU transition logs, emitted-effect assertions, screenshot files, and interpreter evidence under `readiness/`

**Checkpoint**: US4 viewer operation is independently testable.

---

## Phase 7: User Story 5 (US5) - Demonstrate and Prove Parity

### Tests First

- [X] T063 [P] [US5] Add parity evidence report tests requiring one item per pinned-baseline capability with normalized status, evidence type, command, path, and adaptation notes
- [X] T064 [P] [US5] Add clean-checkout package tests proving `FS.Skia.UI`, `FS.Skia.UI.Charts`, and `FS.Skia.UI.Layout` restore, pack, and reference independently
- [X] T065 [P] [US5] Add sample smoke tests for BasicViewer, InteractiveViewer, ParityGallery, EffectsGallery, ChartsGallery, DataGridGallery, LayoutGraphGallery, ScreenshotGallery, and DemoReel
- [X] T066 [P] [US5] Add documentation checks for the parity matrix, Vulkan-only adaptations, Elmish-only adaptations, excluded baseline behaviors, and quickstart commands

### Implementation

- [X] T067 [US5] Implement `FS.Skia.UI.Parity` report types, serialization helpers, baseline capability IDs, and merge-ready validation rules
- [X] T068 [US5] Implement `scripts/parity-evidence.fsx` to generate `readiness/parity-evidence.json` from semantic, screenshot, smoke, package, and documentation evidence
- [X] T069 [US5] Add package metadata, project references, versioning, readme/package notes, and pack verification for all three packages
- [X] T070 [US5] Build `samples/DemoReel` and refresh BasicViewer to demonstrate the combined parity workflow without fallback renderer controls
- [X] T071 [US5] Write consumer documentation and parity matrix mapping baseline capabilities into Vulkan-only Elmish workflows with supported/adapted/excluded rationale
- [X] T072 [US5] Run all documented sample and quickstart commands from a clean checkout or clean working directory and capture logs under `readiness/smoke/`
- [X] T073 [US5] Generate the final parity evidence report and confirm every non-conflicting baseline capability is `Supported` or `Adapted`

**Checkpoint**: US5 parity evidence is independently testable.

---

## Phase 8: Integration & Polish

- [X] T074 Refresh public surface-area baselines for Tier 1 modules and verify no accidental public APIs leak through `.fsi`
- [X] T075 Run `dotnet restore`, `dotnet build`, `dotnet test`, all FSI preludes, package verification, and parity evidence generation; store consolidated logs under `readiness/`
- [X] T076 Run visual/screenshot verification across deterministic galleries and document any manual visual review entries with rationale and reviewer evidence
- [S] T077 Run Windows and Linux smoke evidence where available, or mark platform-limited tasks `[S]` with Principle V disclosures and real-evidence follow-up paths
- [X] T078 Run `speckit.evidence.graph` and confirm no cycles, dangling refs, or unexpected propagated synthetic markers
- [X] T079 Run `speckit.evidence.audit` and confirm PASS, or document every accepted synthetic override with a tracking issue
- [X] T080 Update quickstart, docs, package notes, and sample commands to match the final implemented paths and project names
- [X] T083 Capture first visible frame timing evidence for BasicViewer, requiring under 2 seconds in at least 95% of supported-workstation smoke runs
- [X] T084 Write public API compatibility notes and migration guidance for changed or expanded `FS.Skia.UI`, `FS.Skia.UI.Charts`, and `FS.Skia.UI.Layout` surfaces, including package references, revised core viewer APIs, and `.fsi` baseline impact
- [X] T081 Final readiness review: verify the hard parity gate, three package boundaries, Vulkan-only constraint, Elmish-only constraint, and at least eight runnable samples
- [X] T082 Prepare merge summary with test commands, evidence paths, synthetic-evidence inventory, and known platform caveats

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| T014 | Native Vulkan startup-stage failures are simulated in a diagnostic fixture so the test does not mutate or disable the workstation GPU/driver. | Capture a real unsupported-environment or driver-failure smoke log under `readiness/smoke/` on hardware/CI where Vulkan is unavailable. | GitHub issue #2 |
| T077 | Windows smoke evidence is platform-limited from this Linux workspace; Linux Vulkan evidence is captured, Windows execution is not. | Run the documented smoke commands on a Windows Vulkan-capable workstation and store logs under `readiness/smoke/windows/`. | GitHub issue #3 |

# Tasks: Persistent GUI Runtime

**Feature branch**: `018-persistent-gui-runtime`  
**Spec**: `specs/018-persistent-gui-runtime/spec.md`  
**Plan**: `specs/018-persistent-gui-runtime/plan.md`

## Status Legend

- `[ ]` pending
- `[X]` done with real evidence
- `[S]` done with synthetic evidence only
- `[F]` failed
- `[-]` skipped with written rationale

The `[S*]` marker is computed by the evidence audit from `tasks.deps.yml`; do not write it by hand. Approved synthetic error-handling work uses `[SEH]` plus `synthetic-error-handling-approved` and remains synthetic-only when completed with malformed-input or explicit error-path evidence.

## Task Metadata Discipline

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors the structured `skillist` value using `[skillist: ...]`; `[skillist: []]` means no capability skill materially applies. `[P]` means no dependency inside the current phase.

## Skill Confidence Review

- High-confidence capability matches: `fs-skia-skiaviewer` for viewer host/runtime contracts and diagnostics; `fs-skia-elmish` for state/update/effect boundaries; `fs-skia-keyboard-input` for generated keyboard dispatch; `fs-skia-scene` for rendered game board/readback surface; `fs-skia-testing` for generated product and validation helpers; `speckit-evidence-graph` and `speckit-evidence-audit` for graph/audit gates; `speckit-tasks` and `speckit-implement` for task workflow guidance.
- Medium or indirect matches accepted only where the task spans generated product runtime and package validation: generated-product work is covered by framework capability skills plus `fs-skia-testing` because the template project skill is not registered as a resolvable feature skill.
- Valid-empty `skillist: []` is used for readiness-writing, documentation-only, unsupported-scope inventory, and final broad verification tasks with no single capability owner. Reviewer disposition: accepted-empty unless implementation discovers a narrower owner.
- False-positive omissions: controls, charts, graph controls, DataGrid, layout-only work, samples, and release automation are out of scope for this feature.

## Risk-Level Evidence

Governance risk level is **broad Tier 1** because the feature changes public SkiaViewer contracts, generated product defaults, package verification, evidence/audit rules, and readiness contracts. Focused validation is required for SkiaViewer, generated template/product tests, package resolution, visual evidence, and evidence graph/audit paths. Broad validation is required before completion through `./fake.sh build -t Verify` plus graph/audit gates. Non-authoritative aggregate results must be recorded in readiness with the failing class and focused rerun evidence.

## Phase 1: Setup

- [X] T001 [skillist: speckit-evidence-graph, speckit-evidence-audit] Create `specs/018-persistent-gui-runtime/readiness/` and scaffold required readiness files for interactive lifecycle, evidence launch mode, container session diagnostics, package resolution, generated verify, game visual evidence, task workflow guidance, graph, and audit
- [X] T002 [P] [skillist: []] Record Tier 1 scope, public API impact, generated product impact, package impact, unsupported scope, and required evidence obligations in `readiness/evidence-obligations.md`
- [X] T003 [P] [skillist: speckit-tasks] Record task-generation assumptions, skill confidence review, story grouping, valid-empty skill dispositions, `[SEH]` approval, and graph validation expectations in `readiness/task-generation.md`
- [X] T004 [skillist: []] Inventory affected source, template, test, docs, FAKE target, package, fixture, and readiness paths named by the spec, plan, contracts, and quickstart

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input] Draft `src/SkiaViewer/SkiaViewer.fsi` contracts for `ViewerLaunchMode`, launch outcome fields, `runApp`, explicit evidence launch, desktop diagnostics, `Model`/`Msg`/`Effect`, `init`, pure `update`, and interpreter boundaries
- [X] T006 [skillist: fs-skia-skiaviewer] Add failing-first SkiaViewer semantic and FSI-surface tests for interactive/evidence launch separation, first-frame keep-open behavior, outcome fields, close-source reporting, and desktop diagnostic classification
- [X] T007 [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input] Add failing-first MVU lifecycle tests for launch states, pure update transitions, emitted effects, keyboard dispatch, tick progression, user close, evidence target completion, timeout, and failure transitions
- [X] T008 [P] [skillist: fs-skia-skiaviewer, fs-skia-testing] Add generated template/product tests proving normal generated game execution defaults to interactive launch, evidence mode is explicit, generated tests run, and placeholder verification is non-authoritative
- [X] T009 [P] [skillist: fs-skia-testing] Add generated verification/package tests that fail on `NU1603`, requested/resolved `FS.Skia.UI.*` version mismatch, missing package sources, and generated test projects that exist but do not run
- [X] T010 [P] [skillist: fs-skia-skiaviewer, fs-skia-scene, fs-skia-testing] Add visual evidence tests for screenshot preference, pixel-readback fallback, readable board proof, input/progress observation, and explicit unsupported-host diagnostics
- [X] T011 [P] [skillist: speckit-evidence-audit] Add audit fixtures rejecting missing readiness files, bounded-only substitution for interactive evidence, text-only visual metadata on supported hosts, unresolved package mismatch, and missing generated test execution
- [X] T012 [P] [skillist: speckit-tasks, speckit-implement] Add generated guidance tests for implementation batches, red-green evidence logs, graph before/after records, task skill loading, and non-authoritative aggregate result reporting
- [S] T013 [P] [SEH] synthetic-error-handling-approved [skillist: speckit-evidence-audit] Add pre-implementation synthetic error-handling metadata validation for malformed readiness rows, invalid command arguments, missing required package fields, and corrupt evidence records
- [X] T014 [skillist: fs-skia-skiaviewer] Prepare surface baseline refresh path for `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt` without refreshing the baseline during task generation
- [X] T015 [skillist: []] Document Elmish/MVU evidence obligations, synthetic fake window-loop limits, required real interpreter evidence, small/medium/broad risk levels, focused validation, broad validation trigger, and non-authoritative aggregate handling

**Checkpoint**: Foundation ready. Story implementation may begin in parallel after failing-first tests and contracts exist.

---

## Phase 3: User Story 1 - Play a Generated Game Interactively (P1)

### Tests First

- [X] T016 [P] [US1] [skillist: fs-skia-skiaviewer] Add semantic tests proving `Viewer.runApp` does not complete after first-frame presentation without close, returns `mode=interactive-window`, reports `self-closed-for-evidence=false`, and remains open for at least 30 seconds unless an explicit close action occurs
- [X] T017 [P] [US1] [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input] Add generated host tests for `init`, pure `update`, emitted effects, keyboard input dispatch, time-based tick progression, first-frame state, and explicit user/host close
- [X] T018 [P] [US1] [skillist: fs-skia-skiaviewer, fs-skia-scene, fs-skia-keyboard-input, fs-skia-testing] Add generated product tests that the default Tetris-style executable path renders board/grid plus side information, dispatches keyboard input, advances over time, and keeps evidence flags out of normal launch

### Implementation

- [X] T019 [US1] [skillist: fs-skia-skiaviewer] Implement interactive launch lifecycle, first-frame non-completion, close-source tracking, outcome fields, option validation, and fast desktop-session precheck in `src/SkiaViewer/SkiaViewer.fs`
- [X] T020 [US1] [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input] Implement `GeneratedAppHost` interpretation for `init`, pure `update`, rendered view refresh, keyboard dispatch, tick progression, emitted effects, and close handling
- [X] T021 [US1] [skillist: fs-skia-skiaviewer, fs-skia-scene, fs-skia-elmish, fs-skia-keyboard-input] Update `template/base/src/Product/Program.fs` with default interactive generated game wiring, playable Tetris-style model/view/update/input/tick flow, and explicit non-default evidence flags
- [X] T022 [US1] [skillist: fs-skia-skiaviewer, fs-skia-testing] Update generated product validation helpers so default source/wiring requires interactive launch and rejects metadata-only, bounded-only, scene-only, or self-exiting graphical paths
- [X] T023 [US1] [skillist: []] Record `readiness/interactive-lifecycle.md` with independent validation commands, expected outcome fields, fake window-loop disclosure if used, supported-host path, and explicit close criteria

**Checkpoint**: US1 is independently testable through the generated default executable path.

---

## Phase 4: User Story 2 - Collect Launch Evidence Explicitly (P1)

### Tests First

- [X] T024 [P] [US2] [skillist: fs-skia-skiaviewer] Add tests for explicit evidence launch API/flag selection, `mode=persistent-evidence`, first-frame/input fields, timeout/failure handling, and self-close-for-evidence reporting
- [X] T025 [P] [US2] [skillist: fs-skia-skiaviewer, fs-skia-testing] Add generated command tests proving bounded evidence, first-frame, input-dispatch, screenshot, and pixel-readback checks are opt-in and never reported as ongoing interactive play

### Implementation

- [X] T026 [US2] [skillist: fs-skia-skiaviewer] Implement explicit evidence launch behavior, bounded target completion, self-close semantics, timeout diagnostics, and outcome serialization
- [X] T027 [US2] [skillist: fs-skia-skiaviewer, fs-skia-testing] Wire generated CLI/workflow evidence commands and generated validation outputs for bounded launch evidence without changing the default interactive path
- [X] T028 [US2] [skillist: []] Record `readiness/evidence-launch-mode.md` with command evidence, outcome fields, self-close disclosure, input-dispatch status, and reviewer guidance distinguishing evidence from interactive play

**Checkpoint**: US2 evidence runs are bounded, explicit, and independently reviewable.

---

## Phase 5: User Story 3 - Diagnose Desktop Session Problems Before App Debugging (P2)

### Tests First

- [X] T029 [P] [US3] [skillist: fs-skia-skiaviewer] Add desktop readiness tests for runtime directory presence, ownership suitability, permissions, display variables, Wayland/X11 sockets, session bus reporting, fallback labeling, and unsupported-host reason selection
- [X] T030 [P] [US3] [skillist: fs-skia-skiaviewer, fs-skia-testing] Add generated app diagnostic tests proving normal interactive launch fails fast with environment/session diagnostics and does not silently switch to evidence, text-only metadata, or private runtime fallback

### Implementation

- [X] T031 [US3] [skillist: fs-skia-skiaviewer] Implement desktop-session diagnostic model, Linux/container preflight checks, fallback runtime directory labeling, failure classes, blocked stages, categories, and actionable messages
- [X] T032 [US3] [skillist: fs-skia-skiaviewer] Wire generated app/container readiness commands so diagnostics run before app lifecycle debugging and report environment/session failures separately from product defects
- [X] T033 [US3] [skillist: []] Record `readiness/container-session-diagnostics.md` with invalid configuration matrix, exact missing prerequisites, fallback-not-full-session labeling, supported-host integration notes, and a counted invalid-configuration matrix demonstrating the 95% readiness-validation threshold

**Checkpoint**: US3 diagnostics isolate host/session failures before app lifecycle investigation.

---

## Phase 6: User Story 4 - Verify Generated App Dependencies and Tests (P2)

### Tests First

- [X] T034 [P] [US4] [skillist: fs-skia-testing] Add generated verification tests requiring exact package resolution evidence, configured package sources, `NU1603` failure, generated test execution, authoritative flags, and failure class reporting
- [X] T035 [P] [US4] [skillist: fs-skia-scene, fs-skia-testing] Add generated game readiness tests requiring screenshot proof when available, pixel-readback fallback when screenshot is unavailable, and unsupported-host diagnostic when neither visual path exists
- [X] T036 [P] [US4] [skillist: speckit-evidence-audit] Add audit tests for package-resolution evidence, generated verify evidence, visual game evidence, placeholder/non-authoritative target rejection, and missing readiness acceptance keywords

### Implementation

- [X] T037 [US4] [skillist: fs-skia-testing] Implement generated package-resolution verification, source/resolved version reporting, `NU1603`/mismatch failure, generated test execution checks, and authoritative/non-authoritative output fields
- [X] T038 [US4] [skillist: fs-skia-skiaviewer, fs-skia-scene, fs-skia-testing] Implement generated visual evidence command selection, screenshot capture path, pixel-readback fallback path, readable board/input-progress fields, and unsupported-host diagnostics
- [X] T039 [US4] [skillist: speckit-evidence-audit] Implement audit/readiness checks for required files, required content, package mismatch, generated test execution, visual evidence substitution, and bounded-only lifecycle substitution
- [X] T040 [US4] [skillist: fs-skia-testing] Update `Verify`, generated `Test`, `GeneratedProductCheck`, package verification, and relevant aggregate targets so generated tests and exact package resolution are enforced
- [X] T041 [US4] [skillist: []] Record `readiness/package-resolution.md`, `readiness/generated-verify.md`, and `readiness/game-visual-evidence.md` with requested/resolved versions, sources, generated test command evidence, visual proof or unsupported-host diagnostics, selected risk level, and focused rerun commands

**Checkpoint**: US4 verification distinguishes package, verification-depth, environment/session, app-lifecycle, and product-defect failures.

---

## Phase 7: Integration & Polish

- [X] T042 [skillist: speckit-tasks, speckit-implement] Write `readiness/task-workflow-guidance.md` with implementation batch records, task ids, shared evidence, graph before/after paths, skill-loading notes, and red-green evidence log format
- [X] T043 [skillist: fs-skia-skiaviewer] Refresh `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt` with `./fake.sh build -t RefreshSurfaceBaselines` and verify with `./fake.sh build -t PackageSurfaceCheck`
- [X] T044 [skillist: fs-skia-skiaviewer, fs-skia-testing] Run `./fake.sh build -t PackLocal`, generated consumer restore/test/verify, `./fake.sh build -t TemplateCheck`, and `./fake.sh build -t GeneratedProductCheck`; record package compatibility and generated-product evidence
- [X] T045 [skillist: []] Run documentation and dependency governance checks, including `./fake.sh build -t DependencyReport`, and record no-new-dependency or package impact findings
- [X] T046 [skillist: speckit-evidence-graph] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/018-persistent-gui-runtime --graph-only` after story readiness records exist and capture clean graph output in `readiness/evidence-graph.md`
- [X] T047 [skillist: speckit-evidence-graph, speckit-evidence-audit] Run `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`, then capture PASS or blocking diagnostics in `readiness/evidence-audit.md`
- [X] T048 [skillist: speckit-tasks, speckit-implement] Run `./fake.sh build -t GeneratedGuidanceCheck` and confirm generated task/implementation guidance preserves persistent interactive launch, explicit evidence mode, skillist metadata, `[SEH]` validation, and risk-level rules
- [X] T049 [skillist: []] Run `./fake.sh build -t Verify` for broad Tier 1 validation or record explicit non-authoritative aggregate diagnostics with focused rerun evidence
- [X] T050 [skillist: speckit-evidence-audit] Complete final readiness review with all eight required readiness files, supported-host visual or unsupported-host diagnostics, package/test verification evidence, synthetic inventory, and no bounded-only completion claims — final readiness passed: EvidenceAudit exits 0 with accepted `[SEH]` only, no unaccepted synthetic tasks, no auto-synthetic propagation, no readiness contract hits, no persistent launch/runtime hits, and no blocking diff-scan hits

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the source for the PR description's synthetic-evidence section. For `[SEH]` rows, include the approval label, design-phase source, synthetic input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T013 | Pre-implementation metadata/error-path validation uses malformed readiness/package records that should not require corrupting real product evidence | `specs/018-persistent-gui-runtime/readiness/logs/t013-synthetic-error-evidence.txt` |  | synthetic-error-handling-approved | `specs/018-persistent-gui-runtime/plan.md` synthetic evidence and FR-025 | malformed readiness rows, invalid command arguments, missing required package fields, corrupt evidence records | Audit reports missing accepted synthetic-error metadata or explicit validation error without treating placeholder output as success | accepted-seh |

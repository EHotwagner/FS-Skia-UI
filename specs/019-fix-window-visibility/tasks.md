# Tasks: Fix Window Visibility

**Feature branch**: `019-fix-window-visibility`  
**Spec**: `specs/019-fix-window-visibility/spec.md`  
**Plan**: `specs/019-fix-window-visibility/plan.md`

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

- High-confidence capability matches: `fs-skia-skiaviewer` for viewer host/window lifecycle, close reasons, diagnostics, options, image evidence, and generated viewer startup; `fs-skia-elmish` for pure lifecycle `Model`/`Msg`/`Effect` boundaries; `fs-skia-testing` for generated product validation helpers and template test wiring; `fs-skia-scene` for scene rendering and pixel/image evidence; `speckit-evidence-graph` and `speckit-evidence-audit` for DAG and readiness gates.
- Medium or indirect matches accepted: `fs-skia-keyboard-input` appears only where generated input-device or input-dispatch evidence is materially touched; template/product work is represented by the owning runtime capability plus `fs-skia-testing` because no separate template capability skill is registered.
- Valid-empty `skillist: []` is used for readiness-writing, inventory, broad documentation, and aggregate verification tasks with no single capability owner. Reviewer disposition: accepted-empty unless implementation discovers a narrower owner.
- False-positive omissions: controls, charts, DataGrid, layout-only changes, release automation, marketplace distribution, and new game mechanics are out of scope for this feature.

## Risk-Level Evidence

Governance risk level is **broad Tier 1** because the feature changes public SkiaViewer contracts, generated app launch behavior, validation targets, visual evidence claims, readiness files, and audit expectations. Focused validation is required for changed SkiaViewer contracts, generated product tests, window diagnostics/options, visual evidence, package/generated validation, and evidence graph/audit paths. Broad validation is required before completion through `./fake.sh build -t Verify` plus graph/audit gates. Non-authoritative aggregate results must be recorded in readiness with the failing class and focused rerun evidence.

## Phase 1: Setup

- [X] T001 [skillist: speckit-evidence-graph, speckit-evidence-audit] Create `specs/019-fix-window-visibility/readiness/` and scaffold required readiness files for interactive visible window, close reason separation, window diagnostics, window options, real image evidence, generated validation, and audit, plus supporting governance graph output
- [X] T002 [P] [skillist: []] Record Tier 1 scope, public API impact, generated product impact, package impact, unsupported scope, and evidence obligations in `readiness/evidence-obligations.md`
- [X] T003 [P] [skillist: speckit-tasks] Record task-generation assumptions, skill confidence review, valid-empty skill dispositions, `[SEH]` approval rationale, graph validation expectations, and risk-level evidence rules in `readiness/task-generation.md`
- [X] T004 [skillist: []] Inventory affected source, template, tests, docs, FAKE targets, package guidance, fixtures, and readiness paths named by the spec, plan, contracts, and quickstart

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: fs-skia-skiaviewer, fs-skia-elmish] Draft `src/SkiaViewer/SkiaViewer.fsi` contracts for close reasons, observed values, window behavior requests, option results, launch outcomes, visual evidence artifacts, `Model`/`Msg`/`Effect`, `init`, pure `update`, and interpreter boundaries
- [X] T006 [skillist: fs-skia-skiaviewer] Add failing-first SkiaViewer semantic tests for interactive/evidence mode separation, first-frame non-completion, taskbar-only degradation, close reason derivation, and visibility diagnostic fields
- [X] T007 [skillist: fs-skia-skiaviewer, fs-skia-elmish] Add failing-first MVU lifecycle tests for checking-session, starting-window, visibility-checking, interactive-running, evidence-running, close-requested, unsupported, inaccessible-window, timeout, and failure transitions plus emitted effects
- [X] T008 [P] [skillist: fs-skia-skiaviewer, fs-skia-testing] Add generated template/product tests proving the default command uses interactive visible-window launch and explicit commands are required for bounded, image, pixel-readback, or metadata evidence
- [X] T009 [P] [skillist: fs-skia-testing] Add generated validation tests for exact package resolution, generated tests ran, authoritative flags, failure classes, `NU1603`, missing package source, placeholder success, and non-authoritative source scans
- [X] T010 [P] [skillist: fs-skia-skiaviewer, fs-skia-scene, fs-skia-testing] Add visual evidence tests requiring decodable image artifacts for requested image evidence, clear metadata/hash labeling, scene-vs-desktop-visibility claims, and unsupported-host diagnostics
- [X] T011 [P] [skillist: fs-skia-skiaviewer] Add window option tests for resize policy, maximize policy, startup state, startup position, backend preference, honored/degraded/unsupported/failed results, and fallback messages
- [X] T012 [P] [skillist: speckit-evidence-audit] Add audit fixtures rejecting missing readiness files, process/taskbar-only visible-window substitution, evidence close reported as user close, metadata-only screenshot claims, unresolved package mismatch, and missing generated test execution
- [X] T013 [P] [skillist: speckit-tasks, speckit-implement] Add generated guidance tests for implementation batches, skill loading, graph refresh before/after status changes, red-green evidence logs, persistent launch rules, and non-authoritative aggregate reporting
- [S] T014 [P] [SEH] synthetic-error-handling-approved [skillist: speckit-evidence-audit] Add pre-implementation synthetic error-handling validation for malformed readiness rows, invalid evidence command arguments, corrupt image metadata records, missing required generated-validation fields, and hostile artifact paths; approved synthetic error-handling evidence recorded in `readiness/logs/t014-synthetic-error-evidence.txt`
- [X] T015 [skillist: fs-skia-skiaviewer] Prepare surface baseline refresh path for `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt` without refreshing the baseline during task generation
- [X] T016 [skillist: []] Document Elmish/MVU evidence obligations, fake window-loop limits, required real interpreter evidence, supported-host visible-window requirements, small/medium/broad validation levels, broad-validation trigger, and non-authoritative aggregate handling

**Checkpoint**: Foundation ready. Story implementation may begin in parallel after failing-first tests and contracts exist.

---

## Phase 3: User Story 1 - Launch A Usable Game Window (P1)

### Tests First

- [X] T017 [P] [US1] [skillist: fs-skia-skiaviewer] Add semantic tests proving normal interactive launch remains open after first-frame presentation, reports `mode=interactive-window`, does not self-close for evidence, and only completes after user/app/host/failure close
- [X] T018 [P] [US1] [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input] Add generated host tests for `init`, pure `update`, emitted effects, first-frame state, input-device observation, manual input availability, and explicit user/native close
- [X] T019 [P] [US1] [skillist: fs-skia-skiaviewer, fs-skia-testing] Add generated product validation that rejects default commands which exit after first frame, run bounded evidence, print metadata only, or lack an accessible desktop window claim

### Implementation

- [X] T020 [US1] [skillist: fs-skia-skiaviewer] Implement interactive launch lifecycle, first-frame non-completion, user/app/host/failure close tracking, outcome compatibility fields, and desktop-session precheck in `src/SkiaViewer/SkiaViewer.fs`
- [X] T021 [US1] [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input] Implement generated app host interpretation for pure update, rendered view refresh, input observation, emitted effects, and close handling without conflating evidence close with user close
- [X] T022 [US1] [skillist: fs-skia-skiaviewer, fs-skia-scene, fs-skia-testing] Update `template/base/src/Product/Program.fs`, generated README/help text, and generated tests so the default executable path reaches the persistent visible-window launch
- [X] T023 [US1] [skillist: fs-skia-skiaviewer, fs-skia-testing] Wire generated validation helpers and FAKE targets to prove interactive persistence, supported-host visible-window accessibility, close reason reporting, and no bounded-only substitution
- [X] T024 [US1] [skillist: []] Record `readiness/interactive-visible-window.md` and `readiness/close-reason-separation.md` with command evidence, supported-host visibility facts, first-frame persistence, close criteria, and any fake-loop disclosure

**Checkpoint**: US1 is independently testable through the generated default executable path.

---

## Phase 4: User Story 2 - Diagnose Taskbar-Only Or Invisible Windows (P1)

### Tests First

- [X] T025 [P] [US2] [skillist: fs-skia-skiaviewer] Add semantic tests for taskbar-only, hidden, minimized-only, off-screen, unmapped, zero-sized, surface-less, and unsupported session classifications as degraded/failed rather than visible launch success
- [X] T026 [P] [US2] [skillist: fs-skia-skiaviewer, fs-skia-testing] Add generated diagnostic tests requiring environment/session, window-visibility, app-lifecycle, and product-defect failure classes with observable-vs-unsupported native facts

### Implementation

- [X] T027 [US2] [skillist: fs-skia-skiaviewer] Implement window state diagnostic model and interpreter reads for initialized, handle, visible, focusable, focused, minimized, maximized, client size, renderable surface, backend, and input-device facts
- [X] T028 [US2] [skillist: fs-skia-skiaviewer] Implement classification for taskbar-only, hidden, minimized-only, off-screen, unmapped, zero-sized, surface-less, unsupported, unavailable, and environment/session failures with actionable messages
- [X] T029 [US2] [skillist: fs-skia-skiaviewer, fs-skia-testing] Wire generated app/container readiness commands so diagnostics run before app lifecycle debugging and do not silently switch to private runtime fallback or evidence mode
- [X] T030 [US2] [skillist: speckit-evidence-audit] Update audit/readiness checks for visible-window substitution, missing diagnostic classes, unsupported-host-only claims, and process/taskbar-only success claims
- [X] T031 [US2] [skillist: []] Record `readiness/window-state-diagnostics.md` with invalid configuration matrix, observable native facts, unsupported fields, failure-class examples, and supported-host integration notes

**Checkpoint**: US2 diagnostics isolate invisible-window and unsupported-session failures before app lifecycle debugging.

---

## Phase 5: User Story 3 - Capture Inspectable Visual Evidence (P2)

### Tests First

- [X] T032 [P] [US3] [skillist: fs-skia-skiaviewer, fs-skia-scene] Add visual evidence tests requiring requested screenshot/image artifacts to be decodable images, metadata/hash evidence to be labeled separately, and scene-rendering vs desktop-visibility claims to be explicit
- [X] T033 [P] [US3] [skillist: fs-skia-skiaviewer, fs-skia-scene, fs-skia-testing] Add generated command tests for image evidence success, pixel-readback fallback, metadata/hash output, unsupported-host output, and rejection of text hashes mislabeled as screenshots

### Implementation

- [X] T034 [US3] [skillist: fs-skia-skiaviewer, fs-skia-scene] Implement visual evidence artifact model, image capture or render-surface image output, decodability validation, pixel-readback fallback, metadata/hash labeling, and unsupported-host diagnostics
- [X] T035 [US3] [skillist: fs-skia-skiaviewer, fs-skia-scene, fs-skia-testing] Wire generated CLI/workflow image evidence commands and generated validation output fields without changing the default interactive launch path; packed generated consumer image evidence consumed in T047
- [X] T036 [US3] [skillist: speckit-evidence-audit] Update audit/readiness checks to reject requested image evidence that is metadata-only, undecodable, missing proof fields, or claiming desktop visibility from scene-only pixel readback
- [X] T037 [US3] [skillist: []] Record `readiness/real-image-evidence.md` with artifact paths, decodability proof, scene-rendering claim, desktop-visibility claim, fallback reason, and unsupported-host reason when applicable; updated to packed-package generated consumer evidence from T047

**Checkpoint**: US3 evidence artifacts are inspectable and accurately labeled.

---

## Phase 6: User Story 4 - Configure Expected Window Behavior (P2)

### Tests First

- [X] T038 [P] [US4] [skillist: fs-skia-skiaviewer] Add semantic tests for public window behavior request validation covering resize, maximize, startup state, startup position, backend preference, positive size constraints, invalid coordinates, and unsupported backend reporting
- [X] T039 [P] [US4] [skillist: fs-skia-skiaviewer, fs-skia-testing] Add generated app tests for each supported window behavior setting and diagnostics when a host cannot honor the requested behavior

### Implementation

- [X] T040 [US4] [skillist: fs-skia-skiaviewer] Implement public window behavior request parsing, validation, native option application, observed option result collection, fallback/degraded behavior, and failure-class reporting
- [X] T041 [US4] [skillist: fs-skia-skiaviewer, fs-skia-testing] Wire generated app option files/CLI flags/templates and generated validation outputs for resize, maximize, startup state, startup position, and backend preference; packed `runAppWithWindowBehavior` verified in T047
- [X] T042 [US4] [skillist: speckit-evidence-audit] Update audit/readiness checks for missing option rows, silently ignored unsupported settings, and window-options failures hidden under generic app-lifecycle messages
- [X] T043 [US4] [skillist: []] Record `readiness/window-options.md` with one row per requested option, requested/observed values, honored/degraded/unsupported/failed status, and host-specific messages; readiness updated with T047 packed generated consumer evidence

**Checkpoint**: US4 window behavior settings are honored or explicitly diagnosed.

---

## Phase 7: Generated Validation, Integration & Polish

- [X] T044 [skillist: fs-skia-testing] Implement generated validation contract output for package resolution, generated test execution, default interactive launch validation, bounded evidence validation, close reason validation, window diagnostics, window options, image evidence, authoritative flag, and failure class
- [X] T045 [skillist: fs-skia-testing] Update `GeneratedProductCheck`, generated `Test`, generated `Verify`, `TemplateCheck`, `GeneratedGuidanceCheck`, and package verification targets so misleading or incomplete generated app claims fail validation
- [X] T046 [skillist: fs-skia-skiaviewer] Refresh `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt` with `./fake.sh build -t RefreshSurfaceBaselines` and verify with `./fake.sh build -t PackageSurfaceCheck`
- [X] T047 [skillist: fs-skia-skiaviewer, fs-skia-testing] Run `./fake.sh build -t PackLocal`, generated consumer restore/test/verify, `./fake.sh build -t TemplateCheck`, and `./fake.sh build -t GeneratedProductCheck`; record package compatibility and generated-product evidence — `PackLocal`, `TemplateCheck`, and `GeneratedProductCheck` passed after routing persistent launches through the Vulkan/Skia presenter; see `readiness/logs/t047-retry-pack-local-presenter-tests.txt`, `readiness/logs/t047-retry-template-check-presenter-tests.txt`, `readiness/logs/t047-retry-generated-product-check-presenter-tests.txt`, and `readiness/generated-product-validation.md`
- [X] T048 [skillist: []] Run documentation and dependency governance checks, including `./fake.sh build -t DependencyReport`, and record no-new-dependency or package impact findings — DependencyReport passed and package impact was recorded for the SkiaViewer presenter bridge; see `readiness/logs/t048-dependency-report-after-docs.txt`, `readiness/dependency-report.md`, `readiness/dependencies.md`, and `readiness/dependency-governance.md`
- [X] T049 [skillist: []] Record `readiness/generated-validation.md` with restore/package evidence, generated test command evidence, interactive validation, bounded evidence validation, option validation, image evidence validation, authoritative verdict, and failure classes — authoritative generated validation recorded with package, generated Verify, interactive launch, bounded, options, image, verdict, and failure-class evidence
- [X] T050 [skillist: []] Define the supported-host matrix and repeated-launch attempt count used for SC-003, then record host/session/backend coverage and exception classification rules in `readiness/generated-validation.md` — supported-host matrix, 20-attempt/95% SC-003 rule, current Wayland/GPU-passthrough coverage, and exception classifications recorded
- [X] T051 [skillist: fs-skia-testing] Capture generated validation elapsed time and fail or record blocking diagnostics when required checks exceed 5 minutes on a prepared supported host — dedicated `GeneratedProductCheck` timing passed with `elapsed-seconds=100`, `exit-code=0`; see `readiness/logs/t051-generated-product-check-elapsed.txt` and `readiness/generated-validation.md`
- [X] T052 [skillist: fs-skia-skiaviewer, fs-skia-testing] Capture command-launch-to-manual-ready timing and record whether manual interactive testing begins within 30 seconds without an environment-variable workaround — direct generated command launch completed with visible/accessibility diagnostics and manual close in `5` seconds without env-var workaround; see `readiness/logs/t052-command-launch-manual-ready.txt` and `readiness/generated-validation.md`
- [X] T053 [skillist: speckit-evidence-graph] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/019-fix-window-visibility --graph-only` after story readiness records exist and capture clean graph output in `readiness/evidence-graph.md` — graph-only audit passed and refreshed `readiness/evidence-graph.md`, `readiness/task-graph.md`, and `readiness/task-graph.json`
- [X] T054 [skillist: speckit-evidence-graph, speckit-evidence-audit] Run `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`, then capture PASS or blocking diagnostics in `readiness/evidence-audit.md` — `EvidenceGraph` passed; `EvidenceAudit` failed with exit code `2` and blocking diagnostics captured in `readiness/evidence-audit.md`
- [X] T055 [skillist: speckit-tasks, speckit-implement] Run `./fake.sh build -t GeneratedGuidanceCheck` and confirm generated task/implementation guidance preserves visible interactive launch, explicit evidence modes, skillist metadata, `[SEH]` validation, and risk-level rules — target passed; see `readiness/logs/t055-generated-guidance-check.txt`
- [X] T056 [skillist: []] Run `./fake.sh build -t Verify` for broad Tier 1 validation or record explicit non-authoritative aggregate diagnostics with focused rerun evidence — `Verify` reached the broad test chain after readiness preflight fixes; `Elmish.Tests` was fixed and passed, then the aggregate hung in `Smoke.Tests` after more than five minutes, so non-authoritative aggregate diagnostics were recorded in `readiness/aggregate-hang-diagnostics.md`
- [X] T057 [skillist: speckit-evidence-audit] Complete final readiness review with all seven required readiness files, supported-host visible-window or unsupported-host diagnostics, package/test verification evidence, real image evidence, synthetic inventory, no bounded-only completion claims, and no unaccepted synthetic propagation — final `EvidenceAudit` passed with `unaccepted-synthetic-tasks=0`, `auto-synthetic-tasks=0`, and `diff-scan-hits=0`; see `readiness/logs/t057-evidence-audit-after-synthetic-resolution.txt`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the source for the PR description's synthetic-evidence section. For `[SEH]` rows, include the approval label, design-phase source, synthetic input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T014 | Design-approved malformed/error-path validation may require synthetic corrupt records and hostile paths rather than corrupting real readiness evidence | `specs/019-fix-window-visibility/readiness/logs/t014-synthetic-error-evidence.txt` | n/a | synthetic-error-handling-approved | `specs/019-fix-window-visibility/plan.md` Synthetic Evidence and readiness/generated-validation contracts | malformed readiness rows, invalid evidence command arguments, corrupt image metadata records, missing generated-validation fields, hostile artifact paths | Validator or audit reports explicit validation errors without treating malformed records, placeholder artifacts, or hostile paths as success | accepted-seh |

Resolved ordinary synthetic rows:

- T035/T037 image-evidence fallback was replaced by T047 packed generated consumer evidence in `readiness/generated-consumer-validation/image-evidence.log` and `readiness/generated-consumer-validation/game-image-evidence.png`.
- T041/T043 window-options fallback was replaced by T047 packed generated consumer evidence in `readiness/generated-consumer-validation/window-options.log` and `readiness/generated-consumer-validation/window-options.txt`.

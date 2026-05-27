# Task Graph — 019-fix-window-visibility

## ✓ Graph is acyclic and consistent

## Skill Match Assessments

| Task | Candidate | Confidence | Signals | Reviewer disposition | Diagnostic |
|------|-----------|------------|---------|----------------------|------------|
| T001 | (none) | none |  | declared | T001: no high-confidence capability signal detected |
| T002 | (none) | none |  | accepted-empty | T002: no high-confidence capability signal detected |
| T003 | speckit-tasks | high | task-text | accepted | T003: task text matches speckit-tasks |
| T004 | (none) | none |  | accepted-empty | T004: no high-confidence capability signal detected |
| T005 | (none) | none |  | declared | T005: no high-confidence capability signal detected |
| T006 | (none) | none |  | declared | T006: no high-confidence capability signal detected |
| T007 | (none) | none |  | declared | T007: no high-confidence capability signal detected |
| T008 | (none) | none |  | declared | T008: no high-confidence capability signal detected |
| T009 | (none) | none |  | declared | T009: no high-confidence capability signal detected |
| T010 | (none) | none |  | declared | T010: no high-confidence capability signal detected |
| T011 | (none) | none |  | declared | T011: no high-confidence capability signal detected |
| T012 | (none) | none |  | declared | T012: no high-confidence capability signal detected |
| T013 | (none) | none |  | declared | T013: no high-confidence capability signal detected |
| T014 | (none) | none |  | declared | T014: no high-confidence capability signal detected |
| T015 | (none) | none |  | declared | T015: no high-confidence capability signal detected |
| T016 | (none) | none |  | accepted-empty | T016: no high-confidence capability signal detected |
| T017 | (none) | none |  | declared | T017: no high-confidence capability signal detected |
| T018 | (none) | none |  | declared | T018: no high-confidence capability signal detected |
| T019 | (none) | none |  | declared | T019: no high-confidence capability signal detected |
| T020 | (none) | none |  | declared | T020: no high-confidence capability signal detected |
| T021 | (none) | none |  | declared | T021: no high-confidence capability signal detected |
| T022 | (none) | none |  | declared | T022: no high-confidence capability signal detected |
| T023 | (none) | none |  | declared | T023: no high-confidence capability signal detected |
| T024 | (none) | none |  | accepted-empty | T024: no high-confidence capability signal detected |
| T025 | (none) | none |  | declared | T025: no high-confidence capability signal detected |
| T026 | (none) | none |  | declared | T026: no high-confidence capability signal detected |
| T027 | (none) | none |  | declared | T027: no high-confidence capability signal detected |
| T028 | (none) | none |  | declared | T028: no high-confidence capability signal detected |
| T029 | (none) | none |  | declared | T029: no high-confidence capability signal detected |
| T030 | (none) | none |  | declared | T030: no high-confidence capability signal detected |
| T031 | (none) | none |  | accepted-empty | T031: no high-confidence capability signal detected |
| T032 | (none) | none |  | declared | T032: no high-confidence capability signal detected |
| T033 | (none) | none |  | declared | T033: no high-confidence capability signal detected |
| T034 | (none) | none |  | declared | T034: no high-confidence capability signal detected |
| T035 | (none) | none |  | declared | T035: no high-confidence capability signal detected |
| T036 | (none) | none |  | declared | T036: no high-confidence capability signal detected |
| T037 | (none) | none |  | accepted-empty | T037: no high-confidence capability signal detected |
| T038 | (none) | none |  | declared | T038: no high-confidence capability signal detected |
| T039 | (none) | none |  | declared | T039: no high-confidence capability signal detected |
| T040 | (none) | none |  | declared | T040: no high-confidence capability signal detected |
| T041 | (none) | none |  | declared | T041: no high-confidence capability signal detected |
| T042 | (none) | none |  | declared | T042: no high-confidence capability signal detected |
| T043 | (none) | none |  | accepted-empty | T043: no high-confidence capability signal detected |
| T044 | (none) | none |  | declared | T044: no high-confidence capability signal detected |
| T045 | (none) | none |  | declared | T045: no high-confidence capability signal detected |
| T046 | (none) | none |  | declared | T046: no high-confidence capability signal detected |
| T047 | (none) | none |  | declared | T047: no high-confidence capability signal detected |
| T048 | (none) | none |  | accepted-empty | T048: no high-confidence capability signal detected |
| T049 | (none) | none |  | accepted-empty | T049: no high-confidence capability signal detected |
| T050 | (none) | none |  | accepted-empty | T050: no high-confidence capability signal detected |
| T051 | (none) | none |  | declared | T051: no high-confidence capability signal detected |
| T052 | (none) | none |  | declared | T052: no high-confidence capability signal detected |
| T053 | (none) | none |  | declared | T053: no high-confidence capability signal detected |
| T054 | speckit-evidence-graph | high | task-text | accepted | T054: task text matches speckit-evidence-graph |
| T054 | speckit-evidence-audit | high | task-text | accepted | T054: task text matches speckit-evidence-audit |
| T055 | (none) | none |  | declared | T055: no high-confidence capability signal detected |
| T056 | (none) | none |  | accepted-empty | T056: no high-confidence capability signal detected |
| T057 | speckit-evidence-audit | high | task-text | accepted | T057: task text matches speckit-evidence-audit |

## Status counts (effective)

| Status | Count |
|--------|-------|
| [X] done | 56 |
| [S] synthetic | 1 |
| [S*] auto-synthetic | 0 |
| accepted [SEH] synthetic | 1 |
| unaccepted synthetic | 0 |

## Synthetic Error-Handling Classification

| Task | Accepted | Label | Design source | Synthetic input class | Expected error behavior | Diagnostics |
|------|----------|-------|---------------|-----------------------|-------------------------|-------------|
| T014 | yes | yes | `specs/019-fix-window-visibility/plan.md` Synthetic Evidence and readiness/generated-validation contracts | malformed readiness rows, invalid evidence command arguments, corrupt image metadata records, missing generated-validation fields, hostile artifact paths | Validator or audit reports explicit validation errors without treating malformed records, placeholder artifacts, or hostile paths as success | (none) |

## Graph

```mermaid
graph TD
  T001["T001 Create `specs/019-fix-window-visibility/readiness/"]:::done
  T002["T002 Record Tier 1 scope, public API impact, generated "]:::done
  T003["T003 Record task-generation assumptions, skill confiden"]:::done
  T004["T004 Inventory affected source, template, tests, docs, "]:::done
  T005["T005 Draft `src/SkiaViewer/SkiaViewer.fsi` contracts fo"]:::done
  T006["T006 Add failing-first SkiaViewer semantic tests for in"]:::done
  T007["T007 Add failing-first MVU lifecycle tests for checking"]:::done
  T008["T008 Add generated template/product tests proving the d"]:::done
  T009["T009 Add generated validation tests for exact package r"]:::done
  T010["T010 Add visual evidence tests requiring decodable imag"]:::done
  T011["T011 Add window option tests for resize policy, maximiz"]:::done
  T012["T012 Add audit fixtures rejecting missing readiness fil"]:::done
  T013["T013 Add generated guidance tests for implementation ba"]:::done
  T014["T014 synthetic-error-handling-approved Add pre-implemen"]:::synthetic
  T015["T015 Prepare surface baseline refresh path for `readine"]:::done
  T016["T016 Document Elmish/MVU evidence obligations, fake win"]:::done
  T017["T017 Add semantic tests proving normal interactive laun"]:::done
  T018["T018 Add generated host tests for `init`, pure `update`"]:::done
  T019["T019 Add generated product validation that rejects defa"]:::done
  T020["T020 Implement interactive launch lifecycle, first-fram"]:::done
  T021["T021 Implement generated app host interpretation for pu"]:::done
  T022["T022 Update `template/base/src/Product/Program.fs`, gen"]:::done
  T023["T023 Wire generated validation helpers and FAKE targets"]:::done
  T024["T024 Record `readiness/interactive-visible-window.md` a"]:::done
  T025["T025 Add semantic tests for taskbar-only, hidden, minim"]:::done
  T026["T026 Add generated diagnostic tests requiring environme"]:::done
  T027["T027 Implement window state diagnostic model and interp"]:::done
  T028["T028 Implement classification for taskbar-only, hidden,"]:::done
  T029["T029 Wire generated app/container readiness commands so"]:::done
  T030["T030 Update audit/readiness checks for visible-window s"]:::done
  T031["T031 Record `readiness/window-state-diagnostics.md` wit"]:::done
  T032["T032 Add visual evidence tests requiring requested scre"]:::done
  T033["T033 Add generated command tests for image evidence suc"]:::done
  T034["T034 Implement visual evidence artifact model, image ca"]:::done
  T035["T035 Wire generated CLI/workflow image evidence command"]:::done
  T036["T036 Update audit/readiness checks to reject requested "]:::done
  T037["T037 Record `readiness/real-image-evidence.md` with art"]:::done
  T038["T038 Add semantic tests for public window behavior requ"]:::done
  T039["T039 Add generated app tests for each supported window "]:::done
  T040["T040 Implement public window behavior request parsing, "]:::done
  T041["T041 Wire generated app option files/CLI flags/template"]:::done
  T042["T042 Update audit/readiness checks for missing option r"]:::done
  T043["T043 Record `readiness/window-options.md` with one row "]:::done
  T044["T044 Implement generated validation contract output for"]:::done
  T045["T045 Update `GeneratedProductCheck`, generated `Test`, "]:::done
  T046["T046 Refresh `readiness/surface-baselines/FS.Skia.UI.Sk"]:::done
  T047["T047 Run `./fake.sh build -t PackLocal`, generated cons"]:::done
  T048["T048 Run documentation and dependency governance checks"]:::done
  T049["T049 Record `readiness/generated-validation.md` with re"]:::done
  T050["T050 Define the supported-host matrix and repeated-laun"]:::done
  T051["T051 Capture generated validation elapsed time and fail"]:::done
  T052["T052 Capture command-launch-to-manual-ready timing and "]:::done
  T053["T053 Run `.specify/extensions/evidence/scripts/bash/run"]:::done
  T054["T054 Run `./fake.sh build -t EvidenceGraph` and `./fake"]:::done
  T055["T055 Run `./fake.sh build -t GeneratedGuidanceCheck` an"]:::done
  T056["T056 Run `./fake.sh build -t Verify` for broad Tier 1 v"]:::done
  T057["T057 Complete final readiness review with all seven req"]:::done
  T004 --> T005
  T005 --> T006
  T004 --> T006
  T005 --> T007
  T004 --> T007
  T004 --> T008
  T004 --> T009
  T004 --> T010
  T004 --> T011
  T004 --> T012
  T004 --> T013
  T012 --> T014
  T004 --> T014
  T005 --> T015
  T004 --> T015
  T012 --> T016
  T013 --> T016
  T014 --> T016
  T004 --> T016
  T005 --> T017
  T006 --> T017
  T016 --> T017
  T005 --> T018
  T007 --> T018
  T016 --> T018
  T008 --> T019
  T016 --> T019
  T017 --> T020
  T016 --> T020
  T018 --> T021
  T020 --> T021
  T016 --> T021
  T019 --> T022
  T021 --> T022
  T016 --> T022
  T019 --> T023
  T020 --> T023
  T021 --> T023
  T022 --> T023
  T016 --> T023
  T020 --> T024
  T021 --> T024
  T022 --> T024
  T023 --> T024
  T016 --> T024
  T006 --> T025
  T024 --> T025
  T008 --> T026
  T025 --> T026
  T024 --> T026
  T025 --> T027
  T024 --> T027
  T025 --> T028
  T027 --> T028
  T024 --> T028
  T026 --> T029
  T028 --> T029
  T024 --> T029
  T012 --> T030
  T027 --> T030
  T028 --> T030
  T024 --> T030
  T027 --> T031
  T028 --> T031
  T029 --> T031
  T030 --> T031
  T024 --> T031
  T010 --> T032
  T031 --> T032
  T010 --> T033
  T032 --> T033
  T031 --> T033
  T032 --> T034
  T031 --> T034
  T033 --> T035
  T034 --> T035
  T031 --> T035
  T012 --> T036
  T034 --> T036
  T035 --> T036
  T031 --> T036
  T034 --> T037
  T035 --> T037
  T036 --> T037
  T031 --> T037
  T011 --> T038
  T037 --> T038
  T011 --> T039
  T038 --> T039
  T037 --> T039
  T038 --> T040
  T037 --> T040
  T039 --> T041
  T040 --> T041
  T037 --> T041
  T012 --> T042
  T040 --> T042
  T041 --> T042
  T037 --> T042
  T040 --> T043
  T041 --> T043
  T042 --> T043
  T037 --> T043
  T023 --> T044
  T029 --> T044
  T035 --> T044
  T041 --> T044
  T043 --> T044
  T013 --> T045
  T030 --> T045
  T036 --> T045
  T042 --> T045
  T044 --> T045
  T043 --> T045
  T015 --> T046
  T020 --> T046
  T027 --> T046
  T034 --> T046
  T040 --> T046
  T043 --> T046
  T022 --> T047
  T023 --> T047
  T035 --> T047
  T041 --> T047
  T044 --> T047
  T045 --> T047
  T046 --> T047
  T043 --> T047
  T002 --> T048
  T004 --> T048
  T044 --> T048
  T043 --> T048
  T024 --> T049
  T031 --> T049
  T037 --> T049
  T043 --> T049
  T044 --> T049
  T045 --> T049
  T049 --> T050
  T043 --> T050
  T044 --> T051
  T045 --> T051
  T049 --> T051
  T050 --> T051
  T043 --> T051
  T023 --> T052
  T024 --> T052
  T049 --> T052
  T050 --> T052
  T043 --> T052
  T001 --> T053
  T003 --> T053
  T024 --> T053
  T031 --> T053
  T037 --> T053
  T043 --> T053
  T049 --> T053
  T050 --> T053
  T051 --> T053
  T052 --> T053
  T030 --> T054
  T036 --> T054
  T042 --> T054
  T053 --> T054
  T043 --> T054
  T013 --> T055
  T045 --> T055
  T053 --> T055
  T043 --> T055
  T047 --> T056
  T048 --> T056
  T049 --> T056
  T051 --> T056
  T052 --> T056
  T054 --> T056
  T055 --> T056
  T043 --> T056
  T024 --> T057
  T031 --> T057
  T037 --> T057
  T043 --> T057
  T049 --> T057
  T050 --> T057
  T051 --> T057
  T052 --> T057
  T054 --> T057
  T056 --> T057
  classDef pending fill:#eeeeee,stroke:#999
  classDef done fill:#c8e6c9,stroke:#2e7d32
  classDef synthetic fill:#ffe0b2,stroke:#e65100,stroke-width:2px
  classDef autoSynthetic fill:#ffab91,stroke:#bf360c,stroke-width:2px,stroke-dasharray:5 3
  classDef failed fill:#ffcdd2,stroke:#b71c1c,stroke-width:2px
  classDef skipped fill:#f5f5f5,stroke:#666,stroke-dasharray:3 3
```

## ASCII view

```
T001 [X] Create `specs/019-fix-window-visibility/readiness/` and scaffold required readiness files for interactive visible window, close reason separation, window diagnostics, window options, real image evidence, generated validation, and audit, plus supporting governance graph output
T002 [X] Record Tier 1 scope, public API impact, generated product impact, package impact, unsupported scope, and evidence obligations in `readiness/evidence-obligations.md`
T003 [X] Record task-generation assumptions, skill confidence review, valid-empty skill dispositions, `[SEH]` approval rationale, graph validation expectations, and risk-level evidence rules in `readiness/task-generation.md`
T004 [X] Inventory affected source, template, tests, docs, FAKE targets, package guidance, fixtures, and readiness paths named by the spec, plan, contracts, and quickstart
T005 [X] Draft `src/SkiaViewer/SkiaViewer.fsi` contracts for close reasons, observed values, window behavior requests, option results, launch outcomes, visual evidence artifacts, `Model`/`Msg`/`Effect`, `init`, pure `update`, and interpreter boundaries
T006 [X] Add failing-first SkiaViewer semantic tests for interactive/evidence mode separation, first-frame non-completion, taskbar-only degradation, close reason derivation, and visibility diagnostic fields
T007 [X] Add failing-first MVU lifecycle tests for checking-session, starting-window, visibility-checking, interactive-running, evidence-running, close-requested, unsupported, inaccessible-window, timeout, and failure transitions plus emitted effects
T008 [X] Add generated template/product tests proving the default command uses interactive visible-window launch and explicit commands are required for bounded, image, pixel-readback, or metadata evidence
T009 [X] Add generated validation tests for exact package resolution, generated tests ran, authoritative flags, failure classes, `NU1603`, missing package source, placeholder success, and non-authoritative source scans
T010 [X] Add visual evidence tests requiring decodable image artifacts for requested image evidence, clear metadata/hash labeling, scene-vs-desktop-visibility claims, and unsupported-host diagnostics
T011 [X] Add window option tests for resize policy, maximize policy, startup state, startup position, backend preference, honored/degraded/unsupported/failed results, and fallback messages
T012 [X] Add audit fixtures rejecting missing readiness files, process/taskbar-only visible-window substitution, evidence close reported as user close, metadata-only screenshot claims, unresolved package mismatch, and missing generated test execution
T013 [X] Add generated guidance tests for implementation batches, skill loading, graph refresh before/after status changes, red-green evidence logs, persistent launch rules, and non-authoritative aggregate reporting
T014 [S] synthetic-error-handling-approved Add pre-implementation synthetic error-handling validation for malformed readiness rows, invalid evidence command arguments, corrupt image metadata records, missing required generated-validation fields, and hostile artifact paths; approved synthetic error-handling evidence recorded in `readiness/logs/t014-synthetic-error-evidence.txt`   ← accepted [SEH]
T015 [X] Prepare surface baseline refresh path for `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt` without refreshing the baseline during task generation
T016 [X] Document Elmish/MVU evidence obligations, fake window-loop limits, required real interpreter evidence, supported-host visible-window requirements, small/medium/broad validation levels, broad-validation trigger, and non-authoritative aggregate handling
T017 [X] Add semantic tests proving normal interactive launch remains open after first-frame presentation, reports `mode=interactive-window`, does not self-close for evidence, and only completes after user/app/host/failure close
T018 [X] Add generated host tests for `init`, pure `update`, emitted effects, first-frame state, input-device observation, manual input availability, and explicit user/native close
T019 [X] Add generated product validation that rejects default commands which exit after first frame, run bounded evidence, print metadata only, or lack an accessible desktop window claim
T020 [X] Implement interactive launch lifecycle, first-frame non-completion, user/app/host/failure close tracking, outcome compatibility fields, and desktop-session precheck in `src/SkiaViewer/SkiaViewer.fs`
T021 [X] Implement generated app host interpretation for pure update, rendered view refresh, input observation, emitted effects, and close handling without conflating evidence close with user close
T022 [X] Update `template/base/src/Product/Program.fs`, generated README/help text, and generated tests so the default executable path reaches the persistent visible-window launch
T023 [X] Wire generated validation helpers and FAKE targets to prove interactive persistence, supported-host visible-window accessibility, close reason reporting, and no bounded-only substitution
T024 [X] Record `readiness/interactive-visible-window.md` and `readiness/close-reason-separation.md` with command evidence, supported-host visibility facts, first-frame persistence, close criteria, and any fake-loop disclosure
T025 [X] Add semantic tests for taskbar-only, hidden, minimized-only, off-screen, unmapped, zero-sized, surface-less, and unsupported session classifications as degraded/failed rather than visible launch success
T026 [X] Add generated diagnostic tests requiring environment/session, window-visibility, app-lifecycle, and product-defect failure classes with observable-vs-unsupported native facts
T027 [X] Implement window state diagnostic model and interpreter reads for initialized, handle, visible, focusable, focused, minimized, maximized, client size, renderable surface, backend, and input-device facts
T028 [X] Implement classification for taskbar-only, hidden, minimized-only, off-screen, unmapped, zero-sized, surface-less, unsupported, unavailable, and environment/session failures with actionable messages
T029 [X] Wire generated app/container readiness commands so diagnostics run before app lifecycle debugging and do not silently switch to private runtime fallback or evidence mode
T030 [X] Update audit/readiness checks for visible-window substitution, missing diagnostic classes, unsupported-host-only claims, and process/taskbar-only success claims
T031 [X] Record `readiness/window-state-diagnostics.md` with invalid configuration matrix, observable native facts, unsupported fields, failure-class examples, and supported-host integration notes
T032 [X] Add visual evidence tests requiring requested screenshot/image artifacts to be decodable images, metadata/hash evidence to be labeled separately, and scene-rendering vs desktop-visibility claims to be explicit
T033 [X] Add generated command tests for image evidence success, pixel-readback fallback, metadata/hash output, unsupported-host output, and rejection of text hashes mislabeled as screenshots
T034 [X] Implement visual evidence artifact model, image capture or render-surface image output, decodability validation, pixel-readback fallback, metadata/hash labeling, and unsupported-host diagnostics
T035 [X] Wire generated CLI/workflow image evidence commands and generated validation output fields without changing the default interactive launch path; packed generated consumer image evidence consumed in T047
T036 [X] Update audit/readiness checks to reject requested image evidence that is metadata-only, undecodable, missing proof fields, or claiming desktop visibility from scene-only pixel readback
T037 [X] Record `readiness/real-image-evidence.md` with artifact paths, decodability proof, scene-rendering claim, desktop-visibility claim, fallback reason, and unsupported-host reason when applicable; updated to packed-package generated consumer evidence from T047
T038 [X] Add semantic tests for public window behavior request validation covering resize, maximize, startup state, startup position, backend preference, positive size constraints, invalid coordinates, and unsupported backend reporting
T039 [X] Add generated app tests for each supported window behavior setting and diagnostics when a host cannot honor the requested behavior
T040 [X] Implement public window behavior request parsing, validation, native option application, observed option result collection, fallback/degraded behavior, and failure-class reporting
T041 [X] Wire generated app option files/CLI flags/templates and generated validation outputs for resize, maximize, startup state, startup position, and backend preference; packed `runAppWithWindowBehavior` verified in T047
T042 [X] Update audit/readiness checks for missing option rows, silently ignored unsupported settings, and window-options failures hidden under generic app-lifecycle messages
T043 [X] Record `readiness/window-options.md` with one row per requested option, requested/observed values, honored/degraded/unsupported/failed status, and host-specific messages; readiness updated with T047 packed generated consumer evidence
T044 [X] Implement generated validation contract output for package resolution, generated test execution, default interactive launch validation, bounded evidence validation, close reason validation, window diagnostics, window options, image evidence, authoritative flag, and failure class
T045 [X] Update `GeneratedProductCheck`, generated `Test`, generated `Verify`, `TemplateCheck`, `GeneratedGuidanceCheck`, and package verification targets so misleading or incomplete generated app claims fail validation
T046 [X] Refresh `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt` with `./fake.sh build -t RefreshSurfaceBaselines` and verify with `./fake.sh build -t PackageSurfaceCheck`
T047 [X] Run `./fake.sh build -t PackLocal`, generated consumer restore/test/verify, `./fake.sh build -t TemplateCheck`, and `./fake.sh build -t GeneratedProductCheck`; record package compatibility and generated-product evidence — `PackLocal`, `TemplateCheck`, and `GeneratedProductCheck` passed after routing persistent launches through the Vulkan/Skia presenter; see `readiness/logs/t047-retry-pack-local-presenter-tests.txt`, `readiness/logs/t047-retry-template-check-presenter-tests.txt`, `readiness/logs/t047-retry-generated-product-check-presenter-tests.txt`, and `readiness/generated-product-validation.md`
T048 [X] Run documentation and dependency governance checks, including `./fake.sh build -t DependencyReport`, and record no-new-dependency or package impact findings — DependencyReport passed and package impact was recorded for the SkiaViewer presenter bridge; see `readiness/logs/t048-dependency-report-after-docs.txt`, `readiness/dependency-report.md`, `readiness/dependencies.md`, and `readiness/dependency-governance.md`
T049 [X] Record `readiness/generated-validation.md` with restore/package evidence, generated test command evidence, interactive validation, bounded evidence validation, option validation, image evidence validation, authoritative verdict, and failure classes — authoritative generated validation recorded with package, generated Verify, interactive launch, bounded, options, image, verdict, and failure-class evidence
T050 [X] Define the supported-host matrix and repeated-launch attempt count used for SC-003, then record host/session/backend coverage and exception classification rules in `readiness/generated-validation.md` — supported-host matrix, 20-attempt/95% SC-003 rule, current Wayland/GPU-passthrough coverage, and exception classifications recorded
T051 [X] Capture generated validation elapsed time and fail or record blocking diagnostics when required checks exceed 5 minutes on a prepared supported host — dedicated `GeneratedProductCheck` timing passed with `elapsed-seconds=100`, `exit-code=0`; see `readiness/logs/t051-generated-product-check-elapsed.txt` and `readiness/generated-validation.md`
T052 [X] Capture command-launch-to-manual-ready timing and record whether manual interactive testing begins within 30 seconds without an environment-variable workaround — direct generated command launch completed with visible/accessibility diagnostics and manual close in `5` seconds without env-var workaround; see `readiness/logs/t052-command-launch-manual-ready.txt` and `readiness/generated-validation.md`
T053 [X] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/019-fix-window-visibility --graph-only` after story readiness records exist and capture clean graph output in `readiness/evidence-graph.md` — graph-only audit passed and refreshed `readiness/evidence-graph.md`, `readiness/task-graph.md`, and `readiness/task-graph.json`
T054 [X] Run `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`, then capture PASS or blocking diagnostics in `readiness/evidence-audit.md` — `EvidenceGraph` passed; `EvidenceAudit` failed with exit code `2` and blocking diagnostics captured in `readiness/evidence-audit.md`
T055 [X] Run `./fake.sh build -t GeneratedGuidanceCheck` and confirm generated task/implementation guidance preserves visible interactive launch, explicit evidence modes, skillist metadata, `[SEH]` validation, and risk-level rules — target passed; see `readiness/logs/t055-generated-guidance-check.txt`
T056 [X] Run `./fake.sh build -t Verify` for broad Tier 1 validation or record explicit non-authoritative aggregate diagnostics with focused rerun evidence — `Verify` reached the broad test chain after readiness preflight fixes; `Elmish.Tests` was fixed and passed, then the aggregate hung in `Smoke.Tests` after more than five minutes, so non-authoritative aggregate diagnostics were recorded in `readiness/aggregate-hang-diagnostics.md`
T057 [X] Complete final readiness review with all seven required readiness files, supported-host visible-window or unsupported-host diagnostics, package/test verification evidence, real image evidence, synthetic inventory, no bounded-only completion claims, and no unaccepted synthetic propagation — final `EvidenceAudit` passed with `unaccepted-synthetic-tasks=0`, `auto-synthetic-tasks=0`, and `diff-scan-hits=0`; see `readiness/logs/t057-evidence-audit-after-synthetic-resolution.txt`
```


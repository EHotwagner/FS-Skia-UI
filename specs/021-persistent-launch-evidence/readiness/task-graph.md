# Task Graph — 021-persistent-launch-evidence

## ✓ Graph is acyclic and consistent

## Skill Match Assessments

| Task | Candidate | Confidence | Signals | Reviewer disposition | Diagnostic |
|------|-----------|------------|---------|----------------------|------------|
| T001 | (none) | none |  | accepted-empty | T001: no high-confidence capability signal detected |
| T002 | (none) | none |  | accepted-empty | T002: no high-confidence capability signal detected |
| T003 | (none) | none |  | accepted-empty | T003: no high-confidence capability signal detected |
| T004 | (none) | none |  | accepted-empty | T004: no high-confidence capability signal detected |
| T005 | (none) | none |  | declared | T005: no high-confidence capability signal detected |
| T006 | (none) | none |  | declared | T006: no high-confidence capability signal detected |
| T007 | (none) | none |  | declared | T007: no high-confidence capability signal detected |
| T008 | speckit-evidence-graph | high | task-text | accepted | T008: task text matches speckit-evidence-graph |
| T008 | speckit-evidence-audit | high | task-text | accepted | T008: task text matches speckit-evidence-audit |
| T009 | (none) | none |  | declared | T009: no high-confidence capability signal detected |
| T010 | (none) | none |  | declared | T010: no high-confidence capability signal detected |
| T011 | (none) | none |  | accepted-empty | T011: no high-confidence capability signal detected |
| T012 | (none) | none |  | declared | T012: no high-confidence capability signal detected |
| T013 | (none) | none |  | declared | T013: no high-confidence capability signal detected |
| T014 | (none) | none |  | declared | T014: no high-confidence capability signal detected |
| T015 | (none) | none |  | declared | T015: no high-confidence capability signal detected |
| T016 | (none) | none |  | declared | T016: no high-confidence capability signal detected |
| T017 | (none) | none |  | declared | T017: no high-confidence capability signal detected |
| T018 | (none) | none |  | declared | T018: no high-confidence capability signal detected |
| T019 | (none) | none |  | declared | T019: no high-confidence capability signal detected |
| T020 | (none) | none |  | declared | T020: no high-confidence capability signal detected |
| T021 | (none) | none |  | declared | T021: no high-confidence capability signal detected |
| T022 | (none) | none |  | declared | T022: no high-confidence capability signal detected |
| T023 | (none) | none |  | declared | T023: no high-confidence capability signal detected |
| T024 | (none) | none |  | declared | T024: no high-confidence capability signal detected |
| T025 | (none) | none |  | declared | T025: no high-confidence capability signal detected |
| T026 | (none) | none |  | declared | T026: no high-confidence capability signal detected |
| T027 | (none) | none |  | declared | T027: no high-confidence capability signal detected |
| T028 | (none) | none |  | declared | T028: no high-confidence capability signal detected |
| T029 | (none) | none |  | declared | T029: no high-confidence capability signal detected |
| T030 | (none) | none |  | declared | T030: no high-confidence capability signal detected |
| T031 | (none) | none |  | declared | T031: no high-confidence capability signal detected |
| T032 | (none) | none |  | declared | T032: no high-confidence capability signal detected |
| T033 | (none) | none |  | declared | T033: no high-confidence capability signal detected |
| T034 | (none) | none |  | declared | T034: no high-confidence capability signal detected |
| T035 | (none) | none |  | declared | T035: no high-confidence capability signal detected |
| T036 | (none) | none |  | declared | T036: no high-confidence capability signal detected |
| T037 | speckit-evidence-graph | high | task-text | accepted | T037: task text matches speckit-evidence-graph |
| T038 | speckit-evidence-audit | high | task-text | accepted | T038: task text matches speckit-evidence-audit |
| T039 | (none) | none |  | accepted-empty | T039: no high-confidence capability signal detected |
| T040 | (none) | none |  | declared | T040: no high-confidence capability signal detected |

## Status counts (effective)

| Status | Count |
|--------|-------|
| [X] done | 38 |
| [S] synthetic | 2 |
| [S*] auto-synthetic | 0 |
| accepted [SEH] synthetic | 2 |
| unaccepted synthetic | 0 |

## Synthetic Error-Handling Classification

| Task | Accepted | Label | Design source | Synthetic input class | Expected error behavior | Diagnostics |
|------|----------|-------|---------------|-----------------------|-------------------------|-------------|
| T021 | yes | yes | `specs/021-persistent-launch-evidence/contracts/persistent-launch-evidence-contract.md`; `specs/021-persistent-launch-evidence/contracts/evidence-audit-contract.md` | Missing required fields, invalid enum values, and contradictory `status=ok` claims without real launch facts. | Reject artifact, identify missing or contradictory facts, and never satisfy supported-host persistent-launch readiness. | (none) |
| T024 | yes | yes | `specs/021-persistent-launch-evidence/contracts/persistent-launch-evidence-contract.md`; `specs/021-persistent-launch-evidence/contracts/evidence-audit-contract.md` | Missing required fields, invalid enum values, and contradictory `status=ok` claims without real launch facts. | Reject artifact, identify missing or contradictory facts, and never satisfy supported-host persistent-launch readiness. | (none) |

## Graph

```mermaid
graph TD
  T001["T001 Create `specs/021-persistent-launch-evidence/readi"]:::done
  T002["T002 Record feature tier, affected packages, build-targ"]:::done
  T003["T003 Record MVU applicability: persistent launch is I/O"]:::done
  T004["T004 Record the initial capability-skill evaluation not"]:::done
  T005["T005 Draft `src/SkiaViewer/SkiaViewer.fsi` persistent-l"]:::done
  T006["T006 Draft `src/Testing/Testing.fsi` host warning, gene"]:::done
  T007["T007 Define generated graphical app readiness command s"]:::done
  T008["T008 Define build-target coverage for `Verify`, generat"]:::done
  T009["T009 Exercise the draft public contracts from FSI and c"]:::done
  T010["T010 Prepare package surface baseline expectations for "]:::done
  T011["T011 Record readiness-file discovery requirements and m"]:::done
  T012["T012 Add SkiaViewer semantic tests for persistent-launc"]:::done
  T013["T013 Add artifact serialization tests requiring `status"]:::done
  T014["T014 Add generated product readiness tests that run the"]:::done
  T015["T015 Implement SkiaViewer persistent-launch request/res"]:::done
  T016["T016 Implement first-frame, viewer-owned window identit"]:::done
  T017["T017 Wire generated app evidence-mode launch and readin"]:::done
  T018["T018 Produce `readiness/persistent-launch-evidence.md` "]:::done
  T019["T019 Add tests for desktop prerequisite, process launch"]:::done
  T020["T020 Add tests proving external title/window search fai"]:::done
  T021["T021 synthetic-error-handling-approved Add malformed pe"]:::synthetic
  T022["T022 Implement window-observation diagnostics with diag"]:::done
  T023["T023 Implement observation/capture classification so ex"]:::done
  T024["T024 synthetic-error-handling-approved Implement artifa"]:::synthetic
  T025["T025 Produce `readiness/window-observation-diagnostics."]:::done
  T026["T026 Add host warning classification tests for known GT"]:::done
  T027["T027 Add fatal-preservation tests showing launch, rende"]:::done
  T028["T028 Implement host warning classification results with"]:::done
  T029["T029 Produce `readiness/host-warning-classification.md`"]:::done
  T030["T030 Add generated guidance checks requiring `Product.P"]:::done
  T031["T031 Add generated guidance checks that layout evidence"]:::done
  T032["T032 Update generated docs, samples, and tests to use a"]:::done
  T033["T033 Update generated readiness guidance so persistent-"]:::done
  T034["T034 Produce `readiness/generated-guidance.md` with gen"]:::done
  T035["T035 Refresh public surface baselines for changed SkiaV"]:::done
  T036["T036 Run targeted package, generated product, generated"]:::done
  T037["T037 Run `./fake.sh build -t EvidenceGraph` and confirm"]:::done
  T038["T038 Run `./fake.sh build -t EvidenceAudit`, produce `r"]:::done
  T039["T039 Run `./fake.sh build -t Verify` for broad validati"]:::done
  T040["T040 Run repeated supported-host persistent-launch atte"]:::done
  T004 --> T005
  T004 --> T006
  T004 --> T007
  T004 --> T008
  T005 --> T009
  T006 --> T009
  T004 --> T009
  T005 --> T010
  T006 --> T010
  T004 --> T010
  T004 --> T011
  T005 --> T012
  T011 --> T012
  T005 --> T013
  T011 --> T013
  T007 --> T014
  T011 --> T014
  T012 --> T015
  T011 --> T015
  T013 --> T016
  T015 --> T016
  T011 --> T016
  T014 --> T017
  T016 --> T017
  T011 --> T017
  T016 --> T018
  T017 --> T018
  T011 --> T018
  T005 --> T019
  T018 --> T019
  T005 --> T020
  T007 --> T020
  T018 --> T020
  T006 --> T021
  T018 --> T021
  T019 --> T022
  T018 --> T022
  T020 --> T023
  T022 --> T023
  T018 --> T023
  T021 --> T024
  T018 --> T024
  T023 --> T025
  T024 --> T025
  T018 --> T025
  T006 --> T026
  T025 --> T026
  T006 --> T027
  T025 --> T027
  T026 --> T028
  T027 --> T028
  T025 --> T028
  T028 --> T029
  T025 --> T029
  T007 --> T030
  T029 --> T030
  T007 --> T031
  T029 --> T031
  T030 --> T032
  T029 --> T032
  T031 --> T033
  T029 --> T033
  T032 --> T034
  T033 --> T034
  T029 --> T034
  T010 --> T035
  T016 --> T035
  T024 --> T035
  T028 --> T035
  T034 --> T035
  T018 --> T036
  T025 --> T036
  T029 --> T036
  T034 --> T036
  T035 --> T036
  T036 --> T037
  T034 --> T037
  T037 --> T038
  T034 --> T038
  T038 --> T039
  T034 --> T039
  T018 --> T040
  T034 --> T040
  classDef pending fill:#eeeeee,stroke:#999
  classDef done fill:#c8e6c9,stroke:#2e7d32
  classDef synthetic fill:#ffe0b2,stroke:#e65100,stroke-width:2px
  classDef autoSynthetic fill:#ffab91,stroke:#bf360c,stroke-width:2px,stroke-dasharray:5 3
  classDef failed fill:#ffcdd2,stroke:#b71c1c,stroke-width:2px
  classDef skipped fill:#f5f5f5,stroke:#666,stroke-dasharray:3 3
```

## ASCII view

```
T001 [X] Create `specs/021-persistent-launch-evidence/readiness/` and placeholder names for the five required readiness files.
T002 [X] Record feature tier, affected packages, build-target impact, public-API impact, unsupported scope, and broad validation obligations in readiness notes.
T003 [X] Record MVU applicability: persistent launch is I/O-bearing and requires `Model`, `Msg`, `Effect`, `init`, `update`, emitted-effect tests, and interpreter evidence.
T004 [X] Record the initial capability-skill evaluation notes for valid-empty tasks plus the required `fs-skia-layout-evidence` matches.
T005 [X] Draft `src/SkiaViewer/SkiaViewer.fsi` persistent-launch request, artifact, outcome, window fact, blocked-stage, `Model`, `Msg`, `Effect`, `init`, `update`, and interpreter-boundary signatures.
T006 [X] Draft `src/Testing/Testing.fsi` host warning, generated guidance, persistent artifact validation, and readiness-file discovery contracts.
T007 [X] Define generated graphical app readiness command shape, artifact path, app-qualified naming rules, and separation of layout evidence from persistent-window evidence.
T008 [X] Define build-target coverage for `Verify`, generated `Test`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`, `EvidenceGraph`, and `EvidenceAudit`.
T009 [X] Exercise the draft public contracts from FSI and capture representative request/result/update/effect transcript expectations in `readiness/fsi-session.txt`.
T010 [X] Prepare package surface baseline expectations for `FS.Skia.UI.SkiaViewer` and `FS.Skia.UI.Testing`.
T011 [X] Record readiness-file discovery requirements and missing-fact diagnostics for unsupported or blocked hosts.
T012 [X] Add SkiaViewer semantic tests for persistent-launch `init`/`update` transitions, emitted effects, first-frame recording, input-dispatch recording, and controlled-close state.
T013 [X] Add artifact serialization tests requiring `status`, `mode`, `command`, `window-opened`, `input-dispatch`, `exit-path`, `blocked-stage`, `classification`, `category`, `message`, and first-frame facts.
T014 [X] Add generated product readiness tests that run the explicit evidence-mode command without changing the normal persistent default launch. Evidence: `readiness/logs/t014-generated-product-check-green.txt`.
T015 [X] Implement SkiaViewer persistent-launch request/result model, pure update transitions, emitted effects, and edge interpreter hooks.
T016 [X] Implement first-frame, viewer-owned window identity, input-dispatch status, controlled evidence close, close reason, and artifact serialization.
T017 [X] Wire generated app evidence-mode launch and readiness artifact writing while preserving default user-driven persistent launch.
T018 [X] Produce `readiness/persistent-launch-evidence.md` from a supported-host real launch or record the exact blocked prerequisite stage without claiming pass.
T019 [X] Add tests for desktop prerequisite, process launch, window creation, first-frame/render, observation, capture, input verification, controlled-exit, and artifact-write blocked stages.
T020 [X] Add tests proving external title/window search failure cannot produce headless-only classification when viewer-owned facts and a live process exist.
T021 [S] synthetic-error-handling-approved Add malformed persistent-launch artifact parser tests for missing required fields, invalid field values, and contradictory pass claims.   ← accepted [SEH]
T022 [X] Implement window-observation diagnostics with diagnostic source, host facts, viewer facts, external observation facts, capture facts, missing facts, blocked stage, classification, and message.
T023 [X] Implement observation/capture classification so external observation failure stays observation/capture blocked when desktop prerequisites and viewer-owned launch facts are present.
T024 [S] synthetic-error-handling-approved Implement artifact validation diagnostics for missing fields, synthetic fixture rejection, contradictory pass claims, and actionable messages.   ← accepted [SEH]
T025 [X] Produce `readiness/window-observation-diagnostics.md` with real launch, generic host probe, and any synthetic fixture distinctions disclosed.
T026 [X] Add host warning classification tests for known GTK/module warnings paired with passing launch, first-frame/render, and exit facts.
T027 [X] Add fatal-preservation tests showing launch, rendering, layout, package, and artifact-write failures remain fatal even with benign warning text present.
T028 [X] Implement host warning classification results with raw message, warning class, fatal flag, evidence path, supporting facts, and diagnostics.
T029 [X] Produce `readiness/host-warning-classification.md` showing benign warnings preserved as non-blocking only when required real launch facts pass.
T030 [X] Add generated guidance checks requiring `Product.Program.view`, `Product.Program.generatedHost`, and `Product.Program.update` when framework capability namespaces are open.
T031 [X] Add generated guidance checks that layout evidence, deterministic render hashes, and persistent-window launch evidence are documented as separate proof types.
T032 [X] Update generated docs, samples, and tests to use app-qualified scene, host, and update names in collision-prone contexts.
T033 [X] Update generated readiness guidance so persistent-launch evidence is not described as layout, screenshot, or deterministic render proof.
T034 [X] Produce `readiness/generated-guidance.md` with generated guidance checks and any remaining naming or evidence-separation diagnostics.
T035 [X] Refresh public surface baselines for changed SkiaViewer and Testing contracts with `./fake.sh build -t RefreshSurfaceBaselines`, then verify with `./fake.sh build -t PackageSurfaceCheck`.
T036 [X] Run targeted package, generated product, generated guidance, template, and readiness checks; record small/medium/broad validation results and non-authoritative aggregate notes.
T037 [X] Run `./fake.sh build -t EvidenceGraph` and confirm no cycles, dangling refs, mirror mismatches, or invalid skill metadata.
T038 [X] Run `./fake.sh build -t EvidenceAudit`, produce `readiness/evidence-audit.md`, and confirm required readiness files and persistent-launch artifact fields are internally consistent.
T039 [X] Run `./fake.sh build -t Verify` for broad validation and record any host-specific unsupported prerequisites separately from feature failures.
T040 [X] Run repeated supported-host persistent-launch attempts, record pass ratio against the 95% SC-001 threshold, and classify every failed attempt by blocked stage in `readiness/persistent-launch-evidence.md`.
```


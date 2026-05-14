# Task Graph — 006-template-framework-governance

## ✓ Graph is acyclic and consistent

## Status counts (effective)

| Status | Count |
|--------|-------|
| [X] done | 50 |
| [S] synthetic | 0 |
| [S*] auto-synthetic | 0 |

## Graph

```mermaid
graph TD
  T001["T001 Create feature readiness scaffolding under `specs/"]:::done
  T002["T002 Record the FAKE local-tool and wrapper adoption ba"]:::done
  T003["T003 Inventory existing duplicated restore/build/test/p"]:::done
  T004["T004 Record Tier 1 governance obligations in `readiness"]:::done
  T005["T005 Create a command-target traceability matrix mappin"]:::done
  T006["T006 Add failing command-contract checks for wrapper av"]:::done
  T007["T007 Add failing package-surface checks proving current"]:::done
  T008["T008 Add failing guidance checks proving touched docs, "]:::done
  T009["T009 Add failing verification checks for required v1 ar"]:::done
  T010["T010 Add the repo-local FAKE tool manifest and thin Bas"]:::done
  T011["T011 Implement `build.fsx` foundation helpers and local"]:::done
  T012["T012 Implement the `Dev` target as the fast restore/bui"]:::done
  T013["T013 Isolate existing package consumer smoke behavior b"]:::done
  T014["T014 Run foundation verification for wrapper discovery,"]:::done
  T015["T015 Add command availability tests for `Dev`, `Verify`"]:::done
  T016["T016 Add target behavior tests or fixtures for workflow"]:::done
  T017["T017 Add a real-interpreter evidence plan for running `"]:::done
  T018["T018 Implement `PackLocal` to pack `src/Lib`, `src/Char"]:::done
  T019["T019 Implement `RefreshSurfaceBaselines`, `PackageSurfa"]:::done
  T020["T020 Implement `Verify` as the full v1 workflow requiri"]:::done
  T021["T021 Implement `Ci` as the non-interactive automation e"]:::done
  T022["T022 Capture independent US1 validation evidence by run"]:::done
  T023["T023 Add docs checks for `docs/build.md`, `docs/testing"]:::done
  T024["T024 Add docs checks proving template packaging, depend"]:::done
  T025["T025 Write `docs/build.md` with canonical wrapper usage"]:::done
  T026["T026 Write `docs/testing.md` with target-to-test mappin"]:::done
  T027["T027 Write `docs/evidence.md` with v1 artifact classes,"]:::done
  T028["T028 Update README or existing workflow documentation t"]:::done
  T029["T029 Add package surface tests proving `tests/Package.T"]:::done
  T030["T030 Add refresh-path tests proving `RefreshSurfaceBase"]:::done
  T031["T031 Add artifact-path checks for build/test/package lo"]:::done
  T032["T032 Create root `readiness/surface-baselines/` and see"]:::done
  T033["T033 Update `scripts/refresh-surface-baselines.fsx` and"]:::done
  T034["T034 Route build/test/package logs, FSI transcripts, sa"]:::done
  T035["T035 Remove v1 checks' dependence on historical readine"]:::done
  T036["T036 Capture stable-baseline and evidence-location vali"]:::done
  T037["T037 Add automation inspection checks for `.specify/wor"]:::done
  T038["T038 Add generated task guidance checks for `.specify/p"]:::done
  T039["T039 Update `.specify/workflows/speckit/workflow.yml` i"]:::done
  T040["T040 Update `.specify/presets/fsharp-opinionated/templa"]:::done
  T041["T041 Review `.agents/skills/speckit-tasks/SKILL.md` and"]:::done
  T042["T042 Capture automation and generated-guidance alignmen"]:::done
  T043["T043 Run `./fake.sh build -t RefreshSurfaceBaselines` a"]:::done
  T044["T044 Run `./fake.sh build -t Dev`; store the log and re"]:::done
  T045["T045 Run `./fake.sh build -t Verify` from a clean check"]:::done
  T046["T046 Run `./fake.sh build -t PackLocal`; confirm local "]:::done
  T047["T047 Run `.specify/extensions/evidence/scripts/bash/run"]:::done
  T048["T048 Run `.specify/extensions/evidence/scripts/bash/run"]:::done
  T049["T049 Update quickstart, contract, and plan references o"]:::done
  T050["T050 Prepare the merge summary with command results, ev"]:::done
  T005 --> T006
  T005 --> T007
  T005 --> T008
  T005 --> T009
  T006 --> T010
  T005 --> T010
  T006 --> T011
  T010 --> T011
  T005 --> T011
  T006 --> T012
  T011 --> T012
  T005 --> T012
  T007 --> T013
  T012 --> T013
  T005 --> T013
  T006 --> T014
  T007 --> T014
  T008 --> T014
  T009 --> T014
  T010 --> T014
  T011 --> T014
  T012 --> T014
  T013 --> T014
  T005 --> T014
  T014 --> T015
  T014 --> T016
  T014 --> T017
  T015 --> T018
  T016 --> T018
  T014 --> T018
  T015 --> T019
  T016 --> T019
  T017 --> T019
  T014 --> T019
  T015 --> T020
  T016 --> T020
  T018 --> T020
  T019 --> T020
  T014 --> T020
  T015 --> T021
  T020 --> T021
  T014 --> T021
  T017 --> T022
  T020 --> T022
  T021 --> T022
  T014 --> T022
  T022 --> T023
  T022 --> T024
  T023 --> T025
  T024 --> T025
  T022 --> T025
  T023 --> T026
  T024 --> T026
  T022 --> T026
  T023 --> T027
  T024 --> T027
  T022 --> T027
  T023 --> T028
  T024 --> T028
  T025 --> T028
  T026 --> T028
  T027 --> T028
  T022 --> T028
  T028 --> T029
  T028 --> T030
  T028 --> T031
  T029 --> T032
  T030 --> T032
  T028 --> T032
  T029 --> T033
  T030 --> T033
  T032 --> T033
  T028 --> T033
  T031 --> T034
  T033 --> T034
  T028 --> T034
  T029 --> T035
  T032 --> T035
  T033 --> T035
  T028 --> T035
  T019 --> T036
  T020 --> T036
  T030 --> T036
  T031 --> T036
  T034 --> T036
  T035 --> T036
  T028 --> T036
  T036 --> T037
  T036 --> T038
  T021 --> T039
  T037 --> T039
  T036 --> T039
  T020 --> T040
  T038 --> T040
  T036 --> T040
  T038 --> T041
  T040 --> T041
  T036 --> T041
  T037 --> T042
  T038 --> T042
  T039 --> T042
  T040 --> T042
  T041 --> T042
  T036 --> T042
  T033 --> T043
  T036 --> T043
  T042 --> T043
  T012 --> T044
  T020 --> T044
  T021 --> T044
  T034 --> T044
  T042 --> T044
  T020 --> T045
  T021 --> T045
  T034 --> T045
  T036 --> T045
  T039 --> T045
  T040 --> T045
  T043 --> T045
  T044 --> T045
  T042 --> T045
  T018 --> T046
  T043 --> T046
  T042 --> T046
  T042 --> T047
  T045 --> T048
  T047 --> T048
  T042 --> T048
  T025 --> T049
  T026 --> T049
  T027 --> T049
  T028 --> T049
  T035 --> T049
  T042 --> T049
  T045 --> T049
  T048 --> T049
  T045 --> T050
  T046 --> T050
  T048 --> T050
  T049 --> T050
  T042 --> T050
  classDef pending fill:#eeeeee,stroke:#999
  classDef done fill:#c8e6c9,stroke:#2e7d32
  classDef synthetic fill:#ffe0b2,stroke:#e65100,stroke-width:2px
  classDef autoSynthetic fill:#ffab91,stroke:#bf360c,stroke-width:2px,stroke-dasharray:5 3
  classDef failed fill:#ffcdd2,stroke:#b71c1c,stroke-width:2px
  classDef skipped fill:#f5f5f5,stroke:#666,stroke-dasharray:3 3
```

## ASCII view

```
T001 [X] Create feature readiness scaffolding under `specs/006-template-framework-governance/readiness/` for logs, FSI transcripts, sample smoke output, package notes, graph output, and audit output
T002 [X] Record the FAKE local-tool and wrapper adoption baseline for `.config/dotnet-tools.json`, `fake.sh`, and `fake.cmd`
T003 [X] Inventory existing duplicated restore/build/test/pack/evidence command order in README, docs, scripts, tests, and `.specify/workflows/speckit/workflow.yml`
T004 [X] Record Tier 1 governance obligations in `readiness/evidence-obligations.md`, including no runtime `.fsi` API impact, `BuildModel` / `BuildMsg` / `BuildEffect`, required v1 artifacts, and deferred roadmap categories
T005 [X] Create a command-target traceability matrix mapping `contracts/canonical-workflow.md` targets to planned implementation files, docs, tests, and readiness artifacts
T006 [X] Add failing command-contract checks for wrapper availability, required target names, target dependencies, `BuildModel` / `BuildMsg` / `BuildEffect`, pure `update` behavior, emitted effects, and documented pass/fail behavior
T007 [X] Add failing package-surface checks proving current baselines must be read from `readiness/surface-baselines/*.txt` instead of historical feature readiness folders
T008 [X] Add failing guidance checks proving touched docs, workflows, and generated task guidance reference canonical targets instead of duplicating raw command order
T009 [X] Add failing verification checks for required v1 artifact classes and actionable diagnostics when `Verify` is missing build, test, package, FSI, sample-smoke, task-graph, or audit output
T010 [X] Add the repo-local FAKE tool manifest and thin Bash/Windows wrappers that invoke the same target graph
T011 [X] Implement `build.fsx` foundation helpers and local `BuildModel` / `BuildMsg` / `BuildEffect` workflow effect algebra for repository paths, process execution, log capture, output directories, `Clean`, `Restore`, `Build`, `Test`, and target discovery
T012 [X] Implement the `Dev` target as the fast restore/build/default non-visual test path, keeping deferred package consumer smoke outside the default test set
T013 [X] Isolate existing package consumer smoke behavior behind a deferred path or explicit non-v1 target so `Dev`, `Verify`, and `Ci` do not require it
T014 [X] Run foundation verification for wrapper discovery, command-contract checks, stable-baseline checks, guidance checks, and artifact-diagnostic checks; store logs under `readiness/logs/`
T015 [X] Add command availability tests for `Dev`, `Verify`, `Ci`, `PackLocal`, `RefreshSurfaceBaselines`, `PackageSurfaceCheck`, `FsiTranscripts`, `SampleContractSmoke`, `EvidenceGraph`, and `EvidenceAudit`
T016 [X] Add target behavior tests or fixtures for workflow transition outputs, emitted process/file effects, log capture, required artifact detection, and missing-artifact diagnostics in the full verification path
T017 [X] Add a real-interpreter evidence plan for running `./fake.sh build -t Dev` and focused evidence targets with output captured under feature readiness
T018 [X] Implement `PackLocal` to pack `src/Lib`, `src/Charts`, and `src/Layout` into `~/.local/share/nuget-local/` and capture a package log
T019 [X] Implement `RefreshSurfaceBaselines`, `PackageSurfaceCheck`, `FsiTranscripts`, `SampleContractSmoke`, `EvidenceGraph`, and `EvidenceAudit` targets by wrapping existing tests, scripts, sample smoke commands, and the Spec Kit evidence extension through the build workflow interpreter
T020 [X] Implement `Verify` as the full v1 workflow requiring `Dev`, package surface checks, public contract transcripts, sample smoke output, task graph validation, evidence audit output, and build/test/package logs
T021 [X] Implement `Ci` as the non-interactive automation entry that delegates to `Verify` without duplicating command order
T022 [X] Capture independent US1 validation evidence by running `Dev` and focused graph/surface targets, then store logs and artifact paths under `readiness/`
T023 [X] Add docs checks for `docs/build.md`, `docs/testing.md`, and `docs/evidence.md` covering delivered target names, artifact paths, pass/fail behavior, and stable baseline location
T024 [X] Add docs checks proving template packaging, dependency governance, generated spec/plan hardening, layout evidence, visual evidence, package consumer smoke, and release validation are named as deferred and excluded from v1 `Dev`, `Verify`, and `Ci`
T025 [X] Write `docs/build.md` with canonical wrapper usage, target responsibilities, output locations, future CI guidance, and no duplicated raw command sequence
T026 [X] Write `docs/testing.md` with target-to-test mapping, default non-visual test scope, sample smoke expectations, and package consumer smoke deferral
T027 [X] Write `docs/evidence.md` with v1 artifact classes, stable paths, historical-vs-current evidence rules, synthetic evidence policy, and roadmap extension points
T028 [X] Update README or existing workflow documentation to point to the canonical docs and targets without reimplementing the command order
T029 [X] Add package surface tests proving `tests/Package.Tests/SurfaceAreaTests.fs` reads root-level `readiness/surface-baselines/*.txt` and fails when expected public names are missing
T030 [X] Add refresh-path tests proving `RefreshSurfaceBaselines`, `scripts/refresh-surface-baselines.fsx`, and `PackageSurfaceCheck` write and read the same stable current baseline location
T031 [X] Add artifact-path checks for build/test/package logs, FSI transcripts, sample smoke output, task graph output, and evidence audit output under the feature readiness directory
T032 [X] Create root `readiness/surface-baselines/` and seed `FS.Skia.UI.txt`, `FS.Skia.UI.Charts.txt`, and `FS.Skia.UI.Layout.txt` from the current validated public surface
T033 [X] Update `scripts/refresh-surface-baselines.fsx` and `tests/Package.Tests/SurfaceAreaTests.fs` to use the stable current baseline path
T034 [X] Route build/test/package logs, FSI transcripts, sample smoke output, task graph output, and audit output to the documented feature readiness paths
T035 [X] Remove v1 checks' dependence on historical readiness folders while preserving those folders as historical repository evidence
T036 [X] Capture stable-baseline and evidence-location validation by running `PackageSurfaceCheck`, `FsiTranscripts`, `SampleContractSmoke`, and `EvidenceGraph`
T037 [X] Add automation inspection checks for `.specify/workflows/speckit/workflow.yml` and any touched automation so verification delegates to `Ci`, `Verify`, or named canonical targets
T038 [X] Add generated task guidance checks for `.specify/presets/fsharp-opinionated/templates/tasks-template.md`, requiring canonical workflow entries and preserving `tasks.deps.yml` plus evidence graph requirements
T039 [X] Update `.specify/workflows/speckit/workflow.yml` if needed so repository automation invokes the canonical verification entry instead of duplicating command order
T040 [X] Update `.specify/presets/fsharp-opinionated/templates/tasks-template.md` so future generated tasks call canonical targets such as `Dev`, `Verify`, `PackLocal`, `RefreshSurfaceBaselines`, `PackageSurfaceCheck`, `EvidenceGraph`, and `EvidenceAudit`
T041 [X] Review `.agents/skills/speckit-tasks/SKILL.md` and either align it with canonical-target guidance or record why no skill change is needed
T042 [X] Capture automation and generated-guidance alignment evidence under `readiness/logs/` or `readiness/guidance-alignment.md`
T043 [X] Run `./fake.sh build -t RefreshSurfaceBaselines` and `./fake.sh build -t PackageSurfaceCheck`; store both logs under `readiness/logs/`
T044 [X] Run `./fake.sh build -t Dev`; store the log and record whether it completes within the 10 minute target on the current supported machine
T045 [X] Run `./fake.sh build -t Verify` from a clean checkout or freshly cloned working directory; confirm every required v1 artifact class exists and store build/test/package/evidence logs and machine/runtime assumptions under `readiness/`
T046 [X] Run `./fake.sh build -t PackLocal`; confirm local `.nupkg` outputs under `~/.local/share/nuget-local/` and store package evidence
T047 [X] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/006-template-framework-governance --graph-only` and confirm no cycles or dangling references
T048 [X] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/006-template-framework-governance` and confirm PASS, or document every unresolved synthetic or diff-scan blocker
T049 [X] Update quickstart, contract, and plan references only if final target names or artifact paths changed, then record the final readiness review
T050 [X] Prepare the merge summary with command results, evidence paths, synthetic-evidence inventory, and deferred roadmap boundaries
```


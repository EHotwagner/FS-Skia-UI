# Tasks: Template Framework Governance

**Feature branch**: `006-template-framework-governance`
**Spec**: `specs/006-template-framework-governance/spec.md`
**Plan**: `specs/006-template-framework-governance/plan.md`

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
from a user-facing or operator-facing entry point and that path was actually
exercised. For this feature, operator-facing entry points are the canonical
workflow commands such as `./fake.sh build -t Dev`, `./fake.sh build -t Verify`,
`./fake.sh build -t PackLocal`, and the generated task guidance that names
those commands.

This feature does not add a runtime product Elmish `Model` / `Msg` / `Effect`
API. Because the build workflow is process/file I/O-bearing, Principle IV is
satisfied through a local workflow effect algebra in `build.fsx`: `BuildModel`,
`BuildMsg`, `BuildEffect`, `init`, pure `update`, and an interpreter that
executes effects at the edge. `[X]` requires pure transition tests,
emitted-effect assertions, and real interpreter evidence where safe.

This rule does not apply to Setup, Foundation, Integration, or Polish phase
tasks; those are evaluated against their own phase verification.

## Task Annotations

- **[P]** - parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, ... - user-story scope
- **[T1]** / **[T2]** - Tier 1 (contracted) vs Tier 2 (internal) change

Every task must have a matching entry in `tasks.deps.yml` even if its
dependency list is empty. The evidence graph command refuses to proceed with
dangling references.

---

## Phase 1: Setup

- [X] T001 Create feature readiness scaffolding under `specs/006-template-framework-governance/readiness/` for logs, FSI transcripts, sample smoke output, package notes, graph output, and audit output
- [X] T002 [P] Record the FAKE local-tool and wrapper adoption baseline for `.config/dotnet-tools.json`, `fake.sh`, and `fake.cmd`
- [X] T003 [P] Inventory existing duplicated restore/build/test/pack/evidence command order in README, docs, scripts, tests, and `.specify/workflows/speckit/workflow.yml`
- [X] T004 [P] Record Tier 1 governance obligations in `readiness/evidence-obligations.md`, including no runtime `.fsi` API impact, `BuildModel` / `BuildMsg` / `BuildEffect`, required v1 artifacts, and deferred roadmap categories
- [X] T005 [P] Create a command-target traceability matrix mapping `contracts/canonical-workflow.md` targets to planned implementation files, docs, tests, and readiness artifacts

**Checkpoint**: Setup complete.

---

## Phase 2: Foundation

### Tests First

- [X] T006 [P] Add failing command-contract checks for wrapper availability, required target names, target dependencies, `BuildModel` / `BuildMsg` / `BuildEffect`, pure `update` behavior, emitted effects, and documented pass/fail behavior
- [X] T007 [P] Add failing package-surface checks proving current baselines must be read from `readiness/surface-baselines/*.txt` instead of historical feature readiness folders
- [X] T008 [P] Add failing guidance checks proving touched docs, workflows, and generated task guidance reference canonical targets instead of duplicating raw command order
- [X] T009 [P] Add failing verification checks for required v1 artifact classes and actionable diagnostics when `Verify` is missing build, test, package, FSI, sample-smoke, task-graph, or audit output

### Implementation

- [X] T010 Add the repo-local FAKE tool manifest and thin Bash/Windows wrappers that invoke the same target graph
- [X] T011 Implement `build.fsx` foundation helpers and local `BuildModel` / `BuildMsg` / `BuildEffect` workflow effect algebra for repository paths, process execution, log capture, output directories, `Clean`, `Restore`, `Build`, `Test`, and target discovery
- [X] T012 Implement the `Dev` target as the fast restore/build/default non-visual test path, keeping deferred package consumer smoke outside the default test set
- [X] T013 Isolate existing package consumer smoke behavior behind a deferred path or explicit non-v1 target so `Dev`, `Verify`, and `Ci` do not require it
- [X] T014 Run foundation verification for wrapper discovery, command-contract checks, stable-baseline checks, guidance checks, and artifact-diagnostic checks; store logs under `readiness/logs/`

**Checkpoint**: Foundation ready - story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) - Verify Through One Governed Workflow

### Tests First

- [X] T015 [P] [US1] Add command availability tests for `Dev`, `Verify`, `Ci`, `PackLocal`, `RefreshSurfaceBaselines`, `PackageSurfaceCheck`, `FsiTranscripts`, `SampleContractSmoke`, `EvidenceGraph`, and `EvidenceAudit`
- [X] T016 [P] [US1] Add target behavior tests or fixtures for workflow transition outputs, emitted process/file effects, log capture, required artifact detection, and missing-artifact diagnostics in the full verification path
- [X] T017 [P] [US1] Add a real-interpreter evidence plan for running `./fake.sh build -t Dev` and focused evidence targets with output captured under feature readiness

### Implementation

- [X] T018 [US1] Implement `PackLocal` to pack `src/Lib`, `src/Charts`, and `src/Layout` into `~/.local/share/nuget-local/` and capture a package log
- [X] T019 [US1] Implement `RefreshSurfaceBaselines`, `PackageSurfaceCheck`, `FsiTranscripts`, `SampleContractSmoke`, `EvidenceGraph`, and `EvidenceAudit` targets by wrapping existing tests, scripts, sample smoke commands, and the Spec Kit evidence extension through the build workflow interpreter
- [X] T020 [US1] Implement `Verify` as the full v1 workflow requiring `Dev`, package surface checks, public contract transcripts, sample smoke output, task graph validation, evidence audit output, and build/test/package logs
- [X] T021 [US1] Implement `Ci` as the non-interactive automation entry that delegates to `Verify` without duplicating command order
- [X] T022 [US1] Capture independent US1 validation evidence by running `Dev` and focused graph/surface targets, then store logs and artifact paths under `readiness/`

**Checkpoint**: US1 canonical workflow is independently testable.

---

## Phase 4: User Story 2 (US2) - Document The Roadmap Boundary

### Tests First

- [X] T023 [P] [US2] Add docs checks for `docs/build.md`, `docs/testing.md`, and `docs/evidence.md` covering delivered target names, artifact paths, pass/fail behavior, and stable baseline location
- [X] T024 [P] [US2] Add docs checks proving template packaging, dependency governance, generated spec/plan hardening, layout evidence, visual evidence, package consumer smoke, and release validation are named as deferred and excluded from v1 `Dev`, `Verify`, and `Ci`

### Implementation

- [X] T025 [US2] Write `docs/build.md` with canonical wrapper usage, target responsibilities, output locations, future CI guidance, and no duplicated raw command sequence
- [X] T026 [US2] Write `docs/testing.md` with target-to-test mapping, default non-visual test scope, sample smoke expectations, and package consumer smoke deferral
- [X] T027 [US2] Write `docs/evidence.md` with v1 artifact classes, stable paths, historical-vs-current evidence rules, synthetic evidence policy, and roadmap extension points
- [X] T028 [US2] Update README or existing workflow documentation to point to the canonical docs and targets without reimplementing the command order

**Checkpoint**: US2 documentation boundary is independently testable.

---

## Phase 5: User Story 3 (US3) - Stabilize Current Evidence And Baselines

### Tests First

- [X] T029 [P] [US3] Add package surface tests proving `tests/Package.Tests/SurfaceAreaTests.fs` reads root-level `readiness/surface-baselines/*.txt` and fails when expected public names are missing
- [X] T030 [P] [US3] Add refresh-path tests proving `RefreshSurfaceBaselines`, `scripts/refresh-surface-baselines.fsx`, and `PackageSurfaceCheck` write and read the same stable current baseline location
- [X] T031 [P] [US3] Add artifact-path checks for build/test/package logs, FSI transcripts, sample smoke output, task graph output, and evidence audit output under the feature readiness directory

### Implementation

- [X] T032 [US3] Create root `readiness/surface-baselines/` and seed `FS.Skia.UI.txt`, `FS.Skia.UI.Charts.txt`, and `FS.Skia.UI.Layout.txt` from the current validated public surface
- [X] T033 [US3] Update `scripts/refresh-surface-baselines.fsx` and `tests/Package.Tests/SurfaceAreaTests.fs` to use the stable current baseline path
- [X] T034 [US3] Route build/test/package logs, FSI transcripts, sample smoke output, task graph output, and audit output to the documented feature readiness paths
- [X] T035 [US3] Remove v1 checks' dependence on historical readiness folders while preserving those folders as historical repository evidence
- [X] T036 [US3] Capture stable-baseline and evidence-location validation by running `PackageSurfaceCheck`, `FsiTranscripts`, `SampleContractSmoke`, and `EvidenceGraph`

**Checkpoint**: US3 evidence and baselines are independently testable.

---

## Phase 6: User Story 4 (US4) - Keep Automation And Guidance Aligned

### Tests First

- [X] T037 [P] [US4] Add automation inspection checks for `.specify/workflows/speckit/workflow.yml` and any touched automation so verification delegates to `Ci`, `Verify`, or named canonical targets
- [X] T038 [P] [US4] Add generated task guidance checks for `.specify/presets/fsharp-opinionated/templates/tasks-template.md`, requiring canonical workflow entries and preserving `tasks.deps.yml` plus evidence graph requirements

### Implementation

- [X] T039 [US4] Update `.specify/workflows/speckit/workflow.yml` if needed so repository automation invokes the canonical verification entry instead of duplicating command order
- [X] T040 [US4] Update `.specify/presets/fsharp-opinionated/templates/tasks-template.md` so future generated tasks call canonical targets such as `Dev`, `Verify`, `PackLocal`, `RefreshSurfaceBaselines`, `PackageSurfaceCheck`, `EvidenceGraph`, and `EvidenceAudit`
- [X] T041 [US4] Review `.agents/skills/speckit-tasks/SKILL.md` and either align it with canonical-target guidance or record why no skill change is needed
- [X] T042 [US4] Capture automation and generated-guidance alignment evidence under `readiness/logs/` or `readiness/guidance-alignment.md`

**Checkpoint**: US4 automation and generated guidance are independently testable.

---

## Phase 7: Integration & Polish

- [X] T043 Run `./fake.sh build -t RefreshSurfaceBaselines` and `./fake.sh build -t PackageSurfaceCheck`; store both logs under `readiness/logs/`
- [X] T044 Run `./fake.sh build -t Dev`; store the log and record whether it completes within the 10 minute target on the current supported machine
- [X] T045 Run `./fake.sh build -t Verify` from a clean checkout or freshly cloned working directory; confirm every required v1 artifact class exists and store build/test/package/evidence logs and machine/runtime assumptions under `readiness/`
- [X] T046 Run `./fake.sh build -t PackLocal`; confirm local `.nupkg` outputs under `~/.local/share/nuget-local/` and store package evidence
- [X] T047 Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/006-template-framework-governance --graph-only` and confirm no cycles or dangling references
- [X] T048 Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/006-template-framework-governance` and confirm PASS, or document every unresolved synthetic or diff-scan blocker
- [X] T049 Update quickstart, contract, and plan references only if final target names or artifact paths changed, then record the final readiness review
- [X] T050 Prepare the merge summary with command results, evidence paths, synthetic-evidence inventory, and deferred roadmap boundaries

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| _(none yet)_ | | | |

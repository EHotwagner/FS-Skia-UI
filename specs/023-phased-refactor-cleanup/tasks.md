# Tasks: Phased Refactor Cleanup

**Feature branch**: `023-phased-refactor-cleanup`
**Spec**: `specs/023-phased-refactor-cleanup/spec.md`
**Plan**: `specs/023-phased-refactor-cleanup/plan.md`

## Status Legend

- `[ ]` - pending
- `[X]` - done with real evidence
- `[S]` - done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` - failed
- `[-]` - skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. No synthetic error-handling tasks are
approved for this feature at task generation time.

## Vertical-slice Rule

A task tagged `[US*]` may only be marked `[X]` when the change is reachable
from a user-facing generated command, FAKE target, viewer facade, or generated
profile path and that path was actually exercised. For stateful or I/O-bearing
stories, `[X]` also requires evidence that product or viewer state/effect
boundaries remain behaviorally unchanged, pure transition behavior is covered
where applicable, emitted effects are asserted where applicable, and real
interpreters are exercised where safe.

## Risk-Level Evidence

- **Small risk**: localized helper movement with no compile-order, generated
  profile, FAKE target, public facade, or report-field change. Run focused unit
  or target checks and record the result in the phase readiness file.
- **Medium risk**: generated source ownership, report writer consolidation,
  loaded build script extraction, or viewer internal boundary movement. Run the
  phase checks named in `quickstart.md` and record command, exit code, and
  changed files in the phase readiness file.
- **Broad risk**: any change affecting generated profile inclusion, FAKE target
  dependency wiring, public `.fsi` shape, surface baselines, package IDs, or
  readiness paths. Stop for Tier 1 redesign when public contract changes are
  required; otherwise run `Verify`, `PackageSurfaceCheck`, `EvidenceGraph`, and
  `EvidenceAudit` and record non-authoritative aggregate results as advisory
  alongside focused phase evidence.

## Phase 1: Setup

- [X] T001 [P] [skillist: []] Create `specs/023-phased-refactor-cleanup/readiness/` and placeholder readiness files for baseline, generated evidence cleanup, template split validation, build governance decomposition, and viewer internal boundary
- [X] T002 [P] [skillist: speckit-tasks] Record the task-generation skill review in readiness notes, including valid-empty dispositions and the absence of `fs-skia-template-update` for non-packaging generated source tasks
- [X] T003 [P] [skillist: []] Record feature Tier 2 scope, no-public-contract-change constraints, MVU/effect-boundary preservation requirements, and required real evidence paths
- [X] T004 [skillist: []] Capture initial branch, `git status --short`, selected baseline commands, and any pre-existing failures in `readiness/baseline-status.md`

**Checkpoint**: Setup complete - baseline and evidence locations exist.

## Phase 2: Foundation

- [X] T005 [P] [skillist: []] Inventory stable behavior contracts from the spec and contracts: generated command names, report fields, statuses, output paths, exit codes, profile names, FAKE targets, readiness paths, public signatures, and package IDs
- [X] T006 [P] [skillist: fs-skia-testing] Characterize generated evidence/report behavior in existing tests, including field names, status vocabulary, stdout echo behavior, parent directory creation, and exit-code meanings
- [X] T007 [P] [skillist: []] Classify duplicated helper families from `docs/2026-05-27-2204-refactoring-analysis.md` as consolidate, intentional boundary copy, or deferred, with verification coverage for each decision
- [X] T008 [P] [skillist: fs-skia-skiaviewer] Characterize current viewer diagnostics, host capability classifications, window validation, visual evidence, screenshot evidence, and unsupported-host messages behind the unchanged viewer facade
- [X] T009 [skillist: []] Define the implementation batch evidence log format for phase readiness files, including command, exit code, focused/broad risk level, changed ownership area, and pre-existing failure attribution

**Checkpoint**: Foundation ready - behavior contracts and phase evidence format are explicit.

## Phase 3: User Story 1 - Simplify Generated Product Evidence (P1)

### Tests First

- [X] T010 [P] [US1] [skillist: fs-skia-testing] Before changing generated evidence/report code, capture current branch status, focused baseline commands, exit codes, and any pre-existing failures in `readiness/generated-evidence-cleanup.md`; then add or tighten generated evidence command tests that assert unchanged required report fields, status vocabulary, output paths, stdout echo behavior, parent directory creation, and exit-code meanings before consolidation
- [X] T011 [P] [US1] [skillist: fs-skia-layout-evidence] Add or tighten generated layout/readability evidence checks so report-writer consolidation preserves HUD/gameplay bounds, proof levels, unsupported classifications, and diagnostics

### Implementation

- [X] T012 [P] [US1] [skillist: fs-skia-testing] Introduce one generated-product-local report writing path for equivalent evidence command output without changing generated command names or report schemas
- [X] T013 [US1] [skillist: fs-skia-testing, fs-skia-layout-evidence] Route generated evidence commands through the local report writer while preserving layout evidence fields, unsupported classifications, and command exit semantics
- [X] T014 [US1] [skillist: fs-skia-testing] Remove or consolidate drift-prone specialized generated report writers that no longer own unique behavior, keeping intentional template/package boundary copies documented
- [X] T015 [US1] [skillist: fs-skia-testing, fs-skia-layout-evidence] Run `dotnet test tests/Testing.Tests/Testing.Tests.fsproj`, `./fake.sh build -t TemplateCheck`, and `./fake.sh build -t GeneratedGuidanceCheck`; record commands, results, and verdict in `readiness/generated-evidence-cleanup.md`

**Checkpoint**: US1 independently verifies unchanged generated evidence command behavior.

## Phase 4: User Story 2 - Split Generated Product Responsibilities (P1)

### Tests First

- [X] T016 [P] [US2] [skillist: fs-skia-scene, fs-skia-skiaviewer, fs-skia-testing] Before changing generated source organization, capture current branch status, focused baseline commands, exit codes, and any pre-existing failures in `readiness/template-split-validation.md`; then add generated source-shape and compile-order expectations for product model, view, evidence commands, window options, layout evidence, and entrypoint responsibilities
- [X] T017 [P] [US2] [skillist: fs-skia-testing] Add generated profile validation expectations that every previously supported profile still instantiates, builds, and runs its generated tests without unnecessary testing-helper references

### Implementation

- [X] T018 [P] [US2] [skillist: fs-skia-scene] Extract generated product model, messages, update/state helpers, and pure scene/view description into responsibility-specific generated files
- [X] T019 [P] [US2] [skillist: fs-skia-layout-evidence] Extract generated layout evidence helpers into a responsibility-specific generated file while preserving readability proof levels and unsupported diagnostics
- [X] T020 [P] [US2] [skillist: fs-skia-skiaviewer] Extract generated viewer/window option behavior into a responsibility-specific generated file while preserving launch, host, and unsupported-window behavior
- [X] T021 [US2] [skillist: fs-skia-testing] Move generated evidence command implementations into the responsibility-specific generated file and reduce `Program.fs` to launch and command dispatch responsibilities
- [X] T022 [US2] [skillist: fs-skia-template-update] Update `template/base/src/Product/Product.fsproj` compile order and profile-conditioned generated file inclusion without changing template package IDs or generated profile names
- [X] T023 [US2] [skillist: fs-skia-testing] Update generated docs/tests only where they assert source ownership, preserving public command names, report fields, generated output paths, and exit-code meanings
- [X] T024 [US2] [skillist: fs-skia-testing, fs-skia-layout-evidence] Run `./fake.sh build -t TemplateCheck`, `./fake.sh build -t GeneratedGuidanceCheck`, and `./fake.sh build -t TemplateDrift`; record commands, results, generated profile coverage, and verdict in `readiness/template-split-validation.md`

**Checkpoint**: US2 independently verifies generated profiles build with split responsibilities.

## Phase 5: User Story 3 - Make Build Governance Easier To Maintain (P2)

### Tests First

- [X] T025 [P] [US3] [skillist: []] Before changing build governance helpers, capture current branch status, focused baseline commands, exit codes, and any pre-existing failures in `readiness/build-governance-decomposition.md`; then add or tighten focused governance assertions that preserve FAKE target names, dependency behavior, report outputs, readiness paths, missing-artifact classifications, and actionable failure wording
- [X] T026 [P] [US3] [skillist: []] Add or tighten checks for path, process execution, report writing, generated scanning, package resolution, template validation, and process-health helper behavior before extraction

### Implementation

- [X] T027 [P] [US3] [skillist: []] Extract build path and process execution helpers into loaded scripts under `scripts/build/` while preserving public FAKE target registration in `build.fsx`
- [X] T028 [P] [US3] [skillist: []] Extract report writing, scalar/list parsing, generated scanning, package resolution, template validation, and process-health policy helpers into loaded scripts with stable failure messages
- [X] T029 [US3] [skillist: speckit-evidence-graph, speckit-evidence-audit] Rewire `build.fsx` to load helper scripts in dependency order while keeping `Dev`, `Verify`, `Ci`, `PackLocal`, `DependencyReport`, `TemplateCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` stable
- [X] T030 [US3] [skillist: speckit-evidence-graph] Run focused moved-helper targets plus `./fake.sh build -t Dev`, `./fake.sh build -t Verify`, and `./fake.sh build -t EvidenceGraph`; record focused and aggregate results in `readiness/build-governance-decomposition.md`

**Checkpoint**: US3 independently verifies the public FAKE command surface remains stable.

## Phase 6: User Story 4 - Reduce Viewer Runtime Coordination Hotspots (P3)

### Tests First

- [X] T031 [P] [US4] [skillist: fs-skia-skiaviewer] Before changing viewer internals, capture current branch status, focused baseline commands, exit codes, and any pre-existing failures in `readiness/viewer-internal-boundary.md`; then add or tighten viewer tests for diagnostics filtering, host capability classification, window behavior validation, visual evidence artifacts, screenshot result handling, and unsupported-host classification
- [X] T032 [P] [US4] [skillist: fs-skia-skiaviewer, fs-skia-layout-evidence] Add or tighten governance checks that unsupported screenshot hosts keep explicit unsupported evidence and never claim screenshot proof

### Implementation

- [X] T033 [P] [US4] [skillist: fs-skia-skiaviewer] Extract legacy scene conversion and generated app host interpretation behind the unchanged `src/SkiaViewer/SkiaViewer.fsi` facade
- [X] T034 [P] [US4] [skillist: fs-skia-skiaviewer] Extract diagnostics filtering, desktop session detection, host capability classification, and window behavior validation into internal viewer modules
- [X] T035 [US4] [skillist: fs-skia-skiaviewer, fs-skia-layout-evidence] Extract visual evidence artifact generation and screenshot evidence result handling while preserving diagnostics, unsupported classifications, and existing failure wording
- [X] T036 [US4] [skillist: fs-skia-skiaviewer] Update `src/SkiaViewer/SkiaViewer.fsproj` compile order for new implementation-detail files, confirm no new public signed modules or surface baseline entries are introduced, and confirm `SkiaViewer.fsi` and existing surface baselines remain unchanged
- [X] T037 [US4] [skillist: fs-skia-skiaviewer, speckit-evidence-audit] Run `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj`, `dotnet test tests/Governance.Tests/Governance.Tests.fsproj`, and `./fake.sh build -t EvidenceAudit`; record commands, host classification, surface-baseline status, and verdict in `readiness/viewer-internal-boundary.md`

**Checkpoint**: US4 independently verifies viewer behavior and evidence classification are unchanged.

## Phase 7: Integration & Polish

- [X] T038 [P] [skillist: speckit-evidence-graph, speckit-evidence-audit] Run the evidence graph validation and refresh `readiness/task-graph.json` plus `readiness/task-graph.md`; confirm no cycles, dangling refs, mirror mismatches, or unexpected synthetic propagation
- [X] T039 [P] [skillist: []] Review public surface baselines, package IDs, generated profile names, generated command names, report fields, FAKE target names, and readiness paths; document any unchanged baseline evidence or Tier 1 stop condition
- [X] T040 [skillist: speckit-evidence-graph, speckit-evidence-audit] Run final `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`; record merge-readiness verdict and any non-authoritative aggregate results in readiness notes

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. For `[SEH]`
rows, include the approval label, design-phase source, synthetic input class,
expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |

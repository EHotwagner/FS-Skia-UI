# Tasks: Persistent Viewer Contract

**Feature branch**: `016-persistent-viewer-contract`
**Spec**: `specs/016-persistent-viewer-contract/spec.md`
**Plan**: `specs/016-persistent-viewer-contract/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

## Vertical-Slice Rule (US Phases)

A task tagged `[US*]` may only be marked `[X]` when the change is reachable
from the generated graphical app, SkiaViewer public contract, governance gate,
or documented command named in the task, and that path was actually exercised.
Pure implementation, source-only checks, or bounded evidence alone do not
satisfy `[X]` for interactive graphical readiness.

For this stateful and I/O-bearing feature, `[X]` also requires Elmish/MVU
evidence where applicable: the public `Model` / `Msg` / `ViewerEffect` or
`Cmd<Msg>` contract was exercised, pure `update` transitions were tested,
emitted effects were asserted, and the viewer-edge interpreter was run against
real dependencies where safe. The supported-host persistent launch task cannot
be completed with synthetic, bounded, or unsupported-host-only evidence.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, … — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors
the structured `skillist` value using `[skillist: ...]`.

## Skill Evaluation Notes

- High-confidence SkiaViewer tasks use `fs-skia-skiaviewer` for `src/SkiaViewer/`, viewer tests, generated viewer startup, and `template/fragments/skiaviewer/`.
- High-confidence MVU/generated host wiring uses `fs-skia-elmish`; keyboard-capable default launch and dispatch uses `fs-skia-keyboard-input`; pure scene construction uses `fs-skia-scene`.
- Generated product validation helper work uses `fs-skia-testing` when the task touches validation helper contracts or product test helpers.
- Evidence graph and audit tasks use `speckit-evidence-graph` and `speckit-evidence-audit` in prerequisite order. Task-generation guidance uses `speckit-tasks`; implementation guidance uses `speckit-implement`.
- Valid-empty `skillist: []` is used for broad docs, build orchestration, and readiness filing tasks with no single capability owner. Reviewer disposition: accepted-empty unless implementation discovers a narrower owner.

---

## Phase 1: Setup

- [X] T001 [skillist: speckit-evidence-graph, speckit-evidence-audit] Create readiness scaffolding for persistent viewer contract, generated default launch, bounded evidence separation, runtime diagnostics, generated guidance, evidence graph, evidence audit, surface baselines, and supported-host launch artifacts
- [X] T002 [P] [skillist: []] Record Tier 1 package/API/template/governance scope, public surface impact, generated product impact, and no-new-platform-support boundaries in `readiness/persistent-viewer-contract.md`
- [X] T003 [P] [skillist: speckit-tasks] Record task-generation assumptions, story grouping, skill confidence review, and valid-empty skill dispositions in `readiness/task-generation.md`
- [X] T004 [skillist: []] Inventory affected source, template, test, docs, FAKE target, fixture, and readiness paths named by the spec and plan

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: fs-skia-skiaviewer, fs-skia-scene, fs-skia-elmish, fs-skia-keyboard-input] Draft `src/SkiaViewer/SkiaViewer.fsi` with persistent `Viewer.run`, `Viewer.runApp`, runtime capability, launch outcome, diagnostics, `GeneratedAppHost`, `ViewerEffect`, keyboard mapping, tick, and viewer-edge interpreter boundary
- [X] T006 [P] [skillist: fs-skia-skiaviewer] Add failing-first SkiaViewer semantic and FSI-surface tests for `Viewer.run`, `Viewer.runApp`, launch outcome fields, bounded API separation, and runtime capability shape
- [X] T007 [P] [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input] Add failing-first MVU host tests for `init`, pure `update`, emitted `ViewerEffect` assertions, keyboard dispatch, tick mapping, render refresh, and close intent behavior
- [X] T008 [P] [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input] Add generated template source tests proving default graphical execution calls `Viewer.runApp viewerOptions generatedHost` and bounded evidence is flag-only
- [X] T009 [P] [skillist: speckit-evidence-audit] Add failing-first audit fixtures for bounded-only substitution, unsupported-host-only launch packages, missing persistent evidence fields, and missing supported-host artifact rejection
- [X] T010 [P] [skillist: speckit-tasks] Add generated guidance expectations that future graphical viewer tasks include a non-substitutable persistent launch task and bounded-evidence separation notes
- [X] T011 [P] [skillist: fs-skia-testing, fs-skia-skiaviewer] Add generated product validation expectations for semantic tests, bounded evidence, scene evidence, persistent launch source/wiring, and supported-host artifact collection
- [X] T012 [P] [skillist: []] Add documentation stubs for build, evidence, generated apps, migration guidance, and unsupported-scope diagnostics
- [X] T013 [P] [skillist: speckit-evidence-graph, speckit-evidence-audit] Update evidence command expectations for persistent graphical launch artifact categories, bounded helper categories, and synthetic evidence propagation constraints
- [X] T014 [skillist: fs-skia-skiaviewer] Prepare surface baseline update path for `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt` without refreshing the baseline during task generation
- [X] T015 [skillist: []] Record governance risk level as broad Tier 1, focused validation required for SkiaViewer/template/audit/docs paths, broad validation required before completion, and non-authoritative aggregate-result handling

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (P1) - Launch a Generated Graphical App

### Tests First

- [X] T016 [P] [US1] [skillist: fs-skia-skiaviewer] Add semantic tests that exercise the packed SkiaViewer surface for persistent scene launch outcomes and preserve bounded APIs as explicit helpers
- [X] T017 [P] [US1] [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input] Add generated app host tests for model initialization, pure update transitions, emitted effects, keyboard input dispatch, tick progression, render refresh, and intentional close
- [X] T018 [P] [US1] [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input, fs-skia-testing] Add generated product tests that a viewer-backed Tetris-style graphical profile's default executable path uses the persistent generated host, renders model-derived state, dispatches declared keyboard input, and keeps bounded smoke behind explicit flags

### Implementation

- [X] T019 [US1] [skillist: fs-skia-skiaviewer, fs-skia-scene] Implement `Viewer.run`, persistent launch outcomes, option validation, scene rendering handoff, and bounded API separation in `src/SkiaViewer/SkiaViewer.fs`
- [X] T020 [US1] [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input] Implement `Viewer.runApp`, `GeneratedAppHost`, `ViewerEffect` interpretation, keyboard dispatch, tick dispatch, render refresh, and close behavior at the viewer edge
- [X] T021 [US1] [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input] Update `template/base/src/Product/Program.fs` with a Tetris-style keyboard-capable generated app path using `Model`, `Msg`, `init`, pure `update`, `view`, `mapKey`, `tick`, `viewerOptions`, `generatedHost`, and default persistent launch wiring
- [X] T022 [US1] [skillist: fs-skia-skiaviewer, fs-skia-testing] Update generated product tests and validation helpers so default launch source/wiring is required and bounded smoke, first-frame, frame-count, and scene evidence remain explicit command paths
- [X] T023 [US1] [skillist: []] Document the US1 independent validation path in `readiness/generated-default-launch.md`, including command, expected persistent-window mode, keyboard applicability, and intentional exit criteria
- [X] T024 [US1] [skillist: []] Capture `readiness/supported-host-persistent-launch.txt` from a supported desktop host using the Tetris-style generated app with `status=ok`, `mode=persistent-window`, `window-opened=true`, model-derived state rendered, declared keyboard input dispatch verified, and `exit-path=true`

**Checkpoint**: US1 independently proves a generated graphical app launches through the persistent host by default.

---

## Phase 4: User Story 2 (P1) - Distinguish Evidence From Product Readiness

### Tests First

- [X] T025 [P] [US2] [skillist: speckit-evidence-audit] Add audit tests that reject bounded smoke, first-frame, frame-count, scene metadata, and unsupported-host diagnostics when no distinct supported-host persistent launch artifact exists
- [X] T026 [P] [US2] [skillist: speckit-evidence-audit] Add audit tests for required persistent launch fields: status, mode, command, window-opened, input-dispatch, exit-path, blocked-stage, classification, category, and message
- [X] T027 [P] [US2] [skillist: speckit-tasks] Add generated guidance tests rejecting viewer-backed default paths that only print metadata, count controls, run bounded smoke, emit scene evidence, or exit without a persistent launch attempt

### Implementation

- [X] T028 [US2] [skillist: speckit-evidence-audit] Implement audit classification and blocking diagnostics for missing persistent launch evidence, bounded-only substitution, unsupported-host-only evidence, and ambiguous launch fields
- [X] T029 [US2] [skillist: speckit-tasks] Update generated task guidance so graphical viewer features always include a persistent graphical launch evidence task that bounded helpers cannot complete
- [X] T030 [US2] [skillist: fs-skia-skiaviewer, fs-skia-testing] Update `GeneratedGuidanceCheck` and `GeneratedProductCheck` to require persistent generated host wiring and reject bounded-only or print-only default graphical paths
- [X] T031 [US2] [skillist: []] Update `docs/evidence.md`, `docs/generated-apps.md`, and `template/fragments/skiaviewer/README.md` to label bounded viewer commands as CI and diagnostic helpers, not interactive readiness substitutes
- [X] T032 [US2] [skillist: speckit-evidence-audit] Write `readiness/bounded-evidence-separation.md` with real rejection evidence, evidence package names, selected broad risk level, focused validation command, and non-authoritative aggregate-result notes

**Checkpoint**: US2 independently prevents bounded evidence from satisfying interactive graphical readiness.

---

## Phase 5: User Story 3 (P2) - Diagnose Missing Capability Versus Unsupported Environment

### Tests First

- [X] T033 [P] [US3] [skillist: fs-skia-skiaviewer] Add runtime capability tests distinguishing persistent window support, bounded smoke support, keyboard input support, renderer mode, unsupported host reasons, and missing product/package capability
- [X] T034 [P] [US3] [skillist: fs-skia-skiaviewer, fs-skia-testing] Add generated app diagnostic tests for unsupported display hosts, missing persistent contract capability, missing package capability, blocked stage, category, command, and reviewer-facing message

### Implementation

- [X] T035 [US3] [skillist: fs-skia-skiaviewer] Implement `Viewer.runtimeCapability` and launch failure classification for supported host, unsupported environment, product defect, missing package capability, renderer mode, blocked stage, category, and actionable message
- [X] T036 [US3] [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input] Wire generated app diagnostics so default launch reports unsupported environment or missing capability without falling back to bounded simulation as success
- [X] T037 [US3] [skillist: fs-skia-skiaviewer, fs-skia-testing] Update generated product validation artifacts to record runtime capability diagnostics separately from supported-host persistent launch evidence
- [X] T038 [US3] [skillist: []] Write `readiness/runtime-capability-diagnostics.md` with supported, unsupported-environment, missing-capability, and renderer-mode classifications plus reviewer decision guidance, including a 2-minute classification checklist for SC-007
- [X] T039 [US3] [skillist: []] Update migration guidance so bounded-only generated apps must adopt the persistent host, declare headless/non-interactive scope, or document missing persistent viewer capability as a blocking gap

**Checkpoint**: US3 independently classifies unsupported host conditions separately from product/package capability gaps.

---

## Phase 6: User Story 4 (P3) - Preserve Bounded Evidence Workflows

### Tests First

- [X] T040 [P] [US4] [skillist: fs-skia-skiaviewer] Add regression tests proving `Viewer.runBounded`, `Viewer.runUntilFirstFrame`, and `Viewer.runForFrames` remain available as explicit evidence helpers
- [X] T041 [P] [US4] [skillist: fs-skia-skiaviewer, fs-skia-testing] Add generated product tests for explicit bounded smoke, first-frame, frame-count, and scene evidence commands without changing the default persistent launch path

### Implementation

- [X] T042 [US4] [skillist: fs-skia-skiaviewer] Preserve bounded viewer implementation and diagnostics while keeping all bounded outcomes out of persistent launch success classification
- [X] T043 [US4] [skillist: fs-skia-skiaviewer, fs-skia-testing] Update template flags, generated product validation, and readiness artifact names for bounded smoke, frame diagnostics, and deterministic scene metadata
- [X] T044 [US4] [skillist: []] Write `readiness/generated-guidance-check.md` with GeneratedGuidanceCheck evidence for persistent defaults and explicit bounded helper commands

**Checkpoint**: US4 independently preserves bounded evidence workflows without weakening persistent launch readiness.

---

## Phase 7: Integration & Polish

- [X] T045 [skillist: fs-skia-skiaviewer] Refresh `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt` with `./fake.sh build -t RefreshSurfaceBaselines` and verify with `./fake.sh build -t PackageSurfaceCheck`
- [X] T046 [skillist: fs-skia-skiaviewer, fs-skia-testing] Run `./fake.sh build -t PackLocal`, generated consumer validation, and `./fake.sh build -t TemplateCheck`, then record generated default launch and package compatibility evidence
- [X] T047 [skillist: []] Run documentation and dependency governance checks, including `./fake.sh build -t DependencyReport`, and record no-new-dependency or package impact findings
- [X] T048 [skillist: speckit-evidence-graph] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/016-persistent-viewer-contract --graph-only` and capture clean graph output in `readiness/evidence-graph.md`
- [X] T049 [skillist: speckit-evidence-graph, speckit-evidence-audit] Run `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`, then capture PASS or blocking diagnostics in `readiness/evidence-audit.md`
- [X] T050 [skillist: speckit-tasks, speckit-implement] Run `./fake.sh build -t GeneratedGuidanceCheck` and confirm generated task and implementation guidance preserve persistent launch, skillist, synthetic disclosure, and risk-level rules
- [X] T051 [skillist: []] Run `./fake.sh build -t Verify` for broad Tier 1 validation or record explicit non-authoritative aggregate-result diagnostics with focused rerun evidence
- [X] T052 [skillist: speckit-evidence-audit] Complete final readiness review with supported-host launch artifact, bounded evidence separation, unsupported-host diagnostics, synthetic inventory, and no bounded-only completion claims

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| _None_ | All previously declared synthetic paths have been replaced with real command, packed-library, supported-host launch, or command-derived audit rejection evidence. | `readiness/supported-host-persistent-launch.txt`; `readiness/audit-rejections/*/audit.log`; `readiness/packed-skia-viewer-contract.txt`; `readiness/packed-generated-app-host.txt`; focused test logs from this implementation pass. | None |

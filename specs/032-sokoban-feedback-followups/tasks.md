# Tasks: Sokoban Feedback Follow-ups

**Feature branch**: `032-sokoban-feedback-followups`
**Spec**: `specs/032-sokoban-feedback-followups/spec.md`
**Plan**: `specs/032-sokoban-feedback-followups/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the evidence audit.

Approved synthetic error-handling work uses `[SEH]` plus the `synthetic-error-handling-approved` label. It still remains `[S]` when completed with synthetic-only malformed-input or explicit error-path evidence.

## Vertical-slice rule

A task tagged `[US*]` may only be marked `[X]` when the change is reachable from a user-facing entry point and that path was exercised with real evidence. For stateful or I/O-bearing stories, `[X]` also requires MVU/effect evidence where applicable: public contract review, pure transition tests, emitted-effect assertions, and real interpreter or generated-host evidence where safe.

## Task Annotations

- **[P]** — parallel-safe within the current phase
- **[US1]**, **[US2]**, ... — user-story scope
- **[T1]** / **[T2]** — Tier override when different from the feature classification
- **[SEH]** — design-approved synthetic error-handling task

Every task has matching structured metadata in `tasks.deps.yml` and mirrors its `skillist` here.

## Canonical Verification Targets

Use repository targets rather than ad-hoc command sequences. FAKE-backed commands (`./fake.sh`, `fake.cmd`, or `dotnet fake`) share `.fake` state and must run sequentially when more than one is needed.

Planned feature order:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

`./fake.sh build -t Verify` may be used after focused targets are clean, or as the single broad FAKE-backed final pass.

## Skill Evaluation Notes

- High-confidence capability matches: `fs-skia-layout-evidence` for generated HUD/readability, public generated naming guidance, and supported-host evidence classification; `fs-skia-skiaviewer` for viewer screenshot and persistent close host behavior; `fs-skia-testing` for validation helpers and generated evidence reports; `fs-skia-scene` for Scene text/default rendering; `fs-skia-keyboard-input` and `fs-skia-elmish` for the consumer API map and adapter/effect guidance.
- Medium-confidence matches accepted: guidance-only tasks that update generated docs and local skills use the same capability skills as the surfaced behavior because implementation will need those capability instructions before editing generated product guidance.
- Valid-empty tasks: setup, generic evidence bookkeeping, and final non-FAKE graph/audit invocation tasks do not materially benefit from a capability skill beyond the named Spec Kit graph/audit skills.
- False-positive omissions reviewed: `fs-skia-template-update` is not listed because this feature updates template contents/guidance, not package pin refresh, template package versioning, or `dotnet new` installation workflow.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm current feature artifacts and active branch metadata point to `specs/032-sokoban-feedback-followups`
- [X] T002 [P] [skillist: speckit-evidence-graph, speckit-evidence-audit] Create required readiness placeholders for default text, interactive close, consumer guidance, readiness contract, task guidance, risk levels, runtime limitations, aggregate hang diagnostics, evidence graph, and evidence audit
- [X] T003 [P] [skillist: []] Record feature tier, public-API assumption, MVU/effect applicability, synthetic-evidence policy, risk levels, and serialized FAKE validation order in `readiness/governance-risk-levels.md`
- [X] T004 [skillist: fs-skia-layout-evidence, speckit-implement] Resolve and record capability skill paths needed for generated HUD/readability, viewer host, testing helper, Scene, KeyboardInput, and Elmish work in `readiness/skill-loading-evidence-workflow.md`

---

## Phase 2: Foundation

- [X] T005 [skillist: fs-skia-scene, fs-skia-skiaviewer, fs-skia-testing] Review existing `.fsi` surfaces for default text rendering, screenshot capture, persistent launch, close reason, input dispatch, and validation helpers; document whether public contract changes are needed
- [X] T006 [P] [skillist: fs-skia-layout-evidence, fs-skia-testing] Add failing-first governance/test fixtures for the five guidance scan areas and required readiness terms
- [X] T007 [P] [skillist: fs-skia-skiaviewer, fs-skia-testing] Define common evidence report fields for screenshot glyph capture and persistent close evidence, including unsupported-host and failure classifications
- [X] T008 [skillist: fs-skia-layout-evidence] Document unsupported host/runtime limitations, aggregate hang diagnostics, and non-authoritative aggregate result handling in readiness
- [X] T009 [skillist: fs-skia-scene, fs-skia-skiaviewer, fs-skia-testing, speckit-implement] If T005 finds new public API is required, draft `.fsi` signatures, FSI exercise notes, and expected surface baseline updates before implementation; otherwise record the no-new-surface decision

**Checkpoint**: Foundation ready — user-story implementation may begin in parallel.

---

## Phase 3: User Story 1 - Readable Default Text Evidence (P1)

### Tests First

- [X] T010 [P] [US1] [skillist: fs-skia-scene, fs-skia-skiaviewer] Add failing screenshot/rendering tests that demonstrate default text currently produces non-glyph block coverage in the capture path
- [X] T011 [P] [US1] [skillist: fs-skia-testing] Add failing validation-helper tests for glyph-shaped coverage, solid-block detection, placeholder/tofu detection, decodable screenshot checks, and unsupported-host classification

### Implementation

- [X] T012 [US1] [skillist: fs-skia-scene, fs-skia-skiaviewer] Implement glyph-capable default text rendering in screenshot evidence, reusing native glyph rendering and deterministic vector fallback where possible
- [X] T013 [US1] [skillist: fs-skia-testing] Implement the screenshot-based capability check and report fields for `DefaultTextGlyphEvidence`
- [X] T014 [US1] [skillist: fs-skia-layout-evidence, fs-skia-testing] Capture real default-text glyph readiness evidence in `readiness/default-text-glyph-capture.md`
- [X] T015 [US1] [skillist: fs-skia-layout-evidence] Update generated guidance to warn that explicit fonts are required for brand or typography guarantees beyond default readability

**Checkpoint**: US1 independently proves readable default text screenshot evidence.

---

## Phase 4: User Story 2 - Persistent Interactive Launch Close Evidence (P1)

### Tests First

- [X] T016 [P] [US2] [skillist: fs-skia-skiaviewer, fs-skia-elmish] Add failing tests for app-owned close intent producing emitted host/viewer close effects while reducers remain pure
- [X] T017 [P] [US2] [skillist: fs-skia-skiaviewer, fs-skia-testing] Add failing persistent-close evidence validator tests for real interactive-window mode, first frame, window-opened fact, close request source, clean exit, elapsed time, and bounded-substitution rejection

### Implementation

- [X] T018 [US2] [skillist: fs-skia-skiaviewer, fs-skia-elmish] Wire generated app close-confirmed model state or message flow to a real viewer/window close effect at the host boundary
- [X] T019 [US2] [skillist: fs-skia-skiaviewer, fs-skia-testing] Implement or refine generated persistent launch evidence workflow and report serialization for `PersistentCloseEvidence`
- [X] T020 [US2] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer] Update generated app guidance with the CI-friendly persistent launch close recipe and bounded-evidence distinction
- [X] T021 [US2] [skillist: fs-skia-skiaviewer, fs-skia-testing] Capture real or supported-host-classified interactive close evidence in `readiness/interactive-window-close-evidence.md`

**Checkpoint**: US2 independently proves persistent interactive launch can close cleanly without manual closing.

---

## Phase 5: User Story 3 - Consumer API Shape Before Coding (P2)

### Tests First

- [X] T022 [P] [US3] [skillist: fs-skia-keyboard-input, fs-skia-skiaviewer, fs-skia-scene, fs-skia-elmish] Add failing generated-guidance tests requiring a compact API map for keyboard keys, host callbacks, viewer effects, adapter commands, and common Scene nodes

### Implementation

- [X] T023 [US3] [skillist: fs-skia-keyboard-input, fs-skia-skiaviewer, fs-skia-scene, fs-skia-elmish] Update generated consumer docs, template fragments, and local skills with the compact API map
- [X] T024 [US3] [skillist: fs-skia-layout-evidence] Classify each follow-up item as framework behavior, generated-app guidance, Spec Kit guidance, or consumer-author mistake in guidance/backlog notes
- [X] T025 [US3] [skillist: fs-skia-testing] Capture guidance scan evidence in `readiness/consumer-guidance-scan.md`

**Checkpoint**: US3 guidance lets a consumer author discover the required API shape before coding.

---

## Phase 6: User Story 4 - Readiness Evidence Before Audit Failures (P2)

### Tests First

- [X] T026 [P] [US4] [skillist: fs-skia-testing, speckit-tasks] Add failing guidance tests requiring feature-scoped readiness directory discovery, required readiness files, and mandatory audit terms

### Implementation

- [X] T027 [US4] [skillist: speckit-tasks] Update Spec Kit and generated-app readiness guidance to name the authoritative feature-scoped readiness directory and distinguish repository-level evidence output
- [X] T028 [US4] [skillist: fs-skia-testing] Add or update guidance scans for required terms covering governance risk levels, aggregate hang diagnostics, runtime limitations, and supported-host persistent launch evidence
- [X] T029 [US4] [skillist: fs-skia-testing] Capture readiness contract scan evidence in `readiness/readiness-contract-scan.md`

**Checkpoint**: US4 makes required readiness evidence discoverable before audit execution.

---

## Phase 7: User Story 5 - Task Graph Validator Gotchas (P3)

### Tests First

- [X] T030 [P] [US5] [skillist: speckit-tasks, speckit-evidence-graph] Add failing task-guidance tests for title trigger phrase pitfalls, `tasks.deps.yml` object shape, indentation, one key per task id, and `skillist` mirror rules

### Implementation

- [X] T031 [US5] [skillist: speckit-tasks] Update task-generation templates and `speckit-tasks` guidance with validator pitfall examples and dependency-file formatting rules
- [X] T032 [US5] [skillist: speckit-tasks, speckit-evidence-graph] Run graph-only validation against the updated generated task guidance examples and capture `readiness/task-guidance-scan.md`

**Checkpoint**: US5 documents validator pitfalls before authors run `EvidenceGraph`.

---

## Phase 8: Integration & Polish

- [X] T033 [skillist: fs-skia-layout-evidence, fs-skia-testing] Run `GeneratedGuidanceCheck` sequentially and record command order, log path, and result
- [X] T034 [skillist: fs-skia-layout-evidence, fs-skia-testing] Run `TemplateCheck` sequentially and record command order, log path, and result
- [X] T035 [skillist: fs-skia-skiaviewer, fs-skia-testing] Run `GeneratedProductCheck` sequentially and record command order, log path, and result
- [X] T036 [skillist: fs-skia-scene, fs-skia-skiaviewer, fs-skia-testing] Run `Dev` or focused package tests sequentially and record text/close validation results
- [X] T037 [skillist: fs-skia-scene, fs-skia-skiaviewer, fs-skia-testing] If T009 introduced public surface changes, run surface/package validation and refresh baselines; otherwise record the no-surface-change evidence
- [X] T038 [skillist: speckit-evidence-graph] Run graph validation and refresh `readiness/task-graph.md` plus `readiness/evidence-graph.md`
- [X] T039 [skillist: speckit-evidence-audit] Run evidence audit and refresh `readiness/evidence-audit.md`
- [X] T040 [skillist: []] Update final readiness notes with serialized FAKE-backed order, race-like failure rerun classification, and final follow-up scope decisions

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the source for the PR description's synthetic-evidence section.

For `[SEH]` rows, include the approval label, design-phase source, synthetic input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |

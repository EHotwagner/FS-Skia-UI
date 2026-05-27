# Tasks: Persistent Launch Evidence

**Feature branch**: `021-persistent-launch-evidence`
**Spec**: `specs/021-persistent-launch-evidence/spec.md`
**Plan**: `specs/021-persistent-launch-evidence/plan.md`

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
`synthetic-error-handling-approved` label. It still remains `[S]` when
completed with synthetic-only malformed-input or explicit error-path evidence.

## Vertical-Slice Rule

A task tagged `[US*]` may only be marked `[X]` when the user-facing entry point
was actually exercised. For this I/O-bearing feature, `[X]` also requires the
public request/result/effect contract to be exercised, pure transitions to be
tested, emitted effects to be asserted, and the effect interpreter to be run
against real dependencies where safe.

## Canonical Verification Targets

- `./fake.sh build -t Dev` for fast local verification.
- `./fake.sh build -t Verify` for the full governed workflow.
- `./fake.sh build -t FsiTranscripts` for public FSI evidence.
- `./fake.sh build -t PackageSurfaceCheck` for package surface review.
- `./fake.sh build -t RefreshSurfaceBaselines` for intentional current surface baseline refreshes.
- `./fake.sh build -t GeneratedGuidanceCheck` for generated guidance governance.
- `./fake.sh build -t GeneratedProductCheck` for generated product validation.
- `./fake.sh build -t TemplateCheck` for generated template validation.
- `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit` for graph and readiness gates.

Risk levels: small changes require the targeted project tests; medium changes
require package surface and generated product checks; broad changes require
`Verify`. Non-authoritative aggregate results must be recorded as supporting
diagnostics only, never as replacements for required readiness artifacts.

---

## Phase 1: Setup

- [X] T001 [P] [skillist: []] Create `specs/021-persistent-launch-evidence/readiness/` and placeholder names for the five required readiness files.
- [X] T002 [P] [skillist: []] Record feature tier, affected packages, build-target impact, public-API impact, unsupported scope, and broad validation obligations in readiness notes.
- [X] T003 [P] [skillist: []] Record MVU applicability: persistent launch is I/O-bearing and requires `Model`, `Msg`, `Effect`, `init`, `update`, emitted-effect tests, and interpreter evidence.
- [X] T004 [P] [skillist: []] Record the initial capability-skill evaluation notes for valid-empty tasks plus the required `fs-skia-layout-evidence` matches.

**Checkpoint**: Setup complete.

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: fs-skia-skiaviewer, fs-skia-elmish] Draft `src/SkiaViewer/SkiaViewer.fsi` persistent-launch request, artifact, outcome, window fact, blocked-stage, `Model`, `Msg`, `Effect`, `init`, `update`, and interpreter-boundary signatures.
- [X] T006 [P] [skillist: fs-skia-testing, fs-skia-layout-evidence] Draft `src/Testing/Testing.fsi` host warning, generated guidance, persistent artifact validation, and readiness-file discovery contracts.
- [X] T007 [P] [skillist: fs-skia-skiaviewer, fs-skia-layout-evidence] Define generated graphical app readiness command shape, artifact path, app-qualified naming rules, and separation of layout evidence from persistent-window evidence.
- [X] T008 [P] [skillist: speckit-evidence-graph, speckit-evidence-audit] Define build-target coverage for `Verify`, generated `Test`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`, `EvidenceGraph`, and `EvidenceAudit`.
- [X] T009 [skillist: fs-skia-skiaviewer, fs-skia-testing, fs-skia-elmish] Exercise the draft public contracts from FSI and capture representative request/result/update/effect transcript expectations in `readiness/fsi-session.txt`.
- [X] T010 [skillist: fs-skia-skiaviewer, fs-skia-testing] Prepare package surface baseline expectations for `FS.Skia.UI.SkiaViewer` and `FS.Skia.UI.Testing`.
- [X] T011 [skillist: []] Record readiness-file discovery requirements and missing-fact diagnostics for unsupported or blocked hosts.

**Checkpoint**: Foundation ready - story implementation may begin in parallel.

---

## Phase 3: User Story 1 - Prove Persistent GUI Launch

### Tests First

- [X] T012 [P] [US1] [skillist: fs-skia-skiaviewer, fs-skia-elmish] Add SkiaViewer semantic tests for persistent-launch `init`/`update` transitions, emitted effects, first-frame recording, input-dispatch recording, and controlled-close state.
- [X] T013 [P] [US1] [skillist: fs-skia-skiaviewer] Add artifact serialization tests requiring `status`, `mode`, `command`, `window-opened`, `input-dispatch`, `exit-path`, `blocked-stage`, `classification`, `category`, `message`, and first-frame facts.
- [X] T014 [P] [US1] [skillist: fs-skia-skiaviewer, fs-skia-layout-evidence] Add generated product readiness tests that run the explicit evidence-mode command without changing the normal persistent default launch. Evidence: `readiness/logs/t014-generated-product-check-green.txt`.

### Implementation

- [X] T015 [US1] [skillist: fs-skia-skiaviewer, fs-skia-elmish] Implement SkiaViewer persistent-launch request/result model, pure update transitions, emitted effects, and edge interpreter hooks.
- [X] T016 [US1] [skillist: fs-skia-skiaviewer] Implement first-frame, viewer-owned window identity, input-dispatch status, controlled evidence close, close reason, and artifact serialization.
- [X] T017 [US1] [skillist: fs-skia-skiaviewer, fs-skia-layout-evidence] Wire generated app evidence-mode launch and readiness artifact writing while preserving default user-driven persistent launch.
- [X] T018 [US1] [skillist: fs-skia-skiaviewer, fs-skia-layout-evidence] Produce `readiness/persistent-launch-evidence.md` from a supported-host real launch or record the exact blocked prerequisite stage without claiming pass.

**Checkpoint**: User Story 1 is independently testable.

---

## Phase 4: User Story 2 - Diagnose Capture Failures Honestly

### Tests First

- [X] T019 [P] [US2] [skillist: fs-skia-skiaviewer] Add tests for desktop prerequisite, process launch, window creation, first-frame/render, observation, capture, input verification, controlled-exit, and artifact-write blocked stages.
- [X] T020 [P] [US2] [skillist: fs-skia-skiaviewer, fs-skia-layout-evidence] Add tests proving external title/window search failure cannot produce headless-only classification when viewer-owned facts and a live process exist.
- [S] T021 [P] [US2] [SEH] synthetic-error-handling-approved [skillist: fs-skia-testing] Add malformed persistent-launch artifact parser tests for missing required fields, invalid field values, and contradictory pass claims.

### Implementation

- [X] T022 [US2] [skillist: fs-skia-skiaviewer] Implement window-observation diagnostics with diagnostic source, host facts, viewer facts, external observation facts, capture facts, missing facts, blocked stage, classification, and message.
- [X] T023 [US2] [skillist: fs-skia-skiaviewer, fs-skia-layout-evidence] Implement observation/capture classification so external observation failure stays observation/capture blocked when desktop prerequisites and viewer-owned launch facts are present.
- [S] T024 [US2] [SEH] synthetic-error-handling-approved [skillist: fs-skia-testing] Implement artifact validation diagnostics for missing fields, synthetic fixture rejection, contradictory pass claims, and actionable messages.
- [X] T025 [US2] [skillist: fs-skia-skiaviewer, fs-skia-layout-evidence] Produce `readiness/window-observation-diagnostics.md` with real launch, generic host probe, and any synthetic fixture distinctions disclosed.

**Checkpoint**: User Story 2 is independently testable.

---

## Phase 5: User Story 3 - Keep Benign Host Warnings Non-Blocking

### Tests First

- [X] T026 [P] [US3] [T2] [skillist: fs-skia-testing, fs-skia-layout-evidence] Add host warning classification tests for known GTK/module warnings paired with passing launch, first-frame/render, and exit facts.
- [X] T027 [P] [US3] [T2] [skillist: fs-skia-testing, fs-skia-layout-evidence] Add fatal-preservation tests showing launch, rendering, layout, package, and artifact-write failures remain fatal even with benign warning text present.

### Implementation

- [X] T028 [US3] [T2] [skillist: fs-skia-testing, fs-skia-layout-evidence] Implement host warning classification results with raw message, warning class, fatal flag, evidence path, supporting facts, and diagnostics.
- [X] T029 [US3] [T2] [skillist: fs-skia-testing, fs-skia-layout-evidence] Produce `readiness/host-warning-classification.md` showing benign warnings preserved as non-blocking only when required real launch facts pass.

**Checkpoint**: User Story 3 is independently testable.

---

## Phase 6: User Story 4 - Avoid Generated App Naming Collisions

### Tests First

- [X] T030 [P] [US4] [T2] [skillist: fs-skia-layout-evidence] Add generated guidance checks requiring `Product.Program.view`, `Product.Program.generatedHost`, and `Product.Program.update` when framework capability namespaces are open.
- [X] T031 [P] [US4] [T2] [skillist: fs-skia-layout-evidence] Add generated guidance checks that layout evidence, deterministic render hashes, and persistent-window launch evidence are documented as separate proof types.

### Implementation

- [X] T032 [US4] [T2] [skillist: fs-skia-layout-evidence] Update generated docs, samples, and tests to use app-qualified scene, host, and update names in collision-prone contexts.
- [X] T033 [US4] [T2] [skillist: fs-skia-layout-evidence] Update generated readiness guidance so persistent-launch evidence is not described as layout, screenshot, or deterministic render proof.
- [X] T034 [US4] [T2] [skillist: fs-skia-layout-evidence] Produce `readiness/generated-guidance.md` with generated guidance checks and any remaining naming or evidence-separation diagnostics.

**Checkpoint**: User Story 4 is independently testable.

---

## Phase 7: Integration & Polish

- [X] T035 [skillist: fs-skia-skiaviewer, fs-skia-testing] Refresh public surface baselines for changed SkiaViewer and Testing contracts with `./fake.sh build -t RefreshSurfaceBaselines`, then verify with `./fake.sh build -t PackageSurfaceCheck`.
- [X] T036 [skillist: fs-skia-skiaviewer, fs-skia-testing, fs-skia-layout-evidence] Run targeted package, generated product, generated guidance, template, and readiness checks; record small/medium/broad validation results and non-authoritative aggregate notes.
- [X] T037 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` and confirm no cycles, dangling refs, mirror mismatches, or invalid skill metadata.
- [X] T038 [skillist: speckit-evidence-graph, speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit`, produce `readiness/evidence-audit.md`, and confirm required readiness files and persistent-launch artifact fields are internally consistent.
- [X] T039 [skillist: []] Run `./fake.sh build -t Verify` for broad validation and record any host-specific unsupported prerequisites separately from feature failures.
- [X] T040 [skillist: fs-skia-skiaviewer, fs-skia-layout-evidence] Run repeated supported-host persistent-launch attempts, record pass ratio against the 95% SC-001 threshold, and classify every failed attempt by blocked stage in `readiness/persistent-launch-evidence.md`.

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. For `[SEH]`
rows, include the approval label, design-phase source, synthetic input class,
expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T021 | Parser/classifier error branches require malformed and contradictory artifact inputs that must not be produced by real supported-host launch evidence. | `specs/021-persistent-launch-evidence/readiness/window-observation-diagnostics.md` | | synthetic-error-handling-approved | `specs/021-persistent-launch-evidence/contracts/persistent-launch-evidence-contract.md`; `specs/021-persistent-launch-evidence/contracts/evidence-audit-contract.md` | Missing required fields, invalid enum values, and contradictory `status=ok` claims without real launch facts. | Reject artifact, identify missing or contradictory facts, and never satisfy supported-host persistent-launch readiness. | accepted-seh |
| T024 | Artifact validation diagnostics were verified with malformed and contradictory persistent-launch artifact fixtures only. | `specs/021-persistent-launch-evidence/readiness/window-observation-diagnostics.md` | | synthetic-error-handling-approved | `specs/021-persistent-launch-evidence/contracts/persistent-launch-evidence-contract.md`; `specs/021-persistent-launch-evidence/contracts/evidence-audit-contract.md` | Missing required fields, invalid enum values, and contradictory `status=ok` claims without real launch facts. | Reject artifact, identify missing or contradictory facts, and never satisfy supported-host persistent-launch readiness. | accepted-seh |

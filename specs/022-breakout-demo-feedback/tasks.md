# Tasks: Breakout Demo Feedback

**Feature branch**: `022-breakout-demo-feedback`
**Spec**: `specs/022-breakout-demo-feedback/spec.md`
**Plan**: `specs/022-breakout-demo-feedback/plan.md`

## Status Legend

- `[ ]` - pending
- `[X]` - done with real evidence
- `[S]` - done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` - failed
- `[-]` - skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. It still remains `[S]` when
completed with synthetic-only malformed-input or explicit error-path evidence.
The classification must be assigned during design, planning, clarification, or
task generation. implementation-time relabeling is forbidden; newly discovered
needs go back to task/design review.

## Vertical-slice rule (US phases)

A task tagged `[US*]` may only be marked `[X]` when the change is reachable
from a user-facing entry point and that path was actually exercised: an FSI
session against the packed library, a smoke run of the application, a manual
walk-through with transcript, or a screenshot captured under `readiness/`.
Domain, model, or core-layer changes alone do not satisfy `[X]` for a `[US*]`
task.

For stateful or I/O-bearing stories, `[X]` also requires Elmish/MVU evidence:
the public `Model` / `Msg` / `Effect` or `Cmd<Msg>` contract was exercised,
pure `update` transitions were tested, emitted effects were asserted, and the
effect interpreter was run against real dependencies where safe.

## Task Annotations

- **[P]** - parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, ... - user-story scope
- **[T1]** / **[T2]** - Tier 1 (contracted) vs Tier 2 (internal) change
- **[SEH]** - design-approved synthetic error-handling task paired with
  `synthetic-error-handling-approved`

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors
the structured `skillist` value using `[skillist: ...]`; `[skillist: []]`
means no capability skill materially applies.

## Canonical Verification Targets

- `./fake.sh build -t Dev` for fast local verification.
- `./fake.sh build -t Verify` for the full governed workflow.
- `./fake.sh build -t PackLocal` for local package output.
- `./fake.sh build -t RefreshSurfaceBaselines` for intentional current surface
  baseline refreshes.
- `./fake.sh build -t PackageSurfaceCheck` for package surface review.
- `./fake.sh build -t FsiTranscripts` for public FSI evidence.
- `./fake.sh build -t SampleContractSmoke` for sample smoke evidence.
- `./fake.sh build -t TemplateCheck` for source/package default/minimal
  generated project validation.
- `./fake.sh build -t GeneratedGuidanceCheck` for generated prompt, task-skill,
  and implementation guidance governance.
- `./fake.sh build -t TemplateDrift` for template-owned drift and deferral
  validation.
- `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`
  for graph and synthetic-evidence gates.

## Capability Skill Evaluation Notes

- High-confidence matches: Scene shape tasks use `fs-skia-scene`; viewer
  launch/screenshot tasks use `fs-skia-skiaviewer`; Testing helper/report tasks
  use `fs-skia-testing`; effect-boundary tasks use `fs-skia-elmish`; generated
  layout/evidence/name guidance tasks use `fs-skia-layout-evidence`; template
  packaging or generated project validation tasks use `fs-skia-template-update`.
- Medium-confidence overlaps: generated examples often touch Scene, Viewer,
  Testing, and layout guidance together. The chosen `skillist` order follows
  implementation flow: public capability contract first, then generated
  guidance/template validation.
- Valid-empty tasks: setup, readiness placeholders, generic governance notes,
  and final summary tasks do not need a capability skill beyond the evidence
  skills explicitly listed for final validation.

## Phase 1: Setup

- [X] T001 [skillist: []] Create `specs/022-breakout-demo-feedback/readiness/` with placeholders for `generated-viewer-guidance.md`, `scene-shape-evidence.md`, `screenshot-evidence.md`, `effect-boundary-guidance.md`, and `evidence-report-conventions.md`
- [X] T002 [P] [skillist: []] Record Tier 1 scope, affected packages, generated-template ownership, required real evidence paths, and deferred scope in `readiness/feature-scope.md`
- [X] T003 [P] [skillist: []] Record risk-level evidence policy: small checks for isolated docs/tests, medium checks for package/template changes, broad `Verify` plus graph/audit when public contracts or generated defaults change; aggregate results are non-authoritative unless backed by named artifacts
- [X] T004 [skillist: []] Record Elmish/MVU applicability for this feature: generated apps are stateful and I/O-bearing, so `Model`, `Msg`, app commands, pure `update`, viewer effects, and host interpreter evidence are required

**Checkpoint**: Setup complete.

## Phase 2: Foundation

- [X] T005 [skillist: fs-skia-scene, fs-skia-skiaviewer, fs-skia-testing, fs-skia-elmish] Draft `.fsi` contracts for Scene circle/ellipse primitives, SkiaViewer screenshot results, Testing report/guidance helpers, and generated app `Model`/`Msg`/app command/update/host interpreter boundaries
- [X] T006 [P] [skillist: fs-skia-layout-evidence] Update generated public scene/host/update naming guidance so generated docs and tests use `Product.Program.view`, `Product.Program.generatedHost`, and `Product.Program.update`
- [X] T007 [P] [skillist: fs-skia-template-update] Inspect template inclusion policy and `.template.config/template.json` for any new or renamed generated files needed by this feature
- [X] T008 [P] [skillist: fs-skia-scene, fs-skia-skiaviewer, fs-skia-testing] Add failing-first surface and semantic test skeletons for Scene, SkiaViewer, and Testing public contracts without implementing behavior
- [X] T009 [skillist: fs-skia-scene, fs-skia-skiaviewer, fs-skia-testing] Exercise the draft public contracts from FSI and capture `readiness/fsi-session.txt`, including shape constructors, screenshot result construction, and report helper signatures
- [X] T010 [skillist: fs-skia-scene, fs-skia-skiaviewer, fs-skia-testing] Capture initial package surface baseline diffs under `readiness/surface-baselines/` for `FS.Skia.UI.Scene`, `FS.Skia.UI.SkiaViewer`, and `FS.Skia.UI.Testing`
- [X] T011 [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer] Record unsupported-host and benign-warning classification rules for screenshot evidence so unsupported capture is a real negative host fact, not synthetic success

**Checkpoint**: Foundation ready - story implementation may begin in parallel.

## Phase 3: User Story 1 (Generated Viewer Guidance Matches Reality)

### Tests First

- [X] T012 [P] [US1] [skillist: fs-skia-skiaviewer, fs-skia-template-update] Add generated app tests that compile against the packed package and fail if the documented persistent viewer entry point is missing
- [X] T013 [P] [US1] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer] Add `GeneratedGuidanceCheck` coverage that scans generated source, tests, docs, quickstart text, and readiness wording for one selected persistent-launch contract
- [X] T014 [P] [US1] [skillist: fs-skia-skiaviewer] Add readiness verifier for `readiness/generated-viewer-guidance.md` requiring package version, selected entry point, files scanned, and distinct deterministic/persistent/screenshot evidence kinds

### Implementation

- [X] T015 [US1] [skillist: fs-skia-skiaviewer, fs-skia-template-update] Select the packaged persistent viewer launch contract and update generated source, tests, docs, quickstart, and examples to use that public name consistently
- [X] T016 [US1] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer] Update generated guidance wording to keep deterministic render proof, persistent launch proof, and screenshot proof separate
- [X] T017 [US1] [skillist: fs-skia-template-update] Run fresh generated app validation through `TemplateCheck` and record the persistent launch guidance result in `readiness/generated-viewer-guidance.md`

**Checkpoint**: User Story 1 is fully functional and testable independently.

## Phase 4: User Story 2 (Simple Game Shapes Are First-Class)

### Tests First

- [X] T018 [P] [US2] [skillist: fs-skia-scene] Add failing-first Scene public surface tests for filled circle and filled ellipse constructors through `Scene.fsi`
- [X] T019 [P] [US2] [skillist: fs-skia-scene, fs-skia-layout-evidence] Add deterministic evidence tests that verify circle and ellipse bounds, fill, placement, and partial-out-of-bounds behavior without live screenshot capture
- [X] T020 [P] [US2] [skillist: fs-skia-scene, fs-skia-template-update] Add generated example tests for at least three circular or elliptical entities without rectangle substitution

### Implementation

- [X] T021 [US2] [skillist: fs-skia-scene] Implement public filled circle and filled ellipse Scene primitives, including node shapes, evidence descriptions, and geometry helper constructors where needed
- [X] T022 [US2] [skillist: fs-skia-scene, fs-skia-layout-evidence] Implement deterministic render/evidence support for circle and ellipse shape facts in under 5 seconds for the standard generated scene
- [X] T023 [US2] [skillist: fs-skia-scene, fs-skia-layout-evidence, fs-skia-template-update] Update generated game/chart/interaction examples and geometry guidance to use shared Scene geometry for layout evidence, collision bounds, containment checks, and rendering bounds when it fits
- [X] T024 [US2] [skillist: fs-skia-scene] Record real deterministic shape evidence in `readiness/scene-shape-evidence.md`

**Checkpoint**: User Story 2 is fully functional and testable independently.

## Phase 5: User Story 3 (Screenshot Evidence Is Honest And Bounded)

### Tests First

- [X] T025 [P] [US3] [skillist: fs-skia-skiaviewer, fs-skia-testing] Add failing-first SkiaViewer and Testing tests for screenshot success fields, unsupported fields, normalized statuses, dimensions, output paths, and diagnostics
- [X] T026 [P] [US3] [skillist: fs-skia-skiaviewer, fs-skia-layout-evidence] Add unsupported-host classification tests proving unsupported screenshot capture never claims screenshot proof and always names `fallback=deterministic-scene-evidence`
- [X] T027 [P] [US3] [skillist: fs-skia-skiaviewer, fs-skia-template-update] Add generated screenshot evidence command tests for supported-host success where available and explicit unsupported result where capture is unavailable

### Implementation

- [X] T028 [US3] [skillist: fs-skia-skiaviewer] Implement SkiaViewer screenshot evidence request/result contracts and host interpreter behavior using existing viewer/platform capability, returning explicit unsupported results when capture is unavailable
- [X] T029 [US3] [skillist: fs-skia-testing] Define screenshot-specific report fields and status classification against the shared evidence report helper contract, without introducing a separate screenshot-only writer
- [X] T030 [US3] [skillist: fs-skia-skiaviewer, fs-skia-testing, fs-skia-template-update] Wire generated screenshot evidence commands and docs to the viewer result and report helpers
- [X] T031 [US3] [skillist: fs-skia-skiaviewer, fs-skia-layout-evidence] Record screenshot evidence matrix in `readiness/screenshot-evidence.md`: supported-host `status=ok` facts when capture support is available or a blocked supported-host note with exact missing capability and owner, plus unsupported-host `status=unsupported`, reason, and deterministic fallback without screenshot proof claims

**Checkpoint**: User Story 3 is fully functional and testable independently.

## Phase 6: User Story 4 (App Effects And Viewer Effects Are Distinct)

### Tests First

- [X] T032 [P] [US4] [skillist: fs-skia-elmish, fs-skia-skiaviewer] Add generated source/tests that assert `update` is pure, app commands are emitted separately, and viewer render/window/screenshot effects are produced or interpreted at the host boundary
- [X] T033 [P] [US4] [skillist: fs-skia-layout-evidence, fs-skia-elmish] Add generated guidance checks that fail when examples append viewer effects to app command lists or use inconsistent effect-category names

### Implementation

- [X] T034 [US4] [skillist: fs-skia-elmish, fs-skia-skiaviewer, fs-skia-template-update] Update generated app source to include a complete `Model`, `Msg`, app command, `init`, pure `update`, `view`, generated host, and interpreter-boundary example
- [X] T035 [US4] [skillist: fs-skia-layout-evidence, fs-skia-elmish] Update generated docs and tests so reviewers can identify app commands, viewer effects, and host interpretation from the generated example alone
- [X] T036 [US4] [skillist: fs-skia-elmish, fs-skia-skiaviewer] Run pure transition tests, emitted-effect assertions, real interpreter evidence where safe, and a timed reviewer checklist proving app commands versus viewer effects can be identified in under 2 minutes; record `readiness/effect-boundary-guidance.md`

**Checkpoint**: User Story 4 is fully functional and testable independently.

## Phase 7: User Story 5 (Evidence Reports And Geometry Are Reusable)

### Tests First

- [X] T037 [P] [US5] [skillist: fs-skia-testing] Add failing-first Testing tests for key-value report helper ordering, parent directory creation, stdout/file parity, status vocabulary, unsupported-host fields, and exit behavior
- [X] T038 [P] [US5] [skillist: fs-skia-testing, fs-skia-template-update] Add generated product tests proving at least three evidence commands share the same report conventions
- [X] T039 [P] [US5] [skillist: fs-skia-scene, fs-skia-layout-evidence] Add guidance checks that reject duplicate local geometry records when shared Scene geometry fits the generated app model

### Implementation

- [X] T040 [US5] [skillist: fs-skia-testing] Implement public or generated Testing helpers for stable key-value evidence reports, directory creation, stdout echoing, normalized statuses, unsupported-host fields, and command exit classification
- [X] T041 [US5] [skillist: fs-skia-testing, fs-skia-scene, fs-skia-template-update] Update generated evidence commands and geometry guidance to reuse the standard report helpers and shared Scene geometry conventions
- [X] T042 [US5] [skillist: fs-skia-testing, fs-skia-layout-evidence] Record report convention evidence from at least three generated evidence commands in `readiness/evidence-report-conventions.md`

**Checkpoint**: User Story 5 is fully functional and testable independently.

## Phase 8: Integration & Polish

- [X] T043 [skillist: fs-skia-scene, fs-skia-skiaviewer, fs-skia-testing] Refresh intentional Tier 1 surface baselines with `./fake.sh build -t RefreshSurfaceBaselines` and verify `./fake.sh build -t PackageSurfaceCheck`
- [X] T044 [P] [skillist: fs-skia-template-update] Run `./fake.sh build -t TemplateCheck`, `./fake.sh build -t GeneratedGuidanceCheck`, and `./fake.sh build -t TemplateDrift`; record generated validation artifacts under readiness
- [X] T045 [P] [skillist: fs-skia-scene, fs-skia-skiaviewer, fs-skia-testing, fs-skia-elmish] Run targeted capability tests for Scene, SkiaViewer, Testing, and generated Elmish wiring plus `./fake.sh build -t FsiTranscripts`
- [X] T046 [skillist: []] Run `./fake.sh build -t Verify` for broad validation because public contracts and generated defaults changed; record non-authoritative aggregate output with links to authoritative readiness artifacts
- [X] T047 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` and confirm no cycles, dangling refs, mirror mismatches, invalid skills, or unexpected computed statuses
- [X] T048 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` and confirm verdict PASS or document every accepted synthetic override

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |

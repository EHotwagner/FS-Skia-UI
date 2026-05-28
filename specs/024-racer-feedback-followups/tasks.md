# Tasks: Racer Feedback Follow-Ups

**Feature branch**: `024-racer-feedback-followups`
**Spec**: `specs/024-racer-feedback-followups/spec.md`
**Plan**: `specs/024-racer-feedback-followups/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

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
session against the packed library, a generated product smoke run, a manual
walk-through with transcript, or a screenshot captured under `readiness/`.
Domain, model, or core-layer changes alone do not satisfy `[X]` for a `[US*]`
task, even if their unit tests pass green.

For stateful or I/O-bearing stories, `[X]` also requires explicit boundary
evidence: public request/result contracts were exercised, pure classification
or validation transitions were tested, emitted host/evidence facts were
asserted, and viewer/process/filesystem interpreters were run against real
dependencies where safe. Generated app gameplay MVU workflows remain
unchanged; Principle IV applies to screenshot, launch, warning, and evidence
workflow boundaries introduced or clarified by this feature.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, ... — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change
- **[SEH]** — design-approved synthetic error-handling task paired with
  `synthetic-error-handling-approved`

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors
the structured `skillist` value using `[skillist: ...]`; `[skillist: []]`
means no capability skill materially applies.

## Canonical Verification Targets

- `./fake.sh build -t Dev` for fast local verification.
- `./fake.sh build -t Verify` for the full governed workflow.
- `./fake.sh build -t PackLocal` for local package output.
- `./fake.sh build -t RefreshSurfaceBaselines` for intentional public surface baseline refreshes.
- `./fake.sh build -t PackageSurfaceCheck` for package surface review.
- `./fake.sh build -t FsiTranscripts` for public FSI evidence.
- `./fake.sh build -t SampleContractSmoke` for sample smoke evidence.
- `./fake.sh build -t TemplateCheck` for source/package/default/minimal generated project validation.
- `./fake.sh build -t DependencyReport` for central package governance.
- `./fake.sh build -t GeneratedGuidanceCheck` for generated prompt, task-skill, and implementation guidance governance.
- `./fake.sh build -t TemplateDrift` for template-owned drift and deferral validation.
- `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit` for graph and synthetic-evidence gates.

Governance risk levels for this feature:

- Small: one doc/guidance wording change or one focused validator rule. Run the touched tests plus the named readiness file.
- Medium: generated template, public guidance, or validation workflow changes. Run affected package tests, `GeneratedGuidanceCheck`, `GeneratedProductCheck`, and `TemplateCheck`.
- Broad: public `.fsi`, package surface, generated product behavior, live viewer capture, or readiness/audit semantics change. Run `Verify`, `PackageSurfaceCheck`, `EvidenceGraph`, and `EvidenceAudit`; record any aggregate-only result as non-authoritative until focused evidence exists.

## Skill Evaluation Notes

- High confidence matches: `fs-skia-layout-evidence` for generated guidance, evidence wording, screenshot result semantics, warning classification, and detached launch guidance; `fs-skia-skiaviewer` for viewer screenshot and launch contracts; `fs-skia-testing` for validation helpers/report schemas; `fs-skia-template-update` for template-generated docs/code/tests; `speckit-evidence-graph` and `speckit-evidence-audit` for final gates.
- Medium or indirect signals: `fs-skia-project` applies to generated products after instantiation, but the framework tasks edit the template and governance source, so `fs-skia-template-update` is the minimal source-side skill. `fs-skia-scene` and `fs-skia-layout` are valid-empty because this feature avoids scene/layout primitive changes.
- Reviewer disposition: capability skills are included only where they materially guide the implementation boundary; empty skill lists are intentional for pure scaffolding and baseline note tasks.

## Phase 1: Setup

- [X] T001 [P] [skillist: []] Scaffold `specs/024-racer-feedback-followups/readiness/` with placeholders for `baseline-status.md`, `generated-guidance-validation.md`, `screenshot-capability-detail.md`, `screenshot-success-artifact.md`, `host-warning-classification.md`, and `detached-launch-guidance.md`
- [X] T002 [P] [skillist: []] Record baseline results for `Verify`, `GeneratedGuidanceCheck`, and `TemplateCheck` in `readiness/baseline-status.md`
- [X] T003 [P] [skillist: fs-skia-layout-evidence] Resolve feature capability guidance, including layout/evidence skill scope, screenshot proof restrictions, benign warning rules, and generated guidance naming constraints
- [X] T004 [skillist: fs-skia-layout-evidence] Record Tier 1 scope, public API impact, generated product impact, MVU/effect-boundary applicability, synthetic limitations, small/medium/broad risk levels, and required evidence obligations in readiness notes

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-testing] Draft additive `.fsi` contracts in `src/SkiaViewer/SkiaViewer.fsi` and/or `src/Testing/Testing.fsi` for screenshot capability detail, live-window capture source, viewer-open status, capture availability, warning classification, evidence validators, and the `EvidenceWorkflowModel` / `EvidenceWorkflowMsg` / `EvidenceWorkflowEffect` / `init` / `update` / interpreter boundary
- [X] T006 [P] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-testing] Add failing semantic tests through public contracts for screenshot success fields, unsupported capability separation, deterministic fallback separation, benign GTK warnings, report validators, pure `update` transitions, and emitted evidence effects
- [X] T007 [P] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Add failing governance/generated tests for geometry naming examples, rejected `Rect`/`Point`/`Size` app-domain recommendations, screenshot wording, and detached Linux launch guidance
- [S] T008 [P] [skillist: fs-skia-layout-evidence, fs-skia-testing] Add [SEH] synthetic-error-handling-approved malformed readiness report tests for invalid screenshot proof fields, missing capability details, hidden warnings, and hostile artifact paths
- [X] T009 [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-testing] Exercise draft public evidence records and representative `init`/`update` paths from FSI or focused transcripts, then capture public contract notes under `readiness/screenshot-capability-detail.md`
- [X] T010 [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-testing] Record surface-area baseline expectations for changed SkiaViewer/Testing public modules ahead of intentional refreshes

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 - Avoid Scene Naming Collisions (P1)

### Tests First

- [X] T011 [P] [US1] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Add generated guidance tests that require at least three domain-specific examples such as `WorldRect`, `WorldPoint`, `TrackBounds`, `CarPose`, or `CheckpointBounds`
- [X] T012 [P] [US1] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Add generated guidance tests that reject app-domain recommendations named only `Rect`, `Point`, or `Size` when scene/layout primitives are in scope

### Implementation

- [X] T013 [US1] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Update generated sample docs, template guidance, fragment READMEs, and public generated-app docs to use domain-specific geometry names and avoid ambiguity-driven type annotations
- [X] T014 [US1] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Run `GeneratedGuidanceCheck`, `TemplateCheck`, and `TemplateDrift`, then record checked files, accepted examples, and rejected stale patterns in `readiness/generated-guidance-validation.md`

**Checkpoint**: User Story 1 is fully functional and testable independently.

---

## Phase 4: User Story 2 - Capture Honest Screenshot Evidence (P1)

### Tests First

- [X] T015 [P] [US2] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-testing] Add SkiaViewer and Testing tests requiring successful screenshot records to report `status=ok`, `evidence-kind=screenshot`, PNG artifact path, positive dimensions, first-frame presentation, and live-window capture source
- [X] T016 [P] [US2] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-testing] Add tests requiring unsupported screenshot records to separate viewer-open status, first-frame status when known, capture availability, unsupported reason, deterministic fallback kind, and non-proof fields
- [X] T017 [P] [US2] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-template-update] Add generated product tests proving `--screenshot-evidence` uses the viewer screenshot contract and does not relabel deterministic render or pixel-readback output as screenshot proof

### Implementation

- [X] T018 [US2] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-testing] Implement additive screenshot evidence records, validators, diagnostics, and report fields while keeping window/process/filesystem work at the viewer or evidence interpreter edge
- [X] T019 [US2] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-template-update] Wire generated screenshot evidence output to the additive fields, preserving existing interactive launch, bounded first-frame, deterministic render, screenshot, and unsupported paths
- [F] T020 [US2] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer] Collect real live-window PNG screenshot evidence on at least one supported Windows or Linux desktop host and record `readiness/screenshot-success-artifact.md`
- [X] T021 [US2] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-testing] Collect or document unsupported/capability details for unavailable capture or unavailable supported OS validation hosts and record `readiness/screenshot-capability-detail.md`

**Checkpoint**: User Story 2 is fully functional and testable independently.

---

## Phase 5: User Story 3 - Classify Benign Host Warnings (P2)

### Tests First

- [X] T022 [P] [US3] [T2] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-testing] Add warning-classifier tests for the `colorreload-gtk-module` and `window-decorations-gtk-module` messages with first-frame success, preserved raw text, and no unrelated failures
- [X] T023 [P] [US3] [T2] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-testing] Add negative tests proving unknown warnings, process exits, missing first-frame evidence, renderer errors, package failures, or mixed unrelated warnings are not hidden by benign classification

### Implementation

- [X] T024 [US3] [T2] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-testing] Implement host warning classification records, exact known GTK matching, launch-success gating, raw warning preservation, and final readiness status behavior
- [X] T025 [US3] [T2] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer] Run real captured launch output containing the known GTK messages with first-frame success, preserve the transcript, and record `readiness/host-warning-classification.md`; synthetic warning fixtures remain test-only and do not satisfy acceptance evidence

**Checkpoint**: User Story 3 is fully functional and testable independently.

---

## Phase 6: User Story 4 - Use Reliable Detached GUI Launch Guidance (P2)

### Tests First

- [X] T026 [P] [US4] [T2] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Add docs and generated guidance tests requiring Linux detached-session launch guidance with `setsid`, log capture, stderr redirection, and stdin from `/dev/null`
- [X] T027 [P] [US4] [T2] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Add guidance tests rejecting simple terminal detachment, plain shell backgrounding, or plain `nohup dotnet run ... &` as the preferred reliable GUI default

### Implementation

- [X] T028 [US4] [T2] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Update generated product docs, template docs, fragments, and public generated-app docs to recommend the detached-session launch pattern and preserve log path diagnostics
- [X] T029 [US4] [T2] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Run guidance validation for detached launch instructions and record reviewed files, accepted command patterns, rejected stale guidance, and log/stdin facts in `readiness/detached-launch-guidance.md`

**Checkpoint**: User Story 4 is fully functional and testable independently.

---

## Phase 7: Integration & Polish

- [X] T030 [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-testing] Refresh intentional SkiaViewer/Testing package surface baselines and record `PackageSurfaceCheck` evidence
- [X] T031 [skillist: fs-skia-layout-evidence, fs-skia-template-update] Run `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`, and `TemplateDrift`, then update generated guidance and detached launch readiness artifacts
- [-] T032 [skillist: fs-skia-layout-evidence] Run focused readiness review for `baseline-status.md`, `generated-guidance-validation.md`, `screenshot-capability-detail.md`, `screenshot-success-artifact.md`, `host-warning-classification.md`, and `detached-launch-guidance.md`
- [-] T033 [skillist: fs-skia-layout-evidence] Run the four-follow-up reviewer walkthrough against the source feedback file and six readiness artifacts, require completion under 10 minutes, and record elapsed time plus reviewed paths in readiness notes
- [X] T034 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` and confirm no cycles, dangling refs, mirror mismatches, invalid skill ids, or `[S*]` surprises
- [X] T035 [skillist: speckit-evidence-graph, speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit`, write `readiness/evidence-audit.md`, and document every accepted synthetic or unsupported condition
- [-] T036 [skillist: fs-skia-layout-evidence] Run `./fake.sh build -t Verify` for broad Tier 1 validation, then record focused failures separately from non-authoritative aggregate results

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. For `[SEH]`
rows, include the approval label, design-phase source, synthetic input class,
expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T008 | Validate explicit error-path rejection for malformed or hostile evidence records without requiring unsafe/corrupt real artifacts | `specs/024-racer-feedback-followups/readiness/screenshot-capability-detail.md` | n/a | synthetic-error-handling-approved | `plan.md` Synthetic Evidence and contracts/readiness-evidence-contract.md | malformed readiness report fields, missing required capability data, hidden-warning fixtures, hostile artifact paths | validators reject the report with visible failure diagnostics and no screenshot-success claim | accepted-seh |

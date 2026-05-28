# Tasks: Working Screenshot Taking

**Feature branch**: `026-working-screenshot-taking`
**Spec**: `specs/026-working-screenshot-taking/spec.md`
**Plan**: `specs/026-working-screenshot-taking/plan.md`

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
session against the packed library, a smoke run of the application, a manual
walk-through with transcript, or a screenshot captured under `readiness/`.
Domain, model, or core-layer changes alone do not satisfy `[X]` for a `[US*]`
task, even if their unit tests pass green. If the user-reachable surface is
missing, stubbed, or not yet wired, mark `[ ]` (work continues) or `[S]` with a
disclosed reason in the Synthetic-Evidence Inventory; never `[X]`.

For this feature, `[X]` on an I/O-bearing screenshot workflow task also requires
MVU/effect evidence: the public `EvidenceWorkflowModel`,
`EvidenceWorkflowMsg`, `EvidenceWorkflowEffect`, `initEvidenceWorkflow`, and
`updateEvidenceWorkflow` contract was exercised; pure transitions were tested;
emitted effects were asserted; and the interpreter was run against real viewer,
filesystem, or generated-command dependencies where safe.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, ... — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change
- **[SEH]** — design-approved synthetic error-handling task paired with
  `synthetic-error-handling-approved`

Every task has a matching entry in `tasks.deps.yml` with structured `deps` and
`skillist` metadata. Every task line mirrors that structured `skillist` value.

## Skill Evaluation Notes

Capability review matched screenshot viewer work to `fs-skia-skiaviewer`,
Testing validator work to `fs-skia-testing`, template/package generation work
to `fs-skia-template-update`, and graph/audit gates to
`speckit-evidence-graph` and `speckit-evidence-audit`. Valid-empty tasks are
readiness bookkeeping, local docs that do not change a capability-owned surface,
or repository verification orchestration.

Medium-confidence matches accepted:

| Task area | Candidate | Signals | Disposition |
|-----------|-----------|---------|-------------|
| Generated app screenshot command and guidance | `fs-skia-skiaviewer`, `fs-skia-testing`, `fs-skia-template-update` | generated viewer command, validation helpers, template content | accepted ordered multi-skill set only where files span those owners |
| Screenshot readiness host warning classification | `fs-skia-layout-evidence` | host warning classification wording overlaps but this feature is screenshot, not layout readability | rejected false-positive |
| Elmish/MVU workflow wording | `fs-skia-elmish` | Model/Msg/Effect terminology | rejected false-positive because viewer evidence workflow lives in `SkiaViewer`, not the Elmish adapter package |

## Governance Risk Levels

- **Small**: single-package implementation or docs-only updates. Focused
  validation may stop at the package test and directly touched generated check.
- **Medium**: public contract, generated command, or validator changes. Focused
  validation must include affected package tests, FSI transcripts, package
  surface checks, generated product/guidance checks, and task graph output.
- **Broad**: dependency, template package, aggregate build, audit policy, or
  cross-package behavior changes. Broad validation requires `./fake.sh build -t
  Verify`; any focused rerun after an aggregate hang is non-authoritative and
  must be recorded separately with command, stage, elapsed duration, and last
  observed output.

This feature is **medium by default** and becomes **broad** if a new native
capture dependency, template package pin, aggregate target, or audit policy
change is required.

---

## Phase 1: Setup

- [X] T001 [P] [skillist: speckit-evidence-graph] Create `specs/026-working-screenshot-taking/readiness/` and placeholder files for screenshot capture, artifacts, failure diagnostics, generated guidance, package surface baseline, risk levels, runtime limitations, aggregate hang diagnostics, task graph, and final audit evidence
- [X] T002 [P] [skillist: []] Record the Tier 1 scope, affected layers, public API impact, MVU/effect applicability, synthetic-success prohibition, and required real screenshot evidence obligations in the readiness package
- [X] T003 [P] [skillist: []] Inventory existing screenshot, persistent launch, bounded launch, scene, layout, and generated evidence commands so implementation preserves separation between evidence kinds

**Checkpoint**: Setup complete.

---

## Phase 2: Foundation

- [X] T004 [P] [skillist: fs-skia-skiaviewer] Update `src/SkiaViewer/SkiaViewer.fsi` with additive screenshot capture request/result, capture mode, blocked stage, pixel validation, `EvidenceWorkflowModel`, `EvidenceWorkflowMsg`, `EvidenceWorkflowEffect`, `initEvidenceWorkflow`, `updateEvidenceWorkflow`, and interpreter boundary contracts
- [X] T005 [P] [skillist: fs-skia-testing] Update `src/Testing/Testing.fsi` with screenshot evidence record parsing and artifact validation contracts that reject missing, unreadable, zero-dimension, blank, synthetic, metadata-only, deterministic-scene-only, manual, and untraceable claims
- [X] T006 [P] [skillist: fs-skia-skiaviewer, fs-skia-testing] Add failing FSI transcript coverage for the new SkiaViewer and Testing public contracts, including representative `initEvidenceWorkflow` and `updateEvidenceWorkflow` paths
- [X] T007 [P] [skillist: fs-skia-skiaviewer, fs-skia-testing] Add initial package surface baseline expectations for `FS.Skia.UI.SkiaViewer` and `FS.Skia.UI.Testing` so later intentional public changes are reviewed
- [X] T008 [P] [skillist: []] Record runtime limitations for .NET 10 desktop, Vulkan/Silk.NET, SkiaSharp preview, unsupported macOS/mobile/browser capture, and absence of a software-renderer fallback in `readiness/runtime-limitations.md`
- [X] T009 [skillist: []] Record governance risk levels, focused validation requirements, broad-validation triggers, and non-authoritative aggregate rerun handling in `readiness/governance-risk-levels.md`

**Checkpoint**: Foundation ready; story implementation may begin in parallel.

---

## Phase 3: User Story 1 - Capture A Real Rendered Screenshot

### Tests First

- [X] T010 [P] [US1] [skillist: fs-skia-skiaviewer] Add failing SkiaViewer semantic tests for accepted first-frame screenshot capture with `capture-source=live-viewer-window`, `proves-screenshot=true`, positive decoded dimensions, non-blank pixel validation, command/app/host/capture-mode/timestamp traceability, pure workflow transitions, and emitted capture/write/cleanup effects
- [X] T011 [P] [US1] [skillist: fs-skia-skiaviewer] Add failing SkiaViewer diagnostics tests for launch, first-frame, render, capture/readback, pixel validation, artifact write, timeout, and unsupported-host outcomes without successful screenshot claims
- [X] T012 [P] [US1] [skillist: fs-skia-skiaviewer] Add real-interpreter smoke coverage that runs the supported viewer screenshot path where host prerequisites are available and records unsupported-host negative evidence otherwise

### Implementation

- [X] T013 [US1] [skillist: fs-skia-skiaviewer] Implement the viewer-owned first-frame render-target PNG capture path using existing SkiaSharp/Silk.NET viewer surfaces before considering any new native capture dependency
- [X] T014 [US1] [skillist: fs-skia-skiaviewer] Implement readable PNG write, decoded dimension checks, non-blank pixel sampling, readiness-relative artifact path validation, and precise blocked-stage diagnostics for capture and artifact failures
- [X] T015 [US1] [skillist: fs-skia-skiaviewer] Wire the SkiaViewer evidence workflow interpreter so screenshot capture, file writes, and cleanup remain explicit effects while normal interactive launch behavior remains unchanged
- [X] T016 [US1] [skillist: fs-skia-skiaviewer] Produce `readiness/screenshot-capture-evidence.md` and `readiness/screenshot-artifacts.md` from a supported-host working-code PNG run, or leave a failed task with concrete blocked-stage evidence if the host cannot support capture

**Checkpoint**: US1 independently captures or honestly diagnoses a real viewer-backed screenshot workflow.

---

## Phase 4: User Story 2 - Use Screenshots As Reviewable Visual Evidence

### Tests First

- [X] T017 [P] [US2] [skillist: fs-skia-testing] Add failing Testing semantic tests for accepted screenshot evidence records with all required key/value fields, readiness-local artifact paths, live viewer capture source, positive dimensions, non-blank validation, and reviewer-traceable command/host/sample metadata
- [S] T018 [P] [US2] [SEH] synthetic-error-handling-approved [skillist: fs-skia-testing] Add failing Testing rejection tests for malformed screenshot records, corrupt PNG bytes, missing required fields, out-of-readiness artifact paths, and forced validator error results

### Implementation

- [X] T019 [US2] [skillist: fs-skia-testing] Implement screenshot evidence record parsing and artifact validation helpers in `src/Testing/Testing.fs`, preserving strict rejection of metadata-only, structural, manual, synthetic, fallback-only, blank, unreadable, and untraceable proof
- [X] T020 [US2] [skillist: fs-skia-testing] Connect Testing validators to readiness evidence checks and write reviewer-facing validation output to `readiness/screenshot-artifacts.md`
- [X] T021 [US2] [skillist: fs-skia-testing] Document the accepted screenshot record shape, artifact inspection rules, and rejection cases in `docs/testing.md` and `docs/evidence.md`

**Checkpoint**: US2 lets reviewers validate and inspect screenshot evidence without local reruns.

---

## Phase 5: User Story 3 - Diagnose Capture Failures Honestly

### Tests First

- [S] T022 [P] [US3] [SEH] synthetic-error-handling-approved [skillist: fs-skia-skiaviewer, fs-skia-testing] Add failing rejection and diagnostic tests for invalid command arguments, missing required output paths, forced launch/render/readback/write failures, malformed validator input, and explicit unsupported classifications
- [X] T023 [P] [US3] [skillist: fs-skia-skiaviewer] Add failing tests that unsupported-host and failed capture records include blocked stage, classification, category, host facts, attempted command, message, and missing evidence fields while claiming no screenshot success

### Implementation

- [X] T024 [US3] [skillist: fs-skia-skiaviewer] Implement host-prerequisite detection and earliest-known blocked-stage classification for desktop prerequisite, launch, first frame, render, capture, readback, pixel validation, artifact write, timeout, and unknown failures
- [X] T025 [US3] [skillist: fs-skia-skiaviewer, fs-skia-testing] Write `readiness/capture-failure-diagnostics.md` with real unsupported-host or failure evidence produced by an actual command attempt and no synthetic screenshot substitute
- [X] T026 [US3] [skillist: []] Record aggregate hang diagnostics with verdict, stage, elapsed duration, last observed command, focused rerun command, and non-authoritative aggregate status in `readiness/aggregate-hang-diagnostics.md` whenever broad verification stalls or times out

**Checkpoint**: US3 failure paths are actionable and cannot be mistaken for screenshot proof.

---

## Phase 6: User Story 4 - Keep Screenshot Evidence Separate From Other Evidence

### Tests First

- [X] T027 [P] [US4] [skillist: fs-skia-testing] Add failing tests that launch evidence, persistent-launch evidence, deterministic scene reports, layout/readability evidence, pixel-readback diagnostics, metadata, and manual descriptions do not satisfy screenshot-required readiness packages
- [X] T028 [P] [US4] [skillist: fs-skia-skiaviewer, fs-skia-testing, fs-skia-template-update] Add generated product and generated guidance tests for `--screenshot-evidence` as a distinct opt-in operation on screenshot-ready visual profiles and absence of screenshot requirements on headless/non-ready profiles

### Implementation

- [X] T029 [US4] [skillist: fs-skia-skiaviewer, fs-skia-testing, fs-skia-template-update] Wire generated product screenshot evidence commands in `template/base/src/Product/EvidenceCommands.fs` and related program entry points without changing the default interactive launch path
- [X] T030 [US4] [skillist: fs-skia-template-update] Update `docs/generated-apps.md`, generated product docs, template fragments, and generated guidance text to name the screenshot command, artifact locations, acceptance rules, unsupported behavior, and separation from launch/layout/scene evidence
- [X] T031 [US4] [skillist: fs-skia-testing, speckit-evidence-graph, speckit-evidence-audit] Extend governed validation so screenshot-required visual features fail graph/audit readiness when screenshot records or PNG artifacts are missing, unreadable, blank, synthetic, fallback-only, or untraceable
- [X] T032 [US4] [skillist: fs-skia-template-update] Run template and generated-product validation, then write `readiness/generated-guidance.md` with commands, outputs, and any non-authoritative aggregate caveats

**Checkpoint**: US4 preserves separate evidence categories and generated workflows.

---

## Phase 7: Integration & Polish

- [X] T033 [skillist: fs-skia-skiaviewer, fs-skia-testing] Refresh FSI transcripts and package surface baselines for `FS.Skia.UI.SkiaViewer` and `FS.Skia.UI.Testing`, then record results in `readiness/package-surface-baseline.md`
- [X] T034 [skillist: fs-skia-skiaviewer] Run `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` and record focused results with any host limitation notes
- [X] T035 [skillist: fs-skia-testing] Run `dotnet test tests/Testing.Tests/Testing.Tests.fsproj` and record focused validator results
- [X] T036 [skillist: fs-skia-skiaviewer, fs-skia-testing, fs-skia-template-update] Run generated and governance validation targets: `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`, `PackageSurfaceCheck`, and `FsiTranscripts`
- [X] T037 [skillist: speckit-evidence-graph] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/026-working-screenshot-taking --graph-only` and copy the task graph result to `readiness/evidence-graph.md`
- [X] T038 [skillist: fs-skia-skiaviewer] Run repeated supported-host screenshot capture for a stable graphical sample, record run count, accepted artifact count, pass rate, failures, and artifact paths in `readiness/screenshot-capture-evidence.md`, and verify the result meets SC-001's 95% threshold
- [X] T039 [skillist: speckit-evidence-graph, speckit-evidence-audit] Run final evidence audit and record the result in `readiness/evidence-audit.md`, documenting every remaining blocker without using synthetic screenshot success
- [X] T040 [skillist: []] Run `./fake.sh build -t Verify` for broad validation when triggered by the risk rules, or record why focused medium-risk validation is sufficient for this feature state

**Checkpoint**: Feature readiness is complete only after real screenshot evidence, focused checks, graph validation, and final audit evidence are present.

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T018 | Tests rejection behavior that cannot be represented by accepted screenshot proof and must use malformed or corrupt inputs | `specs/026-working-screenshot-taking/readiness/screenshot-artifacts.md` | _(none)_ | synthetic-error-handling-approved | `specs/026-working-screenshot-taking/plan.md` Synthetic Evidence and `contracts/screenshot-evidence-record-contract.md` Acceptance | malformed line-oriented record, corrupt PNG bytes, missing required data, out-of-readiness path, forced validator error result | validator rejects the record or artifact with a precise failed status and no screenshot proof claim | accepted-seh |
| T022 | Tests explicit error-path diagnostics without manufacturing a successful screenshot | `specs/026-working-screenshot-taking/readiness/capture-failure-diagnostics.md` | _(none)_ | synthetic-error-handling-approved | `specs/026-working-screenshot-taking/plan.md` Synthetic Evidence and `contracts/screenshot-capture-contract.md` Unsupported or Failed Result | invalid command arguments, missing required output path, forced launch/render/readback/write failure, malformed validator input | workflow reports unsupported or failed with blocked stage, classification, category, message, missing evidence fields, and no successful artifact claim | accepted-seh |

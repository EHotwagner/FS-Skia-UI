# Tasks: Asteroids Integration Feedback

**Feature branch**: `020-asteroids-integration-feedback`
**Spec**: `specs/020-asteroids-integration-feedback/spec.md`
**Plan**: `specs/020-asteroids-integration-feedback/plan.md`

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
missing, stubbed, or not yet wired, mark `[ ]` or `[S]` with a disclosed reason
in the Synthetic-Evidence Inventory.

For stateful or I/O-bearing stories, `[X]` also requires Elmish/MVU evidence:
the public `Model` / `Msg` / `Effect` or `Cmd<Msg>` contract was exercised,
pure `update` transitions were tested, emitted effects were asserted, and the
effect interpreter was run against real dependencies where safe.

This feature changes validation and generated-app evidence workflows rather
than gameplay MVU mechanics. Principle IV applies to generated app contracts
and warning/evidence workflow boundaries where state or I/O is introduced; pure
geometry classifiers require pure transition/semantic tests and do not require
a separate Elmish interpreter.

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

- Small: one package-local helper, doc-only wording, or one focused validator rule. Run the touched package tests plus the named readiness file.
- Medium: generated template, public guidance, or validation workflow changes. Run affected package tests, `GeneratedGuidanceCheck`, `GeneratedProductCheck`, and `TemplateCheck`.
- Broad: public `.fsi`, package surface, generated product behavior, or readiness/audit semantics change. Run `Verify`, `PackageSurfaceCheck`, `EvidenceGraph`, and `EvidenceAudit`; record any aggregate-only result as non-authoritative until focused evidence exists.

## Phase 1: Setup

- [X] T001 [P] [skillist: []] Scaffold `specs/020-asteroids-integration-feedback/readiness/` with the six required readiness files and placeholder command/status sections
- [X] T002 [P] [skillist: fs-skia-layout-evidence] Review and harden `.agents/skills/fs-skia-layout-evidence/SKILL.md` against the spec contracts, then record the resolved skill path in readiness notes
- [X] T003 [P] [skillist: fs-skia-layout-evidence] Add the layout evidence skill to `template/capabilities.yml` and any generated capability inventory consumed by task or guidance validation
- [X] T004 [skillist: fs-skia-layout-evidence] Record Tier 1 scope, public API impact, generated product impact, MVU/effect-boundary applicability, synthetic limitations, and required evidence obligations in `readiness/layout-evidence.md`

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: fs-skia-layout-evidence, fs-skia-scene, fs-skia-testing] Draft public `.fsi` contracts for layout proof levels, HUD/gameplay/text bounds, overlap diagnostics, unsupported reasons, and generated validation result types in `src/Scene/Scene.fsi` and/or `src/Testing/Testing.fsi`
- [X] T006 [P] [skillist: fs-skia-layout-evidence, fs-skia-scene, fs-skia-testing] Add failing semantic tests through the public `.fsi` surface for readable layout, deterministic-render-only evidence, unsupported layout inspection, and missing/overlapping bounds
- [X] T007 [P] [skillist: fs-skia-layout-evidence] Add failing governance tests that require `fs-skia-layout-evidence` metadata for layout, evidence, guidance, validation, and warning-classification tasks
- [X] T008 [P] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Add failing generated guidance checks for qualified `Product.Program.view`, `Product.Program.generatedHost`, and `Product.Program.update` names
- [X] T009 [skillist: fs-skia-layout-evidence, fs-skia-scene, fs-skia-testing] Exercise the draft public contracts from FSI, including representative readable, deterministic-only, and unsupported evidence records, and capture `readiness/public-contract-guidance.md`
- [X] T010 [skillist: fs-skia-layout-evidence, fs-skia-scene, fs-skia-testing] Record initial package surface review expectations for changed Scene/Testing signatures and planned baseline updates

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 - Keep Game HUD Readable (P1)

### Tests First

- [X] T011 [P] [US1] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Add generated product tests for 1280x720 HUD/gameplay separation, 640x480 HUD readability, HUD/HUD overlap failure, and HUD/gameplay overlap failure
- [X] T012 [P] [US1] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Add generated product tests proving movement, wrap, spawn, collision, and active entity bounds use the gameplay region instead of the full scene
- [X] T013 [P] [US1] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Add template/guidance fixtures that describe the reserved HUD region, gameplay region, documented small-window size, and evidence command

### Implementation

- [X] T014 [US1] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Update generated game layout and rendering so score, lives, wave, status, and active gameplay entities are emitted with named HUD/gameplay regions
- [X] T015 [US1] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Update generated gameplay coordinate policies so entities wrap, clamp, spawn, and collide inside the gameplay region when the HUD region is reserved
- [X] T016 [US1] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Produce real generated product evidence for 1280x720 and 640x480 validation sizes and write `readiness/hud-layout-readability.md`
- [X] T017 [US1] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Document the independent US1 validation path in generated docs and `readiness/generated-validation.md`

**Checkpoint**: User Story 1 is fully functional and testable independently.

---

## Phase 4: User Story 2 - Discover Public Scene And Host Contracts (P1)

### Tests First

- [X] T018 [P] [US2] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Add public docs and generated guidance tests that fail on omitted or inconsistent `Product.Program.view`, `Product.Program.generatedHost`, or `Product.Program.update`
- [X] T019 [P] [US2] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Add an FSI or generated-product signature smoke test that writes an app-owned signature using `FS.Skia.UI.Scene.Scene`, the generated host value, and a qualified update call

### Implementation

- [X] T020 [US2] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Update `docs/generated-apps.md`, `docs/testing.md`, template docs, and generated examples to use the qualified app-owned scene, host, and update names consistently
- [X] T021 [US2] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Run the consumer guidance smoke path and record command output, source snippets, and result in `readiness/public-contract-guidance.md`

**Checkpoint**: User Story 2 is fully functional and testable independently.

---

## Phase 5: User Story 3 - Validate Layout-Sensitive Scene Evidence (P2)

### Tests First

- [X] T022 [P] [US3] [skillist: fs-skia-layout-evidence, fs-skia-scene, fs-skia-testing] Add semantic tests for evidence report fields: scene, output size, proof level, HUD region, gameplay region, text bounds, gameplay bounds, overlap status, measurement mode, unsupported reason, and diagnostics
- [X] T023 [P] [US3] [skillist: fs-skia-layout-evidence, fs-skia-testing] Add validation helper tests that fail readability claims with missing facts, undisclosed unsupported facts, HUD/HUD overlap, HUD/gameplay overlap, or deterministic-render-only proof
- [X] T024 [P] [US3] [skillist: fs-skia-layout-evidence, fs-skia-scene, fs-skia-testing] Add malformed/missing layout fact coverage for unsupported and invalid evidence reports using public layout evidence constructors and classifiers

### Implementation

- [X] T025 [US3] [skillist: fs-skia-layout-evidence, fs-skia-scene, fs-skia-testing] Implement the public layout evidence records, proof-level classification, conservative approximate measurement disclosure, and overlap diagnostics
- [X] T026 [US3] [skillist: fs-skia-layout-evidence, fs-skia-testing] Implement generated validation helpers that fail on missing, unsupported-without-disclosure, overlapping, or deterministic-only readability claims
- [X] T027 [US3] [skillist: fs-skia-layout-evidence, fs-skia-scene, fs-skia-testing] Produce layout evidence artifacts and write `readiness/layout-evidence.md`

**Checkpoint**: User Story 3 is fully functional and testable independently.

---

## Phase 6: User Story 4 - Separate Benign Host Warnings From Real Failures (P3)

### Tests First

- [X] T028 [P] [US4] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-testing] Add warning-classification tests for benign environment warnings, launch failures, rendering failures, layout failures, package failures, and unknown warnings
- [X] T029 [P] [US4] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer] Add readiness/report tests proving benign warnings are non-fatal only when launch/render/layout/package evidence is otherwise successful or explicitly unsupported without a readability claim

### Implementation

- [X] T030 [US4] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer, fs-skia-testing] Implement host warning classification records, non-fatal rule evaluation, and diagnostics that preserve real launch/render/layout/package failures
- [X] T031 [US4] [skillist: fs-skia-layout-evidence, fs-skia-skiaviewer] Run a persistent graphical launch path or supported unsupported-host diagnostic path and write `readiness/host-warning-classification.md`

**Checkpoint**: User Story 4 is fully functional and testable independently.

---

## Phase 7: Integration & Polish

- [X] T032 [skillist: fs-skia-layout-evidence, fs-skia-scene, fs-skia-testing] Refresh intentional Scene/Testing package surface baselines and record `PackageSurfaceCheck` evidence
- [X] T033 [skillist: fs-skia-layout-evidence, fs-skia-template-update] Run `GeneratedProductCheck`, `GeneratedGuidanceCheck`, and `TemplateCheck`, then update `readiness/generated-validation.md`
- [X] T034 [skillist: fs-skia-layout-evidence] Run focused readiness review for `hud-layout-readability.md`, `public-contract-guidance.md`, `layout-evidence.md`, `host-warning-classification.md`, and `generated-validation.md`
- [X] T035 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` and confirm no cycles, dangling refs, mirror mismatches, invalid skill ids, or `[S*]` surprises
- [X] T036 [skillist: speckit-evidence-graph, speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit`, write `readiness/evidence-audit.md`, and document every accepted synthetic or unsupported condition
- [X] T037 [skillist: fs-skia-layout-evidence, fs-skia-template-update] Measure generated layout-readability and guidance validation duration on a prepared supported host, require completion under 5 minutes, and record elapsed time in `readiness/generated-validation.md`
- [X] T038 [skillist: fs-skia-layout-evidence] Run `./fake.sh build -t Verify` for broad Tier 1 validation, then record focused failures separately from non-authoritative aggregate results

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. For `[SEH]`
rows, include the approval label, design-phase source, synthetic input class,
expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|

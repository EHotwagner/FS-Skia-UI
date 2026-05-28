# Tasks: Upgrade SkiaSharp And Spec Kit

**Feature branch**: `025-upgrade-skia-speckit`
**Spec**: `specs/025-upgrade-skia-speckit/spec.md`
**Plan**: `specs/025-upgrade-skia-speckit/plan.md`

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
No synthetic error-handling tasks are approved for this feature at task
generation time.

## Vertical-slice rule (US phases)

A task tagged `[US*]` may only be marked `[X]` when the change is reachable
from a reviewer-visible entry point and that path was actually exercised:
readiness evidence from package metadata, repository scans, generated output,
FAKE targets, FSI/package-surface transcripts, generated product validation, or
documented unsupported-host command output. In-memory-only tests, placeholder
reports, and aggregate command success alone do not satisfy `[X]` for a user
story task.

Principle IV does not introduce new product MVU work for this feature. Existing
build, dependency, template, package-surface, and evidence workflows are
I/O-bearing boundaries; tasks that change those boundaries must preserve
request/result facts, actionable failure diagnostics, and real command evidence
where safe.

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
- `./fake.sh build -t FsiTranscripts` for public FSI evidence when public signatures change.
- `./fake.sh build -t SampleContractSmoke` for sample smoke evidence.
- `./fake.sh build -t TemplateCheck` for source/package/default/minimal generated project validation.
- `./fake.sh build -t DependencyReport` for central package governance.
- `./fake.sh build -t GeneratedGuidanceCheck` for generated prompt, task-skill, and implementation guidance governance.
- `./fake.sh build -t TemplateDrift` for template-owned drift and deferral validation.
- `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit` for graph and synthetic-evidence gates.

Governance risk levels for this feature:

- Small: readiness-only wording, one focused documentation clarification, or a single validator expectation. Run the touched check plus the named readiness artifact.
- Medium: generated template pins, Spec Kit copied assets, package guidance, or compatibility inventory tooling. Run affected governance/package tests plus `DependencyReport`, `GeneratedGuidanceCheck`, and `TemplateCheck`.
- Broad: central package version movement, public `.fsi` or package-surface difference, template package metadata, generated profile behavior, or native/viewer validation outcome. Run `PackageSurfaceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`, and `Verify`; record aggregate-only results as non-authoritative until focused evidence exists.

## Skill Evaluation Notes

- High confidence matches: `fs-skia-template-update` for template package pins, template package metadata, generated profiles, selected local skills, and `dotnet new fs-skia-ui` validation; `fs-skia-skiaviewer` for SkiaSharp/native asset and viewer diagnostic evidence; `fs-skia-testing` for package-surface and readiness validators; `speckit-evidence-graph` and `speckit-evidence-audit` for final gates.
- Medium or indirect signals: `fs-skia-scene`, `fs-skia-layout`, `fs-skia-elmish`, `fs-skia-keyboard-input`, and `fs-skia-ui-widgets` may be reviewed if package-surface or generated guidance evidence shows touched focused-package public contracts, but the planned upgrade does not change those capability APIs. `fs-skia-project` applies only after a generated product exists, so source-side template work uses `fs-skia-template-update`.
- Valid-empty dispositions: compatibility consumer inventory, release posture, dependency documentation, and version-selection evidence span repository governance rather than one capability skill; `[skillist: []]` is intentional there unless implementation discovers a narrower touched package contract.

## Phase 1: Setup

- [X] T001 [P] [skillist: []] Scaffold or refresh `specs/025-upgrade-skia-speckit/readiness/` placeholders for `version-selection.md`, `dependency-report.md`, `template-version-alignment.md`, `compatibility-consumer-inventory.md`, `compatibility-public-surface-map.md`, `compatibility-sample-migration.md`, `compatibility-release-policy.md`, `package-surface-baseline.md`, `evidence-audit.md`, and `logs/`
- [X] T002 [P] [skillist: []] Capture pre-upgrade baseline status for `DependencyReport`, `PackageSurfaceCheck`, `TemplateCheck`, and `GeneratedGuidanceCheck` with command paths or unsupported reasons under `readiness/logs/`
- [X] T003 [P] [skillist: fs-skia-template-update, fs-skia-skiaviewer, fs-skia-testing] Resolve capability guidance for template package updates, Skia/native viewer validation, package-surface checks, and readiness validator evidence
- [X] T004 [skillist: []] Record Tier 1 scope, affected files, public API default of no `.fsi` change, MVU/effect-boundary applicability, synthetic-evidence restrictions, small/medium/broad risk levels, and required readiness obligations

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: []] Define the version-selection review table for SkiaSharp package family and Spec Kit asset set, including current versions, implementation-time source checks, affected files, alignment rules, risk notes, and validation status
- [X] T006 [P] [skillist: fs-skia-testing] Add failing governance tests for SkiaSharp package-family alignment, Spec Kit root/generated asset alignment, and dependency-report before/after facts
- [X] T007 [P] [skillist: fs-skia-template-update, fs-skia-testing] Add failing governance or package tests for generated template package-pin alignment, selected skill asset alignment, and accidental broad `FS.Skia.UI` dependency detection in generated profiles
- [X] T008 [P] [skillist: fs-skia-testing] Add failing package-surface and compatibility governance checks that guard against unapproved `FS.Skia.UI` public API changes and require a recorded compatibility consumer inventory
- [X] T009 [skillist: []] Run or script the initial repository scan for `FS.Skia.UI` consumers across project references, package references, namespace opens, samples, templates, docs, and packaged-mode guidance
- [X] T010 [skillist: []] Record the Tier 1 public-surface review rule in readiness, including why no `.fsi` sketch is required unless package-surface evidence detects a public contract delta; any discovered compatibility-package surface change pauses implementation for `.fsi` sketch, semantic tests, FSI/package-surface evidence, docs, and explicit approval

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 - Govern Dependency Upgrade Readiness (P1)

### Tests First

- [X] T011 [P] [US1] [skillist: fs-skia-testing] Add or update tests that fail when SkiaSharp managed and native asset package declarations diverge, or when dependency-report evidence lacks before/after versions and cycle/spread status
- [X] T012 [P] [US1] [skillist: fs-skia-template-update, fs-skia-testing] Add or update tests that fail when Spec Kit root metadata, generated template copies, selected local skills, or generated guidance assets do not match the approved version/range

### Implementation

- [X] T013 [US1] [skillist: []] Re-check current official source-of-truth versions for SkiaSharp packages and Spec Kit assets immediately before editing, then write selected versions, checked-at timestamps, source URLs or governed paths, repository maintainer acceptance, affected files, and risks to `readiness/version-selection.md`
- [X] T014 [US1] [skillist: fs-skia-skiaviewer] Update repository-owned SkiaSharp package-family declarations consistently, preserving native asset package alignment and recording any viewer/native validation risk
- [X] T015 [US1] [skillist: fs-skia-template-update] Update repository-owned Spec Kit metadata, extensions, presets, templates, workflows, generated copies, selected local skills, and package guidance files required by the approved Spec Kit version/range
- [X] T016 [US1] [skillist: fs-skia-testing] Run focused dependency/package validation and write `readiness/dependency-report.md` with before/after package graph, dependency closure, cycle status, unexpected spread review, and non-authoritative aggregate notes

**Checkpoint**: User Story 1 is fully functional and testable independently.

---

## Phase 4: User Story 2 - Preserve Compatibility Package Direction (P1)

### Tests First

- [X] T017 [P] [US2] [skillist: fs-skia-testing] Add or update governance tests that require `FS.Skia.UI` consumer inventory coverage for source projects, package metadata, templates, generated output, samples, docs, namespace usage, and packaged-mode guidance
- [X] T018 [P] [US2] [skillist: fs-skia-testing] Add or update checks that require compatibility public-surface classification, focused replacement mapping, package-surface baseline status, and explicit release posture before accepting compatibility-package conclusions

### Implementation

- [X] T019 [US2] [skillist: []] Produce `readiness/compatibility-consumer-inventory.md` from real repository scans, with path, consumer kind, usage kind, package mode, focused replacement, migration status, and notes for every `FS.Skia.UI` consumer
- [X] T020 [US2] [skillist: []] Produce `readiness/compatibility-public-surface-map.md` classifying reviewed public compatibility areas as primary-only, duplicate, facade candidate, deprecated candidate, or permanent compatibility surface with focused equivalents or explicit gaps
- [X] T021 [US2] [skillist: []] Produce `readiness/compatibility-sample-migration.md` documenting representative sample/package-mode keep-unchanged or migration decisions and preserving supported sample behavior
- [X] T022 [US2] [skillist: []] Produce `readiness/compatibility-release-policy.md` with the near-term `FS.Skia.UI` posture, migration guidance, deferred decisions, unknown external-consumer limits, and user-facing package-choice guidance

**Checkpoint**: User Story 2 is fully functional and testable independently.

---

## Phase 5: User Story 3 - Keep Generated Users On Supported Package Pins (P2)

### Tests First

- [X] T023 [P] [US3] [T2] [skillist: fs-skia-template-update, fs-skia-testing] Add or update template tests requiring generated package pins to match repository package posture for default, governed, headless, and sample-pack profiles where applicable
- [X] T024 [P] [US3] [T2] [skillist: fs-skia-template-update, fs-skia-testing] Add or update generated-profile checks proving focused-package profiles do not accidentally regain or expand a broad `FS.Skia.UI` dependency

### Implementation

- [X] T025 [US3] [T2] [skillist: fs-skia-template-update] Update `template/base/Directory.Packages.props`, template Spec Kit assets, template docs/guidance, selected copied skills, and `.template.package/FS.Skia.UI.Template.fsproj` metadata required by the approved versions
- [X] T026 [US3] [T2] [skillist: fs-skia-template-update] Pack/install the local template as needed, instantiate supported profiles, restore/test generated projects or record real unsupported-host facts, and capture profile command logs under `readiness/logs/`
- [X] T027 [US3] [T2] [skillist: fs-skia-template-update] Write `readiness/template-version-alignment.md` with checked profiles, emitted package pins, Spec Kit assets, selected skills, broad-package dependency status, validation commands, and pass/fail/unsupported evidence paths

**Checkpoint**: User Story 3 is fully functional and testable independently.

---

## Phase 6: User Story 4 - Document Upgrade And Compatibility Outcomes (P2)

### Tests First

- [X] T028 [P] [US4] [T2] [skillist: []] Add or update documentation/guidance checks requiring upgraded version facts, compatibility posture, focused-package recommendation, conservative broad-package guidance, and deferred-decision wording
- [X] T029 [P] [US4] [T2] [skillist: fs-skia-skiaviewer] Add or update checks that preserve unsupported-host and viewer/native diagnostic behavior as observable compatibility evidence unless an intentional documented change is approved

### Implementation

- [X] T030 [US4] [T2] [skillist: []] Update `docs/dependencies.md`, generated package guidance, Spec Kit docs, compatibility analysis follow-up notes, and release-facing documentation with selected versions, risk notes, compatibility posture, and deferred decisions
- [X] T031 [US4] [T2] [skillist: fs-skia-skiaviewer] Record any SkiaSharp/native/viewer unsupported-host facts from real command output, including platform, command, failure reason, and whether the result blocks acceptance

**Checkpoint**: User Story 4 is fully functional and testable independently.

---

## Phase 7: Integration & Polish

- [X] T032 [skillist: fs-skia-testing] Run `PackageSurfaceCheck`, compare compatibility-package baselines, and write `readiness/package-surface-baseline.md` as the Tier 1 `.fsi`/surface evidence: unchanged, intentional change with `.fsi` path and migration guidance, or blocked
- [X] T033 [skillist: fs-skia-template-update, fs-skia-testing] Run `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateCheck`, and `TemplateDrift`, then update dependency/template readiness artifacts with focused results and non-authoritative aggregate notes
- [X] T034 [skillist: []] Review the nine required readiness artifacts for real-evidence provenance, source paths or URLs, affected files, before/after facts, generated profile status, compatibility consumer counts, and unsupported-host facts
- [X] T035 [skillist: []] Run `PackLocal` and representative `SampleContractSmoke` or equivalent packaged/local sample validation, then record whether supported sample behavior is preserved or intentionally migrated
- [X] T036 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` and confirm no cycles, dangling refs, mirror mismatches, invalid skill ids, non-minimal skill sets, or `[S*]` surprises
- [X] T037 [skillist: []] Run `./fake.sh build -t Verify` for broad Tier 1 validation, record aggregate result status separately from focused evidence, and note whether `Ci` is required before merge readiness
- [X] T038 [skillist: []] Run the SC-003 reviewer trace: starting from `readiness/compatibility-consumer-inventory.md`, trace every `FS.Skia.UI` repository consumer to classification and migration posture in under 10 minutes, then record elapsed time and reviewed paths
- [X] T039 [skillist: speckit-evidence-graph, speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit`, write `readiness/evidence-audit.md`, and document any accepted unsupported or synthetic condition without using synthetic evidence for version, dependency, template, inventory, or package-surface proof

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section. For `[SEH]`
rows, include the approval label, design-phase source, synthetic input class,
expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |

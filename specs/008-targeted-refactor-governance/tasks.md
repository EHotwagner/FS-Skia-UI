# Tasks: Targeted Refactor and Governance Diagnostics

**Feature branch**: `008-targeted-refactor-governance`
**Spec**: `specs/008-targeted-refactor-governance/spec.md`
**Plan**: `specs/008-targeted-refactor-governance/plan.md`

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
from a maintainer-facing, consumer-facing, or operator-facing entry point and
that path was actually exercised. For this feature, those entry points include
the stable public library signature in `src/Lib/Library.fsi`, packed-library
surface checks, `Viewer.run`/`VulkanHost.run` smoke evidence where safe,
layout `Layout.evaluate`, and canonical workflow targets such as
`./fake.sh build -t Dev`, `PackageSurfaceCheck`, `GeneratedGuidanceCheck`,
`TemplateDrift`, `Verify`, and `Ci`.

Runtime and governance work is stateful or I/O-bearing. Principle IV evidence
must therefore cover the existing public or internal contracts that own state:
`ViewerProgram`, `ViewerEffect`, and `Viewer.run` for runtime behavior;
named Vulkan startup stages plus cleanup ownership at `VulkanHost.run`; and
`BuildModel`, `BuildMsg`, `BuildEffect`, pure `update`, emitted effects, and
the edge interpreter for build/governance workflows. `[X]` for story work
requires pure transition tests where a transition model exists, emitted-effect
assertions, and real interpreter or smoke evidence where safe.

This rule does not apply to Setup, Foundation, Integration, or Polish phase
tasks; those are evaluated against their own phase verification.

## Task Annotations

- **[P]** - parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, ... - user-story scope
- **[T1]** / **[T2]** - Tier 1 (contracted) vs Tier 2 (internal) change

Every task must have a matching entry in `tasks.deps.yml` even if its
dependency list is empty. The evidence graph command refuses to proceed with
dangling references.

## Canonical Verification Targets

Implementation tasks should call repository targets instead of duplicating raw
restore/build/test/package/evidence command order:

- `./fake.sh build -t Dev` for fast local restore/build/default non-visual tests.
- `./fake.sh build -t Verify` for the full governed workflow.
- `./fake.sh build -t Ci` for automation delegation to `Verify`.
- `./fake.sh build -t PackLocal` for local package output.
- `./fake.sh build -t RefreshSurfaceBaselines` for intentional current surface
  baseline refreshes.
- `./fake.sh build -t PackageSurfaceCheck` for package surface review.
- `./fake.sh build -t FsiTranscripts` for public FSI evidence.
- `./fake.sh build -t SampleContractSmoke` for sample smoke evidence.
- `./fake.sh build -t GeneratedGuidanceCheck` for generated spec/plan prompt
  governance.
- `./fake.sh build -t TemplateDrift` for template-owned drift and deferral
  validation.
- `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`
  for graph and synthetic-evidence gates.

Keep `tasks.deps.yml` and the evidence graph status refresh requirements in
this generated task list.

---

## Phase 1: Setup

- [X] T001 Create feature readiness scaffolding under `specs/008-targeted-refactor-governance/readiness/` for public surface, semantic tests, native cleanup, native smoke, build organization, generated guidance, template drift, Yoga fallback diagnostics, record invariants, follow-ups, graph output, and audit output
- [X] T002 [P] Inventory current public surface inputs in `specs/008-targeted-refactor-governance/readiness/public-surface-inventory.md`, including `src/Lib/Library.fsi`, package surface baselines under `readiness/surface-baselines/`, samples, and package tests
- [X] T003 [P] Inventory current `src/Lib/Library.fs` responsibilities in `specs/008-targeted-refactor-governance/readiness/runtime-responsibility-map.md`, covering scene state, diagnostics, drawing, native resources, frame flow, screenshots, and viewer hosting
- [X] T004 [P] Inventory current `build.fsx` concern areas and FAKE target load requirements in `specs/008-targeted-refactor-governance/readiness/build-organization.md`
- [X] T005 [P] Record feature Tier, public-API impact, MVU/effect-boundary applicability, synthetic native evidence policy, unsupported scope, and required evidence obligations in `specs/008-targeted-refactor-governance/readiness/evidence-obligations.md`
- [X] T006 [P] Create a traceability matrix in `specs/008-targeted-refactor-governance/readiness/traceability.md` mapping FR/SC/contract targets to tests, implementation files, commands, and readiness artifacts

**Checkpoint**: Setup complete.

---

## Phase 2: Foundation

### Tests First

- [X] T007 [P] Add shared governance test helpers in `tests/Governance.Tests/TestSupport.fs` for Markdown section spans, path-class fixtures, same-diff evidence fixtures, readiness table parsing, and command output assertions
- [X] T008 [P] Add failing public surface stability checks in `tests/Package.Tests/SurfaceAreaTests.fs` proving `src/Lib/Library.fsi`, package baselines, helper-module exports, and any new helper `.fsi` files do not introduce unapproved package-visible public modules or members for this feature
- [X] T009 [P] Add failing runtime organization checks proving any accepted `src/Lib/Library.fs` split uses paired `.fsi` contracts or documented named fallback sections without top-level visibility modifiers in `.fs` files
- [X] T010 [P] Add failing deterministic native lifecycle test scaffolding in `tests/Lib.Tests/NativeStartupCleanupTests.fs` for owned resource categories, injected acquisition failures, release order, and synthetic disclosure
- [X] T011 [P] Add failing build organization checks in `tests/Governance.Tests/CommandContractTests.fs` for `BuildModel`, `BuildMsg`, `BuildEffect`, pure `update`, emitted effects, edge interpreter behavior, and `Dev`/`Verify`/`Ci` load contracts

### Implementation

- [X] T012 Record failing-first foundation output under `specs/008-targeted-refactor-governance/readiness/logs/` for the public surface, runtime organization, native lifecycle, and build organization checks
- [X] T013 Define the internal helper contract and compile-order strategy in `specs/008-targeted-refactor-governance/readiness/runtime-responsibility-map.md`, including accepted `.fsi` file pairs or named-section fallback rules
- [X] T014 Add native startup stage and ownership fixture contracts used by tests without exposing new public API from `src/Lib/Library.fsi`
- [X] T015 Add governance fixture directories and sample files for generated guidance section failures, deferred-scope placement failures, template drift path classes, same-diff alignment evidence, and accepted deferral records
- [X] T016 Run foundation verification for package surface checks, governance tests, native lifecycle scaffolding, and command-contract checks; store output under `specs/008-targeted-refactor-governance/readiness/logs/`

**Checkpoint**: Foundation ready - story implementation may begin in priority order.

---

## Phase 3: User Story 1 (US1) - Review Runtime Internals Safely

**Goal**: Separate the large runtime implementation into reviewable internal responsibility areas while preserving the public consumer contract.

**Independent Test**: Compare public surface evidence before and after, run semantic library tests, and verify reviewers can inspect runtime responsibilities independently.

### Tests First

- [X] T017 [P] [US1] [T2] Add packed-library and FSI-facing surface tests proving `src/Lib/Library.fsi`, public modules, samples, and package baselines remain source-compatible after internal splitting
- [X] T018 [P] [US1] [T2] Add runtime organization tests or fixtures proving scene model, diagnostics, drawing, native resources, frame flow, screenshots, and viewer hosting are separated by files or named sections with recorded reviewer notes
- [X] T019 [P] [US1] [T2] Add semantic runtime tests for `ViewerProgram`, `ViewerEffect`, pure update behavior, emitted effects, and real interpreter or smoke evidence where safe

### Implementation

- [X] T020 [US1] [T2] Move scene-state and runtime diagnostic helpers from `src/Lib/Library.fs` into paired helper files such as `src/Lib/SceneModel.fsi`/`.fs` and `src/Lib/RuntimeDiagnostics.fsi`/`.fs`, or record named-section fallback evidence
- [X] T021 [US1] [T2] Move drawing, frame, screenshot, and host-adjacent helpers into paired helper files such as `src/Lib/VulkanFrame.fsi`/`.fs`, or record named-section fallback evidence when the split is not compile-stable
- [X] T022 [US1] [T2] Update `src/Lib/Lib.fsproj` compile order and internal call sites so the public `Library.fs` facade and `src/Lib/Library.fsi` contract remain stable
- [X] T023 [US1] [T2] Run `./fake.sh build -t PackageSurfaceCheck`, focused `tests/Lib.Tests`, and packed-library or FSI evidence; store outputs in `specs/008-targeted-refactor-governance/readiness/public-surface.txt` and `semantic-tests.txt`
- [X] T024 [US1] [T2] Finalize `specs/008-targeted-refactor-governance/readiness/runtime-responsibility-map.md` with accepted split files or named-section fallback rationale and reviewer notes

**Checkpoint**: US1 runtime refactor is independently testable.

---

## Phase 4: User Story 2 (US2) - Audit Native Resource Startup and Cleanup

**Goal**: Make Vulkan startup ordering and cleanup ownership explicit, stage-named, and testable.

**Independent Test**: Inject each acquisition failure category, assert acquired resources are released exactly once, verify failure diagnostics name the stage, and retain real native smoke evidence where available.

### Tests First

- [X] T025 [P] [US2] [T2] Add deterministic injected acquisition failure tests in `tests/Lib.Tests/NativeStartupCleanupTests.fs` for Vulkan instance, surface, device/queues, swapchain/images, command pool/buffers, fences, staging buffers/memory, and Skia GPU resources
- [X] T026 [P] [US2] [T2] Add tests for startup diagnostic stage names, original native error preservation, reverse cleanup order, ownership transfer points, successful shutdown, and repeated cleanup idempotency
- [X] T027 [P] [US2] [T2] Add a native smoke evidence plan that runs existing real Vulkan smoke where supported and records unsupported-environment diagnostics separately from implementation defects

### Implementation

- [X] T028 [US2] [T2] Add `src/Lib/VulkanResources.fsi` and `src/Lib/VulkanResources.fs` ownership helpers or equivalent scoped rules for owner, acquire stage, transfer point, release action, release order, and disposal state
- [X] T029 [US2] [T2] Add `src/Lib/VulkanStartup.fsi` and `src/Lib/VulkanStartup.fs` named startup stages with `Result`-based failure propagation and acquisition abstraction for deterministic tests
- [X] T030 [US2] [T2] Refactor `VulkanHost.run` in `src/Lib/Library.fs` to use the staged startup pipeline, unwind ownership in reverse order on failure, and transfer resources only after successful initialization
- [X] T031 [US2] [T2] Wire synthetic disclosure into native failure test names and readiness evidence, and preserve real native smoke invocation where the current environment supports it
- [X] T032 [US2] [T2] Run focused native startup cleanup tests and real native smoke or unsupported-environment smoke diagnostics; store outputs in `native-startup-cleanup-tests.txt` and `native-smoke.txt`
- [X] T033 [US2] [T2] Update `specs/008-targeted-refactor-governance/readiness/native-startup-cleanup.md` with every startup stage, acquired resource, cleanup owner, failure diagnostic, release order, transfer point, and synthetic/real evidence status

**Checkpoint**: US2 native startup cleanup is independently testable.

---

## Phase 5: User Story 3 (US3) - Strengthen Build and Template Governance Checks

**Goal**: Make build organization, generated guidance validation, and template drift validation semantic and actionable.

**Independent Test**: Governance fixtures fail for missing sections, missing prompts, misplaced deferred scope, parity mismatches, unaligned path-class drift, invalid deferrals, and brittle FAKE organization.

### Tests First

- [X] T034 [P] [US3] [T1] Extend `tests/Governance.Tests/GeneratedGuidanceTests.fs` with failing fixtures for missing headings, missing prompts, prompts only in deferred scope, wrong-section prompts, and active/preset parity mismatches
- [X] T035 [P] [US3] [T1] Extend `tests/Governance.Tests/TemplateDriftTests.fs` with failing fixtures for template-owned path classes, required alignment classes, same-diff evidence, active spec/plan/readiness mentions, and accepted deferral schema fields
- [X] T036 [P] [US3] [T1] Extend command-contract tests for physical `build.fsx` split acceptance or named-section fallback, preserving `BuildModel`, `BuildMsg`, `BuildEffect`, pure `update`, emitted effects, interpreter boundaries, and `Dev`/`Verify`/`Ci` target semantics

### Implementation

- [X] T037 [US3] [T1] Replace substring-only `GeneratedGuidanceCheck` logic with structured Markdown section parsing, scoped prompt validation, deferred-scope detection, and active/preset parity diagnostics that name path, section, prompt, and mismatch class
- [X] T038 [US3] [T1] Update `.specify/templates/spec-template.md`, `.specify/presets/fsharp-opinionated/templates/spec-template.md`, `.specify/templates/plan-template.md`, and `.specify/presets/fsharp-opinionated/templates/plan-template.md` only where required by the semantic guidance contract
- [X] T039 [US3] [T1] Refactor `scripts/template-drift.fsx` to classify changed template-owned paths, map path classes to required alignment classes, validate same-diff alignment files plus active feature evidence, and report accepted deferrals with required fields
- [X] T040 [US3] [T1] Attempt physical `build.fsx` organization by concern; accept it only if `Dev`, `Verify`, and `Ci` load cross-platform, otherwise keep one canonical `build.fsx` with path model, effects, interpreter, validation, governance, guidance, and target graph sections
- [X] T041 [US3] [T1] Run `./fake.sh build -t GeneratedGuidanceCheck`, `./fake.sh build -t TemplateDrift`, and Linux plus Windows `Dev`/`Verify`/`Ci` load checks where available; store outputs or unsupported-platform rationale in `generated-guidance.md`, `template-drift.md`, and `build-organization.md`
- [X] T042 [US3] [T1] Update `docs/build.md`, `docs/testing.md`, `docs/evidence.md`, `docs/speckit.md`, README, or workflow docs only where final diagnostics, target semantics, or deferral boundaries changed

**Checkpoint**: US3 governance diagnostics are independently testable.

---

## Phase 6: User Story 4 (US4) - Diagnose Fallbacks and Public Record Invariants

**Goal**: Make recoverable Yoga fallback behavior observable where the existing public surface allows it, and record public record invariant decisions without accidental API changes.

**Independent Test**: Force Yoga execution failure, assert safe fallback bounds plus diagnostics or follow-up deferral, and validate that every public record has an inventory entry with required follow-up IDs.

### Tests First

- [X] T043 [P] [US4] [T1] Add `tests/Layout.Tests/YogaFallbackDiagnosticsTests.fs` forcing recoverable Yoga execution failure and asserting safe fallback bounds plus an observable diagnostic through existing `LayoutDiagnostic` fields when sufficient
- [X] T044 [P] [US4] [T1] Add public record invariant inventory tests in `tests/Governance.Tests/PublicRecordInvariantTests.fs` that enumerate public records from `FS.Skia.UI`, `FS.Skia.UI.Layout`, and `FS.Skia.UI.Charts` and fail on missing inventory rows
- [X] T045 [P] [US4] [T1] Add follow-up proposal validation tests requiring Yoga public-surface blockers and helper-constructor or validation-first recommendations to appear in `specs/008-targeted-refactor-governance/readiness/follow-ups.md` with stable IDs

### Implementation

- [X] T046 [US4] [T1] Evaluate the existing `src/Layout/Types.fsi` diagnostic surface and either implement Yoga fallback diagnostics using existing fields or record a follow-up API proposal without changing public signatures
- [X] T047 [US4] [T1] Update `src/Layout/Layout.fs` fallback handling so recoverable Yoga execution failure keeps deterministic safe bounds and emits `FallbackBoundsApplied` diagnostic data through existing fields when surface-sufficient
- [X] T048 [US4] [T1] Write `specs/008-targeted-refactor-governance/readiness/record-invariants.md` with package, record name, fields, invariant, construction stance, decision, rationale, and follow-up ID where needed for every public record
- [X] T049 [US4] [T1] Write `specs/008-targeted-refactor-governance/readiness/follow-ups.md` for any Yoga diagnostic surface gap or public record helper/validation API recommendation, keeping all public API work out of this feature
- [X] T050 [US4] [T1] Run focused `tests/Layout.Tests`, `tests/Governance.Tests`, and package surface checks for Yoga fallback diagnostics, record inventory completeness, follow-up validation, safe fallback bounds, and no `Library.fsi` change; store outputs in `yoga-fallback-diagnostics.txt`, `record-invariants.md`, and `follow-ups.md`

**Checkpoint**: US4 diagnostics and invariant review are independently testable.

---

## Phase 7: Integration & Polish

- [X] T051 [P] Run `./fake.sh build -t Dev` and store the log under `specs/008-targeted-refactor-governance/readiness/logs/`
- [X] T052 [P] Run `./fake.sh build -t PackageSurfaceCheck` and, only if intentionally refreshing unchanged baselines is required, `./fake.sh build -t RefreshSurfaceBaselines`; store public surface output in `specs/008-targeted-refactor-governance/readiness/public-surface.txt`
- [X] T053 [P] Run focused `dotnet test` commands for `tests/Lib.Tests`, `tests/Layout.Tests`, `tests/Governance.Tests`, and `tests/Package.Tests`; store semantic and diagnostic logs under `specs/008-targeted-refactor-governance/readiness/logs/`
- [X] T054 [P] Run `./fake.sh build -t GeneratedGuidanceCheck` and `./fake.sh build -t TemplateDrift`; confirm readiness reports name missing sections, prompts, path classes, alignment classes, and accepted deferrals
- [X] T055 Run `./fake.sh build -t Verify` and `./fake.sh build -t Ci`; confirm `Ci` delegates to `Verify`, required evidence artifacts exist, and build organization acceptance or fallback evidence is recorded
- [X] T056 [P] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/008-targeted-refactor-governance --graph-only` and confirm no cycles, dangling references, orphaned tasks, or unexpected propagated statuses
- [X] T057 Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/008-targeted-refactor-governance` and confirm PASS, or document every unresolved synthetic-evidence or diff-scan blocker
- [X] T058 Complete the Synthetic-Evidence Inventory, final readiness review, and follow-up proposal cross-links so no synthetic-only evidence or public API recommendation is hidden
- [X] T059 Prepare the merge summary with command results, readiness evidence paths, public surface verdict, native cleanup verdict, governance diagnostic verdict, Yoga/record invariant verdict, and deferred public API work

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| _No `[S]` tasks._ | | | |

### Approved Synthetic Fixture Disclosures

The native cleanup tasks use deterministic symbolic native resources so every
failure stage can be forced without mutating the workstation Vulkan driver.
These are not synthetic-only merge evidence because the same feature retains
real `VulkanHost.run` smoke evidence with `fallback-used=false` in
`readiness/native-smoke.txt`.

| Task | Fixture reason | Paired real evidence | Tracking issue |
|------|----------------|----------------------|----------------|
| T010 | Symbolic startup resource handles make acquisition failure and cleanup order deterministic. | `specs/008-targeted-refactor-governance/readiness/native-smoke.txt` | n/a |
| T014 | Internal startup/ownership fixture contracts simulate failure after each named stage without exposing public API. | `specs/008-targeted-refactor-governance/readiness/native-smoke.txt` | n/a |
| T025 | Injected acquisition failure cases cover every native resource category with symbolic handles. | `specs/008-targeted-refactor-governance/readiness/native-smoke.txt` | n/a |
| T026 | Reverse cleanup, diagnostic, transfer, shutdown, and idempotency assertions use a deterministic synthetic ledger. | `specs/008-targeted-refactor-governance/readiness/native-smoke.txt` | n/a |

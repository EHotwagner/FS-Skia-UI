# Feature Specification: Foundations Baseline & Build-Library Spike

**Feature Branch**: `039-foundations-baseline-spike`
**Created**: 2026-05-31
**Status**: Draft
**Input**: User description: "create specs for the first part of the rewrite" — scoped to **Stage 0 (Foundations, baselines, decisions)** plus the **Stage 3.1 spike** from `docs/reports/2026-05-31-1049-foundations-implementation-plan.md`, the plan's resolved entry point (D5).

## Overview

The foundations programme (companion analysis `docs/reports/2026-05-31-0908-foundations-rewrite-analysis.md`) converges on one keystone — a tested F# governance library extracted from `build.fsx` — and one policy, the two-tier development process. Before any of that extraction work begins, two things must be in place so the rest of the programme is **measurable** and **safe**:

1. A captured, point-in-time **baseline** of the "before" state (script size, governance prose volume, language mix, per-feature ceremony cost) plus **golden evidence fixtures** that will later prove the ported evidence engine produces identical output.
2. A **de-risking spike** that proves the single biggest technical unknown: that a dedicated, compiled build front-end project can reference and drive a compiled governance library in-process — confirming decision **D2** (dedicated FAKE build project) or, if it surfaces a blocker, triggering the documented thin-`build.fsx` fallback.

This feature delivers exactly those two things and the architecture-decision records (ADRs) that the later stages depend on. It changes **no runtime code**, ports **no logic**, and moves **no validators** — those are later stages. Its value is entirely in making the rest of the programme verifiable and unblocking the highest-risk decision early.

## Change Classification

**Tier**: **Tier 1 (contracted change)** — introduces two new build-tooling
projects, a new inter-project contract (the build front-end → governance-library
project reference), and new central package dependencies, so the full artifact
chain applies, *scoped to the new build-tooling projects*. It changes **no
runtime public API**, no existing `.fsi`, and no existing surface baseline:
`PackageSurfaceCheck`/`FsiTranscripts` must show no diff (SC-006).

**Public API impact**: see Framework Governance Prompts §Public contract impact
(none for the runtime surface; one new build-tooling `.fsi`).

**Verification approach**: the per-story Independent Tests and Success Criteria
below, plus the existing serialized validation sequence for no-regression.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - De-risk the dedicated-build-project decision via a spike (Priority: P1)

As the framework maintainer, I need proof that a dedicated, compiled build project can reference and drive a compiled governance library in-process, so that I can commit the rest of the programme to decision D2 (dedicated FAKE build project) rather than discovering a blocker after Stages 3–5 are built on top of it.

**Why this priority**: This is the one technical unknown the plan singles out (D5). Every later stage assumes the build front-end can call the library. Validating it on a minimal slice first is the cheapest possible way to confirm the decision or fall back deliberately.

**Independent test**: Stand up an empty/near-empty governance library project and a dedicated build front-end project that references it, wire a single trivial target whose body lives in the library, and run that target through the normal build entry point. The spike is successful if the target runs to completion driven from the library; it is a deliberate-fallback outcome if a concrete blocker is recorded with evidence and the thin-`build.fsx` alternative is documented as the path forward.

**Acceptance Scenarios**:

1. **Given** a fresh checkout of this branch, **When** the maintainer builds the new governance library and dedicated build front-end projects, **Then** both compile cleanly under the repository's `net10.0` / `TreatWarningsAsErrors` conventions with no new package versions declared outside central package management.
2. **Given** the dedicated build front-end referencing the governance library, **When** a single trivial target whose implementation lives in the library is invoked through the build entry point, **Then** the target executes and reports success driven from the library (no inline duplication of its logic in the front-end).
3. **Given** the spike runs, **When** it completes, **Then** its outcome is recorded as either "D2 confirmed" or "fallback triggered" with a named, reproducible blocker — leaving no ambiguity about which build-front-end form Stage 5 will take.

---

### User Story 2 - Capture a verifiable baseline and golden evidence fixtures (Priority: P1)

As the framework maintainer, I need a point-in-time snapshot of the current state and a frozen set of golden evidence outputs, so that every later stage's reduction claims (lines, languages, prose, ceremony time) are checkable and so that the eventual evidence-engine port has an exact parity oracle.

**Why this priority**: Without a captured baseline, "did we shrink the script?" and "did we regress evidence output?" are unanswerable. The golden fixtures are specifically required as the Stage 4 parity oracle; capturing them now, before anything changes, is the only moment they are authoritative.

**Independent test**: Produce a baseline document containing the required counts and run the existing evidence graph/audit on a defined set of features, archiving their outputs as fixtures. The story is complete when a reviewer can read the baseline numbers and re-run the same evidence commands to reproduce the archived fixtures byte-for-byte.

**Acceptance Scenarios**:

1. **Given** the current tree at a recorded commit, **When** the baseline is captured, **Then** it records `build.fsx` line count with an orchestration-vs-validation breakdown, governance Markdown line counts (including the `.claude`/`.agents` skill mirror and the constitution), the F#/Bash/Python language line mix, and the current per-feature ceremony-time estimate.
2. **Given** the current and two historical features, **When** the existing evidence graph and audit are run on each, **Then** their outputs (task graph data, task graph Markdown, and the audit status/count block) are archived as committed fixtures.
3. **Given** the archived fixtures, **When** a reviewer re-runs the same evidence commands on the same features, **Then** the regenerated outputs match the archived fixtures exactly (these become the Stage 4 parity oracle).
4. **Given** the baseline document, **When** it is committed, **Then** it records the exact git commit it describes so later stages compare against a fixed snapshot, not a moving tree.

---

### User Story 3 - Record the shaping architecture decisions as ADRs (Priority: P2)

As a contributor to later stages, I need the resolved decisions written as durable ADRs, so that I can build Stages 1–7 against stable, referenceable choices instead of re-litigating them or reading the long analysis each time.

**Why this priority**: The decisions are already resolved with the maintainer (D1, D2, D4, D6 plus contract-versioning policy). Recording them is low-effort but prevents drift and ambiguity across the multi-stage programme. It is P2 because the spike (US1) can proceed against the already-known decisions even before the ADRs are formally written.

**Independent test**: A reviewer can open a dedicated ADR location and find one record per shaping decision, each stating the decision, its rationale, and the stages it shapes.

**Acceptance Scenarios**:

1. **Given** the resolved decisions, **When** the ADRs are written, **Then** there is a discrete, dated ADR for each of: governance-library placement & distribution (D1), build front-end form (D2), generated-product contract versioning policy, Spec Kit fork stance (D4), and configuration representation (D6).
2. **Given** each ADR, **When** it is read, **Then** it states the decision, the alternatives considered, the rationale, and which later stages it shapes.

---

### User Story 4 - Establish the meta-process for the programme (Priority: P3)

As the framework maintainer, I want this programme's own work to run under the lighter framework-author process (not full consumer ceremony) and to designate which features intentionally exercise the full pipeline, so that the rewrite does not itself suffer the suffocation it is meant to remove while still keeping the governance harness honest.

**Why this priority**: This is a policy convenience that makes the rest of the programme cheaper to execute. It is P3 because it governs *how* later work proceeds rather than producing a foundational artifact, and the two-tier mechanism itself is specced/implemented in a later feature (Stage 1).

**Independent test**: A reviewer can find a written statement of which validation tier this programme's features run under and which features are designated dogfood features that must run the full pipeline.

**Acceptance Scenarios**:

1. **Given** the programme plan, **When** the meta-process is recorded, **Then** it states that foundations features default to the lightweight framework-author loop except those touching governance or consumer contracts, which escalate.
2. **Given** the programme plan, **When** dogfood features are designated, **Then** the named set that must exercise the full Spec Kit + evidence pipeline is recorded.

---

### Edge Cases

- **Spike surfaces a blocker**: If the dedicated build front-end cannot cleanly reference or drive the library, the feature is still successful provided the blocker is recorded reproducibly and the thin-`build.fsx` fallback is documented as the path Stage 5 will take. A spike that ends ambiguously (neither confirmed nor a recorded blocker) is a failure.
- **Baseline drifts before later stages run**: The baseline must pin the git commit it describes so later comparisons are against a fixed snapshot, not the evolving tree.
- **Evidence output is non-deterministic across runs**: If re-running evidence on the same feature does not reproduce the archived fixture, the fixture is not yet a valid parity oracle; the non-determinism must be identified and the fixture re-captured deterministically before the story is complete.
- **Historical features no longer evaluate cleanly**: If a chosen historical feature cannot produce a stable evidence output, substitute another historical feature and record the substitution rather than committing an unstable fixture.
- **New projects must not perturb existing builds**: Adding the library and build-front-end projects to the solution must not change the behaviour or output of any existing build target.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The feature MUST produce a committed baseline document capturing, at a recorded git commit: the `build.fsx` line count with an orchestration-vs-validation breakdown, governance Markdown line counts (including the `.claude`↔`.agents` skill mirror, the constitution, and governance prose under `templates/` and `specs/**`), the F#/Bash/Python language line mix, and the current per-feature ceremony-time estimate.
- **FR-002**: The feature MUST archive golden evidence fixtures — the evidence graph and audit outputs (task-graph data, task-graph Markdown, and the audit status/count block) — for the current feature and at least two historical features, committed so they are reproducible.
- **FR-003**: The archived golden fixtures MUST be reproducible: re-running the existing evidence commands on the same features regenerates outputs identical to the committed fixtures. These fixtures are designated the Stage 4 parity oracle.
- **FR-004**: The feature MUST record one ADR per shaping decision: governance-library placement & distribution (D1), build front-end form (D2), generated-product contract versioning policy, Spec Kit fork stance (D4), and configuration representation (D6). Each ADR states decision, alternatives, rationale, and the stages it shapes.
- **FR-005**: The feature MUST stand up a compiled governance library project skeleton and a dedicated build front-end project that references it, both building cleanly under the repository's `net10.0` / `TreatWarningsAsErrors` / central-package-management conventions.
- **FR-006**: The spike MUST demonstrate the dedicated build front-end driving at least one trivial target whose implementation lives in the governance library, invoked through the normal build entry point, with no inline duplication of that logic in the front-end.
- **FR-007**: The spike outcome MUST be recorded unambiguously as either "D2 confirmed (dedicated build project viable)" or "fallback triggered" with a named, reproducible blocker and the thin-`build.fsx` fallback documented as the Stage 5 path.
- **FR-008**: The feature MUST record the programme meta-process: that foundations features default to the lightweight framework-author loop (with governance/consumer-contract-touching features escalating) and the named set of dogfood features that must run the full pipeline.
- **FR-009**: The feature MUST NOT modify any runtime code (no edits under `src/Scene`, `src/SkiaViewer`, `src/Elmish`, `src/KeyboardInput`, `src/Layout`, `src/Controls`, `src/Controls.Elmish`, `src/Lib`) and MUST NOT change the public `.fsi` surface or any surface baseline.
- **FR-010**: The feature MUST NOT change the behaviour or output of any existing build target; adding the new projects to the solution is additive only. The existing serialized validation sequence MUST remain green.
- **FR-011**: The feature MUST NOT port the Python evidence engine, move validators out of `build.fsx`, extract the MEL engine, or implement the two-tier `Route` selection — those are explicitly later stages and are out of scope here.
- **FR-012**: New projects MUST NOT introduce any package version outside the repository's central package management, and MUST NOT add a runtime-script-compilation (FSharp Compiler Services) dependency.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Introduces two new **project skeletons** — a governance library (working name `FS.Skia.UI.Build`, per ADR D1) and a dedicated build front-end project — added to the solution. No existing package identity, contents, or version changes in this feature; the library is *created*, not yet populated or published, and generated package consumers are unaffected. (Distribution/packaging of the library is decided in the D1 ADR but exercised in later stages.)
- **Public contract impact**: None. No `.fsi` signatures, documented public APIs, sample contracts, or surface baselines change. `PackageSurfaceCheck`/`FsiTranscripts` must show no baseline diff.
- **State workflow impact**: None. No stateful workflow, I/O, command, effect, subscription, or interpreter behaviour changes. The MEL/interpreter extraction is a later stage and is explicitly out of scope.
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering, screenshots, Vulkan, Skia, or visual-output changes.
- **Evidence obligations**: Golden evidence fixtures (task-graph data + Markdown + audit status/count block) for the current feature and two historical features, captured via the existing `EvidenceGraph`/`EvidenceAudit` path and proven reproducible; the baseline document pinned to a git commit; the spike-outcome record.
- **Unsupported scope**: No runtime changes; no two-tier `Route` process (Stage 1); no single-source generation of duplicated artifacts (Stage 2); no validator moves or YAML-parser retirement (Stage 3.2+); no Python/Bash port (Stage 4); no MEL extraction or `build.fsx` retirement (Stage 5); no prose trimming or contract versioning enforcement (Stage 6). No visual, release, platform, or distribution changes.
- **Build-target impact**: The spike may add new projects and a single trivial demonstration target driven from the library, but MUST NOT alter the behaviour or output of `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `DependencyReport`, `TemplateDrift`, `EvidenceGraph`, or `EvidenceAudit`. All of these must remain green in their existing serialized order; this set (plus `PackageSurfaceCheck`/`FsiTranscripts`) is the representative no-regression gate. The aggregate `Verify`/`Ci` and the `PackLocal` pack target are supersets/derivatives of these gates and are not separately exercised by this additive feature.

## Success Criteria *(mandatory)*

- **SC-001**: A reviewer can read the committed baseline document and find every required metric (script size with breakdown, governance prose volume, language mix, ceremony-time estimate) plus the exact git commit it describes.
- **SC-002**: Re-running the existing evidence commands on the same features regenerates outputs identical to the committed golden fixtures, for the current feature and both historical features (100% byte-for-byte match).
- **SC-003**: Both new projects (governance library skeleton and dedicated build front-end) build cleanly with zero warnings under the repository's standard conventions, and the front-end successfully drives at least one target whose logic lives in the library.
- **SC-004**: The spike outcome is recorded as exactly one of "D2 confirmed" or "fallback triggered with named blocker," leaving no ambiguity about the Stage 5 build-front-end form.
- **SC-005**: One ADR exists for each of the five shaping decisions, each stating decision, alternatives, rationale, and affected stages.
- **SC-006**: The full existing serialized validation sequence remains green and the public surface shows no baseline diff, demonstrating the foundations work changed nothing in the runtime or public contract.
- **SC-007**: The programme meta-process and the named dogfood-feature set are recorded and discoverable in one place.

## Assumptions

- The "first part of the rewrite" is the implementation plan's resolved entry point (D5): **Stage 0 + the Stage 3.1 spike**, run together. The two-tier process (Stage 1), single-source generation (Stage 2), and the bulk library extraction/port (Stages 3.2–6) are separate, later features.
- The shaping decisions D1, D2, D4, and D6 are already resolved with the maintainer (per the implementation plan's "Decisions" section); this feature *records* them as ADRs rather than re-deciding them.
- The governance library working name is `FS.Skia.UI.Build` (ADR D1); the exact project location in the tree is an implementation detail to be settled during planning.
- "Two historical features" for golden fixtures means two already-completed numbered features whose evidence output is stable; the specific features are chosen during planning and recorded in the baseline.
- The evidence graph/audit produce deterministic output for a fixed feature at a fixed commit; if not, the non-determinism is treated as an edge case (re-capture deterministically) rather than accepted into a fixture.
- This feature itself runs under the lightweight framework-author process per the meta-process it records, except where it touches the new build/library projects and existing build wiring, for which the standard build/test/surface gates apply.

## Dependencies

- The companion analysis (`docs/reports/2026-05-31-0908-foundations-rewrite-analysis.md`) and implementation plan (`docs/reports/2026-05-31-1049-foundations-implementation-plan.md`) — source of scope and decisions.
- The existing `EvidenceGraph`/`EvidenceAudit` path (currently Bash + Python) must be runnable to capture the golden fixtures; this feature consumes it unchanged.
- Repository conventions: `Directory.Build.props` (`net10.0`, `TreatWarningsAsErrors`), `Directory.Packages.props` (central package management), and the FAKE serialized-validation ordering.

## Out of Scope

- The two-tier development process, `developer_class` axis, `Route` target, and tier enforcement (Stage 1).
- Single-source generation of the `.claude`↔`.agents` mirror, constitution echoes, and skillist (Stage 2).
- Moving any validator logic out of `build.fsx`, retiring the hand-rolled YAML parser, or representing config as compiled F# values (Stages 3.2–3.3).
- Porting `compute-task-graph.py`, `audit-status-scan.py`, or `run-audit.sh` to F# (Stage 4).
- Extracting the MEL engine, migrating the remaining heavy validators, or retiring `build.fsx` (Stage 5).
- Codifying remaining prose rules, trimming governance Markdown, or enforcing contract versioning (Stage 6).
- Any change to the framework runtime architecture, the public `.fsi` surface, or generated-consumer behaviour.

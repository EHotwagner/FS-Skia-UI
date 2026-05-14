# Feature Specification: Template Framework Governance

**Feature Branch**: `006-template-framework-governance`  
**Created**: 2026-05-14  
**Status**: Draft  
**Input**: User description: "create specs for docs/template-framework-analysis.md"

## Clarifications

### Session 2026-05-14

- Q: What is the v1 delivery scope for Template Framework Governance? → A: Phase 1 only: canonical verification workflow, current evidence wiring, stable baselines, and supporting docs.
- Q: Should v1 update repository automation to use the canonical workflow? → A: Yes; v1 updates repository automation so it invokes the canonical workflow entries.
- Q: Which evidence artifact classes are required in v1? → A: Existing public contract transcripts, package surface baselines, sample smoke output, task graph output, evidence audit output, and build/test/package logs.
- Q: Which generated feature guidance should v1 update? → A: Generated task guidance only, so generated tasks call canonical workflow entries.
- Q: What package validation is in v1 scope? → A: Local package production and package surface review only; package consumer smoke is deferred.

## Change Classification

**Tier**: Tier 1 contracted governance and command-surface change

**Public API Impact**: No runtime F# public API changes are expected. The contracted surface is the repository workflow command set, evidence artifact locations, stable package surface baseline location, repository automation entry points, and generated task guidance.

**Verification Approach**: Planning and tasks must prove the canonical workflow can restore, build, test, pack locally, refresh or check package surface baselines, produce required v1 evidence artifacts, validate task graphs, run the evidence audit, and update touched automation or task guidance to invoke canonical workflow entries.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Verify the Project Through One Governed Workflow (Priority: P1)

As a project maintainer, I need one documented verification workflow for the repository so local work, automated checks, and agent-driven work prove the same build, test, packaging, and v1 evidence expectations.

**Why this priority**: The analysis identifies the missing canonical workflow as the first blocker. Without it, later template, evidence, dependency, and release work would still rely on duplicated or inferred command order.

**Independent Test**: Can be tested on a clean checkout by running the documented fast and full verification workflows and confirming they report build/test/package logs, public contract transcripts, package surface baselines, sample smoke output, task graph output, and evidence audit output without requiring hidden manual steps.

**Acceptance Scenarios**:

1. **Given** a clean checkout, **When** a maintainer runs the documented fast verification workflow, **Then** the repository restores, builds, runs the default test set, and reports a clear pass or fail result.
2. **Given** a feature changes public behavior or v1 evidence obligations, **When** a maintainer runs the full verification workflow, **Then** required evidence artifacts are produced or the workflow fails with the missing evidence identified.
3. **Given** repository automation exists, **When** its command order is reviewed, **Then** it delegates to the same canonical workflows used locally rather than maintaining a separate sequence.

---

### User Story 2 - Document the Template Roadmap Boundary (Priority: P2)

As a template steward, I need the v1 documentation to identify which template framework capabilities are delivered now and which capabilities are deferred so contributors do not mistake roadmap items for current requirements.

**Why this priority**: The analysis includes a multi-phase roadmap. V1 must make the first phase executable while keeping later template packaging, dependency governance, generated artifact hardening, layout evidence, visual evidence, package consumer smoke, and release validation visible but out of scope.

**Independent Test**: Can be tested by reviewing v1 docs and confirming they name the delivered workflow, stable baseline, required v1 evidence set, and supporting docs work separately from deferred roadmap items.

**Acceptance Scenarios**:

1. **Given** a contributor reads the v1 build and evidence docs, **When** they inspect scope, **Then** they can distinguish delivered v1 verification behavior from later template packaging, package consumer smoke, and generated product work.
2. **Given** a roadmap item is deferred, **When** it is mentioned in v1 docs, **Then** it is clearly labeled as future work and does not appear as a required v1 verification target.
3. **Given** historical feature directories exist in the source repository, **When** v1 docs describe future template packaging, **Then** they state that historical feature directories are repository history and not product template history.

---

### User Story 3 - Stabilize Current Evidence and Baselines (Priority: P3)

As a maintainer, I need v1 evidence and package surface baselines to have stable, documented locations so future features do not patch historical readiness folders or rely on hidden evidence paths.

**Why this priority**: Stable evidence locations are part of the Phase 1 scope and are required before later gates can reliably validate package surfaces or evidence drift.

**Independent Test**: Can be tested by running the full verification workflow and confirming build/test/package logs, public contract transcripts, package surface baselines, sample smoke output, task graph output, and evidence audit output are written to stable documented paths.

**Acceptance Scenarios**:

1. **Given** package surface checks run, **When** they compare expected output, **Then** they use the stable current baseline location rather than requiring edits to older feature readiness folders.
2. **Given** v1 evidence scripts or workflows run, **When** they produce transcripts, logs, sample smoke output, task graph output, or audit output, **Then** the output paths are documented and reproducible.
3. **Given** historical readiness evidence remains in the repository, **When** maintainers inspect current verification behavior, **Then** they can distinguish historical evidence from current baselines.

---

### User Story 4 - Keep Automation Aligned With the Canonical Workflow (Priority: P4)

As a contributor or automation maintainer, I need repository automation and feature guidance to call the canonical workflow so command order is not reimplemented in separate scripts, docs, or generated tasks.

**Why this priority**: V1 succeeds only if the canonical workflow becomes the source of truth for humans, agents, automation, and generated task guidance. Duplicated command order is the current gap this phase is meant to close.

**Independent Test**: Can be tested by inspecting automation, docs, and generated task guidance and confirming they reference the canonical workflow rather than duplicating its sequence. Generated specification and plan template hardening remain out of scope for v1.

**Acceptance Scenarios**:

1. **Given** repository automation verifies the project, **When** its configuration is reviewed, **Then** it invokes canonical workflow entries instead of duplicating restore, build, test, package, and evidence ordering.
2. **Given** generated task guidance tells contributors how to verify work, **When** it references checks, **Then** it names the canonical v1 workflow entries.
3. **Given** a future roadmap phase adds a new verification class, **When** v1 docs discuss it, **Then** the docs identify it as a later extension to the canonical workflow.

### Edge Cases

- Clean checkouts without existing generated evidence must create required output directories or fail with clear setup guidance.
- Machines that cannot run visual or graphics-dependent checks must be told that visual evidence is deferred beyond v1, rather than silently treating visual checks as v1 verification.
- Existing historical feature directories must remain available in this repository while being excluded from generated product templates.
- Samples that are intentionally manual must remain visible in the sample inventory with a reason and owner.
- Future template instantiation and package consumer smoke work must be documented as deferred and must not appear in v1 pass/fail criteria.
- A new v1 workflow, script, evidence artifact, sample smoke path, package surface baseline, public contract transcript, or build/test/package log must be documented in the canonical workflow or explicitly deferred.
- Feature-specific readiness evidence must not become the long-term source of truth for current package surface baselines.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The v1 project MUST provide a canonical command surface for fast development verification, full verification, packaging, package surface checks, public contract transcripts, sample smoke output, task graph validation, evidence audit, stable baseline refresh, and supporting docs.
- **FR-002**: The fast verification workflow MUST restore the project, build it, run the default non-visual tests, and report a clear pass or fail result from a single documented command.
- **FR-003**: The v1 full verification workflow MUST include all fast verification checks plus public contract transcript checks, package surface baseline checks, existing sample smoke output, task graph validation, evidence audit, and build/test/package log capture.
- **FR-004**: Repository automation and documentation MUST invoke or reference the canonical command surface instead of duplicating command ordering.
- **FR-005**: Verification workflows MUST produce reproducible outputs for build/test/package logs, public contract transcript status, package surface baseline status, sample smoke status, task graph output, and evidence audit verdicts.
- **FR-006**: The v1 documentation MUST identify template creation, dependency governance, generated artifact hardening, deterministic scaffolding, layout evidence, visual evidence, package consumer smoke validation, and release validation as roadmap items outside the v1 delivery scope.
- **FR-007**: Package surface governance MUST maintain a stable current baseline location and prevent ongoing checks from requiring edits to previous feature readiness folders.
- **FR-008**: Generated task guidance updated by v1 MUST direct contributors to canonical workflow entries for verification instead of duplicating command order; broader generated specification and plan template hardening is deferred.
- **FR-009**: Quality gates MUST detect missing public contract declarations, stale package baselines, missing public contract transcripts, broken task graphs, unsupported synthetic evidence, missing sample smoke output, missing build/test/package logs, undocumented workflow changes, and template drift in v1-owned workflows.
- **FR-010**: Existing workflow documentation MUST cover build workflow, testing workflow, v1 evidence policy, stable baselines, and how later roadmap items will extend the v1 command surface.
- **FR-011**: V1 package validation MUST support local package production and package surface review for the current repository only; package consumer smoke and generated product template validation are deferred to later roadmap phases.
- **FR-012**: Automation and generated task guidance touched by v1 MUST invoke or reference canonical workflow entries instead of duplicating command order.

### Key Entities

- **Command Target**: A named v1 workflow entry point with declared purpose, inputs, outputs, and pass/fail behavior.
- **V1 Evidence Artifact**: A reproducible file or report for build/test/package logs, public contract transcript status, package surface baseline status, sample smoke output, task graph output, or evidence audit verdicts.
- **Stable Package Surface Baseline**: The current package surface reference used by checks without requiring edits to historical feature readiness folders.
- **Repository Automation Entry**: A maintained automation workflow that invokes canonical command targets rather than duplicating command order.
- **Generated Task Guidance**: Project task-generation guidance updated in v1 to direct contributors to canonical command targets for verification.
- **Deferred Roadmap Item**: A template framework capability named in v1 docs but excluded from v1 pass/fail criteria until a later phase.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A clean checkout completes the fast development verification workflow in 10 minutes or less on a supported development machine.
- **SC-002**: The v1 full verification workflow produces build/test/package logs, public contract transcript status, package surface baseline status, sample smoke output, task graph output, and evidence audit verdicts with zero missing required artifact classes.
- **SC-003**: 100% of package surface checks compare against a stable current baseline location without requiring edits to previous feature readiness folders.
- **SC-004**: 100% of v1 evidence workflows have documented output paths and pass/fail behavior.
- **SC-005**: 100% of repository automation and generated task guidance touched by v1 invokes or references canonical workflow entries instead of duplicating command order.
- **SC-006**: V1 documentation names all deferred roadmap categories, including package consumer smoke, and excludes them from v1 pass/fail criteria.
- **SC-007**: The full verification workflow fails with actionable output when a required v1 artifact class is missing.

## Assumptions

- `docs/template-framework-analysis.md` is the source intent for this feature, and its implementation sequencing will be refined during planning.
- The v1 implementation slice is limited to canonical verification workflow, existing evidence wiring, stable baselines, and supporting docs.
- Existing feature directories remain part of this repository's history but are not distributed as product template history.
- Visual or graphics-dependent verification is a roadmap item outside v1 unless existing non-visual smoke behavior already supports it.
- Exact tool and file choices are deferred to planning; this specification describes observable governance outcomes and stakeholder value.

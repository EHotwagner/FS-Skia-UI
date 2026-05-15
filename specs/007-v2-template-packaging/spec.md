# Feature Specification: Template Packaging and Drift Governance

**Feature Branch**: `007-v2-template-packaging`  
**Created**: 2026-05-14  
**Status**: Draft  
**Input**: User description: "create specs for v2"

## Change Classification

**Tier**: Tier 1 template contract and governance change

**Public API Impact**: No runtime library public API changes are expected. The contracted surface is the project template profile, generated project shape, dependency governance policy, generated specification/planning guidance, template validation workflow, and template drift reporting.

**Verification Approach**: V2 must prove that this repository can produce a clean generated project from its maintained template profile through both source-directory installation and a locally packaged template artifact, validate that generated project through the canonical fast workflow, govern dependency versions from one reviewable policy, strengthen generated specification and plan artifacts, and fail clearly when template-owned files drift without the corresponding template, documentation, or deferral updates.

## Clarifications

### Session 2026-05-14

- Q: Which generated project profiles must template validation exercise? → A: Default profile plus one minimal starter profile.
- Q: Which template artifact boundary must validation exercise? → A: Both source-directory installation and a locally packaged template artifact.
- Q: What must the minimal starter profile include? → A: Core library, one basic sample, core tests, package checks, docs, and Spec Kit governance assets.
- Q: What minimum fields must a drift deferral record include? → A: Id, changed paths, rationale, owner, and target phase.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Create a Clean Project From the Template (Priority: P1)

As a template steward, I need this repository to produce a clean project from its maintained template profile so new products can start from the governed framework without copying historical repository state by hand.

**Why this priority**: V1 made the repository workflow canonical. V2 must prove that workflow can be inherited by a generated project; otherwise the framework remains documentation plus source code rather than a usable template.

**Independent Test**: Can be tested by running the documented template validation workflow and confirming it creates fresh generated projects from both source-directory installation and a locally packaged template artifact for the default profile and one minimal starter profile, contains only template-owned starter history, has no unreplaced placeholders, excludes historical feature evidence, and completes the generated projects' fast verification workflow.

**Acceptance Scenarios**:

1. **Given** a clean source checkout with the v2 template profile, **When** a maintainer runs the template validation workflow, **Then** fresh default-profile and minimal-starter generated projects are created in isolated locations and each reports a clear pass or fail result.
2. **Given** a local template artifact has been packaged, **When** template validation runs, **Then** the same default-profile and minimal-starter checks pass through the packaged artifact path as well as the source-directory path.
3. **Given** the generated project is inspected, **When** its files are compared to template ownership rules, **Then** historical feature directories and source-repository-only readiness evidence are absent while the minimal starter profile and governance files are present.
4. **Given** a template placeholder is missing, stale, or left unreplaced, **When** template validation runs, **Then** the workflow fails and names the affected file and placeholder.
5. **Given** a generated project is created with default options, **When** the generated project's fast verification workflow runs, **Then** it restores, builds, runs default non-visual checks, and reports a clear verdict without manual edits.

---

### User Story 2 - Govern Dependency Versions Centrally (Priority: P2)

As a repository maintainer, I need direct dependency versions and ownership rules to be governed from one reviewable policy so the template and generated projects do not drift through scattered package declarations.

**Why this priority**: Template consumers inherit dependency decisions. Without a central policy, generated projects can start with inconsistent versions, undocumented preview risk, or unclear ownership.

**Independent Test**: Can be tested by inspecting package declarations and dependency documentation, then intentionally introducing an unmanaged version declaration and confirming dependency governance fails with an actionable diagnostic.

**Acceptance Scenarios**:

1. **Given** the source repository contains package references, **When** dependency governance checks run, **Then** every direct dependency version is governed from the central policy except explicitly documented validation-only exceptions.
2. **Given** a dependency is listed in the central policy, **When** maintainers inspect the dependency documentation, **Then** they can see its purpose, owner, license posture, upgrade expectation, and preview-risk status where relevant.
3. **Given** a project file reintroduces an unmanaged inline dependency version, **When** verification runs, **Then** it fails and identifies the project and dependency that must be moved back under central governance.
4. **Given** the template is generated, **When** dependency policy files are inspected in the generated project, **Then** the same central governance expectations are present and usable by the new product.

---

### User Story 3 - Harden Generated Feature Guidance (Priority: P3)

As a contributor starting a new product feature, I need generated specifications and plans to ask the right framework-governance questions so package impact, public contract impact, state workflow, layout, rendering boundaries, evidence, and build-target impact are considered before implementation.

**Why this priority**: V1 aligned generated task guidance, but future products also need the earlier specification and planning artifacts to carry the framework's obligations. If those prompts are weak, later tasks inherit missing scope and evidence.

**Independent Test**: Can be tested by generating a new feature specification and plan from the project preset, then confirming the artifacts include the required governance prompts and do not require manual copying from old feature directories.

**Acceptance Scenarios**:

1. **Given** a contributor generates a new feature specification, **When** they review mandatory sections, **Then** they see prompts for package impact, public contract impact, state workflow impact, layout/rendering impact, evidence obligations, unsupported scope, and build-target impact.
2. **Given** a contributor generates an implementation plan, **When** they review the governance checks, **Then** the plan requires decisions for template ownership, dependency impact, command-surface impact, and evidence paths before tasks are created.
3. **Given** generated artifacts mention deferred roadmap items, **When** the contributor reads them, **Then** the docs distinguish current v2 obligations from later visual, release, and external distribution work.

---

### User Story 4 - Detect Template Drift (Priority: P4)

As a template maintainer, I need verification to detect when source files, docs, presets, dependency policy, samples, or command targets change without corresponding template updates so generated projects remain trustworthy.

**Why this priority**: Once this repository becomes the source for generated projects, untracked source changes can silently create stale templates. Drift detection keeps the source framework and generated starter aligned.

**Independent Test**: Can be tested by changing a template-owned file without updating the template profile or deferral record and confirming drift verification fails with the missing alignment work named.

**Acceptance Scenarios**:

1. **Given** a template-owned file changes, **When** drift verification runs, **Then** it identifies whether the template profile, generated docs, dependency policy, or explicit deferral record must be updated.
2. **Given** a change is intentionally source-only, **When** maintainers record a deferral with id, changed paths, rationale, owner, and target phase, **Then** drift verification accepts the deferral and includes it in readiness evidence.
3. **Given** generated project output changes after a template update, **When** template validation runs, **Then** it records the new generated project evidence and fails if required artifact classes are missing.

### Edge Cases

- Existing historical feature directories must remain in this repository but must not be included in generated starter projects except for a minimal template profile example.
- Generated projects must not contain unreplaced project-name, package-name, author, repository, or namespace placeholders.
- Optional template choices must not leave broken references, orphaned tests, or documentation for disabled features.
- Template validation must create or clean its temporary output safely without deleting source files, historical evidence, or local packages.
- Machines without graphics or visual-test support must receive an explicit message that visual evidence remains outside v2 pass/fail criteria.
- Dependency governance must distinguish reusable library dependency policy from generated application or sample validation policy.
- Existing v1 commands must continue to work while v2 adds template-specific validation.
- If external network access is unavailable, validation must fail with a dependency or restore diagnostic rather than silently treating the template as verified.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: V2 MUST define a template profile that identifies template-owned files, product-owned files, generated starter history, excluded source-repository history, and supported generation options.
- **FR-002**: V2 MUST provide a documented template validation workflow that creates fresh default-profile and minimal-starter generated projects from both source-directory installation and a locally packaged template artifact in isolated locations, then reports a clear pass or fail result for each.
- **FR-003**: V2 MUST verify that default-profile and minimal-starter generated projects exclude historical feature directories and source-repository-only readiness evidence while retaining required governance files, starter docs, samples, tests, and command wrappers appropriate to each profile.
- **FR-004**: V2 MUST verify that generated projects contain no unreplaced placeholders or template-only marker text.
- **FR-005**: V2 MUST verify that the generated project's fast verification workflow succeeds without manual edits after generation.
- **FR-006**: V2 MUST centralize direct dependency version governance in one reviewable policy and prevent unmanaged inline dependency versions except for explicitly documented validation-only exceptions.
- **FR-007**: V2 MUST document each direct dependency's purpose, owner, license posture, upgrade expectation, and preview-risk status where relevant.
- **FR-008**: V2 MUST update generated specification guidance so future feature specs ask about package impact, public contract impact, state workflow impact, layout/rendering impact, evidence obligations, unsupported scope, and build-target impact.
- **FR-009**: V2 MUST update generated planning guidance so implementation plans require decisions about template ownership, dependency impact, command-surface impact, generated project impact, and evidence paths before task generation.
- **FR-010**: V2 MUST provide drift verification that detects template-owned source, docs, preset, dependency, sample, and command-surface changes that are not reflected in the template profile or an explicit deferral record.
- **FR-011**: V2 MUST record template validation output, dependency governance output, generated artifact guidance checks, and drift verification output under feature readiness evidence.
- **FR-012**: V2 MUST keep existing v1 fast, full, and automation verification workflows available and must document how template validation extends them rather than replacing them.
- **FR-013**: V2 MUST document deferred roadmap boundaries, including full visual evidence, release validation, external repository split, and broader distribution automation unless explicitly added by a later feature.
- **FR-014**: The minimal starter profile MUST include the core library, one basic sample, core tests, package checks, docs, and Spec Kit governance assets, while excluding optional layout, charts, parity, and visual sample scope.
- **FR-015**: Each accepted drift deferral record MUST include an id, changed paths, rationale, owner, and target phase before drift verification may accept it.

### Key Entities

- **Template Profile**: The maintained definition of files, options, ownership rules, exclusions, and starter evidence included when creating a new project. It includes a default profile and a minimal starter profile.
- **Generated Project**: A fresh project created from the template profile and validated as a product starting point.
- **Template Validation Run**: A reproducible run that creates default-profile and minimal-starter generated projects from both source-directory installation and a locally packaged template artifact, inspects them for placeholders and excluded history, runs their fast verification workflows, and records the result.
- **Dependency Governance Policy**: The central list of dependency versions and review metadata, including owner, purpose, license posture, upgrade expectation, and preview-risk status.
- **Generated Artifact Guidance**: The specification and planning prompts inherited by new features so framework obligations are considered before implementation.
- **Drift Report**: A validation artifact identifying template-owned changes that require template, documentation, policy, or deferral updates.
- **Deferral Record**: A documented id, changed paths, rationale, owner, and target phase for a source-only or future-roadmap change that is intentionally not reflected in the generated template yet.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Fresh default-profile and minimal-starter generated projects can be created and complete their fast verification workflows in 15 minutes or less each on the supported CI/developer baseline documented by the plan, with per-project elapsed time recorded in readiness evidence.
- **SC-002**: 100% of default-profile and minimal-starter validation runs report zero unreplaced placeholders and zero excluded historical feature directories.
- **SC-003**: 100% of direct dependency versions are governed by the central policy or listed as documented validation-only exceptions.
- **SC-004**: 100% of governed dependencies have purpose, owner, license posture, upgrade expectation, and preview-risk status documented where applicable.
- **SC-005**: Generated specification and planning artifacts include all required framework-governance prompts in 100% of validation checks.
- **SC-006**: Drift verification fails with actionable diagnostics when a template-owned source, docs, preset, dependency, sample, or command-surface change lacks a matching template update or deferral.
- **SC-007**: Existing v1 fast, full, and automation verification workflows continue to pass after v2 template validation is added.
- **SC-008**: Template validation evidence includes source-directory validation, local packaged-artifact validation, generated project creation, placeholder scan, excluded-history scan, generated fast verification result, dependency governance result, guidance checks, and drift report with zero missing required artifact classes.
- **SC-009**: 100% of drift deferrals accepted by verification include id, changed paths, rationale, owner, and target phase in readiness evidence.

## Assumptions

- V2 upgrades this repository as the source of truth before any separate template repository is created.
- V2 covers template packaging/instantiation, central dependency governance, generated specification and planning guidance, and template drift detection.
- Full visual evidence, release validation, external repository split, and broader distribution automation remain deferred unless a later spec brings them into scope.
- Generated projects inherit the canonical v1 workflow shape and should be able to run a fast verification path immediately after creation.
- Historical feature directories remain part of this repository's source history and readiness evidence but are not distributed as generated product history.
- The minimal starter profile is intended as the smallest governed product starting point, not as a demonstration of every optional framework package.

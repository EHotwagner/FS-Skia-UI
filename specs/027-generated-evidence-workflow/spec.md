# Feature Specification: Generated Evidence Workflow Authority

**Feature Branch**: `027-generated-evidence-workflow`  
**Created**: 2026-05-28  
**Status**: Draft  
**Input**: User description: `Mailbox/2026-05-28T15-16-53+0200-lunar-lander-fs-skia-ui-feedback.md`

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Trust Generated Evidence Commands (Priority: P1)

As a generated app maintainer, I want the generated evidence graph and audit commands to run the same authoritative validation that feature governance expects, so that a completed generated command cannot hide a broken graph, missing readiness evidence, or invalid audit result.

**Independent Test**: Create a generated app with an intentionally incomplete evidence graph or readiness set. Running the generated graph or audit command fails with the same verdict as the authoritative project validation and does not write a success-only completion report.

### User Story 2 - Produce Required Skill-Loading Evidence (Priority: P1)

As an implementer following task-level skill requirements, I want generated assistance for creating one evidence row per task and skill pairing, so that audit compliance does not depend on hand-writing precise bookkeeping rows after the work is done.

**Independent Test**: Given task dependency declarations that assign multiple skills to multiple tasks, the generated workflow produces a complete skill-loading evidence table with one row for every required task and skill pairing and rejects collapsed range rows.

### User Story 3 - Diagnose Audit Readiness Failures (Priority: P2)

As a maintainer responding to an audit failure, I want diagnostics that name missing readiness terms and missing readiness files, so that I can fix the specific contract gap without reverse-engineering the audit rules.

**Independent Test**: Run the audit with readiness files that omit required terms. The failure output identifies each affected readiness file and the missing terms or sections needed for that file.

### User Story 4 - Follow Generated Framework Guidance Safely (Priority: P2)

As a generated game author, I want examples and guidance for common FS.Skia.UI app friction, so that app message names, scene point conversion, semantic rendering proof, and screenshot fallback claims remain clear and hard to misuse.

**Independent Test**: Review generated game guidance and examples for app message qualification, domain vector to scene point conversion, semantic scene evidence expectations, and strict screenshot proof wording. Each topic appears in a place a generated app author will encounter before implementing evidence.

### Edge Cases

- A generated graph command must fail when the underlying evidence graph has cycles, dangling references, missing task evidence, or unsupported collapsed skill-loading rows.
- A generated audit command must distinguish missing readiness files from readiness files that exist but omit required terms.
- Skill-loading evidence generation must handle tasks with no declared skills, tasks with multiple declared skills, and repeated skill declarations without creating duplicate required rows.
- Screenshot fallback wording must never imply live desktop visibility when the report only proves deterministic scene rendering or pixel readback.
- Guidance for app message qualification must cover names that collide with viewer lifecycle concepts without requiring all app messages to be qualified.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Generated projects MUST expose evidence graph validation that is authoritative for generated app governance and fails whenever the governed graph is invalid.
- **FR-002**: Generated projects MUST expose evidence audit validation that is authoritative for generated app governance and fails whenever required readiness evidence, real-evidence obligations, or synthetic-evidence restrictions are not satisfied.
- **FR-003**: Generated evidence graph and audit commands MUST report failure status, affected feature or generated app identity, and the specific validation area that failed before any completion claim is recorded.
- **FR-004**: Generated workflows MUST provide a way to produce skill-loading evidence with one row for each required task and skill pairing declared by the task plan.
- **FR-005**: Skill-loading evidence validation MUST reject human-readable batch rows or task ranges when a per-task and per-skill row is required.
- **FR-006**: Skill-loading evidence records MUST make it clear that skill loading occurred before work began for the associated task and MUST flag equal or later timestamps as non-compliant.
- **FR-007**: Audit readiness diagnostics MUST identify missing readiness files by name and missing required terms or sections for each incomplete readiness file.
- **FR-008**: Task generation outputs MUST make every audit-enforced readiness file discoverable before implementation starts, either as explicit work items or as generated readiness placeholders.
- **FR-009**: Generated app guidance MUST include a clear example for qualifying app message cases that may collide with viewer lifecycle names.
- **FR-010**: Generated game guidance MUST include a clear pattern for converting app-owned domain vector values into scene point values.
- **FR-011**: Generated evidence guidance MUST explain that deterministic scene evidence proves rendering metadata and stable scene facts, not live screenshot proof or semantic object presence by itself.
- **FR-012**: Generated evidence guidance MUST use strict screenshot vocabulary that separates live screenshot proof, pixel-readback fallback, deterministic scene evidence, fallback reason, fallback kind, and `proves-screenshot=false` claims.
- **FR-013**: Generated game evidence guidance SHOULD describe a stable way for apps to report semantic scene facts such as lander, terrain, landing pad, or HUD metrics without relying on fragile source checks or visual guesses.
- **FR-014**: Normal generated interactive launch behavior MUST remain separate from explicit evidence commands, so everyday app execution does not unexpectedly run audits, close windows, or write evidence artifacts.
- **FR-015**: Generated validation outputs MUST avoid success-only completion logs when no authoritative validation was performed.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity or package version change is required by the specification. Package contents may change where generated project templates, generated guidance, or evidence command assets are packaged for generated consumers.
- **Public contract impact**: No `.fsi` public API change is required by default. Generated sample contracts, template guidance, and evidence command behavior are in scope; surface baselines change only if planning discovers an unavoidable public contract addition.
- **State workflow impact**: Generated evidence commands and evidence bookkeeping workflows change. Normal interactive app state, gameplay reducers, and persistent viewer launch behavior remain out of scope.
- **Layout/rendering impact**: Core layout and rendering behavior do not change. Generated guidance for scene points, semantic scene evidence, screenshots, pixel-readback fallback, and unsupported environment diagnostics is in scope.
- **Evidence obligations**: Required real evidence paths are `specs/027-generated-evidence-workflow/readiness/generated-validation-authority.md`, `specs/027-generated-evidence-workflow/readiness/skill-loading-evidence-workflow.md`, `specs/027-generated-evidence-workflow/readiness/audit-diagnostics.md`, `specs/027-generated-evidence-workflow/readiness/readiness-contract-discovery.md`, `specs/027-generated-evidence-workflow/readiness/framework-guidance.md`, `specs/027-generated-evidence-workflow/readiness/evidence-vocabulary.md`, `specs/027-generated-evidence-workflow/readiness/evidence-graph.md`, and `specs/027-generated-evidence-workflow/readiness/evidence-audit.md`.
- **Unsupported scope**: New game mechanics, renderer redesign, package publishing, new desktop platform support, browser or mobile screenshot capture, and replacement of the screenshot capture contract are out of scope.
- **Build-target impact**: `GeneratedGuidanceCheck`, `TemplateCheck`, `EvidenceGraph`, and `EvidenceAudit` must change. `Verify` and `Ci` must include the resulting validation where they already aggregate generated governance checks. `Dev`, `PackLocal`, `DependencyReport`, and `TemplateDrift` change only if planning identifies direct generated-template or package-content coverage needs.

### Key Entities

- **Generated Evidence Command**: A generated project command that claims graph, audit, or evidence workflow completion and must be backed by authoritative validation.
- **Evidence Graph Result**: The validation outcome for task dependencies, evidence references, cycles, dangling links, and task-to-skill evidence requirements.
- **Audit Readiness Contract**: The set of readiness files, required terms, and required sections that determine whether a feature is audit-ready.
- **Skill-Loading Evidence Row**: A single record tying one task to one required skill, including timing that proves the skill was loaded before task work began.
- **Generated Framework Guidance**: Template-provided instructions and examples that generated app authors use while implementing rendering, input, evidence, and screenshot workflows.
- **Screenshot Proof Claim**: A report statement distinguishing live screenshot proof from deterministic scene evidence, pixel-readback fallback, or unsupported/failure evidence.

### Assumptions

- The generated project should reuse the same governance meaning as the repository-level evidence graph and audit checks, even if planning chooses the exact execution mechanism later.
- The task dependency declaration remains the source of truth for which tasks require which skills.
- Audit diagnostics may expose required terms directly because they are governance contract terms, not user secrets.
- Generated guidance should cover common game-style apps first, because the feedback came from a deterministic lunar lander generated app.

## Success Criteria *(mandatory)*

- **SC-001**: In a generated app with a deliberately broken evidence graph, the generated graph command fails in 100% of validation runs and reports the graph failure area before any completion claim.
- **SC-002**: In a generated app with missing audit readiness terms, the generated audit command names every incomplete readiness file and at least 95% of missing required terms in a single run.
- **SC-003**: For a task plan with declared task-to-skill pairings, the generated skill-loading workflow produces one required row per pairing with 100% coverage and no accepted collapsed range rows.
- **SC-004**: A generated app author can identify the required readiness files before implementation starts without inspecting audit internals.
- **SC-005**: Generated guidance review confirms enforced coverage for app message qualification with a `CloseRequested` collision example, vector-to-scene-point conversion, semantic scene facts, pixel-readback fallback, fallback kind, and `proves-screenshot=false` vocabulary.
- **SC-006**: Normal generated app launch remains a persistent interactive launch in existing supported scenarios and does not run evidence validation unless the user invokes an evidence command.

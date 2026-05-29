# Feature Specification: Claude Code Ready Spec Kit

**Feature Branch**: `030-claude-code-ready`  
**Created**: 2026-05-29  
**Status**: Draft  
**Input**: User description: "make this speckit repository/framework claude code ready. the consumer facing artefacts/speckit repository created with the template also needs to be claude code ready. codex and claude configurations should stay in sync automatically. do online research to get up to date information."

## Clarifications

### Session 2026-05-29

- Q: What synchronization model should keep Codex and Claude Code artifacts aligned? → A: Single source generates both Codex and Claude Code artifacts.
- Q: Which Claude Code workflow surface should be canonical? → A: Project skills are canonical; command compatibility is optional.
- Q: What Claude Code project settings should generated projects include? → A: Project permissions and hook settings for all supported workflows.
- Q: Which generated template profiles must be Claude Code ready? → A: All supported profiles that emit Spec Kit agent artifacts.
- Q: How should validation handle Codex/Claude configuration drift? → A: Fail validation on any Codex/Claude drift.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Repository Works In Claude Code (Priority: P1)

As a framework maintainer, I need the FS.Skia.UI Spec Kit repository to expose Claude Code-ready project instructions, skills, settings, and command entry points so that a contributor can open the repository in Claude Code and follow the same Spec Kit workflow available to Codex users.

**Independent Test**: From a clean checkout, inspect the repository's agent-facing configuration. Claude Code can discover project instructions, invoke Spec Kit workflows, and see the same current governance and evidence expectations that Codex sees, without manually translating Codex-only files.

### User Story 2 - Generated Projects Are Claude Code Ready (Priority: P1)

As a template consumer, I need every generated FS.Skia.UI Spec Kit project to include Claude Code-ready project instructions and workflow artifacts so that I can use Claude Code immediately after creating a project from the template.

**Independent Test**: Generate a new project from the template and inspect the generated files. The generated project includes Claude Code-facing instructions and workflow entry points equivalent to the Codex-facing artifacts, and the generated readiness guidance points to project-local Spec Kit plans and evidence commands.

### User Story 3 - Codex And Claude Stay Synchronized (Priority: P1)

As a maintainer, I need Codex and Claude Code configurations to stay synchronized automatically so that workflow instructions, skills, and generated artifacts do not drift between supported coding agents.

**Independent Test**: Change an authoritative Spec Kit instruction or generated agent artifact and run the repository's validation workflow. The workflow either regenerates the corresponding Codex and Claude Code artifacts or fails with a clear drift report naming the mismatched files and required repair action.

### User Story 4 - Current Claude Code Guidance Is Captured (Priority: P2)

As a reviewer, I need the project to document which current Claude Code concepts it supports so that later changes can be evaluated against the right compatibility targets.

**Independent Test**: Review the feature evidence and generated guidance. It identifies supported Claude Code project instructions, project skills, project commands or skill aliases, shared settings, and hook-related expectations using current official documentation, with no stale or unsupported assumptions.

### Edge Cases

- A generated project is created from a template profile that includes only a subset of Spec Kit capabilities.
- A contributor updates Codex-facing instructions but forgets the Claude Code equivalent.
- A contributor updates Claude Code-facing instructions but forgets the Codex equivalent.
- Claude Code project-local files are missing, stale, malformed, or excluded from generated template output.
- User-local Claude Code settings exist and must not be required for project correctness.
- Optional Claude Code hooks are unavailable, disabled, or unsupported in a contributor environment.
- File watchers or live reload behavior do not notice newly created top-level agent directories until the next session.

## Requirements *(mandatory)*

### Change Classification

- **Tier**: Tier 1 (contracted change)
- **Reason**: This feature changes repository-level agent workflow behavior, generated consumer artifacts, template output, governance validation, and evidence expectations. It may add or change tracked files under agent configuration directories and generated template files.
- **Required evidence**: Official Claude Code documentation notes, repository configuration inventory, generated project validation, drift detection proof, template output proof, and named readiness artifacts.

### Functional Requirements

- **FR-001**: The repository MUST provide Claude Code-facing project instructions that are equivalent in intent and scope to the existing Codex-facing project instructions.
- **FR-002**: The repository MUST expose Claude Code-invokable Spec Kit workflows as project skills for the same core lifecycle commands supported by the current Codex integration: specify, clarify, plan, tasks, implement, checklist, analyze, task-to-issues, and the installed git and evidence extension commands.
- **FR-003**: Claude Code-facing workflows MUST preserve the same user input semantics, hook prompts, feature directory behavior, evidence requirements, and validation expectations as their Codex-facing counterparts.
- **FR-004**: Generated projects created from the template MUST include Claude Code-facing project instructions and workflow artifacts equivalent to the generated Codex-facing artifacts.
- **FR-005**: Generated project guidance MUST tell Claude Code users to read the active feature plan when one exists, matching the current generated Codex instruction behavior.
- **FR-006**: Codex and Claude Code configuration generation MUST use a single authoritative source that generates both artifact sets so that maintainers do not manually duplicate instructions across agents.
- **FR-007**: Validation MUST fail on any drift between Codex and Claude Code artifacts in both the framework repository and generated template output.
- **FR-008**: Drift reports MUST identify the mismatched artifact, affected workflow or instruction, expected source of truth, and the command or action needed to repair it.
- **FR-009**: The repository MUST distinguish shared project configuration from user-local Claude Code configuration and MUST NOT require user-local settings for generated projects to pass validation.
- **FR-010**: Claude Code-facing project settings MUST include permissions and hook settings for all supported workflows, MUST be valid project-shareable settings, and MUST avoid embedding secrets, personal paths, or host-specific approval assumptions.
- **FR-011**: Hook-related support MUST be generated for supported workflows only when the repository can validate that each hook works from project-local files in a normal checkout.
- **FR-012**: Claude Code project skills MUST include enough metadata for automatic discovery and direct invocation. Command-compatible workflow files MAY be generated when they naturally share the same authoritative source, but project skills remain canonical.
- **FR-013**: Documentation and readiness evidence MUST cite current official Claude Code guidance for project instructions, skills or commands, settings, and hooks.
- **FR-014**: Template validation MUST prove that generated Claude Code-facing artifacts are included in all supported template profiles that emit Spec Kit agent artifacts, including every profile that generates Codex-facing artifacts.
- **FR-015**: Evidence audit patterns MUST continue to recognize both Codex and Claude Code agent configuration files where synthetic or readiness disclosure rules apply.
- **FR-016**: The feature MUST avoid changing package identities, package versions, or runtime application behavior unless planning discovers a necessary template package-content change.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Package identities and package versions are not expected to change. Package contents should not change unless implementation discovers that generated template validation or evidence commands require product-source changes. Generated package consumers are expected to change through template and agent workflow artifacts.
- **Public contract impact**: Public `.fsi` signatures and framework APIs are not expected to change. Documented workflow contracts, generated agent artifacts, template output, validation baselines, and governance checks are expected to change.
- **State workflow impact**: Application state workflow is out of scope. Agent workflow orchestration, command invocation, hook prompts, generated readiness command guidance, and configuration synchronization are in scope.
- **Layout/rendering impact**: Layout, rendering, screenshots, Vulkan, Skia, visual output, chart behavior, graph controls, and DataGrid behavior are out of scope except for existing readiness/evidence references that must remain consistent.
- **Evidence obligations**: Required real evidence paths are `specs/030-claude-code-ready/readiness/claude-code-research.md`, `specs/030-claude-code-ready/readiness/repository-agent-inventory.md`, `specs/030-claude-code-ready/readiness/config-sync-validation.md`, `specs/030-claude-code-ready/readiness/generated-template-agent-artifacts.md`, and `specs/030-claude-code-ready/readiness/generated-project-claude-code-ready.md`.
- **Unsupported scope**: New product UI features, generated app gameplay changes, release publishing, package distribution, browser or mobile support, renderer replacement, and broad roadmap changes are out of scope.
- **Build-target impact**: `Verify`, `TemplateCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` may need changes. `Ci` may aggregate the updated checks. `Dev`, `PackLocal`, and `DependencyReport` change only if implementation discovers package-content or dependency impact.

## Success Criteria *(mandatory)*

- **SC-001**: A contributor can identify and invoke every supported Spec Kit workflow from Claude Code project-local artifacts in under 5 minutes from a clean checkout.
- **SC-002**: A newly generated project includes Claude Code-facing instructions and workflow artifacts for 100% of the Codex-facing Spec Kit workflows included in that generated project.
- **SC-003**: Synchronization validation fails when a deliberate one-line drift is introduced between matching Codex and Claude Code workflow instructions and reports the mismatch with the affected files.
- **SC-004**: Template validation proves that Claude Code-facing generated artifacts are present for every supported template profile that emits Spec Kit agent artifacts.
- **SC-005**: Official Claude Code documentation references used by the feature are recorded in readiness evidence with retrieval dates and the supported concept each reference justifies.
- **SC-006**: No project-shareable Claude Code artifact contains personal paths, secrets, user-local credentials, or host-specific approval assumptions.
- **SC-007**: Reviewers can determine whether repository artifacts, generated artifacts, and sync validation are complete from named readiness evidence in under 7 minutes.

## Assumptions

- "Claude Code ready" means usable through project-local Claude Code instructions, project skills, project-shareable permissions and hook settings for supported workflows, and documented hook behavior.
- Codex remains a supported integration and must not be degraded while adding Claude Code support.
- The existing Codex integration files are the practical baseline for workflow parity.
- Project-local artifacts are preferred over user-local configuration because generated repositories must work for consumers without modifying their personal Claude Code setup.
- Current official Claude Code documentation treats project skills as the recommended extension point while existing project command files remain compatible.

## Key Entities

- **Agent Instruction Artifact**: A project-local instruction file that gives an agent repository-specific workflow context.
- **Workflow Artifact**: A project-local skill that lets Claude Code execute a Spec Kit lifecycle command, with command-compatible files treated as optional generated aliases.
- **Synchronization Source**: The single authoritative content source used to generate matching Codex and Claude Code artifacts.
- **Drift Report**: A failing validation result that names out-of-sync agent artifacts and explains how to restore parity.
- **Generated Project Agent Bundle**: The set of agent-facing files emitted by the FS.Skia.UI template into a consumer project.
- **Claude Code Compatibility Evidence**: Readiness evidence that maps implemented project artifacts to current official Claude Code concepts and limitations.

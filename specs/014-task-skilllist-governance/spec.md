# Feature Specification: Task Skilllist Governance

**Feature Branch**: `014-task-skilllist-governance`  
**Created**: 2026-05-18  
**Status**: Draft  
**Input**: User description: "i want a compulsory step after task generation that evaluater all tasks if any capablity skill would help and add those skills to a skillist field that each task has. the implementation is required to load those skills for implementing those tasks. best put this in the constitution.md."

## Clarifications

### Session 2026-05-18

- Q: Where must the required `skillist` field live for generated tasks? → A: Structured task metadata and mirrored in `tasks.md`.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Tasks Record Applicable Skills (Priority: P1)

As a framework maintainer, I need every generated task to be evaluated against the repository's capability skills so task metadata and visible task lists explicitly show which local skills apply before implementation begins.

**Independent Test**: Generate tasks for a feature that touches at least two known capabilities and verify every task has a `skillist` field in structured task metadata and a matching visible mirror in `tasks.md`. Tasks that match capability skill descriptions list the applicable skill names, and tasks with no relevant capability skill use an explicit empty list.

### User Story 2 - Implementation Loads Task Skills (Priority: P1)

As a maintainer reviewing implementation work, I need the implementation workflow to load each task's declared skills before work starts on that task so capability-specific guidance is applied consistently.

**Independent Test**: Run implementation against a task list containing `skillist` entries and verify the workflow records that each listed skill was loaded before the corresponding task was implemented. A task with missing or unreadable declared skills blocks implementation with an actionable diagnostic.

### User Story 3 - Constitution Makes the Rule Mandatory (Priority: P2)

As a contributor, I need the repository constitution to state this requirement so future task templates, agents, and reviews treat task skill evaluation as a governance gate rather than optional advice.

**Independent Test**: Review the updated constitution and task-generation guidance and verify they require post-task skill evaluation, `skillist` population for each task, and implementation-time skill loading as mandatory workflow obligations.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The constitution MUST require a compulsory skill evaluation step immediately after task generation and before any task list is considered ready for implementation.
- **FR-002**: The task-generation workflow MUST evaluate every task against available capability skill descriptions and determine whether one or more skills would materially help implement that task.
- **FR-003**: Every generated task MUST include a `skillist` field in structured task metadata containing the ordered list of applicable skill identifiers, or an explicit empty list when no capability skill applies.
- **FR-004**: When multiple skills apply to a task, the `skillist` field MUST contain the minimal skill set needed for the work and preserve dependency order where one skill should be loaded before another.
- **FR-005**: The implementation workflow MUST load every skill named in a task's `skillist` before implementing that task.
- **FR-006**: Implementation MUST block a task when its `skillist` names a skill that is missing, unreadable, or ambiguous, and the diagnostic MUST identify the task and the unresolved skill.
- **FR-007**: Task readiness validation MUST fail when any task is missing the `skillist` field in structured task metadata, even if no skill applies.
- **FR-008**: Task readiness validation MUST fail when a task's `skillist` conflicts with the task description or omits an obviously applicable capability skill. A capability skill is obviously applicable when the task description, referenced file path, command name, or changed artifact matches the skill id, skill description trigger phrase, or declared capability path. Ambiguous matches MUST be reported for human resolution rather than silently accepted.
- **FR-009**: The human-readable `tasks.md` entry for each generated task MUST mirror the task's structured `skillist` value so reviewers can see applicable skills without opening the metadata file.
- **FR-010**: Task and implementation guidance MUST explain that capability skills are preferred over generic guidance for matching tasks.
- **FR-011**: Existing generated task lists MUST be migrated or regenerated before implementation if they do not contain the required structured `skillist` field and matching `tasks.md` mirror.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identities, package contents, package versions, or generated package consumers are expected to change. This is a governance and Spec Kit workflow change.
- **Public contract impact**: No `.fsi` signatures or public runtime APIs are expected to change. Structured task metadata schema, task templates, guidance, and validation contracts may change.
- **State workflow impact**: The task-generation and implementation workflows change by adding a required post-generation skill evaluation step and a required pre-task skill-loading step.
- **Layout/rendering impact**: No layout, charts, DataGrid, rendering, screenshots, Vulkan, Skia, visual output, or unsupported environment diagnostics change.
- **Evidence obligations**: Required evidence paths should include the updated constitution, task template or task-generation guidance, implementation guidance, and readiness evidence showing validation failures for missing/invalid `skillist` fields and successful loading for valid fields.
- **Unsupported scope**: This feature does not create new capability skills, change application visuals, alter package distribution, modify renderer behavior, or automate external repository migration.
- **Build-target impact**: `EvidenceGraph`, `EvidenceAudit`, task-generation validation, and implementation guidance may need updates. `Dev`, `PackLocal`, `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, and `TemplateDrift` should change only if existing governance checks depend on task schema or generated guidance.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of newly generated tasks contain a structured `skillist` field and matching `tasks.md` mirror after task generation completes.
- **SC-002**: Task readiness validation rejects task lists with missing `skillist` fields in under 30 seconds for a typical feature task list.
- **SC-003**: For a feature touching known capabilities, at least 95% of tasks with obvious capability ownership list the expected capability skill during review.
- **SC-004**: Implementation records skill loading before work begins for 100% of tasks whose `skillist` is non-empty.
- **SC-005**: Contributors can identify the mandatory skill-evaluation and skill-loading requirements from the constitution and task guidance within 5 minutes.
- **SC-006**: Validation fixtures demonstrate multi-skill ordering by rejecting a task whose `skillist` omits a required prerequisite skill or lists dependent skills before their prerequisite.

## Assumptions

- The canonical field name is `skillist`, matching the user request, even though `skillList` or `skill_list` would also be common spellings.
- Available capability skills are the skills already discoverable from repository governance, package-owned skill files, template capability declarations, and the active agent skill registry.
- A task with no applicable capability skill remains valid only when it declares an explicit empty `skillist`.

## Key Entities

- **Task**: A unit of implementation work generated from the feature plan. Each task has an identifier, description, dependencies, status, required structured `skillist` field, and matching `tasks.md` mirror.
- **Capability Skill**: A local skill with a description that can guide implementation for a capability-owned area.
- **Skill Evaluation**: The mandatory review performed after task generation to decide which capability skills apply to each task.
- **Implementation Loader**: The workflow obligation to load each task's declared skills before implementation begins.

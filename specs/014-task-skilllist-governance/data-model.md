# Data Model: Task Skilllist Governance

## Task

Represents one generated implementation task.

Fields:

- `id`: Stable task identifier such as `T001`.
- `status`: One of `[ ]`, `[X]`, `[S]`, `[F]`, or `[-]` in `tasks.md`.
- `description`: Human-readable task work item.
- `dependencies`: Ordered list of task ids from `tasks.deps.yml`.
- `skillist`: Ordered list of applicable skill identifiers. Empty list means no applicable capability skill.
- `skillistMirror`: The visible `tasks.md` representation of the same ordered list.
- `phase`: Setup, Foundation, user story, Integration, or Polish.
- `story`: Optional user story tag such as `US1`.

Validation rules:

- Every task MUST have a `skillist` field in structured metadata.
- `skillist` MUST be a list. It MAY be empty.
- Every value in `skillist` MUST resolve to exactly one readable skill.
- `skillistMirror` MUST match the structured list exactly.
- When a task obviously touches a capability-owned area, the minimal applicable capability skills MUST be present.
- When multiple skills apply, order MUST reflect dependency order where one skill should be loaded first.

## Capability Skill

Represents a local skill that can guide task implementation.

Fields:

- `id`: Canonical identifier such as `fs-skia-layout` or `speckit-tasks`.
- `path`: Repository-relative path to `SKILL.md`.
- `description`: Skill trigger description.
- `source`: Package-owned, template-owned, project-owned, or Spec Kit skill source.
- `dependencies`: Optional prerequisite skill identifiers.

Validation rules:

- A declared task skill MUST resolve unambiguously by id or accepted alias.
- The skill file MUST be readable before implementation starts.
- Capability skills are preferred over generic guidance when both match a task.

## Skill Evaluation

Represents the compulsory post-generation review of all tasks.

Fields:

- `taskId`: Evaluated task.
- `candidateSkills`: Skills whose descriptions match the task.
- `selectedSkills`: Minimal ordered skill set written to `skillist`.
- `decisionReason`: Short rationale for non-obvious inclusion or exclusion.
- `validator`: Workflow, command, or agent that performed the evaluation.

Validation rules:

- Evaluation runs immediately after task generation and before readiness.
- Every task receives a selected list, even when the list is empty.
- Validation fails when an obviously applicable skill is omitted.

## Implementation Skill Load

Represents the per-task precondition before implementation work begins.

Fields:

- `taskId`: Task being implemented.
- `skillIds`: Ordered `skillist` from structured metadata.
- `loadedPaths`: Skill file paths read before work starts.
- `result`: Loaded, blocked-missing, blocked-unreadable, or blocked-ambiguous.
- `diagnostic`: Actionable failure message when blocked.

Validation rules:

- All declared skills MUST be loaded before task implementation starts.
- Missing, unreadable, or ambiguous skills block the task.
- The implementation workflow records load evidence for every non-empty `skillist`.

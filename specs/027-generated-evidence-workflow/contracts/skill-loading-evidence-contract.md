# Contract: Skill-Loading Evidence

## Scope

Applies to task implementation evidence for tasks with non-empty `skillist` metadata.

## Required Input

- `tasks.md` with visible `[skillist: ...]` mirrors.
- `tasks.deps.yml` with structured `skillist` values.
- A skill inventory resolving each skill id to one readable `SKILL.md`.

## Required Row Shape

Each required row represents exactly one task and one skill:

| Field | Required | Rule |
|-------|----------|------|
| `task_id` | Yes | Exact `T###` or `T####` id from task metadata |
| `skill_id` | Yes | Exact skill id from the task's structured `skillist` |
| `skill_path` | Yes | Resolved readable `SKILL.md` path |
| `loaded_at` | Yes | Timestamp when the skill was loaded |
| `work_started_at` | Yes | Timestamp when task work began |
| `source` | Yes | `automatic`, `generated`, or `manual` |

## Validation Rules

- Expected rows are the Cartesian list of every task and every skill listed for that task.
- Tasks with `skillist: []` require no skill-loading row.
- Collapsed task ranges, prose batches, and multi-task rows are invalid.
- `loaded_at` must be earlier than `work_started_at`.
- Equal timestamps are invalid because they do not prove pre-task loading.
- Duplicate rows are diagnostics and do not compensate for missing rows.

## Required Diagnostics

- Missing row: task id and skill id.
- Late row: task id, skill id, loaded timestamp, and work-start timestamp.
- Collapsed row: the invalid text and why it cannot satisfy row-level proof.
- Ambiguous skill path: skill id and candidate paths.

## Verification

- Generated helper tests prove one row per required pairing.
- Graph/audit fixtures reject range rows and late rows.
- Readiness evidence records a valid generated table and at least one rejected malformed fixture under `[SEH]`.

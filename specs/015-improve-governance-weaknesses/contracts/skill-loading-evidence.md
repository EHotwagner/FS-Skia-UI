# Contract: Skill-Loading Evidence

## Record Shape

```yaml
task_skill_evidence:
  - task_id: T017
    declared_skill_id: speckit-implement
    resolved_skill_path: .agents/skills/speckit-implement/SKILL.md
    load_result: loaded
    loaded_at: "2026-05-26T09:14:20Z"
    work_started_at: "2026-05-26T09:16:03Z"
    evidence_path: specs/015-improve-governance-weaknesses/readiness/skill-loading-evidence.md
    exception: null
```

## Required Behavior

- Every non-empty task `skillist` entry produces one evidence row.
- `loaded_at` must precede `work_started_at`.
- `resolved_skill_path` must point to exactly one readable `SKILL.md`.
- Missing, unreadable, ambiguous, late, or absent evidence blocks task completion.
- Reviewer exceptions are allowed only when they include task, skill, reason, approving reviewer, and compensating evidence.

## Diagnostics

Diagnostics must name the task id and skill id:

- `T017: declared skill speckit-implement has no pre-work load evidence`
- `T017: skill speckit-implement loaded after work started`
- `T017: declared skill fs-skia-layout resolves to multiple SKILL.md files`

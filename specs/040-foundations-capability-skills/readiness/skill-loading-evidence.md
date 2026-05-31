# Skill Loading Evidence — 040

Only T029 and T030 carry a non-empty `skillist` (the six `fsharp-*` capability
skills are reference-only per FR-007/SC-005 and deliberately appear in no
`skillist`). The two evidence skills below were resolved to exactly one readable
`SKILL.md` and loaded in declared order before each task's work began.

| Task | Skill id | Resolved path | Load result | loaded_at | work_started_at | Evidence path | Reviewer exception |
|------|----------|---------------|-------------|-----------|-----------------|---------------|--------------------|
| T029 | speckit-evidence-graph | `/home/developer/projects/FS-Skia-UI/.agents/skills/speckit-evidence-graph/SKILL.md` | loaded | 2026-05-31T17:00:00Z | 2026-05-31T17:00:30Z | `readiness/task-graph.md`, `readiness/logs/evidence-graph.txt` | none |
| T030 | speckit-evidence-audit | `/home/developer/projects/FS-Skia-UI/.agents/skills/speckit-evidence-audit/SKILL.md` | loaded | 2026-05-31T17:00:05Z | 2026-05-31T17:01:00Z | `readiness/evidence-audit.md`, `readiness/logs/evidence-audit.txt` | none |

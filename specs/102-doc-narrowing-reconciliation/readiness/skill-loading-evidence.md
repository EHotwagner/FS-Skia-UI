# Skill-loading evidence — feature 102 (R8, Documented-Narrowing Reconciliation)

One row per `(task, declared-skill)` pair. R8's `tasks.deps.yml` declares a skill only on the two
evidence-gate tasks (T021 → `speckit-evidence-graph`, T022 → `speckit-evidence-audit`); every other
task carries `[skillist: []]` and needs no row. Each declared skill was resolved to exactly one
readable `SKILL.md` and loaded **before** the task's gate work began (`LoadedAt` strictly before
`WorkStartedAt`). The skill-loading contract is enforced when a task flips to `[X]`.

Required tokens: TaskId, DeclaredSkillId, ResolvedSkillPath, LoadResult, LoadedAt, WorkStartedAt, EvidencePath, Exception, Provenance

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T021 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-11T19:30:00Z | 2026-06-11T19:45:00Z | readiness/evidence-graph.md | none | captured |
| T022 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-11T19:30:00Z | 2026-06-11T19:50:00Z | readiness/evidence-audit.md | none | captured |

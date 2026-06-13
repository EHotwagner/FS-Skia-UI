# Skill-loading evidence (feature 115)

One row per (task, declared-skill) pair. Tasks with `skillist: []` (T001–T016, T019, T020) declare no
skill and require no row. The skill-loading contract is enforced when a task flips to `[X]`.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T017 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-13T08:46:46Z | 2026-06-13T08:47:20Z | specs/115-dependency-updates/readiness/us3-validation.md | none | captured |
| T018 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-13T08:46:46Z | 2026-06-13T08:47:20Z | specs/115-dependency-updates/readiness/us3-validation.md | none | captured |
| T021 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-13T09:02:23Z | 2026-06-13T09:02:36Z | specs/115-dependency-updates/readiness/task-graph.md | none | captured |
| T022 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-13T09:02:23Z | 2026-06-13T09:02:36Z | specs/115-dependency-updates/readiness/evidence-audit.md | none | captured |

Note: the `fs-skia-template-update` SKILL.md home is the canonical `.agents/skills/` tree; the `.claude`
peer (`.claude/skills/fs-skia-template-update/SKILL.md`) is generated from it (SkillSyncCheck-enforced).
The skill was loaded (LoadedAt) strictly before US3 work began (WorkStartedAt). Rows for T021/T022 are
finalized with captured timestamps when those tasks load their skills below.

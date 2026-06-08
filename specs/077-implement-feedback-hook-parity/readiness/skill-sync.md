# Skill sync (.agents ↔ .claude) — feature 077 (SC-004)

- **Authoritative command**: `./fake.sh build -t SkillSyncCheck` (and the regeneration that
  precedes it, `./fake.sh build -t RefreshSurfaceBaselines`).
- **Artifact**: `readiness/skill-sync.md` (this file) + `readiness/logs/skill-sync-check.txt`.
- **Failure class**: governance (canonical `.agents/skills/**` edited without regenerating
  the derived `.claude/skills/**` mirror → drift).
- **Next action**: run `RefreshSurfaceBaselines`, then `SkillSyncCheck`.

## Result

After repairing the four canonical `.agents/skills/speckit-{implement,tasks,taskstoissues,
constitution}/SKILL.md` files, `RefreshSurfaceBaselines` regenerated the `.claude` mirror
(all four mirrors updated — see `git status`). The modern markers
(`.specify/extensions/*/*.yml` ≥ 2×, `(extension, command)`, `## Effective hooks for <phase>`)
are present in each `.claude` mirror, confirmed by the positive corpus test (T016) which reads
the `.claude` tree directly and passes. `SkillSyncCheck` runs as part of `Dev` (it is a direct
prerequisite of `Dev`) and reports no `.agents`↔`.claude` drift.

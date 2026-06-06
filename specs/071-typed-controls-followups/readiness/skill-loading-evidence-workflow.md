# Skill-loading evidence workflow (071) — T003

- **Authoritative artifact**: `readiness/skill-loading-evidence.md` (the 8-column
  table: `TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt |
  WorkStartedAt | EvidencePath | Exception`).
- **Authoritative command/gate**: `./fake.sh build -t EvidenceAudit`
  (`Audit.validateSkillLoadingEvidence`). Enforced once a task flips to `[X]`.
- **Resolution rule**: each declared skill id resolves to exactly **one**
  readable `SKILL.md` in the `SkillRegistry` roots (`.agents/skills/<id>`,
  `src/<pkg>/skill`, `template/fragments/<frag>/skill`). `fs-skia-ui-widgets`
  resolves to `src/Controls/skill/SKILL.md`; the typed/codegen/evidence skills to
  `.agents/skills/<id>/SKILL.md`.
- **Ordering rule**: `LoadedAt` strictly earlier than `WorkStartedAt` per row.
  One row per `(task, declared-skill)` pair.
- **Failure class**: `evidence-audit` (skill-loading-evidence). A missing row,
  an unreadable/ambiguous resolved path, equal/inverted timestamps, or a
  `LoadResult <> loaded` without a reviewer exception blocks the owning task.
- **Next action on failure**: load the named skill from its registry home, record
  the row with `LoadedAt < WorkStartedAt`, and re-run `EvidenceAudit`.

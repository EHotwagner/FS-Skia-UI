# Generated guidance validation (085)

The generated `.claude` skill tree is **generated from the canonical `.agents`
tree** (never hand-synced): the new `.agents/skills/fs-skia-viewer-host/SKILL.md`
and the edits to `.agents/skills/fs-skia-typed-controls/SKILL.md` and
`.agents/skills/speckit-specify/SKILL.md` are mirrored into `.claude` by
`./fake.sh build -t RefreshSurfaceBaselines` (T034). Currency is enforced by
`SkillSyncCheck`; quality by `SkillQualityCheck`.

- **Authoritative command**: `./fake.sh build -t GeneratedGuidanceCheck`
  (+ `SkillSyncCheck` / `SkillQualityCheck` inside the escalated order).
- **Artifact**: regenerated `.claude/skills/**` mirror + `skillist-reference.md`.
- **Failure class**: a `.claude` mirror edited directly (drift) or a skill missing
  a required quality heading is blocking.
- **Next action**: T034 regenerates the mirror; T035 runs `GeneratedGuidanceCheck`.

Cite `.agents/skills/**` as the source of record (not `.claude/**`).

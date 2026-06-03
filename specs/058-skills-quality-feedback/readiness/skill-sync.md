# Skill-Sync Currency Evidence (T025)

`./fake.sh build -t RefreshSurfaceBaselines` regenerated the derived `.claude/skills`
tree from the canonical `.agents/skills` tree (and re-rendered `validation.contract.yml`
+ target metadata) after the `.agents/skills/fsharp-*`, `fs-skia-layout-evidence`, and
`fs-skia-template-update` edits.

`./fake.sh build -t SkillSyncCheck` → **Status: Ok** (no drift). The eight regenerated
`.claude` mirrors match their canonical `.agents` sources byte-for-byte:

```
.claude/skills/fs-skia-layout-evidence/SKILL.md
.claude/skills/fs-skia-template-update/SKILL.md
.claude/skills/fsharp-build-orchestration/SKILL.md
.claude/skills/fsharp-code-generation/SKILL.md
.claude/skills/fsharp-graph-algorithms/SKILL.md
.claude/skills/fsharp-io-globbing/SKILL.md
.claude/skills/fsharp-parsing/SKILL.md
.claude/skills/fsharp-shell-process/SKILL.md
```

The `.claude` tree is generated, never hand-synced (FR-005); currency is enforced by
`SkillSyncCheck`.

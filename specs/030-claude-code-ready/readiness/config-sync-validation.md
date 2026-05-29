# Config Sync Validation

Validation target: `./fake.sh build -t TemplateDrift`

Covered diagnostics:

- Repository `.agents/skills/*/SKILL.md` and `.claude/skills/*/SKILL.md` peer presence and byte parity.
- Template `.agents/skills` source mappings have matching `.claude/skills` target mappings for lifecycle, extension, and selected capability skills.
- `.claude/settings.json` files parse as JSON and avoid user-local settings dependencies.
- Hook commands use project-local `$CLAUDE_PROJECT_DIR` paths.

Controlled drift fixtures:

| Fixture | Expected result | Required fields |
|---------|-----------------|-----------------|
| `codex-claude-codex-drift` | FAIL | `scope`, `sourceId`, `workflowId`, `expectedPath`, `actualPath`, `differenceSummary`, `repairAction` |
| `codex-claude-claude-drift` | FAIL | `scope`, `sourceId`, `workflowId`, `expectedPath`, `actualPath`, `differenceSummary`, `repairAction` |

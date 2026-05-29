# Repository Agent Inventory

| Class | Source id | Codex path | Claude path | Validation status |
|-------|-----------|------------|-------------|-------------------|
| instruction | repository-instructions | `AGENTS.md` | `CLAUDE.md` | PASS: Claude imports Codex active-plan guidance. |
| settings | claude-settings | n/a | `.claude/settings.json` | PASS: valid project-shareable JSON with project-local hook command. |
| hook | speckit-project-validation | n/a | `.claude/hooks/validate-speckit-project.sh` | PASS: project-local script checks `AGENTS.md` and active feature `plan.md`. |
| workflow skills | speckit-* | `.agents/skills/speckit-*/SKILL.md` | `.claude/skills/speckit-*/SKILL.md` | PASS: peer files are byte-matched. |
| extension skills | speckit-git-*`, `speckit-evidence-*` | `.agents/skills/*/SKILL.md` | `.claude/skills/*/SKILL.md` | PASS: peer files are byte-matched. |
| command aliases | speckit-* | n/a | `.claude/commands/*.md` | PASS: optional aliases delegate to project skills. |

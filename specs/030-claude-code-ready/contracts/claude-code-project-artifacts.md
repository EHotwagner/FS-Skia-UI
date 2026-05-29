# Contract: Claude Code Project Artifacts

## Repository Artifacts

The framework repository must include:

- `CLAUDE.md` importing `AGENTS.md`.
- `.claude/skills/<workflow>/SKILL.md` for every supported Spec Kit lifecycle, git extension, and evidence extension workflow.
- `.claude/settings.json` with project-shareable permissions and validated hook settings for supported workflows.
- `.claude/hooks/*` only for hook scripts proven to work from a normal checkout.
- Optional `.claude/commands/*.md` aliases generated from the same workflow source.

## Generated Product Artifacts

Every generated product profile that emits Codex Spec Kit artifacts must include:

- `CLAUDE.md`.
- `.claude/skills/fs-skia-project/SKILL.md`.
- `.claude/skills/speckit-*` matching the generated Codex lifecycle skills.
- `.claude/skills/<capability>/SKILL.md` for every selected capability skill copied into `.agents/skills`.
- `.claude/settings.json` and any validated project-local hook scripts.

## Settings Rules

- Settings must be valid JSON.
- Settings must not require `.claude/settings.local.json`.
- Settings must not contain secrets, user home paths, machine-specific absolute paths, or approval assumptions tied to this execution environment.
- Hook commands must use project-local paths such as `${CLAUDE_PROJECT_DIR}` when referencing repository scripts.

## Discovery Rules

- Project skills are canonical.
- Command aliases are compatibility affordances only.
- If a skill and command share a name, the skill remains the source of truth.

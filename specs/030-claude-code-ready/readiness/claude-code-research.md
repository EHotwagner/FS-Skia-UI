# Claude Code Research

Retrieved: 2026-05-29

| Concept | Official source | Implemented artifact |
|---------|-----------------|----------------------|
| Project memory and imports | https://docs.claude.com/en/docs/claude-code/memory | `CLAUDE.md` imports `AGENTS.md` with `@AGENTS.md` so active-plan guidance remains single-sourced. |
| Project settings | https://docs.anthropic.com/en/docs/claude-code/settings | `.claude/settings.json` is committed and project-shareable; `.claude/settings.local.json` and `~/.claude/settings.json` are not required. |
| Hooks | https://docs.anthropic.com/en/docs/claude-code/hooks | `.claude/settings.json` references project-local hooks through `$CLAUDE_PROJECT_DIR/.claude/hooks/validate-speckit-project.sh`. |
| Project slash commands | https://docs.anthropic.com/en/docs/claude-code/slash-commands | `.claude/commands/*.md` aliases are optional compatibility wrappers around project skills. |
| Project skills | https://docs.claude.com/en/docs/claude-code/skills | `.claude/skills/*/SKILL.md` mirrors `.agents/skills/*/SKILL.md`; generated products receive matching selected capability skills. |

Limitations recorded for reviewers:

- Project skills are canonical; command aliases are compatibility affordances.
- Project hooks are limited to validating project-local Spec Kit guidance files. User-local settings, enterprise managed policy, and personal preferences are out of scope.
- Claude Code session/watch behavior may require reopening or refreshing a session after newly generated `.claude/**` files appear.

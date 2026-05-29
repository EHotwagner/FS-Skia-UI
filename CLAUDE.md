# FS Skia UI Claude Code Instructions

@AGENTS.md

Claude Code should use the project-local skills in `.claude/skills/` for Spec Kit workflows. The matching Codex source artifacts live under `.agents/skills/`; validation treats these as synchronized peers.

Project settings live in `.claude/settings.json`. User-local settings such as `.claude/settings.local.json` or `~/.claude/settings.json` are optional personal preferences and are not required for repository or generated-project readiness.

# Generated Template Agent Artifacts

Template source and package validation now require:

- `CLAUDE.md`
- `.claude/settings.json`
- `.claude/skills/fs-skia-project/SKILL.md`
- `.claude/skills/speckit-*/SKILL.md`
- `.claude/skills/<selected-capability>/SKILL.md`

`build.fsx` validates source and package template contents and records selected Claude skills in generated file-list reports.

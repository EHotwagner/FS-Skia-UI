# Skill Assignment Guidance

command: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter Asteroids`
scanned files: `.specify/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/commands/speckit.tasks.md`, `.agents/skills/speckit-tasks/SKILL.md`
observed: scene rendering -> fs-skia-scene; screenshot capture -> fs-skia-skiaviewer; layout readability -> fs-skia-layout-evidence; persistent viewer launch -> fs-skia-skiaviewer; deterministic evidence mode -> fs-skia-layout-evidence; generated-package validation -> fs-skia-template-update; graph validation -> speckit-evidence-graph; audit validation -> speckit-evidence-audit; debug-loop skills -> speckit-debug-loop.
missing: none.
failure class: SkillAssignmentGuidance.
next action: keep `[skillist: ...]` visible mirrors aligned with structured metadata.

Resolved skill ids and paths:

| Skill id | Resolved `SKILL.md` path | Matched signals | Confidence | Ambiguity | Reviewer disposition |
|----------|--------------------------|-----------------|------------|-----------|----------------------|
| fs-skia-scene | `src/Scene/skill/SKILL.md` | scene rendering, rasterized scene proof | high | none | accepted |
| fs-skia-skiaviewer | `src/SkiaViewer/skill/SKILL.md` | screenshot capture, persistent viewer launch | high | none | accepted |
| fs-skia-layout-evidence | `.agents/skills/fs-skia-layout-evidence/SKILL.md` | layout readability, visual evidence honesty, feedback classification | high | none | accepted |
| fs-skia-template-update | `.agents/skills/fs-skia-template-update/SKILL.md` | generated-package validation, generated product guidance | high | none | accepted |
| speckit-evidence-graph | `.agents/skills/speckit-evidence-graph/SKILL.md` | graph validation | high | none | accepted |
| speckit-evidence-audit | `.agents/skills/speckit-evidence-audit/SKILL.md` | audit validation | high | none | accepted |
| speckit-debug-loop | `/home/developer/.codex/skills/managed/speckit-fsharp-tooling--speckit-debug-loop/SKILL.md` | debug-loop skills, debug-before-broad-rerun | medium | external managed skill | accepted |

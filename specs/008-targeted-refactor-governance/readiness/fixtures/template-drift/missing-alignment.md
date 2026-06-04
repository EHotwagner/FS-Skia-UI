# Template Drift Report

FAIL

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Scene/Scene.fs` | `source-code` |

## Required Alignment Classes

- `src/Scene/Scene.fs` requires `source-contract`
- `src/Scene/Scene.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: ``
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/061-breakout-consumer-friction-followups`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- scope=repository sourceId=speckit-specify workflowId=speckit-specify expectedPath=.agents/skills/speckit-specify/SKILL.md actualPath=.claude/skills/speckit-specify/SKILL.md differenceSummary=skill contents differ repairAction=regenerate Codex and Claude skills from the shared source
- scope=repository sourceId=speckit-analyze workflowId=speckit-analyze expectedPath=.agents/skills/speckit-analyze/SKILL.md actualPath=.claude/skills/speckit-analyze/SKILL.md differenceSummary=skill contents differ repairAction=regenerate Codex and Claude skills from the shared source
- scope=repository sourceId=speckit-checklist workflowId=speckit-checklist expectedPath=.agents/skills/speckit-checklist/SKILL.md actualPath=.claude/skills/speckit-checklist/SKILL.md differenceSummary=skill contents differ repairAction=regenerate Codex and Claude skills from the shared source
- scope=repository sourceId=speckit-clarify workflowId=speckit-clarify expectedPath=.agents/skills/speckit-clarify/SKILL.md actualPath=.claude/skills/speckit-clarify/SKILL.md differenceSummary=skill contents differ repairAction=regenerate Codex and Claude skills from the shared source
- scope=repository sourceId=speckit-plan workflowId=speckit-plan expectedPath=.agents/skills/speckit-plan/SKILL.md actualPath=.claude/skills/speckit-plan/SKILL.md differenceSummary=skill contents differ repairAction=regenerate Codex and Claude skills from the shared source
- scope=repository sourceId=speckit-tasks workflowId=speckit-tasks expectedPath=.agents/skills/speckit-tasks/SKILL.md actualPath=.claude/skills/speckit-tasks/SKILL.md differenceSummary=skill contents differ repairAction=regenerate Codex and Claude skills from the shared source
- scope=repository sourceId=fs-skia-layout-readability workflowId=fs-skia-layout-readability expectedPath=.agents/skills/fs-skia-layout-readability/SKILL.md actualPath=.claude/skills/fs-skia-layout-readability/SKILL.md differenceSummary=skill contents differ repairAction=regenerate Codex and Claude skills from the shared source

## Diagnostics

- src/Scene/Scene.fs: path class `source-code` is missing same-diff required alignment class `source-contract`.
- src/Scene/Scene.fs: path class `source-code` is missing active feature evidence naming the changed path or affected feature area; required alignment class `active-feature-evidence`.
- scope=repository sourceId=speckit-specify workflowId=speckit-specify expectedPath=.agents/skills/speckit-specify/SKILL.md actualPath=.claude/skills/speckit-specify/SKILL.md differenceSummary=skill contents differ repairAction=regenerate Codex and Claude skills from the shared source
- scope=repository sourceId=speckit-analyze workflowId=speckit-analyze expectedPath=.agents/skills/speckit-analyze/SKILL.md actualPath=.claude/skills/speckit-analyze/SKILL.md differenceSummary=skill contents differ repairAction=regenerate Codex and Claude skills from the shared source
- scope=repository sourceId=speckit-checklist workflowId=speckit-checklist expectedPath=.agents/skills/speckit-checklist/SKILL.md actualPath=.claude/skills/speckit-checklist/SKILL.md differenceSummary=skill contents differ repairAction=regenerate Codex and Claude skills from the shared source
- scope=repository sourceId=speckit-clarify workflowId=speckit-clarify expectedPath=.agents/skills/speckit-clarify/SKILL.md actualPath=.claude/skills/speckit-clarify/SKILL.md differenceSummary=skill contents differ repairAction=regenerate Codex and Claude skills from the shared source
- scope=repository sourceId=speckit-plan workflowId=speckit-plan expectedPath=.agents/skills/speckit-plan/SKILL.md actualPath=.claude/skills/speckit-plan/SKILL.md differenceSummary=skill contents differ repairAction=regenerate Codex and Claude skills from the shared source
- scope=repository sourceId=speckit-tasks workflowId=speckit-tasks expectedPath=.agents/skills/speckit-tasks/SKILL.md actualPath=.claude/skills/speckit-tasks/SKILL.md differenceSummary=skill contents differ repairAction=regenerate Codex and Claude skills from the shared source
- scope=repository sourceId=fs-skia-layout-readability workflowId=fs-skia-layout-readability expectedPath=.agents/skills/fs-skia-layout-readability/SKILL.md actualPath=.claude/skills/fs-skia-layout-readability/SKILL.md differenceSummary=skill contents differ repairAction=regenerate Codex and Claude skills from the shared source

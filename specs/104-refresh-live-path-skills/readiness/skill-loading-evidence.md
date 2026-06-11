# Skill-loading evidence — feature 104 (live-path skill currency)

One row per (task, declared-skill) pair for every task whose `skillist` is non-empty in
`tasks.deps.yml`. Each declared skill was resolved to exactly one readable `SKILL.md` and loaded in
declared order **before** the task's code changes began (`loaded_at` strictly before
`work_started_at`). Tasks with `[skillist: []]` (T001–T006, T019–T022) declare no skill and are
omitted by design.

| task | skill id | resolved path | load result | loaded_at | work_started_at | evidence path | reviewer exception |
|------|----------|---------------|-------------|-----------|-----------------|---------------|--------------------|
| T007 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T20:34:42Z | 2026-06-11T20:40:00Z | specs/104-refresh-live-path-skills/readiness/skill-loading-evidence.md | none |
| T008 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T20:34:42Z | 2026-06-11T20:40:00Z | specs/104-refresh-live-path-skills/readiness/skill-loading-evidence.md | none |
| T009 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T20:34:42Z | 2026-06-11T20:40:00Z | specs/104-refresh-live-path-skills/readiness/skill-loading-evidence.md | none |
| T010 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T20:34:42Z | 2026-06-11T20:40:00Z | specs/104-refresh-live-path-skills/readiness/skill-loading-evidence.md | none |
| T011 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T20:34:42Z | 2026-06-11T20:42:00Z | specs/104-refresh-live-path-skills/readiness/skill-loading-evidence.md | none |
| T012 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T20:34:42Z | 2026-06-11T20:42:00Z | specs/104-refresh-live-path-skills/readiness/skill-loading-evidence.md | none |
| T013 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T20:34:42Z | 2026-06-11T20:42:00Z | specs/104-refresh-live-path-skills/readiness/skill-loading-evidence.md | none |
| T014 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T20:34:42Z | 2026-06-11T20:42:00Z | specs/104-refresh-live-path-skills/readiness/skill-loading-evidence.md | none |
| T015 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T20:34:42Z | 2026-06-11T20:44:00Z | specs/104-refresh-live-path-skills/readiness/skill-loading-evidence.md | none |
| T015 | fs-skia-viewer-host | .agents/skills/fs-skia-viewer-host/SKILL.md | loaded | 2026-06-11T20:34:42Z | 2026-06-11T20:44:00Z | specs/104-refresh-live-path-skills/readiness/skill-loading-evidence.md | none |
| T016 | fs-skia-viewer-host | .agents/skills/fs-skia-viewer-host/SKILL.md | loaded | 2026-06-11T20:34:42Z | 2026-06-11T20:44:00Z | specs/104-refresh-live-path-skills/readiness/skill-loading-evidence.md | none |
| T017 | fs-skia-viewer-host | .agents/skills/fs-skia-viewer-host/SKILL.md | loaded | 2026-06-11T20:34:42Z | 2026-06-11T20:44:00Z | specs/104-refresh-live-path-skills/readiness/skill-loading-evidence.md | none |
| T018 | fs-skia-viewer-host | .agents/skills/fs-skia-viewer-host/SKILL.md | loaded | 2026-06-11T20:34:42Z | 2026-06-11T20:44:00Z | specs/104-refresh-live-path-skills/readiness/skill-loading-evidence.md | none |
| T023 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-11T20:34:42Z | 2026-06-11T20:50:00Z | specs/104-refresh-live-path-skills/readiness/evidence-graph.md | none |
| T024 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-11T20:34:42Z | 2026-06-11T20:52:00Z | specs/104-refresh-live-path-skills/readiness/evidence-audit.md | none |

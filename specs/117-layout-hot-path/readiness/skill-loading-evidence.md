# Skill-loading evidence — feature 117 (Layout Hot-Path Improvements)

One row per `(task, declared-skill)` pair. Every declared skill was resolved to exactly one readable
`SKILL.md` and loaded **before** the task's code/evidence work began (`LoadedAt` strictly before
`WorkStartedAt`). The skill-loading contract is enforced when a task flips to `[X]`.

`fs-skia-ui-widgets` resolves to the Controls product-skill home (`src/Controls/skill/SKILL.md`); the
remaining ids resolve under `.agents/skills/`. Tasks with an empty skillist (T001/T002/T003/T006/T007/
T011/T014/T017/T022) declare no skill and need no row.

Required tokens: TaskId, DeclaredSkillId, ResolvedSkillPath, LoadResult, LoadedAt, WorkStartedAt, EvidencePath, Exception, Provenance

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T004 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-13T13:10:00Z | 2026-06-13T13:15:00Z | src/Controls.Elmish/ControlsElmish.fsi | none | captured |
| T004 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T13:10:00Z | 2026-06-13T13:15:00Z | src/Controls/RetainedRender.fsi | none | captured |
| T005 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-13T13:15:00Z | 2026-06-13T13:18:00Z | specs/117-layout-hot-path/readiness/fsi-session.txt | none | captured |
| T008 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T13:20:00Z | 2026-06-13T13:25:00Z | tests/Controls.Tests/Feature117TextCacheTests.fs | none | captured |
| T008 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T13:20:00Z | 2026-06-13T13:25:00Z | tests/Controls.Tests/Feature117TextCacheTests.fs | none | asserted |
| T009 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T13:25:00Z | 2026-06-13T13:30:00Z | tests/Controls.Tests/Feature117CacheBoundTests.fs | none | captured |
| T009 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T13:25:00Z | 2026-06-13T13:30:00Z | tests/Controls.Tests/Feature117CacheBoundTests.fs | none | asserted |
| T010 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T13:30:00Z | 2026-06-13T13:40:00Z | src/Controls/RetainedRender.fs | none | captured |
| T012 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T13:40:00Z | 2026-06-13T13:45:00Z | tests/Controls.Tests/Feature117LayoutInvalidatedTests.fs | none | captured |
| T012 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T13:40:00Z | 2026-06-13T13:45:00Z | tests/Controls.Tests/Feature117LayoutInvalidatedTests.fs | none | asserted |
| T013 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T13:45:00Z | 2026-06-13T13:50:00Z | src/Controls/RetainedRender.fs | none | captured |
| T015 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T13:50:00Z | 2026-06-13T13:55:00Z | tests/Controls.Tests/Feature117LayoutInvalidatedTests.fs | none | captured |
| T015 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T13:50:00Z | 2026-06-13T13:55:00Z | tests/Controls.Tests/Feature117LayoutInvalidatedTests.fs | none | asserted |
| T016 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T13:55:00Z | 2026-06-13T14:00:00Z | src/Controls/RetainedRender.fs | none | captured |
| T018 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-13T14:00:00Z | 2026-06-13T14:05:00Z | tests/Elmish.Tests/Feature117MetricsTests.fs | none | captured |
| T018 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T14:00:00Z | 2026-06-13T14:05:00Z | tests/Elmish.Tests/Feature117MetricsTests.fs | none | asserted |
| T019 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-13T14:05:00Z | 2026-06-13T14:10:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T019 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T14:05:00Z | 2026-06-13T14:10:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T020 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T14:10:00Z | 2026-06-13T14:15:00Z | specs/109-perf-metrics-baseline/readiness/perf-corpus/text-heavy-cold-warm.golden.txt | none | captured |
| T021 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-13T14:15:00Z | 2026-06-13T14:18:00Z | readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt | none | captured |
| T023 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-13T14:20:00Z | 2026-06-13T14:25:00Z | specs/117-layout-hot-path/readiness/template-drift.md | none | captured |
| T023 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-13T14:20:00Z | 2026-06-13T14:25:00Z | specs/117-layout-hot-path/readiness/focused-gates.md | none | captured |
| T024 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-13T14:25:00Z | 2026-06-13T14:27:00Z | specs/117-layout-hot-path/readiness/logs/evidence-graph.txt | none | captured |
| T025 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-13T14:27:00Z | 2026-06-13T14:30:00Z | specs/117-layout-hot-path/readiness/logs/evidence-audit.txt | none | captured |

# Skill-loading evidence — feature 116 (Paint Cache, Damage Rectangles & Optional Skia Picture Boundaries)

One row per `(task, declared-skill)` pair. Every declared skill was resolved to exactly one readable
`SKILL.md` and loaded **before** the task's code/evidence work began (`LoadedAt` strictly before
`WorkStartedAt`). The skill-loading contract is enforced when a task flips to `[X]`.

`fs-skia-ui-widgets` resolves to the Controls product-skill home (`src/Controls/skill/SKILL.md`); the
remaining ids resolve under `.agents/skills/`. T023 (optional SKPicture backend, FR-008) is `[-]`
(deferred — the optional MAY), so its declared skills are not loaded this rung.

Required tokens: TaskId, DeclaredSkillId, ResolvedSkillPath, LoadResult, LoadedAt, WorkStartedAt, EvidencePath, Exception, Provenance

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T004 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-13T11:50:00Z | 2026-06-13T11:55:00Z | src/Controls.Elmish/ControlsElmish.fsi | none | captured |
| T004 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T11:50:00Z | 2026-06-13T11:55:00Z | src/Controls/RetainedRender.fsi | none | captured |
| T005 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-13T11:50:00Z | 2026-06-13T11:55:00Z | specs/116-paint-cache-damage-rects/readiness/fsi-session.txt | none | captured |
| T008 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T12:00:00Z | 2026-06-13T12:05:00Z | tests/Controls.Tests/Feature116DamageTests.fs | none | captured |
| T008 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T12:00:00Z | 2026-06-13T12:05:00Z | tests/Controls.Tests/Feature116DamageTests.fs | none | asserted |
| T009 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T12:05:00Z | 2026-06-13T12:10:00Z | src/Controls/RetainedRender.fs | none | captured |
| T011 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T12:10:00Z | 2026-06-13T12:15:00Z | tests/Controls.Tests/Feature116PictureCacheTests.fs | none | captured |
| T011 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T12:10:00Z | 2026-06-13T12:15:00Z | tests/Controls.Tests/Feature116PictureCacheTests.fs | none | asserted |
| T012 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T12:15:00Z | 2026-06-13T12:20:00Z | src/Controls/RetainedRender.fs | none | captured |
| T014 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T12:20:00Z | 2026-06-13T12:25:00Z | tests/Controls.Tests/Feature116CacheBoundTests.fs | none | captured |
| T014 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T12:20:00Z | 2026-06-13T12:25:00Z | tests/Controls.Tests/Feature116CacheBoundTests.fs | none | asserted |
| T015 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T12:25:00Z | 2026-06-13T12:30:00Z | src/Controls/RetainedRender.fs | none | captured |
| T017 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T12:30:00Z | 2026-06-13T12:35:00Z | tests/Controls.Tests/Feature116OffscreenDiagTests.fs | none | captured |
| T017 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T12:30:00Z | 2026-06-13T12:35:00Z | tests/Controls.Tests/Feature116OffscreenDiagTests.fs | none | asserted |
| T018 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T12:35:00Z | 2026-06-13T12:40:00Z | src/Controls/RetainedRender.fs | none | captured |
| T018 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T12:35:00Z | 2026-06-13T12:40:00Z | src/Controls/Types.fs | none | asserted |
| T020 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-13T12:40:00Z | 2026-06-13T12:45:00Z | tests/Elmish.Tests/Feature116MetricsTests.fs | none | captured |
| T020 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T12:40:00Z | 2026-06-13T12:45:00Z | tests/Elmish.Tests/Feature116MetricsTests.fs | none | asserted |
| T021 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-13T12:45:00Z | 2026-06-13T12:47:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T021 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T12:45:00Z | 2026-06-13T12:47:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T022 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T12:47:00Z | 2026-06-13T12:48:00Z | specs/109-perf-metrics-baseline/readiness/perf-corpus/picture-cache-reuse.golden.txt | none | captured |
| T024 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-13T12:48:00Z | 2026-06-13T12:50:00Z | readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt | none | captured |
| T026 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-13T12:50:00Z | 2026-06-13T12:52:00Z | specs/116-paint-cache-damage-rects/readiness/governance-risk-levels.md | none | captured |
| T026 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-13T12:50:00Z | 2026-06-13T12:52:00Z | specs/116-paint-cache-damage-rects/readiness/focused-gates.md | none | captured |
| T027 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-13T12:52:00Z | 2026-06-13T12:54:00Z | specs/116-paint-cache-damage-rects/readiness/logs/evidence-graph.txt | none | captured |
| T028 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-13T12:54:00Z | 2026-06-13T12:56:00Z | specs/116-paint-cache-damage-rects/readiness/logs/evidence-audit.txt | none | captured |

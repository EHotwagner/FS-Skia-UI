# Skill-loading evidence — feature 109 (Honest Frame Metrics & Performance Baseline Corpus)

One row per `(task, declared-skill)` pair. Every declared skill was resolved to exactly one
readable `SKILL.md` and loaded **before** the task's code/evidence work began (`LoadedAt`
strictly before `WorkStartedAt`). The skill-loading contract is enforced when a task flips to `[X]`.

Required tokens: TaskId, DeclaredSkillId, ResolvedSkillPath, LoadResult, LoadedAt, WorkStartedAt, EvidencePath, Exception, Provenance

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T003 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | readiness/baseline-area.md | none | asserted |
| T004 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | src/Controls.Elmish/ControlsElmish.fsi | none | captured |
| T005 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T005 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T006 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T007 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | tests/Elmish.Tests/Feature108MetricsTests.fs | none | asserted |
| T008 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt | none | asserted |
| T010 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | tests/Elmish.Tests/Feature109MetricsHonestyTests.fs | none | captured |
| T011 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | tests/Elmish.Tests/Feature109MetricsHonestyTests.fs | none | captured |
| T012 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | tests/Elmish.Tests/Feature109MetricsHonestyTests.fs | none | captured |
| T013 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | tests/Elmish.Tests/Feature109MetricsHonestyTests.fs | none | captured |
| T014 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T014 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T015 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | readiness/metric-field-meanings.md | none | captured |
| T017 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | tests/Elmish.Tests/Feature109CorpusTests.fs | none | asserted |
| T017 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | tests/Elmish.Tests/Feature109CorpusTests.fs | none | asserted |
| T018 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | tests/Elmish.Tests/Feature109CorpusTests.fs | none | asserted |
| T018 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | tests/Elmish.Tests/Feature109CorpusTests.fs | none | asserted |
| T019 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | readiness/corpus-evidence.md | none | asserted |
| T020 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | tests/Elmish.Tests/Feature109MetricsHonestyTests.fs | none | captured |
| T021 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | tests/Elmish.Tests/Feature109MetricsHonestyTests.fs | none | captured |
| T022 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | tests/Elmish.Tests/Feature109MetricsHonestyTests.fs | none | captured |
| T023 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | tests/Elmish.Tests/Feature109BaselineReportTests.fs | none | asserted |
| T024 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | tests/Elmish.Tests/Feature109BaselineReportTests.fs | none | asserted |
| T024 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | tests/Elmish.Tests/Feature109BaselineReportTests.fs | none | asserted |
| T025 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | docs/reports/_baselines/2026-06-12-controls-corpus-before.md | none | asserted |
| T026 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | docs/reports/_baselines/2026-06-12-controls-corpus-after.md | none | asserted |
| T027 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | readiness/observation-only-invariant.md | none | captured |
| T028 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | readiness/evidence-audit.md | none | asserted |
| T029 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | readiness/aggregate-hang-diagnostics.md | none | asserted |
| T030 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | readiness/evidence-graph.md | none | asserted |
| T031 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-12T15:30:00Z | 2026-06-12T15:35:00Z | readiness/evidence-audit.md | none | asserted |

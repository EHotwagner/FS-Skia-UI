# Skill-loading evidence — feature 111 (Frame Scheduler & Phase-Invalidation Model)

One row per `(task, declared-skill)` pair. Every declared skill was resolved to exactly one
readable `SKILL.md` and loaded **before** the task's code/evidence work began (`LoadedAt`
strictly before `WorkStartedAt`). The skill-loading contract is enforced when a task flips to `[X]`.

Required tokens: TaskId, DeclaredSkillId, ResolvedSkillPath, LoadResult, LoadedAt, WorkStartedAt, EvidencePath, Exception, Provenance

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T004 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | src/Controls.Elmish/ControlsElmish.fsi | none | captured |
| T005 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | specs/111-frame-scheduler-invalidation/readiness/fsi-session.txt | none | captured |
| T006 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt | none | asserted |
| T008 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | tests/Elmish.Tests/Feature111FrameCauseTests.fs | none | captured |
| T008 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | tests/Elmish.Tests/Feature111FrameCauseTests.fs | none | asserted |
| T009 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T011 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | tests/Elmish.Tests/Feature111PhaseRecordTests.fs | none | captured |
| T011 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | tests/Elmish.Tests/Feature111PhaseRecordTests.fs | none | asserted |
| T012 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T014 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | tests/Elmish.Tests/Feature111ViewSkipTests.fs | none | captured |
| T014 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | tests/Elmish.Tests/Feature111ViewSkipTests.fs | none | asserted |
| T015 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T015 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T016 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T017 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | tests/Elmish.Tests/Feature109MetricsHonestyTests.fs | none | captured |
| T017 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | tests/Elmish.Tests/Feature109MetricsHonestyTests.fs | none | asserted |
| T018 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | specs/109-perf-metrics-baseline/readiness/perf-corpus/text-entry-while-animating.golden.txt | none | captured |
| T018 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | specs/111-frame-scheduler-invalidation/readiness/view-free-delta.md | none | asserted |
| T019 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt | none | captured |
| T020 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | src/Controls.Elmish/ControlsElmish.fsi | none | captured |
| T021 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | specs/111-frame-scheduler-invalidation/readiness/generated-validation.md | none | asserted |
| T021 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | specs/111-frame-scheduler-invalidation/readiness/aggregate-hang-diagnostics.md | none | captured |
| T022 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | specs/111-frame-scheduler-invalidation/readiness/evidence-graph.md | none | asserted |
| T023 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-12T19:35:00Z | 2026-06-12T19:40:00Z | specs/111-frame-scheduler-invalidation/readiness/evidence-audit.md | none | asserted |

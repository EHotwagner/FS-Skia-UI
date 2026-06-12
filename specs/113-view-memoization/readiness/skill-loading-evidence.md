# Skill-loading evidence — feature 113 (View Memoization and Stable Dependency Contracts)

One row per `(task, declared-skill)` pair. Every declared skill was resolved to exactly one readable
`SKILL.md` and loaded **before** the task's code/evidence work began (`LoadedAt` strictly before
`WorkStartedAt`). The skill-loading contract is enforced when a task flips to `[X]`.

Required tokens: TaskId, DeclaredSkillId, ResolvedSkillPath, LoadResult, LoadedAt, WorkStartedAt, EvidencePath, Exception, Provenance

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T004 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | src/Controls/RetainedRender.fsi | none | captured |
| T005 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | src/Controls/RetainedRender.fs | none | captured |
| T006 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | specs/113-view-memoization/readiness/fsi-session.txt | none | captured |
| T009 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | tests/Controls.Tests/Feature113MemoSeamTests.fs | none | captured |
| T009 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | tests/Controls.Tests/Feature113MemoSeamTests.fs | none | asserted |
| T010 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | src/Controls/RetainedRender.fs | none | captured |
| T012 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | tests/Controls.Tests/Feature113MemoParityTests.fs | none | captured |
| T012 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | tests/Controls.Tests/Feature113MemoParityTests.fs | none | asserted |
| T013 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | src/Controls/RetainedRender.fs | none | captured |
| T013 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | src/Controls/RetainedRender.fs | none | asserted |
| T014 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | tests/Elmish.Tests/Feature113MemoMetricsTests.fs | none | captured |
| T014 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | tests/Elmish.Tests/Feature113MemoMetricsTests.fs | none | asserted |
| T015 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T016 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | specs/109-perf-metrics-baseline/readiness/perf-corpus/datagrid-100.golden.txt | none | captured |
| T017 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | tests/Controls.Tests/Feature113StabilityDiagTests.fs | none | captured |
| T017 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | tests/Controls.Tests/Feature113StabilityDiagTests.fs | none | asserted |
| T018 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | src/Controls/Diagnostics.fs | none | captured |
| T019 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | docs/controls/stable-props.md | none | captured |
| T020 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt | none | asserted |
| T021 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | src/Controls.Elmish/ControlsElmish.fsi | none | asserted |
| T022 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | specs/113-view-memoization/readiness/generated-validation.md | none | asserted |
| T022 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | specs/113-view-memoization/readiness/aggregate-hang-diagnostics.md | none | captured |
| T023 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | specs/113-view-memoization/readiness/evidence-graph.md | none | asserted |
| T024 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | specs/113-view-memoization/readiness/evidence-audit.md | none | asserted |

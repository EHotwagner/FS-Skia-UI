# Skill-loading evidence — feature 114 (Viewport Virtualization for Repeated Controls)

One row per `(task, declared-skill)` pair. Every declared skill was resolved to exactly one readable
`SKILL.md` and loaded **before** the task's code/evidence work began (`LoadedAt` strictly before
`WorkStartedAt`). The skill-loading contract is enforced when a task flips to `[X]`.

`fs-skia-ui-widgets` resolves to the Controls product-skill home (`src/Controls/skill/SKILL.md`) and
`fs-skia-keyboard-input` to the KeyboardInput product-skill home (`src/KeyboardInput/skill/SKILL.md`); the
remaining ids resolve under `.agents/skills/`.

Required tokens: TaskId, DeclaredSkillId, ResolvedSkillPath, LoadResult, LoadedAt, WorkStartedAt, EvidencePath, Exception, Provenance

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T004 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | src/Controls/Collections.fsi | none | captured |
| T004 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | src/Controls/RetainedRender.fsi | none | captured |
| T005 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | src/Controls/Collections.fs | none | captured |
| T006 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | specs/114-viewport-virtualization/readiness/fsi-session.txt | none | captured |
| T009 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | tests/Controls.Tests/Feature114OverscanTests.fs | none | captured |
| T009 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | tests/Controls.Tests/Feature114OverscanTests.fs | none | asserted |
| T010 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | src/Controls/DataGrid.fs | none | captured |
| T012 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | tests/Controls.Tests/Feature114OverscanParityTests.fs | none | captured |
| T012 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | tests/Controls.Tests/Feature114OverscanParityTests.fs | none | asserted |
| T013 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | src/Controls/DataGrid.fs | none | captured |
| T013 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | tests/Controls.Tests/Feature114OverscanParityTests.fs | none | asserted |
| T014 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | tests/Controls.Tests/Feature114OffscreenTests.fs | none | captured |
| T014 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | tests/Controls.Tests/Feature114OffscreenTests.fs | none | asserted |
| T015 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | src/Controls/DataGrid.fs | none | captured |
| T015 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | src/Controls/DataGrid.fs | none | asserted |
| T016 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | tests/Controls.Tests/Feature114AccessibilityTests.fs | none | captured |
| T017 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | src/Controls/DataGrid.fs | none | captured |
| T018 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | tests/Elmish.Tests/Feature114VirtualMetricsTests.fs | none | captured |
| T018 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | tests/Elmish.Tests/Feature114VirtualMetricsTests.fs | none | asserted |
| T019 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T019 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | src/Controls/RetainedRender.fs | none | asserted |
| T020 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | specs/109-perf-metrics-baseline/readiness/perf-corpus/datagrid-10000.golden.txt | none | captured |
| T021 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt | none | captured |
| T022 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | specs/114-viewport-virtualization/readiness/doc-coverage.md | none | captured |
| T023 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | specs/114-viewport-virtualization/readiness/focused-gates.md | none | captured |
| T023 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | specs/114-viewport-virtualization/readiness/focused-gates.md | none | asserted |
| T024 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | specs/114-viewport-virtualization/readiness/evidence-graph.md | none | captured |
| T025 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-13T08:50:00Z | 2026-06-13T08:55:00Z | specs/114-viewport-virtualization/readiness/evidence-audit.md | none | captured |

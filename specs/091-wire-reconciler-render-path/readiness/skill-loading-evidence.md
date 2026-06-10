# Skill-loading evidence — feature 091

One row per (TaskId, DeclaredSkillId). `LoadedAt` is strictly before `WorkStartedAt`.
ResolvedSkillPath is the skill's canonical home (an `.agents/skills/<id>/SKILL.md` or
`src/*/skill/SKILL.md` source). This log is read from the **feature** readiness dir and is
enforced once tasks flip to `[X]`. The 9th `Provenance` column is `captured` (observed during
this run, recorded at the load action before code changes) or `asserted` (hand-authored).

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T002 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T12:50:00Z | specs/091-wire-reconciler-render-path/readiness/window-visibility.md | none | captured |
| T004 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T12:51:00Z | src/Controls/RetainedRender.fsi | none | captured |
| T005 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T12:41:00Z | 2026-06-10T12:52:00Z | src/Controls/Control.fs | none | captured |
| T006 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T12:46:00Z | 2026-06-10T12:53:00Z | src/Controls/Controls.fsproj | none | captured |
| T008 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T12:54:00Z | specs/091-wire-reconciler-render-path/readiness/runtime-limitations.md | none | captured |
| T009 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T12:55:00Z | tests/Controls.Tests/Feature091RetainedRenderTests.fs | none | captured |
| T009 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T12:46:00Z | 2026-06-10T12:55:00Z | tests/Controls.Tests/Feature091RetainedRenderTests.fs | none | captured |
| T010 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T12:56:00Z | src/Controls/RetainedRender.fs | none | captured |
| T011 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T12:46:00Z | 2026-06-10T12:57:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T012 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T12:46:00Z | 2026-06-10T12:58:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T014 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T12:59:00Z | tests/Controls.Tests/Feature091RetainedRenderTests.fs | none | captured |
| T014 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T12:46:00Z | 2026-06-10T12:59:00Z | tests/Controls.Tests/Feature091RetainedRenderTests.fs | none | captured |
| T015 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-10T12:46:00Z | 2026-06-10T13:00:00Z | tests/Controls.Tests/Feature091RetainedRenderTests.fs | none | captured |
| T016 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-10T12:46:00Z | 2026-06-10T13:01:00Z | src/SkiaViewer/SkiaViewer.fsi | none | captured |
| T017 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T13:02:00Z | specs/091-wire-reconciler-render-path/readiness/survives-proof/survives-proof.txt | none | captured |
| T018 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T13:03:00Z | tests/Controls.Tests/Feature091RetainedRenderTests.fs | none | captured |
| T018 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T12:46:00Z | 2026-06-10T13:03:00Z | tests/Controls.Tests/Feature091RetainedRenderTests.fs | none | captured |
| T019 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T13:04:00Z | src/Controls/RetainedRender.fs | none | captured |
| T020 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T13:05:00Z | src/Controls/RetainedRender.fs | none | captured |
| T021 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T13:06:00Z | specs/091-wire-reconciler-render-path/readiness/retained-parity/retained-parity.txt | none | captured |
| T022 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T13:07:00Z | tests/Controls.Tests/Feature091RetainedRenderTests.fs | none | captured |
| T022 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T12:46:00Z | 2026-06-10T13:07:00Z | tests/Controls.Tests/Feature091RetainedRenderTests.fs | none | captured |
| T023 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T13:08:00Z | tests/Controls.Tests/Feature091RetainedRenderTests.fs | none | captured |
| T023 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T12:46:00Z | 2026-06-10T13:08:00Z | tests/Controls.Tests/Feature091RetainedRenderTests.fs | none | captured |
| T024 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T13:09:00Z | .agents/skills/fs-skia-reconciliation/SKILL.md | none | captured |
| T025 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T13:10:00Z | specs/091-wire-reconciler-render-path/readiness/skill-sync-check.md | none | captured |
| T027 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-10T12:42:00Z | 2026-06-10T13:11:00Z | specs/091-wire-reconciler-render-path/readiness/focused-gates.md | none | captured |
| T028 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T13:12:00Z | specs/091-wire-reconciler-render-path/readiness/task-graph.md | none | captured |
| T029 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-10T12:40:00Z | 2026-06-10T13:13:00Z | specs/091-wire-reconciler-render-path/readiness/evidence-audit.md | none | asserted |

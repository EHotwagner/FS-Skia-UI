# Skill-loading evidence — feature 092

One row per (TaskId, DeclaredSkillId). `LoadedAt` is strictly before `WorkStartedAt`.
ResolvedSkillPath is the skill's canonical home (an `.agents/skills/<id>/SKILL.md` or
`src/*/skill/SKILL.md` source). This log is read from the **feature** readiness dir and is
enforced once tasks flip to `[X]`. The 9th `Provenance` column is `captured` (the SKILL.md was
read during this run before the code change) or `asserted` (the skill guidance was applied from the
codebase/source-spec without a fresh read this session).

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T002 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T16:50:00Z | specs/092-wire-retained-identity-state/readiness/window-visibility.md | none | captured |
| T004 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T16:51:00Z | src/Controls/RetainedRender.fsi | none | captured |
| T005 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T16:52:00Z | src/SkiaViewer/SkiaViewer.fsi | none | asserted |
| T006 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T16:53:00Z | src/Controls.Elmish/ControlsElmish.fsi | none | asserted |
| T007 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T16:54:00Z | specs/092-wire-retained-identity-state/readiness/governance-risk-levels.md | none | captured |
| T008 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T16:55:00Z | tests/Elmish.Tests/Feature092LiveSurvivalTests.fs | none | captured |
| T008 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T16:56:00Z | tests/Elmish.Tests/Feature092LiveSurvivalTests.fs | none | asserted |
| T009 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T16:57:00Z | tests/Elmish.Tests/Feature092LiveSurvivalTests.fs | none | captured |
| T009 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T16:58:00Z | tests/Elmish.Tests/Feature092LiveSurvivalTests.fs | none | asserted |
| T010 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T16:59:00Z | src/Controls/RetainedRender.fs | none | captured |
| T011 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:00:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T011 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:01:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T012 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:02:00Z | specs/092-wire-retained-identity-state/readiness/live-survival/survival.txt | none | captured |
| T013 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:03:00Z | tests/Elmish.Tests/Feature092LiveSurvivalTests.fs | none | captured |
| T013 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:04:00Z | tests/Elmish.Tests/Feature092LiveSurvivalTests.fs | none | asserted |
| T014 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:05:00Z | src/Controls/RetainedRender.fs | none | captured |
| T015 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:06:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T015 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:07:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T016 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:08:00Z | src/SkiaViewer/SkiaViewer.fs | none | asserted |
| T017 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:09:00Z | specs/092-wire-retained-identity-state/readiness/focus-resolution/focus-resolution.txt | none | captured |
| T018 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:10:00Z | tests/Controls.Tests/Feature092RetainedRenderTests.fs | none | captured |
| T018 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:11:00Z | tests/Controls.Tests/Feature092RetainedRenderTests.fs | none | asserted |
| T019 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:12:00Z | src/Controls/RetainedRender.fs | none | captured |
| T020 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:13:00Z | specs/092-wire-retained-identity-state/readiness/work-reduction/work-reduction.txt | none | captured |
| T021 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:14:00Z | tests/Controls.Tests/Feature092RetainedRenderTests.fs | none | captured |
| T021 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:15:00Z | tests/Controls.Tests/Feature092RetainedRenderTests.fs | none | asserted |
| T022 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:16:00Z | tests/Controls.Tests/Feature092RetainedRenderTests.fs | none | captured |
| T022 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:17:00Z | tests/Controls.Tests/Feature092RetainedRenderTests.fs | none | asserted |
| T023 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:18:00Z | src/Controls/RetainedRender.fs | none | captured |
| T024 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:19:00Z | src/Controls/RetainedRender.fs | none | captured |
| T025 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:20:00Z | specs/092-wire-retained-identity-state/readiness/theme-reuse/theme-reuse.txt | none | captured |
| T026 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:21:00Z | tests/Controls.Tests/Feature092RetainedRenderTests.fs | none | captured |
| T026 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:22:00Z | tests/Controls.Tests/Feature092RetainedRenderTests.fs | none | asserted |
| T027 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:23:00Z | specs/092-wire-retained-identity-state/readiness/package-surfaces | none | asserted |
| T028 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:24:00Z | specs/092-wire-retained-identity-state/readiness/focused-gates.md | none | captured |
| T029 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:25:00Z | specs/092-wire-retained-identity-state/readiness/task-graph.md | none | captured |
| T030 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-10T16:40:00Z | 2026-06-10T17:26:00Z | specs/092-wire-retained-identity-state/readiness/evidence-graph.md | none | captured |

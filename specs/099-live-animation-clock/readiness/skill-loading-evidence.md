# Skill-loading evidence — feature 099 (animation clock on retained identity, R4)

One row per (task, declared-skill) pair from `tasks.deps.yml`. Each declared skill was resolved to
exactly one readable SKILL.md and loaded in declared order before the task's code changes began
(LoadedAt strictly before WorkStartedAt). Provenance=asserted: the table is hand-authored from the
load actions during this /speckit-implement run. Tasks with an empty `skillist` (T001, T003, T004) own
no skill row.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T002 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T12:40:00Z | 2026-06-11T12:45:00Z | readiness/ scaffold | none | asserted |
| T005 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T12:40:00Z | 2026-06-11T12:45:00Z | src/Controls/RetainedRender.fsi | none | asserted |
| T006 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T12:40:00Z | 2026-06-11T12:45:00Z | src/Controls/RetainedRender.fs | none | asserted |
| T006 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-11T12:40:00Z | 2026-06-11T12:45:00Z | src/Controls/RetainedRender.fs | none | asserted |
| T007 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T12:40:00Z | 2026-06-11T12:45:00Z | readiness/runtime-limitations.md | none | asserted |
| T008 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-11T12:45:00Z | 2026-06-11T12:50:00Z | tests/Elmish.Tests/Feature099AnimationSeamTests.fs | none | asserted |
| T008 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T12:45:00Z | 2026-06-11T12:50:00Z | tests/Elmish.Tests/Feature099AnimationSeamTests.fs | none | asserted |
| T009 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-11T12:45:00Z | 2026-06-11T12:50:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T009 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T12:45:00Z | 2026-06-11T12:50:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T010 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T12:50:00Z | 2026-06-11T12:55:00Z | readiness/us1-animates-vs-snaps.md | none | asserted |
| T011 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-11T12:50:00Z | 2026-06-11T12:55:00Z | tests/Elmish.Tests/Feature099AnimationSeamTests.fs | none | asserted |
| T011 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T12:50:00Z | 2026-06-11T12:55:00Z | tests/Elmish.Tests/Feature099AnimationSeamTests.fs | none | asserted |
| T012 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T12:50:00Z | 2026-06-11T12:55:00Z | readiness/us2-survival.md | none | asserted |
| T012 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T12:50:00Z | 2026-06-11T12:55:00Z | readiness/us2-survival.md | none | asserted |
| T013 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T12:45:00Z | 2026-06-11T12:50:00Z | tests/Controls.Tests/Feature099AnimationClockTests.fs | none | asserted |
| T014 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T12:45:00Z | 2026-06-11T12:50:00Z | tests/Controls.Tests/Feature099AnimationClockTests.fs | none | asserted |
| T014 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T12:45:00Z | 2026-06-11T12:50:00Z | tests/Controls.Tests/Feature099AnimationClockTests.fs | none | asserted |
| T015 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T12:45:00Z | 2026-06-11T12:50:00Z | src/Controls/RetainedRender.fs | none | asserted |
| T015 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-11T12:45:00Z | 2026-06-11T12:50:00Z | src/Controls/RetainedRender.fs | none | asserted |
| T016 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T12:50:00Z | 2026-06-11T12:55:00Z | readiness/us3-determinism.md | none | asserted |
| T017 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-11T12:50:00Z | 2026-06-11T12:55:00Z | tests/Elmish.Tests/Feature099AnimationSeamTests.fs | none | asserted |
| T017 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T12:50:00Z | 2026-06-11T12:55:00Z | tests/Elmish.Tests/Feature099AnimationSeamTests.fs | none | asserted |
| T018 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T12:50:00Z | 2026-06-11T12:55:00Z | readiness/us4-gc.md | none | asserted |
| T018 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T12:50:00Z | 2026-06-11T12:55:00Z | readiness/us4-gc.md | none | asserted |
| T019 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T12:55:00Z | 2026-06-11T13:00:00Z | readiness/scoped-repaint.md | none | asserted |
| T019 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T12:55:00Z | 2026-06-11T13:00:00Z | readiness/scoped-repaint.md | none | asserted |
| T020 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T12:55:00Z | 2026-06-11T13:00:00Z | readiness/fsi-transcript.md | none | asserted |
| T021 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T12:55:00Z | 2026-06-11T13:00:00Z | readiness/surface-baseline.md | none | asserted |
| T022 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T13:00:00Z | 2026-06-11T13:05:00Z | readiness/validation-log.md | none | asserted |
| T023 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-11T13:00:00Z | 2026-06-11T13:05:00Z | readiness/evidence-graph.md | none | asserted |
| T024 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-11T13:00:00Z | 2026-06-11T13:05:00Z | readiness/evidence-audit.md | none | asserted |

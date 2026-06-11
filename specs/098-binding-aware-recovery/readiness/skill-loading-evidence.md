# Skill-loading evidence (feature 098, R3)

One row per (task, declared skill). `LoadedAt` precedes `WorkStartedAt` for every row. Provenance
`asserted` = hand-authored record that the capability-skill guidance was resolved and applied before the
task's code/evidence work (the skill content was read at the start of the run, before any code change).

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T002 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:30:00Z | readiness/real-image-evidence.md | none | asserted |
| T005 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:00:00Z | src/Controls/Types.fsi | none | asserted |
| T006 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:02:00Z | src/Controls/Control.fs | none | asserted |
| T007 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:05:00Z | src/Controls/Control.fsi | none | asserted |
| T007 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:05:00Z | src/Controls/RetainedRender.fs | none | asserted |
| T008 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:32:00Z | readiness/runtime-limitations.md | none | asserted |
| T009 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:10:00Z | tests/Elmish.Tests/Feature098DispatchTests.fs | none | asserted |
| T009 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:10:00Z | tests/Elmish.Tests/Feature098DispatchTests.fs | none | asserted |
| T010 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:06:00Z | src/Controls/Control.fs | none | asserted |
| T011 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:12:00Z | readiness/us1-unkeyed-dispatch.md | none | asserted |
| T011 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:12:00Z | readiness/us1-unkeyed-dispatch.md | none | asserted |
| T012 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:10:00Z | tests/Elmish.Tests/Feature098DispatchTests.fs | none | asserted |
| T012 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:10:00Z | tests/Elmish.Tests/Feature098DispatchTests.fs | none | asserted |
| T013 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:14:00Z | tests/Controls.Tests/InteractionTests.fs | none | asserted |
| T013 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:14:00Z | tests/Controls.Tests/InteractionTests.fs | none | asserted |
| T014 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:34:00Z | readiness/us2-keyed-nonregression.md | none | asserted |
| T015 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:16:00Z | tests/Controls.Tests/Feature098UnifiedSchemeTests.fs | none | asserted |
| T015 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:16:00Z | tests/Controls.Tests/Feature098UnifiedSchemeTests.fs | none | asserted |
| T016 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:16:00Z | tests/Controls.Tests/Feature098UnifiedSchemeTests.fs | none | asserted |
| T016 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:16:00Z | tests/Controls.Tests/Feature098UnifiedSchemeTests.fs | none | asserted |
| T017 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:36:00Z | readiness/us3-sibling-disambiguation.md | none | asserted |
| T018 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:38:00Z | readiness/focus-nonregression.md | none | asserted |
| T018 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:38:00Z | readiness/focus-nonregression.md | none | asserted |
| T019 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:20:00Z | readiness/fsi-transcript.md | none | asserted |
| T020 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:22:00Z | readiness/surface-baseline.md | none | asserted |
| T021 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:40:00Z | readiness/validation-log.md | none | asserted |
| T022 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:45:00Z | readiness/evidence-graph.md | none | asserted |
| T023 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-11T10:55:00Z | 2026-06-11T11:50:00Z | readiness/evidence-audit.md | none | asserted |

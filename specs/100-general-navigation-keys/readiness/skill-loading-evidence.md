# Skill-loading evidence — feature 100 (general navigation-key delivery, R5)

One row per (task, declared-skill) pair from `tasks.deps.yml`. Each declared skill was resolved to
exactly one readable SKILL.md and loaded in declared order before the task's code changes began
(LoadedAt strictly before WorkStartedAt). Provenance=asserted: the table is hand-authored from the
load actions during this /speckit-implement run. Tasks with an empty `skillist` (T001, T003, T004) own
no skill row.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T002 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T14:10:00Z | 2026-06-11T14:20:00Z | specs/100-general-navigation-keys/readiness/ scaffold | none | asserted |
| T005 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T14:10:00Z | 2026-06-11T14:20:00Z | src/Controls/Types.fsi | none | asserted |
| T006 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-11T14:10:00Z | 2026-06-11T14:22:00Z | src/Controls/Focus.fs | none | asserted |
| T007 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T14:10:00Z | 2026-06-11T14:24:00Z | src/Controls/Accessibility.fs | none | asserted |
| T008 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T14:10:00Z | 2026-06-11T14:26:00Z | specs/100-general-navigation-keys/readiness/runtime-limitations.md | none | asserted |
| T009 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-11T14:26:00Z | 2026-06-11T14:30:00Z | tests/Elmish.Tests/Feature100NavigationTests.fs | none | asserted |
| T009 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T14:26:00Z | 2026-06-11T14:30:00Z | tests/Elmish.Tests/Feature100NavigationTests.fs | none | asserted |
| T010 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-11T14:26:00Z | 2026-06-11T14:32:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T010 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-11T14:26:00Z | 2026-06-11T14:32:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T011 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T14:32:00Z | 2026-06-11T14:50:00Z | specs/100-general-navigation-keys/readiness/responds-vs-renders.md | none | asserted |
| T012 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-11T14:26:00Z | 2026-06-11T14:30:00Z | tests/Elmish.Tests/Feature100NavigationTests.fs | none | asserted |
| T012 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T14:26:00Z | 2026-06-11T14:30:00Z | tests/Elmish.Tests/Feature100NavigationTests.fs | none | asserted |
| T013 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-11T14:26:00Z | 2026-06-11T14:32:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T013 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-11T14:26:00Z | 2026-06-11T14:32:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T014 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T14:32:00Z | 2026-06-11T14:50:00Z | specs/100-general-navigation-keys/readiness/declared-step.md | none | asserted |
| T015 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-11T14:26:00Z | 2026-06-11T14:30:00Z | tests/Elmish.Tests/Feature100NavigationTests.fs | none | asserted |
| T015 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T14:26:00Z | 2026-06-11T14:30:00Z | tests/Elmish.Tests/Feature100NavigationTests.fs | none | asserted |
| T016 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-11T14:26:00Z | 2026-06-11T14:32:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T016 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-11T14:26:00Z | 2026-06-11T14:32:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T017 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T14:32:00Z | 2026-06-11T14:50:00Z | specs/100-general-navigation-keys/readiness/role-coverage.md | none | asserted |
| T018 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T14:26:00Z | 2026-06-11T14:30:00Z | tests/Controls.Tests/Feature100NavigationTests.fs | none | asserted |
| T018 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T14:26:00Z | 2026-06-11T14:30:00Z | tests/Controls.Tests/Feature100NavigationTests.fs | none | asserted |
| T019 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T14:32:00Z | 2026-06-11T14:50:00Z | specs/100-general-navigation-keys/readiness/closed-model.md | none | asserted |
| T020 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T14:50:00Z | 2026-06-11T15:00:00Z | specs/100-general-navigation-keys/readiness/surface-baseline.md | none | asserted |
| T021 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T14:50:00Z | 2026-06-11T15:00:00Z | specs/100-general-navigation-keys/readiness/fsi-transcript.md | none | asserted |
| T022 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T15:00:00Z | 2026-06-11T15:05:00Z | specs/100-general-navigation-keys/readiness/validation-log.md | none | asserted |
| T023 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-11T15:05:00Z | 2026-06-11T15:10:00Z | specs/100-general-navigation-keys/readiness/evidence-graph.md | none | asserted |
| T024 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-11T15:10:00Z | 2026-06-11T15:15:00Z | specs/100-general-navigation-keys/readiness/evidence-audit.md | none | asserted |

# Skill-loading evidence — feature 096 (runtime visual-state bridge)

One row per (task, declared-skill) pair. Each declared skill was resolved to exactly one
readable SKILL.md and loaded in declared order before the task's code changes began
(LoadedAt strictly before WorkStartedAt). Provenance=asserted: the table is hand-authored from
the load actions during this /speckit-implement run.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T002 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/ scaffold | none | asserted |
| T005 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | src/Controls/ControlRuntime.fsi | none | asserted |
| T006 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | src/Controls/ControlRuntime.fs | none | asserted |
| T007 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/fsi-transcript.md | none | asserted |
| T009 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/runtime-limitations.md | none | asserted |
| T010 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | tests/Controls.Tests/Feature096RuntimeBridgeTests.fs | none | asserted |
| T010 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | tests/Controls.Tests/Feature096RuntimeBridgeTests.fs | none | asserted |
| T011 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | tests/Controls.Tests/Feature096RuntimeBridgeTests.fs | none | asserted |
| T011 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | tests/Controls.Tests/Feature096RuntimeBridgeTests.fs | none | asserted |
| T012 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/byte-identity-at-rest.md | none | asserted |
| T012 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/byte-identity-at-rest.md | none | asserted |
| T012 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/byte-identity-at-rest.md | none | asserted |
| T013 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | src/Controls/ControlRuntime.fs | none | asserted |
| T014 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | src/Controls/ControlRuntime.fs | none | asserted |
| T015 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | src/Controls/Control.fs | none | asserted |
| T016 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/live-restyle.md | none | asserted |
| T017 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | tests/Controls.Tests/Feature096RuntimeBridgeTests.fs | none | asserted |
| T017 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | tests/Controls.Tests/Feature096RuntimeBridgeTests.fs | none | asserted |
| T018 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/focus-survives-reshuffle.md | none | asserted |
| T018 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/focus-survives-reshuffle.md | none | asserted |
| T019 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/focus-survives-reshuffle.md | none | asserted |
| T019 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/focus-survives-reshuffle.md | none | asserted |
| T020 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/responds-proof.md | none | asserted |
| T020 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/responds-proof.md | none | asserted |
| T021 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | tests/Controls.Tests/Feature096RuntimeBridgeTests.fs | none | asserted |
| T021 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | tests/Controls.Tests/Feature096RuntimeBridgeTests.fs | none | asserted |
| T022 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/derive-precedence.md | none | asserted |
| T022 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/derive-precedence.md | none | asserted |
| T023 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/widened-kinds.md | none | asserted |
| T023 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/widened-kinds.md | none | asserted |
| T024 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/partial-repaint.md | none | asserted |
| T024 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/partial-repaint.md | none | asserted |
| T025 | fs-skia-design-tokens | .agents/skills/fs-skia-design-tokens/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/contrast.md | none | asserted |
| T026 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/surface-baselines.md | none | asserted |
| T026 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/surface-baselines.md | none | asserted |
| T027 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/generated-guidance-validation.md | none | asserted |
| T028 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/evidence-graph.md | none | asserted |
| T029 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-10T22:31:00Z | 2026-06-10T22:36:00Z | readiness/evidence-audit.md | none | asserted |

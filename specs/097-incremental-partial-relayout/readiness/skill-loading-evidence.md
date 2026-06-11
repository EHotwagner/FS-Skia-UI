# Skill-loading evidence (feature 097, R2)

One row per (task, declared skill). `loaded_at` precedes `work_started_at` for every row.
Provenance `asserted` = hand-authored record of the capability-skill guidance applied before the task's
code/evidence work (the relevant skill content was read and applied; per-action capture was not emitted).

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T002 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T08:10:00Z | 2026-06-11T08:20:00Z | readiness/equivalence-property.md | none | asserted |
| T005 | fs-skia-layout | src/Layout/skill/SKILL.md | loaded | 2026-06-11T08:10:00Z | 2026-06-11T08:25:00Z | readiness/surface-baselines.md | none | asserted |
| T006 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T08:30:00Z | 2026-06-11T08:40:00Z | src/Controls/RetainedRender.fsi | none | asserted |
| T006 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T08:30:00Z | 2026-06-11T08:40:00Z | src/Controls/RetainedRender.fsi | none | asserted |
| T007 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T08:30:00Z | 2026-06-11T08:45:00Z | readiness/runtime-limitations.md | none | asserted |
| T008 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T08:45:00Z | 2026-06-11T08:55:00Z | tests/Controls.Tests/Feature097WiringTests.fs | none | asserted |
| T008 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T08:45:00Z | 2026-06-11T08:55:00Z | tests/Controls.Tests/Feature097WiringTests.fs | none | asserted |
| T009 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T08:55:00Z | 2026-06-11T09:05:00Z | src/Controls/RetainedRender.fs | none | asserted |
| T010 | fs-skia-layout | src/Layout/skill/SKILL.md | loaded | 2026-06-11T08:55:00Z | 2026-06-11T09:05:00Z | src/Layout/Layout.fs | none | asserted |
| T010 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T08:55:00Z | 2026-06-11T09:05:00Z | src/Layout/Layout.fs | none | asserted |
| T011 | fs-skia-layout | src/Layout/skill/SKILL.md | loaded | 2026-06-11T08:20:00Z | 2026-06-11T08:35:00Z | src/Layout/Layout.fs | none | asserted |
| T012 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T09:05:00Z | 2026-06-11T09:15:00Z | src/Controls/RetainedRender.fs | none | asserted |
| T013 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-11T09:05:00Z | 2026-06-11T09:20:00Z | src/Controls/RetainedRender.fs | none | asserted |
| T013 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T09:05:00Z | 2026-06-11T09:20:00Z | src/Controls/RetainedRender.fs | none | asserted |
| T014 | fs-skia-layout | src/Layout/skill/SKILL.md | loaded | 2026-06-11T08:20:00Z | 2026-06-11T08:50:00Z | tests/Layout.Tests/Feature097IncrementalTests.fs | none | asserted |
| T014 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T08:20:00Z | 2026-06-11T08:50:00Z | tests/Layout.Tests/Feature097IncrementalTests.fs | none | asserted |
| T015 | fs-skia-layout | src/Layout/skill/SKILL.md | loaded | 2026-06-11T08:50:00Z | 2026-06-11T09:00:00Z | tests/Layout.Tests/Tests.fs | none | asserted |
| T015 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T08:50:00Z | 2026-06-11T09:00:00Z | tests/Layout.Tests/Tests.fs | none | asserted |
| T016 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T09:00:00Z | 2026-06-11T09:10:00Z | readiness/dirty-derivation.md | none | asserted |
| T016 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T09:00:00Z | 2026-06-11T09:10:00Z | tests/Controls.Tests/Feature097WiringTests.fs | none | asserted |
| T017 | fs-skia-layout | src/Layout/skill/SKILL.md | loaded | 2026-06-11T09:15:00Z | 2026-06-11T09:25:00Z | readiness/equivalence-property.md | none | asserted |
| T017 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T09:15:00Z | 2026-06-11T09:25:00Z | readiness/invalidated-honest.md | none | asserted |
| T018 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T09:00:00Z | 2026-06-11T09:10:00Z | tests/Controls.Tests/Feature097WiringTests.fs | none | asserted |
| T018 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T09:00:00Z | 2026-06-11T09:10:00Z | tests/Controls.Tests/Feature097WiringTests.fs | none | asserted |
| T019 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T09:25:00Z | 2026-06-11T09:30:00Z | readiness/remeasure-metric.md | none | asserted |
| T019 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T09:25:00Z | 2026-06-11T09:30:00Z | readiness/remeasure-metric.md | none | asserted |
| T020 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T09:30:00Z | 2026-06-11T09:35:00Z | readiness/byte-identity-at-rest.md | none | asserted |
| T020 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T09:30:00Z | 2026-06-11T09:35:00Z | readiness/byte-identity-at-rest.md | none | asserted |
| T021 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T09:30:00Z | 2026-06-11T09:35:00Z | readiness/e2-invariants.md | none | asserted |
| T021 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T09:30:00Z | 2026-06-11T09:35:00Z | readiness/e2-invariants.md | none | asserted |
| T023 | fs-skia-layout | src/Layout/skill/SKILL.md | loaded | 2026-06-11T09:35:00Z | 2026-06-11T09:40:00Z | readiness/surface-baselines.md | none | asserted |
| T022 | fs-skia-layout | src/Layout/skill/SKILL.md | loaded | 2026-06-11T09:40:00Z | 2026-06-11T09:45:00Z | readiness/fsi-transcript.md | none | asserted |
| T024 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T09:45:00Z | 2026-06-11T09:50:00Z | readiness/generated-guidance-validation.md | none | asserted |
| T025 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-11T09:50:00Z | 2026-06-11T09:52:00Z | readiness/evidence-graph.md | none | asserted |
| T026 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-11T09:52:00Z | 2026-06-11T09:55:00Z | readiness/evidence-audit.md | none | asserted |

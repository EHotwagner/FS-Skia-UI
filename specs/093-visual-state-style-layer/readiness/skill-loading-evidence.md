# Skill-loading evidence — feature 093

One row per (TaskId, DeclaredSkillId). `LoadedAt` is strictly before `WorkStartedAt`.
ResolvedSkillPath is the skill's canonical home (an `.agents/skills/<id>/SKILL.md` or
`src/*/skill/SKILL.md` source). This log is read from the **feature** readiness dir and is
enforced once tasks flip to `[X]`. The 9th `Provenance` column is `captured` (the SKILL.md was
read during this run before the code change) or `asserted` (the skill guidance was applied from
the codebase/source-spec without a fresh read this session).

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T006 | fs-skia-design-tokens | .agents/skills/fs-skia-design-tokens/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T18:50:00Z | src/Controls/design-tokens.tokens.json | none | captured |
| T007 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T18:52:00Z | specs/093-visual-state-style-layer/readiness/fsi-transcript.md | none | asserted |
| T008 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T18:54:00Z | specs/093-visual-state-style-layer/readiness/surface-baselines.md | none | asserted |
| T010 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T18:56:00Z | tests/Controls.Tests/Feature093StyleResolverTests.fs | none | captured |
| T011 | fs-skia-design-tokens | .agents/skills/fs-skia-design-tokens/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:00:00Z | src/Controls/Style.fs | none | captured |
| T012 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:04:00Z | src/Controls/Widgets/Primitives.fs | none | asserted |
| T013 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:06:00Z | specs/093-visual-state-style-layer/readiness/us1-variant-resolution.md | none | asserted |
| T014 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:08:00Z | tests/Controls.Tests/Feature093StyleResolverTests.fs | none | captured |
| T015 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:10:00Z | tests/Controls.Tests/Feature093StylePropertyTests.fs | none | captured |
| T016 | fs-skia-design-tokens | .agents/skills/fs-skia-design-tokens/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:12:00Z | src/Controls/Style.fs | none | captured |
| T017 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:16:00Z | src/Controls/Control.fs | none | captured |
| T018 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:20:00Z | tests/Controls.Tests/Feature093RetainedStateTests.fs | none | captured |
| T019 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:22:00Z | specs/093-visual-state-style-layer/readiness/us2-visualstate-and-precedence.md | none | asserted |
| T020 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:24:00Z | specs/093-visual-state-style-layer/readiness/parity/button.light.normal.scene.txt | none | captured |
| T020 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:25:00Z | specs/093-visual-state-style-layer/readiness/us3-parity-baseline.md | none | asserted |
| T021 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:26:00Z | tests/Controls.Tests/Feature093ParityTests.fs | none | captured |
| T021 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:27:00Z | tests/Controls.Tests/Feature093ParityTests.fs | none | captured |
| T022 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:28:00Z | tests/Controls.Tests/Feature093ParityTests.fs | none | captured |
| T023 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:32:00Z | src/Controls/Control.fs | none | captured |
| T023 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:33:00Z | src/Controls/Control.fs | none | captured |
| T024 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:34:00Z | specs/093-visual-state-style-layer/readiness/us3-parity-baseline.md | none | asserted |
| T025 | fs-skia-design-tokens | .agents/skills/fs-skia-design-tokens/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:36:00Z | specs/093-visual-state-style-layer/readiness/sc006-contrast-authority.md | none | captured |
| T026 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:38:00Z | specs/093-visual-state-style-layer/readiness/surface-baselines.md | none | asserted |
| T027 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:40:00Z | specs/093-visual-state-style-layer/readiness/sc004-determinism-property.md | none | asserted |
| T028 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:44:00Z | specs/093-visual-state-style-layer/readiness/generated-guidance-validation.md | none | asserted |
| T029 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:48:00Z | specs/093-visual-state-style-layer/readiness/evidence-graph.md | none | captured |
| T030 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-10T18:40:00Z | 2026-06-10T19:52:00Z | specs/093-visual-state-style-layer/readiness/evidence-audit.md | none | captured |

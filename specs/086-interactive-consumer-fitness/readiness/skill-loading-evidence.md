# Skill-loading evidence

One row per (TaskId, DeclaredSkillId). `LoadedAt` is strictly before `WorkStartedAt`.
ResolvedSkillPath is the canonical `.agents/skills/<id>/SKILL.md` or `src/*/skill/SKILL.md` home.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|
| T002 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-09T17:05:55Z | 2026-06-09T17:30:00Z | specs/086-interactive-consumer-fitness/readiness/post-085-baseline.md | none |
| T004 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-09T17:05:55Z | 2026-06-09T17:30:00Z | specs/086-interactive-consumer-fitness/readiness/feature-tier-record.md | none |
| T005 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:12:08Z | src/Scene/Scene.fsi | none |
| T010 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-09T17:05:55Z | 2026-06-09T17:30:00Z | specs/086-interactive-consumer-fitness/readiness/governance-risk-levels.md | none |
| T006 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:12:08Z | src/Controls/Types.fsi | none |
| T007 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:12:08Z | src/SkiaViewer/SkiaViewer.fsi | none |
| T017 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:12:30Z | tests/Controls.Tests/Feature086LayoutBoundsTests.fs | none |
| T018 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:12:30Z | tests/Controls.Tests/Feature086PreviewParityTests.fs | none |
| T019 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:12:30Z | src/Controls/Control.fs | none |
| T020 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:12:30Z | src/Controls/Control.fs | none |
| T021 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:12:30Z | specs/086-interactive-consumer-fitness/readiness/rendertree-sidebyside-bounds.txt | none |
| T027 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:12:30Z | tests/Controls.Tests/Feature086LayoutBoundsTests.fs | none |
| T028 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:12:30Z | src/Controls/Control.fs | none |
| T029 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:12:30Z | src/Controls/Control.fs | none |
| T031 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:13:00Z | tests/SkiaViewer.Tests/Feature086SceneTranslateTests.fs | none |
| T032 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:13:00Z | src/Scene/Scene.fs | none |
| T033 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:13:00Z | src/Scene/Scene.fs | none |
| T034 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:13:00Z | specs/086-interactive-consumer-fitness/readiness/scene-translate-sizedtext.txt | none |
| T037 | fs-skia-viewer-host | .agents/skills/fs-skia-viewer-host/SKILL.md | loaded | 2026-06-09T17:13:00Z | 2026-06-09T17:14:00Z | .agents/skills/fs-skia-viewer-host/SKILL.md | none |
| T011 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-09T17:05:55Z | 2026-06-09T17:40:00Z | template/base/tests/Product.Tests/BehaviorTests.fs | none |
| T012 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:40:00Z | template/base/src/Product/Model.fs | none |
| T013 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:40:00Z | template/base/src/Product/View.fs | none |
| T014 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-09T17:05:55Z | 2026-06-09T17:40:00Z | template/base/src/Product/LayoutEvidence.fs | none |
| T016 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T17:10:00Z | 2026-06-09T17:40:00Z | specs/086-interactive-consumer-fitness/readiness/real-controls-render.metadata.txt | none |
| T022 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-09T18:00:00Z | 2026-06-09T18:10:00Z | template/base/tests/Product.Tests/BehaviorTests.fs | none |
| T023 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-09T18:00:00Z | 2026-06-09T18:10:00Z | template/base/tests/Product.Tests/GovernanceTests.fs | none |
| T024 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-09T18:00:00Z | 2026-06-09T18:10:00Z | .template.config/template.json | none |
| T025 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-09T18:00:00Z | 2026-06-09T18:10:00Z | template/base/src/Product/Program.fs | none |
| T026 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T18:00:00Z | 2026-06-09T18:10:00Z | specs/086-interactive-consumer-fitness/readiness/window-visibility.md | none |
| T035 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-09T18:00:00Z | 2026-06-09T18:10:00Z | specs/086-interactive-consumer-fitness/readiness/key-warmup-delivery.txt | none |
| T036 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T18:00:00Z | 2026-06-09T18:10:00Z | src/SkiaViewer/SkiaViewer.fs | none |
| T038 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T18:00:00Z | 2026-06-09T18:10:00Z | specs/086-interactive-consumer-fitness/readiness/key-warmup-delivery.txt | none |
| T042 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-09T18:00:00Z | 2026-06-09T18:20:00Z | specs/086-interactive-consumer-fitness/readiness/logs/test.txt | none |
| T043 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-09T18:00:00Z | 2026-06-09T18:30:00Z | specs/086-interactive-consumer-fitness/readiness/generated-product-check.md | none |
| T044 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-09T18:00:00Z | 2026-06-09T18:35:00Z | specs/086-interactive-consumer-fitness/readiness/task-graph.md | none |
| T045 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-09T18:00:00Z | 2026-06-09T18:40:00Z | specs/086-interactive-consumer-fitness/readiness/logs/evidence-audit.txt | none |

# Skill-loading evidence (085)

One row per (task, declared-skill) pair. `loaded_at` is strictly **before**
`work_started_at`. Resolved paths point at the `.agents/skills/<id>/SKILL.md` (or
`src/*/skill/SKILL.md`) home. The contract is enforced once a task flips to `[X]`.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|
| T005 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T11:40:00Z | src/Controls/Control.fsi | none |
| T005 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T11:40:00Z | src/Controls/Control.fs | none |
| T006 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T11:45:00Z | src/Controls.Elmish/ControlsElmish.fsi | none |
| T006 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T11:45:00Z | src/Controls.Elmish/ControlsElmish.fs | none |
| T007 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T11:54:00Z | specs/085-showcase-feedback-followups/readiness/fsi-session.txt | none |
| T007 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T11:54:00Z | specs/085-showcase-feedback-followups/readiness/fsi-session.txt | none |
| T009 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-09T11:57:00Z | 2026-06-09T11:57:30Z | specs/085-showcase-feedback-followups/readiness/runtime-limitations.md | none |
| T010 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T13:55:00Z | tests/Controls.Tests/RenderTreeTests.fs | none |
| T010 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T13:55:00Z | tests/Controls.Tests/RenderTreeTests.fs | none |
| T011 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T13:55:00Z | tests/Controls.Tests/RenderTreeTests.fs | none |
| T012 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T11:40:00Z | src/Controls/Control.fs | none |
| T012 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T11:40:00Z | src/Controls/Control.fs | none |
| T013 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T14:00:00Z | specs/085-showcase-feedback-followups/evidence/render-distinctness/page-a.png | none |
| T013 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-09T11:57:00Z | 2026-06-09T14:00:00Z | specs/085-showcase-feedback-followups/readiness/real-image-evidence.md | none |
| T015 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T14:04:00Z | tests/SkiaViewer.Tests/Feature085InteractiveHostTests.fs | none |
| T015 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T14:04:00Z | tests/SkiaViewer.Tests/Feature085InteractiveHostTests.fs | none |
| T016 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T14:04:00Z | tests/SkiaViewer.Tests/Feature085InteractiveHostTests.fs | none |
| T016 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T14:04:00Z | tests/SkiaViewer.Tests/Feature085InteractiveHostTests.fs | none |
| T017 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T11:45:00Z | src/Controls.Elmish/ControlsElmish.fs | none |
| T017 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T11:45:00Z | src/Controls.Elmish/ControlsElmish.fs | none |
| T018 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T12:15:00Z | specs/085-showcase-feedback-followups/readiness/interactive-visible-window.md | none |
| T019 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-09T11:57:00Z | 2026-06-09T12:20:00Z | specs/085-showcase-feedback-followups/evidence/pointer-dispatch.md | none |
| T019 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T12:20:00Z | specs/085-showcase-feedback-followups/evidence/pointer-dispatch.md | none |
| T020 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-09T12:21:00Z | 2026-06-09T12:21:30Z | tests/KeyboardInput.Tests/Tests.fs | none |
| T021 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-09T12:21:00Z | 2026-06-09T12:21:30Z | src/KeyboardInput/KeyboardInput.fs | none |
| T022 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-09T12:21:00Z | 2026-06-09T12:22:00Z | specs/085-showcase-feedback-followups/evidence/normalize-mapping.md | none |
| T023 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T12:22:00Z | tests/SkiaViewer.Tests/Feature085InteractiveHostTests.fs | none |
| T023 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T12:22:00Z | tests/SkiaViewer.Tests/Feature085InteractiveHostTests.fs | none |
| T024 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T11:45:00Z | src/SkiaViewer/SkiaViewer.fs | none |
| T025 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T12:23:00Z | specs/085-showcase-feedback-followups/evidence/size-aware-render/extent-400x300.png | none |
| T025 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-09T11:57:00Z | 2026-06-09T12:23:00Z | specs/085-showcase-feedback-followups/readiness/runtime-limitations.md | none |
| T026 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-09T11:34:58Z | 2026-06-09T12:25:00Z | .agents/skills/fs-skia-viewer-host/SKILL.md | none |
| T027 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-09T12:25:00Z | 2026-06-09T12:26:00Z | .agents/skills/fs-skia-typed-controls/SKILL.md | none |
| T028 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-09T12:25:00Z | 2026-06-09T12:26:00Z | template/base/docs/scaffold-map.md | none |
| T029 | speckit-specify | .agents/skills/speckit-specify/SKILL.md | loaded | 2026-06-09T12:26:00Z | 2026-06-09T12:27:00Z | .specify/templates/spec-template.md | none |
| T030 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-09T11:57:00Z | 2026-06-09T12:27:00Z | .agents/skills/fs-skia-evidence-mode/SKILL.md | none |
| T031 | speckit-specify | .agents/skills/speckit-specify/SKILL.md | loaded | 2026-06-09T12:26:00Z | 2026-06-09T12:28:00Z | .agents/skills/speckit-specify/SKILL.md | none |
| T035 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-09T12:40:00Z | 2026-06-09T12:42:00Z | specs/085-showcase-feedback-followups/readiness/generated-validation.md | none |
| T036 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-09T11:30:00Z | 2026-06-09T12:45:00Z | specs/085-showcase-feedback-followups/readiness/evidence-graph.md | none |
| T037 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-09T11:30:00Z | 2026-06-09T12:46:00Z | specs/085-showcase-feedback-followups/readiness/evidence-audit.md | none |
| T038 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-09T11:57:00Z | 2026-06-09T12:47:00Z | specs/085-showcase-feedback-followups/readiness/evidence-audit.md | none |

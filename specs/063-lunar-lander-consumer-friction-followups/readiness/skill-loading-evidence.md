# Skill-loading evidence

Each skilled task loads its declared `skillist` (in order) before any code change for that
task begins, per the implementation-loading discipline. `loaded_at` precedes
`work_started_at` for every row. Resolved paths are the canonical `.agents/skills/**` homes
for governance/authoring skills and the `src/*/skill/**` homes for capability skills. This
log is read from the **feature** readiness dir (`specs/<feature>/readiness/`, not repo-root)
and is enforced only once tasks flip to `[X]`.

| task | skill id | resolved path | load result | loaded_at | work_started_at | evidence path | reviewer exception |
|---|---|---|---|---|---|---|---|
| T009 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-04T19:40:00Z | 2026-06-04T20:00:00Z | this file + tests/SkiaViewer.Tests/Feature063RendererTests.fs | none |
| T010 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-04T19:42:00Z | 2026-06-04T20:05:00Z | this file + tests/SkiaViewer.Tests/Feature063RendererTests.fs | none |
| T011 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-04T19:40:00Z | 2026-06-04T20:15:00Z | this file + src/SkiaViewer/SceneRenderer.fs | none |
| T012 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-04T19:40:00Z | 2026-06-04T20:25:00Z | this file + src/SkiaViewer/SceneRenderer.fs | none |
| T013 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-04T19:45:00Z | 2026-06-04T20:35:00Z | this file + src/SkiaViewer/SkiaViewer.fs + src/SkiaViewer/Host/Vulkan.fs | none |
| T014 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-04T19:40:00Z | 2026-06-04T20:45:00Z | this file + src/Scene/skill/SKILL.md | none |
| T015 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-04T19:45:00Z | 2026-06-04T20:55:00Z | this file + readiness/renderer-image-evidence.md | none |
| T016 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T21:00:00Z | 2026-06-04T21:10:00Z | this file + build/Governance/Targets.fs | none |
| T017 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T21:00:00Z | 2026-06-04T21:20:00Z | this file + build/Governance/Engine/Interpret.fs | none |
| T018 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T21:00:00Z | 2026-06-04T21:35:00Z | this file + validation.contract.yml | none |
| T019 | speckit-analyze | .agents/skills/speckit-analyze/SKILL.md | loaded | 2026-06-04T21:40:00Z | 2026-06-04T21:50:00Z | this file + .agents/skills/speckit-analyze/SKILL.md | none |
| T020 | speckit-analyze | .agents/skills/speckit-analyze/SKILL.md | loaded | 2026-06-04T21:40:00Z | 2026-06-04T22:00:00Z | this file + readiness/symbol-cross-check.md | none |
| T021 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-04T22:15:00Z | this file + tests/Governance.Tests/Feature063GovernanceTests.fs | none |
| T022 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-04T22:25:00Z | this file + build/Governance/Evidence/Render.fs | none |
| T023 | speckit-implement | .agents/skills/speckit-implement/SKILL.md | loaded | 2026-06-04T22:30:00Z | 2026-06-04T22:40:00Z | this file + .agents/skills/speckit-implement/SKILL.md | none |
| T025 | speckit-plan | .agents/skills/speckit-plan/SKILL.md | loaded | 2026-06-04T22:45:00Z | 2026-06-04T22:55:00Z | this file + .agents/skills/speckit-plan/SKILL.md | none |
| T026 | fs-skia-layout-readability | .agents/skills/fs-skia-layout-readability/SKILL.md | loaded | 2026-06-04T22:46:00Z | 2026-06-04T23:05:00Z | this file + template/base/docs/scaffold-map.md | none |
| T027 | speckit-specify | .agents/skills/speckit-specify/SKILL.md | loaded | 2026-06-04T22:47:00Z | 2026-06-04T23:15:00Z | this file + .agents/skills/speckit-specify/SKILL.md | none |
| T028 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-04T22:48:00Z | 2026-06-04T23:25:00Z | this file + readiness/evidence-path-token-scan.md | none |
| T030 | fs-skia-layout-readability | .agents/skills/fs-skia-layout-readability/SKILL.md | loaded | 2026-06-04T22:46:00Z | 2026-06-04T23:35:00Z | this file + tests/SkillSupport.Tests/Tests.fs | none |
| T031 | fs-skia-layout-readability | .agents/skills/fs-skia-layout-readability/SKILL.md | loaded | 2026-06-04T22:46:00Z | 2026-06-04T23:45:00Z | this file + src/SkillSupport/Wrap.fs | none |
| T032 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T23:50:00Z | 2026-06-05T00:00:00Z | this file + readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt | none |
| T033 | fs-skia-layout-readability | .agents/skills/fs-skia-layout-readability/SKILL.md | loaded | 2026-06-04T22:46:00Z | 2026-06-05T00:10:00Z | this file + .agents/skills/fs-skia-layout-readability/SKILL.md | none |
| T034 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-04T19:42:00Z | 2026-06-05T00:20:00Z | this file + .agents/skills/fs-skia-evidence-mode/SKILL.md | none |
| T036 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T23:50:00Z | 2026-06-05T00:35:00Z | this file + readiness/logs (RefreshSurfaceBaselines) | none |
| T037 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T23:50:00Z | 2026-06-05T00:45:00Z | this file + readiness/logs (TemplateCheck/GeneratedProductCheck) | none |
| T038 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-05T00:50:00Z | 2026-06-05T00:55:00Z | readiness/task-graph.md | none |
| T039 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-05T01:00:00Z | 2026-06-05T01:05:00Z | readiness/logs/evidence-audit.txt | none |

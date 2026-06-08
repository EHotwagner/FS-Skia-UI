# Skill-loading evidence

Each skilled task loads its declared `skillist` (in order) before any code change for that
task begins, per the implementation-loading discipline. `loaded_at` precedes
`work_started_at` for every row. Resolved paths are the canonical homes: governance/authoring
skills under `.agents/skills/<id>/SKILL.md`, package-capability skills under
`src/*/skill/SKILL.md`. This log is read from the **feature** readiness dir
(`specs/080-control-render-fidelity/readiness/`, not repo-root) and is enforced only once
tasks flip to `[X]`. One row per (task, declared-skill) pair.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|---|---|---|---|---|---|---|---|
| T002 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T13:45:00Z | 2026-06-08T13:52:00Z | this file + readiness/control-fidelity.md | none |
| T004 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T13:46:00Z | 2026-06-08T13:51:00Z | this file + tests/ControlsPreview.Harness/fixtures/fidelity/lowfi/ | none |
| T005 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-08T13:55:00Z | 2026-06-08T14:00:00Z | this file + tests/ControlsPreview.Harness/PreviewSamples.fs | none |
| T006 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-08T13:55:00Z | 2026-06-08T14:05:00Z | this file + src/Controls/Control.fs | none |
| T007 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T13:45:00Z | 2026-06-08T14:10:00Z | this file + tests/ControlsPreview.Harness/PreviewSamples.fs | none |
| T008 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-08T14:15:00Z | 2026-06-08T14:20:00Z | this file + tests/ControlsPreview.Harness/RendererFidelityTests.fs | none |
| T009 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-08T14:15:00Z | 2026-06-08T14:25:00Z | this file + tests/ControlsPreview.Harness/RendererFidelityTests.fs | none |
| T010 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-08T14:30:00Z | 2026-06-08T14:35:00Z | this file + tests/ControlsPreview.Harness/PreviewSamples.fs | none |
| T011 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-08T14:15:00Z | 2026-06-08T14:40:00Z | this file + src/Controls/Control.fs | none |
| T011 | fs-skia-layout-readability | .agents/skills/fs-skia-layout-readability/SKILL.md | loaded | 2026-06-08T14:32:00Z | 2026-06-08T14:40:00Z | this file + src/Controls/Control.fs | none |
| T012 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T13:46:00Z | 2026-06-08T14:50:00Z | this file + src/SkiaViewer/SceneRenderer.fs | none |
| T013 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-08T13:55:00Z | 2026-06-08T14:55:00Z | this file + src/Controls/Control.fs | none |
| T014 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T13:46:00Z | 2026-06-08T15:00:00Z | this file + readiness/real-image-evidence.md | none |
| T015 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T13:45:00Z | 2026-06-08T15:05:00Z | this file + tests/ControlsPreview.Harness/FidelityTests.fs | none |
| T016 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T13:45:00Z | 2026-06-08T15:10:00Z | this file + tests/ControlsPreview.Harness/Fidelity.fs | none |
| T017 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T13:46:00Z | 2026-06-08T15:20:00Z | this file + tests/ControlsPreview.Harness/Program.fs | none |
| T018 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T13:45:00Z | 2026-06-08T15:25:00Z | this file + tests/ControlsPreview.Harness/fixtures/fidelity/ | none |
| T019 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T15:28:00Z | 2026-06-08T15:30:00Z | this file + build/Governance/Targets.fs | none |
| T020 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T15:28:00Z | 2026-06-08T15:35:00Z | this file + build/Governance/Engine/Update.fs | none |
| T021 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-08T15:40:00Z | 2026-06-08T15:45:00Z | this file + validation.contract.yml | none |
| T022 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T15:28:00Z | 2026-06-08T15:55:00Z | this file + readiness/control-fidelity.md | none |
| T023 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T13:46:00Z | 2026-06-08T16:00:00Z | this file + docs/img/controls/ | none |
| T023 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T13:45:00Z | 2026-06-08T16:00:00Z | this file + docs/img/controls/ | none |
| T024 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-08T15:40:00Z | 2026-06-08T16:10:00Z | this file + docs/controls/ | none |
| T025 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T13:45:00Z | 2026-06-08T16:20:00Z | this file + readiness/usage-coherence.md | none |
| T026 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T13:45:00Z | 2026-06-08T16:30:00Z | this file + readiness/real-image-evidence.md | none |
| T027 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-08T14:15:00Z | 2026-06-08T16:40:00Z | this file + tests/SkiaViewer.Tests/ | none |
| T028 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T13:46:00Z | 2026-06-08T16:50:00Z | this file + readiness/aggregate-hang-diagnostics.md | none |
| T029 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T15:28:00Z | 2026-06-08T17:00:00Z | this file + readiness/aggregate-hang-diagnostics.md | none |
| T030 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-08T17:05:00Z | 2026-06-08T17:10:00Z | this file + readiness/evidence-graph.md | none |
| T031 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-08T17:15:00Z | 2026-06-08T17:20:00Z | this file + readiness/evidence-audit.md | none |

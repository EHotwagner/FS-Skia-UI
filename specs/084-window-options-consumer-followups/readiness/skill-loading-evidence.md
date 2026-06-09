# Skill-loading evidence

Each skilled task loads its declared `skillist` (in order) before any code change for that
task begins, per the implementation-loading discipline. `loaded_at` precedes
`work_started_at` for every row. Resolved paths are the canonical homes: governance/authoring
skills under `.agents/skills/<id>/SKILL.md`, package-capability skills under
`src/*/skill/SKILL.md`. This log is read from the **feature** readiness dir
(`specs/084-window-options-consumer-followups/readiness/`, not repo-root) and is enforced only
once tasks flip to `[X]`. One row per (task, declared-skill) pair.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|---|---|---|---|---|---|---|---|
| T002 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T18:00:00Z | 2026-06-08T18:05:00Z | this file + readiness/interactive-visible-window.md | none |
| T004 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T18:01:00Z | 2026-06-08T18:10:00Z | this file + src/SkiaViewer/SkiaViewer.fsi | none |
| T006 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T18:00:00Z | 2026-06-08T18:12:00Z | this file + readiness/window-state-diagnostics.md | none |
| T007 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T18:01:00Z | 2026-06-08T18:15:00Z | this file + tests/SkiaViewer.Tests/Tests.fs | none |
| T008 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T18:01:00Z | 2026-06-08T18:20:00Z | this file + tests/SkiaViewer.Tests/Tests.fs | none |
| T009 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T18:01:00Z | 2026-06-08T18:25:00Z | this file + src/SkiaViewer/SkiaViewer.fs | none |
| T010 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T18:01:00Z | 2026-06-08T18:30:00Z | this file + src/SkiaViewer/SkiaViewer.fs | none |
| T011 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T18:01:00Z | 2026-06-08T18:35:00Z | this file + src/SkiaViewer/SkiaViewer.fs | none |
| T011 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T18:00:00Z | 2026-06-08T18:35:00Z | this file + src/SkiaViewer/SkiaViewer.fs | none |
| T012 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T18:01:00Z | 2026-06-08T18:40:00Z | this file + template/base/src/Product/WindowOptions.fs | none |
| T013 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T18:01:00Z | 2026-06-08T18:45:00Z | this file + template/base/src/Product/Program.fs | none |
| T014 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T18:01:00Z | 2026-06-08T18:50:00Z | this file + readiness/real-image-evidence.md | none |
| T014 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T18:00:00Z | 2026-06-08T18:50:00Z | this file + readiness/real-image-evidence.md | none |
| T015 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T18:01:00Z | 2026-06-08T18:55:00Z | this file + readiness/fsi-session.txt | none |
| T017 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T19:00:00Z | 2026-06-08T19:05:00Z | this file + tests/Governance.Tests/Feature084GovernanceTests.fs | none |
| T018 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T19:00:00Z | 2026-06-08T19:10:00Z | this file + tests/Governance.Tests/Feature084GovernanceTests.fs | none |
| T019 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-08T19:12:00Z | 2026-06-08T19:15:00Z | this file + build/Governance/Evidence/EvidenceFormatSchema.fs | none |
| T020 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T19:00:00Z | 2026-06-08T19:20:00Z | this file + template/base/docs/evidence-formats.md | none |
| T021 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-08T19:12:00Z | 2026-06-08T19:25:00Z | this file + build/Governance/Evidence/Render.fs | none |
| T022 | fsharp-shell-process | .agents/skills/fsharp-shell-process/SKILL.md | loaded | 2026-06-08T19:28:00Z | 2026-06-08T19:30:00Z | this file + build/Governance/Front/Governance.fs | none |
| T023 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T19:00:00Z | 2026-06-08T19:40:00Z | this file + readiness/audit-diagnostics.md | none |
| T024 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T19:00:00Z | 2026-06-08T19:50:00Z | this file + tests/Governance.Tests/Feature084GovernanceTests.fs | none |
| T025 | fs-skia-layout-readability | .agents/skills/fs-skia-layout-readability/SKILL.md | loaded | 2026-06-08T19:55:00Z | 2026-06-08T20:00:00Z | this file + template/base/docs/scaffold-map.md | none |
| T026 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-08T20:05:00Z | 2026-06-08T20:10:00Z | this file + template/base/docs/scaffold-map.md | none |
| T027 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T19:00:00Z | 2026-06-08T20:20:00Z | this file + template/base/docs/product.md | none |
| T028 | speckit-analyze | .agents/skills/speckit-analyze/SKILL.md | loaded | 2026-06-08T20:25:00Z | 2026-06-08T20:30:00Z | this file + .agents/skills/speckit-analyze/SKILL.md | none |
| T029 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-08T19:12:00Z | 2026-06-08T20:35:00Z | this file + .claude/skills/speckit-analyze/SKILL.md | none |
| T030 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T19:00:00Z | 2026-06-08T20:40:00Z | this file + readiness/symbol-cross-check.md | none |
| T031 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T18:01:00Z | 2026-06-08T20:50:00Z | this file + readiness/per-package-surface/FS.Skia.UI.SkiaViewer.fsi.txt | none |
| T032 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T19:00:00Z | 2026-06-08T21:00:00Z | this file + readiness/logs/ | none |
| T033 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-08T21:05:00Z | 2026-06-08T21:10:00Z | this file + readiness/evidence-graph.md | none |
| T034 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-08T21:15:00Z | 2026-06-08T21:20:00Z | this file + readiness/evidence-audit.md | none |

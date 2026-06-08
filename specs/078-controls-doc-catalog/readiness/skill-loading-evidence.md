# Skill-Loading Evidence (078)

One row per (task, declared-skill) pair. Each skill's `SKILL.md` is loaded (read)
in declared order **before** the task's code work begins, so `LoadedAt` is strictly
earlier than `WorkStartedAt`. Resolved paths are the live `SkillRegistry` homes
(`.agents/skills/<id>/SKILL.md`, or `src/<Lib>/skill/SKILL.md` for product skills).

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|
| T004 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-08T12:05:00Z | 2026-06-08T12:09:00Z | build/Governance/CatalogDocsGen.fsi | |
| T005 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T12:06:00Z | 2026-06-08T12:20:00Z | build/Governance/Targets.fs | |
| T006 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-08T12:05:00Z | 2026-06-08T12:30:00Z | docs/controls/text-block.md | |
| T007 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T12:06:00Z | 2026-06-08T12:40:00Z | build/Governance/validation.contract.yml | |
| T009 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-08T12:05:00Z | 2026-06-08T12:50:00Z | tests/Governance.Tests/CatalogDocsGenTests.fs | |
| T010 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-08T12:48:00Z | 2026-06-08T12:52:00Z | tests/Governance.Tests/CatalogDocsGenTests.fs | |
| T011 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-08T12:05:00Z | 2026-06-08T12:25:00Z | build/Governance/CatalogDocsGen.fs | |
| T012 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-08T12:48:00Z | 2026-06-08T12:55:00Z | build/Governance/Engine/Update.fs | |
| T013 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-08T12:05:00Z | 2026-06-08T13:05:00Z | docs/controls/catalog.md | |
| T014 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T13:10:00Z | 2026-06-08T13:20:00Z | specs/078-controls-doc-catalog/readiness/controls-preview-evidence.md | |
| T014 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-08T13:11:00Z | 2026-06-08T13:20:00Z | specs/078-controls-doc-catalog/readiness/controls-preview-evidence.md | |
| T015 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-08T13:12:00Z | 2026-06-08T13:25:00Z | specs/078-controls-doc-catalog/readiness/controls-preview-evidence.md | |
| T015 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-08T13:10:00Z | 2026-06-08T13:25:00Z | specs/078-controls-doc-catalog/readiness/controls-preview-evidence.md | |
| T018 | fsdocs-build | .agents/skills/fsdocs-build/SKILL.md | loaded | 2026-06-08T13:30:00Z | 2026-06-08T13:40:00Z | specs/078-controls-doc-catalog/readiness/docs-build.md | |
| T021 | fsdocs-technical | .agents/skills/fsdocs-technical/SKILL.md | loaded | 2026-06-08T13:42:00Z | 2026-06-08T13:50:00Z | docs/controls/spec-kit-workflow.md | |
| T022 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-08T13:43:00Z | 2026-06-08T13:52:00Z | docs/controls/spec-kit-workflow.md | |
| T024 | fs-skia-design-tokens | .agents/skills/fs-skia-design-tokens/SKILL.md | loaded | 2026-06-08T13:44:00Z | 2026-06-08T13:55:00Z | docs/controls/spec-kit-workflow.md | |
| T027 | fsdocs-build | .agents/skills/fsdocs-build/SKILL.md | loaded | 2026-06-08T13:30:00Z | 2026-06-08T14:00:00Z | specs/078-controls-doc-catalog/readiness/docs-build.md | |
| T028 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T12:06:00Z | 2026-06-08T14:05:00Z | specs/078-controls-doc-catalog/readiness/governance-suite.md | |
| T029 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-08T11:40:00Z | 2026-06-08T14:10:00Z | specs/078-controls-doc-catalog/readiness/task-graph.md | |
| T030 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-08T14:12:00Z | 2026-06-08T14:15:00Z | specs/078-controls-doc-catalog/readiness/logs/evidence-audit.txt | |

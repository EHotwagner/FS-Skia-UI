# Skill loading evidence

One row per (task, declared-skill) pair. Each declared `skillist` skill was resolved to a
readable `SKILL.md` and loaded before work on the task began (`LoadedAt` strictly before
`WorkStartedAt`). Tasks with an empty `skillist` (T001, T002) are omitted.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|--------|-----------------|-------------------|------------|----------|---------------|-------------|-----------|
| T003 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T20:40:00Z | 2026-06-05T20:41:00Z | readiness/parity-fixtures/ | none |
| T004 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:43:00Z | build/Governance/CatalogGen.fsi | none |
| T005 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:44:00Z | build/Governance/Targets.fs | none |
| T006 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:45:00Z | build/Governance/Engine/Interpret.fs | none |
| T007 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:46:00Z | readiness/typed-catalog-generation.md | none |
| T008 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:46:30Z | build/Governance/CatalogGen.fs | none |
| T009 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:50:00Z | tests/Controls.Tests/CatalogTests.fs | none |
| T010 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:50:30Z | tests/Controls.Tests/CatalogTests.fs | none |
| T011 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:47:00Z | build/Governance/CatalogGen.fs | none |
| T011 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T20:40:00Z | 2026-06-05T20:47:00Z | build/Governance/CatalogGen.fs | none |
| T012 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:48:00Z | src/Controls/Catalog.fs | none |
| T013 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:49:00Z | build/Governance/Engine/Update.fs | none |
| T013 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:49:00Z | build/Governance/Engine/Update.fs | none |
| T014 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:49:30Z | build/Governance/Engine/Update.fs | none |
| T015 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:55:00Z | readiness/typed-catalog-generation.md | none |
| T016 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:50:00Z | tests/Controls.Tests/CatalogTests.fs | none |
| T016 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T20:40:00Z | 2026-06-05T20:50:00Z | tests/Controls.Tests/CatalogTests.fs | none |
| T017 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T20:40:00Z | 2026-06-05T20:51:00Z | tests/Controls.Tests/CatalogTests.fs | none |
| T018 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T20:40:00Z | 2026-06-05T20:52:00Z | readiness/typed-catalog-parity.md | none |
| T019 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:55:00Z | readiness/typed-catalog-parity.md | none |
| T020 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:53:00Z | validation.contract.yml | none |
| T021 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:54:00Z | readiness/logs/route.txt | none |
| T022 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:56:00Z | readiness/governance-risk-levels.md | none |
| T023 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:57:00Z | readiness/evidence-graph.md | none |
| T024 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-05T20:42:00Z | 2026-06-05T20:58:00Z | readiness/evidence-audit.md | none |

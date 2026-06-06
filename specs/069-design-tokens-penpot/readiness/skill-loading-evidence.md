# Skill loading evidence

One row per (task, declared-skill) pair. Each declared `skillist` skill was resolved to a
readable `SKILL.md` and loaded before work on the task began (`LoadedAt` strictly before
`WorkStartedAt`). Tasks with an empty `skillist` (T001, T002, T003, T006, T007, T008, T023,
T026) are omitted. The new `fs-skia-design-tokens` skill (authored by T026 in this branch) is
intentionally absent from every `skillist` and applied by hand.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|--------|-----------------|-------------------|------------|----------|---------------|-------------|-----------|
| T004 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:05:00Z | src/Controls/DesignTokens.fsi | none |
| T005 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:06:00Z | build/Governance/DesignTokenGen.fsi | none |
| T005 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:06:00Z | build/Governance/DesignTokenGen.fsi | none |
| T009 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:10:00Z | tests/Governance.Tests/DesignTokenGenTests.fs | none |
| T010 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:11:00Z | tests/Governance.Tests/DesignTokenGenTests.fs | none |
| T010 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:11:00Z | tests/Governance.Tests/DesignTokenGenTests.fs | none |
| T011 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:12:00Z | tests/Governance.Tests/DesignTokenGenTests.fs | none |
| T011 | fsharp-graph-algorithms | .agents/skills/fsharp-graph-algorithms/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:12:00Z | tests/Governance.Tests/DesignTokenGenTests.fs | none |
| T012 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:15:00Z | build/Governance/DesignTokenGen.fs | none |
| T012 | fsharp-graph-algorithms | .agents/skills/fsharp-graph-algorithms/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:15:00Z | build/Governance/DesignTokenGen.fs | none |
| T012 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:15:00Z | build/Governance/DesignTokenGen.fs | none |
| T013 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:20:00Z | src/Controls/DesignTokens.fs | none |
| T014 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:22:00Z | build/Governance/Targets.fs | none |
| T015 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:24:00Z | build/Governance/Front/Governance.fs | none |
| T016 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:26:00Z | build/Governance/Engine/Update.fs | none |
| T017 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:28:00Z | build/Governance/Routing.fs | none |
| T018 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:30:00Z | readiness/design-token-drift.md | none |
| T019 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:32:00Z | tests/Controls.Tests/DesignTokenParityTests.fs | none |
| T019 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:32:00Z | tests/Controls.Tests/DesignTokenParityTests.fs | none |
| T020 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:33:00Z | tests/Controls.Tests/DesignTokenParityTests.fs | none |
| T021 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:34:00Z | tests/Controls.Tests/DesignTokenParityTests.fs | none |
| T021 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:34:00Z | tests/Controls.Tests/DesignTokenParityTests.fs | none |
| T022 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:36:00Z | src/Controls/Theme.fs | none |
| T024 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:38:00Z | tests/Controls.Tests/DesignTokenParityTests.fs | none |
| T025 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:40:00Z | readiness/package-surface-expectations.md | none |
| T027 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:42:00Z | .claude/skills/fs-skia-design-tokens/SKILL.md | none |
| T028 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:44:00Z | readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt | none |
| T029 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:46:00Z | readiness/logs/route-gates.txt | none |
| T030 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:48:00Z | readiness/task-graph.md | none |
| T031 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-06T09:00:00Z | 2026-06-06T09:50:00Z | readiness/evidence-audit.md | none |

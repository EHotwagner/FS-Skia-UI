# Skill-loading evidence — feature 089

One row per (TaskId, DeclaredSkillId). `LoadedAt` is strictly before
`WorkStartedAt`. ResolvedSkillPath is the canonical `.agents/skills/<id>/SKILL.md`
home. This log is read from the **feature** readiness dir (not repo-root) and is
enforced once tasks flip to `[X]`. The 9th `Provenance` column is `captured`
(observed during the run, recorded at the load action before code changes) or
`asserted` (hand-authored).

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T006 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T02:42:00Z | tests/Governance.Tests/Feature089GovernanceTests.fs | none | captured |
| T007 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T02:42:00Z | tests/Governance.Tests/Feature089GovernanceTests.fs | none | captured |
| T008 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T02:45:00Z | build/Governance/CatalogGen.fs | none | captured |
| T010 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T03:05:00Z | specs/089-typed-surface-and-workflow-followups/readiness/logs/surface-refresh.txt | none | captured |
| T011 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T03:10:00Z | specs/089-typed-surface-and-workflow-followups/readiness/typed-front-door-authoring.md | none | captured |
| T012 | speckit-implement | .agents/skills/speckit-implement/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T02:50:00Z | .agents/skills/speckit-implement/SKILL.md | none | captured |
| T013 | speckit-implement | .agents/skills/speckit-implement/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T02:52:00Z | .agents/skills/speckit-implement/SKILL.md | none | captured |
| T014 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T03:05:00Z | .claude/skills/speckit-implement/SKILL.md | none | captured |
| T015 | speckit-implement | .agents/skills/speckit-implement/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T03:06:00Z | specs/089-typed-surface-and-workflow-followups/readiness/skill-sync-check.md | none | captured |
| T016 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T02:48:00Z | tests/Governance.Tests/Feature089GovernanceTests.fs | none | captured |
| T017 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T02:48:00Z | build/Governance/Evidence/Render.fs | none | captured |
| T019 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T03:20:00Z | specs/089-typed-surface-and-workflow-followups/readiness/logs/evidence-graph.txt | none | captured |
| T020 | speckit-clarify | .agents/skills/speckit-clarify/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T02:55:00Z | .agents/skills/speckit-clarify/SKILL.md | none | captured |
| T021 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T03:05:00Z | .claude/skills/speckit-clarify/SKILL.md | none | captured |
| T022 | speckit-clarify | .agents/skills/speckit-clarify/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T03:06:00Z | specs/089-typed-surface-and-workflow-followups/readiness/skill-sync-check.md | none | captured |
| T023 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T03:30:00Z | specs/089-typed-surface-and-workflow-followups/readiness/logs/dev.txt | none | captured |
| T024 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T03:35:00Z | specs/089-typed-surface-and-workflow-followups/readiness/logs/evidence-graph.txt | none | captured |
| T025 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-10T02:30:00Z | 2026-06-10T03:40:00Z | specs/089-typed-surface-and-workflow-followups/readiness/logs/evidence-audit.txt | none | captured |

# Skill loading evidence

One row per (task, declared-skill) pair. Each declared `skillist` skill was resolved to a
readable `SKILL.md` and loaded before work on the task began (`LoadedAt` strictly before
`WorkStartedAt`). Tasks with an empty `skillist` (T001, T002, T003, T004, T011, T021) are
omitted. `fs-skia-ui-widgets` resolves to the in-repo capability skill home
`src/Controls/skill/SKILL.md` (the same id is also published at
`template/product-skills/fs-skia-ui-widgets/SKILL.md`).

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|--------|-----------------|-------------------|------------|----------|---------------|-------------|-----------|
| T005 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:25:20Z | src/Controls/Reconcile.fsi | none |
| T006 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:25:40Z | src/Controls/Reconcile.fs | none |
| T007 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:26:00Z | tests/Controls.Tests/Controls.Tests.fsproj | none |
| T008 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:26:30Z | specs/067-keyed-reconciliation/readiness/package-surface-expectations.md | none |
| T009 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:27:00Z | tests/Controls.Tests/ReconcileTests.fs | none |
| T010 | fsharp-graph-algorithms | .agents/skills/fsharp-graph-algorithms/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:30:00Z | src/Controls/Reconcile.fs | none |
| T010 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:30:00Z | src/Controls/Reconcile.fs | none |
| T012 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:31:00Z | tests/Controls.Tests/ReconcileTests.fs | none |
| T013 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:31:30Z | src/Controls/Reconcile.fs | none |
| T014 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:32:00Z | tests/Controls.Tests/ReconcileTests.fs | none |
| T015 | fsharp-graph-algorithms | .agents/skills/fsharp-graph-algorithms/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:32:30Z | src/Controls/Reconcile.fs | none |
| T016 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:33:00Z | tests/Controls.Tests/ReconcileTests.fs | none |
| T017 | fsharp-graph-algorithms | .agents/skills/fsharp-graph-algorithms/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:33:30Z | src/Controls/Reconcile.fs | none |
| T018 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:34:00Z | tests/Controls.Tests/ReconcileTests.fs | none |
| T019 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:34:30Z | src/Controls/Reconcile.fs | none |
| T020 | fsharp-graph-algorithms | .agents/skills/fsharp-graph-algorithms/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:35:00Z | tests/Controls.Tests/ReconcileTests.fs | none |
| T022 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:36:00Z | specs/067-keyed-reconciliation/readiness/focused-gates.md | none |
| T023 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:37:00Z | specs/067-keyed-reconciliation/readiness/evidence-graph.md | none |
| T024 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-05T20:21:33Z | 2026-06-05T20:38:00Z | specs/067-keyed-reconciliation/readiness/evidence-audit.md | none |

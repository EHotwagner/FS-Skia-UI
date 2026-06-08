# Skill-loading evidence

Each skilled task loads its declared `skillist` (in order) before any code change for that
task begins, per the implementation-loading discipline. `loaded_at` precedes
`work_started_at` for every row. Resolved paths are the canonical `.agents/skills/**` homes
for governance/authoring skills. This log is read from the **feature** readiness dir
(`specs/077-implement-feedback-hook-parity/readiness/`, not repo-root) and is enforced only
once tasks flip to `[X]`. One row per (task, declared-skill) pair.

| task | skill id | resolved path | load result | loaded_at | work_started_at | evidence path | reviewer exception |
|---|---|---|---|---|---|---|---|
| T004 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-08T08:00:00Z | 2026-06-08T08:10:00Z | this file + build/Governance/PhaseHookParity.fsi | none |
| T006 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T08:00:00Z | 2026-06-08T08:15:00Z | this file + tests/Governance.Tests/PhaseHookParityTests.fs | none |
| T007 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-08T08:00:00Z | 2026-06-08T08:20:00Z | this file + build/Governance/PhaseHookParity.fs | none |
| T008 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T08:00:00Z | 2026-06-08T08:25:00Z | this file + build/Governance/Targets.fs | none |
| T010 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-08T08:00:00Z | 2026-06-08T08:30:00Z | this file + build/Governance/Front/Governance.fs | none |
| T011 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T08:00:00Z | 2026-06-08T08:35:00Z | this file + build/Governance/Routing.fs | none |
| T012 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T08:00:00Z | 2026-06-08T08:40:00Z | this file + tests/Governance.Tests/PhaseHookParityTests.fs | none |
| T016 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T08:00:00Z | 2026-06-08T08:50:00Z | this file + tests/Governance.Tests/PhaseHookParityTests.fs | none |
| T021 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-08T08:00:00Z | 2026-06-08T09:00:00Z | this file + readiness/phase-hook-parity-check.md | none |
| T022 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-08T09:05:00Z | 2026-06-08T09:15:00Z | this file + readiness/template-check.md | none |
| T023 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-08T09:05:00Z | 2026-06-08T09:25:00Z | this file + readiness/generated-product-check.md | none |
| T027 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-08T09:30:00Z | 2026-06-08T09:35:00Z | this file + readiness/evidence-graph.md | none |
| T028 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-08T09:40:00Z | 2026-06-08T09:45:00Z | this file + readiness/evidence-audit.md | none |

# Skill-loading evidence

Each skilled task loads its declared `skillist` (in order) before any code change for that
task begins, per the implementation-loading discipline. `loaded_at` precedes
`work_started_at` for every row.

| task | skill id | resolved path | load result | loaded_at | work_started_at | evidence path | reviewer exception |
|---|---|---|---|---|---|---|---|
| T005 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-02T21:05:00Z | 2026-06-02T21:08:00Z | this file + tests/Package.Tests/Tests.fs + tests/Package.Tests/SurfaceAreaTests.fs | none |
| T014 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-02T21:05:00Z | 2026-06-02T21:20:00Z | this file + build/Governance/Routing.fs + build/Governance/AgentValidation.fs + validation.contract.yml | none |
| T016 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-02T21:05:00Z | 2026-06-02T21:50:00Z | this file + readiness/per-package-surface-enforcement.md | none |
| T017 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-02T21:30:00Z | 2026-06-02T21:35:00Z | this file + build/Governance/GeneratedProduct.fs + readiness/cleanliness-gate.md | none |
| T024 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-02T22:10:00Z | 2026-06-02T22:11:00Z | readiness/task-graph.md | none |
| T025 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-02T22:15:00Z | 2026-06-02T22:16:00Z | readiness/logs/evidence-audit.txt | none |

T024/T025 (graph/audit) are stamped when their gate runs at closeout.

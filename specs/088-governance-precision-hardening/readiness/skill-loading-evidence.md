# Skill-loading evidence — feature 088

One row per (TaskId, DeclaredSkillId). `LoadedAt` is strictly before `WorkStartedAt`.
ResolvedSkillPath is the canonical `.agents/skills/<id>/SKILL.md` home. This log is read from
the **feature** readiness dir (not repo-root) and is enforced once tasks flip to `[X]`. The 9th
`provenance` column is `captured` (observed during the run, recorded at the load action before
code changes) or `asserted` (hand-authored).

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T003 | speckit-tasks | .agents/skills/speckit-tasks/SKILL.md | loaded | 2026-06-10T00:05:00Z | 2026-06-10T00:14:00Z | specs/088-governance-precision-hardening/tasks.md | none | asserted |
| T008 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T00:42:00Z | tests/Governance.Tests/Feature088GovernanceTests.fs | none | captured |
| T009 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T00:42:00Z | tests/Governance.Tests/Feature088GovernanceTests.fs | none | captured |
| T011 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T00:38:00Z | build/Governance/Targets.fs | none | captured |
| T011 | fsharp-graph-algorithms | .agents/skills/fsharp-graph-algorithms/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T00:38:00Z | build/Governance/Targets.fs | none | asserted |
| T013 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T00:40:00Z | build/Governance/AgentValidation.fs | none | captured |
| T014 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T00:43:00Z | specs/088-governance-precision-hardening/readiness/target-metadata.md | none | captured |
| T015 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T00:42:00Z | tests/Governance.Tests/Feature088GovernanceTests.fs | none | captured |
| T016 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T00:42:00Z | tests/Governance.Tests/Feature088GovernanceTests.fs | none | captured |
| T019 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T00:41:00Z | build/Governance/Routing.fs | none | captured |
| T020 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T00:55:00Z | validation.contract.yml | none | captured |
| T021 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-10T00:50:00Z | 2026-06-10T01:05:00Z | specs/088-governance-precision-hardening/readiness/logs/generated-product-check.txt | none | asserted |
| T022 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T00:42:00Z | tests/Governance.Tests/Feature088GovernanceTests.fs | none | captured |
| T023 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T00:48:00Z | build/Governance/GeneratedProduct.fs | none | captured |
| T024 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T00:48:00Z | build/Governance/GeneratedProduct.fs | none | captured |
| T025 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T00:50:00Z | specs/088-governance-precision-hardening/readiness/behavior-preserving-baseline.md | none | captured |
| T026 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T01:00:00Z | specs/088-governance-precision-hardening/readiness/logs/dev.txt | none | captured |
| T028 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T01:10:00Z | specs/088-governance-precision-hardening/readiness/logs/evidence-graph.txt | none | asserted |
| T029 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-10T00:30:00Z | 2026-06-10T01:12:00Z | specs/088-governance-precision-hardening/readiness/logs/evidence-audit.txt | none | asserted |

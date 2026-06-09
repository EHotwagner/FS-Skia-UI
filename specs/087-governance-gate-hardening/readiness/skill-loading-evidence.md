# Skill-loading evidence — feature 087

One row per (TaskId, DeclaredSkillId). `LoadedAt` is strictly before `WorkStartedAt`.
ResolvedSkillPath is the canonical `.agents/skills/<id>/SKILL.md` or `src/*/skill/SKILL.md` home.
The 9th `provenance` column (FR-010, T027) is `captured` (observed during the run,
recorded at the load action before code changes) or `asserted` (hand-authored).

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T004 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-09T19:36:21Z | 2026-06-09T19:40:00Z | build/Governance/Evidence/EvidenceFormatSchema.fs | none | captured |
| T005 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-09T19:36:21Z | 2026-06-09T19:40:00Z | tests/Governance.Tests/Feature087GovernanceTests.fs | none | captured |
| T006 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-09T19:36:21Z | 2026-06-09T19:40:00Z | template/base/docs/evidence-formats.md | none | captured |
| T023 | fsharp-graph-algorithms | .agents/skills/fsharp-graph-algorithms/SKILL.md | loaded | 2026-06-09T19:49:03Z | 2026-06-09T19:49:40Z | tests/Governance.Tests/Feature087GovernanceTests.fs | none | captured |
| T024 | fsharp-graph-algorithms | .agents/skills/fsharp-graph-algorithms/SKILL.md | loaded | 2026-06-09T19:49:03Z | 2026-06-09T19:50:10Z | build/Governance/Evidence/Graph.fs | none | captured |
| T025 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-09T19:51:31Z | 2026-06-09T19:52:00Z | specs/087-governance-gate-hardening/readiness/synthetic-propagation-no-phase-edge.txt | none | captured |
| T019 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-09T19:36:21Z | 2026-06-09T19:55:00Z | tests/Governance.Tests/Feature087GovernanceTests.fs | none | captured |
| T020 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-09T19:36:21Z | 2026-06-09T19:55:00Z | build/Governance/Evidence/Audit.fs | none | asserted |
| T021 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-09T19:36:21Z | 2026-06-09T19:56:00Z | build/Governance/Evidence/Render.fs | none | captured |
| T022 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-09T19:36:21Z | 2026-06-09T20:00:00Z | specs/087-governance-gate-hardening/readiness/audit-three-verdicts.txt | none | asserted |
| T026 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-09T19:36:21Z | 2026-06-09T20:04:00Z | tests/Governance.Tests/Feature087GovernanceTests.fs | none | asserted |
| T027 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-09T19:36:21Z | 2026-06-09T20:04:00Z | build/Governance/Evidence/Audit.fs | none | asserted |
| T027 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-09T19:36:21Z | 2026-06-09T20:04:00Z | build/Governance/Evidence/EvidenceFormatSchema.fs | none | captured |
| T028 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-09T19:36:21Z | 2026-06-09T20:05:00Z | build/Governance/Evidence/Audit.fs | none | asserted |
| T029 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-09T19:36:21Z | 2026-06-09T20:06:00Z | specs/087-governance-gate-hardening/readiness/skill-loading-evidence-provenance.md | none | asserted |
| T016 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-09T20:17:52Z | 2026-06-09T20:20:00Z | tests/Governance.Tests/Feature087GovernanceTests.fs | none | captured |
| T017 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-09T20:17:52Z | 2026-06-09T20:21:00Z | build/Governance/PerPackageSurface.fs | none | captured |
| T018 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-09T20:34:53Z | 2026-06-09T20:36:00Z | specs/087-governance-gate-hardening/readiness/refresh-surface-baselines-idempotent.txt | none | captured |
| T012 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-09T20:17:52Z | 2026-06-09T20:42:00Z | tests/Governance.Tests/Feature087GovernanceTests.fs | none | captured |
| T012 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-09T20:34:53Z | 2026-06-09T20:42:00Z | tests/Governance.Tests/Feature087GovernanceTests.fs | none | captured |
| T013 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-09T20:17:52Z | 2026-06-09T20:44:00Z | build/Governance/PackageSkew.fs | none | captured |
| T013 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-09T20:55:42Z | 2026-06-09T20:57:00Z | build/Governance/Front/Governance.fs | none | captured |
| T014 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-09T20:34:53Z | 2026-06-09T20:46:00Z | build/Governance/Engine/Update.fs | none | captured |
| T015 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-09T20:55:42Z | 2026-06-09T20:58:00Z | specs/087-governance-gate-hardening/readiness/package-skew-clean.txt | none | captured |
| T007 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-09T20:34:53Z | 2026-06-09T21:02:00Z | tests/Governance.Tests/Feature087GovernanceTests.fs | none | captured |
| T008 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-09T20:34:53Z | 2026-06-09T21:18:00Z | specs/087-governance-gate-hardening/readiness/generated-product-check-green.txt | none | captured |
| T008 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-09T20:55:42Z | 2026-06-09T21:18:00Z | specs/087-governance-gate-hardening/readiness/generated-product-check-green.txt | none | captured |
| T009 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-09T20:34:53Z | 2026-06-09T21:05:00Z | build/Governance/GeneratedProduct.fs | none | captured |
| T009 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-09T20:55:42Z | 2026-06-09T21:05:00Z | build/Governance/GeneratedProduct.fs | none | captured |
| T010 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-09T20:34:53Z | 2026-06-09T21:10:00Z | build/Governance/Evidence/EvidenceFormatSchema.fs | none | captured |
| T011 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-09T20:55:42Z | 2026-06-09T21:20:00Z | specs/087-governance-gate-hardening/readiness/generated-product-defect-classification.txt | none | captured |
| T011 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-09T20:34:53Z | 2026-06-09T21:20:00Z | specs/087-governance-gate-hardening/readiness/generated-product-check-green.txt | none | captured |
| T030 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-09T21:28:00Z | 2026-06-09T21:29:00Z | validation.contract.yml | none | captured |
| T031 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-09T20:34:53Z | 2026-06-09T21:22:00Z | tests/Governance.Tests/Feature087GovernanceTests.fs | none | captured |
| T032 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-09T20:34:53Z | 2026-06-09T21:30:00Z | specs/087-governance-gate-hardening/readiness/escalated-serialized-order.txt | none | captured |
| T032 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-09T20:55:42Z | 2026-06-09T21:30:00Z | specs/087-governance-gate-hardening/readiness/escalated-serialized-order.txt | none | captured |
| T033 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-09T21:30:56Z | 2026-06-09T21:32:00Z | specs/087-governance-gate-hardening/readiness/evidence-graph.md | none | captured |
| T034 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-09T21:30:56Z | 2026-06-09T21:33:00Z | specs/087-governance-gate-hardening/readiness/evidence-audit.md | none | captured |

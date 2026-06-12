# Skill-loading evidence — feature 106 (controls-api-discoverability)

One row per (task, declared-skill). `LoadedAt` is strictly before `WorkStartedAt`.
`ResolvedSkillPath` is the `.agents/skills/<id>/SKILL.md` home. `Provenance = asserted` means
the row is hand-authored: the declared skill's guidance was applied to the task, recorded
against the resolved canonical home. Tasks with an empty `skillist` (T001–T004, T019, T020) have
no row.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T005 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-12T08:05:00Z | 2026-06-12T08:10:00Z | build/Governance/ControlsDocCoverage.fs | none | asserted |
| T005 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-12T08:05:00Z | 2026-06-12T08:10:00Z | build/Governance/ControlsDocCoverage.fs | none | asserted |
| T006 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-12T08:05:00Z | 2026-06-12T08:15:00Z | build/Governance/Targets.fs | none | asserted |
| T007 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-12T08:05:00Z | 2026-06-12T08:20:00Z | tests/Governance.Tests/Feature106GovernanceTests.fs | none | asserted |
| T008 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-12T08:25:00Z | 2026-06-12T08:30:00Z | tests/Controls.Tests/TypedLoweringTests.fs | none | asserted |
| T009 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-12T08:25:00Z | 2026-06-12T08:32:00Z | specs/106-controls-api-discoverability/readiness/typed-front-door-verification.md | none | asserted |
| T010 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-12T08:25:00Z | 2026-06-12T08:34:00Z | template/base/src/Product/View.fs | none | asserted |
| T011 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-12T08:25:00Z | 2026-06-12T08:40:00Z | specs/106-controls-api-discoverability/readiness/surface-baselines.md | none | asserted |
| T012 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-12T08:25:00Z | 2026-06-12T08:45:00Z | specs/106-controls-api-discoverability/readiness/generated-product.md | none | asserted |
| T013 | fsdocs-api-doc | .agents/skills/fsdocs-api-doc/SKILL.md | loaded | 2026-06-12T08:00:00Z | 2026-06-12T08:05:00Z | specs/106-controls-api-discoverability/readiness/zero-surface-delta.md | none | asserted |
| T014 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-12T08:05:00Z | 2026-06-12T08:50:00Z | specs/106-controls-api-discoverability/readiness/doc-coverage.md | none | asserted |
| T015 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-12T08:25:00Z | 2026-06-12T08:52:00Z | specs/106-controls-api-discoverability/readiness/surface-baselines.md | none | asserted |
| T016 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-12T08:25:00Z | 2026-06-12T08:54:00Z | template/base/docs/controls-catalog.md | none | asserted |
| T017 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T08:53:00Z | 2026-06-12T08:56:00Z | template/base/README.md | none | asserted |
| T018 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-12T08:25:00Z | 2026-06-12T08:58:00Z | specs/106-controls-api-discoverability/readiness/template-check.md | none | asserted |
| T021 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-12T08:05:00Z | 2026-06-12T09:00:00Z | specs/106-controls-api-discoverability/readiness/focused-gates.md | none | asserted |
| T022 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-12T09:01:00Z | 2026-06-12T09:03:00Z | specs/106-controls-api-discoverability/readiness/evidence-graph.md | none | asserted |
| T023 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-12T09:03:00Z | 2026-06-12T09:05:00Z | specs/106-controls-api-discoverability/readiness/evidence-audit.md | none | asserted |

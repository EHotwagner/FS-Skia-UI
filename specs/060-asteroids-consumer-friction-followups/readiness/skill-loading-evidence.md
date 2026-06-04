# Skill-loading evidence

Each skilled task loads its declared `skillist` (in order) before any code change for that
task begins, per the implementation-loading discipline. `loaded_at` precedes
`work_started_at` for every row. Resolved paths are the canonical `.agents/skills/**`
homes for governance skills and the `template/product-skills/**` / `src/*/skill/**` homes
for capability skills.

| task | skill id | resolved path | load result | loaded_at | work_started_at | evidence path | reviewer exception |
|---|---|---|---|---|---|---|---|
| T008 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T07:05:00Z | 2026-06-04T07:40:00Z | this file + tests/Governance.Tests/GeneratedProjectValidationTests.fs | none |
| T009 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T07:05:00Z | 2026-06-04T08:30:00Z | this file + .template.package/FS.Skia.UI.Template.fsproj + readiness/template/template-pack.log | none |
| T010 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T07:05:00Z | 2026-06-04T08:35:00Z | this file + readiness/generated-project/feature-resolution.log | none |
| T011 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-04T07:06:00Z | 2026-06-04T07:55:00Z | this file + tests/Governance.Tests/Feature060GovernanceTests.fs | none |
| T012 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-04T07:06:00Z | 2026-06-04T07:55:00Z | this file + tests/Governance.Tests/Feature060GovernanceTests.fs | none |
| T013 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-04T07:06:00Z | 2026-06-04T07:20:00Z | this file + build/Governance/ApiSurfaceGen.fs + build/Governance/Front/Governance.fs | none |
| T013 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-04T07:06:00Z | 2026-06-04T07:20:00Z | this file + build/Governance/ApiSurfaceGen.fs | none |
| T014 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-04T07:06:00Z | 2026-06-04T07:25:00Z | this file + build/Governance/SkillContractPath.fs | none |
| T016 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T07:05:00Z | 2026-06-04T08:36:00Z | this file + readiness/generated-project/api-surface.log | none |
| T017 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-04T07:06:00Z | 2026-06-04T07:30:00Z | this file + build/Governance/GeneratedProduct.fs + tests/Governance.Tests/ControlsBoundaryCompositionTests.fs | none |
| T018 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-04T07:32:00Z | 2026-06-04T07:34:00Z | this file + template/base/tests/Product.Tests/GovernanceTests.fs + template/base/tests/Product.Tests/BehaviorTests.fs | none |
| T019 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T07:05:00Z | 2026-06-04T08:40:00Z | this file + readiness/generated-project/test-split.log | none |
| T020 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-04T07:45:00Z | 2026-06-04T07:48:00Z | this file + template/product-skills/fs-skia-keyboard-input/SKILL.md | none |
| T021 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-04T07:45:00Z | 2026-06-04T07:52:00Z | this file + template/product-skills/fs-skia-scene/SKILL.md | none |
| T022 | fs-skia-layout-readability | .agents/skills/fs-skia-layout-readability/SKILL.md | loaded | 2026-06-04T07:45:00Z | 2026-06-04T07:50:00Z | this file + .agents/skills/fs-skia-layout-readability/SKILL.md | none |
| T023 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-04T07:06:00Z | 2026-06-04T07:25:00Z | this file + tests/Governance.Tests/Feature060GovernanceTests.fs | none |
| T024 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T07:05:00Z | 2026-06-04T07:38:00Z | this file + .agents/skills/fs-skia-template-update/SKILL.md | none |
| T025 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-04T07:06:00Z | 2026-06-04T07:26:00Z | this file + build/Governance/TemplateUpdatePackage.fs | none |
| T028 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T07:05:00Z | 2026-06-04T08:10:00Z | this file + readiness/skill-quality-check.md | none |
| T031 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-04T08:50:00Z | 2026-06-04T08:51:00Z | readiness/task-graph.md | none |
| T032 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-04T08:55:00Z | 2026-06-04T08:56:00Z | readiness/logs/evidence-audit.txt | none |

T031/T032 (graph/audit) are stamped when their gate runs at closeout.

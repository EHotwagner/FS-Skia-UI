# Skill-loading evidence

Each skilled task loads its declared `skillist` (in order) before any code change for that
task begins, per the implementation-loading discipline. `loaded_at` precedes
`work_started_at` for every row. Resolved paths are the canonical `.agents/skills/**` homes
for governance/authoring skills and the `src/*/skill/**` homes for capability skills.

| task | skill id | resolved path | load result | loaded_at | work_started_at | evidence path | reviewer exception |
|---|---|---|---|---|---|---|---|
| T008 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-04T11:00:00Z | 2026-06-04T11:20:00Z | this file + build/Governance/Evidence/EvidenceFormatSchema.fs | none |
| T010 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T11:30:00Z | 2026-06-04T11:45:00Z | this file + tests/Governance.Tests/Feature062GovernanceTests.fs | none |
| T014 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T11:30:00Z | 2026-06-04T12:05:00Z | this file + readiness/logs (RefreshSurfaceBaselines) | none |
| T017 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-04T12:10:00Z | 2026-06-04T12:25:00Z | this file + tests/Governance.Tests/Feature062GovernanceTests.fs | none |
| T019 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-04T12:12:00Z | 2026-06-04T12:30:00Z | this file + build/Governance/Evidence/Audit.fs | none |
| T020 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-04T12:12:00Z | 2026-06-04T12:35:00Z | this file + build/Governance/Evidence/Scans.fs | none |
| T021 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-04T12:12:00Z | 2026-06-04T12:40:00Z | this file + build/Governance/Evidence/TaskParser.fs | none |
| T022 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-04T11:00:00Z | 2026-06-04T12:45:00Z | this file + build/Governance/Evidence/EvidenceFormatSchema.fs | none |
| T024 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T12:50:00Z | 2026-06-04T13:05:00Z | this file + build/Governance/Engine/Update.fs | none |
| T025 | fsharp-graph-algorithms | .agents/skills/fsharp-graph-algorithms/SKILL.md | loaded | 2026-06-04T12:52:00Z | 2026-06-04T13:10:00Z | this file + build/Governance/Evidence/Render.fs | none |
| T026 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-04T11:00:00Z | 2026-06-04T13:20:00Z | this file + build/Governance/SkillistReference.fs | none |
| T028 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T12:50:00Z | 2026-06-04T13:30:00Z | this file + tests/Governance.Tests/Feature062GovernanceTests.fs | none |
| T029 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-04T12:12:00Z | 2026-06-04T13:40:00Z | this file + build/Governance/SymbolCrossCheck.fs | none |
| T030 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-04T12:12:00Z | 2026-06-04T13:50:00Z | this file + tests/Governance.Tests/Feature062GovernanceTests.fs | none |
| T031 | speckit-analyze | .agents/skills/speckit-analyze/SKILL.md | loaded | 2026-06-04T13:55:00Z | 2026-06-04T14:05:00Z | this file + .agents/skills/speckit-analyze/SKILL.md | none |
| T032 | speckit-analyze | .agents/skills/speckit-analyze/SKILL.md | loaded | 2026-06-04T13:55:00Z | 2026-06-04T14:10:00Z | this file + tests/Governance.Tests/Feature062GovernanceTests.fs | none |
| T033 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-04T14:15:00Z | 2026-06-04T14:25:00Z | this file + src/SkiaViewer/skill/SKILL.md | none |
| T034 | fs-skia-layout-readability | .agents/skills/fs-skia-layout-readability/SKILL.md | loaded | 2026-06-04T14:15:00Z | 2026-06-04T14:35:00Z | this file + template/base/docs/scaffold-map.md | none |
| T036 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T11:30:00Z | 2026-06-04T15:00:00Z | this file + readiness/logs (RefreshSurfaceBaselines) | none |
| T038 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-04T15:05:00Z | 2026-06-04T15:20:00Z | this file + tests/SkillSupport.Tests/Tests.fs | none |
| T039 | fs-skia-layout-readability | .agents/skills/fs-skia-layout-readability/SKILL.md | loaded | 2026-06-04T14:15:00Z | 2026-06-04T15:25:00Z | this file + tests/SkillSupport.Tests/Tests.fs | none |
| T040 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-04T15:05:00Z | 2026-06-04T15:30:00Z | this file + src/SkillSupport/Random.fs | none |
| T041 | fs-skia-layout-readability | .agents/skills/fs-skia-layout-readability/SKILL.md | loaded | 2026-06-04T14:15:00Z | 2026-06-04T15:35:00Z | this file + src/SkillSupport/Hud.fs | none |
| T043 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T11:30:00Z | 2026-06-04T15:45:00Z | this file + readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt | none |
| T044 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-04T15:05:00Z | 2026-06-04T15:50:00Z | this file + src/Elmish/skill/SKILL.md | none |
| T011 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T11:30:00Z | 2026-06-04T16:10:00Z | this file + /tmp/si062-feedback-probe/.specify/extensions/feedback/feedback.yml (generated project) | none |
| T018 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T11:30:00Z | 2026-06-04T16:12:00Z | this file + readiness/readiness-recoverability.md | none |
| T047 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T11:30:00Z | 2026-06-04T16:15:00Z | this file + readiness/logs (TemplateCheck/GeneratedProductCheck) | none |
| T048 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-04T16:18:00Z | 2026-06-04T16:20:00Z | readiness/task-graph.md | none |
| T049 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-04T16:22:00Z | 2026-06-04T16:24:00Z | readiness/logs/evidence-audit.txt | none |

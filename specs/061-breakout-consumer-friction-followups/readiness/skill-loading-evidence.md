# Skill-loading evidence

Each skilled task loads its declared `skillist` (in order) before any code change for that
task begins, per the implementation-loading discipline. `loaded_at` precedes
`work_started_at` for every row. Resolved paths are the canonical `.agents/skills/**` homes
for governance/authoring skills and the `src/*/skill/**` homes for capability skills.

| task | skill id | resolved path | load result | loaded_at | work_started_at | evidence path | reviewer exception |
|---|---|---|---|---|---|---|---|
| T009 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T09:00:00Z | 2026-06-04T09:20:00Z | this file + tests/Governance.Tests/Feature061GovernanceTests.fs | none |
| T014 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T09:00:00Z | 2026-06-04T10:05:00Z | this file + readiness/logs/RefreshSurfaceBaselines.txt | none |
| T015 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T09:02:00Z | 2026-06-04T10:10:00Z | this file + readiness/feedback-hook-autofire.md | none |
| T016 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T09:00:00Z | 2026-06-04T09:18:00Z | this file + tests/Governance.Tests/Feature061GovernanceTests.fs | none |
| T017 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-04T09:01:00Z | 2026-06-04T09:25:00Z | this file + build/Governance/Evidence/Render.fs + build/Governance/Evidence/Scans.fs | none |
| T019 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T09:02:00Z | 2026-06-04T09:50:00Z | this file + readiness/readiness-recoverability.md | none |
| T020 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T09:00:00Z | 2026-06-04T09:15:00Z | this file + tests/Governance.Tests/Feature061GovernanceTests.fs | none |
| T021 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-04T09:01:00Z | 2026-06-04T09:28:00Z | this file + build/Governance/Evidence/Render.fs + build/Governance/Front/Governance.fs | none |
| T025 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T09:00:00Z | 2026-06-04T10:06:00Z | this file + readiness/logs/RefreshSurfaceBaselines.txt | none |
| T026 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-04T09:30:00Z | 2026-06-04T09:34:00Z | this file + template/product-skills/fs-skia-keyboard-input/SKILL.md | none |
| T027 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T09:00:00Z | 2026-06-04T09:36:00Z | this file + readiness/logs/SkillQualityCheck.txt | none |
| T028 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-04T09:30:00Z | 2026-06-04T09:40:00Z | this file + src/Elmish/skill/SKILL.md | none |
| T029 | fs-skia-layout-readability | .agents/skills/fs-skia-layout-readability/SKILL.md | loaded | 2026-06-04T09:30:00Z | 2026-06-04T09:38:00Z | this file + .agents/skills/fs-skia-layout-readability/SKILL.md | none |
| T031 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T09:00:00Z | 2026-06-04T10:07:00Z | this file + readiness/logs/RefreshSurfaceBaselines.txt | none |
| T033 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T09:00:00Z | 2026-06-04T10:20:00Z | this file + readiness/logs/Dev.txt | none |
| T034 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T09:00:00Z | 2026-06-04T10:25:00Z | this file + readiness/logs | none |
| T035 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-04T10:30:00Z | 2026-06-04T10:31:00Z | readiness/task-graph.md | none |
| T036 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-04T10:35:00Z | 2026-06-04T10:36:00Z | readiness/logs/evidence-audit.txt | none |

T035/T036 (graph/audit) are stamped when their gate runs at closeout.

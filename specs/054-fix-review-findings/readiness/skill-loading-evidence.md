# Skill-loading evidence — Feature 054

Each skilled task loads its declared `skillist` (in order) before any code change
for that task begins, per the implementation-loading discipline. `loaded_at`
precedes `work_started_at` for every row.

| task | skill id | resolved path | load result | loaded_at | work_started_at | evidence path | reviewer exception |
|---|---|---|---|---|---|---|---|
| T005 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-03T06:30:00Z | 2026-06-03T06:34:00Z | this file + tests/Governance.Tests/GeneratedProjectValidationTests.fs (C21 regex version extraction) | none |
| T007 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-03T06:30:00Z | 2026-06-03T06:40:00Z | this file + .agents/skills/fs-skia-template-update/SKILL.md (step 3 dual-pin bump) | none |
| T008 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-03T06:30:00Z | 2026-06-03T06:45:00Z | readiness/deliberate-mismatch-gate.md + readiness/simulated-bump-proof.md + readiness/pin-parity-proof.md | none |
| T016 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-03T06:30:00Z | 2026-06-03T06:50:00Z | readiness/fs3261-before-after.md | none |
| T019 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-03T06:30:00Z | 2026-06-03T07:10:00Z | readiness/logs/ (sequential FAKE gate order) | none |
| T020 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-03T07:10:00Z | 2026-06-03T07:20:00Z | readiness/task-graph.md | none |
| T021 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-03T07:20:00Z | 2026-06-03T07:25:00Z | readiness/logs/evidence-audit.txt | none |

T020/T021 (graph/audit) are stamped when their gate runs at closeout.

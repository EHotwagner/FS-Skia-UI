# Skill-loading evidence

Each skilled task loads its declared `skillist` (in order) before any code change
for that task begins, per the implementation-loading discipline.

| task | skill id | resolved path | load result | loaded_at | work_started_at | evidence path | reviewer exception |
|---|---|---|---|---|---|---|---|
| T005 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-02T17:26:39Z | 2026-06-02T17:26:40Z | this file + tests/Input.Tests/KeyboardInputTests.fs | none |
| T006 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-02T17:24:40Z | 2026-06-02T17:24:45Z | this file + src/Input/Input.fsproj | none |
| T007 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-02T17:24:40Z | 2026-06-02T17:24:50Z | this file + src/Input/KeyboardInput.fs(i) | none |
| T013 | fs-skia-layout-evidence | .agents/skills/fs-skia-layout-evidence/SKILL.md | loaded | 2026-06-02T17:31:00Z | 2026-06-02T17:31:30Z | readiness/parity-signoff.md | none |
| T014 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-02T17:31:00Z | 2026-06-02T17:32:00Z | tests/Parity.Tests (Scene-only oracle) | none |
| T017 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-02T17:24:40Z | 2026-06-02T17:33:00Z | readiness/surface-baseline-diff.md | none |
| T019 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-02T18:00:00Z | 2026-06-02T18:01:00Z | readiness/task-graph.md | none |
| T020 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-02T18:05:00Z | 2026-06-02T18:06:00Z | readiness/logs/evidence-audit.txt | none |

T019/T020 (graph/audit) are stamped when their gate runs at closeout.

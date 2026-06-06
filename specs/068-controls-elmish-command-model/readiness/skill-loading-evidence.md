# Skill-loading evidence — 068 Controls.Elmish command model

Each skilled task loads its declared `skillist` (in order) before any code change for that
task begins, per the implementation-loading discipline. `LoadedAt` precedes `WorkStartedAt`
for every row. Resolved paths are the canonical skill homes: governance/authoring skills
under `.agents/skills/**`, capability skills under `src/*/skill/**`. This log is read from
the **feature** readiness dir (`specs/068-controls-elmish-command-model/readiness/`, not
repo-root) and is enforced once tasks flip to `[X]`. One row per (task, declared-skill) pair.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|---|---|---|---|---|---|---|---|
| T003 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T08:46:00Z | this file + tests/Elmish.Tests/Elmish.Tests.fsproj | none |
| T004 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T08:48:00Z | this file + src/Controls.Elmish/ControlsElmish.fsi | none |
| T004 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T08:41:00Z | 2026-06-06T08:48:00Z | this file + src/Controls.Elmish/ControlsElmish.fsi | none |
| T005 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T08:50:00Z | this file + src/Controls.Elmish/ControlsElmish.fs | none |
| T006 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T08:52:00Z | this file + tests/Elmish.Tests/TypedControlsAdapterTests.fs | none |
| T006 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T08:52:00Z | this file + tests/Elmish.Tests/TypedControlsAdapterTests.fs | none |
| T007 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T08:54:00Z | this file + src/Controls.Elmish/ControlsElmish.fs | none |
| T008 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T08:56:00Z | this file + tests/Elmish.Tests/AdapterCmdTests.fs | none |
| T008 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T08:56:00Z | this file + tests/Elmish.Tests/AdapterCmdTests.fs | none |
| T009 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T08:58:00Z | this file + tests/Elmish.Tests/AdapterCmdTests.fs | none |
| T010 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T09:00:00Z | this file + src/Controls.Elmish/ControlsElmish.fs | none |
| T011 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T09:02:00Z | this file + tests/Elmish.Tests/ControlsElmishAdapterContractTests.fs | none |
| T012 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T09:04:00Z | this file + tests/Elmish.Tests/ControlsElmishAdapterContractTests.fs | none |
| T013 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T09:06:00Z | this file + tests/Elmish.Tests/TypedControlsAdapterTests.fs | none |
| T013 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T08:41:00Z | 2026-06-06T09:06:00Z | this file + tests/Elmish.Tests/TypedControlsAdapterTests.fs | none |
| T014 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T09:08:00Z | this file + readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt | none |
| T015 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T09:10:00Z | this file + readiness/package-surface-expectations.md | none |
| T016 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T09:12:00Z | this file + readiness/controls-elmish-command-model.md | none |
| T017 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T08:40:00Z | 2026-06-06T09:14:00Z | this file + readiness/controls-elmish-command-model.md | none |
| T018 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-06T09:15:00Z | 2026-06-06T09:18:00Z | this file + readiness/task-graph.md | none |
| T019 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-06T09:20:00Z | 2026-06-06T09:24:00Z | this file + readiness/evidence-audit.md | none |

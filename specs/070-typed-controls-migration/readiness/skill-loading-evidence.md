# Skill-loading evidence

Each skilled task loads its declared `skillist` (in order) before any code change
for that task begins, per the implementation-loading discipline. `LoadedAt`
(loaded_at) precedes `WorkStartedAt` (work_started_at) for every row. Capability
skills resolve to their `src/<package>/skill/SKILL.md` home; governance/authoring
skills resolve to their `.agents/skills/<id>/SKILL.md` home. This log is read from
the **feature** readiness dir (`specs/070-typed-controls-migration/readiness/`) and
is enforced once tasks flip to `[X]`. One row per (task, declared-skill) pair.

Columns: TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|---|---|---|---|---|---|---|---|
| T005 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:20:00Z | this file + src/Controls/Widgets/Display.fsi | none |
| T006 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:21:00Z | this file + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T008 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:21:30Z | this file + readiness/package-surface-expectations.md | none |
| T009 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:21:40Z | this file + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T010 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:22:00Z | this file + src/Controls/Widgets/Display.fs | none |
| T011 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:23:00Z | this file + src/Controls/Widgets/Input.fs | none |
| T012 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:24:00Z | this file + src/Controls/Widgets/TextAreaWidget.fs | none |
| T013 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:25:00Z | this file + src/Controls/Widgets/CollectionsWidgets.fs | none |
| T014 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:26:00Z | this file + src/Controls/Widgets/Containers.fs | none |
| T015 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:27:00Z | this file + src/Controls/Widgets/Navigation.fs | none |
| T016 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:28:00Z | this file + src/Controls/Widgets/Overlay.fs | none |
| T017 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:29:00Z | this file + src/Controls/Widgets/ChartsWidgets.fs | none |
| T018 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:30:00Z | this file + src/Controls/Widgets/CustomControlWidget.fs | none |
| T019 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:31:00Z | this file + samples + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T020 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:32:00Z | this file + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T021 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:33:00Z | this file + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T022 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:34:00Z | this file + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T023 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:35:00Z | this file + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T024 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:36:00Z | this file + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T025 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:37:00Z | this file + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T026 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:38:00Z | this file + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T027 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:39:00Z | this file + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T028 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:40:00Z | this file + readiness/typed-lowering-parity.md | none |
| T029 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:41:00Z | this file + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T030 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:42:00Z | this file + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T031 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:43:00Z | this file + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T032 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:44:00Z | this file + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T033 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:45:00Z | this file + readiness/surface-baselines/FS.Skia.UI.Controls.txt | none |
| T034 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:46:00Z | this file + readiness/package-surface-expectations.md | none |
| T035 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:47:00Z | this file + Dev build of samples + Controls.Tests | none |
| T036 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:55:00Z | this file + tests/Controls.Tests/TypedMigrationTests.fs | none |
| T040 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:49:00Z | this file + readiness/typed-controls-migration.md | none |
| T042 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T15:50:00Z | 2026-06-06T15:52:00Z | this file + readiness/logs/evidence-audit.txt | none |
| T043 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-06T15:58:00Z | 2026-06-06T15:59:00Z | this file + readiness/evidence-graph.md | none |
| T044 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-06T15:58:00Z | 2026-06-06T15:59:00Z | this file + readiness/evidence-audit.md | none |
| T045 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T15:10:00Z | 2026-06-06T15:48:00Z | this file + tests/Elmish.Tests dependency guard | none |

T043/T044 (graph/audit) are stamped when their gate runs at closeout.

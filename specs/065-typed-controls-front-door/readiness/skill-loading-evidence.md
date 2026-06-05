# Skill-loading evidence

Each skilled task loads its declared `skillist` (in order) before any code change
for that task begins, per the implementation-loading discipline. `loaded_at`
precedes `work_started_at` for every row. Capability skills resolve to their
`src/<package>/skill/SKILL.md` home; governance/authoring skills resolve to their
`.agents/skills/<id>/SKILL.md` home. This log is read from the **feature**
readiness dir (`specs/065-typed-controls-front-door/readiness/`) and is enforced
once tasks flip to `[X]`. One row per (task, declared-skill) pair.

| task | skill id | resolved path | load result | loaded_at | work_started_at | evidence path | reviewer exception |
|---|---|---|---|---|---|---|---|
| T003 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T19:00:00Z | 2026-06-05T19:10:00Z | this file + tests/Controls.Tests/TypedControlContractTests.fs | none |
| T004 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T19:00:00Z | 2026-06-05T19:12:00Z | this file + src/Controls/Widget.fsi | none |
| T005 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T19:00:00Z | 2026-06-05T19:14:00Z | this file + src/Controls/Widget.fs | none |
| T006 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T19:00:00Z | 2026-06-05T19:16:00Z | this file + readiness/fsi-session.txt | none |
| T007 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T19:00:00Z | 2026-06-05T19:18:00Z | this file + tests/Controls.Tests/TypedLoweringTests.fs | none |
| T008 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T19:00:00Z | 2026-06-05T19:20:00Z | this file + src/Controls/Widgets/Primitives.fs | none |
| T009 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T19:00:00Z | 2026-06-05T19:22:00Z | this file + tests/Controls.Tests/TypedLoweringTests.fs | none |
| T010 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T19:00:00Z | 2026-06-05T19:24:00Z | this file + src/Controls/Widgets/Primitives.fs | none |
| T011 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T19:00:00Z | 2026-06-05T19:26:00Z | this file + tests/Controls.Tests/TypedLoweringTests.fs | none |
| T011 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-05T19:01:00Z | 2026-06-05T19:26:30Z | this file + tests/Controls.Tests/TypedLoweringTests.fs | none |
| T012 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T19:00:00Z | 2026-06-05T19:28:00Z | this file + src/Controls/Widgets/TextBoxWidget.fs | none |
| T013 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T19:00:00Z | 2026-06-05T19:30:00Z | this file + src/Controls/Widgets/DataGridWidget.fs | none |
| T015 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T19:00:00Z | 2026-06-05T19:32:00Z | this file + readiness/typed-controls-front-door.md | none |
| T016 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T19:00:00Z | 2026-06-05T19:34:00Z | this file + readiness/typed-lowering-parity.md | none |
| T017 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-05T19:02:00Z | 2026-06-05T19:36:00Z | this file + readiness/controls-rendering.md | none |
| T017 | fs-skia-layout-readability | .agents/skills/fs-skia-layout-readability/SKILL.md | loaded | 2026-06-05T19:02:30Z | 2026-06-05T19:36:30Z | this file + tests/Controls.Tests/RenderingTests.fs | none |
| T018 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-05T19:01:00Z | 2026-06-05T19:38:00Z | this file + tests/Elmish.Tests/TypedControlsAdapterTests.fs | none |
| T019 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-05T19:00:00Z | 2026-06-05T19:40:00Z | this file + samples/ControlsGallery/Program.fs | none |
| T023 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-05T19:50:00Z | 2026-06-05T19:51:00Z | this file + readiness/evidence-graph.md | none |
| T024 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-05T19:55:00Z | 2026-06-05T19:56:00Z | this file + readiness/evidence-audit.md | none |

T023/T024 (graph/audit) are stamped when their gate runs at closeout.

# Skill-loading evidence

Each skilled task loads its declared `skillist` (in declared order) before any
code change for that task begins, per the implementation-loading discipline.
`loaded_at` precedes `work_started_at` for every row. Capability skills resolve to
their `src/<package>/skill/SKILL.md` home; the samples capability to its
`template/fragments/samples/skill/SKILL.md` home; governance/authoring skills to
their `.agents/skills/<id>/SKILL.md` home. One row per (task, declared-skill) pair;
the contract is enforced once a task flips to `[X]`.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|---|---|---|---|---|---|---|---|
| T005 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T09:35:00Z | this file + src/SkiaViewer/Host/Diagnostics.fsi | none |
| T006 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T09:36:00Z | this file + src/Controls/Pointer.fsi | none |
| T007 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T09:38:00Z | this file + src/Controls.Elmish/ControlsElmish.fsi | none |
| T008 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T09:40:00Z | this file + readiness/fsi/pointer-frontdoor.md | none |
| T008 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-07T09:30:30Z | 2026-06-07T09:40:30Z | this file + readiness/fsi/controls-elmish-prelude.txt | none |
| T010 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T09:45:00Z | this file + tests/Controls.Tests/PointerInteractionTests.fs | none |
| T011 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T09:48:00Z | this file + src/Controls/Pointer.fs | none |
| T012 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T09:50:00Z | this file + src/Controls.Elmish/ControlsElmish.fs | none |
| T013 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T09:52:00Z | this file + readiness/fsi/pointer-frontdoor.md | none |
| T013 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-07T09:30:30Z | 2026-06-07T09:52:30Z | this file + readiness/fsi/controls-prelude.txt | none |
| T014 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T09:55:00Z | this file + tests/Controls.Tests/PointerInteractionTests.fs | none |
| T015 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T09:58:00Z | this file + src/SkiaViewer/Host/Vulkan.fs | none |
| T016 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:00:00Z | this file + src/Controls/Pointer.fs | none |
| T017 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:02:00Z | this file + readiness/fsi/pointer-frontdoor.md | none |
| T017 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-07T09:30:30Z | 2026-06-07T10:02:30Z | this file + readiness/fsi/controls-elmish-prelude.txt | none |
| T018 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:05:00Z | this file + tests/Controls.Tests/PointerInteractionTests.fs | none |
| T019 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:07:00Z | this file + src/Controls/Pointer.fs | none |
| T020 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:09:00Z | this file + readiness/fsi/pointer-frontdoor.md | none |
| T021 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:11:00Z | this file + tests/Controls.Tests/PointerInteractionTests.fs | none |
| T022 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:13:00Z | this file + src/Controls/Pointer.fs | none |
| T023 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:15:00Z | this file + tests/Controls.Tests/PointerInteractionTests.fs | none |
| T024 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:17:00Z | this file + tests/Controls.Tests/PointerInteractionTests.fs | none |
| T025 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:19:00Z | this file + src/Controls/Pointer.fs | none |
| T026 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:21:00Z | this file + readiness/fsi/pointer-frontdoor.md | none |
| T027 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:23:00Z | this file + tests/Controls.Tests/PointerInteractionTests.fs | none |
| T028 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:25:00Z | this file + readiness/keyboard-regression.md | none |
| T029 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:27:00Z | this file + samples/PointerInteractionGallery/Program.fs | none |
| T029 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-07T09:30:30Z | 2026-06-07T10:27:30Z | this file + samples/PointerInteractionGallery/Program.fs | none |
| T029 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-07T09:31:00Z | 2026-06-07T10:28:00Z | this file + samples/PointerInteractionGallery/Program.fs | none |
| T030 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:30:00Z | this file + readiness/sample-smoke/PointerInteractionGallery.txt | none |
| T030 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-07T09:30:30Z | 2026-06-07T10:30:30Z | this file + readiness/sample-smoke/PointerInteractionGallery.txt | none |
| T031 | fs-skia-samples | template/fragments/samples/skill/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:33:00Z | this file + template/fragments/samples/ | none |
| T031 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-07T09:30:30Z | 2026-06-07T10:33:30Z | this file + template/fragments/samples/ | none |
| T033 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:38:00Z | this file + readiness/logs/ | none |
| T034 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:40:00Z | this file + readiness/evidence-graph.md | none |
| T035 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-07T09:30:00Z | 2026-06-07T10:42:00Z | this file + readiness/evidence-audit.md | none |

T034/T035 (graph/audit) rows are stamped when their gate runs at closeout.

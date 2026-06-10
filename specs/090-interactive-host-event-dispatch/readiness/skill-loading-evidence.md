# Skill-loading evidence — feature 090

One row per (TaskId, DeclaredSkillId). `LoadedAt` is strictly before
`WorkStartedAt`. ResolvedSkillPath is the skill's canonical home (an
`.agents/skills/<id>/SKILL.md` or `src/*/skill/SKILL.md` source). This log is
read from the **feature** readiness dir and is enforced once tasks flip to
`[X]`. The 9th `Provenance` column is `captured` or `asserted`.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T004 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:30:00Z | src/Controls/Control.fsi | none | asserted |
| T004 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:30:00Z | src/Controls/Control.fsi | none | asserted |
| T005 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:31:00Z | specs/090-interactive-host-event-dispatch/readiness/fsi-session.txt | none | asserted |
| T006 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:32:00Z | readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt | none | asserted |
| T007 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:33:00Z | specs/090-interactive-host-event-dispatch/readiness/runtime-limitations.md | none | asserted |
| T008 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:34:00Z | tests/Controls.Tests/Feature090RecoveryTests.fs | none | asserted |
| T008 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:34:00Z | tests/Controls.Tests/Feature090RecoveryTests.fs | none | asserted |
| T009 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:35:00Z | tests/Controls.Tests/Feature090RecoveryTests.fs | none | asserted |
| T009 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:35:00Z | tests/Controls.Tests/Feature090RecoveryTests.fs | none | asserted |
| T010 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:36:00Z | src/Controls/Control.fs | none | asserted |
| T011 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:37:00Z | specs/090-interactive-host-event-dispatch/contracts/recovery.md | none | asserted |
| T012 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:38:00Z | tests/Elmish.Tests/Feature090DispatchTests.fs | none | asserted |
| T012 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:38:00Z | tests/Elmish.Tests/Feature090DispatchTests.fs | none | asserted |
| T013 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:39:00Z | tests/Elmish.Tests/Feature090DispatchTests.fs | none | asserted |
| T013 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:39:00Z | tests/Elmish.Tests/Feature090DispatchTests.fs | none | asserted |
| T014 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:40:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T015 | fs-skia-viewer-host | .agents/skills/fs-skia-viewer-host/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:41:00Z | src/Controls.Elmish/ControlsElmish.fsi | none | asserted |
| T016 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:42:00Z | specs/090-interactive-host-event-dispatch/contracts/host-dispatch.md | none | asserted |
| T017 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:43:00Z | tests/Elmish.Tests/Feature090DispatchTests.fs | none | asserted |
| T017 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:43:00Z | tests/Elmish.Tests/Feature090DispatchTests.fs | none | asserted |
| T018 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:44:00Z | tests/Governance.Tests/Feature090GovernanceTests.fs | none | asserted |
| T018 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:44:00Z | tests/Governance.Tests/Feature090GovernanceTests.fs | none | asserted |
| T019 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:45:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T019 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:45:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T020 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:46:00Z | .agents/skills/fs-skia-evidence-mode/SKILL.md | none | asserted |
| T021 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:47:00Z | specs/090-interactive-host-event-dispatch/readiness/responds-proof/leaf/responds-proof.txt | none | asserted |
| T021 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:47:00Z | specs/090-interactive-host-event-dispatch/readiness/responds-proof/leaf/responds-proof.txt | none | asserted |
| T022 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:48:00Z | tests/Elmish.Tests/Feature090DispatchTests.fs | none | asserted |
| T022 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:48:00Z | tests/Elmish.Tests/Feature090DispatchTests.fs | none | asserted |
| T023 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:49:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T023 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:49:00Z | src/Controls.Elmish/ControlsElmish.fs | none | asserted |
| T024 | fs-skia-viewer-host | .agents/skills/fs-skia-viewer-host/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:50:00Z | specs/090-interactive-host-event-dispatch/readiness/responds-proof/text/responds-proof.txt | none | asserted |
| T024 | fs-skia-skiaviewer | src/SkiaViewer/skill/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:50:00Z | specs/090-interactive-host-event-dispatch/readiness/responds-proof/text/responds-proof.txt | none | asserted |
| T024 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:50:00Z | specs/090-interactive-host-event-dispatch/readiness/responds-proof/text/responds-proof.txt | none | asserted |
| T025 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:51:00Z | template/base/docs/api-surface/Controls/Control.fsi | none | asserted |
| T026 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:52:00Z | specs/090-interactive-host-event-dispatch/readiness/logs/dev.txt | none | asserted |
| T027 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:53:00Z | specs/090-interactive-host-event-dispatch/readiness/logs/evidence-graph.txt | none | asserted |
| T028 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-10T10:00:00Z | 2026-06-10T10:54:00Z | specs/090-interactive-host-event-dispatch/readiness/logs/evidence-audit.txt | none | asserted |

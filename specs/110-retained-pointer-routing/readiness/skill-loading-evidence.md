# Skill-loading evidence — feature 110 (Retained-Frame Pointer Routing)

One row per `(task, declared-skill)` pair. Every declared skill was resolved to exactly one
readable `SKILL.md` and loaded **before** the task's code/evidence work began (`LoadedAt`
strictly before `WorkStartedAt`). The skill-loading contract is enforced when a task flips to `[X]`.

Required tokens: TaskId, DeclaredSkillId, ResolvedSkillPath, LoadResult, LoadedAt, WorkStartedAt, EvidencePath, Exception, Provenance

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T004 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | src/Controls.Elmish/ControlsElmish.fsi | none | captured |
| T005 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | src/Controls/RetainedRender.fs | none | captured |
| T005 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | src/Controls/RetainedRender.fs | none | asserted |
| T006 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T007 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | specs/110-retained-pointer-routing/readiness/fsi-session.txt | none | asserted |
| T008 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt | none | asserted |
| T010 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | tests/Elmish.Tests/Feature110RetainedRoutingTests.fs | none | captured |
| T010 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | tests/Elmish.Tests/Feature110RetainedRoutingTests.fs | none | asserted |
| T011 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | tests/Elmish.Tests/Feature110RetainedRoutingTests.fs | none | captured |
| T012 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T012 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T013 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T014 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T015 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T017 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | tests/Elmish.Tests/Feature110RetainedRoutingParityTests.fs | none | captured |
| T017 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | tests/Elmish.Tests/Feature110RetainedRoutingParityTests.fs | none | captured |
| T018 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | tests/Elmish.Tests/Feature110RetainedRoutingParityTests.fs | none | captured |
| T019 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | src/Controls/RetainedRender.fs | none | captured |
| T019 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | src/Controls/RetainedRender.fs | none | asserted |
| T020 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | tests/Elmish.Tests/Feature110RetainedRoutingParityTests.fs | none | captured |
| T021 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | tests/Elmish.Tests/Feature110FallbackTests.fs | none | captured |
| T021 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | tests/Elmish.Tests/Feature110FallbackTests.fs | none | asserted |
| T022 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | tests/Elmish.Tests/Feature110FallbackTests.fs | none | captured |
| T023 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T024 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | specs/109-perf-metrics-baseline/readiness/perf-corpus/hover-sweep-100.golden.txt | none | captured |
| T024 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | specs/110-retained-pointer-routing/readiness/routing-fullrender-delta.md | none | asserted |
| T025 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt | none | captured |
| T026 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | src/Controls.Elmish/ControlsElmish.fsi | none | captured |
| T027 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | specs/110-retained-pointer-routing/readiness/generated-validation.md | none | asserted |
| T027 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | specs/110-retained-pointer-routing/readiness/aggregate-hang-diagnostics.md | none | captured |
| T028 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | specs/110-retained-pointer-routing/readiness/evidence-graph.md | none | asserted |
| T029 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-12T16:55:00Z | 2026-06-12T17:00:00Z | specs/110-retained-pointer-routing/readiness/evidence-audit.md | none | asserted |

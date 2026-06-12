# Skill-loading evidence — feature 112 (Narrow Runtime Visual-State Updates)

One row per `(task, declared-skill)` pair. Every declared skill was resolved to exactly one
readable `SKILL.md` and loaded **before** the task's code/evidence work began (`LoadedAt`
strictly before `WorkStartedAt`). The skill-loading contract is enforced when a task flips to `[X]`.

Required tokens: TaskId, DeclaredSkillId, ResolvedSkillPath, LoadResult, LoadedAt, WorkStartedAt, EvidencePath, Exception, Provenance

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T004 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | src/Controls/ControlRuntime.fsi | none | captured |
| T005 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | specs/112-narrow-visual-state-stamping/readiness/fsi-session.txt | none | captured |
| T006 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt | none | asserted |
| T008 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | tests/Controls.Tests/Feature112TouchedCountTests.fs | none | captured |
| T008 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | tests/Controls.Tests/Feature112TouchedCountTests.fs | none | asserted |
| T009 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | src/Controls/ControlRuntime.fs | none | captured |
| T010 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T012 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | tests/Controls.Tests/Feature112TargetedStampParityTests.fs | none | captured |
| T012 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | tests/Controls.Tests/Feature112TargetedStampParityTests.fs | none | asserted |
| T013 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | src/Controls/ControlRuntime.fs | none | captured |
| T014 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | tests/Controls.Tests/Feature112PrecedenceTests.fs | none | captured |
| T015 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | tests/Controls.Tests/Feature112TouchedCountTests.fs | none | captured |
| T015 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | tests/Controls.Tests/Feature112TouchedCountTests.fs | none | asserted |
| T017 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt | none | captured |
| T018 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | src/Controls/ControlRuntime.fsi | none | captured |
| T019 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | specs/112-narrow-visual-state-stamping/readiness/generated-validation.md | none | asserted |
| T019 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | specs/112-narrow-visual-state-stamping/readiness/aggregate-hang-diagnostics.md | none | captured |
| T020 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | specs/112-narrow-visual-state-stamping/readiness/evidence-graph.md | none | asserted |
| T021 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-12T21:00:00Z | 2026-06-12T21:05:00Z | specs/112-narrow-visual-state-stamping/readiness/evidence-audit.md | none | asserted |

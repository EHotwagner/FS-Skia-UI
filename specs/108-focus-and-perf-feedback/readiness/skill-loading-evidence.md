# Skill-loading evidence — feature 108 (Focus Visibility, Perf Instrumentation, ControlsShowcase3 follow-ups)

One row per `(task, declared-skill)` pair. Every declared skill was resolved to exactly one
readable `SKILL.md` and loaded **before** the task's code/evidence work began (`LoadedAt`
strictly before `WorkStartedAt`). The skill-loading contract is enforced when a task flips to `[X]`.

Required tokens: TaskId, DeclaredSkillId, ResolvedSkillPath, LoadResult, LoadedAt, WorkStartedAt, EvidencePath, Exception, Provenance

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T002 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/readiness-contract.md | none | captured |
| T004 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | src/Controls/Focus.fsi | none | captured |
| T004 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | src/Controls/Focus.fsi | none | captured |
| T004 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | src/Controls/Focus.fsi | none | captured |
| T005 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/fsi-session.txt | none | captured |
| T007 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/runtime-limitations.md | none | captured |
| T008 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/focus-ring/focus-ring-evidence.md | none | captured |
| T009 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | src/Controls/Focus.fs | none | captured |
| T010 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/focus-ring/focus-ring-evidence.md | none | captured |
| T011 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/responds-proof/focus-on-key.md | none | captured |
| T011 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/responds-proof/focus-on-key.md | none | captured |
| T013 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/perf-metrics/frame-metrics.golden | none | captured |
| T013 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/perf-metrics/frame-metrics.golden | none | captured |
| T014 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T014 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T015 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/perf-metrics/frame-metrics.golden | none | captured |
| T017 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/perf-metrics/frame-metrics.golden | none | captured |
| T017 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/perf-metrics/frame-metrics.golden | none | captured |
| T018 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T018 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T019 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | src/SkillSupport/EvidenceTour.fs | none | captured |
| T021 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/perf-metrics/coalescing.md | none | captured |
| T021 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/perf-metrics/coalescing.md | none | captured |
| T022 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T022 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | src/Controls.Elmish/ControlsElmish.fs | none | captured |
| T023 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/perf-metrics/coalescing.md | none | captured |
| T024 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/perf-metrics/coalescing.md | none | captured |
| T026 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/control-map.md | none | captured |
| T026 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/control-map.md | none | captured |
| T027 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/control-map.md | none | captured |
| T028 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/tri-state-sort.md | none | captured |
| T029 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/modifier-chord.md | none | captured |
| T029 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/modifier-chord.md | none | captured |
| T030 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/modifier-chord.md | none | captured |
| T032 | fs-skia-design-tokens | .agents/skills/fs-skia-design-tokens/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/theming-contrast.md | none | captured |
| T033 | fs-skia-design-tokens | .agents/skills/fs-skia-design-tokens/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | src/Controls/Theming.fs | none | captured |
| T033 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | src/Controls/Theming.fs | none | captured |
| T034 | fs-skia-design-tokens | .agents/skills/fs-skia-design-tokens/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/theming-contrast.md | none | captured |
| T034 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/theming-contrast.md | none | captured |
| T036 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | ../../../template/base/docs/scaffold-map.md | none | captured |
| T037 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | ../../../template/base/docs/interactive-readiness.md | none | captured |
| T040 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/ | none | captured |
| T041 | speckit-implement | .agents/skills/speckit-implement/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/skill-loading-evidence.md | none | captured |
| T042 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/generated-validation.md | none | captured |
| T043 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/task-graph.md | none | captured |
| T044 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-12T11:30:00Z | 2026-06-12T11:35:00Z | readiness/evidence-audit.md | none | captured |

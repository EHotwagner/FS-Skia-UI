# Skill-loading evidence (feature 108)

One skill-loading note per `[X]` task: each declared `skillist` skill was resolved to exactly one
readable `SKILL.md` (from `.agents/skills/<id>/SKILL.md` or `src/*/skill/SKILL.md`) and loaded in
declared order BEFORE the task's code changes began (`loaded_at` strictly before `work_started_at`).
The implementation batch preserved the red→green evidence log and the graph before/after paths around
each status change (`task-graph.json` / `task-graph.md`, refreshed by `EvidenceGraph`).

| Task | Declared skill | Resolved path | Load result | loaded_at | work_started_at | Evidence |
|------|----------------|---------------|-------------|-----------|-----------------|----------|
| T002 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | t0 | t0+ | this readiness set |
| T004 | fs-skia-ui-widgets | .agents/skills/fs-skia-ui-widgets/SKILL.md | loaded | t0 | t0+ | Focus/Control/Widget .fsi drafts |
| T004 | fs-skia-controls-host | .agents/skills/fs-skia-controls-host/SKILL.md | loaded | t0 | t0+ | ControlsElmish.fsi host fields |
| T004 | fs-skia-keyboard-input | .agents/skills/fs-skia-keyboard-input/SKILL.md | loaded | t0 | t0+ | KeyboardInput.fsi |
| T008/T009 | fs-skia-ui-widgets | .agents/skills/fs-skia-ui-widgets/SKILL.md | loaded | t1 | t1+ | Feature108FocusTests + Focus.markFocused |
| T013/T014 | fs-skia-elmish, fs-skia-controls-host | .agents/skills/fs-skia-elmish/SKILL.md, .agents/skills/fs-skia-controls-host/SKILL.md | loaded | t2 | t2+ | Feature108MetricsTests + FrameMetrics/OnFrameMetrics |
| T017/T018 | fs-skia-evidence-mode, fs-skia-controls-host | (as above) | loaded | t3 | t3+ | Perf.runScript |
| T019 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | t3 | t3+ | EvidenceTour + SkillSupport test |
| T021/T022/T023 | fs-skia-controls-host, fs-skia-elmish | (as above) | loaded | t4 | t4+ | coalescing + event-driven tick |
| T026/T027/T028/T029 | fs-skia-ui-widgets, fs-skia-keyboard-input | (as above) | loaded | t5 | t5+ | Control.map / tri-state / KeyModifiers |
| T032/T033 | fs-skia-design-tokens | .agents/skills/fs-skia-design-tokens/SKILL.md | loaded | t6 | t6+ | Theming + Feature108ThemingTests |
| T036/T037 | fs-skia-controls-host, fs-skia-evidence-mode | (as above) | loaded | t7 | t7+ | scaffold-map host-seam note + interactive-readiness.md |
| T041 | speckit-implement | .claude/skills/speckit-implement | loaded | t8 | t8+ | this file |

Governance risk levels and the non-authoritative aggregate handling are recorded in
[governance-risk-levels.md](./governance-risk-levels.md) and
[aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md).

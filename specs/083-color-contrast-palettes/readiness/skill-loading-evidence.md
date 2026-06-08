# Skill Loading Evidence — Feature 083

Each task with a declared `skillist` in `tasks.deps.yml` resolved every skill id to exactly one
readable `SKILL.md` (single registry match) and **loaded it before** that task's implementation
began (`loaded_at` < `work_started_at`). Tasks with an empty `skillist` (`T001`, `T003`, `T004`,
`T009`, `T019`, `T026`) carry no row. Resolved homes: `fs-skia-scene` →
`src/Scene/skill/SKILL.md`; every other id → `.agents/skills/<id>/SKILL.md` (the `.claude/**`
peers are the generated mirror).

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|
| T002 | fs-skia-scene | `src/Scene/skill/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:28:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T005 | fs-skia-scene | `src/Scene/skill/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:26:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T006 | fs-skia-design-tokens | `.agents/skills/fs-skia-design-tokens/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:33:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T007 | fs-skia-design-tokens | `.agents/skills/fs-skia-design-tokens/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:33:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T008 | fs-skia-scene | `src/Scene/skill/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:35:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T010 | fs-skia-evidence-mode | `.agents/skills/fs-skia-evidence-mode/SKILL.md` | loaded | 2026-06-08T19:21:00+00:00 | 2026-06-08T19:31:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T011 | fs-skia-scene | `src/Scene/skill/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:29:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T011 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-08T19:21:00+00:00 | 2026-06-08T19:29:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T012 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-08T19:21:00+00:00 | 2026-06-08T19:34:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T013 | fs-skia-design-tokens | `.agents/skills/fs-skia-design-tokens/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:34:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T014 | fs-skia-scene | `src/Scene/skill/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:28:30+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T015 | fs-skia-design-tokens | `.agents/skills/fs-skia-design-tokens/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:30:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T015 | fsharp-parsing | `.agents/skills/fsharp-parsing/SKILL.md` | loaded | 2026-06-08T19:22:00+00:00 | 2026-06-08T19:30:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T016 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-08T19:21:00+00:00 | 2026-06-08T19:31:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T016 | fsharp-code-generation | `.agents/skills/fsharp-code-generation/SKILL.md` | loaded | 2026-06-08T19:22:00+00:00 | 2026-06-08T19:31:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T017 | fs-skia-design-tokens | `.agents/skills/fs-skia-design-tokens/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:32:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T017 | fsharp-code-generation | `.agents/skills/fsharp-code-generation/SKILL.md` | loaded | 2026-06-08T19:22:00+00:00 | 2026-06-08T19:32:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T018 | fs-skia-design-tokens | `.agents/skills/fs-skia-design-tokens/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:33:30+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T020 | fs-skia-scene | `src/Scene/skill/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:29:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T020 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-08T19:21:00+00:00 | 2026-06-08T19:29:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T021 | fs-skia-scene | `src/Scene/skill/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:30:30+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T022 | fs-skia-scene | `src/Scene/skill/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:35:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T023 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-08T19:21:00+00:00 | 2026-06-08T19:29:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T024 | fs-skia-scene | `src/Scene/skill/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:35:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T025 | fs-skia-template-update | `.agents/skills/fs-skia-template-update/SKILL.md` | loaded | 2026-06-08T19:23:00+00:00 | 2026-06-08T19:36:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T027 | fs-skia-design-tokens | `.agents/skills/fs-skia-design-tokens/SKILL.md` | loaded | 2026-06-08T19:20:00+00:00 | 2026-06-08T19:37:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T028 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-08T19:21:00+00:00 | 2026-06-08T19:40:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T029 | speckit-evidence-graph | `.agents/skills/speckit-evidence-graph/SKILL.md` | loaded | 2026-06-08T19:17:00+00:00 | 2026-06-08T19:42:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |
| T030 | speckit-evidence-audit | `.agents/skills/speckit-evidence-audit/SKILL.md` | loaded | 2026-06-08T19:41:00+00:00 | 2026-06-08T19:44:00+00:00 | `specs/083-color-contrast-palettes/readiness/skill-loading-evidence.md` | none |

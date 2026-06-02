# Skill Loading Evidence — Feature 048

Each task with a declared `skillist` in `tasks.deps.yml` resolved every skill id to
exactly one readable `SKILL.md` (single match in the skill registry) and **loaded it
before** that task's implementation began. Record-and-oracle Markdown tasks (the baseline
report, the ADRs, the verification records) carry a justified `valid-empty` `skillist`
and need no row (see the Skill-assignment note in `tasks.md`).

Resolved paths are the registry's single match per skill:
`fsharp-build-orchestration` / `fsharp-io-globbing` / `fs-skia-layout-evidence` /
`speckit-evidence-graph` / `speckit-evidence-audit` → `.agents/skills/<id>/SKILL.md`;
`fs-skia-scene` → `src/Scene/skill/SKILL.md`; `fs-skia-skiaviewer` →
`src/SkiaViewer/skill/SKILL.md` (the `.claude/skills/**` peers are the generated mirror).

| Task | Skill id | Resolved path | Load result | loaded_at | work_started_at | Evidence path | Reviewer exception |
|------|----------|---------------|-------------|-----------|-----------------|---------------|--------------------|
| T004 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-02T09:30:00+00:00 | 2026-06-02T09:40:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T006 | fs-skia-scene | `src/Scene/skill/SKILL.md` | loaded | 2026-06-02T10:00:00+00:00 | 2026-06-02T10:12:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T006 | fs-skia-layout-evidence | `.agents/skills/fs-skia-layout-evidence/SKILL.md` | loaded | 2026-06-02T10:00:00+00:00 | 2026-06-02T10:12:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T008 | fs-skia-scene | `src/Scene/skill/SKILL.md` | loaded | 2026-06-02T10:15:00+00:00 | 2026-06-02T10:25:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T008 | fs-skia-layout-evidence | `.agents/skills/fs-skia-layout-evidence/SKILL.md` | loaded | 2026-06-02T10:15:00+00:00 | 2026-06-02T10:25:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T009 | fs-skia-skiaviewer | `src/SkiaViewer/skill/SKILL.md` | loaded | 2026-06-02T11:00:00+00:00 | 2026-06-02T11:20:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T009 | fs-skia-layout-evidence | `.agents/skills/fs-skia-layout-evidence/SKILL.md` | loaded | 2026-06-02T11:00:00+00:00 | 2026-06-02T11:20:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T011 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-02T09:45:00+00:00 | 2026-06-02T09:55:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T012 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-02T09:45:00+00:00 | 2026-06-02T09:58:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T012 | fsharp-io-globbing | `.agents/skills/fsharp-io-globbing/SKILL.md` | loaded | 2026-06-02T09:45:00+00:00 | 2026-06-02T09:58:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T013 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-02T09:45:00+00:00 | 2026-06-02T10:05:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T014 | fsharp-io-globbing | `.agents/skills/fsharp-io-globbing/SKILL.md` | loaded | 2026-06-02T09:45:00+00:00 | 2026-06-02T10:08:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T015 | fsharp-io-globbing | `.agents/skills/fsharp-io-globbing/SKILL.md` | loaded | 2026-06-02T09:45:00+00:00 | 2026-06-02T10:30:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T016 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-02T09:45:00+00:00 | 2026-06-02T10:40:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T017 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-02T09:45:00+00:00 | 2026-06-02T10:50:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T018 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-02T09:45:00+00:00 | 2026-06-02T11:30:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T023 | speckit-evidence-graph | `.agents/skills/speckit-evidence-graph/SKILL.md` | loaded | 2026-06-02T11:40:00+00:00 | 2026-06-02T11:50:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |
| T024 | speckit-evidence-audit | `.agents/skills/speckit-evidence-audit/SKILL.md` | loaded | 2026-06-02T11:55:00+00:00 | 2026-06-02T12:05:00+00:00 | `specs/048-v3-retirement-baseline/readiness/skill-loading-evidence.md` | none |

All skill ids resolved to a single readable `SKILL.md`; each was loaded in declared order
before the task's work began (`loaded_at` < `work_started_at`).

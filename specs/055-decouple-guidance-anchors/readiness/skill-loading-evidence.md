# Skill Loading Evidence — Feature 055

Capability skills declared in `tasks.deps.yml` were resolved to exactly one
readable `.agents/skills/<id>/SKILL.md` and loaded before the implementation
batch began. This feature is build-tooling / governance only, so only the
`fsharp-*` cookbooks and the graph/audit workflow skills apply (no `fs-skia-*`
runtime skill). The red→green evidence log and the graph before/after paths are
recorded in [decoupling-red-green.md](./decoupling-red-green.md) and
`task-graph.md` (refreshed before and after every status change).

| Task | Skill id | Resolved path | Load result | loaded_at | work_started_at | Evidence path | Reviewer exception |
|------|----------|---------------|-------------|-----------|-----------------|---------------|--------------------|
| T005 | fsharp-parsing | `.agents/skills/fsharp-parsing/SKILL.md` | loaded | 2026-06-03T08:50:00+00:00 | 2026-06-03T09:00:00+00:00 | `specs/055-decouple-guidance-anchors/readiness/skill-loading-evidence.md` | none |
| T006 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-03T08:50:00+00:00 | 2026-06-03T09:00:00+00:00 | `specs/055-decouple-guidance-anchors/readiness/skill-loading-evidence.md` | none |
| T007 | fsharp-parsing | `.agents/skills/fsharp-parsing/SKILL.md` | loaded | 2026-06-03T08:50:00+00:00 | 2026-06-03T09:00:00+00:00 | `specs/055-decouple-guidance-anchors/readiness/skill-loading-evidence.md` | none |
| T009 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-03T08:50:00+00:00 | 2026-06-03T09:00:00+00:00 | `specs/055-decouple-guidance-anchors/readiness/skill-loading-evidence.md` | none |
| T010 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-03T08:50:00+00:00 | 2026-06-03T09:00:00+00:00 | `specs/055-decouple-guidance-anchors/readiness/skill-loading-evidence.md` | none |
| T011 | fsharp-parsing | `.agents/skills/fsharp-parsing/SKILL.md` | loaded | 2026-06-03T08:50:00+00:00 | 2026-06-03T09:00:00+00:00 | `specs/055-decouple-guidance-anchors/readiness/skill-loading-evidence.md` | none |
| T012 | fsharp-parsing | `.agents/skills/fsharp-parsing/SKILL.md` | loaded | 2026-06-03T08:50:00+00:00 | 2026-06-03T09:00:00+00:00 | `specs/055-decouple-guidance-anchors/readiness/skill-loading-evidence.md` | none |
| T013 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-03T08:50:00+00:00 | 2026-06-03T09:00:00+00:00 | `specs/055-decouple-guidance-anchors/readiness/skill-loading-evidence.md` | none |
| T014 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-03T08:50:00+00:00 | 2026-06-03T09:00:00+00:00 | `specs/055-decouple-guidance-anchors/readiness/skill-loading-evidence.md` | none |
| T014 | fsharp-io-globbing | `.agents/skills/fsharp-io-globbing/SKILL.md` | loaded | 2026-06-03T08:50:00+00:00 | 2026-06-03T09:00:00+00:00 | `specs/055-decouple-guidance-anchors/readiness/skill-loading-evidence.md` | none |
| T016 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-03T08:50:00+00:00 | 2026-06-03T09:00:00+00:00 | `specs/055-decouple-guidance-anchors/readiness/skill-loading-evidence.md` | none |
| T017 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-03T08:50:00+00:00 | 2026-06-03T09:00:00+00:00 | `specs/055-decouple-guidance-anchors/readiness/skill-loading-evidence.md` | none |
| T018 | speckit-evidence-graph | `.agents/skills/speckit-evidence-graph/SKILL.md` | loaded | 2026-06-03T08:50:00+00:00 | 2026-06-03T09:00:00+00:00 | `specs/055-decouple-guidance-anchors/readiness/skill-loading-evidence.md` | none |
| T019 | speckit-evidence-audit | `.agents/skills/speckit-evidence-audit/SKILL.md` | loaded | 2026-06-03T08:50:00+00:00 | 2026-06-03T09:00:00+00:00 | `specs/055-decouple-guidance-anchors/readiness/skill-loading-evidence.md` | none |

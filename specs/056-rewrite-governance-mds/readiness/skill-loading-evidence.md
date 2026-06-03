# Skill Loading Evidence — Feature 056

Capability skills declared in `tasks.deps.yml` were resolved to exactly one
readable `.agents/skills/<id>/SKILL.md` and loaded before the implementation
batch began, in declared order. This feature is governance-prose / build-tooling
only, so only the `fsharp-*` cookbooks and the graph/audit workflow skills apply
(no `fs-skia-*` runtime skill). The red→green evidence log is recorded in
[rewrite-red-green.md](./rewrite-red-green.md) and the graph before/after paths
are in `task-graph.md` (refreshed before and after every status change).

| Task | Skill id | Resolved path | Load result | loaded_at | work_started_at | Evidence path | Reviewer exception |
|------|----------|---------------|-------------|-----------|-----------------|---------------|--------------------|
| T008 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-03T11:30:00+00:00 | 2026-06-03T11:40:00+00:00 | `specs/056-rewrite-governance-mds/readiness/skill-loading-evidence.md` | none |
| T009 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-03T11:30:00+00:00 | 2026-06-03T11:40:00+00:00 | `specs/056-rewrite-governance-mds/readiness/skill-loading-evidence.md` | none |
| T010 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-03T11:30:00+00:00 | 2026-06-03T11:40:00+00:00 | `specs/056-rewrite-governance-mds/readiness/skill-loading-evidence.md` | none |
| T011 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-03T11:30:00+00:00 | 2026-06-03T11:40:00+00:00 | `specs/056-rewrite-governance-mds/readiness/skill-loading-evidence.md` | none |
| T012 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-03T11:30:00+00:00 | 2026-06-03T11:40:00+00:00 | `specs/056-rewrite-governance-mds/readiness/skill-loading-evidence.md` | none |
| T013 | fsharp-io-globbing | `.agents/skills/fsharp-io-globbing/SKILL.md` | loaded | 2026-06-03T11:30:00+00:00 | 2026-06-03T11:40:00+00:00 | `specs/056-rewrite-governance-mds/readiness/skill-loading-evidence.md` | none |
| T014 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-03T11:30:00+00:00 | 2026-06-03T11:40:00+00:00 | `specs/056-rewrite-governance-mds/readiness/skill-loading-evidence.md` | none |
| T015 | speckit-evidence-graph | `.agents/skills/speckit-evidence-graph/SKILL.md` | loaded | 2026-06-03T11:30:00+00:00 | 2026-06-03T11:40:00+00:00 | `specs/056-rewrite-governance-mds/readiness/skill-loading-evidence.md` | none |
| T016 | speckit-evidence-audit | `.agents/skills/speckit-evidence-audit/SKILL.md` | loaded | 2026-06-03T11:30:00+00:00 | 2026-06-03T11:40:00+00:00 | `specs/056-rewrite-governance-mds/readiness/skill-loading-evidence.md` | none |

Implementation batch records preserve the red-green evidence log, the graph
before/after paths before and after every status change, and the skill-loading
evidence used for the batch.

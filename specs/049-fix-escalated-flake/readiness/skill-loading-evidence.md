# Skill Loading Evidence — Feature 049

Capability skills declared in `tasks.deps.yml` were resolved to exactly one readable
`.agents/skills/<id>/SKILL.md` and loaded **before** the implementation work for each
task began. This feature is build-tooling behavior confined to the compiled build
front-end and its test project, so only two `fsharp-*` cookbooks
(`fsharp-build-orchestration`, `fsharp-shell-process`) and the two graph/audit
workflow skills (`speckit-evidence-graph`, `speckit-evidence-audit`) apply; no
`fs-skia-*` runtime skill applies. Every other task takes a justified `valid-empty`
skillist.

| Task | Skill id | Resolved path | Load result | loaded_at | work_started_at | Evidence path | Reviewer exception |
|------|----------|---------------|-------------|-----------|-----------------|---------------|--------------------|
| T005 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-02T11:03:43+00:00 | 2026-06-02T11:06:00+00:00 | `specs/049-fix-escalated-flake/readiness/skill-loading-evidence.md` | none |
| T008 | fsharp-shell-process | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-shell-process/SKILL.md` | loaded | 2026-06-02T11:03:43+00:00 | 2026-06-02T11:06:00+00:00 | `specs/049-fix-escalated-flake/readiness/skill-loading-evidence.md` | none |
| T009 | fsharp-shell-process | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-shell-process/SKILL.md` | loaded | 2026-06-02T11:03:43+00:00 | 2026-06-02T11:06:00+00:00 | `specs/049-fix-escalated-flake/readiness/skill-loading-evidence.md` | none |
| T010 | fsharp-shell-process | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-shell-process/SKILL.md` | loaded | 2026-06-02T11:03:43+00:00 | 2026-06-02T11:06:00+00:00 | `specs/049-fix-escalated-flake/readiness/skill-loading-evidence.md` | none |
| T011 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-02T11:03:43+00:00 | 2026-06-02T11:06:00+00:00 | `specs/049-fix-escalated-flake/readiness/skill-loading-evidence.md` | none |
| T012 | fsharp-shell-process | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-shell-process/SKILL.md` | loaded | 2026-06-02T11:03:43+00:00 | 2026-06-02T11:06:00+00:00 | `specs/049-fix-escalated-flake/readiness/skill-loading-evidence.md` | none |
| T015 | speckit-evidence-graph | `/home/developer/projects/FS-Skia-UI/.agents/skills/speckit-evidence-graph/SKILL.md` | loaded | 2026-06-02T11:55:00+00:00 | 2026-06-02T11:56:00+00:00 | `specs/049-fix-escalated-flake/readiness/skill-loading-evidence.md` | none |
| T016 | speckit-evidence-audit | `/home/developer/projects/FS-Skia-UI/.agents/skills/speckit-evidence-audit/SKILL.md` | loaded | 2026-06-02T11:55:00+00:00 | 2026-06-02T11:56:00+00:00 | `specs/049-fix-escalated-flake/readiness/skill-loading-evidence.md` | none |

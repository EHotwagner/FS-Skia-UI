# Skill Loading Evidence — Feature 045

Capability skills declared in `tasks.deps.yml` were resolved to exactly one readable
`SKILL.md` and loaded before the implementation work for each task began. This feature is
build-tooling / governance only, so only the `fsharp-*` cookbooks (build-orchestration,
shell-process, io-globbing, code-generation, parsing) and the two graph/audit workflow skills
apply — no `fs-skia-*` runtime skill (no scene/viewer/Elmish-runtime/layout/widgets surface).

| Task | Skill id | Resolved path | Load result | loaded_at | work_started_at | Evidence path | Reviewer exception |
|------|----------|---------------|-------------|-----------|-----------------|---------------|--------------------|
| T004 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T008 | fsharp-shell-process | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-shell-process/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T009 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T010 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T011 | fsharp-io-globbing | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-io-globbing/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T011 | fsharp-code-generation | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-code-generation/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T012 | fsharp-parsing | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-parsing/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T012 | fsharp-io-globbing | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-io-globbing/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T013 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T013 | fsharp-shell-process | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-shell-process/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T014 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T020 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T021 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T022 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T025 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T026 | speckit-evidence-graph | `/home/developer/projects/FS-Skia-UI/.agents/skills/speckit-evidence-graph/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |
| T027 | speckit-evidence-audit | `/home/developer/projects/FS-Skia-UI/.agents/skills/speckit-evidence-audit/SKILL.md` | loaded | 2026-06-01T14:05:00+00:00 | 2026-06-01T14:35:00+00:00 | `specs/045-foundations-build-frontend/readiness/skill-loading-evidence.md` | none |

# Skill Loading Evidence — Feature 046

Capability skills declared in `tasks.deps.yml` were resolved to exactly one readable
`SKILL.md` and loaded **before** the implementation work for each task began. This feature
is build-tooling / governance only, so only the `fsharp-*` cookbooks (parsing,
build-orchestration, code-generation) and the two graph/audit workflow skills apply — no
`fs-skia-*` runtime skill (no scene/viewer/Elmish-runtime/layout/widgets surface).

| Task | Skill id | Resolved path | Load result | loaded_at | work_started_at | Evidence path | Reviewer exception |
|------|----------|---------------|-------------|-----------|-----------------|---------------|--------------------|
| T008 | fsharp-parsing | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-parsing/SKILL.md` | loaded | 2026-06-02T05:30:00+00:00 | 2026-06-02T06:00:00+00:00 | `specs/046-foundations-rule-codification/readiness/skill-loading-evidence.md` | none |
| T008 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-02T05:30:00+00:00 | 2026-06-02T06:00:00+00:00 | `specs/046-foundations-rule-codification/readiness/skill-loading-evidence.md` | none |
| T010 | fsharp-parsing | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-parsing/SKILL.md` | loaded | 2026-06-02T05:30:00+00:00 | 2026-06-02T06:00:00+00:00 | `specs/046-foundations-rule-codification/readiness/skill-loading-evidence.md` | none |
| T011 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-02T05:30:00+00:00 | 2026-06-02T06:00:00+00:00 | `specs/046-foundations-rule-codification/readiness/skill-loading-evidence.md` | none |
| T013 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-02T05:30:00+00:00 | 2026-06-02T06:00:00+00:00 | `specs/046-foundations-rule-codification/readiness/skill-loading-evidence.md` | none |
| T014 | fsharp-code-generation | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-code-generation/SKILL.md` | loaded | 2026-06-02T05:30:00+00:00 | 2026-06-02T06:00:00+00:00 | `specs/046-foundations-rule-codification/readiness/skill-loading-evidence.md` | none |
| T015 | fsharp-code-generation | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-code-generation/SKILL.md` | loaded | 2026-06-02T05:30:00+00:00 | 2026-06-02T06:00:00+00:00 | `specs/046-foundations-rule-codification/readiness/skill-loading-evidence.md` | none |
| T024 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-02T05:30:00+00:00 | 2026-06-02T06:00:00+00:00 | `specs/046-foundations-rule-codification/readiness/skill-loading-evidence.md` | none |
| T025 | speckit-evidence-graph | `/home/developer/projects/FS-Skia-UI/.agents/skills/speckit-evidence-graph/SKILL.md` | loaded | 2026-06-02T05:30:00+00:00 | 2026-06-02T06:00:00+00:00 | `specs/046-foundations-rule-codification/readiness/skill-loading-evidence.md` | none |
| T026 | speckit-evidence-audit | `/home/developer/projects/FS-Skia-UI/.agents/skills/speckit-evidence-audit/SKILL.md` | loaded | 2026-06-02T05:30:00+00:00 | 2026-06-02T06:00:00+00:00 | `specs/046-foundations-rule-codification/readiness/skill-loading-evidence.md` | none |
| T027 | fsharp-build-orchestration | `/home/developer/projects/FS-Skia-UI/.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-02T05:30:00+00:00 | 2026-06-02T06:00:00+00:00 | `specs/046-foundations-rule-codification/readiness/skill-loading-evidence.md` | none |

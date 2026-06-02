# Skill Loading Evidence — Feature 047

The two genuine workflow tasks (T019 / T020) declare capability skills in
`tasks.deps.yml`. Each was resolved to exactly one readable `SKILL.md` and **loaded
before** that task's gate-run work began. Every other task takes a justified
`valid-empty` `skillist` (this is a documentation / measurement / verification-record
feature — no F# source, no scene/viewer/Elmish/layout/widgets surface — so no `fs-skia-*`
runtime skill and no `fsharp-*` cookbook applies; see the Skill-assignment note in
`tasks.md`).

| Task | Skill id | Resolved path | Load result | loaded_at | work_started_at | Evidence path | Reviewer exception |
|------|----------|---------------|-------------|-----------|-----------------|---------------|--------------------|
| T019 | speckit-evidence-graph | `/home/developer/projects/FS-Skia-UI/.agents/skills/speckit-evidence-graph/SKILL.md` | loaded | 2026-06-02T06:42:00+00:00 | 2026-06-02T06:43:00+00:00 | `specs/047-foundations-programme-closeout/readiness/skill-loading-evidence.md` | none |
| T020 | speckit-evidence-audit | `/home/developer/projects/FS-Skia-UI/.agents/skills/speckit-evidence-audit/SKILL.md` | loaded | 2026-06-02T06:42:00+00:00 | 2026-06-02T06:43:00+00:00 | `specs/047-foundations-programme-closeout/readiness/skill-loading-evidence.md` | none |

Both skills resolved to a single readable `SKILL.md` under the canonical
`.agents/skills/**` source (the `.claude/skills/**` peers are the generated mirror). They
were loaded in declared order before the `EvidenceGraph` (T019) and `EvidenceAudit`
(T020) gate runs, which is what those tasks execute.

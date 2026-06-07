# Skill-loading evidence — Typed-Controls Plan Closeout (074)

Each skilled task loads its declared `skillist` (in order) before any work for that task
begins; `LoadedAt` strictly precedes `WorkStartedAt` for every row. Resolved paths are the
canonical registry homes under `.agents/skills/<id>/SKILL.md`.

## Selected skills

- `fsharp-code-generation` — `.agents/skills/fsharp-code-generation/SKILL.md` (T005, US1 C13
  worked example)
- `speckit-evidence-graph` — `.agents/skills/speckit-evidence-graph/SKILL.md` (T019)
- `speckit-evidence-audit` — `.agents/skills/speckit-evidence-audit/SKILL.md` (T020)

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|---|---|---|---|---|---|---|---|
| T005 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T23:30:00Z | 2026-06-06T23:35:00Z | this file + .agents/skills/fsharp-code-generation/SKILL.md (C13 section) + readiness/skill-loading-evidence-workflow.md | none |
| T019 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-06T23:55:00Z | 2026-06-06T23:57:00Z | this file + readiness/evidence-graph.md | none |
| T020 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-07T00:00:00Z | 2026-06-07T00:02:00Z | this file + readiness/evidence-audit.md | none |

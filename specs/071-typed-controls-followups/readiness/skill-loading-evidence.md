# Skill-loading evidence (071)

One row per `(task, declared-skill)` pair. Skills are resolved against the
`FS.Skia.UI.Build.Evidence.SkillRegistry` roots (`.agents/skills/<id>`,
`src/<pkg>/skill`, `template/fragments/<frag>/skill`) — so `fs-skia-ui-widgets`
resolves to `src/Controls/skill/SKILL.md` (its registry home), not the
`template/product-skills` copy. `LoadedAt` is strictly earlier than
`WorkStartedAt` for every row.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|
| T006 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-06T16:57:00+02:00 | 2026-06-06T17:05:00+02:00 | specs/071-typed-controls-followups/readiness/catalog-single-source.md | |
| T007 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T16:57:00+02:00 | 2026-06-06T17:20:00+02:00 | specs/071-typed-controls-followups/readiness/catalog-single-source.md | |
| T008 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T16:57:00+02:00 | 2026-06-06T17:35:00+02:00 | specs/071-typed-controls-followups/readiness/catalog-single-source.md | |
| T009 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T16:57:00+02:00 | 2026-06-06T17:45:00+02:00 | specs/071-typed-controls-followups/readiness/catalog-single-source.md | |
| T010 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T16:57:00+02:00 | 2026-06-06T18:00:00+02:00 | specs/071-typed-controls-followups/readiness/catalog-single-source.md | |
| T013 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T16:57:00+02:00 | 2026-06-06T18:20:00+02:00 | specs/071-typed-controls-followups/readiness/controls-rendering.md | |
| T014 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-06T16:57:00+02:00 | 2026-06-06T18:35:00+02:00 | specs/071-typed-controls-followups/readiness/controls-rendering.md | |
| T014 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T16:57:00+02:00 | 2026-06-06T18:35:00+02:00 | specs/071-typed-controls-followups/readiness/controls-rendering.md | |
| T015 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T16:57:00+02:00 | 2026-06-06T18:50:00+02:00 | specs/071-typed-controls-followups/readiness/controls-rendering.md | |
| T017 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-06T16:57:00+02:00 | 2026-06-06T19:05:00+02:00 | specs/071-typed-controls-followups/readiness/controls-rendering.md | |
| T020 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-06T16:57:00+02:00 | 2026-06-06T19:20:00+02:00 | specs/071-typed-controls-followups/readiness/evidence-graph.md | |
| T021 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-06T16:57:00+02:00 | 2026-06-06T19:35:00+02:00 | specs/071-typed-controls-followups/readiness/evidence-audit.md | |

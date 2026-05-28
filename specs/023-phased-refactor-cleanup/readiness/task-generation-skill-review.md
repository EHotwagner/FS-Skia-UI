# Task Generation Skill Review

Captured at: `2026-05-27T22:33:40+02:00`

Source graph evidence:
`specs/023-phased-refactor-cleanup/readiness/task-graph.md`

## Review Notes

- Task metadata uses structured `deps` and `skillist` entries in
  `tasks.deps.yml`, with matching visible `skillist` mirrors in `tasks.md`.
- The graph validator reports the DAG is acyclic and consistent.
- Empty `skillist` dispositions are valid-empty where no local capability skill
  materially applies.
- Non-packaging generated source tasks intentionally do not declare
  `fs-skia-template-update`; only T022 changes template project compile order
  and profile-conditioned generated file inclusion, so T022 owns that skill.
- Product-area skill declarations resolve to exactly one local capability path:
  `fs-skia-testing` -> `src/Testing/skill/SKILL.md`,
  `fs-skia-skiaviewer` -> `src/SkiaViewer/skill/SKILL.md`,
  `fs-skia-scene` -> `src/Scene/skill/SKILL.md`,
  `fs-skia-layout-evidence` -> `.agents/skills/fs-skia-layout-evidence/SKILL.md`,
  `fs-skia-template-update` -> `.agents/skills/fs-skia-template-update/SKILL.md`.
- Spec Kit skill declarations resolve to `.agents/skills/<skill>/SKILL.md`.

## Validator Summary

Initial graph command:

```text
.specify/extensions/evidence/scripts/python/compute-task-graph.py specs/023-phased-refactor-cleanup
```

Exit code: 0.

Result: 40 tasks parsed, graph files refreshed, 40 pending tasks, no synthetic
propagation.

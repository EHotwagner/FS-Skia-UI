# Contract: `fs-skia-layout-evidence` Task Skill

The repository must include a local capability skill named
`fs-skia-layout-evidence`.

## Scope

Tasks must list `fs-skia-layout-evidence` when they change any of:

- Generated game HUD/status layout.
- Gameplay movement or bounds relative to a reserved HUD region.
- Scene layout evidence or readability evidence claims.
- Generated validation for HUD/readability facts.
- Public scene-returning, generated host, or update naming guidance.
- Benign desktop host warning classification.

## Metadata Requirements

- `tasks.deps.yml` must include `fs-skia-layout-evidence` in the structured
  `skillist` for applicable tasks.
- The matching `tasks.md` line must visibly mirror the same skill metadata.
- Tasks with no applicable skill must still declare `skillist: []`.
- The skill id must resolve to exactly one readable `SKILL.md` before
  implementation starts.

## Validation

Task generation and evidence graph validation must fail when applicable tasks
omit the skill or when the skill cannot be resolved.

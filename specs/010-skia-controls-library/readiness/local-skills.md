# Local Skills

## Verdict

PASS for local and generated skill consolidation evidence.

## Evidence

- `./fake.sh build -t SkillCheck` passed.
- `readiness/selected-skills.md` shows the default generated app receives
  `fs-skia-ui-widgets`.
- Generated-product validation rejects unrelated capability skills.
- `src/Controls/skill/SKILL.md` and `template/fragments/controls/skill/SKILL.md`
  define the Controls/widgets authoring skill.
- Scene, KeyboardInput, and Layout skills route widget/control/chart/graph work
  to `fs-skia-ui-widgets` where applicable.

## Generated App Skill Set

The default generated app selected skills are:

- `fs-skia-project`
- `fs-skia-scene`
- `fs-skia-skiaviewer`
- `fs-skia-elmish`
- `fs-skia-keyboard-input`
- `fs-skia-ui-widgets`

`fs-skia-charts` and generated `fs-skia-layout` widget guidance are intentionally
absent from generated products.

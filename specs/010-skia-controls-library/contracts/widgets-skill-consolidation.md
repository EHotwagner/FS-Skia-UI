# Contract: Widgets Skill Consolidation

## Purpose

Generated product guidance for controls, widgets, layout-oriented controls,
charts, and graphs must be consolidated into one local agent skill.

## Required Skill

Source path:

```text
src/Controls/skill/SKILL.md
```

Generated destination:

```text
.agents/skills/fs-skia-ui-widgets/SKILL.md
```

Required skill name:

```text
fs-skia-ui-widgets
```

## Required Sections

The skill must include the repository-required sections:

- `## Scope`
- `## Public Contract`
- `## Build Commands`
- `## Test Commands`
- `## Evidence`
- `## Package Boundary`
- `## Generated Product`

It must also describe:

- control DSL authoring
- catalog requirements
- widget/layout-control guidance
- chart and graph control ownership
- accessibility and diagnostics expectations
- generated product example ownership
- when to defer to Scene, SkiaViewer, Elmish, KeyboardInput, Layout, or Testing
  skills for non-widget work

## Generated Product Selection

Generated products with Controls must receive:

```text
fs-skia-ui-widgets
```

Generated products must not receive:

```text
fs-skia-charts
fs-skia-layout
```

Layout remains a runtime capability and package. Framework-internal Layout
engine work may still have package-owned guidance, but generated product
layout-control and widget guidance must come from `fs-skia-ui-widgets`.

## Related Skill Updates

Scene, SkiaViewer, Elmish, KeyboardInput, Layout, Testing, and project guidance
must direct control/widget/chart/graph work to `fs-skia-ui-widgets` where
applicable. They must not retain generated-product instructions that tell
agents to use `fs-skia-charts` or `fs-skia-layout` for widget/control authoring.

## Validation Contract

`SkillCheck`, `GeneratedProductCheck`, or equivalent validation must fail when:

- `src/Controls/skill/SKILL.md` is missing
- the skill lacks required sections
- generated products with Controls lack `fs-skia-ui-widgets`
- generated products contain `fs-skia-charts`
- generated products contain generated `fs-skia-layout` widget guidance
- related skills retain stale chart or generated layout-control guidance
- the skill omits build/test/evidence commands

Required readiness path:

```text
specs/010-skia-controls-library/readiness/local-skills.md
```

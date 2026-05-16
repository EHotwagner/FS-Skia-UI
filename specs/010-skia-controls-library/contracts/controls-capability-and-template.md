# Contract: Controls Capability And Template Integration

## Purpose

This contract defines how Controls appears in the V3 capability catalog,
generated products, templates, package references, and compatibility evidence.

## Capability Catalog Entry

`template/capabilities.yml` must include a `controls` runtime capability:

```yaml
- id: controls
  displayName: Controls
  packageId: FS.Skia.UI.Controls
  project: src/Controls/Controls.fsproj
  contracts:
    - src/Controls/Types.fsi
    - src/Controls/Control.fsi
    - src/Controls/Attributes.fsi
    - src/Controls/Theme.fsi
    - src/Controls/Accessibility.fsi
    - src/Controls/Diagnostics.fsi
    - src/Controls/Catalog.fsi
    - src/Controls/TextInput.fsi
    - src/Controls/Collections.fsi
    - src/Controls/Charts.fsi
    - src/Controls/CustomControl.fsi
  tests:
    - tests/Controls.Tests/Controls.Tests.fsproj
  skill: src/Controls/skill/SKILL.md
  templateFragment: template/fragments/controls
  dependencies: [scene, layout, keyboard-input]
  profiles: [app, governed, sample-pack]
  defaultApp: true
  evidence:
    - control-catalog
    - public-surface
    - semantic-tests
    - interaction-tests
    - layout-rendering
    - generated-product
  surfaceBaseline: readiness/surface-baselines/FS.Skia.UI.Controls.txt
  docs: docs/controls.md
  ownerNotes: Declarative controls, widgets, charts, graphs, and custom wrappers.
```

Dependency details may be narrowed only if implementation proves a dependency
is unnecessary and generated product validation still passes.

## Default App Contract

The default app profile must resolve to this capability set:

```text
Scene
SkiaViewer
Elmish
KeyboardInput
Layout
Controls
```

It must not resolve to the separate Charts capability.

## Charts Removal Contract

Generated capability validation must reject:

- `charts` as a selectable generated capability
- `FS.Skia.UI.Charts` as a default generated product package reference
- `template/fragments/charts` as an active default fragment
- `fs-skia-charts` as a generated local skill

The repository may keep compatibility notes or migration documentation, but
active generated product selection must point chart and graph work to Controls.

## Template Fragment Contract

Required fragment path:

```text
template/fragments/controls/
```

The fragment must add only product-owned generated assets, such as:

- controls package reference
- concise controls guidance
- representative product-owned example view
- product test coverage for the example view
- selected `fs-skia-ui-widgets` skill

The fragment must not copy:

- framework reference gallery source
- framework samples
- framework readiness evidence
- framework historical specs
- framework implementation projects

## Generated Product Contract

Default generated products must contain:

- exactly one product app project unless a profile explicitly allows more
- exactly one product test project unless a profile explicitly allows more
- package references for Scene, SkiaViewer, Elmish, KeyboardInput, Layout, and
  Controls
- a product-owned controls example view
- `fs-skia-ui-widgets`
- no `fs-skia-charts`
- no generated `fs-skia-layout` widget guidance skill
- full product governance assets

Generated product validation must fail when stale Charts package references,
stale chart fragments, stale generated chart skills, or copied framework
gallery/source paths appear in the default app profile.

## Compatibility Evidence

Required path:

```text
specs/010-skia-controls-library/readiness/compatibility-impact.md
```

The record must describe:

- removed active Charts capability/package/template/skill ownership
- replacement Controls package/module paths
- how lower-level Scene, SkiaViewer, Layout, and KeyboardInput APIs compose
  with Controls
- which compatibility work is in scope
- which migration or release automation work is out of scope

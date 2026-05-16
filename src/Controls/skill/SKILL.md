---
name: fs-skia-ui-widgets
description: Build declarative FS.Skia.UI controls, widgets, chart controls, graph controls, and generated product widget examples.
---

# Controls And Widgets

## Scope

Use this skill for user-facing widgets and controls built with
`FS.Skia.UI.Controls`: forms, buttons, text input, lists, tables, layout
containers, chart controls, graph controls, custom control wrappers, catalog
examples, and generated product widget guidance.

## Public Contract

The supported API lives in `src/Controls/*.fsi`. View functions should build
`Control<'msg>` values with module-per-control `create` functions and
declarative attributes such as `TextBox.value`, `Button.onClick`, and
`Stack.children`. Persistent values stay in the Elmish model; controls may keep
only keyed transient interaction state.

## Build Commands

Run `./fake.sh build -t Dev` for normal development and
`./fake.sh build -t Verify` before readiness sign-off. Use
`./fake.sh build -t PackLocal` and `./fake.sh build -t PackageSurfaceCheck`
when changing `.fsi` files.

## Test Commands

Run `dotnet test tests/Controls.Tests/Controls.Tests.fsproj` for focused
coverage. The governed targets are `./fake.sh build -t ControlsCatalogCheck`,
`./fake.sh build -t ControlsInteractionCheck`, and
`./fake.sh build -t ControlsRenderingCheck`.

## Evidence

Update `specs/010-skia-controls-library/readiness/control-catalog.md`,
`semantic-tests.md`, `interaction-tests.md`, `layout-rendering.md`,
`public-surface.md`, and generated-product evidence when behavior or public
surface changes. Supported catalog rows need purpose, attributes, events,
visual states, accessibility metadata, examples, tests, and evidence.

## Package Boundary

Controls owns widgets, chart controls, graph controls, custom wrappers, the
catalog, and generated widget guidance. Scene, SkiaViewer, Elmish,
KeyboardInput, Layout, and Testing remain separate capabilities for non-widget
work. Layout remains a runtime package; generated layout-control guidance comes
from this skill.

## Generated Product

Generated products with Controls receive `fs-skia-ui-widgets` and must not
receive `fs-skia-charts` or generated `fs-skia-layout` widget guidance. Product
examples must be product-owned and must not copy framework galleries, samples,
historical specs, readiness evidence, or implementation projects.

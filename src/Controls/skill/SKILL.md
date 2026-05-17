---
name: fs-skia-ui-widgets
description: Build Skia-rendered FS.Skia.UI Controls, rich text, chart controls, graph controls, DataGrid, custom wrappers, and generated product examples.
---

# Controls

## Scope

Use this skill for user-facing controls built with `FS.Skia.UI.Controls`:
forms, buttons, text input, lists, tables, rich text, layout containers, chart
controls, graph controls, DataGrid, custom control wrappers, catalog examples,
and generated product guidance.

## Public Contract

The supported API lives in `src/Controls/*.fsi`. View functions should build
`Control<'msg>` values with module-per-control `create` functions and
declarative attributes such as `TextBox.value`, `Button.onClick`,
`LineChart.series`, `DataGrid.columns`, `DataGrid.rows`, and `Stack.children`.
Persistent values stay in the product model; controls may keep only keyed
transient interaction state through product-owned `ControlRuntime`.

## Generated Product Pattern

Generated examples should keep product state and messages local:

```fsharp
type Msg =
    | NameChanged of string
    | SaveRequested
    | GridSelectionChanged of string

let view model : Control<Msg> =
    Stack.create [
        Stack.children [
            TextBox.create [
                TextBox.value model.Name
                TextBox.onChanged NameChanged
            ]
            Button.create [
                Button.text "Save"
                Button.onClick SaveRequested
            ]
            LineChart.create [
                LineChart.series [ "Revenue", model.Revenue ]
            ]
            DataGrid.create [
                DataGrid.columns model.Columns
                DataGrid.rows model.Rows
            ]
        ]
    ]
```

When Elmish program integration is selected, use the
`FS.Skia.UI.Controls.Elmish` adapter for commands, subscriptions, and program
wiring at the product edge.

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

Controls owns ordinary controls, rich text, chart controls, graph controls,
DataGrid, custom wrappers, the catalog, and generated controls guidance. Scene,
SkiaViewer, Elmish, KeyboardInput, Layout, and Testing remain separate
capabilities for lower-level or host-specific work. Layout remains a runtime
package dependency; generated control authoring stays in Controls.

## Generated Product

Generated products with Controls receive this skill. Product examples must be
product-owned and must not copy framework galleries, samples, historical specs,
readiness evidence, docs, or implementation projects.

## Charts migration

Users moving from the legacy Charts package should replace chart declarations
with Controls `LineChart`, `BarChart`, `PieChart`, `ScatterPlot`, `GraphView`,
and `DataGrid` declarations. There is no compatibility shim; generated
products should use `FS.Skia.UI.Controls` directly.

---
name: fs-skia-ui-widgets
description: Generated product guidance for Skia-rendered FS.Skia.UI Controls, rich text, chart controls, graph controls, DataGrid, and custom wrappers.
---

# Generated Controls

## Scope

Use this skill for generated product screens that compose controls in an
Elmish-style view function. Controls is the generated authoring path for
ordinary controls, rich text, chart controls, graph controls, DataGrid, and
custom wrappers.

## Public Contract

Reference `FS.Skia.UI.Controls` and build `Control<'msg>` values with
module-per-control `create` functions and declarative attributes.
DataGrid is a data control with product-owned rows, columns, selection, focus,
and viewport state.

## Generic Message Flow

Keep product state and messages in the generated product:

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

Use `GraphView.create`, `BarChart.create`, `PieChart.create`, and
`ScatterPlot.create` from the same Controls package when the product needs
graph or chart variants.

When the generated product also selects Elmish program integration, use the
`FS.Skia.UI.Controls.Elmish` adapter at the product edge for commands and
subscriptions.

## Build Commands

Run `./fake.sh build -t Dev` and `./fake.sh build -t Verify` in the generated
product.

## Test Commands

Run `./fake.sh build -t Test` for product-owned control examples.

## Evidence

Product evidence belongs in the generated product readiness folder. Do not copy
framework readiness reports.

## Package Boundary

Controls owns ordinary controls, rich text, chart controls, graph controls,
DataGrid, and custom wrappers. Layout remains a runtime package dependency;
generated control authoring stays in Controls.

## Generated Product

Keep examples small and product-owned. Do not copy framework galleries,
framework samples, framework readiness evidence, historical specs, framework
docs, or framework implementation projects.

## Charts migration

Users moving from the legacy Charts package should replace chart declarations
with Controls `LineChart`, `BarChart`, `PieChart`, `ScatterPlot`, `GraphView`,
and `DataGrid` declarations. There is no compatibility shim; generated
products should use `FS.Skia.UI.Controls` directly.

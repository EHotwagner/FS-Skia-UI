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

type Model =
    { Name: string
      Revenue: ChartSeries list
      Columns: DataGridColumn list
      Rows: DataGridRow list }

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
            LineChart.create [ LineChart.series model.Revenue ]
            GraphView.create [ GraphView.nodes [ "form"; "chart"; "grid" ] ]
            DataGrid.create model.Columns [
                DataGrid.rows model.Rows
                DataGrid.visibleRange {
                    FirstIndex = 0
                    Count = model.Rows.Length
                    Total = model.Rows.Length
                }
            ]
        ]
    ]
```

When Elmish program integration is selected, use the
`FS.Skia.UI.Controls.Elmish` adapter for commands, subscriptions, and program
wiring at the product edge.

## Build Commands

Run `./fake.sh build -t Dev` for normal development and
`./fake.sh build -t VerifyPreflight` before broad verification. Run
`./fake.sh build -t Verify` before readiness sign-off. Use `./fake.sh build -t
PackLocal` and `./fake.sh build -t PackageSurfaceCheck` when changing `.fsi`
files. If `Verify` or `Ci` reports `environment-failure`, focused gates are
diagnostic only; final readiness needs a later healthy broad pass in a fresh
shell, fresh container, or CI runner.

## Test Commands

Run `dotnet test tests/Controls.Tests/Controls.Tests.fsproj` for focused
coverage. The governed targets are `./fake.sh build -t ControlsCatalogCheck`,
`./fake.sh build -t ControlsInteractionCheck`, and
`./fake.sh build -t ControlsRenderingCheck`.

## Evidence

Update the active feature readiness reports for control catalog, semantic
tests, interaction tests, layout/rendering, public surface, and generated
product evidence when behavior or public surface changes. Stable public
surface baselines live under `readiness/surface-baselines/`. Supported catalog
rows need purpose, attributes, events, visual states, accessibility metadata,
examples, tests, and evidence.

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

## Persistent problems

When a problem outlasts reasonable in-repo attempts, extensive external research is
**mandatory** — consult **official online docs first** (the F#/.NET docs and the driven
library's own documentation/API reference), then community sources (forums, Reddit, Q&A
sites, issue trackers and changelogs). Record the findings and resolving links in the
feature's `specs/<feature>/feedback/` folder and, for durable lessons, in this skill's
**Sources** line. Offline, the mandate degrades to recording "research blocked — <why>"
rather than hard-failing the phase.

## Related

- [[fs-skia-layout]] is the runtime layout engine these controls compose over.
- [[fs-skia-scene]] is the primitive surface controls ultimately render into.

## Sources / links

- F#/.NET docs: https://learn.microsoft.com/en-us/dotnet/fsharp/
- SkiaSharp (the driven Skia rendering library): https://github.com/mono/SkiaSharp

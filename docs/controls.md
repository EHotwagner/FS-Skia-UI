# Controls

`FS.Skia.UI.Controls` provides Skia-rendered declarative controls for
Elmish-shaped view functions. Persistent values such as text, selected items,
validation state, and committed values stay in the application model. Transient
interaction state is held by a product-owned `ControlRuntimeModel`, keyboard
state is held by `FS.Skia.UI.KeyboardInput.KeyboardModel`, and direct command or
subscription wiring lives in `FS.Skia.UI.Controls.Elmish`.

The public surface is owned by `src/Controls/*.fsi` and the supported catalog is
declared by `src/Controls/catalog.yml` plus `Catalog.supportedControls`. Charts
and graphs are Controls-owned for generated products; the previous Charts
capability remains compatibility source only and is not selected by the default
app profile.

Validate changes with:

```bash
./fake.sh build -t ControlsCatalogCheck
./fake.sh build -t ControlsInteractionCheck
./fake.sh build -t ControlsRenderingCheck
./fake.sh build -t PackageSurfaceCheck
```

## Supported Catalog

The catalog has 47 supported rows across display, input, selection,
navigation, layout, feedback, data, chart, graph, and custom categories. Every
row in `src/Controls/catalog.yml` records purpose, required attributes, common
attributes, events where applicable, visual states, accessibility metadata,
examples, tests, evidence paths, support status, and Controls ownership.

## Authoring Pattern

Controls follow the same `create : Attr<'msg> list -> Control<'msg>` shape.
Persistent state is read from the model and events emit messages:

```fsharp
let view model =
    Stack.create [
        Stack.children [
            TextBlock.create [ TextBlock.text model.Title ]
            TextBox.create [
                TextBox.value model.Name
                TextBox.validation model.NameValidation
                TextBox.onChanged NameChanged
            ]
            Button.create [
                Button.text "Save"
                Button.enabled model.CanSave
                Button.onClick SaveRequested
            ]
            RichText.create model.RichIntro []
        ]
    ]
```

Stateful interaction stays explicit:

```fsharp
let keyboard, keyboardEffects =
    Keyboard.init [ { Key = "S"; Command = "save" } ]

let runtime, _ = ControlRuntime.init ()

let commands =
    keyboardEffects
    |> List.collect (ControlsElmish.interpretKeyboardEffect (fun _ -> SaveRequested))
```

The maintained sample in `samples/ControlsGallery` combines stable records,
rich text, chart and graph controls, a custom Skia escape hatch,
`ControlRuntime`, `KeyboardInput`, and `Controls.Elmish` adapter wiring through
the public package surfaces.

## Controls And Lower-Level Paths

Controls is the supported high-level authoring path for ordinary controls, rich
Skia rendering, chart controls, graph views, and DataGrid in generated product
profiles. It is appropriate when product code wants declarative control records,
generic product messages, catalog validation, and optional adapter wiring.

Lower-level packages remain supported for products that do not select Controls:

| Path | Use when |
|------|----------|
| `FS.Skia.UI.Scene` | Product code needs immutable scene primitives, paint data, diagnostics, or render-readback evidence without controls. |
| `FS.Skia.UI.Layout` | Product code needs layout evaluation or graph layout helpers while owning its own view/control layer. |
| `FS.Skia.UI.KeyboardInput` | Product code needs keyboard runtime state, YAML command configuration, diagnostics, or state display without Controls. |
| `FS.Skia.UI.SkiaViewer` | Product code needs the desktop Skia/Vulkan host boundary directly. |
| `FS.Skia.UI.Elmish` | Product code needs the general viewer Elmish integration without Controls-specific command interpretation. |
| `FS.Skia.UI.Controls.Elmish` | Product code uses Controls and wants command, subscription, or program helpers for Controls and KeyboardInput effects. |

## Charts, Graphs, And DataGrid

Charts and graph views are authored with `LineChart`, `BarChart`, `PieChart`,
`ScatterPlot`, and `GraphView` from `FS.Skia.UI.Controls`. DataGrid is a data
control, not a chart category; products provide columns, rows, selection/focus
state, sort/filter metadata, and visible-range state through the Controls
DataGrid APIs.

Catalog rows for chart, graph, and DataGrid controls link to
`specs/011-controls-boundary-refactor/readiness/chart-datagrid-controls.md` for
implementation and validation evidence. New applications should not select the
legacy Charts package or a chart-specific generated skill.

Existing chart users should migrate new authoring to `FS.Skia.UI.Controls`:
`LineChart`, `BarChart`, `PieChart`, and `ScatterPlot` cover chart controls,
`GraphView` covers graph-view authoring, and `DataGrid` covers table-like data
controls. This is a replacement path, not a compatibility shim or automated
external-app migration promise.

## Validation Path

Use `./fake.sh build -t Dev` for the default framework build and tests. Use
`ControlsCatalogCheck`, `ControlsInteractionCheck`, and
`ControlsRenderingCheck` before adding or changing catalog rows. Use
`FsiTranscripts`, `SampleContractSmoke`, `GeneratedProductCheck`, and
`TemplateCheck` before changing public surface, samples, or generated product
support.

For the form-and-dashboard walkthrough, build from the catalog docs with at
least 10 controls, 3 nested layout regions, and 5 interactions. The maintained
reference path is `samples/ControlsGallery --contract-smoke`; first-time human
evaluator evidence is release-readiness work and is tracked separately from the
in-repo command evidence.

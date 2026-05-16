# Controls

`FS.Skia.UI.Controls` provides declarative controls for Elmish-style view
functions. Persistent values such as text, selected items, validation state,
and committed values stay in the application model. Controls dispatch messages
and retain only keyed transient interaction state.

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

The initial catalog has 46 supported rows across display, input, selection,
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
        ]
    ]
```

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

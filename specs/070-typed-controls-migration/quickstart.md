# Quickstart: Authoring the Migrated Controls Through the Typed Front Door

After `070`, **every** catalog control is authorable through
`FS.Skia.UI.Controls.Typed` with a compiler-checked `Props` record — not just the
`065` six. Author with `{ Module.defaults with … }`, compose containers over
`Widget<'msg>` children, and finish at the Elmish adapter with `Widget.toControl`.

## 1. Pure controls — pick fields off `defaults`

```fsharp
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Typed

let nameLabel  = Label.view  { Label.defaults  with Text = "Name" }
let saveButton = IconButton.view { IconButton.defaults with Text = "Save"; Intent = Primary; OnClick = Some Save }
let progress   = ProgressBar.view { ProgressBar.defaults with Value = 0.6 }
```

`OnClick = None` (the default) lowers to **no** event binding — never a default
message (FR-005).

## 2. Containers compose over typed children (no `toControl` in author code)

```fsharp
let form =
    Panel.view
        { Panel.defaults with
            Children =
                [ nameLabel
                  TextBox.view { (TextBox.defaults "name") with Value = model.Name } model.NameInput
                  Wrap.view { Wrap.defaults with Spacing = 8.0; Children = [ saveButton; progress ] } ] }
```

Children are `Widget<'msg>`; the container lowers them with `Widget.toControl`
internally, order preserved. To drop a legacy `Control<'msg>` into a typed child
list, lift it with `Widget.ofControl`.

## 3. Stateful controls reuse the existing model — never a fork

```fsharp
// list-view delegates to the existing Collections model
let model0, _eff = ListView.init { (ListView.defaults "orders") with Items = orders }

// in your update, the typed update IS the Collections update
let model1, _eff = ListView.update collMsg model0

let ordersView = ListView.view { (ListView.defaults "orders") with Items = orders; OnSelected = Some Select } model1
```

`text-area` reuses `TextInput`; the selection collections reuse `Collections`;
charts/graph reuse the chart/graph models; `data-grid` reuses `DataGrid`. The
typed `update` returns the existing model/effect types and equals the reused
model's `update` (SC-003).

## 4. The escape hatch — `custom-control`

`custom-control` has no `Props` schema. Build it the legacy way and lift it:

```fsharp
let custom = CustomControl.create myDefinition myAttrs |> Widget.ofControl
```

## 5. Finish at the adapter

The Elmish adapter consumes a lowered `Control<'msg>` unchanged (no adapter edit —
`068` already added `programOfWidget`/`widgetView`):

```fsharp
let view model = form |> Widget.toControl     // or ControlsElmish.widgetView (fun m -> form)
```

## 6. Verify

```bash
./fake.sh build -t Route          # prints the controls-public-surface + skill gates; run ONLY those
# then, sequentially (shared .fake state):
./fake.sh build -t Dev
# ... the escalated six-target order through EvidenceAudit (verdict PASS, zero [S])
```

Every typed control above lowers to the same `Control<'msg>` its legacy builder
emits — proven by the per-control parity test (`tests/Controls.Tests/
TypedLoweringTests.fs`). That is what makes the typed surface a transparent façade
over the unchanged render/layout/a11y/evidence pipeline.

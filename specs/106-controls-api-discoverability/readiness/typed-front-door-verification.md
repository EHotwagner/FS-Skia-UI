# US1 verification — typed front door is the demonstrated default (T009)

## SC-003 — zero legacy attr-list constructions in the starter view

```
$ grep -nE "\.create\s*\[" template/base/src/Product/View.fs
(no matches — zero legacy "Module.create [ ... ]" constructions)
```

The rewritten `controlsWidgetView` authors every control through the typed Props front door
(`FS.Skia.UI.Controls.Typed`):

```
46  TextBlock.view { TextBlock.defaults with Text = "Product controls" }
47  RichText.view { RichText.defaults with Runs = model.RichIntro.Runs }
58  TextBox.view nameProps (fst (TextBox.init nameProps))
…   Button.view { Button.defaults with Id = Some "save"; Text = "Save"; … OnClick = Some SaveRequested }
70  LineChart.view { LineChart.defaults with Series = model.Revenue }
71  GraphView.view { GraphView.defaults with Nodes = [ "form"; "chart"; "grid" ] }
82  DataGrid.view gridProps (fst (DataGrid.init gridProps))
```

`controlsExampleView = controlsWidgetView >> Widget.toControl` lowers the typed `Widget<'msg>`
tree to the `Control<'msg>` IR the render path + `ControlsElmish.program` consume — the single
documented lowering seam. The demonstrated variety satisfies FR-002: a display control
(`TextBlock`/`RichText`), an interactive input (`TextBox` with `OnChanged`), and a button with
an event handler (`Button` with `OnClick = Some SaveRequested`); the `OnClick = None` →
"binds nothing" idiom is documented in the view comments.

## FR-002 stateful-control idiom (no invented literal)

The stateful `TextBox`/`DataGrid` controls source their per-identity model from the props via
`TextBox.init` / `DataGrid.init` (not a hand-written literal); the live host then retains edits
across frames keyed by control identity, exactly as the legacy starter relied on. The starter
comment states where the model comes from.

## SC-001 — add an unshown control kind with only `defaults` + IntelliSense

Walkthrough (no reflection, no framework-source read): to add, e.g., a check box, an author
types `CheckBox.defaults` and IntelliSense enumerates the `CheckBoxProps` fields
(`Id`, `Text`, `Checked`, `Classes`, `OnChanged`); the author writes
`CheckBox.view { CheckBox.defaults with Text = "Agree"; Checked = model.Agreed; OnChanged = Some Toggled }`
and drops it into `Stack.defaults.Children`. The compiler enumerates and checks every field.
This compiles and renders through the same path as the demonstrated controls — proven by
`GeneratedProductCheck` (`generated-product.md`) and the `TypedLoweringTests` parity suite,
which now also covers RichText/LineChart/GraphView (T008). The per-control attribute contract
is discoverable from `Catalog.*` and `docs/controls-catalog.md` without reflection (US3).

## FR-003 — behaviour preserved

The typed front door lowers structurally equal to the legacy builders (the framework's
`tests/Controls.Tests/TypedLoweringTests.fs` parity suite, 13 cases incl. the three added in
T008). The starter therefore renders the same controls it did before; `GeneratedProductCheck`
confirms it compiles and renders.

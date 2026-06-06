# Phase 1 Data Model: Migrate Remaining 41 Controls to the Typed Front Door

Entities are the public typed authoring surface for the 41 remaining catalog
controls. Every field is strongly typed — **no `obj`, no string-named event**
(FR-003/SC-005). Rule (uniform with `065`): each catalog **required attribute**
(PascalCased) is a **non-optional** `Props` field; every other value is optional
and resolves through `defaults`; every catalog **event** is an optional callback
field that lowers to **no binding** when `None` (FR-005). Field names that match a
catalog `requiredAttribute` are constrained by the `066` cross-check
(`CatalogTests.typedPropsById`: `requiredAttribute` PascalCased ∈ `Props` fields).

All `view` functions return `Widget<'msg>` and lower (per research R3) to the
existing legacy builder for that id — a dedicated `*.create` where one exists,
else `Control.standard kind` — proven by a per-control structural-parity test.
Reused entities (`Widget`, `Control`, `Attr`, `Theme`, the existing models) are
**unchanged**.

The variable taxonomy (Identity, Content, Data, Behavior, Variant, Layout,
Theme/style, Accessibility, Events) is applied per record exactly as in `065`
(`data-model.md` §"Variable taxonomy"). Sketches below show the load-bearing
fields per control; `Id: ControlId option` (Identity) and standard
Layout/Accessibility optionals (e.g. `Width`/`Height`/`AccessibleName`) are
implied per taxonomy and omitted for brevity unless required.

## Group 1 — Pure display (`Widgets/Display.fs`)

Pure `Props -> Widget`; no model; no events (except none). Lower to the dedicated
legacy `*.create` (`RichText`, `Label`, `Image`, `Icon`, `Separator`, `Badge`,
`ProgressBar`, `Spinner`, `ValidationMessage`).

| id | Module | Props (load-bearing fields) |
| --- | --- | --- |
| `rich-text` | `RichText` | `{ Runs: RichTextRun list }` (reuse the existing `RichText` run type; `runs` required) |
| `label` | `Label` | `{ Text: string }` |
| `image` | `Image` | `{ Value: ImageSource }` (`value` required; reuse the existing image source type, never `obj`) |
| `icon` | `Icon` | `{ Text: string }` (glyph/name as `Text` per catalog `text` required) |
| `separator` | `Separator` | `{ Orientation: Orientation }` (no required attr) |
| `badge` | `Badge` | `{ Text: string }` |
| `progress-bar` | `ProgressBar` | `{ Value: float }` (`value` required) |
| `spinner` | `Spinner` | `{ }` (no required attr; optional size/intent via defaults) |
| `validation-message` | `ValidationMessage` | `{ Text: string; Severity: ValidationState }` |

## Group 2 — Pure input / command (`Widgets/Input.fs`)

Pure `Props -> Widget` carrying one optional event; `event = None` → no binding.

| id | Module | Props |
| --- | --- | --- |
| `icon-button` | `IconButton` | `{ Text: string; Enabled: bool; Intent: ButtonIntent; OnClick: 'msg option }` (reuse `ButtonIntent` from `065`) |
| `numeric-input` | `NumericInput` | `{ Value: float; Min: float option; Max: float option; Step: float; ReadOnly: bool; OnChanged: (float -> 'msg) option }` |
| `radio-group` | `RadioGroup` | `{ Items: RadioItem list; SelectedKey: string option; OnChanged: (string -> 'msg) option }` (`items` required) |
| `switch` | `Switch` | `{ Checked: bool; OnChanged: (bool -> 'msg) option }` |
| `slider` | `Slider` | `{ Value: float; Min: float; Max: float; Step: float; OnChanged: (float -> 'msg) option }` (`value` required) |

## Group 3 — Stateful input reusing existing MVU (`Widgets/TextAreaWidget.fs`)

`text-area` reuses the `TextInput` model exactly as the `065` typed `TextBox`
does — no new model type (FR-004/SC-003). Shape mirrors `TextBoxWidget`:

```fsharp
type TextAreaProps<'msg> =
    { Id: ControlId
      Value: string
      ReadOnly: bool
      Validation: ValidationState
      OnChanged: (string -> 'msg) option }

module TextArea =
    val defaults : ControlId -> TextAreaProps<'msg>
    val init     : TextAreaProps<'msg> -> TextInputModel * TextInputEffect list   // delegates to TextInput.init (multiline mode)
    val update   : TextInputMsg -> TextInputModel -> TextInputModel * TextInputEffect list  // delegates to TextInput.update
    val view     : TextAreaProps<'msg> -> TextInputModel -> Widget<'msg>          // lowers ≡ legacy TextArea.create for the model state
```

## Group 4 — Selection collections reusing the `Collections` model (`Widgets/CollectionsWidgets.fs`)

Five per-id typed modules, each **delegating to the same existing `Collections`
model** (`CollectionModel`/`CollectionMsg`/`CollectionEffect`,
`Collections.init`/`update`) — never forked (FR-004/SC-003, research R2). Each has
a per-id `Props` with `Items` (required) and its catalog event. They lower (no
dedicated `*.create`) to `Control.standard <kind>` for that id (research R3).

| id | Module | Props (load-bearing) | Event |
| --- | --- | --- | --- |
| `list-view` | `ListView` | `{ Id: ControlId; Items: 'item list; SelectedKey: string option }` | `OnSelected: (string -> 'msg) option` |
| `list-box` | `ListBox` | as `ListView` | `OnSelected` |
| `multi-select-list` | `MultiSelectList` | `{ Id; Items; SelectedKeys: Set<string> }` | `OnChanged: (string list -> 'msg) option` |
| `combo-box` | `ComboBox` | `{ Id; Items; SelectedKey: string option; IsOpen: bool }` | `OnChanged: (string -> 'msg) option` |
| `tree-view` | `TreeView` | `{ Id; Items: TreeNode list; Expanded: Set<string> }` | `OnSelected: (string -> 'msg) option` |

Each `module` exposes `defaults`/`init`/`update`/`view`; `init`/`update` delegate
to `Collections.*` and the interaction test asserts result equality vs.
`Collections.update` directly (SC-003).

## Group 5 — Layout containers over `Widget` children (`Widgets/Containers.fs`)

Pure `Props -> Widget`. Children/content are `Widget<'msg>` (`Children` for the
many-child kinds, `Child` for `border`/`overlay`/`scroll-viewer`), lowered via
`Widget.toControl` with order preserved (the `065` `Stack` pattern, spec edge
case). Legacy controls bridge in via `Widget.ofControl`.

| id | Module | Props (load-bearing) |
| --- | --- | --- |
| `grid` | `Grid` | `{ Children: Widget<'msg> list; Rows: GridLength list; Columns: GridLength list }` |
| `dock` | `Dock` | `{ Children: (DockSide * Widget<'msg>) list }` |
| `wrap` | `Wrap` | `{ Children: Widget<'msg> list; Orientation: Orientation; Spacing: float }` |
| `border` | `Border` | `{ Child: Widget<'msg>; Thickness: float; Padding: float }` |
| `panel` | `Panel` | `{ Children: Widget<'msg> list }` |
| `scroll-viewer` | `ScrollViewer` | `{ Id: ControlId; Child: Widget<'msg>; OnChanged: (ScrollState -> 'msg) option }` (scroll is stateful — see note) |
| `split-view` | `SplitView` | `{ Children: Widget<'msg> list; Orientation: Orientation; OnChanged: (float -> 'msg) option }` |

> `scroll-viewer`/`split-view` carry an `onChanged` event in the catalog. Per
> research R2, if their scroll/split runtime is owned by the `Collections` model
> (the catalog lists them under `Collections`), the typed module delegates to it;
> if their legacy path is purely an attribute on a container, they stay pure
> `Props -> Widget` with an optional event that lowers to a binding. The
> per-control parity + interaction test pins which, against the legacy builder.

## Group 6 — Navigation / composite (`Widgets/Navigation.fs`)

| id | Module | Props (load-bearing) | Event |
| --- | --- | --- | --- |
| `tabs` | `Tabs` | `{ Items: TabItem<'msg> list; SelectedKey: string option }` (each `TabItem` content is `Widget<'msg>`) | `OnChanged: (string -> 'msg) option` |
| `menu` | `Menu` | `{ Items: MenuItem<'msg> list }` | `OnSelected: (string -> 'msg) option` |
| `context-menu` | `ContextMenu` | `{ Items: MenuItem<'msg> list }` (reuse the `MenuItem` type) | `OnSelected: (string -> 'msg) option` |
| `toolbar` | `Toolbar` | `{ Children: Widget<'msg> list }` | `OnClick: 'msg option` |

`menu`/`context-menu` are distinct per-id modules over the same legacy `Menu`
builder (research R1/R2).

## Group 7 — Overlay / transient (`Widgets/Overlay.fs`)

| id | Module | Props (load-bearing) | Event |
| --- | --- | --- | --- |
| `tooltip` | `Tooltip` | `{ Text: string; Target: Widget<'msg> option }` | — |
| `dialog` | `Dialog` | `{ Children: Widget<'msg> list; IsOpen: bool; Title: string option }` | `OnSelected: (string -> 'msg) option` |
| `toast` | `Toast` | `{ Text: string; Severity: ValidationState }` | — |
| `overlay` | `Overlay` | `{ Child: Widget<'msg>; IsOpen: bool }` | — |

## Group 8 — Charts / graph reusing existing models (`Widgets/ChartsWidgets.fs`)

Per-id typed modules carrying product-owned `Data`-class fields that **reuse the
existing chart/graph data types** (`ChartSeries`/`ChartPoint` from `Charts.fsi`,
the graph node/edge types) — the façade does not redefine the data model (spec
edge case). They lower to the dedicated legacy `*.create` in `Charts.fsi`.

| id | Module | Props (load-bearing) | Event |
| --- | --- | --- | --- |
| `line-chart` | `LineChart` | `{ Series: ChartSeries list }` | `OnSelected: (string -> 'msg) option` |
| `bar-chart` | `BarChart` | `{ Series: ChartSeries list }` | `OnSelected` |
| `pie-chart` | `PieChart` | `{ Values: ChartPoint list }` | `OnSelected` |
| `scatter-plot` | `ScatterPlot` | `{ Series: ChartSeries list }` | `OnSelected` |
| `graph-view` | `GraphView` | `{ Nodes: GraphNode list; Edges: GraphEdge list }` | `OnSelected: (string -> 'msg) option` |

Where a chart owns runtime selection/zoom state in an existing model, the module
exposes `init`/`update` delegating to it (FR-004); otherwise it is pure
`Props -> Widget` with the optional event.

## Group 9 — Escape hatch (`Widgets/CustomControlWidget.fs`)

`custom-control` gets **no `Props` schema** (FR-006, research R4). Its typed
affordance is the existing public bridge:

```fsharp
Widget.ofControl : Control<'msg> -> Widget<'msg>   // already public from 065
```

An author builds the `Control<'msg>` with the legacy `CustomControl.create
definition attrs` and lifts it into the typed tree with `Widget.ofControl`. The
catalog row is marked bridge-typed so SC-001 "all 47 typed" is satisfied honestly
and the `066` cross-check skips a `Props`-type mapping for this one id.

## Reused entities (unchanged)

`Widget<'msg>`, `Control<'msg>`, `Attr<'msg>`, `AttrValue<'msg>`,
`ControlEventBinding<'msg>`, `Theme`, `ControlRenderResult<'msg>` (`Types.fsi`);
`TextInputModel`/`Msg`/`Effect` (`TextInput.fsi`); `CollectionModel`/`Msg`/`Effect`
(`Collections.fsi`); `DataGridModel`/`Msg`/`Effect` (`DataGrid.fsi`);
`ChartSeries`/`ChartPoint` and the graph types (`Charts.fsi`);
`CustomControlDefinition` (`CustomControl.fsi`); the existing legacy `*.create`
modules. **None of these change** (FR-007/FR-008).

## Validation rules (from requirements)

- No `Props` field is `obj`, untyped, or a string-named event (FR-003/SC-005) —
  enforced by a contract test that greps every new `.fsi` for `obj`.
- Every typed `view` lowers structurally equal to its legacy builder output modulo
  attribute order (FR-002/SC-002) — enforced per control in `TypedLoweringTests.fs`
  (the keystone 41-row parity matrix).
- Unset event callbacks (`None`) produce no binding (FR-005) — enforced in
  interaction tests.
- Stateful typed `init`/`update` equal the reused model's results (FR-004/SC-003)
  — asserted against `TextInput`/`Collections`/`DataGrid`/chart models directly.
- Each catalog `requiredAttribute` PascalCased is a `Props` field, and
  `catalogFacts` ids == typed ids (FR-012/SC-007) — enforced by the extended
  `CatalogTests.typedPropsById` cross-check after regenerating `catalogFacts` to 47.
- The regenerated `FS.Skia.UI.Controls` surface delta is additive-only (FR-010/
  SC-004) — enforced by `PackageSurfaceCheck`/`PerPackageSurfaceDiff`.

> Concrete type names above (e.g. `RadioItem`, `TabItem`, `MenuItem`, `TreeNode`,
> `GridLength`, `DockSide`, `ImageSource`, `ScrollState`, `GraphNode`/`GraphEdge`,
> `Orientation`) reuse the existing legacy types where they already exist in the
> Controls package; a typed `view`'s parity test pins the exact lowering and is the
> source of truth for the final field set per control. The sketches fix the
> taxonomy intent, not the byte-final signature — those are confirmed during
> implementation against each control's legacy builder.

# Typed Front-Door Authoring — Feature 089 (T011, US1)

**Claim (SC-001/SC-002, FR-003/FR-004):** from the *published* consumer surface
alone — the emitted `docs/api-surface/Controls/*.fsi` plus the `catalog.yml`
`typedModule:` index — a consumer authors a correct typed `Props` value and
`view` call for any supported control **without reflecting/decompiling
`FS.Skia.UI.Controls.dll`**.

The resolution path a consumer follows: **control id → `catalog.yml` `typedModule`
→ the `module`'s `*Props`/`view` in the published `.fsi`.**

## Three stateful controls authored from the published surface alone

Each `Props` record and `view` arity below was read **only** from the published
api-surface `.fsi` (no DLL probe). The legacy builder `.fsi` remain published
(additive), but none was consulted.

### 1. `text-box` → `typedModule: TextBox` (TextInputModel-backed)

Published `docs/api-surface/Controls/TextBoxWidget.fsi` declares
`TextBoxProps<'msg>` with `Id: ControlId`, `Mode: TextInputMode`, `Value: string`,
`ReadOnly: bool`, `Validation: ValidationState`, `OnChanged: (string -> 'msg)
option`, and `view: props -> TextInputModel -> Widget<'msg>`.

```fsharp
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Typed

let nameProps : TextBoxProps<Msg> =
    { TextBox.defaults (ControlId.ofString "name-field") with
        Value = "initial"
        OnChanged = Some (fun text -> NameChanged text) }

let nameModel, _effects = TextBox.init nameProps
let nameWidget = TextBox.view nameProps nameModel
```

### 2. `list-view` → `typedModule: ListView` (CollectionModel-backed)

Note the **id→module skew this feature makes navigable**: `list-view`'s legacy
`module` token is `Collections`, but its `typedModule` is `ListView`. Published
`CollectionsWidgets.fsi` declares `ListViewProps<'msg>` with `Id: ControlId`,
`Items: string list`, `OnSelected: (string -> 'msg) option`, and
`view: props -> CollectionModel -> Widget<'msg>`.

```fsharp
let itemsProps : ListViewProps<Msg> =
    { ListView.defaults (ControlId.ofString "files") with
        Items = [ "a.fs"; "b.fs"; "c.fs" ]
        OnSelected = Some (fun key -> FileSelected key) }

let itemsModel, _ = ListView.init itemsProps
let itemsWidget = ListView.view itemsProps itemsModel
```

### 3. `combo-box` → `typedModule: ComboBox` (CollectionModel-backed)

`combo-box` also skews (`module: Collections`, `typedModule: ComboBox`). Published
`CollectionsWidgets.fsi` declares `ComboBoxProps<'msg>` with `Id: ControlId`,
`Items: string list`, `OnChanged: (string -> 'msg) option`.

```fsharp
let pickProps : ComboBoxProps<Msg> =
    { ComboBox.defaults (ControlId.ofString "theme") with
        Items = [ "light"; "dark" ]
        OnChanged = Some (fun choice -> ThemePicked choice) }

let pickModel, _ = ComboBox.init pickProps
let pickWidget = ComboBox.view pickProps pickModel
```

## Whole-catalog coverage (not spot-checked)

Per-control spot checks would be incomplete. The T007 governance test
(`feature 089 TYPED-SURFACE-1 catalog TypedModule index`) mechanically asserts the
SC-001 "100%" claim: all **52** `catalog.yml` control ids map to a `TypedModule`
that is declared in an enrolled `src/Controls/Widgets/*.fsi` **and** exposes a
typed `view`, with the single bridge-typed `custom-control` (no `Props`/`view`)
explicitly excepted. The E1⟂E2 cross-check guarantees no `TypedModule` is a
dangling pointer. So the three worked examples above generalise to the whole
catalog by construction, not by sampling.

# Phase 1 Data Model: Typed Controls Front Door

Entities are the public typed authoring surface. Every field is strongly typed —
no `obj` payload, no string-named event (FR-005). Required values are
non-optional; optional values get their value from `defaults`.

## Widget<'msg> (the lowering seam)

The opaque public return type of every typed `view`.

- **Public surface (`.fsi`)**: `[<Sealed>] type Widget<'msg>` + `module Widget`.
- **Internal representation (`.fs` only)**: `{ Lowered: Control<'msg> }` (private
  record field; never on the `.fsi`).
- **Operations**:
  - `Widget.toControl : Widget<'msg> -> Control<'msg>` — the lowering accessor
    (used by render + the Elmish adapter). (FR-002)
  - `Widget.ofControl : Control<'msg> -> Widget<'msg>` — the migration bridge
    (drop a legacy `Control` into a typed `Stack.Children`). (FR-002)
  - `Widget.render : Theme -> Widget<'msg> -> ControlRenderResult<'msg>` —
    convenience = `Control.render theme (Widget.toControl w)`.
- **Round-trip invariant**: `Widget.toControl (Widget.ofControl c) = c` (spec edge
  case — bridging a legacy control then lowering reproduces it unchanged).

## Variable taxonomy (applied per Props record)

Each `Props` record draws fields from a fixed taxonomy so the six records stay
consistent and future controls follow the template: Identity, Content, Data,
Behavior, Variant, Layout, Theme/style, Accessibility, Events. Rule: required →
non-optional field; optional → defaulted via `defaults`.

## The six control entities

### TextBlock (pure, content-only)

`type TextBlockProps<'msg> = { Id: ControlId option; Text: string }`
- `defaults : TextBlockProps<'msg>` — `{ Id = None; Text = "" }`
- `view : TextBlockProps<'msg> -> Widget<'msg>`
- Lowers to ≡ `TextBlock.create [ TextBlock.text props.Text ]` (+ key when `Id`).

### Button (command/event + variant)

`type ButtonIntent = Primary | Secondary | Danger | Ghost`
`type ButtonProps<'msg> = { Id: ControlId option; Text: string; Enabled: bool; Intent: ButtonIntent; OnClick: 'msg option }`
- `defaults` — `{ Id=None; Text=""; Enabled=true; Intent=Primary; OnClick=None }`
- `view : ButtonProps<'msg> -> Widget<'msg>`
- Event rule: `OnClick = Some m` lowers to the same binding `Button.onClick m`
  produces; `OnClick = None` lowers to **no** event binding (edge case: an unset
  callback must not dispatch a null/default message). (FR-008)
- `Intent` lowers into the variant attribute the legacy path uses.

### CheckBox (boolean + (bool -> 'msg) event)

`type CheckBoxProps<'msg> = { Id: ControlId option; Text: string; Checked: bool; OnChanged: (bool -> 'msg) option }`
- `defaults` — `{ Id=None; Text=""; Checked=false; OnChanged=None }`
- `view : CheckBoxProps<'msg> -> Widget<'msg>`
- Lowers to ≡ `CheckBox.create [ CheckBox.text; CheckBox.checked'; CheckBox.onChanged ]`
  with `OnChanged=None` → no binding.

### Stack (layout composition over Widget children)

`type StackOrientation = Vertical | Horizontal`
`type StackProps<'msg> = { Id: ControlId option; Orientation: StackOrientation; Spacing: float; Children: Widget<'msg> list }`
- `defaults` — `{ Id=None; Orientation=Vertical; Spacing=0.0; Children=[] }`
- `view : StackProps<'msg> -> Widget<'msg>`
- Children lower via `Widget.toControl` into `Stack.children`, preserving order
  (spec US-2 — children render in order). Legacy controls bridge in via
  `Widget.ofControl`.

### TextBox (stateful — reuses TextInput model)

Reuses `TextInputModel` / `TextInputMsg` / `TextInputEffect` (FR-006). No parallel
state type.
`type TextBoxProps<'msg> = { Id: ControlId; Mode: TextInputMode; Value: string; ReadOnly: bool; Validation: ValidationState; OnChanged: (string -> 'msg) option }`
- `defaults : ControlId -> TextBoxProps<'msg>` (id is required identity).
- `init : TextBoxProps<'msg> -> TextInputModel * TextInputEffect list` — delegates
  to `TextInput.init props.Id props.Mode props.Value`.
- `update : TextInputMsg -> TextInputModel -> TextInputModel * TextInputEffect list`
  — delegates to `TextInput.update` (result equality asserted, §10.4).
- `view : TextBoxProps<'msg> -> TextInputModel -> Widget<'msg>` — lowers to the
  legacy `TextBox.create` attrs for the current model state.

### DataGrid (stateful — reuses DataGrid model)

Reuses `DataGridModel` / `DataGridMsg` / `DataGridEffect` (FR-006).
`type DataGridProps<'msg> = { Id: ControlId; Columns: DataGridColumn list; Rows: DataGridRow list; RowHeight: float; ViewportHeight: float; SelectedRows: Set<string>; OnSelectionChanged: (string list -> 'msg) option }`
- `defaults : ControlId -> DataGridProps<'msg>`.
- `init : DataGridProps<'msg> -> DataGridModel * DataGridEffect list` — delegates
  to `DataGrid.init props.Id props.Columns props.Rows.Length props.RowHeight props.ViewportHeight`.
- `update : DataGridMsg -> DataGridModel -> DataGridModel * DataGridEffect list` —
  delegates to `DataGrid.update`.
- `view : DataGridProps<'msg> -> DataGridModel -> Widget<'msg>` — lowers to the
  legacy `DataGrid.create columns` attrs for the current model state.

## Reused entities (unchanged)

`Control<'msg>`, `Attr<'msg>`, `AttrValue<'msg>`, `ControlEvent`,
`ControlEventBinding<'msg>`, `Theme`, `ControlRenderResult<'msg>` (Types.fsi);
`TextInputModel`/`Msg`/`Effect` (TextInput.fsi); `DataGridModel`/`Msg`/`Effect`,
`DataGridColumn`, `DataGridRow` (DataGrid.fsi). None of these change.

## Validation rules (from requirements)

- No `Props` field may be `obj` or a string-named event (FR-005) — enforced by a
  contract test that greps the new `.fsi` for `obj`.
- Every typed `view` lowers structurally-equal to the legacy builder output
  modulo attribute order (FR-004, SC-002) — enforced by `TypedLoweringTests.fs`.
- Unset event callbacks produce no binding (edge case) — enforced in interaction
  tests.
- Stateful typed `init`/`update` equal the existing control's results (FR-006) —
  asserted against `TextInput`/`DataGrid` directly.

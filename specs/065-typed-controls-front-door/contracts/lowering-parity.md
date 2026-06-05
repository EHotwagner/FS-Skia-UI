# Contract: Lowering Parity (the keystone)

Every typed `view` MUST lower to a `Control<'msg>` structurally equal to the
legacy authoring call it replaces, modulo attribute order (FR-004, SC-002).

## Parity assertion shape

```fsharp
// normalize: sort Attributes by Name (recursively over Children) so ordering
// differences are removed from the comparison.
let legacy = Button.create [ Button.text "Submit"; Button.enabled true; Button.onClick Save ]
let typed  = Typed.Button.view { Typed.Button.defaults with Text = "Submit"; OnClick = Some Save }
             |> Widget.toControl
Expect.equal (normalize typed) (normalize legacy) "typed Button lowers to legacy IR"
```

## Parity matrix (six controls × legacy ≡ typed)

| Control | Legacy builder compared against | Notable lowering rule |
| --- | --- | --- |
| TextBlock | `TextBlock.create [ TextBlock.text t ]` | content-only |
| Button | `Button.create [ text; enabled; onClick ]` | `OnClick=None` → no binding; `Intent` → variant attr |
| CheckBox | `CheckBox.create [ text; checked'; onChanged ]` | `OnChanged=None` → no binding |
| Stack | `Stack.create [ Stack.children cs ]` | children via `Widget.toControl`, order preserved |
| TextBox | `TextBox.create [ value; readOnly; validation; onChanged ]` | lowered for current `TextInputModel` |
| DataGrid | `DataGrid.create columns [ rows; selectedRows; … ]` | lowered for current `DataGridModel` |

## Event-binding parity (FR-008)

Dispatching a typed-authored binding yields the same `'msg` (command events) or
applies the same payload mapping (value-changed events) as the legacy path. An
unset callback (`None`) lowers to a control with **no** event binding.

## Stateful delegation parity (FR-006)

`Typed.TextBox.init`/`update` equal `TextInput.init`/`update`; `Typed.DataGrid`
likewise. Asserted directly against the existing control functions, not a copy.

## Round-trip bridge parity

`Widget.toControl (Widget.ofControl c) = c` — bridging a legacy control into a
typed container then lowering reproduces the original control unchanged.

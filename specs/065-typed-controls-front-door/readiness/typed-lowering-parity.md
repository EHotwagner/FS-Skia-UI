# Typed lowering parity — the keystone (065)

Proves SC-002 / FR-004: every typed `view` lowers to a `Control<'msg>`
**structurally equal** to the legacy authoring call it replaces, modulo attribute
order. Evidence: `tests/Controls.Tests/TypedLoweringTests.fs` (the parity matrix)
and `tests/Controls.Tests/InteractionTests.fs` (event-binding dispatch parity).

## How parity is proven

The typed `view` calls the **same legacy string-keyed builders** it replaces
(`TextBlock.create`/`Button.create`/`CheckBox.create`/`Stack.create`/
`TextBox.create`/`DataGrid.create`), so the lowered IR is equal by construction.
The test normalizes out attribute order (sorts attributes by name) and
canonicalizes event closures — because `Control<'msg>` is not an equality type
(its `AttrValue` DU carries a function case), an `EventValue f` is compared by the
message it produces for a representative `ControlEvent`, then the order-normalized
controls are compared via a deterministic `%A` projection.

## Parity matrix (six controls × legacy ≡ typed)

| Control | Legacy builder compared against | Notable lowering rule | Result |
| --- | --- | --- | --- |
| TextBlock | `TextBlock.create [ TextBlock.text t ]` | content-only | ✅ equal |
| Button | `Button.create [ text; enabled; style; onClick ]` | `Intent` → `Attr.style`; `OnClick=None` → no binding | ✅ equal |
| CheckBox | `CheckBox.create [ text; checked'; onChanged ]` | `OnChanged=None` → no binding; bool payload maps identically | ✅ equal |
| Stack | `Stack.create [ orientation; spacing; children ]` | children via `Widget.toControl`, **order preserved** | ✅ equal |
| TextBox | `TextBox.create [ value; readOnly; validation ] |> withKey id` | lowered for the current `TextInputModel.DraftText`/`Validation` | ✅ equal |
| DataGrid | `DataGrid.create columns [ rows; visibleRange; selectedRows; focusedCell ] |> withKey id` | lowered for the current `DataGridModel` | ✅ equal |

## Event-binding parity (FR-008)

- `Button.OnClick = Some m` dispatches the same `'msg` through `Control.dispatch`
  as `Button.onClick m`; `OnClick = None` lowers to a control with **no** event
  binding (asserted by `eventAttrs` being empty).
- `CheckBox.OnChanged = Some f` maps the boolean payload identically to the legacy
  `CheckBox.onChanged f`; `OnChanged = None` lowers to no binding.
- Disabled / read-only typed controls suppress dispatch identically to legacy.

## Stateful delegation parity (FR-006)

- `Typed.TextBox.init`/`update` return models and effects **equal** to
  `TextInput.init`/`TextInput.update` (asserted directly, not against a copy).
- `Typed.DataGrid.init`/`update` return models and effects **equal** to
  `DataGrid.init`/`DataGrid.update`.

## Round-trip bridge parity

`Widget.toControl (Widget.ofControl c) = c` — a legacy `Control` bridged into a
typed `Stack.Children` lowers back unchanged (asserted via `%A` projection).

## Result

100% parity across all six controls. Lowering is **real** — no `[S]`/`[S*]`
disclosure required; the Synthetic-Evidence Inventory in `tasks.md` is empty.
Full run: `dotnet test tests/Controls.Tests/Controls.Tests.fsproj` → 56 passed,
0 failed.

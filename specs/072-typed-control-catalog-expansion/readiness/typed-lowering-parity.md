# Typed Lowering-Parity Matrix — Catalog Breadth Expansion (072)

Keystone proof (FR-005, SC-002). For each new control, `view props |>
Widget.toControl` is structurally equal — order-normalized, events canonicalized
to the message they produce — to the explicit hand-written composition of existing
legacy builders. Lowering is **real**; no `[S]`.

Tests: `tests/Controls.Tests/TypedExpansionTests.fs`.

| control | typed `view` | ≡ explicit legacy composition | test |
|---------|--------------|-------------------------------|------|
| toggle-button | `ToggleButton.view` | `Button.create [ text; enabled; Attr.selected IsOn; onClick (OnToggle (not IsOn)); a11y Button ]` | `ToggleButton lowers structurally equal …` |
| split-button | `SplitButton.view` | `Toolbar.create [ children [ primary Button; trigger Button; Overlay (Menu items) ]; a11y Menu ]` | `SplitButton lowers structurally equal …` |
| date-picker | `DatePicker.view` | `Stack.create [ children [ TextBox field; trigger Button; Overlay (Grid of day Buttons) ]; a11y TextBox ]` | `DatePicker lowers structurally equal …` (+ empty-value case) |
| time-picker | `TimePicker.view` | `Stack.create [ children [ hour Button; ":" Label; minute Button ]; a11y TextBox ]` | `TimePicker lowers structurally equal …` |
| color-picker | `ColorPicker.view` | `Wrap.create [ children [ swatch Buttons … ]; a11y List ]` | `ColorPicker lowers structurally equal …` |

5 / 5 new controls pass the lowering-parity test (100%, SC-002).

## Cross-feature golden fixtures (coupling pointer)

The per-id catalog golden bytes for the 5 new ids live under the **066** feature
readiness, where the `066` catalog cross-check reads them:

`specs/066-typed-catalog-generation/readiness/parity-fixtures/`
- `Catalog.fs.<id>.txt` and `catalog.yml.<id>.txt` for
  `toggle-button`, `split-button`, `date-picker`, `time-picker`, `color-picker`.

These are captured byte-for-byte from `CatalogGen.renderFSharpRow` /
`renderYamlRow` (the single source), not fabricated literals. **066 archival must
retain these fixtures** — they are live inputs to `CatalogTests.fs` "each generated
row is byte-identical to its captured pre-migration row".

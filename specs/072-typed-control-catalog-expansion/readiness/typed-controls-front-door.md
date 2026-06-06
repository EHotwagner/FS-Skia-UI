# Typed Controls Front Door — Catalog Breadth Expansion (072)

**Feature tier**: Tier 1 (contracted change). `Route` escalates to
`controls-public-surface` (public `.fsi` + catalog facts change).
**Affected layer**: `FS.Skia.UI.Controls` (package-internal typed modules under
`FS.Skia.UI.Controls.Typed`), additive public API only.
**Principle IV applicability**: **Not applicable** — the five new controls are
stateless from the framework's view. No new `Model`/`Msg`/`Effect` is introduced;
product-owned values (`IsOn`, `Value`, `Selected`, `IsOpen`) live in the `Props`
record, mirroring `CheckBox`/`Switch`. The MVU evidence obligation is met by the
per-control callback-dispatch interaction tests (`InteractionTests.fs`,
T013/T025), not a new pure-`update` suite.

## The five new controls

| id | Module | Lowers to (existing legacy builders only) | Events |
|----|--------|-------------------------------------------|--------|
| `toggle-button` | `ToggleButton` | a `Button` carrying `Attr.selected IsOn`; activation → `OnToggle (not IsOn)` | `onToggle` |
| `split-button` | `SplitButton` | a `Toolbar` of [ primary `Button`; trigger `Button`; an `Overlay` `Menu` of `Items` ] | `onClick`, `onSelected` |
| `date-picker` | `DatePicker` | a `Stack` of [ read-only field `TextBox`; trigger `Button`; an `Overlay` `Grid` of day `Button`s ] | `onChange` |
| `time-picker` | `TimePicker` | a `Stack` of [ hour segment `Button`; `:` `Label`; minute segment `Button` ] | `onChange` |
| `color-picker` | `ColorPicker` | a `Wrap` of colored swatch `Button` cells (`Selected` highlighted) | `onSelected` |

Each `view : Props<'msg> -> Widget<'msg>` composes **only** existing legacy
builders (`Button`/`Stack`/`Toolbar`/`Menu`/`Overlay`/`Grid`/`Wrap`/`TextBox`/
`Label`). **No new `StandardControlKind` variant, no renderer/layout change, no new
dependency** (date/time use BCL `System.DateOnly`/`System.TimeOnly`;
`ColorPicker` reuses `FS.Skia.UI.Scene.Color`). SC-007 holds.

## Lowering is REAL — no `[S]`

Lowering parity is proven against **real composed IR**: every `view |>
Widget.toControl` is asserted structurally equal (order-normalized, events
canonicalized to the message they produce) to the explicit hand-written
composition of existing legacy builders, in
`tests/Controls.Tests/TypedExpansionTests.fs` (T012 DatePicker keystone; T024 the
other four). There is **no mock, stub, placeholder, or synthetic fixture** on any
new-control path. `EvidenceAudit` is expected PASS with no `[S]`/`[S*]`/`[SEH]`
disclosures (SC-006). The 5-control parity matrix is in
[`typed-lowering-parity.md`](./typed-lowering-parity.md).

## Accessibility (FR-009)

Each lowered root carries an explicit `Attr.accessibility` metadata with its
catalog role (`Button`/`Menu`/`TextBox`/`TextBox`/`List`), an accessible name, and
a focusable keyboard affordance (Enter/Space activation; popup-bearing controls
add arrow-key navigation). Asserted at ≥2 viewports by
`AccessibilityTests.fs`/`RenderingTests.fs` (T014/T026). See
[`controls-rendering.md`](./controls-rendering.md).

## Per-control independent validation paths

- **DatePicker (US1)**: author `DatePicker.view { defaults with Value = Some d;
  OnChange = Some msg }` in the gallery `typedAuthoringPanel` → render at 320×240
  and 1024×768 with no diagnostics → confirm lowering parity → selecting day `n`
  dispatches `OnChange (DateOnly(y, m, n))`. `Value = None` renders an empty field
  and empty calendar and dispatches nothing.
- **ToggleButton (US3)**: `IsOn = true` lowers to the pressed-state `Button`;
  activation dispatches `OnToggle false`; `OnToggle = None` ⇒ no binding.
- **SplitButton (US3)**: primary activation dispatches `OnClick`; a menu item
  dispatches `OnSelected key`; empty `Items` lowers to an empty menu without
  failing.
- **TimePicker (US3)**: the hour/minute segments dispatch `OnChange` with the
  advanced `TimeOnly`; `Value = None` shows `--`/`--` and dispatches nothing.
- **ColorPicker (US3)**: a swatch cell dispatches `OnSelected swatch`; the
  `Selected` cell is highlighted (`Attr.selected`); empty `Swatches` lowers to an
  empty grid.

All five are dogfooded end-to-end in
`samples/ControlsGallery/Program.fs` `typedAuthoringPanel` (FR-010, T017/T030).

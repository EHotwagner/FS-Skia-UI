# Phase 1 Data Model — New Typed Controls

All types are **additive** to `FS.Skia.UI.Controls.Typed`. Field classes reference the
variable taxonomy (plan §3.4 of the front-door plan): Identity / Content / Data / Behavior /
Variant / Layout / Accessibility / Events. Every event is an **optional callback** that
lowers to **no binding** when `None`. No field is `obj` or a string-encoded structured value.

Per **FR-009**, every control's lowered tree carries its accessibility **role**, an accessible
**name**, and a **keyboard affordance** — a focusable trigger plus the activation key(s)
appropriate to the control (Enter/Space to toggle or open; Arrow keys to move within a popup
calendar / menu / swatch grid). The rendering/accessibility suites assert the role and the
keyboard affordance at ≥2 viewports (T014, T026).

## Shared / value types

| Type | Definition | Notes |
| --- | --- | --- |
| `SplitButtonItem` | `{ Key: string; Label: string }` | A secondary command in a split button's popup menu. `Key` is the selection identity dispatched by `OnSelected`. Mirrors existing string-keyed menu/collections items. |
| `ColorSwatch` | `{ Name: string; Color: Color }` | One palette entry. `Color` is the reused `FS.Skia.UI.Scene.Color` (no new dependency, no hex string). `Name` is the accessible label. |

> Date/time use BCL `System.DateOnly` / `System.TimeOnly` directly — no wrapper type.

## ToggleButton (button family)

- **Catalog**: id `toggle-button`, Module `ToggleButton`, category `input`,
  RequiredAttributes `["text"]`, Events `["onToggle"]`, role `Button`.
- **Props** `ToggleButtonProps<'msg>`:

  | Field | Type | Class | Notes |
  | --- | --- | --- | --- |
  | `Id` | `ControlId option` | Identity | |
  | `Text` | `string` | Content | required attribute |
  | `IsOn` | `bool` | Data | product-owned pressed state |
  | `Enabled` | `bool` | Behavior | |
  | `OnToggle` | `(bool -> 'msg) option` | Events | dispatched with the **next** state on activation |

- **Lowers to**: a `Button` carrying the pressed/selected visual state reflecting `IsOn`,
  whose activation event maps to `OnToggle (not IsOn)`. `OnToggle = None` ⇒ no binding.

## SplitButton (button family)

- **Catalog**: id `split-button`, Module `SplitButton`, category `input`,
  RequiredAttributes `["text"]`, Events `["onClick"; "onSelected"]`, role `Menu`.
- **Props** `SplitButtonProps<'msg>`:

  | Field | Type | Class | Notes |
  | --- | --- | --- | --- |
  | `Id` | `ControlId option` | Identity | |
  | `Text` | `string` | Content | primary action label (required) |
  | `Enabled` | `bool` | Behavior | |
  | `IsOpen` | `bool` | Behavior | popup visibility (product-owned) |
  | `Items` | `SplitButtonItem list` | Data | secondary commands; empty ⇒ empty/disabled popup |
  | `OnClick` | `'msg option` | Events | primary action |
  | `OnSelected` | `(string -> 'msg) option` | Events | dispatched with the chosen item `Key` |

- **Lowers to**: a `Toolbar`/`Stack` of [ primary `Button`; dropdown-trigger `Button` ] plus
  an `Overlay`/`Menu` of the `Items` shown when `IsOpen`. Empty `Items` lowers to an
  empty/disabled menu (must not fail to lower). `None` callbacks ⇒ no binding. The catalog
  `role` `Menu` names the composite menu-button widget at the root; the primary action stays a
  focusable child `Button`, so assistive tech announces both the action and the menu.

## DatePicker (date-time + picker family) — P1 keystone

- **Catalog**: id `date-picker`, Module `DatePicker`, category `input`,
  RequiredAttributes `[]`, Events `["onChange"]`, role `TextBox`.
- **Props** `DatePickerProps<'msg>`:

  | Field | Type | Class | Notes |
  | --- | --- | --- | --- |
  | `Id` | `ControlId option` | Identity | |
  | `Value` | `DateOnly option` | Data | `None` = no selection (empty field) |
  | `Enabled` | `bool` | Behavior | |
  | `IsOpen` | `bool` | Behavior | calendar popup visibility (product-owned) |
  | `OnChange` | `(DateOnly -> 'msg) option` | Events | dispatched with the chosen day |

- **Lowers to**: `Border`/`Stack` of [ field control showing the formatted `Value` or a
  placeholder; trigger `Button` ] plus an `Overlay` popup (when `IsOpen`) containing a
  `Stack`/`Grid` of day `Button`s. `Value = None` ⇒ placeholder, no message; `OnChange =
  None` ⇒ no binding.

## TimePicker (date-time family)

- **Catalog**: id `time-picker`, Module `TimePicker`, category `input`,
  RequiredAttributes `[]`, Events `["onChange"]`, role `TextBox`.
- **Props** `TimePickerProps<'msg>`:

  | Field | Type | Class | Notes |
  | --- | --- | --- | --- |
  | `Id` | `ControlId option` | Identity | |
  | `Value` | `TimeOnly option` | Data | `None` = no selection |
  | `Enabled` | `bool` | Behavior | |
  | `OnChange` | `(TimeOnly -> 'msg) option` | Events | dispatched with the chosen time |

- **Lowers to**: a `Stack` of hour/minute segment controls (composed from existing
  field/`Button` builders) showing `Value` or a placeholder. Out-of-range time is
  unrepresentable (`TimeOnly` is total over 00:00–23:59:59).

## ColorPicker (picker family — palette/swatch)

- **Catalog**: id `color-picker`, Module `ColorPicker`, category `selection`,
  RequiredAttributes `["swatches"]`, Events `["onSelected"]`, role `List`.
- **Props** `ColorPickerProps<'msg>`:

  | Field | Type | Class | Notes |
  | --- | --- | --- | --- |
  | `Id` | `ControlId option` | Identity | |
  | `Swatches` | `ColorSwatch list` | Data | the palette (required attribute); empty ⇒ empty grid |
  | `Selected` | `ColorSwatch option` | Data | currently selected swatch |
  | `OnSelected` | `(ColorSwatch -> 'msg) option` | Events | dispatched with the chosen swatch |

- **Lowers to**: a `Wrap`/`Grid` of colored `Border`/`Button` cells (one per swatch), the
  `Selected` cell visually highlighted. Empty `Swatches` lowers to an empty grid (must not
  fail). `OnSelected = None` ⇒ no binding. (Full color-wheel/gradient is out of scope.)

## Catalog fact rows (single source — `CatalogGen.catalogFacts`, 47 → 52)

| id | Module | category | RequiredAttributes | Events | role |
| --- | --- | --- | --- | --- | --- |
| `toggle-button` | `ToggleButton` | input | `["text"]` | `["onToggle"]` | Button |
| `split-button` | `SplitButton` | input | `["text"]` | `["onClick";"onSelected"]` | Menu |
| `date-picker` | `DatePicker` | input | `[]` | `["onChange"]` | TextBox |
| `time-picker` | `TimePicker` | input | `[]` | `["onChange"]` | TextBox |
| `color-picker` | `ColorPicker` | selection | `["swatches"]` | `["onSelected"]` | List |

`supportedCount`: `catalog.yml` header 47 → 52; `CatalogTests.fs` assertion 47 → 52;
`Catalog.supportedCount ()` (row count) updates automatically. The `typedPropsById`
cross-check gains 5 entries mapping each id to its `*Props` type; each `RequiredAttributes`
entry (PascalCased) must be a field of that `Props` type.

## Lowering-parity fixtures

One per new control: the explicit hand-written composition of existing legacy builders the
`view` must equal (order-normalized, events canonicalized). These are the `typed-lowering-parity`
keystone fixtures (R4). Catalog golden bytes (`Catalog.fs.<id>.txt` / `catalog.yml.<id>.txt`)
are captured from the generator output, one pair per new id.

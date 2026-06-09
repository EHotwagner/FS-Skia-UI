---
title: Controls Gallery — Selection & Toggles Page Spec
category: Controls Showcase specs
categoryindex: 8
---

# Controls Gallery — Selection & Toggles Page Spec

Inherits the shell, palette, pointer contract, keyboard contract, and
determinism rules from [`00-controls-gallery-overview.md`](00-controls-gallery-overview.md).

## Goal

Demonstrate every boolean and selection control — single and multi — with
pointer-driven selection, including the color picker's swatch palette wired to the
gallery accent.

## Controls Demonstrated

| Control | Module | Required | Events | Demonstrates |
|---------|--------|----------|--------|--------------|
| check-box | CheckBox | `text` | `onChanged` | Boolean with checked/indeterminate |
| radio-group | RadioGroup | `items` | `onChanged` | Single choice from a visible set |
| switch | Switch | — | `onChanged` | Compact boolean setting |
| list-box | Collections | `items` | `onSelected` | Single-selection list |
| multi-select-list | Collections | `items` | `onChanged` | Multi-selection with selected keys |
| combo-box | Collections | `items` | `onChanged` | Compact dropdown selection |
| color-picker | ColorPicker | `swatches` | `onSelected` | Palette swatch selection |

## User Experience

The page is a settings-style form. The user clicks checkboxes and switches to flip
booleans, picks one option from a radio group and one row from a list box, selects
several rows in the multi-select list, chooses from a combo dropdown, and picks an
accent from the color picker's swatches. A live "Selection summary" reflects all
current selections.

## Layout

- A heading `Selection & Toggles` and description.
- A column of single-value controls: a group of `CheckBox`es (including one
  indeterminate), a `RadioGroup`, and a row of `Switch`es.
- A `ListBox` (single-select) beside a `MultiSelectList` (multi-select), each
  ~8 rows tall and scrollable.
- A `ComboBox` of options and a `ColorPicker` whose swatches are the categorical
  series palette plus the three accent variants.
- A right-hand "Selection summary" `Panel`.

## Mouse & Pointer Interactions

- `Click` a checkbox or switch toggles it and fires `onChanged`.
- `Click` a radio option selects it and deselects its siblings.
- `Click` a list-box row selects it (single); `Click` a multi-select row toggles
  its membership; `Shift+Click` extends a contiguous range; `Ctrl+Click` toggles
  one row without clearing others.
- `Click` the combo box opens its dropdown; `Click` an item selects it and closes
  the dropdown.
- `Click` a color-picker swatch fires `onSelected`; selecting an accent swatch also
  updates the gallery accent live.
- Wheel `Scroll` scrolls the list bodies; `HoverEnter` highlights the row/swatch
  under the pointer.

## Keyboard

- `Space` toggles the focused checkbox/switch; `Arrow` keys move within the radio
  group, lists, and combo.
- `Enter` commits a combo selection; `Esc` closes the combo dropdown.
- `Ctrl+Click` / `Shift+Click` equivalents: `Ctrl+Space` toggles, `Shift+Arrow`
  range-extends in the multi-select list.

## Core Behaviors

- Checkbox supports checked, unchecked, and indeterminate; the indeterminate one
  resolves to checked on first toggle.
- Radio group enforces exactly one selection.
- List box holds exactly one selected key; multi-select holds a set of keys with
  range and toggle semantics.
- Combo box reflects its single selected item in its closed state.
- The color picker reports the selected swatch; accent swatches retint the gallery.

## Data Model

- Per-checkbox and per-switch boolean (or tri-state) values.
- Radio group options and selected key.
- List-box selected key; multi-select selected key set.
- Combo options and selected key; combo open flag.
- Color-picker swatches and selected color.

## Visual / Palette Requirements

- Checked boxes, the on switch, the selected radio, and selected rows use accent /
  accent-soft; the selected swatch shows an accent ring.
- Hovered rows use surface-raised; focused controls show the focus ring.
- The indeterminate checkbox uses a distinct dash mark, not a check.

## App State

Track: all boolean/tri-state values; radio selection; list and multi-select
selections; combo selection and open state; selected swatch; hovered ids;
selection summary snapshot.

## Determinism and Evidence

- Accept a seed for sample list contents.
- Evidence mode toggles a checkbox and a switch, picks a radio option, selects a
  list-box row, range-selects three multi-select rows, opens and selects a combo
  item, and picks an accent swatch.
- Evidence outcome: all selection states, the multi-select key set, the chosen
  accent, and close reason.
- Screenshot evidence shows the multi-select with a contiguous range selected, the
  combo open, and the color picker with a swatch chosen.

## Acceptance Criteria

- Each boolean control toggles by pointer and keyboard and fires `onChanged`.
- The radio group keeps exactly one selection.
- Single-select holds one row; multi-select supports click-toggle, `Shift` range,
  and `Ctrl` toggle.
- The combo opens, selects, and closes, reflecting the selection when closed.
- Selecting an accent swatch retints the gallery live.
- The indeterminate checkbox shows a dash and resolves on first toggle.

## Out of Scope

- Editable / autocomplete combo boxes.
- Drag-reordering list rows.
- A full HSV/RGB color wheel beyond the fixed swatch palette.

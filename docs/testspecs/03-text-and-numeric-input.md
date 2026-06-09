---
title: Controls Gallery — Text & Numeric Input Page Spec
category: Controls Showcase specs
categoryindex: 8
---

# Controls Gallery — Text & Numeric Input Page Spec

Inherits the shell, palette, pointer contract, keyboard contract, and
determinism rules from [`00-controls-gallery-overview.md`](00-controls-gallery-overview.md).

## Goal

Demonstrate every value-entry control — single- and multi-line text, numeric,
slider, and the date/time pickers — including pointer-driven editing, caret
placement, drag-to-set on the slider, and popup pickers, with a live bound preview
of all values.

## Controls Demonstrated

| Control | Module | Required | Events | Demonstrates |
|---------|--------|----------|--------|--------------|
| text-box | TextBox | `value` | `onChanged` | Single-line entry, validation |
| text-area | TextArea | `value` | `onChanged` | Multi-line entry, wrapping |
| numeric-input | NumericInput | `value` | `onChanged` | Numeric editor with step buttons |
| slider | Slider | `value` | `onChanged` | Drag-to-set continuous value |
| date-picker | DatePicker | — | `onChange` | Typed date entry + popup calendar |
| time-picker | TimePicker | — | `onChange` | Hour/minute segment entry |

## User Experience

The page is a form. The user clicks into a field to place the caret and types;
clicks and drags the slider thumb to set a value; opens the date picker's calendar
and clicks a day; and steps the numeric input with its buttons or the wheel. A
live "Bound values" panel mirrors every field's current value, proving two-way
binding through `onChanged` / `onChange`.

## Layout

- A heading `Text & Numeric Input` and description.
- A two-column `Grid` of labeled fields: text box, text area, numeric input,
  slider, date picker, time picker — each with a `Label` and a
  `ValidationMessage` slot.
- A right-hand "Bound values" `Panel` mirroring each field live.
- The text box includes an inline validation rule (non-empty, max length); the
  numeric input enforces a min/max range.

## Mouse & Pointer Interactions

- `Click` in a text box / text area places the caret and focuses the field
  (`FocusMovedByPointer`).
- `DragBegin`/`DragMove`/`DragEnd` on the slider thumb set its value continuously,
  honoring the 4.0 px drag threshold; a click on the track jumps the thumb.
- `Click` on the numeric input's step buttons increments/decrements by the step;
  wheel `Scroll` over it adjusts by one step.
- `Click` on the date picker opens its popup calendar; `Click` on a day selects it
  and closes the popup, firing `onChange`.
- `Click` on the time picker's hour/minute segments selects a segment; wheel or
  step adjusts it.
- `HoverEnter` on any field shows its hover border; the status strip narrates the
  pointer kind, target, and coordinates.

## Keyboard

- `Tab` / `Shift+Tab` moves between fields.
- Printable keys edit the focused text field; `Arrow` keys move the caret.
- `Arrow Up`/`Down` on the numeric input and slider adjust by one step.
- `Esc` closes an open picker popup without committing; `Enter` commits.

## Core Behaviors

- Each field is two-way bound: edits dispatch `onChanged`/`onChange` and the bound
  preview updates immediately.
- The text box validation shows an error marker and message on an empty or
  over-length value and a valid marker otherwise.
- The slider clamps to its min/max; its value is continuous within range.
- The numeric input clamps to range and rounds to its step.
- The date and time pickers parse typed input and reflect popup selection; invalid
  typed input shows a stable error rather than crashing.

## Data Model

- Per-field current value (string, number, date, time) and validation state.
- Slider min/max/step; numeric min/max/step.
- Picker open/closed flags and popup selection.
- The mirrored bound-values snapshot.

## Visual / Palette Requirements

- Fields use surface fill with border-divider outlines; focused fields show the
  2px focus ring; hovered fields use the accent border.
- Valid state uses the success role; invalid uses the danger role on both the
  marker and the `ValidationMessage`.
- The slider track is border-divider; its filled portion and thumb use accent.

## App State

Track: per-field values and validation; slider/numeric ranges; picker open state
and selections; the bound-values snapshot; focused field id; drag-in-progress flag.

## Determinism and Evidence

- Accept a seed for any prefilled sample values.
- Evidence mode focuses the text box and types a value, drags the slider to a set
  fraction, steps the numeric input, opens the date picker and selects a day, and
  triggers one validation error then corrects it.
- Evidence outcome: final bound values, validation transitions, slider value,
  selected date/time, drag count, and close reason.
- Screenshot evidence shows the focused text box, the slider mid-range, the open
  date calendar, and one field in the error state.

## Acceptance Criteria

- Clicking a field places the caret and focuses it; typing dispatches `onChanged`
  and updates the bound preview.
- Dragging the slider sets a continuous value within range; the threshold prevents
  a click from registering as a drag.
- The numeric input steps by pointer buttons and wheel and clamps to range.
- The date and time pickers open, select by pointer, and fire `onChange`; invalid
  typed input shows a stable error.
- Validation shows correct success/danger states and messages.

## Out of Scope

- Rich-text editing or formatting toolbars in the text area.
- Locale-specific date/time parsing beyond a single fixed format.
- Multi-thumb / range sliders.

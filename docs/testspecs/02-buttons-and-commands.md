---
title: Controls Gallery — Buttons & Commands Page Spec
category: Controls Showcase specs
categoryindex: 8
---

# Controls Gallery — Buttons & Commands Page Spec

Inherits the shell, palette, pointer contract, keyboard contract, and
determinism rules from [`00-controls-gallery-overview.md`](00-controls-gallery-overview.md).

## Goal

Demonstrate every command control across its full pointer state machine —
hover, press, release, click — including icon-only, toggle, and split variants,
with each activation counted and echoed.

## Controls Demonstrated

| Control | Module | Required | Events | Demonstrates |
|---------|--------|----------|--------|--------------|
| button | Button | `text` | `onClick` | Primary, secondary, danger, ghost intents |
| icon-button | IconButton | `text` | `onClick` | Icon-only command with tooltip |
| toggle-button | ToggleButton | `text` | `onToggle` | Product-owned pressed/on state |
| split-button | SplitButton | `text` | `onClick`, `onSelected` | Primary action plus secondary menu |

## User Experience

The page shows a command bar of buttons in every intent, an icon-button toolbar,
a row of toggle buttons whose on/off state persists, and a split button whose
caret opens a menu of secondary actions. A live activation log shows the last few
commands. Every button visibly transitions through hover → pressed → released as
the pointer acts on it.

## Layout

- A heading `Buttons & Commands` and description.
- A `Wrap` of `Button` instances: Primary, Secondary, Danger, Ghost, plus a
  Disabled button that ignores pointer input.
- A `Toolbar` row of `IconButton`s (e.g. save, copy, delete) each with a tooltip.
- A row of `ToggleButton`s (e.g. Bold, Italic, Underline) holding independent
  on/off state.
- A `SplitButton` labeled `Save` whose caret opens a menu (`Save`, `Save As…`,
  `Save All`).
- A right-hand activation log (`ListView`) of the last 8 commands.

## Mouse & Pointer Interactions

- `HoverEnter` / `HoverLeave` drive each button's hover state and tooltip.
- `PressedDown` shows the pressed fill; `ReleasedUp` inside the bounds fires
  `Click`; release outside the bounds cancels without firing.
- `Click` on a button appends `<id> clicked` to the activation log and increments
  a per-button counter.
- `Click` on a toggle button flips its `onToggle` state and recolors it.
- `Click` on the split button's primary region fires `onClick`; `Click` on its
  caret opens the secondary menu, and selecting an item fires `onSelected`.
- The Disabled button shows no hover/press response and never fires.

## Keyboard

- `Tab` moves across all commands in order.
- `Enter` / `Space` activates the focused button or flips the focused toggle.
- `Arrow Down` on the focused split button opens its menu.

## Core Behaviors

- A click is press-inside followed by release-inside; release-outside cancels.
- Intent (`primary`/`secondary`/`danger`/`ghost`) drives the button's palette role.
- Toggle state is product-owned and survives navigation away and back.
- The split button exposes both a primary command and a secondary menu.
- Disabled commands are inert to pointer and keyboard.

## Data Model

- A button descriptor list (id, label, intent, enabled, click count).
- Per-toggle on/off state keyed by id.
- Split-button menu items and the selected secondary action.
- A bounded activation log (most-recent-first, capped at 8).

## Visual / Palette Requirements

- Primary uses accent fill; secondary uses surface-raised with accent border;
  danger uses the danger role; ghost is borderless until hovered.
- Pressed uses the accent-hover shade; toggled-on uses accent-soft with accent text.
- The disabled button uses muted foreground and shows no state changes.

## App State

Track: button descriptors and counts; toggle states; split-button menu and
selection; activation log; hovered/pressed/focused ids.

## Determinism and Evidence

- Evidence mode clicks one button of each intent, flips one toggle, opens the
  split menu and selects an item, and attempts the disabled button (no effect).
- Evidence outcome: click counts per button, toggle states, the selected secondary
  action, the disabled-button no-op, and close reason.
- Screenshot evidence shows one button mid-press, one toggle on, and the split menu
  open.

## Acceptance Criteria

- Every button transitions hover → pressed → released and fires `Click` only on
  release-inside.
- Release-outside cancels without firing.
- Each intent shows its correct palette role; ghost reveals on hover.
- Toggle state flips, recolors, and persists across navigation.
- The split button fires `onClick` for the primary region and `onSelected` from
  its menu.
- The disabled button is inert.

## Out of Scope

- Long-press, double-click, or chorded mouse gestures.
- Asynchronous command results or progress within a button.

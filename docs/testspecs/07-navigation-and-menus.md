---
title: Controls Gallery — Navigation & Menus Page Spec
category: Controls Showcase specs
categoryindex: 8
---

# Controls Gallery — Navigation & Menus Page Spec

Inherits the shell, palette, pointer contract, keyboard contract, and
determinism rules from [`00-controls-gallery-overview.md`](00-controls-gallery-overview.md).

## Goal

Demonstrate the navigation and command-surface controls — page tabs, a menu bar,
a right-click context menu, and a toolbar — fully driven by pointer hover and
click, including secondary-button activation.

## Controls Demonstrated

| Control | Module | Required | Events | Demonstrates |
|---------|--------|----------|--------|--------------|
| tabs | Tabs | `items` | `onChanged` | Active-page selection within the page |
| menu | Menu | `items` | `onSelected` | Top menu bar with submenus |
| context-menu | Menu | `items` | `onSelected` | Right-click contextual commands |
| toolbar | Toolbar | `children` | `onClick` | Compact grouped command strip |

## User Experience

The page hosts its own inner tab strip switching between sub-panels, a menu bar
whose menus open on click and reveal items, a toolbar of grouped commands, and a
large surface that opens a context menu on secondary-click at the pointer
location. A command log records every menu/toolbar selection.

## Layout

- A heading `Navigation & Menus` and description.
- A `Menu` bar across the top (`File`, `Edit`, `View`) with items per menu.
- A `Toolbar` row of grouped `IconButton`/`Button` commands with separators.
- A `Tabs` strip (`Overview`, `Details`, `Activity`) over an inner content panel
  whose body changes with the selected tab.
- A large "Right-click here" surface that raises a `ContextMenu`.
- A command log (`ListView`) of recent selections.

## Mouse & Pointer Interactions

- `Click` a menu-bar title opens its dropdown; `Click` an item fires `onSelected`,
  appends to the log, and closes the menu; clicking elsewhere closes it.
- `HoverEnter` moves the menu highlight between items while a menu is open.
- `Click` a tab fires `onChanged` and swaps the inner panel; the active tab is
  visually distinct.
- `Click` a toolbar command fires `onClick` and logs it; hovering shows its
  tooltip.
- `PressedDown` with `PointerButton.Secondary` on the context surface opens the
  `ContextMenu` anchored at the pointer `(x, y)`; selecting an item fires
  `onSelected` and logs it; `Esc` or an outside click dismisses it.

## Keyboard

- `Arrow Left`/`Right` move between tabs; `Enter` activates the focused tab.
- `Alt`+letter (or `F10`) opens the corresponding menu; `Arrow` keys move within an
  open menu; `Enter` selects; `Esc` closes.
- `Shift+F10` opens the context menu at the focused surface.

## Core Behaviors

- Exactly one tab is active; switching tabs preserves each panel's own sub-state.
- A menu opens on click, closes on selection or outside click, and shows at most
  one open menu at a time.
- The context menu opens at the pointer position, never off-screen (it flips to
  stay within bounds), and dismisses cleanly.
- The toolbar groups commands with separators and reflects disabled commands as
  inert.

## Data Model

- Inner tab items and the active tab key; per-tab sub-state.
- Menu definitions (titles + items) and the open-menu key.
- Toolbar command list with enabled flags.
- Context-menu items, open flag, and anchor coordinates.
- A bounded command log.

## Visual / Palette Requirements

- The active tab uses an accent underline/fill; open menu titles use accent-soft.
- Hovered menu/context items use surface-raised; the context menu casts an
  elevation shadow over content.
- Toolbar separators use border-divider; disabled commands use muted foreground.

## App State

Track: active tab and per-tab state; open menu; toolbar enabled flags; context-menu
open flag and anchor; command log; hovered ids.

## Determinism and Evidence

- Menu/tab/toolbar contents are fixed (seed-independent).
- Evidence mode switches tabs in order, opens a menu and selects an item, clicks a
  toolbar command, and secondary-clicks the surface to open and select a context
  item.
- Evidence outcome: tabs visited, menu/toolbar/context selections, context-menu
  anchor, and close reason.
- Screenshot evidence shows an open menu, the active tab's panel, and the context
  menu open at a pointer anchor.

## Acceptance Criteria

- Tabs switch by pointer and keyboard, preserving per-tab state.
- Menus open, select (firing `onSelected`), and close correctly with one open at a
  time.
- The toolbar fires `onClick` and shows tooltips; disabled commands are inert.
- Secondary-click opens the context menu at the pointer, kept on-screen, and
  selection fires `onSelected`.
- `Esc` and outside clicks dismiss open menus and the context menu.

## Out of Scope

- Nested multi-level submenus beyond one level.
- Customizable / draggable toolbar layout.
- Tab reordering or closing by pointer.

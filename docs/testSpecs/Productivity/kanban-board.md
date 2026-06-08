---
title: Kanban Board Demo Spec
category: Productivity specs
categoryindex: 10
---

# Kanban Board Demo Spec

## Goal

Build a complete Kanban board demo that exercises fixed columns, movable cards, card creation and editing, moving cards across columns with the keyboard, work-in-progress counts, and evidence-friendly rendering.

## User Experience

The app opens to a board with several columns such as `Backlog`, `In Progress`, and `Done`, each holding cards. The user moves a focused card between and within columns, adds new cards, and edits titles. The layout should be readable, fast to understand, and deterministic under a seed.

## Layout

- A horizontal row of fixed columns, each with a header showing its title and card count.
- Each column holds a vertical, scrollable stack of cards.
- A focused-card indicator shows which card the keyboard acts on.
- A status strip showing total cards, the focused column, and the focused card title.

## Controls

- `Arrow Left` / `Arrow Right`: move focus between columns.
- `Arrow Up` / `Arrow Down`: move focus between cards in a column.
- `Shift+Arrow`: move the focused card in that direction.
- `N`: create a new card in the focused column and begin editing it.
- `Enter` / `F2`: edit the focused card title.
- `Esc`: cancel an edit.
- `Delete`: remove the focused card.

## Core Behaviors

- Columns are fixed in number and order for the base mode.
- A card belongs to exactly one column and one position within it.
- Moving a card across a column boundary inserts it at a stable position in the target column.
- Card creation appends to the focused column and focuses the new card.
- Editing must reject an empty title and restore the previous title on cancel.
- Focus must always reference a valid card or an empty column placeholder.

## Data Model

- An ordered list of columns, each with an ordered list of cards.
- Each card has a stable id and a title.
- A focus reference of column index and card index, and an edit buffer.

## Visual Requirements

- Show every column header with its title and live card count.
- The focused card must be clearly outlined and distinct from the rest.
- Empty columns must show a clear placeholder rather than collapsing.
- Column headers and the status strip must keep stable dimensions while cards scroll.

## App State

Track at minimum:

- Columns and their ordered cards with stable ids.
- Focused column and card indices, edit-in-progress flag, and edit buffer.
- Per-column scroll offsets and random seed.

## Determinism and Evidence

- Accept an optional seed for any generated sample cards.
- Evidence mode should inject a deterministic script that creates a card, edits its title, and moves it to the next column.
- Evidence outcome should include frame count, move count, total cards, focused column, focused card title, and close reason.
- Screenshot evidence should show all columns, their counts, and the focused card.

## Acceptance Criteria

- Focus moves between columns and cards within board bounds.
- Moving a card across a boundary updates both column counts.
- Creating a card focuses it and begins editing.
- Editing rejects an empty title and restores on cancel.
- Deleting a card updates the column count and refocuses a valid target.
- Empty columns show a placeholder and never collapse.
- Headers and status strip stay fixed while cards scroll.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Adding, removing, or reordering columns at runtime.
- Drag-and-drop with a pointer.
- Labels, due dates, or assignees beyond a title.
- External assets or audio.

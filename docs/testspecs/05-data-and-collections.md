---
title: Controls Gallery — Data & Collections Page Spec
category: Controls Showcase specs
categoryindex: 8
---

# Controls Gallery — Data & Collections Page Spec

Inherits the shell, palette, pointer contract, keyboard contract, and
determinism rules from [`00-controls-gallery-overview.md`](00-controls-gallery-overview.md).

## Goal

Demonstrate the bounded-range data controls — a virtualized list view, a
hierarchical tree, and a sortable, selectable data grid — driven by pointer
selection, header-click sorting, expand/collapse, and wheel scrolling over large
datasets.

## Controls Demonstrated

| Control | Module | Required | Events | Demonstrates |
|---------|--------|----------|--------|--------------|
| list-view | Collections | `items` | `onSelected` | Bounded visible-range virtual list |
| tree-view | Collections | `items` | `onSelected` | Hierarchical expand/collapse |
| data-grid | DataGrid | `columns`, `rows` | `onSelected`, `onFocusChanged`, `onSortChanged` | Rows, columns, sort, focus, selection |

## User Experience

The page splits into three panes. A large `ListView` scrolls smoothly over
thousands of generated items rendering only its visible range. A `TreeView` shows a
nested structure the user expands and collapses by clicking disclosure triangles. A
`DataGrid` of ~1000 rows supports clicking a header to sort, clicking a row to
select, moving the focused cell, and wheel-scrolling — all without resizing.

## Layout

- A heading `Data & Collections` and description.
- A `SplitView` with the `ListView` and `TreeView` stacked on the left and the
  `DataGrid` filling the right.
- The data grid has typed columns (e.g. Id: int, Name: text, Status: badge,
  Value: number) with a header row and a fixed visible window over the row set.
- A status line under the grid shows total rows, the visible range, the selected
  row, the focused cell, and the active sort.

## Mouse & Pointer Interactions

- Wheel `Scroll` over the list, tree, or grid advances the visible range; only the
  visible window renders.
- `Click` a list row fires `onSelected` and highlights it.
- `Click` a tree disclosure triangle expands/collapses that node; `Click` a node
  label selects it.
- `Click` a grid header fires `onSortChanged`, cycling ascending → descending →
  none, and re-orders rows.
- `Click` a grid row fires `onSelected`; clicking a cell fires `onFocusChanged` and
  moves the focused-cell outline.
- `HoverEnter` on a row/cell raises the hover highlight; the status strip narrates
  the pointer kind, target, and coordinates.

## Keyboard

- `Arrow Up`/`Down` move the selection/focus within the focused collection;
  `PageUp`/`PageDown` move by a visible page.
- `Arrow Left`/`Right` collapse/expand the focused tree node.
- `Enter` activates the focused row; `Home`/`End` jump to first/last.

## Core Behaviors

- Each collection renders only its bounded visible range and recycles rows on
  scroll; selection and focus survive scrolling out of and back into view.
- The tree maintains per-node expanded state; collapsing a node hides its subtree
  but preserves its descendants' selection state.
- The grid sorts stably by the clicked column, preserves the selected row identity
  across a sort, and keeps the focused cell within bounds.
- Headers, the grid's column row, and the status line keep fixed dimensions while
  rows scroll.

## Data Model

- ListView: a large generated item sequence and the selected index/key.
- TreeView: a nested node structure with per-node expanded flags and selection.
- DataGrid: a column schema (name + type), a row set, the visible range, the
  selected row key, the focused cell (row, column), and the active sort (column +
  direction).

## Visual / Palette Requirements

- Selected rows use accent-soft fill with accent text; the focused grid cell shows
  the focus ring; hovered rows use surface-raised.
- The grid's `Status` column renders badges in the success/warning/danger roles.
- Sort direction shows an accent indicator in the active header.

## App State

Track: list items and selection; tree structure, expanded set, and selection; grid
columns, rows, visible range, selection, focused cell, and sort; hovered ids;
scroll offsets; seed.

## Determinism and Evidence

- Accept a seed governing generated list items and grid rows; the same seed yields
  identical data.
- Evidence mode scrolls the list by a page, expands then collapses a tree node,
  clicks a grid header to sort, selects a row, and moves the focused cell.
- Evidence outcome: total rows, final visible range, selected keys, focused cell,
  active sort, scroll deltas, and close reason.
- Screenshot evidence shows the list mid-scroll, an expanded tree node, and the
  grid sorted by a column with a selected row and focused cell.

## Acceptance Criteria

- Each collection renders only its visible range and scrolls by wheel and keyboard.
- List, tree, and grid selection fire their events and survive scrolling.
- Tree nodes expand/collapse by pointer and keyboard, preserving subtree state.
- Grid header clicks cycle and apply a stable sort, preserving selected-row
  identity.
- The focused cell moves by click and arrows and stays in bounds.
- Headers and the status line stay fixed while rows scroll.

## Out of Scope

- Inline cell editing in the data grid.
- Column reordering or resizing by drag.
- Grouping, pivoting, or aggregation rows.

---
title: Spreadsheet Editor Demo Spec
category: Productivity specs
categoryindex: 10
---

# Spreadsheet Editor Demo Spec

## Goal

Build a complete spreadsheet editor demo that exercises a scrollable cell grid, cell selection and editing, a formula bar, simple expression evaluation, column and row headers, and evidence-friendly rendering.

## User Experience

The app opens directly to a grid of empty cells with a focused active cell. The user navigates with the keyboard or pointer, types values or formulas, and sees computed results update immediately. The layout should be readable, fast to understand, and deterministic under a seed.

## Layout

- A column header row labeled `A`, `B`, `C`, … and a row header column labeled `1`, `2`, `3`, ….
- A scrollable cell grid, for example 26 columns by 50 rows, that scales to the window while preserving aligned cells.
- A formula bar above the grid showing the active cell reference and its raw content.
- A status strip showing the active cell, selection size, and last evaluation state.

## Controls

- `Arrow keys`: move the active cell.
- `Tab` / `Shift+Tab`: move right / left.
- `Enter`: commit the edit and move down; `Shift+Enter` moves up.
- `F2` or any printable key: begin editing the active cell.
- `Esc`: cancel the current edit.
- `Delete`: clear the active cell or selection.
- `Ctrl+C` / `Ctrl+V`: copy and paste a single cell value.

## Core Behaviors

- Each cell holds either a literal (text or number) or a formula beginning with `=`.
- Formulas support `+`, `-`, `*`, `/`, parentheses, numeric literals, and cell references such as `A1`.
- A `SUM(range)` function over a contiguous rectangular range, for example `=SUM(A1:A5)`, must be supported.
- Editing a cell re-evaluates every dependent cell.
- A reference cycle must be detected and shown as an error rather than looping.
- An invalid formula must display a stable error marker, not crash.

## Data Model

- A sparse map from cell address to raw content.
- A derived map from cell address to evaluated value or error.
- A dependency relationship sufficient to recompute dependents and detect cycles.

## Visual Requirements

- Show column headers, row headers, the grid, the formula bar, and the status strip.
- The active cell must be clearly outlined and distinguishable from the rest of the selection.
- Numeric values are right-aligned; text is left-aligned; errors are visually distinct.
- The header and formula bar must keep stable dimensions; content changes must not resize the grid.

## App State

Track at minimum:

- Raw cell contents, evaluated values, and per-cell error state.
- Active cell address, selection range, edit-in-progress flag, and edit buffer.
- Scroll offset, clipboard value, and random seed.

## Determinism and Evidence

- Accept an optional seed for any sample data population.
- Evidence mode should inject a deterministic script that enters a few literals and at least one `SUM` formula, then moves the active cell.
- Evidence outcome should include frame count, edit count, evaluated cell count, active cell, a sampled value, and close reason.
- Screenshot evidence should show the grid, headers, formula bar, and a computed formula result.

## Acceptance Criteria

- Arrow navigation moves the active cell within grid bounds.
- Typing a value commits it and updates the displayed result.
- A `=SUM(A1:A5)` formula computes the sum of its range.
- Editing a referenced cell updates its dependents.
- A reference cycle is reported as an error instead of looping.
- Invalid input shows a stable error marker.
- Headers and formula bar stay fixed while the grid scrolls.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Multi-sheet workbooks.
- Charts, formatting, or styling beyond alignment.
- Persistence to disk or external formats.
- External assets or audio.

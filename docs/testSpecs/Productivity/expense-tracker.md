---
title: Expense Tracker Demo Spec
category: Productivity specs
categoryindex: 10
---

# Expense Tracker Demo Spec

## Goal

Build a complete expense tracker demo that exercises an expense list, category assignment, per-category and total summaries, a simple bar breakdown, filtering by category, and evidence-friendly rendering.

## User Experience

The app opens to a list of expenses with a running total and a category breakdown. The user adds expenses, assigns categories, filters the view, and sees the totals and breakdown update. The layout should be readable, fast to understand, and deterministic under a seed.

## Layout

- A header with the total spend and an add-expense affordance.
- A main list of expense rows showing description, category, and amount.
- A breakdown panel with a per-category total and a proportional bar for each category.
- A status strip showing the visible expense count and the filtered total.

## Controls

- `Arrow Up` / `Arrow Down`: move the focused expense row.
- `N`: add a new expense and focus its description.
- `Enter`: commit the new expense or an edit.
- `F2`: edit the focused expense.
- `C`: cycle the category of the focused expense.
- `F`: cycle the active category filter, including an `All` option.
- `Delete`: remove the focused expense.
- `Esc`: cancel an edit.

## Core Behaviors

- Each expense has a description, a category from a fixed set, and a positive amount.
- The total is the sum of all expense amounts; the filtered total reflects the active filter.
- Each category's total is the sum of its expenses, and its bar length is proportional to the largest category total.
- The filter restricts visible rows without changing the underlying data.
- Amount must reject non-numeric, zero, or negative input and restore the prior value.
- All money values must display with a fixed two-decimal format.

## Data Model

- An ordered list of expenses, each with a stable id, description, category, and amount.
- Derived per-category totals and an overall total.
- A focus index, an active filter, and an edit buffer.

## Visual Requirements

- Show the header total, the expense list, the breakdown panel, and the status strip.
- The focused expense row must be clearly outlined.
- Category bars must be proportional and labeled with their totals.
- The header and breakdown panel must keep stable dimensions while the list scrolls.

## App State

Track at minimum:

- Expenses with ids, descriptions, categories, and amounts.
- Derived totals, focus index, active filter, edit buffer, and validation state.
- Scroll offset and random seed.

## Determinism and Evidence

- Accept an optional seed for any generated sample expenses.
- Evidence mode should inject a deterministic script that adds an expense, changes its category, and switches the filter.
- Evidence outcome should include frame count, add count, expense count, overall total, the largest category, and close reason.
- Screenshot evidence should show the list, the total, and the category breakdown bars.

## Acceptance Criteria

- The overall total equals the sum of all amounts.
- Each category total equals the sum of its expenses.
- Category bars are proportional to the largest category total.
- The filter restricts visible rows and updates the filtered total.
- Non-numeric, zero, or negative amounts are rejected and the prior value restored.
- Money values display with two decimals.
- Header and breakdown panel stay fixed while the list scrolls.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Custom categories or budgets.
- Date ranges or recurring expenses.
- Persistence or sync.
- External assets or audio.

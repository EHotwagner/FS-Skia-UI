---
title: Invoice Builder Demo Spec
category: Productivity specs
categoryindex: 10
---

# Invoice Builder Demo Spec

## Goal

Build a complete invoice builder demo that exercises an editable line-item table, quantity and unit-price entry, computed line and invoice totals, tax and discount handling, and evidence-friendly rendering.

## User Experience

The app opens to an invoice with a header, a few line items, and a totals summary. The user edits descriptions, quantities, and prices, adds or removes lines, and sees totals recompute immediately. The layout should be readable, fast to understand, and deterministic under a seed.

## Layout

- An invoice header with an invoice number, a bill-to field, and an issue label.
- A line-item table with columns for description, quantity, unit price, and line total.
- A totals panel showing subtotal, discount, tax, and grand total.
- A status strip showing the line count and the grand total.

## Controls

- `Arrow keys`: move the active cell across the line-item table.
- `Enter` / `F2`: edit the active cell.
- `Tab` / `Shift+Tab`: move to the next / previous editable field.
- `N`: add a new line item and focus its description.
- `Delete`: remove the focused line item.
- `Esc`: cancel the current edit.

## Core Behaviors

- Each line item has a description, a quantity, and a unit price.
- A line total equals quantity times unit price, recomputed on each edit.
- The subtotal is the sum of all line totals.
- A discount is applied as a percentage of the subtotal; tax is applied to the discounted subtotal.
- The grand total equals subtotal minus discount plus tax.
- Quantity and price must reject non-numeric or negative input and restore the prior value.
- All money values must display with a fixed two-decimal format.

## Data Model

- An ordered list of line items, each with a stable id, description, quantity, and unit price.
- A discount rate, a tax rate, and derived subtotal, discount, tax, and grand-total values.
- An active cell reference and an edit buffer.

## Visual Requirements

- Show the header, the line-item table, the totals panel, and the status strip.
- The active cell must be clearly outlined.
- Numeric columns must be right-aligned and money values formatted consistently.
- The header and totals panel must keep stable dimensions while the table scrolls.

## App State

Track at minimum:

- Line items with ids and fields, plus discount and tax rates.
- Derived totals, active cell reference, edit buffer, and validation state.
- Scroll offset and random seed.

## Determinism and Evidence

- Accept an optional seed for any generated sample lines.
- Evidence mode should inject a deterministic script that edits a quantity, edits a unit price, and adds one line item.
- Evidence outcome should include frame count, edit count, line count, subtotal, grand total, and close reason.
- Screenshot evidence should show the line-item table and the totals panel.

## Acceptance Criteria

- A line total equals quantity times unit price.
- The subtotal sums all line totals.
- Discount and tax apply in the defined order to produce the grand total.
- Non-numeric or negative quantity or price is rejected and the prior value restored.
- Adding and removing lines recomputes all totals.
- Money values display with two decimals.
- Header and totals panel stay fixed while the table scrolls.
- Interactive mode remains open until explicitly closed.

## Out of Scope

- Multiple invoices or templates.
- Currency conversion or per-line tax rates.
- PDF export or persistence.
- External assets or audio.

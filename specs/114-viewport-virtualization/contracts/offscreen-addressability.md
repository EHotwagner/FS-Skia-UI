# Contract: Offscreen Addressability + Accessibility Totals

Virtualization is only acceptable if it is invisible to correctness. Focus, selection, and
accessibility MUST remain correct across the visible/offscreen boundary.

## O1 — Offscreen focus targeting

Keyboard **focus** MUST be targetable to a logical row **outside** the realized window.
Focusing an offscreen logical row (by `FocusCell` over its `RowKey`) MUST:

1. record it as focused in the **logical** model (`FocusedCell`), and
2. **relocate** the realized window to it (so it and its neighbours within `V + overscan`
   materialize),

**without** materializing every intervening logical row. (FR-009 / SC-004.)

## O2 — Offscreen selection targeting

**Selection** MUST be targetable to an offscreen logical row by key. A row's selection
state is a property of the **logical item** (`SelectedRows : Set<string>` keyed by
`row.Key`), not of its materialized control, so `SelectRow` / `ToggleRow` on an offscreen
key updates the model without materializing the whole range. (FR-010 / SC-004.)

## O3 — Boundary-crossing navigation

Keyboard navigation that **crosses** the visible/offscreen boundary (e.g. moving focus past
the last realized row) MUST land on the correct next **logical** row (not the next
materialized one) and advance the realized window to include it. (FR-011 / SC-005.)

## O4 — Bound preserved during targeting (relocate, do not expand)

Targeting an offscreen row **relocates** the realized window to that row; it does **not**
expand the window to span the path from the old position. At all times
`VirtualItemsMaterialized <= V + 2*overscan` holds — the window moves, it does not grow.
(Resolves FR-009/FR-011 vs FR-003.)

## O5 — Accessibility total + position

A virtualized control's `AccessibilityMetadata` MUST report:

- the **total** logical item count (`Collection.TotalItems`), and
- the **current focused position** (`Collection.FocusedIndex`, the index within the total),

both computed from the **logical** model (`RowCount` + focused logical index), independent
of how many items are materialized — so assistive technology sees the true size and
position, not the realized slice. Non-collection controls report `Collection = None`
(at-rest a11y unchanged). (FR-012 / SC-005.)

## O6 — Materialized-row outcomes unchanged

For rows that **are** materialized, focus/keyboard routing semantics and every dispatch
outcome are **byte-identical** to the pre-feature state. Offscreen targeting is a newly
*reachable* capability (an offscreen key previously had no realized-window effect), not a
changed outcome for an already-materialized row. (FR-016 / SC-007.)

## Test mapping

| Contract | Test | Location |
|----------|------|----------|
| O1 | offscreen `FocusCell` records + relocates, no path materialization | `Feature114OffscreenTests` (Controls.Tests) |
| O2 | offscreen `SelectRow`/`ToggleRow` updates logical model, no path materialization | `Feature114OffscreenTests` |
| O3 | boundary-crossing focus move lands on correct next logical row | `Feature114OffscreenTests` |
| O4 | materialized count stays `<= V + 2*overscan` after relocation | `Feature114OffscreenTests` |
| O5 | a11y reports `TotalItems` + `FocusedIndex` from logical model | `Feature114AccessibilityTests` |
| O6 | materialized-row dispatch outcomes byte-identical | standing Scene-parity / dispatch tests + `Feature114OffscreenTests` |

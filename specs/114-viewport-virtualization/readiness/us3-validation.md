# US3 independent validation — focus / selection / a11y across the visible-offscreen boundary

**Story**: Focus, selection, and accessibility remain correct across the visible/offscreen boundary.

## Path

Confirm focus/selection are addressable on an offscreen logical row by key without materializing the
path; confirm the realized window **relocates** to a target (via `ScrollRowsTo`, the index-based
relocation primitive — research decision (d)) and stays bounded (relocate, do not expand); confirm
boundary-crossing navigation lands on the correct next logical row; confirm a11y reports the logical
total + focused position from the logical model.

## Design note (offscreen relocation mechanism)

Per research (d), the realized window relocates via the index-based `ScrollRowsTo targetIndex`; selection
(`SelectRow`/`ToggleRow`) and focus (`FocusCell`) record state on the **logical** model (a row key, not a
materialized control), so they are addressable on an offscreen row with no path materialization, and
their pre-feature outcomes are unchanged (FR-016). A11y `FocusedIndex` is derived from `FocusedCell.RowKey`
against the row set at the `DataGrid.create` site (where the rows are available). The FR-003 bound
(`materialized <= V + 2*overscan`) holds at all times — the window relocates, it never expands to span
the path.

## Evidence

- `tests/Controls.Tests/Feature114OffscreenTests.fs` — offscreen `SelectRow`/`ToggleRow`/`FocusCell`
  record logically with no path materialization; `ScrollRowsTo(offscreen)` relocates and stays bounded
  (relocate, not expand); boundary-crossing `ScrollRowsTo(lastRealized+1)` lands on the next logical row
  and advances the window; a visible-row dispatch is byte-identical (FR-016).
- `tests/Controls.Tests/Feature114AccessibilityTests.fs` — `AccessibilityMetadata.Collection` reports
  `TotalItems` = logical row count (independent of materialization) and `FocusedIndex` = the focused row's
  logical index; non-collection controls report `Collection = None`.

Result: PASS (SC-004 / SC-005).

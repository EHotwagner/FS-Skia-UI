# Role coverage — value + linear-selection + grid (feature 100, R5, T017/T019)

evidence-kind=role-coverage
status=observed

Representative coverage spans one role from **each** intent class (SC-006/FR-010), each validated by
`Accessibility.validate` (no `Error`-severity diagnostic) and exercised through the real
`routeFocusedKey` seam. Not slider-only.

## Value role — Slider (ValueStep)

- declared `NavRange { Step = 5.0; Min = 0.0; Max = 100.0 }`; ArrowRight at 50 → dispatched 55.0,
  `Nav = SteppedValue 55.0`; min/max clamp = no-op. Default-step slider byte-identical (see
  [declared-step.md](./declared-step.md)).
- `Accessibility.validate` on the slider control: no error diagnostic.

## Linear-selection role — RadioGroup / Tab (SelectionMove)

- radio-group items `[ "A"; "B"; "C" ]`, selected "B"; ArrowDown → `MovedSelection (2, Some "C")`,
  `Payload = Some "C"`; first/last boundary clamp = no-op; empty group / unresolved index = no dispatch
  (see [responds-vs-renders.md](./responds-vs-renders.md)).
- pure `Focus.route` for a Tab role maps Left/Right → Previous/Next.
- `Accessibility.validate` on the radio-group control: no error diagnostic.

## Grid role — DataGrid (GridMove)

- grid dims 3 rows × 2 columns, current cell (r1, c0); ArrowDown → `MovedCell (2, 0)`,
  `Payload = Some "r2:c0"`; ArrowRight → `MovedCell (1, 1)`; an edge cell + an outward arrow = no
  dispatch (edge clamp, FR-009).
- `Accessibility.validate` on the data-grid control: no error diagnostic.

## Non-navigable role — Button (FR-008 no-op)

- a focused button is an arrow no-op (no navigation dispatch) while Space/Enter activation (E4) is
  unaffected.

## Source

`tests/Controls.Tests/Feature100NavigationTests.fs` (pure route + `Accessibility.validate`) and
`tests/Elmish.Tests/Feature100NavigationTests.fs` (host resolver per role).

# Host resolver behavior contract — feature 100 (R5)

The `routeFocusedKey` `Navigate` arm (`src/Controls.Elmish/ControlsElmish.fs:455-478`)
changes from the slider-only float path into a **uniform per-intent resolver**. This is
internal host routing (a host seam, only promoted to `ControlsElmish.fsi` if it must be
public); the MVU contract (`view : 'model -> Control<'msg>`, the consumer's `'model`) is
unchanged. The resolver is a pure function over `(node, NavIntent)` returning the dispatched
`'msg list` — interpretation (the actual key event) stays at the host edge.

## Inputs

- The focused node's `Control`, its `Accessibility` metadata (`Role`, `Navigation`), and its
  own event bindings (`ControlInternals.eventBindingsOf`, filtered to `b.ControlId = nodeId`).
- The `NavIntent` from `Focus.route`.
- The live selection/value facts read from the control (current value; `Items` list +
  current selected item id; grid dims + current cell).

## Per-intent resolution

### `ValueStep delta`  (range roles — Slider, Progress, …)

1. Read current value (`controlFloatValue`), and `NavRange { Step; Min; Max }` from metadata.
2. `target = clamp(current + delta, Min, Max)`.
3. If `target = current` (already at the bound) → **no dispatch** (clamp no-op, FR-009).
4. Else dispatch the control's value binding (`EventKind = "changed"`) with
   `Payload = Some (target string)` **and** `Nav = Some (SteppedValue target)`.

Replaces the hardcoded `navStep = 0.1` / `Math.Clamp(.., 0.0, 1.0)` in `steppedValue`.

### `SelectionMove dir`  (linear selection — RadioGroup, Tab, Menu, List)

1. Read `Items` (count) and current index (index of current `value`/`selected` in `Items`).
2. Empty items or unresolved current index → **no dispatch** (FR-008/edge cases, research R-7).
3. Compute new index per `dir`: `Previous = i-1`, `Next = i+1`, `First = 0`, `Last = n-1`;
   **clamp** to `[0, n-1]` (FR-009). If clamped index = current → **no dispatch**.
4. Dispatch the control's **selection binding** — match `EventKind = "selected"`, else fall
   back to `"changed"` (research R-2) — with `Payload = Some itemId` **and**
   `Nav = Some (MovedSelection (newIndex, Some itemId))`.

### `GridMove (rowDelta, colDelta)`  (grid — Grid, DataGrid)

1. Read grid dimensions + current `(row, col)`.
2. `newRow = clamp(row + rowDelta, 0, rows-1)`, `newCol = clamp(col + colDelta, 0, cols-1)`.
3. If `(newRow, newCol) = (row, col)` → **no dispatch** (edge clamp).
4. Dispatch the selection binding (selected-then-changed) with `Nav = Some (MovedCell …)`
   and `Payload` set to the resulting cell/item id.

## Invariants

- **No per-kind branch beyond role classification** (FR-006): the only role-specific logic
  is `Focus.route`'s role → `NavIntent`. The resolver branches on the *intent*, not the kind.
- **Activation unaffected** (FR-008): the `Activate` arm is untouched; Space/Enter still
  dispatch click-equivalent bindings once.
- **Non-regressive numeric** (FR-007/SC-002): a default-step slider
  (`{0.1; 0.0; 1.0}`) produces a dispatched value byte-identical to the pre-R5 path.
- **No free-form key surface** (FR-006/SC-005): no consumer-supplied key handler; everything
  flows from declared metadata + the closed intent/payload set.

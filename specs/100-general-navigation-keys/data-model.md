# Phase 1 Data Model: General Navigation-Key Delivery (R5 / feature 100)

The "data" here is the **closed type surface** for navigation intent and payload, plus the
metadata fields that drive it. All types are F# discriminated unions / records living in
`FS.Skia.UI.Controls`. No persistent store; these are values flowing through the pure
router and the host resolver.

## Entity: `Direction` (new — `Focus`)

Closed selection-move direction.

| Case | Meaning |
|------|---------|
| `Previous` | move to the prior item |
| `Next` | move to the following item |
| `First` | move to the first item (Home) |
| `Last` | move to the last item (End) |

- **Closed**: exactly four cases; exhaustively matched.
- **Source**: produced by `Focus.route` from the role's declared `NavigationKeys` and the
  pressed key.

## Entity: `NavIntent` (new — `Focus`)

The closed, role-derived classification of a focused control's navigation key (FR-001,
Key Entities). One role maps to exactly one intent class (Assumptions: one role → one
intent class).

| Case | Payload | Roles | Source of the magnitude |
|------|---------|-------|-------------------------|
| `ValueStep of delta: float` | signed step delta | Slider, Progress (interactive), numeric stepper, Chart/Graph range | declared `NavRange.Step` × key sign |
| `SelectionMove of Direction` | direction only | RadioGroup, Tab, Menu, List | host reads count + current index |
| `GridMove of rowDelta: int * colDelta: int` | 2-D unit delta | Grid, DataGrid | host reads dims + current cell |

- **Closed & exhaustive** (SC-005): the property/exhaustiveness test pins these three
  cases.
- **`ValueStep` carries a delta, not a resolved value** (research R-1) — `Focus.route` is
  pure and does not see the live value; the host applies + clamps.
- **Validation rule**: `ValueStep` is only produced when the role carries a
  `NavRange`; `SelectionMove`/`GridMove` never carry a value (the live model supplies it).

## Entity: `KeyRouting` (modified — `Focus`)

Existing 4-case router result; the `Navigate` case now **carries** the intent.

| Case | Before | After |
|------|--------|-------|
| `Activate` | unchanged | unchanged |
| `Navigate` | nullary | **`Navigate of NavIntent`** |
| `Traverse of FocusMove` | unchanged | unchanged |
| `Fallthrough` | unchanged | unchanged |

- **State transition**: `Focus.route role keyboard key isTab shift` → one of the four cases.
  Activation membership and navigation membership are tested **before** the Tab test
  (unchanged precedence from E4); `Navigate` now additionally classifies the role into a
  `NavIntent`. A focused control whose role declares no matching navigation key →
  `Fallthrough` (FR-008 no-op).

## Entity: `NavRange` (new — `Accessibility` / `Types`)

Declared range metadata for value roles; the **sole** source of step/bounds (research R-4,
FR-002).

| Field | Type | Meaning |
|-------|------|---------|
| `Step` | `float` | value increment per arrow press |
| `Min` | `float` | lower bound (clamp floor; Home target) |
| `Max` | `float` | upper bound (clamp ceiling; End target) |

- **Validation rule**: `Min ≤ Max`; `Step > 0`. A **default-step slider** declares
  `{ Step = 0.1; Min = 0.0; Max = 1.0 }`, reproducing the pre-R5 constant byte-identically
  (FR-007 non-regressive).

## Entity: `AccessibilityMetadata` (modified — `Types`)

Gains one optional field; existing fields unchanged.

| Field | Type | Status |
|-------|------|--------|
| `Role` | `AccessibilityRole` | unchanged |
| `NameSource` | `string` | unchanged |
| `State` | `string list` | unchanged |
| `FocusOrder` | `int option` | unchanged |
| `Keyboard` | `KeyboardOperation` | unchanged |
| `Contrast` | `ContrastEvidence option` | unchanged |
| **`Navigation`** | **`NavRange option`** | **new** — `Some` for range roles, `None` otherwise |

- **Relationship**: `Focus.route` and the host resolver both read `Navigation`; `validate`
  may reason about it (a range role with `Navigation = None` is still valid — it simply
  cannot value-step, matching FR-008).

## Entity: `NavPayload` (new — `Types` / `ControlRuntime`)

The closed set of navigation-outcome payload shapes (FR-005, SC-005). Mirrors `NavIntent`.

| Case | Payload | Carried on |
|------|---------|-----------|
| `SteppedValue of value: float` | resolved, clamped value | value binding (`"changed"`) |
| `MovedSelection of index: int * item: string option` | new index + item id | selection binding (`"selected"` or `"changed"`) |
| `MovedCell of row: int * col: int` | new cell coordinate | grid selection binding |

- **Closed & exhaustive** (SC-005), one-to-one with `NavIntent`.

## Entity: `ControlEvent` (modified — `Types`)

Gains one closed optional field; `Payload: string option` retained for backward
compatibility (research R-3).

| Field | Type | Status |
|-------|------|--------|
| `Kind` | `string` | unchanged |
| `ControlId` | `ControlId option` | unchanged |
| `Origin` | `ControlEventOrigin` | unchanged |
| `Payload` | `string option` | unchanged — selection sets the moved item id here too |
| **`Nav`** | **`NavPayload option`** | **new** — the closed typed nav outcome |

- **Dual-set rule** (research R-2): a selection move sets `Payload = Some itemId` **and**
  `Nav = Some (MovedSelection ...)` so existing string consumers and the closed-set proof
  are both satisfied.
- **`Origin`**: navigation dispatches carry `Origin = Keyboard` (existing convention for
  the focused-key path).

## Selection-model facts (read-only, existing)

Not new types — the resolver reads what selection controls already hold:

| Fact | Source today |
|------|--------------|
| item count | length of `Items` (`Attr.items`, `src/Controls/Widgets/Input.fs:18-22`) |
| current index | index of current `value`/`selected` within `Items` |
| grid dims / current cell | `data-grid` `Columns`/`Rows` for dimensions + `FocusedCell` for the current `(row, col)` (`DataGridWidget.fs:7-8,35`) |

- **Validation rule** (research R-7): zero items or an unresolvable current index → the
  resolver dispatches **nothing** (no-op), no spurious event.

## Type-placement summary (`.fsi` impact)

| Type | `.fsi` file |
|------|-------------|
| `Direction`, `NavIntent`, modified `KeyRouting`, `route` signature | `src/Controls/Focus.fsi` |
| `NavRange`, modified `AccessibilityMetadata`, `NavPayload`, modified `ControlEvent` | `src/Controls/Types.fsi` |
| `Accessibility.metadata` widening (accept `NavRange option`), `keyboardFor` | `src/Controls/Accessibility.fsi` |
| possible resolver seam (only if promoted beyond module-internal) | `src/Controls.Elmish/ControlsElmish.fsi` |

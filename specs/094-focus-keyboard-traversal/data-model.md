# Phase 1 Data Model: Focus, Keyboard Traversal & Input Routing (E4)

Entities are the **new** types introduced by E4 plus the **existing** types it reads/consumes. E4
introduces no new durable state of its own — focus state lives in the existing
`ControlRuntime.FocusedControl`; E4 adds **pure value types** for the tab order and key routing.

## New entities (public, `src/Controls/Focus.fsi`)

### `FocusStop`
One focusable stop in the computed tab order.

| Field | Type | Notes |
|-------|------|-------|
| `Control` | `ControlId` | the authored/lowered id of the focusable control |
| `Role` | `AccessibilityRole` | from the control's `AccessibilityMetadata.Role` |
| `Keyboard` | `KeyboardOperation` | the control's `{ Focusable; ActivationKeys; NavigationKeys }` |
| `FocusOrder` | `int option` | the control's `AccessibilityMetadata.FocusOrder` (ordering key) |

- Derived purely from `AccessibilityMetadata`; carries no new accessibility primitive (FR-008).
- Only controls with `Keyboard.Focusable = true` appear (FR-001).

### `TabOrder`
The deterministic single tab order over a view's focusable controls.

| Field | Type | Notes |
|-------|------|-------|
| `Stops` | `FocusStop list` | in traversal order: `FocusOrder` ascending, `None` last, document-order tiebreak (FR-001) |

- Named `TabOrder` (not `FocusOrder`) to avoid confusion with the `AccessibilityMetadata.FocusOrder`
  *field*.
- A view with no focusable controls yields `{ Stops = [] }` (edge case: Tab is a no-op).

### `FocusMove`
A traversal command derived from an unconsumed traversal key.

| Case | Meaning |
|------|---------|
| `Next` | Tab — advance to the next stop (wraps to first after last) |
| `Previous` | Shift+Tab — retreat to the previous stop (wraps to last from first) |

### `KeyRouting`
The verdict of routing a delivered key against the focused control's `KeyboardOperation` (FR-003/
FR-007). Closed set → the host match is total.

| Case | Meaning | Host action |
|------|---------|-------------|
| `Activate` | key ∈ `ActivationKeys` | fire the control's authored **activation** binding once (pointer-equivalent) |
| `Navigate` | key ∈ `NavigationKeys` | fire the control's authored **value-change/selection** binding |
| `Traverse of FocusMove` | unconsumed Tab / Shift+Tab | `Focus.traverse` + emit `FocusControl next` |
| `Fallthrough` | no match | no-op for the control; fall through to `host.MapKey` |

- The control's own consumption (`Activate`/`Navigate`) is tested **before** `Traverse`, so a
  control that lists a traversal key in its own `ActivationKeys`/`NavigationKeys` wins it (FR-007).
- The **text** path (E1) is handled by the host *before* `Focus.route` is consulted (R3 step 1), so
  `KeyRouting` carries no text case — text delivery stays the unchanged E1 seam (SC-003).

## Existing entities consumed (not modified, or `.fs`-only behavioral fix)

### `ControlRuntimeModel.FocusedControl : ControlId option` (`ControlRuntime.fsi:42`)
The durable focus state. E4 **reads** it and produces `FocusControl` messages; it does **not**
duplicate or re-own it (FR-004).

### `ControlRuntimeMsg.FocusControl of ControlId option` (`ControlRuntime.fsi:54`)
The existing message traversal emits to move focus. Reused unchanged.

### `KeyboardOperation` (`Types.fsi:159`) — `{ Focusable; ActivationKeys; NavigationKeys }`
The sole source of focusability and key semantics (FR-008). **Unchanged** as a type. Its **default
values** for the representative controls are corrected in `Accessibility.fs` per Research R1 (Tab
out of `NavigationKeys`; intra-control arrows in).

### `AccessibilityMetadata` (`Types.fsi:172`) — `{ Role; FocusOrder; Keyboard; … }`
The per-control metadata `Focus.order` reads. Unchanged.

### `Accessibility.defaultFor` / `Accessibility.validate` (`Accessibility.fs`) — **`.fs`-only fix**
- `defaultFor`: stop seeding `NavigationKeys = ["Tab"; "Shift+Tab"]`; seed intra-control arrows per
  role instead (R1). Signature unchanged.
- `validate`: relax "focusable ⇒ non-empty `NavigationKeys`" so an activation-only control is valid;
  keep validating that focusable controls have *some* operable key set (activation or navigation).
  Signature unchanged.

### `RetainedRender<'msg>` / `RetainedId` (E2, `RetainedRender.fsi`)
Consumed at the **host seam** for stable focus identity (`retainedHitTest` / `resolveFocus`). E4
does not alter the reconciler identity scheme (FR-004). `RetainedId` does **not** appear in the
pure `Focus` surface (R4).

## State transitions (the traversal reducer, FR-002)

`Focus.traverse : TabOrder -> ControlId option -> FocusMove -> ControlId option`

| Current | Move | Next |
|---------|------|------|
| `None` | `Next` | first stop (or `None` if empty) |
| `None` | `Previous` | last stop (or `None` if empty) |
| `Some c` (c at index i) | `Next` | stop at `(i+1) mod n` (cyclic wrap) |
| `Some c` (c at index i) | `Previous` | stop at `(i-1+n) mod n` (cyclic wrap) |
| `Some c` (c not in order) | either | the next stop at `c`'s former position in the order (or `None` if empty) — stale-target recovery, edge case |

- Pure, total, deterministic: identical `(order, current, move)` → identical `next` (SC-006).
- Non-focusable controls are absent from `Stops`, so they are never a `Next` target (FR-001 / US1.3).

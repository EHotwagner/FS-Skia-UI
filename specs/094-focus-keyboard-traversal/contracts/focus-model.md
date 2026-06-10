# Contract: Pure Focus Model (`FS.Skia.UI.Controls`, `module Focus`)

The new public surface on `FS.Skia.UI.Controls`. Pure, total, deterministic — no I/O, no live
window, property-testable to ≥1000 generated combinations (SC-006). Sketched as `.fsi` and
exercised in FSI before any `.fs` body (Constitution Principle I).

## Types (`src/Controls/Focus.fsi`)

```fsharp
namespace FS.Skia.UI.Controls

/// One focusable stop in the computed tab order, derived purely from AccessibilityMetadata.
type FocusStop =
    { Control: ControlId
      Role: AccessibilityRole
      Keyboard: KeyboardOperation
      FocusOrder: int option }

/// The deterministic single tab order over a view's focusable controls (FR-001).
/// Stops are in traversal order: FocusOrder ascending, None last, document-order tiebreak.
type TabOrder =
    { Stops: FocusStop list }

/// A traversal command derived from an unconsumed traversal key (FR-002).
type FocusMove =
    | Next        // Tab
    | Previous    // Shift+Tab

/// How a delivered key routes against the focused control's KeyboardOperation (FR-003/FR-007).
/// Closed → the host's match is total. Text delivery is the host's E1 seam, consulted first,
/// so there is no text case here.
type KeyRouting =
    | Activate                 // key ∈ ActivationKeys → authored activation binding (once)
    | Navigate                 // key ∈ NavigationKeys → authored value-change/selection binding
    | Traverse of FocusMove    // unconsumed Tab/Shift+Tab → global traversal
    | Fallthrough              // no match → no-op for control; host.MapKey fallback

module Focus =

    /// Derive the deterministic tab order from a lowered Control tree (FR-001):
    /// keep controls whose Accessibility.Keyboard.Focusable = true, ordered by
    /// (FocusOrder ascending with None last, then document/pre-order index). Non-focusable
    /// controls never appear. Pure, total; never throws.
    val order: control: Control<'msg> -> TabOrder

    /// Pure traversal reduction (FR-002): (order, current focus, move) → next focus.
    /// None + Next → first; None + Previous → last; wraps cyclically at both ends; a current
    /// id absent from the order resolves to the next stop at its former position, or None if the
    /// order is empty (stale-target recovery — clarified).
    /// Total/deterministic: identical inputs → identical output.
    val traverse: order: TabOrder -> current: ControlId option -> move: FocusMove -> ControlId option

    /// Route a normalized key against the focused control's KeyboardOperation (FR-003/FR-007).
    /// `key` is the normalized key name matched against Activation/NavigationKeys; `isTab`/`shift`
    /// describe a traversal candidate. The control's own consumption wins: membership in
    /// ActivationKeys → Activate, in NavigationKeys → Navigate, are tested BEFORE the Tab test, so
    /// a control that lists a traversal key consumes it. Only an unconsumed Tab/Shift+Tab →
    /// Traverse (Next/Previous by `shift`). Otherwise Fallthrough. Pure, total; never throws.
    val route: keyboard: KeyboardOperation -> key: string -> isTab: bool -> shift: bool -> KeyRouting
```

## Laws (asserted by tests)

- **Order — focusable-only & sorted** (FR-001 / SC-001): `Focus.order c |> .Stops` contains exactly
  the `Focusable = true` controls of `c`, sorted by `(FocusOrder ?? +∞, docIndex)`. Non-focusable
  controls never appear (US1.3).
- **Order — determinism**: `Focus.order c = Focus.order c` for all `c` (no clock/randomness).
- **Traverse — cyclic & total** (FR-002 / SC-006): for a non-empty order of `n` stops, `n`
  successive `Next` from any start returns to the start; `Next` then `Previous` is identity;
  `None + Next = first`, `None + Previous = last`; empty order → `None` for any move.
- **Traverse — skips non-focusable**: only stops in `order` are ever returned (US1.3).
- **Route — consumption wins** (FR-007): if `key ∈ ActivationKeys` → `Activate`; else if
  `key ∈ NavigationKeys` → `Navigate`; else if `isTab` → `Traverse (if shift then Previous else Next)`;
  else `Fallthrough`. A key in *both* a control's keys and the traversal set never routes to
  `Traverse` (the multi-line-text-area Tab edge case).
- **Route — no-op never throws** (SC-006): every `(keyboard, key, isTab, shift)` yields one of the
  four cases; an unmatched key is `Fallthrough`, never an exception.
- **Property** (SC-006): `order`/`traverse`/`route` are pure & deterministic over ≥1000 FsCheck
  combinations.

## FSI exercise (Principle I)

```fsharp
#r "FS.Skia.UI.Controls.dll"
open FS.Skia.UI.Controls

let view = (* a small Control<'msg> with mixed FocusOrder + a non-focusable static text *)
let order = Focus.order view
// inspect order.Stops — focusable-only, FocusOrder-then-doc-order
let f0 = Focus.traverse order None Next            // first stop
let f1 = Focus.traverse order f0 Next              // next; wraps at end
let r  = Focus.route buttonKeyboard "Enter" false false   // Activate
let t  = Focus.route buttonKeyboard "Tab"   true  false   // Traverse Next (Tab not in button keys)
```

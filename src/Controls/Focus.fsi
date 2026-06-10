namespace FS.Skia.UI.Controls

/// Feature 094 (E4) — the pure focus model: a deterministic single tab order derived purely from
/// `AccessibilityMetadata`, keyboard traversal over that order, and classification of a delivered
/// key against the focused control's `KeyboardOperation`. Pure, total, deterministic — no I/O, no
/// live window, property-testable to >=1000 generated combinations (SC-006). The `ControlId`<->`RetainedId`
/// binding lives at the host seam (`Controls.Elmish.routeFocusedKey`), so `RetainedId` is absent here (R4).

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
    | Next
    | Previous

/// How a delivered key routes against the focused control's KeyboardOperation (FR-003/FR-007).
/// Closed -> the host's match is total. Text delivery is the host's E1 seam, consulted first,
/// so there is no text case here.
type KeyRouting =
    | Activate
    | Navigate
    | Traverse of FocusMove
    | Fallthrough

/// Public contract module exposed by this FS.Skia.UI package.
module Focus =

    /// Derive the deterministic tab order from a lowered Control tree (FR-001): a pre-order walk
    /// that keeps controls whose `Accessibility.Keyboard.Focusable = true`, ordered by
    /// (FocusOrder ascending with None last, then document/pre-order index). A focusable control
    /// is a SINGLE stop — its subtree is not descended for further stops (a composite is one tab
    /// stop, clarified). Non-focusable controls never appear. Pure, total; never throws.
    val order: control: Control<'msg> -> TabOrder

    /// Pure traversal reduction (FR-002): (order, current focus, move) -> next focus.
    /// None + Next -> first; None + Previous -> last; wraps cyclically at both ends; a current
    /// id absent from the order resolves to the first stop (Next) / last stop (Previous), or None
    /// if the order is empty (stale-target recovery — clarified).
    /// Total/deterministic: identical inputs -> identical output.
    val traverse: order: TabOrder -> current: ControlId option -> move: FocusMove -> ControlId option

    /// Route a normalized key against the focused control's KeyboardOperation (FR-003/FR-007).
    /// `key` is the normalized key name matched against Activation/NavigationKeys; `isTab`/`shift`
    /// describe a traversal candidate. The control's own consumption wins: membership in
    /// ActivationKeys -> Activate, in NavigationKeys -> Navigate, are tested BEFORE the Tab test, so
    /// a control that lists a traversal key consumes it. Only an unconsumed Tab/Shift+Tab ->
    /// Traverse (Next/Previous by `shift`). Otherwise Fallthrough. Pure, total; never throws.
    val route: keyboard: KeyboardOperation -> key: string -> isTab: bool -> shift: bool -> KeyRouting

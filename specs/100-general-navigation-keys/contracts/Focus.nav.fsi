// CONTRACT SKETCH — feature 100 (R5). Not compiled here; this is the intended public
// surface to draft in FSI and exercise before the .fs body exists (Constitution I/II).
// Final form lands in src/Controls/Focus.fsi + src/Controls/Types.fsi.

namespace FS.Skia.UI.Controls

/// Closed selection-move direction (Home/End fold to First/Last). Exhaustive.
type Direction =
    | Previous
    | Next
    | First
    | Last

/// The closed, role-derived classification of a focused control's navigation key (FR-001).
/// One role -> exactly one case. `ValueStep` carries a SIGNED STEP DELTA (declared step x
/// key sign), NOT a resolved value — the host applies it to the live value and clamps.
type NavIntent =
    | ValueStep of delta: float
    | SelectionMove of Direction
    | GridMove of rowDelta: int * colDelta: int

module Focus =

    /// Existing router result; the Navigate case now CARRIES the intent.
    type KeyRouting =
        | Activate
        | Navigate of NavIntent
        | Traverse of FocusMove
        | Fallthrough

    /// Route a normalized key against the focused control's role + keyboard metadata.
    /// Precedence is unchanged from E4 (activation, then navigation, then Tab); the new
    /// behavior is that a Navigate result is classified into a closed `NavIntent` from the
    /// control's `role` and (for range roles) its declared `NavRange`. A key absent from the
    /// role's NavigationKeys -> Fallthrough (FR-008 no-op). Pure, total; never throws.
    val route:
        role: AccessibilityRole ->
        keyboard: KeyboardOperation ->
        navRange: NavRange option ->
        key: string ->
        isTab: bool ->
        shift: bool ->
            KeyRouting

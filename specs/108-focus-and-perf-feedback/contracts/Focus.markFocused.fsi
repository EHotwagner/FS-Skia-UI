// CONTRACT SKETCH — feature 108 US1. Not compiled; shapes the real src/Controls/Focus.fsi edit.
namespace FS.Skia.UI.Controls

module Focus =
    // existing: order / traverse / route …

    /// Stamp `VisualState.Focused` on the single control whose identity
    /// (`Key ?? structural path`, the feature-098 unification) equals `focused`.
    /// Every other control is untouched; a consumer-set non-Normal state (Disabled)
    /// is preserved (Focused does not override it). `None` returns the tree
    /// byte-identical — no stamp, at-rest output unchanged (SC-012).
    ///
    /// Drives the focus ring from the SAME identity `order`/`traverse` enumerate, so
    /// every focusable control — keyed OR unkeyed — is reachable (FR-002) and
    /// structural/non-focusable elements are never stamped (FR-004). Exactly one
    /// control carries the ring at a time (FR-003). Consumers reflect their own focus
    /// model by calling `markFocused model.Focused (view …)` in `view` (FR-005).
    val markFocused: focused: ControlId option -> control: Control<'msg> -> Control<'msg>

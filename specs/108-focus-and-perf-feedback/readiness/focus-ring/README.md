# US1 focus-ring — independent validation path (T010/T012, SC-001/002/012)

The story is the public `Focus.markFocused : ControlId option -> Control<'msg> -> Control<'msg>` call a
consumer makes inside `view` (`markFocused model.Focused (view …)`). It is reachable and proven without
a live window:

1. **Structural-Scene proof** ([focus-ring-evidence.md](./focus-ring-evidence.md)): for each focusable
   kind, stamping the focused id makes exactly ONE control carry `VisualState.Focused`; `markFocused
   None tree` is structurally identical to `tree`; structural/non-focusable elements are never stamped;
   a consumer-set `Disabled` is preserved. Enforced by `tests/Controls.Tests/Feature108FocusTests.fs`.
2. **Multi-control traversal walkthrough**: author a `Stack` of keyed and UNKEYED focusable controls
   (button, slider, text box, radio group, switch). Compute the focused id as `Key ?? structural path`
   (root "0", child `path + "." + i`) — the SAME id `collectBoundsWith` / dispatch mint. Call
   `Focus.markFocused (Some id) root`; assert via `Control.renderTree` that only the targeted control's
   Scene carries the ring. An unkeyed same-kind sibling is reachable by its distinct path id (FR-002).
3. **Interactive responds-proof** ([../responds-proof/focus-on-key.md](../responds-proof/focus-on-key.md)).

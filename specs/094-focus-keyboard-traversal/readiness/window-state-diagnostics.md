# Window-State Diagnostics (094)

status=not-applicable

## Diagnostic classes (separation preserved)

- diagnostic-class=environment-session — no desktop session window is opened by this feature.
- diagnostic-class=window-visibility — not-applicable; no window is created, so there is no
  visible/invisible window state to observe.
- diagnostic-class=app-lifecycle — no persistent app lifecycle is started; the deterministic focus
  reducer / route-probe / responds-proof suites run to completion and exit.
- diagnostic-class=product-defect — none observed; no window code path is exercised by this feature.

## Observable-vs-unsupported native facts

native-handle=not-applicable
visible=not-applicable
focusable=not-applicable
renderable-surface=not-applicable
input-devices=not-applicable

No taskbar-entry or process-only success is claimed, and no unsupported-host-only visibility claim
is made — there is simply no window in scope. The focus model is exercised through pure reducers and
the offscreen `routeFocusedKey` adapter path; the responds-proof uses the production
`Control.renderTree` (off-window) path, not a windowed host.

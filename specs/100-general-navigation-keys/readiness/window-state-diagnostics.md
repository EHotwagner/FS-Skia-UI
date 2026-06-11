# Window-State Diagnostics (100, R5)

status=not-applicable

## Diagnostic classes (separation preserved)

- diagnostic-class=environment-session — no desktop session window is opened by this feature.
- diagnostic-class=window-visibility — not-applicable; no window is created, so there is no
  visible/invisible window state to observe.
- diagnostic-class=app-lifecycle — no persistent app lifecycle is started; the deterministic
  selection-move / declared-step / grid-move / boundary-clamp / closed-model suites run to completion
  and exit.
- diagnostic-class=product-defect — none observed; no window code path is exercised by this feature.

## Observable-vs-unsupported native facts

native-handle=not-applicable
visible=not-applicable
focusable=not-applicable
renderable-surface=not-applicable
input-devices=not-applicable

No taskbar-entry or process-only success is claimed, and no unsupported-host-only visibility claim is
made — there is simply no window in scope. Navigation is exercised through the pure `Focus.route` ->
`NavIntent` classifier and the host per-intent resolver via the real `routeFocusedKey` seam
(off-window); the responds-vs-renders proof uses the production retained render path, not a windowed
host.

# Window-State Diagnostics (078)

status=not-applicable

## Diagnostic classes (separation preserved)

- diagnostic-class=environment-session — no desktop session window is opened by
  this docs feature.
- diagnostic-class=window-visibility — not-applicable; no window is created, so
  there is no visible/invisible window state to observe.
- diagnostic-class=app-lifecycle — no persistent app lifecycle; the render-only
  preview path runs to completion and exits.
- diagnostic-class=product-defect — none observed; no window code path is
  exercised.

## Observable-vs-unsupported native facts

native-handle=not-applicable
visible=not-applicable
focusable=not-applicable
renderable-surface=not-applicable
input-devices=not-applicable

No taskbar-entry or process-only success is claimed, and no unsupported-host-only
visibility claim is made — there is simply no window in scope for this feature.

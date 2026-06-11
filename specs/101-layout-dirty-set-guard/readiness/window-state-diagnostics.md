# Window state diagnostics — applicability (feature 101, R7)

R7 opens no window; every window-state diagnostic class is recorded not-applicable with honest values.

diagnostic-class=environment-session status=not-applicable note=no host session is started by R7 (in-process Expecto only)
diagnostic-class=window-visibility status=not-applicable note=no window is created; rendering output byte-identical to pre-R7
diagnostic-class=app-lifecycle status=not-applicable note=no default-executable / persistent-launch entry point is added
diagnostic-class=product-defect status=not-applicable note=no product defect class applies to a build/test-time classifier guard

native-handle=not-applicable
visible=not-applicable
focusable=not-applicable
renderable-surface=not-applicable
input-devices=not-applicable

The user-reachable surface for R7 is the failing build/test gate, not a window — see
[window-visibility.md](./window-visibility.md).

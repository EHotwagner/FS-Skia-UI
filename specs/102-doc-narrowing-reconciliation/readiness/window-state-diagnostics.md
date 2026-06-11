# Window state diagnostics — applicability (feature 102, R8)

R8 opens no window; every window-state diagnostic class is recorded not-applicable with honest values.

diagnostic-class=environment-session status=not-applicable note=no host session is started by R8 (prose + source comments only)
diagnostic-class=window-visibility status=not-applicable note=no window is created; rendering output byte-identical to pre-R8
diagnostic-class=app-lifecycle status=not-applicable note=no default-executable / persistent-launch entry point is added
diagnostic-class=product-defect status=not-applicable note=no product defect class applies to a documentation/comment honesty pass

native-handle=not-applicable
visible=not-applicable
focusable=not-applicable
renderable-surface=not-applicable
input-devices=not-applicable

The user-reachable surface for R8 is the reconciled report prose and source comments, not a window —
see [window-visibility.md](./window-visibility.md).

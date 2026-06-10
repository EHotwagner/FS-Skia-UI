# Window-state diagnostics (feature 095)

Feature 095 opens no window, so the native window facts are recorded as not-applicable rather than
claimed observed. The four diagnostic classes are listed so the contract's required class set is
present; none is raised as a failure here because there is no window lifecycle to diagnose.

status=deferred

diagnostic-class=environment-session
diagnostic-class=window-visibility
diagnostic-class=app-lifecycle
diagnostic-class=product-defect

native-handle=not-applicable
visible=not-applicable
focusable=not-applicable
renderable-surface=not-applicable
input-devices=not-applicable
taskbar-entry=false

Note: render-only feature; parity is structural Scene / lowered-Control equality, not a desktop
window observation. environment-session/window-visibility/app-lifecycle are not-applicable (no host
session, window, or loop is started); product-defect is none (the slot lowering is pure, total, and
parity-proven). ([[fs-skia-evidence-mode]])

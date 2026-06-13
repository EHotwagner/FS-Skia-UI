# window-state-diagnostics — applicability (feature 114, T002)

Every diagnostic-class is not-applicable for feature 114 (no new window is created; the existing
interactive host launch contract is unchanged — the change is a virtualization contract + two additive
`FrameMetrics` fields + an additive overscan/a11y surface + offscreen addressability on the logical
model).

diagnostic-class=environment-session status=not-applicable (no new host session launched)
diagnostic-class=window-visibility status=not-applicable (no new window)
diagnostic-class=app-lifecycle status=not-applicable (no new app launch/exit)
diagnostic-class=product-defect status=none (no window-bound product surface exercised by this feature)

native-handle=not-applicable
visible=not-applicable
focusable=not-applicable
renderable-surface=not-applicable (deterministic overscan/offscreen/a11y/metrics via Controls.Tests + Perf.runScript; no new Vulkan surface)
input-devices=not-applicable (the metric/offscreen scenarios are constructed in-test, not a live device loop)

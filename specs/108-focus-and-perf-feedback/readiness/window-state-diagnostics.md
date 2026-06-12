# Window-state diagnostics — applicability (feature 108, T002/T040)

Every diagnostic-class is not-applicable for feature 108 (no new window is created at any point; the
existing interactive host launch contract is unchanged).

diagnostic-class=environment-session status=not-applicable (no new host session launched)
diagnostic-class=window-visibility status=not-applicable (no new window)
diagnostic-class=app-lifecycle status=not-applicable (no new app launch/exit)
diagnostic-class=product-defect status=none (no window-bound product surface exercised by this feature)

native-handle=not-applicable
visible=not-applicable
focusable=not-applicable
renderable-surface=not-applicable (deterministic Scene assembly + responds-proof; no new Vulkan surface)
input-devices=not-applicable (the pointer/key seams are exercised headlessly via Perf.runScript and the
responds-proof, not through a live device loop in this feature's evidence)

# Window-state diagnostics — applicability (feature 103, R6, T002/T003)

Every diagnostic-class is not-applicable for R6 (no window is created at any point).

diagnostic-class=environment-session status=not-applicable (no host session launched)
diagnostic-class=window-visibility status=not-applicable (no window)
diagnostic-class=app-lifecycle status=not-applicable (no app launch/exit)
diagnostic-class=product-defect status=none (no window-bound product surface exercised)

native-handle=not-applicable
visible=not-applicable
focusable=not-applicable
renderable-surface=not-applicable (GPU-free deterministic scene assembly; no Vulkan surface)
input-devices=not-applicable (no pointer/keyboard host loop in this feature)

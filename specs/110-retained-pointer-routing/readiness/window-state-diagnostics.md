# Window-state diagnostics — applicability (feature 110, T002)

Every diagnostic-class is not-applicable for feature 110 (no new window is created at any point; the
existing interactive host launch contract is unchanged — the routing-mechanism change is observability +
internal-seam only).

diagnostic-class=environment-session status=not-applicable (no new host session launched)
diagnostic-class=window-visibility status=not-applicable (no new window)
diagnostic-class=app-lifecycle status=not-applicable (no new app launch/exit)
diagnostic-class=product-defect status=none (no window-bound product surface exercised by this feature)

native-handle=not-applicable
visible=not-applicable
focusable=not-applicable
renderable-surface=not-applicable (deterministic FrameMetrics + parity assembly via Perf.runScript and the internal retained-route seams; no new Vulkan surface)
input-devices=not-applicable (the pointer seams are exercised headlessly via Perf.runScript and the internal route, not a live device loop)

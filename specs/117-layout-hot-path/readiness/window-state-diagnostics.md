# window-state-diagnostics — applicability (feature 117, T002)

Every diagnostic-class is not-applicable for feature 117 (no new window is created; the existing
interactive host launch contract is unchanged — the change is an internal text-measure cache + a
layout-invalidated metric + three additive `FrameMetrics` fields, with no edit to the `runInteractiveApp`
launch seam).

diagnostic-class=environment-session status=not-applicable (no new host session launched)
diagnostic-class=window-visibility status=not-applicable (no new window)
diagnostic-class=app-lifecycle status=not-applicable (no new app launch/exit)
diagnostic-class=product-defect status=not-applicable (no window is created, so no product-defect window state can arise; the deterministic seam/metrics evidence carries no window)

## Observable-vs-unsupported native facts

No window is created, so every native fact is not-applicable (none is silently assumed):

native-handle=not-applicable (no window handle is allocated)
visible=not-applicable (no window to present)
focusable=not-applicable (no window to focus)
renderable-surface=not-applicable (no Vulkan/SkiaSharp surface is opened; the feature renders to scene lists checked headlessly)
input-devices=not-applicable (no live pointer/keyboard device is attached; input routing is unchanged and exercised deterministically)

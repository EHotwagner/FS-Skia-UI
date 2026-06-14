# window-state-diagnostics — applicability (feature 121)

Every diagnostic-class is not-applicable for feature 121: it drives no new window in this evidence. The
change is an additive `ViewerOptions.FrameRateCap` (live-loop cadence), an allocation-free idle clock
tick, and published docs — with no edit to the `runInteractiveApp` / `runInteractiveViewer` launch seam.

diagnostic-class=environment-session status=not-applicable (no new host session launched)
diagnostic-class=window-visibility status=not-applicable (no new window)
diagnostic-class=app-lifecycle status=not-applicable (no new app launch/exit observed in this evidence)
diagnostic-class=product-defect status=not-applicable (no window is created, so no product-defect window state can arise; the deterministic pacing-decision / idle-tick evidence carries no window)

## Observable-vs-unsupported native facts

No window is created, so every native fact is not-applicable (none is silently assumed):

native-handle=not-applicable (no window handle is allocated)
visible=not-applicable (no window to present)
focusable=not-applicable (no window to focus)
renderable-surface=not-applicable (no OpenGL/SkiaSharp surface is opened; the loop change is checked via the pure `GlHost.shouldAdvanceFrame` decision, headlessly)
input-devices=not-applicable (no live pointer/keyboard device is attached; input routing is unchanged)
</content>

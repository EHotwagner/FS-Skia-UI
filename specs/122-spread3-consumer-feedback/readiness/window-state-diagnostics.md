# window-state-diagnostics — applicability (feature 122)

Every diagnostic-class is not-applicable for feature 122: it drives no new window in this evidence. The
change is a live present-path correctness fix (buffer-fill re-present), an additive
`runInteractiveAppWithWindowBehavior` overload + generated `Program.fs` threading, a `CustomControl` null
guard, and documentation — with no window opened in CI.

diagnostic-class=environment-session status=not-applicable (no new host session launched)
diagnostic-class=window-visibility status=not-applicable (no new window)
diagnostic-class=app-lifecycle status=not-applicable (no new app launch/exit observed in this evidence)
diagnostic-class=product-defect status=not-applicable (no window is created, so no product-defect window state can arise; the present-decision / buffer-fill evidence carries no window)

## Observable-vs-unsupported native facts

No window is created, so every native fact is not-applicable (none is silently assumed):

native-handle=not-applicable (no window handle is allocated)
visible=not-applicable (no window to present)
focusable=not-applicable (no window to focus)
renderable-surface=not-applicable (no OpenGL/SkiaSharp surface is opened; the present-path change is checked via the pure `GlHost.planPresent` decision, headlessly)
input-devices=not-applicable (no live pointer/keyboard device is attached; input routing is unchanged)

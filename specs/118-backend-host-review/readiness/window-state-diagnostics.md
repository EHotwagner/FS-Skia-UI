# window-state-diagnostics — feature 118 (US1)

The feature-118 live-host run opens a real Vulkan window; native facts below are observed from
that run (no native fact is silently assumed). The present-mode/readback diagnostic itself is a
backend signal classified under window-visibility/app-lifecycle, never a silent product defect.

diagnostic-class=environment-session status=observed (real desktop session on display :1, AMD/RADV Vulkan backend present)
diagnostic-class=window-visibility status=observed (a real native window opened and presented 40 frames)
diagnostic-class=app-lifecycle status=observed (init → 40 presented frames → evidence self-close, clean exit RESULT: ok)
diagnostic-class=product-defect status=none (no crash, no corrupt frame; the DirectToSwapchain wrap limitation degrades safely to readback with a Warning — a binding limitation, not a product defect)

## Observable native facts

native-handle=observed (the windowing system allocated a real window handle; the Vulkan surface was created against it)
visible=observed (window opened on display :1)
focusable=not-verified (the bounded evidence run does not drive input focus; input routing is unchanged by this feature and exercised elsewhere)
renderable-surface=observed (40 frames presented through the Vulkan swapchain — the surface is renderable)
input-devices=not-verified (no live pointer/keyboard input is asserted by this present-mode evidence; input routing is unchanged)

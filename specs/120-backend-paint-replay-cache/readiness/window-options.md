# window-options — feature 120 (US2)

Feature 120 adds backend paint caching + an idle-skip without changing window-option handling. The
live-host run uses default window options. Each option below carries status/observed; an unsupported
option would diagnose under diagnostic-class=window-options (never silently ignored).

option=resize status=observed observed=true (default resizable window; a framebuffer-size change forces a present even when the scene is unchanged — GlHost.shouldPresent sizeChanged=true — so the idle-skip never leaves a resized/blank surface, FR-006)
option=maximize status=unchanged observed=false (default; not exercised by this bounded evidence run)
option=startup-state status=unchanged observed=false (default Normal startup state)
option=startup-position status=unchanged observed=false (default position)
option=backend status=observed observed=true (OpenGL backend selected and present on the real GPU; DirectToSwapchain default present mode)

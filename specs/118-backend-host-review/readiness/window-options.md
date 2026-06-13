# window-options — feature 118 (US1)

Feature 118 does not change window-option handling; it adds a present-mode selector orthogonal
to window options. The live-host run uses default window options. Each option below carries
status/observed; an unsupported option would diagnose under diagnostic-class=window-options
(never silently ignored).

option=resize status=unchanged observed=false (default resizable window; present mode is orthogonal — both modes recreate per-image direct resources on swapchain recreation per FR-006 design)
option=maximize status=unchanged observed=false (default; not exercised by this bounded evidence run)
option=startup-state status=unchanged observed=false (default Normal startup state)
option=startup-position status=unchanged observed=false (default position)
option=backend status=observed observed=true (Vulkan backend selected and present on the real GPU; present mode = OffscreenReadback effective, DirectToSwapchain degrades to it per audit/present-path-audit.md)

# window-options — feature 119 (US1)

Feature 119 swaps the present backend (Vulkan → OpenGL) without changing window-option handling.
The live-host run uses default window options. Each option below carries status/observed; an
unsupported option would diagnose under diagnostic-class=window-options (never silently ignored).

option=resize status=observed observed=true (default resizable window; the GL framebuffer surface is recreated leak-free on resize per FR-006)
option=maximize status=unchanged observed=false (default; not exercised by this bounded evidence run)
option=startup-state status=unchanged observed=false (default Normal startup state)
option=startup-position status=unchanged observed=false (default position)
option=backend status=observed observed=true (OpenGL backend selected and present on the real GPU; ViewerBackendPreference.OpenGL is Honored, the default backend selects OpenGL, and Vulkan is reported unsupported)

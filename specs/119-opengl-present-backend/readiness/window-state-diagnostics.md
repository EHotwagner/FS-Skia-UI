# window-state-diagnostics — feature 119 (US1)

The feature-119 live-host run opens a real **OpenGL** window; native facts below are observed
from that run (no native fact is silently assumed). The present-mode/readback diagnostic itself is
a backend signal classified under window-visibility/app-lifecycle, never a silent product defect.

diagnostic-class=environment-session status=observed (real desktop session on display :1, AMD Radeon Mesa OpenGL backend present, /dev/dri/renderD128)
diagnostic-class=window-visibility status=observed (a real native window opened and presented 60 frames)
diagnostic-class=app-lifecycle status=observed (init → 60 presented frames → evidence self-close, clean exit result=Ok)
diagnostic-class=product-defect status=none (no crash, no corrupt frame; the GL direct present renders straight onto FBO 0 and presents with SwapBuffers, readback=false)

## Observable native facts

native-handle=observed (the windowing system allocated a real window handle; the GL context was created against it)
visible=observed (window opened on display :1)
focusable=observed (the host attaches keyboard/mouse input mapping; focus-loss drives the deterministic pointer-cancel path)
renderable-surface=observed (60 frames presented through the OpenGL default framebuffer — the surface is renderable)
input-devices=observed (Silk.NET input keyboards/mice attached during the run)

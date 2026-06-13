# window-state-diagnostics — feature 120 (US2)

The feature-120 live-host run opens a real **OpenGL** window; native facts below are observed from
that run (no native fact is silently assumed). The present-mode/readback diagnostic and the
idle-skip are backend signals classified under window-visibility/app-lifecycle, never a silent
product defect.

diagnostic-class=environment-session status=observed (real desktop session on display :1, AMD Radeon Mesa OpenGL backend present)
diagnostic-class=window-visibility status=observed (a real native window opened and presented frames; idle frames are skipped with the front buffer intact)
diagnostic-class=app-lifecycle status=observed (init → changed frames → idle-skipped frames → evidence self-close, clean exit result=Ok)
diagnostic-class=product-defect status=none (no crash, no corrupt frame; DirectToSwapchain renders straight onto FBO 0 and presents with SwapBuffers, readback=false; an idle frame performs no clear/walk/swap)

## Observable native facts

native-handle=observed (the windowing system allocated a real window handle; the GL context was created against it)
visible=observed (window opened on display :1)
focusable=observed (the host attaches keyboard/mouse input mapping)
renderable-surface=observed (frames presented through the OpenGL default framebuffer — the surface is renderable; the idle-skip re-presents the valid front buffer)
input-devices=observed (Silk.NET input keyboards/mice attached during the run)

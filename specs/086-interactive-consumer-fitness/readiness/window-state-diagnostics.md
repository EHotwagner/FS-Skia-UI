# Window-State Diagnostics (086)

status=ok

## Diagnostic classes (separation preserved)

- diagnostic-class=environment-session — `DISPLAY=:1` (X11-normalized) granted a real desktop
  session; the live window is launched from a compiled exe (Silk.NET Vulkan windowing does not
  initialize under `dotnet fsi`).
- diagnostic-class=window-visibility — a real visible window was observed
  (`window-visible=observed:true`, `first-frame-presented=true`) for the controls-family
  `runInteractiveApp` launch (see `interactive-visible-window.md`).
- diagnostic-class=app-lifecycle — the `runInteractiveApp` interpreter opened the window,
  presented frames through the Vulkan/Skia swapchain, then self-closed on the host's
  `Tick -> CloseWindow` (`close-reason=AppRequestedClose`).
- diagnostic-class=product-defect — none observed; the durable launch succeeded and the
  routing/MVU path is independently green (`tests/SkiaViewer.Tests`, generated BehaviorTests).

## Observable-vs-unsupported native facts

native-handle=observed:true
visible=observed:true
focusable=observed:true
renderable-surface=observed:true
input-devices=observed:true

No taskbar-entry or process-only success is claimed: a real visible desktop window
(`renderer-mode=skia`) presented its first frame and self-closed for evidence.

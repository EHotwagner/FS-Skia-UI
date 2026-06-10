# Window-State Diagnostics (090)

status=observed

## Diagnostic classes (separation preserved)

- diagnostic-class=environment-session — a desktop session IS available
  (`DISPLAY=:1`); the dual Wayland/X11 `libdecor-gtk` hazard is avoided by forcing
  the X11 path (`live-window-launch.md`).
- diagnostic-class=window-visibility — observed: the live Vulkan/Skia window opened
  and presented its first frame (`window-visible=Observed true`,
  `first-frame-presented=true`).
- diagnostic-class=app-lifecycle — the host ran the production
  `runInteractiveApp` loop and self-closed via `CloseWindow` (`AppRequestedClose`).
- diagnostic-class=product-defect — none observed in the render/dispatch path.

## Observable-vs-unsupported native facts

native-handle=observed (a real Vulkan/Skia window was created)
visible=observed:true
focusable=observed:true
renderable-surface=observed (production Control.renderTree presented live)
input-devices=present (DISPLAY=:1; OS pointer-injection tooling absent, so a live
  on-screen click is proven on the identical code path by the responds-proof)

No taskbar-entry or process-only success is claimed; the window genuinely opened
and presented.

# Interactive Visible Window Evidence (086) — controls-family governed default

status=ok
mode=interactive-window
window-visible=observed:true
accessible-window=true
first-frame-presented=true
self-closed-for-evidence=true

## Real visible-window launch (SC-002, captured on a display-capable host)

Captured on `DISPLAY=:1` (X11-normalized: `WAYLAND_DISPLAY` unset, `GDK_BACKEND=x11`,
`SDL_VIDEODRIVER=x11`) by launching the controls-family governed default path
(`ControlsElmish.runInteractiveApp`) from a **tiny compiled self-closing host**
(`readiness/harness/InteractiveHostEvidence`, `Tick -> CloseWindow` after the first frames
present). The harness renders the REAL example controls (TextBlock "Product controls", a
TextBox, a keyed "Save" Button, a LineChart, and a GraphView) through `Control.renderTree` —
the same production path the generated `app` product's default launch uses. A compiled exe is
required — the live Silk.NET Vulkan window surface does not initialize under `dotnet fsi`
(FSI limitation, not an environment limit). Log: `readiness/logs/interactive-launch.txt`.

The launch opened a real visible desktop window that presented its first frame through the
Vulkan/Skia swapchain (`renderer-mode=skia`, `first-frame-presented=true`,
`window-visible=Observed true`) and then self-closed on the host's `Tick -> CloseWindow`
request (`close-reason=AppRequestedClose`). This is the SC-002 production-launch evidence
(real styled controls in a live window), complementing the headless render-target PNG
(`real-controls-render.png`). No taskbar-only / process-only success is claimed.

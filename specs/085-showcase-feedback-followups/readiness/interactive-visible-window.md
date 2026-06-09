# Interactive Visible Window Evidence (085)

status=ok
mode=interactive-window
window-visible=observed:true
accessible-window=true
first-frame-presented=true
self-closed-for-evidence=true

## Real visible-window launch (SC-002, captured on a display-capable host)

Captured on `DISPLAY=:1` (X11-normalized: `WAYLAND_DISPLAY` unset, `GDK_BACKEND=x11`,
`SDL_VIDEODRIVER=x11`) by launching the new durable interactive path
(`ControlsElmish.runInteractiveApp`) from a **tiny compiled self-closing host**
(`readiness/harness/InteractiveHostEvidence`, `Tick -> CloseWindow` after the first frames
present). A compiled exe is required — the live Silk.NET Vulkan window surface does not
initialize under `dotnet fsi` (FSI limitation, not an environment limit). Log:
`readiness/logs/interactive-launch.txt`.

The launch opened a real visible desktop window that presented its first frame through the
Vulkan/Skia swapchain and then self-closed for evidence:

| key | value |
|-----|-------|
| status | ok |
| mode | interactive-window |
| window-opened | true |
| window-visible | observed:true |
| first-frame-presented | true |
| renderer-mode | skia |
| close-reason | AppRequestedClose |
| user-close-observed | false |
| app-close-observed | true |
| exit-path | true |

No taskbar-only or process-only substitution is claimed — this was a real visible window
(`window-visible=observed:true`, `renderer-mode=skia`) that presented its first frame and then
self-closed for evidence (the host's `Tick -> CloseWindow` ⇒ `close-reason=AppRequestedClose`,
`user-close-observed=false`). `self-closed-for-evidence=true` reflects that the close was the
app's own evidence-driven request, not a user action. The pointer-routing surface is
independently proven by `tests/SkiaViewer.Tests` (`Feature 085 interactive pointer host`) and
the synthetic-through-adapter dispatch evidence in `evidence/pointer-dispatch.md`.

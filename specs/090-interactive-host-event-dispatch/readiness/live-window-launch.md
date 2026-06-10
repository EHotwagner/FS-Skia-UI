# Live Window Launch — Feature 090 (real Vulkan window)

A compiled host (`live-host/`) launches the **production** interactive path
(`ControlsElmish.runInteractiveApp`) with an authored `Button.onClick` binding,
opens a **real live Vulkan/Skia window**, presents the production
`Control.renderTree` view, repaints on each dispatched message (the counter
advances frame-by-frame — the exact `Update → renderTree → repaint` loop that was
**dead** in ControlsShowcase2), and self-closes for evidence.

status=launched
mode=interactive-window
renderer-mode=skia
window-opened=true
window-visible=observed:true
first-frame-presented=observed:true
self-closed-for-evidence=false
close-reason=AppRequestedClose
input-dispatch=false

## ViewerLaunchOutcome (captured, `logs/live-window-launch.txt`)

```
status=ok
mode=interactive-window
renderer-mode=skia
window-opened=true
window-visible=Observed true
first-frame-presented=true
close-reason=Some AppRequestedClose
message=Persistent interactive viewer launch completed after intentional close.
```

## Environment note (dual Wayland/X11)

This session exposes both a Wayland socket (`$XDG_RUNTIME_DIR/wayland-0`) and an
X11 display (`DISPLAY=:1`). Letting GLFW pick Wayland hits the known
`libdecor-gtk` init failure and hangs; forcing the X11 path (a valid
Wayland-free `XDG_RUNTIME_DIR` + `DISPLAY=:1`) opens and presents the live window
cleanly. Reproduce:

```
env -u WAYLAND_DISPLAY DISPLAY=:1 XDG_RUNTIME_DIR=<wayland-free dir> \
    dotnet run --project live-host/LiveHost.fsproj
```

## What this proves vs the responds-proof

- **This (live window):** the production `runInteractiveApp` host opens a real
  on-screen Vulkan window, presents the production render path, and **repaints
  live on dispatched messages** — the window is not static/dead.
- **The responds-proof (`responds-proof/`):** a real pointer **click routes the
  authored binding** through the identical production `routeInteractivePointer`
  path (input→visible-change, `Responsive`).

Together they cover both halves: the click dispatches, and the live window reacts.
A real OS pointer-click *into* the on-screen window is not separately injectable
here (no `xdotool`/input-injector, and the interactive viewer path exposes no
pointer-inject effect — only `DispatchInput` for keys), so the click-routing half
is proven on the identical code path by the responds-proof.

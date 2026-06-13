# interactive-visible-window — feature 119 (US1)

status=applicable
mode=live-windowed
window-visible=observed
accessible-window=observed
first-frame-presented=observed
self-closed-for-evidence=observed

Feature 119's live-host evidence launches a real persistent **OpenGL** window on display `:1`
through the production path `Host.Viewer.run → GlHost.run → renderFrameDirect`. The window opens
(window-visible), presents its first frame and 60 frames total (first-frame-presented), and
self-closes after the bounded frame count for evidence (self-closed-for-evidence) without a user
action. The run is driven the same way a user-reachable host runs (an Elmish viewer program over
`Viewer.run` with pointer/keyboard input mapping attached), in the default `DirectToSwapchain`
present mode. accessible-window=observed — the window is a real native window the windowing system
created and rendered into (renderable surface confirmed by the 60 presented frames). See
`supported-host-persistent-launch.txt` and `smoke/zero-readback-present.md`.

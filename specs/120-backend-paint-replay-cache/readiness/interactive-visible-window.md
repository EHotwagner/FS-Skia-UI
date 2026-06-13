# interactive-visible-window — feature 120 (US2)

status=applicable
mode=live-windowed
window-visible=observed
accessible-window=observed
first-frame-presented=observed
self-closed-for-evidence=observed

Feature 120's live-host evidence launches a real persistent **OpenGL** window on display `:1`
through the production path `Host.Viewer.run → GlHost.run → renderFrame → renderFrameDirect`. The
window opens (window-visible), presents its first frame (first-frame-presented), runs changed then
unchanged (idle-skipped) frames, and self-closes after the bounded tick count for evidence
(self-closed-for-evidence) without a user action. The run is driven the same way a user-reachable
host runs (an Elmish viewer program over `Viewer.run`), in the default `DirectToSwapchain` present
mode. accessible-window=observed — a real native window the windowing system created and rendered
into (result `Ok ()`, present diagnostic emitted). See `sample-smoke/live-host-evidence.txt`.

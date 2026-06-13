# interactive-visible-window — feature 118 (US1)

status=applicable
mode=live-windowed
window-visible=observed
accessible-window=observed
first-frame-presented=observed
self-closed-for-evidence=observed

Feature 118's live-host evidence (`readiness/live-host`) launches a real persistent Vulkan
window on display `:1` through the production path `Host.Viewer.run → VulkanHost.run →
renderFrame`. The window opens (window-visible), presents its first frame and 40 frames total
(first-frame-presented; `RESULT: ok frames=40`), and self-closes after the bounded frame count
for evidence (self-closed-for-evidence) without a user action. The run is driven the same way a
user-reachable host runs (the Elmish viewer program over `Viewer.run`), in both present modes
(`DirectToSwapchain` degrades to `OffscreenReadback`; `OffscreenReadback` direct). accessible-
window=observed — the window is a real native window the windowing system created and rendered
into (renderable surface confirmed by presented frames).

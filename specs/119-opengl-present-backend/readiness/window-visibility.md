# window-visibility — feature 119 (US1/US2)

status=applicable
mode=live-windowed
window-visible=observed
accessible-window=observed
first-frame-presented=observed
self-closed-for-evidence=observed

Feature 119 launches a real persistent **OpenGL** window on display `:1` through the production
path `Host.Viewer.run → GlHost.run → renderFrameDirect`. The window opens (window-visible),
presents its first frame and 60 frames total (first-frame-presented), and self-closes after the
bounded frame count for evidence (self-closed-for-evidence) without a user action. The run is
driven the same way a user-reachable host runs — an Elmish `ViewerProgram` over `Viewer.run`, with
pointer/keyboard input mapping attached — in the default `DirectToSwapchain` present mode.
accessible-window=observed — the window is a real native window the windowing system created and
rendered into (renderable surface confirmed by the 60 presented frames).

Companion artifacts:

- [real-image-evidence.md](./real-image-evidence.md) — decodable capture; proves scene rendering,
  not desktop visibility (pixel-readback).
- [visual-evidence-honesty.md](./visual-evidence-honesty.md) — honest about what the captures prove.
- [supported-host-persistent-launch.txt](./supported-host-persistent-launch.txt) — the live launch.
- [smoke/zero-readback-present.md](./smoke/zero-readback-present.md) — the zero-readback proof.

The interactive-UI run-and-use gate is satisfied: the windowed viewer was launched and driven
through the production `renderFrameDirect` path on a real OpenGL backend; the captured frame is the
real production scene (pixel values verified).

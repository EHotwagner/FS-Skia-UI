# visual-evidence-honesty — feature 119 (US2)

status=applicable

Feature 119 produces real visual evidence and is explicit about what it does and does not prove:

- The capture (`sample-smoke/gl-direct-present-frame.png`) is a real, decodable PNG of the
  production scene rendered through the on-demand offscreen readback routine (FR-004). Its sampled
  pixels match the rendered scene exactly, so it **proves scene rendering** through the shipped
  `SceneRenderer` path the live host uses — not a hand-built parallel scene.
- It is a **pixel-readback** capture, so per the evidence contract it does **not** prove desktop
  visibility on its own (`real-image-evidence.md`: proves-desktop-visibility=false). Desktop
  presentation is evidenced separately by the live windowed run presenting 60 frames on the real
  OpenGL backend on display `:1` (`supported-host-persistent-launch.txt`).
- The readback-free claim is honest and structurally grounded: in `DirectToSwapchain` the live
  present path performs **zero** `ReadPixels` (`smoke/zero-readback-present.md`); the screenshot is
  produced by the **separate** on-demand offscreen routine only when a capture is requested, so the
  capture does not contradict the zero-readback live path.
- The benign/blocking host-warning classification is honest: a GL-unavailable environment is
  classified `UnsupportedEnvironment` (benign), never a product defect
  (`smoke/unsupported-gl-diagnostic.md`). No visual claim overstates what the artifact shows.

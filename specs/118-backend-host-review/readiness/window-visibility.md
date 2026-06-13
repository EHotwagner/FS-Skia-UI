# window-visibility — feature 118 (US1)

status=applicable

Feature 118 launches a real persistent Vulkan window for its live present-path evidence, so the
window-visibility set is authored as applicable (observed from the live-host run on display :1):

- [interactive-visible-window.md](./interactive-visible-window.md) — window-visible / first-frame-presented / self-closed-for-evidence all observed
- [close-reason-separation.md](./close-reason-separation.md) — evidence self-close, kept distinct from user close
- [window-state-diagnostics.md](./window-state-diagnostics.md) — environment-session / window-visibility / app-lifecycle observed; product-defect none
- [window-options.md](./window-options.md) — backend observed (Vulkan); other options unchanged/default
- [real-image-evidence.md](./real-image-evidence.md) — decodable byte-identical captures; proves scene rendering, not desktop visibility (pixel-readback)
- [visual-evidence-honesty.md](./visual-evidence-honesty.md) — honest about what the captures prove and the benign fallback Warning

The interactive-UI run-and-use gate is satisfied: the windowed viewer was launched and driven
through the production `renderFrame` path on a real Vulkan backend in both present modes; the
`DirectToSwapchain` selection safely degrades to `OffscreenReadback` (FR-005) with one Warning.

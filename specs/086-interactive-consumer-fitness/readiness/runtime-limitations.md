# Runtime limitations — feature 086

The interactive consumer host targets a **.NET 10 desktop** runtime with a **Vulkan**
swapchain rendered through **SkiaSharp preview** bindings.

- Supported: Windows and Linux desktop with a Vulkan-capable display session.
- **Unsupported macOS/mobile/browser** targets — no host window is opened there.
- There is **no software-renderer fallback** for the live window: when the GPU/display
  session is unavailable the host reports an `unsupported` host fact (non-failing) rather
  than silently substituting a fake surface.
- Headless render-target PNG evidence (`Viewer.captureScreenshotEvidence`,
  `ViewerRenderTargetPng`) renders onto a raster `SKBitmap` and does **not** require a
  window — it is the deterministic, environment-independent visual proof path used by the
  Scene/Controls render tests.
- Live-window evidence (persistent launch screenshot, real keystroke delivery) requires a
  compiled self-closing host with GPU/display passthrough; `fsi` cannot open a Vulkan window.

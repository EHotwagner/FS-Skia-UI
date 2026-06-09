# Runtime limitations (085)

The faithful nested-tree renderer (`Control.renderTree`) and the pointer-routing,
size-aware durable host (`InteractiveAppHost` / `Viewer.runInteractiveApp`) run on
a render-capable host only. Platform envelope:

- **.NET 10 desktop** is the supported target framework/runtime for the
  render-capable harness and the SkiaSharp-free governance build.
- Rendering uses the **SkiaSharp preview** package already pinned in
  `Directory.Packages.props` (no new dependency); GPU acceleration is via
  **Vulkan** where present.
- **unsupported macOS/mobile/browser**: the renderer and the interactive host are
  validated for Windows and Linux desktop only; macOS, mobile, and browser targets
  are out of scope.
- There is **no software-renderer fallback**: when native Skia is unavailable the
  path reports a classified **blocking host warning** rather than silently passing
  (Principle VII).

## Live input injection (US2, research D6)

A full **live** pointer/keyboard confirmation may require an OS injection tool
absent from the headless host. The honest bar is **synthetic-event delivered
through the real host/adapter path** (a synthetic `PointerPressed`/`PointerReleased`
at a control's bounds routed through `runInteractiveApp`, observing the bound `msg`
dispatched + model change). Because it exercises the real host/adapter — not a
literal fixture — it is **not** marked `[S]`. A wholly-fabricated literal
`PointerInteraction` not delivered through the host WOULD be `[S]` (none are
expected; see plan "Synthetic evidence").

## Windowed-fullscreen blur workaround (US4, SC-004)

Windowed fullscreen scales a fixed-size scene up to the monitor work area, which
blurs a fixed-resolution render. The workaround is the size-aware
`View: Size -> 'model -> SceneNode` (content laid out to the actual swapchain
extent), **or** exactly one documented flag — `--window-startup normal` — for
1:1 sharp output.

**Captured evidence (SC-004, T025)**: `Control.renderTree` rendered the same tree at two
extents — `evidence/size-aware-render/extent-400x300.png` (400×300, 4033 B, non-blank) and
`evidence/size-aware-render/extent-900x600.png` (900×600, 6156 B, non-blank), both
`ScreenshotOk`/`PixelContentNonBlank`. The two differ, confirming content is **laid out to
the actual extent** (no fixed-size render that is then upscaled). The interactive host wires
this via `InteractiveViewerHost.View: Size -> 'model -> SceneNode`, with `runInteractiveViewer`
tracking the current swapchain/window size (updated on `Resized`) and re-rendering through it.

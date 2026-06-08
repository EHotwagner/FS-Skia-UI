# Runtime limitations (080)

The faithful renderer and the `ControlFidelityCheck` decode gate run on a
render-capable host only. Platform envelope:

- **.NET 10 desktop** is the supported target framework/runtime for the
  render-capable harness and the SkiaSharp-free governance build.
- Rendering uses the **SkiaSharp preview** package already pinned in
  `Directory.Packages.props` (no new dependency); GPU acceleration is via
  **Vulkan** where present.
- **unsupported macOS/mobile/browser**: the fidelity gate's pixel decode is
  validated for Windows and Linux desktop only; macOS, mobile, and browser
  targets are out of scope.
- There is **no software-renderer fallback** for the gate: when native Skia is
  unavailable the gate reports a classified **blocking host warning** (cannot
  decode) rather than silently passing (FR-008, Principle VII).

The SkiaSharp-free `ControlsCatalogDocsCheck` byte-floor currency gate is
unaffected and continues to run in GPU-free CI.

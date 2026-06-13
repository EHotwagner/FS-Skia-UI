# Unsupported-GL classified diagnostic (feature 119, US3 / FR-005 / SC-004)

**Failure class**: environment-limitation classification (benign vs. blocking).
**Next action**: none; classification is unit-verified and the live environment cannot be made
GL-unavailable without a native driver crash (documented below).

## Classification logic (real, shipped)

The GL-unavailable path is classified honestly in the shipped host:

- `Host.Diagnostics.glUnavailable` → `Fatal`, stage `GlContext`, message names **OpenGL** and
  states there is **no fallback renderer**; never suggests a Vulkan or software fallback.
- `GlHost.createWindow` / `initializeWindow` wrap Silk window creation in `try/with` → a classified
  `startupFailed GlSurface` `RenderDiagnostic` (never an unhandled throw).
- `GlHost.createSkiaContext` returns a classified `startupFailed GlContext` / `SkiaContext` when the
  GL context is absent or `GRGlInterface.CreateOpenGl` / `GRContext.CreateGl` returns null.
- `ensureFramebufferSurface` returns `startupFailed Framebuffer` when `SKSurface.Create` over FBO 0
  fails — never a crash or a black window.
- The viewer-level mapping (`SkiaViewer.fs` `toViewerFailure`) classifies `GlContext` /
  `GlRenderer` / `GlSurface` / `Framebuffer` stages as `ViewerDiagnosticCategory.OpenGl` and the
  blocked stage as `UnsupportedEnvironment` (benign) — never `ProductDefect` for an
  environment limitation.

## Unit verification (real, passing)

- `Feature119OpenGlHostTests`: "GL-unavailable diagnostic is classified honestly with no false
  fallback" — asserts `glUnavailable` is `Fatal` / `GlContext`, names OpenGL, states no fallback,
  and never suggests Vulkan/software.
- `SkiaViewer.Tests` failure-classification table: `ViewerRunBlockedStage.GlContext` →
  `UnsupportedEnvironment` / `ViewerDiagnosticCategory.Framebuffer`.

Both pass under `Dev` (73 SkiaViewer.Tests green).

## Live reproduction attempts (real command logs)

This host has **robust** GL availability, which made a clean "GL absent" state unreproducible:

1. `env -u DISPLAY -u WAYLAND_DISPLAY dotnet run` (no display server):
   → `result=Ok ()`, `readback=false`, frame rendered. The host creates a GL context **headlessly**
   through the DRM render node (`/dev/dri/renderD128`) — a positive finding for CI/headless.
2. `env GALLIUM_DRIVER=nonexistent MESA_LOADER_DRIVER_OVERRIDE=nonexistent dotnet run`:
   → `libEGL warning: egl: failed to create dri2 screen`, then Mesa falls back to its software
   rasteriser and the host still renders `Ok ()`. GL stays available below the host.
3. Fully corrupting the EGL vendor library produced a **native** (driver-level) crash before any
   managed code runs — i.e. an OS/driver fault, not a state the managed host can classify.

**Conclusion**: on this hardware OpenGL is always reachable (hardware acceleration, or Mesa's
software rasteriser as a Mesa-internal fallback **below** the host). A clean managed
GL-unavailable failure is therefore not reproducible here. The classification path is real and
unit-verified; the corresponding **live** capture is recorded as **skipped** in `tasks.md` (T026,
`[-]`) with this rationale rather than fabricated. This refines feature 118's assumption that a
no-GPU shell would reproduce GL-unavailable — on a Mesa host it does not, because Mesa always
offers a software GL path.

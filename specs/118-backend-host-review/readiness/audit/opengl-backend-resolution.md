# Resolution path: an OpenGL present backend (feature 118 → next roadmap phase)

The readback round-trip documented in [present-path-audit.md](./present-path-audit.md) is a
consequence of hosting on **Vulkan**, where SkiaSharp's managed binding cannot wrap a swapchain
image as an `SKSurface` ([#1502](https://github.com/mono/SkiaSharp/issues/1502)) or hand off the
present layout ([#2191](https://github.com/mono/SkiaSharp/issues/2191)). Hosting on **OpenGL**
removes the problem at the root, because SkiaSharp's GL interop is complete.

## Why OpenGL fixes it

For GL, SkiaSharp exposes `GRGlFramebufferInfo` and `GRBackendRenderTarget.GetGlFramebufferInfo`,
and `SKSurface.Create(context, glBackendRenderTarget, …)` succeeds. The standard pattern:

1. Create a GL context for the window (Silk.NET windowing already supports GL; `GRGlInterface`).
2. Wrap the window's default framebuffer (FBO 0) in a `GRBackendRenderTarget` from
   `GRGlFramebufferInfo { FramebufferObjectId = 0; Format = GL_RGBA8 }`.
3. `SKSurface.Create` that render target → draw the scene with the existing `SceneRenderer` →
   `surface.Flush()`.
4. The windowing toolkit calls `SwapBuffers` (eglSwapBuffers / wglSwapBuffers).

**Direct present, zero readback, no staging buffer, no per-frame command pool, no
`vkQueueWaitIdle`.** GL also has an implicit "framebuffer 0", so there is no per-swapchain-image
wrapping problem the way Vulkan has. Skia's GL backend is also its most mature/battle-tested
GPU backend (Skia warns many Vulkan drivers have bugs it triggers), so robustness likely
improves too. The scene renderer is `SKCanvas`-based and backend-agnostic, so visual output is
unchanged.

## Consequences (why this is its own phase, not a Phase-9 change)

- **Breaking public surface.** `Host/Vulkan.fsi` exposes public contract modules
  `VulkanResources` (resource-ownership ledger), `VulkanStartup` (startup-stage model), and
  `VulkanHost`. A GL backend rewrites or removes these → a breaking `.fsi` change with test
  re-authoring.
- **Dependency change.** Drop `Silk.NET.Vulkan` + `Silk.NET.Vulkan.Extensions.KHR`, add
  `Silk.NET.OpenGL` → `Directory.Packages.props`, `DependencyReport`, `docs/dependencies`.
- **Constitution amendment.** `constitution.md` currently mandates the Vulkan backend and a
  "Vulkan smoke" clause. Changing the backend is a constitution-level decision
  (`/speckit-constitution`).
- **Governance-token churn.** The generated `evidence-formats.md` makes `Vulkan` a *required*
  token in every feature's `runtime-limitations.md` (rule lives in `FS.Skia.UI.Build`); ~90+
  files repo-wide reference Vulkan (docs/reports, ADRs, architecture, governance tests,
  skills). Wide but mechanical.
- **The 1,770-line `Vulkan.fs` host backend** is replaced by a much smaller GL host (net
  simplification), but it is still a from-scratch backend swap with real risk.

## Platform notes

- **Windows + Linux desktop (the only supported targets): fine.** GL is ubiquitous (Mesa /
  vendor drivers). Wayland uses EGL (works); the single-threaded render loop suits GL's
  thread-affine context.
- **macOS GL-deprecation is moot** — macOS/mobile/browser are already out of scope.

## Recommended sequencing

1. **Feature 118 (this rung):** ship the public `ViewerPresentMode` contract, config threading,
   FR-004 capture decoupling, FR-005 safe fallback, FR-007 diagnostic, and this audit. Record
   FR-002/SC-002 as blocked-by-dependency. *(done)*
2. **Next phase — "OpenGL present backend":** its own spec + plan + constitution amendment +
   dependency change. The `ViewerPresentMode` seam is the plug-in point — a GL-backed direct
   present becomes the `DirectToSwapchain` implementation (or a third mode), at which point
   FR-002/SC-002's readback-free goal is finally met.

## Sources

- [mono/SkiaSharp #1502](https://github.com/mono/SkiaSharp/issues/1502) — SKSurface from VkImage RT returns null
- [mono/SkiaSharp #2191](https://github.com/mono/SkiaSharp/issues/2191) — Vulkan ImageLayout not exposed
- [mono/SkiaSharp #1381](https://github.com/mono/SkiaSharp/issues/1381) — ReadPixels ~120× slower than draw
- [MS Learn: GRBackendRenderTarget.GetGlFramebufferInfo](https://learn.microsoft.com/en-us/dotnet/api/skiasharp.grbackendrendertarget.getglframebufferinfo?view=skiasharp-2.88)
- [Skia Vulkan docs](https://skia.org/docs/user/special/vulkan/)

# SkiaSharp Vulkan present-layout API research (feature 118, US1)

## Question
Does any SkiaSharp version expose `GRBackendSurfaceMutableState` (or any API to set/
transition a Vulkan `VkImage` layout to `PRESENT_SRC_KHR` during/after flush), which
research R1/R2 assumed for the cached per-image direct-to-swapchain present path (FR-006)?

## Finding: NO — not in any available version, including the newest preview.
- Newest SkiaSharp on NuGet = `4.147.0-preview.3.1` (the 4.x line is a major rewrite
  tracking Google Skia m147). We are pinned to it; there is nothing newer to upgrade to.
- Reflection of the actual `4.147.0-preview.3.1` `SkiaSharp.dll`: NO
  `GRBackendSurfaceMutableState`, NO `GRFlushInfo`, NO semaphore/signal/layout flush
  overloads. Only `SKSurface.Flush()` / `Flush(submit, synchronous)` and
  `GRContext.Flush()/Flush(submit,sync)/Submit(sync)`.
- The underlying Skia C++ API HAS `GrBackendSurfaceMutableState` / `skgpu::MutableTextureState`
  (Skia docs: "at the end of the flush we transition the surface to the requested state …
  used if the surface will be used for presenting"). The .NET binding never bound it.
- mono/SkiaSharp #2191 "[BUG] Vulkan Interop requires exposure of ImageLayout" (opened
  Aug 2022) requests exactly this; STILL OPEN/UNRESOLVED. Maintainers: "these functions
  aren't exposed in SkiaSharp."

## Resolution / chosen design (correct without the API)
Re-wrap the acquired swapchain image per frame in a `GRBackendRenderTarget`/`SKSurface`
with `GRVkImageInfo.ImageLayout = UNDEFINED` (valid barrier source from any prior layout,
discards stale contents — correct because each frame is a full Clear+redraw). Own the
`COLOR_ATTACHMENT_OPTIMAL → PRESENT_SRC_KHR` transition via a PERSISTENT per-swapchain
command pool + per-image command buffer/semaphore/fence (built once, rebuilt on swapchain
recreation). Semaphore-synced present (transition submit signals → QueuePresent waits) →
no `vkQueueWaitIdle`. This fully achieves SC-002 (no readback, no per-frame staging buffer,
no per-frame command pool, no per-frame queue stall). The only spec deviation: FR-006's
literal "cache the GRBackendRenderTarget per swapchain image index" — the cheap CPU-side
wrap is recreated per frame instead; caching it is unsafe without the unexposed layout API.
The persistent per-image command/sync resources DO follow the FR-006 rebuild-on-recreation
lifecycle.

## Sources
- https://github.com/mono/SkiaSharp/issues/2191
- https://api.skia.org/classGrDirectContext.html
- https://skia.org/docs/user/special/vulkan/

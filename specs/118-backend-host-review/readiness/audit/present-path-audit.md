# Present-path audit (feature 118, US3 / FR-009)

The backend-and-host-mode review of `src/SkiaViewer/Host/Vulkan.fs`. This is the central,
fully-delivered output of feature 118: an honest account of what the live present path
costs, why the readback-free `DirectToSwapchain` path **cannot** be built on the current
SkiaSharp binding, and the concrete resolution (an OpenGL present backend).

## 1. The live present path today (OffscreenReadback — the only working path)

Every live frame (`renderFrameReadback`, `Vulkan.fs:1106`) performs a GPU→CPU→GPU round-trip:

| Step | Call site | Cost |
|------|-----------|------|
| Offscreen render | `renderSceneToPixels` (`Vulkan.fs:929`) — Skia-allocated `SKSurface` | GPU draw (cheap, ~sub-ms) |
| **GPU→CPU readback** | `surface.ReadPixels(...)` (`Vulkan.fs:959`) | **dominant cost — see §2** |
| Per-frame staging buffer | `createStagingBuffer` (`Vulkan.fs:865`, called `:971`) — `vkCreateBuffer`/`vkAllocateMemory`/`vkMapMemory`/`Marshal.Copy` every frame | per-frame allocation + CPU→GPU copy |
| Per-frame command pool | `vk.CreateCommandPool` (`Vulkan.fs:979`) + command buffer, image-layout transitions, `vkCmdCopyBufferToImage` | per-frame allocation |
| **Full pipeline stall** | `vk.QueueWaitIdle(queue)` (`Vulkan.fs:1079`) | **whole-queue idle every frame** |
| Present | `swapchainExt.QueuePresent` (`Vulkan.fs:1092`) | present |

The **same** `renderSceneToPixels` readback routine is also the evidence/screenshot routine,
so ordinary live frames pay evidence-mode cost. (Feature 118 decoupled capture from present —
FR-004 — so capture is now an on-demand offscreen render, but the readback is still on the
live present path because there is no alternative; see §3.)

## 2. Quantified cost of the readback round-trip

GPU→CPU readback is the well-documented slow path:

- `SKSurface.ReadPixels` measured at **~30 ms vs ~0.25 ms to draw — a ~120× slowdown**
  ([mono/SkiaSharp #1381](https://github.com/mono/SkiaSharp/issues/1381), OpenGL backend).
- Skia's own docs: reading pixels back from the GPU is slow; minimize it
  ([skia.org/docs/user/special/vulkan](https://skia.org/docs/user/special/vulkan/)).
- This codebase's path is *worse* than a bare `ReadPixels`: it adds a per-frame staging
  buffer allocate+map+copy, a per-frame command pool, and a full `vkQueueWaitIdle` stall.

The cost is **structural, not tunable**: async readback (transfer buffers) is not exposed by
the binding, and the two ways to avoid the readback entirely are both blocked (§3).

## 3. Why the readback cannot be removed on SkiaSharp (the central finding)

Feature 118 set out to add a readback-free `DirectToSwapchain` present path (FR-002/SC-002).
The implementation seam exists (`presentDirectImage`, `Vulkan.fs:1302`; `renderFrameDirect`,
`:1365`) and was exercised on a real AMD/RADV Vulkan GPU. It is **blocked upstream** by
SkiaSharp's managed-binding Vulkan gap:

- **Cannot wrap a swapchain image as an `SKSurface`.** `SKSurface.Create(context,
  vkBackendRenderTarget, …)` returns **null** for every `ImageLayout`
  (Undefined/ColorAttachment/General/PresentSrc) × colorspace (null/sRGB), even though
  `GRBackendRenderTarget.IsValid = true`, the `VkImage` handle is valid, and the format
  matches (`B8G8R8A8_UNORM` / Bgra8888). Reproduced live (see `probeDirectWrap`,
  `Vulkan.fs:1176`, and `readiness/smoke/direct-mode-smoke.md`). Confirmed by
  [mono/SkiaSharp #1502](https://github.com/mono/SkiaSharp/issues/1502) — open since Sep 2020.
- **Cannot set/query the `VkImage` layout for present handoff.**
  [mono/SkiaSharp #2191](https://github.com/mono/SkiaSharp/issues/2191) — open since Aug 2022.
- **The reverse trick is also blocked.** Rendering to a Skia-allocated offscreen surface and
  `vkCmdBlitImage`-ing it to the swapchain (no CPU readback) needs the offscreen surface's
  `VkImage`/`GRVkImageInfo` handle — which SkiaSharp also will not expose (#2191). Nothing to
  blit *from*.
- We are on the **newest** SkiaSharp (`4.147.0-preview.3.1`); the 4.x line is a major rewrite
  tracking Google Skia m147. There is no newer version, and the gap predates it by years.

**Conclusion:** a readback-free windowed Vulkan present is **not achievable on any SkiaSharp
version**. FR-002 / SC-002 are recorded as **blocked-by-dependency**, not achieved.
`DirectToSwapchain` therefore detects the limitation once at init (`probeDirectWrap`) and
safely degrades to the proven `OffscreenReadback` path with a single `Warning` diagnostic
(FR-005) — verified live in `readiness/smoke/safe-fallback.md`.

## 4. Why this does not affect SkiaSharp's mainstream use

The gap only bites an app that (a) selects the **Vulkan** backend *and* (b) drives its own
window swapchain present directly, with no platform compositor — exactly what FS-Skia-UI is.
SkiaSharp's mainstream consumers dodge it:

- **CPU/raster surfaces** (`SKCanvasView`, image/PDF/SVG export): no GPU swapchain at all.
- **OpenGL-backed GPU** (`SKGLView`, MAUI `GpuRenderingEngine`, Avalonia GL): SkiaSharp **does**
  expose `GRGlFramebufferInfo` / `GRBackendRenderTarget.GetGlFramebufferInfo`, so Skia wraps
  the window framebuffer (FBO 0) directly and the toolkit calls `SwapBuffers` — direct present,
  no readback. The exact operation broken for Vulkan **works for GL**.
- **Platform-composited controls** (Xamarin/MAUI/Avalonia): the framework owns present.

## 5. Resolution

See [opengl-backend-resolution.md](./opengl-backend-resolution.md). The readback round-trip is
retired by hosting on an **OpenGL** backend, where SkiaSharp's framebuffer-wrap interop is
complete. That is a ground-up host re-architecture (its own spec/plan + constitution amendment
+ dependency change), captured as the next roadmap phase — *not* a Phase-9 change. The public
`ViewerPresentMode` seam feature 118 adds is the plug-in point: a GL-backed direct present
becomes a third path (or the `DirectToSwapchain` implementation) the day the backend lands.

## 6. Deferred scope (FR-010 / FR-011, unchanged)

Out of scope and explicitly deferred: render-thread/compositor split, layer/scene-submission
diffing, scene-graph/GPU/layer caching, and any timing-based pass/fail gate. Backend timing is
a human/diagnostic signal only; deterministic gating stays on counts and booleans.

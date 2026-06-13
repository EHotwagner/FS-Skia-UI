# Zero-readback present proof (feature 119, US1 / SC-001)

**Command**: production GL host driven by the evidence harness
(`readiness/sample-smoke/evidence-harness.fs`), `dotnet run` against
`Host.Viewer.run` → `GlHost.run`, real AMD Radeon (Mesa) OpenGL GPU under `DISPLAY=:1`.
**Artifact path**: this file; the captured frame is `readiness/sample-smoke/gl-direct-present-frame.png`.
**Failure class**: none (Ok). **Counts/booleans only — no timing gate** (per 118 §6).
**Next action**: none; this unblocks feature 118 SC-002.

## Proof (counts and booleans, not timing)

| Signal | Value | Meaning |
|--------|-------|---------|
| present mode | `DirectToSwapchain` | the GL readback-free default |
| host present diagnostic | `present-mode=DirectToSwapchain readback=false` | the host's own honest readback flag |
| per-frame GPU→CPU readback | **0** | `renderFrameDirect` returns an empty `Pixels` array — no `ReadPixels`, no staging buffer, no queue stall |
| frames presented | 60 | continuous direct present via `IWindow.SwapBuffers` |
| run result | `Ok ()` | clean launch + run + shutdown |

## Why this is genuinely readback-free

`GlHost.renderFrameDirect` draws the scene straight onto the FBO-0-bound `SKSurface`
(`SKSurface.Create` over a `GRBackendRenderTarget` wrapping the window's default framebuffer),
flushes the GL context, and calls `IWindow.SwapBuffers`. It performs **no** `surface.ReadPixels`,
allocates **no** staging buffer, and issues **no** queue wait. The returned `FrameSnapshot.Pixels`
is `[||]` — the structural signal that no readback occurred. The on-demand screenshot path renders
its **own** offscreen surface only when a capture is requested (FR-004), so the steady-state live
present path never reads back.

This is the exact operation that was infeasible on Vulkan (feature 118: `SKSurface.Create` over a
Vulkan swapchain image returns null, mono/SkiaSharp #1502). On OpenGL it succeeds
(`GRBackendRenderTarget.IsValid = true`, non-null `SKSurface`), so feature 118 FR-002/SC-002 is
delivered.

## Captured frame verification (production render path)

`readiness/sample-smoke/gl-direct-present-frame.png` (640×480, PNG) was produced by the on-demand
capture mid-run and decoded with SkiaSharp. Sampled pixels match the rendered scene **exactly**:

- background corner `#ff12161e` = scene background `rgb(18,22,30)`
- text band `#ffebebf0` = scene text colour `rgb(235,235,240)`
- 144 sampled orange pixels = the moving rect `rgb(255,138,0)`

The pixels are the real production scene, confirming the capture exercises the shipped render
path, not a hand-built parallel scene.

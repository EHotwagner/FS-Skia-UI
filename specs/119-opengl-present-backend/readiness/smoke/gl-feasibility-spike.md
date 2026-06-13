# GL present-path feasibility spike (pre-implementation de-risk)

**Command**: standalone spike `/tmp/glspike` (Silk.NET.Windowing 2.23.0 + Silk.NET.OpenGL 2.23.0 +
SkiaSharp 4.147.0-preview.3.1), `dotnet run -c Release` under `DISPLAY=:1` (Wayland session,
AMD Radeon renoir, Mesa 26.1.2, `/dev/dri/renderD128`).
**Artifact path**: this file (program + verbatim output captured below).
**Failure class**: feasibility / research (not a routed gate; de-risks FR-001/SC-001 before the host rewrite).
**Next action**: implement the production `Host/OpenGl.fs` host using the validated API sequence.

## Result

```
SPIKE-OK: ctx+FBO0 wrap valid; size=320x240 center=#ffff4500 corner=#ff6495ed
```

- `center=#ffff4500` — OrangeRed circle drawn by Skia, read back from the GPU surface.
- `corner=#ff6495ed` — CornflowerBlue clear, read back from the GPU surface.

This is the exact operation that returns `null` on the Vulkan backend
(`SKSurface.Create` over a backend render target, mono/SkiaSharp #1502): on GL it succeeds with
`GRBackendRenderTarget.IsValid = true` and a non-null `SKSurface` wrapping **FBO 0**, drawn and
presented with `SwapBuffers` and **no GPU→CPU readback**.

## Validated production API sequence (carried into Host/OpenGl.fs)

1. `WindowOptions.Default` with `API <- GraphicsAPI(ContextAPI.OpenGL, ContextProfile.Core, ContextFlags.Default, APIVersion(3,3))`.
2. On window `Load`: `window.CreateOpenGL()`, then build a proc-address loader from
   `window.GLContext.TryGetProcAddress(name)`.
3. `GRGlInterface.CreateOpenGl(GRGlGetProcedureAddressDelegate getProc)` — **non-null required**
   (parameterless `GRGlInterface.Create()` returns null on Linux Mesa; the explicit loader is mandatory).
4. `GRContext.CreateGl(glInterface)`.
5. `GRGlFramebufferInfo(0u, uint32 (SKColorType.Rgba8888.ToGlSizedFormat()))` (GL_RGBA8).
6. `new GRBackendRenderTarget(fbWidth, fbHeight, samples=0, stencil=8, fbInfo)` sized from
   `window.FramebufferSize` (high-DPI/Wayland correct).
7. `SKSurface.Create(grContext, rt, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888)`.
8. Draw scene → `surface.Canvas.Flush()` → `grContext.Flush()` → `window.SwapBuffers()`. No readback.

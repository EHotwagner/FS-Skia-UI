# Phase 1 Data Model: OpenGL Present Backend

This feature is a present/host **backend swap**, not a new data domain. The "entities" are the
host-state and contract types whose shape changes. The Elmish `Model`/`Msg`/`Effect` of the
viewer program are **unchanged**; only the interpreter-edge host types change.

## Entity: GL host backend (replaces the Vulkan host body)

The GL successor to `VulkanHost` / `VulkanResources` / `VulkanStartup`. Owns the
interpreter-edge GPU state. Internal helpers stay hidden; only the run entry is public.

| Field / member | Meaning | Notes |
|----------------|---------|-------|
| GL context | The window's OpenGL context (`Silk.NET` `GL` + `GRGlInterface`) | thread-affine; single render thread |
| `GRContext` | Skia GL GPU context (`GRContext.CreateGl`) | created once per context; recreated on context loss |
| Framebuffer render target | `GRBackendRenderTarget` over FBO 0 (`GRGlFramebufferInfo`) | recreated on resize; sized from framebuffer pixels |
| `SKSurface` | Skia surface wrapping the render target | `GRSurfaceOrigin.BottomLeft`, `Rgba8888`; recreated on resize |
| `run` | Public entry: `program: ViewerProgram<'model,'msg> -> Result<unit, RenderDiagnostic>` | same signature shape as `VulkanHost.run` (source-stable for `Viewer.fs`) |

**Lifecycle / state transitions** (the startup-stage model, GL analogue of `VulkanStartup`):

```
CreateWindow → CreateGlContext → CreateGrContext → WrapFramebuffer(FBO0) → RenderLoop
                     │                  │                  │
                     ▼ (fail)           ▼ (fail)           ▼ (fail)
              UnsupportedEnvironment (benign, classified)  /  blocking defect
RenderLoop --resize--> recreate RenderTarget+SKSurface --> RenderLoop   (leak-free, FR-006)
RenderLoop --context-loss--> classified diagnostic --> honest fail / best-effort recover (R5)
```

Per-frame (the interpreter step): `update`→draw scene via `SceneRenderer` onto `SKSurface.Canvas`
→ `Flush` → `SwapBuffers`. **No** readback / staging buffer / command pool / queue stall.

## Entity: `ViewerPresentMode` (retained public DU — semantics re-mapped)

`src/SkiaViewer/PresentMode.fsi`. **Cases unchanged; doc-comments re-mapped to GL** (FR-007).

| Case | Vulkan meaning (before) | GL meaning (after) |
|------|-------------------------|--------------------|
| `DirectToSwapchain` | render onto acquired Vulkan swapchain image (infeasible → degraded) | render onto FBO-0 `SKSurface`, `SwapBuffers` — **readback-free, default** |
| `OffscreenReadback` | offscreen render + GPU→CPU readback + upload (the only working path) | offscreen render + readback — evidence/screenshot routine + explicit fallback |

Validation rule: `ViewerOptions.PresentMode` default flips to `DirectToSwapchain` on GL
(documented intentional change, R4). Field type/shape unchanged → source-stable.

## Entity: Diagnostic classification surface (reconciled to GL)

`src/SkiaViewer/SkiaViewer.fsi`. Public DUs lose the Vulkan-specific cases / gain GL-meaningful
ones (breaking surface change, regenerated baselines):

| Type | Before (Vulkan) | After (GL) |
|------|-----------------|-----------|
| `ViewerDiagnosticCategory` | `… | Vulkan | Skia | Swapchain | …` | `… | OpenGl | Skia | Framebuffer | …` (final names settled in contract) |
| `ViewerRunBlockedStage` | `… | Surface | Swapchain | … | Readback | …` | `… | Surface | GlContext | … | Readback | …` |

`Readback` stays (the `OffscreenReadback`/evidence path still reads back). Final case naming is
fixed in `contracts/gl-host-surface.md` and exercised in FSI before the `.fs` body.

## Entity: Unsupported-environment diagnostic (FR-005)

Reuses the existing classified `RenderDiagnostic` shape and the benign/blocking host-warning
classifier. New content: the **failed stage** is GL context/FBO acquisition; benign →
`UnsupportedEnvironment`, blocking → defect. No new public type — content/classification only.

## Entity: Dependency manifest (FR-008)

`Directory.Packages.props`: `Silk.NET.Vulkan` + `Silk.NET.Vulkan.Extensions.KHR` removed;
`Silk.NET.OpenGL` (`2.23.0`) added. `src/SkiaViewer/SkiaViewer.fsproj` `PackageReference`s
follow. Not a runtime data type — recorded here as the dependency-edge entity the
`DependencyReport` validates.

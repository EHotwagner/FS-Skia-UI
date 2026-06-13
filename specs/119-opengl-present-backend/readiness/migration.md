# Migration guide — Vulkan host → OpenGL host (feature 119)

Feature 119 replaces the Vulkan live-present host backend with an OpenGL backend. This is a
**breaking** public-surface change in `FS.Skia.UI.SkiaViewer`, shipped in a new bumped package
version. The high-level consumer entry points are **source-stable** — most consumers need no
change.

## Source-stable (no change required) — SC-005

- `Controls.Elmish.runInteractiveApp`
- `Viewer.runApp`, `Viewer.runAppWithWindowBehavior`, `Viewer.runInteractiveViewer`,
  `Viewer.runInteractiveViewerWithWindowBehavior`, `Viewer.runBounded`, `Viewer.runUntilFirstFrame`,
  `Viewer.runForFrames`, `Viewer.captureScreenshotEvidence`
- `ViewerOptions` (record shape unchanged; `PresentMode` field retained — see default change below)

These compile unchanged against the new package (verified by FSI + the full `Dev` build).

## Removed / renamed public members → GL replacements

| Removed / renamed (Vulkan) | Replacement (OpenGL) | Notes |
|----------------------------|----------------------|-------|
| `Host.VulkanResources` (module) | `Host.GlResources` | GL resource-ownership ledger; `ResourceCategory` cases are now `GlContext`/`GlSurface`/`GrContext`/`Framebuffer`/`SkiaSurface`/`SkiaGpu` |
| `Host.VulkanStartup` (module) | `Host.GlStartup` | GL startup-stage ordering + cleanup model; same function shapes (`stages`, `stageByName`, `simulateFailure`, `simulateSuccessfulShutdown`) |
| `Host.VulkanHost.run` | `Host.GlHost.run` | identical signature `ViewerProgram<'model,'msg> -> Result<unit, RenderDiagnostic>`; `Viewer.run` routes to it unchanged |
| `Host.DiagnosticStage.VulkanInstance` | `Host.DiagnosticStage.GlContext` | GL context creation stage |
| `Host.DiagnosticStage.VulkanDevice` | `Host.DiagnosticStage.GlRenderer` | GL renderer/device stage |
| `Host.DiagnosticStage.VulkanSurface` | `Host.DiagnosticStage.GlSurface` | window surface/drawable stage |
| `Host.DiagnosticStage.VulkanSwapchain` | `Host.DiagnosticStage.Framebuffer` | default-framebuffer (FBO 0) wrap stage |
| `Host.Diagnostics.vulkanUnavailable` | `Host.Diagnostics.glUnavailable` | classified GL-unavailable diagnostic |
| `ViewerDiagnosticCategory.Vulkan` | `ViewerDiagnosticCategory.OpenGl` | public diagnostic category |
| `ViewerDiagnosticCategory.Swapchain` | `ViewerDiagnosticCategory.Framebuffer` | public diagnostic category |
| `ViewerRunBlockedStage.Swapchain` | `ViewerRunBlockedStage.GlContext` | public blocked-stage |

## Retained, re-documented (no code change) — FR-007

- `ViewerPresentMode` (DU retained; both cases `OffscreenReadback` | `DirectToSwapchain` kept).
  **Semantics re-mapped to GL**: `DirectToSwapchain` now renders straight onto the window's default
  framebuffer (FBO 0) and presents via buffer swap — genuinely **readback-free**, and is now the
  **default** (`Viewer.defaultConfiguration` / applied `ViewerOptions.PresentMode`). `OffscreenReadback`
  backs the on-demand evidence/screenshot routine and an explicit fallback.

## Retained with changed semantics

- `ViewerBackendPreference.Vulkan` (case retained for source compatibility) is now reported
  `UnsupportedOption` ("Vulkan backend is no longer supported; this viewer host presents through
  OpenGL"). `ViewerBackendPreference.OpenGL` is now `Honored`, and the default backend selects OpenGL.

## Dependency change

`Silk.NET.Vulkan` + `Silk.NET.Vulkan.Extensions.KHR` are removed; `Silk.NET.OpenGL` (`2.23.0`) is
added. Consumers referencing the high-level entry points need no dependency change; consumers that
referenced `Silk.NET.Vulkan` directly through the host must migrate to `Silk.NET.OpenGL`.

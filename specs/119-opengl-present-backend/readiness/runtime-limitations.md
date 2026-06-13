# Runtime limitations & failure diagnostics (feature 119)

## Platform / runtime support boundary

Feature 119 replaces the live viewer host's Vulkan present backend with an **OpenGL** backend. It
runs on a **.NET 10 desktop** host rendering through **OpenGL** via the **SkiaSharp preview**
native binding, on Windows and Linux desktop (`net10.0`). **unsupported macOS/mobile/browser** —
those targets are out of scope (GL deprecation on macOS is moot). There is **no software-renderer fallback**:
a host without a working GL context classifies as `UnsupportedEnvironment`, not a product defect.
This feature is **no package/API/runtime support expansion** — it is a present/host backend swap
with source-stable consumer entry points.

## Documented evidence path

- Public `ViewerPresentMode` contract (re-documented for GL), the GL host surface
  (`GlResources`/`GlStartup`/`GlHost`), and the reconciled diagnostic DUs
  (`ViewerDiagnosticCategory.OpenGl`/`Framebuffer`, `ViewerRunBlockedStage.GlContext`) —
  deterministic tests (`Feature119` + `Feature118PresentModeTests` + `NativeStartupCleanupTests`).
- The live present path — a real persistent window on a real **AMD Radeon (Mesa) OpenGL GPU**
  presenting 60 frames in `DirectToSwapchain` mode with **zero per-frame readback**
  (`readiness/supported-host-persistent-launch.txt`, `readiness/smoke/zero-readback-present.md`).
- The on-demand capture (FR-004) decoupled from the live present —
  `readiness/sample-smoke/gl-direct-present-frame.png` (pixels verified against the rendered
  scene: background `rgb(18,22,30)`, rect `rgb(255,138,0)`, text `rgb(235,235,240)`).
- The FR-005 safe GL-unavailable classification — `readiness/smoke/unsupported-gl-diagnostic.md`.

## Resolution of feature 118's blocked finding

The readback-free direct present (feature 118 FR-002/SC-002) was **blocked upstream** on Vulkan:
`SKSurface.Create` could not wrap a Vulkan swapchain image (mono/SkiaSharp #1502). On the
**OpenGL** backend the same operation — `SKSurface.Create` over a `GRBackendRenderTarget` bound to
the window's default framebuffer (FBO 0) — **succeeds** (`GRBackendRenderTarget.IsValid = true`,
non-null surface), giving genuine zero-readback direct present. Feature 118's deferred deliverable
is delivered.

## Failure diagnostics

- A missing required evidence artifact fails `Route --enforce` (it names the artifact + tier).
- A GL context / framebuffer-wrap failure emits an honest classified diagnostic
  (`UnsupportedEnvironment` when GL is absent/broken; a blocking defect otherwise) — never a crash
  or corrupt frame, never a false success (FR-005, Principle VII).
- A non-byte-identical default-mode render would surface in the standing Scene-parity goldens
  under `Dev`; the canvas-based scene renderer is unchanged across the backend swap.
- A race-like or unknown-concurrent-FAKE failure is rerun sequentially before any
  product-regression classification (shared `.fake` state).

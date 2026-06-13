# Smoke: present-mode honesty (T034, SC-008 / FR-016 / FR-017)

**Authoritative command:** `dotnet run --project tests/SkiaViewer.Tests` (`Feature118PresentModeTests`)
+ source review of `samples/DemoReel/Program.fs` and `src/SkiaViewer/SkiaViewer.fsi`.
**Failure class:** product-defect.

## What was proven

- **FR-016 docstring:** `ViewerOptions.PresentMode` now documents the shipped default
  `ViewerPresentMode.DirectToSwapchain` (the zero-readback direct present), correcting the stale
  feature-118 text that named `OffscreenReadback`. The shipped default at
  `Viewer.defaultConfiguration` is `DirectToSwapchain` (feature 119), so the docstring agrees with it.
- **FR-017 sample:** `samples/DemoReel/Program.fs` `viewerOptions` now sets
  `PresentMode = ViewerPresentMode.DirectToSwapchain` for the live interactive window; the
  evidence/screenshot path (`SceneEvidence.renderPng`, an offscreen readback) is unchanged.
- **readback=false present diagnostic:** in `DirectToSwapchain`, `GlHost.renderFrame` announces once
  `present-mode=DirectToSwapchain readback=false (live frames render straight onto the default
  framebuffer)` and returns a snapshot with empty `Pixels` (no GPU→CPU readback). This is the same
  zero-readback live present feature 119 launch-verified on real AMD Mesa hardware.

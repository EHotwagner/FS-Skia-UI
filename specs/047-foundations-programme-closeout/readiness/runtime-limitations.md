# Runtime limitations (T003)

This feature is **documentation / measurement / verification-record only** and changes
no runtime path; the runtime-limitation statements are reproduced here for completeness
and remain unchanged (FR-010, SC-006).

- **.NET 10 desktop**: the framework targets `net10.0` desktop hosts.
- **Vulkan**: the SkiaSharp viewer backend renders through Vulkan.
- **SkiaSharp preview**: built on a preview SkiaSharp; APIs may shift.
- **Unsupported**: macOS, mobile, and browser/WASM hosts are **not supported**
  (unsupported macOS/mobile/browser).
- **No software-renderer fallback**: there is no software rasteriser fallback; a host
  without a working Vulkan surface cannot run the viewer.

No runtime/visual/screenshot evidence is in scope (SC-006); the product `src/**` surface
and all visual paths are untouched (`git diff --stat -- 'src/**'` is empty —
`readiness/runtime-untouched.md`).

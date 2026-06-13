# Runtime limitations & failure diagnostics (feature 118)

## Platform / runtime support boundary

Feature 118 is a backend present-mode review for the live viewer host. It runs on a
**.NET 10 desktop** host rendering through **Vulkan** via the **SkiaSharp preview** native
binding, on Windows and Linux desktop (`net10.0`). **unsupported macOS/mobile/browser** — those
targets are out of scope and there is **no software-renderer fallback**; a host without a Vulkan
backend classifies as `UnsupportedEnvironment`, not a product defect.

## Documented evidence path

- Public `ViewerPresentMode` contract + config threading + the FR-007 diagnostic-category
  mapping — deterministic tests (`Feature118PresentModeTests`).
- The live present path — a real persistent window on a real AMD/RADV Vulkan GPU presenting 40
  frames in each present mode (`readiness/smoke/direct-mode-smoke.md`,
  `default-byte-identity.md`), byte-identical captures (sha256 match).
- The FR-005 safe fallback — a real forced wrap failure on the real backend degrades to the
  readback path with one `Warning` (`readiness/smoke/safe-fallback.md`).

## Blocking runtime limitation discovered (the central finding)

The readback-free `DirectToSwapchain` present path (FR-002/SC-002) is **blocked upstream** by
the **SkiaSharp preview** managed binding: `SKSurface.Create` cannot wrap a Vulkan swapchain
image (returns null even with a valid render target; mono/SkiaSharp #1502), and the Vulkan
image-layout interop is unbound (#2191). This holds on the newest SkiaSharp
(`4.147.0-preview.3.1`). The path therefore degrades safely to `OffscreenReadback`. See
`audit/present-path-audit.md` and `audit/opengl-backend-resolution.md`.

## Failure diagnostics

- A missing required evidence artifact fails `Route --enforce` (it names the artifact + tier).
- A direct-path wrap/init failure degrades to the proven readback path with an actionable
  `Warning` (FR-005) — never a crash or corrupt frame.
- A non-byte-identical default-mode render would surface in the standing Scene-parity goldens
  under `Dev`; the two present modes' captures are byte-identical here.
- A race-like or unknown-concurrent-FAKE failure is rerun sequentially before any
  product-regression classification (shared `.fake` state).

# Runtime Limitations — Feature 055

This feature is a governance-internal validator refactor with **no runtime
behavior change**; the standard FS.Skia.UI runtime limitations still apply and
are restated here for the readiness contract.

- **.NET 10 desktop** — the runtime targets .NET 10 desktop hosts (Windows and
  Linux). This feature changes only `FS.Skia.UI.Build` governance logic.
- **Vulkan** — GPU presentation goes through a Vulkan-backed swapchain; no
  software-renderer path is provided.
- **SkiaSharp preview** — rendering uses a SkiaSharp preview package pin; the
  preview surface is the only supported drawing path.
- **unsupported macOS/mobile/browser** — macOS, mobile (iOS/Android), and
  browser/WASM targets are unsupported.
- **no software-renderer fallback** — there is no software-renderer fallback; a
  host without a working Vulkan device cannot present.

None of these limitations are exercised or altered by feature 055.

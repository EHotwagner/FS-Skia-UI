# Runtime Limitations — Feature 057

This feature is a **structural governance refactor** (single-sourcing the
duplicated corpus) with **no runtime behavior change**; the standard FS.Skia.UI
runtime limitations still apply and are restated here for the readiness contract.

- **.NET 10 desktop** — the runtime targets .NET 10 desktop hosts (Windows and
  Linux). This feature edits only the compiled governance front-end
  (`build/Governance/**`) and governance prose under `.agents`/`.specify`/
  `template`/`src/Controls/skill`.
- **Vulkan** — GPU presentation goes through a Vulkan-backed swapchain; no
  software-renderer path is provided.
- **SkiaSharp preview** — rendering uses a SkiaSharp preview package pin; the
  preview surface is the only supported drawing path.
- **unsupported macOS/mobile/browser** — macOS, mobile (iOS/Android), and
  browser/WASM targets are unsupported.
- **no software-renderer fallback** — there is no software-renderer fallback; a
  host without a working Vulkan device cannot present.

None of these limitations are exercised or altered by feature 057: governance
generation and currency-checking stay pure file-scan + file-generation over the
repository corpus.

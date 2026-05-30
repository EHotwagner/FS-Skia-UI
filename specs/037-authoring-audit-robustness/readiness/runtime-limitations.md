# Runtime Limitations (T005)

Records the runtime envelope assumed by this feature's evidence (notably the
US4 generated FSI load script and US3 mixed-open compile). These are platform
constraints, not defects.

## Supported runtime

- **.NET 10 desktop** (`net10.0`) on Windows and Linux. The governance tooling
  and FSI evidence run on the Linux dev container.
- **Vulkan** is the rendering backend for the on-host (non-headless) path.
- **SkiaSharp preview** — SkiaSharp is consumed at a preview package level; its
  API surface may shift between previews and is pinned in
  `Directory.Packages.props`.

## Unsupported

This feature is **unsupported macOS/mobile/browser**: desktop macOS rendering,
mobile, and browser/WASM targets are all out of scope.


- **macOS** desktop rendering — not a target platform for this feature.
- **Mobile** (iOS/Android) — out of scope.
- **Browser / WASM** — out of scope.
- **No software-renderer fallback.** When a host lacks a usable Vulkan surface
  (e.g. a headless CI container), rendering does not silently fall back to a
  software rasterizer. The path instead surfaces benign host warnings (per the
  spec 021 host-warning contract) while load/first-frame succeed, and surfaces a
  real `RenderingFailure` / `LaunchFailure` when they do not.

## Impact on this feature's evidence

- The US4 FSI load script loads the generated app's assemblies. In an
  unsupported headless host, benign host warnings (e.g. GTK module load
  failures) remain classified **benign** when load and first frame succeed; real
  failures stay fatal and are never suppressed by the load script.
- The US3 mixed-open compile is a pure compile check and does not require a
  rendering host.

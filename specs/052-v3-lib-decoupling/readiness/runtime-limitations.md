# Runtime limitations

command: `./fake.sh build -t Dev` (build host) + `./fake.sh build -t PerPackageSurfaceDiff`
scanned files: `src/Input/**`, `src/Lib/**`, `tests/**`, `readiness/surface-baselines/**`.
observed: the rich input runtime relocated `src/Lib` → the new `FS.Skia.UI.Input` package with a pure
namespace rename; behaviour preserved (migrated suite green).
failure class: RuntimeLimitation.
next action: none — this is a package-boundary move, not a runtime-behaviour change.

- This is a **.NET 10 desktop** build-host change. The relocated `FS.Skia.UI.Input` runtime couples to
  the **Vulkan**/Skia host (`FS.Skia.UI.SkiaViewer.Host`) exactly as before; no host, **Vulkan**, or
  **SkiaSharp preview** rendering behaviour changes — only the package that owns the input runtime.
- Targets remain **unsupported macOS/mobile/browser**; this feature does not change platform support.
- The deterministic scene-output parity oracle is headless and re-derives byte-identically to the
  Stage-0 golden. Reference-frame re-capture stays headless-GPU-infeasible (disclosed corroboration,
  not synthetic): there is **no software-renderer fallback** for the persistent Vulkan host in CI, so
  scene-output is the authoritative oracle.

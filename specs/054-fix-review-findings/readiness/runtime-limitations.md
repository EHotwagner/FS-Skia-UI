# Runtime Limitations — Feature 054

This feature touches only build-tooling/governance and the generated template's
engine pin. No runtime/graphics surface changes. The standing runtime limitations
of the framework are unchanged and restated here for the readiness contract:

- Targets **.NET 10 desktop** only.
- Rendering requires a **Vulkan**-capable backend.
- Uses **SkiaSharp preview** packages.
- **unsupported macOS/mobile/browser** — desktop Windows/Linux only.
- **no software-renderer fallback** — a usable GPU/Vulkan backend is required;
  there is no CPU/software rasterizer path.

These constraints are not altered by feature 054 (the nullness cleanup, the
template `#r` pin alignment, and the `.gitignore`/scratch removal are all
behaviour-preserving and non-runtime).

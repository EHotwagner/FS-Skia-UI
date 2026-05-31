# Runtime Limitations (T005)

Context for the generated consumer project's runtime envelope. None of this
feature's changes alter these limits; they are recorded so guidance and evidence
stay honest.

- **.NET 10 desktop only.** Generated products target `net10.0` desktop. The
  viewer host is a desktop window host.
- **Vulkan backend.** The viewer's default renderer path targets Vulkan via
  Silk.NET; `ViewerBackendPreference` enumerates `DefaultBackend`/`Vulkan`/
  `OpenGL`/`Software` but the supported desktop path is Vulkan-first.
- **SkiaSharp preview.** Rendering uses a preview SkiaSharp build; treat its
  surface as preview-pinned.
- **Unsupported targets** — unsupported macOS/mobile/browser: macOS, mobile
  (iOS/Android), and browser/WASM hosts are **not** supported runtime targets for
  the generated viewer.
- **No software-renderer fallback.** There is no guaranteed software raster
  fallback; when the desktop session/GPU path is unavailable the viewer reports
  an `UnsupportedEnvironment` / `WindowVisibility` failure class rather than
  silently rendering headless. Evidence must classify such cases as unsupported,
  not as readable layout proof.

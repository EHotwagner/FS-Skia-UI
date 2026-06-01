# Runtime Limitations — Feature 042

This feature is build-tooling and process only. It does **not** touch the runtime
(`Scene → SkiaViewer → Elmish`), the declarative boundary, or any visual surface.

- **.NET 10 desktop**: the host/runtime stack is unchanged; no `.NET` runtime,
  Vulkan, or SkiaSharp code path is added or modified by this feature.
- **Vulkan + SkiaSharp preview**: the rendering stack (Vulkan presenter + SkiaSharp
  preview) is not exercised — no rendering, screenshots, or GPU work.
- **unsupported macOS/mobile/browser**: desktop only; this feature changes none of
  that. There is **no software-renderer fallback** — build tooling is console-only
  and imposes no such requirement.
- **Headless CI**: the documented 039 headless `SkiaViewer.Tests` / `FsiTranscripts`
  libdecor-gtk flakes remain pre-existing environment limitations; they are
  unrelated to the `Route` selector and are reran focused when they trip.

The only new code is the pure compiled `Routing`/`ContractView` build-tooling
modules and the `Route` FAKE target; all I/O (git union-diff, `File.Exists`,
printing) stays at the `build.fsx` interpreter edge.

# Runtime Limitations — Feature 043

This feature is build-tooling only. It does **not** touch the runtime
(`Scene → SkiaViewer → Elmish`), the declarative boundary, or any visual surface.
`git diff --stat` over product `src/**` is **0** (see `logs/runtime-untouched.md`).

- **.NET 10 desktop**: the host/runtime stack is unchanged; no `.NET` runtime,
  Vulkan, or SkiaSharp code path is added or modified by this feature.
- **Vulkan + SkiaSharp preview**: the rendering stack (Vulkan presenter + SkiaSharp
  preview) is not exercised — no rendering, screenshots, or GPU work. The evidence
  engine emits only governance text artifacts (JSON/Markdown).
- **unsupported macOS/mobile/browser**: desktop only; this feature changes none of
  that. There is **no software-renderer fallback** — the evidence engine is
  console/library-only and imposes no such requirement.
- **Headless CI**: the documented 039 headless `SkiaViewer.Tests` / `FsiTranscripts`
  libdecor-gtk flakes remain pre-existing environment limitations; they are
  unrelated to the evidence engine and are rerun focused when they trip.

The only new code is the ten pure compiled `FS.Skia.UI.Build.Evidence` modules and
the two rewired `build.fsx` gate arms; all I/O (file reads, the `git` diff,
artifact writes) stays at the `build.fsx` interpreter edge (Principle IV).

# Runtime Limitations — Feature 044

This feature is build-tooling only. It does **not** touch the runtime
(`Scene → SkiaViewer → Elmish`), the declarative boundary, or any visual surface.
`git diff --stat` over product `src/**` is **0** (see `logs/runtime-untouched.md`).

- **.NET 10 desktop**: the host/runtime stack is unchanged; no `.NET` runtime, Vulkan,
  or SkiaSharp code path is added or modified by this feature.
- **Vulkan + SkiaSharp preview**: the rendering stack (Vulkan presenter + SkiaSharp
  preview) is not exercised — no rendering, screenshots, or GPU work. The generation
  modules emit only governance text artifacts (Markdown/manifest/template splices).
- **unsupported macOS/mobile/browser**: desktop only; this feature changes none of that.
  There is **no software-renderer fallback** — the generation path is console/library
  only and imposes no such requirement.
- **Headless CI**: the documented 039 headless `SkiaViewer.Tests` / `FsiTranscripts`
  libdecor-gtk flakes remain pre-existing environment limitations; they are unrelated to
  this feature and are rerun focused when they trip.

The only new code is the three pure compiled `FS.Skia.UI.Build` governance modules
(`SkillTreeGen`, `SkillistView`, `ConstitutionFragments`) and the reframed/retired
`build.fsx` gate arms; all I/O (tree enumeration, file reads, the splice writes) stays at
the `build.fsx` interpreter edge (Principle IV). No `FSharp.Compiler.*`, no
`diff`/`cmp`/`sha256sum`/symlink shelling (in-process copy-generation only).

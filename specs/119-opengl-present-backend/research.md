# Phase 0 Research: OpenGL Present Backend

All unknowns from Technical Context are resolved below. Decision / Rationale / Alternatives.

## R1 — SkiaSharp GL framebuffer-wrap interop is available and direct

- **Decision**: Use SkiaSharp's GL backend to wrap the window's default framebuffer (FBO 0):
  `GRGlInterface.Create()` → `GRContext.CreateGl(interface)` → build a `GRBackendRenderTarget`
  from `GRGlFramebufferInfo { FramebufferObjectId = 0u; Format = GL_RGBA8 (0x8058) }` sized to
  the framebuffer → `SKSurface.Create(grContext, backendRenderTarget, GRSurfaceOrigin.BottomLeft,
  SKColorType.Rgba8888)` → draw the existing scene via `SceneRenderer` → `surface.Canvas.Flush()`
  / `grContext.Flush()` → toolkit `SwapBuffers`.
- **Rationale**: This is the exact operation that returns `null` on Vulkan
  ([#1502](https://github.com/mono/SkiaSharp/issues/1502)) but **works** on GL — confirmed by
  feature 118's audit (`present-path-audit.md` §4) and the standard `SKGLView` / MAUI
  `GpuRenderingEngine` / Avalonia-GL pattern. No CPU readback, no staging buffer, no command
  pool, no queue stall.
- **Alternatives considered**: (a) stay on Vulkan with per-frame readback — rejected, that is
  exactly the ~120× slow path feature 118 documented as structurally untunable; (b) Vulkan
  blit-from-offscreen — rejected, blocked by #2191 (no `GRVkImageInfo` handle exposed); (c) wait
  for a newer SkiaSharp — rejected, we are already on the newest `4.147.0-preview.3.1` and the
  gap predates it by years.

## R2 — Silk.NET OpenGL context + windowing

- **Decision**: Keep `Silk.NET.Windowing` + `.Windowing.Extensions` + `Silk.NET.Input`; create
  the window with `GraphicsAPI` set to OpenGL (request a core/compat profile sufficient for
  Skia's GL backend, default version), obtain the `GL` API via `GL.GetApi(window)`, and let the
  toolkit own `SwapBuffers` (`window.SwapBuffers()` / driven by the render callback). Add
  `Silk.NET.OpenGL` `2.23.0` (same train as the existing Silk.NET packages); drop
  `Silk.NET.Vulkan` + `Silk.NET.Vulkan.Extensions.KHR`.
- **Rationale**: Silk.NET windowing already supports GL contexts on Windows (WGL) and Linux
  (GLX/EGL incl. Wayland); the single-threaded render loop suits GL's thread-affine context, so
  no threading-model change. Same Silk.NET version train avoids a transitive-version conflict.
- **Alternatives considered**: a new windowing dependency (OpenTK/GLFW direct) — rejected, the
  spec assumption is to reuse Silk.NET and minimize dependency churn.

## R3 — `ViewerPresentMode` re-mapping onto GL (FR-007)

- **Decision**: **Retain** the public `ViewerPresentMode` DU (`OffscreenReadback` |
  `DirectToSwapchain`). Re-map semantics: on GL, `DirectToSwapchain` means *render onto the FBO-0
  surface and SwapBuffers* — now genuinely readback-free — and becomes the **default** live
  present path; `OffscreenReadback` means *render to an offscreen Skia surface then read pixels*,
  retained as the evidence/screenshot routine and the explicit fallback. Re-write the DU's
  XML-doc comments to describe GL semantics; do not rename the cases (keeps the seam
  source-stable for consumers, resolving the FR-007 vs FR-009 tension in the spec).
- **Rationale**: The spec's conflict-resolution note prefers retaining the present-mode contract
  over removing it. The case names remain meaningful: "direct to swapchain" generalizes to
  "direct framebuffer present"; "offscreen readback" is unchanged in mechanism.
- **Alternatives considered**: add a third `DirectFramebuffer` case — rejected, unnecessary
  surface growth; the two existing cases cover direct-vs-readback cleanly. Removing
  `DirectToSwapchain` — rejected, breaks the 118 seam consumers.

## R4 — Default present mode change is safe

- **Decision**: With GL, `DirectToSwapchain` (direct framebuffer present) is correct and fast on
  all supported targets, so it becomes the default. `ViewerOptions.PresentMode` retains its field
  and type; the *default value* the host applies flips to direct. Document this in `migration.md`
  and the `PresentMode` doc-comment as an intentional, beneficial behavior change (FR-002 allows
  documented intentional deviations).
- **Rationale**: On Vulkan the default was `OffscreenReadback` because direct present was
  infeasible; on GL there is no reason to default to the slow path. Visual output is identical
  (same `SceneRenderer`), so the change is performance-only.
- **Alternatives considered**: keep `OffscreenReadback` default for caution — rejected, it would
  ship the feature without delivering its own headline (SC-001) by default.

## R5 — Resize, context-loss, high-DPI (edge cases)

- **Decision**: On the window resize/framebuffer-resize callback, dispose and recreate the
  `GRBackendRenderTarget` + `SKSurface` at the new framebuffer pixel size (FBO 0 is implicit, so
  only the render target wrapper is recreated — no GPU resource leak). For high-DPI / fractional
  scaling (Wayland), size the render target from the **framebuffer** pixel size, not logical
  window units, so presented pixels match scene intent. For GL context loss / driver reset:
  detect the failed context, emit a classified blocking diagnostic, and fail honestly rather than
  wedging the loop (no silent retry storm); recovery (recreate context) is best-effort and
  documented.
- **Rationale**: FR-006 + the edge-case list require leak-free resize and defined context-loss
  behavior; Principle VII forbids silent failure.
- **Alternatives considered**: caching/reusing the surface across resize — rejected, the render
  target is bound to a specific size; recreate is the correct, leak-free pattern.

## R6 — Unsupported-GL classification (FR-005 / SC-004)

- **Decision**: When `GRGlInterface.Create()` returns null, `GRContext.CreateGl` fails, or the
  window cannot obtain a GL context (headless, no GPU passthrough, broken driver), classify as
  `UnsupportedEnvironment` (benign environment-limitation) — the GL analogue of today's
  Vulkan-missing classification — and emit a structured diagnostic naming GL context creation as
  the failed stage. A genuine post-context implementation error stays a **blocking** defect. Reuse
  the existing benign/blocking host-warning classifier (`fs-skia-evidence-mode`). Never report
  false success; never crash unclassified.
- **Rationale**: The constitution requires presentation failures to distinguish environment
  limitation from defect; CI/headless must stay honest. Mirrors the established Vulkan path so
  the classification machinery is reused, not reinvented.
- **Alternatives considered**: a software-renderer fallback — rejected, "no software-renderer
  fallback" is an explicit runtime-limitation token and out of scope.

## R7 — Governance-token churn is single-sourced and currency-checked (FR-010)

- **Decision**: Flip the canonical token from `Vulkan` → `OpenGL` in **one** place:
  `build/Governance/Evidence/EvidenceFormatSchema.fs` `readinessContractChecks` (the
  `runtime-limitations.md` row). This both (a) re-enforces *this* feature's
  `runtime-limitations.md` to require `OpenGL`, and (b) regenerates `evidence-formats.md`
  (single-sourced reference), caught by `GeneratedGuidanceCheck`. Also update the generated-product
  seed (`GeneratedProduct.fs:970`), the constitution constraint fragment (`GovernedBlocks.fs:267`,
  which the constitution amendment must match), `build/Governance/README.md`, the
  `tests/Governance.Tests/*` assertions, and the docs/ADR/architecture/report prose. The
  `.claude` skill peers regenerate from `.agents` via `RefreshSurfaceBaselines`.
- **Rationale**: The scan (`Scans.readinessContract`) runs only over the **active feature's**
  readiness directory, so flipping the required token does **not** retroactively break archived
  features' `runtime-limitations.md`; it only obligates 119 (and future) to declare `OpenGL`.
  Confirmed `scripts/dependency-report.fsx` is generic CPM-conformance (no hard-coded Silk.NET
  expected-ref list to edit).
- **Alternatives considered**: leave Vulkan as the required token — rejected, FR-010/SC-006
  forbid governance asserting a backend that no longer exists; a hand-synced reference — rejected,
  governance is generated-from-single-source by mandate (CLAUDE.md).

## R8 — Historical harness `.fsproj`s referencing Silk.NET.Vulkan

- **Decision**: Leave the two archived readiness-harness projects
  (`specs/085-.../InteractiveHostEvidence.fsproj`, `specs/086-.../...`) untouched. They are
  historical evidence harnesses, not packable libs, and are not rebuilt by the routed gate set.
- **Rationale**: Editing archived readiness would falsify historical provenance; the dependency
  swap is scoped to the live host (`src/SkiaViewer`). Verify in implementation that
  `DependencyReport` does not flag them as in-scope; if it does, scope them out explicitly.
- **Alternatives considered**: delete/retarget the harnesses — rejected, out of scope and
  history-altering.

## Open risks carried into implementation (not unknowns)

- **GL profile/version selection** vs. Skia's GL backend expectations — validate the actual
  context profile during the first real launch; adjust the Silk.NET `GraphicsAPI` request if
  Skia needs a specific profile. (Resolved empirically on the GPU-passthrough machine.)
- **Surface-origin / Y-flip**: GL is bottom-left origin; use `GRSurfaceOrigin.BottomLeft` so the
  `SceneRenderer` output is not vertically flipped — verify against the visual baseline (SC-002).

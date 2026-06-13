# Phase 0 Research: Backend and Host Mode Review (Feature 118)

All NEEDS CLARIFICATION resolved. Findings are grounded in the present-path audit of
`src/SkiaViewer/Host/Vulkan.fs` and the public surface in `src/SkiaViewer/SkiaViewer.fsi`.

## Audit baseline (what exists today)

The single present path, `renderFrame` (Vulkan.fs:1078), is:

1. `getSwapchainImages` (Vulkan.fs:1081) → `acquireImage` (`vkAcquireNextImageKHR`,
   Vulkan.fs:771) → `image = images[imageIndex]`.
2. `renderSceneToPixels configuration skiaState extent colorType scene` (Vulkan.fs:1096,
   defined :904): creates an **offscreen** `SKSurface.Create(context, true, imageInfo, 1,
   TopLeft)` (:910), draws, flush/submit, then `surface.ReadPixels(...)` — the **GPU→CPU
   readback** (:934).
3. `copyPixelsToSwapchainImage` (Vulkan.fs:1097, defined :945): per-frame
   `createStagingBuffer` (:946), per-frame `vkCreateCommandPool` (:954) + command buffer,
   image-layout transitions, copy, `vkQueueSubmit`, then **`vkQueueWaitIdle`** (:1054,
   full-pipeline stall) and **`vkQueuePresentKHR`** (`swapchainExt.QueuePresent`, :1067).

The **same** `renderSceneToPixels` routine is the evidence/screenshot readback routine —
so ordinary live frames pay evidence-mode cost (the spec's central finding). No
`GRBackendRenderTarget` / `GRVkImageInfo` / direct-to-swapchain rendering exists anywhere
today.

---

## R1 — Direct-to-swapchain Skia/Vulkan wrap mechanics

**Decision.** Render the scene directly onto the acquired swapchain image by wrapping it
in a Skia backend render target:

- Build a `GRVkImageInfo` from the swapchain image: `Image` = the `VkImage` handle,
  `Format` = the swapchain `VkFormat` (already computed as `swapchainState.Format`,
  mapped to a Skia `colorType` by `colorTypeForFormat`, Vulkan.fs:1092), `ImageTiling =
  Optimal`, `ImageLayout` = the image's current layout, `LevelCount = 1`, sample count 1
  (matches the existing offscreen surface's sample count 1, FR / edge "color-type /
  sample-count match").
- `use rt = new GRBackendRenderTarget(width, height, sampleCount=1, GRVkImageInfo)`.
- `use surface = SKSurface.Create(skiaState.Context, rt, GRSurfaceOrigin.TopLeft,
  colorType)` → `surface.Canvas.Clear`; `drawScene scene surface.Canvas` (the **same**
  `drawScene` the offscreen path uses, so the rendered scene is identical, FR-003);
  `surface.Flush()` / `context.Flush()`.
- Hand the image to the presentation engine in `PRESENT_SRC_KHR` layout. Skia's
  `GRBackendSurfaceMutableState` / the flush-with-target-layout API lets Skia perform the
  final layout transition during flush, so the present-time layout barrier is owned by
  Skia rather than a hand-rolled per-frame command pool. Acquire/present synchronization
  uses the swapchain's semaphores (the existing `acquireImage` fence/semaphore seam),
  **not** a per-frame `vkQueueWaitIdle`.

**Result vs. acceptance criteria.** No `ReadPixels` (no GPU→CPU readback), no per-frame
`createStagingBuffer` / `vkCreateCommandPool` (Skia reuses its own command resources), no
per-frame `vkQueueWaitIdle` full stall (SC-002, FR-002).

**Rationale.** This is the canonical SkiaSharp+Vulkan windowed-present pattern and the
only way to satisfy "no readback in ordinary frames" without a compositor rewrite
(explicitly deferred, FR-010). The scene is drawn by the same `drawScene`, so visual
output is identical and provable by on-demand screenshot equivalence (FR-003) rather than
goldens (the headless `Perf.runScript` driver has no backend, FR-008).

**Alternatives considered.** (a) Keep readback but skip only `vkQueueWaitIdle` — rejected:
readback + staging upload remain, fails "no accidental readback." (b) Blit offscreen
surface → swapchain on-GPU (`vkCmdBlitImage`) without CPU readback — removes the readback
but keeps a per-frame command pool and an extra full-surface copy; strictly worse than
direct render and still not the report's intent. (c) Compositor/layer split — deferred by
FR-010.

---

## R2 — Per-swapchain-image render targets + recreation on resize

**Decision.** Cache one `GRBackendRenderTarget` (and optionally its `SKSurface`) **per
swapchain image index** on `SwapchainState`, keyed by image index. `renderFrame` selects
the cached target for the acquired `imageIndex` (the spec's "wrap/track a backend render
target per swapchain image index, not assume a single image"). On swapchain recreation
(resize / minimize / device-lost recovery — the existing `createSwapchain`, Vulkan.fs:627,
and resize handling), dispose and rebuild the per-image targets alongside the new
swapchain images (FR-006). The readback path keeps its existing resize handling unchanged.

**Rationale.** Swapchain images are fixed for a swapchain's lifetime and re-presented in a
ring; building the wrap once per image (not per frame) is what removes per-frame
allocation. Tying recreation to the existing swapchain-recreation seam keeps a single
lifecycle owner.

**Alternatives considered.** Rebuild the wrap every frame — rejected: reintroduces
per-frame allocation the feature exists to remove. Assume a single image — rejected:
incorrect for the common `MinImageCount + 1` (Vulkan.fs:659) double/triple-buffered case.

---

## R3 — FR-007 live diagnostic with `Category = Swapchain | Frame`

**Decision.** Emit the present-mode / readback fact as a `ViewerDiagnosticEvent` whose
`Category` is `Swapchain` (or `Frame`). **Plumbing gap to fix:** today every backend
`RenderDiagnostic` flows backend → `Host.ViewerEvent.DiagnosticReported` →
`LegacyDiagnosticReported` (SkiaViewer.fs:1281) and is published with `Category` **hardcoded
to `Renderer`** (SkiaViewer.fs:1290). Resolve by carrying a category from the backend for
this diagnostic — the lowest-risk option is to map the internal `RenderDiagnostic.Stage`
(`Diagnostics.fsi` `DiagnosticStage`: `VulkanSwapchain`, `FrameRender`, …) to the public
`ViewerDiagnosticCategory` in the `LegacyDiagnosticReported` arm (`VulkanSwapchain →
Swapchain`, `FrameRender → Frame`, else `Renderer`), or add a dedicated present-mode
diagnostic carrier. The present-mode/readback diagnostic is emitted on the live present
path only (it never reaches the headless `Perf.runScript` metric path), so it is
**live-only and non-golden** by construction (FR-007).

**Risk check.** Mapping `Stage → Category` would change the published category of *existing*
swapchain/frame-stage diagnostics from `Renderer` to `Swapchain`/`Frame`. Verify no
existing test or window-state classification keys off `Category = Renderer` for those
stages before choosing the broad mapping; if any does, prefer the dedicated-carrier option
so existing diagnostics keep `Renderer`. Decide in Phase 1/implementation against the test
suite (failing-first category test).

**Rationale.** Reuses the existing consumer-facing `ViewerDiagnosticsOptions.Sink`
channel (no new public diagnostic plumbing), and keeps the live-timing vs.
deterministic-counts separation the report mandates.

**Alternatives considered.** A new public diagnostic type/channel — rejected as
over-engineering; the existing event already carries `Category`, `Level`, `Message`,
`Elapsed`. A `FrameMetrics` field — **forbidden** by FR-008 (headless driver, permanently
zero/absent → misleading).

---

## R4 — Safe fallback + color-type / sample-count match (FR-005)

**Decision.** Wrap direct-path setup (image-info build, `GRBackendRenderTarget`
construction, `SKSurface.Create`) in a guarded attempt. On any failure — unsupported
swapchain format/color type, Skia/Vulkan interop failure, a driver that refuses the wrap,
or a color-type/sample-count mismatch — fall back to the proven
`renderSceneToPixels`+`copyPixelsToSwapchainImage` readback path **for that frame onward**,
emit a `Warning` diagnostic with the cause, and continue presenting. A direct-path failure
MUST NOT crash or present a corrupt/garbage frame (SC-005). The direct render target's
format and sample count MUST match the swapchain image's (sample count 1, matching the
current offscreen surface); a mismatch is an init failure handled by this fallback.

**Rationale.** Safety wins over performance (spec's conflict resolution). The readback path
is the already-proven default; degrading to it is the honest, observable failure mode
required by Principle VII.

**Alternatives considered.** Hard-fail the viewer on direct-path init failure — rejected:
violates FR-005 and the constitution's safe-degradation requirement. Silently retry direct
mode every frame — rejected: risks repeated stalls; once direct init fails, prefer staying
on the fallback for the swapchain's lifetime (re-attempt is a natural swapchain-recreation
boundary).

---

## R5 — Construction-site churn + optional smart constructor

**Decision.** Adding `PresentMode` to the public `ViewerOptions` record is a breaking
record-shape change: every literal construction site must add
`PresentMode = ViewerPresentMode.OffscreenReadback` (preserving byte-identity, FR-001).
Inventory (from the repo-wide scan): `template/base/src/Product/EvidenceCommands.fs` (×2);
`samples/{BasicViewer,EffectsGallery,ParityGallery,DemoReel}/Program.fs`;
`tests/SkiaViewer.Tests/Tests.fs` + the `Feature0xx*Tests.fs` files (~30 literals);
`tests/Elmish.Tests/Tests.fs`; `tests/ControlsPreview.Harness/PreviewRender.fs`;
`specs/085*/086*/090*` readiness harnesses and `.fsx` preludes; the internal "Generated
App" literal at `SkiaViewer.fs:~2899`. `RefreshSurfaceBaselines` Build and the sample/FSI
compile catch any missed site. `with`-expression sites that derive from a base options
value need **no** edit.

Do **not** add a smart-constructor / `defaultOptions` to dodge the churn: the public record
is constructed directly by consumers and the template, the default field value already
preserves behavior, and introducing a parallel construction path would expand surface for
no benefit. Keep the record, make the new field default-bearing at every site.

**Rationale.** The default value is the entire byte-identity guarantee (FR-001); the
mechanical add at each site is low-risk and compiler-enforced. The template/generated
sites are exactly why Route escalates to `TemplateCheck` / `GeneratedProductCheck`.

**Alternatives considered.** A separate options-builder API — rejected (new surface,
FR-001 already satisfied by the default). Making `PresentMode` optional (`option`) — rejected:
muddies the default and complicates the backend branch; a closed DU with a default case is
clearer.

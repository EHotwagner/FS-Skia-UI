# Phase 1 Data Model: Backend and Host Mode Review (Feature 118)

Entities are render-backend configuration and internal backend state. There is **no new
durable workflow state, `Msg`, or `Effect`** — `PresentMode` is configuration carried in
the existing `ViewerModel.Options` (`Options: ViewerOptions`). The mechanism and fallback
live in the backend interpreter (`Vulkan.fs run`/`renderFrame`).

## 1. `ViewerPresentMode` (new public DU)

Closed, `[<RequireQualifiedAccess>]` discriminated union — the present-mechanism selector.

| Case | Meaning |
|------|---------|
| `OffscreenReadback` | Today's path: offscreen `SKSurface` → `ReadPixels` → per-frame staging buffer/command pool → `vkQueueWaitIdle` → present. **Default.** Byte-identical to the pre-feature baseline. |
| `DirectToSwapchain` | Opt-in: render the Skia scene directly onto the acquired swapchain image via `GRBackendRenderTarget`/`GRVkImageInfo`. No per-frame readback, no per-frame staging buffer/command-pool, no per-frame `vkQueueWaitIdle`. |

Validation / invariants:
- Closed DU; no `Other`/escape case. Adding a third mode later is a deliberate surface change.
- `OffscreenReadback` is the **only** value any default-constructed `ViewerOptions` carries.
- Headless (`Perf.runScript`): no backend exists; the value is inert (never consulted).

## 2. `ViewerOptions` (public record — field addition)

Before: `{ Title: string; InitialSize: Size }`.
After: `{ Title: string; InitialSize: Size; PresentMode: ViewerPresentMode }`.

| Field | Type | Notes |
|-------|------|-------|
| `Title` | `string` | unchanged |
| `InitialSize` | `Size` | unchanged |
| `PresentMode` | `ViewerPresentMode` | **new.** Default value at every construction site = `ViewerPresentMode.OffscreenReadback`. Breaking record-shape change → all literals updated (see plan/research R5). |

Invariant: with `PresentMode = OffscreenReadback`, the present path, screenshots, window
diagnostics, and visual output are byte-identical to the pre-feature baseline (FR-001/SC-001).

## 3. `ViewerConfiguration` (internal record — field addition, `Host/Diagnostics.fsi`)

Before: `{ Title; InitialSize; ClearColor; TargetFrameRate; Diagnostics; ConfigureWindow }`.
Add: `PresentMode: ViewerPresentMode`.

- Threaded from `ViewerOptions.PresentMode` in `Host.Viewer.defaultConfiguration`
  (Viewer.fs:10) / the config-build site (`SkiaViewer.fs:~1231`).
- `renderFrame configuration …` branches on `configuration.PresentMode`.
- `[<NoEquality; NoComparison>]` already on `ViewerConfiguration`; unchanged.

## 4. Per-swapchain-image render-target cache (internal backend state)

Lives on `SwapchainState` (Vulkan.fs), populated only when `PresentMode = DirectToSwapchain`.

| Element | Shape | Lifecycle |
|---------|-------|-----------|
| direct targets | per-image `GRBackendRenderTarget` (+ optional `SKSurface`), indexed by swapchain image index | Built once on swapchain (re)creation; selected by acquired `imageIndex` each frame; disposed on swapchain recreation (resize/recovery, FR-006) and shutdown. |
| direct-mode availability | a flag/`bool` recording whether the direct path successfully initialized | Set false on first init failure → frames use the readback fallback (FR-005); re-evaluated at the swapchain-recreation boundary. |

Invariants:
- Target count == swapchain image count (`MinImageCount + 1`, clamped; Vulkan.fs:659). No
  single-image assumption.
- Each target's format == swapchain `VkFormat`; sample count == 1 (matches the offscreen
  surface). Mismatch ⇒ init failure ⇒ readback fallback (FR-005, edge "color-type/sample-count match").

## 5. Present-mode live diagnostic (transient, non-golden)

A `ViewerDiagnosticEvent` emitted on the live present path only:

| Field | Value |
|-------|-------|
| `Level` | `Info` (mode report) / `Warning` (FR-005 direct→readback fallback) |
| `Category` | `Swapchain` (or `Frame`) — **not** `Renderer` (see research R3 plumbing fix) |
| `Message` | active present mode + whether ordinary frames read back (optionally present/readback timing) |
| `FrameIndex` / `Elapsed` | optional live timing (human/diagnostic signal only, FR-011) |

Invariants:
- Never enters the `Perf.runScript` deterministic metric path → excluded from goldens (FR-007).
- **No** `FrameMetrics` field added (FR-008): `FrameMetrics` is produced by the headless
  driver with no backend; a backend field would be permanently zero/absent and misleading.

## State flow (per ordinary live frame)

```
acquireImage → imageIndex
  match configuration.PresentMode with
  | OffscreenReadback ->            // default, unchanged
      renderSceneToPixels → ReadPixels → copyPixelsToSwapchainImage (staging+pool+WaitIdle) → present
  | DirectToSwapchain when directInitOk ->
      select per-image GRBackendRenderTarget → SKSurface.Create(rt) → drawScene → flush(present layout) → present
      // no readback, no per-frame staging/pool, no vkQueueWaitIdle
  | DirectToSwapchain (init failed) ->
      Warning diagnostic (once) → readback fallback path  // FR-005
```

On-demand screenshot / evidence capture is **independent** of this branch: it always uses
the offscreen `renderSceneToPixels` readback routine, invoked only when a capture is
requested (FR-004) — under both present modes.

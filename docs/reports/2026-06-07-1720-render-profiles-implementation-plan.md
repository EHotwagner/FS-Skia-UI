---
title: Render Profiles Implementation Plan
index: 20
description: Implementation plan for two host render profiles — a screenshot/readback profile for development and debugging, and a GPU-direct present profile for release — replacing the current unconditional per-frame GPU→CPU→GPU round trip.
---

# Render Profiles Implementation Plan

- **Timestamp:** 2026-06-07T17:20:00Z
- **Author:** Claude (Opus 4.8)
- **Status:** Implementation plan, not implemented
- **Audience:** Maintainers working on the SkiaViewer Vulkan host
- **Scope:** `src/SkiaViewer/Host/**` (Vulkan present path, host configuration)
- **Reference:** [architecture/host-skiaviewer](https://ehotwagner.github.io/FS-Skia-UI/architecture/host-skiaviewer.html)

## Executive Summary

The SkiaViewer Vulkan host renders every frame to an **offscreen GPU
`SKSurface`**, reads the pixels back into a managed CPU array (`lastFrame`), then
re-uploads those pixels through a Vulkan staging buffer and presents. This
GPU→CPU→GPU round trip is unconditional and is documented as a deliberate
trade: it keeps screenshot/evidence capture trivially available at the cost of a
full readback, a CPU→GPU re-upload, and a `vkQueueWaitIdle` stall **on every
frame**.

This plan introduces **two selectable render profiles**:

1. **`ReadbackCapture`** (development / debugging — current behaviour) — keeps the
   offscreen-render → readback → re-upload path so `lastFrame` always holds the
   most recent pixels and live-window screenshots are free.
2. **`DirectGpu`** (release) — wraps the acquired swapchain image as a Skia
   surface, draws the scene **directly into it**, and presents with proper
   semaphore synchronisation. No per-frame readback, no staging copy, no
   queue-wait-idle stall. Live-window screenshots become an on-demand readback
   instead of a hot-path one.

The profile is a field on `ViewerConfiguration`, defaulted by build configuration
(`#if DEBUG`) and overridable by environment variable, so released apps get the
fast path automatically while development builds keep screenshots free.

## Current Pipeline (verified)

All line numbers are against `src/SkiaViewer/Host/Vulkan.fs` at the time of
writing.

- **Swapchain creation** — `createSwapchain` (line 622). Image usage is hard-coded
  to `ImageUsageFlags.ColorAttachmentBit` only (line 659). `imageUsage` is stored
  on `SwapchainState` (line 683) but never varied.
- **Per-frame loop** — `renderFrame` (line 1073):
  1. `getSwapchainImages` → `createFence` → `acquireImage` (fence-based acquire,
     no semaphores).
  2. `renderSceneToPixels` (line 899): creates an offscreen GPU `SKSurface`
     (`SKSurface.Create(context, true, imageInfo, …)`, line 906), clears, draws
     the scene, flushes/submits (`Submit(true)`), then `surface.ReadPixels(…)`
     into a pinned managed `byte[]` (line 929). **GPU→CPU.**
  3. `copyPixelsToSwapchainImage` (line 940): staging buffer, one-time command
     buffer, barrier `Undefined → TransferDstOptimal`, `vkCmdCopyBufferToImage`
     (line 1011), barrier `TransferDstOptimal → PresentSrcKhr`, `vkQueueSubmit`,
     **`vkQueueWaitIdle` (line 1049)**, `vkQueuePresentKHR`. **CPU→GPU + stall.**
  4. Returns a `FrameSnapshot { Width; Height; ColorType; Pixels }`.
- **Frame snapshot storage** — `lastFrame` mutable (line 1159); set to the new
  snapshot after a successful `RenderFrame` effect (line 1214).
- **Screenshot capture** — `interpretEffect` / `CaptureScreenshot` (line 1222)
  reads `lastFrame.Pixels` and encodes via `encodeSnapshot` (line 1104). If no
  frame has rendered yet it queues into `pendingScreenshots` (line 1229).
- **Configuration** — `ViewerConfiguration` in `Host/Diagnostics.fs` (lines
  10–15): `Title`, `InitialSize`, `ClearColor`, `TargetFrameRate`, `Diagnostics`.
  Public via `Host/Diagnostics.fsi`. Defaulted by `Viewer.defaultConfiguration`.

### Latent issue surfaced by this work

The current copy path writes into the swapchain image with
`vkCmdCopyBufferToImage` at layout `TransferDstOptimal`, but the swapchain is
created with `ColorAttachmentBit` **only** — it is missing `TransferDstBit`.
This is technically a usage-flag violation (it appears to work because validation
layers are off and the driver tolerates it). The `ReadbackCapture` profile
should set `ColorAttachmentBit | TransferDstBit` and thereby **fix this latent
bug** as a side effect.

### What is *not* affected

The headless deterministic-evidence path (`RendererMode` strings such as
`pixel-readback`, `metadata-hash`, `deterministic-scene` in `Scene.fs` and
`SkiaViewer.fs`) does **not** go through the live `VulkanHost` present loop. CI
evidence capture is therefore independent of the host render profile. The render
profile only governs the **live window** present strategy and the live-window
`CaptureScreenshot` effect. Note: `RenderProfile` (host present strategy) and
`RendererMode` (evidence label) are orthogonal — do not conflate them.

## Design

### 1. The profile type and configuration

Add to `Host/Diagnostics.fs` (and `.fsi`):

```fsharp
/// Host present strategy. Governs how a rendered frame reaches the swapchain.
type RenderProfile =
    /// Offscreen render → CPU readback → re-upload → present.
    /// Keeps `lastFrame` populated so live-window screenshots are free.
    /// Default for development/debug builds.
    | ReadbackCapture
    /// Draw directly into the acquired swapchain image and present with
    /// semaphore sync. No per-frame readback. Default for release builds.
    /// Live-window screenshots fall back to an on-demand readback.
    | DirectGpu
```

Extend `ViewerConfiguration`:

```fsharp
type ViewerConfiguration =
    { Title: string
      InitialSize: Size
      ClearColor: Color option
      TargetFrameRate: int option
      RenderProfile: RenderProfile
      Diagnostics: DiagnosticOptions }
```

`RenderProfile` is non-optional with an explicit default chosen in
`Viewer.defaultConfiguration` (`src/SkiaViewer/Host/Viewer.fs`):

```fsharp
let private defaultRenderProfile =
    match Environment.GetEnvironmentVariable "FS_SKIA_RENDER_PROFILE" with
    | "readback" | "ReadbackCapture" -> ReadbackCapture
    | "gpu" | "direct" | "DirectGpu" -> DirectGpu
    | _ ->
#if DEBUG
        ReadbackCapture
#else
        DirectGpu
#endif
```

This gives the requested behaviour by default — debug builds get screenshots,
release builds get the GPU-only fast path — while the env var allows a release
build to be put into readback mode for field debugging and vice versa.

### 2. Swapchain usage flags per profile

`createSwapchain` (line 622) must select usage from the profile and the surface's
`capabilities.SupportedUsageFlags` (already traced at line 636):

```fsharp
let imageUsage =
    match configuration.RenderProfile with
    | ReadbackCapture -> ImageUsageFlags.ColorAttachmentBit ||| ImageUsageFlags.TransferDstBit
    | DirectGpu       -> ImageUsageFlags.ColorAttachmentBit
```

Guard each required flag against `capabilities.SupportedUsageFlags` and emit a
`VulkanSwapchain` diagnostic if unsupported (TransferDst is near-universal but
not guaranteed). `ColorAttachmentBit` is mandated by spec for all swapchains, so
`DirectGpu` is always satisfiable.

### 3. Frame dispatch by profile

Split `renderFrame` (line 1073) into the shared acquire prologue plus two present
strategies. Keep the existing functions for `ReadbackCapture`; add a new path for
`DirectGpu`.

```fsharp
let renderFrame configuration vk swapchainExt physicalDevice device
                swapchainState skiaState queueFamily scene =
    // ... acquire (see synchronisation note below) ...
    match configuration.RenderProfile with
    | ReadbackCapture ->
        // existing: renderSceneToPixels >>= copyPixelsToSwapchainImage,
        // returns a populated FrameSnapshot (Pixels = real bytes).
    | DirectGpu ->
        renderSceneDirect configuration vk skiaState swapchainState
                          image imageIndex colorType scene
        // returns a FrameSnapshot with Pixels = [||] (empty sentinel).
```

`renderSceneDirect` (new) does:

1. Build a `GRVkImageInfo` describing the acquired swapchain `VkImage`
   (image handle, `ImageTiling.Optimal`, current layout, the swapchain
   `Format`, `LevelCount = 1`, sample count 1, the queue family). The swapchain
   image has no `VkDeviceMemory`/`VkAlloc` we own — SkiaSharp's
   `GRVkImageInfo` accepts the externally owned image; we pass `Alloc` as the
   default and let Skia manage layout transitions.
2. `use backendRT = new GRBackendRenderTarget(width, height, sampleCount=1, imageInfo)`.
3. `use surface = SKSurface.Create(skiaState.Context, backendRT, GRSurfaceOrigin.TopLeft, colorType)`.
   Null-check → `FrameRender` diagnostic.
4. `surface.Canvas.Clear clear; drawScene scene surface.Canvas`.
5. Flush **with semaphores** (see next section) so the present queue waits on
   render completion: `surface.Flush(submitContext)` /
   `context.Flush(GRFlushInfo with signal semaphore)` then `context.Submit`.
   Tell Skia the desired final layout is `PresentSrcKhr` via
   `surface.Flush` with a `GRBackendSurfaceMutableState(PresentSrcKhr, queueFamily)`
   so Skia inserts the transition for us — no manual barrier needed.
6. `vkQueuePresentKHR` waiting on the render-finished semaphore.
7. Return `FrameSnapshot { Width; Height; ColorType; Pixels = [||] }` — empty
   pixels signal "no readback available this frame".

### 4. Synchronisation

The current `ReadbackCapture` path is correct but coarse — it acquires with a
**fence** and serialises with `vkQueueWaitIdle`. That is acceptable for the
debug profile (correctness over throughput) and can be left as-is initially.

`DirectGpu` must not stall. Replace per-frame `vkQueueWaitIdle` with:

- An **image-available semaphore** signalled by `vkAcquireNextImageKHR`
  (switch acquire from fence to semaphore on this path).
- A **render-finished semaphore** signalled by Skia's flush (`GRFlushInfo` with
  `SignalSemaphores`), waited on by `vkQueuePresentKHR`.
- A per-frame **in-flight fence** to bound CPU/GPU overlap (start with
  frames-in-flight = 1; this already removes the readback+copy+stall, the big
  win; deeper pipelining is a follow-up).

Skia owns the swapchain image's layout while it holds the backend render target,
so the manual `transitionBarrier` calls in `copyPixelsToSwapchainImage` have no
equivalent on this path — Skia emits the `→ PresentSrcKhr` transition via the
mutable-state flush.

### 5. Screenshots under `DirectGpu`

`lastFrame` will carry empty `Pixels` under `DirectGpu`, so `CaptureScreenshot`
(line 1222) needs a fallback. Two-tier strategy in `interpretEffect`:

```fsharp
| CaptureScreenshot request ->
    match lastFrame with
    | Some snapshot when snapshot.Pixels.Length > 0 ->
        saveScreenshot request snapshot          // ReadbackCapture: free
    | _ ->
        // DirectGpu (or pre-first-frame): do an on-demand offscreen render.
        match renderOnDemandSnapshot () with     // reuse renderSceneToPixels on pendingScene
        | Ok snapshot -> saveScreenshot request snapshot
        | Error d -> ...queue or diagnostic as today
```

`renderOnDemandSnapshot` re-runs the existing `renderSceneToPixels` against the
last `pendingScene`/scene for a single readback frame — paying the round trip
**only when a screenshot is actually requested** instead of every frame. This
preserves screenshot capability in release while keeping the steady-state hot
path readback-free. (If on-demand readback in release is undesirable, the
alternative is to return a clear "screenshots require the ReadbackCapture
profile" diagnostic; recommend keeping on-demand so the capability never
silently disappears.)

### 6. Diagnostics & observability

- Add a `trace` line in `renderFrame` naming the active profile.
- Optionally extend `DiagnosticStage` with no new case (reuse `FrameRender`) to
  avoid a public-surface churn; the profile is already visible via config.
- Surface the active profile in the verbose startup banner so a misconfigured
  release build is diagnosable from logs.

## Files to change

| File | Change | Surface impact |
|------|--------|----------------|
| `src/SkiaViewer/Host/Diagnostics.fs` | Add `RenderProfile` DU; add field to `ViewerConfiguration` | public |
| `src/SkiaViewer/Host/Diagnostics.fsi` | Mirror the above | **public `.fsi` — escalates** |
| `src/SkiaViewer/Host/Viewer.fs` | Default profile (env + `#if DEBUG`) in `defaultConfiguration` | internal |
| `src/SkiaViewer/Host/Viewer.fsi` | Only if a `withRenderProfile` builder is exposed | public if added |
| `src/SkiaViewer/Host/Vulkan.fs` | Profile-aware `createSwapchain` usage; split `renderFrame`; add `renderSceneDirect`; semaphore sync; on-demand screenshot fallback | internal |
| `src/SkiaViewer/Host/Vulkan.fsi` | No change expected (only `VulkanHost.run` is public) | none |

## Routing & validation

Per `AGENTS.md` / `CLAUDE.md`, **run `./fake.sh build -t Route` first** against
the working-tree diff and run only the gates it prints. The `Diagnostics.fsi`
change makes this a **public `src/**/*.fsi` change**, which Routing **escalates**
to the `maintainer-verify` path. Expect to run the serialized six-target order:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

A new public type/field changes the **per-package `.fsi` surface baseline** for
the SkiaViewer package — recapture via `PerPackageSurface.captureCurrent` (this
is *not* regenerated by `RefreshSurfaceBaselines`; see the per-package-baseline
note). Confirm with `./fake.sh build -t Route --enforce` that no escalated
evidence artifact is missing.

## Testing

- **Unit:** `defaultRenderProfile` selection table (env override, debug/release
  default) — pure, fast.
- **Swapchain usage:** assert `imageUsage` includes `TransferDstBit` iff
  `ReadbackCapture`; assert the capability guard rejects unsupported flags.
- **Snapshot sentinel:** `DirectGpu` returns `Pixels = [||]`; `CaptureScreenshot`
  routes to the on-demand path when `Pixels` is empty.
- **Manual / runtime:** launch a sample under each profile; verify the window
  renders identically and a screenshot still writes a correct PNG in both. The
  GPU path cannot be unit-tested headlessly (needs a real device); cover it with
  a runtime smoke check and the existing evidence path for pixel correctness.
- **Parity:** because both profiles draw through the same `SceneRenderer.paintNode`
  (shared painter, feature 063), pixel output must match; an on-demand readback
  in `DirectGpu` can be diffed against a `ReadbackCapture` frame for a one-time
  parity assertion.

## Risks & mitigations

- **SkiaSharp Vulkan backend-RT wrapping of swapchain images is fiddly** —
  `GRVkImageInfo` layout/format/queue-family must match the actual swapchain
  image, or you get corruption or device-lost. *Mitigation:* implement
  `DirectGpu` behind the profile flag with `ReadbackCapture` remaining the
  default-on-debug fallback; validate with Vulkan validation layers enabled for
  this work.
- **No validation layers today** — the latent missing `TransferDstBit` shows the
  project runs without validation. *Mitigation:* enable validation layers
  temporarily while building `DirectGpu`; the `ReadbackCapture` flag fix removes
  the existing violation.
- **Screenshot regression in release** — if the on-demand fallback is dropped,
  release screenshots silently fail. *Mitigation:* keep the on-demand readback;
  cover with a test asserting a PNG is produced under `DirectGpu`.
- **Resize handling** — direct-present must recreate the backend render target on
  swapchain recreation. *Mitigation:* tie backend-RT lifetime to swapchain
  lifetime; recreate together.

## Phased rollout

1. **Config plumbing** — add `RenderProfile`, thread through, default it; keep
   both branches calling the *existing* readback path. No behaviour change.
   (Escalated `.fsi` change; run the six-target order.)
2. **Swapchain usage fix** — profile-aware `imageUsage`; fixes the latent
   `TransferDstBit` violation for `ReadbackCapture`.
3. **`DirectGpu` present path** — `renderSceneDirect` + semaphore sync, behind the
   flag. Validate with layers on.
4. **On-demand screenshot fallback** — wire `CaptureScreenshot` to the empty-pixel
   sentinel.
5. **Flip release default to `DirectGpu`** and document.

## Open questions

1. **On-demand readback vs. unsupported** in release — recommend on-demand so the
   capability is never silently lost. Confirm acceptable.
2. **Frames-in-flight** — start at 1 (already removes the stall) or go straight to
   2–3 for smoother pacing? Recommend 1 first; pipeline depth is an independent
   follow-up.
3. **Profile naming** — `ReadbackCapture` / `DirectGpu` proposed; could also be
   `Debug` / `Release`, but coupling names to build config is misleading once the
   env override exists. Recommend the behaviour-descriptive names.

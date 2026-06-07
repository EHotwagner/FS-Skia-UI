---
title: Host (SkiaViewer)
category: Architecture
categoryindex: 3
index: 1
description: The SkiaViewer host — the Skia-on-Vulkan window host that owns the render loop, the GPU surface, and frame presentation.
---

# Host (SkiaViewer)

`FS.Skia.UI.SkiaViewer` is the framework's **host**: the part that turns a pure
Elmish/MVU program plus a declarative [scene](./scene.html) into pixels on a real
desktop window. It owns the operating-system window, the Vulkan instance/device/
swapchain, the SkiaSharp GPU context, and the frame loop that drives all of them.
Everything above it — your `Model`, `Msg`, `update`, and `view` — is pure data and
pure functions; everything *inside* SkiaViewer is the side-effecting machinery that
the rest of the framework deliberately keeps at arm's length. Per
[ADR 0007](../adr/0007-host-ownership.md), this package owns the host outright: the
Vulkan/Skia modules live here, not in a monolith, and SkiaViewer depends only on
the split packages it needs (`Scene`, `KeyboardInput`) rather than pulling a
whole framework onto every consumer's transitive graph.

See the API reference for the full surface:
[`FS.Skia.UI.SkiaViewer`](../reference/fs-skia-ui-skiaviewer.html),
[`Viewer`](../reference/fs-skia-ui-skiaviewer-viewer.html), and the
[reference index](../reference/index.html).

## What the host does

The host has one job stated three ways:

1. **Own the window and its event sources.** It creates a Silk.NET window, wires
   the load/update/render/resize/close callbacks, and attaches keyboard and mouse
   input. Raw Silk.NET events become typed `ViewerEvent` values.
2. **Own the GPU pipeline.** It brings up Vulkan (instance → presentation surface →
   physical/logical device + queue → swapchain) and a SkiaSharp `GRContext` backed
   by that same Vulkan device, then renders each frame into an offscreen Skia
   surface and copies the result into a swapchain image for presentation.
3. **Run the Elmish loop against those effects.** It holds the current model,
   dispatches messages through the application's `update`, and interprets the
   resulting effects (`RenderFrame`, `CaptureScreenshot`, `Shutdown`, …) as real
   side effects.

The renderer path is intentionally narrow: there is **no software fallback**. If
Vulkan or Skia setup fails, the host returns a structured `RenderDiagnostic`
rather than silently degrading to another backend.

## Two front doors

The package exposes the host through two layers, and it helps to know which one you
are looking at.

### The Elmish edge — `FS.Skia.UI.SkiaViewer.Host.Viewer`

This is the small, composable surface most apps use. A `ViewerProgram<'model,'msg>`
bundles the configuration plus your `Init`/`Update`/`View` and the mapper functions
that translate host events and app messages:

```fsharp
type ViewerProgram<'model, 'msg> =
    { Configuration: ViewerConfiguration
      Init: unit -> 'model * Cmd<'msg>
      Update: 'msg -> 'model -> 'model * Cmd<'msg>
      View: 'model -> Scene
      EventMapper: ViewerEvent -> 'msg option
      EffectMapper: 'msg -> ViewerEffect<'msg> option
      Subscriptions: 'model -> (string list * (Dispatch<'msg> -> IDisposable)) list }
```

You build one with `Viewer.create`, refine it with the `withSubscription` /
`withEventMapping` / `withEffectMapping` combinators, and start it with
`Viewer.run`, which returns `Result<unit, RenderDiagnostic>`. `Viewer.run` first
validates the configuration — non-empty title, positive size, positive frame rate,
a supported OS (Windows or Linux) — and only then hands off to the Vulkan host
body. This is the contract documented in
[`Viewer`](../reference/fs-skia-ui-skiaviewer-viewer.html) and the shape that
[Elmish/MVU](./elmish-mvu.html) bindings target.

### The package-level façade — `FS.Skia.UI.SkiaViewer`

The top-level [`SkiaViewer`](../reference/fs-skia-ui-skiaviewer.html) namespace adds
a much larger, evidence-and-lifecycle-oriented vocabulary: `ViewerOptions`,
`ViewerLaunchOutcome`, `ScreenshotEvidenceResult`, `ViewerRunRequest`/
`ViewerRunEvidence`, a `GeneratedAppHost<'model,'msg>` record, and pure
`init`/`update` state machines (`ViewerModel`/`ViewerMsg`, `ViewerRunModel`,
`EvidenceWorkflowModel`). These exist so that generated apps and the test/evidence
harness can drive and *describe* a viewer run — bounded smoke runs, first-frame
capture, screenshot evidence, desktop-session diagnostics — much of it as pure data
that can be asserted without ever opening a GPU surface.

## Control and data flow

The runtime keeps pure application logic strictly separated from host side effects.
The cycle, per frame, is:

```text
Silk.NET window/input event
  -> ViewerEvent              (Loaded, UpdateTick, RenderTick, KeyDown/Up,
                               PointerMoved/Pressed/Released/Scrolled/Exited,
                               Resized, CloseRequested, DiagnosticReported)
  -> EventMapper              (ViewerEvent -> 'msg option)
  -> application Msg
  -> Update                   ('msg -> 'model -> 'model * Cmd<'msg>)
  -> Cmd<'msg>                (Elmish effects)
  -> EffectMapper             ('msg -> ViewerEffect<'msg> option)
  -> ViewerEffect             (RenderFrame, CaptureScreenshot, Shutdown,
                               ReportDiagnostic, Dispatch, InitializeRenderer)
  -> interpreter side effect  (draw / save PNG / close / log)
```

Inside the host body (`VulkanHost.run`), `dispatch` is the hub: for a given
message it first consults the `EffectMapper`; if that yields a `ViewerEffect`, the
host interprets it directly, otherwise it runs the application's `update`, stores
the new model, and executes the returned `Cmd<'msg>`. `View` is called to produce
the `Scene`, and `RenderFrame` is the effect that actually paints it.

### Rendering a frame

`RenderFrame scene` walks down through `renderFrame` → `renderSceneToPixels` →
the shared [`SceneRenderer.paintNode`](./scene.html) painter:

1. Acquire the next swapchain image (with a fence wait).
2. Create an **offscreen** GPU `SKSurface` on the shared `GRContext`, clear it to
   the configured clear color, and draw every `Scene` node into its canvas via the
   single exhaustive painter shared with the screenshot path.
3. Flush Skia, read the pixels back into a managed array.
4. Upload those pixels through a Vulkan staging buffer and a one-time command
   buffer that transitions the swapchain image to transfer-dst, copies, transitions
   to present-src, then `vkQueuePresentKHR`.

Notably the host renders Skia to an **offscreen** surface and then *copies* the
pixels into the swapchain image, rather than wrapping the swapchain image as a Skia
render target directly. This keeps the readback (and therefore screenshot capture)
trivially available — `lastFrame` always holds the most recent pixel snapshot — at
the cost of a per-frame GPU→CPU→GPU round trip.

### Frame timing

`run` does not lean on Silk.NET's own loop; it runs a manual `while not closing &&
not shutdownRequested` loop that calls `DoEvents`, then `DoUpdate`/`DoRender` gated
by a stopwatch against the target frame interval, with a 1 ms `Thread.Sleep` to
avoid a busy spin.

## Vulkan startup, ownership, and shutdown

Bring-up is a fixed, ordered staircase, encoded as data in `VulkanStartup.stages`:
instance → presentation surface → logical device & queues → swapchain → command
pool → command buffers → fence → staging buffer → staging memory → Skia GPU
context. Each stage returns `Result<_, RenderDiagnostic>`, and the whole sequence is
threaded through a small `result { … }` computation expression so that the first
failure short-circuits with a precise, stage-tagged diagnostic.

Resource lifetime is modelled explicitly. `VulkanResources` is a pure
**ownership ledger** (`acquire`, `transfer`, `releaseAll`) that records each owned
handle, its category, and its release action; `releaseAll` releases in **reverse
acquisition order**. The companion `VulkanStartup.simulateFailure` /
`simulateSuccessfulShutdown` functions use this ledger to *prove* the reverse-order,
idempotent cleanup contract synthetically — without opening a real device — which
is how the host's teardown discipline is tested in environments with no GPU.

The live teardown mirrors that contract: `run`'s `finally` block disposes
subscriptions and event mappings, then tears down Skia context, swapchain, device,
surface, instance, and window in reverse, each guard-checked against a non-zero
handle so partial bring-up still unwinds cleanly.

## Diagnostics and evidence

Failures are first-class values, not exceptions-as-control-flow. `RenderDiagnostic`
carries a `Severity`, a `DiagnosticStage` (e.g. `VulkanSurface`, `SkiaContext`,
`FrameRender`), a message, and an optional cause; the `Diagnostics` module provides
named constructors (`startupFailed`, `frameRenderFailed`, `screenshotFailed`, …).
The richer `SkiaViewer`-level evidence types then classify a whole run — blocked
stage, failure classification, visual-evidence artifacts — so the test harness can
distinguish an unsupported environment from a genuine product defect.

## How it fits the rest of the framework

- **Above it:** the [Elmish/MVU runtime](./elmish-mvu.html) supplies the
  `Model`/`Msg`/`update`/`view` shape and the animation tick; the host is the thing
  that actually runs that program against a window.
- **Beside it:** the [Scene](./scene.html) package is the *only* drawing
  vocabulary the host understands. Per
  [ADR 0008](../adr/0008-scene-vocabulary-single-source.md), there is a single
  canonical `Scene` type — the host is retyped directly onto it, with no conversion
  shim — and the exhaustive `SceneRenderer.paintNode` is the single place those
  nodes become Skia draw calls.
- **Below it:** Silk.NET (windowing + Vulkan bindings) and SkiaSharp (the GPU
  canvas) are the external dependencies; the host is the boundary that contains
  them so consumers never touch them directly.

## Analysis

### Implementation strengths

- The full Vulkan bring-up sequence is threaded through a `Result`-returning
  `result { … }` computation expression, so a failure at any stage
  (`createInstance`, `createSwapchain`, `createSkiaContext`, …) short-circuits with
  a precise `RenderDiagnostic` carrying the exact `DiagnosticStage` and cause —
  there is no silent partial initialization.
- Teardown is genuinely careful: `run`'s `finally` unwinds Skia context, swapchain,
  device, surface, instance, and window in reverse order, each step guarded by a
  non-zero-handle check, so a crash midway through bring-up still releases what was
  acquired.
- Resource cleanup discipline is independently testable: `VulkanResources` /
  `VulkanStartup` model the acquire/release ledger as pure data and verify
  reverse-order, idempotent release synthetically, which is what lets cleanup be
  validated on machines with no GPU.
- The interactive frame path and the screenshot/evidence path both go through the
  same `SceneRenderer.paintNode`, so what you see on screen and what gets captured
  as evidence cannot drift apart.

### Implementation weaknesses

- Every frame renders Skia to an offscreen surface, reads the pixels back to a
  managed `byte[]`, then re-uploads them through a Vulkan staging buffer and a
  one-time command buffer — a GPU→CPU→GPU round trip per frame that is convenient
  for screenshots but wasteful as a steady-state render strategy.
- The frame loop allocates and destroys a command pool, command buffer, fence, and
  staging buffer **per frame** (`renderFrame` / `copyPixelsToSwapchainImage`)
  rather than reusing pooled resources, which adds avoidable allocation and
  validation churn on the hot path.
- `VulkanHost.run` is a single very large function holding well over a dozen
  `mutable` locals for the live handles; correctness depends on disciplined manual
  bookkeeping in that one scope rather than on the type system.
- The host swallows `QueueWaitIdle` after each frame upload (`vkQueueWaitIdle`),
  fully serializing CPU and GPU every frame — simple and correct, but it forfeits
  any overlap between frames.

### Design pros

- Owning the host in this package (ADR 0007) keeps the dependency graph honest:
  consumers of the host pull in only `Scene` and `KeyboardInput`, not an entire
  framework, which is the whole point of the split-package distribution.
- The pure/effectful boundary is sharp and well chosen: applications are pure
  `Model`/`Msg`/`update`/`view`, and *all* side effects funnel through the
  `ViewerEffect` interpreter, so app logic is testable without a GPU and the host is
  the single auditable place where the world is touched.
- Modelling failure as data (`RenderDiagnostic` with stage + cause, then the
  richer run/evidence classifications) makes "why didn't it render?" answerable and
  lets tooling distinguish an unsupported environment from a real defect.
- The deliberate no-fallback policy means behaviour is predictable: the host either
  renders with Vulkan/Skia or reports exactly why it could not, never quietly
  switching to a different, differently-behaving backend.

### Design cons

- The two front doors (`Host.Viewer`'s small Elmish edge versus the large
  package-level `SkiaViewer` evidence/lifecycle vocabulary) present a big, partly
  overlapping surface; a newcomer must learn which layer to use for what, and the
  sheer number of `ViewerOptions`/`ViewerModel`/`ViewerRunModel`/evidence types is
  daunting.
- Tying the host to Vulkan-only with no software path maximizes fidelity but
  narrows reach: there is no headless or non-Vulkan rendering route for
  environments where a Vulkan device is unavailable, only a diagnostic.
- Restricting supported platforms to Windows and Linux (the `Viewer.run`
  validation) is a clear, defensible scope choice but an explicit limitation for
  anyone targeting macOS.
- The evidence/screenshot machinery is deeply woven into the host package; it
  serves this project's governance and proof-of-render needs well, but it is a lot
  of host-specific apparatus for a consumer who only wants to draw a window.

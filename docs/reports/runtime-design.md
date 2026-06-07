---
title: Runtime Design
category: Design history
categoryindex: 90
index: 3
description: Viewer, scene, rendering, effect, screenshot, and diagnostic runtime design.
---

# Runtime Design

The runtime design separates pure application logic from host side effects.
Applications provide an Elmish-shaped program and declarative scenes; the viewer
host owns window events, Vulkan/Skia setup, frame execution, screenshots,
diagnostics, and shutdown.

## Viewer Program Contract

The public viewer contract is centered on `ViewerProgram<'model, 'msg>`:

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

Applications own `Model`, `Msg`, `init`, `update`, and `view`. The viewer owns
the interpreter for `ViewerEffect<'msg>`. The public shape is declared in
[src/Lib/Library.fsi](../src/Lib/Library.fsi), and the host interpreter lives
in [src/Lib/Library.fs](../src/Lib/Library.fs).

## Event And Effect Flow

```text
Silk.NET window/input event
  -> ViewerEvent
  -> EventMapper
  -> application Msg
  -> Update
  -> Cmd<Msg>
  -> EffectMapper
  -> ViewerEffect
  -> interpreter side effect
```

`ViewerEffect<'msg>` currently covers renderer initialization, frame rendering,
screenshot capture, shutdown, diagnostic reporting, and message dispatch. This
keeps state transitions testable: samples can collect emitted messages/effects
in `--contract-smoke` paths without creating a GPU surface. Representative
sample consumers live under [samples](../samples/), and semantic coverage lives
in [tests/Lib.Tests](../tests/Lib.Tests/).

## Scene Model

`Scene` is an opaque immutable value built with `Scene.rectangle`,
`Scene.path`, `Scene.textRun`, `Scene.image`, `Scene.clipped`,
`Scene.withPerspective`, `Scene.picture`, and related functions. The API
exposes inspection helpers such as `Scene.describe`, `Scene.diagnostics`, and
`Scene.renderReadbackEvidence` so tests can assert semantic coverage without
pixel comparisons. The public contract is in
[src/Lib/Library.fsi](../src/Lib/Library.fsi); the internal scene nodes and
Skia drawing implementation are in [src/Lib/Library.fs](../src/Lib/Library.fs).

The renderer walks `Scene` values and maps the supported elements to Skia
drawing operations on the Vulkan-backed surface. Unsupported or invalid inputs
are reported as diagnostics instead of silently falling back to another
renderer.

## Rendering Boundary

The supported renderer path is intentionally narrow:

```text
Viewer.run
  -> platform validation
  -> Silk.NET window and Vulkan surface
  -> Vulkan instance/device/swapchain
  -> Skia Vulkan context
  -> frame rendering from Scene
```

There is no OpenGL, CPU, browser, mobile, or software fallback renderer. A
startup or frame problem returns `Result.Error RenderDiagnostic` with a
`DiagnosticStage` such as `VulkanInstance`, `VulkanDevice`, `VulkanSurface`,
`VulkanSwapchain`, `SkiaContext`, `FrameRender`, or `ScreenshotCapture`.
The implementation is concentrated in the internal `VulkanHost` module in
[src/Lib/Library.fs](../src/Lib/Library.fs).

## Screenshot Design

Screenshot capture is requested with `ViewerEffect.CaptureScreenshot`. The
viewer writes PNG or JPEG from the last successful Vulkan/Skia frame. A capture
before any successful frame is a `ScreenshotCapture` diagnostic because the
library does not synthesize a software-rendered replacement image.
The screenshot request and diagnostic contracts are declared in
[src/Lib/Library.fsi](../src/Lib/Library.fsi).

## Subscriptions

`ViewerProgram.Subscriptions` lets applications attach disposable event sources
that dispatch messages. Subscriptions are part of the viewer program contract,
but their side effects still sit outside `update`; they enter the app as
messages and return through the same update/effect flow.

## Testing Implications

Runtime changes should have:

- semantic tests for pure scene, diagnostic, path, keyboard input, chart, or
  layout behavior;
- FSI transcript coverage for public entry points when the public surface
changes;
- sample `--contract-smoke` coverage when a user-reachable sample path changes;
- live Vulkan smoke only where the target explicitly requires visual/runtime
  evidence.

See [Testing Workflow](testing.md) and [Evidence Policy](evidence.md) for the
current required gates. The tests most directly covering this surface are in
[tests/Lib.Tests](../tests/Lib.Tests/), [tests/Smoke.Tests](../tests/Smoke.Tests/),
and [tests/Package.Tests](../tests/Package.Tests/).

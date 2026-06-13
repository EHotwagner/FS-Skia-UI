# Contract Delta: `FS.Skia.UI.SkiaViewer` (Feature 118)

The viewer is a UI host; its "contract" is the public `.fsi` surface plus the internal
config seam. This is the **only** public-surface change in the feature. It escalates Route
to the SkiaViewer public-surface gate set (plus `TemplateCheck` / `GeneratedProductCheck`
because the template/generated product constructs `ViewerOptions`).

## Public — `src/SkiaViewer/SkiaViewer.fsi`

### New type (insert before `ViewerOptions`, ~line 7)

```fsharp
[<RequireQualifiedAccess>]
/// Selects how the live viewer presents each rendered frame. The default
/// (`OffscreenReadback`) is today's offscreen-render-plus-GPU→CPU-readback path and is
/// byte-identical to the pre-feature baseline; `DirectToSwapchain` is an opt-in path that
/// renders the Skia scene straight onto the acquired Vulkan swapchain image with no
/// per-frame readback, staging buffer/command pool, or `vkQueueWaitIdle` stall.
type ViewerPresentMode =
    /// Offscreen render then GPU→CPU readback then CPU→GPU upload to the swapchain image
    /// (the default, unchanged present path). Also the on-demand evidence/screenshot routine.
    | OffscreenReadback
    /// Render directly onto the acquired swapchain image via a backend render target; no
    /// per-frame readback, staging buffer/command pool, or full-queue stall. Opt-in;
    /// degrades safely to `OffscreenReadback` with a `Warning` diagnostic on init failure.
    | DirectToSwapchain
```

### Changed record — `ViewerOptions` (~lines 8–10)

```fsharp
/// Public contract type exposed by this FS.Skia.UI package.
type ViewerOptions =
    { Title: string
      InitialSize: Size
      /// Live present mechanism (feature 118). Defaults to `ViewerPresentMode.OffscreenReadback`
      /// at every construction site, preserving today's behavior; set to
      /// `ViewerPresentMode.DirectToSwapchain` to opt into the readback-free live present path.
      PresentMode: ViewerPresentMode }
```

Notes:
- Attribute-before-doc-before-type ordering preserved for `ViewerPresentMode`
  (`[<RequireQualifiedAccess>]` → `///` → `type`) to satisfy the XML-doc gate.
- `///` precedes each new public field/case.
- Adding a record field is a **breaking record-shape change**: every `ViewerOptions`
  construction site adds `PresentMode = ViewerPresentMode.OffscreenReadback`
  (see plan "Construction-site updates" + research R5). `with`-expression sites are exempt.
- Per-package and top-level surface baselines refresh via `RefreshSurfaceBaselines`.

## Internal — `src/SkiaViewer/Host/Diagnostics.fsi`

`ViewerConfiguration` gains a matching field threaded into `renderFrame`:

```fsharp
[<NoEquality; NoComparison>]
type ViewerConfiguration =
    { Title: string
      InitialSize: Size
      ClearColor: Color option
      TargetFrameRate: int option
      Diagnostics: DiagnosticOptions
      ConfigureWindow: (Silk.NET.Windowing.WindowOptions -> Silk.NET.Windowing.WindowOptions) option
      PresentMode: ViewerPresentMode }   // new — threaded from ViewerOptions.PresentMode
```

`Host.Viewer.defaultConfiguration` (Viewer.fs:10) and the config-build site
(`SkiaViewer.fs:~1231`) set `PresentMode` from the option.

## `Vulkan.fsi` (internal backend)

`renderFrame`'s signature is unchanged at the call site (`configuration` already carries
the new field). Any new internal helper for the direct path (e.g. building/caching the
per-image `GRBackendRenderTarget`, the present-mode diagnostic) is added under the existing
internal surface; no public exposure.

## Diagnostic-category plumbing (FR-007)

The present-mode/readback live diagnostic is published as a `ViewerDiagnosticEvent` with
`Category = ViewerDiagnosticCategory.Swapchain` (or `Frame`). This requires the
`LegacyDiagnosticReported` mapping (`SkiaViewer.fs:1290`, currently hardcoded to
`Renderer`) to carry a category — by mapping the internal `RenderDiagnostic.Stage`
(`VulkanSwapchain → Swapchain`, `FrameRender → Frame`, else `Renderer`) or via a dedicated
present-mode diagnostic carrier (decide against the failing-first category test; see
research R3). No public diagnostic **type** changes.

## What does NOT change

- No `FrameMetrics` field (FR-008). `FrameMetrics` lives in `FS.Skia.UI.Controls.Elmish`
  `Perf` (headless, no backend) and is untouched; `Perf.runScript` goldens are unchanged
  (SC-007).
- No new `ViewerMsg` / `ViewerEffect` / `update` change (present mode is config).
- No package identity / dependency change.
- No public entry-point signature change (`runInteractiveViewer`, `runApp`, `runBounded`,
  `runForFrames`, `runUntilFirstFrame`, … all unchanged — they already take `ViewerOptions`).

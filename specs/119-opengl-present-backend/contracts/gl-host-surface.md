# Contract: GL host public surface (FSI-drafted before `.fs`)

The UI contract for this feature is the **public `.fsi` surface** of `FS.Skia.UI.SkiaViewer`.
Per Principle I, this shape is drafted as `.fsi` and exercised in FSI (prelude transcript) before
any `.fs` body exists. Three deltas: (1) the host module replacement, (2) the diagnostic-DU
reconciliation, (3) the retained-but-re-documented present-mode DU. **Source-stable** entry
points are listed last and MUST NOT change shape.

## 1. Host module: replace `Host/Vulkan.fsi` → GL host

File renamed `Host/Vulkan.fsi` → `Host/OpenGl.fsi` (namespace `FS.Skia.UI.SkiaViewer.Host`).
The three Vulkan public modules are replaced by GL successors:

```fsharp
namespace FS.Skia.UI.SkiaViewer.Host

/// GL resource-ownership ledger (GL successor to VulkanResources).
module GlResources =
    type ResourceCategory =
        | GlContext
        | GlSurface          // window surface / drawable
        | GrContext          // Skia GL GPU context
        | Framebuffer        // FBO 0 wrapper / GRBackendRenderTarget
        | SkiaSurface        // SKSurface over the framebuffer
        | SkiaGpu
    type OwnershipState = Acquired | Transferred | Released
    type OwnedResource =
        { Id: string; Category: ResourceCategory; AcquireStage: string
          Owner: string; TransferPoint: string option; ReleaseAction: string; State: OwnershipState }
    type ReleaseRecord = { Id: string; Category: ResourceCategory; Stage: string; Order: int }
    type ResourceLedger = { Owned: OwnedResource list; Released: ReleaseRecord list }
    val empty: ResourceLedger
    val acquire: id: string -> category: ResourceCategory -> acquireStage: string -> owner: string -> releaseAction: string -> ledger: ResourceLedger -> ResourceLedger
    val transfer: id: string -> transferPoint: string -> ledger: ResourceLedger -> ResourceLedger
    val acquired: ledger: ResourceLedger -> OwnedResource list
    val releaseAll: stage: string -> ledger: ResourceLedger -> ResourceLedger * ReleaseRecord list

/// GL startup-stage ordering + cleanup model (GL successor to VulkanStartup).
module GlStartup =
    type StartupStage =
        { Name: string; Order: int; Resource: GlResources.ResourceCategory option; DiagnosticStage: string }
    type StartupFailureCase =
        { FailedStage: StartupStage
          AcquiredBeforeFailure: GlResources.OwnedResource list
          ExpectedReleaseOrder: GlResources.ResourceCategory list
          ObservedReleaseOrder: GlResources.ResourceCategory list
          DiagnosticStage: string; DiagnosticCause: string; Synthetic: bool }
    val stages: StartupStage list
    val stageByName: name: string -> StartupStage option
    val simulateFailure: failedStageName: string -> StartupFailureCase
    val simulateSuccessfulShutdown: unit -> GlResources.ReleaseRecord list

/// The OpenGL/Skia presentation host body (internal helpers hidden; only `run` is reachable).
module GlHost =
    /// Signature shape preserved from VulkanHost.run so Host/Viewer.fs routes unchanged.
    val run: program: ViewerProgram<'model, 'msg> -> Result<unit, RenderDiagnostic>
```

> The resource/startup ledger modules are kept (rather than dropped) so the existing
> ownership/startup-order **property tests** carry over with GL categories — preserving test
> evidence parity across the swap. If the ledger's only consumer was Vulkan-specific tests, it MAY
> be slimmed; the decision is recorded in the surface baseline diff, not hidden.

## 2. Diagnostic DUs reconciled to GL (`SkiaViewer.fsi`)

```fsharp
type ViewerDiagnosticCategory =
    | Startup | EnvironmentSession | Input | Frame | Renderer
    | OpenGl            // was: Vulkan
    | Skia
    | Framebuffer      // was: Swapchain
    | Scene | Screenshot

type ViewerRunBlockedStage =
    | DesktopPrerequisite | ProcessLaunch | WindowCreation | FirstFrameRender | Observation
    | Capture | InputVerification | ControlledExit | ArtifactWrite | Window | Surface | Renderer
    | GlContext        // was: Swapchain
    | Scene
    | Readback         // RETAINED — OffscreenReadback / evidence path still reads back
    | App | Timeout | Unknown
```

## 3. Present-mode DU — retained, re-documented (`PresentMode.fsi`)

Cases unchanged (`OffscreenReadback` | `DirectToSwapchain`); XML-doc re-mapped to GL semantics
(see `data-model.md`). `DirectToSwapchain` documented as the **default** readback-free GL path.

## 4. Source-stable entry points — MUST NOT change shape (SC-005)

```fsharp
type ViewerOptions = { … ; PresentMode: ViewerPresentMode }     // field retained; default flips to Direct
module Viewer =
    val runApp: options -> host -> Result<ViewerLaunchOutcome, ViewerRunFailure>
    val runAppWithWindowBehavior: …
    val runInteractiveViewer: options -> host -> Result<ViewerLaunchOutcome, ViewerRunFailure>
    val runInteractiveViewerWithWindowBehavior: …
    val runBounded / runUntilFirstFrame / runForFrames / captureScreenshotEvidence: …  // unchanged
```

`Controls.Elmish.runInteractiveApp` (the high-level consumer front door) compiles unchanged.

## Contract verification

- **FSI**: a prelude transcript (`readiness/fsi/*`) loads the packed `FS.Skia.UI.SkiaViewer` and
  calls `GlResources.empty`/`acquire`, `GlStartup.stages`, and constructs `ViewerOptions` with
  each `ViewerPresentMode`, before any `.fs` body change is final.
- **Surface baselines**: `RefreshSurfaceBaselines` regenerates top-level + per-package baselines;
  `PerPackageSurfaceDiff` shows exactly the intended breaking delta and nothing else.
- **Migration** (`readiness/migration.md`): every removed/renamed public member
  (`VulkanResources`/`VulkanStartup`/`VulkanHost`, `ViewerDiagnosticCategory.Vulkan`/`Swapchain`,
  `ViewerRunBlockedStage.Swapchain`) is named with its GL replacement.

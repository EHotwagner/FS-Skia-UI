namespace FS.Skia.UI.SkiaViewer.Host

/// GL resource-ownership ledger (feature 119; GL successor to the former VulkanResources).
module GlResources =
    /// Public contract type exposed by this FS.Skia.UI package.
    type ResourceCategory =
        | GlContext
        | GlSurface
        | GrContext
        | Framebuffer
        | SkiaSurface
        | SkiaGpu

    /// Public contract type exposed by this FS.Skia.UI package.
    type OwnershipState =
        | Acquired
        | Transferred
        | Released

    /// Public contract type exposed by this FS.Skia.UI package.
    type OwnedResource =
        { Id: string
          Category: ResourceCategory
          AcquireStage: string
          Owner: string
          TransferPoint: string option
          ReleaseAction: string
          State: OwnershipState }

    /// Public contract type exposed by this FS.Skia.UI package.
    type ReleaseRecord =
        { Id: string
          Category: ResourceCategory
          Stage: string
          Order: int }

    /// Public contract type exposed by this FS.Skia.UI package.
    type ResourceLedger =
        { Owned: OwnedResource list
          Released: ReleaseRecord list }

    /// Public contract function exposed by this FS.Skia.UI package.
    val empty: ResourceLedger

    /// Public contract function exposed by this FS.Skia.UI package.
    val acquire:
        id: string ->
        category: ResourceCategory ->
        acquireStage: string ->
        owner: string ->
        releaseAction: string ->
        ledger: ResourceLedger ->
            ResourceLedger

    /// Public contract function exposed by this FS.Skia.UI package.
    val transfer: id: string -> transferPoint: string -> ledger: ResourceLedger -> ResourceLedger
    /// Public contract function exposed by this FS.Skia.UI package.
    val acquired: ledger: ResourceLedger -> OwnedResource list
    /// Public contract function exposed by this FS.Skia.UI package.
    val releaseAll: stage: string -> ledger: ResourceLedger -> ResourceLedger * ReleaseRecord list

/// GL startup-stage ordering + cleanup model (feature 119; GL successor to the former VulkanStartup).
module GlStartup =
    /// Public contract type exposed by this FS.Skia.UI package.
    type StartupStage =
        { Name: string
          Order: int
          Resource: GlResources.ResourceCategory option
          DiagnosticStage: string }

    /// Public contract type exposed by this FS.Skia.UI package.
    type StartupFailureCase =
        { FailedStage: StartupStage
          AcquiredBeforeFailure: GlResources.OwnedResource list
          ExpectedReleaseOrder: GlResources.ResourceCategory list
          ObservedReleaseOrder: GlResources.ResourceCategory list
          DiagnosticStage: string
          DiagnosticCause: string
          Synthetic: bool }

    /// Public contract function exposed by this FS.Skia.UI package.
    val stages: StartupStage list
    /// Public contract function exposed by this FS.Skia.UI package.
    val stageByName: name: string -> StartupStage option
    /// Public contract function exposed by this FS.Skia.UI package.
    val simulateFailure: failedStageName: string -> StartupFailureCase
    /// Public contract function exposed by this FS.Skia.UI package.
    val simulateSuccessfulShutdown: unit -> GlResources.ReleaseRecord list

/// The OpenGL/Skia presentation host body (internal helpers hidden; only `run` is reachable).
module GlHost =
    /// Public contract function exposed by this FS.Skia.UI package. Signature shape preserved
    /// from the former VulkanHost.run so Host/Viewer.fs routes unchanged.
    val run: program: ViewerProgram<'model, 'msg> -> Result<unit, RenderDiagnostic>

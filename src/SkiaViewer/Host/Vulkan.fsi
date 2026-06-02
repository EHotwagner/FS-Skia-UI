namespace FS.Skia.UI.SkiaViewer.Host

/// Vulkan resource-ownership ledger (moved from the FS.Skia.UI monolith; behaviour preserved).
module VulkanResources =
    /// Public contract type exposed by this FS.Skia.UI package.
    type ResourceCategory =
        | VulkanInstance
        | VulkanSurface
        | VulkanDevice
        | VulkanSwapchain
        | CommandPool
        | CommandBuffer
        | Fence
        | StagingBuffer
        | StagingMemory
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

/// Vulkan startup-stage ordering + cleanup model (moved from the FS.Skia.UI monolith; behaviour preserved).
module VulkanStartup =
    /// Public contract type exposed by this FS.Skia.UI package.
    type StartupStage =
        { Name: string
          Order: int
          Resource: VulkanResources.ResourceCategory option
          DiagnosticStage: string }

    /// Public contract type exposed by this FS.Skia.UI package.
    type StartupFailureCase =
        { FailedStage: StartupStage
          AcquiredBeforeFailure: VulkanResources.OwnedResource list
          ExpectedReleaseOrder: VulkanResources.ResourceCategory list
          ObservedReleaseOrder: VulkanResources.ResourceCategory list
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
    val simulateSuccessfulShutdown: unit -> VulkanResources.ReleaseRecord list

/// The Vulkan/Skia presentation host body (internal helpers hidden; only `run` is reachable).
module VulkanHost =
    /// Public contract function exposed by this FS.Skia.UI package.
    val run: program: ViewerProgram<'model, 'msg> -> Result<unit, RenderDiagnostic>

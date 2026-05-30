namespace FS.Skia.UI

/// Public contract module exposed by this FS.Skia.UI package.
module internal VulkanResources =
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
    val empty : ResourceLedger

    /// Public contract function exposed by this FS.Skia.UI package.
    val acquire :
        id: string ->
        category: ResourceCategory ->
        acquireStage: string ->
        owner: string ->
        releaseAction: string ->
        ledger: ResourceLedger ->
            ResourceLedger

    /// Public contract function exposed by this FS.Skia.UI package.
    val transfer : id: string -> transferPoint: string -> ledger: ResourceLedger -> ResourceLedger
    /// Public contract function exposed by this FS.Skia.UI package.
    val acquired : ledger: ResourceLedger -> OwnedResource list
    /// Public contract function exposed by this FS.Skia.UI package.
    val releaseAll : stage: string -> ledger: ResourceLedger -> ResourceLedger * ReleaseRecord list

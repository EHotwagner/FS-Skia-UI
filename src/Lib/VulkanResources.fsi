namespace FS.Skia.UI

module internal VulkanResources =
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

    type OwnershipState =
        | Acquired
        | Transferred
        | Released

    type OwnedResource =
        { Id: string
          Category: ResourceCategory
          AcquireStage: string
          Owner: string
          TransferPoint: string option
          ReleaseAction: string
          State: OwnershipState }

    type ReleaseRecord =
        { Id: string
          Category: ResourceCategory
          Stage: string
          Order: int }

    type ResourceLedger =
        { Owned: OwnedResource list
          Released: ReleaseRecord list }

    val empty : ResourceLedger

    val acquire :
        id: string ->
        category: ResourceCategory ->
        acquireStage: string ->
        owner: string ->
        releaseAction: string ->
        ledger: ResourceLedger ->
            ResourceLedger

    val transfer : id: string -> transferPoint: string -> ledger: ResourceLedger -> ResourceLedger
    val acquired : ledger: ResourceLedger -> OwnedResource list
    val releaseAll : stage: string -> ledger: ResourceLedger -> ResourceLedger * ReleaseRecord list

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

    let empty = { Owned = []; Released = [] }

    let acquire id category acquireStage owner releaseAction ledger =
        let resource =
            { Id = id
              Category = category
              AcquireStage = acquireStage
              Owner = owner
              TransferPoint = None
              ReleaseAction = releaseAction
              State = Acquired }

        { ledger with Owned = ledger.Owned @ [ resource ] }

    let transfer id transferPoint ledger =
        let update (resource: OwnedResource) =
            if resource.Id = id && resource.State <> Released then
                { resource with
                    State = Transferred
                    TransferPoint = Some transferPoint }
            else
                resource

        { ledger with Owned = ledger.Owned |> List.map update }

    let acquired (ledger: ResourceLedger) =
        ledger.Owned
        |> List.filter (fun resource -> resource.State <> Released)

    let releaseAll stage ledger =
        let releasable = acquired ledger |> List.rev

        let records =
            releasable
            |> List.mapi (fun index resource ->
                { Id = resource.Id
                  Category = resource.Category
                  Stage = stage
                  Order = index + 1 })

        let releasedIds = records |> List.map _.Id |> Set.ofList

        let owned =
            ledger.Owned
            |> List.map (fun resource ->
                if releasedIds.Contains resource.Id then
                    { resource with State = Released }
                else
                    resource)

        { Owned = owned
          Released = ledger.Released @ records },
        records

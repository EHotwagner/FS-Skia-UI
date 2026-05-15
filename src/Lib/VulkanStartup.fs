namespace FS.Skia.UI

module internal VulkanStartup =
    type StartupStage =
        { Name: string
          Order: int
          Resource: VulkanResources.ResourceCategory option
          DiagnosticStage: string }

    type StartupFailureCase =
        { FailedStage: StartupStage
          AcquiredBeforeFailure: VulkanResources.OwnedResource list
          ExpectedReleaseOrder: VulkanResources.ResourceCategory list
          ObservedReleaseOrder: VulkanResources.ResourceCategory list
          DiagnosticStage: string
          DiagnosticCause: string
          Synthetic: bool }

    let stages =
        [ { Name = "create-vulkan-instance"
            Order = 10
            Resource = Some VulkanResources.VulkanInstance
            DiagnosticStage = "VulkanInstance" }
          { Name = "create-presentation-surface"
            Order = 20
            Resource = Some VulkanResources.VulkanSurface
            DiagnosticStage = "VulkanSurface" }
          { Name = "create-logical-device-and-queues"
            Order = 30
            Resource = Some VulkanResources.VulkanDevice
            DiagnosticStage = "VulkanDevice" }
          { Name = "create-swapchain-and-images"
            Order = 40
            Resource = Some VulkanResources.VulkanSwapchain
            DiagnosticStage = "VulkanSwapchain" }
          { Name = "create-command-pool"
            Order = 50
            Resource = Some VulkanResources.CommandPool
            DiagnosticStage = "FrameRender" }
          { Name = "allocate-command-buffers"
            Order = 60
            Resource = Some VulkanResources.CommandBuffer
            DiagnosticStage = "FrameRender" }
          { Name = "create-fence"
            Order = 70
            Resource = Some VulkanResources.Fence
            DiagnosticStage = "FrameRender" }
          { Name = "create-staging-buffer"
            Order = 80
            Resource = Some VulkanResources.StagingBuffer
            DiagnosticStage = "FrameRender" }
          { Name = "allocate-staging-memory"
            Order = 90
            Resource = Some VulkanResources.StagingMemory
            DiagnosticStage = "FrameRender" }
          { Name = "create-skia-gpu-context"
            Order = 100
            Resource = Some VulkanResources.SkiaGpu
            DiagnosticStage = "SkiaContext" } ]

    let stageByName name =
        stages |> List.tryFind (fun stage -> stage.Name = name)

    let releaseAction category =
        match category with
        | VulkanResources.VulkanInstance -> "vkDestroyInstance"
        | VulkanResources.VulkanSurface -> "vkDestroySurfaceKHR"
        | VulkanResources.VulkanDevice -> "vkDestroyDevice"
        | VulkanResources.VulkanSwapchain -> "vkDestroySwapchainKHR"
        | VulkanResources.CommandPool -> "vkDestroyCommandPool"
        | VulkanResources.CommandBuffer -> "free-with-command-pool"
        | VulkanResources.Fence -> "vkDestroyFence"
        | VulkanResources.StagingBuffer -> "vkDestroyBuffer"
        | VulkanResources.StagingMemory -> "vkFreeMemory"
        | VulkanResources.SkiaGpu -> "dispose-GRContext"

    let acquireStage ledger stage =
        match stage.Resource with
        | None -> ledger
        | Some category ->
            VulkanResources.acquire
                $"{stage.Name}-resource"
                category
                stage.Name
                "VulkanHost.run"
                (releaseAction category)
                ledger

    let acquireBefore failedStage =
        stages
        |> List.filter (fun stage -> stage.Order < failedStage.Order)
        |> List.fold acquireStage VulkanResources.empty

    let simulateFailure failedStageName =
        // SYNTHETIC: symbolic resource handles force each startup failure path; real native smoke is recorded in readiness/native-smoke.txt.
        let failedStage =
            stageByName failedStageName
            |> Option.defaultWith (fun () -> invalidArg (nameof failedStageName) $"Unknown startup stage: {failedStageName}")

        let ledger = acquireBefore failedStage
        let acquired = VulkanResources.acquired ledger
        let _, releases = VulkanResources.releaseAll failedStage.Name ledger

        { FailedStage = failedStage
          AcquiredBeforeFailure = acquired
          ExpectedReleaseOrder = acquired |> List.rev |> List.map _.Category
          ObservedReleaseOrder = releases |> List.map _.Category
          DiagnosticStage = failedStage.DiagnosticStage
          DiagnosticCause = $"{failedStage.Name} failed with synthetic native error"
          Synthetic = true }

    let simulateSuccessfulShutdown () =
        // SYNTHETIC: symbolic successful acquisition verifies idempotent reverse cleanup order without opening a real Vulkan device.
        let ledger = stages |> List.fold acquireStage VulkanResources.empty
        let _, firstRelease = VulkanResources.releaseAll "shutdown" ledger
        let afterFirst, _ = VulkanResources.releaseAll "shutdown" ledger
        let _, secondRelease = VulkanResources.releaseAll "shutdown" afterFirst

        firstRelease @ secondRelease

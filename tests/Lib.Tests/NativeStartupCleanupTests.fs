module internal FS.Skia.UI.NativeStartupCleanupTests

open Expecto
open FS.Skia.UI

[<Tests>]
let nativeStartupCleanupTests =
    testList "Native startup cleanup" [
        let resourceCategories =
            [ VulkanResources.VulkanInstance
              VulkanResources.VulkanSurface
              VulkanResources.VulkanDevice
              VulkanResources.VulkanSwapchain
              VulkanResources.CommandPool
              VulkanResources.CommandBuffer
              VulkanResources.Fence
              VulkanResources.StagingBuffer
              VulkanResources.StagingMemory
              VulkanResources.SkiaGpu ]

        test "startup stage inventory covers every owned Vulkan and Skia resource category" {
            let stageResources =
                VulkanStartup.stages
                |> List.choose _.Resource

            resourceCategories
            |> List.iter (fun category -> Expect.contains stageResources category $"stage inventory contains {category}")

            let ordered = VulkanStartup.stages |> List.sortBy _.Order
            Expect.equal ordered VulkanStartup.stages "startup stages are declared in acquisition order"
        }

        test "injected acquisition failures Synthetic release acquired resources once in reverse order" {
            // SYNTHETIC: symbolic handles force every failure stage deterministically; real native smoke path is readiness/native-smoke.txt.
            for stage in VulkanStartup.stages |> List.tail do
                let failure = VulkanStartup.simulateFailure stage.Name

                Expect.isTrue failure.Synthetic "failure fixture discloses synthetic acquisition"
                Expect.equal failure.ExpectedReleaseOrder failure.ObservedReleaseOrder $"release order is reversed for {stage.Name}"
                Expect.equal failure.DiagnosticStage stage.DiagnosticStage $"diagnostic stage is preserved for {stage.Name}"
                Expect.stringContains failure.DiagnosticCause stage.Name $"cause names failing stage {stage.Name}"

                let releasedIds =
                    failure.ObservedReleaseOrder
                    |> List.countBy id
                    |> List.filter (fun (_, count) -> count <> 1)

                Expect.isEmpty releasedIds $"every acquired category is released once for {stage.Name}"
        }

        test "successful shutdown Synthetic releases all acquired resources once and repeated cleanup is idempotent" {
            // SYNTHETIC: symbolic successful acquisition avoids opening a real Vulkan device; real native smoke path is readiness/native-smoke.txt.
            let releases = VulkanStartup.simulateSuccessfulShutdown ()
            let categories = releases |> List.map _.Category

            Expect.equal categories (resourceCategories |> List.rev) "successful shutdown releases resources in reverse acquisition order"

            let duplicates =
                categories
                |> List.countBy id
                |> List.filter (fun (_, count) -> count <> 1)

            Expect.isEmpty duplicates "repeated cleanup does not double-release any resource"
        }
    ]

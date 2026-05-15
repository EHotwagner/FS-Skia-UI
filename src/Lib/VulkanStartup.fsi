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

    val stages : StartupStage list
    val stageByName : name: string -> StartupStage option
    val simulateFailure : failedStageName: string -> StartupFailureCase
    val simulateSuccessfulShutdown : unit -> VulkanResources.ReleaseRecord list

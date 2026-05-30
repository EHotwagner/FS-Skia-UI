namespace FS.Skia.UI

/// Public contract module exposed by this FS.Skia.UI package.
module internal VulkanStartup =
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
    val stages : StartupStage list
    /// Public contract function exposed by this FS.Skia.UI package.
    val stageByName : name: string -> StartupStage option
    /// Public contract function exposed by this FS.Skia.UI package.
    val simulateFailure : failedStageName: string -> StartupFailureCase
    /// Public contract function exposed by this FS.Skia.UI package.
    val simulateSuccessfulShutdown : unit -> VulkanResources.ReleaseRecord list

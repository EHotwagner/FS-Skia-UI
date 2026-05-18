module IntegrationDiagnosticFixtures

type IntegrationFailureClassification =
    | UnsupportedHost
    | ProductDefect
    | SetupDrift

type IntegrationBlockedStage =
    | Window
    | Surface
    | Renderer
    | Swapchain
    | Scene
    | Readback
    | App
    | Timeout
    | Unknown

type IntegrationDiagnosticCategory =
    | Startup
    | Input
    | Frame
    | Renderer
    | Vulkan
    | Skia
    | Swapchain
    | Scene
    | Screenshot
    | Package

type GeneratedAppFlow =
    | InitialStart
    | OptionsNavigation
    | PrimaryInteraction
    | PauseOrBack
    | EndRestart

type DiagnosticFixture =
    { Flow: GeneratedAppFlow option
      InputValue: string option
      Screen: string option
      Stage: IntegrationBlockedStage option
      Category: IntegrationDiagnosticCategory
      Classification: IntegrationFailureClassification option
      PackageId: string option
      EvidencePath: string option
      Message: string }

module IntegrationDiagnosticFixtures =
    let inputFlow flow input screen =
        { Flow = Some flow
          InputValue = Some input
          Screen = Some screen
          Stage = None
          Category = Input
          Classification = None
          PackageId = None
          EvidencePath = Some "specs/013-tetris-demo-integration/readiness/generated-template-input-flows.md"
          Message = $"Input {input} should transition {screen} through {flow}." }

    let blockedStage stage classification =
        { Flow = None
          InputValue = None
          Screen = None
          Stage = Some stage
          Category = Startup
          Classification = Some classification
          PackageId = None
          EvidencePath = Some "specs/013-tetris-demo-integration/readiness/bounded-viewer-smoke.md"
          Message = $"Viewer startup blocked at {stage}." }

    let packageDrift packageId =
        { Flow = None
          InputValue = None
          Screen = None
          Stage = None
          Category = Package
          Classification = Some SetupDrift
          PackageId = Some packageId
          EvidencePath = Some "specs/013-tetris-demo-integration/readiness/local-consumer-packages.md"
          Message = $"Local package feed drift for {packageId}." }

    let scannerInputs =
        [ "ArrowLeft"
          "Left"
          "Enter"
          "Return"
          "Space"
          "Escape"
          "Backspace"
          "A"
          "7"
          "F12"
          "VendorKey" ]

    let failureStages =
        [ IntegrationBlockedStage.Window
          IntegrationBlockedStage.Surface
          IntegrationBlockedStage.Renderer
          IntegrationBlockedStage.Swapchain
          IntegrationBlockedStage.Scene
          IntegrationBlockedStage.Readback
          IntegrationBlockedStage.App
          IntegrationBlockedStage.Timeout
          IntegrationBlockedStage.Unknown ]

    let requiredPackageIds =
        [ "FS.Skia.UI.Scene"
          "FS.Skia.UI.SkiaViewer"
          "FS.Skia.UI.KeyboardInput"
          "FS.Skia.UI.Testing" ]

    let requiredEvidencePaths =
        [ "specs/013-tetris-demo-integration/readiness/normalized-viewer-input.md"
          "specs/013-tetris-demo-integration/readiness/bounded-viewer-smoke.md"
          "specs/013-tetris-demo-integration/readiness/diagnostics.md"
          "specs/013-tetris-demo-integration/readiness/headless-scene-evidence.md"
          "specs/013-tetris-demo-integration/readiness/generated-template-input-flows.md"
          "specs/013-tetris-demo-integration/readiness/local-consumer-packages.md"
          "specs/013-tetris-demo-integration/readiness/generated-consumer-validation.md" ]

module FS.Skia.UI.Build.Targets

type Target =
    | Clean
    | Restore
    | Build
    | Test
    | Dev
    | PackLocal
    | RefreshSurfaceBaselines
    | PackageSurfaceCheck
    | PerPackageSurfaceDiff
    | FsiTranscripts
    | SampleContractSmoke
    | TemplatePack
    | TemplateInstallSource
    | TemplateInstallPackage
    | TemplateInstantiate
    | TemplateSmoke
    | TemplateCheck
    | CapabilityCheck
    | SkillCheck
    | GeneratedProductCheck
    // Feature 088 (US2, FR-006): the GeneratedProductCheck split (umbrella stays resolvable).
    | GeneratedProductStructure
    | GeneratedConsumerValidation
    | ControlsCatalogCheck
    // Feature 066 (US3, FR-006): the typed-catalog generation-currency (drift) gate.
    | ControlsCatalogGenerationCheck
    // Feature 069 (US1, FR-006): the design-token generation-currency (drift) gate.
    | DesignTokenDrift
    // Feature 083 (US1, FR-007): the WCAG color-contrast gate over the shipped theme tokens.
    | ContrastCheck
    // Feature 078 (US1, FR-005): the controls-catalog docs currency/completeness/preview gate.
    | ControlsCatalogDocsCheck
    // Feature 106 (US2, FR-007): the Controls public-surface documentation-coverage gate.
    | ControlsDocCoverageCheck
    // Feature 080 (US2, FR-007/FR-012): the render-capable decoded-content fidelity gate.
    | ControlFidelityCheck
    | ControlsInteractionCheck
    | ControlsRenderingCheck
    | DependencyReport
    | SymbolCrossCheck
    | GeneratedGuidanceCheck
    | SkillSyncCheck
    | SkillQualityCheck
    // Feature 077 (FR-006): the phase-skill hook-discovery anti-drift gate.
    | PhaseHookParityCheck
    | SkillContractPathCheck
    | TemplateUpdateSkillPackageCheck
    | TemplateDrift
    | EvidenceGraph
    | EvidenceAudit
    | AgentReady
    | TargetMetadata
    | TargetMetadataDrift
    | VerifyPreflight
    | CiPreflight
    | StaleBoundaryScan
    | FinalReadiness
    | Verify
    | Ci
    | Route
    // Feature 064 (FR-001/FR-006/FR-007): the distribution targets.
    | PrePublishCheck
    | Publish
    | PackageSmoke
    | BuildWorkflowCheck

type TargetSpec =
    { Target: Target
      Name: string
      DirectPrerequisites: Target list
      TimeoutClass: string
      Cost: string
      FailureOwner: string }

// Registry order (replaces requiredTargets). PackageSmoke/BuildWorkflowCheck are
// dispatched but excluded here so the metadata registry stays at 42 rows
// (feature 044 retired SkillExamplesCheck; feature 063 added SymbolCrossCheck;
// feature 064 added PrePublishCheck + Publish, 38 -> 40; feature 088 added the
// GeneratedProductStructure/GeneratedConsumerValidation split sub-targets, 40 -> 42).
let allTargets =
    [ Clean
      Restore
      Build
      Test
      Dev
      PackLocal
      RefreshSurfaceBaselines
      PackageSurfaceCheck
      PerPackageSurfaceDiff
      FsiTranscripts
      SampleContractSmoke
      TemplatePack
      TemplateInstallSource
      TemplateInstallPackage
      TemplateInstantiate
      TemplateSmoke
      TemplateCheck
      CapabilityCheck
      SkillCheck
      GeneratedProductCheck
      GeneratedProductStructure
      GeneratedConsumerValidation
      ControlsCatalogCheck
      ControlsCatalogGenerationCheck
      DesignTokenDrift
      ContrastCheck
      ControlsCatalogDocsCheck
      ControlsDocCoverageCheck
      ControlFidelityCheck
      ControlsInteractionCheck
      ControlsRenderingCheck
      DependencyReport
      SymbolCrossCheck
      GeneratedGuidanceCheck
      SkillSyncCheck
      SkillQualityCheck
      PhaseHookParityCheck
      SkillContractPathCheck
      TemplateUpdateSkillPackageCheck
      TemplateDrift
      EvidenceGraph
      EvidenceAudit
      AgentReady
      TargetMetadata
      TargetMetadataDrift
      VerifyPreflight
      CiPreflight
      StaleBoundaryScan
      FinalReadiness
      Verify
      Ci
      Route
      PrePublishCheck
      Publish ]

let dispatchTargets = allTargets @ [ PackageSmoke; BuildWorkflowCheck ]

let name target =
    match target with
    | Clean -> "Clean"
    | Restore -> "Restore"
    | Build -> "Build"
    | Test -> "Test"
    | Dev -> "Dev"
    | PackLocal -> "PackLocal"
    | RefreshSurfaceBaselines -> "RefreshSurfaceBaselines"
    | PackageSurfaceCheck -> "PackageSurfaceCheck"
    | PerPackageSurfaceDiff -> "PerPackageSurfaceDiff"
    | FsiTranscripts -> "FsiTranscripts"
    | SampleContractSmoke -> "SampleContractSmoke"
    | TemplatePack -> "TemplatePack"
    | TemplateInstallSource -> "TemplateInstallSource"
    | TemplateInstallPackage -> "TemplateInstallPackage"
    | TemplateInstantiate -> "TemplateInstantiate"
    | TemplateSmoke -> "TemplateSmoke"
    | TemplateCheck -> "TemplateCheck"
    | CapabilityCheck -> "CapabilityCheck"
    | SkillCheck -> "SkillCheck"
    | GeneratedProductCheck -> "GeneratedProductCheck"
    | GeneratedProductStructure -> "GeneratedProductStructure"
    | GeneratedConsumerValidation -> "GeneratedConsumerValidation"
    | ControlsCatalogCheck -> "ControlsCatalogCheck"
    | ControlsCatalogGenerationCheck -> "ControlsCatalogGenerationCheck"
    | DesignTokenDrift -> "DesignTokenDrift"
    | ContrastCheck -> "ContrastCheck"
    | ControlsCatalogDocsCheck -> "ControlsCatalogDocsCheck"
    | ControlsDocCoverageCheck -> "ControlsDocCoverageCheck"
    | ControlFidelityCheck -> "ControlFidelityCheck"
    | ControlsInteractionCheck -> "ControlsInteractionCheck"
    | ControlsRenderingCheck -> "ControlsRenderingCheck"
    | DependencyReport -> "DependencyReport"
    | SymbolCrossCheck -> "SymbolCrossCheck"
    | GeneratedGuidanceCheck -> "GeneratedGuidanceCheck"
    | SkillSyncCheck -> "SkillSyncCheck"
    | SkillQualityCheck -> "SkillQualityCheck"
    | PhaseHookParityCheck -> "PhaseHookParityCheck"
    | SkillContractPathCheck -> "SkillContractPathCheck"
    | TemplateUpdateSkillPackageCheck -> "TemplateUpdateSkillPackageCheck"
    | TemplateDrift -> "TemplateDrift"
    | EvidenceGraph -> "EvidenceGraph"
    | EvidenceAudit -> "EvidenceAudit"
    | AgentReady -> "AgentReady"
    | TargetMetadata -> "TargetMetadata"
    | TargetMetadataDrift -> "TargetMetadataDrift"
    | VerifyPreflight -> "VerifyPreflight"
    | CiPreflight -> "CiPreflight"
    | StaleBoundaryScan -> "StaleBoundaryScan"
    | FinalReadiness -> "FinalReadiness"
    | Verify -> "Verify"
    | Ci -> "Ci"
    | Route -> "Route"
    | PrePublishCheck -> "PrePublishCheck"
    | Publish -> "Publish"
    | PackageSmoke -> "PackageSmoke"
    | BuildWorkflowCheck -> "BuildWorkflowCheck"

let directPrerequisites target =
    match target with
    | Clean -> []
    | Restore -> []
    | Build -> [ Restore ]
    | Test -> [ Build; SampleContractSmoke ]
    | Dev -> [ Test; SkillSyncCheck ]
    | PackLocal -> []
    | RefreshSurfaceBaselines -> [ Build ]
    | PackageSurfaceCheck -> [ Build ]
    | PerPackageSurfaceDiff -> [ Build ]
    | FsiTranscripts -> [ Build ]
    | SampleContractSmoke -> [ Build ]
    | TemplatePack -> []
    | TemplateInstallSource -> []
    | TemplateInstallPackage -> [ TemplatePack ]
    | TemplateInstantiate -> [ TemplatePack; TemplateInstallSource; TemplateInstallPackage ]
    | TemplateSmoke -> [ TemplateInstantiate; Test ]
    | TemplateCheck -> [ TemplatePack; TemplateInstallSource; TemplateInstallPackage; TemplateInstantiate; TemplateSmoke ]
    | CapabilityCheck -> []
    | SkillCheck -> [ CapabilityCheck ]
    // Feature 088 (US2, FR-006/FR-007): the umbrella keeps its setup prerequisites and now
    // composes the two split sub-targets, so `GeneratedProductCheck` produces the identical
    // evidence/verdict while delegating the cheap structural scan and the expensive consumer
    // validation to resolvable sub-targets. The umbrella arm no longer re-emits the scan/
    // validation effects (they live on the sub-targets), so nothing runs twice.
    | GeneratedProductCheck ->
        [ CapabilityCheck; SkillCheck; Dev; TemplateCheck; GeneratedProductStructure; GeneratedConsumerValidation ]
    // The structural sub-target is independent so it fails fast before any consumer cost; it
    // reads the same local package feed (populated by PackLocal) the umbrella always assumed.
    | GeneratedProductStructure -> []
    | GeneratedConsumerValidation -> [ GeneratedProductStructure ]
    | ControlsCatalogCheck -> []
    | ControlsCatalogGenerationCheck -> []
    | DesignTokenDrift -> []
    | ContrastCheck -> []
    | ControlsCatalogDocsCheck -> []
    | ControlsDocCoverageCheck -> []
    | ControlFidelityCheck -> []
    | ControlsInteractionCheck -> []
    | ControlsRenderingCheck -> []
    | DependencyReport -> []
    | SymbolCrossCheck -> []
    | GeneratedGuidanceCheck -> []
    | SkillSyncCheck -> []
    | SkillQualityCheck -> []
    | PhaseHookParityCheck -> [ Build ]
    | SkillContractPathCheck -> []
    | TemplateUpdateSkillPackageCheck -> []
    | TemplateDrift -> []
    | EvidenceGraph -> []
    | EvidenceAudit -> [ EvidenceGraph ]
    | AgentReady -> [ EvidenceGraph ]
    | TargetMetadata -> []
    | TargetMetadataDrift -> [ TargetMetadata ]
    | VerifyPreflight -> []
    | CiPreflight -> []
    | StaleBoundaryScan -> []
    | FinalReadiness -> [ EvidenceAudit ]
    | Verify ->
        [ VerifyPreflight
          Dev
          PackLocal
          PackageSurfaceCheck
          FsiTranscripts
          SampleContractSmoke
          TemplateCheck
          CapabilityCheck
          SkillCheck
          GeneratedProductCheck
          ControlsCatalogCheck
          ControlsInteractionCheck
          ControlsRenderingCheck
          DependencyReport
          GeneratedGuidanceCheck
          SkillContractPathCheck
          TemplateUpdateSkillPackageCheck
          TemplateDrift
          EvidenceAudit
          TargetMetadataDrift ]
    | Ci -> [ CiPreflight; Verify ]
    | Route -> []
    // Feature 064: PrePublishCheck composes with TemplateCheck (pin parity + metadata over
    // the packed/template set); Publish gates on the pre-publish check and the two pack
    // targets so the 12 .nupkg artifacts exist before the push.
    | PrePublishCheck -> [ TemplateCheck ]
    | Publish -> [ PrePublishCheck; PackLocal; TemplatePack ]
    | PackageSmoke -> [ PackageSurfaceCheck ]
    | BuildWorkflowCheck -> []

let private timeoutClass target =
    match target with
    | Verify
    | Ci
    | TemplateCheck -> "broad"
    | GeneratedProductCheck
    | GeneratedConsumerValidation
    | PackageSurfaceCheck
    | ControlFidelityCheck
    | FsiTranscripts -> "medium"
    | _ -> "focused"

let private cost target =
    match target with
    | Verify
    | Ci -> "high"
    | TemplateCheck
    | GeneratedProductCheck
    | GeneratedConsumerValidation
    | ControlFidelityCheck -> "medium"
    | _ -> "low"

let private failureOwner target =
    match target with
    | TemplateCheck
    | GeneratedProductCheck
    | GeneratedProductStructure
    | GeneratedConsumerValidation -> "template"
    | ControlsCatalogCheck
    | ControlsCatalogGenerationCheck
    | DesignTokenDrift
    | ContrastCheck
    | ControlsCatalogDocsCheck
    | ControlFidelityCheck
    | ControlsInteractionCheck
    | ControlsRenderingCheck -> "product"
    | _ -> "governance"

let spec target =
    { Target = target
      Name = name target
      DirectPrerequisites = directPrerequisites target
      TimeoutClass = timeoutClass target
      Cost = cost target
      FailureOwner = failureOwner target }

let requiredTargetNames = allTargets |> List.map name

let targetDependencyRows =
    dispatchTargets
    |> List.map (fun target -> name target, directPrerequisites target |> List.map name)

// Feature 088 (US1, FR-003): the routable-gate projection — the single source for
// AgentValidation.knownGates. A routing rule can require any of these gates, plus the two
// aggregate composites (Verify/Ci). Rendered in `allTargets` registry order so the derived
// allowlist is byte-stable. Set-equal to the prior hand-maintained knownGates literal.
let routableGates =
    let routable target =
        match target with
        | Dev
        | PackageSurfaceCheck
        | PerPackageSurfaceDiff
        | FsiTranscripts
        | TemplateCheck
        | GeneratedProductCheck
        | ControlsCatalogCheck
        | ControlsCatalogGenerationCheck
        | DesignTokenDrift
        | ContrastCheck
        | ControlsCatalogDocsCheck
        | ControlsDocCoverageCheck
        | ControlFidelityCheck
        | ControlsInteractionCheck
        | ControlsRenderingCheck
        | SymbolCrossCheck
        | GeneratedGuidanceCheck
        | SkillSyncCheck
        | SkillQualityCheck
        | PhaseHookParityCheck
        | SkillContractPathCheck
        | TemplateUpdateSkillPackageCheck
        | TemplateDrift
        | EvidenceGraph
        | EvidenceAudit
        | AgentReady
        | TargetMetadataDrift
        | Verify
        | Ci
        | PrePublishCheck
        | Publish -> true
        | _ -> false

    allTargets |> List.filter routable

// Feature 088 (US1, FR-004): whether a target is one of Verify's product-facing evidence
// gates. The membership is pinned to the historical `ProductChecksRun` set so the verdict's
// derived list is byte-identical to the prior literal.
let isProductCheck target =
    match target with
    | Dev
    | PackageSurfaceCheck
    | FsiTranscripts
    | ControlsCatalogCheck
    | ControlsInteractionCheck
    | ControlsRenderingCheck
    | DependencyReport
    | TemplateCheck
    | GeneratedProductCheck
    | GeneratedGuidanceCheck
    | TemplateDrift
    | EvidenceAudit -> true
    | _ -> false

// Feature 088 (US1, FR-004): Verify's product-check gates in the pinned canonical order the
// verdict has always rendered (NOT registry order — the historical ProductChecksRun literal
// lists ControlsCatalog/Interaction/Rendering/Dependency before Template/GeneratedProduct).
// `productCheckGates |> List.map name` equals the prior literal byte-for-byte and in order.
let productCheckGates =
    [ Dev
      PackageSurfaceCheck
      FsiTranscripts
      ControlsCatalogCheck
      ControlsInteractionCheck
      ControlsRenderingCheck
      DependencyReport
      TemplateCheck
      GeneratedProductCheck
      GeneratedGuidanceCheck
      TemplateDrift
      EvidenceAudit ]

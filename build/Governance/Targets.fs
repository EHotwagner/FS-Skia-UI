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
    | ControlsCatalogCheck
    // Feature 066 (US3, FR-006): the typed-catalog generation-currency (drift) gate.
    | ControlsCatalogGenerationCheck
    | ControlsInteractionCheck
    | ControlsRenderingCheck
    | DependencyReport
    | SymbolCrossCheck
    | GeneratedGuidanceCheck
    | SkillSyncCheck
    | SkillQualityCheck
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
// dispatched but excluded here so the metadata registry stays at 40 rows
// (feature 044 retired SkillExamplesCheck; feature 063 added SymbolCrossCheck;
// feature 064 added PrePublishCheck + Publish, 38 -> 40).
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
      ControlsCatalogCheck
      ControlsCatalogGenerationCheck
      ControlsInteractionCheck
      ControlsRenderingCheck
      DependencyReport
      SymbolCrossCheck
      GeneratedGuidanceCheck
      SkillSyncCheck
      SkillQualityCheck
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
    | ControlsCatalogCheck -> "ControlsCatalogCheck"
    | ControlsCatalogGenerationCheck -> "ControlsCatalogGenerationCheck"
    | ControlsInteractionCheck -> "ControlsInteractionCheck"
    | ControlsRenderingCheck -> "ControlsRenderingCheck"
    | DependencyReport -> "DependencyReport"
    | SymbolCrossCheck -> "SymbolCrossCheck"
    | GeneratedGuidanceCheck -> "GeneratedGuidanceCheck"
    | SkillSyncCheck -> "SkillSyncCheck"
    | SkillQualityCheck -> "SkillQualityCheck"
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
    | GeneratedProductCheck -> [ CapabilityCheck; SkillCheck; Dev; TemplateCheck ]
    | ControlsCatalogCheck -> []
    | ControlsCatalogGenerationCheck -> []
    | ControlsInteractionCheck -> []
    | ControlsRenderingCheck -> []
    | DependencyReport -> []
    | SymbolCrossCheck -> []
    | GeneratedGuidanceCheck -> []
    | SkillSyncCheck -> []
    | SkillQualityCheck -> []
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
    | PackageSurfaceCheck
    | FsiTranscripts -> "medium"
    | _ -> "focused"

let private cost target =
    match target with
    | Verify
    | Ci -> "high"
    | TemplateCheck
    | GeneratedProductCheck -> "medium"
    | _ -> "low"

let private failureOwner target =
    match target with
    | TemplateCheck
    | GeneratedProductCheck -> "template"
    | ControlsCatalogCheck
    | ControlsCatalogGenerationCheck
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

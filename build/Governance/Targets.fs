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
    | ControlsInteractionCheck
    | ControlsRenderingCheck
    | DependencyReport
    | GeneratedGuidanceCheck
    | SkillSyncCheck
    | SkillExamplesCheck
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
// dispatched but excluded here so the metadata registry stays at 38 rows.
let allTargets =
    [ Clean
      Restore
      Build
      Test
      Dev
      PackLocal
      RefreshSurfaceBaselines
      PackageSurfaceCheck
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
      ControlsInteractionCheck
      ControlsRenderingCheck
      DependencyReport
      GeneratedGuidanceCheck
      SkillSyncCheck
      SkillExamplesCheck
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
      Ci ]

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
    | ControlsInteractionCheck -> "ControlsInteractionCheck"
    | ControlsRenderingCheck -> "ControlsRenderingCheck"
    | DependencyReport -> "DependencyReport"
    | GeneratedGuidanceCheck -> "GeneratedGuidanceCheck"
    | SkillSyncCheck -> "SkillSyncCheck"
    | SkillExamplesCheck -> "SkillExamplesCheck"
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
    | PackageSmoke -> "PackageSmoke"
    | BuildWorkflowCheck -> "BuildWorkflowCheck"

let directPrerequisites target =
    match target with
    | Clean -> []
    | Restore -> []
    | Build -> [ Restore ]
    | Test -> [ Build; SampleContractSmoke ]
    | Dev -> [ Test; SkillSyncCheck; SkillExamplesCheck ]
    | PackLocal -> []
    | RefreshSurfaceBaselines -> [ Build ]
    | PackageSurfaceCheck -> [ Build ]
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
    | ControlsInteractionCheck -> []
    | ControlsRenderingCheck -> []
    | DependencyReport -> []
    | GeneratedGuidanceCheck -> []
    | SkillSyncCheck -> []
    | SkillExamplesCheck -> [ SkillSyncCheck ]
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
          TemplateDrift
          EvidenceAudit
          TargetMetadataDrift ]
    | Ci -> [ CiPreflight; Verify ]
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

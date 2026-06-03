// Model.fsi — the build-side MEL state/events/effects (feature 045, T009, Principle II).
module FS.Skia.UI.Build.Engine.Model

open FS.Skia.UI.Build
open FS.Skia.UI.Build.Preflight
open FS.Skia.UI.Build.Front.Support

type BuildModel =
    { RepositoryRoot: string
      FeatureId: string
      FeatureDir: string
      ReadinessDir: string
      LogDir: string
      FsiDir: string
      SampleSmokeDir: string
      PackageEvidenceDir: string
      SurfaceBaselineDir: string
      LocalPackageDir: string
      TemplateArtifactDir: string
      TemplateWorkDir: string
      TemplateEvidenceDir: string
      GeneratedFileListsDir: string
      GeneratedProductVerifyDir: string
      GeneratedProductRootsDir: string
      PackageSurfaceReportDir: string
      CapabilityCatalogPath: string
      CapabilityCatalogReportPath: string
      SelectedSkillsReportPath: string
      DependencyReportPath: string
      GeneratedGuidanceReportPath: string
      TemplateDriftReportPath: string
      ProcessHealthPath: string
      BootstrapRunnerPath: string
      VerificationVerdictsPath: string
      FocusedGatesReportPath: string
      TargetMetadataReportPath: string
      TargetMetadataDriftReportPath: string
      GovernanceScannersPath: string
      StaleBoundaryScanPath: string
      GeneratedProductValidationPath: string
      EvidenceGraphReportPath: string
      EvidenceAuditReportPath: string
      DeferralsPath: string
      CompletedTargets: string list }

type BuildMsg =
    | StartTarget of Targets.Target
    | TargetCompleted of string
    | TargetFailed of string * string
    | ProcessHealthCollected of ProcessHealthSnapshot
    | BootstrapValidated of BootstrapValidation
    | VerificationVerdictWritten of VerificationVerdict
    | FocusedGateCompleted of FocusedGateContract

type BuildEffect =
    | EnsureDirectory of string
    | CleanDirectoryContents of string
    | RunProcess of label: string * fileName: string * arguments: string * workingDirectory: string * outputPath: string * environment: Map<string, string>
    | RunDotnetAction of label: string * action: string * solutionFile: string * projects: string list * extraArguments: string * outputPath: string
    | InstallTemplate of label: string * source: TemplateInstallSource * outputPath: string
    | InstantiateTemplates of outputPath: string
    | ScanGeneratedProjects of outputPath: string
    | CapabilityCatalogCheck
    | SkillCatalogCheck
    | GenerateV3Products
    | ScanV3GeneratedProducts
    | ValidateGeneratedConsumer
    | PackageSurfaceReport
    | DependencyOwnershipReport
    | ValidateTemplatePackage of outputPath: string
    | GeneratedGuidanceScan of outputPath: string
    | CollectProcessHealth of target: string * outputPath: string * verdictPath: string
    | ValidateRunnerBootstrap of target: string * outputPath: string * verdictPath: string
    | WriteVerificationVerdict of VerificationVerdict
    | WriteFocusedGateSummary of FocusedGateContract
    | CheckFocusedGateAssumptions of FocusedGateContract
    | WriteStructuredReport of label: string * path: string * content: string
    | WriteStructuredJsonReport of label: string * path: string * content: string
    | WriteFile of path: string * content: string
    | RequireFiles of artifactClass: string * paths: string list
    | FailWith of string
    | WorkflowSelfCheck
    | SkillSyncGate
    // Feature 044: single-source regeneration effects (interpreter edge — all tree
    // enumeration / file reads / writes live in interpret; update emits effect data only).
    | RegenerateSkillTree
    | RegenerateConstitutionFragments
    // Feature 057: splice every canonical GovernedBlock into its home files.
    | RegenerateGovernedBlocks
    | RouteSelect
    // Feature 043: in-process evidence gates (model is re-derived in interpret, so no payload).
    | EvidenceGraphCheck
    | EvidenceAuditCheck
    // Feature 048: additive per-package surface diff (re-derived in interpret, so no payload).
    | PerPackageSurfaceDiffCheck

/// Repository root (discovered by walking up for .specify/feature.json).
val repositoryRoot: string
/// The active feature id resolved from .specify/feature.json.
val featureId: string
/// Shell-quote a value (verbatim from build.fsx).
val quote: value: string -> string
/// The user-local nuget-local package directory.
val localPackageDir: unit -> string
/// Derive the path model from the repository root and emit startup directory effects.
val init: root: string -> BuildModel * BuildEffect list

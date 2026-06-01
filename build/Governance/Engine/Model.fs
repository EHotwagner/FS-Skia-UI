module FS.Skia.UI.Build.Engine.Model

open System
open System.IO
open BuildPaths
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Preflight
open FS.Skia.UI.Build.Front.Support

// Relocated from build.fsx (feature 045, T009): BuildModel/BuildMsg/BuildEffect + init.
// repositoryRoot was `__SOURCE_DIRECTORY__` in the FSX front-end; the compiled exe
// discovers the repository root by walking up for the .specify/feature.json marker
// (the launchers cd to the repo root, so this resolves identically at runtime).
let rec private findRepoRoot (dir: string) =
    if String.IsNullOrEmpty dir then
        Directory.GetCurrentDirectory()
    elif File.Exists(Path.Combine(dir, ".specify", "feature.json")) then
        dir
    else
        findRepoRoot (Path.GetDirectoryName dir)

let repositoryRoot = findRepoRoot (Directory.GetCurrentDirectory())

let activeFeatureId root =
    let featureJson = path [ root; ".specify"; "feature.json" ]

    // FR-001/FR-002 (spec 037): resolve the active feature authoritatively from
    // .specify/feature.json. There is no placeholder fallback — an unresolved
    // feature is never a passable state, so we hard-fail loudly naming the
    // expected source rather than silently auditing a stub.
    let fail reason =
        failwithf
            "Cannot resolve the active feature: %s. Expected an authoritative \"feature_directory\" entry in %s. The evidence graph/audit refuses to fall back to a placeholder feature (FR-001, FR-002)."
            reason
            featureJson

    if not (File.Exists featureJson) then
        fail "the file does not exist"
    else
        let content = File.ReadAllText featureJson
        let marker = "\"feature_directory\""
        let markerIndex = content.IndexOf(marker, StringComparison.Ordinal)

        if markerIndex < 0 then
            fail "no \"feature_directory\" key was found"
        else
            let afterMarker = content.Substring(markerIndex + marker.Length)
            let colonIndex = afterMarker.IndexOf(':')

            if colonIndex < 0 then
                fail "the \"feature_directory\" key has no value"
            else
                let afterColon = afterMarker.Substring(colonIndex + 1)
                let firstQuote = afterColon.IndexOf('"')

                if firstQuote < 0 then
                    fail "the \"feature_directory\" value is not a quoted string"
                else
                    let afterFirstQuote = afterColon.Substring(firstQuote + 1)
                    let secondQuote = afterFirstQuote.IndexOf('"')

                    if secondQuote < 0 then
                        fail "the \"feature_directory\" value is not terminated"
                    else
                        let featureDirectory = afterFirstQuote.Substring(0, secondQuote)

                        if String.IsNullOrWhiteSpace featureDirectory then
                            fail "the \"feature_directory\" value is empty"
                        else
                            Path.GetFileName(featureDirectory.TrimEnd('/', '\\'))

let featureId = activeFeatureId repositoryRoot

let quote (value: string) =
    "\"" + value.Replace("\"", "\\\"") + "\""

let featureDir root =
    path [ root; "specs"; featureId ]

let featureReadiness root =
    let dir = featureDir root

    if Directory.Exists dir then
        path [ dir; "readiness" ]
    else
        path [ root; "readiness"; "workflow" ]

let localPackageDir () =
    let home =
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)

    path [ home; ".local"; "share"; "nuget-local" ]

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
    | RouteSelect
    // Feature 043: in-process evidence gates (model is re-derived in interpret, so no payload).
    | EvidenceGraphCheck
    | EvidenceAuditCheck

let init root =
    let readiness = featureReadiness root

    let model =
        { RepositoryRoot = root
          FeatureId = featureId
          FeatureDir = featureDir root
          ReadinessDir = readiness
          LogDir = path [ readiness; "logs" ]
          FsiDir = path [ readiness; "fsi" ]
          SampleSmokeDir = path [ readiness; "sample-smoke" ]
          PackageEvidenceDir = path [ readiness; "package" ]
          SurfaceBaselineDir = path [ root; "readiness"; "surface-baselines" ]
          LocalPackageDir = localPackageDir ()
          TemplateArtifactDir = path [ root; "artifacts"; "templates" ]
          TemplateWorkDir = path [ root; "artifacts"; "template-check"; featureId ]
          TemplateEvidenceDir = path [ readiness; "template" ]
          GeneratedFileListsDir = path [ readiness; "generated-file-lists" ]
          GeneratedProductVerifyDir = path [ readiness; "generated-product-verify" ]
          GeneratedProductRootsDir = path [ root; "artifacts"; "generated-products"; featureId ]
          PackageSurfaceReportDir = path [ readiness; "package-surfaces" ]
          CapabilityCatalogPath = path [ root; "template"; "capabilities.yml" ]
          CapabilityCatalogReportPath = path [ readiness; "capability-catalog.md" ]
          SelectedSkillsReportPath = path [ readiness; "selected-skills.md" ]
          DependencyReportPath = path [ readiness; "dependency-report.md" ]
          GeneratedGuidanceReportPath = path [ readiness; "generated-guidance.md" ]
          TemplateDriftReportPath = path [ readiness; "template-drift.md" ]
          ProcessHealthPath = path [ readiness; "process-health.md" ]
          BootstrapRunnerPath = path [ readiness; "bootstrap-runner.md" ]
          VerificationVerdictsPath = path [ readiness; "verification-verdicts.md" ]
          FocusedGatesReportPath = path [ readiness; "focused-gates.md" ]
          TargetMetadataReportPath = path [ readiness; "target-metadata.json" ]
          TargetMetadataDriftReportPath = path [ readiness; "target-metadata-drift.md" ]
          GovernanceScannersPath = path [ readiness; "governance-scanners.md" ]
          StaleBoundaryScanPath = path [ readiness; "stale-boundary-scan.md" ]
          GeneratedProductValidationPath = path [ readiness; "generated-product-validation.md" ]
          EvidenceGraphReportPath = path [ readiness; "evidence-graph.md" ]
          EvidenceAuditReportPath = path [ readiness; "evidence-audit.md" ]
          DeferralsPath = path [ root; "readiness"; "template-deferrals.yml" ]
          CompletedTargets = [] }

    let effects =
        [ model.ReadinessDir
          model.LogDir
          model.FsiDir
          model.SampleSmokeDir
          model.PackageEvidenceDir
          model.SurfaceBaselineDir
          model.LocalPackageDir
          model.TemplateArtifactDir
          model.TemplateWorkDir
          model.TemplateEvidenceDir
          model.GeneratedFileListsDir
          model.GeneratedProductVerifyDir
          model.GeneratedProductRootsDir
          model.PackageSurfaceReportDir
          path [ model.TemplateEvidenceDir; "source-app" ]
          path [ model.TemplateEvidenceDir; "source-headless-scene" ]
          path [ model.TemplateEvidenceDir; "source-governed" ]
          path [ model.TemplateEvidenceDir; "source-sample-pack" ]
          path [ model.TemplateEvidenceDir; "package-app" ]
          path [ model.TemplateEvidenceDir; "package-headless-scene" ]
          path [ model.TemplateEvidenceDir; "package-governed" ]
          path [ model.TemplateEvidenceDir; "package-sample-pack" ] ]
        |> List.map EnsureDirectory

    model, effects

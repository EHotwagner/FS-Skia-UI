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
        findRepoRoot (Path.GetDirectoryName dir |> Option.ofObj |> Option.defaultValue "")

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
                            |> Option.ofObj
                            |> Option.defaultValue ""

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

// Feature 064 (FR-001/FR-002): release inputs, read from the environment at the interpreter
// edge (never committed). The pure `update` only emits the publish effect; the interpreter
// builds the PublishConfig and the per-package plan, then performs the read/push.
type PublishConfig =
    { FeedUrl: string
      ReadUrl: string
      ApiKeyPresent: bool
      DryRun: bool
      IsLocalFeed: bool }

type PublishDecision =
    | Push
    | Skip

// One row per package (12 rows: 11 packProjects libs + FS.Skia.UI.Template). Computed by the
// interpreter's anonymous feed read; rendered by dry-run and used to decide the push.
type PublishPlanRow =
    { PackageId: string
      Version: string
      FeedHasVersion: bool
      Decision: PublishDecision }

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
    // Feature 057: splice every canonical GovernedBlock into its home files (same edge,
    // same precedent as the skill-tree / constitution-fragment regeneration above).
    | RegenerateGovernedBlocks
    // Feature 066 (US1, FR-002): splice the six typed-catalog rows into both
    // `src/Controls/catalog.yml` and `src/Controls/Catalog.fs` from the single
    // `CatalogGen.catalogFacts` source in one operation, so the two generated outputs
    // cannot diverge (partial-regeneration edge case cannot occur). The file reads/writes
    // live at the interpret edge; `CatalogGen.splice*` are pure (Principle IV).
    | RegenerateCatalog
    // Feature 069 (US1, FR-002): regenerate src/Controls/DesignTokens.fs (whole-file) from
    // the DTCG source `src/Controls/design-tokens.tokens.json` in one operation. The file
    // read/write lives at the interpret edge; `DesignTokenGen.splice` is pure (Principle IV).
    | RegenerateDesignTokens
    | RouteSelect
    // Feature 043: in-process evidence gates (model is re-derived in interpret, so no payload).
    | EvidenceGraphCheck
    | EvidenceAuditCheck
    // Feature 048: additive per-package surface diff (pure diff + edge interpreter re-derived
    // in interpret over the real source tree + committed baselines, so no payload).
    | PerPackageSurfaceDiffCheck
    // Feature 058 (US1): pure skill-quality rubric check; enumeration/IO at the interpret edge.
    | SkillQualityScan
    // Feature 077 (FR-006): pure phase-hook parity check; the roster SKILL.md reads, the
    // report write, and the `failwith` on findings all live at the interpret edge.
    | PhaseHookScan
    // Feature 060: api-surface single-source regeneration (FR-003) folded into the
    // RefreshSurfaceBaselines refresh, plus the two anti-drift scans (FR-004 / FR-009).
    // Catalog/skill/packable-set reads + emitted-tree writes live at the interpret edge.
    | RegenerateApiSurface
    // Feature 062 (FR-006): regenerate docs/skillist-reference.md from the live
    // SkillRegistry + Audit.ownsVocabulary (registry build + write at the edge).
    | RegenerateSkillistReference
    | SkillContractPathScan
    | TemplateUpdatePackageScan
    // Feature 063 (FR-003): run the existing SymbolCrossCheck analyzer over the active
    // feature's plan/data-model/tasks (paths derived from the feature dir in interpret,
    // so no payload), print the markdown, and write readiness/symbol-cross-check.md.
    | SymbolCrossCheckAnalyze
    // Feature 064 (FR-001/FR-002): push all 12 packages to the configured feed. The
    // PublishConfig (feed URL, api-key presence, dry-run) is read from env in interpret; the
    // anonymous feed read, the per-package skip/push decision, and `dotnet nuget push
    // --skip-duplicate` all live at the edge, so no payload (Principle IV).
    | PublishPackages
    // Feature 064 (FR-006): the pre-publish consistency gate (pin parity, engine-pin match,
    // no machine-local path in the emitted config, required metadata). All file reads happen
    // in interpret; the PrePublish checks themselves are pure, so no payload.
    | PrePublishValidate

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

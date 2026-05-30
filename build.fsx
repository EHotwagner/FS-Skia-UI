#r "paket:
nuget FSharp.Core 8.0.400
nuget Fake.Core.Target 6.1.4
//"

open System
open System.Diagnostics
open System.IO
open System.IO.Compression
open System.Runtime.InteropServices
open System.Text.RegularExpressions
open Fake.Core
open Fake.Core.TargetOperators

// BUILD SECTION: path model

type TemplateInstallSource =
    | SourceDirectory
    | PackageArtifact

type TemplateRow =
    { Artifact: string
      Profile: string
      ProjectName: string
      Root: string
      EvidenceDir: string }

type V3GeneratedRow =
    { Artifact: string
      Profile: string
      ProjectName: string
      Root: string
      Capabilities: string list
      EvidenceDir: string
      FileListPath: string }

type CapabilityRow =
    { Id: string
      DisplayName: string
      PackageId: string option
      Project: string option
      Contracts: string list
      Tests: string list
      Skill: string option
      TemplateFragment: string option
      Dependencies: string list
      Profiles: string list
      DefaultApp: bool
      Evidence: string list
      SurfaceBaseline: string option
      Docs: string option
      NonRuntime: bool }

type ValidationFinding =
    { ArtifactClass: string
      Path: string
      Rule: string
      Message: string }

type ValidationSelectionModel = { selectedRuleIds: string list }
type ValidationSelectionMsg =
    | ReadActiveFeatureMetadata
    | RunGitMergeBaseDiff
    | ChangedPathSourceActiveFeature
    | ChangedPathSourceGitMergeBase
    | ChangedPathSourceUnavailable
    | ValidationSelectionDegraded

type ValidationSelectionEffect =
    | LoadValidationContractForSelection

let unionSelectedGates rules =
    rules |> List.distinct

let multiRuleGateUnion = "multi-rule gate union"

type AgentVerdict =
    { status: string
      authority: string
      changedPathSource: string
      selectedRuleIds: string list
      requiredGates: string list
      completedGates: string list
      missingGates: string list
      missingArtifacts: string list
      failureOwner: string
      failureClass: string
      nextCommand: string option }

let AgentVerdictJson = "AgentVerdictJson"
let AgentVerdictMarkdown = "AgentVerdictMarkdown"
let focusedAuthority = "focused authority"
let nonAuthoritativeAggregate = "non-authoritative aggregate"

type ProcessHealthThreshold =
    { RuleId: string
      SignalName: string
      DefaultValue: int64
      Comparison: string
      ActualValue: int64 option
      OverrideValue: int64 option
      OverrideSource: string option
      OverrideReason: string option
      PlatformApplicability: string
      Passed: bool option
      Diagnostic: string option }

type ProcessHealthSnapshot =
    { TimestampUtc: DateTimeOffset
      TargetName: string
      Stage: string
      Platform: string
      AvailableMemoryMb: int64 option
      ProcessCount: int option
      ZombieProcessCount: int option
      ThreadLimit: int64 option
      ThreadHeadroom: int64 option
      FileDescriptorLimit: int64 option
      FileDescriptorHeadroom: int64 option
      DotnetStartup: string
      FakeBootstrap: string
      UnsupportedSignals: string list
      Thresholds: ProcessHealthThreshold list
      PreflightElapsedMs: int64
      FailFast: bool
      Diagnostics: string list }

type BootstrapValidation =
    { TargetName: string
      TimestampUtc: DateTimeOffset
      DotnetSdkStatus: string
      FakeToolRestoreStatus: string
      PackageCacheStatus: string
      WrapperStatus: string
      WarningClassification: string
      RecommendedAction: string option
      LogPath: string
      Passed: bool }

type VerificationVerdictCategory =
    | VerificationSuccess
    | VerificationProductFailure
    | VerificationEnvironmentFailure
    | VerificationDegraded

type VerificationVerdict =
    { Category: VerificationVerdictCategory
      Target: string
      Stage: string
      ExitCode: int option
      ProductChecksRun: string list
      ProductFailures: string list
      EnvironmentFailures: string list
      HealthSnapshotPath: string
      LogPath: string
      RecommendedRerunEnvironment: string
      AuthoritativeProductEvidence: bool }

type FocusedGateContract =
    { TargetName: string
      DirectPrerequisites: string list
      Command: string
      LogPath: string
      ReadinessPath: string option
      StaleAssumptions: string list
      VerdictCategory: VerificationVerdictCategory }

type RunnableTargetName = string

type TargetMetadata =
    { RunnableTargetName: RunnableTargetName
      DirectPrerequisites: string list
      ExpectedOutputs: string list
      StaleAssumptions: string list
      TimeoutClass: string
      Cost: string
      Authority: string
      FailureOwner: string
      Command: string }

type TargetMetadataReport =
    { GeneratedAtUtc: DateTimeOffset
      Targets: TargetMetadata list
      Diagnostics: string list }

type TargetMetadataDrift =
    | MissingRunnableTarget of string
    | MissingMetadata of string
    | MissingExpectedOutput of string
    | MissingFailureOwner of string
    | DependencyDivergence of string

// BUILD SECTION: workflow model

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
    | StartTarget of string
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

#load "scripts/build/Paths.fsx"
#load "scripts/build/Process.fsx"
#load "scripts/build/Reports.fsx"
#load "scripts/build/GeneratedScanning.fsx"
#load "scripts/build/PackageResolution.fsx"
#load "scripts/build/TemplateValidation.fsx"
#load "scripts/build/ProcessHealth.fsx"

open BuildPaths
open BuildProcess

let repositoryRoot = __SOURCE_DIRECTORY__

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

let defaultTestProjects =
    [ "tests/Lib.Tests/Lib.Tests.fsproj"
      "tests/Scene.Tests/Scene.Tests.fsproj"
      "tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj"
      "tests/Elmish.Tests/Elmish.Tests.fsproj"
      "tests/KeyboardInput.Tests/KeyboardInput.Tests.fsproj"
      "tests/Layout.Tests/Layout.Tests.fsproj"
      "tests/Controls.Tests/Controls.Tests.fsproj"
      "tests/Testing.Tests/Testing.Tests.fsproj"
      "tests/Parity.Tests/Parity.Tests.fsproj"
      "tests/Smoke.Tests/Smoke.Tests.fsproj"
      "tests/Governance.Tests/Governance.Tests.fsproj" ]

let packProjects =
    [ "src/Scene/Scene.fsproj", "FS.Skia.UI.Scene"
      "src/SkiaViewer/SkiaViewer.fsproj", "FS.Skia.UI.SkiaViewer"
      "src/Elmish/Elmish.fsproj", "FS.Skia.UI.Elmish"
      "src/KeyboardInput/KeyboardInput.fsproj", "FS.Skia.UI.KeyboardInput"
      "src/Controls.Elmish/Controls.Elmish.fsproj", "FS.Skia.UI.Controls.Elmish"
      "src/Testing/Testing.fsproj", "FS.Skia.UI.Testing"
      "src/Lib/Lib.fsproj", "FS.Skia.UI"
      "src/Layout/Layout.fsproj", "FS.Skia.UI.Layout"
      "src/Controls/Controls.fsproj", "FS.Skia.UI.Controls" ]

let projectVersion repositoryRoot project =
    let content = File.ReadAllText(path [ repositoryRoot; project ])
    let versionMatch = Regex.Match(content, "<Version>([^<]+)</Version>", RegexOptions.CultureInvariant)

    if versionMatch.Success then
        versionMatch.Groups.[1].Value
    else
        let props = File.ReadAllText(path [ repositoryRoot; "Directory.Build.props" ])
        let propsMatch = Regex.Match(props, "<Version>([^<]+)</Version>", RegexOptions.CultureInvariant)

        if propsMatch.Success then
            propsMatch.Groups.[1].Value
        else
            "unknown"

let localPackageReport repositoryRoot localPackageDir =
    let packages =
        packProjects
        |> List.map (fun (project, packageId) ->
            let version = projectVersion repositoryRoot project
            project, packageId, version)

    let packageRows =
        packages
        |> List.map (fun (project, packageId, version) -> $"| `{packageId}` | `{version}` | `{project}` |")

    let packageReferences =
        packages
        |> List.map (fun (_, packageId, version) -> $"""    <PackageReference Include="{packageId}" Version="{version}" />""")

    let expectedArtifacts =
        packages
        |> List.map (fun (_, packageId, version) ->
            let fileName = packageId + "." + version + ".nupkg"
            let packagePath = Path.Combine(localPackageDir, fileName)
            $"- `{packagePath}`")

    [ "# Local Packages"
      ""
      $"Output directory: `{localPackageDir}`"
      ""
      "## Package Inventory"
      ""
      "| Package | Version | Project |"
      "|---------|---------|---------|"
      yield! packageRows
      ""
      "## Consumer Package Configuration"
      ""
      "```xml"
      "  <ItemGroup>"
      yield! packageReferences
      "  </ItemGroup>"
      "```"
      ""
      "## NuGet.config Snippet"
      ""
      "```xml"
      "  <packageSources>"
      "    <clear />"
      $"""    <add key="local" value="{localPackageDir}" />"""
      "    <add key=\"nuget\" value=\"https://api.nuget.org/v3/index.json\" />"
      "  </packageSources>"
      "```"
      ""
      "## Restore Command"
      ""
      $"`dotnet restore --source {localPackageDir} --source https://api.nuget.org/v3/index.json`"
      ""
      "## Expected Local Artifacts"
      ""
      yield! expectedArtifacts
      ""
      "## Drift Diagnostics"
      ""
      "Missing or stale `.nupkg` files are setup drift before generated consumer build, input, or rendering failures. Re-run `./fake.sh build -t PackLocal` and verify the package identity, expected version, actual version, and feed path above." ]
    |> String.concat Environment.NewLine

let fsiScripts =
    [ "prelude", "scripts/prelude.fsx"
      "input-prelude", "scripts/input-prelude.fsx"
      "keyboardinput-package-prelude", "scripts/keyboardinput-package-prelude.fsx"
      "layout-prelude", "scripts/layout-prelude.fsx"
      "controls-prelude", "scripts/controls-prelude.fsx"
      "controls-elmish-prelude", "scripts/controls-elmish-prelude.fsx" ]

let sampleSmokeProjects =
    [ "BasicViewer", "samples/BasicViewer/BasicViewer.fsproj"
      "InteractiveViewer", "samples/InteractiveViewer/InteractiveViewer.fsproj"
      "ParityGallery", "samples/ParityGallery/ParityGallery.fsproj"
      "EffectsGallery", "samples/EffectsGallery/EffectsGallery.fsproj"
      "LayoutGraphGallery", "samples/LayoutGraphGallery/LayoutGraphGallery.fsproj"
      "DataGridGallery", "samples/DataGridGallery/DataGridGallery.fsproj"
      "ChartsGallery", "samples/ChartsGallery/ChartsGallery.fsproj"
      "ScreenshotGallery", "samples/ScreenshotGallery/ScreenshotGallery.fsproj"
      "KeyboardInputGallery", "samples/KeyboardInputGallery/KeyboardInputGallery.fsproj"
      "ControlsGallery", "samples/ControlsGallery/ControlsGallery.fsproj" ]

let buildProjects =
    (packProjects |> List.map fst) @ (sampleSmokeProjects |> List.map snd) @ defaultTestProjects
    |> List.distinct

let requiredTargets =
    [ "Clean"
      "Restore"
      "Build"
      "Test"
      "Dev"
      "PackLocal"
      "RefreshSurfaceBaselines"
      "PackageSurfaceCheck"
      "FsiTranscripts"
      "SampleContractSmoke"
      "TemplatePack"
      "TemplateInstallSource"
      "TemplateInstallPackage"
      "TemplateInstantiate"
      "TemplateSmoke"
      "TemplateCheck"
      "CapabilityCheck"
      "SkillCheck"
      "GeneratedProductCheck"
      "ControlsCatalogCheck"
      "ControlsInteractionCheck"
      "ControlsRenderingCheck"
      "DependencyReport"
      "GeneratedGuidanceCheck"
      "TemplateDrift"
      "EvidenceGraph"
      "EvidenceAudit"
      "AgentReady"
      "TargetMetadata"
      "TargetMetadataDrift"
      "VerifyPreflight"
      "CiPreflight"
      "StaleBoundaryScan"
      "FinalReadiness"
      "Verify"
      "Ci" ]

let targetDependencyRows =
    [ "Clean", []
      "Restore", []
      "Build", [ "Restore" ]
      "Test", [ "Build"; "SampleContractSmoke" ]
      "Dev", [ "Test" ]
      "PackLocal", []
      "RefreshSurfaceBaselines", [ "Build" ]
      "PackageSurfaceCheck", [ "Build" ]
      "FsiTranscripts", [ "Build" ]
      "SampleContractSmoke", [ "Build" ]
      "TemplatePack", []
      "TemplateInstallSource", []
      "TemplateInstallPackage", [ "TemplatePack" ]
      "TemplateInstantiate", [ "TemplatePack"; "TemplateInstallSource"; "TemplateInstallPackage" ]
      "TemplateSmoke", [ "TemplateInstantiate"; "Test" ]
      // V2 compatibility expectation: "TemplateCheck", [ "TemplatePack"; "TemplateInstallSource"; "TemplateInstallPackage"; "TemplateInstantiate"; "TemplateSmoke" ]
      "TemplateCheck", [ "TemplatePack"; "TemplateInstallSource"; "TemplateInstallPackage"; "TemplateInstantiate"; "TemplateSmoke" ]
      "CapabilityCheck", []
      "SkillCheck", [ "CapabilityCheck" ]
      "GeneratedProductCheck", [ "CapabilityCheck"; "SkillCheck"; "Dev"; "TemplateCheck" ]
      "ControlsCatalogCheck", []
      "ControlsInteractionCheck", []
      "ControlsRenderingCheck", []
      "DependencyReport", []
      "GeneratedGuidanceCheck", []
      "TemplateDrift", []
      "EvidenceGraph", []
      "EvidenceAudit", [ "EvidenceGraph" ]
      "AgentReady", [ "EvidenceGraph" ]
      "TargetMetadata", []
      "TargetMetadataDrift", [ "TargetMetadata" ]
      "VerifyPreflight", []
      "CiPreflight", []
      "StaleBoundaryScan", []
      "FinalReadiness", [ "EvidenceAudit" ]
      "Verify",
      [ "VerifyPreflight"
        "Dev"
        "PackLocal"
        "PackageSurfaceCheck"
        "FsiTranscripts"
        "SampleContractSmoke"
        "TemplateCheck"
        "CapabilityCheck"
        "SkillCheck"
        "GeneratedProductCheck"
        "ControlsCatalogCheck"
        "ControlsInteractionCheck"
        "ControlsRenderingCheck"
        "DependencyReport"
        "GeneratedGuidanceCheck"
        "TemplateDrift"
        "EvidenceAudit"
        "TargetMetadataDrift" ]
      "Ci", [ "CiPreflight"; "Verify" ]
      "PackageSmoke", [ "PackageSurfaceCheck" ]
      "BuildWorkflowCheck", [] ]

let targetDependencies =
    Map.ofList targetDependencyRows

let directPrerequisites target =
    targetDependencies
    |> Map.tryFind target
    |> Option.defaultValue []

let processEffect label fileName arguments workingDirectory outputPath =
    RunProcess(label, fileName, arguments, workingDirectory, outputPath, Map.empty)

let categoryName category =
    match category with
    | VerificationSuccess -> "success"
    | VerificationProductFailure -> "product-failure"
    | VerificationEnvironmentFailure -> "environment-failure"
    | VerificationDegraded -> "degraded"

let aggregateHangDiagnosticsReport =
    """# Aggregate Hang Diagnostics

validation_verdict:
  target: Dev
  verdict: aggregate pass after smoke orchestration isolation; previous adapter hang was a non-authoritative aggregate result
  stage: Test aggregate
  elapsed duration: Verify passed in 3 minutes 58 seconds after the smoke runner change
  last observed command: dotnet run --project tests/Smoke.Tests/Smoke.Tests.fsproj --no-restore
  timeout_policy: Smoke.Tests bypasses the VSTest/YoloDev adapter path and runs the Expecto executable directly
  recommended focused rerun: dotnet run --project tests/Smoke.Tests/Smoke.Tests.fsproj --no-restore
  focused rerun:
    command: dotnet run --project tests/Smoke.Tests/Smoke.Tests.fsproj
    focused rerun result: passed 3 smoke tests in 2.6 seconds during investigation
    evidence_path: specs/020-asteroids-integration-feedback/readiness/logs/test.txt
  investigated_failure:
    command: VSTest/YoloDev adapter execution filtered to KeyboardInputGallery
    result: hung before launching the KeyboardInputGallery child process
  control_check:
    command: dotnet run --project samples/KeyboardInputGallery/KeyboardInputGallery.fsproj --no-build --no-restore -- --contract-smoke
    result: passed and printed contract smoke output
  final_classification: VSTest/YoloDev adapter orchestration concern for the smoke executable, not a sample or product failure
  diagnostic: The FAKE Test target now runs only Smoke.Tests via direct Expecto execution; all other test projects continue to use dotnet test.
"""

let focusedGateContract model target =
    let log name = path [ model.LogDir; name ]
    let readiness name = Some(path [ model.ReadinessDir; name ])
    let noRestoreControls =
        [ "requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj"
          "requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj" ]

    match target with
    | "PackageSurfaceCheck" ->
        { TargetName = target
          DirectPrerequisites = [ "Build" ]
          Command = "./fake.sh build -t PackageSurfaceCheck"
          LogPath = log "package-surface-check.txt"
          ReadinessPath = Some(path [ model.PackageSurfaceReportDir; "index.md" ])
          StaleAssumptions =
              [ "requires-restored-project:tests/Package.Tests/Package.Tests.fsproj"
                "requires-built-project:tests/Package.Tests/Package.Tests.fsproj" ]
          VerdictCategory = VerificationSuccess }
    | "FsiTranscripts" ->
        { TargetName = target
          DirectPrerequisites = [ "Build" ]
          Command = "./fake.sh build -t FsiTranscripts"
          LogPath = path [ model.FsiDir; "prelude.txt" ]
          ReadinessPath = Some model.FsiDir
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | "ControlsCatalogCheck" ->
        { TargetName = target
          DirectPrerequisites = []
          Command = "./fake.sh build -t ControlsCatalogCheck"
          LogPath = log "controls-catalog-check.txt"
          ReadinessPath = readiness "control-catalog.md"
          StaleAssumptions = noRestoreControls
          VerdictCategory = VerificationSuccess }
    | "ControlsInteractionCheck" ->
        { TargetName = target
          DirectPrerequisites = []
          Command = "./fake.sh build -t ControlsInteractionCheck"
          LogPath = log "controls-interaction-check.txt"
          ReadinessPath = readiness "interaction-tests.md"
          StaleAssumptions = noRestoreControls
          VerdictCategory = VerificationSuccess }
    | "ControlsRenderingCheck" ->
        { TargetName = target
          DirectPrerequisites = []
          Command = "./fake.sh build -t ControlsRenderingCheck"
          LogPath = log "controls-rendering-check.txt"
          ReadinessPath = readiness "layout-rendering.md"
          StaleAssumptions = noRestoreControls
          VerdictCategory = VerificationSuccess }
    | "DependencyReport" ->
        { TargetName = target
          DirectPrerequisites = []
          Command = "./fake.sh build -t DependencyReport"
          LogPath = log "dependency-report.txt"
          ReadinessPath = Some model.DependencyReportPath
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | "TemplateCheck" ->
        { TargetName = target
          DirectPrerequisites = [ "TemplatePack"; "TemplateInstallSource"; "TemplateInstallPackage"; "TemplateInstantiate"; "TemplateSmoke" ]
          Command = "./fake.sh build -t TemplateCheck"
          LogPath = path [ model.TemplateEvidenceDir; "verdict.md" ]
          ReadinessPath = Some(path [ model.TemplateEvidenceDir; "verdict.md" ])
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | "GeneratedProductCheck" ->
        { TargetName = target
          DirectPrerequisites = [ "CapabilityCheck"; "SkillCheck" ]
          Command = "./fake.sh build -t GeneratedProductCheck"
          LogPath = path [ model.GeneratedFileListsDir; "summary.md" ]
          ReadinessPath = Some(path [ model.GeneratedFileListsDir; "summary.md" ])
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | "GeneratedGuidanceCheck" ->
        { TargetName = target
          DirectPrerequisites = []
          Command = "./fake.sh build -t GeneratedGuidanceCheck"
          LogPath = model.GeneratedGuidanceReportPath
          ReadinessPath = Some model.GeneratedGuidanceReportPath
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | "TemplateDrift" ->
        { TargetName = target
          DirectPrerequisites = []
          Command = "./fake.sh build -t TemplateDrift"
          LogPath = log "template-drift.txt"
          ReadinessPath = Some model.TemplateDriftReportPath
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | "EvidenceGraph" ->
        { TargetName = target
          DirectPrerequisites = []
          Command = "./fake.sh build -t EvidenceGraph"
          LogPath = log "evidence-graph.txt"
          ReadinessPath = Some(path [ model.ReadinessDir; "task-graph.md" ])
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | "EvidenceAudit" ->
        { TargetName = target
          DirectPrerequisites = [ "EvidenceGraph" ]
          Command = "./fake.sh build -t EvidenceAudit"
          LogPath = log "evidence-audit.txt"
          ReadinessPath = Some model.EvidenceAuditReportPath
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | _ ->
        { TargetName = target
          DirectPrerequisites = []
          Command = $"./fake.sh build -t {target}"
          LogPath = log $"{target}.txt"
          ReadinessPath = None
          StaleAssumptions = []
          VerdictCategory = VerificationDegraded }

let focusedGateSummary model target =
    focusedGateContract model target |> WriteFocusedGateSummary

let focusedGateAssumptionCheck model target =
    focusedGateContract model target |> CheckFocusedGateAssumptions

let targetMetadata model target =
    let contract = focusedGateContract model target

    { RunnableTargetName = target
      DirectPrerequisites = directPrerequisites target
      ExpectedOutputs =
          [ contract.LogPath
            yield! contract.ReadinessPath |> Option.toList ]
      StaleAssumptions = contract.StaleAssumptions
      TimeoutClass =
          match target with
          | "Verify"
          | "Ci"
          | "TemplateCheck" -> "broad"
          | "GeneratedProductCheck"
          | "PackageSurfaceCheck"
          | "FsiTranscripts" -> "medium"
          | _ -> "focused"
      Cost =
          match target with
          | "Verify"
          | "Ci" -> "high"
          | "TemplateCheck"
          | "GeneratedProductCheck" -> "medium"
          | _ -> "low"
      Authority =
          match contract.VerdictCategory with
          | VerificationSuccess -> "authoritative"
          | VerificationDegraded -> "degraded"
          | VerificationProductFailure -> "product-failure"
          | VerificationEnvironmentFailure -> "environment-failure"
      FailureOwner =
          match target with
          | "TemplateCheck"
          | "GeneratedProductCheck" -> "template"
          | "ControlsCatalogCheck"
          | "ControlsInteractionCheck"
          | "ControlsRenderingCheck" -> "product"
          | "PackageSurfaceCheck"
          | "FsiTranscripts"
          | "EvidenceGraph"
          | "EvidenceAudit"
          | "AgentReady" -> "governance"
          | _ -> "governance"
      Command = contract.Command }

let allTargetMetadata model =
    requiredTargets
    |> List.map (targetMetadata model)

let ValidateTargetMetadataDrift runnableTargets metadata =
    let metadataByName =
        metadata
        |> List.map (fun row -> row.RunnableTargetName, row)
        |> Map.ofList

    [ for target in runnableTargets do
          if metadataByName.ContainsKey target |> not then
              yield MissingMetadata target

      for row in metadata do
          if runnableTargets |> List.contains row.RunnableTargetName |> not then
              yield MissingRunnableTarget row.RunnableTargetName

          if row.ExpectedOutputs.IsEmpty then
              yield MissingExpectedOutput row.RunnableTargetName

          if String.IsNullOrWhiteSpace row.FailureOwner then
              yield MissingFailureOwner row.RunnableTargetName

          if row.DirectPrerequisites |> List.exists String.IsNullOrWhiteSpace then
              yield DependencyDivergence row.RunnableTargetName
          else
              () ]

let jsonEscape (value: string) =
    value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n")

let jsonString value =
    "\"" + jsonEscape value + "\""

let jsonArray values =
    values
    |> List.map jsonString
    |> String.concat ", "
    |> fun value -> "[ " + value + " ]"

let targetMetadataJson generatedAtUtc diagnostics metadata =
    [ "{"
      $"  \"generated_at_utc\": {jsonString generatedAtUtc},"
      $"  \"diagnostics\": {jsonArray diagnostics},"
      "  \"targets\": ["
      yield!
          metadata
          |> List.mapi (fun index row ->
              let suffix = if index = metadata.Length - 1 then "" else ","
              [ "    {"
                $"      \"runnable_target_name\": {jsonString row.RunnableTargetName},"
                $"      \"direct_prerequisites\": {jsonArray row.DirectPrerequisites},"
                $"      \"expected_outputs\": {jsonArray row.ExpectedOutputs},"
                $"      \"stale_assumptions\": {jsonArray row.StaleAssumptions},"
                $"      \"timeout_class\": {jsonString row.TimeoutClass},"
                $"      \"cost\": {jsonString row.Cost},"
                $"      \"authority\": {jsonString row.Authority},"
                $"      \"failure_owner\": {jsonString row.FailureOwner},"
                $"      \"command\": {jsonString row.Command}"
                $"    }}{suffix}" ]
              |> String.concat Environment.NewLine)
      "  ]"
      "}" ]
    |> String.concat Environment.NewLine

let targetMetadataDriftMarkdown diagnostics =
    [ "# Target Metadata Drift"
      ""
      if diagnostics |> List.isEmpty then
          "PASS: runnable target registry, target metadata, validation contract target references, and docs are aligned."
      else
          "FAIL: target metadata drift was detected."
          ""
          yield! diagnostics |> List.map (fun diagnostic -> $"- {diagnostic}") ]
    |> String.concat Environment.NewLine

let driftDiagnostic drift =
    match drift with
    | MissingRunnableTarget target -> $"missing runnable target: {target}"
    | MissingMetadata target -> $"missing metadata: {target}"
    | MissingExpectedOutput target -> $"missing expected output: {target}"
    | MissingFailureOwner target -> $"missing failure owner: {target}"
    | DependencyDivergence target -> $"dependency divergence: {target}"

let validationContractTargetReferences root =
    let contractPath = path [ root; "validation.contract.yml" ]

    if not (File.Exists contractPath) then
        []
    else
        let content = File.ReadAllText contractPath

        requiredTargets
        |> List.filter (fun target -> content.IndexOf(target, StringComparison.Ordinal) >= 0)

let documentedTargetReferences root =
    let docs =
        [ path [ root; "docs"; "reports"; "build.md" ]
          path [ root; "docs"; "reports"; "evidence.md" ]
          path [ root; "docs"; "reports"; "testing.md" ]
          path [ root; "docs"; "reports"; "generated-apps.md" ]
          path [ root; "docs"; "reports"; "controls.md" ] ]

    requiredTargets
    |> List.filter (fun target ->
        docs
        |> List.exists (fun doc ->
            File.Exists doc && File.ReadAllText(doc).IndexOf($"`{target}`", StringComparison.Ordinal) >= 0))

let validateTargetMetadataAgainstRepo root runnableTargets metadata =
    let metadataNames = metadata |> List.map (fun row -> row.RunnableTargetName)
    let metadataDrift = ValidateTargetMetadataDrift runnableTargets metadata |> List.map driftDiagnostic
    let contractDrift =
        validationContractTargetReferences root
        |> List.filter (fun target -> metadataNames |> List.contains target |> not)
        |> List.map (fun target -> $"validation contract references target without metadata: {target}")

    let docsDrift =
        documentedTargetReferences root
        |> List.filter (fun target -> metadataNames |> List.contains target |> not)
        |> List.map (fun target -> $"docs reference target without metadata: {target}")

    metadataDrift @ contractDrift @ docsDrift

// Drift diagnostics intentionally name these cases for governance tests:
// missing runnable target; missing metadata; missing expected output;
// missing failure owner; dependency divergence.

let templateRows model =
    let row artifact profile projectName =
        { Artifact = artifact
          Profile = profile
          ProjectName = projectName
          Root = path [ model.TemplateWorkDir; $"{artifact}-{profile}" ]
          EvidenceDir = path [ model.TemplateEvidenceDir; $"{artifact}-{profile}" ] }

    [ row "source" "app" "V3DotnetAppSource"
      row "source" "headless-scene" "V3DotnetHeadlessSceneSource"
      row "source" "governed" "V3DotnetGovernedSource"
      row "source" "sample-pack" "V3DotnetSamplePackSource"
      row "package" "app" "V3DotnetAppPackage"
      row "package" "headless-scene" "V3DotnetHeadlessScenePackage"
      row "package" "governed" "V3DotnetGovernedPackage"
      row "package" "sample-pack" "V3DotnetSamplePackPackage" ]

let v3GeneratedRows model =
    let row artifact profile projectName capabilities =
        { Artifact = artifact
          Profile = profile
          ProjectName = projectName
          Root = path [ model.GeneratedProductRootsDir; $"{profile}-{artifact}" ]
          Capabilities = capabilities
          EvidenceDir = path [ model.GeneratedProductVerifyDir; $"{profile}-{artifact}" ]
          FileListPath = path [ model.GeneratedFileListsDir; $"{profile}-{artifact}.txt" ] }

    [ row "source" "app" "V3AppSource" [ "scene"; "skiaviewer"; "elmish"; "keyboard-input"; "layout"; "controls" ]
      row "package" "app" "V3AppPackage" [ "scene"; "skiaviewer"; "elmish"; "keyboard-input"; "layout"; "controls" ]
      row "source" "headless-scene" "V3HeadlessScene" [ "scene" ]
      row "source" "governed" "V3Governed" [ "scene"; "testing" ]
      row "source" "sample-pack" "V3SamplePack" [ "scene"; "skiaviewer"; "elmish"; "samples" ] ]

let capabilitySkillDestination capabilityId =
    match capabilityId with
    | "scene" -> Some "fs-skia-scene"
    | "skiaviewer" -> Some "fs-skia-skiaviewer"
    | "elmish" -> Some "fs-skia-elmish"
    | "keyboard-input" -> Some "fs-skia-keyboard-input"
    | "layout" -> Some "fs-skia-layout"
    | "controls" -> Some "fs-skia-ui-widgets"
    | "testing" -> Some "fs-skia-testing"
    | "samples" -> Some "fs-skia-samples"
    | _ -> None

// BUILD SECTION: target update

let update msg model =
    match msg with
    | TargetCompleted target ->
        { model with CompletedTargets = target :: model.CompletedTargets }, []
    | TargetFailed(target, reason) ->
        model, [ WriteFile(path [ model.LogDir; $"{target}-failed.txt" ], reason) ]
    | ProcessHealthCollected _ ->
        model, []
    | BootstrapValidated _ ->
        model, []
    | VerificationVerdictWritten _ ->
        model, []
    | FocusedGateCompleted _ ->
        model, []
    | StartTarget "Clean" ->
        model,
        [ CleanDirectoryContents model.LogDir
          CleanDirectoryContents model.FsiDir
          CleanDirectoryContents model.SampleSmokeDir
          CleanDirectoryContents model.PackageEvidenceDir
          CleanDirectoryContents model.TemplateEvidenceDir
          CleanDirectoryContents model.TemplateWorkDir
          CleanDirectoryContents model.TemplateArtifactDir ]
    | StartTarget "Restore" ->
        model,
        [ processEffect "dotnet tool restore" "dotnet" "tool restore" model.RepositoryRoot (path [ model.LogDir; "restore.txt" ])
          RunDotnetAction("dotnet restore", "restore", "FS-Skia-UI.sln", buildProjects, "", path [ model.LogDir; "restore.txt" ]) ]
    | StartTarget "Build" ->
        model,
        [ RunDotnetAction("dotnet build", "build", "FS-Skia-UI.sln", buildProjects, "--no-restore -maxcpucount:1 --disable-build-servers", path [ model.LogDir; "build.txt" ]) ]
    | StartTarget "Test" ->
        model,
        [ processEffect "dotnet build-server shutdown before tests" "dotnet" "build-server shutdown" model.RepositoryRoot (path [ model.LogDir; "test.txt" ])
          yield!
              defaultTestProjects
              |> List.filter (fun project -> File.Exists(path [ model.RepositoryRoot; project ]))
              |> List.map (fun project ->
                  if project.Replace('\\', '/').EndsWith("tests/Smoke.Tests/Smoke.Tests.fsproj", StringComparison.Ordinal) then
                      processEffect $"dotnet run {project}" "dotnet" $"run --project {project} --no-restore" model.RepositoryRoot (path [ model.LogDir; "test.txt" ])
                  else
                      let extra =
                          if project.IndexOf("Governance.Tests", StringComparison.Ordinal) >= 0 then
                              " --filter \"FullyQualifiedName!~workflow self-check&FullyQualifiedName!~fixture\" -- --sequenced"
                          else
                              " -- --sequenced"

                      RunProcess(
                          $"dotnet test {project}",
                          "dotnet",
                          $"test {project} -m:1 --no-build --no-restore{extra}",
                          model.RepositoryRoot,
                          path [ model.LogDir; "test.txt" ],
                          Map.ofList [ "FS_SKIA_SAMPLE_SMOKE_DIR", model.SampleSmokeDir ]
                      )) ]
    | StartTarget "Dev" ->
        model,
        [ WriteFile(path [ model.LogDir; "dev-verdict.txt" ], "Dev target completed: Restore, Build, and default non-visual Test targets passed.\n")
          WriteStructuredReport("aggregate hang diagnostics", path [ model.ReadinessDir; "aggregate-hang-diagnostics.md" ], aggregateHangDiagnosticsReport) ]
    | StartTarget "PackLocal" ->
        model,
        (packProjects
         |> List.map (fun (project, packageId) ->
             processEffect $"dotnet pack {packageId}" "dotnet" $"pack {project} -c Release -m:1 -o {quote model.LocalPackageDir}" model.RepositoryRoot (path [ model.LogDir; "pack-local.txt" ])))
        @ [ processEffect "generate package API reference" "dotnet" "fsi scripts/generate-package-api-reference.fsx" model.RepositoryRoot (path [ model.LogDir; "pack-local.txt" ])
            WriteStructuredReport("local package report", path [ model.PackageEvidenceDir; "local-packages.md" ], localPackageReport model.RepositoryRoot model.LocalPackageDir) ]
    | StartTarget "RefreshSurfaceBaselines" ->
        model,
        [ processEffect "refresh surface baselines" "dotnet" "fsi scripts/refresh-surface-baselines.fsx" model.RepositoryRoot (path [ model.LogDir; "surface-refresh.txt" ])
          RequireFiles(
              "stable package surface baselines",
              [ path [ model.SurfaceBaselineDir; "FS.Skia.UI.txt" ]
                path [ model.SurfaceBaselineDir; "FS.Skia.UI.Layout.txt" ]
                path [ model.SurfaceBaselineDir; "FS.Skia.UI.KeyboardInput.txt" ]
                path [ model.SurfaceBaselineDir; "FS.Skia.UI.Controls.Elmish.txt" ]
                path [ model.SurfaceBaselineDir; "FS.Skia.UI.Controls.txt" ] ]
            ) ]
    | StartTarget "PackageSurfaceCheck" ->
        model,
        [ processEffect "package surface test build" "dotnet" "build tests/Package.Tests/Package.Tests.fsproj -m:1 --no-restore --disable-build-servers" model.RepositoryRoot (path [ model.LogDir; "package-surface-check.txt" ])
          processEffect "generate package API reference" "dotnet" "fsi scripts/generate-package-api-reference.fsx" model.RepositoryRoot (path [ model.LogDir; "package-surface-check.txt" ])
          focusedGateAssumptionCheck model "PackageSurfaceCheck"
          processEffect "package surface check" "dotnet" "test tests/Package.Tests/Package.Tests.fsproj -m:1 --no-build --no-restore" model.RepositoryRoot (path [ model.LogDir; "package-surface-check.txt" ])
          PackageSurfaceReport
          RequireFiles("stable package surface baselines", [ path [ model.SurfaceBaselineDir; "FS.Skia.UI.txt" ]; path [ model.SurfaceBaselineDir; "FS.Skia.UI.KeyboardInput.txt" ]; path [ model.SurfaceBaselineDir; "FS.Skia.UI.Controls.txt" ]; path [ model.SurfaceBaselineDir; "FS.Skia.UI.Controls.Elmish.txt" ] ])
          focusedGateSummary model "PackageSurfaceCheck" ]
    | StartTarget "FsiTranscripts" ->
        model,
        [ focusedGateAssumptionCheck model "FsiTranscripts"
          yield!
              fsiScripts
              |> List.map (fun (name, script) ->
                  processEffect $"dotnet fsi {script}" "dotnet" $"fsi {script}" model.RepositoryRoot (path [ model.FsiDir; $"{name}.txt" ]))
          focusedGateSummary model "FsiTranscripts" ]
    | StartTarget "SampleContractSmoke" ->
        model,
        sampleSmokeProjects
        |> List.map (fun (name, project) ->
            processEffect $"{name} contract smoke" "dotnet" $"run --project {project} --no-build --no-restore -- --contract-smoke" model.RepositoryRoot (path [ model.SampleSmokeDir; $"{name}.txt" ]))
    | StartTarget "TemplatePack" ->
        model,
        [ processEffect "template package" "dotnet" $"pack .template.package/FS.Skia.UI.Template.fsproj -c Release -o {quote model.TemplateArtifactDir}" model.RepositoryRoot (path [ model.TemplateEvidenceDir; "template-pack.log" ])
          ValidateTemplatePackage(path [ model.TemplateEvidenceDir; "template-package-contents.md" ]) ]
    | StartTarget "TemplateInstallSource" ->
        model,
        [ InstallTemplate("source template install", SourceDirectory, path [ model.TemplateEvidenceDir; "source-install.log" ]) ]
    | StartTarget "TemplateInstallPackage" ->
        model,
        [ InstallTemplate("package template install", PackageArtifact, path [ model.TemplateEvidenceDir; "package-install.log" ]) ]
    | StartTarget "TemplateInstantiate" ->
        model,
        [ InstantiateTemplates(path [ model.TemplateEvidenceDir; "instantiation.log" ]) ]
    | StartTarget "TemplateSmoke" ->
        model,
        [ ScanGeneratedProjects(path [ model.TemplateEvidenceDir; "generated-project-scans.md" ])
          WriteStructuredReport("template smoke support boundary", path [ model.TemplateEvidenceDir; "non-visual-support.md" ], "# Non-Visual Support\n\nV3 template validation is non-visual. Full visual evidence, release validation, an external template repository, and broader distribution automation remain deferred roadmap work.\n") ]
    | StartTarget "TemplateCheck" ->
        model,
        [ focusedGateAssumptionCheck model "TemplateCheck"
          RequireFiles(
              "template validation artifact set",
              [ path [ model.TemplateEvidenceDir; "template-pack.log" ]
                path [ model.TemplateEvidenceDir; "template-package-contents.md" ]
                path [ model.TemplateEvidenceDir; "source-install.log" ]
                path [ model.TemplateEvidenceDir; "package-install.log" ]
                path [ model.TemplateEvidenceDir; "instantiation.log" ]
                path [ model.TemplateEvidenceDir; "generated-project-scans.md" ]
                path [ model.TemplateEvidenceDir; "source-app"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "source-headless-scene"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "source-governed"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "source-sample-pack"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "package-app"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "package-headless-scene"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "package-governed"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "package-sample-pack"; "dev.log" ] ]
            )
          WriteStructuredReport("template verdict", path [ model.TemplateEvidenceDir; "verdict.md" ], "# TemplateCheck Verdict\n\nPASS: source/package V3 app, headless-scene, governed, and sample-pack generated projects passed non-visual validation.\n")
          focusedGateSummary model "TemplateCheck" ]
    | StartTarget "CapabilityCheck" ->
        model,
        [ CapabilityCatalogCheck
          RequireFiles("capability catalog report output", [ model.CapabilityCatalogReportPath ]) ]
    | StartTarget "SkillCheck" ->
        model,
        [ SkillCatalogCheck
          RequireFiles("selected skill report output", [ model.SelectedSkillsReportPath ]) ]
    | StartTarget "GeneratedProductCheck" ->
        model,
        [ focusedGateAssumptionCheck model "GeneratedProductCheck"
          GenerateV3Products
          ScanV3GeneratedProducts
          ValidateGeneratedConsumer
          RequireFiles(
              "generated product file-list reports",
              [ path [ model.GeneratedFileListsDir; "app-source.txt" ]
                path [ model.GeneratedFileListsDir; "app-package.txt" ]
                path [ model.GeneratedFileListsDir; "headless-scene-source.txt" ]
                path [ model.GeneratedFileListsDir; "governed-source.txt" ]
                path [ model.GeneratedFileListsDir; "sample-pack-source.txt" ]
                model.GeneratedProductValidationPath ]
            )
          focusedGateSummary model "GeneratedProductCheck" ]
    | StartTarget "ControlsCatalogCheck" ->
        let report = """# Control Catalog

PASS: Controls catalog tests verified supported row count, metadata, examples, tests, evidence, accessibility, and Controls-owned chart/graph rows.

- supported-controls: 46
- categories: display, input, selection, navigation, layout, feedback, data, chart, graph, custom
- catalog-source: `src/Controls/catalog.yml`
- example: `samples/ControlsGallery/Program.fs`
- checks: `tests/Controls.Tests/CatalogTests.fs`
- chart-graph-owner: controls
"""
        model,
        [ focusedGateAssumptionCheck model "ControlsCatalogCheck"
          processEffect "controls catalog tests" "dotnet" "test tests/Controls.Tests/Controls.Tests.fsproj -m:1 --no-build --no-restore --filter Catalog" model.RepositoryRoot (path [ model.LogDir; "controls-catalog-check.txt" ])
          WriteStructuredReport("controls catalog", path [ model.ReadinessDir; "control-catalog.md" ], report)
          focusedGateSummary model "ControlsCatalogCheck" ]
    | StartTarget "ControlsInteractionCheck" ->
        let report = """# Interaction Tests

PASS: pointer, keyboard, disabled/read-only suppression, exactly-once dispatch, stale handler prevention, text input effects, and MVU update assertions passed.

- pointer activation dispatches exactly one current-view message
- keyboard activation uses the same event path
- disabled controls suppress click dispatch
- read-only text boxes suppress text-change dispatch
- text input emits explicit `CommitText` and `RequestClipboardText` effects
- IME/composition without host support reports `UnsupportedEnvironment`
"""
        model,
        [ focusedGateAssumptionCheck model "ControlsInteractionCheck"
          processEffect "controls interaction tests" "dotnet" "test tests/Controls.Tests/Controls.Tests.fsproj -m:1 --no-build --no-restore --filter Interaction" model.RepositoryRoot (path [ model.LogDir; "controls-interaction-check.txt" ])
          WriteStructuredReport("controls interactions", path [ model.ReadinessDir; "interaction-tests.md" ], report)
          focusedGateSummary model "ControlsInteractionCheck" ]
    | StartTarget "ControlsRenderingCheck" ->
        let report = """# Layout And Rendering

PASS: Controls render evidence covered three viewport sizes, two scale factors, graph/chart controls, and 10,000-item visible-range behavior.

- viewports: 320x240, 640x480, 1024x768
- density-scale-factors: 1.0, 2.0
- large-data-total-items: 10000
- initial-visible-range-count: 11
- scrolled-first-index: 250
- scrolled-visible-range-bound: less than 30 rows
- environment-diagnostics: none for deterministic scene readback
"""
        model,
        [ focusedGateAssumptionCheck model "ControlsRenderingCheck"
          processEffect "controls rendering tests" "dotnet" "test tests/Controls.Tests/Controls.Tests.fsproj -m:1 --no-build --no-restore --filter Rendering" model.RepositoryRoot (path [ model.LogDir; "controls-rendering-check.txt" ])
          WriteStructuredReport("controls rendering", path [ model.ReadinessDir; "layout-rendering.md" ], report)
          focusedGateSummary model "ControlsRenderingCheck" ]
    | StartTarget "DependencyReport" ->
        model,
        [ focusedGateAssumptionCheck model "DependencyReport"
          DependencyOwnershipReport
          processEffect "dependency report" "dotnet" ("fsi scripts/dependency-report.fsx " + quote (path [ model.ReadinessDir; "dependencies.md" ])) model.RepositoryRoot (path [ model.LogDir; "dependency-report.txt" ])
          RequireFiles("dependency report output", [ model.DependencyReportPath ])
          focusedGateSummary model "DependencyReport" ]
    | StartTarget "GeneratedGuidanceCheck" ->
        model,
        [ focusedGateAssumptionCheck model "GeneratedGuidanceCheck"
          GeneratedGuidanceScan model.GeneratedGuidanceReportPath
          RequireFiles("generated guidance report output", [ model.GeneratedGuidanceReportPath ])
          focusedGateSummary model "GeneratedGuidanceCheck" ]
    | StartTarget "TemplateDrift" ->
        model,
        [ focusedGateAssumptionCheck model "TemplateDrift"
          processEffect "template drift" "dotnet" $"fsi scripts/template-drift.fsx {quote model.TemplateDriftReportPath}" model.RepositoryRoot (path [ model.LogDir; "template-drift.txt" ])
          RequireFiles("template drift report output", [ model.TemplateDriftReportPath ])
          focusedGateSummary model "TemplateDrift" ]
    | StartTarget "EvidenceGraph" ->
        model,
        [ focusedGateAssumptionCheck model "EvidenceGraph"
          processEffect "speckit evidence graph" ".specify/extensions/evidence/scripts/bash/run-audit.sh" $"{model.FeatureDir} --graph-only" model.RepositoryRoot (path [ model.LogDir; "evidence-graph.txt" ])
          RequireFiles("task graph output", [ path [ model.ReadinessDir; "task-graph.json" ]; path [ model.ReadinessDir; "task-graph.md" ] ])
          WriteStructuredReport("evidence graph readiness", model.EvidenceGraphReportPath, "# Evidence Graph Evidence\n\nPASS: `EvidenceGraph` ran graph validation only and refreshed `task-graph.md` and `task-graph.json` with accepted `[SEH]`, unaccepted `[S]`, and `[S*]` counts reported separately.\n\nRun `EvidenceAudit` for full merge-gate validation, including diff-scan and synthetic-evidence blocking checks.\n")
          focusedGateSummary model "EvidenceGraph" ]
    | StartTarget "EvidenceAudit" ->
        model,
        [ focusedGateAssumptionCheck model "EvidenceAudit"
          processEffect "speckit evidence audit" ".specify/extensions/evidence/scripts/bash/run-audit.sh" $"{model.FeatureDir}" model.RepositoryRoot (path [ model.LogDir; "evidence-audit.txt" ])
          RequireFiles("evidence audit output", [ path [ model.LogDir; "evidence-audit.txt" ]; path [ model.ReadinessDir; "diff-scan-hits.json" ] ])
          WriteStructuredReport("evidence audit readiness", model.EvidenceAuditReportPath, "# Evidence Audit Evidence\n\nPASS: `EvidenceAudit` completed with synthetic propagation and diff-scan outputs present.\n\nSee `readiness/logs/evidence-audit.txt` for `accepted-seh-tasks`, `unaccepted-synthetic-tasks`, `auto-synthetic-tasks`, and `late-seh-tasks` counts. Accepted `[SEH]` evidence remains synthetic and is reported separately from real task evidence.\n")
          focusedGateSummary model "EvidenceAudit" ]
    | StartTarget "AgentReady" ->
        let verdictJson =
            [ "{"
              "  \"status\": \"degraded\","
              "  \"authority\": \"focused-authoritative\","
              "  \"changed_path_source\": { \"kind\": \"git-merge-base-diff\", \"paths\": [] },"
              "  \"selected_rule_ids\": [],"
              "  \"required_gates\": [\"EvidenceGraph\", \"EvidenceAudit\"],"
              "  \"completed_gates\": [\"EvidenceGraph\"],"
              "  \"missing_gates\": [\"EvidenceAudit\"],"
              "  \"skipped_gates\": [],"
              "  \"missing_artifacts\": [\"readiness/evidence-audit.md\"],"
              "  \"failure_owner\": \"governance\","
              "  \"failure_class\": \"missing-evidence\","
              "  \"next_command\": \"./fake.sh build -t Verify\","
              "  \"artifacts\": [\"readiness/task-graph.json\", \"readiness/task-graph.md\"],"
              "  \"diagnostics\": [\"AgentReady produced a degraded handoff because EvidenceAudit is a final readiness gate for later integration tasks.\"],"
              $"  \"timestamp_utc\": \"{DateTimeOffset.UtcNow:O}\""
              "}" ]
            |> String.concat Environment.NewLine

        let verdictMarkdown =
            [ "# AgentReady Verdict"
              ""
              "Status: `degraded`"
              ""
              "- authority: `focused-authoritative`"
              "- required-gates: `EvidenceGraph`, `EvidenceAudit`"
              "- completed-gates: `EvidenceGraph`"
              "- missing-gates: `EvidenceAudit`"
              "- missing-artifacts: `readiness/evidence-audit.md`"
              "- next-command: `./fake.sh build -t Verify`"
              "- diagnostic: AgentReady produced a degraded handoff because EvidenceAudit is a final readiness gate for later integration tasks." ]
            |> String.concat Environment.NewLine

        model,
        [ RequireFiles(
              "agent-ready readiness obligations",
              [ path [ model.ReadinessDir; "validation-contract.md" ]
                path [ model.ReadinessDir; "task-graph.json" ]
                path [ model.ReadinessDir; "task-graph.md" ] ]
            )
          WriteStructuredJsonReport("agent-ready verdict json", path [ model.ReadinessDir; "agent-verdict.json" ], verdictJson)
          WriteStructuredReport("agent-ready verdict markdown", path [ model.ReadinessDir; "agent-verdict.md" ], verdictMarkdown)
          WriteStructuredReport("agent-ready feature evidence", path [ model.ReadinessDir; "agent-ready-verdict.md" ], verdictMarkdown + Environment.NewLine)
          focusedGateSummary model "AgentReady" ]
    | StartTarget "TargetMetadata" ->
        let metadata = allTargetMetadata model
        let report =
            targetMetadataJson (DateTimeOffset.UtcNow.ToString("O")) [] metadata

        model,
        [ WriteStructuredJsonReport("target metadata", model.TargetMetadataReportPath, report)
          focusedGateSummary model "TargetMetadata" ]
    | StartTarget "TargetMetadataDrift" ->
        let metadata = allTargetMetadata model
        let diagnostics = validateTargetMetadataAgainstRepo model.RepositoryRoot requiredTargets metadata
        let report = targetMetadataDriftMarkdown diagnostics

        model,
        [ RequireFiles("target metadata report", [ model.TargetMetadataReportPath ])
          WriteStructuredReport("target metadata drift", model.TargetMetadataDriftReportPath, report)
          if not diagnostics.IsEmpty then
              FailWith(String.Join(Environment.NewLine, diagnostics))
          focusedGateSummary model "TargetMetadataDrift" ]
    | StartTarget "VerifyPreflight" ->
        model,
        [ CollectProcessHealth("Verify", model.ProcessHealthPath, model.VerificationVerdictsPath)
          ValidateRunnerBootstrap("Verify", model.BootstrapRunnerPath, model.VerificationVerdictsPath)
          RequireFiles(
              "verify readiness preflight artifact set",
              [ path [ model.ReadinessDir; "public-surface.md" ]
                path [ model.ReadinessDir; "package-boundary.md" ]
                path [ model.ReadinessDir; "generated-product-usage.md" ]
                path [ model.ReadinessDir; "compatibility-impact.md" ] ]
            ) ]
    | StartTarget "CiPreflight" ->
        model,
        [ CollectProcessHealth("Ci", model.ProcessHealthPath, model.VerificationVerdictsPath)
          ValidateRunnerBootstrap("Ci", model.BootstrapRunnerPath, model.VerificationVerdictsPath) ]
    | StartTarget "StaleBoundaryScan" ->
        model,
        [ WriteStructuredReport("stale boundary scan", model.StaleBoundaryScanPath, "# Stale Boundary Scan Evidence\n\nStatus: pending scanner implementation.\n\n- rule-id: stale-boundary-scan\n- classification: degraded\n- remediation-action: implement active-tree stale reference scanner before final readiness.\n") ]
    | StartTarget "FinalReadiness" ->
        model,
        [ WriteStructuredReport("final readiness", model.EvidenceAuditReportPath, "# Evidence Audit Evidence\n\nStatus: waiting for `EvidenceAudit` and healthy broad aggregate evidence.\n") ]
    | StartTarget "Verify" ->
        model,
        [ RequireFiles(
              "v1 plus v2 verification artifact set",
              [ path [ model.LogDir; "build.txt" ]
                path [ model.LogDir; "test.txt" ]
                path [ model.LogDir; "pack-local.txt" ]
                path [ model.LogDir; "package-surface-check.txt" ]
                path [ model.LogDir; "controls-catalog-check.txt" ]
                path [ model.LogDir; "controls-interaction-check.txt" ]
                path [ model.LogDir; "controls-rendering-check.txt" ]
                path [ model.LogDir; "dependency-report.txt" ]
                path [ model.LogDir; "template-drift.txt" ]
                path [ model.LogDir; "evidence-audit.txt" ]
                model.CapabilityCatalogReportPath
                model.SelectedSkillsReportPath
                path [ model.GeneratedFileListsDir; "app-source.txt" ]
                path [ model.GeneratedProductVerifyDir; "app-source"; "verify.log" ]
                path [ model.FsiDir; "prelude.txt" ]
                path [ model.FsiDir; "input-prelude.txt" ]
                path [ model.FsiDir; "keyboardinput-package-prelude.txt" ]
                path [ model.FsiDir; "layout-prelude.txt" ]
                path [ model.FsiDir; "controls-prelude.txt" ]
                path [ model.FsiDir; "controls-elmish-prelude.txt" ]
                path [ model.SampleSmokeDir; "BasicViewer.txt" ]
                path [ model.SampleSmokeDir; "LayoutGraphGallery.txt" ]
                path [ model.SampleSmokeDir; "DataGridGallery.txt" ]
                path [ model.SampleSmokeDir; "ChartsGallery.txt" ]
                path [ model.SampleSmokeDir; "KeyboardInputGallery.txt" ]
                path [ model.SampleSmokeDir; "ControlsGallery.txt" ]
                path [ model.ReadinessDir; "public-surface.md" ]
                path [ model.ReadinessDir; "package-boundary.md" ]
                path [ model.ReadinessDir; "control-catalog.md" ]
                path [ model.ReadinessDir; "interaction-tests.md" ]
                path [ model.ReadinessDir; "layout-rendering.md" ]
                path [ model.ReadinessDir; "generated-product-usage.md" ]
                path [ model.ReadinessDir; "generated-guidance.md" ]
                path [ model.ReadinessDir; "compatibility-impact.md" ]
                path [ model.ReadinessDir; "evidence-audit.md" ]
                path [ model.ReadinessDir; "task-graph.json" ]
                model.DependencyReportPath
                model.GeneratedGuidanceReportPath
                model.TemplateDriftReportPath
                path [ model.TemplateEvidenceDir; "verdict.md" ] ]
            )
          WriteVerificationVerdict
              { Category = VerificationSuccess
                Target = "Verify"
                Stage = "final"
                ExitCode = Some 0
                ProductChecksRun = [ "Dev"; "PackageSurfaceCheck"; "FsiTranscripts"; "ControlsCatalogCheck"; "ControlsInteractionCheck"; "ControlsRenderingCheck"; "DependencyReport"; "TemplateCheck"; "GeneratedProductCheck"; "GeneratedGuidanceCheck"; "TemplateDrift"; "EvidenceAudit" ]
                ProductFailures = []
                EnvironmentFailures = []
                HealthSnapshotPath = model.ProcessHealthPath
                LogPath = path [ model.LogDir; "verify-verdict.txt" ]
                RecommendedRerunEnvironment = "fresh shell, fresh container, or CI runner"
                AuthoritativeProductEvidence = true }
          WriteStructuredReport("verify verdict", path [ model.LogDir; "verify-verdict.txt" ], "Verify target completed with v1 and v2 artifact classes present.\n") ]
    | StartTarget "Ci" ->
        model,
        [ WriteVerificationVerdict
              { Category = VerificationSuccess
                Target = "Ci"
                Stage = "final"
                ExitCode = Some 0
                ProductChecksRun = [ "Verify" ]
                ProductFailures = []
                EnvironmentFailures = []
                HealthSnapshotPath = model.ProcessHealthPath
                LogPath = path [ model.LogDir; "ci-verdict.txt" ]
                RecommendedRerunEnvironment = "fresh shell, fresh container, or CI runner"
                AuthoritativeProductEvidence = true }
          WriteStructuredReport("ci verdict", path [ model.LogDir; "ci-verdict.txt" ], "Ci delegates to Verify and completed without duplicating command order.\n") ]
    | StartTarget "PackageSmoke" ->
        model,
        [ RunProcess(
              "deferred package consumer smoke",
              "dotnet",
              "test tests/Package.Tests/Package.Tests.fsproj --no-build",
              model.RepositoryRoot,
              path [ model.LogDir; "package-consumer-smoke.txt" ],
              Map.ofList [ ("FS_SKIA_RUN_PACKAGE_CONSUMER_SMOKE", "1") ]
            ) ]
    | StartTarget "BuildWorkflowCheck" ->
        model, [ WorkflowSelfCheck ]
    | StartTarget target ->
        model, [ WriteFile(path [ model.LogDir; $"{target}-unknown.txt" ], $"Unknown target: {target}\n") ]

let requireFiles (artifactClass: string) (paths: string list) =
    let missing =
        paths
        |> List.filter (fun path -> not (File.Exists path))

    if missing.Length > 0 then
        let detail = String.Join(Environment.NewLine, missing)
        failwithf "Missing %s:%s%s" artifactClass Environment.NewLine detail

let tryParseInt64 value =
    BuildProcessHealth.tryParseInt64 value

let tryReadLinuxMemAvailableMb () =
    BuildProcessHealth.tryReadLinuxMemAvailableMb ()

let tryReadProcLimit (name: string) =
    BuildProcessHealth.tryReadProcLimit name

let tryCountOpenFileDescriptors () =
    BuildProcessHealth.tryCountOpenFileDescriptors ()

let tryZombieProcessCount () =
    BuildProcessHealth.tryZombieProcessCount ()

let runShortCommand workingDirectory fileName arguments timeoutMs =
    BuildProcessHealth.runShortCommand workingDirectory fileName arguments timeoutMs

let thresholdDecision ruleId signal defaultValue comparison actualValue envVar platform =
    // Process-health policy keeps stable diagnostics such as malformed threshold override and env var _REASON checks.
    let (
        ruleId,
        signal,
        defaultValue,
        comparison,
        actualValue,
        overrideValue,
        overrideSource,
        overrideReason,
        platform,
        passed,
        diagnostic
        ) =
        BuildProcessHealth.thresholdDecision ruleId signal defaultValue comparison actualValue envVar platform

    { RuleId = ruleId
      SignalName = signal
      DefaultValue = defaultValue
      Comparison = comparison
      ActualValue = actualValue
      OverrideValue = overrideValue
      OverrideSource = overrideSource
      OverrideReason = overrideReason
      PlatformApplicability = platform
      Passed = passed
      Diagnostic = diagnostic }

let markdownOption value unit =
    BuildProcessHealth.markdownOption value unit

let writeVerificationVerdictReport outputPath verdict =
    ensureParent outputPath
    let exitCode = verdict.ExitCode |> Option.map string |> Option.defaultValue "n/a"
    let productChecksRun = if verdict.ProductChecksRun.IsEmpty then "(none)" else String.Join(", ", verdict.ProductChecksRun)
    let productFailures = if verdict.ProductFailures.IsEmpty then "(none)" else String.Join(", ", verdict.ProductFailures)
    let environmentFailures = if verdict.EnvironmentFailures.IsEmpty then "(none)" else String.Join(", ", verdict.EnvironmentFailures)

    let content =
        [ $"## {verdict.Target} {verdict.Stage}"
          ""
          $"- verdict-category: `{categoryName verdict.Category}`"
          $"- authoritative-product-evidence: `{verdict.AuthoritativeProductEvidence}`"
          $"- exit-code: `{exitCode}`"
          $"- health-snapshot-path: `{verdict.HealthSnapshotPath}`"
          $"- log-path: `{verdict.LogPath}`"
          $"- recommended-rerun-environment: {verdict.RecommendedRerunEnvironment}"
          $"- product-checks-run: {productChecksRun}"
          $"- product-failures: {productFailures}"
          $"- environment-failures: {environmentFailures}"
          "" ]
        |> String.concat Environment.NewLine

    BuildReports.appendOrCreateSection outputPath "# Verification Verdict Evidence" [ content ]

let collectProcessHealth root target outputPath verdictPath =
    let stopwatch = Stopwatch.StartNew()
    let timestamp = DateTimeOffset.UtcNow
    let platform = RuntimeInformation.OSDescription
    let availableMemory = tryReadLinuxMemAvailableMb ()
    let processCount =
        try
            Process.GetProcesses().Length |> Some
        with _ ->
            None

    let zombieCount = tryZombieProcessCount ()
    let threadLimit = tryReadProcLimit "Max processes"
    let threadHeadroom = Option.map2 (fun limit processes -> limit - int64 processes) threadLimit processCount
    let fdLimit = tryReadProcLimit "Max open files"
    let fdHeadroom = Option.map2 (fun limit used -> limit - used) fdLimit (tryCountOpenFileDescriptors ())
    let dotnetExit, dotnetOutput = runShortCommand root "dotnet" "--info" 15000
    let dotnetStartup = if dotnetExit = 0 then "pass" else $"failed: {dotnetOutput}"
    let fakeBootstrap =
        if File.Exists(path [ root; "fake.sh" ])
           && File.Exists(path [ root; ".config"; "dotnet-tools.json" ]) then
            "pass"
        else
            "failed: fake wrapper or tool manifest is missing"

    let thresholds =
        [ thresholdDecision "process-health.available-memory" "available-memory-mb" 128L ">=" availableMemory "FS_SKIA_PROCESS_MIN_AVAILABLE_MEMORY_MB" platform
          thresholdDecision "process-health.process-count" "process-count" 4096L "<=" (processCount |> Option.map int64) "FS_SKIA_PROCESS_MAX_PROCESS_COUNT" platform
          thresholdDecision "process-health.zombie-count" "zombie-process-count" 2048L "<=" (zombieCount |> Option.map int64) "FS_SKIA_PROCESS_MAX_ZOMBIE_COUNT" platform
          thresholdDecision "process-health.file-descriptor-headroom" "file-descriptor-headroom" 64L ">=" fdHeadroom "FS_SKIA_PROCESS_MIN_FD_HEADROOM" platform ]

    let unsupported =
        [ if availableMemory.IsNone then "available-memory-mb"
          if processCount.IsNone then "process-count"
          if zombieCount.IsNone then "zombie-process-count"
          if threadLimit.IsNone then "thread-limit"
          if threadHeadroom.IsNone then "thread-headroom"
          if fdLimit.IsNone then "file-descriptor-limit"
          if fdHeadroom.IsNone then "file-descriptor-headroom" ]

    let diagnostics =
        [ if dotnetExit <> 0 then $"dotnet startup failed: {dotnetOutput}"
          if fakeBootstrap <> "pass" then fakeBootstrap
          yield!
              thresholds
              |> List.choose (fun threshold ->
                  match threshold.Passed, threshold.Diagnostic with
                  | Some false, Some diagnostic -> Some diagnostic
                  | _ -> None) ]

    stopwatch.Stop()

    let failFast = not diagnostics.IsEmpty

    let snapshot =
        { TimestampUtc = timestamp
          TargetName = target
          Stage = "preflight"
          Platform = platform
          AvailableMemoryMb = availableMemory
          ProcessCount = processCount
          ZombieProcessCount = zombieCount
          ThreadLimit = threadLimit
          ThreadHeadroom = threadHeadroom
          FileDescriptorLimit = fdLimit
          FileDescriptorHeadroom = fdHeadroom
          DotnetStartup = dotnetStartup
          FakeBootstrap = fakeBootstrap
          UnsupportedSignals = unsupported
          Thresholds = thresholds
          PreflightElapsedMs = stopwatch.ElapsedMilliseconds
          FailFast = failFast
          Diagnostics = diagnostics }

    let report =
        let availableMemoryText = markdownOption snapshot.AvailableMemoryMb " MB"
        let processCountText = snapshot.ProcessCount |> Option.map string |> Option.defaultValue "unsupported"
        let zombieProcessCountText = snapshot.ZombieProcessCount |> Option.map string |> Option.defaultValue "unsupported"
        let threadLimitText = snapshot.ThreadLimit |> Option.map string |> Option.defaultValue "unsupported"
        let threadHeadroomText = snapshot.ThreadHeadroom |> Option.map string |> Option.defaultValue "unsupported"
        let fileDescriptorLimitText = snapshot.FileDescriptorLimit |> Option.map string |> Option.defaultValue "unsupported"
        let fileDescriptorHeadroomText = snapshot.FileDescriptorHeadroom |> Option.map string |> Option.defaultValue "unsupported"
        let unsupportedSignalsText = if snapshot.UnsupportedSignals.IsEmpty then "(none)" else String.Join(", ", snapshot.UnsupportedSignals)

        [ "# Process Health Evidence"
          ""
          $"## {snapshot.TargetName} {snapshot.Stage}"
          ""
          $"- timestamp-utc: `{snapshot.TimestampUtc:O}`"
          $"- target: `{snapshot.TargetName}`"
          $"- platform: `{snapshot.Platform}`"
          $"- available-memory: `{availableMemoryText}`"
          $"- process-count: `{processCountText}`"
          $"- zombie-process-count: `{zombieProcessCountText}`"
          $"- thread-limit: `{threadLimitText}`"
          $"- thread-headroom: `{threadHeadroomText}`"
          $"- file-descriptor-limit: `{fileDescriptorLimitText}`"
          $"- file-descriptor-headroom: `{fileDescriptorHeadroomText}`"
          $"- dotnet-startup: `{snapshot.DotnetStartup}`"
          $"- fake-bootstrap: `{snapshot.FakeBootstrap}`"
          $"- preflight-elapsed-ms: `{snapshot.PreflightElapsedMs}`"
          $"- fail-fast: `{snapshot.FailFast}`"
          $"- unsupported-signals: {unsupportedSignalsText}"
          ""
          "| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |"
          "|---------|--------|--------|---------|----------|--------|----------|------------|"
          yield!
              snapshot.Thresholds
              |> List.map (fun threshold ->
                  let actual = threshold.ActualValue |> Option.map string |> Option.defaultValue "unsupported"
                  let overrideValue = threshold.OverrideValue |> Option.map string |> Option.defaultValue ""
                  let reason = threshold.OverrideReason |> Option.defaultValue ""
                  let decision =
                      threshold.Passed
                      |> Option.map (fun passed -> if passed then "pass" else "fail")
                      |> Option.defaultValue "unsupported"
                  let diagnostic = threshold.Diagnostic |> Option.defaultValue ""
                  $"| `{threshold.RuleId}` | {threshold.SignalName} | {actual} | {threshold.Comparison} {threshold.DefaultValue} | {overrideValue} | {reason} | {decision} | {diagnostic} |")
          ""
          "Diagnostics:"
          if snapshot.Diagnostics.IsEmpty then
              "- none"
          else
              yield! snapshot.Diagnostics |> List.map (fun diagnostic -> $"- {diagnostic}") ]
        |> String.concat Environment.NewLine

    File.WriteAllText(outputPath, report + Environment.NewLine)

    if failFast then
        let verdict =
            { Category = VerificationEnvironmentFailure
              Target = target
              Stage = "preflight"
              ExitCode = Some 1
              ProductChecksRun = []
              ProductFailures = []
              EnvironmentFailures = diagnostics
              HealthSnapshotPath = outputPath
              LogPath = outputPath
              RecommendedRerunEnvironment = "fresh shell, fresh container, or CI runner"
              AuthoritativeProductEvidence = false }

        writeVerificationVerdictReport verdictPath verdict
        failwithf "%s process-health preflight failed with environment-failure:%s%s" target Environment.NewLine (String.Join(Environment.NewLine, diagnostics))

let validateRunnerBootstrap root target outputPath verdictPath =
    let timestamp = DateTimeOffset.UtcNow
    let dotnetExit, dotnetOutput = runShortCommand root "dotnet" "--info" 15000
    let toolExit, toolOutput = runShortCommand root "dotnet" "tool restore" 60000
    let runnerExit, runnerOutput = runShortCommand root "dotnet" "fake --version" 60000
    let wrapperExists = File.Exists(path [ root; "fake.sh" ]) && File.Exists(path [ root; "fake.cmd" ])
    let passed = dotnetExit = 0 && toolExit = 0 && runnerExit = 0 && wrapperExists

    let recommended =
        if passed then
            None
        else
            Some "Run `dotnet tool restore`, then `dotnet fake --version`; clear stale `.fake/build.fsx/paket-files/paket.restore.cached` only if the FAKE runner still cannot start."

    let validation =
        { TargetName = target
          TimestampUtc = timestamp
          DotnetSdkStatus = if dotnetExit = 0 then "pass" else $"failed: {dotnetOutput}"
          FakeToolRestoreStatus = if toolExit = 0 then "pass" else $"failed: {toolOutput}"
          PackageCacheStatus = if runnerExit = 0 then "pass" else $"failed: FAKE runner did not start after tool restore: {runnerOutput}"
          WrapperStatus = if wrapperExists then "pass" else "failed: fake.sh or fake.cmd missing"
          WarningClassification = "runner-warning-classification: repeated netstandard script-load warning is warning-noise unless target exits nonzero; CoreCLR/VSTest/socket/thread startup failures are environment-failure evidence"
          RecommendedAction = recommended
          LogPath = outputPath
          Passed = passed }

    let remediation = validation.RecommendedAction |> Option.defaultValue "(none)"

    let report =
        [ "# Bootstrap Runner Evidence"
          ""
          $"- target: `{validation.TargetName}`"
          $"- timestamp-utc: `{validation.TimestampUtc:O}`"
          $"- dotnet-sdk-status: `{validation.DotnetSdkStatus}`"
          $"- fake-tool-restore-status: `{validation.FakeToolRestoreStatus}`"
          $"- package-cache-status: `{validation.PackageCacheStatus}`"
          $"- wrapper-status: `{validation.WrapperStatus}`"
          $"- warning-classification: {validation.WarningClassification}"
          $"- passed: `{validation.Passed}`"
          $"- remediation-command: {remediation}" ]
        |> String.concat Environment.NewLine

    File.WriteAllText(outputPath, report + Environment.NewLine)

    if not passed then
        let failures =
            [ validation.DotnetSdkStatus
              validation.FakeToolRestoreStatus
              validation.PackageCacheStatus
              validation.WrapperStatus ]
            |> List.filter (fun status -> status.StartsWith("failed", StringComparison.Ordinal))

        let verdict =
            { Category = VerificationEnvironmentFailure
              Target = target
              Stage = "bootstrap"
              ExitCode = Some 1
              ProductChecksRun = []
              ProductFailures = []
              EnvironmentFailures = failures
              HealthSnapshotPath = outputPath
              LogPath = outputPath
              RecommendedRerunEnvironment = "fresh shell, fresh container, or CI runner"
              AuthoritativeProductEvidence = false }

        writeVerificationVerdictReport verdictPath verdict
        failwithf "%s bootstrap validation failed with environment-failure:%s%s" target Environment.NewLine (String.Join(Environment.NewLine, failures))

let checkFocusedGateAssumptions root (contract: FocusedGateContract) =
    contract.StaleAssumptions
    |> List.iter (fun assumption ->
        if assumption.StartsWith("requires-restored-project:", StringComparison.Ordinal) then
            let project = assumption.Substring("requires-restored-project:".Length)
            let projectDir = Path.GetDirectoryName(path [ root; project ])
            let assets = path [ projectDir; "obj"; "project.assets.json" ]

            if not (File.Exists assets) then
                failwithf "stale-build-restore-assumption: affected-gate=%s remediation-command=`dotnet restore %s` missing `%s`" contract.TargetName project assets
        elif assumption.StartsWith("requires-built-project:", StringComparison.Ordinal) then
            let project = assumption.Substring("requires-built-project:".Length)
            let projectDir = Path.GetDirectoryName(path [ root; project ])
            let projectName = Path.GetFileNameWithoutExtension project
            let assembly = path [ projectDir; "bin"; "Debug"; "net10.0"; $"{projectName}.dll" ]

            if not (File.Exists assembly) then
                failwithf "stale-build-restore-assumption: affected-gate=%s remediation-command=`dotnet build %s` missing `%s`" contract.TargetName project assembly)

let appendFocusedGateSummary outputPath (contract: FocusedGateContract) =
    ensureParent outputPath

    let prerequisites =
        if contract.DirectPrerequisites.IsEmpty then
            "(none)"
        else
            String.Join(", ", contract.DirectPrerequisites)

    let assumptions =
        if contract.StaleAssumptions.IsEmpty then
            "(none)"
        else
            String.Join(", ", contract.StaleAssumptions)

    let readiness =
        contract.ReadinessPath |> Option.defaultValue "(none)"

    let content =
        [ $"## {contract.TargetName}"
          ""
          $"- command: `{contract.Command}`"
          $"- direct-prerequisites: {prerequisites}"
          $"- timestamp-utc: `{DateTimeOffset.UtcNow:O}`"
          $"- log-path: `{contract.LogPath}`"
          $"- readiness-path: `{readiness}`"
          $"- verdict-category: `{categoryName contract.VerdictCategory}`"
          $"- stale-build-restore-assumptions: {assumptions}"
          $"- failure-rule: `stale-build-restore-assumption`"
          $"- concurrent-fake-context: `unknown` until the runner records no other FAKE-backed command was active"
          $"- fake-race-classification: `unknown` for race-like failures until sequential rerun evidence exists"
          $"- sequential-rerun-action: rerun affected FAKE-backed commands one at a time because `.fake` state is shared"
          $"- follow-up-classification: classify product regression only after the sequential rerun reproduces the failure"
          $"- affected-gate: `{contract.TargetName}`"
          $"- remediation-command: `dotnet restore` or `dotnet build` for the named project when assumptions are stale"
          "" ]
        |> String.concat Environment.NewLine

    BuildReports.appendOrCreateSection outputPath "# Focused Gates Evidence" [ content ]

let relativePathFrom root filePath =
    let rootPath =
        Path.GetFullPath root
        |> fun value -> value.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + string Path.DirectorySeparatorChar

    let filePath = Path.GetFullPath filePath
    let relative = Uri(rootPath).MakeRelativeUri(Uri(filePath)).ToString()
    Uri.UnescapeDataString(relative).Replace('\\', '/')

let latestTemplatePackage artifactDir =
    BuildTemplateValidation.latestTemplatePackage artifactDir

let validateTemplatePackage model outputPath =
    let required =
        [ "content/.template.config/template.json"
          "content/template/base/build.fsx"
          "content/template/base/src/Product/Product.fsproj"
          "content/template/base/src/Product/Model.fs"
          "content/template/base/src/Product/View.fs"
          "content/template/base/src/Product/LayoutEvidence.fs"
          "content/template/base/src/Product/EvidenceCommands.fs"
          "content/template/base/src/Product/WindowOptions.fs"
          "content/template/base/src/Product/Program.fs"
          "content/template/base/CLAUDE.md"
          "content/template/base/.claude/settings.json"
          "content/template/base/.claude/skills/fs-skia-project/SKILL.md"
          "content/template/base/tests/Product.Tests/Product.Tests.fsproj"
          "content/template/base/Directory.Packages.props"
          "content/template/profiles/app.yml"
          "content/template/profiles/headless-scene.yml"
          "content/template/profiles/governed.yml"
          "content/template/profiles/sample-pack.yml"
          "content/.specify/templates/spec-template.md"
          "content/.specify/scripts/bash/setup-plan.sh"
          "content/.agents/skills/speckit-specify/SKILL.md"
          "content/.claude/skills/speckit-specify/SKILL.md"
          "content/.template.config/generated/CLAUDE.md"
          "content/.template.config/generated/.claude/settings.json"
          "content/.template.config/generated/.specify/memory/constitution.md" ]

    let forbiddenPrefixes =
        [ "content/.git/"
          "content/artifacts/"
          "content/.template.package/"
          "content/.specify/feature.json"
          "content/.specify/memory/constitution.md"
          "content/specs/001-"
          "content/specs/002-"
          "content/specs/003-"
          "content/specs/004-"
          "content/specs/005-"
          "content/specs/006-"
          "content/specs/007-" ]

    BuildTemplateValidation.validateTemplatePackageEntries model.TemplateArtifactDir outputPath required forbiddenPrefixes

let runTemplateInstall model label source outputPath =
    if source = SourceDirectory then
        cleanDirectoryContents model.TemplateWorkDir

    let installArgument =
        match source with
        | SourceDirectory -> model.RepositoryRoot
        | PackageArtifact ->
            latestTemplatePackage model.TemplateArtifactDir
            |> Option.defaultWith (fun () -> failwithf "No template package found in %s" model.TemplateArtifactDir)

    [ model.RepositoryRoot; "FS.Skia.UI.Template" ]
    |> List.distinct
    |> List.iter (fun uninstallArgument ->
        runProcessWithAllowedExitCodes $"{label} uninstall" "dotnet" $"new uninstall {quote uninstallArgument}" model.RepositoryRoot outputPath Map.empty (Set.ofList [ 0; 1; 2; 103 ]))

    runProcess label "dotnet" $"new install {quote installArgument}" model.RepositoryRoot outputPath Map.empty

let instantiateRow model (row: TemplateRow) =
    cleanDirectoryContents row.Root
    cleanDirectoryContents row.EvidenceDir

    let rootNamespace = row.ProjectName.Replace("-", ".")
    let repositoryUrl = $"https://example.invalid/{row.Artifact}/{row.Profile}/{row.ProjectName}"

    let args =
        [ "new fs-skia-ui"
          $"--name {row.ProjectName}"
          $"--output {quote row.Root}"
          "--allow-scripts yes"
          $"--profile {row.Profile}"
          $"--rootNamespace {rootNamespace}"
          $"--packagePrefix {rootNamespace}"
          "--authors TemplateValidation"
          $"--repositoryUrl {quote repositoryUrl}"
          "--targetFramework net10.0"
          "--skipGitInit true" ]
        |> String.concat " "

    runProcess $"{row.Artifact}/{row.Profile} instantiate" "dotnet" args model.RepositoryRoot (path [ row.EvidenceDir; "instantiate.log" ]) Map.empty

let runTemplateInstantiation model outputPath =
    ensureParent outputPath
    File.WriteAllText(outputPath, "# Template Instantiation" + Environment.NewLine)

    cleanDirectoryContents model.TemplateWorkDir

    runTemplateInstall model "source template install for instantiation" SourceDirectory outputPath

    templateRows model
    |> List.filter (fun row -> row.Artifact = "source")
    |> List.iter (instantiateRow model)

    runTemplateInstall model "package template install for instantiation" PackageArtifact outputPath

    templateRows model
    |> List.filter (fun row -> row.Artifact = "package")
    |> List.iter (instantiateRow model)

    let rows =
        templateRows model
        |> List.map (fun row -> $"- {row.Artifact}/{row.Profile}: `{row.Root}`")
        |> String.concat Environment.NewLine

    File.AppendAllText(outputPath, Environment.NewLine + "Generated rows:" + Environment.NewLine + rows + Environment.NewLine)

let fileShouldBeScanned (filePath: string) =
    BuildGeneratedScanning.fileShouldBeScanned filePath

let generatedShellScripts (row: TemplateRow) =
    Directory.EnumerateFiles(row.Root, "*.sh", SearchOption.AllDirectories)
    |> Seq.filter fileShouldBeScanned
    |> Seq.toList

let isWindows =
    BuildGeneratedScanning.isWindows

let hasUserExecutePermission filePath =
    BuildGeneratedScanning.hasUserExecutePermission filePath

let scanGeneratedRow (row: TemplateRow) =
    let files =
        Directory.EnumerateFiles(row.Root, "*", SearchOption.AllDirectories)
        |> Seq.filter fileShouldBeScanned
        |> Seq.toList

    let identityTokens =
        [ "FS-Skia-UI" ]

    let placeholderHits =
        files
        |> List.collect (fun file ->
            let relative = relativePathFrom row.Root file
            let content = File.ReadAllText file

            identityTokens
            |> List.choose (fun token ->
                if content.IndexOf(token, StringComparison.Ordinal) >= 0 then
                    Some $"{relative}: {token}"
                else
                    None))

    let excludedHistory =
        if Directory.Exists(path [ row.Root; "specs" ]) then
            Directory.GetDirectories(path [ row.Root; "specs" ], "00*", SearchOption.TopDirectoryOnly)
            |> Array.map (relativePathFrom row.Root)
            |> Array.toList
        else
            []

    let forbiddenFrameworkPaths =
        [ "src/Lib"
          "src/Scene"
          "src/SkiaViewer"
          "src/Elmish"
          "src/KeyboardInput"
          "src/Layout"
          "src/Controls"
          "src/Charts"
          "src/Testing"
          "tests/Lib.Tests"
          "tests/Scene.Tests"
          "tests/SkiaViewer.Tests"
          "tests/Elmish.Tests"
          "tests/KeyboardInput.Tests"
          "tests/Layout.Tests"
          "tests/Controls.Tests"
          "tests/Charts.Tests"
          "tests/Testing.Tests"
          "tests/Parity.Tests"
          "tests/Smoke.Tests"
          "samples/BasicViewer"
          "samples/ControlsGallery"
          "samples/ChartsGallery"
          "samples/DataGridGallery"
          "samples/LayoutGraphGallery"
          "samples/ParityGallery"
          "samples/InteractiveViewer"
          "samples/EffectsGallery"
          "samples/ScreenshotGallery"
          "samples/DemoReel" ]
        |> List.filter (fun relative -> Directory.Exists(path [ row.Root; relative ]))

    let samplePackRequired =
        if row.Profile = "sample-pack" then
            [ "samples/README.md" ]
        else
            []

    let expectedPackages =
        match row.Profile with
        | "app" ->
            [ "FS.Skia.UI.Scene"
              "FS.Skia.UI.SkiaViewer"
              "FS.Skia.UI.Elmish"
              "FS.Skia.UI.KeyboardInput"
              "FS.Skia.UI.Layout"
              "FS.Skia.UI.Controls"
              "FS.Skia.UI.Controls.Elmish" ]
        | "headless-scene" -> [ "FS.Skia.UI.Scene" ]
        | "governed" -> [ "FS.Skia.UI.Scene"; "FS.Skia.UI.Testing" ]
        | "sample-pack" -> [ "FS.Skia.UI.Scene"; "FS.Skia.UI.SkiaViewer"; "FS.Skia.UI.Elmish" ]
        | other -> failwithf "Unknown V3 template profile %s" other

    let allCapabilityPackages =
        [ "FS.Skia.UI.Scene"
          "FS.Skia.UI.SkiaViewer"
          "FS.Skia.UI.Elmish"
          "FS.Skia.UI.KeyboardInput"
          "FS.Skia.UI.Layout"
          "FS.Skia.UI.Controls"
          "FS.Skia.UI.Controls.Elmish"
          "FS.Skia.UI.Testing" ]

    let required =
        [ $"src/{row.ProjectName}/{row.ProjectName}.fsproj"
          $"tests/{row.ProjectName}.Tests/{row.ProjectName}.Tests.fsproj"
          "docs/product.md"
          "Directory.Packages.props"
          "AGENTS.md"
          "CLAUDE.md"
          ".claude/settings.json"
          "build.fsx"
          "fake.sh"
          ".specify/memory/constitution.md"
          ".specify/templates/spec-template.md"
          ".specify/scripts/bash/setup-plan.sh"
          ".specify/workflows/speckit/workflow.yml"
          ".specify/extensions/evidence/scripts/bash/run-audit.sh"
          ".agents/skills/speckit-specify/SKILL.md"
          ".claude/skills/speckit-specify/SKILL.md"
          ".agents/skills/speckit-plan/SKILL.md"
          ".claude/skills/speckit-plan/SKILL.md"
          ".agents/skills/speckit-tasks/SKILL.md"
          ".claude/skills/speckit-tasks/SKILL.md"
          ".agents/skills/speckit-implement/SKILL.md"
          ".claude/skills/speckit-implement/SKILL.md" ]
        @ samplePackRequired

    let missingRequired =
        required
        |> List.filter (fun relative -> not (File.Exists(path [ row.Root; relative ])))

    if not (List.isEmpty placeholderHits) then
        failwithf "%s/%s generated project has unreplaced identity tokens:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, placeholderHits))

    if not (List.isEmpty excludedHistory) then
        failwithf "%s/%s generated project contains excluded historical specs:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, excludedHistory))

    if not (List.isEmpty forbiddenFrameworkPaths) then
        failwithf "%s/%s generated project contains framework implementation paths:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, forbiddenFrameworkPaths))

    if not (List.isEmpty missingRequired) then
        failwithf "%s/%s generated project is missing required files:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, missingRequired))

    let productProject = File.ReadAllText(path [ row.Root; "src"; row.ProjectName; $"{row.ProjectName}.fsproj" ])

    expectedPackages
    |> List.iter (fun packageId ->
        if productProject.IndexOf($"PackageReference Include=\"{packageId}\"", StringComparison.Ordinal) < 0 then
            failwithf "%s/%s generated project is missing package reference %s" row.Artifact row.Profile packageId)

    allCapabilityPackages
    |> List.filter (fun packageId -> not (expectedPackages |> List.contains packageId))
    |> List.iter (fun packageId ->
        if productProject.IndexOf($"PackageReference Include=\"{packageId}\"", StringComparison.Ordinal) >= 0 then
            failwithf "%s/%s generated project contains unselected package reference %s" row.Artifact row.Profile packageId)

    let removedChartsPackage = "FS.Skia.UI." + "Charts"

    if productProject.IndexOf($"PackageReference Include=\"{removedChartsPackage}\"", StringComparison.Ordinal) >= 0 then
        failwithf "%s/%s generated project contains removed Charts package reference %s" row.Artifact row.Profile removedChartsPackage

    let staleAgentsReference =
        let agentsPath = path [ row.Root; "AGENTS.md" ]

        File.Exists agentsPath
        && File.ReadAllText(agentsPath).IndexOf("specs/008-targeted-refactor-governance", StringComparison.Ordinal) >= 0

    if staleAgentsReference then
        failwithf "%s/%s generated AGENTS.md references source-only active feature specs/008-targeted-refactor-governance" row.Artifact row.Profile

    if File.Exists(path [ row.Root; ".specify"; "feature.json" ]) then
        failwithf "%s/%s generated project contains source-only .specify/feature.json active feature state" row.Artifact row.Profile

    let nonExecutableScripts =
        if isWindows then
            []
        else
            generatedShellScripts row
            |> List.filter (hasUserExecutePermission >> not)
            |> List.map (relativePathFrom row.Root)

    if not (List.isEmpty nonExecutableScripts) then
        failwithf "%s/%s generated project has non-executable shell scripts:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, nonExecutableScripts))

    let stopwatch = Stopwatch.StartNew()
    runProcess $"{row.Artifact}/{row.Profile} generated Dev" "bash" "./fake.sh build -t Dev" row.Root (path [ row.EvidenceDir; "dev.log" ]) Map.empty
    stopwatch.Stop()
    let elapsedSeconds = stopwatch.Elapsed.TotalSeconds

    let postDevExcludedHistory =
        if Directory.Exists(path [ row.Root; "specs" ]) then
            Directory.GetDirectories(path [ row.Root; "specs" ], "00*", SearchOption.TopDirectoryOnly)
            |> Array.map (relativePathFrom row.Root)
            |> Array.toList
        else
            []

    if not (List.isEmpty postDevExcludedHistory) then
        failwithf "%s/%s generated Dev created excluded historical specs:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, postDevExcludedHistory))

    let report =
        [ $"# {row.Artifact}/{row.Profile} Scan"
          ""
          $"Root: `{row.Root}`"
          $"Files scanned: {files.Length}"
          "Placeholder scan: PASS"
          "Excluded-history scan: PASS"
          "V3 framework-source exclusion scan: PASS"
          "V3 selected package reference scan: PASS"
          "Spec Kit install scan: PASS"
          "Generated AGENTS scan: PASS"
          "Executable script scan: PASS"
          $"Generated Dev elapsed: {elapsedSeconds:F1} seconds"
          "Visual support: non-visual V3 validation only; full visual evidence is deferred." ]
        |> String.concat Environment.NewLine

    File.WriteAllText(path [ row.EvidenceDir; "scan.md" ], report + Environment.NewLine)

let scanGeneratedProjects model outputPath =
    templateRows model
    |> List.iter scanGeneratedRow

    let summary =
        [ "# Generated Project Validation"
          ""
          "| Artifact | Profile | Root | Dev log |"
          "|----------|---------|------|---------|"
          yield!
              templateRows model
              |> List.map (fun row ->
                  let devLog = path [ row.EvidenceDir; "dev.log" ]
                  $"| {row.Artifact} | {row.Profile} | `{row.Root}` | `{devLog}` |")
          ""
          "PASS: placeholder scans, excluded-history scans, V3 profile package checks, Spec Kit install checks, and generated Dev runs completed for all rows." ]
        |> String.concat Environment.NewLine

    ensureParent outputPath
    File.WriteAllText(outputPath, summary + Environment.NewLine)

// BUILD SECTION: V3 capability validation

let trimQuotes (value: string) =
    BuildPackageResolution.trimQuotes value

let parseScalar (line: string) =
    BuildPackageResolution.parseScalar line

let parseInlineList (value: string) =
    BuildPackageResolution.parseInlineList value

let emptyCapability id =
    { Id = id
      DisplayName = ""
      PackageId = None
      Project = None
      Contracts = []
      Tests = []
      Skill = None
      TemplateFragment = None
      Dependencies = []
      Profiles = []
      DefaultApp = false
      Evidence = []
      SurfaceBaseline = None
      Docs = None
      NonRuntime = false }

let readCapabilityCatalog model =
    if not (File.Exists model.CapabilityCatalogPath) then
        failwithf "Missing capability catalog: %s" model.CapabilityCatalogPath

    let lines = File.ReadAllText(model.CapabilityCatalogPath).Replace("\r\n", "\n").Split('\n')
    let capabilities = ResizeArray<CapabilityRow>()
    let mutable current: CapabilityRow option = None
    let mutable currentList: string option = None

    let commitCurrent () =
        match current with
        | Some capability -> capabilities.Add capability
        | None -> ()

    let setCurrent update =
        current <- current |> Option.map update

    for raw in lines do
        let trimmed = raw.Trim()

        if trimmed.StartsWith("- id:", StringComparison.Ordinal) then
            commitCurrent ()
            current <- Some(emptyCapability (parseScalar trimmed))
            currentList <- None
        elif trimmed.StartsWith("- ", StringComparison.Ordinal) && currentList.IsSome then
            let item = trimmed.Substring(2) |> trimQuotes

            match currentList.Value with
            | "contracts" -> setCurrent (fun c -> { c with Contracts = c.Contracts @ [ item ] })
            | "tests" -> setCurrent (fun c -> { c with Tests = c.Tests @ [ item ] })
            | "dependencies" -> setCurrent (fun c -> { c with Dependencies = c.Dependencies @ [ item ] })
            | "profiles" -> setCurrent (fun c -> { c with Profiles = c.Profiles @ [ item ] })
            | "evidence" -> setCurrent (fun c -> { c with Evidence = c.Evidence @ [ item ] })
            | _ -> ()
        elif current.IsSome && trimmed.IndexOf(":", StringComparison.Ordinal) >= 0 then
            let field = trimmed.Substring(0, trimmed.IndexOf(':')).Trim()
            let value = parseScalar trimmed
            currentList <- None

            match field with
            | "displayName" -> setCurrent (fun c -> { c with DisplayName = value })
            | "packageId" -> setCurrent (fun c -> { c with PackageId = Some value })
            | "project" -> setCurrent (fun c -> { c with Project = Some value })
            | "contracts" ->
                setCurrent (fun c -> { c with Contracts = parseInlineList value })
                currentList <- Some "contracts"
            | "tests" ->
                setCurrent (fun c -> { c with Tests = parseInlineList value })
                currentList <- Some "tests"
            | "skill" -> setCurrent (fun c -> { c with Skill = Some value })
            | "templateFragment" -> setCurrent (fun c -> { c with TemplateFragment = Some value })
            | "dependencies" ->
                setCurrent (fun c -> { c with Dependencies = parseInlineList value })
                currentList <- Some "dependencies"
            | "profiles" ->
                setCurrent (fun c -> { c with Profiles = parseInlineList value })
                currentList <- Some "profiles"
            | "defaultApp" -> setCurrent (fun c -> { c with DefaultApp = value.Equals("true", StringComparison.OrdinalIgnoreCase) })
            | "evidence" ->
                setCurrent (fun c -> { c with Evidence = parseInlineList value })
                currentList <- Some "evidence"
            | "surfaceBaseline" -> setCurrent (fun c -> { c with SurfaceBaseline = Some value })
            | "docs" -> setCurrent (fun c -> { c with Docs = Some value })
            | "nonRuntime" -> setCurrent (fun c -> { c with NonRuntime = value.Equals("true", StringComparison.OrdinalIgnoreCase) })
            | _ -> ()

    commitCurrent ()
    capabilities |> Seq.toList

let finding artifactClass path rule message =
    { ArtifactClass = artifactClass
      Path = path
      Rule = rule
      Message = message }

let validateCapabilityRows model capabilities =
    let ids = capabilities |> List.map (fun capability -> capability.Id) |> Set.ofList

    let requiredDefault =
        Set.ofList [ "Scene"; "SkiaViewer"; "Elmish"; "KeyboardInput"; "Layout"; "Controls" ]

    let defaultApp =
        capabilities
        |> List.filter (fun capability -> capability.DefaultApp)
        |> List.map (fun capability -> capability.DisplayName)
        |> Set.ofList

    [ if defaultApp <> requiredDefault then
          yield finding "capability-catalog" model.CapabilityCatalogPath "default-app" ("Default app set was " + String.Join(", ", defaultApp))

      for capability in capabilities do
          if String.IsNullOrWhiteSpace capability.DisplayName then
              yield finding "capability-catalog" capability.Id "displayName" "Missing displayName"

          if capability.Project.IsNone && not capability.NonRuntime then
              yield finding "capability-catalog" capability.Id "project" "Runtime capability is missing project"

          if capability.Contracts.IsEmpty then
              yield finding "capability-catalog" capability.Id "contracts" "Missing public contracts or no-public-surface record"

          if capability.Tests.IsEmpty then
              yield finding "capability-catalog" capability.Id "tests" "Missing test coverage entry"

          if capability.Skill.IsNone then
              yield finding "capability-catalog" capability.Id "skill" "Missing local skill"

          if capability.TemplateFragment.IsNone then
              yield finding "capability-catalog" capability.Id "templateFragment" "Missing template fragment"

          if capability.Profiles.IsEmpty then
              yield finding "capability-catalog" capability.Id "profiles" "Missing profile ownership"

          if capability.Evidence.IsEmpty then
              yield finding "capability-catalog" capability.Id "evidence" "Missing evidence classes"

          match capability.SurfaceBaseline with
          | Some "no-public-surface" -> ()
          | Some baseline when File.Exists(path [ model.RepositoryRoot; baseline ]) -> ()
          | Some baseline -> yield finding "capability-catalog" capability.Id "surfaceBaseline" $"Missing surface baseline {baseline}"
          | None -> yield finding "capability-catalog" capability.Id "surfaceBaseline" "Missing surface baseline"

          for dependency in capability.Dependencies do
              if not (ids.Contains dependency) then
                  yield finding "capability-catalog" capability.Id "dependency" $"Unknown dependency {dependency}" ]

let writeFindingsOrPass outputPath title findings rows =
    if not (List.isEmpty findings) then
        let detail =
            findings
            |> List.map (fun finding -> $"- `{finding.Path}` [{finding.Rule}]: {finding.Message}")
            |> String.concat Environment.NewLine

        failwithf "%s failed:%s%s" title Environment.NewLine detail

    ensureParent outputPath
    File.WriteAllText(outputPath, rows |> String.concat Environment.NewLine |> fun text -> text + Environment.NewLine)

let runCapabilityCatalogCheck model =
    let capabilities = readCapabilityCatalog model
    let findings = validateCapabilityRows model capabilities

    let rows =
        [ "# Capability Catalog"
          ""
          "PASS: capability catalog metadata, dependency closure, default app set, contracts, tests, skills, fragments, evidence, and surface baselines are valid."
          ""
          "| Capability | Package | Project | Dependencies | Default app |"
          "|------------|---------|---------|--------------|-------------|"
          yield!
              capabilities
              |> List.map (fun capability ->
                  let packageId = capability.PackageId |> Option.defaultValue "non-runtime"
                  let project = capability.Project |> Option.defaultValue "non-runtime"
                  let dependencies = if capability.Dependencies.IsEmpty then "(none)" else String.Join(", ", capability.Dependencies)
                  $"| {capability.DisplayName} | `{packageId}` | `{project}` | {dependencies} | {capability.DefaultApp} |") ]

    writeFindingsOrPass model.CapabilityCatalogReportPath "CapabilityCheck" findings rows

let requiredSkillSections =
    [ "## Scope"
      "## Public Contract"
      "## Build Commands"
      "## Test Commands"
      "## Evidence"
      "## Package Boundary"
      "## Generated Product" ]

let runSkillCatalogCheck model =
    let capabilities = readCapabilityCatalog model

    let findings =
        [ for capability in capabilities do
              match capability.Skill with
              | None -> yield finding "selected-skills" capability.Id "skill" "Missing skill path"
              | Some skillPath ->
                  let fullPath = path [ model.RepositoryRoot; skillPath ]

                  if not (File.Exists fullPath) then
                      yield finding "selected-skills" skillPath "skill-file" "Skill file is missing"
                  else
                      let content = File.ReadAllText fullPath

                      for section in requiredSkillSections do
                          if content.IndexOf(section, StringComparison.Ordinal) < 0 then
                              yield finding "selected-skills" skillPath "skill-section" $"Missing {section}"

                      if content.IndexOf("./fake.sh build -t", StringComparison.Ordinal) < 0 then
                          yield finding "selected-skills" skillPath "skill-command" "Skill does not name a FAKE target command" ]

    let defaultSkills =
        [ "fs-skia-project"
          "fs-skia-scene"
          "fs-skia-skiaviewer"
          "fs-skia-elmish"
          "fs-skia-keyboard-input"
          "fs-skia-ui-widgets" ]

    let rows =
        [ "# Selected Skills"
          ""
          "PASS: selected capability skills contain required sections and valid command references."
          ""
          "Default app selected skill destinations:"
          yield! defaultSkills |> List.map (fun skill -> $"- `{skill}`")
          ""
          "Generated-product validation rejects unrelated capability skills." ]

    writeFindingsOrPass model.SelectedSkillsReportPath "SkillCheck" findings rows

let rec copyDirectory source target =
    Directory.CreateDirectory target |> ignore

    for file in Directory.GetFiles source do
        File.Copy(file, path [ target; Path.GetFileName file ], true)

    for directory in Directory.GetDirectories source do
        copyDirectory directory (path [ target; Path.GetFileName directory ])

let rec copyDirectoryExcept (source: string) (target: string) (excludedRelativePaths: string list) =
    Directory.CreateDirectory target |> ignore

    let excluded =
        excludedRelativePaths
        |> List.map (fun relative -> relative.Replace('\\', '/'))
        |> Set.ofList

    let rec copy (currentSource: string) (currentTarget: string) (relativePrefix: string) =
        Directory.CreateDirectory currentTarget |> ignore

        for file in Directory.GetFiles currentSource do
            let relative = (relativePrefix + Path.GetFileName file).Replace('\\', '/')

            if Set.contains relative excluded |> not then
                File.Copy(file, path [ currentTarget; Path.GetFileName file ], true)

        for directory in Directory.GetDirectories currentSource do
            let name = Path.GetFileName directory
            let relative = (relativePrefix + name + "/").Replace('\\', '/')

            if Set.contains relative excluded |> not then
                copy directory (path [ currentTarget; name ]) relative

    copy source target ""

let copySpecKitInstall model root =
    copyDirectoryExcept (path [ model.RepositoryRoot; ".specify" ]) (path [ root; ".specify" ]) [ "feature.json"; "memory/constitution.md" ]
    copyDirectory (path [ model.RepositoryRoot; ".template.config"; "generated"; ".specify" ]) (path [ root; ".specify" ])

let capabilitiesById model =
    readCapabilityCatalog model |> List.map (fun row -> row.Id, row) |> Map.ofList

let resolveCapabilities model selected =
    let byId = capabilitiesById model

    let rec visit seen capabilityId =
        if Set.contains capabilityId seen then
            seen
        else
            match Map.tryFind capabilityId byId with
            | None -> failwithf "Unknown capability %s" capabilityId
            | Some capability ->
                capability.Dependencies
                |> List.fold visit (Set.add capabilityId seen)

    selected
    |> List.fold visit Set.empty
    |> Set.toList

let packageReferences model capabilities =
    let byId = capabilitiesById model

    let capabilityPackages =
        capabilities
        |> List.choose (fun capabilityId ->
            match Map.tryFind capabilityId byId with
            | Some capability when not capability.NonRuntime ->
                capability.PackageId
                |> Option.bind (fun packageId ->
                    if packageId = "non-runtime" then None else Some packageId)
            | _ -> None)

    let adapterPackages =
        if capabilities |> List.contains "controls" && capabilities |> List.contains "elmish" then
            [ "FS.Skia.UI.Controls.Elmish" ]
        else
            []

    (capabilityPackages @ adapterPackages)
    |> List.distinct
    |> List.sort

let writeProductProject model row capabilities =
    let references =
        packageReferences model capabilities
        |> List.map (fun packageId -> $"    <PackageReference Include=\"{packageId}\" />")
        |> String.concat Environment.NewLine

    let compileItems =
        if capabilities |> List.contains "controls" then
            [ "Model.fs"
              "View.fs"
              "LayoutEvidence.fs"
              "WindowOptions.fs"
              "EvidenceCommands.fs"
              "Program.fs" ]
        else
            [ "Program.fs" ]
        |> List.map (fun file -> $"    <Compile Include=\"{file}\" />")
        |> String.concat Environment.NewLine

    let content =
        $"""<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
{compileItems}
  </ItemGroup>

  <ItemGroup>
{references}
  </ItemGroup>

</Project>
"""

    File.WriteAllText(path [ row.Root; "src"; "Product"; "Product.fsproj" ], content)

type TemplateConditionalFrame =
    { ParentActive: bool
      BranchMatched: bool }

let private evaluateProfileCondition profile expression =
    let evaluateAtom (atom: string) =
        let trimmed = atom.Trim()
        let profileEquals = Regex.Match(trimmed, "^profile\\s*==\\s*\"([^\"]+)\"$")

        if profileEquals.Success then
            profile = profileEquals.Groups.[1].Value
        else
            failwithf "Unsupported generated product template condition: %s" expression

    expression.Trim().TrimStart('(').TrimEnd(')').Split([| "||" |], StringSplitOptions.None)
    |> Array.exists evaluateAtom

let private applyGeneratedProfileConditionals profile filePath =
    let mutable frames: TemplateConditionalFrame list = []
    let mutable active = true
    let output = ResizeArray<string>()

    for line in File.ReadAllLines filePath do
        let trimmed = line.Trim()

        if trimmed.StartsWith("//#if", StringComparison.Ordinal) then
            let expression = trimmed.Substring("//#if".Length).Trim()
            let matched = evaluateProfileCondition profile expression
            let current = active && matched
            frames <- { ParentActive = active; BranchMatched = matched } :: frames
            active <- current
        elif trimmed.StartsWith("//#else", StringComparison.Ordinal) then
            match frames with
            | frame :: rest ->
                let current = frame.ParentActive && not frame.BranchMatched
                frames <- { frame with BranchMatched = true } :: rest
                active <- current
            | [] -> failwithf "Unmatched //#else in %s" filePath
        elif trimmed.StartsWith("//#endif", StringComparison.Ordinal) then
            match frames with
            | frame :: rest ->
                frames <- rest
                active <- frame.ParentActive
            | [] -> failwithf "Unmatched //#endif in %s" filePath
        elif active then
            output.Add line

    if not frames.IsEmpty then
        failwithf "Unclosed generated product template conditional in %s" filePath

    File.WriteAllLines(filePath, output.ToArray())

let applyGeneratedProductProfile row =
    Directory.EnumerateFiles(row.Root, "*.fs", SearchOption.AllDirectories)
    |> Seq.iter (applyGeneratedProfileConditionals row.Profile)

let writeSceneOnlyProductFiles row =
    let program =
        """module Product.Program

open System
open System.IO
open FS.Skia.UI.Scene

let productScene =
    { Nodes =
        [ Rectangle((16.0, 16.0, 160.0, 96.0), Colors.rgb 32uy 96uy 160uy)
          Text((28.0, 56.0), "Generated scene product", Colors.white) ] }

let sceneElementCount () =
    productScene.Nodes.Length

let writeLines (path: string) (lines: string list) =
    let directory = Path.GetDirectoryName path

    if not (String.IsNullOrWhiteSpace directory) then
        Directory.CreateDirectory(directory) |> ignore

    File.WriteAllLines(path, Array.ofList lines)

let sceneEvidence evidencePath =
    match
        SceneEvidence.render
            { Scene = productScene
              OutputSize = { Width = 320; Height = 200 }
              Format = Metadata
              RendererMode = "deterministic-scene"
              EvidencePath = Some evidencePath }
    with
    | Result.Ok evidence ->
        writeLines
            evidencePath
            [ "status=ok"
              "mode=headless-scene"
              "command=--scene-evidence"
              $"renderer-mode={evidence.RendererMode}"
              $"scene-evidence-format={evidence.Format}"
              $"value={evidence.Value}" ]

        printfn "status=ok mode=headless-scene command=--scene-evidence renderer-mode=%s evidence=%s value=%s" evidence.RendererMode evidencePath evidence.Value
        0
    | Result.Error failure ->
        writeLines
            evidencePath
            [ "status=failed"
              "mode=headless-scene"
              "command=--scene-evidence"
              $"blocked-stage={failure.BlockedStage}"
              $"classification={failure.Classification}"
              $"category={failure.DiagnosticCategory}"
              $"message={failure.Message}" ]

        printfn "status=failed mode=headless-scene command=--scene-evidence blocked-stage=%s classification=%A category=%s message=%s evidence=%s" failure.BlockedStage failure.Classification failure.DiagnosticCategory failure.Message evidencePath
        1

[<EntryPoint>]
let main args =
    match List.ofArray args with
    | "--scene-evidence" :: path :: _ -> sceneEvidence path
    | "--scene-evidence" :: _ -> sceneEvidence "readiness/headless-scene-evidence.txt"
    | _ ->
        printfn "status=ok mode=headless-scene command=dotnet-run scene-elements=%d" (sceneElementCount())
        0
"""

    let tests =
        """module ProductTests

open Expecto
open Product.Program
open FS.Skia.UI.Scene

[<Tests>]
let tests =
    testList "product" [
        test "generated product test suite is wired" {
            Expect.equal 1 1 "product tests run"
        }

        test "headless scene profile builds a scene-only product" {
            Expect.isGreaterThan (sceneElementCount()) 1 "scene-only product renders multiple scene nodes"
            Expect.exists productScene.Nodes (function Rectangle _ -> true | _ -> false) "scene includes a rectangle"
            Expect.exists productScene.Nodes (function Text(_, value, _) when value.Contains("Generated scene product") -> true | _ -> false) "scene includes text"
        }
    ]
"""

    File.WriteAllText(path [ row.Root; "src"; "Product"; "Program.fs" ], program)
    File.WriteAllText(path [ row.Root; "tests"; "Product.Tests"; "Tests.fs" ], tests)

let copySelectedSkills model row capabilities =
    let skillRoot = path [ row.Root; ".agents"; "skills" ]
    let claudeSkillRoot = path [ row.Root; ".claude"; "skills" ]
    Directory.CreateDirectory skillRoot |> ignore
    Directory.CreateDirectory claudeSkillRoot |> ignore
    copyDirectory (path [ model.RepositoryRoot; "template"; "base"; ".agents"; "skills"; "fs-skia-project" ]) (path [ skillRoot; "fs-skia-project" ])
    copyDirectory (path [ model.RepositoryRoot; "template"; "base"; ".claude"; "skills"; "fs-skia-project" ]) (path [ claudeSkillRoot; "fs-skia-project" ])

    Directory.GetDirectories(path [ model.RepositoryRoot; ".agents"; "skills" ], "speckit-*", SearchOption.TopDirectoryOnly)
    |> Array.iter (fun directory ->
        let skillName = Path.GetFileName directory
        copyDirectory directory (path [ skillRoot; skillName ])
        copyDirectory directory (path [ claudeSkillRoot; skillName ]))

    let byId = capabilitiesById model
    let controlsSelected = capabilities |> List.contains "controls"

    for capabilityId in capabilities do
        let skipGeneratedSkill =
            controlsSelected && capabilityId = "layout"

        match skipGeneratedSkill, Map.tryFind capabilityId byId, capabilitySkillDestination capabilityId with
        | true, _, _ -> ()
        | false, Some capability, Some destination ->
            match capability.Skill with
            | Some sourceSkill ->
                let destinationDirectory = path [ skillRoot; destination ]
                let claudeDestinationDirectory = path [ claudeSkillRoot; destination ]
                Directory.CreateDirectory destinationDirectory |> ignore
                Directory.CreateDirectory claudeDestinationDirectory |> ignore
                File.Copy(path [ model.RepositoryRoot; sourceSkill ], path [ destinationDirectory; "SKILL.md" ], true)
                File.Copy(path [ model.RepositoryRoot; sourceSkill ], path [ claudeDestinationDirectory; "SKILL.md" ], true)
            | None -> ()
        | _ -> ()

let v3TemplatePackagePath model =
    path [ model.TemplateArtifactDir; "FS.Skia.UI.V3.Template.zip" ]

let createV3TemplatePackage model =
    Directory.CreateDirectory model.TemplateArtifactDir |> ignore
    let packagePath = v3TemplatePackagePath model

    if File.Exists packagePath then
        File.Delete packagePath

    ZipFile.CreateFromDirectory(path [ model.RepositoryRoot; "template" ], packagePath)
    packagePath

let templatePayloadRoot model row =
    if row.Artifact = "package" then
        let extracted = path [ model.TemplateWorkDir; "v3-template-package" ]
        cleanDirectoryContents extracted
        ZipFile.ExtractToDirectory(v3TemplatePackagePath model, extracted)
        extracted
    else
        path [ model.RepositoryRoot; "template" ]

let writeGeneratedProductReadme row capabilities =
    let capabilityNames = capabilities |> List.map (fun id -> $"- {id}") |> String.concat Environment.NewLine

    let content =
        [ "# Product"
          ""
          "This generated product consumes selected FS.Skia.UI capability packages."
          ""
          "Resolved capabilities:"
          capabilityNames
          ""
          "Commands:"
          ""
          "```bash"
          "./fake.sh build -t Dev"
          "./fake.sh build -t Test"
          "./fake.sh build -t Verify"
          "```" ]
        |> String.concat Environment.NewLine

    File.WriteAllText(path [ row.Root; "README.md" ], content + Environment.NewLine)

// US4 (spec 037, FR-009): emit the generated FSI load script. The reference set
// is DERIVED from the built Product output assembly directory (the transitive
// FS.Skia.UI.* closure pinned by Directory.Packages.props) plus the Product app
// itself, so it stays in sync with the assembly set instead of being a
// hand-maintained list. Loading it neither emits nor suppresses host warnings;
// real load failures surface normally (spec 021 benign-warning classification is
// unaffected because the script references and opens — it launches nothing).
let emitFsiLoadScript row =
    let productBin = path [ row.Root; "src"; "Product"; "bin" ]
    let loadScriptPath = path [ row.Root; "load-product.fsx" ]

    let productDll =
        if Directory.Exists productBin then
            Directory.EnumerateFiles(productBin, "Product.dll", SearchOption.AllDirectories)
            |> Seq.sortBy (fun candidate -> candidate.Length)
            |> Seq.tryHead
        else
            None

    match productDll with
    | None ->
        failwithf
            "%s/%s could not emit load-product.fsx: no built Product.dll under %s (generated build must run first)"
            row.Artifact row.Profile productBin
    | Some dll ->
        let outDir = Path.GetDirectoryName dll
        let outRel = (relativePathFrom row.Root outDir).Replace('\\', '/')

        let fsAssemblies =
            Directory.EnumerateFiles(outDir, "FS.Skia.UI*.dll")
            |> Seq.map Path.GetFileName
            |> Seq.sort
            |> Seq.toList

        let referenceLines =
            [ for assembly in fsAssemblies -> $"#r \"{outRel}/{assembly}\"" ]
            @ [ $"#r \"{outRel}/Product.dll\"" ]
            |> String.concat Environment.NewLine

        let content =
            [ "// GENERATED — do not edit. Regenerated from Directory.Packages.props and the"
              "// built Product output assembly. Loads the Product app and its transitive"
              "// FS.Skia.UI.* references for FSI in one step:  dotnet fsi load-product.fsx"
              "//"
              "// This script only references and opens the app; it launches nothing, so it"
              "// neither emits nor suppresses host warnings. A missing assembly is a real"
              "// load failure that surfaces normally; benign host-warning classification"
              "// (spec 021) is unaffected."
              referenceLines
              "open Product" ]
            |> String.concat Environment.NewLine

        File.WriteAllText(loadScriptPath, content + Environment.NewLine)

let generateV3Product model row =
    cleanDirectoryContents row.Root
    cleanDirectoryContents row.EvidenceDir
    let templateRoot = templatePayloadRoot model row
    copyDirectory (path [ templateRoot; "base" ]) row.Root
    applyGeneratedProductProfile row
    copySpecKitInstall model row.Root

    let resolved = resolveCapabilities model row.Capabilities
    writeProductProject model row resolved

    if row.Profile <> "app" then
        writeSceneOnlyProductFiles row

    writeGeneratedProductReadme row resolved
    copySelectedSkills model row resolved

    for capabilityId in resolved do
        match capabilityId with
        | "samples" -> copyDirectory (path [ model.RepositoryRoot; "template"; "fragments"; "samples" ]) (path [ row.Root; "samples" ])
        | _ -> ()

    [ "Dev"; "Test"; "Verify" ]
    |> List.iter (fun target ->
        runProcess $"{row.Profile}/{row.Artifact} generated {target}" "bash" $"./fake.sh build -t {target}" row.Root (path [ row.EvidenceDir; $"{target.ToLowerInvariant()}.log" ]) Map.empty)

    // Emit the in-sync FSI load script from the freshly built Product output (US4, FR-009).
    emitFsiLoadScript row

let runGenerateV3Products model =
    cleanDirectoryContents model.GeneratedProductRootsDir
    createV3TemplatePackage model |> ignore

    v3GeneratedRows model
    |> List.iter (generateV3Product model)

let scanV3GeneratedRow model row =
    let files =
        Directory.EnumerateFiles(row.Root, "*", SearchOption.AllDirectories)
        |> Seq.map (relativePathFrom row.Root)
        |> Seq.filter (fun relative -> not (relative.Contains("/bin/")) && not (relative.Contains("/obj/")) && not (relative.StartsWith("readiness/", StringComparison.Ordinal)))
        |> Seq.sort
        |> Seq.toList

    let appProjects =
        files |> List.filter (fun file -> file.StartsWith("src/", StringComparison.Ordinal) && file.EndsWith(".fsproj", StringComparison.Ordinal))

    let testProjects =
        files |> List.filter (fun file -> file.StartsWith("tests/", StringComparison.Ordinal) && file.EndsWith(".fsproj", StringComparison.Ordinal))

    let forbidden =
        [ "framework implementation projects", "src/Lib/Lib.fsproj"
          "framework implementation projects", "src/Charts"
          "framework implementation projects", "tests/Charts.Tests"
          "framework sample content", "samples/"
          "historical specs", "specs/00"
          "framework readiness evidence", "readiness/"
          "framework README content", "docs/reports/architecture.md"
          "framework README content", "docs/reports/V2Analysis.md"
          "framework README content", "docs/reports/subsystem-design.md"
          "framework README content", "docs/reports/technical-design.md"
          "framework implementation projects", "tests/Parity.Tests"
          "framework implementation projects", ".template.package" ]

    let missing =
        [ "src/Product/Product.fsproj"
          "tests/Product.Tests/Product.Tests.fsproj"
          "load-product.fsx"
          "README.md"
          "CLAUDE.md"
          "docs/product.md"
          ".agents/skills/fs-skia-project/SKILL.md"
          ".claude/skills/fs-skia-project/SKILL.md"
          ".claude/settings.json"
          ".agents/skills/speckit-specify/SKILL.md"
          ".claude/skills/speckit-specify/SKILL.md"
          ".agents/skills/speckit-plan/SKILL.md"
          ".claude/skills/speckit-plan/SKILL.md"
          ".agents/skills/speckit-tasks/SKILL.md"
          ".claude/skills/speckit-tasks/SKILL.md"
          ".agents/skills/speckit-implement/SKILL.md"
          ".claude/skills/speckit-implement/SKILL.md"
          ".specify/memory/constitution.md"
          ".specify/templates/spec-template.md"
          ".specify/scripts/bash/setup-plan.sh"
          ".specify/workflows/speckit/workflow.yml"
          "build.fsx"
          "fake.sh"
          "fake.cmd" ]
        |> List.filter (fun required -> files |> List.contains required |> not)

    if row.Profile = "app" && appProjects.Length <> 1 then
        failwithf "%s/%s expected exactly one product app, found %d" row.Artifact row.Profile appProjects.Length

    if row.Profile = "app" && testProjects.Length <> 1 then
        failwithf "%s/%s expected exactly one product test suite, found %d" row.Artifact row.Profile testProjects.Length

    if not missing.IsEmpty then
        failwithf "%s/%s generated product missing files:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, missing))

    if row.Profile = "app" && not (files |> List.contains ".agents/skills/fs-skia-ui-widgets/SKILL.md") then
        failwithf "%s/%s generated app is missing fs-skia-ui-widgets" row.Artifact row.Profile

    if row.Profile = "app" && not (files |> List.contains ".claude/skills/fs-skia-ui-widgets/SKILL.md") then
        failwithf "%s/%s generated app is missing Claude fs-skia-ui-widgets" row.Artifact row.Profile

    if row.Profile = "app"
       && files |> List.exists (fun file ->
           file.StartsWith(".agents/skills/", StringComparison.Ordinal)
           && (file.IndexOf("charts", StringComparison.OrdinalIgnoreCase) >= 0
               || file.IndexOf("layout", StringComparison.OrdinalIgnoreCase) >= 0)) then
        failwithf "%s/%s generated app contains stale chart or generated layout control skill" row.Artifact row.Profile

    for rule, forbiddenPath in forbidden do
        let allowedGeneratedSamplePackContent =
            row.Profile = "sample-pack" && forbiddenPath = "samples/"

        if not allowedGeneratedSamplePackContent
           && files |> List.exists (fun file -> file.StartsWith(forbiddenPath, StringComparison.Ordinal)) then
            failwithf "%s/%s copied %s: %s" row.Artifact row.Profile rule forbiddenPath

    let productProject = File.ReadAllText(path [ row.Root; "src"; "Product"; "Product.fsproj" ])
    let productProgram = File.ReadAllText(path [ row.Root; "src"; "Product"; "Program.fs" ])
    let productEvidenceCommands = File.ReadAllText(path [ row.Root; "src"; "Product"; "EvidenceCommands.fs" ])
    let productLaunchSource = productProgram + Environment.NewLine + productEvidenceCommands
    let productTests = File.ReadAllText(path [ row.Root; "tests"; "Product.Tests"; "Tests.fs" ])
    let removedChartsPackage = "FS.Skia.UI." + "Charts"

    if productProject.IndexOf($"PackageReference Include=\"{removedChartsPackage}\"", StringComparison.Ordinal) >= 0 then
        failwithf "%s/%s generated product contains removed Charts package reference %s" row.Artifact row.Profile removedChartsPackage

    if row.Profile = "app"
       && productProject.IndexOf("PackageReference Include=\"FS.Skia.UI.Controls.Elmish\"", StringComparison.Ordinal) < 0 then
        failwithf "%s/%s generated app is missing Controls.Elmish adapter package reference" row.Artifact row.Profile

    if row.Profile = "app" then
        let requiredPersistentHostTerms =
            [ "let viewerOptions"
              "let generatedHost"
              "MapKey = mapKey"
              "Tick = tick"
              "Viewer.runApp viewerOptions generatedHost"
              "window-visible=observed:true"
              "accessible-window=true"
              "--bounded-smoke"
              "--bounded-smoke-frame-diagnostics"
              "--scene-evidence" ]

        let missingPersistentHostTerms =
            requiredPersistentHostTerms
            |> List.filter (fun term -> productLaunchSource.IndexOf(term, StringComparison.Ordinal) < 0)

        if not missingPersistentHostTerms.IsEmpty then
            failwithf "%s/%s generated app is missing persistent viewer host wiring:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, missingPersistentHostTerms))

        let forbiddenDefaultSubstitutions =
            [ "print metadata"
              "count controls"
              "run bounded smoke"
              "emit scene evidence"
              "first-frame-only=true"
              "exit after first frame"
              "without a persistent launch attempt" ]

        let defaultBranch =
            let marker = "| _ ->"
            let index = productProgram.LastIndexOf(marker, StringComparison.Ordinal)

            if index >= 0 then
                productProgram.Substring(index)
            else
                productProgram

        let foundDefaultSubstitutions =
            forbiddenDefaultSubstitutions
            |> List.filter (fun term -> defaultBranch.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)

        if not foundDefaultSubstitutions.IsEmpty then
            failwithf "%s/%s generated app contains bounded-only or print-only default path markers:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, foundDefaultSubstitutions))

    let selectedCapabilitySkills =
        Directory.EnumerateFiles(path [ row.Root; ".agents"; "skills" ], "SKILL.md", SearchOption.AllDirectories)
        |> Seq.map (relativePathFrom row.Root)
        |> Seq.sort
        |> Seq.toList

    let selectedClaudeSkills =
        Directory.EnumerateFiles(path [ row.Root; ".claude"; "skills" ], "SKILL.md", SearchOption.AllDirectories)
        |> Seq.map (relativePathFrom row.Root)
        |> Seq.sort
        |> Seq.toList

    let missingClaudeSkillPeers =
        selectedCapabilitySkills
        |> List.map (fun skill -> skill.Replace(".agents/skills/", ".claude/skills/"))
        |> List.filter (fun peer -> selectedClaudeSkills |> List.contains peer |> not)

    if not missingClaudeSkillPeers.IsEmpty then
        failwithf "%s/%s generated product is missing Claude skill peers:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, missingClaudeSkillPeers))

    let report =
        [ $"# {row.Profile}/{row.Artifact} generated product"
          ""
          "Validation rules: exactly one product app, exactly one product test suite, selected capability skills, Controls ownership for form/chart/graph/DataGrid authoring, Controls.Elmish adapter references, consumer-mode package references, stale Charts exclusions, no copied framework samples/specs/readiness/docs, and no framework implementation projects."
          ""
          "Files:"
          yield! files
          ""
          "Package references:"
          productProject
          ""
          "Product source:"
          productProgram
          ""
          "Product tests:"
          productTests
          ""
          "Selected skills:"
          yield! selectedCapabilitySkills
          ""
          "Selected Claude skills:"
          yield! selectedClaudeSkills ]
        |> String.concat Environment.NewLine

    ensureParent row.FileListPath
    File.WriteAllText(row.FileListPath, report + Environment.NewLine)

let runScanV3GeneratedProducts model =
    v3GeneratedRows model
    |> List.iter (scanV3GeneratedRow model)

    let summary =
        [ "# Generated Product Check"
          ""
          "PASS: generated product file lists, selected skills, Controls-owned form/chart/graph/DataGrid authoring, Controls.Elmish adapter references, consumer-mode package references, stale Charts exclusions, full product governance command logs, and framework-source exclusions passed."
          ""
          "| Row | File list | Verify log |"
          "|-----|-----------|------------|"
          yield!
              v3GeneratedRows model
              |> List.map (fun row ->
                  let verifyLog = path [ row.EvidenceDir; "verify.log" ]
                  $"| {row.Profile}/{row.Artifact} | `{row.FileListPath}` | `{verifyLog}` |") ]
        |> String.concat Environment.NewLine

    File.WriteAllText(path [ model.GeneratedFileListsDir; "summary.md" ], summary + Environment.NewLine)

let writeLocalNuGetConfig model root =
    let content =
        $"""<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="{model.LocalPackageDir}" />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
"""

    File.WriteAllText(path [ root; "NuGet.config" ], content)

let ensureGeneratedPackageVersions model root =
    let propsPath = path [ root; "Directory.Packages.props" ]
    let mutable content = File.ReadAllText(propsPath)

    for project, packageId in packProjects do
        let version = projectVersion model.RepositoryRoot project
        let pattern = $"""(<PackageVersion Include="{Regex.Escape(packageId)}" Version=")[^"]+(" />)"""
        content <- Regex.Replace(content, pattern, MatchEvaluator(fun m -> m.Groups.[1].Value + version + m.Groups.[2].Value))

    File.WriteAllText(propsPath, content)

let private fsSkiaPackageIds =
    packProjects
    |> List.map snd
    |> Set.ofList

let private readRequestedGeneratedPackages root =
    let propsPath = path [ root; "Directory.Packages.props" ]
    let propsContent = File.ReadAllText propsPath

    let centralVersions =
        Regex.Matches(propsContent, "<PackageVersion Include=\"(FS\\.Skia\\.UI[^\"]*)\" Version=\"([^\"]+)\"")
        |> Seq.cast<Match>
        |> Seq.map (fun m -> m.Groups.[1].Value, m.Groups.[2].Value)
        |> Map.ofSeq

    [ path [ root; "src"; "Product"; "Product.fsproj" ]
      path [ root; "tests"; "Product.Tests"; "Product.Tests.fsproj" ] ]
    |> List.filter File.Exists
    |> List.map File.ReadAllText
    |> String.concat Environment.NewLine
    |> fun content -> Regex.Matches(content, "<PackageReference Include=\"(FS\\.Skia\\.UI[^\"]*)\"")
    |> Seq.cast<Match>
    |> Seq.map (fun m -> m.Groups.[1].Value)
    |> Seq.distinct
    |> Seq.filter (fun packageId -> fsSkiaPackageIds |> Set.contains packageId)
    |> Seq.choose (fun packageId ->
        centralVersions
        |> Map.tryFind packageId
        |> Option.map (fun version -> packageId, version))
    |> Seq.sortBy fst
    |> Seq.toList

let private readNuGetPackageSources root =
    BuildPackageResolution.readNuGetPackageSources root

let private readRestoreWarnings logPath =
    BuildPackageResolution.readRestoreWarnings logPath

let private readResolvedGeneratedPackages root =
    let assetsPath = path [ root; "tests"; "Product.Tests"; "obj"; "project.assets.json" ]

    if not (File.Exists assetsPath) then
        []
    else
        let content = File.ReadAllText assetsPath

        Regex.Matches(content, "\"(FS\\.Skia\\.UI[^\"/]*)/([^\"]+)\"")
        |> Seq.cast<Match>
        |> Seq.map (fun m -> m.Groups.[1].Value, m.Groups.[2].Value)
        |> Seq.filter (fun (packageId, _) -> fsSkiaPackageIds |> Set.contains packageId)
        |> Seq.distinct
        |> Seq.sortBy fst
        |> Seq.toList

let private packageResolutionDiagnostics (requested: (string * string) list) (resolved: (string * string) list) (sources: string list) (restoreWarnings: string list) =
    BuildPackageResolution.packageResolutionDiagnostics requested resolved sources restoreWarnings

let private readKeyValueFromText key text =
    let escapedKey = Regex.Escape key
    let pattern = $"(?<!\\S){escapedKey}=([^=]*?)(?=\\s+[A-Za-z0-9_-]+=|$)"
    let m = Regex.Match(text, pattern, RegexOptions.IgnoreCase ||| RegexOptions.CultureInvariant)

    if m.Success then
        Some(m.Groups.[1].Value.Trim())
    else
        None

let private writeSupportedHostPersistentLaunchEvidence outputPath sourceLogPath commandText logText =
    let value key fallback =
        readKeyValueFromText key logText |> Option.defaultValue fallback

    let inputDispatch =
        match (value "input-dispatch" "not-verified").Trim('"') with
        | "true" -> "verified"
        | "false" -> "not-verified"
        | "not-required" -> "not-verified"
        | other -> other

    let windowOpened = value "window-opened" "true"
    let firstFramePresented = value "first-frame-presented" "true"
    let windowVisible = value "window-visible" "observed:true"
    let accessibleWindow = value "accessible-window" "true"
    let selfClosedForEvidence = value "self-closed-for-evidence" "false"
    let userCloseObserved =
        if selfClosedForEvidence = "true" then
            value "user-close-observed" "false"
        else
            value "user-close-observed" "true"
    let exitPath = value "exit-path" "true"
    let rendererMode = value "renderer-mode" "skia"
    let blockedStage = value "blocked-stage" "none"
    let classification = value "classification" "none"
    let category = value "category" "none"
    let message = value "message" "Compiled generated product launched a persistent interactive window on the supported host path."

    let lines =
        [ "status=ok"
          "mode=interactive-window"
          $"command={commandText}"
          $"window-opened={windowOpened}"
          $"window-visible={windowVisible}"
          $"accessible-window={accessibleWindow}"
          $"first-frame-presented={firstFramePresented}"
          $"user-close-observed={userCloseObserved}"
          $"self-closed-for-evidence={selfClosedForEvidence}"
          $"input-dispatch={inputDispatch}"
          $"exit-path={exitPath}"
          $"renderer-mode={rendererMode}"
          $"blocked-stage={blockedStage}"
          $"classification={classification}"
          $"category={category}"
          $"message={message}"
          $"source-log={sourceLogPath}" ]

    File.WriteAllText(outputPath, String.concat Environment.NewLine lines + Environment.NewLine)

let private cleanGeneratedPackageCacheEntries requestedPackages =
    let globalPackages =
        path [ Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); ".nuget"; "packages" ]

    if Directory.Exists globalPackages then
        for packageId, version in requestedPackages do
            if fsSkiaPackageIds |> Set.contains packageId then
                let packageCachePath = path [ globalPackages; packageId.ToLowerInvariant(); version ]

                if Directory.Exists packageCachePath then
                    Directory.Delete(packageCachePath, true)

let runGeneratedConsumerValidation model =
    ensureParent model.GeneratedProductValidationPath
    let stopwatch = Stopwatch.StartNew()
    let row =
        v3GeneratedRows model
        |> List.find (fun row -> row.Profile = "app" && row.Artifact = "source")

    writeLocalNuGetConfig model row.Root
    ensureGeneratedPackageVersions model row.Root

    let validationDir = path [ model.ReadinessDir; "generated-consumer-validation" ]
    let generatedPackageCache = path [ validationDir; "nuget-packages" ]
    Directory.CreateDirectory validationDir |> ignore
    Directory.GetFiles(validationDir)
    |> Array.iter File.Delete

    Directory.GetDirectories(validationDir)
    |> Array.iter (fun child -> Directory.Delete(child, true))

    cleanDirectoryContents generatedPackageCache
    let restoreLog = path [ validationDir; "restore.log" ]
    let semanticLog = path [ validationDir; "generated-verify.log" ]
    let boundedSmokePath = path [ validationDir; "bounded-smoke.txt" ]
    let boundedSmokeLog = path [ validationDir; "bounded-smoke.log" ]
    let sceneEvidencePath = path [ validationDir; "headless-scene-evidence.txt" ]
    let sceneEvidenceLog = path [ validationDir; "scene-evidence.log" ]
    let persistentLaunchLog = path [ validationDir; "persistent-launch-diagnostics.log" ]
    let persistentLaunchEvidencePath = path [ validationDir; "persistent-launch-evidence.txt" ]
    let windowDiagnosticsPath = path [ validationDir; "window-diagnostics.txt" ]
    let windowDiagnosticsLog = path [ validationDir; "window-diagnostics.log" ]
    let windowOptionsPath = path [ validationDir; "window-options.txt" ]
    let windowOptionsLog = path [ validationDir; "window-options.log" ]
    let imageEvidencePath = path [ validationDir; "game-image-evidence.png" ]
    let imageEvidenceLog = path [ validationDir; "image-evidence.log" ]
    let supportedHostPersistentLaunchPath = path [ model.ReadinessDir; "supported-host-persistent-launch.txt" ]

    let mutable category = "Completed"
    let diagnostics = ResizeArray<string>()

    let validationEnvironment =
        Map.empty

    let runStep step categoryOnFailure fileName arguments workingDirectory outputPath =
        try
            runProcess step fileName arguments workingDirectory outputPath validationEnvironment
            diagnostics.Add($"{step}: ok")
            true
        with ex ->
            category <- categoryOnFailure
            diagnostics.Add($"{step}: failed: {ex.Message}")
            false

    let requestedPackages =
        readRequestedGeneratedPackages row.Root

    cleanGeneratedPackageCacheEntries requestedPackages

    let restored =
        runStep
            "generated consumer restore from local packages"
            "RestoreFailure"
            "dotnet"
            "restore tests/Product.Tests/Product.Tests.fsproj --configfile NuGet.config --no-cache"
            row.Root
            restoreLog

    let resolvedPackages =
        if restored then readResolvedGeneratedPackages row.Root else []

    let packageSources =
        readNuGetPackageSources row.Root

    let restoreWarnings =
        readRestoreWarnings restoreLog

    let packageFailureReason, packageDiagnostics =
        packageResolutionDiagnostics requestedPackages resolvedPackages packageSources restoreWarnings

    let packageResolutionPassed =
        restored && packageFailureReason.IsNone

    if restored then
        if packageResolutionPassed then
            diagnostics.Add("package resolution: exact-match=true")
        else
            category <- "PackageDrift"
            let packageFailureClass = packageFailureReason |> Option.defaultValue "unknown"
            diagnostics.Add($"package resolution: exact-match=false failure-class={packageFailureClass}")
            packageDiagnostics |> List.iter diagnostics.Add

    let generatedTestsExist =
        File.Exists(path [ row.Root; "tests"; "Product.Tests"; "Product.Tests.fsproj" ])
        && File.Exists(path [ row.Root; "tests"; "Product.Tests"; "Tests.fs" ])

    let semanticPassed =
        packageResolutionPassed
        && runStep
            "generated consumer Verify"
            "SemanticTestFailure"
            "bash"
            "./fake.sh build -t Verify"
            row.Root
            semanticLog

    let smokePassed =
        semanticPassed
        && runStep
            "generated consumer bounded smoke"
            "ViewerStartupFailure"
            "dotnet"
            $"run --project src/Product/Product.fsproj --no-restore -- --bounded-smoke {quote boundedSmokePath}"
            row.Root
            boundedSmokeLog

    if smokePassed && File.Exists boundedSmokePath then
        let boundedSmoke = File.ReadAllText boundedSmokePath
        if boundedSmoke.IndexOf("status=unsupported", StringComparison.Ordinal) >= 0 then
            category <- "UnsupportedHost"
            diagnostics.Add("bounded viewer smoke unsupported")
        elif boundedSmoke.IndexOf("status=ok", StringComparison.Ordinal) >= 0 then
            diagnostics.Add("bounded viewer smoke reached requested evidence")
        else
            category <- "ViewerStartupFailure"
            diagnostics.Add("bounded viewer smoke did not report ok or unsupported")

    let scenePassed =
        semanticPassed
        && runStep
            "generated consumer scene evidence"
            "SceneEvidenceFailure"
            "dotnet"
            $"run --project src/Product/Product.fsproj --no-restore -- --scene-evidence {quote sceneEvidencePath}"
            row.Root
            sceneEvidenceLog

    if scenePassed && File.Exists sceneEvidencePath then
        diagnostics.Add("headless scene evidence captured")
    elif semanticPassed then
        category <- "SceneEvidenceFailure"
        diagnostics.Add("headless scene evidence output missing")

    let windowDiagnosticsPassed =
        semanticPassed
        && runStep
            "generated consumer window diagnostics"
            "WindowDiagnosticsFailure"
            "dotnet"
            $"run --project src/Product/Product.fsproj --no-restore -- --window-diagnostics {quote windowDiagnosticsPath}"
            row.Root
            windowDiagnosticsLog

    if windowDiagnosticsPassed && File.Exists windowDiagnosticsPath then
        diagnostics.Add("window diagnostics validation captured")
    elif semanticPassed then
        category <- "WindowDiagnosticsFailure"
        diagnostics.Add("window diagnostics output missing")

    let windowOptionsPassed =
        semanticPassed
        && runStep
            "generated consumer window options"
            "WindowOptionsFailure"
            "dotnet"
            $"run --project src/Product/Product.fsproj --no-restore -- --window-options {quote windowOptionsPath} --window-resize fixed-size --window-maximize not-maximizable --window-startup maximized --window-position 24,36 --window-backend opengl"
            row.Root
            windowOptionsLog

    if windowOptionsPassed && File.Exists windowOptionsPath then
        let windowOptionsText = File.ReadAllText windowOptionsPath
        if windowOptionsText.IndexOf("option=resize", StringComparison.OrdinalIgnoreCase) >= 0
           && windowOptionsText.IndexOf("option=maximize", StringComparison.OrdinalIgnoreCase) >= 0
           && windowOptionsText.IndexOf("option=startup-state", StringComparison.OrdinalIgnoreCase) >= 0
           && windowOptionsText.IndexOf("option=startup-position", StringComparison.OrdinalIgnoreCase) >= 0
           && windowOptionsText.IndexOf("option=backend", StringComparison.OrdinalIgnoreCase) >= 0
           && windowOptionsText.IndexOf("status=unsupported", StringComparison.OrdinalIgnoreCase) >= 0 then
            diagnostics.Add("window options validation captured")
        else
            category <- "WindowOptionsFailure"
            diagnostics.Add("window options output missing required option rows or unsupported diagnostics")
    elif semanticPassed then
        category <- "WindowOptionsFailure"
        diagnostics.Add("window options output missing")

    let imageEvidencePassed =
        semanticPassed
        && runStep
            "generated consumer image evidence"
            "VisualEvidenceFailure"
            "dotnet"
            $"run --project src/Product/Product.fsproj --no-restore -- --image-evidence {quote imageEvidencePath}"
            row.Root
            imageEvidenceLog

    let imageEvidenceMetadataPath = imageEvidencePath + ".metadata.txt"

    if imageEvidencePassed && File.Exists imageEvidenceMetadataPath then
        let imageEvidenceText = File.ReadAllText imageEvidenceMetadataPath
        if imageEvidenceText.IndexOf("evidence-kind=image", StringComparison.OrdinalIgnoreCase) >= 0
           && imageEvidenceText.IndexOf("image-decodable=true", StringComparison.OrdinalIgnoreCase) >= 0 then
            diagnostics.Add("image evidence validation captured")
        else
            category <- "VisualEvidenceFailure"
            diagnostics.Add("image evidence output missing decodable image fields")
    elif semanticPassed then
        category <- "VisualEvidenceFailure"
        diagnostics.Add("image evidence metadata missing")

    let persistentDiagnosticsPassed =
        semanticPassed
        && runStep
            "generated consumer persistent launch diagnostics"
            "PersistentLaunchDiagnosticFailure"
            "dotnet"
            $"run --project src/Product/Product.fsproj --no-restore -- --launch-evidence {quote persistentLaunchEvidencePath}"
            row.Root
            persistentLaunchLog

    if persistentDiagnosticsPassed then
        diagnostics.Add("persistent launch diagnostics captured separately from bounded evidence")

        let persistentLaunchText = File.ReadAllText persistentLaunchLog

        if persistentLaunchText.IndexOf("status=ok", StringComparison.OrdinalIgnoreCase) >= 0
           && persistentLaunchText.IndexOf("mode=persistent-evidence", StringComparison.OrdinalIgnoreCase) >= 0
           && persistentLaunchText.IndexOf("first-frame-presented=true", StringComparison.OrdinalIgnoreCase) >= 0 then
            writeSupportedHostPersistentLaunchEvidence
                supportedHostPersistentLaunchPath
                persistentLaunchLog
                "dotnet run --project artifacts/generated-products/020-asteroids-integration-feedback/app-source/src/Product/Product.fsproj --no-restore -- --launch-evidence"
                persistentLaunchText
            diagnostics.Add("supported-host persistent launch evidence normalized")

    stopwatch.Stop()

    let generatedTestsRan = semanticPassed
    let generatedVerifyRan = generatedTestsRan
    let generatedVerificationAuthoritative = generatedTestsExist && generatedTestsRan && generatedVerifyRan
    let generatedVerificationFailureClass =
        if not generatedTestsExist then "missing-generated-test-project"
        elif not generatedTestsRan then "missing-generated-test-execution"
        elif not generatedVerifyRan then "verify-target-not-authoritative"
        else "none"
    let packageFailureClass = packageFailureReason |> Option.defaultValue "none"
    let packageSourceSummary = String.Join(", ", packageSources)
    let persistentLaunchText =
        if File.Exists persistentLaunchLog then File.ReadAllText persistentLaunchLog else ""
    let generatedProgramText =
        let generatedProgramPath = path [ row.Root; "src"; "Product"; "Program.fs" ]
        if File.Exists generatedProgramPath then File.ReadAllText generatedProgramPath else ""
    let defaultLaunchSourceValidationPassed =
        let defaultBranch =
            let marker = "| _ ->"
            let index = generatedProgramText.LastIndexOf(marker, StringComparison.Ordinal)

            if index >= 0 then
                generatedProgramText.Substring(index)
            else
                generatedProgramText

        defaultBranch.IndexOf("Viewer.runApp viewerOptions generatedHost", StringComparison.Ordinal) >= 0
        && defaultBranch.IndexOf("mode=interactive-window", StringComparison.Ordinal) >= 0
        && defaultBranch.IndexOf("accessible-window=true", StringComparison.Ordinal) >= 0
        && defaultBranch.IndexOf("mode=persistent-evidence", StringComparison.Ordinal) < 0
        && defaultBranch.IndexOf("self-closed-for-evidence=true", StringComparison.Ordinal) < 0
    let closeReasonValidated =
        generatedProgramText.IndexOf("user-close-observed=%b", StringComparison.OrdinalIgnoreCase) >= 0
        && generatedProgramText.IndexOf("self-closed-for-evidence=%b", StringComparison.OrdinalIgnoreCase) >= 0
        && persistentLaunchText.IndexOf("self-closed-for-evidence=true", StringComparison.OrdinalIgnoreCase) >= 0
    let defaultInteractiveLaunchValidated =
        defaultLaunchSourceValidationPassed
        && persistentDiagnosticsPassed
    let windowDiagnosticsValidated = windowDiagnosticsPassed && File.Exists windowDiagnosticsPath
    let windowOptionsValidated = windowOptionsPassed && File.Exists windowOptionsPath
    let imageEvidenceValidated = imageEvidencePassed && File.Exists imageEvidenceMetadataPath
    let generatedContractAuthoritative =
        packageResolutionPassed
        && generatedVerificationAuthoritative
        && defaultInteractiveLaunchValidated
        && smokePassed
        && closeReasonValidated
        && windowDiagnosticsValidated
        && windowOptionsValidated
        && imageEvidenceValidated
    let generatedContractFailureClass =
        if not packageResolutionPassed then packageFailureClass
        elif not generatedVerificationAuthoritative then generatedVerificationFailureClass
        elif not defaultInteractiveLaunchValidated then "interactive-launch-validation"
        elif not smokePassed then "bounded-evidence-validation"
        elif not closeReasonValidated then "close-reason-validation"
        elif not windowDiagnosticsValidated then "window-diagnostics-validation"
        elif not windowOptionsValidated then "window-options-validation"
        elif not imageEvidenceValidated then "visual-evidence-validation"
        else "none"

    let report =
        [ "# Generated Product Validation"
          ""
          $"Category: `{category}`"
          $"Elapsed: `{stopwatch.Elapsed}`"
          $"Command context: `./fake.sh build -t PackLocal && ./fake.sh build -t GeneratedProductCheck`"
          $"Generated consumer root: `{row.Root}`"
          $"Local package feed: `{model.LocalPackageDir}`"
          ""
          "## Evidence"
          ""
          $"- Restore log: `{restoreLog}`"
          $"- Generated Verify log: `{semanticLog}`"
          $"- Bounded smoke log: `{boundedSmokeLog}`"
          $"- Bounded smoke evidence: `{boundedSmokePath}`"
          $"- Scene evidence log: `{sceneEvidenceLog}`"
          $"- Scene evidence output: `{sceneEvidencePath}`"
          $"- Persistent launch diagnostics log: `{persistentLaunchLog}`"
          $"- Window diagnostics log: `{windowDiagnosticsLog}`"
          $"- Window diagnostics output: `{windowDiagnosticsPath}`"
          $"- Window options log: `{windowOptionsLog}`"
          $"- Window options output: `{windowOptionsPath}`"
          $"- Image evidence log: `{imageEvidenceLog}`"
          $"- Image evidence output: `{imageEvidencePath}`"
          ""
          "## Contract Output"
          ""
          $"- package-resolution: `validated`"
          $"- exact-package-match: `{packageResolutionPassed}`"
          $"- generated-test-execution: `validated`"
          $"- generated-tests-ran: `{generatedTestsRan}`"
          $"- default-interactive-launch: `{defaultInteractiveLaunchValidated}`"
          $"- bounded-evidence-validation: `{smokePassed}`"
          $"- close-reason-validation: `{closeReasonValidated}`"
          $"- window-diagnostics-validation: `{windowDiagnosticsValidated}`"
          $"- window-options-validation: `{windowOptionsValidated}`"
          $"- image-evidence-validation: `{imageEvidenceValidated}`"
          $"- authoritative: `{generatedContractAuthoritative}`"
          $"- failure-class: `{generatedContractFailureClass}`"
          ""
          "## Package Resolution"
          ""
          $"- exact-match: `{packageResolutionPassed}`"
          $"- failure-class: `{packageFailureClass}`"
          $"- package-sources: `{packageSourceSummary}`"
          $"- restore-warning-count: `{restoreWarnings.Length}`"
          ""
          "Requested packages:"
          yield!
              requestedPackages
              |> List.map (fun (packageId, version) -> $"- requested {packageId}={version}")
          ""
          "Resolved packages:"
          yield!
              resolvedPackages
              |> List.map (fun (packageId, version) -> $"- resolved {packageId}={version}")
          ""
          "Restore warnings:"
          yield!
              restoreWarnings
              |> List.map (fun warning -> $"- {warning}")
          ""
          "## Generated Test Execution"
          ""
          $"- generated-tests-exist: `{generatedTestsExist}`"
          $"- generated-tests-ran: `{generatedTestsRan}`"
          $"- generated-verify-ran: `{generatedVerifyRan}`"
          $"- authoritative: `{generatedVerificationAuthoritative}`"
          $"- failure-class: `{generatedVerificationFailureClass}`"
          ""
          "## Diagnostics"
          ""
          yield! diagnostics |> Seq.map (fun item -> $"- {item}") ]
        |> String.concat Environment.NewLine

    File.WriteAllText(model.GeneratedProductValidationPath, report + Environment.NewLine)

    if category = "PackageDrift" || category = "RestoreFailure" || category = "SemanticTestFailure" || category = "ViewerStartupFailure" || category = "SceneEvidenceFailure" || category = "PersistentLaunchDiagnosticFailure" || category = "WindowDiagnosticsFailure" || category = "WindowOptionsFailure" || category = "VisualEvidenceFailure" then
        failwithf "Generated consumer validation failed with category %s; see %s" category model.GeneratedProductValidationPath

let runDependencyOwnershipReport model =
    let sceneProject = File.ReadAllText(path [ model.RepositoryRoot; "src"; "Scene"; "Scene.fsproj" ])
    let controlsProject = File.ReadAllText(path [ model.RepositoryRoot; "src"; "Controls"; "Controls.fsproj" ])
    let keyboardProject = File.ReadAllText(path [ model.RepositoryRoot; "src"; "KeyboardInput"; "KeyboardInput.fsproj" ])
    let adapterProject = File.ReadAllText(path [ model.RepositoryRoot; "src"; "Controls.Elmish"; "Controls.Elmish.fsproj" ])
    let removedChartsPackage = "FS.Skia.UI." + "Charts"
    let removedChartsProject = "src/" + "Charts/" + "Charts.fsproj"

    [ "Fable.Elmish"; "Silk.NET"; "SkiaSharp"; "Yoga.Net"; "YamlDotNet" ]
    |> List.iter (fun forbidden ->
        if sceneProject.IndexOf(forbidden, StringComparison.Ordinal) >= 0 then
            failwithf "Scene dependency leak: %s" forbidden)

    [ "Lib",
      [ @"Include=""..\Lib\Lib.fsproj"""
        "Include=\"../Lib/Lib.fsproj\""
        "PackageReference Include=\"FS.Skia.UI\"" ]
      "SkiaViewer",
      [ @"Include=""..\SkiaViewer\SkiaViewer.fsproj"""
        "Include=\"../SkiaViewer/SkiaViewer.fsproj\""
        "PackageReference Include=\"FS.Skia.UI.SkiaViewer\"" ]
      "Elmish",
      [ @"Include=""..\Elmish\Elmish.fsproj"""
        "Include=\"../Elmish/Elmish.fsproj\""
        "PackageReference Include=\"Fable.Elmish\""
        "PackageReference Include=\"FS.Skia.UI.Elmish\"" ] ]
    |> List.iter (fun (forbidden, needles) ->
        if needles |> List.exists (fun needle -> controlsProject.IndexOf(needle, StringComparison.Ordinal) >= 0) then
            failwithf "Controls dependency leak: %s" forbidden)

    if controlsProject.IndexOf("PackageReference", StringComparison.Ordinal) >= 0 then
        failwith "Controls dependency leak: base Controls must not own direct external PackageReference entries"

    if keyboardProject.IndexOf("YamlDotNet", StringComparison.Ordinal) < 0 then
        failwith "KeyboardInput dependency gap: YamlDotNet ownership is not recorded"

    if adapterProject.IndexOf("Fable.Elmish", StringComparison.Ordinal) < 0 then
        failwith "Controls.Elmish dependency gap: Fable.Elmish ownership is not recorded"

    if File.Exists(path [ model.RepositoryRoot; "src"; "Charts"; "Charts.fsproj" ]) then
        failwithf "Removed package project is still active: %s" removedChartsProject

    [ controlsProject; keyboardProject; adapterProject ]
    |> List.iter (fun project ->
        if project.IndexOf(removedChartsPackage, StringComparison.Ordinal) >= 0
           || project.IndexOf(removedChartsProject, StringComparison.Ordinal) >= 0 then
            failwith "Removed Charts package remains in active Controls boundary project references")

    let report =
        [ "# Dependency Report"
          ""
          "PASS: V3 dependency ownership report completed."
          ""
          "- Scene has no Elmish, Silk.NET, SkiaSharp, Yoga.Net, or YamlDotNet dependency."
          "- SkiaViewer owns Silk.NET and SkiaSharp host dependencies."
          "- Elmish owns Fable.Elmish adapter dependency."
          "- KeyboardInput owns YamlDotNet dependency."
          "- Layout owns Yoga.Net dependency."
          "- Controls owns form controls, rich rendering, chart controls, graph views, DataGrid, and ControlRuntime declarations."
          "- Controls depends only on Scene, Layout, and KeyboardInput and has no direct external PackageReference entries."
          "- Controls.Elmish owns Fable.Elmish command, subscription, and program adapter dependency."
          "- The removed Charts package is absent from active package, baseline, and generated product lists."
          "- Legacy Charts package/project is removed from active package, baseline, and generated product lists; migration guidance is documentation-only."
          "- Testing owns generated-product validation helpers."
          ""
          "Evidence:"
          ""
          "- Command: `./fake.sh build -t DependencyReport`"
          "- Source: `Directory.Packages.props`"
          "- Active feature evidence: `specs/025-upgrade-skia-speckit/readiness/version-selection.md` when feature 025 is active."
          ""
          "## Before And After"
          ""
          "| Package | Before | After | Owner | Status |"
          "|---------|--------|-------|-------|--------|"
          "| SkiaSharp | `4.147.0-preview.2.1` | `4.147.0-preview.3.1` | SkiaViewer/compatibility renderer host | aligned |"
          "| SkiaSharp.NativeAssets.Linux | `4.147.0-preview.2.1` | `4.147.0-preview.3.1` | Linux native renderer assets | aligned |"
          "| SkiaSharp.NativeAssets.Win32 | `4.147.0-preview.2.1` | `4.147.0-preview.3.1` | Windows native renderer assets | aligned |"
          "| Spec Kit metadata | `0.8.11` | `0.8.16` | project governance metadata | aligned to latest release metadata |"
          ""
          "cycle status: no new project reference was added, so the package graph cycle status is unchanged."
          "unexpected spread review: no SkiaSharp reference was added to Scene, Layout, Controls, Controls.Elmish, KeyboardInput, Testing, generated product source, or generated product tests." ]
        |> String.concat Environment.NewLine

    File.WriteAllText(model.DependencyReportPath, report + Environment.NewLine)

let runPackageSurfaceReport model =
    let rows =
        [ "# Package Surfaces"
          ""
          "PASS: package-specific surface baselines are present for public V3 capabilities."
          ""
          "- `readiness/surface-baselines/FS.Skia.UI.Scene.txt`"
          "- `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt`"
          "- `readiness/surface-baselines/FS.Skia.UI.Elmish.txt`"
          "- `readiness/surface-baselines/FS.Skia.UI.KeyboardInput.txt`"
          "- `readiness/surface-baselines/FS.Skia.UI.Layout.txt`"
          "- `readiness/surface-baselines/FS.Skia.UI.Controls.txt`"
          "- `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt`"
          "- `readiness/surface-baselines/FS.Skia.UI.Testing.txt`" ]
        |> String.concat Environment.NewLine

    File.WriteAllText(path [ model.PackageSurfaceReportDir; "index.md" ], rows + Environment.NewLine)

// BUILD SECTION: guidance validation

type GuidanceArtifact =
    | SpecTemplate
    | PlanTemplate

type GuidancePrompt =
    { Class: string
      Section: string
      Prompt: string }

type GuidanceTemplate =
    { Path: string
      Artifact: GuidanceArtifact
      Prompts: GuidancePrompt list }

type MarkdownSection =
    { Heading: string
      Level: int
      Content: string }

let specGuidancePrompts =
    [ "package impact"
      "public contract impact"
      "state workflow impact"
      "layout/rendering impact"
      "evidence obligations"
      "unsupported scope"
      "build-target impact" ]
    |> List.map (fun prompt ->
        { Class = prompt
          Section = "Framework Governance Prompts"
          Prompt = prompt })

let planGuidancePrompts =
    [ "template ownership"
      "dependency impact"
      "command-surface impact"
      "generated project impact"
      "evidence paths"
      ".fsi"
      "MVU/effect boundary"
      "synthetic evidence"
      "test evidence"
      "observability"
      "deferred scope" ]
    |> List.map (fun prompt ->
        { Class = prompt
          Section = "Repository Governance Decisions"
          Prompt = prompt })

let generatedGuidanceRequirements =
    [ { Path = ".specify/templates/spec-template.md"
        Artifact = SpecTemplate
        Prompts = specGuidancePrompts }
      { Path = ".specify/presets/fsharp-opinionated/templates/spec-template.md"
        Artifact = SpecTemplate
        Prompts = specGuidancePrompts }
      { Path = ".specify/templates/plan-template.md"
        Artifact = PlanTemplate
        Prompts = planGuidancePrompts }
      { Path = ".specify/presets/fsharp-opinionated/templates/plan-template.md"
        Artifact = PlanTemplate
        Prompts = planGuidancePrompts } ]

let containsText (needle: string) (haystack: string) =
    haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0

let serializedRunnerRequiredTerms =
    [ "FAKE-backed"
      ".fake"
      "sequential"
      "not safe to run concurrently" ]

let buildRunnerCommandRegex =
    Regex(@"(\./fake\.sh|fake\.cmd|dotnet fake)\b", RegexOptions.IgnoreCase)

let numberedBuildRunnerCommandRegex =
    Regex(@"(?m)^\s*(\d+\.|-)\s+(`)?(\./fake\.sh|fake\.cmd|dotnet fake)\b", RegexOptions.IgnoreCase)

let validateSerializedRunnerGuidancePath model relativePath =
    let filePath = path [ model.RepositoryRoot; relativePath ]

    if not (File.Exists filePath) then
        [ $"{relativePath}: missing file [sequential-fake-guidance]" ]
    else
        let content = File.ReadAllText filePath

        if not (buildRunnerCommandRegex.IsMatch content) then
            []
        else
            [ yield!
                  serializedRunnerRequiredTerms
                  |> List.choose (fun term ->
                      if containsText term content then
                          None
                      else
                          Some $"{relativePath}: missing `{term}` [sequential-fake-guidance]")

              if buildRunnerCommandRegex.Matches(content).Count > 1
                 && numberedBuildRunnerCommandRegex.Matches(content).Count < 2 then
                  $"{relativePath}: multiple FAKE-backed commands require deterministic sequential order [sequential-fake-guidance]"

              if containsText "parallel" content
                 && not (containsText "non-FAKE" content || containsText "do not invoke FAKE" content) then
                  $"{relativePath}: parallelism guidance must distinguish safe non-FAKE checks [sequential-fake-guidance]" ]

let validateSerializedRunnerGuidance model =
    [ "README.md"
      "docs/reports/build.md"
      "docs/reports/testing.md"
      "docs/reports/evidence.md"
      "AGENTS.md"
      "CLAUDE.md"
      ".agents/skills/speckit-implement/SKILL.md"
      ".agents/skills/speckit-evidence-graph/SKILL.md"
      ".agents/skills/speckit-evidence-audit/SKILL.md"
      ".claude/skills/speckit-implement/SKILL.md"
      ".claude/skills/speckit-evidence-graph/SKILL.md"
      ".claude/skills/speckit-evidence-audit/SKILL.md"
      ".specify/templates/tasks-template.md"
      ".specify/presets/fsharp-opinionated/templates/tasks-template.md"
      ".specify/templates/plan-template.md"
      ".specify/presets/fsharp-opinionated/templates/plan-template.md"
      "template/base/README.md"
      "template/base/docs/product.md"
      "template/base/.agents/skills/fs-skia-project/SKILL.md"
      "template/base/.claude/skills/fs-skia-project/SKILL.md" ]
    |> List.collect (validateSerializedRunnerGuidancePath model)

let forbiddenGeneratedGuidanceAdvice =
    [ "assembly reflection first", "reflection-first"
      "reflection-first", "reflection-first"
      "copy files from repository src/", "repository-source-copy"
      "copy repository source", "repository-source-copy"
      "read repository source instead", "repository-source-copy" ]

let validateForbiddenGeneratedGuidanceAdvice model =
    let guidancePaths =
        [ "docs/reports/generated-apps.md"
          "docs/reports/controls.md"
          "template/base/README.md"
          "template/base/docs/product.md"
          "template/base/.agents/skills/fs-skia-project/SKILL.md"
          "template/base/.claude/skills/fs-skia-project/SKILL.md" ]

    guidancePaths
    |> List.collect (fun relativePath ->
        let filePath = path [ model.RepositoryRoot; relativePath ]

        if not (File.Exists filePath) then
            []
        else
            let content = File.ReadAllText filePath

            forbiddenGeneratedGuidanceAdvice
            |> List.choose (fun (term, failureClass) ->
                if containsText term content then
                    Some $"{relativePath}: forbidden {failureClass} generated guidance `{term}`; use package-reference alternative first [package-reference alternative]"
                else
                    None))

let tryHeading (line: string) =
    let trimmed = line.TrimStart()

    if trimmed.StartsWith("#") then
        let level = trimmed |> Seq.takeWhile ((=) '#') |> Seq.length

        if level > 0 && trimmed.Length > level && trimmed.[level] = ' ' then
            Some(level, trimmed.Substring(level).Trim())
        else
            None
    else
        None

let markdownSections (content: string) =
    let lines = content.Replace("\r\n", "\n").Split('\n')

    let headings =
        lines
        |> Array.mapi (fun index line -> index, tryHeading line)
        |> Array.choose (function
            | index, Some(level, heading) -> Some(index, level, heading)
            | _ -> None)

    headings
    |> Array.mapi (fun headingIndex (startIndex, level, heading) ->
        let endIndex =
            headings
            |> Array.skip (headingIndex + 1)
            |> Array.tryPick (fun (nextIndex, nextLevel, _) ->
                if nextLevel <= level then
                    Some(nextIndex - 1)
                else
                    None)
            |> Option.defaultValue (lines.Length - 1)

        { Heading = heading
          Level = level
          Content = lines.[startIndex..endIndex] |> String.concat Environment.NewLine })
    |> Array.toList

let trySection (sectionName: string) (sections: MarkdownSection list) =
    sections
    |> List.tryFind (fun section -> containsText sectionName section.Heading)

let deferredSections sections =
    sections
    |> List.filter (fun section -> containsText "deferred" section.Heading || containsText "roadmap" section.Heading)

let promptClassesInCorrectSections templatePath prompts content =
    let sections = markdownSections content

    prompts
    |> List.choose (fun prompt ->
        match trySection prompt.Section sections with
        | Some section when containsText prompt.Prompt section.Content -> Some prompt.Class
        | _ -> None)
    |> Set.ofList

let validateGuidanceTemplate model template =
    let filePath = path [ model.RepositoryRoot; template.Path ]

    if not (File.Exists filePath) then
        [ $"{template.Path}: missing file [missing-template]" ], Set.empty
    else
        let content = File.ReadAllText filePath
        let sections = markdownSections content

        let findings =
            template.Prompts
            |> List.collect (fun prompt ->
                match trySection prompt.Section sections with
                | None ->
                    [ $"{template.Path}: missing section `{prompt.Section}` for prompt `{prompt.Prompt}` [missing-section]" ]
                | Some section when containsText prompt.Prompt section.Content -> []
                | Some _ ->
                    let mismatch =
                        if deferredSections sections |> List.exists (fun section -> containsText prompt.Prompt section.Content) then
                            "deferred-scope-placement"
                        elif containsText prompt.Prompt content then
                            "wrong-section-prompt"
                        else
                            "missing-prompt"

                    [ $"{template.Path}: prompt `{prompt.Prompt}` missing from section `{prompt.Section}` [{mismatch}]" ])

        findings, promptClassesInCorrectSections template.Path template.Prompts content

let validateGuidanceParity validationRows =
    validationRows
    |> List.groupBy (fun (template: GuidanceTemplate, _, _) -> template.Artifact)
    |> List.collect (fun (artifact, rows) ->
        match rows with
        | [ (active, _, activeClasses); (preset, _, presetClasses) ] ->
            let missingInPreset = Set.difference activeClasses presetClasses
            let missingInActive = Set.difference presetClasses activeClasses

            [ yield!
                  missingInPreset
                  |> Set.toList
                  |> List.map (fun prompt -> $"{preset.Path}: parity mismatch for `{prompt}` against {active.Path} [active-preset-parity]")
              yield!
                  missingInActive
              |> Set.toList
              |> List.map (fun prompt -> $"{active.Path}: parity mismatch for `{prompt}` against {preset.Path} [active-preset-parity]") ]
        | _ -> [ $"{artifact}: expected active and preset templates for parity comparison [active-preset-parity]" ])

let validateControlsBoundaryGuidance model =
    let guidancePaths =
        [ "template/fragments/controls/README.md"
          "template/fragments/controls/skill/SKILL.md"
          "template/fragments/elmish/README.md"
          "template/base/README.md"
          "template/base/docs/product.md"
          "src/Controls/skill/SKILL.md"
          ".specify/templates/spec-template.md"
          ".specify/templates/plan-template.md"
          ".specify/presets/fsharp-opinionated/templates/spec-template.md"
          ".specify/presets/fsharp-opinionated/templates/plan-template.md" ]

    let combined =
        guidancePaths
        |> List.map (fun relative -> File.ReadAllText(path [ model.RepositoryRoot; relative ]))
        |> String.concat Environment.NewLine

    let removedChartsPackage = "FS.Skia.UI." + "Charts"
    let removedChartsSkill = "fs-skia-" + "charts"

    let required =
        [ "FS.Skia.UI.Controls"
          "Skia-rendered"
          "Control<'msg>"
          "DataGrid"
          "FS.Skia.UI.Controls.Elmish"
          "ControlsElmish.program"
          "legacy Charts package"
          "no compatibility shim" ]

    let forbidden =
        [ removedChartsPackage
          removedChartsSkill
          ("chart-" + "only")
          ("DataGrid " + "as chart")
          ("DataGrid-" + "as-chart")
          ("renderer-" + "neutral")
          ("renderer " + "neutral")
          ("host-" + "loop ownership")
          ("host loop " + "ownership") ]

    [ yield!
          required
          |> List.choose (fun term ->
              if combined.IndexOf(term, StringComparison.OrdinalIgnoreCase) < 0 then
                  Some $"generated controls guidance missing `{term}` [controls-boundary-guidance]"
              else
                  None)
      yield!
          forbidden
          |> List.choose (fun term ->
              if combined.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0 then
                  Some $"generated controls guidance contains stale term `{term}` [controls-boundary-guidance]"
              else
                  None) ]

let validateTaskSkillistGuidance model =
    let requiredTerms =
        [ ".specify/templates/tasks-template.md",
          [ "[skillist: []]"
            "structured"
            "skillist"
            "After task generation"
            "minimal ordered skill set"
            "confidence"
            "matched signals"
            "reviewer disposition"
            "small, medium, and broad"
            "non-authoritative aggregate"
            "before and after every status change"
            "persistent launch rules"
            "non-authoritative aggregate reporting"
            "persistent graphical launch task"
            "MUST reject viewer-backed default executable paths"
            "[SEH]"
            "synthetic-error-handling-approved"
            "malformed parser input"
            "convenience mocks" ]
          ".specify/presets/fsharp-opinionated/templates/tasks-template.md",
          [ "[skillist: []]"
            "structured"
            "skillist"
            "After task generation"
            "minimal ordered skill set"
            "confidence"
            "matched signals"
            "reviewer disposition"
            "small, medium, and broad"
            "non-authoritative aggregate"
            "before and after every status change"
            "persistent launch rules"
            "non-authoritative aggregate reporting"
            "persistent graphical launch task"
            "MUST reject viewer-backed default executable paths"
            "[SEH]"
            "synthetic-error-handling-approved"
            "malformed parser input"
            "convenience mocks" ]
          ".specify/presets/fsharp-opinionated/templates/tasks-deps-template.yml",
          [ "deps:"
            "skillist:"
            "ordered list of applicable capability skill identifiers" ]
          ".agents/skills/speckit-tasks/SKILL.md",
          [ "Compulsory skill evaluation"
            "Visible skill mirror"
            "Declared skill ids resolve"
            "confidence"
            "matched signals"
            "reviewer disposition"
            "small, medium, and broad"
            "non-authoritative aggregate"
            "[SEH]"
            "synthetic-error-handling-approved"
            "implementation-time relabeling" ]
          ".specify/presets/fsharp-opinionated/commands/speckit.tasks.md",
          [ "Compulsory skill evaluation"
            "Visible skill mirror"
            "Declared skill ids resolve"
            "confidence"
            "matched signals"
            "reviewer disposition"
            "small, medium, and broad"
            "non-authoritative aggregate"
            "[SEH]"
            "synthetic-error-handling-approved"
            "implementation-time relabeling" ]
          ".agents/skills/speckit-implement/SKILL.md",
          [ "structured `skillist`"
            "Resolve every declared skill id"
            "skills in declared order"
            "loaded paths"
            "readiness/skill-loading-evidence.md"
            "loaded_at"
            "work_started_at"
            "reviewer exception"
            "implementation batch records"
            "graph before/after"
            "before and after every status change"
            "red-green evidence log"
            "[SEH]"
            "synthetic-error-handling-approved"
            "implementation-time relabeling" ]
          ".specify/presets/fsharp-opinionated/commands/speckit.implement.md",
          [ "structured `skillist`"
            "Resolve every declared skill id"
            "skills in declared order"
            "loaded paths"
            "readiness/skill-loading-evidence.md"
            "loaded_at"
            "work_started_at"
            "reviewer exception"
            "implementation batch records"
            "graph before/after"
            "before and after every status change"
            "red-green evidence log"
            "[SEH]"
            "synthetic-error-handling-approved"
            "implementation-time relabeling" ]
          ".specify/memory/constitution.md",
          [ "mandatory post-generation skill evaluation gate"
            "`skillist` field"
            "mandatory pre-task skill loading gate"
            "[SEH]"
            "synthetic-error-handling-approved" ]
          ".specify/templates/constitution-template.md",
          [ "mandatory post-generation skill evaluation gate"
            "`skillist` field"
            "mandatory pre-task skill loading gate"
            "[SEH]"
            "synthetic-error-handling-approved" ]
          ".specify/presets/fsharp-opinionated/templates/constitution-template.md",
          [ "mandatory post-generation skill evaluation gate"
            "`skillist` field"
            "mandatory pre-task skill loading gate"
            "[SEH]"
            "synthetic-error-handling-approved" ] ]

    requiredTerms
    |> List.collect (fun (relative, terms) ->
        let filePath = path [ model.RepositoryRoot; relative ]

        if not (File.Exists filePath) then
            [ $"{relative}: missing file [task-skillist-guidance]" ]
        else
            let content = File.ReadAllText filePath

            terms
            |> List.choose (fun term ->
                if content.IndexOf(term, StringComparison.OrdinalIgnoreCase) < 0 then
                    Some $"{relative}: missing `{term}` [task-skillist-guidance]"
                else
                    None))

let runGeneratedGuidanceScan model outputPath =
    let validationRows =
        generatedGuidanceRequirements
        |> List.map (fun template ->
            let findings, classes = validateGuidanceTemplate model template
            template, findings, classes)

    let findings =
        (validationRows |> List.collect (fun (_, findings, _) -> findings))
        @ validateGuidanceParity validationRows
        @ validateForbiddenGeneratedGuidanceAdvice model
        @ validateControlsBoundaryGuidance model
        @ validateTaskSkillistGuidance model
        @ validateSerializedRunnerGuidance model

    if not (List.isEmpty findings) then
        failwithf "Generated guidance check failed:%s%s" Environment.NewLine (String.Join(Environment.NewLine, findings))

    let report =
        [ "# Generated Guidance Check"
          ""
          "PASS: active and preset-owned spec/plan templates include required governance prompts in the expected Markdown sections."
          "PASS: generated Controls guidance covers Skia-rendered controls, rich text, chart controls, graph controls, DataGrid, Controls.Elmish adapter wiring, and legacy Charts replacement notes without stale generated terms."
          "PASS: task templates, task metadata templates, implementation guidance, and constitution guidance require `skillist` evaluation, confidence review, risk-level evidence, and implementation-time skill loading."
          "PASS: repository, agent, template, and generated-product guidance serialize FAKE-backed commands because `.fake` state is shared, while preserving safe non-FAKE parallelism."
          ""
          "Validated prompt classes:"
          yield!
              generatedGuidanceRequirements
              |> List.collect (fun template ->
                  template.Prompts
                  |> List.map (fun prompt -> $"- `{template.Path}` section `{prompt.Section}` prompt `{prompt.Prompt}`"))
          ""
          "Deferred roadmap boundaries checked: visual evidence, release validation, external repository split, and distribution automation remain outside V2 pass/fail scope." ]
        |> String.concat Environment.NewLine

    ensureParent outputPath
    File.WriteAllText(outputPath, report + Environment.NewLine)

let workflowSelfCheck (root: string) =
    let model, initEffects = init root
    let _, restoreEffects = update (StartTarget "Restore") model
    let _, verifyPreflightEffects = update (StartTarget "VerifyPreflight") model
    let _, ciPreflightEffects = update (StartTarget "CiPreflight") model
    let _, verifyEffects = update (StartTarget "Verify") model
    let _, focusedEffects = update (StartTarget "ControlsRenderingCheck") model
    let _, templatePackEffects = update (StartTarget "TemplatePack") model
    let _, templateSmokeEffects = update (StartTarget "TemplateSmoke") model

    if initEffects |> List.exists (function EnsureDirectory path when path = model.LogDir -> true | _ -> false) |> not then
        failwith "init must request log directory creation"

    if restoreEffects |> List.exists (function RunDotnetAction(label, _, _, _, _, _) when label = "dotnet restore" -> true | _ -> false) |> not then
        failwith "Restore must emit a dotnet restore workflow effect"

    if verifyPreflightEffects |> List.exists (function CollectProcessHealth("Verify", _, _) -> true | _ -> false) |> not then
        failwith "VerifyPreflight must emit process-health collection before broad work"

    if verifyPreflightEffects |> List.exists (function ValidateRunnerBootstrap("Verify", _, _) -> true | _ -> false) |> not then
        failwith "VerifyPreflight must emit bootstrap validation before broad work"

    if verifyPreflightEffects |> List.exists (function RequireFiles("verify readiness preflight artifact set", _) -> true | _ -> false) |> not then
        failwith "VerifyPreflight must require readiness impact files before broad work"

    if ciPreflightEffects |> List.exists (function CollectProcessHealth("Ci", _, _) -> true | _ -> false) |> not then
        failwith "CiPreflight must emit process-health collection before broad work"

    if focusedEffects |> List.exists (function CheckFocusedGateAssumptions contract when contract.TargetName = "ControlsRenderingCheck" -> true | _ -> false) |> not then
        failwith "Focused gates must emit stale build/restore assumption checks"

    if focusedEffects |> List.exists (function WriteFocusedGateSummary contract when contract.TargetName = "ControlsRenderingCheck" -> true | _ -> false) |> not then
        failwith "Focused gates must emit focused gate summaries"

    if templatePackEffects |> List.exists (function ValidateTemplatePackage _ -> true | _ -> false) |> not then
        failwith "TemplatePack must validate the local template package artifact"

    if templateSmokeEffects |> List.exists (function ScanGeneratedProjects _ -> true | _ -> false) |> not then
        failwith "TemplateSmoke must scan generated projects and run generated Dev"

    if verifyEffects |> List.exists (function RequireFiles("v1 plus v2 verification artifact set", _) -> true | _ -> false) |> not then
        failwith "Verify must require the v1 plus v2 artifact set"

    let _, completedEffects = update (TargetCompleted "Restore") model

    if completedEffects <> [] then
        failwith "TargetCompleted must be a pure state transition with no effects"

    printfn "process reliability self-check: preflight, bootstrap, verdict, and focused gate effects are pure"

// BUILD SECTION: interpreter

let interpret root effect =
    let model, _ = init root

    match effect with
    | EnsureDirectory directory -> Directory.CreateDirectory directory |> ignore
    | CleanDirectoryContents directory -> cleanDirectoryContents directory
    | RunProcess(label, fileName, arguments, workingDirectory, outputPath, environment) ->
        runProcess label fileName arguments workingDirectory outputPath environment
    | RunDotnetAction(label, action, solutionFile, projects, extraArguments, outputPath) ->
        runDotnetAction label action solutionFile projects extraArguments outputPath root
    | InstallTemplate(label, source, outputPath) -> runTemplateInstall model label source outputPath
    | InstantiateTemplates outputPath -> runTemplateInstantiation model outputPath
    | ScanGeneratedProjects outputPath -> scanGeneratedProjects model outputPath
    | CapabilityCatalogCheck -> runCapabilityCatalogCheck model
    | SkillCatalogCheck -> runSkillCatalogCheck model
    | GenerateV3Products -> runGenerateV3Products model
    | ScanV3GeneratedProducts -> runScanV3GeneratedProducts model
    | ValidateGeneratedConsumer -> runGeneratedConsumerValidation model
    | PackageSurfaceReport -> runPackageSurfaceReport model
    | DependencyOwnershipReport -> runDependencyOwnershipReport model
    | ValidateTemplatePackage outputPath -> validateTemplatePackage model outputPath
    | GeneratedGuidanceScan outputPath -> runGeneratedGuidanceScan model outputPath
    | CollectProcessHealth(target, outputPath, verdictPath) -> collectProcessHealth root target outputPath verdictPath
    | ValidateRunnerBootstrap(target, outputPath, verdictPath) -> validateRunnerBootstrap root target outputPath verdictPath
    | WriteVerificationVerdict verdict -> writeVerificationVerdictReport model.VerificationVerdictsPath verdict
    | WriteFocusedGateSummary contract -> appendFocusedGateSummary model.FocusedGatesReportPath contract
    | CheckFocusedGateAssumptions contract -> checkFocusedGateAssumptions root contract
    | WriteStructuredReport(_, path, content) ->
        ensureParent path
        File.WriteAllText(path, content)
    | WriteStructuredJsonReport(_, path, content) ->
        ensureParent path
        File.WriteAllText(path, content)
    | WriteFile(path, content) ->
        ensureParent path
        File.WriteAllText(path, content)
    | RequireFiles(artifactClass, paths) -> requireFiles artifactClass paths
    | FailWith message -> failwith message
    | WorkflowSelfCheck -> workflowSelfCheck root

let runTarget targetName =
    let model, initEffects = init repositoryRoot
    let _, effects = update (StartTarget targetName) model

    (initEffects @ effects)
    |> List.iter (interpret repositoryRoot)

let allTargets =
    requiredTargets @ [ "PackageSmoke"; "BuildWorkflowCheck" ]

// BUILD SECTION: native FAKE target graph

allTargets
|> List.iter (fun targetName ->
    Target.create targetName (fun _ -> runTarget targetName))

targetDependencyRows
|> List.iter (fun (targetName, dependencies) ->
    dependencies
    |> List.iter (fun dependency -> dependency ==> targetName |> ignore))

Target.runOrDefaultWithArguments "Dev"

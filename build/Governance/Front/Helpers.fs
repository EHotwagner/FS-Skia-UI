module FS.Skia.UI.Build.Front.Helpers

open System
open System.IO
open System.Text.RegularExpressions
open BuildPaths
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Preflight
open FS.Skia.UI.Build.Front.Support
open FS.Skia.UI.Build.Engine.Model

// Relocated verbatim from build.fsx (feature 045): the update-supporting helpers
// (project lists, focused-gate contract, target metadata, template/v3 rows).

let defaultTestProjects =
    [ "tests/Lib.Tests/Lib.Tests.fsproj"
      "tests/Scene.Tests/Scene.Tests.fsproj"
      // Feature 083: the FS.Skia.UI.Color contrast/palette suite (reference pairs, verdicts, ramps).
      "tests/Color.Tests/Color.Tests.fsproj"
      "tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj"
      "tests/Elmish.Tests/Elmish.Tests.fsproj"
      "tests/KeyboardInput.Tests/KeyboardInput.Tests.fsproj"
      "tests/Layout.Tests/Layout.Tests.fsproj"
      "tests/Controls.Tests/Controls.Tests.fsproj"
      "tests/Testing.Tests/Testing.Tests.fsproj"
      "tests/Parity.Tests/Parity.Tests.fsproj"
      "tests/Smoke.Tests/Smoke.Tests.fsproj"
      "tests/SkillSupport.Tests/SkillSupport.Tests.fsproj"
      "tests/Governance.Tests/Governance.Tests.fsproj" ]

let packProjects =
    [ "src/Scene/Scene.fsproj", "FS.Skia.UI.Scene"
      // Feature 083: the WCAG contrast + accessible-palette package (depends only on Scene).
      "src/Color/Color.fsproj", "FS.Skia.UI.Color"
      "src/SkiaViewer/SkiaViewer.fsproj", "FS.Skia.UI.SkiaViewer"
      "src/Elmish/Elmish.fsproj", "FS.Skia.UI.Elmish"
      "src/KeyboardInput/KeyboardInput.fsproj", "FS.Skia.UI.KeyboardInput"
      "src/Input/Input.fsproj", "FS.Skia.UI.Input"
      "src/Controls.Elmish/Controls.Elmish.fsproj", "FS.Skia.UI.Controls.Elmish"
      "src/Testing/Testing.fsproj", "FS.Skia.UI.Testing"
      "src/Layout/Layout.fsproj", "FS.Skia.UI.Layout"
      "src/Controls/Controls.fsproj", "FS.Skia.UI.Controls"
      // 043: the published governance engine; generated consumers reference it
      // in-process instead of copying the Python + run-audit.sh scripts.
      "build/Governance/FS.Skia.UI.Build.fsproj", "FS.Skia.UI.Build"
      // 058 (US2): the shipped skill-support library backing the fsharp-* skills.
      "src/SkillSupport/SkillSupport.fsproj", "FS.Skia.UI.SkillSupport" ]

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

// Feature 041 (FR-001): the runnable-target registry and the dependency rows are now
// DERIVED from the typed Targets.Target DU + total Targets.spec — no longer maintained
// as parallel string lists. A renamed/mistyped target is a compile error in Targets.fs,
// making TargetMetadataDrift's "second source of truth" structurally impossible (SC-003).
let requiredTargets = Targets.requiredTargetNames

let targetDependencyRows = Targets.targetDependencyRows

let processEffect label fileName arguments workingDirectory outputPath =
    RunProcess(label, fileName, arguments, workingDirectory, outputPath, Map.empty)


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
  diagnostic: The FAKE Test target runs the native-GUI Expecto suites (Smoke.Tests and SkiaViewer.Tests) via direct Expecto execution to bypass the VSTest/YoloDev adapter testhost (libdecor-gtk crash under a dual Wayland/X11 display); all other test projects continue to use dotnet test.
"""

// Feature 088 (US1, FR-001/FR-002/FR-005): the focused-gate contract is keyed by the typed
// Targets.Target (was a `string` with a `_ -> degraded` wildcard). The match is exhaustive
// and wildcard-free, so adding a future Target case without classifying it is a COMPILE
// ERROR (SC-001) rather than a silent verification-degraded fall-through. Every routable gate
// resolves to a non-degraded contract (SC-003); true non-routable/internal targets resolve
// through `internalTargetContract`, which reproduces the exact former wildcard value so their
// target-metadata stays byte-identical (FR-002).
let focusedGateContract model (target: Targets.Target) =
    let log name = path [ model.LogDir; name ]
    let readiness name = Some(path [ model.ReadinessDir; name ])
    let noRestoreControls =
        [ "requires-restored-project:tests/Controls.Tests/Controls.Tests.fsproj"
          "requires-built-project:tests/Controls.Tests/Controls.Tests.fsproj" ]

    let nm = Targets.name target

    // The exact former wildcard value (`VerificationDegraded`, no readiness path) reproduced
    // verbatim for non-routable/internal targets — preserves target-metadata byte-identity.
    let internalTargetContract () =
        { TargetName = nm
          DirectPrerequisites = []
          Command = $"./fake.sh build -t {nm}"
          LogPath = log $"{nm}.txt"
          ReadinessPath = None
          StaleAssumptions = []
          VerdictCategory = VerificationDegraded }

    // A routable gate that previously fell through the wildcard now resolves to a non-degraded
    // (authoritative) contract of the same shape — the SC-003 fix, minimal and explicit.
    let routableTargetContract () =
        { internalTargetContract () with VerdictCategory = VerificationSuccess }

    match target with
    | Targets.PackageSurfaceCheck ->
        { TargetName = nm
          DirectPrerequisites = [ "Build" ]
          Command = "./fake.sh build -t PackageSurfaceCheck"
          LogPath = log "package-surface-check.txt"
          ReadinessPath = Some(path [ model.PackageSurfaceReportDir; "index.md" ])
          StaleAssumptions =
              [ "requires-restored-project:tests/Package.Tests/Package.Tests.fsproj"
                "requires-built-project:tests/Package.Tests/Package.Tests.fsproj" ]
          VerdictCategory = VerificationSuccess }
    | Targets.FsiTranscripts ->
        { TargetName = nm
          DirectPrerequisites = [ "Build" ]
          Command = "./fake.sh build -t FsiTranscripts"
          LogPath = path [ model.FsiDir; "prelude.txt" ]
          ReadinessPath = Some model.FsiDir
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | Targets.ControlsCatalogCheck ->
        { TargetName = nm
          DirectPrerequisites = []
          Command = "./fake.sh build -t ControlsCatalogCheck"
          LogPath = log "controls-catalog-check.txt"
          ReadinessPath = readiness "control-catalog.md"
          StaleAssumptions = noRestoreControls
          VerdictCategory = VerificationSuccess }
    | Targets.ControlsCatalogGenerationCheck ->
        // Feature 066: pure text-comparison currency gate over committed files — no project
        // build prerequisite (so no requires-restored/built assumptions, unlike the sibling
        // Controls test gates).
        { TargetName = nm
          DirectPrerequisites = []
          Command = "./fake.sh build -t ControlsCatalogGenerationCheck"
          LogPath = log "controls-catalog-generation-check.txt"
          ReadinessPath = readiness "control-catalog-generation.md"
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | Targets.ControlsInteractionCheck ->
        { TargetName = nm
          DirectPrerequisites = []
          Command = "./fake.sh build -t ControlsInteractionCheck"
          LogPath = log "controls-interaction-check.txt"
          ReadinessPath = readiness "interaction-tests.md"
          StaleAssumptions = noRestoreControls
          VerdictCategory = VerificationSuccess }
    | Targets.ControlsRenderingCheck ->
        { TargetName = nm
          DirectPrerequisites = []
          Command = "./fake.sh build -t ControlsRenderingCheck"
          LogPath = log "controls-rendering-check.txt"
          ReadinessPath = readiness "layout-rendering.md"
          StaleAssumptions = noRestoreControls
          VerdictCategory = VerificationSuccess }
    | Targets.DependencyReport ->
        { TargetName = nm
          DirectPrerequisites = []
          Command = "./fake.sh build -t DependencyReport"
          LogPath = log "dependency-report.txt"
          ReadinessPath = Some model.DependencyReportPath
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | Targets.SymbolCrossCheck ->
        { TargetName = nm
          DirectPrerequisites = []
          Command = "./fake.sh build -t SymbolCrossCheck"
          LogPath = log "symbol-cross-check.txt"
          ReadinessPath = readiness "symbol-cross-check.md"
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | Targets.TemplateCheck ->
        { TargetName = nm
          DirectPrerequisites = [ "TemplatePack"; "TemplateInstallSource"; "TemplateInstallPackage"; "TemplateInstantiate"; "TemplateSmoke" ]
          Command = "./fake.sh build -t TemplateCheck"
          LogPath = path [ model.TemplateEvidenceDir; "verdict.md" ]
          ReadinessPath = Some(path [ model.TemplateEvidenceDir; "verdict.md" ])
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | Targets.GeneratedProductCheck ->
        { TargetName = nm
          DirectPrerequisites = [ "CapabilityCheck"; "SkillCheck" ]
          Command = "./fake.sh build -t GeneratedProductCheck"
          LogPath = path [ model.GeneratedFileListsDir; "summary.md" ]
          ReadinessPath = Some(path [ model.GeneratedFileListsDir; "summary.md" ])
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | Targets.GeneratedGuidanceCheck ->
        { TargetName = nm
          DirectPrerequisites = []
          Command = "./fake.sh build -t GeneratedGuidanceCheck"
          LogPath = model.GeneratedGuidanceReportPath
          ReadinessPath = Some model.GeneratedGuidanceReportPath
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | Targets.TemplateDrift ->
        { TargetName = nm
          DirectPrerequisites = []
          Command = "./fake.sh build -t TemplateDrift"
          LogPath = log "template-drift.txt"
          ReadinessPath = Some model.TemplateDriftReportPath
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | Targets.SkillSyncCheck ->
        { TargetName = nm
          DirectPrerequisites = []
          Command = "./fake.sh build -t SkillSyncCheck"
          LogPath = log "skill-sync-check.txt"
          ReadinessPath = readiness "skill-sync-check.md"
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | Targets.EvidenceGraph ->
        { TargetName = nm
          DirectPrerequisites = []
          Command = "./fake.sh build -t EvidenceGraph"
          LogPath = log "evidence-graph.txt"
          ReadinessPath = Some(path [ model.ReadinessDir; "task-graph.md" ])
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    | Targets.EvidenceAudit ->
        { TargetName = nm
          DirectPrerequisites = [ "EvidenceGraph" ]
          Command = "./fake.sh build -t EvidenceAudit"
          LogPath = log "evidence-audit.txt"
          ReadinessPath = Some model.EvidenceAuditReportPath
          StaleAssumptions = []
          VerdictCategory = VerificationSuccess }
    // Routable gates that previously fell through the wildcard — now explicit, non-degraded
    // (SC-003). Same contract shape as the former wildcard, but authoritative. Includes the
    // two new GeneratedProductCheck split sub-targets (Feature 088, US2).
    | Targets.Dev
    | Targets.PerPackageSurfaceDiff
    | Targets.DesignTokenDrift
    | Targets.ContrastCheck
    | Targets.ControlsCatalogDocsCheck
    | Targets.ControlsDocCoverageCheck
    | Targets.ControlFidelityCheck
    | Targets.SkillQualityCheck
    | Targets.PhaseHookParityCheck
    | Targets.SkillContractPathCheck
    | Targets.TemplateUpdateSkillPackageCheck
    | Targets.AgentReady
    | Targets.TargetMetadataDrift
    | Targets.Verify
    | Targets.Ci
    | Targets.PrePublishCheck
    | Targets.Publish
    | Targets.GeneratedProductStructure
    | Targets.GeneratedConsumerValidation -> routableTargetContract ()
    // Non-routable / internal targets — reproduce the exact former wildcard value verbatim
    // (VerificationDegraded, no readiness) so target-metadata is byte-identical (FR-002).
    | Targets.Clean
    | Targets.Restore
    | Targets.Build
    | Targets.Test
    | Targets.PackLocal
    | Targets.RefreshSurfaceBaselines
    | Targets.SampleContractSmoke
    | Targets.TemplatePack
    | Targets.TemplateInstallSource
    | Targets.TemplateInstallPackage
    | Targets.TemplateInstantiate
    | Targets.TemplateSmoke
    | Targets.CapabilityCheck
    | Targets.SkillCheck
    | Targets.TargetMetadata
    | Targets.VerifyPreflight
    | Targets.CiPreflight
    | Targets.StaleBoundaryScan
    | Targets.FinalReadiness
    | Targets.Route
    | Targets.PackageSmoke
    | Targets.BuildWorkflowCheck -> internalTargetContract ()

let focusedGateSummary model (target: Targets.Target) =
    focusedGateContract model target |> WriteFocusedGateSummary

let focusedGateAssumptionCheck model (target: Targets.Target) =
    focusedGateContract model target |> CheckFocusedGateAssumptions

// Feature 041 (FR-002): TargetMetadata is computed from the typed Targets.spec
// (TimeoutClass/Cost/FailureOwner/DirectPrerequisites are single-sourced there) plus
// the edge-resolved focused-gate contract (paths/command/verdict stay at the build
// interpreter edge, Principle IV). The record type itself is owned by the library.
let targetMetadata model (target: Targets.Target) : TargetMetadata.TargetMetadata =
    let spec = Targets.spec target
    let contract = focusedGateContract model spec.Target

    { RunnableTargetName = spec.Name
      DirectPrerequisites = spec.DirectPrerequisites |> List.map Targets.name
      ExpectedOutputs =
          [ contract.LogPath
            yield! contract.ReadinessPath |> Option.toList ]
      StaleAssumptions = contract.StaleAssumptions
      TimeoutClass = spec.TimeoutClass
      Cost = spec.Cost
      Authority =
          match contract.VerdictCategory with
          | VerificationSuccess -> "authoritative"
          | VerificationDegraded -> "degraded"
          | VerificationProductFailure -> "product-failure"
          | VerificationEnvironmentFailure -> "environment-failure"
      FailureOwner = spec.FailureOwner
      Command = contract.Command }

let allTargetMetadata model =
    Targets.allTargets |> List.map (targetMetadata model)

// Feature 044 (US3, data-model §3): which generated principle fragments each template
// carries. The mapping is the per-template expected set the currency check enforces and
// the regeneration splices (the locked Phase-1 inventory). Forward-slash relative paths.
let constitutionRelPath = ".specify/memory/constitution.md"

let constitutionTemplateRegions =
    [ ".specify/templates/plan-template.md", [ "fsi-visibility" ]
      ".specify/templates/tasks-template.md", [ "tests-first"; "mvu-boundary"; "synthetic-disclosure" ] ]

let repoRelPath root (relForwardSlash: string) =
    path (root :: (relForwardSlash.Split('/') |> List.ofArray))

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

// Edge wrapper: the file reads (validation.contract.yml + docs scans) stay here; the
// pure drift logic is the library's TargetMetadata.validateAgainstRepo (FR-002).
let validateTargetMetadataAgainstRepo root runnableTargets metadata =
    TargetMetadata.validateAgainstRepo
        (validationContractTargetReferences root)
        (documentedTargetReferences root)
        runnableTargets
        metadata

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


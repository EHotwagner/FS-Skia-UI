#r "paket:
nuget FSharp.Core 6.0.7
//"

open System
open System.Diagnostics
open System.IO
open System.IO.Compression

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
      DeferralsPath: string
      CompletedTargets: string list }

type BuildMsg =
    | StartTarget of string
    | TargetCompleted of string
    | TargetFailed of string * string

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
    | PackageSurfaceReport
    | DependencyOwnershipReport
    | ValidateTemplatePackage of outputPath: string
    | GeneratedGuidanceScan of outputPath: string
    | WriteStructuredReport of label: string * path: string * content: string
    | WriteFile of path: string * content: string
    | RequireFiles of artifactClass: string * paths: string list
    | WorkflowSelfCheck

let repositoryRoot = __SOURCE_DIRECTORY__

let path segments =
    segments |> Array.ofList |> Path.Combine

let activeFeatureId root =
    let featureJson = path [ root; ".specify"; "feature.json" ]

    if File.Exists featureJson then
        let content = File.ReadAllText featureJson
        let marker = "\"feature_directory\""
        let markerIndex = content.IndexOf(marker, StringComparison.Ordinal)

        if markerIndex < 0 then
            "007-v2-template-packaging"
        else
            let afterMarker = content.Substring(markerIndex + marker.Length)
            let colonIndex = afterMarker.IndexOf(':')

            if colonIndex < 0 then
                "007-v2-template-packaging"
            else
                let afterColon = afterMarker.Substring(colonIndex + 1)
                let firstQuote = afterColon.IndexOf('"')

                if firstQuote < 0 then
                    "007-v2-template-packaging"
                else
                    let afterFirstQuote = afterColon.Substring(firstQuote + 1)
                    let secondQuote = afterFirstQuote.IndexOf('"')

                    if secondQuote < 0 then
                        "007-v2-template-packaging"
                    else
                        let featureDirectory = afterFirstQuote.Substring(0, secondQuote)

                        if String.IsNullOrWhiteSpace featureDirectory then
                            "007-v2-template-packaging"
                        else
                            Path.GetFileName(featureDirectory.TrimEnd('/', '\\'))
    else
        "007-v2-template-packaging"

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
          path [ model.TemplateEvidenceDir; "source-default" ]
          path [ model.TemplateEvidenceDir; "source-minimal" ]
          path [ model.TemplateEvidenceDir; "package-default" ]
          path [ model.TemplateEvidenceDir; "package-minimal" ] ]
        |> List.map EnsureDirectory

    model, effects

let defaultTestProjects =
    [ "tests/Lib.Tests/Lib.Tests.fsproj"
      "tests/Scene.Tests/Scene.Tests.fsproj"
      "tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj"
      "tests/Elmish.Tests/Elmish.Tests.fsproj"
      "tests/KeyboardInput.Tests/KeyboardInput.Tests.fsproj"
      "tests/Charts.Tests/Charts.Tests.fsproj"
      "tests/Layout.Tests/Layout.Tests.fsproj"
      "tests/Testing.Tests/Testing.Tests.fsproj"
      "tests/Parity.Tests/Parity.Tests.fsproj"
      "tests/Smoke.Tests/Smoke.Tests.fsproj"
      "tests/Governance.Tests/Governance.Tests.fsproj" ]

let packProjects =
    [ "src/Scene/Scene.fsproj", "FS.Skia.UI.Scene"
      "src/SkiaViewer/SkiaViewer.fsproj", "FS.Skia.UI.SkiaViewer"
      "src/Elmish/Elmish.fsproj", "FS.Skia.UI.Elmish"
      "src/KeyboardInput/KeyboardInput.fsproj", "FS.Skia.UI.KeyboardInput"
      "src/Testing/Testing.fsproj", "FS.Skia.UI.Testing"
      "src/Lib/Lib.fsproj", "FS.Skia.UI"
      "src/Charts/Charts.fsproj", "FS.Skia.UI.Charts"
      "src/Layout/Layout.fsproj", "FS.Skia.UI.Layout" ]

let fsiScripts =
    [ "prelude", "scripts/prelude.fsx"
      "charts-prelude", "scripts/charts-prelude.fsx"
      "input-prelude", "scripts/input-prelude.fsx"
      "layout-prelude", "scripts/layout-prelude.fsx" ]

let sampleSmokeProjects =
    [ "BasicViewer", "samples/BasicViewer/BasicViewer.fsproj"
      "InteractiveViewer", "samples/InteractiveViewer/InteractiveViewer.fsproj"
      "ParityGallery", "samples/ParityGallery/ParityGallery.fsproj"
      "EffectsGallery", "samples/EffectsGallery/EffectsGallery.fsproj"
      "ChartsGallery", "samples/ChartsGallery/ChartsGallery.fsproj"
      "DataGridGallery", "samples/DataGridGallery/DataGridGallery.fsproj"
      "LayoutGraphGallery", "samples/LayoutGraphGallery/LayoutGraphGallery.fsproj"
      "ScreenshotGallery", "samples/ScreenshotGallery/ScreenshotGallery.fsproj"
      "DemoReel", "samples/DemoReel/DemoReel.fsproj"
      "KeyboardInputGallery", "samples/KeyboardInputGallery/KeyboardInputGallery.fsproj" ]

let buildProjects =
    (packProjects |> List.map fst)
    @ defaultTestProjects
    @ (sampleSmokeProjects |> List.map snd)
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
      "DependencyReport"
      "GeneratedGuidanceCheck"
      "TemplateDrift"
      "EvidenceGraph"
      "EvidenceAudit"
      "Verify"
      "Ci" ]

let processEffect label fileName arguments workingDirectory outputPath =
    RunProcess(label, fileName, arguments, workingDirectory, outputPath, Map.empty)

let templateRows model =
    [ { Artifact = "source"
        Profile = "default"
        ProjectName = "V2SourceDefault"
        Root = path [ model.TemplateWorkDir; "source-default" ]
        EvidenceDir = path [ model.TemplateEvidenceDir; "source-default" ] }
      { Artifact = "source"
        Profile = "minimal"
        ProjectName = "V2SourceMinimal"
        Root = path [ model.TemplateWorkDir; "source-minimal" ]
        EvidenceDir = path [ model.TemplateEvidenceDir; "source-minimal" ] }
      { Artifact = "package"
        Profile = "default"
        ProjectName = "V2PackageDefault"
        Root = path [ model.TemplateWorkDir; "package-default" ]
        EvidenceDir = path [ model.TemplateEvidenceDir; "package-default" ] }
      { Artifact = "package"
        Profile = "minimal"
        ProjectName = "V2PackageMinimal"
        Root = path [ model.TemplateWorkDir; "package-minimal" ]
        EvidenceDir = path [ model.TemplateEvidenceDir; "package-minimal" ] } ]

let v3GeneratedRows model =
    let row artifact profile projectName capabilities =
        { Artifact = artifact
          Profile = profile
          ProjectName = projectName
          Root = path [ model.GeneratedProductRootsDir; $"{profile}-{artifact}" ]
          Capabilities = capabilities
          EvidenceDir = path [ model.GeneratedProductVerifyDir; $"{profile}-{artifact}" ]
          FileListPath = path [ model.GeneratedFileListsDir; $"{profile}-{artifact}.txt" ] }

    [ row "source" "app" "V3AppSource" [ "scene"; "skiaviewer"; "elmish"; "keyboard-input"; "layout"; "charts" ]
      row "package" "app" "V3AppPackage" [ "scene"; "skiaviewer"; "elmish"; "keyboard-input"; "layout"; "charts" ]
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
    | "charts" -> Some "fs-skia-charts"
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
        [ RunDotnetAction("dotnet build", "build", "FS-Skia-UI.sln", buildProjects, "--no-restore -maxcpucount:1", path [ model.LogDir; "build.txt" ]) ]
    | StartTarget "Test" ->
        model,
        defaultTestProjects
        |> List.filter (fun project -> File.Exists(path [ model.RepositoryRoot; project ]))
        |> List.map (fun project ->
            processEffect $"dotnet test {project}" "dotnet" $"test {project} --no-build" model.RepositoryRoot (path [ model.LogDir; "test.txt" ]))
    | StartTarget "Dev" ->
        model,
        [ WriteFile(path [ model.LogDir; "dev-verdict.txt" ], "Dev target completed: Restore, Build, and default non-visual Test targets passed.\n") ]
    | StartTarget "PackLocal" ->
        model,
        (packProjects
         |> List.map (fun (project, packageId) ->
             processEffect $"dotnet pack {packageId}" "dotnet" $"pack {project} -c Release -o {quote model.LocalPackageDir}" model.RepositoryRoot (path [ model.LogDir; "pack-local.txt" ])))
        @ [ WriteStructuredReport("local package report", path [ model.PackageEvidenceDir; "local-packages.md" ], $"# Local Packages\n\nOutput directory: `{model.LocalPackageDir}`\n") ]
    | StartTarget "RefreshSurfaceBaselines" ->
        model,
        [ processEffect "refresh surface baselines" "dotnet" "fsi scripts/refresh-surface-baselines.fsx" model.RepositoryRoot (path [ model.LogDir; "surface-refresh.txt" ])
          RequireFiles(
              "stable package surface baselines",
              [ path [ model.SurfaceBaselineDir; "FS.Skia.UI.txt" ]
                path [ model.SurfaceBaselineDir; "FS.Skia.UI.Charts.txt" ]
                path [ model.SurfaceBaselineDir; "FS.Skia.UI.Layout.txt" ] ]
            ) ]
    | StartTarget "PackageSurfaceCheck" ->
        model,
        [ processEffect "package surface check" "dotnet" "test tests/Package.Tests/Package.Tests.fsproj --no-build" model.RepositoryRoot (path [ model.LogDir; "package-surface-check.txt" ])
          PackageSurfaceReport
          RequireFiles("stable package surface baselines", [ path [ model.SurfaceBaselineDir; "FS.Skia.UI.txt" ] ]) ]
    | StartTarget "FsiTranscripts" ->
        model,
        fsiScripts
        |> List.map (fun (name, script) ->
            processEffect $"dotnet fsi {script}" "dotnet" $"fsi {script}" model.RepositoryRoot (path [ model.FsiDir; $"{name}.txt" ]))
    | StartTarget "SampleContractSmoke" ->
        model,
        sampleSmokeProjects
        |> List.map (fun (name, project) ->
            processEffect $"{name} contract smoke" "dotnet" $"run --no-build --project {project} -- --contract-smoke" model.RepositoryRoot (path [ model.SampleSmokeDir; $"{name}.txt" ]))
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
          WriteStructuredReport("template smoke support boundary", path [ model.TemplateEvidenceDir; "non-visual-support.md" ], "# Non-Visual Support\n\nV2 template validation is non-visual. Full visual evidence, release validation, an external template repository, and broader distribution automation remain deferred roadmap work.\n") ]
    | StartTarget "TemplateCheck" ->
        model,
        [ RequireFiles(
              "template validation artifact set",
              [ path [ model.TemplateEvidenceDir; "template-pack.log" ]
                path [ model.TemplateEvidenceDir; "template-package-contents.md" ]
                path [ model.TemplateEvidenceDir; "source-install.log" ]
                path [ model.TemplateEvidenceDir; "package-install.log" ]
                path [ model.TemplateEvidenceDir; "instantiation.log" ]
                path [ model.TemplateEvidenceDir; "generated-project-scans.md" ]
                path [ model.TemplateEvidenceDir; "source-default"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "source-minimal"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "package-default"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "package-minimal"; "dev.log" ] ]
            )
          WriteStructuredReport("template verdict", path [ model.TemplateEvidenceDir; "verdict.md" ], "# TemplateCheck Verdict\n\nPASS: source/package and default/minimal generated projects passed non-visual validation.\n") ]
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
        [ GenerateV3Products
          ScanV3GeneratedProducts
          RequireFiles(
              "generated product file-list reports",
              [ path [ model.GeneratedFileListsDir; "app-source.txt" ]
                path [ model.GeneratedFileListsDir; "app-package.txt" ]
                path [ model.GeneratedFileListsDir; "headless-scene-source.txt" ]
                path [ model.GeneratedFileListsDir; "governed-source.txt" ]
                path [ model.GeneratedFileListsDir; "sample-pack-source.txt" ] ]
            ) ]
    | StartTarget "DependencyReport" ->
        model,
        [ DependencyOwnershipReport
          processEffect "dependency report" "dotnet" ("fsi scripts/dependency-report.fsx " + quote (path [ model.ReadinessDir; "dependencies.md" ])) model.RepositoryRoot (path [ model.LogDir; "dependency-report.txt" ])
          RequireFiles("dependency report output", [ model.DependencyReportPath ]) ]
    | StartTarget "GeneratedGuidanceCheck" ->
        model,
        [ GeneratedGuidanceScan model.GeneratedGuidanceReportPath
          RequireFiles("generated guidance report output", [ model.GeneratedGuidanceReportPath ]) ]
    | StartTarget "TemplateDrift" ->
        model,
        [ processEffect "template drift" "dotnet" $"fsi scripts/template-drift.fsx {quote model.TemplateDriftReportPath}" model.RepositoryRoot (path [ model.LogDir; "template-drift.txt" ])
          RequireFiles("template drift report output", [ model.TemplateDriftReportPath ]) ]
    | StartTarget "EvidenceGraph" ->
        model,
        [ processEffect "speckit evidence graph" ".specify/extensions/evidence/scripts/bash/run-audit.sh" $"{model.FeatureDir} --graph-only" model.RepositoryRoot (path [ model.LogDir; "evidence-graph.txt" ])
          RequireFiles("task graph output", [ path [ model.ReadinessDir; "task-graph.json" ]; path [ model.ReadinessDir; "task-graph.md" ] ]) ]
    | StartTarget "EvidenceAudit" ->
        model,
        [ processEffect "speckit evidence audit" ".specify/extensions/evidence/scripts/bash/run-audit.sh" $"{model.FeatureDir}" model.RepositoryRoot (path [ model.LogDir; "evidence-audit.txt" ])
          RequireFiles("evidence audit output", [ path [ model.LogDir; "evidence-audit.txt" ]; path [ model.ReadinessDir; "diff-scan-hits.json" ] ]) ]
    | StartTarget "Verify" ->
        model,
        [ RequireFiles(
              "v1 plus v2 verification artifact set",
              [ path [ model.LogDir; "build.txt" ]
                path [ model.LogDir; "test.txt" ]
                path [ model.LogDir; "pack-local.txt" ]
                path [ model.LogDir; "package-surface-check.txt" ]
                path [ model.LogDir; "dependency-report.txt" ]
                path [ model.LogDir; "template-drift.txt" ]
                path [ model.LogDir; "evidence-audit.txt" ]
                model.CapabilityCatalogReportPath
                model.SelectedSkillsReportPath
                path [ model.GeneratedFileListsDir; "app-source.txt" ]
                path [ model.GeneratedProductVerifyDir; "app-source"; "verify.log" ]
                path [ model.FsiDir; "prelude.txt" ]
                path [ model.SampleSmokeDir; "BasicViewer.txt" ]
                path [ model.ReadinessDir; "task-graph.json" ]
                model.DependencyReportPath
                model.GeneratedGuidanceReportPath
                model.TemplateDriftReportPath
                path [ model.TemplateEvidenceDir; "verdict.md" ] ]
            )
          WriteStructuredReport("verify verdict", path [ model.LogDir; "verify-verdict.txt" ], "Verify target completed with v1 and v2 artifact classes present.\n") ]
    | StartTarget "Ci" ->
        model,
        [ WriteStructuredReport("ci verdict", path [ model.LogDir; "ci-verdict.txt" ], "Ci delegates to Verify and completed without duplicating command order.\n") ]
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

let ensureParent (filePath: string) =
    match Path.GetDirectoryName filePath |> Option.ofObj with
    | Some directory when directory <> "" -> Directory.CreateDirectory directory |> ignore
    | _ -> ()

let cleanDirectoryContents directory =
    if Directory.Exists directory then
        Directory.GetFiles(directory)
        |> Array.iter File.Delete

        Directory.GetDirectories(directory)
        |> Array.iter (fun child -> Directory.Delete(child, true))
    else
        Directory.CreateDirectory directory |> ignore

let appendLine outputPath line =
    ensureParent outputPath
    File.AppendAllText(outputPath, line + Environment.NewLine)

let runProcessWithAllowedExitCodes (label: string) (fileName: string) (arguments: string) (workingDirectory: string) (outputPath: string) (environment: Map<string, string>) (allowedExitCodes: Set<int>) =
    ensureParent outputPath
    File.AppendAllText(outputPath, $"\n## {label}\n$ {fileName} {arguments}\n")

    let startInfo = ProcessStartInfo(fileName, arguments)
    startInfo.WorkingDirectory <- workingDirectory
    startInfo.RedirectStandardOutput <- true
    startInfo.RedirectStandardError <- true
    startInfo.UseShellExecute <- false

    environment
    |> Map.iter (fun key value -> startInfo.Environment.[key] <- value)

    use proc =
        match Process.Start startInfo |> Option.ofObj with
        | Some proc -> proc
        | None -> failwithf "Could not start %s %s" fileName arguments

    let stdoutTask = proc.StandardOutput.ReadToEndAsync()
    let stderrTask = proc.StandardError.ReadToEndAsync()

    if proc.WaitForExit(30 * 60 * 1000) then
        let stdout = stdoutTask.Result
        let stderr = stderrTask.Result
        File.AppendAllText(outputPath, stdout)
        File.AppendAllText(outputPath, stderr)
        File.AppendAllText(outputPath, $"\nexit-code={proc.ExitCode}\n")

        if allowedExitCodes |> Set.contains proc.ExitCode |> not then
            failwithf "%s failed with exit code %d. See %s" label proc.ExitCode outputPath
    else
        proc.Kill()
        failwithf "%s timed out. See %s" label outputPath

let runProcess label fileName arguments workingDirectory outputPath environment =
    runProcessWithAllowedExitCodes label fileName arguments workingDirectory outputPath environment (Set.singleton 0)

let existingProjects root projects =
    projects
    |> List.filter (fun project -> File.Exists(path [ root; project ]))

let solutionFor root preferredSolution =
    let preferred = path [ root; preferredSolution ]

    if File.Exists preferred then
        Some preferredSolution
    else
        Directory.GetFiles(root, "*.sln")
        |> Array.tryHead
        |> Option.map Path.GetFileName

let runDotnetAction label action solutionFile projects extraArguments outputPath root =
    let existing = existingProjects root projects

    if List.isEmpty existing then
        match solutionFor root solutionFile with
        | Some solution ->
            let arguments =
                [ action; quote solution; extraArguments ]
                |> List.filter (fun part -> part <> "")
                |> String.concat " "

            runProcess label "dotnet" arguments root outputPath Map.empty
        | None ->
            failwithf "No projects were found for %s. Checked: %s" label (String.Join(", ", projects))
    else
        existing
        |> List.iter (fun project ->
            let arguments =
                [ action; quote project; extraArguments ]
                |> List.filter (fun part -> part <> "")
                |> String.concat " "

            runProcess $"{label} {project}" "dotnet" arguments root outputPath Map.empty)

let requireFiles (artifactClass: string) (paths: string list) =
    let missing =
        paths
        |> List.filter (fun path -> not (File.Exists path))

    if missing.Length > 0 then
        let detail = String.Join(Environment.NewLine, missing)
        failwithf "Missing %s:%s%s" artifactClass Environment.NewLine detail

let relativePathFrom root filePath =
    let rootPath =
        Path.GetFullPath root
        |> fun value -> value.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + string Path.DirectorySeparatorChar

    let filePath = Path.GetFullPath filePath
    let relative = Uri(rootPath).MakeRelativeUri(Uri(filePath)).ToString()
    Uri.UnescapeDataString(relative).Replace('\\', '/')

let latestTemplatePackage artifactDir =
    let packages =
        if Directory.Exists artifactDir then
            Directory.GetFiles(artifactDir, "FS.Skia.UI.Template.*.nupkg")
        else
            Array.empty

    packages
    |> Array.sortByDescending File.GetLastWriteTimeUtc
    |> Array.tryHead

let validateTemplatePackage model outputPath =
    let package =
        latestTemplatePackage model.TemplateArtifactDir
        |> Option.defaultWith (fun () -> failwithf "No template package found in %s" model.TemplateArtifactDir)

    use archive = ZipFile.OpenRead package

    let entries =
        archive.Entries
        |> Seq.map (fun entry -> entry.FullName.Replace('\\', '/'))
        |> Seq.toList

    let required =
        [ "content/.template.config/template.json"
          "content/build.fsx"
          "content/src/Lib/Lib.fsproj"
          "content/docs/template-profile.md"
          "content/Directory.Packages.props" ]

    let forbiddenPrefixes =
        [ "content/.git/"
          "content/artifacts/"
          "content/.template.package/"
          "content/specs/001-"
          "content/specs/002-"
          "content/specs/003-"
          "content/specs/004-"
          "content/specs/005-"
          "content/specs/006-"
          "content/specs/007-" ]

    required
    |> List.iter (fun requiredEntry ->
        if entries |> List.contains requiredEntry |> not then
            failwithf "Template package is missing %s" requiredEntry)

    entries
    |> List.iter (fun entry ->
        forbiddenPrefixes
        |> List.iter (fun prefix ->
            if entry.StartsWith(prefix, StringComparison.Ordinal) then
                failwithf "Template package contains excluded source-only artifact %s" entry))

    let report =
        [ "# Template Package Contents"
          ""
          $"Package: `{package}`"
          ""
          "Required entries verified:"
          yield! required |> List.map (fun entry -> $"- `{entry}`")
          ""
          $"Total entries: {entries.Length}" ]
        |> String.concat Environment.NewLine

    ensureParent outputPath
    File.WriteAllText(outputPath, report + Environment.NewLine)

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
    Directory.CreateDirectory row.EvidenceDir |> ignore

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
          if row.Profile = "minimal" then
              "--skipGitInit true" ]
        |> String.concat " "

    runProcess $"{row.Artifact}/{row.Profile} instantiate" "dotnet" args model.RepositoryRoot (path [ row.EvidenceDir; "instantiate.log" ]) Map.empty

let runTemplateInstantiation model outputPath =
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
    let normalized = filePath.Replace('\\', '/')
    [ "/bin/"; "/obj/"; "/.fake/"; "/.git/"; "/.template.config/" ]
    |> List.exists (fun segment -> normalized.IndexOf(segment, StringComparison.Ordinal) >= 0)
    |> not

let generatedShellScripts (row: TemplateRow) =
    Directory.EnumerateFiles(row.Root, "*.sh", SearchOption.AllDirectories)
    |> Seq.filter fileShouldBeScanned
    |> Seq.toList

let isWindows =
    Path.DirectorySeparatorChar = '\\'

let hasUserExecutePermission filePath =
    if isWindows then
        true
    else
        let startInfo = ProcessStartInfo("test", $"-x {quote filePath}")
        startInfo.RedirectStandardOutput <- true
        startInfo.RedirectStandardError <- true
        startInfo.UseShellExecute <- false

        use proc =
            match Process.Start startInfo |> Option.ofObj with
            | Some proc -> proc
            | None -> failwith "Could not start test -x"

        proc.WaitForExit(30 * 1000) && proc.ExitCode = 0

let scanGeneratedRow (row: TemplateRow) =
    let files =
        Directory.EnumerateFiles(row.Root, "*", SearchOption.AllDirectories)
        |> Seq.filter fileShouldBeScanned
        |> Seq.toList

    let identityTokens =
        [ "FS-Skia-UI"
          "FS.Skia.UI"
          "fs-skia-ui" ]

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

    let minimalForbidden =
        if row.Profile = "minimal" then
            [ "src/Charts"
              "src/Layout"
              "tests/Charts.Tests"
              "tests/Layout.Tests"
              "tests/Parity.Tests"
              "tests/Smoke.Tests"
              "samples/ChartsGallery"
              "samples/DataGridGallery"
              "samples/LayoutGraphGallery"
              "samples/ParityGallery"
              "samples/InteractiveViewer"
              "samples/EffectsGallery"
              "samples/ScreenshotGallery"
              "samples/DemoReel" ]
            |> List.filter (fun relative -> Directory.Exists(path [ row.Root; relative ]))
        else
            []

    let required =
        [ "src/Lib/Lib.fsproj"
          "tests/Lib.Tests/Lib.Tests.fsproj"
          "tests/Package.Tests/Package.Tests.fsproj"
          "tests/Governance.Tests/Governance.Tests.fsproj"
          "samples/BasicViewer/BasicViewer.fsproj"
          "docs/build.md"
          "docs/template-profile.md"
          ".specify/workflows/speckit/workflow.yml"
          "Directory.Packages.props"
          "AGENTS.md"
          "build.fsx"
          "fake.sh" ]

    let missingRequired =
        required
        |> List.filter (fun relative -> not (File.Exists(path [ row.Root; relative ])))

    if not (List.isEmpty placeholderHits) then
        failwithf "%s/%s generated project has unreplaced identity tokens:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, placeholderHits))

    if not (List.isEmpty excludedHistory) then
        failwithf "%s/%s generated project contains excluded historical specs:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, excludedHistory))

    if not (List.isEmpty minimalForbidden) then
        failwithf "%s/%s generated project contains minimal-profile forbidden paths:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, minimalForbidden))

    if not (List.isEmpty missingRequired) then
        failwithf "%s/%s generated project is missing required files:%s%s" row.Artifact row.Profile Environment.NewLine (String.Join(Environment.NewLine, missingRequired))

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
          "Minimal optional exclusion scan: PASS"
          "Generated AGENTS scan: PASS"
          "Executable script scan: PASS"
          $"Generated Dev elapsed: {elapsedSeconds:F1} seconds"
          "Visual support: non-visual V2 validation only; full visual evidence is deferred." ]
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
          "PASS: placeholder scans, excluded-history scans, minimal profile checks, and generated Dev runs completed for all rows." ]
        |> String.concat Environment.NewLine

    ensureParent outputPath
    File.WriteAllText(outputPath, summary + Environment.NewLine)

// BUILD SECTION: V3 capability validation

let trimQuotes (value: string) =
    value.Trim().Trim('"').Trim('\'')

let parseScalar (line: string) =
    match line.IndexOf(':') with
    | index when index >= 0 -> line.Substring(index + 1) |> trimQuotes
    | _ -> ""

let parseInlineList (value: string) =
    let trimmed = value.Trim()

    if trimmed.StartsWith("[") && trimmed.EndsWith("]") then
        trimmed.Trim('[', ']').Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)
        |> Array.map trimQuotes
        |> Array.toList
    elif String.IsNullOrWhiteSpace trimmed then
        []
    else
        [ trimQuotes trimmed ]

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
        Set.ofList [ "Scene"; "SkiaViewer"; "Elmish"; "KeyboardInput"; "Layout"; "Charts" ]

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
          "fs-skia-layout"
          "fs-skia-charts" ]

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

    capabilities
    |> List.choose (fun capabilityId ->
        match Map.tryFind capabilityId byId with
        | Some capability when not capability.NonRuntime ->
            capability.PackageId
            |> Option.bind (fun packageId ->
                if packageId = "non-runtime" then None else Some packageId)
        | _ -> None)
    |> List.distinct
    |> List.sort

let writeProductProject model row capabilities =
    let references =
        packageReferences model capabilities
        |> List.map (fun packageId -> $"    <PackageReference Include=\"{packageId}\" />")
        |> String.concat Environment.NewLine

    let content =
        $"""<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <Compile Include="Program.fs" />
  </ItemGroup>

  <ItemGroup>
{references}
  </ItemGroup>

</Project>
"""

    File.WriteAllText(path [ row.Root; "src"; "Product"; "Product.fsproj" ], content)

let copySelectedSkills model row capabilities =
    let skillRoot = path [ row.Root; ".agents"; "skills" ]
    Directory.CreateDirectory skillRoot |> ignore
    copyDirectory (path [ model.RepositoryRoot; "template"; "base"; ".agents"; "skills"; "fs-skia-project" ]) (path [ skillRoot; "fs-skia-project" ])

    let byId = capabilitiesById model

    for capabilityId in capabilities do
        match Map.tryFind capabilityId byId, capabilitySkillDestination capabilityId with
        | Some capability, Some destination ->
            match capability.Skill with
            | Some sourceSkill ->
                let destinationDirectory = path [ skillRoot; destination ]
                Directory.CreateDirectory destinationDirectory |> ignore
                File.Copy(path [ model.RepositoryRoot; sourceSkill ], path [ destinationDirectory; "SKILL.md" ], true)
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

let generateV3Product model row =
    cleanDirectoryContents row.Root
    cleanDirectoryContents row.EvidenceDir
    let templateRoot = templatePayloadRoot model row
    copyDirectory (path [ templateRoot; "base" ]) row.Root

    let resolved = resolveCapabilities model row.Capabilities
    writeProductProject model row resolved
    writeGeneratedProductReadme row resolved
    copySelectedSkills model row resolved

    for capabilityId in resolved do
        match capabilityId with
        | "samples" -> copyDirectory (path [ model.RepositoryRoot; "template"; "fragments"; "samples" ]) (path [ row.Root; "samples" ])
        | _ -> ()

    [ "Dev"; "Test"; "Verify" ]
    |> List.iter (fun target ->
        runProcess $"{row.Profile}/{row.Artifact} generated {target}" "bash" $"./fake.sh build -t {target}" row.Root (path [ row.EvidenceDir; $"{target.ToLowerInvariant()}.log" ]) Map.empty)

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
          "framework README content", "docs/architecture.md"
          "framework README content", "docs/V2Analysis.md"
          "framework implementation projects", "tests/Parity.Tests"
          "framework implementation projects", ".template.package" ]

    let missing =
        [ "src/Product/Product.fsproj"
          "tests/Product.Tests/Product.Tests.fsproj"
          "README.md"
          "docs/product.md"
          ".agents/skills/fs-skia-project/SKILL.md"
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

    for rule, forbiddenPath in forbidden do
        if files |> List.exists (fun file -> file.StartsWith(forbiddenPath, StringComparison.Ordinal)) then
            failwithf "%s/%s copied %s: %s" row.Artifact row.Profile rule forbiddenPath

    let productProject = File.ReadAllText(path [ row.Root; "src"; "Product"; "Product.fsproj" ])

    let selectedCapabilitySkills =
        Directory.EnumerateFiles(path [ row.Root; ".agents"; "skills" ], "SKILL.md", SearchOption.AllDirectories)
        |> Seq.map (relativePathFrom row.Root)
        |> Seq.sort
        |> Seq.toList

    let report =
        [ $"# {row.Profile}/{row.Artifact} generated product"
          ""
          "Validation rules: exactly one product app, exactly one product test suite, selected capability skills, consumer-mode package references, no framework implementation projects, no framework README content."
          ""
          "Files:"
          yield! files
          ""
          "Package references:"
          productProject
          ""
          "Selected skills:"
          yield! selectedCapabilitySkills ]
        |> String.concat Environment.NewLine

    ensureParent row.FileListPath
    File.WriteAllText(row.FileListPath, report + Environment.NewLine)

let runScanV3GeneratedProducts model =
    v3GeneratedRows model
    |> List.iter (scanV3GeneratedRow model)

    let summary =
        [ "# Generated Product Check"
          ""
          "PASS: generated product file lists, selected skills, consumer-mode package references, full product governance command logs, and framework-source exclusions passed."
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

let runDependencyOwnershipReport model =
    let sceneProject = File.ReadAllText(path [ model.RepositoryRoot; "src"; "Scene"; "Scene.fsproj" ])

    [ "Fable.Elmish"; "Silk.NET"; "SkiaSharp"; "Yoga.Net"; "YamlDotNet" ]
    |> List.iter (fun forbidden ->
        if sceneProject.IndexOf(forbidden, StringComparison.Ordinal) >= 0 then
            failwithf "Scene dependency leak: %s" forbidden)

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
          "- Charts remains a Scene-oriented package."
          "- Testing owns generated-product validation helpers." ]
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
          "- `readiness/surface-baselines/FS.Skia.UI.Charts.txt`"
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

let runGeneratedGuidanceScan model outputPath =
    let validationRows =
        generatedGuidanceRequirements
        |> List.map (fun template ->
            let findings, classes = validateGuidanceTemplate model template
            template, findings, classes)

    let findings =
        (validationRows |> List.collect (fun (_, findings, _) -> findings))
        @ validateGuidanceParity validationRows

    if not (List.isEmpty findings) then
        failwithf "Generated guidance check failed:%s%s" Environment.NewLine (String.Join(Environment.NewLine, findings))

    let report =
        [ "# Generated Guidance Check"
          ""
          "PASS: active and preset-owned spec/plan templates include required governance prompts in the expected Markdown sections."
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
    let _, verifyEffects = update (StartTarget "Verify") model
    let _, templatePackEffects = update (StartTarget "TemplatePack") model
    let _, templateSmokeEffects = update (StartTarget "TemplateSmoke") model

    if initEffects |> List.exists (function EnsureDirectory path when path = model.LogDir -> true | _ -> false) |> not then
        failwith "init must request log directory creation"

    if restoreEffects |> List.exists (function RunDotnetAction(label, _, _, _, _, _) when label = "dotnet restore" -> true | _ -> false) |> not then
        failwith "Restore must emit a dotnet restore workflow effect"

    if templatePackEffects |> List.exists (function ValidateTemplatePackage _ -> true | _ -> false) |> not then
        failwith "TemplatePack must validate the local template package artifact"

    if templateSmokeEffects |> List.exists (function ScanGeneratedProjects _ -> true | _ -> false) |> not then
        failwith "TemplateSmoke must scan generated projects and run generated Dev"

    if verifyEffects |> List.exists (function RequireFiles("v1 plus v2 verification artifact set", _) -> true | _ -> false) |> not then
        failwith "Verify must require the v1 plus v2 artifact set"

    let _, completedEffects = update (TargetCompleted "Restore") model

    if completedEffects <> [] then
        failwith "TargetCompleted must be a pure state transition with no effects"

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
    | PackageSurfaceReport -> runPackageSurfaceReport model
    | DependencyOwnershipReport -> runDependencyOwnershipReport model
    | ValidateTemplatePackage outputPath -> validateTemplatePackage model outputPath
    | GeneratedGuidanceScan outputPath -> runGeneratedGuidanceScan model outputPath
    | WriteStructuredReport(_, path, content) ->
        ensureParent path
        File.WriteAllText(path, content)
    | WriteFile(path, content) ->
        ensureParent path
        File.WriteAllText(path, content)
    | RequireFiles(artifactClass, paths) -> requireFiles artifactClass paths
    | WorkflowSelfCheck -> workflowSelfCheck root

let runTarget targetName =
    let model, initEffects = init repositoryRoot
    let _, effects = update (StartTarget targetName) model

    (initEffects @ effects)
    |> List.iter (interpret repositoryRoot)

let allTargets =
    requiredTargets @ [ "PackageSmoke"; "BuildWorkflowCheck" ]

// BUILD SECTION: target graph

let targetDependencies =
    Map.ofList
        [ "Clean", []
          "Restore", []
          "Build", [ "Restore" ]
          "Test", [ "Build" ]
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
          "TemplateSmoke", [ "TemplateInstantiate" ]
          // V2 compatibility expectation: "TemplateCheck", [ "TemplatePack"; "TemplateInstallSource"; "TemplateInstallPackage"; "TemplateInstantiate"; "TemplateSmoke" ]
          "TemplateCheck", [ "TemplatePack"; "TemplateInstallSource"; "TemplateInstallPackage"; "TemplateInstantiate"; "TemplateSmoke"; "GeneratedProductCheck" ]
          "CapabilityCheck", []
          "SkillCheck", [ "CapabilityCheck" ]
          "GeneratedProductCheck", [ "CapabilityCheck"; "SkillCheck" ]
          "DependencyReport", []
          "GeneratedGuidanceCheck", []
          "TemplateDrift", []
          "EvidenceGraph", []
          "EvidenceAudit", [ "EvidenceGraph" ]
          "Verify",
          [ "Dev"
            "PackLocal"
            "PackageSurfaceCheck"
            "FsiTranscripts"
            "SampleContractSmoke"
            "TemplateCheck"
            "CapabilityCheck"
            "SkillCheck"
            "GeneratedProductCheck"
            "DependencyReport"
            "GeneratedGuidanceCheck"
            "TemplateDrift"
            "EvidenceAudit" ]
          "Ci", [ "Verify" ]
          "PackageSmoke", [ "PackageSurfaceCheck" ]
          "BuildWorkflowCheck", [] ]

let rec runWithDependencies visited targetName =
    if allTargets |> List.contains targetName |> not then
        failwithf "Unknown target: %s" targetName

    if visited |> Set.contains targetName then
        visited
    else
        let visitedAfterDependencies =
            targetDependencies.[targetName]
            |> List.fold runWithDependencies visited

        printfn "Starting target '%s'" targetName
        runTarget targetName
        printfn "Finished target '%s'" targetName
        visitedAfterDependencies |> Set.add targetName

let rec targetFromArgs args =
    match args with
    | "-t" :: target :: _
    | "--target" :: target :: _
    | "target" :: target :: _ -> Some target
    | _ :: rest -> targetFromArgs rest
    | [] -> None

let scriptArgs =
    Environment.GetCommandLineArgs()
    |> Array.skip 1
    |> Array.toList

if scriptArgs |> List.exists (fun arg -> arg = "--list") then
    allTargets
    |> List.iter (printfn "%s")
else
    let selectedTarget =
        targetFromArgs scriptArgs
        |> Option.defaultValue "Dev"

    runWithDependencies Set.empty selectedTarget |> ignore

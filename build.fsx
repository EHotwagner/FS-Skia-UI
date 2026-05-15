#r "paket:
nuget FSharp.Core 6.0.7
//"

open System
open System.Diagnostics
open System.IO
open System.IO.Compression

type TemplateInstallSource =
    | SourceDirectory
    | PackageArtifact

type TemplateRow =
    { Artifact: string
      Profile: string
      ProjectName: string
      Root: string
      EvidenceDir: string }

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
    | ValidateTemplatePackage of outputPath: string
    | GeneratedGuidanceScan of outputPath: string
    | WriteStructuredReport of label: string * path: string * content: string
    | WriteFile of path: string * content: string
    | RequireFiles of artifactClass: string * paths: string list
    | WorkflowSelfCheck

let repositoryRoot = __SOURCE_DIRECTORY__
let featureId = "007-v2-template-packaging"

let path segments =
    segments |> Array.ofList |> Path.Combine

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
          DependencyReportPath = path [ readiness; "dependencies.md" ]
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
          path [ model.TemplateEvidenceDir; "source-default" ]
          path [ model.TemplateEvidenceDir; "source-minimal" ]
          path [ model.TemplateEvidenceDir; "package-default" ]
          path [ model.TemplateEvidenceDir; "package-minimal" ] ]
        |> List.map EnsureDirectory

    model, effects

let defaultTestProjects =
    [ "tests/Lib.Tests/Lib.Tests.fsproj"
      "tests/Charts.Tests/Charts.Tests.fsproj"
      "tests/Layout.Tests/Layout.Tests.fsproj"
      "tests/Parity.Tests/Parity.Tests.fsproj"
      "tests/Smoke.Tests/Smoke.Tests.fsproj"
      "tests/Governance.Tests/Governance.Tests.fsproj" ]

let packProjects =
    [ "src/Lib/Lib.fsproj", "FS.Skia.UI"
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
    | StartTarget "DependencyReport" ->
        model,
        [ processEffect "dependency report" "dotnet" $"fsi scripts/dependency-report.fsx {quote model.DependencyReportPath}" model.RepositoryRoot (path [ model.LogDir; "dependency-report.txt" ])
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
    match solutionFor root solutionFile with
    | Some solution ->
        let arguments =
            [ action; quote solution; extraArguments ]
            |> List.filter (fun part -> part <> "")
            |> String.concat " "

        runProcess label "dotnet" arguments root outputPath Map.empty
    | None ->
        let existing = existingProjects root projects

        if List.isEmpty existing then
            failwithf "No projects were found for %s. Checked: %s" label (String.Join(", ", projects))

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

let instantiateRow model row =
    cleanDirectoryContents row.Root
    Directory.CreateDirectory row.EvidenceDir |> ignore

    let rootNamespace = row.ProjectName.Replace("-", ".")
    let repositoryUrl = $"https://example.invalid/{row.Artifact}/{row.Profile}/{row.ProjectName}"

    let args =
        [ "new fs-skia-ui"
          $"--name {row.ProjectName}"
          $"--output {quote row.Root}"
          $"--profile {row.Profile}"
          $"--rootNamespace {rootNamespace}"
          $"--packagePrefix {rootNamespace}"
          "--authors TemplateValidation"
          $"--repositoryUrl {quote repositoryUrl}"
          "--targetFramework net10.0" ]
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

let scanGeneratedRow row =
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

let generatedGuidanceRequirements =
    [ ".specify/templates/spec-template.md",
      [ "package impact"
        "public contract impact"
        "state workflow impact"
        "layout/rendering impact"
        "evidence obligations"
        "unsupported scope"
        "build-target impact" ]
      ".specify/presets/fsharp-opinionated/templates/spec-template.md",
      [ "package impact"
        "public contract impact"
        "state workflow impact"
        "layout/rendering impact"
        "evidence obligations"
        "unsupported scope"
        "build-target impact" ]
      ".specify/templates/plan-template.md",
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
      ".specify/presets/fsharp-opinionated/templates/plan-template.md",
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
        "deferred scope" ] ]

let runGeneratedGuidanceScan model outputPath =
    let findings =
        generatedGuidanceRequirements
        |> List.collect (fun (relativePath, needles) ->
            let filePath = path [ model.RepositoryRoot; relativePath ]

            if not (File.Exists filePath) then
                [ $"{relativePath}: missing file" ]
            else
                let content = File.ReadAllText(filePath).ToLowerInvariant()

                needles
                |> List.choose (fun needle ->
                    if content.IndexOf(needle.ToLowerInvariant(), StringComparison.Ordinal) >= 0 then
                        None
                    else
                        Some $"{relativePath}: missing `{needle}`"))

    if not (List.isEmpty findings) then
        failwithf "Generated guidance check failed:%s%s" Environment.NewLine (String.Join(Environment.NewLine, findings))

    let report =
        [ "# Generated Guidance Check"
          ""
          "PASS: active and preset-owned spec/plan templates include V2 governance prompts."
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

let targetDependencies =
    Map.ofList
        [ "Clean", []
          "Restore", []
          "Build", [ "Restore" ]
          "Test", [ "Build" ]
          "Dev", [ "Test" ]
          "PackLocal", [ "Build" ]
          "RefreshSurfaceBaselines", [ "Build" ]
          "PackageSurfaceCheck", [ "Build" ]
          "FsiTranscripts", [ "Build" ]
          "SampleContractSmoke", [ "Build" ]
          "TemplatePack", []
          "TemplateInstallSource", []
          "TemplateInstallPackage", [ "TemplatePack" ]
          "TemplateInstantiate", [ "TemplatePack"; "TemplateInstallSource"; "TemplateInstallPackage" ]
          "TemplateSmoke", [ "TemplateInstantiate" ]
          "TemplateCheck", [ "TemplatePack"; "TemplateInstallSource"; "TemplateInstallPackage"; "TemplateInstantiate"; "TemplateSmoke" ]
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

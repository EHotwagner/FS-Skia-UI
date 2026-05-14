#r "paket:
nuget FSharp.Core 6.0.7
//"

open System
open System.Diagnostics
open System.IO

type BuildModel =
    { RepositoryRoot: string
      FeatureDir: string
      ReadinessDir: string
      LogDir: string
      FsiDir: string
      SampleSmokeDir: string
      PackageEvidenceDir: string
      SurfaceBaselineDir: string
      LocalPackageDir: string
      CompletedTargets: string list }

type BuildMsg =
    | StartTarget of string
    | TargetCompleted of string
    | TargetFailed of string * string

type BuildEffect =
    | EnsureDirectory of string
    | CleanDirectoryContents of string
    | RunProcess of label: string * fileName: string * arguments: string * workingDirectory: string * outputPath: string * environment: Map<string, string>
    | WriteFile of path: string * content: string
    | RequireFiles of artifactClass: string * paths: string list
    | WorkflowSelfCheck

let repositoryRoot = __SOURCE_DIRECTORY__
let featureId = "006-template-framework-governance"

let path segments =
    segments |> Array.ofList |> Path.Combine

let featureDir root =
    path [ root; "specs"; featureId ]

let featureReadiness root =
    path [ featureDir root; "readiness" ]

let localPackageDir () =
    let home =
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)

    path [ home; ".local"; "share"; "nuget-local" ]

let init root =
    let readiness = featureReadiness root

    let model =
        { RepositoryRoot = root
          FeatureDir = featureDir root
          ReadinessDir = readiness
          LogDir = path [ readiness; "logs" ]
          FsiDir = path [ readiness; "fsi" ]
          SampleSmokeDir = path [ readiness; "sample-smoke" ]
          PackageEvidenceDir = path [ readiness; "package" ]
          SurfaceBaselineDir = path [ root; "readiness"; "surface-baselines" ]
          LocalPackageDir = localPackageDir ()
          CompletedTargets = [] }

    let effects =
        [ model.ReadinessDir
          model.LogDir
          model.FsiDir
          model.SampleSmokeDir
          model.PackageEvidenceDir
          model.SurfaceBaselineDir
          model.LocalPackageDir ]
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
      "EvidenceGraph"
      "EvidenceAudit"
      "Verify"
      "Ci" ]

let processEffect label fileName arguments workingDirectory outputPath =
    RunProcess(label, fileName, arguments, workingDirectory, outputPath, Map.empty)

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
          CleanDirectoryContents model.PackageEvidenceDir ]
    | StartTarget "Restore" ->
        model,
        [ processEffect "dotnet tool restore" "dotnet" "tool restore" model.RepositoryRoot (path [ model.LogDir; "restore.txt" ])
          processEffect "dotnet restore" "dotnet" "restore FS-Skia-UI.sln" model.RepositoryRoot (path [ model.LogDir; "restore.txt" ]) ]
    | StartTarget "Build" ->
        model,
        [ processEffect "dotnet build" "dotnet" "build FS-Skia-UI.sln --no-restore" model.RepositoryRoot (path [ model.LogDir; "build.txt" ]) ]
    | StartTarget "Test" ->
        model,
        defaultTestProjects
        |> List.map (fun project ->
            processEffect $"dotnet test {project}" "dotnet" $"test {project} --no-build" model.RepositoryRoot (path [ model.LogDir; "test.txt" ]))
    | StartTarget "Dev" ->
        model,
        [ WriteFile(path [ model.LogDir; "dev-verdict.txt" ], "Dev target completed: Restore, Build, and default non-visual Test targets passed.\n") ]
    | StartTarget "PackLocal" ->
        model,
        (packProjects
         |> List.map (fun (project, packageId) ->
             processEffect $"dotnet pack {packageId}" "dotnet" $"pack {project} -c Release -o {model.LocalPackageDir}" model.RepositoryRoot (path [ model.LogDir; "pack-local.txt" ])))
        @ [ WriteFile(path [ model.PackageEvidenceDir; "local-packages.md" ], $"# Local Packages\n\nOutput directory: `{model.LocalPackageDir}`\n") ]
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
          RequireFiles(
              "stable package surface baselines",
              [ path [ model.SurfaceBaselineDir; "FS.Skia.UI.txt" ]
                path [ model.SurfaceBaselineDir; "FS.Skia.UI.Charts.txt" ]
                path [ model.SurfaceBaselineDir; "FS.Skia.UI.Layout.txt" ] ]
            ) ]
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
              "v1 verification artifact set",
              [ path [ model.LogDir; "build.txt" ]
                path [ model.LogDir; "test.txt" ]
                path [ model.LogDir; "pack-local.txt" ]
                path [ model.LogDir; "package-surface-check.txt" ]
                path [ model.LogDir; "evidence-audit.txt" ]
                path [ model.FsiDir; "prelude.txt" ]
                path [ model.SampleSmokeDir; "BasicViewer.txt" ]
                path [ model.ReadinessDir; "task-graph.json" ] ]
            )
          WriteFile(path [ model.LogDir; "verify-verdict.txt" ], "Verify target completed with all required v1 artifact classes present.\n") ]
    | StartTarget "Ci" ->
        model,
        [ WriteFile(path [ model.LogDir; "ci-verdict.txt" ], "Ci delegates to Verify and completed without duplicating command order.\n") ]
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

let runProcess (label: string) (fileName: string) (arguments: string) (workingDirectory: string) (outputPath: string) (environment: Map<string, string>) =
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

    if proc.WaitForExit(20 * 60 * 1000) then
        let stdout = stdoutTask.Result
        let stderr = stderrTask.Result
        File.AppendAllText(outputPath, stdout)
        File.AppendAllText(outputPath, stderr)
        File.AppendAllText(outputPath, $"\nexit-code={proc.ExitCode}\n")

        if proc.ExitCode <> 0 then
            failwithf "%s failed with exit code %d. See %s" label proc.ExitCode outputPath
    else
        proc.Kill()
        failwithf "%s timed out. See %s" label outputPath

let requireFiles (artifactClass: string) (paths: string list) =
    let missing =
        paths
        |> List.filter (fun path -> not (File.Exists path))

    if missing.Length > 0 then
        let detail = String.Join(Environment.NewLine, missing)
        failwithf "Missing %s:%s%s" artifactClass Environment.NewLine detail

let workflowSelfCheck (root: string) =
    let model, initEffects = init root
    let _, restoreEffects = update (StartTarget "Restore") model
    let _, verifyEffects = update (StartTarget "Verify") model

    if initEffects |> List.exists (function EnsureDirectory path when path = model.LogDir -> true | _ -> false) |> not then
        failwith "init must request log directory creation"

    if restoreEffects |> List.exists (function RunProcess(label, _, _, _, _, _) when label = "dotnet restore" -> true | _ -> false) |> not then
        failwith "Restore must emit a dotnet restore process effect"

    if verifyEffects |> List.exists (function RequireFiles("v1 verification artifact set", _) -> true | _ -> false) |> not then
        failwith "Verify must require the v1 artifact set"

    let _, completedEffects = update (TargetCompleted "Restore") model

    if completedEffects <> [] then
        failwith "TargetCompleted must be a pure state transition with no effects"

let interpret root effect =
    match effect with
    | EnsureDirectory directory -> Directory.CreateDirectory directory |> ignore
    | CleanDirectoryContents directory -> cleanDirectoryContents directory
    | RunProcess(label, fileName, arguments, workingDirectory, outputPath, environment) ->
        runProcess label fileName arguments workingDirectory outputPath environment
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
          "EvidenceGraph", []
          "EvidenceAudit", [ "EvidenceGraph" ]
          "Verify", [ "Dev"; "PackLocal"; "PackageSurfaceCheck"; "FsiTranscripts"; "SampleContractSmoke"; "EvidenceAudit" ]
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

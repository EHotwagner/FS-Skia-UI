module ParityTests

open System
open System.Diagnostics
open System.IO
open System.Text.Json
open Expecto
open FS.Skia.UI

let rec findRepositoryRoot (directory: string) =
    if Directory.GetFiles(directory, "*.sln").Length > 0 || File.Exists(Path.Combine(directory, "build.fsx")) then
        directory
    else
        match Directory.GetParent directory |> Option.ofObj with
        | Some parent -> findRepositoryRoot parent.FullName
        | None -> failwithf "Could not locate repository root from %s" directory

let repositoryRoot = findRepositoryRoot AppContext.BaseDirectory

let runDotnet (arguments: string) =
    let startInfo: ProcessStartInfo = ProcessStartInfo("dotnet", arguments)
    startInfo.WorkingDirectory <- repositoryRoot
    startInfo.RedirectStandardOutput <- true
    startInfo.RedirectStandardError <- true
    startInfo.UseShellExecute <- false

    match Process.Start(startInfo) |> Option.ofObj with
    | None -> failwithf "Could not start dotnet %s" arguments
    | Some proc ->
        use proc = proc
        let stdoutTask = proc.StandardOutput.ReadToEndAsync()
        let stderrTask = proc.StandardError.ReadToEndAsync()

        if proc.WaitForExit(60000) then
            proc.ExitCode, stdoutTask.GetAwaiter().GetResult(), stderrTask.GetAwaiter().GetResult()
        else
            proc.Kill(true)
            -1, stdoutTask.GetAwaiter().GetResult(), stderrTask.GetAwaiter().GetResult()

let readiness segments =
    let historicalFeature = Path.Combine(repositoryRoot, "specs", "002-skia-feature-parity")
    let historicalReadiness = Path.Combine(historicalFeature, "readiness")
    let rootReadiness =
        if File.Exists(Path.Combine(historicalFeature, "spec.md")) then
            historicalReadiness
        else
            Path.Combine(repositoryRoot, "readiness", "parity")

    Path.Combine(Array.ofList (rootReadiness :: segments))

[<Tests>]
let parityReportTests =
    testList "Parity evidence report" [
        test "baseline commit is pinned" {
            Expect.equal Parity.baselineCommit "7aac43dd12903f93004d0c2bf7c6254318a366dc" "baseline commit matches the inspected upstream"
            Expect.equal Parity.baselineCommit.Length 40 "baseline commit is a SHA-1"
        }

        test "report validation requires every pinned-baseline capability" {
            let items =
                Parity.capabilityIds
                |> List.map (fun id ->
                    Parity.createItem id id Supported SemanticTest "dotnet test" "readiness/logs/final-pass-dotnet-test.txt" "" false)

            let report = Parity.createReport "002-skia-feature-parity" items
            Expect.isEmpty (Parity.validateMergeReady report) "complete supported report is merge-ready"
        }

        test "report validation rejects non-conflicting not-yet-supported capabilities" {
            let item =
                Parity.createItem "core-viewer" "Core viewer" NotYetSupported SemanticTest "dotnet test" "readiness/log.txt" "" false

            let report = Parity.createReport "002-skia-feature-parity" [ item ]
            let failures = Parity.validateMergeReady report

            Expect.exists failures (fun failure -> failure.Contains "not merge-ready") "not-yet-supported non-conflicting capability blocks readiness"
        }

        test "parity evidence script writes normalized report JSON" {
            let exitCode, stdout, stderr =
                runDotnet "fsi scripts/parity-evidence.fsx"

            Expect.equal exitCode 0 stderr
            Expect.stringContains stdout "parity-evidence.json" "script reports output path"

            let path = readiness [ "parity-evidence.json" ]
            Expect.isTrue (File.Exists path) "report is written under readiness"

            use document = JsonDocument.Parse(File.ReadAllText path)
            let root = document.RootElement
            let items = root.GetProperty("items")

            Expect.equal (root.GetProperty("baselineCommit").GetString()) Parity.baselineCommit "report contains pinned commit"
            Expect.equal (items.GetArrayLength()) Parity.capabilityIds.Length "one evidence item exists per baseline capability"

            let statuses =
                items.EnumerateArray()
                |> Seq.map (fun item -> item.GetProperty("status").GetString())
                |> Set.ofSeq

            Expect.isFalse (statuses.Contains "NotYetSupported") "merge-ready report has no non-conflicting not-yet-supported capability"

            items.EnumerateArray()
            |> Seq.iter (fun item ->
                Expect.isNonEmpty (item.GetProperty("capabilityId").GetString()) "item has id"
                Expect.isNonEmpty (item.GetProperty("evidenceType").GetString()) "item has evidence type"
                Expect.isNonEmpty (item.GetProperty("command").GetString()) "item has command"
                Expect.isNonEmpty (item.GetProperty("path").GetString()) "item has evidence path")
        }

        test "documentation covers parity matrix and project adaptations" {
            let historicalFeature = Path.Combine(repositoryRoot, "specs", "002-skia-feature-parity")
            let quickstartPath = Path.Combine(historicalFeature, "quickstart.md")
            let matrixPath = readiness [ "parity"; "parity-matrix.md" ]

            if File.Exists quickstartPath && File.Exists matrixPath then
                let quickstart = File.ReadAllText quickstartPath

                let matrix = File.ReadAllText matrixPath
                Expect.stringContains matrix Parity.baselineCommit "matrix names pinned baseline"
                Expect.stringContains matrix "Vulkan-only" "matrix documents Vulkan-only adaptation"
                Expect.stringContains matrix "Elmish-only" "matrix documents Elmish-only adaptation"
                Expect.stringContains matrix "fallback renderer" "matrix documents excluded fallback behavior"
                Expect.stringContains quickstart "samples/ScreenshotGallery/ScreenshotGallery.fsproj" "quickstart lists screenshot sample command"
                Expect.stringContains quickstart "samples/DemoReel/DemoReel.fsproj" "quickstart lists demo reel command"
            else
                Expect.isFalse (Directory.Exists historicalFeature) "historical parity specs are excluded from generated projects"
        }
    ]

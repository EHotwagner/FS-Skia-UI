open System
open System.Diagnostics
open System.IO

let path parts = Path.Combine(Array.ofList parts)

let targetFromArgs args =
    let rec loop values =
        match values with
        | "-t" :: target :: _
        | "--target" :: target :: _
        | "target" :: target :: _ -> target
        | _ :: rest -> loop rest
        | [] -> "Dev"

    loop args

let writeLog target =
    Directory.CreateDirectory("readiness/logs") |> ignore
    File.WriteAllText(Path.Combine("readiness", "logs", target + ".txt"), $"{target} completed for generated product.{Environment.NewLine}")
    printfn "%s completed for generated product" target

let generatedTargetDependencies =
    [ ("EvidenceAudit", [ "EvidenceGraph" ]) ]

let authoritativeEvidenceScriptContract = ".specify/extensions/evidence/scripts/bash/run-audit.sh"
let generatedFailedStatusContract = "status=failed"

let writeLines (filePath: string) lines =
    let directory = Path.GetDirectoryName filePath

    if not (String.IsNullOrWhiteSpace directory) then
        Directory.CreateDirectory directory |> ignore

    File.WriteAllLines(filePath, Array.ofList lines)

let tryWriteTextLog (filePath: string) (content: string) =
    try
        let directory = Path.GetDirectoryName filePath

        if not (String.IsNullOrWhiteSpace directory) then
            Directory.CreateDirectory directory |> ignore

        File.WriteAllText(filePath, content)
        None
    with ex ->
        Some $"unreadable readiness log: {filePath}; diagnostics={ex.Message}"

let envOption name =
    match Environment.GetEnvironmentVariable name with
    | null -> None
    | value when String.IsNullOrWhiteSpace value -> None
    | value -> Some value

let writeGeneratedEvidencePackageFile featureDir relativePath lines =
    writeLines (path [ featureDir; relativePath ]) lines

let ensureGeneratedEvidencePackage () =
    let selected =
        [ "SPECKIT_FEATURE_DIR"; "GENERATED_EVIDENCE_FEATURE_DIR" ]
        |> List.tryPick envOption
        |> Option.map Path.GetFullPath
        |> Option.filter Directory.Exists

    match selected with
    | Some featureDir -> featureDir
    | None ->
        let specsDir = Path.GetFullPath "specs"
        let featureDir = path [ specsDir; "generated-evidence-workflow" ]
        let readinessDir = path [ featureDir; "readiness" ]
        Directory.CreateDirectory readinessDir |> ignore

        writeGeneratedEvidencePackageFile
            featureDir
            "spec.md"
            [ "# Generated Evidence Workflow"
              ""
              "Generated project package for authoritative evidence command validation." ]

        writeGeneratedEvidencePackageFile
            featureDir
            "plan.md"
            [ "# Generated Evidence Workflow Plan"
              ""
              "Run the copied Spec Kit evidence graph and audit scripts over this generated package." ]

        writeGeneratedEvidencePackageFile
            featureDir
            "tasks.md"
            [ "# Tasks: Generated Evidence Workflow"
              ""
              "## Status Legend"
              ""
              "- `[ ]` - pending"
              "- `[X]` - done with real evidence"
              "- `[S]` - done with synthetic evidence only"
              "- `[F]` - failed"
              "- `[-]` - skipped"
              ""
              "## Phase 1: Generated Evidence"
              ""
              "- [X] T001 [skillist: []] Validate generated evidence command package with authoritative Spec Kit scripts"
              ""
              "## Synthetic-Evidence Inventory"
              ""
              "| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |"
              "|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|" ]

        writeGeneratedEvidencePackageFile
            featureDir
            "tasks.deps.yml"
            [ "schema_version: \"1.0\""
              "tasks:"
              "  T001:"
              "    deps: []"
              "    skillist: []" ]

        writeGeneratedEvidencePackageFile
            featureDir
            "readiness/governance-risk-levels.md"
            [ "# Governance Risk Levels"
              ""
              "small medium broad required evidence broad validation" ]

        writeGeneratedEvidencePackageFile
            featureDir
            "readiness/aggregate-hang-diagnostics.md"
            [ "# Aggregate Hang Diagnostics"
              ""
              "verdict stage elapsed duration last observed command focused rerun non-authoritative aggregate" ]

        writeGeneratedEvidencePackageFile
            featureDir
            "readiness/runtime-limitations.md"
            [ "# Runtime Limitations"
              ""
              ".NET 10 desktop Vulkan SkiaSharp preview unsupported macOS/mobile/browser no software-renderer fallback" ]

        featureDir

let runAuthoritativeEvidence target featureDir graphOnly =
    let script = authoritativeEvidenceScriptContract

    if not (File.Exists script) then
        4, "", $"missing authoritative evidence script: {script}"
    else
        Directory.CreateDirectory(path [ "readiness"; "logs" ]) |> ignore
        let arguments =
            if graphOnly then
                $"{script} {featureDir} --graph-only"
            else
                $"{script} {featureDir}"

        let startInfo = ProcessStartInfo("bash", arguments)
        startInfo.RedirectStandardOutput <- true
        startInfo.RedirectStandardError <- true
        startInfo.UseShellExecute <- false
        startInfo.WorkingDirectory <- Directory.GetCurrentDirectory()

        try
            match Process.Start(startInfo) |> Option.ofObj with
            | None -> 5, "", $"failed command launch: bash {arguments}"
            | Some proc ->
                use proc = proc
                // Drain stdout and stderr concurrently before waiting: reading one
                // stream to end before the other deadlocks when the child fills the
                // other pipe (e.g. a large evidence-audit diff scan).
                let stdoutTask = proc.StandardOutput.ReadToEndAsync()
                let stderrTask = proc.StandardError.ReadToEndAsync()
                proc.WaitForExit()
                let stdout = stdoutTask.Result
                let stderr = stderrTask.Result

                let output = stdout + stderr
                let logPath = path [ "readiness"; "logs"; target + ".txt" ]

                match tryWriteTextLog logPath output with
                | Some diagnostic -> 6, stdout, stderr + Environment.NewLine + diagnostic
                | None ->
                    printf "%s" output
                    proc.ExitCode, stdout, stderr
        with ex ->
            5, "", $"failed command launch: bash {arguments}; diagnostics={ex.Message}"

let writeGeneratedEvidenceReport (target: string) (featureDir: string) (exitCode: int) (stdout: string) (stderr: string) =
    let reportPath =
        match target with
        | "EvidenceGraph" -> path [ "readiness"; "evidence-graph.md" ]
        | "EvidenceAudit" -> path [ "readiness"; "evidence-audit.md" ]
        | _ -> path [ "readiness"; target + ".md" ]

    let status = if exitCode = 0 then "ok" else "failed"
    let validationArea =
        if target = "EvidenceGraph" then "graph-validation-only"
        elif stdout.Contains("Readiness contract scan", StringComparison.OrdinalIgnoreCase) || stderr.Contains("readiness contract", StringComparison.OrdinalIgnoreCase) then "readiness-contract"
        elif stdout.Contains("diff-scan", StringComparison.OrdinalIgnoreCase) || stderr.Contains("diff-scan", StringComparison.OrdinalIgnoreCase) then "diff-scan"
        elif stdout.Contains("synthetic", StringComparison.OrdinalIgnoreCase) || stderr.Contains("synthetic", StringComparison.OrdinalIgnoreCase) then "synthetic-evidence"
        elif stdout.Contains("unsupported", StringComparison.OrdinalIgnoreCase) || stderr.Contains("unsupported", StringComparison.OrdinalIgnoreCase) then "unsupported-host-classification"
        else "evidence-audit"

    let diagnostics =
        (stdout + stderr).Replace("\r\n", "\n").Split('\n')
        |> Array.filter (fun line ->
            line.Contains("verdict=", StringComparison.OrdinalIgnoreCase)
            || line.Contains("[BLOCK]", StringComparison.OrdinalIgnoreCase)
            || line.Contains("failed", StringComparison.OrdinalIgnoreCase)
            || line.Contains("missing", StringComparison.OrdinalIgnoreCase))
        |> Array.truncate 12
        |> Array.toList

    let message =
        if exitCode = 0 then
            "authoritative validation passed"
        else
            "authoritative validation failed"

    let graphOnlyNotes =
        if target = "EvidenceGraph" then
            [ "mode=graph-validation-only"
              "next-action=Run EvidenceAudit for full merge-gate validation, including diff-scan and synthetic-evidence blocking checks." ]
        else
            []

    let lines =
        [ "# Generated Evidence Command Report"
          ""
          $"command=./fake.sh build -t {target}"
          $"target={target}"
          $"generated-project-identity={Directory.GetCurrentDirectory()}"
          $"feature-directory={featureDir}"
          "authority=delegated-authoritative"
          $"status={status}"
          $"exit-code={exitCode}"
          $"validation-area={validationArea}"
          $"report-path={reportPath}"
          $"message={message}" ]
        @ graphOnlyNotes
        @ [ "diagnostics=" ]
        @ (if List.isEmpty diagnostics then [ "- none" ] else diagnostics |> List.map (fun line -> "- " + line.Trim()))

    writeLines reportPath lines
    exitCode

let runGeneratedEvidenceGraph () =
    let featureDir = ensureGeneratedEvidencePackage ()
    let exitCode, stdout, stderr = runAuthoritativeEvidence "EvidenceGraph" featureDir true
    writeGeneratedEvidenceReport "EvidenceGraph" featureDir exitCode stdout stderr

let runGeneratedEvidenceAudit () =
    let featureDir = ensureGeneratedEvidencePackage ()
    let graphExitCode, graphStdout, graphStderr = runAuthoritativeEvidence "EvidenceGraph" featureDir true
    writeGeneratedEvidenceReport "EvidenceGraph" featureDir graphExitCode graphStdout graphStderr |> ignore

    if graphExitCode <> 0 then
        writeGeneratedEvidenceReport "EvidenceAudit" featureDir graphExitCode graphStdout graphStderr
    else
        let auditExitCode, auditStdout, auditStderr = runAuthoritativeEvidence "EvidenceAudit" featureDir false
        writeGeneratedEvidenceReport "EvidenceAudit" featureDir auditExitCode auditStdout auditStderr

let runProcess (target: string) (fileName: string) (arguments: string) =
    Directory.CreateDirectory("readiness/logs") |> ignore
    let logPath = Path.Combine("readiness", "logs", target + ".txt")
    let startInfo = ProcessStartInfo(fileName, arguments)
    startInfo.RedirectStandardOutput <- true
    startInfo.RedirectStandardError <- true
    startInfo.UseShellExecute <- false
    startInfo.WorkingDirectory <- Directory.GetCurrentDirectory()

    let proc =
        try
            Process.Start(startInfo) |> Option.ofObj
        with ex ->
            failwithf "%s failed command launch: %s %s; diagnostics=%s" target fileName arguments ex.Message

    use proc =
        match proc with
        | Some proc -> proc
        | None -> failwithf "%s failed command launch: %s %s" target fileName arguments

    // Drain stdout and stderr concurrently before waiting: reading one stream to
    // end before the other deadlocks when the child fills the other pipe (e.g. a
    // large evidence-audit diff scan).
    let stdoutTask = proc.StandardOutput.ReadToEndAsync()
    let stderrTask = proc.StandardError.ReadToEndAsync()
    proc.WaitForExit()
    let stdout = stdoutTask.Result
    let stderr = stderrTask.Result

    let output = stdout + stderr

    match tryWriteTextLog logPath output with
    | Some diagnostic -> failwithf "%s failed readiness log write; %s" target diagnostic
    | None -> ()

    printf "%s" output

    if output.IndexOf("NU1603", StringComparison.OrdinalIgnoreCase) >= 0 then
        failwithf "%s failed package-resolution: NU1603 fallback is not authoritative generated-product evidence" target

    if proc.ExitCode <> 0 then
        failwithf "%s failed with exit code %d; see %s" target proc.ExitCode logPath

let runGeneratedTests () =
    runProcess "Test" "dotnet" "test tests/Product.Tests/Product.Tests.fsproj -m:1 --disable-build-servers"
    printfn "Test completed for generated product"

let run target =
    match target with
    | "Dev"
    | "GeneratedGuidanceCheck"
    | "TemplateDrift" -> writeLog target
    | "EvidenceGraph" ->
        let exitCode = runGeneratedEvidenceGraph ()
        if exitCode <> 0 then
            failwithf "EvidenceGraph failed with exit code %d; see readiness/evidence-graph.md" exitCode
    | "EvidenceAudit" ->
        let exitCode = runGeneratedEvidenceAudit ()
        if exitCode <> 0 then
            failwithf "EvidenceAudit failed with exit code %d; see readiness/evidence-audit.md" exitCode
    | "Test" -> runGeneratedTests ()
    | "Verify" ->
        [ "Dev"; "GeneratedGuidanceCheck"; "TemplateDrift" ]
        |> List.iter writeLog
        let graphExitCode = runGeneratedEvidenceGraph ()
        if graphExitCode <> 0 then
            failwithf "EvidenceGraph failed with exit code %d; see readiness/evidence-graph.md" graphExitCode
        let auditExitCode = runGeneratedEvidenceAudit ()
        if auditExitCode <> 0 then
            failwithf "EvidenceAudit failed with exit code %d; see readiness/evidence-audit.md" auditExitCode
        runGeneratedTests ()
        writeLog "Verify"
        printfn "Verify completed for generated product"
    | other ->
        failwithf "Unknown generated product target: %s" other

Environment.GetCommandLineArgs()
|> Array.skip 1
|> Array.toList
|> targetFromArgs
|> run

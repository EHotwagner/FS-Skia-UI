#r "nuget: FS.Skia.UI.Build, 0.1.58-preview.1"

open System
open System.Diagnostics
open System.IO
open FS.Skia.UI.Build.Evidence

// Feature 043 (FR-013): generated projects run the EvidenceGraph / EvidenceAudit
// gates IN-PROCESS through the published FS.Skia.UI.Build engine. No Python or
// shell audit scripts are copied into or executed by generated products; the only
// retained external process is `dotnet test`. The `#r "nuget:"` version above is
// kept in sync with the FS.Skia.UI.Build pin in Directory.Packages.props.

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
              "Generated project package for in-process evidence command validation." ]

        writeGeneratedEvidencePackageFile
            featureDir
            "plan.md"
            [ "# Generated Evidence Workflow Plan"
              ""
              "Run the in-process FS.Skia.UI.Build evidence graph and audit engine over this generated package." ]

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
              "- [X] T001 [skillist: []] Validate generated evidence command package with the in-process engine"
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

// ----- in-process evidence engine (FS.Skia.UI.Build.Evidence) -----

let private evidenceReadAll p = if File.Exists p then File.ReadAllText p else ""

let buildEvidenceInputs (featureDir: string) : EvidenceInputs =
    let repoRoot = Directory.GetCurrentDirectory()
    let readinessDir = path [ featureDir; "readiness" ]
    let featureName =
        Path.GetFileName(featureDir.TrimEnd('/', '\\')) |> Option.ofObj |> Option.defaultValue "generated"
    let readinessFiles =
        if Directory.Exists readinessDir then
            Directory.GetFiles(readinessDir, "*", SearchOption.AllDirectories)
            |> Array.map (fun p -> p.Substring(readinessDir.Length + 1).Replace('\\', '/'), File.ReadAllText p)
            |> List.ofArray
        else
            []
    let featureText =
        [ "spec.md"; "plan.md"; "tasks.md" ]
        |> List.map (fun n -> evidenceReadAll (path [ featureDir; n ]))
        |> String.concat "\n"
    let auditStatusFiles =
        readinessFiles
        |> List.filter (fun (rel, c) ->
            rel.EndsWith ".md"
            && c.Contains "```audit-status"
            && not (rel.ToLowerInvariant().StartsWith "audit-fixtures/")
            && not (rel.ToLowerInvariant().StartsWith "audit-rejections/"))
    let slePath = path [ readinessDir; "skill-loading-evidence.md" ]
    { FeatureName = featureName
      TasksMd = evidenceReadAll (path [ featureDir; "tasks.md" ])
      DepsYml = evidenceReadAll (path [ featureDir; "tasks.deps.yml" ])
      Registry = SkillRegistry.build repoRoot
      SkillLoadingEvidence = (if File.Exists slePath then Some(File.ReadAllText slePath) else None)
      ResolvedExists = (fun p -> File.Exists(if Path.IsPathRooted p then p else path [ repoRoot; p ]))
      Canonicalize = (fun p -> Path.GetFullPath(if Path.IsPathRooted p then p else path [ repoRoot; p ]))
      RecordedFeature = Some featureName
      Scan = { ReadinessDir = readinessDir; FeatureText = featureText; ReadinessFiles = readinessFiles }
      AuditStatusFiles = auditStatusFiles
      PatternsYml = evidenceReadAll (path [ ".specify"; "extensions"; "evidence"; "audit-patterns.yml" ])
      UnifiedDiff = "" }

let evidenceWrite (p: string) (content: string) =
    let dir = Path.GetDirectoryName p
    if not (String.IsNullOrWhiteSpace dir) then
        Directory.CreateDirectory dir |> ignore
    File.WriteAllText(p, content)

let writeGeneratedEvidenceReport (target: string) (featureDir: string) (exitCode: int) (validationArea: string) (diagnostics: string list) =
    let reportPath =
        match target with
        | "EvidenceGraph" -> path [ "readiness"; "evidence-graph.md" ]
        | "EvidenceAudit" -> path [ "readiness"; "evidence-audit.md" ]
        | _ -> path [ "readiness"; target + ".md" ]

    let status = if exitCode = 0 then "ok" else "failed"
    let message = if exitCode = 0 then "in-process validation passed" else "in-process validation failed"

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
          "authority=in-process-engine"
          "engine=FS.Skia.UI.Build.Evidence"
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

let private writeArtifacts featureDir (arts: AuditArtifacts) =
    let r = path [ featureDir; "readiness" ]
    evidenceWrite (path [ r; "task-graph.json" ]) arts.TaskGraphJson
    evidenceWrite (path [ r; "task-graph.md" ]) arts.TaskGraphMd
    evidenceWrite (path [ r; "seh-audit-summary.json" ]) arts.SehAuditSummary
    evidenceWrite (path [ r; "readiness-contract-hits.json" ]) arts.ReadinessContractHits
    evidenceWrite (path [ r; "persistent-launch-hits.json" ]) arts.PersistentLaunchHits
    evidenceWrite (path [ r; "persistent-gui-runtime-hits.json" ]) arts.PersistentGuiRuntimeHits
    evidenceWrite (path [ r; "window-visibility-hits.json" ]) arts.WindowVisibilityHits
    evidenceWrite (path [ r; "audit-status-hits.json" ]) arts.AuditStatusHits
    evidenceWrite (path [ r; "diff-scan-hits.json" ]) arts.DiffScanHits

let runGeneratedEvidenceGraph () =
    let featureDir = ensureGeneratedEvidencePackage ()
    let inputs = buildEvidenceInputs featureDir
    let gr, graphArts = Engine.runGraph inputs
    evidenceWrite (path [ featureDir; "readiness"; "task-graph.json" ]) graphArts.TaskGraphJson
    evidenceWrite (path [ featureDir; "readiness"; "task-graph.md" ]) graphArts.TaskGraphMd
    let exitCode =
        match gr.Verdict with
        | GraphVerdict.Ok -> 0
        | GraphVerdict.Error -> 2
    let verdictStr = if exitCode = 0 then "ok" else "error"
    let diagnostics =
        [ $"verdict={verdictStr}"
          $"tasks={List.length gr.Tasks}" ]
        @ (gr.Errors |> List.truncate 12)
    writeGeneratedEvidenceReport "EvidenceGraph" featureDir exitCode "graph-validation-only" diagnostics

let private auditValidationArea (res: AuditResult) =
    if res.ReadinessContract > 0 then "readiness-contract"
    elif res.DiffBlocking > 0 then "diff-scan"
    elif not (List.isEmpty res.SehSummary.UnacceptedSyntheticTasks) || not (List.isEmpty res.SehSummary.Diagnostics) then "synthetic-evidence"
    elif res.PersistentLaunch > 0 || res.PersistentGuiRuntime > 0 || res.WindowVisibility > 0 then "unsupported-host-classification"
    else "evidence-audit"

let runGeneratedEvidenceAudit () =
    let featureDir = ensureGeneratedEvidencePackage ()
    let inputs = buildEvidenceInputs featureDir
    let gr, graphArts = Engine.runGraph inputs
    evidenceWrite (path [ featureDir; "readiness"; "task-graph.json" ]) graphArts.TaskGraphJson
    evidenceWrite (path [ featureDir; "readiness"; "task-graph.md" ]) graphArts.TaskGraphMd

    match gr.Verdict with
    | GraphVerdict.Error ->
        let diagnostics = [ "verdict=error" ] @ (gr.Errors |> List.truncate 12)
        writeGeneratedEvidenceReport "EvidenceGraph" featureDir 2 "graph-validation-only" diagnostics |> ignore
        writeGeneratedEvidenceReport "EvidenceAudit" featureDir 2 "graph-validation-only" diagnostics
    | GraphVerdict.Ok ->
        writeGeneratedEvidenceReport "EvidenceGraph" featureDir 0 "graph-validation-only" [ "verdict=ok"; $"tasks={List.length gr.Tasks}" ] |> ignore
        let res, arts = Engine.runAudit inputs
        writeArtifacts featureDir arts
        let exitCode =
            match res.Verdict with
            | AuditVerdict.Pass -> 0
            | AuditVerdict.Fail -> 2
        let verdictStr = if exitCode = 0 then "PASS" else "FAIL"
        let diagnostics =
            [ $"verdict={verdictStr}"
              $"real-tasks={res.RealTasks}"
              $"unaccepted-synthetic-tasks={List.length res.SehSummary.UnacceptedSyntheticTasks}"
              $"late-seh-tasks={List.length res.SehSummary.LateSehTasks}"
              $"total-blockers={res.TotalBlockers}" ]
        writeGeneratedEvidenceReport "EvidenceAudit" featureDir exitCode (auditValidationArea res) diagnostics

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
    // end before the other deadlocks when the child fills the other pipe.
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

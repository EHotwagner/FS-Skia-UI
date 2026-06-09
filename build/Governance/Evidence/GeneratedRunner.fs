namespace FS.Skia.UI.Build.Evidence

open System
open System.IO

// Feature 064 (FR-004 / R1): verbatim relocation of the generated-product evidence
// orchestration from template/base/build.fsx into the engine, so the generated script can
// reflection-invoke it after resolving the engine assembly from <FsSkiaUiVersion>. This is
// an I/O edge (reads readiness, writes artifacts/reports); the Evidence.Engine it drives
// stays pure.
module GeneratedRunner =

    let private path parts = Path.Combine(Array.ofList parts)

    let private writeLines (filePath: string) (lines: string list) =
        let directory = Path.GetDirectoryName filePath |> Option.ofObj |> Option.defaultValue ""

        if not (String.IsNullOrWhiteSpace directory) then
            Directory.CreateDirectory directory |> ignore

        File.WriteAllLines(filePath, Array.ofList lines)

    let private envOption name =
        match Environment.GetEnvironmentVariable name with
        | null -> None
        | value when String.IsNullOrWhiteSpace value -> None
        | value -> Some value

    let private featureDirectoryFromJson (jsonPath: string) : string option =
        if not (File.Exists jsonPath) then
            None
        else
            let content = File.ReadAllText jsonPath
            let marker = "\"feature_directory\""
            let markerIndex = content.IndexOf(marker, StringComparison.Ordinal)

            if markerIndex < 0 then
                None
            else
                let afterMarker = content.Substring(markerIndex + marker.Length)
                let colonIndex = afterMarker.IndexOf(':')

                if colonIndex < 0 then
                    None
                else
                    let afterColon = afterMarker.Substring(colonIndex + 1)
                    let firstQuote = afterColon.IndexOf('"')

                    if firstQuote < 0 then
                        None
                    else
                        let afterFirstQuote = afterColon.Substring(firstQuote + 1)
                        let secondQuote = afterFirstQuote.IndexOf('"')

                        if secondQuote < 0 then
                            None
                        else
                            let value = afterFirstQuote.Substring(0, secondQuote)
                            if String.IsNullOrWhiteSpace value then None else Some value

    let private resolveFeatureDir (repoRoot: string) : string =
        let absolutize (p: string) = if Path.IsPathRooted p then p else path [ repoRoot; p ]
        let featureJson = path [ repoRoot; ".specify"; "feature.json" ]

        match envOption "SPECKIT_FEATURE_DIR" |> Option.map absolutize with
        | Some dir when Directory.Exists dir ->
            printfn "feature-source=SPECKIT_FEATURE_DIR override"
            dir
        | Some dir ->
            failwithf
                "Cannot resolve the feature to validate: SPECKIT_FEATURE_DIR=%s does not exist. Point it at an existing feature directory, or unset it to resolve from .specify/feature.json."
                dir
        | None ->
            match featureDirectoryFromJson featureJson |> Option.map absolutize with
            | Some dir when Directory.Exists dir ->
                printfn "feature-source=.specify/feature.json"
                dir
            | Some dir ->
                failwithf
                    "Cannot resolve the feature to validate: .specify/feature.json names \"feature_directory\"=%s, which does not exist. Run /speckit.specify to record a feature, or set SPECKIT_FEATURE_DIR to override. Validation never falls back to a bundled sample."
                    dir
            | None ->
                failwithf
                    "Cannot resolve the feature to validate: no SPECKIT_FEATURE_DIR override is set and %s has no usable \"feature_directory\" entry. Run /speckit.specify to record a feature, or set SPECKIT_FEATURE_DIR to the feature directory to validate. Validation never falls back to a bundled sample."
                    featureJson

    let private evidenceReadAll p = if File.Exists p then File.ReadAllText p else ""

    let private buildEvidenceInputs (repoRoot: string) (featureDir: string) : EvidenceInputs =
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
          PatternsYml = evidenceReadAll (path [ repoRoot; ".specify"; "extensions"; "evidence"; "audit-patterns.yml" ])
          BaseRef = None
          UnifiedDiff = "" }

    let private evidenceWrite (p: string) (content: string) =
        let dir = Path.GetDirectoryName p |> Option.ofObj |> Option.defaultValue ""
        if not (String.IsNullOrWhiteSpace dir) then
            Directory.CreateDirectory dir |> ignore
        File.WriteAllText(p, content)

    let private writeGeneratedEvidenceReport (repoRoot: string) (target: string) (featureDir: string) (exitCode: int) (validationArea: string) (diagnostics: string list) =
        let reportPath =
            match target with
            | "EvidenceGraph" -> path [ repoRoot; "readiness"; "evidence-graph.md" ]
            | "EvidenceAudit" -> path [ repoRoot; "readiness"; "evidence-audit.md" ]
            | _ -> path [ repoRoot; "readiness"; target + ".md" ]

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
              $"generated-project-identity={repoRoot}"
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

    let private writeArtifacts (featureDir: string) (arts: AuditArtifacts) =
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

    let private runGeneratedEvidenceGraph (repoRoot: string) =
        let featureDir = resolveFeatureDir repoRoot
        let inputs = buildEvidenceInputs repoRoot featureDir
        let gr, graphArts = Engine.runGraph inputs
        printfn "feature-directory=%s" featureDir
        printfn "tasks=%d" (List.length gr.Tasks)
        evidenceWrite (path [ featureDir; "readiness"; "task-graph.json" ]) graphArts.TaskGraphJson
        evidenceWrite (path [ featureDir; "readiness"; "task-graph.md" ]) graphArts.TaskGraphMd

        let exitCode =
            match gr.Verdict with
            | GraphVerdict.Ok -> 0
            | GraphVerdict.Error -> 2

        let verdictStr = if exitCode = 0 then "ok" else "error"

        let diagnostics =
            [ $"verdict={verdictStr}"; $"tasks={List.length gr.Tasks}" ] @ (gr.Errors |> List.truncate 12)

        writeGeneratedEvidenceReport repoRoot "EvidenceGraph" featureDir exitCode "graph-validation-only" diagnostics

    let private auditValidationArea (res: AuditResult) =
        if res.ReadinessContract > 0 then "readiness-contract"
        elif res.DiffBlocking > 0 then "diff-scan"
        elif not (List.isEmpty res.SehSummary.UnacceptedSyntheticTasks) || not (List.isEmpty res.SehSummary.Diagnostics) then "synthetic-evidence"
        elif res.PersistentLaunch > 0 || res.PersistentGuiRuntime > 0 || res.WindowVisibility > 0 then "unsupported-host-classification"
        else "evidence-audit"

    let private runGeneratedEvidenceAudit (repoRoot: string) =
        let featureDir = resolveFeatureDir repoRoot
        let inputs = buildEvidenceInputs repoRoot featureDir
        let gr, graphArts = Engine.runGraph inputs
        printfn "feature-directory=%s" featureDir
        printfn "tasks=%d" (List.length gr.Tasks)
        evidenceWrite (path [ featureDir; "readiness"; "task-graph.json" ]) graphArts.TaskGraphJson
        evidenceWrite (path [ featureDir; "readiness"; "task-graph.md" ]) graphArts.TaskGraphMd

        match gr.Verdict with
        | GraphVerdict.Error ->
            let diagnostics = [ "verdict=error" ] @ (gr.Errors |> List.truncate 12)
            writeGeneratedEvidenceReport repoRoot "EvidenceGraph" featureDir 2 "graph-validation-only" diagnostics |> ignore
            writeGeneratedEvidenceReport repoRoot "EvidenceAudit" featureDir 2 "graph-validation-only" diagnostics
        | GraphVerdict.Ok ->
            writeGeneratedEvidenceReport repoRoot "EvidenceGraph" featureDir 0 "graph-validation-only" [ "verdict=ok"; $"tasks={List.length gr.Tasks}" ] |> ignore
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

            writeGeneratedEvidenceReport repoRoot "EvidenceAudit" featureDir exitCode (auditValidationArea res) diagnostics

    let run (target: string) (repoRoot: string) : int =
        match target with
        | "EvidenceGraph" -> runGeneratedEvidenceGraph repoRoot
        | "EvidenceAudit" -> runGeneratedEvidenceAudit repoRoot
        | other -> failwithf "GeneratedRunner only handles EvidenceGraph/EvidenceAudit; got %s" other

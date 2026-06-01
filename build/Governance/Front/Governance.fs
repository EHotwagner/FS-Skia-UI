module FS.Skia.UI.Build.Front.Governance

open System
open System.IO
open BuildPaths
open BuildProcess
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Engine.Model
open FS.Skia.UI.Build.Engine.Update
open FS.Skia.UI.Build.Front.Helpers

// Relocated verbatim from build.fsx (feature 045): workflow self-check, skill-tree +
// constitution regeneration, route selection, and the evidence graph/audit gate drivers.

let workflowSelfCheck (root: string) =
    let model, initEffects = init root
    let _, restoreEffects = update (StartTarget Targets.Restore) model
    let _, verifyPreflightEffects = update (StartTarget Targets.VerifyPreflight) model
    let _, ciPreflightEffects = update (StartTarget Targets.CiPreflight) model
    let _, verifyEffects = update (StartTarget Targets.Verify) model
    let _, focusedEffects = update (StartTarget Targets.ControlsRenderingCheck) model
    let _, templatePackEffects = update (StartTarget Targets.TemplatePack) model
    let _, templateSmokeEffects = update (StartTarget Targets.TemplateSmoke) model

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

// Feature 044 — skill-tree enumeration edge (US1). Reads every file under a tree root
// into a SkillTreeGen.SkillFile (repo-relative, forward-slash RelPath + raw bytes), sorted
// for determinism. This is the only file I/O for the generator; the pure plan/currency
// live in the library (Principle IV). No diff/cmp/sha256sum/symlink shelling — in-process.
let enumerateSkillTree (root: string) (treeRelRoot: string) : FS.Skia.UI.Build.SkillTreeGen.SkillFile list =
    let fullRoot = path (root :: (treeRelRoot.Split('/') |> List.ofArray))

    if not (Directory.Exists fullRoot) then
        []
    else
        Directory.EnumerateFiles(fullRoot, "*", SearchOption.AllDirectories)
        |> Seq.sort
        |> Seq.map (fun full ->
            let rel = (Path.GetRelativePath(root, full)).Replace('\\', '/')
            let segments = rel.Split('/')

            let slug =
                match segments |> Array.tryFindIndex (fun s -> s = "skills") with
                | Some i when i + 1 < segments.Length -> segments.[i + 1]
                | _ -> rel

            let file: FS.Skia.UI.Build.SkillTreeGen.SkillFile =
                { Slug = slug
                  RelPath = rel
                  Bytes = File.ReadAllBytes full }

            file)
        |> Seq.toList

// Feature 044 — SkillSyncCheck (reframed): generation-currency over the whole tree.
// `.claude/skills` MUST be a current regeneration of canonical `.agents/skills` across
// every enumerated file (coverage by enumeration, no allowlist). Fails with an actionable
// "regenerate via RefreshSurfaceBaselines" diagnostic, never a bare "A and B differ".
let runSkillSyncGate (model: BuildModel) =
    let canonical = enumerateSkillTree model.RepositoryRoot FS.Skia.UI.Build.SkillTreeGen.canonicalRoot
    let derived = enumerateSkillTree model.RepositoryRoot FS.Skia.UI.Build.SkillTreeGen.derivedRoot
    let plan = FS.Skia.UI.Build.SkillSync.planFromCanonical canonical
    let currency = FS.Skia.UI.Build.SkillSync.currency plan derived
    let report = FS.Skia.UI.Build.SkillSync.renderReport plan currency
    let reportPath = path [ model.ReadinessDir; "skill-sync-check.md" ]
    let logPath = path [ model.LogDir; "skill-sync-check.txt" ]
    ensureParent reportPath
    File.WriteAllText(reportPath, report)
    ensureParent logPath
    File.WriteAllText(logPath, report)

    if not (FS.Skia.UI.Build.SkillSync.isCurrent currency) then
        failwith (FS.Skia.UI.Build.SkillSync.renderFailureMessage currency)

// Feature 044 — RefreshSurfaceBaselines regeneration edge (US1). Plan the derived tree
// from canonical, write each derived file's bytes + the provenance manifest, and remove
// any orphan derived file with no canonical source so the mirror stays exact.
let regenerateSkillTree (model: BuildModel) =
    let root = model.RepositoryRoot
    let canonical = enumerateSkillTree root FS.Skia.UI.Build.SkillTreeGen.canonicalRoot
    let plan = FS.Skia.UI.Build.SkillSync.planFromCanonical canonical

    let toFull (rel: string) = path (root :: (rel.Split('/') |> List.ofArray))

    let expected =
        plan.ManifestRelPath :: (plan.Entries |> List.map (fun e -> e.DerivedRelPath))
        |> List.map (fun rel -> (toFull rel).Replace('\\', '/'))
        |> Set.ofList

    // Write the manifest + every derived file (byte-identical to canonical, FR-003).
    let manifestFull = toFull plan.ManifestRelPath
    ensureParent manifestFull
    File.WriteAllBytes(manifestFull, plan.ManifestBytes)

    for entry in plan.Entries do
        let full = toFull entry.DerivedRelPath
        ensureParent full
        File.WriteAllBytes(full, entry.Bytes)

    // Remove orphan derived files (a canonical skill removed upstream must vanish here),
    // then prune any directory left empty by that removal so no stale skill dir lingers.
    let derivedFullRoot = toFull FS.Skia.UI.Build.SkillTreeGen.derivedRoot

    if Directory.Exists derivedFullRoot then
        for existing in Directory.EnumerateFiles(derivedFullRoot, "*", SearchOption.AllDirectories) do
            if not (expected.Contains(existing.Replace('\\', '/'))) then
                File.Delete existing

        for dir in Directory.EnumerateDirectories(derivedFullRoot, "*", SearchOption.AllDirectories) |> Seq.sortDescending do
            if Directory.Exists dir && Seq.isEmpty (Directory.EnumerateFileSystemEntries dir) then
                Directory.Delete dir

// Feature 044 — RefreshSurfaceBaselines regeneration edge (US3). Extract the principle
// fragments from the constitution and splice them into each governed template's BEGIN/END
// GENERATED regions, preserving every out-of-marker byte (FR-010).
let regenerateConstitutionFragments (model: BuildModel) =
    let constitutionPath = repoRelPath model.RepositoryRoot constitutionRelPath

    if not (File.Exists constitutionPath) then
        failwithf "RefreshSurfaceBaselines: %s is missing — cannot derive principle fragments (Principle VII)." constitutionRelPath

    let fragments = ConstitutionFragments.extract (File.ReadAllText constitutionPath)

    for (relTemplate, _ids) in constitutionTemplateRegions do
        let templatePath = repoRelPath model.RepositoryRoot relTemplate

        if not (File.Exists templatePath) then
            failwithf "RefreshSurfaceBaselines: %s is missing — cannot splice constitution fragments (Principle VII)." relTemplate

        let spliced = ConstitutionFragments.splice fragments (File.ReadAllText templatePath)
        File.WriteAllText(templatePath, spliced)

// Feature 042 (FR-002a, research R2): the git union-diff is read here at the `Route`
// interpreter edge so the Routing selector stays pure and unit-testable without git.
let routeGitCapture root (arguments: string) =
    try
        let startInfo = System.Diagnostics.ProcessStartInfo("git", arguments)
        startInfo.WorkingDirectory <- root
        startInfo.RedirectStandardOutput <- true
        startInfo.RedirectStandardError <- true
        startInfo.UseShellExecute <- false

        use proc = System.Diagnostics.Process.Start startInfo
        let stdout = proc.StandardOutput.ReadToEnd()
        let stderr = proc.StandardError.ReadToEnd()
        proc.WaitForExit() |> ignore

        if proc.ExitCode = 0 then Ok stdout else Error(stderr.Trim())
    with ex ->
        Error ex.Message

let routeWorkingTreePaths (porcelain: string) =
    porcelain.Replace("\r\n", "\n").Split('\n')
    |> Array.toList
    |> List.collect (fun line ->
        let trimmed = line.TrimEnd()

        if trimmed.Length <= 3 then
            []
        else
            let payload = trimmed.Substring(3)
            // `git status --porcelain` renders renames as "old -> new"; take the new path.
            let arrow = payload.IndexOf(" -> ", StringComparison.Ordinal)
            if arrow >= 0 then [ payload.Substring(arrow + 4) ] else [ payload ])

let runRouteSelection root =
    let argv = System.Environment.GetCommandLineArgs() |> Array.toList
    let enforce = argv |> List.exists (fun arg -> arg = "--enforce")

    let developerClass =
        if argv |> List.exists (fun arg -> arg = "consumer-agent") then
            Routing.ConsumerAgent
        else
            Routing.FrameworkAuthor

    let mergeBaseAgainst ref =
        match routeGitCapture root (sprintf "merge-base HEAD %s" ref) with
        | Ok value when value.Trim() <> "" -> Some(value.Trim())
        | _ -> None

    let mergeBase =
        // Trunk is `main` (renamed from `master`); fall back to `master` for old checkouts.
        match mergeBaseAgainst "main" with
        | Some _ as found -> found
        | None ->
            match mergeBaseAgainst "master" with
            | Some _ as found -> found
            | None ->
                printfn "Route: could not resolve 'git merge-base HEAD main' (or master); using working-tree changes only (no branch baseline)."
                None

    let committedPaths =
        match mergeBase with
        | Some baseCommit ->
            match routeGitCapture root (sprintf "diff --name-only %s...HEAD" baseCommit) with
            | Ok value -> value.Replace("\r\n", "\n").Split('\n') |> Array.toList
            | Error message ->
                printfn "Route: 'git diff' against the merge-base failed (%s); continuing with working-tree changes." message
                []
        | None -> []

    let workingPaths =
        match routeGitCapture root "status --porcelain --untracked-files=all" with
        | Ok value -> routeWorkingTreePaths value
        | Error message ->
            printfn "Route: 'git status' failed (%s)." message
            []

    let changedPaths =
        committedPaths @ workingPaths
        |> List.map (fun p -> p.Trim().Trim('"'))
        |> List.filter (fun p -> p <> "")
        |> List.distinct

    let selection =
        Routing.selectForFeature developerClass featureId { Routing.Diff.ChangedPaths = changedPaths }

    printfn "%s" (Routing.renderSelection selection)

    if enforce then
        let present =
            selection.ExpectedArtifacts
            |> List.filter (fun artifact -> File.Exists(path [ root; artifact ]))
            |> Set.ofList

        let missing = Routing.unmetArtifacts present selection

        if not (List.isEmpty missing) then
            failwith (Routing.enforceDiagnostic selection missing)

// Feature 043 (FR-009/FR-012, Principle IV): the EvidenceGraph/EvidenceAudit gates compute
// the task DAG and merge-gate audit in-process via FS.Skia.UI.Build.Evidence. Every read
// (tasks.md, tasks.deps.yml, readiness files, the skill registry, the git diff) and every
// artifact write stays at this interpreter edge so the engine itself performs no I/O.
let private evidenceReadAll p = if File.Exists p then File.ReadAllText p else ""

let buildEvidenceInputs (model: BuildModel) (unifiedDiff: string) : FS.Skia.UI.Build.Evidence.EvidenceInputs =
    let featDir = model.FeatureDir
    let readinessDir = model.ReadinessDir
    let repoRoot = model.RepositoryRoot
    let featureName = Path.GetFileName(featDir.TrimEnd('/', '\\'))
    let readinessFiles =
        if Directory.Exists readinessDir then
            Directory.GetFiles(readinessDir, "*", SearchOption.AllDirectories)
            |> Array.map (fun p -> p.Substring(readinessDir.Length + 1).Replace('\\', '/'), File.ReadAllText p)
            |> List.ofArray
        else
            []
    let featureText =
        [ "spec.md"; "plan.md"; "tasks.md" ]
        |> List.map (fun n -> evidenceReadAll (Path.Combine(featDir, n)))
        |> String.concat "\n"
    let auditStatusFiles =
        readinessFiles
        |> List.filter (fun (rel, c) ->
            rel.EndsWith ".md"
            && c.Contains "```audit-status"
            && not (rel.ToLowerInvariant().StartsWith "audit-fixtures/")
            && not (rel.ToLowerInvariant().StartsWith "audit-rejections/"))
    let slePath = Path.Combine(readinessDir, "skill-loading-evidence.md")
    { FeatureName = featureName
      TasksMd = evidenceReadAll (Path.Combine(featDir, "tasks.md"))
      DepsYml = evidenceReadAll (Path.Combine(featDir, "tasks.deps.yml"))
      Registry = FS.Skia.UI.Build.Evidence.SkillRegistry.build repoRoot
      SkillLoadingEvidence = (if File.Exists slePath then Some(File.ReadAllText slePath) else None)
      ResolvedExists = (fun p -> File.Exists(if Path.IsPathRooted p then p else Path.Combine(repoRoot, p)))
      Canonicalize = (fun p -> Path.GetFullPath(if Path.IsPathRooted p then p else Path.Combine(repoRoot, p)))
      RecordedFeature = Some featureName
      Scan = { ReadinessDir = readinessDir; FeatureText = featureText; ReadinessFiles = readinessFiles }
      AuditStatusFiles = auditStatusFiles
      PatternsYml = evidenceReadAll (Path.Combine(repoRoot, ".specify/extensions/evidence/audit-patterns.yml"))
      UnifiedDiff = unifiedDiff }

let private evidenceWrite (p: string) (content: string) =
    ensureParent p
    File.WriteAllText(p, content)

let runEvidenceGraphCheck (model: BuildModel) =
    let inputs = buildEvidenceInputs model ""
    let gr, arts = FS.Skia.UI.Build.Evidence.Engine.runGraph inputs
    evidenceWrite (path [ model.ReadinessDir; "task-graph.json" ]) arts.TaskGraphJson
    evidenceWrite (path [ model.ReadinessDir; "task-graph.md" ]) arts.TaskGraphMd
    let status =
        match gr.Verdict with
        | FS.Skia.UI.Build.Evidence.GraphVerdict.Ok -> "ok"
        | _ -> "error"
    evidenceWrite
        (path [ model.LogDir; "evidence-graph.txt" ])
        (sprintf "=== speckit.evidence.graph (in-process) ===\nfeature: %s\ntasks: %d\nverdict: %s\n" inputs.FeatureName (List.length gr.Tasks) status)
    match gr.Verdict with
    | FS.Skia.UI.Build.Evidence.GraphVerdict.Error ->
        failwithf "Evidence graph validation failed (%d errors); see %s" (List.length gr.Errors) (path [ model.ReadinessDir; "task-graph.md" ])
    | _ -> ()

let private resolveBaseRef root =
    let hasRef name =
        match routeGitCapture root (sprintf "show-ref --verify --quiet refs/heads/%s" name) with
        | Ok _ -> true
        | Error _ -> false
    if hasRef "main" then "main"
    elif hasRef "master" then "master"
    else "HEAD~1"

let runEvidenceAuditCheck root (model: BuildModel) =
    let baseRef = resolveBaseRef root
    let mergeBase =
        match routeGitCapture root (sprintf "merge-base %s HEAD" baseRef) with
        | Ok s when s.Trim() <> "" -> s.Trim()
        | _ -> baseRef
    let unifiedDiff =
        match routeGitCapture root (sprintf "diff %s --unified=0" mergeBase) with
        | Ok s -> s
        | Error _ -> ""
    let inputs = buildEvidenceInputs model unifiedDiff
    let gr, graphArts = FS.Skia.UI.Build.Evidence.Engine.runGraph inputs
    evidenceWrite (path [ model.ReadinessDir; "task-graph.json" ]) graphArts.TaskGraphJson
    evidenceWrite (path [ model.ReadinessDir; "task-graph.md" ]) graphArts.TaskGraphMd
    match gr.Verdict with
    | FS.Skia.UI.Build.Evidence.GraphVerdict.Error ->
        failwithf "Evidence graph compute failed; see %s" (path [ model.ReadinessDir; "task-graph.md" ])
    | _ ->
        let res, arts = FS.Skia.UI.Build.Evidence.Engine.runAudit inputs
        let r = model.ReadinessDir
        evidenceWrite (path [ r; "seh-audit-summary.json" ]) arts.SehAuditSummary
        evidenceWrite (path [ r; "readiness-contract-hits.json" ]) arts.ReadinessContractHits
        evidenceWrite (path [ r; "persistent-launch-hits.json" ]) arts.PersistentLaunchHits
        evidenceWrite (path [ r; "persistent-gui-runtime-hits.json" ]) arts.PersistentGuiRuntimeHits
        evidenceWrite (path [ r; "window-visibility-hits.json" ]) arts.WindowVisibilityHits
        evidenceWrite (path [ r; "audit-status-hits.json" ]) arts.AuditStatusHits
        evidenceWrite (path [ r; "diff-scan-hits.json" ]) arts.DiffScanHits
        let verdictStr =
            match res.Verdict with
            | FS.Skia.UI.Build.Evidence.AuditVerdict.Pass -> "PASS"
            | _ -> "FAIL"
        let log =
            sprintf
                "=== speckit.evidence.audit (in-process) ===\nfeature: %s\nverdict=%s\nreal-tasks=%d\naccepted-seh-tasks=%d\nunaccepted-synthetic-tasks=%d\nauto-synthetic-tasks=%d\nlate-seh-tasks=%d\ndiff-scan-hits=%d\nreadiness-contract-hits=%d\npersistent-launch-hits=%d\npersistent-gui-runtime-hits=%d\nwindow-visibility-hits=%d\naudit-status-hits=%d\ntotal-blockers=%d\n"
                inputs.FeatureName verdictStr res.RealTasks (List.length res.SehSummary.AcceptedSehTasks)
                (List.length res.SehSummary.UnacceptedSyntheticTasks) (List.length res.SehSummary.AutoSyntheticTasks)
                (List.length res.SehSummary.LateSehTasks) res.DiffBlocking res.ReadinessContract res.PersistentLaunch
                res.PersistentGuiRuntime res.WindowVisibility res.AuditStatus res.TotalBlockers
        evidenceWrite (path [ model.LogDir; "evidence-audit.txt" ]) log
        match res.Verdict with
        | FS.Skia.UI.Build.Evidence.AuditVerdict.Fail ->
            failwithf "Evidence audit FAIL (%d blockers); see %s" res.TotalBlockers (path [ model.LogDir; "evidence-audit.txt" ])
        | _ -> ()

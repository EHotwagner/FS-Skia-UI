namespace FS.Skia.UI.Build.Evidence

[<NoEquality; NoComparison>]
type EvidenceInputs =
    { FeatureName: string
      TasksMd: string
      DepsYml: string
      Registry: SkillRegistry
      SkillLoadingEvidence: string option
      ResolvedExists: string -> bool
      Canonicalize: string -> string
      RecordedFeature: string option
      Scan: ScanInput
      AuditStatusFiles: (string * string) list
      PatternsYml: string
      // FR-009: the resolved diff-scan base ref (default branch), threaded so
      // DiffScanResult.BaseRef and the audit summary report it instead of null.
      BaseRef: string option
      UnifiedDiff: string
      // Feature 087 (FR-008): the durable accepted-deferral records parsed from
      // readiness/synthetic-evidence.json at the interpreter edge.
      AcceptedDeferrals: AcceptedDeferral list }

type GraphArtifacts =
    { TaskGraphJson: string
      TaskGraphMd: string }

type AuditArtifacts =
    { TaskGraphJson: string
      TaskGraphMd: string
      AuditCounts: string
      SehAuditSummary: string
      ReadinessContractHits: string
      PersistentLaunchHits: string
      PersistentGuiRuntimeHits: string
      WindowVisibilityHits: string
      AuditStatusHits: string
      DiffScanHits: string }

module Engine =

    let private computeGraph (inputs: EvidenceInputs) : GraphResult =
        let errors = ResizeArray<string>()
        let warnings = ResizeArray<string>()

        let parsed = TaskParser.parse inputs.TasksMd
        errors.AddRange parsed.Errors
        let deps = DepsParser.parse inputs.DepsYml
        errors.AddRange deps.Errors

        match inputs.RecordedFeature with
        | Some rf when rf <> inputs.FeatureName ->
            warnings.Add(
                sprintf
                    "recorded-feature-vs-scanned mismatch: .specify/feature.json records '%s' but the audit scanned '%s'"
                    rf inputs.FeatureName)
        | _ -> ()

        if List.isEmpty parsed.Tasks then
            errors.Add(
                sprintf
                    "resolved feature '%s' has an empty or unparseable task file (tasks.md); refusing to report a stub task count"
                    inputs.FeatureName)

        let mutable tasks = parsed.Tasks
        if not (List.isEmpty parsed.Tasks) then
            let merge = Audit.validateAndMerge inputs.Registry tasks deps
            errors.AddRange merge.Errors
            tasks <- merge.Tasks
            let sle =
                Audit.validateSkillLoadingEvidence tasks inputs.Registry.Skills inputs.SkillLoadingEvidence
                    inputs.ResolvedExists inputs.Canonicalize
            errors.AddRange sle
            warnings.AddRange inputs.Registry.Warnings

        let g = Graph.ofTasks tasks
        let mutable cycles : string list list = []
        let mutable resolved : ResolvedTask list = []
        let mutable rootCause : Map<string, string list> = Map.empty

        if not (List.isEmpty tasks) && errors.Count = 0 then
            cycles <- Graph.detectCycles g
            if List.isEmpty cycles then
                let order = Graph.topoSort g
                if List.length order <> List.length tasks then
                    errors.Add(
                        sprintf
                            "topological sort incomplete: expected %d tasks, got %d. A cycle may have been missed."
                            (List.length tasks) (List.length order))
                else
                    let r, rc = Graph.propagate g order
                    resolved <- r
                    rootCause <- rc

        let finalResolved =
            if not (List.isEmpty resolved) then
                resolved
            else
                tasks
                |> List.sortBy (fun t -> t.Id)
                |> List.map (fun t -> { Task = t; Effective = DeclaredEff t.Declared; RootCause = [] })

        let verdict = if errors.Count = 0 && List.isEmpty cycles then Ok else Error

        { FeatureName = inputs.FeatureName
          Verdict = verdict
          Errors = List.ofSeq errors
          Warnings = List.ofSeq warnings
          Cycles = cycles
          Tasks = finalResolved
          RootCause = rootCause }

    let runGraph (inputs: EvidenceInputs) : GraphResult * GraphArtifacts =
        let gr = computeGraph inputs
        gr,
        { TaskGraphJson = Render.taskGraphJson gr
          TaskGraphMd = Render.taskGraphMd inputs.Registry gr }

    let runAudit (inputs: EvidenceInputs) : AuditResult * AuditArtifacts =
        let gr = computeGraph inputs
        let resolved = gr.Tasks
        let seh = Audit.sehSummary resolved
        let rc = Scans.readinessContract inputs.Scan
        let pl = Scans.persistentLaunch inputs.Scan
        let pg = Scans.persistentGui inputs.Scan
        let wv = Scans.windowVisibility inputs.Scan
        let asScan = StatusRegion.scan inputs.AuditStatusFiles
        let diff = DiffScan.scan inputs.BaseRef inputs.PatternsYml inputs.UnifiedDiff

        let result =
            Audit.verdict resolved seh inputs.AcceptedDeferrals (List.length diff.Blocking) rc.BlockingCount pl.BlockingCount pg.BlockingCount
                wv.BlockingCount asScan.BlockingCount

        let auditStatusBlockingLines =
            inputs.AuditStatusFiles
            |> List.map (fun (p, t) -> StatusRegion.scanText p t)
            |> List.filter (fun s -> s.Blocking)
            |> List.map (fun s -> sprintf "BLOCK %s:" s.Path)

        result,
        { TaskGraphJson = Render.taskGraphJson gr
          TaskGraphMd = Render.taskGraphMd inputs.Registry gr
          AuditCounts =
            Render.auditCounts inputs.FeatureName result.RealTasks (List.length seh.AcceptedSehTasks)
                (List.length seh.UnacceptedSyntheticTasks) (List.length seh.AutoSyntheticTasks) (List.length seh.LateSehTasks)
          SehAuditSummary = Render.sehAuditSummaryJson result
          ReadinessContractHits = Render.scanHitsJson "readiness-contract" rc
          PersistentLaunchHits = Render.scanHitsJson "persistent-launch" pl
          PersistentGuiRuntimeHits = Render.scanHitsJson "persistent-gui-runtime" pg
          WindowVisibilityHits = Render.scanHitsJson "window-visibility" wv
          AuditStatusHits = Render.auditStatusJson (inputs.AuditStatusFiles |> List.map fst) auditStatusBlockingLines
          DiffScanHits = Render.diffScanJson diff }

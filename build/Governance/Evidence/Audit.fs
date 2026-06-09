namespace FS.Skia.UI.Build.Evidence

open System
open System.Collections.Generic
open System.Text.RegularExpressions

type MergeResult =
    { Tasks: TaskRecord list
      Errors: string list }

type SehSummary =
    { AcceptedSehTasks: string list
      UnacceptedSyntheticTasks: string list
      AutoSyntheticTasks: string list
      LateSehTasks: string list
      Diagnostics: (string * string * string * string) list }

// `AuditVerdict` is single-sourced in EvidenceFormatSchema (feature 087, FR-007);
// it is the three-state Pass | PassWithAcceptedDeferrals | Fail.

type AuditResult =
    { Verdict: AuditVerdict
      SehSummary: SehSummary
      // Feature 087 (FR-008): the accepted deferrals that converted otherwise
      // unaccepted synthetic tasks into accepted ones, plus the separated counts.
      AcceptedDeferrals: AcceptedDeferral list
      AcceptedSyntheticCount: int
      UnacceptedSyntheticCount: int
      RealTasks: int
      TotalBlockers: int
      DiffBlocking: int
      ReadinessContract: int
      PersistentLaunch: int
      PersistentGuiRuntime: int
      WindowVisibility: int
      AuditStatus: int }

module Audit =

    // --- gated-evidence ownership vocabulary (feature 059, FR-010) ----------
    //
    // Replaces the title-trigger capability matcher (`capabilityTriggerGroups` /
    // `expectedCapabilityMatches`, removed in 059). Task titles are now fully
    // free-form and are NEVER scanned. A task declares the gated evidence it owns
    // through the explicit `owns:` field in tasks.deps.yml; each owns value implies
    // a required skill that MUST appear in that task's declared skillist. The
    // vocabulary is a closed set — unknown values are a directive error.

    // Feature 062 (FR-006): exposed (was private) so the generated
    // docs/skillist-reference.md single-sources the closed owns:→implied-skill
    // table from this same enforcing list (it cannot drift).
    let ownsVocabulary : (string * string) list =
        [ "graph-validation", "speckit-evidence-graph"
          "evidence-audit", "speckit-evidence-audit"
          "task-generation", "speckit-tasks"
          "implementation-loading", "speckit-implement"
          "constitution", "speckit-constitution" ]

    let private ownsAllowed = ownsVocabulary |> List.map fst |> String.concat ", "

    let private skillPrerequisites : (string * string list) list =
        [ "speckit-evidence-audit", [ "speckit-evidence-graph" ]
          "speckit-implement", [ "speckit-tasks" ] ]

    // --- validate and merge -------------------------------------------------

    let validateAndMerge (registry: SkillRegistry) (tasks: TaskRecord list) (deps: DepsModel) : MergeResult =
        let errors = ResizeArray<string>()
        let taskMap = tasks |> List.map (fun t -> t.Id, t) |> Map.ofList
        let mdIds = tasks |> List.map (fun t -> t.Id) |> Set.ofList
        let ymlIds = deps.Order |> Set.ofList

        let onlyMd = Set.difference mdIds ymlIds |> Set.toList |> List.sort
        let onlyYml = Set.difference ymlIds mdIds |> Set.toList |> List.sort
        // Feature 059 (FR-007): when tasks.deps.yml failed to parse a single task
        // (empty Order) but tasks.md has tasks, the root cause is structural — the
        // missing `tasks:` wrapper, already reported standalone by DepsParser. Do not
        // bury that directive under one "no key" error per task line (SC-003).
        let depsStructurallyEmpty = List.isEmpty deps.Order && not (List.isEmpty tasks)
        if not depsStructurallyEmpty then
            for tid in onlyMd do
                errors.Add(sprintf "tasks.md declares %s but tasks.deps.yml has no key for it" tid)
        for tid in onlyYml do
            errors.Add(sprintf "tasks.deps.yml declares %s but tasks.md has no task line" tid)

        // accumulators for mutation
        let skillistOf = Dictionary<string, string list>()
        let explicitDepsOf = Dictionary<string, string list>()
        let assessmentsOf = Dictionary<string, ResizeArray<SkillAssessment>>()
        for t in tasks do
            assessmentsOf.[t.Id] <- ResizeArray<SkillAssessment>()

        let metaOrder = deps.Order

        // Loop A: dangling / self / bare-list / missing-deps
        for tid in metaOrder do
            let meta = deps.Map.[tid]
            if taskMap.ContainsKey tid then
                if meta.LegacyBareList then
                    errors.Add(
                        sprintf
                            "tasks.deps.yml: %s: existing bare-list metadata must be migrated to object form with deps and skillist"
                            tid)
                let dlist =
                    match meta.Deps with
                    | None ->
                        errors.Add(sprintf "%s: missing deps field in tasks.deps.yml" tid)
                        []
                    | Some d -> d
                for d in dlist do
                    if not (taskMap.ContainsKey d) then
                        errors.Add(sprintf "tasks.deps.yml: %s depends on %s, which does not exist" tid d)
                    elif d = tid then
                        errors.Add(sprintf "tasks.deps.yml: %s depends on itself" tid)

        // registry warnings folded into errors (Python validate_and_merge)
        for w in registry.Warnings do
            errors.Add w

        // Loop B: skillist merge, mirror, resolution, assessments, prereq order
        for tid in metaOrder do
            let meta = deps.Map.[tid]
            if taskMap.ContainsKey tid then
                let task = taskMap.[tid]
                match meta.Skillist with
                | None -> errors.Add(sprintf "%s: missing structured skillist in tasks.deps.yml" tid)
                | Some sk -> skillistOf.[tid] <- sk

                match task.SkillistMirror with
                | None ->
                    // Feature 044 (US2): the derived [skillist: …] view is absent. Absent
                    // metadata is invalid and is reported, never silently inserted (FR-007).
                    errors.Add(
                        sprintf
                            "%s: missing tasks.md [skillist: …] view; regenerate via ./fake.sh build -t RefreshSurfaceBaselines"
                            tid)
                | Some mirror ->
                    match meta.Skillist with
                    | Some sk when mirror <> sk ->
                        // Feature 044 (US2): reframed from a symmetric peer complaint into an
                        // asymmetric currency diagnostic — tasks.deps.yml skillist: is canonical
                        // and the tasks.md [skillist: …] annotation is the derived view. The token
                        // render is delegated to SkillistView. This engine only reads the ACTIVE
                        // feature, so historical feature directories are never re-derived
                        // (FR-007, SC-004); the compared value (mirror <> sk) is unchanged.
                        errors.Add(FS.Skia.UI.Build.SkillistView.staleDiagnostic tid sk)
                    | _ -> ()

                let declaredSkillist = defaultArg meta.Skillist []
                for skillId in declaredSkillist do
                    let matches = defaultArg (registry.Skills.TryFind skillId) []
                    if List.isEmpty matches then
                        match registry.DirectoryAliases.TryFind skillId with
                        | Some(acceptedId, path) ->
                            errors.Add(
                                sprintf
                                    "%s: declared skill %s is a directory-like name for %s; accepted declared id is %s"
                                    tid skillId path acceptedId)
                        | None ->
                            errors.Add(sprintf "%s: declared skill %s is not readable or not registered" tid skillId)
                    elif List.length matches > 1 then
                        errors.Add(sprintf "%s: declared skill %s is ambiguous: %s" tid skillId (String.concat ", " matches))

                // Feature 059 (FR-009/FR-010): evidence ownership is structured, not
                // inferred from the title. Each declared `owns:` value must be in the
                // closed vocabulary and must carry its implied skill in the skillist.
                let declaredOwns = defaultArg meta.Owns []
                let mutable ownsAssessmentCount = 0
                for ownsValue in declaredOwns do
                    match ownsVocabulary |> List.tryFind (fun (v, _) -> v = ownsValue) with
                    | None ->
                        errors.Add(sprintf "%s: unknown owns value '%s'; allowed: %s" tid ownsValue ownsAllowed)
                    | Some(_, impliedSkill) ->
                        ownsAssessmentCount <- ownsAssessmentCount + 1
                        let present = List.contains impliedSkill declaredSkillist
                        if not present then
                            errors.Add(
                                sprintf
                                    "%s: owns %s requires skill %s in skillist; declared_skillist=[%s]"
                                    tid ownsValue impliedSkill (String.concat ", " declaredSkillist))
                        assessmentsOf.[tid].Add
                            { TaskId = tid
                              DeclaredSkillist = declaredSkillist
                              CandidateSkillId = Some impliedSkill
                              MatchedSignals = [ sprintf "owns:%s" ownsValue ]
                              Confidence = "high"
                              Ambiguity = None
                              ReviewerDisposition = (if present then Some "accepted" else None)
                              Diagnostic =
                                sprintf "%s: owns %s requires skill %s; trigger_group=owns; matched_trigger=owns:%s" tid ownsValue impliedSkill ownsValue }

                if ownsAssessmentCount = 0 then
                    assessmentsOf.[tid].Add
                        { TaskId = tid
                          DeclaredSkillist = declaredSkillist
                          CandidateSkillId = None
                          MatchedSignals = []
                          Confidence = "none"
                          Ambiguity = None
                          ReviewerDisposition = (if List.isEmpty declaredSkillist then Some "accepted-empty" else Some "declared")
                          Diagnostic = sprintf "%s: skillist trusted as declared; no owns-based capability requirement" tid }

                let listed = declaredSkillist
                let positions = listed |> List.mapi (fun i s -> s, i) |> Map.ofList
                for (skillId, prerequisites) in skillPrerequisites do
                    match positions.TryFind skillId with
                    | Some pos ->
                        for prereq in prerequisites do
                            match positions.TryFind prereq with
                            | Some ppos when ppos > pos ->
                                errors.Add(sprintf "%s: skillist order places %s before prerequisite %s" tid skillId prereq)
                            | _ -> ()
                    | None -> ()

        // Loop C: explicit_deps
        for tid in metaOrder do
            let meta = deps.Map.[tid]
            if taskMap.ContainsKey tid then
                explicitDepsOf.[tid] <- (defaultArg meta.Deps []) |> List.filter taskMap.ContainsKey

        let updated =
            tasks
            |> List.map (fun t ->
                { t with
                    Skillist = (match skillistOf.TryGetValue t.Id with | true, v -> v | _ -> t.Skillist)
                    ExplicitDeps = (match explicitDepsOf.TryGetValue t.Id with | true, v -> v | _ -> t.ExplicitDeps)
                    SkillMatchAssessments =
                        (match assessmentsOf.TryGetValue t.Id with | true, v -> List.ofSeq v | _ -> t.SkillMatchAssessments) })

        { Tasks = updated; Errors = List.ofSeq errors }

    // --- skill-loading evidence --------------------------------------------

    type private EvidenceRow =
        { TaskId: string
          DeclaredSkillId: string
          ResolvedSkillPath: string
          LoadResult: string
          LoadedAt: string
          WorkStartedAt: string
          EvidencePath: string
          Exception: string
          // Feature 087 (FR-010): the 9th column. "" when the row is a legacy
          // 8-column row (lenient); a present value must be captured|asserted.
          Provenance: string }

    let private rangeRe = Regex(@"\bT\d{3,4}\s*-\s*T\d{3,4}\b", RegexOptions.Compiled)
    let private twoIdsRe = Regex(@"\bT\d{3,4}\b.*\bT\d{3,4}\b", RegexOptions.Compiled)
    let private andOrRe = Regex(@"\b(and|or)\b", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)

    let private parseEvidence (text: string) : EvidenceRow list * string list =
        let rows = ResizeArray<EvidenceRow>()
        let errors = ResizeArray<string>()
        for line in Lines.split text do
            let trimmed = line.Trim()
            if not (trimmed.StartsWith "|") || trimmed.Contains "---" || trimmed.Contains "Task | Skill id" then
                ()
            else
                let cells =
                    trimmed.Trim('|').Split('|')
                    |> Array.map (fun c -> c.Trim().Trim('`'))
                    |> List.ofArray
                if List.length cells >= 8 then
                    let taskId = cells.[0]
                    let skillId = cells.[1]
                    if rangeRe.IsMatch taskId then
                        errors.Add(sprintf "collapsed task range row is invalid: %s" taskId)
                    if taskId.Contains "," || twoIdsRe.IsMatch taskId then
                        errors.Add(sprintf "multi-task prose row is invalid: %s" taskId)
                    if skillId.Contains "," || andOrRe.IsMatch skillId then
                        errors.Add(sprintf "multi-skill prose row is invalid: %s" skillId)
                    rows.Add
                        { TaskId = taskId
                          DeclaredSkillId = skillId
                          ResolvedSkillPath = cells.[2]
                          LoadResult = cells.[3]
                          LoadedAt = cells.[4]
                          WorkStartedAt = cells.[5]
                          EvidencePath = cells.[6]
                          Exception = cells.[7]
                          Provenance = (if List.length cells >= 9 then cells.[8] else "") }
        List.ofSeq rows, List.ofSeq errors

    let private parseUtc (value: string) : DateTimeOffset option =
        match DateTimeOffset.TryParse(value, Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.RoundtripKind) with
        | true, v -> Some v
        | _ -> None

    let validateSkillLoadingEvidence
        (tasks: TaskRecord list)
        (skills: Map<string, string list>)
        (evidenceText: string option)
        (resolvedExists: string -> bool)
        (canonicalize: string -> string)
        : string list =
        let errors = ResizeArray<string>()
        let rows, rowErrors =
            match evidenceText with
            | Some t -> parseEvidence t
            | None -> [], []
        errors.AddRange rowErrors

        let rowByKey = Dictionary<string * string, EvidenceRow>()
        let duplicateKeys = HashSet<string * string>()
        for row in rows do
            let key = (row.TaskId, row.DeclaredSkillId)
            if rowByKey.ContainsKey key then duplicateKeys.Add key |> ignore
            else rowByKey.[key] <- row
        for (taskId, skillId) in duplicateKeys |> Seq.sort do
            errors.Add(
                sprintf "%s: duplicate skill-loading evidence row for %s; duplicate rows do not mask missing required rows" taskId skillId)

        let taskMap = tasks |> List.map (fun t -> t.Id, t) |> Map.ofList
        let expectedRows =
            [ for t in tasks do
                  if t.Declared = Done || t.Declared = Synthetic then
                      for skillId in t.Skillist do
                          yield (t.Id, skillId) ]

        let observed = rowByKey.Keys |> Set.ofSeq
        for (taskId, skillId) in expectedRows do
            if not (observed.Contains(taskId, skillId)) then
                errors.Add(sprintf "%s: declared skill %s has no pre-work load evidence" taskId skillId)

        for (taskId, skillId) in expectedRows do
            match taskMap.TryFind taskId with
            | Some task ->
                match rowByKey.TryGetValue((task.Id, skillId)) with
                | true, row ->
                    if row.LoadResult <> "loaded" && row.Exception = "" then
                        errors.Add(sprintf "%s: declared skill %s has incomplete reviewer exception" task.Id skillId)
                    let loadedAt = parseUtc row.LoadedAt
                    let workStartedAt = parseUtc row.WorkStartedAt
                    match loadedAt, workStartedAt with
                    | None, _
                    | _, None -> errors.Add(sprintf "%s: declared skill %s has invalid load/work timestamp" task.Id skillId)
                    | Some la, Some wa ->
                        if la = wa then
                            errors.Add(
                                sprintf "%s: skill %s equal timestamps are invalid; loaded_at must be earlier than work_started_at" task.Id skillId)
                        elif la > wa then
                            errors.Add(
                                sprintf
                                    "%s: skill %s loaded_at must be earlier than work_started_at (loaded_at=%s work_started_at=%s)"
                                    task.Id skillId row.LoadedAt row.WorkStartedAt)
                    let matches = defaultArg (skills.TryFind skillId) []
                    if not (resolvedExists row.ResolvedSkillPath) then
                        errors.Add(sprintf "%s: declared skill %s evidence path is unreadable: %s" task.Id skillId row.ResolvedSkillPath)
                    elif List.length matches <> 1 || canonicalize (List.head matches) <> canonicalize row.ResolvedSkillPath then
                        errors.Add(sprintf "%s: declared skill %s evidence path does not match resolved skill path" task.Id skillId)
                    // Feature 087 (FR-010): a present provenance column must be in
                    // the closed set; a legacy 8-column row (empty) is lenient.
                    if row.Provenance <> "" then
                        match EvidenceFormatSchema.parseProvenance row.Provenance with
                        | Some _ -> ()
                        | None ->
                            errors.Add(
                                sprintf "%s: declared skill %s has invalid provenance '%s'; expected captured|asserted" task.Id skillId row.Provenance)
                | _ -> ()
            | None -> ()

        List.ofSeq errors

    // Feature 087 (FR-010): the declared-but-unloaded (task, skill) gaps surfaced
    // AT IMPLEMENTATION TIME — for every task carrying a non-empty declared
    // skillist, REGARDLESS of [X]/[S] status — so a missing load is reported when
    // the declaring task is being implemented, not deferred to the [X] flip
    // (which is what `validateSkillLoadingEvidence` enforces only for Done/Synthetic).
    let skillLoadingGapsAtImplementation
        (tasks: TaskRecord list)
        (evidenceText: string option)
        : (string * string) list =
        let rows, _ =
            match evidenceText with
            | Some t -> parseEvidence t
            | None -> [], []
        let observed = rows |> List.map (fun r -> r.TaskId, r.DeclaredSkillId) |> Set.ofList
        [ for t in tasks do
              for skillId in t.Skillist do
                  if not (observed.Contains(t.Id, skillId)) then
                      yield (t.Id, skillId) ]

    // Feature 062 (FR-005): the skill-loading-evidence evidence-format schema text
    // (the 8-column row, the `loaded_at < work_started_at` ordering rule, and the
    // resolved `.agents/skills/<id>/SKILL.md` path), single-sourced from
    // EvidenceFormatSchema so the printed diagnostic cannot drift from the row
    // shape `validateSkillLoadingEvidence` enforces above. Printed when a
    // skill-loading-evidence error is present so the shape is recoverable without
    // decompiling FS.Skia.UI.Build.dll or copying a sibling project.
    let skillLoadingEvidenceSchemaText () : string =
        EvidenceFormatSchema.renderClass SkillLoadingEvidence

    // --- SEH summary + verdict ---------------------------------------------

    let private lateTerms = [ "implementation"; "readiness cleanup"; "after audit"; "after-failure"; "late" ]
    let private nonEligibleTerms =
        [ "convenience mock"; "incomplete integration"; "unavailable product capability"; "missing host support"
          "placeholder output"; "speed-only"; "ordinary in-memory"; "unsupported-host substitute" ]

    let sehSummary (resolved: ResolvedTask list) : SehSummary =
        let accepted = ResizeArray<string>()
        let unaccepted = ResizeArray<string>()
        let auto = ResizeArray<string>()
        let late = ResizeArray<string>()
        let diagnostics = ResizeArray<string * string * string * string>()
        for r in resolved do
            let t = r.Task
            let effective = Graph.effectiveString r.Effective
            let isSynthetic = effective = "synthetic" || effective = "auto-synthetic"
            let isAccepted = TaskParser.isAcceptedSeh t
            let designSource = t.Seh.DesignSource.ToLowerInvariant()
            let inputClass = t.Seh.SyntheticInputClass.ToLowerInvariant()
            let rationale = t.Seh.Rationale.ToLowerInvariant()
            let acceptanceStatus = t.Seh.AcceptanceStatus.ToLowerInvariant()
            if isAccepted then accepted.Add t.Id
            elif isSynthetic then unaccepted.Add t.Id
            if effective = "auto-synthetic" then auto.Add t.Id
            if t.Seh.Annotation || t.Seh.ApprovalLabel then
                let failedRules = ResizeArray<string>(t.Seh.Diagnostics)
                if lateTerms |> List.exists (fun term -> designSource.Contains term || acceptanceStatus.Contains term) then
                    failedRules.Add "late [SEH] classification"
                    late.Add t.Id
                if nonEligibleTerms |> List.exists (fun term -> inputClass.Contains term || rationale.Contains term) then
                    failedRules.Add "non-eligible synthetic evidence class"
                if failedRules.Count > 0 then
                    let rule = failedRules |> Seq.distinct |> Seq.sort |> String.concat "; "
                    let source = if t.Seh.DesignSource = "" then "(missing)" else t.Seh.DesignSource
                    diagnostics.Add(
                        t.Id, rule, source,
                        "Return to design/task generation and record valid [SEH] classification before implementation.")
        { AcceptedSehTasks = List.ofSeq accepted
          UnacceptedSyntheticTasks = List.ofSeq unaccepted
          AutoSyntheticTasks = List.ofSeq auto
          LateSehTasks = List.ofSeq late
          Diagnostics = List.ofSeq diagnostics }

    // Feature 087 (FR-008): parse the durable accepted-deferral records from
    // readiness/synthetic-evidence.json. `--accept-synthetic` writes these (with
    // the Constitution §V written justification); the verdict reads them. A record
    // with an empty taskId or empty justification is ignored (justification is
    // required). Tolerant: a missing file / malformed JSON yields no deferrals.
    let parseAcceptedDeferrals (jsonText: string option) : AcceptedDeferral list =
        match jsonText with
        | None -> []
        | Some text when String.IsNullOrWhiteSpace text -> []
        | Some text ->
            let getStr (el: System.Text.Json.JsonElement) (name: string) : string =
                match el.TryGetProperty name with
                | true, v when v.ValueKind = System.Text.Json.JsonValueKind.String ->
                    v.GetString() |> Option.ofObj |> Option.defaultValue ""
                | _ -> ""
            try
                use doc = System.Text.Json.JsonDocument.Parse text
                let mutable arr = Unchecked.defaultof<System.Text.Json.JsonElement>
                if doc.RootElement.TryGetProperty("acceptedDeferrals", &arr)
                   && arr.ValueKind = System.Text.Json.JsonValueKind.Array then
                    [ for el in arr.EnumerateArray() do
                          let taskId = getStr el "taskId"
                          let justification = getStr el "justification"
                          if taskId <> "" && justification <> "" then
                              yield
                                  { TaskId = taskId
                                    Justification = justification
                                    RealEvidencePath = getStr el "realEvidencePath"
                                    AwaitedHostCapability = getStr el "awaitedHostCapability" } ]
                else
                    []
            with _ -> []

    let verdict
        (resolved: ResolvedTask list)
        (seh: SehSummary)
        (acceptedDeferrals: AcceptedDeferral list)
        (diffBlocking: int)
        (readinessContract: int)
        (persistentLaunch: int)
        (persistentGuiRuntime: int)
        (windowVisibility: int)
        (auditStatus: int)
        : AuditResult =
        // An accepted deferral converts an otherwise-unaccepted synthetic task
        // into an accepted one. It can NEVER cover a blocking hit or an
        // invalid-SEH diagnostic (FR-011) — those are counted independently.
        let acceptedIds = acceptedDeferrals |> List.map (fun d -> d.TaskId) |> Set.ofList
        let acceptedSyntheticCount =
            seh.UnacceptedSyntheticTasks |> List.filter acceptedIds.Contains |> List.length
        let unacceptedSyntheticCount =
            seh.UnacceptedSyntheticTasks |> List.filter (acceptedIds.Contains >> not) |> List.length
        let invalidSeh = List.length seh.Diagnostics
        let blocking =
            invalidSeh + diffBlocking + readinessContract + persistentLaunch
            + persistentGuiRuntime + windowVisibility + auditStatus
        let total = unacceptedSyntheticCount + blocking
        let realTasks = resolved |> List.filter (fun r -> Graph.effectiveString r.Effective = "done") |> List.length
        // FR-007/FR-011: PassWithAcceptedDeferrals is reachable ONLY with zero
        // unaccepted synthetic AND zero blocking hits, and only when at least one
        // accepted deferral actually covered a synthetic task.
        let v =
            if total > 0 then Fail
            elif acceptedSyntheticCount > 0 then PassWithAcceptedDeferrals
            else Pass
        { Verdict = v
          SehSummary = seh
          AcceptedDeferrals = acceptedDeferrals
          AcceptedSyntheticCount = acceptedSyntheticCount
          UnacceptedSyntheticCount = unacceptedSyntheticCount
          RealTasks = realTasks
          TotalBlockers = total
          DiffBlocking = diffBlocking
          ReadinessContract = readinessContract
          PersistentLaunch = persistentLaunch
          PersistentGuiRuntime = persistentGuiRuntime
          WindowVisibility = windowVisibility
          AuditStatus = auditStatus }

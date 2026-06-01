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

type AuditVerdict =
    | Pass
    | Fail

type AuditResult =
    { Verdict: AuditVerdict
      SehSummary: SehSummary
      RealTasks: int
      TotalBlockers: int
      DiffBlocking: int
      ReadinessContract: int
      PersistentLaunch: int
      PersistentGuiRuntime: int
      WindowVisibility: int
      AuditStatus: int }

module Audit =

    // --- capability trigger groups (ported from compute-task-graph.py) ------

    let private capabilityTriggerGroups : (string * string * string list) list =
        [ "speckit-evidence-graph",
          "graph validation",
          [ "task graph"; "evidence graph"; "readiness validation"; "tasks.deps.yml"; "structured task metadata"
            "mirror mismatch"; "skillist field"; "skillist, list typing"; "obvious capability"
            "multi-skill dependency order"; "migration blocker"; "validator diagnostics"; "EvidenceGraph" ]
          "speckit-evidence-audit",
          "evidence audit",
          [ "evidence audit"; "diff-scan"; "synthetic propagation"; "readiness-blocking"; "EvidenceAudit" ]
          "speckit-tasks",
          "task generation",
          [ "/speckit.tasks"; "speckit.tasks"; "task-generation"; "task templates"; "tasks-template"
            "tasks template"; "generated task guidance"; "post-generation skill evaluation" ]
          "speckit-implement",
          "implementation loading",
          [ "/speckit.implement"; "speckit.implement"; "implementation-loading"; "implementation skill"
            "implementation command"; "load each"; "skill-load"; "before implementation" ]
          "speckit-constitution", "constitution", [ "constitution"; "constitutional" ] ]

    let private skillPrerequisites : (string * string list) list =
        [ "speckit-evidence-audit", [ "speckit-evidence-graph" ]
          "speckit-implement", [ "speckit-tasks" ] ]

    let private wordChar (c: char) =
        (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c = '.' || c = '_' || c = '/' || c = '-'

    let private tokenContext (text: string) (start: int) (endIdx: int) : string =
        let mutable left = start
        while left > 0 && wordChar text.[left - 1] do
            left <- left - 1
        let mutable right = endIdx
        while right < text.Length && wordChar text.[right] do
            right <- right + 1
        text.Substring(left, right - left)

    let private filenameRe = Regex(@"\.[A-Za-z0-9]{1,8}$", RegexOptions.Compiled)
    let private isFilenameContext (token: string) = filenameRe.IsMatch token

    let private triggerMatchesTitle (title: string) (trigger: string) : (string * string) option =
        let pattern = @"(?<![A-Za-z0-9])" + Regex.Escape(trigger) + @"(?![A-Za-z0-9])"
        let mutable found : (string * string) option = None
        for m in Regex.Matches(title, pattern, RegexOptions.IgnoreCase) do
            if found.IsNone then
                let token = tokenContext title m.Index (m.Index + m.Length)
                if not (isFilenameContext token) then found <- Some(trigger, token)
        found

    let expectedCapabilityMatches (title: string) : (string * string * string) list =
        [ for (skillId, group, triggers) in capabilityTriggerGroups do
              let firstMatch =
                  triggers
                  |> List.tryPick (fun trigger ->
                      match triggerMatchesTitle title trigger with
                      | Some(t, _) -> Some t
                      | None -> None)
              match firstMatch with
              | Some t -> yield (skillId, group, t)
              | None -> () ]

    let private completeReadinessRe = Regex(@"^Complete readiness notes", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)

    // --- validate and merge -------------------------------------------------

    let validateAndMerge (registry: SkillRegistry) (tasks: TaskRecord list) (deps: DepsModel) : MergeResult =
        let errors = ResizeArray<string>()
        let taskMap = tasks |> List.map (fun t -> t.Id, t) |> Map.ofList
        let mdIds = tasks |> List.map (fun t -> t.Id) |> Set.ofList
        let ymlIds = deps.Order |> Set.ofList

        let onlyMd = Set.difference mdIds ymlIds |> Set.toList |> List.sort
        let onlyYml = Set.difference ymlIds mdIds |> Set.toList |> List.sort
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
                | None -> errors.Add(sprintf "%s: missing tasks.md skillist mirror" tid)
                | Some mirror ->
                    match meta.Skillist with
                    | Some sk when mirror <> sk ->
                        errors.Add(
                            sprintf "%s: tasks.md mirror [%s] does not match tasks.deps.yml [%s]" tid
                                (String.concat ", " mirror) (String.concat ", " sk))
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

                let expected =
                    if completeReadinessRe.IsMatch task.Title then []
                    else expectedCapabilityMatches task.Title

                for (skillId, triggerGroup, matchedTrigger) in expected do
                    let disposition = if List.contains skillId declaredSkillist then Some "accepted" else None
                    assessmentsOf.[tid].Add
                        { TaskId = tid
                          DeclaredSkillist = declaredSkillist
                          CandidateSkillId = Some skillId
                          MatchedSignals = [ matchedTrigger ]
                          Confidence = "high"
                          Ambiguity = None
                          ReviewerDisposition = disposition
                          Diagnostic =
                            sprintf "%s: task text matches %s; trigger_group=%s; matched_trigger=%s" tid skillId triggerGroup matchedTrigger }
                    if not (List.contains skillId declaredSkillist) then
                        errors.Add(
                            sprintf
                                "%s: high-confidence skill match omitted declared skill %s; trigger_group=%s; matched_trigger=%s; declared_skillist=[%s]"
                                tid skillId triggerGroup matchedTrigger (String.concat ", " declaredSkillist))

                if List.isEmpty expected then
                    assessmentsOf.[tid].Add
                        { TaskId = tid
                          DeclaredSkillist = declaredSkillist
                          CandidateSkillId = None
                          MatchedSignals = []
                          Confidence = "none"
                          Ambiguity = None
                          ReviewerDisposition = (if List.isEmpty declaredSkillist then Some "accepted-empty" else Some "declared")
                          Diagnostic = sprintf "%s: no high-confidence capability signal detected" tid }

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
          Exception: string }

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
                          Exception = cells.[7] }
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
                | _ -> ()
            | None -> ()

        List.ofSeq errors

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

    let verdict
        (resolved: ResolvedTask list)
        (seh: SehSummary)
        (diffBlocking: int)
        (readinessContract: int)
        (persistentLaunch: int)
        (persistentGuiRuntime: int)
        (windowVisibility: int)
        (auditStatus: int)
        : AuditResult =
        let invalidSeh = List.length seh.Diagnostics
        let total =
            List.length seh.UnacceptedSyntheticTasks
            + invalidSeh + diffBlocking + readinessContract + persistentLaunch
            + persistentGuiRuntime + windowVisibility + auditStatus
        let realTasks = resolved |> List.filter (fun r -> Graph.effectiveString r.Effective = "done") |> List.length
        { Verdict = (if total = 0 then Pass else Fail)
          SehSummary = seh
          RealTasks = realTasks
          TotalBlockers = total
          DiffBlocking = diffBlocking
          ReadinessContract = readinessContract
          PersistentLaunch = persistentLaunch
          PersistentGuiRuntime = persistentGuiRuntime
          WindowVisibility = windowVisibility
          AuditStatus = auditStatus }

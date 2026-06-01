namespace FS.Skia.UI.Build.Evidence

open System.Collections.Generic
open System.Text.RegularExpressions

type DeclaredStatus =
    | Pending
    | Done
    | Synthetic
    | Failed
    | Skipped

type SkillAssessment =
    { TaskId: string
      DeclaredSkillist: string list
      CandidateSkillId: string option
      MatchedSignals: string list
      Confidence: string
      Ambiguity: string option
      ReviewerDisposition: string option
      Diagnostic: string }

type SehMetadata =
    { Annotation: bool
      ApprovalLabel: bool
      DesignSource: string
      SyntheticInputClass: string
      ExpectedErrorBehavior: string
      Rationale: string
      AcceptanceStatus: string
      Diagnostics: string list }

type TaskRecord =
    { Id: string
      Declared: DeclaredStatus
      Phase: int option
      Story: string option
      Tier: string option
      Parallel: bool
      LineNo: int
      Title: string
      SkillistMirror: string list option
      Skillist: string list
      ExplicitDeps: string list
      PhaseDeps: string list
      SkillMatchAssessments: SkillAssessment list
      Seh: SehMetadata }

type TasksParse =
    { Tasks: TaskRecord list
      Errors: string list }

[<RequireQualifiedAccess>]
module internal Lines =
    /// Python str.splitlines() for the \n / \r\n / \r cases: drops exactly one
    /// trailing line terminator so "a\n" -> ["a"].
    let split (text: string) : string list =
        if text = "" then
            []
        else
            let parts = Regex.Split(text, "\r\n|\r|\n")
            let arr = parts
            // Regex.Split keeps a trailing "" when the string ends with a terminator.
            let endsWithTerm =
                text.EndsWith("\n") || text.EndsWith("\r")
            let lst =
                if endsWithTerm && arr.Length > 0 && arr.[arr.Length - 1] = "" then
                    arr.[.. arr.Length - 2]
                else
                    arr
            List.ofArray lst

module TaskParser =

    let private taskIdRe = Regex(@"\bT\d{3,4}\b", RegexOptions.Compiled)

    let private taskLineRe =
        Regex(@"^\s*-\s*\[(?<box>[ X\-FS*])\]\s+(?<id>T\d{3,4})\b(?<rest>.*)$", RegexOptions.Compiled)

    let private phaseHeaderRe =
        Regex(@"^\s*##\s+Phase\s+(\d+)\s*:", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)

    let private checkpointRe =
        Regex(@"^\s*\*\*Checkpoint", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)

    let private parallelRe = Regex(@"\[P\]", RegexOptions.Compiled)
    let private storyRe = Regex(@"\[(US\d+)\]", RegexOptions.Compiled)
    let private tierRe = Regex(@"\[(T[12])\]", RegexOptions.Compiled)
    let private sehAnnotationRe = Regex(@"(?<!`)\[SEH\](?!`)", RegexOptions.Compiled)
    let private sehApprovalRe = Regex(@"(?<!`)synthetic-error-handling-approved(?!`)", RegexOptions.Compiled)
    let private skillistRe = Regex(@"\[skillist:\s*(?<value>\[\]|[^\]]*)\]", RegexOptions.Compiled)
    let private titleAnnotRe = Regex(@"(?<!`)\[(P|US\d+|T[12]|SEH)\](?!`)\s*", RegexOptions.Compiled)
    let private titleSkillistRe = Regex(@"\[skillist:\s*(?:\[\]|[^\]])*\]\s*", RegexOptions.Compiled)

    let declaredString (s: DeclaredStatus) : string =
        match s with
        | Pending -> "pending"
        | Done -> "done"
        | Synthetic -> "synthetic"
        | Failed -> "failed"
        | Skipped -> "skipped"

    // Mutable working record mirroring the Python dataclass; collapsed to the
    // immutable TaskRecord at the end of parse.
    type private MTask =
        { Id: string
          mutable Declared: DeclaredStatus
          mutable Phase: int option
          mutable Story: string option
          mutable Tier: string option
          mutable Parallel: bool
          mutable LineNo: int
          mutable Title: string
          mutable SkillistMirror: string list option
          mutable Skillist: string list
          mutable ExplicitDeps: string list
          mutable PhaseDeps: string list
          mutable SkillMatchAssessments: SkillAssessment list
          mutable SehAnnotation: bool
          mutable SehApprovalLabel: bool
          mutable DesignSource: string
          mutable SyntheticInputClass: string
          mutable ExpectedErrorBehavior: string
          mutable SehRationale: string
          mutable SehAcceptanceStatus: string
          mutable SehDiagnostics: string list }

    let private isAcceptedSehM (t: MTask) : bool =
        t.Declared = Synthetic
        && t.SehAnnotation
        && t.SehApprovalLabel
        && t.SehAcceptanceStatus = "accepted-seh"
        && List.isEmpty t.SehDiagnostics

    let isAcceptedSeh (t: TaskRecord) : bool =
        t.Declared = Synthetic
        && t.Seh.Annotation
        && t.Seh.ApprovalLabel
        && t.Seh.AcceptanceStatus = "accepted-seh"
        && List.isEmpty t.Seh.Diagnostics

    let private boxToStatus (box: string) : Choice<DeclaredStatus, unit> =
        match box with
        | " " -> Choice1Of2 Pending
        | "X" -> Choice1Of2 Done
        | "S" -> Choice1Of2 Synthetic
        | "F" -> Choice1Of2 Failed
        | "-" -> Choice1Of2 Skipped
        | _ -> Choice2Of2 () // "*"

    let parse (tasksMd: string) : TasksParse =
        let errors = ResizeArray<string>()
        let tasks = Dictionary<string, MTask>()
        let order = ResizeArray<string>()
        let mutable currentPhase : int option = None
        let phaseTasks = Dictionary<int, ResizeArray<string>>()
        let phaseFoundation = Dictionary<int, ResizeArray<string>>()
        let mutable pastCheckpoint = false

        let getList (d: Dictionary<int, ResizeArray<string>>) (k: int) =
            match d.TryGetValue k with
            | true, v -> v
            | _ ->
                let v = ResizeArray<string>()
                d.[k] <- v
                v

        let allLines = Lines.split tasksMd

        allLines
        |> List.iteri (fun idx0 line ->
            let i = idx0 + 1
            let phaseM = phaseHeaderRe.Match line
            if phaseM.Success then
                currentPhase <- Some(int (phaseM.Groups.[1].Value))
                pastCheckpoint <- false
            elif checkpointRe.IsMatch line then
                pastCheckpoint <- true
            else
                let taskM = taskLineRe.Match line
                if taskM.Success then
                    let box = taskM.Groups.["box"].Value
                    let tid = taskM.Groups.["id"].Value
                    let rest = taskM.Groups.["rest"].Value.Trim()
                    match boxToStatus box with
                    | Choice2Of2 () ->
                        errors.Add(
                            sprintf
                                "tasks.md:%d: invalid status [*] on %s — [S*] is computed, never written manually"
                                i tid)
                    | Choice1Of2 declared ->
                        let isParallel = parallelRe.IsMatch rest
                        let storyM = storyRe.Match rest
                        let tierM = tierRe.Match rest
                        let sehAnnotation = sehAnnotationRe.IsMatch rest
                        let sehApproval = sehApprovalRe.IsMatch rest
                        let skillistM = skillistRe.Match rest
                        let skillistMirror =
                            if skillistM.Success then
                                let raw = skillistM.Groups.["value"].Value.Trim()
                                if raw = "[]" then
                                    Some []
                                else
                                    Some(
                                        raw.Split(',')
                                        |> Array.map (fun x -> x.Trim())
                                        |> Array.filter (fun x -> x <> "")
                                        |> List.ofArray)
                            else
                                None
                        let title =
                            let t1 = titleAnnotRe.Replace(rest, "").Trim()
                            titleSkillistRe.Replace(t1, "").Trim()
                        if tasks.ContainsKey tid then
                            errors.Add(sprintf "tasks.md:%d: duplicate task id %s" i tid)
                        else
                            let t =
                                { Id = tid
                                  Declared = declared
                                  Phase = currentPhase
                                  Story = (if storyM.Success then Some storyM.Groups.[1].Value else None)
                                  Tier = (if tierM.Success then Some tierM.Groups.[1].Value else None)
                                  Parallel = isParallel
                                  LineNo = i
                                  Title = title
                                  SkillistMirror = skillistMirror
                                  Skillist = []
                                  ExplicitDeps = []
                                  PhaseDeps = []
                                  SkillMatchAssessments = []
                                  SehAnnotation = sehAnnotation
                                  SehApprovalLabel = sehApproval
                                  DesignSource = ""
                                  SyntheticInputClass = ""
                                  ExpectedErrorBehavior = ""
                                  SehRationale = ""
                                  SehAcceptanceStatus = ""
                                  SehDiagnostics = [] }
                            tasks.[tid] <- t
                            order.Add tid
                            match currentPhase with
                            | Some p ->
                                (getList phaseTasks p).Add tid
                                if not pastCheckpoint then
                                    (getList phaseFoundation p).Add tid
                            | None -> ())

        // --- Synthetic-Evidence Inventory merge -----------------------------
        let linesArr = List.toArray allLines
        let mutable handledInventory = false
        let mutable index = 0
        while not handledInventory && index < linesArr.Length do
            if linesArr.[index].Trim().ToLowerInvariant() = "## synthetic-evidence inventory" then
                let mutable header : string list option = None
                let mutable j = index + 1
                let mutable stop = false
                while not stop && j < linesArr.Length do
                    let stripped = linesArr.[j].Trim()
                    if stripped.StartsWith("## ") then
                        stop <- true
                    elif not (stripped.StartsWith("|") && stripped.EndsWith("|")) then
                        ()
                    else
                        let cells =
                            stripped.Trim('|').Split('|')
                            |> Array.map (fun c -> c.Trim())
                            |> List.ofArray
                        let isSeparator =
                            not (List.isEmpty cells)
                            && cells |> List.forall (fun c -> c <> "" && (c |> Seq.forall (fun ch -> ch = '-')))
                        if isSeparator then
                            ()
                        elif header.IsNone then
                            header <- Some(cells |> List.map (fun c -> c.ToLowerInvariant()))
                        elif (not (List.isEmpty cells)) && (List.head cells).StartsWith("_(") then
                            ()
                        else
                            let hdr = Option.defaultValue [] header
                            let n = min (List.length hdr) (List.length cells)
                            let rowData = Dictionary<string, string>()
                            for k in 0 .. n - 1 do
                                rowData.[hdr.[k]] <- cells.[k]
                            let getCell key =
                                match rowData.TryGetValue key with
                                | true, v -> v
                                | _ -> ""
                            let taskIdM = taskIdRe.Match(getCell "task")
                            if taskIdM.Success then
                                match tasks.TryGetValue taskIdM.Value with
                                | true, task ->
                                    let label = getCell "label"
                                    task.SehApprovalLabel <-
                                        task.SehApprovalLabel || label = "synthetic-error-handling-approved"
                                    let withDefault key (cur: string) =
                                        match rowData.TryGetValue key with
                                        | true, v -> v.Trim()
                                        | _ -> cur.Trim()
                                    task.DesignSource <- withDefault "design source" task.DesignSource
                                    task.SyntheticInputClass <- withDefault "synthetic input class" task.SyntheticInputClass
                                    task.ExpectedErrorBehavior <- withDefault "expected error behavior" task.ExpectedErrorBehavior
                                    task.SehAcceptanceStatus <- withDefault "acceptance status" task.SehAcceptanceStatus
                                    task.SehRationale <- withDefault "reason" task.SehRationale
                                | _ -> ()
                    j <- j + 1
                handledInventory <- true
            index <- index + 1

        // --- SEH diagnostics ------------------------------------------------
        for tid in order do
            let task = tasks.[tid]
            if task.Declared = Synthetic && (task.SehAnnotation || task.SehApprovalLabel) then
                let diags = ResizeArray<string>()
                if not task.SehAnnotation then diags.Add "missing [SEH] annotation"
                if not task.SehApprovalLabel then diags.Add "missing synthetic-error-handling-approved label"
                if task.DesignSource = "" then diags.Add "missing design-phase source"
                if task.SyntheticInputClass = "" then diags.Add "missing synthetic input class"
                if task.ExpectedErrorBehavior = "" then diags.Add "missing expected error behavior"
                if task.SehRationale = "" then diags.Add "missing rationale"
                if task.Declared = Synthetic && task.SehAcceptanceStatus <> "accepted-seh" then
                    diags.Add "missing accepted-seh acceptance status"
                task.SehDiagnostics <- List.ofSeq diags

        // --- phase-checkpoint edge injection --------------------------------
        let sortedPhases = phaseTasks.Keys |> Seq.sort |> List.ofSeq
        sortedPhases
        |> List.iteri (fun idx phase ->
            if idx > 0 then
                let prev = sortedPhases.[idx - 1]
                let foundation =
                    let f = getList phaseFoundation prev
                    if f.Count > 0 then f else getList phaseTasks prev
                if foundation.Count > 0 then
                    let anchor = foundation.[foundation.Count - 1]
                    for tid in getList phaseTasks phase do
                        if tid <> anchor && tasks.ContainsKey anchor then
                            let t = tasks.[tid]
                            t.PhaseDeps <- t.PhaseDeps @ [ anchor ])

        let records =
            order
            |> Seq.map (fun tid ->
                let m = tasks.[tid]
                { Id = m.Id
                  Declared = m.Declared
                  Phase = m.Phase
                  Story = m.Story
                  Tier = m.Tier
                  Parallel = m.Parallel
                  LineNo = m.LineNo
                  Title = m.Title
                  SkillistMirror = m.SkillistMirror
                  Skillist = m.Skillist
                  ExplicitDeps = m.ExplicitDeps
                  PhaseDeps = m.PhaseDeps
                  SkillMatchAssessments = m.SkillMatchAssessments
                  Seh =
                    { Annotation = m.SehAnnotation
                      ApprovalLabel = m.SehApprovalLabel
                      DesignSource = m.DesignSource
                      SyntheticInputClass = m.SyntheticInputClass
                      ExpectedErrorBehavior = m.ExpectedErrorBehavior
                      Rationale = m.SehRationale
                      AcceptanceStatus = m.SehAcceptanceStatus
                      Diagnostics = m.SehDiagnostics } })
            |> List.ofSeq

        { Tasks = records; Errors = List.ofSeq errors }

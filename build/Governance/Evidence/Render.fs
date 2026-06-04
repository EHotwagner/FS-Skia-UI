namespace FS.Skia.UI.Build.Evidence

open System.Text

module Render =

    // --- Python-compatible JSON (json.dumps indent=2, ensure_ascii=True) ----

    type private Json =
        | JNull
        | JBool of bool
        | JInt of int
        | JStr of string
        | JArr of Json list
        | JObj of (string * Json) list

    let private escapeString (s: string) : string =
        let sb = StringBuilder()
        sb.Append '"' |> ignore
        for c in s do
            match c with
            | '\\' -> sb.Append "\\\\" |> ignore
            | '"' -> sb.Append "\\\"" |> ignore
            | '\b' -> sb.Append "\\b" |> ignore
            | '\f' -> sb.Append "\\f" |> ignore
            | '\n' -> sb.Append "\\n" |> ignore
            | '\r' -> sb.Append "\\r" |> ignore
            | '\t' -> sb.Append "\\t" |> ignore
            | c when c < ' ' || c > '~' -> sb.AppendFormat("\\u{0:x4}", int c) |> ignore
            | c -> sb.Append c |> ignore
        sb.Append '"' |> ignore
        sb.ToString()

    let rec private ser (j: Json) (ind: int) : string =
        let pad n = System.String(' ', n)
        match j with
        | JNull -> "null"
        | JBool b -> if b then "true" else "false"
        | JInt n -> string n
        | JStr s -> escapeString s
        | JArr [] -> "[]"
        | JArr xs ->
            let inner = xs |> List.map (fun x -> pad (ind + 2) + ser x (ind + 2)) |> String.concat ",\n"
            "[\n" + inner + "\n" + pad ind + "]"
        | JObj [] -> "{}"
        | JObj kvs ->
            let inner =
                kvs |> List.map (fun (k, v) -> pad (ind + 2) + escapeString k + ": " + ser v (ind + 2)) |> String.concat ",\n"
            "{\n" + inner + "\n" + pad ind + "}"

    let private jStrList xs = JArr(xs |> List.map JStr)
    let private jOptStr (o: string option) = match o with | Some s -> JStr s | None -> JNull
    let private jOptInt (o: int option) = match o with | Some n -> JInt n | None -> JNull
    let private jOptStrList (o: string list option) = match o with | Some xs -> jStrList xs | None -> JNull

    // --- shared lookups -----------------------------------------------------

    let private statusBox (eff: string) =
        match eff with
        | "pending" -> "[ ]"
        | "done" -> "[X]"
        | "synthetic" -> "[S]"
        | "failed" -> "[F]"
        | "skipped" -> "[-]"
        | "auto-synthetic" -> "[S*]"
        | _ -> "[ ]"

    let private mermaidClass (eff: string) =
        match eff with
        | "pending" -> "pending"
        | "done" -> "done"
        | "synthetic" -> "synthetic"
        | "failed" -> "failed"
        | "skipped" -> "skipped"
        | "auto-synthetic" -> "autoSynthetic"
        | _ -> "pending"

    // --- JSON ---------------------------------------------------------------

    let private assessmentJson (a: SkillAssessment) : Json =
        JObj
            [ "task_id", JStr a.TaskId
              "declared_skillist", jStrList a.DeclaredSkillist
              "candidate_skill_id", jOptStr a.CandidateSkillId
              "matched_signals", jStrList a.MatchedSignals
              "confidence", JStr a.Confidence
              "ambiguity", jOptStr a.Ambiguity
              "reviewer_disposition", jOptStr a.ReviewerDisposition
              "diagnostic", JStr a.Diagnostic ]

    let private taskJson (r: ResolvedTask) : Json =
        let t = r.Task
        JObj
            [ "id", JStr t.Id
              "declared", JStr(TaskParser.declaredString t.Declared)
              "effective", JStr(Graph.effectiveString r.Effective)
              "phase", jOptInt t.Phase
              "story", jOptStr t.Story
              "tier", jOptStr t.Tier
              "parallel", JBool t.Parallel
              "title", JStr t.Title
              "skillist", jStrList t.Skillist
              "skillist_mirror", jOptStrList t.SkillistMirror
              "skill_match_assessments", JArr(t.SkillMatchAssessments |> List.map assessmentJson)
              "explicit_deps", jStrList t.ExplicitDeps
              "phase_deps", jStrList t.PhaseDeps
              "root_cause", jStrList r.RootCause
              "seh",
              JObj
                  [ "annotation", JBool t.Seh.Annotation
                    "approval_label", JBool t.Seh.ApprovalLabel
                    "design_source", JStr t.Seh.DesignSource
                    "synthetic_input_class", JStr t.Seh.SyntheticInputClass
                    "expected_error_behavior", JStr t.Seh.ExpectedErrorBehavior
                    "rationale", JStr t.Seh.Rationale
                    "acceptance_status", JStr t.Seh.AcceptanceStatus
                    "diagnostics", jStrList t.Seh.Diagnostics
                    "accepted", JBool(TaskParser.isAcceptedSeh t) ] ]

    let taskGraphJson (g: GraphResult) : string =
        let verdict =
            if List.isEmpty g.Errors && List.isEmpty g.Cycles then "ok" else "error"
        let payload =
            JObj
                [ "schema_version", JStr "1.0"
                  "verdict", JStr verdict
                  "errors", jStrList g.Errors
                  "warnings", jStrList g.Warnings
                  "cycles", JArr(g.Cycles |> List.map jStrList)
                  "tasks", JArr(g.Tasks |> List.map taskJson) ]
        ser payload 0 + "\n"

    // --- Markdown -----------------------------------------------------------

    let private renderMermaid (tasks: ResolvedTask list) (nodes: Map<string, ResolvedTask>) : string =
        let lines = ResizeArray<string>()
        lines.Add "```mermaid"
        lines.Add "graph TD"
        for r in tasks do
            let title = r.Task.Title.Replace("\"", "'")
            let label = if title.Length > 50 then title.Substring(0, 50) else title
            lines.Add(sprintf "  %s[\"%s %s\"]:::%s" r.Task.Id r.Task.Id label (mermaidClass (Graph.effectiveString r.Effective)))
        // FR-007 (062): render the EFFECTIVE DAG with the auto-injected Phase N+1 →
        // Phase N checkpoint edges (from PhaseDeps) DISTINCTLY labeled — explicit
        // deps are solid `-->`, injected checkpoint edges are dashed `-. injected .->`
        // — so the author sees the effective graph before a full run.
        for r in tasks do
            for d in r.Task.ExplicitDeps do
                if nodes.ContainsKey d then
                    lines.Add(sprintf "  %s --> %s" d r.Task.Id)
            for d in r.Task.PhaseDeps do
                if nodes.ContainsKey d && not (List.contains d r.Task.ExplicitDeps) then
                    lines.Add(sprintf "  %s -. injected .-> %s" d r.Task.Id)
        lines.AddRange
            [ "  classDef pending fill:#eeeeee,stroke:#999"
              "  classDef done fill:#c8e6c9,stroke:#2e7d32"
              "  classDef synthetic fill:#ffe0b2,stroke:#e65100,stroke-width:2px"
              "  classDef autoSynthetic fill:#ffab91,stroke:#bf360c,stroke-width:2px,stroke-dasharray:5 3"
              "  classDef failed fill:#ffcdd2,stroke:#b71c1c,stroke-width:2px"
              "  classDef skipped fill:#f5f5f5,stroke:#666,stroke-dasharray:3 3"
              "```" ]
        String.concat "\n" lines

    let private renderAscii (tasks: ResolvedTask list) (nodes: Map<string, ResolvedTask>) (rootCause: Map<string, string list>) : string =
        let lines = ResizeArray<string>()
        for r in tasks do
            let eff = Graph.effectiveString r.Effective
            let box = statusBox eff
            let marker =
                if TaskParser.isAcceptedSeh r.Task then "   ← accepted [SEH]"
                elif eff = "auto-synthetic" then "   ← auto-synthetic"
                elif eff = "synthetic" then "   ← root cause"
                else ""
            lines.Add(sprintf "%s %s %s%s" r.Task.Id box r.Task.Title marker)
            match rootCause.TryFind r.Task.Id with
            | Some rcs ->
                for rc in rcs do
                    match nodes.TryFind rc with
                    | Some rcTask ->
                        let rcBox = statusBox (Graph.effectiveString rcTask.Effective)
                        lines.Add(sprintf "    └── %s %s %s" rc rcBox rcTask.Task.Title)
                    | None -> ()
            | None -> ()
        String.concat "\n" lines

    let taskGraphMd (g: GraphResult) : string =
        let out = ResizeArray<string>()
        let tasks = g.Tasks
        let nodes = tasks |> List.map (fun r -> r.Task.Id, r) |> Map.ofList
        let eff (r: ResolvedTask) = Graph.effectiveString r.Effective

        out.Add(sprintf "# Task Graph — %s" g.FeatureName)
        out.Add ""

        if not (List.isEmpty g.Errors) || not (List.isEmpty g.Cycles) then
            out.Add "## ✗ Graph validation failed"
            if not (List.isEmpty g.Cycles) then
                out.Add ""
                out.Add "### Cycles detected"
                for cy in g.Cycles do
                    out.Add(sprintf "- %s" (String.concat " → " cy))
            if not (List.isEmpty g.Errors) then
                out.Add ""
                out.Add "### Errors"
                for e in g.Errors do
                    out.Add(sprintf "- %s" e)
                // FR-005 (062): when a skill-loading-evidence error blocks the graph,
                // print the complete required row shape (8 columns, ordering rule,
                // resolved-path pattern) single-sourced from EvidenceFormatSchema, so
                // an author recovers the contract from the graph's own output without
                // decompiling FS.Skia.UI.Build.dll or copying a sibling project.
                let skillLoadingError (e: string) =
                    e.Contains "load evidence" || e.Contains "loaded_at"
                    || e.Contains "skill-loading evidence" || e.Contains "evidence path"
                if g.Errors |> List.exists skillLoadingError then
                    out.Add ""
                    out.Add "### skill-loading-evidence required shape (FR-005)"
                    out.Add ""
                    out.Add "```"
                    out.Add((Audit.skillLoadingEvidenceSchemaText ()).TrimEnd('\n'))
                    out.Add "```"
            out.Add ""
        else
            out.Add "## ✓ Graph is acyclic and consistent"
            out.Add ""

        if not (List.isEmpty g.Warnings) then
            out.Add "### Warnings"
            for w in g.Warnings do
                out.Add(sprintf "- %s" w)
            out.Add ""

        out.Add "## Skill Match Assessments"
        out.Add ""
        out.Add "| Task | Candidate | Confidence | Signals | Reviewer disposition | Diagnostic |"
        out.Add "|------|-----------|------------|---------|----------------------|------------|"
        for r in tasks do
            for a in r.Task.SkillMatchAssessments do
                let candidate = defaultArg a.CandidateSkillId "(none)"
                let signals = String.concat ", " a.MatchedSignals
                let disposition = defaultArg a.ReviewerDisposition "(required)"
                out.Add(sprintf "| %s | %s | %s | %s | %s | %s |" r.Task.Id candidate a.Confidence signals disposition a.Diagnostic)
        out.Add ""

        // counts
        let counts = System.Collections.Generic.Dictionary<string, int>()
        let bump k = counts.[k] <- (match counts.TryGetValue k with | true, v -> v | _ -> 0) + 1
        let mutable acceptedSeh = 0
        let mutable unacceptedSynthetic = 0
        for r in tasks do
            let e = eff r
            bump e
            if e = "synthetic" && TaskParser.isAcceptedSeh r.Task then acceptedSeh <- acceptedSeh + 1
            elif e = "synthetic" || e = "auto-synthetic" then unacceptedSynthetic <- unacceptedSynthetic + 1
        let countOf k = match counts.TryGetValue k with | true, v -> v | _ -> 0
        out.Add "## Status counts (effective)"
        out.Add ""
        out.Add "| Status | Count |"
        out.Add "|--------|-------|"
        for key in [ "pending"; "done"; "synthetic"; "auto-synthetic"; "failed"; "skipped" ] do
            if countOf key <> 0 || key = "synthetic" || key = "auto-synthetic" then
                out.Add(sprintf "| %s %s | %d |" (statusBox key) key (countOf key))
        out.Add(sprintf "| accepted [SEH] synthetic | %d |" acceptedSeh)
        out.Add(sprintf "| unaccepted synthetic | %d |" unacceptedSynthetic)
        out.Add ""

        if acceptedSeh <> 0 || unacceptedSynthetic <> 0 then
            out.Add "## Synthetic Error-Handling Classification"
            out.Add ""
            out.Add "| Task | Accepted | Label | Design source | Synthetic input class | Expected error behavior | Diagnostics |"
            out.Add "|------|----------|-------|---------------|-----------------------|-------------------------|-------------|"
            for r in tasks do
                let e = eff r
                if (e = "synthetic" || e = "auto-synthetic") || r.Task.Seh.Annotation || r.Task.Seh.ApprovalLabel then
                    let diagnostics = String.concat "; " r.Task.Seh.Diagnostics
                    let orMissing (s: string) = if s = "" then "(missing)" else s
                    out.Add(
                        sprintf "| %s | %s | %s | %s | %s | %s | %s |"
                            r.Task.Id
                            (if TaskParser.isAcceptedSeh r.Task then "yes" else "no")
                            (if r.Task.Seh.ApprovalLabel then "yes" else "no")
                            (orMissing r.Task.Seh.DesignSource)
                            (orMissing r.Task.Seh.SyntheticInputClass)
                            (orMissing r.Task.Seh.ExpectedErrorBehavior)
                            (if diagnostics = "" then "(none)" else diagnostics))
            out.Add ""

        out.Add "## Graph"
        out.Add ""
        out.Add(renderMermaid tasks nodes)
        out.Add ""

        out.Add "## ASCII view"
        out.Add ""
        out.Add "```"
        out.Add(renderAscii tasks nodes g.RootCause)
        out.Add "```"
        out.Add ""

        // FR-007 (062): the effective DAG also lists the auto-injected Phase N+1 →
        // Phase N checkpoint edges as a distinct subsection (the edges that are
        // invisible in tasks.deps.yml until a full run), and the resolved skillist-id
        // set, alongside the existing graphVerdictLine — so the author reviews the
        // effective graph before trusting it.
        let injectedEdges =
            [ for r in tasks do
                  for d in r.Task.PhaseDeps do
                      if nodes.ContainsKey d && not (List.contains d r.Task.ExplicitDeps) then
                          yield (d, r.Task.Id) ]
        out.Add "## Injected checkpoint edges (Phase N+1 → Phase N) — FR-007"
        out.Add ""
        if List.isEmpty injectedEdges then
            out.Add "_(none — no cross-phase checkpoint edges injected)_"
        else
            for (dep, tid) in injectedEdges do
                out.Add(sprintf "- %s → %s  (auto-injected Phase-checkpoint edge)" dep tid)
        out.Add ""

        let resolvedSkillIds =
            tasks
            |> List.collect (fun r -> r.Task.Skillist)
            |> List.distinct
            |> List.sort
        out.Add "## Resolved skillist ids — FR-007"
        out.Add ""
        if List.isEmpty resolvedSkillIds then
            out.Add "_(none declared)_"
        else
            out.Add(sprintf "Resolved skillist-id set (%d): %s" (List.length resolvedSkillIds) (String.concat ", " resolvedSkillIds))
        out.Add ""

        if not (Map.isEmpty g.RootCause) then
            out.Add "## Propagation report"
            out.Add ""
            out.Add (
                "The following tasks are marked `[S*]` because at least one of "
                + "their dependencies is synthetic-only. Clearing the upstream "
                + "`[S]` tasks (real evidence) will automatically clear these.")
            out.Add ""
            for tid in g.RootCause |> Map.toList |> List.map fst |> List.sort do
                let rcStr = String.concat ", " g.RootCause.[tid]
                out.Add(sprintf "- **%s** ([S*]) ← %s" tid rcStr)
            out.Add ""

        String.concat "\n" out + "\n"

    // --- audit counts -------------------------------------------------------

    let auditCounts
        (featureName: string)
        (realTasks: int)
        (acceptedSeh: int)
        (unacceptedSynthetic: int)
        (autoSynthetic: int)
        (lateSeh: int)
        : string =
        let lines =
            [ sprintf "# Audit count block — %s" featureName
              "# Graph-derived merge-gate counts (the four fields named in"
              "# contracts/golden-fixture-reproducibility.contract.md), computed by the"
              "# existing evidence engine from readiness/task-graph.json. Deterministic."
              sprintf "real-tasks=%d" realTasks
              sprintf "accepted-seh-tasks=%d" acceptedSeh
              sprintf "unaccepted-synthetic-tasks=%d" unacceptedSynthetic
              sprintf "auto-synthetic-tasks=%d" autoSynthetic
              sprintf "late-seh-tasks=%d" lateSeh ]
        String.concat "\n" lines + "\n"

    // --- scan / diff / summary serializers ----------------------------------

    let private scanHitJson (area: string) (h: ScanHit) : Json =
        match area with
        | "readiness-contract" ->
            [ yield "path", JStr h.Path
              yield "status", (match h.Status with | Some s -> JStr s | None -> JNull)
              yield "reason", JStr h.Reason
              match h.Missing with | Some m -> yield "missing", jStrList m | None -> ()
              yield "missing_terms", (match h.MissingTerms with | Some m -> jStrList m | None -> JArr [])
              yield "missing_sections", (match h.MissingSections with | Some m -> jStrList m | None -> JArr [])
              yield "blocking", JBool(defaultArg h.Blocking true)
              yield "validation_area", (match h.ValidationArea with | Some v -> JStr v | None -> JStr "readiness-contract") ]
            |> JObj
        | "persistent-launch" ->
            [ yield "path", JStr h.Path
              yield "reason", JStr h.Reason
              match h.Missing with | Some m -> yield "missing", jStrList m | None -> ()
              match h.Required with | Some r -> yield "required", jStrList r | None -> () ]
            |> JObj
        | _ ->
            // persistent-gui-runtime / window-visibility: {path, reason, missing?}
            [ yield "path", JStr h.Path
              yield "reason", JStr h.Reason
              match h.Missing with | Some m -> yield "missing", jStrList m | None -> () ]
            |> JObj

    let scanHitsJson (area: string) (result: ScanResult) : string =
        ser (JArr(result.Hits |> List.map (scanHitJson area))) 0 + "\n"

    let private diffHitJson (h: DiffHit) : Json =
        JObj
            [ "file", JStr h.File
              "line", JInt h.Line
              "pattern", JStr h.Pattern
              "severity", JStr h.Severity
              "reason", JStr h.Reason
              "match", JStr h.Match ]

    let diffScanJson (r: DiffScanResult) : string =
        let payload =
            JObj
                [ "base_ref", (match r.BaseRef with | Some s -> JStr s | None -> JNull)
                  "blocking", JArr(r.Blocking |> List.map diffHitJson)
                  "advisory", JArr(r.Advisory |> List.map diffHitJson) ]
        ser payload 0 + "\n"

    let auditStatusJson (scannedFiles: string list) (blocking: string list) : string =
        ser (JObj [ "scanned_files", jStrList scannedFiles; "blocking", jStrList blocking ]) 0 + "\n"

    let sehAuditSummaryJson (s: SehSummary) : string =
        let diag (task, rule, source, action) =
            JObj
                [ "task", JStr task
                  "failed_rule", JStr rule
                  "source", JStr source
                  "required_action", JStr action ]
        let payload =
            JObj
                [ "accepted_seh_tasks", jStrList s.AcceptedSehTasks
                  "unaccepted_synthetic_tasks", jStrList s.UnacceptedSyntheticTasks
                  "auto_synthetic_tasks", jStrList s.AutoSyntheticTasks
                  "late_seh_tasks", jStrList s.LateSehTasks
                  "diagnostics", JArr(s.Diagnostics |> List.map diag) ]
        ser payload 0 + "\n"

    // --- terminal verdict / self-describing diagnostics (feature 061) --------

    /// FR-007 (061): the explicit, greppable terminal verdict line for an
    /// `EvidenceGraph` run, in the same `verdict=…` token style as
    /// `EvidenceAudit` (GV-1/GV-2/GV-3). A clean graph reads at a glance; a
    /// failing graph names the reason inline. Additive to exit-code semantics.
    let graphVerdictLine (gr: GraphResult) : string =
        match gr.Verdict with
        | GraphVerdict.Ok -> "verdict=ok (no cycles, no dangling refs, no [S*])"
        | GraphVerdict.Error ->
            let reason =
                match gr.Errors with
                | [] -> "graph invalid"
                | es -> es |> List.truncate 3 |> String.concat "; "
            sprintf "verdict=error (%s)" reason

    /// FR-004 (061): the self-describing readiness-contract failure diagnostic.
    /// For every failing readiness file the audit prints the COMPLETE expected
    /// shape — file name, status, the full enforced `required-tokens` list, and
    /// the subset actually `missing` — so a consumer can recover the contract
    /// from the audit's own output without decompiling `FS.Skia.UI.Build.dll`
    /// or copying a sibling project (RC-1/RC-2/RC-3). The token list is sourced
    /// from the same `Required`/`MissingTerms` data the scan enforces, so the
    /// printout cannot drift from the rule.
    let readinessContractDiagnostics (rc: ScanResult) : string =
        if List.isEmpty rc.Hits then
            ""
        else
            let sb = StringBuilder()
            for h in rc.Hits do
                let fileName = System.IO.Path.GetFileName h.Path
                let status =
                    match h.Status with
                    | Some "incomplete" -> "partial"
                    | Some s -> s
                    | None -> "partial"
                sb.Append(sprintf "readiness-contract: %s\n" fileName) |> ignore
                sb.Append(sprintf "  status: %s\n" status) |> ignore
                (match h.Required with
                 | Some terms when not (List.isEmpty terms) ->
                     sb.Append(sprintf "  required-tokens: %s\n" (String.concat ", " terms)) |> ignore
                 | _ -> ())
                (match h.MissingTerms with
                 | Some missing when not (List.isEmpty missing) ->
                     sb.Append(sprintf "  missing: %s\n" (String.concat ", " missing)) |> ignore
                 | _ -> ())
            sb.ToString()

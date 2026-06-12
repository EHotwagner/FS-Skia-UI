module FS.Skia.UI.Build.ControlsDocCoverage

open System
open System.Text.RegularExpressions

type DocReason =
    | Placeholder
    | Empty
    | DuplicateOnly

type DocFinding =
    { File: string
      Line: int
      Identifier: string
      Reason: DocReason
      Detail: string }

// The boilerplate sentence family: the original `function` wording plus the mechanically
// reworded `type` / `module` / `value` variants that ship the same non-information.
let private placeholderRegex =
    Regex(
        @"Public contract (function|type|module|value) exposed by this FS\.Skia\.UI package\.",
        RegexOptions.Compiled
    )

let private collapseWhitespace (s: string) = Regex.Replace(s, @"\s+", " ").Trim()

let isPlaceholderSummary (summary: string) =
    placeholderRegex.IsMatch(collapseWhitespace summary)

// A declaration line we may attach a leading `///` block to. `and`/`member` are recognised so
// a placeholder above them is still caught, but only `val`/`type`/`module` REQUIRE a summary
// (the Empty rule) — `and`/`member` continuations legitimately share or omit docs.
let private declRegex = Regex(@"^\s*(val|type|module|member|and)\b", RegexOptions.Compiled)

let private requiresDoc kind = kind = "val" || kind = "type" || kind = "module"

let private parseIdentifier (line: string) =
    let m = Regex.Match(line, @"^\s*(?:val|type|module|member|and)\s+(.*)$")

    if not m.Success then
        "(declaration)"
    else
        // Drop visibility / binding modifiers, then take the first identifier-shaped token.
        let rest = Regex.Replace(m.Groups.[1].Value, @"^(mutable|rec|internal|private|public|inline)\s+", "")
        let nm = Regex.Match(rest, @"^\(?\s*([A-Za-z_][A-Za-z0-9_']*)")
        if nm.Success then nm.Groups.[1].Value else collapseWhitespace rest

// A summary "carries a member-specific token" when it names something concrete — a backticked
// identifier or a numeric value/unit. The duplicate-only rule only fires on token-LESS sentences.
let private hasMemberToken (summary: string) =
    summary.Contains("`") || Regex.IsMatch(summary, @"\d")

// Scan one file, returning immediate findings plus the (summary, line, identifier) candidates
// that feed the cross-member duplicate-only analysis.
let private analyzeFile (file: string) (content: string) : DocFinding list * (string * string * int * string) list =
    let lines = content.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n')
    let mutable buffer: string list = [] // accumulated /// text, reversed
    let mutable bufferStart = 0
    let mutable findings: DocFinding list = []
    let mutable candidates: (string * string * int * string) list = []

    for idx in 0 .. lines.Length - 1 do
        let raw = lines.[idx]
        let trimmed = raw.Trim()
        let lineNo = idx + 1

        if trimmed.StartsWith("///") then
            if List.isEmpty buffer then
                bufferStart <- lineNo

            buffer <- trimmed.Substring(3).Trim() :: buffer
        elif trimmed = "" then
            buffer <- []
        elif trimmed.StartsWith("[<") then
            () // attribute between the summary and its declaration — keep the buffer
        elif declRegex.IsMatch raw then
            let kind = (Regex.Match(raw, @"^\s*([A-Za-z]+)")).Groups.[1].Value
            let ident = parseIdentifier raw
            // `internal` members are out of scope (doc-comment-standard: only the public surface
            // must be documented). They never carry the placeholder boilerplate, so skipping the
            // Empty rule for them keeps the gate scoped to what a consumer actually sees.
            let isInternal = Regex.IsMatch(raw, @"\binternal\b")
            let mustDoc = requiresDoc kind && not isInternal

            if not (List.isEmpty buffer) then
                let joined = buffer |> List.rev |> String.concat " " |> collapseWhitespace

                if joined = "" then
                    if mustDoc then
                        findings <-
                            { File = file
                              Line = bufferStart
                              Identifier = ident
                              Reason = Empty
                              Detail = sprintf "whitespace-only summary on `%s`" ident }
                            :: findings
                elif isPlaceholderSummary joined then
                    findings <-
                        { File = file
                          Line = bufferStart
                          Identifier = ident
                          Reason = Placeholder
                          Detail = sprintf "placeholder boilerplate summary on `%s`" ident }
                        :: findings
                else
                    candidates <- (joined, file, bufferStart, ident) :: candidates
            elif mustDoc then
                findings <-
                    { File = file
                      Line = lineNo
                      Identifier = ident
                      Reason = Empty
                      Detail = sprintf "no `///` summary on `%s`" ident }
                    :: findings

            buffer <- []
        else
            // A record field, DU case, or other content line: a pending block belonged to it
            // (e.g. a field doc we do not enforce), so drop it without attribution.
            buffer <- []

    List.rev findings, candidates

// Identical across this many members (within one file) with no member-specific token marks a
// mechanically-reworded placeholder rather than an honest terse repeat.
let private duplicateThreshold = 8

let analyze (files: (string * string) list) : DocFinding list =
    let perFile = files |> List.map (fun (f, c) -> analyzeFile f c)
    let immediate = perFile |> List.collect fst
    let candidates = perFile |> List.collect snd

    let duplicateFindings =
        candidates
        |> List.groupBy (fun (summary, file, _, _) -> file, summary)
        |> List.collect (fun ((_, summary), members) ->
            if List.length members >= duplicateThreshold && not (hasMemberToken summary) then
                members
                |> List.map (fun (_, file, line, ident) ->
                    { File = file
                      Line = line
                      Identifier = ident
                      Reason = DuplicateOnly
                      Detail =
                          sprintf "summary shared verbatim by %d members with no member-specific token" (List.length members) })
            else
                [])

    immediate @ duplicateFindings
    |> List.sortBy (fun f -> f.File, f.Line)

let private reasonText =
    function
    | Placeholder -> "placeholder"
    | Empty -> "empty"
    | DuplicateOnly -> "duplicate-only"

let failureDiagnostics (findings: DocFinding list) =
    findings
    |> List.map (fun f -> sprintf "%s:%d %s — %s — %s" f.File f.Line f.Identifier (reasonText f.Reason) f.Detail)

let renderReport (files: (string * string) list) (findings: DocFinding list) : string =
    let fileCount = List.length files

    let memberCount =
        files
        |> List.sumBy (fun (_, c) ->
            c.Replace("\r\n", "\n").Split('\n')
            |> Array.filter (fun l -> Regex.IsMatch(l, @"^\s*(val|type|module)\b"))
            |> Array.length)

    if List.isEmpty findings then
        [ "# Controls Documentation Coverage"
          ""
          sprintf
              "PASS: findings=0 over %d documentable members across %d `src/Controls/**/*.fsi` files."
              memberCount
              fileCount
          ""
          "- gate: ControlsDocCoverageCheck"
          "- rule: no placeholder / empty / duplicate-only summary on the Controls public surface"
          "- failure-class: controls-doc-placeholder" ]
        |> String.concat Environment.NewLine
    else
        [ "# Controls Documentation Coverage"
          ""
          sprintf
              "FAIL: %d documentation finding(s) over %d documentable members across %d files."
              (List.length findings)
              memberCount
              fileCount
          ""
          yield! failureDiagnostics findings |> List.map (fun d -> sprintf "- %s" d)
          ""
          "- failure-class: controls-doc-placeholder" ]
        |> String.concat Environment.NewLine

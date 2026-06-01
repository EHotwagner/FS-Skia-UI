namespace FS.Skia.UI.Build.Evidence

open System.Text.RegularExpressions

type ScanHit =
    { Path: string
      Reason: string
      Status: string option
      Missing: string list option
      MissingTerms: string list option
      MissingSections: string list option
      Required: string list option
      Blocking: bool option
      ValidationArea: string option }

type ScanResult =
    { Area: string
      Hits: ScanHit list
      BlockingCount: int }

type StatusScan =
    { Path: string
      Status: string
      Blocking: bool
      Reasons: string list
      Values: (string * string) list }

module ScanHit =
    let basic (path: string) (reason: string) : ScanHit =
        { Path = path
          Reason = reason
          Status = None
          Missing = None
          MissingTerms = None
          MissingSections = None
          Required = None
          Blocking = None
          ValidationArea = None }

module StatusRegion =

    let private fenceOpenRe = Regex(@"^\s*```audit-status\s*$", RegexOptions.Compiled)
    let private fenceAnyRe = Regex(@"^\s*```", RegexOptions.Compiled)
    let private trueSet = set [ "true"; "yes" ]
    let private falseSet = set [ "false"; "no" ]

    /// Python repr() of a string for the malformed-entry diagnostics.
    let private pyRepr (s: string) : string =
        let hasSingle = s.Contains("'")
        let hasDouble = s.Contains("\"")
        let quote = if hasSingle && not hasDouble then '"' else '\''
        let sb = System.Text.StringBuilder()
        sb.Append quote |> ignore
        for ch in s do
            match ch with
            | '\\' -> sb.Append "\\\\" |> ignore
            | '\n' -> sb.Append "\\n" |> ignore
            | '\r' -> sb.Append "\\r" |> ignore
            | '\t' -> sb.Append "\\t" |> ignore
            | c when c = quote -> sb.Append('\\').Append(c) |> ignore
            | c -> sb.Append c |> ignore
        sb.Append quote |> ignore
        sb.ToString()

    let private extractFirstRegion (text: string) : string list * bool * bool =
        let lines = text.Replace("\r\n", "\n").Split('\n') |> List.ofArray
        let region = ResizeArray<string>()
        let mutable inRegion = false
        let mutable result : (string list * bool * bool) option = None
        let rec loop (ls: string list) =
            match result, ls with
            | Some _, _ -> ()
            | None, [] -> ()
            | None, line :: rest ->
                if not inRegion then
                    if fenceOpenRe.IsMatch line then inRegion <- true
                    loop rest
                elif fenceAnyRe.IsMatch line then
                    result <- Some(List.ofSeq region, true, true)
                else
                    region.Add line
                    loop rest
        loop lines
        match result with
        | Some r -> r
        | None -> if inRegion then (List.ofSeq region, true, false) else ([], false, false)

    let private parseRegion (regionLines: string list) : (string * string) list * string list =
        let values = ResizeArray<string * string>()
        let keys = System.Collections.Generic.HashSet<string>()
        let errors = ResizeArray<string>()
        for raw in regionLines do
            let line = raw.Trim()
            if line = "" || line.StartsWith("#") then
                ()
            elif not (line.Contains "=") then
                errors.Add(sprintf "malformed entry (no '='): %s" (pyRepr line))
            else
                let idx = line.IndexOf '='
                let key = line.Substring(0, idx).Trim().ToLowerInvariant()
                let value = line.Substring(idx + 1).Trim()
                if key = "" then
                    errors.Add(sprintf "malformed entry (empty key): %s" (pyRepr line))
                elif keys.Contains key then
                    errors.Add(sprintf "duplicate key in audit-status region: %s" key)
                else
                    keys.Add key |> ignore
                    values.Add(key, value)
        List.ofSeq values, List.ofSeq errors

    let private blockingReasons (values: (string * string) list) : string list =
        let m = dict values
        let get k =
            match m.TryGetValue k with
            | true, v -> Some v
            | _ -> None
        let lower (s: string) = s.ToLowerInvariant()
        let reasons = ResizeArray<string>()
        (match get "taskbar-only" with
         | Some v when trueSet.Contains(lower v) ->
             reasons.Add "process/taskbar-only declared (taskbar-only=true)"
         | _ -> ())
        (match get "taskbar-entry", get "window-visible" with
         | Some te, Some wv when trueSet.Contains(lower te) && falseSet.Contains(lower wv) ->
             reasons.Add "process/taskbar-only (taskbar-entry=true with window-visible=false)"
         | _ -> ())
        (match get "exact-package-match" with
         | Some exact when not (trueSet.Contains(lower exact)) ->
             reasons.Add(sprintf "unresolved package mismatch (exact-package-match=%s)" exact)
         | _ -> ())
        (match get "package-resolution" with
         | Some v when lower v = "nu1603" ->
             reasons.Add "unresolved package mismatch (package-resolution=nu1603)"
         | _ -> ())
        List.ofSeq reasons

    let scanText (path: string) (text: string) : StatusScan =
        let region, found, closed = extractFirstRegion text
        if not found then
            { Path = path; Status = "no-region"; Blocking = false; Reasons = []; Values = [] }
        else
            let values, errors = parseRegion region
            let errors = if not closed then errors @ [ "malformed audit-status region (unterminated fence)" ] else errors
            let reasons = errors @ blockingReasons values
            { Path = path
              Status = (if List.isEmpty reasons then "pass" else "block")
              Blocking = not (List.isEmpty reasons)
              Reasons = reasons
              Values = values }

    let scan (files: (string * string) list) : ScanResult =
        let scans = files |> List.map (fun (p, t) -> scanText p t)
        let blocking = scans |> List.filter (fun s -> s.Blocking)
        { Area = "audit-status"
          Hits = blocking |> List.map (fun s -> ScanHit.basic s.Path (String.concat "; " s.Reasons))
          BlockingCount = List.length blocking }

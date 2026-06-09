namespace FS.Skia.UI.Build.Evidence

open System.IO
open System.Text.RegularExpressions
open YamlDotNet.RepresentationModel

type DiffHit =
    { File: string
      Line: int
      Pattern: string
      Severity: string
      Reason: string
      Match: string }

type DiffScanResult =
    { BaseRef: string option
      Blocking: DiffHit list
      Advisory: DiffHit list }

module DiffScan =

    type private Pattern =
        { Id: string
          Regex: string
          mutable Severity: string
          Reason: string }

    type private Whitelist =
        { PatternId: string
          FileGlob: string option
          LineRegex: string option }

    let private scalar (n: YamlNode) : string option =
        match n with
        | :? YamlScalarNode as s -> Option.ofObj s.Value
        | _ -> None

    let private field (m: YamlMappingNode) (name: string) : string option =
        m.Children
        |> Seq.tryPick (fun kv ->
            match scalar kv.Key with
            | Some k when k = name -> scalar kv.Value
            | _ -> None)

    let private parsePatterns (text: string) : Pattern list * Whitelist list * Map<string, string> =
        let stream = YamlStream()
        use reader = new StringReader(text)
        stream.Load reader
        let root = stream.Documents.[0].RootNode :?> YamlMappingNode
        let getNode name =
            root.Children
            |> Seq.tryPick (fun kv ->
                match scalar kv.Key with
                | Some k when k = name -> Some kv.Value
                | _ -> None)
        let patterns =
            match getNode "patterns" with
            | Some (:? YamlSequenceNode as seq) ->
                [ for item in seq.Children do
                      match item with
                      | :? YamlMappingNode as m ->
                          yield
                              { Id = defaultArg (field m "id") ""
                                Regex = defaultArg (field m "regex") ""
                                Severity = defaultArg (field m "severity") "block"
                                Reason = defaultArg (field m "reason") "" }
                      | _ -> () ]
            | _ -> []
        let whitelist =
            match getNode "whitelist" with
            | Some (:? YamlSequenceNode as seq) ->
                [ for item in seq.Children do
                      match item with
                      | :? YamlMappingNode as m when (field m "pattern_id").IsSome ->
                          yield
                              { PatternId = (field m "pattern_id").Value
                                FileGlob = field m "file_glob"
                                LineRegex = field m "line_regex" }
                      | _ -> () ]
            | _ -> []
        let overrides =
            match getNode "severity_overrides" with
            | Some (:? YamlMappingNode as m) ->
                m.Children
                |> Seq.choose (fun kv ->
                    match scalar kv.Key, scalar kv.Value with
                    | Some k, Some v when (v = "block" || v = "advisory") -> Some(k, v)
                    | _ -> None)
                |> Map.ofSeq
            | _ -> Map.empty
        patterns, whitelist, overrides

    /// Python fnmatch.translate semantics ("*" matches "/", anchored with \Z).
    let private fnmatchToRegex (pat: string) : string =
        let sb = System.Text.StringBuilder()
        sb.Append "(?s:" |> ignore
        let mutable i = 0
        let n = pat.Length
        while i < n do
            let c = pat.[i]
            i <- i + 1
            match c with
            | '*' -> sb.Append ".*" |> ignore
            | '?' -> sb.Append "." |> ignore
            | '[' ->
                let mutable j = i
                if j < n && (pat.[j] = '!' || pat.[j] = ']') then j <- j + 1
                while j < n && pat.[j] <> ']' do
                    j <- j + 1
                if j >= n then
                    sb.Append "\\[" |> ignore
                else
                    let mutable stuff = pat.Substring(i, j - i).Replace("\\", "\\\\")
                    i <- j + 1
                    if stuff.StartsWith("!") then stuff <- "^" + stuff.Substring(1)
                    elif stuff.StartsWith("^") then stuff <- "\\" + stuff
                    sb.Append('[').Append(stuff).Append(']') |> ignore
            | _ -> sb.Append(Regex.Escape(string c)) |> ignore
        sb.Append @")\Z" |> ignore
        sb.ToString()

    let private fnmatch (path: string) (glob: string) : bool =
        Regex.IsMatch(path, fnmatchToRegex glob)

    let private headerRe = Regex(@"^@@ -\d+(?:,\d+)? \+(\d+)(?:,\d+)? @@", RegexOptions.Compiled)

    let scan (baseRef: string option) (patternsYml: string) (unifiedDiff: string) : DiffScanResult =
        let patterns, whitelist, overrides = parsePatterns patternsYml
        for p in patterns do
            match overrides.TryFind p.Id with
            | Some s -> p.Severity <- s
            | None -> ()

        let hits = ResizeArray<DiffHit>()
        let mutable curFile : string option = None
        let mutable curLine = 0

        for raw in unifiedDiff.Replace("\r\n", "\n").Split('\n') do
            if raw.StartsWith("+++ b/") then
                curFile <- Some(raw.Substring 6)
            elif raw.StartsWith("+++ ") then
                curFile <- Some(raw.Substring 4)
            else
                let m = headerRe.Match raw
                if m.Success then
                    curLine <- int (m.Groups.[1].Value) - 1
                elif raw.StartsWith("+") && not (raw.StartsWith("+++")) then
                    curLine <- curLine + 1
                    let content = raw.Substring 1
                    match curFile with
                    | None -> ()
                    | Some file ->
                        for p in patterns do
                            let matched =
                                try
                                    Regex.IsMatch(content, p.Regex)
                                with _ -> false
                            if matched then
                                let whitelisted =
                                    whitelist
                                    |> List.exists (fun wl ->
                                        if wl.PatternId <> p.Id then
                                            false
                                        else
                                            let fgOk =
                                                match wl.FileGlob with
                                                | Some fg -> fnmatch file fg
                                                | None -> true
                                            let lrOk =
                                                match wl.LineRegex with
                                                | Some lr ->
                                                    (try Regex.IsMatch(content, lr) with _ -> false)
                                                | None -> true
                                            // Python: if fg and not match -> skip (continue);
                                            // if lr and not match -> skip. Both must pass to whitelist.
                                            fgOk && lrOk)
                                if not whitelisted then
                                    let trimmed = content.Trim()
                                    let m120 = if trimmed.Length > 120 then trimmed.Substring(0, 120) else trimmed
                                    hits.Add
                                        { File = file
                                          Line = curLine
                                          Pattern = p.Id
                                          Severity = p.Severity
                                          Reason = p.Reason
                                          Match = m120 }
                elif raw.StartsWith(" ") then
                    curLine <- curLine + 1

        { BaseRef = baseRef
          Blocking = hits |> Seq.filter (fun h -> h.Severity = "block") |> List.ofSeq
          Advisory = hits |> Seq.filter (fun h -> h.Severity = "advisory") |> List.ofSeq }

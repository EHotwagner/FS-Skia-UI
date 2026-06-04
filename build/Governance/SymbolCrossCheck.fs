module FS.Skia.UI.Build.SymbolCrossCheck

open System.Text
open System.Text.RegularExpressions

type SymbolKind =
    | MsgCase
    | UnionOrScreenVariant
    | EntityRecord
    | FrId
    | ScId

type Artifact =
    | Plan
    | DataModel
    | Tasks

type Symbol =
    { Kind: SymbolKind
      Name: string
      PresentIn: Set<Artifact> }

let kindLabel (k: SymbolKind) : string =
    match k with
    | MsgCase -> "msg-case"
    | UnionOrScreenVariant -> "union-or-screen-variant"
    | EntityRecord -> "entity-record"
    | FrId -> "fr-id"
    | ScId -> "sc-id"

let private artifactLabel (a: Artifact) : string =
    match a with
    | Plan -> "plan"
    | DataModel -> "data-model"
    | Tasks -> "tasks"

let private frRe = Regex(@"\bFR-\d{1,4}\b", RegexOptions.Compiled)
let private scRe = Regex(@"\bSC-\d{1,4}\b", RegexOptions.Compiled)
// A backtick-quoted PascalCase identifier — the deterministic structural-symbol
// token. Requiring backticks + a structural keyword on the line keeps extraction
// precise (no prose noise) while still catching the SI-6 finds (e.g. a `Msg` case
// referenced as `ViewerKeyEventReceived`).
let private backtickPascalRe = Regex(@"`([A-Z][A-Za-z0-9]+)`", RegexOptions.Compiled)

let private structuralKind (line: string) : SymbolKind option =
    let lower = line.ToLowerInvariant()
    if lower.Contains "msg" then Some MsgCase
    elif lower.Contains "screen" || lower.Contains "union" || lower.Contains "variant" then Some UnionOrScreenVariant
    elif lower.Contains "record" || lower.Contains "entity" then Some EntityRecord
    else None

let extract (artifact: Artifact) (text: string) : Symbol list =
    let acc = System.Collections.Generic.Dictionary<SymbolKind * string, unit>()
    let add kind name = acc.[(kind, name)] <- ()

    for m in frRe.Matches text do
        add FrId m.Value
    for m in scRe.Matches text do
        add ScId m.Value

    for rawLine in text.Replace("\r\n", "\n").Split('\n') do
        match structuralKind rawLine with
        | Some kind ->
            for m in backtickPascalRe.Matches rawLine do
                add kind m.Groups.[1].Value
        | None -> ()

    acc.Keys
    |> Seq.map (fun (kind, name) -> { Kind = kind; Name = name; PresentIn = Set.singleton artifact })
    |> List.ofSeq

let diff (plan: string) (dataModel: string) (tasks: string) : Symbol list =
    let all =
        [ extract Plan plan; extract DataModel dataModel; extract Tasks tasks ]
        |> List.concat

    // Merge PresentIn sets per (kind, name).
    let merged = System.Collections.Generic.Dictionary<SymbolKind * string, Set<Artifact>>()
    for s in all do
        let key = (s.Kind, s.Name)
        let existing = match merged.TryGetValue key with | true, v -> v | _ -> Set.empty
        merged.[key] <- Set.union existing s.PresentIn

    let allThree = set [ Plan; DataModel; Tasks ]

    merged
    |> Seq.choose (fun kv ->
        let (kind, name) = kv.Key
        let presentIn = kv.Value
        // A non-empty PROPER subset: present in some, missing from others.
        if not (Set.isEmpty presentIn) && presentIn <> allThree then
            Some { Kind = kind; Name = name; PresentIn = presentIn }
        else
            None)
    // Deterministic ordering: by kind label, then name.
    |> Seq.sortBy (fun s -> (kindLabel s.Kind, s.Name))
    |> List.ofSeq

let render (findings: Symbol list) : string =
    let sb = StringBuilder()
    sb.Append("## Symbol consistency (analyze pass G)\n") |> ignore
    if List.isEmpty findings then
        sb.Append("- no cross-artifact set-differences detected\n") |> ignore
    else
        let allThree = set [ Plan; DataModel; Tasks ]
        for s in findings do
            let present =
                s.PresentIn |> Set.toList |> List.map artifactLabel |> String.concat ", "
            let missing =
                Set.difference allThree s.PresentIn
                |> Set.toList
                |> List.map artifactLabel
                |> String.concat ", "
            let designOnly =
                // present only in design artifacts (data-model), absent from a spec FR:
                // flag for human judgment, never hard-fail (spec edge case).
                if s.PresentIn = Set.singleton DataModel then "   [design-only? human judgment]" else ""
            sb.Append(sprintf "- %s %s — in {%s}, missing from {%s}%s\n" (kindLabel s.Kind) s.Name present missing designOnly)
            |> ignore
    sb.ToString()

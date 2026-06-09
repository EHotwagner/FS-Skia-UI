module FS.Skia.UI.Build.PackageSkew

open System
open System.Text
open System.Text.RegularExpressions
open FS.Skia.UI.Build.Evidence

// PackageSkewFinding is the single-source value type from EvidenceFormatSchema (feature 087).

// --- pure core ----------------------------------------------------------------

// An F# identifier (type/member/field/case/module segment).
let private ident = @"[A-Za-z_][A-Za-z0-9_']*"

let surfaceSymbols (fsiText: string) : Set<string> =
    // Declaration sites in a `.fsi` surface that introduce a consumer-visible name. Each
    // pattern captures the declared identifier in group 1. We intentionally cast a wide net
    // (so any symbol a generated reference could resolve to is present), since the check
    // only fails on a referenced symbol that resolves NOWHERE in the surface.
    let patterns =
        [ // type / and-continued type declarations
          @"(?m)^\s*(?:type|and)\s+(?:\[<[^\]]*>\]\s*)*(?:private\s+|internal\s+)?(" + ident + @")"
          // module declarations
          @"(?m)^\s*module\s+(?:[A-Za-z0-9_.']+\.)?(" + ident + @")"
          // namespace segments (every dotted segment is resolvable)
          @"(?m)^\s*namespace\s+(.+)$"
          // members / vals / abstract members / static members / constructors
          @"(?:member|val|abstract|override|default|static\s+member)\s+(?:mutable\s+)?(?:[A-Za-z_][A-Za-z0-9_']*\.)?(" + ident + @")"
          // union cases (| Case) — leading bar then capitalised-or-any identifier
          @"(?m)^\s*\|\s*(" + ident + @")"
          // record / anonymous-record field labels:  Label :
          @"(?m)(?:^|\{|;)\s*(?:mutable\s+)?(" + ident + @")\s*:" ]

    let acc = System.Collections.Generic.HashSet<string>()

    for p in patterns do
        for m in Regex.Matches(fsiText, p) do
            let raw = m.Groups.[1].Value
            // namespace lines contribute each dotted segment
            if raw.Contains "." then
                for seg in raw.Split([| '.'; ' '; '\t' |], StringSplitOptions.RemoveEmptyEntries) do
                    acc.Add seg |> ignore
            else
                acc.Add raw |> ignore

    Set.ofSeq acc

// True when a dotted `FS.Skia.UI...` reference path is rooted at a surface-tracked
// consumer package (the longest package-id prefix that matches a tracked package). A
// reference rooted at a non-tracked package — notably the build-tooling `FS.Skia.UI.Build`
// — is out of scope for the consumer skew check and contributes nothing.
let private rootedInTracked (trackedPackages: Set<string>) (segs: string[]) : bool =
    seq { 3 .. segs.Length }
    |> Seq.exists (fun take -> Set.contains (String.Join(".", Array.truncate take segs)) trackedPackages)

let referencedSymbols (trackedPackages: Set<string>) (file: string) (sourceText: string) : (string * string) list =
    let acc = System.Collections.Generic.List<string>()

    let consider (segs: string[]) =
        // Only references rooted at a surface-tracked consumer package, with a symbol leaf
        // beyond the package id, carry a skew signal.
        if segs.Length > 3 && rootedInTracked trackedPackages segs then
            acc.Add segs.[segs.Length - 1]

    // (a) `open FS.Skia.UI.<...>` — the opened namespace's leaf must resolve.
    for m in Regex.Matches(sourceText, @"(?m)^\s*open\s+(FS\.Skia\.UI(?:\." + ident + @")*)") do
        consider (m.Groups.[1].Value.Split('.'))

    // (b) fully-qualified `FS.Skia.UI.<...>.<Symbol>` value/type references — the trailing
    //     symbol must resolve in the pinned surface.
    for m in Regex.Matches(sourceText, @"\bFS\.Skia\.UI(?:\." + ident + @")+") do
        consider (m.Value.Split('.'))

    acc |> Seq.map (fun s -> s, file) |> Seq.distinct |> List.ofSeq

let detectSkew
    (pinnedVersion: string)
    (localVersion: string)
    (pinnedSurface: Set<string>)
    (referenced: (string * string) list)
    : PackageSkewFinding list =
    referenced
    |> List.filter (fun (symbol, _) -> not (Set.contains symbol pinnedSurface))
    |> List.map (fun (symbol, file) ->
        { Symbol = symbol
          File = file
          PinnedVersion = pinnedVersion
          LocalVersion = localVersion })
    |> List.distinct

let renderFindings (pinnedVersion: string) (localVersion: string) (findings: PackageSkewFinding list) : string =
    let sb = StringBuilder()
    let line (s: string) = sb.Append(s).Append('\n') |> ignore
    line "# Package skew (pinned-vs-local) check"
    line ""
    line (sprintf "pinned-version=%s" pinnedVersion)
    line (sprintf "local-version=%s" localVersion)
    line (sprintf "version-gap=%s" (if pinnedVersion = localVersion then "none" else pinnedVersion + " -> " + localVersion))
    line ""

    if List.isEmpty findings then
        line "status=clean"
        line "findings=0"
        line "message=every referenced FS.Skia.UI symbol resolves in the pinned/published surface (no network restore)."
    else
        line "status=skew"
        line (sprintf "findings=%d" (List.length findings))

        for f in findings do
            line (sprintf "- symbol=%s file=%s pinned=%s local=%s (referenced symbol is absent from the pinned surface)" f.Symbol f.File f.PinnedVersion f.LocalVersion)

    sb.ToString()

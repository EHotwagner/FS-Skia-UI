module FS.Skia.UI.Build.TemplateUpdatePackage

open System
open System.Text.RegularExpressions

type PackableProject =
    { ProjectPath: string
      PackageId: string
      Leaf: string }

// `build/Governance/FS.Skia.UI.Build.fsproj` -> `Build`; `src/Controls.Elmish/Controls.Elmish.fsproj`
// -> `Controls.Elmish`. The project file stem already carries the package leaf except for
// the `FS.Skia.UI.Build` engine, whose stem is the full id.
let leafOfProject (projectPath: string) =
    let normalized = projectPath.Replace('\\', '/')

    let stem =
        let fileName =
            match normalized.LastIndexOf('/') with
            | -1 -> normalized
            | index -> normalized.Substring(index + 1)

        if fileName.EndsWith(".fsproj", StringComparison.Ordinal) then
            fileName.Substring(0, fileName.Length - ".fsproj".Length)
        else
            fileName

    let prefix = "FS.Skia.UI."

    if stem.StartsWith(prefix, StringComparison.Ordinal) then
        stem.Substring(prefix.Length)
    else
        stem

// Capture the leaves listed by a `for <var> in <leaves>; do` loop. Leaves may contain a
// dot (`Controls.Elmish`). Whichever loop variable the skill uses, the enumerated set is
// the same authoritative package list.
let private feedLoopPattern =
    Regex(@"for\s+\w+\s+in\s+([A-Za-z0-9._ ]+?)\s*;?\s*do", RegexOptions.Compiled)

let feedLoopLeaves (skillText: string) : string list =
    feedLoopPattern.Matches skillText
    |> Seq.collect (fun m ->
        m.Groups.[1].Value.Split([| ' '; '\t' |], StringSplitOptions.RemoveEmptyEntries))
    |> Seq.distinct
    |> Seq.toList

// The phantom bare-Lib check is the leaf-less `FS.Skia.UI.$v.nupkg` (or `${v}`) feed probe
// or the "(Lib)" / "the Lib package" phrasing 053 left stale.
let private phantomLibPattern =
    Regex(@"FS\.Skia\.UI\.\$\{?v\}?\.nupkg|the Lib package|MISS \(Lib\)|OK  \(Lib\)", RegexOptions.Compiled)

let hasPhantomLib (skillText: string) = phantomLibPattern.IsMatch skillText

let private staleCountPattern = Regex(@"nine repo packages", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)

let hasStaleCount (skillText: string) = staleCountPattern.IsMatch skillText

let check (packableLeaves: string list) (skillPath: string) (skillText: string) : string list =
    let packable = Set.ofList packableLeaves
    let feed = feedLoopLeaves skillText |> Set.ofList

    let missing =
        Set.difference packable feed
        |> Set.toList
        |> List.sort
        |> List.map (fun leaf ->
            sprintf
                "template-update skill missing FS.Skia.UI.%s from the step-5 feed loop (%s)"
                leaf
                skillPath)

    let phantom =
        Set.difference feed packable
        |> Set.toList
        |> List.sort
        |> List.map (fun leaf ->
            sprintf "template-update skill lists FS.Skia.UI.%s — not packable (%s)" leaf skillPath)

    let libPhantom =
        if hasPhantomLib skillText then
            [ sprintf
                  "template-update skill checks the phantom bare-Lib FS.Skia.UI package — 053 unpublished it (%s)"
                  skillPath ]
        else
            []

    let staleCount =
        if hasStaleCount skillText then
            [ sprintf
                  "template-update skill carries the stale \"nine repo packages\" count — the packable set is %d (%s)"
                  (List.length packableLeaves)
                  skillPath ]
        else
            []

    missing @ phantom @ libPhantom @ staleCount

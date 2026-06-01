module FS.Skia.UI.Build.ConstitutionFragments

open System
open System.Text.RegularExpressions

type PrincipleFragment =
    { FragmentId: string
      SourceHeading: string
      RenderedText: string }

type MarkerRegion =
    { FragmentId: string
      StartLine: int
      EndLine: int
      CurrentInner: string }

type FragmentCurrency =
    { TemplatePath: string
      StaleFragments: string list
      UnknownMarkers: string list
      MissingMarkers: string list }

let private regenCommand = "./fake.sh build -t RefreshSurfaceBaselines"

// The fixed fragment inventory, locked by the Phase-1 contract (data-model §3). Each
// fragment is anchored to a constitution `### <roman>.` heading; adding/removing a
// fragment is a contract change, not an implementation detail.
let private fragmentAnchors =
    [ "fsi-visibility", "### II."
      "mvu-boundary", "### IV."
      "synthetic-disclosure", "### V."
      "tests-first", "### VI." ]

let fragmentIds = fragmentAnchors |> List.map fst

let private normalizeNewlines (text: string) = text.Replace("\r\n", "\n")

/// First sentence of a principle body: skip blank lines after the heading, join the
/// first paragraph onto one line, take up to the first period followed by whitespace/EOL.
let private firstSentence (bodyLines: string list) =
    let paragraph =
        bodyLines
        |> List.skipWhile (fun l -> l.Trim() = "")
        |> List.takeWhile (fun l -> l.Trim() <> "")
        |> List.map (fun l -> l.Trim())
        |> String.concat " "

    let m = Regex.Match(paragraph, @"^(.*?\.)(\s|$)")
    if m.Success then m.Groups.[1].Value.Trim() else paragraph.Trim()

let extract (constitutionText: string) : PrincipleFragment list =
    let lines = (normalizeNewlines constitutionText).Split('\n') |> Array.toList

    fragmentAnchors
    |> List.map (fun (fragmentId, anchor) ->
        match lines |> List.tryFindIndex (fun l -> l.StartsWith(anchor, StringComparison.Ordinal)) with
        | None ->
            failwithf
                "ConstitutionFragments.extract: required principle heading '%s' (fragment %s) is missing from the constitution source (Principle VII)."
                anchor
                fragmentId
        | Some idx ->
            let headingLine = lines.[idx]
            let heading = headingLine.Substring(4).Trim() // drop the leading "### "
            let body = lines |> List.skip (idx + 1)
            let sentence = firstSentence body

            { FragmentId = fragmentId
              SourceHeading = heading
              RenderedText = sprintf "**%s** — %s" heading sentence })

// One BEGIN/END pair, capturing the marker id and the inner span (Singleline so `.`
// crosses newlines). The non-greedy inner stops at the first matching END marker.
let private regionRe =
    Regex(
        @"<!-- BEGIN GENERATED: constitution/(?<id>[^\s]+) -->(?<inner>.*?)<!-- END GENERATED: constitution/\k<id> -->",
        RegexOptions.Singleline ||| RegexOptions.Compiled)

let regions (templateText: string) : MarkerRegion list =
    let normalized = normalizeNewlines templateText

    [ for m in regionRe.Matches normalized ->
          let before = normalized.Substring(0, m.Index)
          let startLine = (before |> Seq.filter ((=) '\n') |> Seq.length) + 1
          let withinMatch = m.Value
          let endLine = startLine + (withinMatch |> Seq.filter ((=) '\n') |> Seq.length)

          { FragmentId = m.Groups.["id"].Value
            StartLine = startLine
            EndLine = endLine
            CurrentInner = m.Groups.["inner"].Value } ]

let private expectedInner (fragment: PrincipleFragment) =
    "\n" + fragment.RenderedText + "\n"

let splice (fragments: PrincipleFragment list) (templateText: string) : string =
    let byId = fragments |> List.map (fun f -> f.FragmentId, f) |> Map.ofList

    regionRe.Replace(
        templateText,
        (fun (m: Match) ->
            let id = m.Groups.["id"].Value

            match byId.TryFind id with
            | Some fragment ->
                let beginMarker = sprintf "<!-- BEGIN GENERATED: constitution/%s -->" id
                let endMarker = sprintf "<!-- END GENERATED: constitution/%s -->" id
                beginMarker + expectedInner fragment + endMarker
            | None ->
                // Unknown marker: leave it byte-for-byte untouched (currency reports it).
                m.Value))

let currency (templatePath: string) (fragments: PrincipleFragment list) (templateText: string) : FragmentCurrency =
    let foundRegions = regions templateText
    let regionIds = foundRegions |> List.map (fun r -> r.FragmentId) |> Set.ofList
    let fragmentById = fragments |> List.map (fun f -> f.FragmentId, f) |> Map.ofList
    let expectedIds = fragments |> List.map (fun f -> f.FragmentId) |> Set.ofList

    let stale =
        foundRegions
        |> List.choose (fun r ->
            match fragmentById.TryFind r.FragmentId with
            | Some fragment when normalizeNewlines r.CurrentInner <> expectedInner fragment -> Some r.FragmentId
            | _ -> None)
        |> List.distinct

    let unknown =
        foundRegions
        |> List.map (fun r -> r.FragmentId)
        |> List.filter (fun id -> not (expectedIds.Contains id))
        |> List.distinct

    let missing =
        fragments
        |> List.map (fun f -> f.FragmentId)
        |> List.filter (fun id -> not (regionIds.Contains id))

    { TemplatePath = templatePath
      StaleFragments = stale
      UnknownMarkers = unknown
      MissingMarkers = missing }

let currencyDrift (currency: FragmentCurrency) : string option =
    if List.isEmpty currency.StaleFragments
       && List.isEmpty currency.UnknownMarkers
       && List.isEmpty currency.MissingMarkers then
        None
    else
        let parts =
            [ if not (List.isEmpty currency.StaleFragments) then
                  yield sprintf "stale generated regions: %s" (String.concat ", " currency.StaleFragments)
              if not (List.isEmpty currency.MissingMarkers) then
                  yield sprintf "missing marker regions: %s" (String.concat ", " currency.MissingMarkers)
              if not (List.isEmpty currency.UnknownMarkers) then
                  yield sprintf "unknown marker regions (no source fragment): %s" (String.concat ", " currency.UnknownMarkers) ]

        Some(
            sprintf
                "%s constitution fragment(s) are stale — %s. Regenerate via %s."
                currency.TemplatePath
                (String.concat "; " parts)
                regenCommand)

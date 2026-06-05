module FS.Skia.UI.Build.Publish

open System
open System.Text
open System.Text.RegularExpressions
open FS.Skia.UI.Build.Engine.Model

// Feature 064 (FR-001/FR-002): pure publish-plan helpers. No env / network / process here —
// all I/O is at the Interpret edge. See Publish.fsi for the curated surface.

let defaultFeedUrl = "https://api.nuget.org/v3/index.json"

let isLocalFeedUrl (feedUrl: string) =
    not (
        feedUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
        || feedUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
    )

let deriveReadUrl (feedUrl: string) (isLocalFeed: bool) =
    if isLocalFeed then
        // A local-directory feed is read by listing `<dir>/<id>.<ver>.nupkg`.
        feedUrl
    else
        // nuget.org's anonymous read is the flat-container base; the service index URL
        // `.../v3/index.json` maps to `.../v3-flatcontainer`. Per-package reads append
        // `/<id-lowercase>/index.json` at the edge.
        let trimmed = feedUrl.TrimEnd('/')

        if trimmed.EndsWith("/v3/index.json", StringComparison.OrdinalIgnoreCase) then
            trimmed.Substring(0, trimmed.Length - "/v3/index.json".Length) + "/v3-flatcontainer"
        elif trimmed.EndsWith("/index.json", StringComparison.OrdinalIgnoreCase) then
            trimmed.Substring(0, trimmed.Length - "/index.json".Length) + "-flatcontainer"
        else
            trimmed

let private nonEmpty (value: string option) =
    match value with
    | Some v when not (String.IsNullOrWhiteSpace v) -> Some v
    | _ -> None

let configFromEnv (lookup: string -> string option) =
    let feedUrl =
        match nonEmpty (lookup "FSSKIA_PUBLISH_FEED") with
        | Some v -> v
        | None -> defaultFeedUrl

    let isLocalFeed = isLocalFeedUrl feedUrl

    { FeedUrl = feedUrl
      ReadUrl = deriveReadUrl feedUrl isLocalFeed
      // The credential is read from the environment like `gh` reads GH_TOKEN — either the
      // feature var FSSKIA_PUBLISH_API_KEY or NuGet's native NUGET_API_KEY (NuGet 7.6+); the
      // interpreter passes it to `dotnet nuget push` via NUGET_API_KEY, never on the command line.
      ApiKeyPresent = (nonEmpty (lookup "FSSKIA_PUBLISH_API_KEY")).IsSome || (nonEmpty (lookup "NUGET_API_KEY")).IsSome
      DryRun = (nonEmpty (lookup "FSSKIA_PUBLISH_DRYRUN")).IsSome
      IsLocalFeed = isLocalFeed }

let validateConfig (config: PublishConfig) =
    if not config.DryRun && not config.ApiKeyPresent then
        Some
            "No publish credential set (NUGET_API_KEY or FSSKIA_PUBLISH_API_KEY): a real push requires one and nothing was pushed. Set FSSKIA_PUBLISH_DRYRUN=1 for a credential-free dry-run, or export NUGET_API_KEY for a real push."
    else
        None

let buildPlan (packages: (string * string) list) (feedHasVersion: string -> string -> bool) =
    packages
    |> List.map (fun (packageId, version) ->
        let has = feedHasVersion packageId version

        { PackageId = packageId
          Version = version
          FeedHasVersion = has
          Decision = if has then Skip else Push })

let parseFlatContainerVersions (json: string) =
    if String.IsNullOrWhiteSpace json then
        Set.empty
    else
        let arrayMatch =
            Regex.Match(json, "\"versions\"\\s*:\\s*\\[(?<items>[^\\]]*)\\]", RegexOptions.CultureInvariant)

        if not arrayMatch.Success then
            Set.empty
        else
            Regex.Matches(arrayMatch.Groups.["items"].Value, "\"([^\"]+)\"")
            |> Seq.cast<Match>
            |> Seq.map (fun m -> m.Groups.[1].Value.ToLowerInvariant())
            |> Set.ofSeq

let private decisionLabel decision =
    match decision with
    | Push -> "Push"
    | Skip -> "Skip"

let renderPlan (config: PublishConfig) (rows: PublishPlanRow list) =
    let mode = if config.DryRun then "dry-run (no push)" else "real push"
    let pushCount = rows |> List.filter (fun r -> r.Decision = Push) |> List.length
    let skipCount = rows |> List.filter (fun r -> r.Decision = Skip) |> List.length

    let sb = StringBuilder()
    sb.AppendLine "# Publish plan" |> ignore
    sb.AppendLine "" |> ignore
    sb.AppendLine(sprintf "- feed: `%s`" config.FeedUrl) |> ignore
    sb.AppendLine(sprintf "- read-url: `%s`" config.ReadUrl) |> ignore
    sb.AppendLine(sprintf "- mode: %s" mode) |> ignore
    sb.AppendLine(sprintf "- api-key-present: %b" config.ApiKeyPresent) |> ignore
    sb.AppendLine(sprintf "- local-feed: %b" config.IsLocalFeed) |> ignore
    sb.AppendLine(sprintf "- packages: %d (push %d, skip %d)" (List.length rows) pushCount skipCount) |> ignore
    sb.AppendLine "" |> ignore
    sb.AppendLine "| PackageId | Version | feed-state | Decision |" |> ignore
    sb.AppendLine "|-----------|---------|------------|----------|" |> ignore

    for row in rows do
        let feedState = if row.FeedHasVersion then "present" else "absent"
        sb.AppendLine(sprintf "| `%s` | `%s` | %s | %s |" row.PackageId row.Version feedState (decisionLabel row.Decision))
        |> ignore

    sb.ToString()

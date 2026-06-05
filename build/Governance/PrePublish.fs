module FS.Skia.UI.Build.PrePublish

open System
open System.Text
open System.Text.RegularExpressions

// Feature 064 (FR-006/FR-010): pure pre-publish consistency rules. No I/O — the interpreter
// reads the files and assembles PrePublishInputs. See PrePublish.fsi for the surface.

type PrePublishRule =
    | PinParity
    | EnginePinMatch
    | NoMachineLocalPath
    | RequiredMetadata

type PrePublishFinding =
    { Package: string
      Field: string
      Rule: PrePublishRule
      Detail: string }

type PackageMetadata =
    { PackageId: string
      LicenseExpression: string option
      RepositoryUrl: string option
      Authors: string option
      Description: string option
      ReadmeFile: string option }

type PrePublishInputs =
    { ShippedVersions: (string * string) list
      EngineShippedVersion: string
      TemplateProps: string
      EngineVersionFromBuildFsx: string option
      ConsumerNuGetConfig: string
      Metadata: PackageMetadata list }

let ruleName rule =
    match rule with
    | PinParity -> "PinParity"
    | EnginePinMatch -> "EnginePinMatch"
    | NoMachineLocalPath -> "NoMachineLocalPath"
    | RequiredMetadata -> "RequiredMetadata"

// Resolve a pin Version="..." against the <FsSkiaUiVersion> property when it references it.
let private resolvePropertyVersion (props: string) (rawVersion: string) =
    let trimmed = rawVersion.Trim()

    if trimmed.Equals("$(FsSkiaUiVersion)", StringComparison.OrdinalIgnoreCase) then
        let m = Regex.Match(props, "<FsSkiaUiVersion>([^<]+)</FsSkiaUiVersion>", RegexOptions.CultureInvariant)
        if m.Success then Some(m.Groups.[1].Value.Trim()) else None
    else
        Some trimmed

let checkPinParity (inputs: PrePublishInputs) =
    let shipped = Map.ofList inputs.ShippedVersions

    // Every FS.Skia.UI.* PackageVersion pin in the template props must resolve to the
    // version being shipped for that package. Pins for packages not in the shipped set are
    // out of scope (the template legitimately omits some libs, e.g. FS.Skia.UI.Input).
    Regex.Matches(
        inputs.TemplateProps,
        "<PackageVersion\\s+Include=\"(FS\\.Skia\\.UI[^\"]*)\"\\s+Version=\"([^\"]+)\"",
        RegexOptions.CultureInvariant
    )
    |> Seq.cast<Match>
    |> Seq.choose (fun m ->
        let packageId = m.Groups.[1].Value
        let rawVersion = m.Groups.[2].Value

        match Map.tryFind packageId shipped with
        | None -> None
        | Some shippedVersion ->
            match resolvePropertyVersion inputs.TemplateProps rawVersion with
            | None ->
                Some
                    { Package = packageId
                      Field = "Directory.Packages.props pin"
                      Rule = PinParity
                      Detail =
                        sprintf
                            "pin Version=\"%s\" references <FsSkiaUiVersion> but the property is absent from Directory.Packages.props"
                            rawVersion }
            | Some resolved when resolved <> shippedVersion ->
                Some
                    { Package = packageId
                      Field = "Directory.Packages.props pin"
                      Rule = PinParity
                      Detail = sprintf "pin resolves to %s but the shipped version is %s" resolved shippedVersion }
            | Some _ -> None)
    |> Seq.toList

let checkEnginePinMatch (inputs: PrePublishInputs) =
    match inputs.EngineVersionFromBuildFsx with
    | None ->
        [ { Package = "FS.Skia.UI.Build"
            Field = "build.fsx / FsSkiaUiVersion"
            Rule = EnginePinMatch
            Detail =
              "build.fsx's engine version could not be resolved (no literal #r version and no <FsSkiaUiVersion> property to read)" } ]
    | Some resolved when resolved <> inputs.EngineShippedVersion ->
        [ { Package = "FS.Skia.UI.Build"
            Field = "build.fsx / FsSkiaUiVersion"
            Rule = EnginePinMatch
            Detail =
              sprintf "build.fsx resolves engine version %s but the shipped FS.Skia.UI.Build version is %s" resolved inputs.EngineShippedVersion } ]
    | Some _ -> []

let checkNoMachineLocalPath (inputs: PrePublishInputs) =
    let config = inputs.ConsumerNuGetConfig

    let hasLocalSourceKey = Regex.IsMatch(config, "key=\"local\"", RegexOptions.CultureInvariant)
    let hasNugetLocalDir = config.IndexOf("nuget-local", StringComparison.OrdinalIgnoreCase) >= 0
    // an absolute path value: POSIX (/...) or Windows (C:\...).
    let hasAbsolutePathValue =
        Regex.IsMatch(config, "value=\"(/|[A-Za-z]:\\\\)[^\"]*\"", RegexOptions.CultureInvariant)

    if hasLocalSourceKey || hasNugetLocalDir || hasAbsolutePathValue then
        [ { Package = "template"
            Field = "NuGet.config:local"
            Rule = NoMachineLocalPath
            Detail =
              "the consumer-emitted NuGet.config carries a machine-local feed source (key=\"local\" / nuget-local / an absolute path); a fresh consumer has no such directory and restore would fail (FR-003)" } ]
    else
        []

let checkRequiredMetadata (inputs: PrePublishInputs) =
    let blank value = match value with | Some v when not (String.IsNullOrWhiteSpace v) -> false | _ -> true

    inputs.Metadata
    |> List.collect (fun meta ->
        [ if blank meta.LicenseExpression then "license expression"
          if blank meta.RepositoryUrl then "repository URL"
          if blank meta.Authors then "authors"
          if blank meta.Description then "description"
          if blank meta.ReadmeFile then "README" ]
        |> List.map (fun field ->
            { Package = meta.PackageId
              Field = field
              Rule = RequiredMetadata
              Detail = sprintf "%s carries no %s; a public nuget.org listing requires it (FR-010)" meta.PackageId field }))

let check (inputs: PrePublishInputs) =
    checkPinParity inputs
    @ checkEnginePinMatch inputs
    @ checkNoMachineLocalPath inputs
    @ checkRequiredMetadata inputs

let render (findings: PrePublishFinding list) =
    let sb = StringBuilder()
    sb.AppendLine "# Pre-publish consistency check" |> ignore
    sb.AppendLine "" |> ignore

    if List.isEmpty findings then
        sb.AppendLine "Status: PASS — no findings; the release set is internally consistent and the publish may proceed."
        |> ignore
    else
        sb.AppendLine(sprintf "Status: FAIL — %d finding(s); the publish is aborted." (List.length findings))
        |> ignore
        sb.AppendLine "" |> ignore
        sb.AppendLine "| Rule | Package | Field | Detail |" |> ignore
        sb.AppendLine "|------|---------|-------|--------|" |> ignore

        for f in findings do
            sb.AppendLine(sprintf "| %s | `%s` | %s | %s |" (ruleName f.Rule) f.Package f.Field f.Detail)
            |> ignore

    sb.ToString()

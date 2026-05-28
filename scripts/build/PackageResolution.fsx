module BuildPackageResolution

open System
open System.IO
open System.Text.RegularExpressions
open BuildPaths

let trimQuotes (value: string) =
    value.Trim().Trim('"').Trim('\'')

let parseScalar (line: string) =
    match line.IndexOf(':') with
    | index when index >= 0 -> line.Substring(index + 1) |> trimQuotes
    | _ -> ""

let parseInlineList (value: string) =
    let trimmed = value.Trim()

    if trimmed.StartsWith("[") && trimmed.EndsWith("]") then
        trimmed.Trim('[', ']').Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)
        |> Array.map trimQuotes
        |> Array.toList
    elif String.IsNullOrWhiteSpace trimmed then
        []
    else
        [ trimQuotes trimmed ]

let readNuGetPackageSources root =
    let configPath = path [ root; "NuGet.config" ]

    if not (File.Exists configPath) then
        []
    else
        let content = File.ReadAllText configPath

        Regex.Matches(content, "<add\\s+key=\"[^\"]+\"\\s+value=\"([^\"]+)\"")
        |> Seq.cast<Match>
        |> Seq.map (fun m -> m.Groups.[1].Value)
        |> Seq.toList

let readRestoreWarnings logPath =
    if not (File.Exists logPath) then
        []
    else
        File.ReadAllLines logPath
        |> Array.filter (fun line -> line.IndexOf("NU1603", StringComparison.OrdinalIgnoreCase) >= 0)
        |> Array.distinct
        |> Array.toList

let packageResolutionDiagnostics (requested: (string * string) list) (resolved: (string * string) list) (sources: string list) (restoreWarnings: string list) =
    let resolvedMap = resolved |> Map.ofList

    let drift =
        requested
        |> List.choose (fun (packageId, version) ->
            match resolvedMap |> Map.tryFind packageId with
            | Some actual when actual = version -> None
            | Some actual -> Some $"package mismatch: {packageId} requested={version} resolved={actual}"
            | None -> Some $"package mismatch: {packageId} requested={version} resolved=missing")

    let diagnostics =
        [ if List.isEmpty sources then
              "missing package sources"
          for warning in restoreWarnings do
              $"restore warning: {warning}"
          yield! drift ]

    let failureReason =
        if restoreWarnings |> List.exists (fun warning -> warning.IndexOf("NU1603", StringComparison.OrdinalIgnoreCase) >= 0) then
            Some "NU1603"
        elif not drift.IsEmpty then
            Some "version-mismatch"
        elif List.isEmpty sources then
            Some "missing-package-sources"
        else
            None

    failureReason, diagnostics

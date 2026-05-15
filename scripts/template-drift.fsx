open System
open System.Diagnostics
open System.IO

let scriptDir = __SOURCE_DIRECTORY__
let repositoryRoot = Directory.GetParent(scriptDir).FullName

let path segments =
    segments |> Array.ofList |> Path.Combine

let args =
    let raw = Environment.GetCommandLineArgs() |> Array.skip 1 |> Array.toList

    match raw with
    | script :: rest when script.EndsWith(".fsx", StringComparison.OrdinalIgnoreCase) -> rest
    | other -> other

let writeOutput (outputPath: string) content =
    if outputPath.EndsWith(".fsx", StringComparison.OrdinalIgnoreCase) then
        failwithf "Refusing to write report over script file: %s" outputPath

    match Path.GetDirectoryName outputPath |> Option.ofObj with
    | Some directory when directory <> "" -> Directory.CreateDirectory directory |> ignore
    | _ -> ()

    File.WriteAllText(outputPath, content + Environment.NewLine)

let outputPath, fixture =
    match args with
    | "--fixture" :: name :: output :: _ -> output, Some name
    | output :: _ -> output, None
    | [] -> path [ repositoryRoot; "specs"; "007-v2-template-packaging"; "readiness"; "template-drift.md" ], None

match fixture with
| Some "invalid-deferral" ->
    let report =
        "# Template Drift Report\n\nFAIL: deferral `fixture-invalid` is missing owner and target_phase."

    writeOutput outputPath report
    failwith "invalid deferral record: missing owner and target_phase"
| Some other -> failwithf "Unknown template-drift fixture: %s" other
| None -> ()

let runGit (arguments: string) =
    let startInfo = ProcessStartInfo("git", arguments)
    startInfo.WorkingDirectory <- repositoryRoot
    startInfo.RedirectStandardOutput <- true
    startInfo.RedirectStandardError <- true
    startInfo.UseShellExecute <- false

    use proc =
        match Process.Start startInfo |> Option.ofObj with
        | Some proc -> proc
        | None -> failwithf "Could not start git %s" arguments

    let stdout = proc.StandardOutput.ReadToEnd()
    let stderr = proc.StandardError.ReadToEnd()
    proc.WaitForExit() |> ignore

    if proc.ExitCode <> 0 then
        failwithf "git %s failed: %s" arguments stderr

    stdout

let changedPaths =
    runGit "status --short --untracked-files=all"
    |> fun text -> text.Split([| '\n'; '\r' |], StringSplitOptions.RemoveEmptyEntries)
    |> Array.choose (fun line ->
        if line.Length < 4 then
            None
        else
            let pathText = line.Substring(3).Trim()
            let pathText =
                if pathText.Contains(" -> ", StringComparison.Ordinal) then
                    pathText.Split([| " -> " |], StringSplitOptions.None) |> Array.last
                else
                    pathText

            Some(pathText.Replace('\\', '/')))
    |> Array.toList

let startsWithAny (prefixes: string list) (value: string) =
    prefixes |> List.exists (fun prefix -> value.StartsWith(prefix, StringComparison.Ordinal))

let templateOwnedPrefixes =
    [ ".template.config/"
      ".template.package/"
      ".specify/templates/"
      ".specify/presets/fsharp-opinionated/"
      ".specify/workflows/"
      "src/"
      "tests/"
      "samples/"
      "docs/"
      "scripts/dependency-report.fsx"
      "scripts/template-drift.fsx"
      "Directory.Build.props"
      "Directory.Packages.props"
      "build.fsx"
      "fake.sh"
      "fake.cmd"
      "README.md" ]

let alignmentPrefixes =
    [ ".template.config/template.json"
      "docs/template-profile.md"
      "docs/dependencies.md"
      "docs/speckit.md"
      "docs/build.md"
      "docs/testing.md"
      "docs/evidence.md"
      ".specify/templates/"
      ".specify/presets/fsharp-opinionated/templates/"
      ".specify/workflows/"
      "Directory.Packages.props"
      "build.fsx"
      "scripts/dependency-report.fsx"
      "scripts/template-drift.fsx"
      "readiness/template-deferrals.yml" ]

let templateOwnedChanges =
    changedPaths
    |> List.filter (startsWithAny templateOwnedPrefixes)

let hasAlignmentChange =
    changedPaths |> List.exists (startsWithAny alignmentPrefixes)

let deferralsPath = path [ repositoryRoot; "readiness"; "template-deferrals.yml" ]

let validateDeferrals () =
    if not (File.Exists deferralsPath) then
        [ "readiness/template-deferrals.yml is missing" ]
    else
        let lines = File.ReadAllLines deferralsPath |> Array.toList

        if lines |> List.exists (fun line -> line.Trim() = "accepted_deferrals: []") then
            []
        else
            let records =
                lines
                |> List.fold
                    (fun (records, current) line ->
                        let trimmed = line.Trim()

                        if trimmed.StartsWith("- id:", StringComparison.Ordinal) then
                            ((current |> Set.ofList) :: records, [ "id" ])
                        elif trimmed.StartsWith("paths:", StringComparison.Ordinal) then
                            (records, "paths" :: current)
                        elif trimmed.StartsWith("rationale:", StringComparison.Ordinal) then
                            (records, "rationale" :: current)
                        elif trimmed.StartsWith("owner:", StringComparison.Ordinal) then
                            (records, "owner" :: current)
                        elif trimmed.StartsWith("target_phase:", StringComparison.Ordinal) then
                            (records, "target_phase" :: current)
                        else
                            (records, current))
                    ([], [])
                |> fun (records, current) ->
                    if List.isEmpty current then records else (current |> Set.ofList) :: records
                |> List.filter (fun record -> record.Contains "id")

            records
            |> List.mapi (fun index record ->
                let required = [ "id"; "paths"; "rationale"; "owner"; "target_phase" ] |> Set.ofList
                let missing = Set.difference required record

                if Set.isEmpty missing then
                    None
                else
                    let missingFields = String.Join(", ", missing)
                    Some $"deferral #{index + 1} is missing {missingFields}")
            |> List.choose id

let deferralViolations = validateDeferrals ()

let driftViolations =
    if List.isEmpty templateOwnedChanges || hasAlignmentChange then
        []
    else
        [ "template-owned changes detected without template, docs, dependency, guidance, command, or deferral alignment" ]

let violations = deferralViolations @ driftViolations

let report =
    [ yield "# Template Drift Report"
      yield ""
      yield if List.isEmpty violations then "PASS" else "FAIL"
      yield ""
      yield "## Changed Template-Owned Paths"
      yield ""
      if List.isEmpty templateOwnedChanges then
          yield "- none"
      else
          yield! templateOwnedChanges |> List.map (fun changed -> "- `" + changed + "`")
      yield ""
      yield "## Alignment"
      yield ""
      yield $"- Alignment change present: {hasAlignmentChange}"
      yield $"- Deferral file: `{deferralsPath}`"
      yield ""
      yield "## Diagnostics"
      yield ""
      if List.isEmpty violations then
          yield "- No drift blockers."
      else
          yield! violations |> List.map (fun violation -> "- " + violation) ]
    |> String.concat Environment.NewLine

writeOutput outputPath report

if not (List.isEmpty violations) then
    failwithf "Template drift failed:%s%s" Environment.NewLine (String.Join(Environment.NewLine, violations))

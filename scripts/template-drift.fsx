open System
open System.Diagnostics
open System.IO
open System.Text.Json

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
    | [] -> path [ repositoryRoot; "specs"; "008-targeted-refactor-governance"; "readiness"; "template-drift.md" ], None

let featureDirectory () =
    let featureJson = path [ repositoryRoot; ".specify"; "feature.json" ]

    if File.Exists featureJson then
        use document = JsonDocument.Parse(File.ReadAllText featureJson)

        match document.RootElement.TryGetProperty("feature_directory") with
        | true, value when not (String.IsNullOrWhiteSpace(value.GetString())) -> value.GetString().Replace('\\', '/')
        | _ -> "specs/008-targeted-refactor-governance"
    else
        "specs/008-targeted-refactor-governance"

let activeFeatureDir = featureDirectory ()

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

let changedPathsFromGit () =
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

let changedPaths =
    match fixture with
    | Some "missing-alignment" -> [ "src/Lib/Library.fs" ]
    | Some "invalid-deferral" -> [ "scripts/template-drift.fsx"; "readiness/template-deferrals.yml" ]
    | Some other -> failwithf "Unknown template-drift fixture: %s" other
    | None -> changedPathsFromGit ()

let startsWithAny (prefixes: string list) (value: string) =
    prefixes |> List.exists (fun prefix -> value.StartsWith(prefix, StringComparison.Ordinal))

type PathClass =
    { Name: string
      Prefixes: string list
      RequiredAlignmentClasses: string list
      FeatureEvidenceTerms: string list }

type AlignmentClass =
    { Name: string
      Prefixes: string list }

let pathClasses =
    [ { Name = "template-manifest"
        Prefixes = [ ".template.config/"; ".template.package/" ]
        RequiredAlignmentClasses = [ "template-profile"; "active-feature-evidence" ]
        FeatureEvidenceTerms = [ "template"; ".template.config"; ".template.package" ] }
      { Name = "spec-kit-guidance"
        Prefixes = [ ".specify/templates/"; ".specify/presets/fsharp-opinionated/templates/" ]
        RequiredAlignmentClasses = [ "generated-guidance"; "active-feature-evidence" ]
        FeatureEvidenceTerms = [ ".specify/templates"; "generated guidance"; "spec-template"; "plan-template" ] }
      { Name = "spec-kit-workflow"
        Prefixes = [ ".specify/workflows/" ]
        RequiredAlignmentClasses = [ "speckit-docs"; "active-feature-evidence" ]
        FeatureEvidenceTerms = [ ".specify/workflows"; "workflow" ] }
      { Name = "source-code"
        Prefixes = [ "src/" ]
        RequiredAlignmentClasses = [ "source-contract"; "active-feature-evidence" ]
        FeatureEvidenceTerms = [ "src/"; "runtime"; "layout"; "charts"; "public surface" ] }
      { Name = "test-code"
        Prefixes = [ "tests/" ]
        RequiredAlignmentClasses = [ "test-evidence"; "active-feature-evidence" ]
        FeatureEvidenceTerms = [ "tests/"; "test evidence"; "focused tests" ] }
      { Name = "sample-code"
        Prefixes = [ "samples/" ]
        RequiredAlignmentClasses = [ "sample-contract"; "active-feature-evidence" ]
        FeatureEvidenceTerms = [ "samples/"; "sample" ] }
      { Name = "documentation"
        Prefixes = [ "docs/"; "README.md" ]
        RequiredAlignmentClasses = [ "docs-alignment"; "active-feature-evidence" ]
        FeatureEvidenceTerms = [ "docs/"; "README"; "documentation" ] }
      { Name = "dependency-policy"
        Prefixes = [ "Directory.Packages.props"; "Directory.Build.props"; "scripts/dependency-report.fsx" ]
        RequiredAlignmentClasses = [ "dependency-docs"; "active-feature-evidence" ]
        FeatureEvidenceTerms = [ "dependency"; "Directory.Packages.props" ] }
      { Name = "command-surface"
        Prefixes = [ "build.fsx"; "fake.sh"; "fake.cmd" ]
        RequiredAlignmentClasses = [ "command-docs"; "active-feature-evidence" ]
        FeatureEvidenceTerms = [ "build.fsx"; "Dev"; "Verify"; "Ci"; "command-surface" ] }
      { Name = "governance-script"
        Prefixes = [ "scripts/template-drift.fsx" ]
        RequiredAlignmentClasses = [ "template-drift-docs"; "active-feature-evidence" ]
        FeatureEvidenceTerms = [ "scripts/template-drift.fsx"; "TemplateDrift"; "template drift" ] } ]

let alignmentClasses =
    [ { Name = "template-profile"
        Prefixes = [ ".template.config/template.json"; "docs/template-profile.md" ] }
      { Name = "generated-guidance"
        Prefixes = [ ".specify/templates/"; ".specify/presets/fsharp-opinionated/templates/"; "docs/speckit.md" ] }
      { Name = "speckit-docs"
        Prefixes = [ "docs/speckit.md"; ".specify/workflows/" ] }
      { Name = "source-contract"
        Prefixes = [ activeFeatureDir + "/spec.md"; activeFeatureDir + "/plan.md"; activeFeatureDir + "/readiness/" ] }
      { Name = "test-evidence"
        Prefixes = [ "docs/testing.md"; activeFeatureDir + "/readiness/"; "tests/" ] }
      { Name = "sample-contract"
        Prefixes = [ "docs/testing.md"; "README.md"; activeFeatureDir + "/readiness/sample-smoke/" ] }
      { Name = "docs-alignment"
        Prefixes = [ "docs/"; "README.md"; activeFeatureDir + "/readiness/" ] }
      { Name = "dependency-docs"
        Prefixes = [ "docs/dependencies.md"; "Directory.Packages.props"; activeFeatureDir + "/readiness/" ] }
      { Name = "command-docs"
        Prefixes = [ "docs/build.md"; "docs/evidence.md"; "build.fsx"; activeFeatureDir + "/readiness/build-organization.md" ] }
      { Name = "template-drift-docs"
        Prefixes = [ "scripts/template-drift.fsx"; "docs/template-profile.md"; activeFeatureDir + "/readiness/template-drift.md" ] }
      { Name = "active-feature-evidence"
        Prefixes = [ activeFeatureDir + "/spec.md"; activeFeatureDir + "/plan.md"; activeFeatureDir + "/readiness/" ] } ]

let tryClassify changedPath =
    pathClasses
    |> List.tryFind (fun pathClass -> startsWithAny pathClass.Prefixes changedPath)

let alignmentClassByName name =
    alignmentClasses
    |> List.find (fun alignmentClass -> alignmentClass.Name = name)

let templateOwnedChanges =
    changedPaths
    |> List.choose (fun changed ->
        tryClassify changed
        |> Option.map (fun pathClass -> changed, pathClass))

let changedAlignmentClasses =
    alignmentClasses
    |> List.choose (fun alignmentClass ->
        if changedPaths |> List.exists (startsWithAny alignmentClass.Prefixes) then
            Some alignmentClass.Name
        else
            None)
    |> Set.ofList

let activeFeatureEvidenceText () =
    let root = path [ repositoryRoot; activeFeatureDir ]

    if Directory.Exists root then
        Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
        |> Seq.filter (fun file ->
            let extension = Path.GetExtension(file)
            extension = ".md" || extension = ".txt" || extension = ".yml" || extension = ".json")
        |> Seq.map File.ReadAllText
        |> String.concat Environment.NewLine
    else
        ""

let featureEvidence = activeFeatureEvidenceText ()

let controlsBoundaryGuidanceFiles =
    [ "template/fragments/controls/README.md"
      "template/fragments/controls/skill/SKILL.md"
      "template/fragments/elmish/README.md"
      "template/base/README.md"
      "template/base/docs/product.md"
      "src/Controls/skill/SKILL.md"
      ".specify/templates/spec-template.md"
      ".specify/templates/plan-template.md"
      ".specify/presets/fsharp-opinionated/templates/spec-template.md"
      ".specify/presets/fsharp-opinionated/templates/plan-template.md" ]

let controlsBoundaryGuidanceText () =
    controlsBoundaryGuidanceFiles
    |> List.choose (fun relative ->
        let absolute = path [ repositoryRoot; relative ]

        if File.Exists absolute then
            Some(File.ReadAllText absolute)
        else
            None)
    |> String.concat Environment.NewLine

let controlsBoundaryGuidanceViolations () =
    let combined = controlsBoundaryGuidanceText ()
    let removedChartsPackage = "FS.Skia.UI." + "Charts"
    let removedChartsSkill = "fs-skia-" + "charts"

    let required =
        [ "FS.Skia.UI.Controls"
          "Skia-rendered"
          "DataGrid"
          "FS.Skia.UI.Controls.Elmish"
          "ControlsElmish.program"
          "legacy Charts package"
          "no compatibility shim" ]

    let forbidden =
        [ removedChartsPackage
          removedChartsSkill
          ("chart-" + "only")
          ("DataGrid " + "as chart")
          ("DataGrid-" + "as-chart")
          ("renderer-" + "neutral")
          ("renderer " + "neutral")
          ("host-" + "loop ownership")
          ("host loop " + "ownership") ]

    [ yield!
          required
          |> List.choose (fun term ->
              if combined.IndexOf(term, StringComparison.OrdinalIgnoreCase) < 0 then
                  Some $"generated controls guidance missing `{term}` [controls-boundary-guidance]"
              else
                  None)
      yield!
          forbidden
          |> List.choose (fun term ->
              if combined.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0 then
                  Some $"generated controls guidance contains stale term `{term}` [controls-boundary-guidance]"
              else
                  None) ]

let evidenceMentionsChange changedPath (pathClass: PathClass) =
    let candidates =
        changedPath
        :: pathClass.Name
        :: pathClass.FeatureEvidenceTerms

    candidates
    |> List.exists (fun term -> featureEvidence.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)

let deferralsPath = path [ repositoryRoot; "readiness"; "template-deferrals.yml" ]

let validateDeferrals () =
    if fixture = Some "invalid-deferral" then
        [ "deferral `fixture-invalid` is missing owner, target_phase" ]
    elif not (File.Exists deferralsPath) then
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

let classViolations =
    templateOwnedChanges
    |> List.collect (fun (changedPath, pathClass) ->
        pathClass.RequiredAlignmentClasses
        |> List.choose (fun required ->
            let hasAlignment = changedAlignmentClasses.Contains required

            let hasFeatureEvidence =
                required <> "active-feature-evidence" || evidenceMentionsChange changedPath pathClass

            if hasAlignment && hasFeatureEvidence then
                None
            elif required = "active-feature-evidence" then
                Some $"{changedPath}: path class `{pathClass.Name}` is missing active feature evidence naming the changed path or affected feature area; required alignment class `{required}`."
            else
                Some $"{changedPath}: path class `{pathClass.Name}` is missing same-diff required alignment class `{required}`." ))

let controlsBoundaryViolations = controlsBoundaryGuidanceViolations ()

let violations = validateDeferrals () @ classViolations @ controlsBoundaryViolations

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
          yield "| Path | Path Class |"
          yield "|------|------------|"
          yield!
              templateOwnedChanges
              |> List.map (fun (changed, pathClass) -> $"| `{changed}` | `{pathClass.Name}` |")
      yield ""
      yield "## Required Alignment Classes"
      yield ""
      if List.isEmpty templateOwnedChanges then
          yield "- none"
      else
          yield!
              templateOwnedChanges
              |> List.collect (fun (changed, pathClass) ->
                  pathClass.RequiredAlignmentClasses
                  |> List.map (fun required -> $"- `{changed}` requires `{required}`"))
      yield ""
      yield "## Alignment"
      yield ""
      let changedAlignmentText = String.Join(", ", changedAlignmentClasses)
      yield $"- Changed alignment classes: `{changedAlignmentText}`"
      yield $"- Deferral file: `{deferralsPath}`"
      yield $"- Active feature evidence: `{activeFeatureDir}`"
      yield ""
      yield "## Controls Boundary Guidance"
      yield ""
      if List.isEmpty controlsBoundaryViolations then
          yield "- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms."
      else
          yield! controlsBoundaryViolations |> List.map (fun violation -> "- " + violation)
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

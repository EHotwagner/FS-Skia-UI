module FS.Skia.UI.Build.Guidance

open System
open System.IO
open System.Text.RegularExpressions
open BuildPaths
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Engine.Model
open FS.Skia.UI.Build.Front.Helpers

// Relocated verbatim from build.fsx (feature 045, T012): generated-guidance /
// skill-section markdown scanners. Behaviour-preserving.

type GuidanceArtifact =
    | SpecTemplate
    | PlanTemplate

type GuidancePrompt =
    { Class: string
      Section: string
      Prompt: string }

type GuidanceTemplate =
    { Path: string
      Artifact: GuidanceArtifact
      Prompts: GuidancePrompt list }

type MarkdownSection =
    { Heading: string
      Level: int
      Content: string }

let specGuidancePrompts =
    [ "package impact"
      "public contract impact"
      "state workflow impact"
      "layout/rendering impact"
      "evidence obligations"
      "unsupported scope"
      "build-target impact" ]
    |> List.map (fun prompt ->
        { Class = prompt
          Section = "Framework Governance Prompts"
          Prompt = prompt })

let planGuidancePrompts =
    [ "template ownership"
      "dependency impact"
      "command-surface impact"
      "generated project impact"
      "evidence paths"
      ".fsi"
      "MVU/effect boundary"
      "synthetic evidence"
      "test evidence"
      "observability"
      "deferred scope" ]
    |> List.map (fun prompt ->
        { Class = prompt
          Section = "Repository Governance Decisions"
          Prompt = prompt })

let generatedGuidanceRequirements =
    [ { Path = ".specify/templates/spec-template.md"
        Artifact = SpecTemplate
        Prompts = specGuidancePrompts }
      { Path = ".specify/presets/fsharp-opinionated/templates/spec-template.md"
        Artifact = SpecTemplate
        Prompts = specGuidancePrompts }
      { Path = ".specify/templates/plan-template.md"
        Artifact = PlanTemplate
        Prompts = planGuidancePrompts }
      { Path = ".specify/presets/fsharp-opinionated/templates/plan-template.md"
        Artifact = PlanTemplate
        Prompts = planGuidancePrompts } ]

let containsText (needle: string) (haystack: string) =
    haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0

let serializedRunnerRequiredTerms =
    [ "FAKE-backed"
      ".fake"
      "sequential"
      "not safe to run concurrently" ]

let buildRunnerCommandRegex =
    Regex(@"(\./fake\.sh|fake\.cmd|dotnet fake)\b", RegexOptions.IgnoreCase)

let numberedBuildRunnerCommandRegex =
    Regex(@"(?m)^\s*(\d+\.|-)\s+(`)?(\./fake\.sh|fake\.cmd|dotnet fake)\b", RegexOptions.IgnoreCase)

let validateSerializedRunnerGuidancePath model relativePath =
    let filePath = path [ model.RepositoryRoot; relativePath ]

    if not (File.Exists filePath) then
        [ $"{relativePath}: missing file [sequential-fake-guidance]" ]
    else
        let content = File.ReadAllText filePath

        if not (buildRunnerCommandRegex.IsMatch content) then
            []
        else
            [ yield!
                  serializedRunnerRequiredTerms
                  |> List.choose (fun term ->
                      if containsText term content then
                          None
                      else
                          Some $"{relativePath}: missing `{term}` [sequential-fake-guidance]")

              if buildRunnerCommandRegex.Matches(content).Count > 1
                 && numberedBuildRunnerCommandRegex.Matches(content).Count < 2 then
                  $"{relativePath}: multiple FAKE-backed commands require deterministic sequential order [sequential-fake-guidance]"

              if containsText "parallel" content
                 && not (containsText "non-FAKE" content || containsText "do not invoke FAKE" content) then
                  $"{relativePath}: parallelism guidance must distinguish safe non-FAKE checks [sequential-fake-guidance]" ]

let validateSerializedRunnerGuidance model =
    [ "README.md"
      "docs/reports/build.md"
      "docs/reports/testing.md"
      "docs/reports/evidence.md"
      "AGENTS.md"
      "CLAUDE.md"
      ".agents/skills/speckit-implement/SKILL.md"
      ".agents/skills/speckit-evidence-graph/SKILL.md"
      ".agents/skills/speckit-evidence-audit/SKILL.md"
      ".claude/skills/speckit-implement/SKILL.md"
      ".claude/skills/speckit-evidence-graph/SKILL.md"
      ".claude/skills/speckit-evidence-audit/SKILL.md"
      ".specify/templates/tasks-template.md"
      ".specify/presets/fsharp-opinionated/templates/tasks-template.md"
      ".specify/templates/plan-template.md"
      ".specify/presets/fsharp-opinionated/templates/plan-template.md"
      "template/base/README.md"
      "template/base/docs/product.md"
      "template/base/.agents/skills/fs-skia-project/SKILL.md"
      "template/base/.claude/skills/fs-skia-project/SKILL.md" ]
    |> List.collect (validateSerializedRunnerGuidancePath model)

let forbiddenGeneratedGuidanceAdvice =
    [ "assembly reflection first", "reflection-first"
      "reflection-first", "reflection-first"
      "copy files from repository src/", "repository-source-copy"
      "copy repository source", "repository-source-copy"
      "read repository source instead", "repository-source-copy" ]

let validateForbiddenGeneratedGuidanceAdvice model =
    let guidancePaths =
        [ "docs/reports/generated-apps.md"
          "docs/reports/controls.md"
          "template/base/README.md"
          "template/base/docs/product.md"
          "template/base/.agents/skills/fs-skia-project/SKILL.md"
          "template/base/.claude/skills/fs-skia-project/SKILL.md" ]

    guidancePaths
    |> List.collect (fun relativePath ->
        let filePath = path [ model.RepositoryRoot; relativePath ]

        if not (File.Exists filePath) then
            []
        else
            let content = File.ReadAllText filePath

            forbiddenGeneratedGuidanceAdvice
            |> List.choose (fun (term, failureClass) ->
                if containsText term content then
                    Some $"{relativePath}: forbidden {failureClass} generated guidance `{term}`; use package-reference alternative first [package-reference alternative]"
                else
                    None))

let tryHeading (line: string) =
    let trimmed = line.TrimStart()

    if trimmed.StartsWith("#") then
        let level = trimmed |> Seq.takeWhile ((=) '#') |> Seq.length

        if level > 0 && trimmed.Length > level && trimmed.[level] = ' ' then
            Some(level, trimmed.Substring(level).Trim())
        else
            None
    else
        None

let markdownSections (content: string) =
    let lines = content.Replace("\r\n", "\n").Split('\n')

    let headings =
        lines
        |> Array.mapi (fun index line -> index, tryHeading line)
        |> Array.choose (function
            | index, Some(level, heading) -> Some(index, level, heading)
            | _ -> None)

    headings
    |> Array.mapi (fun headingIndex (startIndex, level, heading) ->
        let endIndex =
            headings
            |> Array.skip (headingIndex + 1)
            |> Array.tryPick (fun (nextIndex, nextLevel, _) ->
                if nextLevel <= level then
                    Some(nextIndex - 1)
                else
                    None)
            |> Option.defaultValue (lines.Length - 1)

        { Heading = heading
          Level = level
          Content = lines.[startIndex..endIndex] |> String.concat Environment.NewLine })
    |> Array.toList

let trySection (sectionName: string) (sections: MarkdownSection list) =
    sections
    |> List.tryFind (fun section -> containsText sectionName section.Heading)

let deferredSections sections =
    sections
    |> List.filter (fun section -> containsText "deferred" section.Heading || containsText "roadmap" section.Heading)

let promptClassesInCorrectSections templatePath prompts content =
    let sections = markdownSections content

    prompts
    |> List.choose (fun prompt ->
        match trySection prompt.Section sections with
        | Some section when containsText prompt.Prompt section.Content -> Some prompt.Class
        | _ -> None)
    |> Set.ofList

let validateGuidanceTemplate model template =
    let filePath = path [ model.RepositoryRoot; template.Path ]

    if not (File.Exists filePath) then
        [ $"{template.Path}: missing file [missing-template]" ], Set.empty
    else
        let content = File.ReadAllText filePath
        let sections = markdownSections content

        let findings =
            template.Prompts
            |> List.collect (fun prompt ->
                match trySection prompt.Section sections with
                | None ->
                    [ $"{template.Path}: missing section `{prompt.Section}` for prompt `{prompt.Prompt}` [missing-section]" ]
                | Some section when containsText prompt.Prompt section.Content -> []
                | Some _ ->
                    let mismatch =
                        if deferredSections sections |> List.exists (fun section -> containsText prompt.Prompt section.Content) then
                            "deferred-scope-placement"
                        elif containsText prompt.Prompt content then
                            "wrong-section-prompt"
                        else
                            "missing-prompt"

                    [ $"{template.Path}: prompt `{prompt.Prompt}` missing from section `{prompt.Section}` [{mismatch}]" ])

        findings, promptClassesInCorrectSections template.Path template.Prompts content

let validateGuidanceParity validationRows =
    validationRows
    |> List.groupBy (fun (template: GuidanceTemplate, _, _) -> template.Artifact)
    |> List.collect (fun (artifact, rows) ->
        match rows with
        | [ (active, _, activeClasses); (preset, _, presetClasses) ] ->
            let missingInPreset = Set.difference activeClasses presetClasses
            let missingInActive = Set.difference presetClasses activeClasses

            [ yield!
                  missingInPreset
                  |> Set.toList
                  |> List.map (fun prompt -> $"{preset.Path}: parity mismatch for `{prompt}` against {active.Path} [active-preset-parity]")
              yield!
                  missingInActive
              |> Set.toList
              |> List.map (fun prompt -> $"{active.Path}: parity mismatch for `{prompt}` against {preset.Path} [active-preset-parity]") ]
        | _ -> [ $"{artifact}: expected active and preset templates for parity comparison [active-preset-parity]" ])

let validateControlsBoundaryGuidance model =
    let guidancePaths =
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

    let combined =
        guidancePaths
        |> List.map (fun relative -> File.ReadAllText(path [ model.RepositoryRoot; relative ]))
        |> String.concat Environment.NewLine

    let removedChartsPackage = "FS.Skia.UI." + "Charts"
    let removedChartsSkill = "fs-skia-" + "charts"

    let required =
        [ "FS.Skia.UI.Controls"
          "Skia-rendered"
          "Control<'msg>"
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

let validateTaskSkillistGuidance model =
    let requiredTerms =
        [ ".specify/templates/tasks-template.md",
          [ "[skillist: []]"
            "structured"
            "skillist"
            "After task generation"
            "minimal ordered skill set"
            "confidence"
            "matched signals"
            "reviewer disposition"
            "small, medium, and broad"
            "non-authoritative aggregate"
            "before and after every status change"
            "persistent launch rules"
            "non-authoritative aggregate reporting"
            "persistent graphical launch task"
            "MUST reject viewer-backed default executable paths"
            "[SEH]"
            "synthetic-error-handling-approved"
            "malformed parser input"
            "convenience mocks" ]
          ".specify/presets/fsharp-opinionated/templates/tasks-template.md",
          [ "[skillist: []]"
            "structured"
            "skillist"
            "After task generation"
            "minimal ordered skill set"
            "confidence"
            "matched signals"
            "reviewer disposition"
            "small, medium, and broad"
            "non-authoritative aggregate"
            "before and after every status change"
            "persistent launch rules"
            "non-authoritative aggregate reporting"
            "persistent graphical launch task"
            "MUST reject viewer-backed default executable paths"
            "[SEH]"
            "synthetic-error-handling-approved"
            "malformed parser input"
            "convenience mocks" ]
          ".specify/presets/fsharp-opinionated/templates/tasks-deps-template.yml",
          [ "deps:"
            "skillist:"
            "ordered list of applicable capability skill identifiers" ]
          ".agents/skills/speckit-tasks/SKILL.md",
          [ "Compulsory skill evaluation"
            "Visible skill mirror"
            "Declared skill ids resolve"
            "confidence"
            "matched signals"
            "reviewer disposition"
            "small, medium, and broad"
            "non-authoritative aggregate"
            "[SEH]"
            "synthetic-error-handling-approved"
            "implementation-time relabeling" ]
          ".specify/presets/fsharp-opinionated/commands/speckit.tasks.md",
          [ "Compulsory skill evaluation"
            "Visible skill mirror"
            "Declared skill ids resolve"
            "confidence"
            "matched signals"
            "reviewer disposition"
            "small, medium, and broad"
            "non-authoritative aggregate"
            "[SEH]"
            "synthetic-error-handling-approved"
            "implementation-time relabeling" ]
          ".agents/skills/speckit-implement/SKILL.md",
          [ "structured `skillist`"
            "Resolve every declared skill id"
            "skills in declared order"
            "loaded paths"
            "readiness/skill-loading-evidence.md"
            "loaded_at"
            "work_started_at"
            "reviewer exception"
            "implementation batch records"
            "graph before/after"
            "before and after every status change"
            "red-green evidence log"
            "[SEH]"
            "synthetic-error-handling-approved"
            "implementation-time relabeling" ]
          ".specify/presets/fsharp-opinionated/commands/speckit.implement.md",
          [ "structured `skillist`"
            "Resolve every declared skill id"
            "skills in declared order"
            "loaded paths"
            "readiness/skill-loading-evidence.md"
            "loaded_at"
            "work_started_at"
            "reviewer exception"
            "implementation batch records"
            "graph before/after"
            "before and after every status change"
            "red-green evidence log"
            "[SEH]"
            "synthetic-error-handling-approved"
            "implementation-time relabeling" ]
          ".specify/memory/constitution.md",
          [ "mandatory post-generation skill evaluation gate"
            "`skillist` field"
            "mandatory pre-task skill loading gate"
            "[SEH]"
            "synthetic-error-handling-approved" ]
          ".specify/templates/constitution-template.md",
          [ "mandatory post-generation skill evaluation gate"
            "`skillist` field"
            "mandatory pre-task skill loading gate"
            "[SEH]"
            "synthetic-error-handling-approved" ]
          ".specify/presets/fsharp-opinionated/templates/constitution-template.md",
          [ "mandatory post-generation skill evaluation gate"
            "`skillist` field"
            "mandatory pre-task skill loading gate"
            "[SEH]"
            "synthetic-error-handling-approved" ] ]

    requiredTerms
    |> List.collect (fun (relative, terms) ->
        let filePath = path [ model.RepositoryRoot; relative ]

        if not (File.Exists filePath) then
            [ $"{relative}: missing file [task-skillist-guidance]" ]
        else
            let content = File.ReadAllText filePath

            terms
            |> List.choose (fun term ->
                if content.IndexOf(term, StringComparison.OrdinalIgnoreCase) < 0 then
                    Some $"{relative}: missing `{term}` [task-skillist-guidance]"
                else
                    None))

// US1 (FR-001/002/003, SC-007): skill-id resolution guard.
// Build the advertised-id set from the single-line `-> <id>` mappings in the
// speckit-tasks SKILL.md copies, resolve each against the declared `name:` of
// every skill, and fail on any unresolved id, directory/name disagreement, or
// `.agents`<->`.claude` peer drift. Reads only repository files (a FAKE target
// cannot enumerate the runtime "available skills" harness surface).

let skillResolutionAdvertisingFiles =
    [ ".agents/skills/speckit-tasks/SKILL.md"
      ".claude/skills/speckit-tasks/SKILL.md" ]

let advertisedSkillIdRegex =
    Regex(@"->\s*((?:fs-skia|speckit)-[a-z0-9]+(?:-[a-z0-9]+)*)", RegexOptions.IgnoreCase)

let readDeclaredSkillName (file: string) =
    File.ReadAllLines file
    |> Array.tryPick (fun line ->
        let trimmed = line.Trim()

        if trimmed.StartsWith("name:", StringComparison.Ordinal) then
            Some(trimmed.Substring(5).Trim().Trim('"').Trim('\''))
        else
            None)

type SkillIdentityRecord =
    { Registry: string
      KeyDir: string
      DeclaredName: string option
      RelativeFile: string
      EnforceDirEqualsName: bool }

let collectSkillIdentityRecords root =
    let toRelative (full: string) =
        Path.GetRelativePath(root, full).Replace('\\', '/')

    let flatRegistry registry enforce =
        let registryRoot = path [ root; registry ]

        if Directory.Exists registryRoot then
            Directory.GetDirectories registryRoot
            |> Array.toList
            |> List.choose (fun dir ->
                let file = Path.Combine(dir, "SKILL.md")

                if File.Exists file then
                    Some
                        { Registry = registry
                          KeyDir = Path.GetFileName dir
                          DeclaredName = readDeclaredSkillName file
                          RelativeFile = toRelative file
                          EnforceDirEqualsName = enforce }
                else
                    None)
        else
            []

    let nestedRegistry parentRelative =
        let parentRoot = path [ root; parentRelative ]

        if Directory.Exists parentRoot then
            Directory.GetDirectories parentRoot
            |> Array.toList
            |> List.choose (fun dir ->
                let file = path [ dir; "skill"; "SKILL.md" ]

                if File.Exists file then
                    Some
                        { Registry = parentRelative
                          KeyDir = Path.GetFileName dir
                          DeclaredName = readDeclaredSkillName file
                          RelativeFile = toRelative file
                          EnforceDirEqualsName = false }
                else
                    None)
        else
            []

    flatRegistry ".agents/skills" true
    @ flatRegistry ".claude/skills" true
    // The skill set a generated consumer project receives (FR-002 edge case): an
    // id may resolve in this repo yet not in the generated project's skills.
    @ flatRegistry "template/base/.agents/skills" true
    @ flatRegistry "template/base/.claude/skills" true
    @ nestedRegistry "src"
    @ nestedRegistry "template/fragments"

let advertisedSkillIds root =
    skillResolutionAdvertisingFiles
    |> List.collect (fun relative ->
        let file = path [ root; relative ]

        if File.Exists file then
            File.ReadAllLines file
            |> Array.toList
            |> List.mapi (fun index line -> index + 1, line)
            |> List.collect (fun (lineNo, line) ->
                [ for m in advertisedSkillIdRegex.Matches line -> m.Groups.[1].Value, $"{relative}:{lineNo}" ])
        else
            [])

let validateSkillIdResolution model =
    let root = model.RepositoryRoot
    let records = collectSkillIdentityRecords root
    let declaredNames = records |> List.choose (fun r -> r.DeclaredName) |> Set.ofList

    let recordsByKey registry =
        records
        |> List.filter (fun r -> r.Registry = registry)
        |> List.map (fun r -> r.KeyDir, r)
        |> Map.ofList

    let advertised = advertisedSkillIds root

    let peerFindingsFor agentsRegistry claudeRegistry =
        let agents = recordsByKey agentsRegistry
        let claude = recordsByKey claudeRegistry

        (agents
         |> Map.toList
         |> List.choose (fun (key, agentRecord) ->
             match Map.tryFind key claude with
             | Some claudeRecord when claudeRecord.DeclaredName <> agentRecord.DeclaredName ->
                 Some
                     $"{agentRecord.RelativeFile} vs {claudeRecord.RelativeFile}: peer skill `{key}` declares different `name:` [skill-id-resolution]"
             | None -> Some $"{agentRecord.RelativeFile}: `{agentsRegistry}` skill `{key}` has no `{claudeRegistry}` peer [skill-id-resolution]"
             | _ -> None))
        @ (claude
           |> Map.toList
           |> List.choose (fun (key, claudeRecord) ->
               if Map.containsKey key (recordsByKey agentsRegistry) then
                   None
               else
                   Some $"{claudeRecord.RelativeFile}: `{claudeRegistry}` skill `{key}` has no `{agentsRegistry}` peer [skill-id-resolution]"))

    let missingNameFindings =
        records
        |> List.choose (fun r ->
            if Option.isNone r.DeclaredName then
                Some $"{r.RelativeFile}: SKILL.md has no `name:` declaration [skill-id-resolution]"
            else
                None)

    let dirNameFindings =
        records
        |> List.choose (fun r ->
            match r.DeclaredName with
            | Some name when r.EnforceDirEqualsName && name <> r.KeyDir ->
                Some $"{r.RelativeFile}: directory `{r.KeyDir}` disagrees with declared name `{name}` [skill-id-resolution]"
            | _ -> None)

    let peerFindings =
        peerFindingsFor ".agents/skills" ".claude/skills"
        @ peerFindingsFor "template/base/.agents/skills" "template/base/.claude/skills"

    let resolutionFindings =
        advertised
        |> List.choose (fun (id, location) ->
            if Set.contains id declaredNames then
                None
            else
                Some $"{location}: advertised skill id `{id}` does not resolve to any declared skill `name:` [skill-id-resolution]")

    let advertisedSetFor relative =
        advertised
        |> List.filter (fun (_, loc) -> loc.StartsWith(relative, StringComparison.Ordinal))
        |> List.map fst
        |> Set.ofList

    let driftFindings =
        let agentsAdvertised = advertisedSetFor ".agents/skills/speckit-tasks/SKILL.md"
        let claudeAdvertised = advertisedSetFor ".claude/skills/speckit-tasks/SKILL.md"

        if agentsAdvertised = claudeAdvertised then
            []
        else
            let drift =
                Set.union (Set.difference agentsAdvertised claudeAdvertised) (Set.difference claudeAdvertised agentsAdvertised)
                |> Set.toList
                |> String.concat ", "

            [ $".agents/.claude speckit-tasks advertised id sets drift: {drift} [skill-id-resolution]" ]

    missingNameFindings
    @ dirNameFindings
    @ peerFindings
    @ resolutionFindings
    @ driftFindings

let runGeneratedGuidanceScan model outputPath =
    let validationRows =
        generatedGuidanceRequirements
        |> List.map (fun template ->
            let findings, classes = validateGuidanceTemplate model template
            template, findings, classes)

    let findings =
        (validationRows |> List.collect (fun (_, findings, _) -> findings))
        @ validateGuidanceParity validationRows
        @ validateForbiddenGeneratedGuidanceAdvice model
        @ validateControlsBoundaryGuidance model
        @ validateTaskSkillistGuidance model
        @ validateSerializedRunnerGuidance model
        @ validateSkillIdResolution model

    if not (List.isEmpty findings) then
        failwithf "Generated guidance check failed:%s%s" Environment.NewLine (String.Join(Environment.NewLine, findings))

    let report =
        [ "# Generated Guidance Check"
          ""
          "PASS: active and preset-owned spec/plan templates include required governance prompts in the expected Markdown sections."
          "PASS: generated Controls guidance covers Skia-rendered controls, rich text, chart controls, graph controls, DataGrid, Controls.Elmish adapter wiring, and legacy Charts replacement notes without stale generated terms."
          "PASS: task templates, task metadata templates, implementation guidance, and constitution guidance require `skillist` evaluation, confidence review, risk-level evidence, and implementation-time skill loading."
          "PASS: repository, agent, template, and generated-product guidance serialize FAKE-backed commands because `.fake` state is shared, while preserving safe non-FAKE parallelism."
          "PASS: every advertised skill id in the speckit-tasks hints resolves to a declared skill `name:`; skill directory/name agree and `.agents`/`.claude` peers are synchronized."
          ""
          "Validated prompt classes:"
          yield!
              generatedGuidanceRequirements
              |> List.collect (fun template ->
                  template.Prompts
                  |> List.map (fun prompt -> $"- `{template.Path}` section `{prompt.Section}` prompt `{prompt.Prompt}`"))
          ""
          "Deferred roadmap boundaries checked: visual evidence, release validation, external repository split, and distribution automation remain outside V2 pass/fail scope." ]
        |> String.concat Environment.NewLine

    ensureParent outputPath
    File.WriteAllText(outputPath, report + Environment.NewLine)


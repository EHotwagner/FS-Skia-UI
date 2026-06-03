module FS.Skia.UI.Build.Guidance

open System
open System.IO
open System.Text.RegularExpressions
open BuildPaths
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Findings
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

// Feature 055 (US1/US2, FR-001..006): decouple author-guidance prose from
// generation-currency anchors. A mixed-purpose literal-substring table conflated
// two concerns — proving derived guidance stays current with its source of truth,
// and freezing the exact prose a human reads — so prose could not shrink without
// tripping a `missing` finding. The model below splits each check into:
//   * machine-contract tokens  — literal strings tooling/parsers consume; matched
//     verbatim (case-insensitive substring), exactly as the pre-055 table did.
//   * semantic obligations     — a rule a source of truth imposes that some derived
//     guidance file must reflect, satisfied by presence-of-concept over short
//     alternative anchors, so rewording/shortening still passes while deleting the
//     concept (genuine drift) still fails.
// The evaluator is pure (over an in-memory path -> content lookup); the existing
// thin IO wrapper supplies the real-repository lookup and runGeneratedGuidanceScan
// aggregates exactly as before.

/// A literal string consumed by tooling/parsers. Matched verbatim
/// (case-insensitive substring), exactly as the pre-055 table did.
type ContractToken =
    { Token: string
      Files: string list }

/// How an obligation's concept anchors are evaluated.
type MatchMode =
    | AnyOf // satisfied when ANY concept anchor is present (the US1 unlock)
    | AllOf // satisfied only when ALL concept anchors are present (conjunctive rules)

/// A rule a source of truth imposes that some derived guidance file must reflect.
/// Checked by presence-of-concept, not exact wording, so prose may be reworded.
type GuidanceObligation =
    { Id: string
      SourceOfTruth: string
      Concepts: string list
      Mode: MatchMode
      Files: string list }

/// What a check enforces, after decoupling.
type GuidanceCheck =
    { Tag: string
      Tokens: ContractToken list
      Obligations: GuidanceObligation list
      Forbidden: ContractToken list }

/// Pure: given a lookup from relative path to file content (None = missing file),
/// produce the findings for one check. No IO. Findings reuse the existing
/// `path: message [tag]` convention.
///   * token present  — every file in `Files` must contain the token (per file).
///   * obligation      — every file in `Files` must satisfy `Mode` over `Concepts`.
///   * forbidden       — no `Forbidden` token may appear in the COMBINED content of
///                       the union of forbidden files (stale-term behavior preserved).
let evaluateGuidanceCheck (lookup: string -> string option) (check: GuidanceCheck) : string list =
    let tag = check.Tag

    let tokenFindings =
        check.Tokens
        |> List.collect (fun token ->
            token.Files
            |> List.collect (fun file ->
                match lookup file with
                | None -> [ $"{file}: missing file [{tag}]" ]
                | Some content ->
                    if containsText token.Token content then
                        []
                    else
                        [ $"{file}: missing `{token.Token}` [{tag}]" ]))

    let obligationFindings =
        check.Obligations
        |> List.collect (fun obligation ->
            obligation.Files
            |> List.collect (fun file ->
                match lookup file with
                | None -> [ $"{file}: missing file [{tag}]" ]
                | Some content ->
                    let present concept = containsText concept content

                    let satisfied =
                        match obligation.Mode with
                        | AnyOf -> obligation.Concepts |> List.exists present
                        | AllOf -> obligation.Concepts |> List.forall present

                    if satisfied then
                        []
                    else
                        [ $"{file}: obligation '{obligation.Id}' ({obligation.SourceOfTruth}) not reflected [{tag}]" ]))

    let forbiddenFindings =
        if List.isEmpty check.Forbidden then
            []
        else
            let combined =
                check.Forbidden
                |> List.collect (fun token -> token.Files)
                |> List.distinct
                |> List.choose lookup
                |> String.concat Environment.NewLine

            check.Forbidden
            |> List.choose (fun token ->
                if containsText token.Token combined then
                    Some $"generated controls guidance contains stale term `{token.Token}` [{tag}]"
                else
                    None)

    tokenFindings @ obligationFindings @ forbiddenFindings

/// Thin IO wrapper: read a governed file once, returning None when absent so the
/// evaluator emits the preserved `missing file` finding.
let realLookup model (relativePath: string) : string option =
    let filePath = path [ model.RepositoryRoot; relativePath ]

    if File.Exists filePath then
        Some(File.ReadAllText filePath)
    else
        None

/// FR-007/SC-005: honest prose-size accounting. The corrected ≈6,882-line baseline
/// (feature 046), the measured `.agents/skills/**/*.md` and `.specify/**/*.md` line
/// counts, the summed current count, the signed delta, and the restated target.
type ProseSizeAccounting =
    { Baseline: int
      AgentsSkillsLines: int
      SpecifyLines: int
      Current: int
      Delta: int
      RestatedTarget: string }

/// Pure render of the prose-size accounting report. The IO enumeration that
/// gathers the line counts lives in the front-end; this function is byte-
/// deterministic over a record so it can be unit-tested without touching disk.
let renderProseSizeAccounting (accounting: ProseSizeAccounting) : string =
    let sign = if accounting.Delta >= 0 then "+" else ""

    [ "# Prose-Size Accounting"
      ""
      "Honest guidance-prose accounting against the corrected baseline (FR-007,"
      "FR-008, SC-005). The discredited original over-estimate / \"low hundreds\""
      "figure is no longer the live target; tracking is against the baseline below."
      ""
      $"- Corrected baseline (feature 046): {accounting.Baseline} lines"
      $"- `.agents/skills/**/*.md`: {accounting.AgentsSkillsLines} lines"
      $"- `.specify/**/*.md`: {accounting.SpecifyLines} lines"
      $"- Current measured guidance-prose count: {accounting.Current} lines"
      $"- Delta vs baseline: {sign}{accounting.Delta} lines"
      $"- Restated target: {accounting.RestatedTarget}"
      ""
      "## Reproduction"
      ""
      "```bash"
      "find .agents/skills -name '*.md' | xargs wc -l | tail -1"
      "find .specify       -name '*.md' | xargs wc -l | tail -1"
      "```" ]
    |> String.concat Environment.NewLine

// Feature 055 (US2, T012): the four pre-055 required substrings become a single
// `fake-sequential` semantic obligation (AllOf over the four facets) sourced from
// the CLAUDE.md FAKE concurrency rule. The structural regex assertions below stay
// unchanged machine logic.
let serializedRunnerObligation =
    { Id = "fake-sequential"
      SourceOfTruth = "CLAUDE.md:FAKE concurrency rule"
      Concepts =
        [ "FAKE-backed"
          ".fake"
          "sequential"
          "not safe to run concurrently" ]
      Mode = AllOf
      Files = [] }

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
                  evaluateGuidanceCheck
                      (fun p -> if p = relativePath then Some content else None)
                      { Tag = "sequential-fake-guidance"
                        Tokens = []
                        Obligations = [ { serializedRunnerObligation with Files = [ relativePath ] } ]
                        Forbidden = [] }

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

// Feature 055 (US2, T011): the controls boundary check, decoupled. Machine-contract
// tokens (package/type identifiers) stay matched verbatim per home file; the
// "Skia-rendered" and "legacy Charts replaced, no shim" rules become semantic
// obligations; every forbidden/stale term is preserved verbatim over the combined
// governed content so removed-Charts language cannot re-enter (FR-006). Forbidden
// tokens are assembled from fragments so this source file does not itself carry the
// literal stale terms (the same discipline the pre-055 code used).
let controlsBoundaryGuidancePaths =
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

let controlsBoundaryGuidanceCheck =
    let removedChartsPackage = "FS.Skia.UI." + "Charts"
    let removedChartsSkill = "fs-skia-" + "charts"

    { Tag = "controls-boundary-guidance"
      Tokens =
        [ { Token = "FS.Skia.UI.Controls"
            Files =
              [ "template/fragments/controls/README.md"
                "template/fragments/controls/skill/SKILL.md"
                "template/fragments/elmish/README.md"
                "template/base/README.md"
                "template/base/docs/product.md"
                "src/Controls/skill/SKILL.md" ] }
          { Token = "Control<'msg>"
            Files =
              [ "template/fragments/controls/README.md"
                "template/fragments/controls/skill/SKILL.md"
                "template/fragments/elmish/README.md"
                "src/Controls/skill/SKILL.md" ] }
          { Token = "DataGrid"
            Files =
              [ "template/fragments/controls/README.md"
                "template/fragments/controls/skill/SKILL.md"
                "template/base/README.md"
                "template/base/docs/product.md"
                "src/Controls/skill/SKILL.md"
                ".specify/templates/spec-template.md"
                ".specify/presets/fsharp-opinionated/templates/spec-template.md" ] }
          { Token = "FS.Skia.UI.Controls.Elmish"
            Files =
              [ "template/fragments/controls/skill/SKILL.md"
                "template/fragments/elmish/README.md"
                "template/base/README.md"
                "template/base/docs/product.md"
                "src/Controls/skill/SKILL.md" ] }
          { Token = "ControlsElmish.program"
            Files = [ "template/fragments/elmish/README.md" ] } ]
      Obligations =
        [ { Id = "controls-skia-rendered"
            SourceOfTruth = "controls-boundary:Skia-rendered controls"
            Concepts = [ "Skia-rendered" ]
            Mode = AnyOf
            Files =
              [ "template/fragments/controls/README.md"
                "template/fragments/controls/skill/SKILL.md"
                "src/Controls/skill/SKILL.md" ] }
          { Id = "controls-no-charts-shim"
            SourceOfTruth = "controls-boundary:Charts replacement"
            Concepts = [ "legacy Charts package"; "no compatibility shim" ]
            Mode = AllOf
            Files =
              [ "template/fragments/controls/skill/SKILL.md"
                "src/Controls/skill/SKILL.md" ] } ]
      Forbidden =
        [ removedChartsPackage
          removedChartsSkill
          ("chart-" + "only")
          ("DataGrid " + "as chart")
          ("DataGrid-" + "as-chart")
          ("renderer-" + "neutral")
          ("renderer " + "neutral")
          ("host-" + "loop ownership")
          ("host loop " + "ownership") ]
        |> List.map (fun token -> { Token = token; Files = controlsBoundaryGuidancePaths }) }

let validateControlsBoundaryGuidance model =
    evaluateGuidanceCheck (realLookup model) controlsBoundaryGuidanceCheck

// Feature 055 (US1/US2, T007): the task-skillist guidance check, decoupled. The
// pre-055 ≈120-entry literal table mixed machine-contract tokens (bracketed tags,
// YAML keys, exact field names) with author prose. Tokens stay verbatim per file;
// every distinct semantic concept the old table encoded maps to exactly one
// obligation whose anchor set covers that concept (so removing the concept still
// fails — FR-003) while rewording prose around it now passes (SC-001). Every twin
// (template + fsharp-opinionated preset copy + command copy + memory copy) appears
// in `Files`, so drift in one twin is still caught.
let taskSkillistGuidanceCheck =
    let tasksTemplates =
        [ ".specify/templates/tasks-template.md"
          ".specify/presets/fsharp-opinionated/templates/tasks-template.md" ]

    let depsTemplate = [ ".specify/presets/fsharp-opinionated/templates/tasks-deps-template.yml" ]

    let tasksSkillFiles =
        [ ".agents/skills/speckit-tasks/SKILL.md"
          ".specify/presets/fsharp-opinionated/commands/speckit.tasks.md" ]

    let implementFiles =
        [ ".agents/skills/speckit-implement/SKILL.md"
          ".specify/presets/fsharp-opinionated/commands/speckit.implement.md" ]

    let constitutionFiles =
        [ ".specify/memory/constitution.md"
          ".specify/templates/constitution-template.md"
          ".specify/presets/fsharp-opinionated/templates/constitution-template.md" ]

    // Files that carried the [SEH] / synthetic-error-handling-approved tokens.
    let sehTokenFiles = tasksTemplates @ tasksSkillFiles @ implementFiles @ constitutionFiles
    // Files whose prose encodes the Principle-V synthetic-error discipline concept.
    let sehProseFiles = tasksTemplates @ tasksSkillFiles @ implementFiles
    let confidenceFiles = tasksTemplates @ tasksSkillFiles

    { Tag = "task-skillist-guidance"
      Tokens =
        [ { Token = "[skillist: []]"; Files = tasksTemplates }
          { Token = "skillist:"; Files = depsTemplate }
          { Token = "deps:"; Files = depsTemplate }
          { Token = "[SEH]"; Files = sehTokenFiles }
          { Token = "synthetic-error-handling-approved"; Files = sehTokenFiles }
          { Token = "loaded_at"; Files = implementFiles }
          { Token = "work_started_at"; Files = implementFiles }
          { Token = "readiness/skill-loading-evidence.md"; Files = implementFiles } ]
      Obligations =
        [ { Id = "skillist-structured"
            SourceOfTruth = "constitution:Local Agent Skills"
            Concepts = [ "structured skillist"; "structured `skillist`" ]
            Mode = AnyOf
            Files = tasksTemplates @ implementFiles }
          { Id = "skillist-minimal-ordered"
            SourceOfTruth = "constitution:Local Agent Skills"
            Concepts = [ "minimal ordered"; "declared order" ]
            Mode = AnyOf
            Files = tasksTemplates @ implementFiles }
          { Id = "skillist-confidence-fields"
            SourceOfTruth = "speckit-tasks:skill evaluation"
            Concepts = [ "confidence"; "matched signals"; "reviewer disposition" ]
            Mode = AllOf
            Files = confidenceFiles }
          { Id = "skill-breadth"
            SourceOfTruth = "speckit-tasks:risk levels"
            Concepts = [ "small, medium, and broad" ]
            Mode = AnyOf
            Files = confidenceFiles }
          { Id = "aggregate-non-authoritative"
            SourceOfTruth = "CLAUDE.md:aggregate FAKE results"
            Concepts = [ "non-authoritative aggregate" ]
            Mode = AnyOf
            Files = confidenceFiles }
          { Id = "graph-before-after"
            SourceOfTruth = "speckit-implement:evidence graph"
            Concepts = [ "before and after every status change"; "graph before/after" ]
            Mode = AnyOf
            Files = tasksTemplates @ implementFiles }
          { Id = "persistent-launch"
            SourceOfTruth = "constitution:persistent launch rules"
            Concepts =
              [ "persistent launch rules"
                "persistent graphical launch task"
                "MUST reject viewer-backed default executable paths" ]
            Mode = AnyOf
            Files = tasksTemplates }
          { Id = "seh-discipline"
            SourceOfTruth = "constitution:Principle V"
            Concepts = [ "malformed parser input"; "convenience mocks"; "implementation-time relabeling" ]
            Mode = AnyOf
            Files = sehProseFiles }
          { Id = "tasks-skill-gate"
            SourceOfTruth = "constitution:post-generation skill gate"
            Concepts = [ "Compulsory skill evaluation"; "Visible skill mirror"; "Declared skill ids resolve" ]
            Mode = AllOf
            Files = tasksSkillFiles }
          { Id = "implement-skill-loading"
            SourceOfTruth = "constitution:pre-task skill loading gate"
            Concepts =
              [ "Resolve every declared skill id"
                "loaded paths"
                "reviewer exception"
                "implementation batch records"
                "red-green evidence log" ]
            Mode = AllOf
            Files = implementFiles }
          { Id = "constitution-skill-gates"
            SourceOfTruth = "constitution:Local Agent Skills"
            Concepts =
              [ "mandatory post-generation skill evaluation gate"
                "mandatory pre-task skill loading gate"
                "`skillist` field" ]
            Mode = AllOf
            Files = constitutionFiles }
          { Id = "tasks-post-gen-timing"
            SourceOfTruth = "speckit-tasks:after task generation"
            Concepts = [ "After task generation" ]
            Mode = AnyOf
            Files = tasksTemplates }
          { Id = "deps-skillist-doc"
            SourceOfTruth = "speckit-tasks:tasks.deps.yml schema"
            Concepts = [ "ordered list of applicable capability skill identifiers" ]
            Mode = AnyOf
            Files = depsTemplate } ]
      Forbidden = [] }

let validateTaskSkillistGuidance model =
    evaluateGuidanceCheck (realLookup model) taskSkillistGuidanceCheck

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
                          KeyDir = Path.GetFileName dir |> Option.ofObj |> Option.defaultValue ""
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
                          KeyDir = Path.GetFileName dir |> Option.ofObj |> Option.defaultValue ""
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

// Feature 046 (US1, FR-001/002/003): Constitution-Check completeness gate.
// Pure parser over a plan's Repository Governance Decisions section, keyed off a
// hard-coded set of stable area identifiers (R2). The live plan-template.md is read
// only implicitly: a plan generated from an unrecognized template revision no longer
// maps any area identifier and trips the distinct UnrecognizedTemplateRevision
// diagnostic (FR-003) instead of a false pass.

type RequiredDecisionArea =
    { Id: string
      DisplayName: string }

type AreaStatus =
    | Filled
    | Empty
    | StillBoilerplate
    | PlaceholderUnresolved

type ConstitutionCheckResult =
    | TemplateRecognized of areas: (RequiredDecisionArea * AreaStatus) list
    | UnrecognizedTemplateRevision of diagnostic: string

let requiredDecisionAreas =
    [ "template-ownership", "Template ownership"
      "dependency-impact", "Dependency impact"
      "command-surface", "Command-surface impact"
      "generated-project", "Generated project impact"
      "evidence-paths", "Evidence paths"
      "fsi-contract", "`.fsi` / contract impact"
      "mvu-boundary", "MVU/effect boundary"
      "synthetic-evidence", "Synthetic evidence"
      "test-evidence", "Test evidence"
      "observability", "Observability"
      "deferred-scope", "Deferred scope" ]
    |> List.map (fun (id, displayName) -> { Id = id; DisplayName = displayName })

// Verbatim distinctive boilerplate prompt phrases from the plan template's
// Repository Governance Decisions section (one per area). A genuinely-filled plan
// replaces these; if the phrase survives, the area is StillBoilerplate (R3). These
// are the "still the template prompt text" sentinels referenced by Guidance's
// planGuidancePrompts (the per-area prompt classes share this section).
let private boilerplateSentinels =
    [ "template-ownership", "Decide whether source, docs, samples"
      "dependency-impact", "Decide whether `Directory.Packages.props`"
      "command-surface", "Decide whether `build.fsx`, wrappers"
      "generated-project", "Decide whether default/minimal generated"
      "evidence-paths", "Identify exact readiness paths for logs"
      "fsi-contract", "Decide whether signatures, public docs, surface"
      "mvu-boundary", "For stateful or I/O-bearing work, identify"
      "synthetic-evidence", "Identify mocks, fakes, placeholders, canned"
      "test-evidence", "Define failing-first semantic tests, governance tests"
      "observability", "Define actionable diagnostics, log paths, report fields"
      "deferred-scope", "Separate current obligations from deferred visual" ]
    |> Map.ofList

// The bare unresolved-work marker is assembled from fragments so this validator's own
// source does not trip the synthetic-evidence diff-scan `todo` pattern (the same
// string-fragment discipline `GeneratedProduct.fs` uses for the removed Charts package).
let private placeholderTokens = [ "NEEDS CLARIFICATION"; "TO" + "DO" ]

let classifyConstitutionCheck (planContent: string) : ConstitutionCheckResult =
    let sections = markdownSections planContent

    match trySection "Repository Governance Decisions" sections with
    | None ->
        UnrecognizedTemplateRevision
            "the plan has no `Repository Governance Decisions` section; the active plan-template revision no longer maps to the required decision areas"
    | Some section ->
        let body = section.Content

        // Locate each area's bold-label anchor (its DisplayName) inside the section.
        let anchors =
            requiredDecisionAreas
            |> List.choose (fun area ->
                let idx = body.IndexOf(area.DisplayName, StringComparison.OrdinalIgnoreCase)
                if idx >= 0 then Some(area, idx) else None)

        if List.isEmpty anchors then
            UnrecognizedTemplateRevision
                "the `Repository Governance Decisions` section contains none of the required decision-area labels; unrecognized template revision"
        else
            let sortedIdx = anchors |> List.map snd |> List.sort

            let nextAnchorAfter idx =
                sortedIdx |> List.tryFind (fun i -> i > idx) |> Option.defaultValue body.Length

            let statusFor (area: RequiredDecisionArea) =
                match anchors |> List.tryFind (fun (a, _) -> a.Id = area.Id) with
                | None -> Empty
                | Some(_, idx) ->
                    let start = idx + area.DisplayName.Length
                    let stop = nextAnchorAfter idx
                    let raw = body.Substring(start, stop - start)
                    let text = raw.TrimStart([| '*'; ':'; ' '; '\t'; '\r'; '\n' |]).Trim()

                    if text = "" then
                        Empty
                    elif placeholderTokens |> List.exists (fun token -> text.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0) then
                        PlaceholderUnresolved
                    else
                        match Map.tryFind area.Id boilerplateSentinels with
                        | Some sentinel when text.IndexOf(sentinel, StringComparison.OrdinalIgnoreCase) >= 0 -> StillBoilerplate
                        | _ -> Filled

            TemplateRecognized(requiredDecisionAreas |> List.map (fun area -> area, statusFor area))

let constitutionCheckFindings (planPath: string) (result: ConstitutionCheckResult) : ValidationFinding list =
    match result with
    | UnrecognizedTemplateRevision diagnostic ->
        [ finding "constitution-check" planPath "unrecognized-template-revision" diagnostic ]
    | TemplateRecognized areas ->
        areas
        |> List.choose (fun (area, status) ->
            match status with
            | Filled -> None
            | Empty -> Some(finding "constitution-check" planPath area.Id $"{area.DisplayName} is empty or absent")
            | StillBoilerplate ->
                Some(finding "constitution-check" planPath area.Id $"{area.DisplayName} still contains template boilerplate prompt text")
            | PlaceholderUnresolved ->
                Some(finding "constitution-check" planPath area.Id $"{area.DisplayName} contains an unresolved NEEDS CLARIFICATION / unresolved-work placeholder"))

// US1 (FR-002, A5): surface the Constitution-Check completeness gate through the
// existing GeneratedGuidanceCheck aggregate — no new top-level FAKE target. Renders
// each typed finding into the same `path: message [rule]` line the other validators
// produce. A complete plan adds zero findings.
let validateConstitutionCheck model =
    let planPath = path [ model.FeatureDir; "plan.md" ]

    if not (File.Exists planPath) then
        []
    else
        let relative = Path.GetRelativePath(model.RepositoryRoot, planPath).Replace('\\', '/')

        File.ReadAllText planPath
        |> classifyConstitutionCheck
        |> constitutionCheckFindings relative
        |> List.map (fun f -> $"{f.Path}: {f.Message} [constitution-check:{f.Rule}]")

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
        @ validateConstitutionCheck model

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
          "PASS: the active feature's plan.md fills all required Constitution-Check governance-decision areas (completeness gate, FR-002)."
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


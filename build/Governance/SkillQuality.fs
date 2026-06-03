module FS.Skia.UI.Build.SkillQuality

open System
open System.Text.RegularExpressions

type RequiredSection =
    | Scope
    | DrivenLibraryApi
    | RunnableExample
    | ResearchLinks
    | PersistentProblemMandate
    | Related
    | Sources

type ParsedSkill =
    { Slug: string
      RelPath: string
      Body: string
      InScope: bool }

type SkillCheckResult =
    { Slug: string
      InScope: bool
      Missing: RequiredSection list }

let sectionName section =
    match section with
    | Scope -> "Scope / when-to-use"
    | DrivenLibraryApi -> "Driven-library API"
    | RunnableExample -> "Runnable example"
    | ResearchLinks -> "External research links"
    | PersistentProblemMandate -> "Persistent-problem mandate"
    | Related -> "Related"
    | Sources -> "Sources"

let isInScope (relPath: string) =
    let p = relPath.Replace('\\', '/')
    // FR-004: the vendored speckit-* tree is never in scope.
    let isSpeckit = p.Contains "/speckit-"
    let agentSkill = p.StartsWith ".agents/skills/"
    let productSkill = p.StartsWith "template/product-skills/"
    let baseAgentSkill = p.StartsWith "template/base/.agents/skills/"
    let srcSkill = p.StartsWith "src/" && p.EndsWith "/skill/SKILL.md"
    let fragmentSkill = p.StartsWith "template/fragments/" && p.EndsWith "/skill/SKILL.md"
    // Feature 058 (US3): the opt-in feedback command skill is shipped only under
    // `--feedback true` from template/feedback/, but is itself in-scope for the bar.
    let feedbackSkill = p.StartsWith "template/feedback/" && p.EndsWith "/skill/SKILL.md"
    not isSpeckit
    && (agentSkill || productSkill || baseAgentSkill || srcSkill || fragmentSkill || feedbackSkill)

let parse (relPath: string) (text: string) =
    let lines = text.Replace("\r\n", "\n").Split('\n')

    let slugFromName =
        lines
        |> Array.tryPick (fun l ->
            let t = l.Trim()
            if t.StartsWith "name:" then Some(t.Substring(5).Trim()) else None)

    let normalized = relPath.Replace('\\', '/')

    let slug =
        match slugFromName with
        | Some s when s <> "" -> s
        | _ ->
            let parts = normalized.Split('/')
            if parts.Length >= 2 then parts.[parts.Length - 2] else normalized

    { Slug = slug
      RelPath = normalized
      Body = text
      InScope = isInScope normalized }

let private headingLines (body: string) =
    body.Replace("\r\n", "\n").Split('\n')
    |> Array.filter (fun l -> l.TrimStart().StartsWith "#")
    |> Array.map (fun l -> l.ToLowerInvariant())

let checkSkill (parsed: ParsedSkill) : SkillCheckResult =
    if not parsed.InScope then
        { Slug = parsed.Slug
          InScope = false
          Missing = [] }
    else
        let body = parsed.Body
        let lower = body.ToLowerInvariant()
        let headings = headingLines body
        let headingHas (token: string) = headings |> Array.exists (fun h -> h.Contains token)

        let noBackingLib = lower.Contains "no backing library"
        let fenceCount = lower.Split([| "```" |], StringSplitOptions.None).Length - 1
        let urlCount = Regex.Matches(body, "https?://").Count

        let hasScope = headingHas "scope" || headingHas "when to use"
        let hasApi = noBackingLib || headingHas "api" || headingHas "contract" || headingHas "driven" || lower.Contains ".fsi"
        let hasExample = noBackingLib || fenceCount >= 2
        let hasResearch = urlCount >= 2
        let hasMandate = headingHas "persistent problem" || lower.Contains "official online docs first" || lower.Contains "official docs first"
        let hasRelated = body.Contains "[["
        let hasSources = headingHas "sources" || lower.Contains "## sources"

        let missing =
            [ if not hasScope then Scope
              if not hasApi then DrivenLibraryApi
              if not hasExample then RunnableExample
              if not hasResearch then ResearchLinks
              if not hasMandate then PersistentProblemMandate
              if not hasRelated then Related
              if not hasSources then Sources ]

        { Slug = parsed.Slug
          InScope = true
          Missing = missing }

let checkCorpus (skills: ParsedSkill list) : Findings.ValidationFinding list =
    skills
    |> List.filter (fun s -> s.InScope)
    |> List.sortBy (fun s -> s.RelPath)
    |> List.collect (fun s ->
        let result = checkSkill s

        result.Missing
        |> List.map (fun section ->
            Findings.finding
                "skill-quality"
                s.RelPath
                (sectionName section)
                (sprintf "skill '%s' is missing the required rubric section: %s" s.Slug (sectionName section))))

let renderReport (skills: ParsedSkill list) : string =
    let inScope =
        skills |> List.filter (fun s -> s.InScope) |> List.sortBy (fun s -> s.RelPath)

    let results = inScope |> List.map checkSkill
    let passed = results |> List.filter (fun r -> List.isEmpty r.Missing)
    let failed = results |> List.filter (fun r -> not (List.isEmpty r.Missing))

    [ yield "# Skill-quality check (SkillQualityCheck)"
      yield ""
      yield sprintf "Checked %d in-scope skill(s); vendored `speckit-*` excluded (FR-004)." (List.length inScope)
      yield ""
      yield sprintf "- PASS: %d" (List.length passed)
      yield sprintf "- FAIL: %d" (List.length failed)
      yield ""
      if not (List.isEmpty failed) then
          yield "## Failing skills"
          yield ""
          for r in failed do
              yield sprintf "- `%s` — missing: %s" r.Slug (r.Missing |> List.map sectionName |> String.concat ", ")
          yield ""
      yield "## Per-skill results"
      yield ""
      for r in results do
          if List.isEmpty r.Missing then
              yield sprintf "- ✅ `%s`" r.Slug
          else
              yield sprintf "- ❌ `%s` — missing: %s" r.Slug (r.Missing |> List.map sectionName |> String.concat ", ") ]
    |> String.concat "\n"

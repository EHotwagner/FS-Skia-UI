module FS.Skia.UI.Build.PhaseHookParity

open System

let roster =
    [ "specify"
      "clarify"
      "plan"
      "tasks"
      "analyze"
      "implement"
      "checklist"
      "taskstoissues"
      "constitution" ]

type ParsedPhaseSkill =
    { Phase: string
      RelPath: string
      Body: string }

// The three strict modern markers (contracts/modern-hook-block.md). Deliberately literal
// (mirrors SkillQuality's detector style) to stay low-brittleness.
type private Marker =
    | MultiFileEnumeration
    | DedupeByExtensionCommand
    | EffectiveHooksNotice

let private markerLabel (phase: string) marker =
    match marker with
    | MultiFileEnumeration -> "multi-file .specify/extensions/*/*.yml enumeration"
    | DedupeByExtensionCommand -> "(extension, command) dedupe language"
    | EffectiveHooksNotice -> sprintf "## Effective hooks for %s notice" phase

// Count case-insensitive non-overlapping occurrences of `needle` in `haystack`.
let private countOccurrences (needle: string) (haystack: string) =
    if needle = "" then
        0
    else
        let mutable count = 0
        let mutable idx = haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase)

        while idx >= 0 do
            count <- count + 1
            idx <- haystack.IndexOf(needle, idx + needle.Length, StringComparison.OrdinalIgnoreCase)

        count

// Pre + post discovery blocks each enumerate `.specify/extensions/*/*.yml`, so the modern
// block contains the token at least twice; a legacy single-file block fails this.
let private missingMarkers (skill: ParsedPhaseSkill) =
    let body = skill.Body
    let lower = body.ToLowerInvariant()
    let hasMulti = countOccurrences ".specify/extensions/*/*.yml" body >= 2
    let hasDedupe = lower.Contains "(extension, command)"
    let hasNotice = lower.Contains(sprintf "## effective hooks for %s" (skill.Phase.ToLowerInvariant()))

    [ if not hasMulti then MultiFileEnumeration
      if not hasDedupe then DedupeByExtensionCommand
      if not hasNotice then EffectiveHooksNotice ]

let checkCorpus (skills: ParsedPhaseSkill list) : Findings.ValidationFinding list =
    let presentPhases = skills |> List.map (fun s -> s.Phase) |> Set.ofList

    // A roster phase with no provided SKILL.md ⇒ named failure (Observability).
    let missingPhaseFindings =
        roster
        |> List.filter (fun phase -> not (Set.contains phase presentPhases))
        |> List.map (fun phase ->
            Findings.finding
                "phase-hook-parity"
                (sprintf ".agents/skills/speckit-%s/SKILL.md" phase)
                "missing-skill"
                (sprintf
                    "phase skill '%s' is missing or unreadable (no SKILL.md found for the roster phase)"
                    phase))

    // One finding per (in-scope skill, missing marker).
    let markerFindings =
        skills
        |> List.filter (fun s -> List.contains s.Phase roster)
        |> List.sortBy (fun s -> s.RelPath)
        |> List.collect (fun s ->
            missingMarkers s
            |> List.map (fun marker ->
                Findings.finding
                    "phase-hook-parity"
                    s.RelPath
                    (markerLabel s.Phase marker)
                    (sprintf "phase skill '%s' is missing the required marker: %s" s.Phase (markerLabel s.Phase marker))))

    missingPhaseFindings @ markerFindings

let renderReport (skills: ParsedPhaseSkill list) : string =
    // Group by phase so the report is per-phase (a phase passes only when every one of its
    // SKILL.md files — `.agents` canonical + `.claude` mirror — carries the modern block).
    let phaseResult phase =
        let entries = skills |> List.filter (fun s -> s.Phase = phase)

        if List.isEmpty entries then
            phase, [ "missing or unreadable SKILL.md" ]
        else
            let missing =
                entries
                |> List.collect missingMarkers
                |> List.distinct
                |> List.map (markerLabel phase)

            phase, missing

    let results = roster |> List.map phaseResult
    let passed = results |> List.filter (fun (_, missing) -> List.isEmpty missing)
    let failed = results |> List.filter (fun (_, missing) -> not (List.isEmpty missing))

    [ yield "# Phase-hook parity check (PhaseHookParityCheck)"
      yield ""
      yield sprintf "Checked %d in-scope phase skill(s)." (List.length roster)
      yield ""
      yield sprintf "- PASS: %d" (List.length passed)
      yield sprintf "- FAIL: %d" (List.length failed)
      yield ""
      if not (List.isEmpty failed) then
          yield "## Failing skills"
          yield ""
          for (phase, missing) in failed do
              yield sprintf "- `%s` — missing: %s" phase (String.concat ", " missing)
          yield ""
      yield "## Per-skill results"
      yield ""
      for (phase, missing) in results do
          if List.isEmpty missing then
              yield sprintf "- ✅ `%s`" phase
          else
              yield sprintf "- ❌ `%s` — missing: %s" phase (String.concat ", " missing) ]
    |> String.concat "\n"

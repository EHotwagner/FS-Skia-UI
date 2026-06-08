# Phase 1 Data Model: Implement-Phase Feedback Hook Parity

This feature is governance-text + a pure validation rule. The "entities" are the
roster, the marker set, and the finding records the guard produces.

## Entity: `PhaseSkill` (the unit the guard checks)

| Field | Type | Notes |
| --- | --- | --- |
| `Phase` | string | one of the nine lifecycle phase names (`specify`, `clarify`, `plan`, `tasks`, `analyze`, `implement`, `checklist`, `taskstoissues`, `constitution`) |
| `CanonicalPath` | string | `.agents/skills/speckit-<phase>/SKILL.md` |
| `DerivedPath` | string | `.claude/skills/speckit-<phase>/SKILL.md` (mirror) |
| `Body` | string | the SKILL.md text (read at the interpreter edge, never inside the pure check) |

**Roster rule**: the nine phases are fixed data derived from the
`before_*`/`after_*` keys in `.specify/extensions.yml`. A roster phase whose
SKILL.md is missing or unreadable is a **named failure**, not a silent skip
(Observability).

## Entity: `RequiredMarker` (strict modern markers)

Each in-scope `PhaseSkill.Body` MUST contain all of:

| Marker | Detector (literal substring, case-insensitive) | Proves |
| --- | --- | --- |
| `MultiFileEnumeration` | `.specify/extensions/*/*.yml` occurs **≥ 2×** | both pre- and post-hook blocks enumerate per-extension files |
| `DedupeByExtensionCommand` | `(extension, command)` (dedupe language) present | merge/dedupe semantics present |
| `EffectiveHooksNotice` | `## Effective hooks for <phase>` present | the consolidated D2 notice is emitted |

A **legacy single-file** block (central `extensions.yml` only) fails
`MultiFileEnumeration` and `EffectiveHooksNotice`. **Total absence** fails all
three. Markers are deliberately literal (mirrors `SkillQuality`'s detector style)
to stay low-brittleness.

## Entity: `PhaseHookFinding` (guard output)

Reuses the existing `Findings.ValidationFinding` shape (rule = `"phase-hook-parity"`).

| Field | Value |
| --- | --- |
| `Rule` | `"phase-hook-parity"` |
| `Path` | the canonical (or derived) SKILL.md path |
| `Marker` | the missing marker's name |
| `Message` | e.g. `phase skill 'tasks' is missing the required marker: ## Effective hooks for tasks notice` |

Empty finding list ⇒ guard PASS. Non-empty ⇒ `PhaseHookParityCheck` fails
(`failwith`) after writing the report.

## Pure surface (curated `PhaseHookParity.fsi`)

```fsharp
module FS.Skia.UI.Build.PhaseHookParity

/// The nine lifecycle phases that can carry registered before_*/after_* hooks.
val roster : string list

type ParsedPhaseSkill =
    { Phase: string
      RelPath: string
      Body: string }

/// Pure: returns one finding per (skill, missing marker). Empty ⇒ all pass.
val checkCorpus : ParsedPhaseSkill list -> Findings.ValidationFinding list

/// Pure: human-readable PASS/FAIL report for readiness/phase-hook-parity-check.md.
val renderReport : ParsedPhaseSkill list -> string
```

I/O (reading the SKILL.md files, writing the report, `failwith` on findings) lives
at the **existing** governance Engine interpreter edge (`Engine/Interpret.fs`),
not in this pure module — Principle IV reuse, no new MVU surface.

## State transitions

None. The guard is a pure fold over the roster. The only "transition" is the
existing governance Engine `Idle → StartTarget PhaseHookParityCheck →
PhaseHookScan effect → finding fold → report+verdict`.

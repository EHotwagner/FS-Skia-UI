// PhaseHookParity.fsi — the FR-006 anti-drift guard for Spec Kit phase-skill hook
// discovery (feature 077; Principle II: visibility lives in the .fsi).
//
// Build-tooling scope only. Pure roster + strict-marker check over the nine lifecycle
// phase skills (`specify`, `clarify`, `plan`, `tasks`, `analyze`, `implement`,
// `checklist`, `taskstoissues`, `constitution`). It fails when any in-scope phase skill
// lacks the modern multi-file hook-discovery block. Enumeration/IO (reading the SKILL.md
// files, writing the report, `failwith`) lives at the Engine/Interpret edge, never here.
module FS.Skia.UI.Build.PhaseHookParity

/// The nine lifecycle phases that can carry registered before_*/after_* hooks. Fixed data
/// derived from the `before_*`/`after_*` keys in `.specify/extensions.yml`. A roster phase
/// whose SKILL.md is missing/unreadable is a named failure, not a silent skip.
val roster: string list

/// A parsed phase skill ready for checking. `Body` is the raw `SKILL.md` text (read at the
/// interpret edge); `RelPath` is the `.agents`/`.claude` SKILL.md relative path it came from.
type ParsedPhaseSkill =
    { Phase: string
      RelPath: string
      Body: string }

/// Pure: one finding per (skill, missing marker), plus a named failure for any roster phase
/// absent from the input. Empty ⇒ every in-scope phase skill carries the modern block.
val checkCorpus: skills: ParsedPhaseSkill list -> Findings.ValidationFinding list

/// Pure: the per-phase PASS/FAIL report body written to
/// `readiness/phase-hook-parity-check.md`.
val renderReport: skills: ParsedPhaseSkill list -> string

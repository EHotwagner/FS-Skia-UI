// SkillQuality.fsi — the pure skill-quality rubric check (feature 058, FR-001/FR-003;
// Principle II: visibility lives in the .fsi).
//
// Build-tooling scope only. Parses each in-scope `SKILL.md` and reports the required
// rubric sections it is missing as structured Findings. The vendored `speckit-*`
// skills are out of scope (FR-004) and are never flagged. Pure — enumeration/IO lives
// at the interpret edge (Front/Governance.fs).
module FS.Skia.UI.Build.SkillQuality

/// The rubric rows every in-scope skill must satisfy (contracts/skill-quality-rubric.md).
type RequiredSection =
    | Scope
    | DrivenLibraryApi
    | RunnableExample
    | ResearchLinks
    | PersistentProblemMandate
    | Related
    | Sources

/// A parsed skill ready for checking. `Body` is the raw `SKILL.md` text; `Slug` is the
/// frontmatter `name` (falling back to the directory slug); `InScope` is false for the
/// vendored `speckit-*` tree.
type ParsedSkill =
    { Slug: string
      RelPath: string
      Body: string
      InScope: bool }

/// The per-skill result. `Missing` empty ⇒ the skill passes.
type SkillCheckResult =
    { Slug: string
      InScope: bool
      Missing: RequiredSection list }

/// Canonical human label for a rubric row (used in report/failure text).
val sectionName: RequiredSection -> string

/// True when a `/`-normalized `SKILL.md` relative path is an in-scope FS-authored skill
/// (excludes the vendored `speckit-*` tree, FR-004).
val isInScope: relPath: string -> bool

/// Parse a `SKILL.md` (relative path + raw text) into a `ParsedSkill`.
val parse: relPath: string -> text: string -> ParsedSkill

/// Pure: given a parsed skill, report the rubric rows it is missing (empty ⇒ pass).
val checkSkill: parsed: ParsedSkill -> SkillCheckResult

/// Pure: aggregate over the enumerated corpus → one Finding per missing section on each
/// in-scope skill (naming skill + missing section, FR-003). Out-of-scope skills produce
/// no findings.
val checkCorpus: skills: ParsedSkill list -> Findings.ValidationFinding list

/// Render the per-skill PASS/FAIL report body written to readiness (the artifact T026
/// captures). Pure.
val renderReport: skills: ParsedSkill list -> string

// SkillTreeGen.fsi — public surface of the canonical-to-derived skill-tree
// generator + currency checker (feature 044 / US1, Principle II: visibility lives
// in the .fsi).
//
// Build-tooling scope only (build/Governance under net10.0). NOT part of the
// tracked runtime surface baselines. Pure generation + currency over values passed
// in; all filesystem enumeration/read/write stays at the build.fsx interpreter edge
// (Principle IV). No shelling to diff/cmp/sha256sum (in-process byte comparison).
//
// `.agents/skills/` is canonical (the Codex source); `.claude/skills/` is generated
// by enumeration over every canonical file — there is NO hardcoded slug allowlist
// (FR-002), so adding a skill needs zero allowlist edits (SC-002).
module FS.Skia.UI.Build.SkillTreeGen

/// One skill document read from a tree (canonical or derived). Paths are
/// repo-relative (e.g. `.agents/skills/fsharp-parsing/SKILL.md`); byte-identity is
/// over `Bytes`.
type SkillFile =
    { Slug: string
      RelPath: string
      Bytes: byte[] }

/// One derived-file write the generator prescribes (repo-relative derived path +
/// the canonical bytes, reproduced unchanged — FR-003).
type GenPlanEntry =
    { DerivedRelPath: string
      Bytes: byte[] }

/// The full derivation plan plus the tree-level provenance manifest (research R5).
type SkillTreePlan =
    { Entries: GenPlanEntry list
      ManifestRelPath: string
      ManifestBytes: byte[] }

/// Result of comparing a committed derived tree against the plan; clean iff all
/// three lists are empty.
type SkillCurrency =
    { StaleSlugs: string list
      MissingDerived: string list
      OrphanDerived: string list }

/// Canonical-tree-relative -> derived-tree-relative path translation
/// (`.agents/skills/X/SKILL.md` -> `.claude/skills/X/SKILL.md`). Pure.
val derivedRelPath: canonicalRelPath: string -> string

/// Repo-relative path of the canonical skill-tree root (`.agents/skills`).
val canonicalRoot: string

/// Repo-relative path of the derived skill-tree root (`.claude/skills`).
val derivedRoot: string

/// Render the tree-level provenance manifest bytes (canonical source + the
/// regeneration command). Pure.
val renderManifest: unit -> byte[]

/// Build the derivation plan from the enumerated canonical files.
/// Raises if the canonical set is empty or any file's bytes are absent
/// (Principle VII — a partial derived tree is never emitted).
val plan: canonical: SkillFile list -> SkillTreePlan

/// Compare the committed derived files against the plan; clean iff all three lists
/// empty. The provenance manifest is an expected derived entry (not an orphan).
val currency: plan: SkillTreePlan -> derived: SkillFile list -> SkillCurrency

/// True when the derived tree is a current reproduction of the canonical tree.
val isCurrent: currency: SkillCurrency -> bool

/// Actionable currency diagnostic naming `./fake.sh build -t RefreshSurfaceBaselines`;
/// None when current.
val currencyDrift: currency: SkillCurrency -> string option

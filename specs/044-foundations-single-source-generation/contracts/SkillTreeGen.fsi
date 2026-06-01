// Contract sketch (Phase 1) for build/Governance/SkillTreeGen.fsi — US1.
// Pure generation + currency for the canonical .agents/skills -> derived .claude/skills tree.
// All filesystem enumeration/read/write stays at the build.fsx interpreter edge; this surface
// operates only on values passed in (Principle IV). No shelling to diff/cmp/sha256sum.
namespace FS.Skia.UI.Build

module SkillTreeGen =

    /// One skill document read from a tree (canonical or derived).
    type SkillFile =
        { Slug: string
          RelPath: string
          Bytes: byte[] }

    /// One derived-file write the generator prescribes.
    type GenPlanEntry =
        { DerivedRelPath: string
          Bytes: byte[] }

    /// The full derivation plan plus the tree-level provenance manifest (R5).
    type SkillTreePlan =
        { Entries: GenPlanEntry list
          ManifestRelPath: string
          ManifestBytes: byte[] }

    /// Result of comparing a committed derived tree against the plan.
    type SkillCurrency =
        { StaleSlugs: string list
          MissingDerived: string list
          OrphanDerived: string list }

    /// Canonical-tree-relative -> derived-tree-relative path translation
    /// (".agents/skills/X/SKILL.md" -> ".claude/skills/X/SKILL.md").
    val derivedRelPath: canonicalRelPath: string -> string

    /// Render the tree-level provenance manifest bytes (source + regeneration command).
    val renderManifest: unit -> byte[]

    /// Build the derivation plan from the enumerated canonical files.
    /// Raises if the canonical set is empty or a file is unreadable (Principle VII).
    val plan: canonical: SkillFile list -> SkillTreePlan

    /// Compare the committed derived files against the plan; clean iff all three lists empty.
    val currency: plan: SkillTreePlan -> derived: SkillFile list -> SkillCurrency

    /// True when the derived tree is a current reproduction of the canonical tree.
    val isCurrent: currency: SkillCurrency -> bool

    /// Actionable currency diagnostic naming the regeneration command; None when current.
    val currencyDrift: currency: SkillCurrency -> string option

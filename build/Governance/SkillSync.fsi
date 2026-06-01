// SkillSync.fsi — public surface of the SkillSyncCheck gate's generation-currency
// adapter (feature 044 / US1 reframe; Principle II: visibility lives in the .fsi).
//
// Build-tooling scope only (build/Governance under net10.0). NOT part of the tracked
// runtime surface baselines. Feature 040 framed this as a six-slug byte-identity PEER
// check; feature 044 reframes it as a CURRENCY check over the whole tree: `.claude/skills`
// must be a current regeneration of the canonical `.agents/skills` (all skills, by
// enumeration — no allowlist), delegating to `SkillTreeGen` (FR-002/FR-004). The pure
// comparison stays here; all file enumeration/read lives at the build.fsx edge.
module FS.Skia.UI.Build.SkillSync

/// Build the canonical derivation plan from the enumerated `.agents/skills` files.
/// Thin alias over `SkillTreeGen.plan` so the gate edge has one entry point.
val planFromCanonical: canonical: SkillTreeGen.SkillFile list -> SkillTreeGen.SkillTreePlan

/// Compute currency of the committed `.claude/skills` tree against the canonical plan.
val currency: plan: SkillTreeGen.SkillTreePlan -> derived: SkillTreeGen.SkillFile list -> SkillTreeGen.SkillCurrency

/// True iff the derived tree is a current reproduction of the canonical tree.
val isCurrent: currency: SkillTreeGen.SkillCurrency -> bool

/// PASS/FAIL report body (Markdown) for the SkillSyncCheck readiness artifact, naming
/// the enumerated skill count and, on drift, the stale/missing/orphan paths.
val renderReport: plan: SkillTreeGen.SkillTreePlan -> currency: SkillTreeGen.SkillCurrency -> string

/// Actionable currency failure message naming the regeneration target; empty string
/// when the derived tree is current (FR-012). Delegates to `SkillTreeGen.currencyDrift`.
val renderFailureMessage: currency: SkillTreeGen.SkillCurrency -> string

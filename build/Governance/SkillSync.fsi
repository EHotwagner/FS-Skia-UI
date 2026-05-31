// SkillSync.fsi — public surface of the capability-skill byte-identity checker
// (feature 040, Principle II: visibility lives in the .fsi).
//
// Build-tooling scope only (build/Governance under net10.0). NOT part of the
// tracked runtime surface baselines. Pure comparison core (hashing + verdict)
// with file I/O confined to the `checkPair`/`checkAll` edge so the core stays
// testable without touching disk.
module FS.Skia.UI.Build.SkillSync

/// One capability skill's byte-identity result across the two skill trees.
/// `ClaudeHash`/`AgentsHash` are `None` when the file on that side is missing
/// (a hard failure, never a skip — Principle VII).
type SkillPairResult =
    { Slug: string
      ClaudePath: string
      AgentsPath: string
      ClaudeHash: string option
      AgentsHash: string option }

/// The six capability-skill slugs this gate governs (FR-001). Exactly these,
/// in this order, must be discovered across both trees.
val expectedSlugs: string list

/// Relative path to the Claude-tree copy of a skill, e.g.
/// `.claude/skills/<slug>/SKILL.md`.
val claudeRelPath: slug: string -> string

/// Relative path to the Codex/agents-tree copy of a skill, e.g.
/// `.agents/skills/<slug>/SKILL.md`.
val agentsRelPath: slug: string -> string

/// Lowercase hex SHA-256 of the raw bytes (no newline normalization). Pure.
val sha256Hex: bytes: byte[] -> string

/// True iff both files are present and their digests are equal. Pure.
val inSync: result: SkillPairResult -> bool

/// The drifted (or missing-file) subset of results. Pure.
val drifted: results: SkillPairResult list -> SkillPairResult list

/// Read both copies of one skill under `root` and compute their digests.
/// A missing file yields `None` for that side (surfaced as drift by `inSync`).
val checkPair: root: string -> slug: string -> SkillPairResult

/// `checkPair` over every `expectedSlugs` entry.
val checkAll: root: string -> SkillPairResult list

/// PASS report body (Markdown): the six slugs and their shared hash when all
/// are in sync; the drifted slugs and both digests otherwise.
val renderReport: results: SkillPairResult list -> string

/// One-line-per-offender failure message naming each drifted slug and both
/// digests (or which side is missing). Empty string when nothing drifted.
val renderFailureMessage: results: SkillPairResult list -> string

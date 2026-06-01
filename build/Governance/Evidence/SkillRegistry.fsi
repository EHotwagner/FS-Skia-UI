// SkillRegistry.fsi — skill discovery across .agents/skills, src/*/skill, and
// template/fragments/*/skill (feature 043, FR-003; Principle II).
//
// Build-tooling only. `build` is the discovery edge (reads SKILL.md files like
// Capabilities.readCatalog); the resulting value is data the pure Engine
// consumes. Faithfully ports discover_skill_registry from compute-task-graph.py.
namespace FS.Skia.UI.Build.Evidence

/// Discovered skills: id -> resolved SKILL.md paths (repo-relative, forward
/// slashes); directory-name aliases that differ from the declared id; and any
/// unreadable-file warnings.
type SkillRegistry =
    { Skills: Map<string, string list>
      DirectoryAliases: Map<string, string * string>
      Warnings: string list }

module SkillRegistry =

    /// Discover the skill registry rooted at `repoRoot` (I/O edge). FR-003.
    val build: repoRoot: string -> SkillRegistry

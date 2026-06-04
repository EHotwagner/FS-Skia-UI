// TemplateUpdatePackage.fsi — TemplateUpdateSkillPackageCheck (feature 060,
// FR-009 / SC-006; Principle II).
//
// Guarantees the `fs-skia-template-update` skill's enumerated package set exactly
// equals the current packable-project set, so it cannot drift (no phantom bare-Lib
// `FS.Skia.UI`, no missing `SkillSupport`/`Input`). The discovery of the packable set
// (`*.fsproj` scan) stays at the build edge; parsing/checking are pure.
module FS.Skia.UI.Build.TemplateUpdatePackage

/// A repo project that produces a NuGet package.
type PackableProject =
    { /// repo-relative `.fsproj` path.
      ProjectPath: string
      /// derived package id (e.g. `FS.Skia.UI.Scene`).
      PackageId: string
      /// last dotted segment (e.g. `Scene`) — the suffix used in the local-feed loop.
      Leaf: string }

/// Derive the package leaf from a `.fsproj` path (e.g. `src/Scene/Scene.fsproj` -> `Scene`,
/// `build/Governance/FS.Skia.UI.Build.fsproj` -> `Build`). Pure.
val leafOfProject: projectPath: string -> string

/// The package leaves enumerated by the skill's step-5 local-feed verification loop
/// (`for <var> in <leaves>; do`). Pure.
val feedLoopLeaves: skillText: string -> string list

/// True when the skill still checks the phantom bare-Lib `FS.Skia.UI` package (053
/// deleted `src/Lib` and unpublished it). Pure.
val hasPhantomLib: skillText: string -> bool

/// True when the skill still carries the stale "nine repo packages" count text. Pure.
val hasStaleCount: skillText: string -> bool

/// Check the skill's enumeration against the packable leaf set. Build-failure
/// diagnostics (empty == clean): missing packable leaf, phantom non-packable leaf,
/// phantom bare-Lib check, or stale count text. Pure.
val check: packableLeaves: string list -> skillPath: string -> skillText: string -> string list

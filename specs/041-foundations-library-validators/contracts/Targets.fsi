// Targets.fsi — the typed build-target model (feature 041, FR-001; Principle II).
//
// Single source of truth for target identity, direct prerequisites, and the
// per-target attributes from which TargetMetadata is derived (replacing the
// stringly-typed requiredTargets + targetDependencyRows registries). Build-tooling
// only. A renamed/mistyped target becomes a compile error, not a runtime drift.
module FS.Skia.UI.Build.Targets

/// One case per runnable build target. Closed union ⇒ exhaustive dispatch ⇒
/// `spec` is total ⇒ a target without metadata (or metadata without a target) is
/// unrepresentable (SC-003). Cases mirror the current `requiredTargets` list, in order.
type Target =
    | Clean
    | Restore
    | Build
    | Test
    | Dev
    | CapabilityCheck
    | SkillCheck
    | TargetMetadata
    | TargetMetadataDrift
    | SkillSyncCheck
    | SkillExamplesCheck
    // … remaining runnable targets, one nullary case each, preserving registry order (FR-013).

/// Intrinsic per-target attributes — the single source metadata derives from (R3).
/// Runtime path strings (LogPath/ReadinessPath) are NOT here; they are injected at the
/// build.fsx interpreter edge to keep this module pure (Principle IV).
type TargetSpec =
    { Target: Target
      Name: string
      DirectPrerequisites: Target list
      TimeoutClass: string
      Cost: string
      FailureOwner: string }

/// Every runnable target, in registry order (replaces `requiredTargets`).
val allTargets: Target list

/// The single source of truth: total over `Target` (a missing case fails to compile).
val spec: target: Target -> TargetSpec

/// Canonical runnable-target string (for FAKE registration and report text).
val name: target: Target -> string

/// Direct prerequisites as typed targets (replaces the `targetDependencyRows` lookup).
val directPrerequisites: target: Target -> Target list

/// Derived string view of the runnable-target registry (build.fsx compatibility). Pure.
val requiredTargetNames: string list

/// Derived `(name, prereq-names)` rows (replaces the maintained `targetDependencyRows`). Pure.
val targetDependencyRows: (string * string list) list

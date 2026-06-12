// Targets.fsi — the typed build-target model (feature 041, FR-001; Principle II).
//
// Single source of truth for target identity, direct prerequisites, and the
// per-target attributes from which TargetMetadata is derived (replacing the
// stringly-typed requiredTargets + targetDependencyRows registries). Build-tooling
// only. A renamed/mistyped target becomes a compile error, not a runtime drift.
module FS.Skia.UI.Build.Targets

/// One case per dispatched build target. Closed union ⇒ exhaustive dispatch ⇒
/// `spec` is total ⇒ a target without metadata (or metadata without a target) is
/// unrepresentable (SC-003). The first 37 cases mirror the current `requiredTargets`
/// list, in order (feature 044 retired `SkillExamplesCheck`); `PackageSmoke`/
/// `BuildWorkflowCheck` are dispatched but not part of the runnable-target metadata
/// registry (FR-013).
type Target =
    | Clean
    | Restore
    | Build
    | Test
    | Dev
    | PackLocal
    | RefreshSurfaceBaselines
    | PackageSurfaceCheck
    | PerPackageSurfaceDiff
    | FsiTranscripts
    | SampleContractSmoke
    | TemplatePack
    | TemplateInstallSource
    | TemplateInstallPackage
    | TemplateInstantiate
    | TemplateSmoke
    | TemplateCheck
    | CapabilityCheck
    | SkillCheck
    | GeneratedProductCheck
    // Feature 088 (US2, FR-006): the GeneratedProductCheck split. The umbrella above stays
    // resolvable (FR-007); these two sub-targets carry the cheap structural scan and the
    // expensive consumer validation respectively, so a structural break fails fast before the
    // consumer restore/build/Verify pays its cost.
    | GeneratedProductStructure
    | GeneratedConsumerValidation
    | ControlsCatalogCheck
    // Feature 066 (US3, FR-006): the typed-catalog generation-currency (drift) gate.
    | ControlsCatalogGenerationCheck
    // Feature 069 (US1, FR-006): the design-token generation-currency (drift) gate.
    | DesignTokenDrift
    // Feature 083 (US1, FR-007): the WCAG color-contrast gate over the shipped theme tokens.
    | ContrastCheck
    // Feature 078 (US1, FR-005): the controls-catalog docs currency/completeness/preview gate.
    | ControlsCatalogDocsCheck
    // Feature 106 (US2, FR-007): the Controls public-surface documentation-coverage gate.
    | ControlsDocCoverageCheck
    // Feature 080 (US2, FR-007/FR-012): the render-capable decoded-content fidelity gate.
    | ControlFidelityCheck
    | ControlsInteractionCheck
    | ControlsRenderingCheck
    | DependencyReport
    | SymbolCrossCheck
    | GeneratedGuidanceCheck
    | SkillSyncCheck
    | SkillQualityCheck
    | PhaseHookParityCheck
    | SkillContractPathCheck
    | TemplateUpdateSkillPackageCheck
    | TemplateDrift
    | EvidenceGraph
    | EvidenceAudit
    | AgentReady
    | TargetMetadata
    | TargetMetadataDrift
    | VerifyPreflight
    | CiPreflight
    | StaleBoundaryScan
    | FinalReadiness
    | Verify
    | Ci
    | Route
    // Feature 064 (FR-001/FR-006/FR-007): the distribution targets.
    | PrePublishCheck
    | Publish
    | PackageSmoke
    | BuildWorkflowCheck

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

/// The runnable-target registry, in registry order (replaces `requiredTargets`).
/// Excludes `PackageSmoke`/`BuildWorkflowCheck` (which are dispatched but never
/// carried metadata rows), preserving target-metadata.json parity.
val allTargets: Target list

/// Every dispatched target, in FAKE-registration order (`allTargets` plus the two
/// non-registry targets). Drives FAKE target creation and dependency wiring (FR-013).
val dispatchTargets: Target list

/// The single source of truth: total over `Target` (a missing case fails to compile).
val spec: target: Target -> TargetSpec

/// Canonical runnable-target string (for FAKE registration and report text).
val name: target: Target -> string

/// Direct prerequisites as typed targets (replaces the `targetDependencyRows` lookup).
val directPrerequisites: target: Target -> Target list

/// Derived string view of the runnable-target registry (build.fsx compatibility). Pure.
val requiredTargetNames: string list

/// Derived `(name, prereq-names)` rows over every dispatched target (replaces the
/// maintained `targetDependencyRows`). Pure.
val targetDependencyRows: (string * string list) list

/// Feature 088 (US1, FR-003): the single source for the gates a routing rule can require
/// plus the composites `Verify`/`Ci`. `AgentValidation.knownGates` derives from
/// `routableGates |> List.map name`, so a renamed gate can no longer drift the allowlist.
/// Rendered in `allTargets` registry order for byte-stable output.
val routableGates: Target list

/// Feature 088 (US1, FR-004): whether a target is a product-facing evidence gate (the
/// gates `Verify` reports as `ProductChecksRun`). Excludes preflight/pack/internal steps.
val isProductCheck: target: Target -> bool

/// Feature 088 (US1, FR-004): the single source for `Verify`'s `ProductChecksRun`, in the
/// pinned canonical order the verdict has always rendered. `productCheckGates |> List.map
/// name` equals the prior hand-maintained literal byte-for-byte and in order.
val productCheckGates: Target list

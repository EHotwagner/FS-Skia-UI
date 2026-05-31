// TargetMetadata.fsi — target-metadata derivation, drift validation, and report
// rendering (feature 041, FR-002; Principle II). Build-tooling only.
//
// Pure over its typed inputs; the file/repo reads that feed `validateAgainstRepo`
// (validation.contract.yml + docs scans) stay at the build.fsx edge and are passed in.
// `metadataJson` takes the timestamp as a parameter so the function is deterministic
// (R2): the build injects DateTimeOffset.UtcNow; tests inject a fixed value.
module FS.Skia.UI.Build.TargetMetadata

/// Per-target descriptor, computed from `Targets.TargetSpec` + edge-supplied paths.
type TargetMetadata =
    { RunnableTargetName: string
      DirectPrerequisites: string list
      ExpectedOutputs: string list
      StaleAssumptions: string list
      TimeoutClass: string
      Cost: string
      Authority: string
      FailureOwner: string
      Command: string }

/// Typed drift findings (the cases unit tests assert against — SC-004).
type TargetMetadataDrift =
    | MissingRunnableTarget of string
    | MissingMetadata of string
    | MissingExpectedOutput of string
    | MissingFailureOwner of string
    | DependencyDivergence of string

/// Structural drift between the runnable-target registry and the metadata rows. Pure.
val validateMetadataDrift: runnableTargets: string list -> metadata: TargetMetadata list -> TargetMetadataDrift list

/// Full repo drift: structural drift + contract-reference drift + docs-reference drift.
/// `contractReferences`/`docReferences` are the (edge-read) target names found in
/// validation.contract.yml and the docs set. Returns diagnostic strings. Pure.
val validateAgainstRepo:
    contractReferences: string list ->
    docReferences: string list ->
    runnableTargets: string list ->
    metadata: TargetMetadata list ->
        string list

/// Exact per-drift message string (e.g. "missing metadata: Foo"). Pure.
val driftDiagnostic: drift: TargetMetadataDrift -> string

/// The `TargetMetadata` JSON report. `generatedAtUtc` is a parameter (R2). Pure.
val metadataJson: generatedAtUtc: string -> diagnostics: string list -> metadata: TargetMetadata list -> string

/// The `# Target Metadata Drift` markdown report (PASS line or FAIL + diagnostics). Pure.
val driftMarkdown: diagnostics: string list -> string

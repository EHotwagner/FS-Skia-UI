// Findings.fsi — uniform structured validation result for every extracted validator
// (feature 041, FR-004; Principle II: visibility lives in the .fsi).
//
// Build-tooling scope only (build/Governance under net10.0). NOT part of the tracked
// runtime surface baselines. Moved verbatim from build.fsx so report/failure text is
// reconstructed byte-identically (FR-006).
module FS.Skia.UI.Build.Findings

/// A single validation violation, rendered into report/failure text.
type ValidationFinding =
    { ArtifactClass: string
      Path: string
      Rule: string
      Message: string }

/// Construct a finding (was the inline `finding` helper in build.fsx ~2310). Pure.
val finding: artifactClass: string -> path: string -> rule: string -> message: string -> ValidationFinding

/// One `- `{Path}` [{Rule}]: {Message}` line per finding, joined by newlines —
/// the exact detail block `writeFindingsOrPass` produced. Pure.
val renderDetail: findings: ValidationFinding list -> string

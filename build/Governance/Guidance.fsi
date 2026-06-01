// Guidance.fsi — generated-guidance / skill-section scanners (feature 045, T012).
// Behaviour-preserving relocation; the curated surface is the single gate entry point.
module FS.Skia.UI.Build.Guidance

open FS.Skia.UI.Build.Engine.Model

/// Validate generated spec/plan/task/constitution guidance + skill-id resolution; writes a
/// byte-identical report to outputPath and fails (failwithf) with the collected findings.
val runGeneratedGuidanceScan: model: BuildModel -> outputPath: string -> unit

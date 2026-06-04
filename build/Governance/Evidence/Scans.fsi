// Scans.fsi — readiness-contract, persistent-launch, persistent-gui-runtime and
// window-visibility readiness scans (feature 043, FR-006a; Principle II).
//
// Build-tooling only. Pure over supplied readiness file contents — faithfully
// ports the four heredoc scans embedded in run-audit.sh. Each scan preserves the
// Python blocking severity, hit vocabulary and output JSON shape.
namespace FS.Skia.UI.Build.Evidence

/// Inputs for the readiness scans, all read at the build.fsx edge:
///  - `ReadinessDir`: absolute readiness directory (used in hit path strings).
///  - `FeatureText`: spec.md + plan.md + tasks.md joined with "\n" (marker probe).
///  - `ReadinessFiles`: (readiness-relative forward-slash path, contents) for
///    every file under readiness, recursively.
type ScanInput =
    { ReadinessDir: string
      FeatureText: string
      ReadinessFiles: (string * string) list }

module Scans =

    /// readiness-contract scan — always runs; three required contract files.
    val readinessContract: ScanInput -> ScanResult

    /// persistent-launch scan — gated by the persistent-launch feature markers.
    val persistentLaunch: ScanInput -> ScanResult

    /// persistent-gui-runtime readiness scan — gated by its feature markers.
    val persistentGui: ScanInput -> ScanResult

    /// window-visibility readiness scan — gated by its feature markers.
    val windowVisibility: ScanInput -> ScanResult

    /// FR-005 (062): the window-visibility evidence-format schema text
    /// (per-file required keys + the `diagnostic-class` value set), single-sourced
    /// from EvidenceFormatSchema. Printed on a failing window-visibility class.
    val windowVisibilitySchemaText: unit -> string

// DiffScan.fsi — audit-patterns.yml pattern scan over a supplied unified diff
// (feature 043, FR-010; Principle II). Pure: git is invoked at the build.fsx
// edge and the diff text is passed in as data. Faithfully ports the diff-scan
// heredoc in run-audit.sh.
namespace FS.Skia.UI.Build.Evidence

/// A single diff-scan hit (the diff-scan-hits.json hit shape).
type DiffHit =
    { File: string
      Line: int
      Pattern: string
      Severity: string
      Reason: string
      Match: string }

/// diff-scan result: `{base_ref, blocking[], advisory[]}`. `BaseRef` is always
/// `None` (the Python engine writes `"base_ref": null`).
type DiffScanResult =
    { BaseRef: string option
      Blocking: DiffHit list
      Advisory: DiffHit list }

module DiffScan =

    /// Scan `unifiedDiff` against the patterns/whitelist/severity-overrides in
    /// `patternsYml` (read via YamlDotNet). Pure. FR-010.
    val scan: patternsYml: string -> unifiedDiff: string -> DiffScanResult

// StatusRegion.fsi — audit-status fenced-region scanner (feature 043, FR-006;
// Principle II). Faithfully ports audit-status-scan.py.
//
// Build-tooling only. Pure over file text. Also declares the shared ScanHit /
// ScanResult types consumed by Scans, DiffScan, Audit and Render (this module
// compiles before all of them).
namespace FS.Skia.UI.Build.Evidence

/// A readiness-scan hit. Optional fields are emitted by the renderer only when
/// `Some`, reproducing the per-scan JSON object shapes byte-for-byte.
type ScanHit =
    { Path: string
      Reason: string
      Status: string option
      Missing: string list option
      MissingTerms: string list option
      MissingSections: string list option
      Required: string list option
      Blocking: bool option
      ValidationArea: string option }

/// One readiness scan's result: its area name, hits, and blocking count.
type ScanResult =
    { Area: string
      Hits: ScanHit list
      BlockingCount: int }

/// Per-file audit-status resolution (the scan_file shape): status is one of
/// "no-region" | "pass" | "block" | "unreadable".
type StatusScan =
    { Path: string
      Status: string
      Blocking: bool
      Reasons: string list
      Values: (string * string) list }

module ScanHit =
    /// A minimal {path, reason} hit (all optionals None).
    val basic: path: string -> reason: string -> ScanHit

module StatusRegion =

    /// Resolve a single file's audit-status region (pure). `path` is carried for
    /// reporting only; the logic reads `text`. FR-006.
    val scanText: path: string -> text: string -> StatusScan

    /// Aggregate audit-status scan over (path, contents) pairs. `Area` =
    /// "audit-status"; `BlockingCount` = number of files that block.
    val scan: files: (string * string) list -> ScanResult

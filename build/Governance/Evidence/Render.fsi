// Render.fsi — byte-parity serializers for task-graph.json, task-graph.md and
// the audit count block (feature 043, FR-007; Principle II).
//
// Build-tooling only. Pure string producers. Output is byte-identical to the
// Python engine for identical inputs (the golden-fixture parity oracle):
// json.dumps(indent=2, ensure_ascii=True) formatting, 4-space markdown nesting,
// the Mermaid classDef CSS, the ASCII tree glyphs, and the exact trailing
// newline are all reproduced.
namespace FS.Skia.UI.Build.Evidence

module Render =

    /// Serialize the graph result to task-graph.json (FR-007).
    val taskGraphJson: GraphResult -> string

    /// Serialize the graph result to task-graph.md (FR-007).
    val taskGraphMd: GraphResult -> string

    /// The audit-counts.txt count block for `featureName` (FR-008 oracle).
    val auditCounts:
        featureName: string ->
        realTasks: int ->
        acceptedSeh: int ->
        unacceptedSynthetic: int ->
        autoSynthetic: int ->
        lateSeh: int ->
            string

    /// Serialize a readiness scan's hits to its `*-hits.json` shape (per-area
    /// key ordering; empty -> "[]\n"). `area` selects the key layout.
    val scanHitsJson: area: string -> ScanResult -> string

    /// Serialize diff-scan-hits.json (`{base_ref, blocking[], advisory[]}`).
    val diffScanJson: DiffScanResult -> string

    /// Serialize audit-status-hits.json (`{scanned_files, blocking}`).
    val auditStatusJson: scannedFiles: string list -> blocking: string list -> string

    /// Serialize seh-audit-summary.json.
    val sehAuditSummaryJson: SehSummary -> string

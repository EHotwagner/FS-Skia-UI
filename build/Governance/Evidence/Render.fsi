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

    /// Feature 089 (EVGRAPH-ECHO-1, FR-008/FR-009): the pure `skillist id →
    /// SKILL.md path` resolution block, reusing the SAME `SkillRegistry` the
    /// Audit validator consults so the echo cannot disagree with the gate.
    /// Resolved ids render first; alias / ambiguous / unresolved ids are grouped
    /// into a distinct flagged section. Unit-tested directly (no parallel
    /// resolver). Pure.
    val skillistResolution: registry: SkillRegistry -> ids: string list -> string

    /// Serialize the graph result to task-graph.md (FR-007). Feature 089 threads
    /// the `SkillRegistry` so the rendered output echoes each declared skillist
    /// id's resolution (`skillistResolution`).
    val taskGraphMd: registry: SkillRegistry -> GraphResult -> string

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

    /// Serialize seh-audit-summary.json (feature 087 C1: three-state `verdict`,
    /// separated accepted/unaccepted synthetic counts, and durable
    /// `acceptedDeferrals` records, alongside the existing summary lists).
    val sehAuditSummaryJson: AuditResult -> string

    /// FR-007 (061): the explicit terminal `verdict=…` line for an EvidenceGraph
    /// run (clean: `verdict=ok (no cycles, no dangling refs, no [S*])`; failing:
    /// `verdict=error (<reason>)`). Additive to exit-code semantics.
    val graphVerdictLine: GraphResult -> string

    /// FR-004 (061): the self-describing readiness-contract failure diagnostic —
    /// per failing file, its name, status, full enforced `required-tokens`, and
    /// the `missing` subset, derived from the same data that enforces the rule.
    /// Empty string when the scan has no hits.
    val readinessContractDiagnostics: ScanResult -> string

    /// FR-008 (084): one legible block per blocker — `(area, hitsFileName, scanResult)`
    /// triples render to area + file + one-line reason + absent/missing detail + the
    /// originating hit-file path, so the audit is self-sufficient on stdout. Empty
    /// string when no area has hits.
    val auditBlockerDiagnostics: (string * string * ScanResult) list -> string

    /// FR-009 (084): the diff-scan base-ref line — resolved base ref (with merge-base
    /// sha when known) or an explicit absence message.
    val diffScanBaseRefLine: baseRef: string option -> mergeBase: string option -> string

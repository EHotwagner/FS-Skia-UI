// EvidenceFormatSchema.fsi — single source of every evidence-format's required
// shape (feature 062, FR-005, D5). Both the FR-005 failing-class diagnostics
// (Render) and the generated `template/base/docs/evidence-formats.md` reference
// (ApiSurfaceGen-style) derive from the SAME constants here, and the enforcing
// scans/audit/task-parser reference these same lists where they validate — so the
// printed schema, the generated reference, and the validator can never drift.
// Build-tooling only. Pure data. Visibility lives here (Principle II).
namespace FS.Skia.UI.Build.Evidence

/// The evidence-format classes whose required shape is single-sourced and
/// recoverable without decompiling (SC-002).
type EvidenceFormatClass =
    | ReadinessContract
    | SkillLoadingEvidence
    | WindowVisibility
    | SehAcceptance

/// The per-file required shape (data-model §2). One value per enforced file.
type EvidenceFormatSchema =
    { FileName: string
      FormatClass: EvidenceFormatClass
      /// The full enforced token/key list (the same list the validator checks).
      RequiredTokens: string list
      /// Column names for tabular formats (the 8-column skill-loading-evidence row).
      TableColumns: string list option
      /// Ordering constraints, e.g. `loaded_at < work_started_at`.
      OrderingRules: string list
      /// Resolved-path pattern for path-bearing rows, e.g. `.agents/skills/<id>/SKILL.md`.
      ResolvedPathPattern: string option
      /// Whether a violation hard-blocks the audit.
      Blocking: bool }

module EvidenceFormatSchema =

    /// Human-readable label for a format class (used by the diagnostics + doc).
    val classLabel: EvidenceFormatClass -> string

    // --- single-source constant lists (referenced by the enforcing code) -----

    /// The 8 columns of one `skill-loading-evidence.md` row, in order.
    val skillLoadingColumns: string list

    /// The `loaded_at < work_started_at` ordering rule text.
    val skillLoadingOrderingRule: string

    /// The resolved `SKILL.md` path pattern recorded per `(task, skill)` row.
    val skillLoadingPathPattern: string

    /// The closed `diagnostic-class` value set for window-visibility evidence.
    val windowDiagnosticClasses: string list

    /// The required `key=value` keys in `interactive-visible-window.md`.
    val interactiveVisibleWindowKeys: string list

    /// The SEH acceptance tokens (acceptance status + approval label), no backticks.
    val sehAcceptanceTokens: string list

    /// The readiness-contract `(fileName, requiredTokens, reason)` checks — the
    /// canonical source the `Scans.readinessContract` scan enforces.
    val readinessContractChecks: (string * string list * string) list

    // --- the enumerated schema (consumed by Render + the generated doc) ------

    /// Every enforced evidence-format file, in a stable order.
    val schemas: EvidenceFormatSchema list

    /// Render the complete required shape of one schema entry (file name, required
    /// tokens/columns, ordering rules, resolved-path pattern, blocking) — the text
    /// printed by the FR-005 diagnostics and listed in `docs/evidence-formats.md`.
    val renderSchema: schema: EvidenceFormatSchema -> string

    /// Render every schema entry of a format class (FR-005 per-class diagnostic).
    /// Empty string when the class has no entries.
    val renderClass: formatClass: EvidenceFormatClass -> string

    /// The repo-relative emitted path of the generated reference doc.
    val referenceDocPath: string

    /// Render the full `docs/evidence-formats.md` reference (FR-005, D5) from the
    /// same `schemas` the validators enforce — deterministic, byte-stable, no
    /// clock/env. Currency-checked so it cannot drift from the enforcing constants.
    val renderReferenceDoc: unit -> string

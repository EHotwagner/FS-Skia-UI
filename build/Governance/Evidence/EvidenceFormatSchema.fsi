// EvidenceFormatSchema.fsi — single source of every evidence-format's required
// shape (feature 062, FR-005, D5). Both the FR-005 failing-class diagnostics
// (Render) and the generated `template/base/docs/evidence-formats.md` reference
// (ApiSurfaceGen-style) derive from the SAME constants here, and the enforcing
// scans/audit/task-parser reference these same lists where they validate — so the
// printed schema, the generated reference, and the validator can never drift.
// Build-tooling only. Pure data. Visibility lives here (Principle II).
namespace FS.Skia.UI.Build.Evidence

/// FR-007: the three-state merge-gate verdict (replaces the binary Pass|Fail).
/// Single-sourced here so the audit, the renderers, and the interpreter edge
/// reference one definition. `PassWithAcceptedDeferrals` is reachable only with
/// zero unaccepted synthetic and zero blocking hits (FR-011 invariant).
type AuditVerdict =
    | Pass
    | PassWithAcceptedDeferrals
    | Fail

/// FR-008: a durable accepted-deferral record (synthetic-evidence.json).
type AcceptedDeferral =
    { TaskId: string
      Justification: string
      RealEvidencePath: string
      AwaitedHostCapability: string }

/// FR-010: skill-loading-evidence provenance — captured (observed during the
/// run) vs asserted (hand-authored).
type LoadProvenance =
    | Captured
    | Asserted

/// FR-002: per-step generated-product failure classification.
type StepClassification =
    | ProductDefect
    | Environment

/// FR-004: the explicit package-source tag a generated-product report states.
type PackageSet =
    | LocalPacked
    | Pinned

/// FR-002: a per-step generated-product result. `Classification` is only meaningful when
/// `Passed = false`. `PackageSet` records which package source the step exercised (FR-004).
type GeneratedProductStepResult =
    { Step: string
      Passed: bool
      Classification: StepClassification
      PackageSet: PackageSet }

/// FR-002: the overall generated-product verdict aggregated over per-step results.
/// `ProductDefectFail` iff ANY step failed as a `ProductDefect`; `EnvironmentNonAuthoritative`
/// when the only failures are `Environment` (reported, never a clean green, never a defect);
/// `ProductPass` when every step passed.
type GeneratedProductVerdict =
    | ProductPass
    | EnvironmentNonAuthoritative
    | ProductDefectFail

/// FR-003: a static pinned-vs-local package-skew finding.
type PackageSkewFinding =
    { Symbol: string
      File: string
      PinnedVersion: string
      LocalVersion: string }

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

    // --- feature 087 label/parse helpers (single source) --------------------

    /// The JSON/string label for an `AuditVerdict` (the C1 verdict-state enum).
    val auditVerdictLabel: AuditVerdict -> string

    /// Whether a verdict is a passing state (`Pass` or `PassWithAcceptedDeferrals`).
    val isPassingVerdict: AuditVerdict -> bool

    /// The JSON/string label for a `StepClassification` (C5).
    val stepClassificationLabel: StepClassification -> string

    /// The JSON/string label for a `PackageSet` (C4/C5).
    val packageSetLabel: PackageSet -> string

    /// The JSON/string label for a `GeneratedProductVerdict` (FR-002).
    val generatedProductVerdictLabel: GeneratedProductVerdict -> string

    /// FR-002: map a generated-consumer failure category to its step classification. A
    /// genuine host-environment obstacle (`UnsupportedHost`, the unsupported-host signal)
    /// classifies as `Environment`; every product/contract failure category classifies as
    /// `ProductDefect`, so an environment obstacle can never relabel a product defect.
    val classifyGeneratedCategory: category: string -> StepClassification

    /// FR-002 (SC-002): aggregate per-step results into the overall verdict — `ProductDefectFail`
    /// iff any step failed as `ProductDefect` (an `Environment` failure can NEVER suppress it),
    /// else `EnvironmentNonAuthoritative` if any step failed, else `ProductPass`.
    val generatedProductVerdict: steps: GeneratedProductStepResult list -> GeneratedProductVerdict

    /// The skill-loading-evidence `provenance` column value (C3, FR-010).
    val loadProvenanceLabel: LoadProvenance -> string

    /// Parse a `provenance` column value; `None` for any value outside the
    /// closed `{ captured, asserted }` set.
    val parseProvenance: string -> LoadProvenance option

    // --- single-source constant lists (referenced by the enforcing code) -----

    /// The 8 columns of one `skill-loading-evidence.md` row, in order.
    val skillLoadingColumns: string list

    /// The `loaded_at < work_started_at` ordering rule text.
    val skillLoadingOrderingRule: string

    /// Feature 087 (FR-010): the closed `provenance ∈ { captured, asserted }`
    /// value-set rule text for the 9th skill-loading-evidence column.
    val skillLoadingProvenanceRule: string

    /// The resolved `SKILL.md` path pattern recorded per `(task, skill)` row.
    val skillLoadingPathPattern: string

    /// The closed `diagnostic-class` value set for window-visibility evidence.
    val windowDiagnosticClasses: string list

    /// The required `key=value` keys in `interactive-visible-window.md`.
    val interactiveVisibleWindowKeys: string list

    /// The complete window-visibility readiness file set the engine enforces,
    /// single-sourced so `Scans.windowVisibility` and the generated
    /// `evidence-formats.md` share one ordered list (FR-007 / SC-003).
    val windowVisibilityFiles: string list

    /// The required native-window facts in `window-state-diagnostics.md`.
    val windowNativeFacts: string list

    /// The required `option=` rows in `window-options.md`.
    val windowOptionRows: string list

    /// The close-reason vs evidence-close separation keys in `close-reason-separation.md`.
    val closeReasonSeparationKeys: string list

    /// The decodable-image proof keys in `real-image-evidence.md`.
    val realImageEvidenceKeys: string list

    /// The required generated-project validation fields in `generated-validation.md`.
    val generatedValidationKeys: string list

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

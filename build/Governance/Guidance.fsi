// Guidance.fsi — generated-guidance / skill-section scanners (feature 045, T012).
// Behaviour-preserving relocation; the curated surface is the single gate entry point.
module FS.Skia.UI.Build.Guidance

open FS.Skia.UI.Build.Engine.Model
open FS.Skia.UI.Build.Findings

// Feature 046 (US1, FR-001/002/003): Constitution-Check completeness validator.
// A pure parser over the active feature's plan.md Repository Governance Decisions
// section. Build-tooling scope (NOT a tracked product surface baseline). Folded into
// the existing GeneratedGuidanceCheck gate — no new top-level FAKE target (A5).

/// One canonical required governance-decision area (FR-001, R2). The set is
/// hard-coded; adding/removing one is a deliberate code change + test.
type RequiredDecisionArea =
    { Id: string
      DisplayName: string }

/// Whether a single area's body carries a real decision.
type AreaStatus =
    | Filled                  // a real decision (incl. explicit N/A-with-rationale)
    | Empty                   // body blank / heading only / area bullet absent
    | StillBoilerplate        // body still the template's verbatim prompt text
    | PlaceholderUnresolved   // body contains NEEDS CLARIFICATION / an unresolved-work marker

/// Outcome of classifying a plan against the required areas.
type ConstitutionCheckResult =
    | TemplateRecognized of areas: (RequiredDecisionArea * AreaStatus) list
    | UnrecognizedTemplateRevision of diagnostic: string   // FR-003

/// The canonical 11 required decision areas, in report order (FR-001).
val requiredDecisionAreas: RequiredDecisionArea list

/// Classify the Repository Governance Decisions section of a plan's content.
val classifyConstitutionCheck: planContent: string -> ConstitutionCheckResult

/// Derive build-failing findings from a classification (one per unfilled area, or
/// one for the unrecognized-template-revision case). Empty list => pass.
val constitutionCheckFindings: planPath: string -> ConstitutionCheckResult -> ValidationFinding list

/// Validate generated spec/plan/task/constitution guidance + skill-id resolution; writes a
/// byte-identical report to outputPath and fails (failwithf) with the collected findings.
val runGeneratedGuidanceScan: model: BuildModel -> outputPath: string -> unit

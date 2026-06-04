// Guidance.fsi — generated-guidance / skill-section scanners (feature 045, T012).
// Behaviour-preserving relocation; the curated surface is the single gate entry point.
module FS.Skia.UI.Build.Guidance

open FS.Skia.UI.Build.Engine.Model
open FS.Skia.UI.Build.Findings

// Feature 055 (US1/US2, FR-001..006): decouple author-guidance prose from
// generation-currency anchors. Build-tooling scope (NOT a tracked product surface
// baseline); exported so the pure-core unit tests can exercise the evaluator and the
// real check values directly. The gate entry point `runGeneratedGuidanceScan` is
// unchanged.

/// A literal string consumed by tooling/parsers. Matched verbatim
/// (case-insensitive substring), exactly as the pre-055 table did.
type ContractToken =
    { Token: string
      Files: string list }

/// How an obligation's concept anchors are evaluated.
type MatchMode =
    | AnyOf // satisfied when ANY concept anchor is present (the US1 unlock)
    | AllOf // satisfied only when ALL concept anchors are present (conjunctive rules)

/// A rule a source of truth imposes that some derived guidance file must reflect.
/// Checked by presence-of-concept, not exact wording, so prose may be reworded.
type GuidanceObligation =
    { Id: string
      SourceOfTruth: string
      Concepts: string list
      Mode: MatchMode
      Files: string list }

/// What a check enforces, after decoupling.
type GuidanceCheck =
    { Tag: string
      Tokens: ContractToken list
      Obligations: GuidanceObligation list
      Forbidden: ContractToken list }

/// Pure: given a lookup from relative path to file content (None = missing file),
/// produce the findings for one check. No IO.
val evaluateGuidanceCheck: lookup: (string -> string option) -> check: GuidanceCheck -> string list

/// The decoupled task-skillist guidance check value (tokens + obligations).
val taskSkillistGuidanceCheck: GuidanceCheck

/// The decoupled controls-boundary guidance check value (tokens + obligations + forbidden).
val controlsBoundaryGuidanceCheck: GuidanceCheck

/// The `fake-sequential` semantic obligation (AllOf over the four FAKE-serialization facets).
val serializedRunnerObligation: GuidanceObligation

/// FR-007/SC-005: honest prose-size accounting record.
type ProseSizeAccounting =
    { Baseline: int
      AgentsSkillsLines: int
      SpecifyLines: int
      Current: int
      Delta: int
      RestatedTarget: string }

/// Pure, byte-deterministic render of the prose-size accounting report.
val renderProseSizeAccounting: accounting: ProseSizeAccounting -> string

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

// Feature 062 (FR-001, D12): low-cost feedback-hook regression guard, folded into
// the GeneratedGuidanceCheck gate. Build-tooling scope. The pure core is exported
// so the failing-first unit test can exercise it directly.

/// Pure: given the relative path and the yaml text of
/// `template/feedback/extensions/feedback.yml`, return one finding per surviving
/// `optional: true` line (FR-001 requires every feedback hook to be mandatory).
val feedbackHookOptionalFindings: relativePath: string -> yamlText: string -> string list

/// Validate generated spec/plan/task/constitution guidance + skill-id resolution; writes a
/// byte-identical report to outputPath and fails (failwithf) with the collected findings.
val runGeneratedGuidanceScan: model: BuildModel -> outputPath: string -> unit

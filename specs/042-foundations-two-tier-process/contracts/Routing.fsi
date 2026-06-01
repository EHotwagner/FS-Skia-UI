// Routing.fsi — the typed two-tier development-process selector (feature 042, FR-001;
// Principle II). Single source of truth for tiers, the framework-author/consumer-agent
// axis, the routing rules, and the dogfood feature set. Build-tooling only; pure (git
// invocation and File.Exists stay at the build.fsx `Route` edge). A renamed/mistyped
// gate is a compile error because gate lists are `Targets.Target` values (SC-006).
module FS.Skia.UI.Build.Routing

open FS.Skia.UI.Build

/// Who is changing what. Defaults to FrameworkAuthor (the light path; no required
/// argument). `--developer-class consumer-agent` raises the floor explicitly; consumer-
/// contract PATHS escalate regardless of this value (FR-002).
type DeveloperClass =
    | FrameworkAuthor
    | ConsumerAgent

/// A typed validation level. The first five are the authoritative tiers; Tier1/Tier2 are
/// retained legacy aliases mapped onto the same severity lattice (R3).
type Tier =
    | InnerLoop
    | FocusedAuthority
    | AgentReady
    | MaintainerVerify
    | AutomationFinal
    | Tier1
    | Tier2

/// Severity rank for "highest applicable tier wins" (FR-003). Total over Tier. Pure.
val tierRank: tier: Tier -> int

/// The set of changed paths the selector reasons over: the UNION of the branch-vs-merge-
/// base diff and the uncommitted/untracked working tree (FR-002a). Repo-relative paths.
type Diff = { ChangedPaths: string list }

/// A typed routing rule: a glob predicate over a Diff yielding a Tier, its required gates
/// (typed Targets.Target — mistype ⇒ compile error), and the artifacts `--enforce` checks.
type RoutingRule =
    { Id: string
      Matches: Diff -> bool
      Tier: Tier
      RequiredGates: Targets.Target list
      ExpectedArtifacts: string list
      FailureOwner: string }

/// The resolved selection for a change: the class used, the escalated tier, the minimal
/// de-duplicated gate list (registry order, deterministic output), the ids of matched
/// rules, the union of expected artifacts, and whether a dogfood override forced it.
type Selection =
    { DeveloperClass: DeveloperClass
      Tier: Tier
      Gates: Targets.Target list
      MatchedRuleIds: string list
      ExpectedArtifacts: string list
      DogfoodForced: bool }

/// The compiled routing-rule table (replaces the YAML `routing_rules` as source of truth).
val rules: RoutingRule list

/// The framework-author inner-loop base gates: `[Dev]`. A public `src/**/*.fsi` surface edit does
/// NOT add a check here — it escalates via the `package-surface` rule (FR-002; F1).
val innerLoopGates: Targets.Target list

/// The full serialized dogfood/maintainer pipeline gate set (FR-006/FR-015):
/// Dev, GeneratedGuidanceCheck, TemplateCheck, GeneratedProductCheck, EvidenceGraph,
/// EvidenceAudit — in serialized order.
val fullPipelineGates: Targets.Target list

/// Typed dogfood feature ids: governance policy in the compiled source of truth (ADR D6).
/// Includes "042" (this feature is a named dogfood feature, FR-006).
val dogfoodFeatureIds: string list

/// True when `featureId` is a dogfood feature. Pure.
val isDogfood: featureId: string -> bool

/// PURE core selection (FR-002/FR-003): default-deny unmatched paths to the broad fallback,
/// escalate to the highest matched tier, never route a consumer-contract change to inner-loop.
/// `ConsumerAgent` raises the base floor to `FocusedAuthority` (path escalation composes on top).
/// Does NOT apply the dogfood override (that needs the active feature id).
val select: developerClass: DeveloperClass -> diff: Diff -> Selection

/// Selection with the dogfood override applied: if `featureId` is a dogfood feature, force
/// `fullPipelineGates`/`MaintainerVerify` regardless of the diff's tier (FR-006, SC-005).
val selectForFeature: developerClass: DeveloperClass -> featureId: string -> diff: Diff -> Selection

/// PURE `--enforce` core (FR-005, SC-003): the selection's expected artifacts NOT present
/// in `present`. The edge builds `present` from File.Exists. Empty ⇒ enforce passes.
val unmetArtifacts: present: Set<string> -> selection: Selection -> string list

/// The single-line human diagnostic for an `--enforce` failure naming each missing artifact
/// and the requiring tier (FR-005). Pure.
val enforceDiagnostic: selection: Selection -> missing: string list -> string

/// The deterministic `Route` report text: developer-class, tier, and gate list (SC-001/2).
/// Pure given the resolved selection.
val renderSelection: selection: Selection -> string

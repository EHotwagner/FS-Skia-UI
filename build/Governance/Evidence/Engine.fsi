// Engine.fsi — runGraph / runAudit orchestration entry points (feature 043;
// Principle II/IV). The Engine is PURE: every read (tasks.md, tasks.deps.yml,
// readiness files, the skill registry, the git diff) is performed at the
// build.fsx interpreter edge and supplied here as data; the Engine returns
// typed results plus the artifact texts to write.
namespace FS.Skia.UI.Build.Evidence

/// All inputs the interpreter has already read for one feature.
[<NoEquality; NoComparison>]
type EvidenceInputs =
    { FeatureName: string
      TasksMd: string
      DepsYml: string
      Registry: SkillRegistry
      /// readiness/skill-loading-evidence.md text, or None when absent.
      SkillLoadingEvidence: string option
      /// Does a skill-loading-evidence row's resolved path exist as a file?
      ResolvedExists: string -> bool
      /// Canonicalise a repo-relative or absolute path to compare resolved skill
      /// paths (Path.GetFullPath at the edge).
      Canonicalize: string -> string
      /// .specify/feature.json recorded feature_directory basename, or None.
      RecordedFeature: string option
      Scan: ScanInput
      /// (path, contents) for readiness .md files containing an audit-status region.
      AuditStatusFiles: (string * string) list
      PatternsYml: string
      /// FR-009: the resolved diff-scan base ref (default branch), or None when no
      /// default-branch ancestor resolves; threaded into DiffScanResult.BaseRef.
      BaseRef: string option
      UnifiedDiff: string
      /// Feature 087 (FR-008): durable accepted-deferral records parsed from
      /// readiness/synthetic-evidence.json at the interpreter edge.
      AcceptedDeferrals: AcceptedDeferral list }

/// The two graph artifacts to write under readiness/.
type GraphArtifacts =
    { TaskGraphJson: string
      TaskGraphMd: string }

/// Every audit artifact to write under readiness/.
type AuditArtifacts =
    { TaskGraphJson: string
      TaskGraphMd: string
      AuditCounts: string
      SehAuditSummary: string
      ReadinessContractHits: string
      PersistentLaunchHits: string
      PersistentGuiRuntimeHits: string
      WindowVisibilityHits: string
      AuditStatusHits: string
      DiffScanHits: string }

module Engine =

    /// Compute the graph result + the task-graph artifacts (pure).
    val runGraph: EvidenceInputs -> GraphResult * GraphArtifacts

    /// Compute the merge-gate audit result + every audit artifact (pure).
    val runAudit: EvidenceInputs -> AuditResult * AuditArtifacts

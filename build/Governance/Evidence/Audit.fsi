// Audit.fsi — cross-file validate-and-merge, capability-trigger skill matching,
// skill-loading-evidence validation, SEH summary, and merge-gate verdict
// (feature 043, FR-006/FR-008; Principle II/V). Pure over supplied data.
namespace FS.Skia.UI.Build.Evidence

/// Result of merging tasks.md + tasks.deps.yml + the skill registry: the updated
/// records (skillist, explicit deps, skill-match assessments filled) and the
/// accumulated errors, in the Python `validate_and_merge` ordering.
type MergeResult =
    { Tasks: TaskRecord list
      Errors: string list }

/// The four merge-gate count classes plus structured diagnostics (FR-008).
type SehSummary =
    { AcceptedSehTasks: string list
      UnacceptedSyntheticTasks: string list
      AutoSyntheticTasks: string list
      LateSehTasks: string list
      Diagnostics: (string * string * string * string) list } // task, failedRule, source, requiredAction

/// Merge-gate verdict.
type AuditVerdict =
    | Pass
    | Fail

/// Aggregated audit result.
type AuditResult =
    { Verdict: AuditVerdict
      SehSummary: SehSummary
      RealTasks: int
      TotalBlockers: int
      DiffBlocking: int
      ReadinessContract: int
      PersistentLaunch: int
      PersistentGuiRuntime: int
      WindowVisibility: int
      AuditStatus: int }

module Audit =

    /// Capability-trigger matches for a task title: (skillId, triggerGroup,
    /// matchedTrigger). Faithfully ports expected_capability_matches.
    val expectedCapabilityMatches: title: string -> (string * string * string) list

    /// Cross-file consistency + skill merge + assessments (FR-006). Pure.
    val validateAndMerge: registry: SkillRegistry -> tasks: TaskRecord list -> deps: DepsModel -> MergeResult

    /// Validate readiness/skill-loading-evidence.md against the declared skillist
    /// (FR-006). `evidenceText` is None when the file is absent; `resolvedExists`
    /// reports whether a row's resolved path exists as a file (I/O injected).
    val validateSkillLoadingEvidence:
        tasks: TaskRecord list ->
        skills: Map<string, string list> ->
        evidenceText: string option ->
        resolvedExists: (string -> bool) ->
        canonicalize: (string -> string) ->
            string list

    /// SEH classification summary over the resolved tasks (FR-008).
    val sehSummary: ResolvedTask list -> SehSummary

    /// Aggregate the merge-gate verdict from the SEH summary and scan counts.
    /// `--accept-synthetic` never changes this (Principle V).
    val verdict:
        resolved: ResolvedTask list ->
        seh: SehSummary ->
        diffBlocking: int ->
        readinessContract: int ->
        persistentLaunch: int ->
        persistentGuiRuntime: int ->
        windowVisibility: int ->
        auditStatus: int ->
            AuditResult

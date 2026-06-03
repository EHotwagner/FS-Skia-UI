// Audit.fsi — cross-file validate-and-merge, owns-driven skill-ownership
// validation, skill-loading-evidence validation, SEH summary, and merge-gate
// verdict (feature 043, FR-006/FR-008; feature 059 FR-010; Principle II/V).
// Pure over supplied data.
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

    /// Cross-file consistency + skill merge + assessments (FR-006). Pure.
    /// Feature 059 (FR-010): the title-trigger capability matcher is removed;
    /// evidence ownership is read from each task's `owns:` field (closed
    /// vocabulary, implied-skill coupling validated here). Titles are free-form.
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

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

// `AuditVerdict` is single-sourced in EvidenceFormatSchema (feature 087, FR-007):
// the three-state Pass | PassWithAcceptedDeferrals | Fail.

/// Aggregated audit result.
type AuditResult =
    { Verdict: AuditVerdict
      SehSummary: SehSummary
      /// Feature 087 (FR-008): the accepted deferrals applied to this run, plus
      /// the separated accepted-vs-unaccepted synthetic counts.
      AcceptedDeferrals: AcceptedDeferral list
      AcceptedSyntheticCount: int
      UnacceptedSyntheticCount: int
      RealTasks: int
      TotalBlockers: int
      DiffBlocking: int
      ReadinessContract: int
      PersistentLaunch: int
      PersistentGuiRuntime: int
      WindowVisibility: int
      AuditStatus: int }

module Audit =

    /// FR-006 (062): the closed `owns:` vocabulary — each value paired with the
    /// skill it implies. Exposed so the generated docs/skillist-reference.md
    /// single-sources the same table this module enforces.
    val ownsVocabulary: (string * string) list

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

    /// Feature 087 (FR-010): the declared-but-unloaded `(taskId, skillId)` gaps
    /// surfaced AT IMPLEMENTATION TIME — for every task with a non-empty declared
    /// skillist, regardless of `[X]`/`[S]` status — so a missing load is reported
    /// when the declaring task is implemented, not deferred to the `[X]` flip.
    val skillLoadingGapsAtImplementation:
        tasks: TaskRecord list -> evidenceText: string option -> (string * string) list

    /// FR-005 (062): the skill-loading-evidence evidence-format schema text (the
    /// 8-column row, the `loaded_at < work_started_at` ordering rule, and the
    /// resolved `.agents/skills/<id>/SKILL.md` path), single-sourced from
    /// EvidenceFormatSchema. Printed when a skill-loading-evidence error is present.
    val skillLoadingEvidenceSchemaText: unit -> string

    /// SEH classification summary over the resolved tasks (FR-008).
    val sehSummary: ResolvedTask list -> SehSummary

    /// Feature 087 (FR-008): parse the durable accepted-deferral records from
    /// readiness/synthetic-evidence.json text (None / malformed → no deferrals).
    /// A record with an empty taskId or empty justification is ignored.
    val parseAcceptedDeferrals: jsonText: string option -> AcceptedDeferral list

    /// Aggregate the three-state merge-gate verdict (FR-007) from the SEH summary,
    /// the accepted-deferral set, and the scan counts. `PassWithAcceptedDeferrals`
    /// is reachable only with zero unaccepted synthetic AND zero blocking hits
    /// (FR-011); an accepted deferral can never mask a blocking hit.
    val verdict:
        resolved: ResolvedTask list ->
        seh: SehSummary ->
        acceptedDeferrals: AcceptedDeferral list ->
        diffBlocking: int ->
        readinessContract: int ->
        persistentLaunch: int ->
        persistentGuiRuntime: int ->
        windowVisibility: int ->
        auditStatus: int ->
            AuditResult

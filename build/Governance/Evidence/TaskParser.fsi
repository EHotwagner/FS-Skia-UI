// TaskParser.fsi — tasks.md line-grammar + Synthetic-Evidence Inventory parser
// (feature 043, FR-001; Principle II: visibility lives in the .fsi).
//
// Build-tooling only (build/Governance under net10.0). NOT a product surface.
// Pure over the file text — no I/O. Faithfully ports parse_tasks_md from the
// legacy compute-task-graph.py so the downstream renderers reproduce the Python
// engine byte-for-byte (the golden-fixture parity oracle).
namespace FS.Skia.UI.Build.Evidence

/// Declared status box on a tasks.md line. `[*]` is rejected as a parse error
/// (auto-synthetic `[S*]` is computed, never written), so it is not a case here.
type DeclaredStatus =
    | Pending
    | Done
    | Synthetic
    | Failed
    | Skipped

/// One row of the per-task skill-match assessment table. Field order is the
/// JSON key order the renderer must emit (parity).
type SkillAssessment =
    { TaskId: string
      DeclaredSkillist: string list
      CandidateSkillId: string option
      MatchedSignals: string list
      Confidence: string
      Ambiguity: string option
      ReviewerDisposition: string option
      Diagnostic: string }

/// Synthetic-error-handling metadata, merged from the tasks.md line annotations
/// and the Synthetic-Evidence Inventory table.
type SehMetadata =
    { Annotation: bool
      ApprovalLabel: bool
      DesignSource: string
      SyntheticInputClass: string
      ExpectedErrorBehavior: string
      Rationale: string
      AcceptanceStatus: string
      Diagnostics: string list }

/// A parsed tasks.md task line (≈ the Python `Task`). `Effective` and the
/// resolved deps/skillist are filled by later stages; the parser leaves
/// `Effective = declared`, `Skillist = []`, `ExplicitDeps = []`.
type TaskRecord =
    { Id: string
      Declared: DeclaredStatus
      Phase: int option
      Story: string option
      Tier: string option
      Parallel: bool
      LineNo: int
      Title: string
      SkillistMirror: string list option
      Skillist: string list
      ExplicitDeps: string list
      PhaseDeps: string list
      SkillMatchAssessments: SkillAssessment list
      Seh: SehMetadata }

/// Result of parsing tasks.md: id-keyed records (in first-seen order) plus the
/// accumulated parse errors. Mirrors the Python `(tasks, errors)` return so the
/// engine can render even when errors are present.
type TasksParse =
    { Tasks: TaskRecord list
      Errors: string list }

/// Internal text helper shared across the Evidence modules (Python splitlines).
[<RequireQualifiedAccess>]
module internal Lines =
    /// Split text the way Python str.splitlines() does for \n / \r\n / \r.
    val split: text: string -> string list

module TaskParser =

    /// The canonical lowercase status string ("pending"/"done"/…) the Python
    /// engine uses in JSON and counts.
    val declaredString: DeclaredStatus -> string

    /// True when the task is an accepted, fully-governed [SEH] synthetic task
    /// (the propagation/audit exclusion). Mirrors `is_accepted_seh`.
    val isAcceptedSeh: TaskRecord -> bool

    /// Parse tasks.md text into records + errors (pure). Phase-checkpoint edges
    /// are injected into each record's `PhaseDeps`; Synthetic-Evidence Inventory
    /// rows and SEH diagnostics are merged. FR-001.
    val parse: tasksMd: string -> TasksParse

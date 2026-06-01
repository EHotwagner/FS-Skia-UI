// SkillistView.fsi — public surface of the skillist derived-view renderer + currency
// checker (feature 044 / US2; Principle II: visibility lives in the .fsi).
//
// Build-tooling scope only (build/Governance under net10.0). Renders the DERIVED
// tasks.md `[skillist: …]` annotation from the CANONICAL tasks.deps.yml `skillist:`,
// and reports currency for the ACTIVE FEATURE only. Pure; consumes already-parsed
// inputs (the same `(taskId, skillist)` pairs Evidence.DepsParser / Evidence.TaskParser
// produce). No historical re-derivation (FR-007). The file read/write stays at the edge.
module FS.Skia.UI.Build.SkillistView

/// Per-task currency result for the active feature.
type SkillistCurrencyItem =
    { TaskId: string
      Canonical: string list
      DerivedNow: string list option
      ExpectedAnnotation: string
      IsStale: bool }

/// Render the derived annotation token from a canonical skillist.
/// `[]` -> `"[skillist: []]"`; `[a; b]` -> `"[skillist: a, b]"` (order preserved).
val renderAnnotation: canonical: string list -> string

/// Replace only the `[skillist: …]` token on a single tasks.md task line, preserving
/// every other byte of the line. Raises if the line carries no annotation token
/// (omitted metadata is invalid per the constitution's Local Agent Skills rule).
val spliceAnnotation: canonical: string list -> taskLine: string -> string

/// Compute per-task currency from canonical deps skillists and parsed tasks.md mirrors,
/// keyed by task id. `derivedMirror = None` means the annotation is absent (reported,
/// not inserted). Only tasks present in `canonicalByTask` are considered (active scope).
val currency:
    canonicalByTask: (string * string list) list ->
    derivedByTask: (string * (string list option)) list ->
        SkillistCurrencyItem list

/// Actionable currency diagnostic for the stale tasks, naming
/// `./fake.sh build -t RefreshSurfaceBaselines`; None when every task is current.
val currencyDrift: items: SkillistCurrencyItem list -> string option

/// Single-task asymmetric currency diagnostic — "the tasks.md `[skillist: …]` view for
/// <task> is stale relative to its canonical tasks.deps.yml source; regenerate …". Used
/// by the active-feature merge-gate (Evidence.Audit) to replace the old symmetric peer
/// complaint while delegating the rendered token to this module (FR-006/FR-012).
val staleDiagnostic: taskId: string -> canonical: string list -> string

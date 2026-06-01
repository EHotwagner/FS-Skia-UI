// Contract sketch (Phase 1) for build/Governance/SkillistView.fsi — US2.
// Renders the DERIVED tasks.md [skillist: …] annotation from the CANONICAL tasks.deps.yml
// skillist:, and reports currency for the ACTIVE FEATURE only. Pure; consumes already-parsed
// inputs from Evidence.DepsParser / Evidence.TaskParser. No historical re-derivation (FR-007).
namespace FS.Skia.UI.Build

module SkillistView =

    /// Per-task currency result for the active feature.
    type SkillistCurrencyItem =
        { TaskId: string
          Canonical: string list
          DerivedNow: string list option
          ExpectedAnnotation: string
          IsStale: bool }

    /// Render the derived annotation token from a canonical skillist.
    /// [] -> "[skillist: []]"; [a; b] -> "[skillist: a, b]" (order preserved).
    val renderAnnotation: canonical: string list -> string

    /// Replace only the [skillist: …] token on a single tasks.md task line, preserving every
    /// other byte of the line. Raises if the line carries no annotation token (invalid per
    /// the constitution's Local Agent Skills rule).
    val spliceAnnotation: canonical: string list -> taskLine: string -> string

    /// Compute per-task currency from canonical deps skillists and parsed tasks.md mirrors,
    /// keyed by task id. derivedMirror None = annotation absent (reported, not inserted).
    val currency:
        canonicalByTask: (string * string list) list ->
        derivedByTask: (string * (string list option)) list ->
            SkillistCurrencyItem list

    /// Actionable currency diagnostic for the stale tasks; None when every task is current.
    val currencyDrift: items: SkillistCurrencyItem list -> string option

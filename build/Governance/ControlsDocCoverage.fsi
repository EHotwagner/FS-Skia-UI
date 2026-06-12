module FS.Skia.UI.Build.ControlsDocCoverage

// Feature 106 (US2, FR-007): the Controls documentation-coverage gate. A pure analysis over
// the FS.Skia.UI.Controls public `.fsi` surface that flags any `///` summary which is the
// known placeholder boilerplate (`Public contract function|type|module exposed by this
// FS.Skia.UI package.`), empty, or a duplicate-only reworded placeholder shared across many
// members with no member-specific token. File reads + report write stay at the
// Engine/Interpret.fs edge (Principle IV) — `analyze` is pure over in-memory text.

/// Why a Controls public-surface member's `///` summary fails the documentation-coverage gate:
/// `Placeholder` = the known boilerplate sentence; `Empty` = no/whitespace summary on a
/// `val`/`type`/`module`; `DuplicateOnly` = a reworded generic sentence shared across many
/// members with no member-specific token (the anti-evasion rule).
type DocReason =
    | Placeholder
    | Empty
    | DuplicateOnly

/// One offending member: the repo-relative `File`, declaration `Line`, member `Identifier`,
/// the `Reason` rule that fired, and a human-actionable `Detail`.
type DocFinding =
    { File: string
      Line: int
      Identifier: string
      Reason: DocReason
      Detail: string }

/// True when a (whitespace-normalized) summary contains the placeholder boilerplate sentence
/// in any of its `function` / `type` / `module` / `value` wordings.
val isPlaceholderSummary: summary: string -> bool

/// Pure analysis over `(repoRelativePath, fileContent)` pairs: associates each leading `///`
/// block with the next `val`/`type`/`module`/`member` declaration and returns a finding for
/// every member violating the placeholder / empty / duplicate-only predicate.
val analyze: files: (string * string) list -> DocFinding list

/// One actionable line per finding (`File:Line Identifier — reason — detail`), used for the
/// `FailWith` diagnostics when the gate blocks the build.
val failureDiagnostics: findings: DocFinding list -> string list

/// The `readiness/doc-coverage.md` report body: the enumerated surface plus every finding, or
/// an explicit `findings=0 over N members across M files` pass line (no silent pass).
val renderReport: files: (string * string) list -> findings: DocFinding list -> string

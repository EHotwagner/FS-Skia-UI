// SymbolCrossCheck.fsi — mechanical, deterministic cross-artifact symbol
// set-difference (feature 062, FR-008, D8; Principle II).
//
// A pure function that extracts named symbols (Msg cases, union/Screen variants,
// entity record names, FR-/SC- IDs) from plan.md / data-model.md / tasks.md and
// reports symbols present in a PROPER SUBSET of the three artifacts — surfaced as
// findings for human judgment by the speckit-analyze detection pass G, never a
// hard merge gate. Build-tooling scope; unit-tested. No I/O.
module FS.Skia.UI.Build.SymbolCrossCheck

/// The kind of a named symbol (data-model §3).
type SymbolKind =
    | MsgCase
    | UnionOrScreenVariant
    | EntityRecord
    | FrId
    | ScId

/// The three cross-checked feature artifacts.
type Artifact =
    | Plan
    | DataModel
    | Tasks

/// One extracted symbol and the set of artifacts it appears in.
type Symbol =
    { Kind: SymbolKind
      Name: string
      PresentIn: Set<Artifact> }

/// Human-readable label for a kind (used in the rendered findings).
val kindLabel: SymbolKind -> string

/// Extract the named symbols from one artifact's text (pure).
val extract: artifact: Artifact -> text: string -> Symbol list

/// Cross-artifact set-difference: every symbol whose `PresentIn` is a non-empty
/// PROPER SUBSET of `{ Plan; DataModel; Tasks }` (present in some, missing from
/// others). Deterministic ordering. Pure.
val diff: plan: string -> dataModel: string -> tasks: string -> Symbol list

/// Render the analyze pass-G findings block (FR-008 contract C3). Empty findings
/// produce an explicit "no set-differences" line. Pure, byte-stable.
val render: findings: Symbol list -> string

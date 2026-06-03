// DepsParser.fsi — tasks.deps.yml reader (feature 043, FR-002; Principle II).
//
// Build-tooling only. Reads BOTH the object form ({deps, skillist}) and the
// legacy bare-list form via YamlDotNet behind a typed model — no bespoke
// hand-rolled YAML parser (FR-002). Pure over the file text; the read is at the
// build.fsx edge. The legacy-bare-list MIGRATION error and the missing-field
// errors are raised by Audit.validateAndMerge (matching the Python ordering).
namespace FS.Skia.UI.Build.Evidence

/// One task's dependency metadata. `Deps`/`Skillist` are `None` when the field
/// is absent (the Python `TaskMetadata` `None` sentinel that later errors).
/// `Owns` (feature 059, FR-010) is the optional gated-evidence ownership list;
/// `None` when the key is absent (most tasks own nothing). Its closed vocabulary
/// and implied-skill coupling are validated by `Audit.validateAndMerge`.
type DepsEntry =
    { Deps: string list option
      Skillist: string list option
      Owns: string list option
      LegacyBareList: bool }

/// Parsed tasks.deps.yml: ordered task ids, id-keyed entries, parse errors.
type DepsModel =
    { Order: string list
      Map: Map<string, DepsEntry>
      Errors: string list }

module DepsParser =

    /// Parse tasks.deps.yml text via YamlDotNet into the typed model (pure).
    /// An empty or unparseable document yields a blocking error (FR-002).
    val parse: depsYml: string -> DepsModel

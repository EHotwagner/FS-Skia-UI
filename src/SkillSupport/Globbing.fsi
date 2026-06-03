// Globbing.fsi — fsharp-io-globbing family.
//
// fnmatch-style glob matching, file discovery, and DiffPlex-based
// generation-currency diffing for the SkillSupport shipped library. Consumers in
// build/Governance (the Route path tables, the currency checks) call these.
// Visibility lives here (Principle II).
namespace FS.Skia.UI.SkillSupport

module Globbing =

    /// fnmatch-style match: `**` crosses `/`, `*` and `?` stay within a single
    /// path segment, and both inputs are `/`-normalized before matching.
    val isMatch: glob: string -> path: string -> bool

    /// Recursively enumerate files under `root` whose `/`-normalized path
    /// (relative to `root`) matches any of `globs`. Returns relative paths in
    /// deterministic (sorted) order.
    val discover: root: string -> globs: string list -> string list

    /// DiffPlex-based generation-currency diff: the lines that differ between the
    /// `expected` (regenerated) text and the `actual` (on-disk) text. An empty list
    /// means the on-disk artifact is current.
    val currencyDiff: expected: string -> actual: string -> string list

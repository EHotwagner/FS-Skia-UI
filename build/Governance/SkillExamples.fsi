// SkillExamples.fsi — public surface of the ` ```fsharp ` block extractor and
// tangler (feature 040, Principle II: visibility lives in the .fsi).
//
// Build-tooling scope only. Pure extraction + rendering; the SkillExamplesCheck
// target performs the disk reads/writes and the real `dotnet build`. The
// generated `.fs` files are derived artifacts — regenerated deterministically
// from the SKILL.md text on every run, never hand-edited.
module FS.Skia.UI.Build.SkillExamples

/// One ` ```fsharp ` fenced block extracted from a SKILL.md. `Source` is the
/// verbatim text between the fences (the single source of the example).
type CodeBlock =
    { SkillSlug: string
      BlockIndex: int
      StartLine: int
      Source: string }

/// `<slug>` with `-` replaced by `_`, used in the generated module namespace.
val underscoreSlug: slug: string -> string

/// The fully-qualified generated module name for a block, e.g.
/// `Skill.fsharp_parsing.Block01` (R1/R4 — stable identity for diagnostics).
val moduleName: block: CodeBlock -> string

/// Extract every ` ```fsharp ` block from `markdown` in document order.
/// `BlockIndex` is 1-based per skill; `StartLine` is the 1-based line of the
/// opening fence. Pure.
val extractBlocks: skillSlug: string -> markdown: string -> CodeBlock list

/// Render the generated `.fs` text for one skill's blocks: a `namespace
/// Skill.<slug_>` header followed by one `// source: <skillPath>:<startLine>`
/// comment and `module Block<NN>` wrapper per block (deterministic). Pure.
val renderSkillFile: skillRelPath: string -> blocks: CodeBlock list -> string

/// `extractBlocks` over the raw markdown of every governed skill (keyed by
/// slug), pairing each with its source path. Pure over its inputs.
val extractAll: skills: (string * string * string) list -> (string * CodeBlock list) list

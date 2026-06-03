// CodeGen.fsi — fsharp-code-generation family.
//
// Deterministic Markdown / Mermaid / ASCII-tree builders for the SkillSupport
// shipped library. Plain StringBuilder assembly — no code quotations, no
// reflection (Principle III). Consumers in build/Governance render their specific
// document layouts on top of these. Visibility lives here (Principle II).
namespace FS.Skia.UI.SkillSupport

module CodeGen =

    /// Render a Mermaid `graph TD` block from nodes and directed edges. Node and
    /// edge order is preserved; output is deterministic.
    val mermaidGraph: nodes: string list -> edges: (string * string) list -> string

    /// Render a GitHub-flavoured Markdown table. `headers` defines the columns;
    /// each row is rendered in order. Cell counts are not padded — callers supply
    /// rows matching the header arity.
    val markdownTable: headers: string list -> rows: string list list -> string

    /// Render a stable ASCII tree. `roots` are the top-level node labels; `children`
    /// maps a node label to its ordered child labels. Uses `├─`/`└─`/`│` connectors.
    val asciiTree: roots: string list -> children: (string -> string list) -> string

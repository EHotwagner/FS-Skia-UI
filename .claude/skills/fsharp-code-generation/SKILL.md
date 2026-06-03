---
name: fsharp-code-generation
description: Emit governance artifacts (Markdown/Mermaid/JSON) and generate typed F# source; when NOT to use code quotations.
compatibility: F# governance library (build/Governance) under net10.0; build-tooling scope only.
metadata:
  author: fs-skia-ui
  source: docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md
---

# fsharp-code-generation

Cookbook for the two *different* generation jobs in the port — keeping C10/C11 (text) distinct from C12
(F# source) is its main pitfall. Owns **C10** (document/text generation — `task-graph.md` Mermaid +
ASCII tree + count tables), **C11** (document generation + currency check — `.claude/skills/**` from
`.agents/skills/**`, skillist render), and **C12** (one-shot F# *source* generation). Verdicts from the
capability report (`metadata.source`) §5.

## When to use

- Rendering `task-graph.md` (Mermaid `graph TD`, ASCII `└──` tree, count tables) and `task-graph.json`.
- Generating `.claude/skills/**` from `.agents/skills/**` and rendering the `tasks.md` skillist from
  `tasks.deps.yml`, **with a currency check** that fails when the on-disk copy is stale.
- The single F#-source job: turning `capabilities.yml` into a typed `Config.fs` (Stage-3 migration).

## Library verdicts

- **Document/artifact generation (C10, C11) → NO library.** Plain typed rendering — `StringBuilder` for
  Markdown/Mermaid/ASCII, `Utf8JsonWriter` for JSON — so output is **deterministic and byte-comparable**
  to the Stage-0 golden fixtures (Invariant 6). **FSharp.Formatting / Markdig rejected** for *emitting*
  these constrained artifacts.
- **Currency check (C11) → DiffPlex 1.9.0.** Regenerate to a string, diff against disk, fail if stale —
  strictly better than today's unguarded drift. See [[fsharp-io-globbing]].
- **F# source generation (C12) → Fabulous.AST + Fantomas (one-shot, preferred) or Myriad
  (recurring).** The ONLY genuine F#-source job. **Code quotations rejected** (see below).

## Exact rule to reproduce (parity-critical)

Rendered artifacts MUST be reproducible **byte-for-byte** (stable ordering, no clock/env in output)
so the golden diff (Invariant 6) is meaningful. Match the Python's exact spacing, connector glyphs
(`├── ` / `└── `), and key ordering before deleting it.

## API walkthrough + runnable examples

### C10 — Mermaid `graph TD` (StringBuilder)

```fsharp
open System.Text

let renderMermaid (edges: (string * string) list) =
    let sb = StringBuilder()
    sb.AppendLine "graph TD" |> ignore

    for source, target in edges do
        sb.AppendLine(sprintf "  %s --> %s" source target) |> ignore

    sb.ToString()
```

### C10 — ASCII dependency tree (stable connector glyphs)

```fsharp
open System.Text

let renderTree (root: string) (children: string list) =
    let sb = StringBuilder()
    sb.AppendLine root |> ignore
    let last = List.length children - 1

    children
    |> List.iteri (fun i child ->
        let connector = if i = last then "└── " else "├── "
        sb.AppendLine(connector + child) |> ignore)

    sb.ToString()
```

### C10 — count table

```fsharp
open System.Text

let renderCounts (rows: (string * int) list) =
    let sb = StringBuilder()
    sb.AppendLine "| Status | Count |" |> ignore
    sb.AppendLine "|--------|-------|" |> ignore

    for label, count in rows do
        sb.AppendLine(sprintf "| %s | %d |" label count) |> ignore

    sb.ToString()
```

### C11 — generation-currency check (DiffPlex)

Regenerate into memory, diff against the committed copy, fail when they differ. `InlineDiffBuilder`
over a `Differ()` gives a line model whose `Type` flags the drift.

```fsharp
open DiffPlex
open DiffPlex.DiffBuilder
open DiffPlex.DiffBuilder.Model

let private model (current: string) (regenerated: string) =
    InlineDiffBuilder(Differ()).BuildDiffModel(current, regenerated)

/// True when the committed copy no longer matches a fresh regeneration.
let isStale (current: string) (regenerated: string) =
    (model current regenerated).Lines
    |> Seq.exists (fun line -> line.Type <> ChangeType.Unchanged)

/// Readable per-line drift for the failure report.
let staleLines (current: string) (regenerated: string) =
    (model current regenerated).Lines
    |> Seq.filter (fun line -> line.Type <> ChangeType.Unchanged)
    |> Seq.map (fun line -> sprintf "%A | %s" line.Type line.Text)
    |> Seq.toList
```

### C5 link — JSON artifacts (`Utf8JsonWriter`)

`task-graph.json` (schema 1.0) is emitted with `Utf8JsonWriter` for byte-stable layout — the C5
technique from [[fsharp-parsing]]. Keep one renderer so JSON and Markdown stay lock-step with the
golden fixtures.

### C12 — F# source generation (prose; NOT in the adopt-set examples project)

The single genuine F#-*source* job is turning `capabilities.yml` into a typed `Config.fs` during the
YAML→compiled-F# migration (config ADR D6).

- **Fabulous.AST + Fantomas** — a DSL over Fantomas's Oak AST; describe the F# as a node tree, Fantomas
  pretty-prints style-correct source. Best for **one-shot/occasional** generation, **preferred** because
  D6 points at hand-owned compiled values (no permanent build dependency).
- **Myriad** (v0.85) — a **pre-build plugin** wired into the `.fsproj` (`<MyriadFile>`), regenerating
  every build. Choose only if the catalog must **stay as data** and be compiled each build.

Neither package is in the adopt set, so C12 stays prose — record the chosen path in the Stage-5 ADR
before adding the dependency.

## Pitfall: code quotations are the WRONG tool — reject

F# code quotations (`<@ … @>`) are **runtime metaprogramming** producing `Expr` trees evaluated at run
time — they do **not** emit source or build artifacts, and would re-introduce the runtime-evaluation
tax the foundations programme is removing (config ADR D6, no-FCS stance). Do not use them for the
governance port; noted only to prevent the common conflation with source generation.

## Library API + runnable example

The document renderers now ship as the real `CodeGen` module in the **`FS.Skia.UI.SkillSupport`**
package — its `.fsi` surface lives at `src/SkillSupport/CodeGen.fsi`. `FS.Skia.UI.Build` consumes the
same code via ProjectReference, so the governance build renders through exactly these functions:

- `CodeGen.mermaidGraph` — render `graph TD` Mermaid from edge pairs.
- `CodeGen.markdownTable` — render a Markdown count/status table from header + rows.
- `CodeGen.asciiTree` — render the `├── `/`└── ` ASCII dependency tree (stable connector glyphs).

```fsharp
open FS.Skia.UI.SkillSupport

let edges = [ ("T001", "T002"); ("T002", "T003") ]
let mermaid = CodeGen.mermaidGraph edges      // "graph TD\n  T001 --> T002\n  ..."

let table = CodeGen.markdownTable [ "Status"; "Count" ] [ [ "done"; "3" ]; [ "pending"; "1" ] ]

let tree = CodeGen.asciiTree "root" [ "T001"; "T002" ]   // root\n├── T001\n└── T002
```

## Cautions

- **Determinism.** Rendered artifacts reproducible byte-for-byte (no clock/env, stable ordering) so
  golden diffs are meaningful.
- **Do not conflate C10/C11 (text) with C12 (F# source).** Most "generation" in the port is text
  rendering with no library; only `Config.fs` is real source generation.
- **Build-tooling scope only.** No FCS; ships nowhere.

## Consuming stages

Stage 2 (`GenerateAgentSkills` + skillist render + currency check), Stage 4 (`task-graph.md` /
`task-graph.json` render), Stage 3.3/5.5 (the one-shot `Config.fs` generation). See the plan in
`metadata.source`.

## Persistent problems

When a problem outlasts reasonable in-repo attempts, extensive external research is
**mandatory** — consult **official online docs first** (the F#/.NET docs and the driven
library's own documentation/API reference), then community sources (forums, Reddit, Q&A
sites, issue trackers and changelogs). Record the findings and resolving links in the
feature's `specs/<feature>/feedback/` folder and, for durable lessons, in this skill's
**Sources** line. Offline, the mandate degrades to recording "research blocked — <why>"
rather than hard-failing the phase.

## Sources / links

- DiffPlex: <https://github.com/mmanela/diffplex>
- `Utf8JsonWriter`: <https://learn.microsoft.com/dotnet/api/system.text.json.utf8jsonwriter>
- Fabulous.AST: <https://github.com/edgarfgp/Fabulous.AST> · Fantomas: <https://fsprojects.github.io/fantomas/>
- Myriad: <https://github.com/MoiraeSoftware/myriad>
- Capability report: `docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md` §5.

## Related

[[fsharp-parsing]], [[fsharp-graph-algorithms]] (produce the model rendered here),
[[fsharp-io-globbing]] (currency-check diffing), [[fsharp-build-orchestration]] (DiffPlex also backs
the golden-parity gate).

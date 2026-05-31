---
name: fsharp-code-generation
description: Emit governance artifacts (Markdown/Mermaid/JSON) and generate typed F# source; when NOT to use code quotations.
compatibility: F# governance library (build/Governance) under net10.0; build-tooling scope only.
metadata:
  author: fs-skia-ui
  source: docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md
---

# fsharp-code-generation

Cookbook for the two *different* generation jobs in the port. Owns **C10** (document/text
generation — `task-graph.md` Mermaid + ASCII tree + count tables), **C11** (document generation +
currency check — `.claude/skills/**` from `.agents/skills/**`, skillist render), and **C12** (the
one-shot F# *source* generation). Conflating C10/C11 with C12 is the main pitfall this skill
prevents. Verdicts come from the capability report (`metadata.source`) §5.

## When to use

- Rendering `task-graph.md` (Mermaid `graph TD`, ASCII `└──` tree, count tables) and `task-graph.json`.
- Generating `.claude/skills/**` from `.agents/skills/**` and rendering the `tasks.md` skillist from
  `tasks.deps.yml`, **with a currency check** that fails when the on-disk copy is stale.
- The single F#-source job: turning `capabilities.yml` into a typed `Config.fs` (Stage-3 migration).

## Library verdicts

- **Document/artifact generation (C10, C11) → NO library.** Plain typed rendering — `StringBuilder`
  for Markdown/Mermaid/ASCII and `Utf8JsonWriter` for JSON — so output is **deterministic and
  byte-comparable** to the Stage-0 golden fixtures (Invariant 6). Mermaid and the ASCII tree are a
  few dozen lines each. **FSharp.Formatting / Markdig rejected** for *emitting* these constrained
  artifacts.
- **Currency check (C11) → DiffPlex 1.9.0.** Regenerate to a string, diff against disk, fail if
  stale — a currency check strictly better than today's unguarded drift. See [[fsharp-io-globbing]].
- **F# source generation (C12) → Fabulous.AST + Fantomas (one-shot, preferred) or Myriad
  (recurring).** This is the ONLY genuine F#-source job. **Code quotations rejected** (see below).

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

Regenerate the artifact into memory, diff it against what is committed, and fail when they differ.
`InlineDiffBuilder` over a `Differ()` gives a line model whose `Type` flags the drift.

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

`task-graph.json` (schema 1.0) is emitted with `Utf8JsonWriter` for byte-stable layout — the same
technique shown for C5 in [[fsharp-parsing]]. Keep one renderer so the JSON and the Markdown stay in
lock-step with the golden fixtures.

### C12 — F# source generation (prose; NOT in the adopt-set examples project)

The single genuine F#-*source* job is turning `capabilities.yml` into a typed `Config.fs` during the
YAML→compiled-F# migration (config ADR D6).

- **Fabulous.AST + Fantomas** — a DSL over Fantomas's Oak AST; you describe the F# as a node tree and
  Fantomas pretty-prints style-correct source. Best for a **one-shot/occasional** generation, and
  **preferred** because D6 points at hand-owned compiled values (no permanent build dependency).
- **Myriad** (v0.85) — a **pre-build plugin** wired into the `.fsproj` (`<MyriadFile>`); regenerates
  every build. Choose only if the catalog must **stay as data** and be compiled each build.

Neither package is referenced by the examples project (the adopt set is deliberately minimal), so C12
guidance is prose — record the chosen path in the Stage-5 ADR before adding the dependency.

## Pitfall: code quotations are the WRONG tool — reject

F# code quotations (`<@ … @>`) are **runtime metaprogramming** producing `Expr` trees evaluated at
run time. They do **not** emit source or build artifacts, and using them would re-introduce the
runtime-evaluation tax the foundations programme is removing (config ADR D6, no-FCS stance). Do not
use quotations for the governance port; they are noted only to prevent the common conflation with
source generation.

## Cautions

- **Determinism.** Rendered artifacts must be reproducible byte-for-byte (no clock/env, stable
  ordering) so golden diffs are meaningful.
- **Do not conflate C10/C11 (text) with C12 (F# source).** Most of the "generation" in the port is
  text rendering with no library; only `Config.fs` is real source generation.
- **Build-tooling scope only.** No FCS; nothing here ships in a generated product.

## Consuming stages

Stage 2 (`GenerateAgentSkills` + skillist render + currency check), Stage 4 (`task-graph.md` /
`task-graph.json` render), Stage 3.3/5.5 (the one-shot `Config.fs` generation). See the plan
referenced from `metadata.source`.

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

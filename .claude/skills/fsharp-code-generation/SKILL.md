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
`.agents/skills/**`, skillist render), **C12** (one-shot F# *source* generation), and **C13** (the
feature-066 single-source *product-surface* catalog generator — one canonical `catalogFacts` table
spliced into both `catalog.yml` and `Catalog.fs`, drift-guarded by `ControlsCatalogGenerationCheck`).
Verdicts from the capability report (`metadata.source`) §5.

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

### C13 — product-surface catalog generation (feature 066, worked example)

The catalog generator is the **first product-surface generator** and the template every later
single-source generator (design tokens in `069`, catalog expansion in `071+`) copies. It is the same
single-source discipline as C11 (`.claude`←`.agents`) and `validation.contract.yml`←`Routing.fs`,
applied to the typed-control catalog. The whole generator is `FS.Skia.UI.Build.CatalogGen`
(`build/Governance/CatalogGen.fsi`/`.fs`); like C10–C12 it is pure rendering + currency over in-memory
text, with every filesystem read/write kept at the `Engine/Interpret.fs` edge (Principle IV).

**One canonical source, two generated artifacts.** A single typed value —
`catalogFacts : TypedCatalogFact list` (51 rows: the six 065-seeded controls plus the 072 expansion) —
is the one place each catalog fact is declared. Two artifacts are generated from it:

- `catalog.yml` (`catalogYmlRel`) — rendered per row by `renderYamlRow`, spliced by `spliceYaml`.
- `Catalog.fs` (`catalogFsRel`) — rendered per row by `renderFSharpRow`, spliced by `spliceFSharp`.

**Splice only the marked regions — never the whole file.** Each renderer writes its row into that
control's `typed-catalog/<id>` marked region (`// BEGIN GENERATED: typed-catalog/<id>` …
`// END GENERATED: typed-catalog/<id>`). Every byte **outside** a marker — the hand-authored rows that
are not part of `catalogFacts`, the file preamble, unknown marker ids — is left untouched, and the
splice is idempotent on an already-current file. This is what lets a single fact table coexist with
hand-maintained catalog content in the same two files.

**Regenerate via `RegenerateCatalog`, inside `RefreshSurfaceBaselines`.** To add or change a catalog
row, edit `catalogFacts` (and the typed module it names), then run
`./fake.sh build -t RefreshSurfaceBaselines` — which runs `RegenerateCatalog` to re-splice both
artifacts. **Never hand-edit a generated `typed-catalog/<id>` region**: the drift gate will fail it.

**The drift gate: `ControlsCatalogGenerationCheck`.** It re-renders `catalogFacts` in memory and
compares each on-disk region against the fresh render via `currency : catalogYmlText -> catalogFsText
-> CatalogCurrency list` (each region is `Current`/`Stale`/`Missing`); the file set is clean iff
`isCurrent` is true. On drift, `currencyDrift` returns one actionable line per stale/missing region,
naming the divergent control, the file, and `./fake.sh build -t RefreshSurfaceBaselines` as the fix —
so a hand-edited generated region (or a `catalogFacts` change with no regen) fails the gate with a
pointer at the regeneration target, exactly like C11's `isStale`/`staleLines` currency check.

**Cross-check against the typed surface.** Each row's `Module` and `RequiredAttributes` facts are
cross-checked against the `FS.Skia.UI.Controls.Typed` surface, so the catalog cannot claim a typed
control or required attribute the typed front door does not actually expose. (Authoring that typed
surface is [[fs-skia-typed-controls]]'s job; this generator records and drift-guards the catalog facts
about it.)

```fsharp
open FS.Skia.UI.Build

// Single source: one fact per typed control. (abbreviated — real rows carry purpose/events/role)
let catalogFacts : CatalogGen.TypedCatalogFact list =
    [ { Id = "button"; DisplayName = "Button"; Category = "input"; Module = "Button"
        Purpose = "Pointer and keyboard activatable command."
        RequiredAttributes = [ "text" ]; Events = [ "onClick" ]; AccessibilityRole = "Button" }
      // … 50 more rows … ]

// Regeneration re-splices ONLY the typed-catalog/<id> regions of both files.
let regeneratedFs  = CatalogGen.spliceFSharp (System.IO.File.ReadAllText CatalogGen.catalogFsRel)
let regeneratedYml = CatalogGen.spliceYaml   (System.IO.File.ReadAllText CatalogGen.catalogYmlRel)

// Drift gate: clean iff every region matches a fresh render; else currencyDrift names the fix.
let regions = CatalogGen.currency regeneratedYml regeneratedFs
if not (CatalogGen.isCurrent regions) then
    CatalogGen.currencyDrift regions  // e.g. "catalog.yml is stale — its typed-catalog/button region … Regenerate via ./fake.sh build -t RefreshSurfaceBaselines."
    |> List.iter (eprintfn "%s")
```

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
the golden-parity gate), [[fs-skia-typed-controls]] (authors the typed front door that C13's
`catalogFacts` `Module`/required-attribute facts are cross-checked against; typed authoring is the
preferred front door it owns).

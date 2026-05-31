---
name: fsharp-code-generation
description: Emit governance artifacts (Markdown/Mermaid/JSON) and generate typed F# source; when NOT to use code quotations.
compatibility: F# governance library (build/Governance) under net10.0; build-tooling scope only.
metadata:
  author: fs-skia-ui
  source: docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md
---

# fsharp-code-generation

Capability guidance for the two *different* generation jobs in the port. See the capability report
(`metadata.source`) §5. Conflating them is the main pitfall this skill prevents.

## Two distinct capabilities

### 1. Document / artifact generation (the common case) — NO library

`task-graph.md` (Mermaid `graph TD`, ASCII `└──` tree, count tables), `task-graph.json`, and the
generated `.claude/skills/**` are **text/document** outputs, not F# source. Build them with plain
typed rendering — string builders and `Utf8JsonWriter` — so output is **deterministic and
byte-comparable** to the Stage-0 golden fixtures (Invariant 6). Mermaid and ASCII tree are a few
dozen lines each. For the `.claude`-from-`.agents` generation and the skillist render, the pattern
is: generate to a temp, diff with **DiffPlex**, fail if stale (a *currency* check, strictly better
than the current unguarded drift). See [[fsharp-io-globbing]].

### 2. F# source generation (one task only) — Fabulous.AST/Fantomas or Myriad

The single genuine F#-source job is turning `capabilities.yml` into a typed `Config.fs` during the
YAML→compiled-F# migration (plan 3.3/5.5, config ADR D6).

- **Fabulous.AST + Fantomas** — DSL over Fantomas's Oak AST; describe the F# as a node tree,
  Fantomas pretty-prints style-correct source. Best for a **one-shot/occasional** generation.
  **Preferred** given D6 points at hand-owned compiled values (no permanent build dependency).
- **Myriad** (v0.85) — **pre-build plugin** wired into the `.fsproj` (`<MyriadFile>`); regenerates
  every build. Choose only if the catalog must **stay as data** and be compiled each build.

Record the chosen path in the Stage-5 ADR.

## Pitfall: code quotations are the WRONG tool here — reject

F# code quotations (`<@ … @>`) are **runtime metaprogramming** producing `Expr` trees evaluated at
run time. They do **not** emit source or build artifacts, and using them would re-introduce the
runtime-evaluation tax the foundations programme is removing (config ADR D6, no-FCS stance). Do not
use quotations for the governance port; they are noted only to prevent the common conflation with
source generation.

## Cautions

- Determinism: rendered artifacts must be reproducible byte-for-byte (no clock/env in output, stable
  ordering) so golden diffs are meaningful.

Related: [[fsharp-parsing]], [[fsharp-graph-algorithms]] (produce the model rendered here),
[[fsharp-io-globbing]] (currency-check diffing).

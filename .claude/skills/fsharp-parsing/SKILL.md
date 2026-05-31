---
name: fsharp-parsing
description: Parse governance inputs (YAML, tasks.md line grammar, audit-status regions, JSON) in compiled F#.
compatibility: F# governance library (build/Governance) under net10.0; build-tooling scope only.
metadata:
  author: fs-skia-ui
  source: docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md
---

# fsharp-parsing

Capability guidance for porting the Bash/Python parsers into typed, compiled F# in the governance
library. Replaces the two hand-rolled YAML parsers (Python + `build.fsx`) and the regex scanners.
See the capability report (`metadata.source`) §3 for the full rationale and verdicts.

## When to use

- Reading `tasks.deps.yml`, `capabilities.yml`, `validation.contract.yml`, `audit-patterns.yml`.
- Parsing `tasks.md` task lines, annotations, phase/checkpoint headers, Synthetic-Evidence
  Inventory tables.
- Scanning ` ```audit-status ` fenced regions.
- Reading `.specify/feature.json` and emitting `task-graph.json`.

## Library verdicts

- **YAML → YamlDotNet** (already in `Directory.Packages.props` 17.1.0). Deserialize into immutable
  F# records/DUs behind the API; retire both bespoke parsers. **Legivel rejected** (YamlDotNet
  present; minimal-YAML inputs don't need 1.2 conformance).
- **JSON → System.Text.Json + FSharp.SystemTextJson** (DU round-trip). **Thoth/Newtonsoft rejected.**
- **Line/region grammar → regex port FIRST, then XParsec.** Faithful `System.Text.RegularExpressions`
  port clears the byte-parity gate against the Stage-0 golden fixtures; migrate to **XParsec**
  (pure F#, MIT, v1.0.0, Fable-capable) once parity is signed off. **Full Markdown AST
  (FSharp.Formatting/Markdig) rejected** — inputs are a constrained line grammar.

## Exact grammars to reproduce (parity-critical)

Task line (`compute-task-graph.py`):
`^\s*-\s*\[(?P<box>[ X\-FS*])\]\s+(?P<id>T\d{3,4})\b(?P<rest>.*)$`
Boxes: `[ ]` pending · `[X]` done · `[S]` synthetic · `[F]` failed · `[-]` skipped · `[*]` computed-only.
Annotations (order varies): `[P]` parallel · `[US\d+]` user story · `[T[12]]` tier · `[SEH]` +
`synthetic-error-handling-approved` label · `[skillist: [...]]` (empty brackets = no skills).

`tasks.deps.yml` accepts **two shapes** — object `{deps, skillist}` and legacy bare-list. Accept
both; fixture-test both before deleting the Python.

`audit-status` region semantics (`audit-status-scan.py`): first region wins · detect unclosed ·
`#` comments and blank lines ignored · `key=value` · **duplicate key = hard error** (never
last-wins) · key normalize `.lower().strip()`.

## Cautions

- **Parity over elegance.** Match the Python's exact tie-breaks/ordering; gate on the golden
  fixtures (Invariant 6) before deleting any Python.
- **Determinism.** Parsers must be pure over their input — no env/clock reads at parse time.

Related: [[fsharp-graph-algorithms]] (consumes parsed model), [[fsharp-code-generation]] (emits
JSON/Markdown), [[fsharp-io-globbing]] (discovers the files to parse).

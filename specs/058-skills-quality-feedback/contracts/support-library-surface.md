# Contract: `FS.Skia.UI.SkillSupport` Public Surface

Per-family `.fsi` surface for the new shipped library. Bodies are **moved** from
`build/Governance` behind these signatures (full extraction); shapes are stable so
existing governance tests re-point without rewrite. Sketches below are design
intent — the curated `.fsi` files plus
`readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt` are authoritative.

## Module `Graph` (← `Evidence/Graph.fs`, fsharp-graph-algorithms)

```fsharp
type NodeId = string
type Edge = NodeId * NodeId

/// Kahn topological sort with deterministic tie-break (ascending NodeId).
val topoSort: nodes: NodeId list -> edges: Edge list -> Result<NodeId list, NodeId list> // Error = remaining cyclic nodes
/// 3-colour DFS cycle detection; returns one cycle witness if present.
val detectCycle: nodes: NodeId list -> edges: Edge list -> NodeId list option
```

Governance keeps synthetic-propagation (`[S]`/`[S*]`/`[SEH]`) as a consumer.

## Module `Parsing` (← `TaskParser`/`DepsParser`/`StatusRegion` helpers, fsharp-parsing)

```fsharp
/// Typed YAML read (YamlDotNet).
val readYaml<'T> : text: string -> Result<'T, string>
/// JSON read (System.Text.Json + FSharp.SystemTextJson).
val readJson<'T> : text: string -> Result<'T, string>
/// Regex line-grammar helper: apply a compiled pattern to each line.
val matchLines: pattern: string -> lines: string seq -> (int * System.Text.RegularExpressions.Match) seq
```

Governance keeps the exact `tasks.md`/`tasks.deps.yml` grammars as consumers.

## Module `Globbing` (← `Routing` glob + currency, fsharp-io-globbing)

```fsharp
/// fnmatch-style: ** crosses '/', * and ? stay within a segment; '/'-normalized.
val isMatch: glob: string -> path: string -> bool
val discover: root: string -> globs: string list -> string list
/// DiffPlex-based generation-currency diff.
val currencyDiff: expected: string -> actual: string -> string list   // empty = current
```

## Module `CodeGen` (← `Render`/`ContractView`, fsharp-code-generation)

```fsharp
/// Deterministic builders (StringBuilder; no code quotations).
val mermaidGraph: nodes: string list -> edges: (string * string) list -> string
val markdownTable: headers: string list -> rows: string list list -> string
val asciiTree: roots: string list -> children: (string -> string list) -> string
```

## Module `ShellProcess` (← `Front/BuildProcess.fs`, fsharp-shell-process)

```fsharp
type ProcResult = { ExitCode: int; StdOut: string; StdErr: string }
val run: exe: string -> args: string list -> workingDir: string -> ProcResult
val git: args: string list -> workingDir: string -> ProcResult
```

## Governance registration (FR-010)

- `PerPackageSurface.packagesInScope` += `"FS.Skia.UI.SkillSupport"`
- `PerPackageSurface.packageSourceDir` += `"FS.Skia.UI.SkillSupport" -> "SkillSupport"`
- baseline: `readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt`
- template pin: `template/base/Directory.Packages.props`
- `FS.Skia.UI.Build` adds `ProjectReference` to `src/SkillSupport`

## Invariants

- Visibility lives in `.fsi` only (Principle II); no access modifiers in `.fs`.
- Additive: no existing public `.fsi` signature changes.
- Moved bodies preserve behavior; re-pointed governance tests stay green (parity).

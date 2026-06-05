# FS.Skia.UI.SkillSupport

Shipped backing library for the FS.Skia.UI fsharp-* authoring skills: generic DAG graph algorithms, governance-input parsing, fnmatch-style globbing + currency diff, deterministic Markdown/Mermaid/ASCII code generation, and a captured external-process runner.

`FS.Skia.UI.SkillSupport` is one of the **FS.Skia.UI** distribution packages — an F# / Elmish UI and 2D
scene-graph framework for .NET 10 desktop, rendered through Vulkan + SkiaSharp.

## Install

```bash
dotnet add package FS.Skia.UI.SkillSupport
```

Or scaffold a full governed project that wires the FS.Skia.UI packages together:

```bash
dotnet new install FS.Skia.UI.Template
dotnet new fs-skia-ui -o MyApp
```

## Usage

```fsharp
open FS.Skia.UI.SkillSupport

// Discover the source files an authoring skill cares about.
let sources = Globbing.discover "src" [ "**/*.fsi"; "**/*.fs" ]
printfn "matched %d files" (List.length sources)

// Confirm a single path against an fnmatch-style glob.
let isFsi = Globbing.isMatch "**/*.fsi" "src/SkillSupport/Graph.fsi"

// Order a small task DAG with a deterministic ascending-id tie-break.
let nodes = [ "spec"; "plan"; "tasks" ]
let edges = [ ("spec", "plan"); ("plan", "tasks") ]

match Graph.topoSort nodes edges with
| Ok order -> printfn "run order: %s" (String.concat " -> " order)
| Error remaining -> printfn "cycle blocks: %A" remaining

// Render the same DAG as a Mermaid block for a generated doc.
let diagram = CodeGen.mermaidGraph nodes edges
printfn "%s" diagram
```

## API at a glance

- **`Graph`** — DAG core: `topoSort` (Kahn sort with deterministic ascending-id tie-break, returning `Ok order` or `Error remaining`) and `detectCycle` (3-colour DFS returning one cycle witness).
- **`Parsing`** — typed governance-input reads: `readYaml<'T>`, `readJson<'T>` (both `Result`-returning), and `matchLines` for applying a compiled regex line-grammar.
- **`Globbing`** — fnmatch-style `isMatch`, recursive `discover` of files matching globs (sorted relative paths), and `currencyDiff` for DiffPlex-based generation-currency checks.
- **`CodeGen`** — deterministic document builders: `mermaidGraph`, `markdownTable`, and `asciiTree`.
- **`ShellProcess`** — captured external-process runner: `run` returns a `ProcResult` (`ExitCode`/`StdOut`/`StdErr`); `git` is a thin convenience wrapper.
- **`Hud`** — `reserveHudBand` partitions an axis into a fixed HUD `Band` and a clamped gameplay remainder (`HudLayout`).
- **`Wrap`** — `wrapDeltaX`, the shortest wrap-aware signed delta on a toroidal axis.
- **`Random`** — deterministic seeded RNG: `seedRng`, `nextRng`, and `nextBelow`, threading an opaque `RngState` through a pure `update`.

## Versioning

All `FS.Skia.UI.*` libraries share one version and move together. In a generated project a
single `<FsSkiaUiVersion>` in `Directory.Packages.props` pins every package — upgrading is one
edit; see `docs/UPGRADING.md`. Pre-release versions use a `-preview.N` suffix.

## Links

- Repository & issues: https://github.com/FS-Skia-UI/FS-Skia-UI
- License: MIT

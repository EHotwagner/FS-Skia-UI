---
name: fsharp-io-globbing
description: File discovery, fnmatch-style glob matching, and generation-currency diffing in compiled F#.
compatibility: F# governance library (build/Governance) under net10.0; build-tooling scope only.
metadata:
  author: fs-skia-ui
  source: docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md
---

# fsharp-io-globbing

Cookbook for file discovery and glob matching ported from the Bash/Python scripts. Owns **C13** (file
discovery / skill globbing) and **C14** (whitelist glob matching with `fnmatch` semantics). Verdicts
come from the capability report (`metadata.source`) §6.

## When to use

- Discovering skills: `.agents/skills/*/SKILL.md`, `src/*/skill/SKILL.md`,
  `template/fragments/*/skill/SKILL.md`.
- Matching files against `audit-patterns.yml` whitelist globs (`docs/**`, `**/*.md`).
- Reading/writing readiness artifacts; generation-currency checks (the diff itself lives in
  [[fsharp-code-generation]] via DiffPlex).

## Library verdicts

- **Skill discovery (C13) → `Fake.IO.Globbing` 6.1.4** (FAKE family, already in-tree via the build
  front-end), or `System.IO.Directory.EnumerateFiles`. **Prefer Fake.IO.Globbing** for one IO idiom
  across the build.
- **Whitelist glob matching (C14) → `Microsoft.Extensions.FileSystemGlobbing.Matcher` 10.0.8**
  (first-party). Native `*` / `**` include/exclude semantics.
- **Generation-currency diff → DiffPlex** (regenerate to temp/string, diff, fail if stale). See
  [[fsharp-code-generation]].

## Parity caution (the live risk)

.NET glob semantics differ from Python `fnmatch` at the edges — a single `*` crossing directory
separators, a leading `**/`, trailing-slash behaviour. **Golden-test every `audit-patterns.yml`
whitelist entry against the Python before cutover.** One of the two most likely silent divergences in
the port (the other is the two `tasks.deps.yml` YAML shapes, see [[fsharp-parsing]]).

## API walkthrough + runnable examples

### C13 — skill discovery (`Fake.IO.Globbing`)

`!!` builds an include pattern; `++` adds includes; `--` excludes. The result is an
`IGlobbingPattern` enumerating as a `seq<string>`; sort before rendering for reproducible golden diffs.

```fsharp
open Fake.IO.Globbing.Operators

/// Discover the three skill source locations the Python scanned, as a stable
/// sorted list (deterministic for golden diffs).
let discoverSkills () =
    !! ".agents/skills/*/SKILL.md"
    ++ "src/*/skill/SKILL.md"
    ++ "template/fragments/*/skill/SKILL.md"
    |> Seq.sort
    |> Seq.toList
```

### C14 — whitelist matching (`Microsoft.Extensions.FileSystemGlobbing`)

`Matcher().AddInclude(glob)` accumulates `*`/`**` include patterns; `.Match(files)` returns a
`PatternMatchingResult` whose `HasMatches` and `Files` (each with a `.Path`) drive the whitelist
decision. `AddExclude` for negative patterns.

```fsharp
open Microsoft.Extensions.FileSystemGlobbing

/// Apply the audit-patterns.yml whitelist globs to a candidate file set.
let whitelisted (globs: string list) (files: string list) =
    let matcher = Matcher()

    for glob in globs do
        matcher.AddInclude glob |> ignore

    let result = matcher.Match(files)
    result.HasMatches, [ for m in result.Files -> m.Path ]
```

### C13 fallback — BCL enumeration when one idiom is overkill

For a single directory the BCL is enough; still sort for determinism.

```fsharp
open System.IO

let enumerateMarkdown (root: string) =
    Directory.EnumerateFiles(root, "*.md", SearchOption.AllDirectories)
    |> Seq.sort
    |> Seq.toList
```

## Cautions

- **.NET glob vs Python `fnmatch` drift** — the live risk (see Parity caution above): golden-test
  every whitelist entry against the Python before cutover.
- **Determinism.** Enumerate with a **stable sort** before rendering, for reproducible golden diffs.
- **Keep true OS-glue in Bash** — `fake.sh`/`fake.cmd` launchers and container entrypoints; no payoff
  to F#-ifying a three-line launcher.
- **Build-tooling scope only.** Lives under `build/Governance`; ships nowhere; no FCS.

## Consuming stages

Stage 4 (skill discovery + audit-patterns whitelist matching), Stage 2 (generation-currency checks
over the discovered files). See the plan in `metadata.source`.

## Sources / links

- Fake.IO.Globbing: <https://fake.build/reference/fake-io-globbing-operators.html>
- Microsoft.Extensions.FileSystemGlobbing: <https://learn.microsoft.com/dotnet/api/microsoft.extensions.filesystemglobbing.matcher>
- Capability report: `docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md` §6.

## Related

[[fsharp-parsing]] (parses the discovered files), [[fsharp-shell-process]] (diff file matching),
[[fsharp-code-generation]] (currency-check diffing via DiffPlex).

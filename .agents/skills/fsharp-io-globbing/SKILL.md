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

## Library API + runnable example

Discovery and glob matching now ship as the real `Globbing` module in the **`FS.Skia.UI.SkillSupport`**
package — its `.fsi` surface lives at `src/SkillSupport/Globbing.fsi`. `FS.Skia.UI.Build` consumes the
same code via ProjectReference, so the governance build matches through exactly these functions:

- `Globbing.isMatch: string -> string -> bool` — `isMatch glob path`; fnmatch-parity `*`/`**` matching.
- `Globbing.discover: string -> string list -> string list` — enumerate files under a root matching the
  given include globs, returned as a stable sorted list (deterministic for golden diffs).
- `Globbing.currencyDiff: string -> string -> string list` — per-line drift between the on-disk copy and
  a fresh regeneration; empty list means current.

```fsharp
open FS.Skia.UI.SkillSupport

// Whitelist-style glob check (C14).
let ok = Globbing.isMatch "**/*.md" "docs/reports/report.md"   // true

// Discover the skill sources under the repo root (C13), stably sorted.
let skills = Globbing.discover "." [ ".agents/skills/*/SKILL.md"; "src/*/skill/SKILL.md" ]

// Generation-currency drift: empty when the committed copy is up to date.
let drift = Globbing.currencyDiff onDiskText regeneratedText
if not (List.isEmpty drift) then failwithf "stale: %A" drift
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

## Persistent problems

When a problem outlasts reasonable in-repo attempts, extensive external research is
**mandatory** — consult **official online docs first** (the F#/.NET docs and the driven
library's own documentation/API reference), then community sources (forums, Reddit, Q&A
sites, issue trackers and changelogs). Record the findings and resolving links in the
feature's `specs/<feature>/feedback/` folder and, for durable lessons, in this skill's
**Sources** line. Offline, the mandate degrades to recording "research blocked — <why>"
rather than hard-failing the phase.

## Sources / links

- Fake.IO.Globbing: <https://fake.build/reference/fake-io-globbing-operators.html>
- Microsoft.Extensions.FileSystemGlobbing: <https://learn.microsoft.com/dotnet/api/microsoft.extensions.filesystemglobbing.matcher>
- Capability report: `docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md` §6.

## Related

[[fsharp-parsing]] (parses the discovered files), [[fsharp-shell-process]] (diff file matching),
[[fsharp-code-generation]] (currency-check diffing via DiffPlex).

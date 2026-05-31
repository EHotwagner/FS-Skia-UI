---
name: fsharp-io-globbing
description: File discovery, fnmatch-style glob matching, and generation-currency diffing in compiled F#.
compatibility: F# governance library (build/Governance) under net10.0; build-tooling scope only.
metadata:
  author: fs-skia-ui
  source: docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md
---

# fsharp-io-globbing

Capability guidance for file discovery and glob matching ported from the Bash/Python scripts. See
the capability report (`metadata.source`) §6.

## When to use

- Discovering skills: `.agents/skills/*/SKILL.md`, `src/*/skill/SKILL.md`,
  `template/fragments/*/skill/SKILL.md`.
- Matching files against `audit-patterns.yml` whitelist globs (`docs/**`, `**/*.md`).
- Reading/writing readiness artifacts; generation-currency checks.

## Verdicts

- **Skill discovery → `Fake.IO.Globbing`** (FAKE family already in-tree via the front-end) or
  `System.IO.Directory.EnumerateFiles`. Prefer Fake.IO.Globbing to keep one IO idiom across the
  build.
- **Whitelist glob matching → `Microsoft.Extensions.FileSystemGlobbing.Matcher`** (first-party).
  Native `*` / `**` include/exclude semantics. Pattern: `Matcher().AddInclude(glob).Match(root, files)`.
- **Generation-currency diff → DiffPlex** (regenerate to temp, diff, fail if stale). See
  [[fsharp-code-generation]].

## Parity caution (the live risk)

.NET glob semantics differ from Python `fnmatch` at the edges — a single `*` crossing directory
separators, leading `**/`, and trailing-slash behaviour. **Golden-test every `audit-patterns.yml`
whitelist entry against the Python before cutover.** This is one of the two most likely silent
divergences in the whole port (the other is the two `tasks.deps.yml` YAML shapes, see
[[fsharp-parsing]]).

## Notes

- Keep true OS-glue (`fake.sh`/`fake.cmd` launchers, container entrypoints) in Bash — no payoff to
  F#-ifying a three-line launcher.
- Determinism: enumerate with a **stable sort** before rendering, so discovery order is reproducible
  for golden diffs.

Related: [[fsharp-parsing]], [[fsharp-shell-process]], [[fsharp-code-generation]].

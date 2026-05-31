---
name: fsharp-build-orchestration
description: Drive FAKE targets from the compiled front-end; golden-diff parity with DiffPlex; property/unit tests with Expecto + FsCheck.
compatibility: F# governance library (build/Governance) + build/Build.fsproj under net10.0; build-tooling scope only.
metadata:
  author: fs-skia-ui
  source: docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md
---

# fsharp-build-orchestration

Capability guidance for the compiled build front-end, the parity gates, and the test harness. See
the capability report (`metadata.source`) §8.

## Orchestration / CLI

- **`Fake.Core.Target`** is adopted (6.1.4, present). The D2 spike proved it drives targets from
  the compiled `build/Build.fsproj` exe with **no FSX runner** and **no `FSharp.Compiler.*`**
  transitive (FR-012). Dispatch via `Target.runOrDefaultWithArguments`; delegate each target body
  to a library function (no inlined logic in `Program.fs`).
- Tiers/routing become **compiled F#** (`Routing.fs`, plan 5.5): `Diff -> Tier` predicates over a
  typed `Target` union, so a mistyped target name fails to **compile**. No YAML, no FCS.
- **Argu** (typed CLI) and **Spectre.Console** (tables/colour) are **optional** — adopt only if the
  `Route` CLI grows.

## Golden-output parity (the merge gate) — DiffPlex

Every ported parser/algorithm/renderer must produce **byte-identical** output to the Stage-0 golden
fixtures before the Python/Bash is deleted (Invariant 6). **DiffPlex** (v1.9.0) renders readable
unified/side-by-side diffs for the parity gate and for the Stage-2 generation-currency check.
**Adopt DiffPlex.**

## Testing — Expecto + FsCheck

- **Expecto 10.2.2** is in-tree; `tests/Governance.Tests` references it. Re-point moved-logic tests
  at the real library functions (assert typed errors, not strings).
- **FsCheck v3** via `Expecto.FsCheck` for the graph property tests — see
  [[fsharp-graph-algorithms]]. **Adopt FsCheck 3.**

## Cautions

- **FAKE concurrency:** never run FAKE-backed targets concurrently (shared `.fake` state); use the
  deterministic serialized order from `CLAUDE.md`/`AGENTS.md`.
- All new packages are **build-tooling scope** — referenced by `build/Governance/**` or
  `tests/Governance.Tests`, **never shipped** in a generated product; versions go in
  `Directory.Packages.props` (Central Package Management).
- Record warm/cold build wall-clock vs the Stage-0 baseline; the compiled library should beat the
  207 KB script recompilation.

Related: [[fsharp-parsing]], [[fsharp-graph-algorithms]], [[fsharp-code-generation]],
[[fsharp-io-globbing]], [[fsharp-shell-process]].

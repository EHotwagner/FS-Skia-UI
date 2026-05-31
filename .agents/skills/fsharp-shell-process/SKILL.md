---
name: fsharp-shell-process
description: Wrap git and residual external processes from F#; in-process-first eliminates most run-audit.sh shelling.
compatibility: F# governance library (build/Governance) under net10.0; build-tooling scope only.
metadata:
  author: fs-skia-ui
  source: docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md
---

# fsharp-shell-process

Capability guidance for replacing the Bash orchestration (`run-audit.sh`, parts of `common.sh`)
with F#. See the capability report (`metadata.source`) §7.

## Principle: in-process-first

The strategic win is **not** to re-wrap Bash in F# — it is to make most shelling *disappear*. Once
the graph/audit/diff-scan live in the governance library, the build calls F# functions directly
instead of `build.fsx → run-audit.sh → python → JSON → re-parse`. `run-audit.sh` shrinks to a shim
or is deleted (plan 4.5–4.6). Only genuine external work remains.

## Verdicts for what remains

- **Git (base-ref resolve, `merge-base`, `diff --unified=0`) → `Fake.Tools.Git`** (FAKE family),
  or `Fake.Core.Process` for raw `git`. **Adopt Fake.Tools.Git.**
- **Residual processes (`dotnet`, smoke runners) → `Fake.Core.Process`** — already transitively
  present via `Fake.Core.Target`; keeps the dependency family consistent. **Adopt.**
- **CliWrap / Fli rejected** unless rich piping/async is needed — they add a dependency where
  Fake.Core.Process suffices.
- **Keep OS-glue in Bash** — `fake.sh`/`fake.cmd` launchers and container entrypoints stay shell;
  F#-ifying a three-line launcher has no payoff.

## Cautions

- **FAKE concurrency:** FAKE-backed commands share `.fake` state and are **not** safe to run
  concurrently — run them sequentially in the deterministic order from `CLAUDE.md`/`AGENTS.md`.
- **Determinism:** capture stdout/exit codes explicitly; preserve the Python/Bash exit-code contract
  (e.g. `0` PASS, `2` needs-evidence, `3` graph-compute-failed) when a gate is rewired, so callers
  and CI keep working.
- Resolve base ref the same way `run-audit.sh` does (main → master → `HEAD~1`) for diff-scan parity.

Related: [[fsharp-io-globbing]] (diff file matching), [[fsharp-graph-algorithms]] (the in-process
gate that replaces the shell orchestration), [[fsharp-build-orchestration]].

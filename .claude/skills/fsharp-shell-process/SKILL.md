---
name: fsharp-shell-process
description: Wrap git and residual external processes from F#; in-process-first eliminates most run-audit.sh shelling.
compatibility: F# governance library (build/Governance) under net10.0; build-tooling scope only.
metadata:
  author: fs-skia-ui
  source: docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md
---

# fsharp-shell-process

Cookbook for replacing the Bash orchestration (`run-audit.sh`, parts of `common.sh`) with F#. Owns
**C15** (git wrapping: base-ref resolve, `merge-base`, `diff --unified=0`) and **C17** (residual
process orchestration + the exit-code contract). Verdicts come from the capability report
(`metadata.source`) §7.

## Principle: in-process-first

The strategic win is **not** to re-wrap Bash in F# — it is to make most shelling *disappear*. Once
the graph/audit/diff-scan live in the governance library, the build calls F# functions directly
instead of `build.fsx → run-audit.sh → python → JSON → re-parse`. `run-audit.sh` shrinks to a shim
or is deleted (plan 4.5–4.6). Only genuine external work (git, `dotnet`) remains.

## Library verdicts

- **Git (C15) → `Fake.Tools.Git` 6.1.4** (FAKE family), or `Fake.Core.Process` for raw `git`.
  **Adopt Fake.Tools.Git.**
- **Residual processes (`dotnet`, smoke runners) (C17) → `Fake.Core.Process`** — already transitively
  present via `Fake.Core.Target`; keeps the dependency family consistent. **Adopt.**
- **CliWrap / Fli rejected** unless rich piping/async is needed — they add a dependency where
  `Fake.Core.Process` suffices.
- **Keep OS-glue in Bash** — `fake.sh`/`fake.cmd` launchers and container entrypoints stay shell;
  F#-ifying a three-line launcher has no payoff.

## API walkthrough + runnable examples

### C15 — git wrapping (`Fake.Tools.Git`)

`directRunGitCommand repoDir args` runs `git` and returns a success `bool` (use it to *probe* a ref);
`runSimpleGitCommand repoDir args` runs `git` and returns its stdout as a string (use it to *capture*
`merge-base` / `diff`). Resolve the base ref the same way `run-audit.sh` does — `main` → `master` →
`HEAD~1` — for diff-scan parity.

```fsharp
open Fake.Tools

/// C15 — probe candidate base refs in run-audit.sh order; first that verifies wins.
let resolveBaseRef (repoDir: string) =
    let exists (ref: string) =
        Fake.Tools.Git.CommandHelper.directRunGitCommand repoDir (sprintf "rev-parse --verify %s" ref)

    [ "origin/main"; "origin/master"; "HEAD~1" ] |> List.tryFind exists

/// merge-base and a zero-context unified diff, captured as strings.
let mergeBase (repoDir: string) (left: string) (right: string) =
    Fake.Tools.Git.CommandHelper.runSimpleGitCommand repoDir (sprintf "merge-base %s %s" left right)

let unifiedDiff (repoDir: string) (baseRef: string) =
    Fake.Tools.Git.CommandHelper.runSimpleGitCommand repoDir (sprintf "diff --unified=0 %s" baseRef)
```

### C17 — residual process with explicit exit-code capture (`Fake.Core.Process`)

`CreateProcess.fromRawCommandLine` + `Proc.run` gives an explicit `ExitCode` and captured output.
Preserve the Bash/Python exit-code contract when a gate is rewired (e.g. `0` PASS, `2`
needs-evidence, `3` graph-compute-failed) so callers and CI keep working.

```fsharp
open Fake.Core

/// C17 — run dotnet (or any residual tool), capturing exit code + stdout.
let runDotnet (workingDir: string) (arguments: string) =
    let result =
        CreateProcess.fromRawCommandLine "dotnet" arguments
        |> CreateProcess.withWorkingDirectory workingDir
        |> CreateProcess.redirectOutput
        |> Proc.run

    result.ExitCode, result.Result.Output
```

## Cautions

- **FAKE concurrency:** FAKE-backed commands share `.fake` state and are **not** safe to run
  concurrently — run them sequentially in the deterministic order from `CLAUDE.md`/`AGENTS.md`.
- **Exit-code contract.** Capture stdout/exit codes explicitly and preserve the Python/Bash exit-code
  contract when rewiring a gate, so callers and CI keep working.
- **Base-ref parity.** Resolve the base ref the way `run-audit.sh` does (`main` → `master` →
  `HEAD~1`) for diff-scan parity. Pair with the C16 diff scanner in [[fsharp-parsing]].
- **Build-tooling scope only.** Lives under `build/Governance`; ships nowhere; no FCS.

## Consuming stages

Stage 4.5–4.6 (shrink/delete `run-audit.sh`; in-process git + diff), Stage 5 (compiled front-end
running residual `dotnet`/smoke processes). See the plan referenced from `metadata.source`.

## Sources / links

- Fake.Tools.Git: <https://fake.build/reference/fake-tools-git.html>
- Fake.Core.Process: <https://fake.build/reference/fake-core-process.html>
- Capability report: `docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md` §7.

## Related

[[fsharp-io-globbing]] (diff file matching), [[fsharp-graph-algorithms]] (the in-process gate that
replaces the shell orchestration), [[fsharp-parsing]] (the C16 diff scanner),
[[fsharp-build-orchestration]] (the compiled front-end that calls these).

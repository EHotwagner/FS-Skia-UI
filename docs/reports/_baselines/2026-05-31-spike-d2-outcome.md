# Spike Outcome — D2 (dedicated compiled FAKE build front-end) — 2026-05-31

## Verdict

**D2 confirmed.**

A dedicated, compiled F# `Exe` consumes the `Fake.Core.Target` API as an
ordinary NuGet **library** (no FSX script runner, no FSharp Compiler Services),
registers a target whose body lives in a referenced **compiled governance
library**, and runs it to success via `dotnet run`. All four contract rows hold
(`contracts/spike-target.contract.md`); the FR-007/SC-004 decision rule yields
`"D2 confirmed"`.

## Pinned context

| Field | Value |
|---|---|
| `git_commit` | `34faf1ed61ec0ec2a8a2a81168517cb5ccf499d1` |
| `git_commit` (short) | `34faf1e` |
| `captured_at` | `2026-05-31T13:16:44Z` |
| dotnet SDK | `10.0.300` |
| FAKE version | `6.1.4` (matches `.config/dotnet-tools.json` + `build.fsx.lock`) |

## What was built

- `build/Governance/FS.Skia.UI.Build.fsproj` — governance library skeleton with
  a curated `Spike.fsi` (`val run : unit -> string`) and `Spike.fs`.
- `build/Build.fsproj` (Exe) — dedicated front-end whose `Program.fs` registers
  one `SpikeHello` target via `Fake.Core.Target`, the body delegating **only** to
  `FS.Skia.UI.Build.Spike.run` (no inlined logic), dispatched via
  `Target.runOrDefaultWithArguments`.

Added to `FS-Skia-UI.sln` additively (32 → 36 `Project(` entries).

## Contract rows (contracts/spike-target.contract.md)

| Row | Check | Result |
|---|---|---|
| 1 | Both projects compile clean under `net10.0`/`TreatWarningsAsErrors` (`-warnaserror`); no `PackageVersion` outside `Directory.Packages.props` | **PASS** — both `0 Warning(s), 0 Error(s)` |
| 2 | `SpikeHello` runs via `dotnet run` and prints the value returned by `Spike.run` (body ran from the library, not inlined) | **PASS** — see run output below |
| 3 | Outcome recorded as exactly one of confirmed/fallback | **PASS** — `D2 confirmed` |
| 4 | No `FSharp.Compiler.*` in the restored transitive package graph | **PASS** — `dotnet list build/Build.fsproj package --include-transitive` shows none |

## Reproduction commands and output

```bash
dotnet build build/Governance/FS.Skia.UI.Build.fsproj -warnaserror   # 0/0
dotnet build build/Build.fsproj -warnaserror                         # 0/0
dotnet list build/Build.fsproj package --include-transitive | grep -i FSharp.Compiler   # (no match)
dotnet run --project build/Build.fsproj -- SpikeHello
```

`dotnet run --project build/Build.fsproj -- SpikeHello` (key lines):

```
Starting target 'SpikeHello'
FS.Skia.UI.Build.Spike.run: D2 spike target executed from the governance library.
Finished (Success) 'SpikeHello' in 00:00:00.003
...
Status:      Ok
```

The printed success line is the exact value returned by
`FS.Skia.UI.Build.Spike.run` (defined in `build/Governance/Spike.fs`), proving
the target body executed **from the library**, not inlined in the front-end.

### FCS-absence verification (FR-012)

`build/spike-verify.sh` runs
`dotnet list build/Build.fsproj package --include-transitive` and greps for
`FSharp.Compiler` — **no match**. The modular `Fake.Core.Target` package does
not transitively pull FSharp Compiler Services; the per-invocation FSX compile
tax is absent, exactly as D2 requires.

## Verification scaffold

`build/spike-verify.sh` is the committed verification scaffold (the failing-first
contract encoder): it fails if either project is missing or fails to build, if
`FSharp.Compiler.*` appears, or if the `SpikeHello` output does not contain the
library's success line. Latest run: **`SPIKE-VERIFY PASS: D2 confirmed`**
(exit 0).

## Stage 5 path forward

D2 is confirmed; Stage 5 may proceed to migrate target bodies into the compiled
governance library behind the `dotnet run` front-end and retire the
FSX-compiled `build.fsx` once population is complete. The documented
thin-`build.fsx` `#r`-the-DLL shim is **not needed** (it was the FR-007 fallback
reserved for a concrete FAKE-as-library blocker, which did not materialise).

This outcome is referenced by ADR 0002 (build front-end form).

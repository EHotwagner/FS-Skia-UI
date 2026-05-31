# ADR 0002 — Build front-end form (D2)

- **Status**: Accepted — spike outcome **D2 confirmed**.
- **Date**: 2026-05-31
- **Decision source**: foundations plan + `specs/039-foundations-baseline-spike/`
  (research R1/R2/R3). Spike outcome:
  `docs/reports/_baselines/2026-05-31-spike-d2-outcome.md`.

## Context

`build.fsx` is run via the FSX **script runner** (`dotnet fake`), which pulls
FSharp Compiler Services (FCS) to compile the script on every invocation — the
per-invocation compile tax the programme exists to remove. The question: what
form should the build front-end take so target bodies can live in a *compiled*
governance library?

## Decision

The build front-end is a **dedicated, compiled F# `Exe`** (`build/Build.fsproj`)
that takes a `PackageReference` on **`Fake.Core.Target`** (and the minimal
companion `Fake.Core.*` packages its API needs), defines targets in `Program.fs`
via the modular `Target.create` / `runOrDefaultWithArguments` API, and is invoked
with `dotnet run --project build/Build.fsproj -- <target>`. Target **bodies
delegate to the referenced compiled library** (`FS.Skia.UI.Build.Spike.run`) — no
logic is inlined in the front-end. **No `FSharp.Compiler.Service`** is taken as a
dependency (FR-012).

## Alternatives considered

- **Thin `build.fsx` `#r` shim (the documented fallback):** keep `dotnet fake`
  but `#r` the compiled DLL from a <200-line script. Rejected as the *primary*
  path because it retains the per-invocation FSX compile tax; reserved only as
  the FR-007 fallback if a concrete FAKE-as-library blocker had surfaced. The
  spike confirmed D2, so the fallback is **not** needed.
- **Custom MSBuild targets / Nuke / Cake (rejected):** changes the orchestration
  model wholesale and is out of scope; D2 is decided as FAKE-as-library.
- **FAKE meta-package (rejected):** drags in the script runner and FCS,
  contradicting FR-012; the minimal `Fake.Core.Target` set is used instead.

## Consequences / rationale

- A compiled `dotnet run` front-end builds the whole project graph with IDE-grade
  tooling and no DLL bootstrap-order wrinkle, and removes the FSX compile tax.
- Driving a target from a library validated that the modular `Fake.Core.Target`
  API works in an ordinary compiled assembly — the property D2 bets on.

## Stages shaped

- **Stage 5** retires `build.fsx` in favour of the compiled front-end, migrating
  target bodies into the governance library behind the `dotnet run` entry point.

## Verification in feature 039 (spike)

**D2 confirmed.** Both projects compile clean under
`net10.0`/`TreatWarningsAsErrors` (`0 warnings, 0 errors`); `SpikeHello` runs via
`dotnet run` and prints the value returned by `FS.Skia.UI.Build.Spike.run`
(proving the body ran from the library, not inlined); and
`dotnet list build/Build.fsproj package --include-transitive` shows **no**
`FSharp.Compiler.*` (FR-012). The committed `build/spike-verify.sh` reproduces the
result (`SPIKE-VERIFY PASS: D2 confirmed`). Full evidence and commands are in
`docs/reports/_baselines/2026-05-31-spike-d2-outcome.md`.

# Quickstart: Migrating to the Compiled Build Front-End

The capture → relocate → diff → delete walkthrough that gates this feature. Order matters: the
golden baseline is captured from the **live `build.fsx` path before** any relocation, and
`build.fsx` is deleted **only after** the parity set is clean.

## 0. Route first (escalated path expected)

```bash
./fake.sh build -t Route
```
This change touches `build.fsx` / launchers / `.config/dotnet-tools.json` / governance paths, so
`Route` escalates to the full serialized gate set. Run the gates it prints (the six-target order).

## 1. Capture the golden baseline (BEFORE touching anything)

For every target in `Targets.dispatchTargets`, capture its deterministic reports/artifacts from the
current `dotnet fake` path into `readiness/parity/<target>/baseline/`. Record the Class-C
stash-control proof (`FsiTranscripts`, `TemplateCheck` headless flake) in
`readiness/parity/exclusions.md`. See [contracts/parity-oracle.md](./contracts/parity-oracle.md).

## 2. Relocate the MEL engine into the library

Move `BuildModel`/`BuildMsg`/`BuildEffect`/`init` → `Engine/Model.fs(+.fsi)`, the pure `update` →
`Engine/Update.fs(+.fsi)`, and `interpret` + `runTarget` → `Engine/Interpret.fs(+.fsi)`. Add them
to `build/Governance/FS.Skia.UI.Build.fsproj`. `update`'s `.fsi` exposes no I/O symbol.

```bash
dotnet build build/Governance/FS.Skia.UI.Build.fsproj   # clean under TreatWarningsAsErrors
```

## 3. Relocate the three heavy validators

`GeneratedProduct.fs`, `Guidance.fs`, `Preflight.fs` (+ `.fsi` each), behaviour-identical. Repoint
the `interpret` arms at the relocated functions.

## 4. Grow the front-end + delegate every target

`build/Program.fs`: register every `Targets.dispatchTargets` target, wire `==>` from
`Targets.targetDependencyRows`, and make each `Target.create` body call
`Engine.Interpret.runTarget`. Remove `SpikeHello` and the spike residue (`build/spike-verify.sh`,
`build/SkillExamples/`).

```bash
dotnet run --project build/Build.fsproj -- Dev      # behaves like ./fake.sh build -t Dev
```

## 5. Rewire the launchers + toolchain

- `fake.sh` → `dotnet run --project build/Build.fsproj -- "$@"` (drop `dotnet tool restore` + `dotnet fake`).
- `fake.cmd` → `dotnet run --project build/Build.fsproj -- %*` (preserve `%ERRORLEVEL%`).
- Remove `fake-cli` from `.config/dotnet-tools.json`.

```bash
grep -rn "dotnet fake" fake.sh fake.cmd scripts/ ; grep -rn "fake-cli" .config/  # expect: nothing
```

## 6. Prove parity (the merge gate)

Re-run every target through the new front-end into `readiness/parity/<target>/after/`; normalize
and diff vs `baseline/`. Every Class-A/Class-B diff must be empty. Resolve any diff by fixing the
relocation — never by weakening the oracle.

## 7. Delete `build.fsx`

Only now. Record the line delta (4,767 → 0) in `readiness/build-fsx-line-delta.md`. Use the
≤200-line `#r`-the-DLL shim **only** if a concrete blocker appeared (record the residual count).

## 8. Tests + timing

```bash
dotnet test tests/Governance.Tests   # update effect-list tests + relocated-validator tests, all green
```
Record cold/warm wall-clock vs baseline in `readiness/logs/build-timing.md` (recorded, not gated).

## 9. Run the escalated serialized gate set (sequential, never concurrent)

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

## 10. Grep proofs

```bash
# no FSharp.Compiler.* / FCS anywhere
grep -rn "FSharp.Compiler" --include=*.fs --include=*.fsproj --include=*.fsx . || echo "clean"
```
Capture into `readiness/logs/{no-dotnet-fake,no-fake-cli,no-fcs}.txt`.

## Done-when

`build.fsx` deleted; every Class-A/B target byte-identical to baseline; `update` + relocated
validators unit-tested green; no `dotnet fake`/`fake-cli`/`FSharp.Compiler.*`; serialized gates
green; timing recorded; invariants 1–6 hold.

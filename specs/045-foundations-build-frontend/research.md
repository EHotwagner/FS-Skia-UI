# Phase 0 Research: Dedicated Build Front-End + MEL Engine Extraction

All spec clarifications were resolved in the 2026-06-01 session (parity-oracle scope;
warm-build recorded-not-blocking). No `NEEDS CLARIFICATION` remains. This document records the
**design decisions** the relocation depends on, each as Decision / Rationale / Alternatives.

## R1. Engine module split (where `update`/`interpret`/`init` land)

- **Decision**: Split the MEL engine into three library modules under `build/Governance/Engine/`:
  `Model.fs` (`BuildModel`, `BuildMsg`, `BuildEffect`, `init`), `Update.fs` (the **pure** `update`),
  and `Interpret.fs` (the I/O `interpret` edge plus `runTarget`). `BuildMsg.StartTarget` carries the
  **typed** `Targets.Target` (already true in `build.fsx`), so dispatch is typed end-to-end.
- **Rationale**: Putting `update` in its own module with its own `.fsi` makes Principle IV
  enforceable by the compiler — `update`'s `.fsi` can expose only `Msg × Model → Model × Effect list`
  and nothing that does I/O, so a stray `File.*` call in `update` won't compile against the helper
  surface. `Interpret.fs` is the only module that opens `System.IO`/`git`/process helpers. This is
  the smallest split that keeps the pure/edge boundary a *type* boundary, not a convention.
- **Alternatives**: (a) One `Engine.fs` — rejected: re-creates the "everything in one unit" problem
  at library scale and lets `update` reach I/O helpers. (b) Keep `init`/`interpret` in the
  front-end exe — rejected: FR-001 forbids inlined orchestration logic in the exe; the exe must only
  register + delegate.

## R2. The 35-case `BuildEffect` DU and its interpreter helpers move wholesale

- **Decision**: Relocate `BuildEffect` (the ~35 cases incl. `EnsureDirectory`, `RunProcess`,
  `CapabilityCatalogCheck`, `ScanGeneratedProjects`, `EvidenceGraphCheck`, `RouteSelect`, …) and the
  `interpret` `match` verbatim, repointing the validator-call arms (`runCapabilityCatalogCheck`,
  `scanGeneratedProjects`, `runGeneratedConsumerValidation`, `runGeneratedGuidanceScan`,
  `collectProcessHealth`, `validateRunnerBootstrap`, …) at the relocated library functions (R4).
  Effects that already delegate to `FS.Skia.UI.Build` (evidence, routing, skill-sync, capability)
  keep delegating; only their *call site* moves from script into `Interpret.fs`.
- **Rationale**: The effect algebra is already the right shape and is the unit the parity proof is
  written against. Moving it verbatim keeps the diff reviewable and the parity diff honest.
- **Alternatives**: Re-categorizing/renaming effects during the move — rejected: it would muddy the
  parity proof and is gratuitous churn for a behaviour-preserving refactor.

## R3. The three remaining heavy validators relocate behaviour-identically

- **Decision**: Move, behaviour-preserving, into dedicated library modules:
  - **`GeneratedProduct.fs`** ← `scanGeneratedProjects`, `runGenerateV3Products`,
    `runScanV3GeneratedProducts`, `runGeneratedConsumerValidation` (build.fsx ~2052–3500, the
    ~800-line generated-product structural validation). **No `schema_version`/deprecation window**
    (that is Stage 6.4, out of scope) — a pure move with byte-identical reports.
  - **`Guidance.fs`** ← `runGeneratedGuidanceScan` + the skill-section/markdown scanners
    (build.fsx ~3635–4300, ~200 lines).
  - **`Preflight.fs`** ← `collectProcessHealth` + `validateRunnerBootstrap` and their
    `ProcessHealthThreshold`/`ProcessHealthSnapshot`/`BootstrapValidation` types
    (build.fsx ~118–162, 1431–1800, ~267 lines).
  Each returns **typed findings** (reuse `Findings.ValidationFinding`) and renders the **same**
  report text; the `interpret` arm calls the library and writes the report at the edge.
- **Rationale**: These are the last domain blocks keeping `build.fsx` large; relocating them is what
  makes deletion possible and gives them their first unit tests (FR-013). Keeping structural checks
  byte-identical preserves the consumer contract (Invariant 3) and keeps the Stage-6.4 versioning
  work cleanly separable.
- **Alternatives**: Folding versioning in now — rejected (Stage 6.4, would break the clean parity
  proof). Leaving preflight in the front-end — rejected (FR-010 + FR-001).

## R4. Front-end registration completeness is a compile/startup guarantee

- **Decision**: The front-end registers targets by iterating `Targets.dispatchTargets` (the typed
  registry, already the FAKE-registration driver in `build.fsx`) and wiring `==>` from
  `Targets.targetDependencyRows`, with each `Target.create` body calling
  `Engine.Interpret.runTarget target`. A target present in the `Targets` DU but absent from dispatch
  is impossible because both derive from the same closed union — a **compile error** if a case is
  unhandled. The exe contains **no** inlined target body.
- **Rationale**: FR-001 + the spec's "missing registration must be a compile error or startup
  failure, never silently absent." Reusing the existing `dispatchTargets`-driven registration (which
  `build.fsx` already does at its tail) means the registration code moves nearly verbatim into
  `Program.fs`.
- **Alternatives**: A hand-maintained registration list in the exe — rejected: re-introduces a
  second source of truth and a silent-omission risk.

## R5. Parity oracle construction (the gate that makes deletion safe)

- **Decision**: Per the clarification — capture a **golden baseline** of every target's
  deterministic governance reports/artifacts from the **current `build.fsx` path** *before* any
  relocation, then diff the post-migration outputs with known nondeterminism **normalized**
  (timestamps, absolute paths, run-ordering sorted as the script already sorts). **Test-shelling**
  targets (those that run `dotnet test`/log with timestamps) are compared by **verdict + report**,
  not raw stdout. The two documented **pre-existing-RED** gates (`FsiTranscripts`, `TemplateCheck`'s
  `SkiaViewer.Tests` headless flake) are **excluded** from the byte-diff via the same **stash-control
  disclosure** feature 039 used (prove they fail identically with this feature's edits stashed).
  Exclusions are **enumerated and justified**, never silent. Captured/diffed under
  `readiness/parity/<target>/`.
- **Rationale**: This is the same capture-then-diff discipline that gated the Stage-4 Python port
  (proven to byte-identical on 036/037/038). It is the real merge gate (SC-002); build-time is only
  an observation (R6).
- **Alternatives**: Diffing raw stdout for all targets — rejected: test-shelling/log targets carry
  irreducible nondeterminism; the clarification explicitly chose normalized-reports + verdict.

## R6. Warm/cold build timing is recorded, not gated

- **Decision**: Capture cold (`dotnet fake` baseline vs `dotnet run` cold) and warm wall-clock into
  `readiness/logs/build-timing.md`; report vs baseline; **do not block** on a non-improvement.
- **Rationale**: FR-014/SC-007 clarification — behaviour parity (SC-002) is the gate; timing is an
  observation. A compiled library replacing the 207 KB script recompile is *expected* to be at least
  as fast warm, but variance on this toolchain makes a hard gate brittle.
- **Alternatives**: Hard warm-build-faster gate — rejected by clarification.

## R7. Launcher + toolchain rewire (drop `dotnet fake`/`fake-cli`)

- **Decision**: `fake.sh` → `dotnet run --project build/Build.fsproj -- "$@"` (drop the
  `dotnet tool restore` + `dotnet fake` lines; keep `set -euo pipefail` and the `cd` to script dir;
  keep `FAKE_*` env only if the Target API still reads it under `dotnet run` — verified during
  implementation, else removed). `fake.cmd` → `dotnet run --project build/Build.fsproj -- %*`
  preserving the `%ERRORLEVEL%` propagation. Remove `fake-cli` from `.config/dotnet-tools.json`
  (leaving an empty `tools` object or deleting the file if nothing else uses it). Argument
  forwarding must keep `-t Dev` and bare `Route --enforce` working identically (contract:
  `contracts/front-end-cli.md`).
- **Rationale**: FR-002/FR-003/SC-003 — no `dotnet fake` invocation may remain (grep-proven), and a
  stale `fake-cli` restore step must not linger (edge case in the spec). `dotnet run` builds the
  whole project graph, so no DLL bootstrap-order wrinkle (ADR D2).
- **Alternatives**: Keeping `dotnet tool restore` "just in case" — rejected: leaves the removed
  tool half-wired and fails the grep proof.

## R8. Fallback policy: delete, don't shim (D2 confirmed)

- **Decision**: **Delete `build.fsx`.** The ≤200-line `#r`-the-DLL shim (still via `dotnet fake`) is
  retained **only** as a documented fallback **if** a concrete blocker surfaces during migration; if
  used, the residual line count is recorded against the 4,767/4,688 baseline. Planning **confirms
  deletion** as the default — the 039 spike (`2026-05-31-spike-d2-outcome.md`,
  `SPIKE-VERIFY PASS: D2 confirmed`) already proved the FAKE Target API drives targets from a
  compiled exe with no FSX runner and no `FSharp.Compiler.*`.
- **Rationale**: D2 is confirmed; FR-011/SC-001. The fallback exists for honesty, not as the
  expected path.
- **Alternatives**: Default to the shim — rejected: contradicts D2 and the whole-programme
  definition of done (`build.fsx` deleted).

## R9. No new `FSharp.Compiler.*` / FCS; config stays compiled

- **Decision**: Introduce no FCS / runtime-script-loading dependency; grep-prove its absence
  (`readiness/logs/no-fcs.txt`). The front-end consumes the existing compiled `Routing.fs` for the
  `Route` target (FR-005) — no new routing logic, no `select-tier.fsx`.
- **Rationale**: ADR D6 / FR-004 / SC-004. The whole programme removes the per-invocation compile
  tax; re-introducing FCS would undo it.
- **Alternatives**: none viable under D6.

## Resolved spec clarifications (for traceability)

- **Parity gate scope** → normalized deterministic reports; verdict+report for test-shelling
  targets; stash-control-excluded pre-existing-RED gates (`FsiTranscripts`, `TemplateCheck` headless
  flake), enumerated. (Drives R5, `contracts/parity-oracle.md`.)
- **Warm-build criterion** → recorded-and-explained, not merge-blocking. (Drives R6.)

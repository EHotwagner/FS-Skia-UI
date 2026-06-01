# Implementation Plan: Dedicated Compiled Build Front-End + MEL Engine Extraction

**Branch**: `045-foundations-build-frontend` | **Date**: 2026-06-01 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/045-foundations-build-frontend/spec.md`

## Summary

Finish the foundations keystone (Stage 5 of the foundations programme). Grow the existing
single-`SpikeHello` front-end (`build/Build.fsproj`, feature 039) into the **real** compiled FAKE
front-end that registers **every** target from the typed `Targets` graph and delegates each target
body to `FS.Skia.UI.Build`; relocate the **MEL engine** (`BuildMsg`/`BuildEffect`/`BuildModel` +
`update` + `interpret`) and the three remaining heavy validators (generated-product ~800 lines,
generated-guidance ~200 lines, process-health/preflight ~267 lines) out of the 4,767-line
`build.fsx` and into compiled, unit-tested library modules; rewire `fake.sh`/`fake.cmd` to
`dotnet run --project build/Build.fsproj`, drop `fake-cli` from `.config/dotnet-tools.json`; and
**delete `build.fsx`**. The whole move is gated on a **byte-identical golden parity** proof
captured from the current `build.fsx` path before deletion. The runtime (`Scene → SkiaViewer →
Elmish`) and the product public `.fsi` surface are untouched; the only `.fsi` edits are curated
signatures for the new build-tooling modules. Routing (Stage 5.5) was already delivered in feature
042 — this feature only **consumes** `Routing.fs` from the relocated front-end.

This is a **Tier 2 (internal) behaviour-preserving refactor** of the tooling around the framework,
proven by parity rather than by new behaviour. Per `Route`, the change touches `build.fsx` /
launchers / `.config/dotnet-tools.json` / governance paths and therefore **escalates** to the full
serialized gate set.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (inherits `Directory.Build.props`,
`TreatWarningsAsErrors`, `FS0078`-as-error, Central Package Management)
**Primary Dependencies**: `Fake.Core.Target` (build-tooling, already central), `YamlDotNet`
(already central); **no new package identity**; **no `FSharp.Compiler.*` / FCS** anywhere (ADR D6)
**Testing**: Expecto + FsCheck unit/property tests in `tests/Governance.Tests` (already
project-references `FS.Skia.UI.Build`); DiffPlex golden byte-diff as the parity oracle; the
serialized FAKE gate sequence
**Target Platform**: Windows and Linux (launchers `fake.sh` + `fake.cmd`)
**Project Type**: Build-tooling / governance library + dedicated build-front-end exe
**Performance**: Cold/warm build wall-clock **recorded vs baseline, not a merge gate** (FR-014/SC-007)
**Baseline**: `build.fsx` = **4,767** lines working / **4,688** lines Stage-0
(`docs/reports/_baselines/2026-05-31-foundations.md`); the D2 spike outcome
(`docs/reports/_baselines/2026-05-31-spike-d2-outcome.md`) confirmed the dedicated-exe form
**Unknowns**: none requiring clarification — the two open spec questions were resolved in the
2026-06-01 clarification session (parity oracle = normalized reports + verdict-for-shelling-targets;
warm-build = recorded-not-blocking). Remaining design choices are recorded in
[research.md](./research.md), not flagged NEEDS CLARIFICATION.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-checked after Phase 1 design.*

**Constitution version**: 1.3.0. **Verdict: PASS (pre-Phase-0) and PASS (post-Phase-1).**

| Principle | Assessment |
|---|---|
| **I. Spec → FSI → Tests → Impl** | The relocated modules get curated `.fsi` first; `update` is exercised through the library surface in `Governance.Tests` (FSI-honest: the tests call the same public functions a script/host would). Order honoured: this plan precedes the `.fs` bodies. |
| **II. Visibility in `.fsi`** | Every new/relocated public module (`Engine/Model.fs`, `Engine/Update.fs`, `Engine/Interpret.fs`, `GeneratedProduct.fs`, `Guidance.fs`, `Preflight.fs`) ships a curated `.fsi`; no `private`/`internal`/`public` modifiers in `.fs`. These are **build-tooling** surfaces, not product surface — no product surface-baseline changes. |
| **III. Idiomatic simplicity** | Pure relocation: the engine stays a DU + `match` + record; no SRTP, type providers, reflection, or custom operators introduced. `==>` is FAKE's existing operator, retained. Any `mutable` carried over keeps its use-site comment. |
| **IV. Elmish/MVU boundary** | This is the **build-side** MVU engine. `update` stays a **pure** `Msg × Model → Model × Effect list`; **all** filesystem/`git`/process/write I/O stays at the `interpret` edge. The relocation makes `update` unit-testable for the first time (FR-007/FR-013) — strengthens, not weakens, the boundary. The product runtime Elmish is untouched. |
| **V. Synthetic evidence** | **None planned.** Parity is real (golden baseline captured from the live `build.fsx` path); unit tests assert real typed effect lists and real fixture findings. No `[S]`/`[SEH]` expected. If any fixture-only validator test arises it gets full disclosure, but the design avoids it. |
| **VI. Test evidence** | New failing-first unit tests for `update` (typed `BuildEffect` lists) and the three relocated validators (typed findings + golden report parity); the full serialized FAKE gate sequence green. Because the phase order extracts the bodies (Phase 3) before the suites are authored (Phase 5), failing-first is captured by a **stash control** — stash the relocated body for RED, unstash for GREEN — the same stash-control discipline the parity oracle uses. |
| **VII. Observability** | Behaviour-preserving: every relocated diagnostic/report keeps its exact text (parity proves it). `Preflight.fs` carries the process-health/bootstrap diagnostics verbatim. |

### Repository Governance Decisions

- **Template ownership**: **No template content change of substance.** `template/base/build.fsx`
  is the *generated-consumer* build script and is **out of scope** (its evidence engine already
  consumes the packaged `FS.Skia.UI.Build` since feature 043). This feature relocates **framework-repo**
  build logic only. `.template.config/template.json` does not change. The generated-product
  *structural* checks must stay byte-identical (their versioning is Stage 6.4).
- **Dependency impact**: No new `PackageVersion` outside `Directory.Packages.props`; no new package
  identity. `docs/reports/dependencies.md` unchanged (no new dependency). `DependencyReport` must
  stay green and behaviour-identical.
- **Command-surface impact**: This is a build-**system** change — **every** target's invocation
  path moves from `dotnet fake` to the compiled front-end, so `Dev`, `Verify`, `Ci`, `PackLocal`,
  `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`,
  `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`, `Route`, `RefreshSurfaceBaselines`,
  and the rest must remain registered and behaviour-identical. `fake.sh`/`fake.cmd` +
  `.config/dotnet-tools.json` change. `build.fsx` is **deleted**. FAKE-backed commands stay
  sequential (never concurrent).
- **Generated project impact**: **None to the consumer contract.** Generated projects already
  consume the packaged evidence engine; relocating framework-repo build logic must not change the
  consumer package contract or the generated-product structural expectations
  (`TemplateCheck`/`GeneratedProductCheck`/`GeneratedGuidanceCheck` stay byte-identical).
- **Evidence paths**: golden baseline + post-migration diffs under
  `specs/045-foundations-build-frontend/readiness/parity/<target>/`; grep proofs under
  `readiness/logs/` (`no-dotnet-fake.txt`, `no-fake-cli.txt`, `no-fcs.txt`); line-delta under
  `readiness/build-fsx-line-delta.md`; wall-clock under `readiness/logs/build-timing.md`; the
  serialized FAKE gate logs under `readiness/logs/`.
- **`.fsi` / contract impact**: **No product `.fsi` change.** Only **curated** governance-library
  `.fsi` files for the relocated modules (Principle II). No product surface-baseline, sample
  contract, or public-API change → `PackageSurfaceCheck`/`FsiTranscripts` show **no product
  baseline diff**.
- **MVU/effect boundary**: `BuildModel` (state), `BuildMsg` (`StartTarget of Targets.Target` + the
  completion/health/verdict messages), `BuildEffect` (the ~35-case effect DU), `init` (initial
  model + startup effects), **pure** `update`, edge `interpret`. Emitted-effect assertions for
  representative targets are the headline new tests.
- **Synthetic evidence**: none planned; `[S]`/`[SEH]` disclosure machinery on standby but expected
  unused.
- **Test evidence**: failing-first (stash-control RED→GREEN) `update` effect-list tests;
  relocated-validator typed-finding + golden-report tests; parity golden diff; serialized FAKE gates.
- **Observability**: relocated diagnostics preserved verbatim; missing-artifact failures
  (`RequireFiles`) preserved; unsupported-environment messages unchanged (the two pre-existing-RED
  gates are excluded via the same stash-control disclosure feature 039 used).
- **Deferred scope**: generated-product contract **versioning** / `schema_version` / deprecation
  window (Stage 6.4); prose/skill/constitution trimming (Stage 6.2–6.3); new bucket-(a) production
  gates (Stage 6.1); `.gitignore`/evidence hygiene (Stage 6.5); any further Python/Bash porting;
  any runtime/rendering/packaging-identity/public-`.fsi` change.

## Project Structure

### Documentation (this feature)

```
specs/045-foundations-build-frontend/
├── spec.md              # input (complete, clarified)
├── plan.md              # this file
├── research.md          # Phase 0 — decisions & rationale
├── data-model.md        # Phase 1 — engine/validator entities + module layout
├── quickstart.md        # Phase 1 — capture-baseline → migrate → diff → delete walkthrough
├── contracts/
│   ├── front-end-cli.md        # dotnet run / fake.sh argument-forwarding contract
│   ├── library-modules.md      # curated .fsi surfaces for the relocated modules
│   └── parity-oracle.md        # what the golden diff compares, normalizes, excludes
└── checklists/          # (existing)
```

### Source (repository) — paths this feature changes

```
build/
├── Build.fsproj                         # CHANGED: grows from SpikeHello to the real front-end
├── Program.fs                           # CHANGED: registers every Targets.dispatchTargets target,
│                                        #          delegates each body to the library; spike removed
├── spike-verify.sh                      # REMOVED (spike residue)
├── SkillExamples/                       # REMOVED (spike residue; SkillExamplesCheck retired in 044)
└── Governance/
    ├── FS.Skia.UI.Build.fsproj          # CHANGED: + Engine/*, GeneratedProduct, Guidance, Preflight
    ├── Engine/
    │   ├── Model.fs(+.fsi)              # NEW: BuildModel, BuildMsg, BuildEffect, init
    │   ├── Update.fs(+.fsi)             # NEW: pure update
    │   └── Interpret.fs(+.fsi)          # NEW: interpret edge (I/O) + runTarget
    ├── GeneratedProduct.fs(+.fsi)       # NEW: ~800-line generated-product validation, behaviour-identical
    ├── Guidance.fs(+.fsi)               # NEW: ~200-line generated-guidance / skill-section scanners
    └── Preflight.fs(+.fsi)              # NEW: ~267-line process-health / bootstrap preflight

build.fsx                                # DELETED (4,767 → 0; ≤200-line shim only as documented fallback)
fake.sh                                  # CHANGED: dotnet run --project build/Build.fsproj -- "$@"
fake.cmd                                 # CHANGED: dotnet run --project build/Build.fsproj -- %*
.config/dotnet-tools.json                # CHANGED: fake-cli removed

tests/Governance.Tests/
├── Governance.Tests.fsproj             # CHANGED: + new test files
├── BuildEngineUpdateTests.fs           # NEW: pure update → typed BuildEffect list assertions
├── GeneratedProductValidatorTests.fs   # NEW: typed findings + golden report parity
├── GuidanceValidatorTests.fs           # NEW: typed findings + golden report parity
└── PreflightValidatorTests.fs          # NEW: typed findings + golden report parity
```

(Final file names/splits may consolidate during implementation; the `.fsi` boundary and the
pure-`update`/edge-`interpret` split are fixed.)

## Phase 0 — Research

See [research.md](./research.md). All spec clarifications were resolved 2026-06-01; Phase 0
consolidates the **design decisions** the move depends on (engine module split, parity-oracle
construction, front-end registration completeness, launcher/toolchain rewire, fallback policy) with
rationale and alternatives. **No NEEDS CLARIFICATION remains.**

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md): the engine entities (`BuildModel`/`BuildMsg`/`BuildEffect`),
  the typed-`Target` dispatch wiring, the three relocated-validator entities, and the golden
  target-output baseline.
- [contracts/front-end-cli.md](./contracts/front-end-cli.md): the `dotnet run -- <target> [flags]`
  ↔ `fake.sh`/`fake.cmd` argument-forwarding contract (incl. `Route --enforce`, default target).
- [contracts/library-modules.md](./contracts/library-modules.md): the curated `.fsi` surfaces.
- [contracts/parity-oracle.md](./contracts/parity-oracle.md): the byte-diff comparison set,
  normalization rules, verdict-comparison for shelling targets, and the enumerated/justified
  pre-existing-RED exclusions.
- Agent context: the AGENTS.md plan pointer is updated to this plan.

## Complexity Tracking

No constitution deviations to justify. The MVU engine, FAKE `==>` operator, and YAML-behind-typed-model
are all pre-existing and merely relocated; no new complex feature (SRTP, type providers, reflection,
non-trivial CE, multi-case active patterns) is introduced.

## Phase 2 — (handled by `/speckit-tasks`)

This command stops after Phase 1. Task breakdown is produced by `/speckit-tasks`.

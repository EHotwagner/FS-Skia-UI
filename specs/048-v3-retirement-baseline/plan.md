# Implementation Plan: V3 Stage 0 — Monolith-Retirement Baseline, Per-Package Surface Baselines & Parity Oracle

**Branch**: `048-v3-retirement-baseline` | **Date**: 2026-06-02 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/048-v3-retirement-baseline/spec.md`
**Programme source**: `docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md` §Stage 0

## Summary

Stage 0 of the V3 monolith-retirement programme is **record-and-oracle only**: it changes no
runtime code and moves no library code between packages (FR-010, SC-007). It produces four
classes of durable artifact that make the later runtime moves *provable* rather than asserted:

1. a **SHA-pinned baseline report** with monolith LOC, the runtime dependency graph, the
   duplicate-scene-type inventory, a reproducible **leak proof**, and the full consumer
   inventory (FR-001/002/003);
2. a captured **parity oracle** — deterministic scene-output golden fixtures (authoritative) plus
   reference screenshots (corroboration) taken from the *current* host, with the capture
   environment recorded (FR-004/005);
3. **per-package public-surface baselines** for the 8 public split packages and a **new,
   additive per-package surface-diff capability** that reports drift per package and flags exactly
   one package on a seeded violation (FR-006/007/008);
4. **ADRs 0007–0011** locking the retirement's shape (FR-009).

### Planning finding that shapes the approach (read before tasks)

The spec's premise ("today only an aggregate surface baseline exists") is **directionally correct
but the on-disk reality is more specific**, and the difference changes the design:

- A target named **`PackageSurfaceCheck` already exists** and is wired into `Routing.fs`
  (rule `package-surface`, on `src/**/*.fsi` + `readiness/surface-baselines/**`). Its baselines
  live at repo-root `readiness/surface-baselines/*.txt` and are **exported-type-NAME sets**
  extracted by reflection over compiled assemblies (`scripts/refresh-surface-baselines.fsx`,
  `tests/Package.Tests/SurfaceAreaTests.fs`).
- That existing check is **coarse and monolith-inclusive**: its generator drives only 5 assemblies
  (the monolith `FS.Skia.UI` + `Layout`/`KeyboardInput`/`Controls`/`Controls.Elmish`); the
  `Scene`/`SkiaViewer`/`Elmish`/`Testing` `.txt` files are stale stubs. A *type-name set* cannot
  detect a changed **signature** inside an existing type/module — so it cannot satisfy SC-005.

**Decision (locked here, recorded in `research.md`):** treat the existing `PackageSurfaceCheck` as
the **aggregate** check the spec says must stay green and unweakened (FR-011), and add the new
capability as a **distinctly-named, additive** target — **`PerPackageSurfaceDiff`** — over a new
artifact tree `readiness/per-package-surface/`, capturing each package's **normalized full `.fsi`
surface text** (signature-sensitive, monolith-excluded). The programme-plan's loose prose name
"PackageSurfaceCheck extension" (§0.3) is superseded by FR-011's "must not weaken or replace."

A second finding: **`tests/Parity.Tests` today is not a scene-output golden harness** — it
validates a `parity-evidence.json` report against an upstream Skia commit SHA (the historical
`002-skia-feature-parity` work) and references the monolith's `FS.Skia.UI.Parity` module. So the
scene-output golden fixtures (FR-004) are **captured fresh** from the current host under a new
fixtures tree; the existing `Parity.Tests` is left untouched (it retires in programme Stage 4).

### Implementation finding (recorded post-build): the `PerPackageSurfaceDiff` Routing rule is deferred

During implementation a **runtime-coupling** surfaced that the plan's §0.3/D9 (add a `Routing.fs`
rule for the new target) did not anticipate: a Routing rule renders `PerPackageSurfaceDiff` into
`validation.contract.yml`'s `routing_rules.required_gates`, and the contract validator's
**known-gate allowlist lives in the runtime monolith** (`src/Lib/AgentValidation.fs` `knownGates`).
Teaching that allowlist the new gate is a **runtime code change**, which this feature forbids
(record-and-oracle only; `src/**` byte-unchanged — FR-010/SC-007). The spec constraint wins over
the plan's rule sub-step. **Resolution:** the `PerPackageSurfaceDiff` target ships **additive and
runnable directly** (`./fake.sh build -t PerPackageSurfaceDiff`, the escalated gate set, and the
quickstart), but **no Routing rule is added** and `validation.contract.yml` is left unchanged.
Route-gating the new target is **deferred** with the Stage-5 hard-gate enforcement (which the
capability contract already defers) and the Stage-2 `AgentValidation` relocation into the
governance library (ADR 0009) — after which the known-gate allowlist becomes governance config and
the rule can be added without touching runtime code. Recorded in
`readiness/per-package-surface-expectations.md`, `readiness/runtime-untouched.md`, and the T016
task line.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (governance library `FS.Skia.UI.Build` conventions).
**Primary Dependencies**: no new runtime dependencies. The new diff capability reuses the
repository's existing **DiffPlex** golden-diff facility and the in-process glob/IO helpers
(`fsharp-io-globbing`); no package additions to `Directory.Packages.props`.
**Testing**: Expecto semantic tests driving the new capability through its public `.fsi`; FAKE
targets (`PerPackageSurfaceDiff`, plus the full serialized escalated set); FSI transcript for the
diff surface. Parity golden re-derivation is a deterministic byte-equality test.
**Target Platform**: Windows and Linux. Screenshot capture is Linux-headless-aware (the known
`SkiaViewer.Tests` libdecor-gtk flake) — scene-output is the authoritative oracle.
**Baseline pin SHA**: `031e56072779c736adf6dd8b0345e17b58a62e73` (recorded in the report; if the
branch advances the pin, the branch-point SHA is recorded instead, per Assumptions).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-checked after Phase 1 design (below).*

**Change classification**: **Tier 1 (contracted change)** for the **governance/build surface only** —
it adds a new build target and a new curated-`.fsi` governance module and new baseline artifacts.
It is **Tier 2-equivalent (zero change) for the runtime**: no public runtime `.fsi`, no package
identity/version, no rendering behaviour changes (FR-010, FR-011, SC-007). `Route` escalates the
change to the full serialized gate set because it touches governance/build paths (dogfood).

### Repository Governance Decisions

- **Template ownership**: **N/A — no template change.** No `template/**`, `.template.config`, sample
  fragment, package policy, or command-surface change reaches generated products. The new target is
  a framework-internal governance gate, not a generated-consumer capability; `template.json` is
  untouched. Generated-consumer contract checks (`TemplateCheck`, `GeneratedProductCheck`,
  `GeneratedGuidanceCheck`) must remain green and unchanged in behaviour (FR-011, SC-008).
- **Dependency impact**: **No dependency change.** No new `PackageVersion` in
  `Directory.Packages.props`; the diff capability reuses existing DiffPlex + BCL IO. `docs/reports/dependencies.md`
  and `DependencyReport` coverage are unchanged. The baseline report *reads* the dependency graph
  (via `dotnet list package --include-transitive` / packed-graph dump) but adds no dependency.
- **Command-surface impact**: **One additive target.** The front-end gains a new
  `PerPackageSurfaceDiff` FAKE target (compiled `Target` case in `build/Governance/Targets.fs(i)`,
  wired through `Engine/Model.fs` + `Engine/Update.fs` + `Engine/Interpret.fs` + a
  `Front/Governance.fs` interpreter edge). **No `Routing.fs` rule is added** (see the implementation
  finding above), so `validation.contract.yml` is **unchanged** and `TargetMetadataDrift`/
  `ContractView` currency holds against the committed file. **No change to the behaviour** of `Dev`,
  `Verify`, `Ci`, `PackLocal`, `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`,
  `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`, or the existing aggregate `PackageSurfaceCheck`.
  FAKE-backed commands share `.fake` state — run **sequentially** in the deterministic serialized
  order; safe non-FAKE reads/checks may still parallelize.
- **Generated project impact**: **N/A — none.** Default/minimal generated contents, selected
  Controls guidance, local skills, validation logs, placeholder/excluded-history scans, and generated
  `Dev` behaviour are unchanged. The new target is never shipped to or run by a generated product.
- **Evidence paths**: exact readiness/output paths —
  - Baseline report: `docs/reports/_baselines/2026-06-02-v3-before.md`.
  - Parity oracle: `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/*.txt` (golden),
    `tests/Parity.Tests/fixtures/v3-host-golden/screenshots/*.png` (corroboration),
    `tests/Parity.Tests/fixtures/v3-host-golden/capture-environment.md` (recorded environment).
  - Per-package baselines: `readiness/per-package-surface/<Package>.fsi.txt` (8 split packages).
  - New-target run evidence: `specs/048-v3-retirement-baseline/readiness/per-package-surface-diff.md`
    (zero-drift run) and `.../seeded-violation.md` (one-package-drift demonstration).
  - FSI transcript: `specs/048-v3-retirement-baseline/readiness/fsi/per-package-surface-diff.txt`.
  - Surface-diff expectations doc (capability contract; Routing rule deferred — see the implementation finding): `readiness/per-package-surface-expectations.md`.
  - Standard evidence gate output: `specs/048-v3-retirement-baseline/readiness/{evidence-graph.md,evidence-audit.md,validation-contract.md,aggregate-hang-diagnostics.md}`.
- **`.fsi` / contract impact**: **No runtime `.fsi` changes** (SC-007). The only new `.fsi` is the
  governance module `build/Governance/PerPackageSurface.fsi` (curated public surface of the new
  capability, per Principle II). Per-package surface **baselines are captured, not modified** — they
  are descriptive artifacts of the unchanged public surfaces. No sample/compatibility-note change.
- **MVU/effect boundary**: **No stateful/I/O workflow** (spec State-workflow impact = None). The
  diff capability is a **pure comparison** (`diff : Baseline -> Surface -> PackageDrift list`) over
  captured text, with file reads confined to a thin **edge interpreter** that loads `.fsi` files and
  baseline files and writes the report. No `Model`/`Msg`/`Cmd`/subscription is introduced; Elmish
  ceremony is not warranted (Principle IV: simple pure functions need no MVU). Purity is asserted by
  semantic tests on the pure `diff`; the edge is exercised by a real-filesystem interpreter test.
- **Synthetic evidence**: **None planned — all evidence is real.** The zero-drift run reads the real
  captured `.fsi` surfaces; the seeded-violation demonstration is a **real experimental edit** to a
  real `.fsi`, reverted after capture (a real diff over real files, not a mock). Parity golden
  fixtures are produced by the **real current host**. No `[S]`/`[S*]`/`[SEH]` tasks are anticipated;
  `EvidenceAudit` must return PASS on zero-synthetic evidence (SC-008). If any capture turns out
  infeasible in the headless environment, it is disclosed per Principle V rather than faked.
- **Test evidence**: failing-first semantic tests for the new capability —
  (a) **pure-diff** tests: identical surfaces → empty drift; a single mutated signature → drift for
  exactly one package (the SC-005 oracle, as a unit test over literal-but-real surface text);
  (b) **interpreter** test: run the capability over the real captured baselines → zero drift at the
  pin (SC-004); (c) **golden re-derivation** test: scene-output fixtures re-derive byte-identically
  from the current host (SC-003). Governance test placement: `tests/Governance.Tests` (the new
  capability lives in `FS.Skia.UI.Build`). The `PerPackageSurfaceDiff` target runs these in the gate.
- **Observability**: the diff capability **fails loud with actionable context** — on drift it names
  the package, the added/removed/changed signature lines, and the baseline path to update; on a
  missing baseline it fails with the expected path and the regeneration command (it never silently
  passes a package with no baseline — Principle VII). The baseline report names the exact
  reproduction command beside every headline metric. Capture-environment is recorded so a screenshot
  mismatch is attributable to environment, not regression.
- **Deferred scope**: explicitly **out of this feature** (programme later stages) — turning
  per-package drift into a **hard merge gate** (Stage 5), the host move (Stage 1), `AgentValidation`
  relocation (Stage 2), sample/test repointing (Stages 3–4), monolith deletion/unpublish (Stage 5),
  the separate `FS.Skia.UI.Charts` split, template-profile expansion, and history rewrite. This
  feature delivers the capability **additive and green at baseline only**; enforcement is deferred.

## Project Structure

```
specs/048-v3-retirement-baseline/
  spec.md                     # (exists)
  plan.md                     # this file
  research.md                 # Phase 0 — decisions (naming, surface representation, parity method)
  data-model.md               # Phase 1 — entities: Baseline report, Parity oracle, Surface baseline, PackageDrift
  quickstart.md               # Phase 1 — how to reproduce baseline, run the diff, capture the oracle
  contracts/
    per-package-surface-diff.md   # contract of the new capability (inputs, outputs, drift semantics)
    baseline-report.md            # required sections + reproduction-command contract
  checklists/                 # (exists)
  readiness/                  # evidence (graph/audit/fsi/diff runs) — created during implement

# Durable artifacts produced by this feature (outside the spec dir)
docs/reports/_baselines/2026-06-02-v3-before.md          # FR-001/002/003 baseline report
docs/adr/0007-host-ownership.md                          # FR-009 ADRs 0007–0011
docs/adr/0008-scene-vocabulary-single-source.md
docs/adr/0009-agentvalidation-placement.md
docs/adr/0010-legacy-sample-policy.md
docs/adr/0011-parity-oracle-method.md
tests/Parity.Tests/fixtures/v3-host-golden/             # FR-004/005 parity oracle (golden + screenshots + env)
readiness/per-package-surface/<Package>.fsi.txt         # FR-006 per-package baselines (8 split packages)
readiness/per-package-surface-expectations.md           # capability contract (Routing rule deferred — see finding)

# New / changed governance code (compiled FS.Skia.UI.Build)
build/Governance/PerPackageSurface.fsi                   # FR-007/008 capability — curated public surface
build/Governance/PerPackageSurface.fs                    #            pure diff + edge interpreter
build/Governance/Targets.fs(i)                           # add Target.PerPackageSurfaceDiff + metadata
build/Governance/Engine/Model.fs(i)                      # add PerPackageSurfaceDiffCheck BuildEffect case
build/Governance/Engine/Update.fs                        # add StartTarget arm
build/Governance/Engine/Interpret.fs                     # dispatch the new effect
build/Governance/Front/Governance.fs                     # runPerPackageSurfaceDiff edge interpreter
# build/Governance/Routing.fs                            # Routing rule DEFERRED (runtime-coupling — see finding); validation.contract.yml unchanged
tests/Governance.Tests/PerPackageSurfaceTests.fs         # failing-first semantic + interpreter tests
```

**Packages in scope for per-package baselines (8):** `Scene`, `SkiaViewer`, `Elmish`,
`KeyboardInput`, `Layout`, `Controls`, `Controls.Elmish`, `Testing`. **Excluded:** the retiring
monolith `FS.Skia.UI` (`src/Lib`) and the build-tooling library `FS.Skia.UI.Build`. The `Controls`
package aggregates its multiple `.fsi` files (`Types.fsi`, `Charts.fsi`, `DataGrid.fsi`, …) into one
baseline in deterministic filename order.

## Validation (escalated serialized gate set)

`Route` escalates this change (governance/build paths). Run FAKE-backed targets **sequentially**:

1. `./fake.sh build -t Route` — confirm escalation + required-artifact list (`--enforce` to fail on a missing artifact)
2. `./fake.sh build -t Dev`
3. `./fake.sh build -t PerPackageSurfaceDiff` — new capability, zero drift at baseline (SC-004); invoked **explicitly** (no Routing rule selects it — see the implementation finding)
4. `./fake.sh build -t GeneratedGuidanceCheck`
5. `./fake.sh build -t TemplateCheck`
6. `./fake.sh build -t GeneratedProductCheck`
7. `./fake.sh build -t EvidenceGraph`
8. `./fake.sh build -t EvidenceAudit`

The existing aggregate `PackageSurfaceCheck` must remain green and unchanged (FR-011). Re-run any
race-like FAKE failure sequentially before product debugging.

## Post-Design Constitution Re-Check

After Phase 1 (data-model + contracts), no new violations: the design keeps `update`-equivalent
logic pure with I/O at the edge (Principle IV satisfied without MVU ceremony), adds exactly one
curated `.fsi` for the new module (Principle II), introduces no runtime `.fsi`/package/dependency
change (Tier-1 governance surface only; runtime untouched), and plans real failing-first evidence
with zero synthetic dependence (Principles V/VI). **PASS.**

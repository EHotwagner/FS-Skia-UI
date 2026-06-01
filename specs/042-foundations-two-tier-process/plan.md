# Implementation Plan: Foundations Two-Tier Development Process

**Branch**: `042-foundations-two-tier-process` | **Date**: 2026-06-01 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/042-foundations-two-tier-process/spec.md`

## Summary

Stage 1 of the foundations programme: stop applying consumer-grade governance ceremony to routine
framework changes. The tiers already *listed* in `validation.contract.yml` (`inner-loop`,
`focused-authority`, `agent-ready`, `maintainer-verify`, `automation-final`) become **authoritative and
enforced** through a single new entry point — **`./fake.sh build -t Route`** — that answers "what must I
run for *this* change?" with a deterministic gate list instead of prose. Routine framework-internal work
resolves to the light **inner-loop** tier (`Dev` only); consumer-contract changes (`template/**`,
`.specify/**`, governance/build-target paths, public `src/**/*.fsi`) **escalate** automatically (a
public `src/**/*.fsi` surface edit escalates rather than adding a check to inner-loop); dogfood-marked
features (this one, `042`) force the full
serialized pipeline so the harness cannot rot.

Per the maintainer's resolved decision, and because `FS.Skia.UI.Build` now exists (039/040/041), the
selector is **compiled F#** from the start — pulling Stage 5.5's `Routing.fs` forward, mirroring how 041
pulled target-typing forward. Two new curated module pairs (`Routing`, `ContractView`) and one additive
`Targets.Target` case (`Route`) implement it; the selector is a **pure** `Diff -> Selection` function
whose gate lists are typed `Targets.Target` values (a mistyped gate is a compile error). No
`select-tier.fsx`, no `dotnet fsi`, no FCS is ever introduced. `validation.contract.yml` is **retained but
generated** from `Routing.fs` and guarded by a currency check, so its existing consumers (`build.fsx`, the
`TargetMetadataDrift` reference check, `AgentReady`) keep reading a coherent file while drift becomes
structurally impossible. This is a **build-tooling and process** change only: no runtime `src/**` edit, no
product `.fsi` change, no shipped package, no build front-end migration (Stage 5).

## Technical Context

**Language/Version**: F# / .NET `net10.0` (inherits `Directory.Build.props`: `TreatWarningsAsErrors`,
`FS0078`-as-error, Central Package Management).
**Primary Dependencies**: **None new.** `Targets.Target` DU + `dispatchTargets` (041) are the foundation
the gate lists reference; `BuildPaths` glob + `BuildProcess` (git) are already present; `Fake.Core.Target`
(front-end only) unchanged. No `PackageVersion` outside `Directory.Packages.props` (FR-012/FR-014).
**Testing**: Expecto (`tests/Governance.Tests`, already references `FS.Skia.UI.Build.fsproj`) — ≥6 typed
selector cases + a currency-check case; FAKE targets in the repository's deterministic serialized order
(dogfood, FR-015), never concurrent.
**Target Platform**: Windows and Linux (build-tooling; no runtime/visual surface touched).

**Resolved unknowns** (see [research.md](./research.md)): R1 currency check folds into `TargetMetadataDrift`
(detect) + `RefreshSurfaceBaselines` (regen), keeping `Route` the only new target; R2 git union-diff is
computed at the `Route` edge so the selector is pure; R3 a total `tierRank` makes "highest tier wins" a
one-liner; R4 `--enforce` artifact presence is a pure predicate with `File.Exists` at the edge; R5 the YAML
`routing_rules` map to typed `RoutingRule` literals with `Targets.Target` gate lists (`template/**` and
`.specify/**` broadened to full consumer-contract coverage, F2); R6 the
developer-class flag defaults to `FrameworkAuthor` and the typed `dogfoodFeatureIds` includes `"042"`.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change tier (spec):** Tier 1 by Change Classification — it adds a new command-surface entry point
(`Route` target) and new build-tooling contracts, and changes the documented governance process. It
ships the full artifact chain (spec, plan, `.fsi` contracts, tests, docs) **and** runs the full serialized
evidence pipeline as a **named dogfood feature** (FR-015). No *product* public API moves.

**Principle compliance (re-checked post-Phase 1 — PASS):**
- **I (Spec→FSI→tests→impl):** Public surface drafted as `.fsi` in [contracts/](./contracts/) before any
  `.fs`; Governance.Tests exercise those signatures failing-first; `.fs` written against the stable
  signature. ✔
- **II (Visibility in `.fsi`):** Every new module (`Routing`, `ContractView`) ships a curated `.fsi`; no
  `private`/`internal`/`public` in `.fs` (FR-011). The `Targets.Route` case is added to both `.fsi` and
  `.fs`. ✔
- **III (Idiomatic simplicity):** Plain DUs + records + pure functions + a glob predicate; no SRTP,
  reflection, type providers, or non-trivial CEs. Escalation is `List.maxBy tierRank`. ✔
- **IV (MVU boundary):** The selector and renderers are **pure**; all I/O (git union-diff read,
  `File.Exists` for `--enforce`, contract read/write, printing) stays at the `build.fsx` interpreter edge
  in the existing `BuildMsg`/`BuildEffect`/`update`/`interpret` boundary. No new long-lived stateful
  workflow; the engine relocation remains Stage 5 (out of scope). ✔
- **V (Synthetic disclosure):** No synthetic evidence anticipated — selector unit inputs are real crafted
  `Diff` values exercising real routing/escalation/default-deny paths (not `[SEH]`); `Route` transcripts and
  FAKE logs are real captures. No `[S]`/`[SEH]` tasks expected. ✔
- **VI (Test evidence):** Failing-first ≥6 typed-selector cases (SC-004) + a currency-check case (SC-007),
  plus the dogfood serialized FAKE logs (FR-015). ✔
- **VII (Observability):** `--enforce` fails fast with a precise diagnostic naming the missing artifact and
  the requiring tier; the currency check fails with a "regenerate from `Routing.fs`" diagnostic; the empty/
  unknown-diff cases print a deterministic result rather than failing silently. ✔

### Repository Governance Decisions

- **Template ownership:** No `.template.config/template.json` change; the template and generated products
  are untouched. `Route` is a repo-development entry point, not a generated-product capability.
- **Dependency impact:** **No new dependency** (FR-014). No `Directory.Packages.props`, `docs/reports/
  dependencies.md`, or `DependencyReport` change. Routing needs no package beyond what 039–041 shipped.
- **Command-surface impact:** **One new FAKE target, `Route`** (typed `Targets.Target` case + derived
  metadata + dispatch wiring) — additive; no existing target's name, dependencies, outputs, or graph
  position changes (FR-004/FR-016). The **bodies** of `TargetMetadataDrift` (currency detection) and
  `RefreshSurfaceBaselines` (contract regeneration) change, in the 041 manner. The serialized six-target
  order is unchanged as the **escalated** path. FAKE-backed validation runs serially (`Dev` →
  `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`),
  never concurrent.
- **Generated project impact:** None. No package ships into any generated product (FR-014); generated
  contents, selected guidance, local skills, validation logs unchanged.
- **Evidence paths:** `specs/042-foundations-two-tier-process/readiness/` — captured `Route` output for
  inner-loop (SC-001), escalation + `.fsi` surface (SC-002), unknown-path fallback, `Route --enforce`
  pass/fail transcripts (SC-003), dogfood full-pipeline (SC-005), the currency-check reject/accept
  demonstration (SC-007), new Governance.Tests results (SC-004), and the full serialized FAKE gate logs
  under `readiness/logs/` (FR-015). `git diff src/** = empty` proof (SC-009).
- **`.fsi` / contract impact:** New **build-tooling** `.fsi` only (`Routing.fsi`, `ContractView.fsi`) +
  the `Targets.Route` case in `Targets.fsi`. No product `.fsi`, surface baseline, or sample contract
  change (FR-013); `PackageSurfaceCheck`/`FsiTranscripts` show no baseline diff. The generated
  `validation.contract.yml` view is the only "contract" artifact that changes shape, and only as a
  faithful derivation of `Routing.fs`.
- **MVU/effect boundary:** Preserved (see Principle IV). No new `Model`/`Msg`/`Effect`; the `Route` arm
  reuses existing print/`RequireFiles`/`FailWith`-style effects at the edge.
- **Synthetic evidence:** None planned. Any discovered case returns to task review (no implementation-time
  relabeling, Principle V).
- **Test evidence:** Failing-first typed-selector unit tests + currency-check test under the existing
  Governance.Tests gate, plus the dogfood serialized FAKE evidence.
- **Observability:** Actionable diagnostics for `--enforce` (names artifact + tier), the currency check
  (names regeneration command), and the empty/unknown-diff deterministic prints.
- **Deferred scope:** Stage 5 (MVU-engine relocation, dedicated build front-end / `build.fsx` retirement —
  only the `Route` target-typing touch is in scope, consistent with 041), Stage 4 (Bash/Python evidence
  port), Stage 2.2–2.5 (single-source generation of skills/constitution/skillist), Stage 6 (prose-trimming /
  contract-versioning) — all out of scope (Framework Governance Prompts §Unsupported scope).

**Gate result: PASS** (no unjustified violations; no NEEDS CLARIFICATION remaining after research).

## Project Structure

```
build/Governance/
  FS.Skia.UI.Build.fsproj      # +2 module pairs in <Compile> after TargetMetadata, before Capabilities
  Targets.fsi / Targets.fs     # EDIT — additive `Route` case (DU, name, directPrerequisites=[], allTargets)
  Routing.fsi / Routing.fs     # NEW — DeveloperClass/Tier/Diff/RoutingRule/Selection; select/selectForFeature;
                               #       innerLoopGates; tierRank; unmetArtifacts; enforceDiagnostic;
                               #       renderSelection; rules; dogfoodFeatureIds (FR-001/2/3/5/6)
  ContractView.fsi / .fs       # NEW — render (validation.contract.yml from Routing) + currencyDrift (FR-007)
  (existing modules unchanged)

build.fsx                      # EDIT — #loads Routing.fs + ContractView.fs; new `StartTarget Targets.Route`
                               #   arm (git union-diff edge + flag parse + selectForFeature + print/enforce);
                               #   TargetMetadataDrift body gains currencyDrift detection;
                               #   RefreshSurfaceBaselines body gains ContractView.render regeneration

validation.contract.yml        # REGENERATED from Routing.fs (retained; currency-checked) — FR-007

tests/Governance.Tests/
  Governance.Tests.fsproj      # +2 test files in <Compile> before Program.fs
  RoutingTests.fs              # NEW — ≥6 typed selector cases (SC-004 / FR-010)
  ContractViewTests.fs         # NEW — currencyDrift None/Some (SC-007)

CLAUDE.md / AGENTS.md          # EDIT — "run `Route` first; run only the gates it prints"; six-target order
                               #   reframed as the escalated/maintainer-verify path (FR-008)
docs/reports/build.md          # EDIT — tiers, developer-class axis, Route selection, --enforce (FR-009)
docs/reports/speckit.md        # EDIT — tiered process + Route entry point (FR-009)
tests/Governance.Tests/SequentialFakeGuidanceTests.fs  # EDIT — assert the Route-first guidance (FR-008)
```

## Phase 0 — Research

Output: [research.md](./research.md). Six engineering decisions resolved (R1 currency-check siting,
R2 pure-selector/edge-git split, R3 tier total order, R4 `--enforce` predicate, R5 YAML→typed-rule
mapping, R6 developer-class flag + typed dogfood list). No NEEDS CLARIFICATION remain.

## Phase 1 — Design & Contracts

Outputs:
- [data-model.md](./data-model.md) — `DeveloperClass`, `Tier`, `Diff`, `RoutingRule` (with the full
  rule table), `GateSet`, `Selection`, `DogfoodMarker`, the `Route` target, and `ContractView`, with
  derivation rules, the escalate-only / default-deny / compile-checked-gates / single-source / purity
  invariants, and the state-transition diagram.
- [contracts/](./contracts/) — curated `Routing.fsi` and `ContractView.fsi` (the Principle I FSI sketch,
  validated before any `.fs` exists).
- [quickstart.md](./quickstart.md) — the FSI-sketch → failing-tests → implement → wire-edge →
  generate-contract → capture-evidence recipe.
- Agent context updated: `AGENTS.md` SPECKIT marker repointed to this plan.

**Post-design Constitution re-check: PASS** (recorded above).

## Phase 2 — (planning stops here)

Task breakdown is produced by `/speckit-tasks`; not generated by this command.

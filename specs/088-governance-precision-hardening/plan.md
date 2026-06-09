# Implementation Plan: Governance Precision Hardening

**Branch**: `088-governance-precision-hardening` | **Date**: 2026-06-10 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/088-governance-precision-hardening/spec.md`

## Summary

Close three classes of gap in `FS.Skia.UI.Build` (`build/Governance/**`) where reality diverges
from the system's advertised promises, as three independently shippable tiers:

- **Tier 1 (P1) — Typed gate identity / single source.** Re-key `Front/Helpers.focusedGateContract`
  by `Targets.Target` (not `string`) so a new/renamed gate without a contract arm is a **compile
  error**; resolve the gates that currently fall through the `VerificationDegraded` wildcard to
  explicit contracts; derive `AgentValidation.knownGates` and `Verify`'s `ProductChecksRun` from a
  **routable-gate projection** of the single `Targets` enumeration rather than hand-maintained string
  literals, byte-identical to today.
- **Tier 2 (P2) — Target granularity & routing precision.** Split `GeneratedProductCheck` into a
  cheap structural sub-target (generate + structural scan + file-list evidence) and an expensive
  consumer-validation sub-target (consumer restore/build/`Verify`), keeping `GeneratedProductCheck`
  as a resolvable **umbrella** with identical evidence/verdict; add routing rules so documentation-only
  changes under `template/**` and `src/Controls/**` route to a lighter gate list, while build-infra /
  governance paths stay conservative and mixed (doc + source) changes still re-escalate.
- **Tier 3 (P3) — Governance code health.** Behavior-preserving extraction of the two near-identical
  generated-row scan functions (`scanGeneratedRow`, `scanV3GeneratedRow`) onto shared validators and
  consolidation of the paired NuGet-config templates — byte-identical evidence, no `.fsi` / contract
  change. Large module re-splits and table-driven engine dispatch are explicitly **out of scope**.

The technical approach leans entirely on the existing typed `Targets` DU + `spec`/`directPrerequisites`
single source (feature 041) and the FAKE front-end that already derives target creation and `==>` edges
from `Targets.dispatchTargets` / `Targets.targetDependencyRows` (`build/Program.fs`). Each tier is a
mechanical move toward that single source; no product runtime, layout, rendering, or package-identity
change is involved.

## Technical Context

**Language/Version**: F# / .NET (`net10.0`); build front-end is the compiled `FS.Skia.UI.Build` library + `build/Program.fs` exe
**Primary Dependencies**: FAKE (`Fake.Core.Target`), DiffPlex (surface diffs), Expecto + FsCheck (governance tests) — no new dependency
**Testing**: Expecto governance suites under `tests/Governance.Tests/**`; FAKE targets (`Route`, `TargetMetadataDrift`, the escalated six-target order); byte-identical golden/contract comparison
**Target Platform**: Windows and Linux (build tooling only; no runtime/GPU surface)

**Working surfaces (confirmed line ranges):**

- `build/Governance/Front/Helpers.fs:188-328` — `focusedGateContract model target`, a `string` match with a `_ -> VerificationDegraded` wildcard (FR-001/FR-002). `targetMetadata` (`:340`) calls it with `spec.Name`; `focusedGateSummary`/`focusedGateAssumptionCheck` (`:330-334`) are the call-site funnels.
- `build/Governance/Engine/Update.fs` — per-target `StartTarget Targets.X` arms pass bare gate-name **strings** to `focusedGateAssumptionCheck`/`focusedGateSummary` (e.g. `:238`, `:251`); `Verify`'s `ProductChecksRun` string literal at `:971`; `Ci`'s `ProductChecksRun = [ "Verify" ]` at `:986`. `GeneratedProductCheck` arm at `:236-251` emits `GenerateV3Products; ScanV3GeneratedProducts; ValidateGeneratedConsumer; RequireFiles(...)` (FR-006).
- `build/Governance/AgentValidation.fs:361-391` — `ValidationContract.knownGates` hand-maintained `string list` (FR-003); `ValidationGate = string` (`AgentValidation.fsi:4`).
- `build/Governance/Targets.fs` / `Targets.fsi` — the single-source DU + `name`/`directPrerequisites`/`spec`/`allTargets`/`dispatchTargets`/`requiredTargetNames`/`targetDependencyRows`. New additive sub-target cases land here (FR-006).
- `build/Governance/Routing.fs:129-341` — the `rules` table + `internalRule`/`internalRuleMatcher`/`select`/`selectForFeature` (union of `RequiredGates`, max-tier `tierRank`). `internalGlobToRegex` (`:53`) has no negation today (FR-008 design point).
- `build/Governance/GeneratedProduct.fs:148` (`scanGeneratedRow`) and `:1010` (`scanV3GeneratedRow`) — the two near-identical scans (FR-011); `runGenerateV3Products` (`:1003`), `runScanV3GeneratedProducts` (`:1410`), `runGeneratedConsumerValidation` interpreted in `Engine/Interpret.fs:32-34`.
- `build/Program.fs:42-49` — FAKE target creation from `Targets.dispatchTargets`; `==>` edges from `Targets.targetDependencyRows`. New target cases auto-register; no exe edit needed.

**Unknowns**: none remaining — resolved in [research.md](./research.md). (Spec's only open item, "confirm exact line ranges," is closed above.)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

This is a **Tier 2 (internal change)** under the constitution's Change Classification for Tiers 1 & 3
(behavior-preserving, no public product API surface change), and a **Tier 1 (contracted change)** only
in the narrow sense that Tier 2 of *this feature* adds new build-target identities and intentionally
updates the generated `validation.contract.yml` — both governance-internal, not product public API.
No constitution principle is violated; complexity additions (none beyond exhaustive matches and a
matcher refinement) require no Principle III justification.

### Repository Governance Decisions

- **Template ownership**: N/A — no `template/**` *source* change and no `.template.config/template.json`
  change. Tier 2 *routing* gains a doc-only relaxation for `template/**/*.md`, but the template's shipped
  contents, package policy, and command surface are untouched. No Spec Kit asset change.
- **Dependency impact**: N/A — no `Directory.Packages.props`, `docs/dependencies.md`, or template
  package-inclusion change; no new dependency. `DependencyReport` coverage unchanged.
- **Command-surface impact**: `build.fsx`/`build/Program.fs` exe body is **unchanged** (targets and `==>`
  edges are already derived from `Targets`). The set of addressable targets **grows additively**: new
  `GeneratedProductCheck` sub-targets (working names `GeneratedProductStructure`,
  `GeneratedConsumerValidation`) are added to `Targets`; `GeneratedProductCheck` stays a resolvable
  umbrella (FR-007). `TargetMetadataDrift` must be re-satisfied (regenerated `validation.contract.yml`).
  `Dev`, `Verify`, `Ci`, `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`,
  `EvidenceGraph`, `EvidenceAudit` keep their **effective** coverage. FAKE-backed commands share `.fake`
  state and MUST run sequentially; the escalated order is:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: N/A — no change to default/minimal generated contents, selected Controls
  guidance, local skills, placeholder/excluded-history scans, or generated `Dev` behavior. Tier 3
  refactors the *scanning code* that validates generated products; FR-013 mandates byte-identical
  findings, so generated-product validation behavior does not change.
- **Evidence paths**: `Route` before/after captures →
  `specs/088-governance-precision-hardening/readiness/route-before.txt` and `route-after-{doconly,source,structural}.txt`;
  six-target logs → `readiness/logs/{dev,generated-guidance,template-check,generated-product-check,evidence-graph,evidence-audit}.txt`;
  `TargetMetadataDrift` → `readiness/target-metadata.md`; `SkillSyncCheck` → `readiness/skill-sync-check.md`;
  regenerated-contract diff → `readiness/validation-contract-diff.md`; Tier 1/3 byte-identity proof →
  `readiness/behavior-preserving-baseline.md`; agent-ready verdict → `readiness/agent-ready-verdict.md`.
  (Exact filenames finalized at `/speckit-tasks`; paths above are the planned readiness contract.)
- **`.fsi` / contract impact**: **Governance-internal `.fsi` change only** — `Targets.fsi` (new sub-target
  cases + a `routableGates`/`productCheckGates` projection val), `Front/Helpers` signature (typed
  `focusedGateContract: BuildModel -> Targets.Target -> _`), possibly `AgentValidation.fsi` (derived
  `knownGates`). **No product `.fsi`, public API, sample-contract, or product surface-baseline change**
  is intended; if any per-package/aggregate surface baseline for `FS.Skia.UI.Build` itself moves, it is
  re-captured as part of the change (build-tooling surface, not product surface). `validation.contract.yml`
  is byte-identical for Tier 1/3 and intentionally updated (with diff rationale) for Tier 2.
- **MVU/effect boundary**: The build engine **is** the MVU boundary and is reused, not redesigned:
  `Model`/`Msg` (`Engine/Model.fs[i]`, `Msg.StartTarget of Targets.Target`), pure `update`
  (`Engine/Update.fs`), `Effect` DU (`GenerateV3Products`/`ScanV3GeneratedProducts`/
  `ValidateGeneratedConsumer`, …), edge interpreter (`Engine/Interpret.fs`). Tier 2's split adds new
  `StartTarget Targets.GeneratedProductStructure` / `…ConsumerValidation` **arms** that re-emit the
  **existing** effects (no new effect constructor needed); the umbrella arm composes via prerequisites.
  Pure-transition tests assert the emitted-effect lists for the new arms and the unchanged umbrella;
  interpreter evidence is the real generated-product run. `update` stays pure; I/O stays at the edge.
- **Synthetic evidence**: None planned. All evidence is real `Route`/target output and real
  generated-product scans over real on-disk generated projects. No mocks, fakes, placeholders, or canned
  responses. (If any arises during implementation, Principle V `[S]` disclosure applies and the audit
  hard-blocks; none is anticipated.)
- **Test evidence**: Failing-first governance tests — (T1) a property/unit test asserting the typed
  `focusedGateContract` resolves every routable gate to a non-degraded contract (red before the
  re-key, green after) and that derived `knownGates`/`ProductChecksRun` equal the prior literals;
  (T2) `Route` selection tests for doc-only vs. source vs. mixed diffs under `template/**` and
  `src/Controls/**`, and a structural-vs-consumer split test asserting the umbrella's composed gate
  set/evidence; (T3) byte-identical scan-findings tests over a baseline for the extracted validators.
  Plus the escalated six-target order and `TargetMetadataDrift`/`SkillSyncCheck` currency gates.
- **Observability**: `Route` keeps printing `tier`/`gates`/`matched-rules`; the new sub-targets emit
  their own focused-gate summaries and `RequireFiles` failures naming the missing artifact; a missed
  contract arm is a **compile error** (loudest possible failure) rather than a silent degrade. No new
  unsupported-environment message class; existing diagnostics preserved.
- **Deferred scope**: Larger structural re-splits of the ~2,200-line `GeneratedProduct.fs` and
  table-driven dispatch for the ~990-line engine `match` (FR-014); relaxation of governance/build-infra
  routing for doc-only edits (FR-009 keeps it conservative); any dependency-chain tightening (FR-010)
  that would change *effective* coverage — opportunistic only where provably coverage-neutral, else
  deferred. No visual/release/platform/distribution work.

**Initial Constitution Check: PASS** (no unjustified gate violations; complexity additions are an
exhaustive match and a coverage-neutral matcher refinement, both inside the existing MVU boundary).

**Post-Design Constitution Check (after Phase 1): PASS** — the contracts in
[contracts/](./contracts/) preserve every public product surface, keep `update` pure, route all new
I/O through existing effects, and the routable-gate projection reproduces the hand-maintained lists
byte-for-byte (see [data-model.md](./data-model.md) §Routable-gate projection).

## Project Structure

All changes are confined to the build-governance library and its tests (no product `src/**` runtime,
no `template/**` source, no docs site):

```
build/Governance/
  Targets.fs / Targets.fsi                 # +sub-target cases; +routableGates / productCheckGates projection (FR-003/004/006)
  Front/Helpers.fs                          # focusedGateContract re-keyed by Targets.Target; exhaustive arms (FR-001/002/005)
  AgentValidation.fs                        # knownGates derived from routable-gate projection (FR-003)
  Engine/Update.fs                          # typed gate args at call sites; new StartTarget arms for split; derived ProductChecksRun (FR-004/005/006)
  Routing.fs                                # doc-only rules + matcher refinement for template/** & src/Controls/** (FR-008/009/010)
  GeneratedProduct.fs                       # shared scan validators; behavior-preserving (FR-011/013)
  <paired NuGet-config templates>           # consolidated, behavior-preserving (FR-012)
build/Program.fs                            # unchanged (derives from Targets)
validation.contract.yml                     # byte-identical (T1/T3) / regenerated with rationale (T2)
tests/Governance.Tests/**                   # failing-first tests for T1/T2/T3 (see Test evidence)
specs/088-governance-precision-hardening/   # this plan + research/data-model/contracts/quickstart + readiness/
```

## Phase 0 — Outline & Research

See [research.md](./research.md). Resolved decisions: (R1) exhaustive typed contract with an explicit
**non-routable/internal** arm group instead of a silent wildcard; (R2) routable-gate projection as the
single source for `knownGates` and `ProductChecksRun`, with a documented filter that reproduces the
prior literals byte-identically; (R3) doc-only routing via a *matcher refinement* (a rule matches its
heavy gates only when the diff contains a non-doc path under its tree) plus a dedicated doc-only rule —
relying on the existing union/max-tier composition so mixed changes re-escalate; (R4) the
`GeneratedProductCheck` split seam (structural sub-target independent of and ordered before consumer
validation, umbrella composes both); (R5) Tier 3 extraction boundaries and the byte-identical baseline
strategy.

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md): the typed entities — `Target` (+ additive sub-targets), `TargetSpec`,
  `FocusedGateContract`, the routable-gate projection, `RoutingRule` (refined matcher), and the
  `GeneratedProductCheck` umbrella/sub-target family — with field-level rules and the byte-identity
  invariants.
- [contracts/](./contracts/): governance-internal contract deltas — `targets-fsi-delta.md` (new cases +
  projection vals), `focused-gate-contract.md` (typed signature + exhaustiveness obligation),
  `routing-rules.md` (doc-only/source classification table + composition invariants), and
  `behavior-preserving.md` (the Tier 1/3 byte-identity acceptance contract).
- [quickstart.md](./quickstart.md): the maintainer validation walkthrough — `Route` before/after for
  representative diffs, the escalated six-target order, `TargetMetadataDrift`/`SkillSyncCheck`, and the
  byte-identical `validation.contract.yml` / scan-findings checks.
- **Agent context update**: the `<!-- SPECKIT START/END -->` reference in `AGENTS.md` is repointed to
  this plan.

## Phase 2 — (planned, executed by `/speckit-tasks`)

Story-grouped, dependency-ordered tasks with `tasks.deps.yml` + `skillist` metadata. Tier 1 → Tier 2 →
Tier 3 ordering preserves independent shippability (SC-007): each tier is a self-contained, mergeable
slice that passes all routed gates without the others present.

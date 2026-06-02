# Implementation Plan: Codify Remaining Rules, Trim Prose, Version the Contract

**Branch**: `046-foundations-rule-codification` | **Date**: 2026-06-01 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/046-foundations-rule-codification/spec.md`

## Summary

Stage 6 of the foundations programme. Convert the one remaining un-codified bucket-(a)
prose rule — **Constitution-Check completeness** — into a build-failing library gate;
**version the generated-product contract** (`schema_version` + deprecation window + typed
changelog); **trim** the skill prose that build-failing gates now enforce; and add a
forward-looking **evidence-hygiene `.gitignore`** rule. The other three Stage-6.1 rules
(`[SEH]` timing, skill-id resolution, surface-baseline presence) are already enforced by
features 041/043 and are only *verified still-blocking* before their prose is deleted.

**Technical approach** (grounded in reconnaissance, see `research.md`):

1. **Constitution-Check gate** — a pure validator added to `build/Governance/Guidance.fs`,
   reusing its `markdownSections` parser and `planGuidancePrompts` boilerplate strings,
   surfaced through the **existing `GeneratedGuidanceCheck`** gate
   (`runGeneratedGuidanceScan`). The required decision-area set is a hard-coded typed list
   of 11 stable identifiers; the live `plan-template.md` is read only to detect an
   unrecognized template revision. **No new FAKE target** (A5).
2. **Versioned contract** — a new `build/Governance/GeneratedProductContract.fs(/.fsi)`
   module holding a typed `schema_version`, per-rule lifecycle state
   (`Required | Deprecated{removalVersion} | Removed`), and an embedded typed changelog;
   `GeneratedProduct.runScanV3GeneratedProducts` consults it so a `Deprecated`-only
   violation warns (naming the removal version) instead of failing. Surfaced in
   `GeneratedProductCheck` output.
3. **Prose trim** — delete code-enforced rules from `.agents/skills/**` (gate-before-prose,
   FR-008), keep rationale/intent, regenerate `.claude/skills/**` byte-identically
   (feature-044 currency check stays green), and record the line/byte deltas.
4. **`.gitignore`** — scoped forward-looking ignore for regenerable readiness logs/zips.

This change **escalates** via `Route` (governance + generated-product-contract +
`template/**`-adjacent) to the full serialized six-target set, and is run as a **dogfood**
feature. The product runtime, public `.fsi` surface, and all visual paths are untouched.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (build-tooling library `FS.Skia.UI.Build`)
**Primary Dependencies**: Existing — `Fake.Core.Target`, `Expecto`, `FsCheck`, `DiffPlex`. No new dependencies.
**Testing**: Expecto typed-result unit tests in `tests/Governance.Tests/**`; live gate runs via the serialized escalated FAKE set; `git check-ignore` evidence for `.gitignore`.
**Target Platform**: Windows and Linux (build-tooling only; no runtime/visual change).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.* — **PASS** (initial and post-design).

### Repository Governance Decisions

- **Template ownership**: No `.template.config/template.json` change. The
  **generated-product contract** (a consumer contract) gains `schema_version` + a
  deprecation window — additive, with a migration window by design (FR-004/005/006). No
  Spec Kit asset, package-policy, or command-surface identity change beyond folding a gate
  into `GeneratedGuidanceCheck`.
- **Dependency impact**: No `Directory.Packages.props` / `docs/dependencies.md` change; no
  new dependency; `DependencyReport` coverage unchanged.
- **Command-surface impact**: **No new top-level FAKE target** (A5). The Constitution-Check
  validator folds into the existing `GeneratedGuidanceCheck` gate
  (`Guidance.runGeneratedGuidanceScan`); the versioned-contract logic folds into
  `GeneratedProductCheck` (`GeneratedProduct.runScanV3GeneratedProducts`). FAKE-backed
  commands run **sequentially** in the deterministic escalated order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: Generated-product structural checks gain a version +
  deprecation window; a current generated project still validates green (SC-003). No
  change to default/minimal contents, selected-Controls guidance, or generated `Dev`
  behaviour. Generated **consumer skills** are regenerated only as a consequence of the
  prose trim (byte-identity preserved).
- **Evidence paths**: (real evidence)
  - Constitution-Check unit tests: `tests/Governance.Tests/ConstitutionCheckTests.fs`
  - Contract-versioning unit tests: `tests/Governance.Tests/GeneratedProductContractTests.fs`
  - Live gate logs: `specs/046-foundations-rule-codification/readiness/` (serialized
    six-target logs: `dev.log`, `generated-guidance-check.log`, `template-check.log`,
    `generated-product-check.log`, `evidence-graph.log`, `evidence-audit.log`)
  - Seeded-violation proofs (FR-008): `specs/046-foundations-rule-codification/readiness/seeded-violations/` (one per deleted rule)
  - Prose-delta measurement (FR-010): `specs/046-foundations-rule-codification/readiness/prose-delta.md` with reproduction commands
  - Currency check (FR-009): captured in `generated-guidance-check.log` / skill-sync output
  - `.gitignore` proof (FR-011/012): `specs/046-foundations-rule-codification/readiness/gitignore-check.md` (`git check-ignore` + tracked-control)
  - Audit verdict: `EvidenceAudit` `verdict=PASS`, zero synthetic.
- **`.fsi` / contract impact**: **No product `.fsi`** signature, public doc, surface
  baseline, or sample contract changes (SC-009). New/changed *build-tooling* `.fsi`:
  `build/Governance/Guidance.fsi` (validator surface), new
  `build/Governance/GeneratedProductContract.fsi`, possibly `GeneratedProduct.fsi` — curated
  per Principle II (build-tooling scope, not tracked runtime baselines).
- **MVU/effect boundary**: Build-side MEL `update` stays a pure `Msg × Model → Model ×
  Effect list`; all new validators are **pure functions** returning typed results, with
  file I/O confined to the `interpret`/`Front` edge (Principle IV). No product runtime,
  command, effect, subscription, or interpreter change.
- **Synthetic evidence**: **None planned.** All evidence is real (typed unit tests over
  real parsers, live gate runs, real `git check-ignore`). The seeded-violation proofs are
  deliberate real failures, not synthetic fixtures. No `[S]`/`[SEH]` tasks expected; the
  audit must return `verdict=PASS` with zero synthetic (SC-010).
- **Test evidence**: Failing-first typed Expecto tests for the Constitution-Check
  pass/fail/N-A/unrecognized-template-revision cases and the contract deprecation-window
  transitions (Required → fail; Deprecated → warn naming removal version; Deprecated past
  removal version → fail; promoted-to-required → fail) (FR-013, SC-008), plus a typed
  changelog⇄`schema_version` consistency test (a breaking `PromotedToRequired`/`RuleRemoved`
  changelog entry without a matching version bump fails — FR-006, SC-011). `tests/Governance.Tests`
  green.
- **Observability**: The Constitution-Check failure names each missing/unfilled area **and
  the file it was expected in** (FR-002); the unrecognized-template-revision case emits a
  distinct actionable diagnostic (FR-003). Contract deprecation warnings name the removal
  version (FR-005). All surfaced through existing report paths; no silent failure.
- **Deferred scope**: Stage 7 work (interim-scaffolding removal, the final before/after
  baseline report, the new-normal docs pass, the dogfood retrospective) is **out of scope**.
  No visual/screenshot, release/publishing (beyond the routine merge version bump),
  platform/distribution, or V3 modular-package work.

## Project Structure

```
specs/046-foundations-rule-codification/
├── spec.md
├── plan.md                      # this file
├── research.md                  # Phase 0
├── data-model.md                # Phase 1 — typed entities
├── quickstart.md                # Phase 1 — validation walkthrough
├── contracts/
│   ├── constitution-check.md    # Constitution-Check gate contract
│   └── generated-product-contract.md  # versioned-contract + deprecation-window contract
└── readiness/                   # evidence (gitignored logs/zips per FR-011)

build/Governance/
├── Guidance.fs / .fsi           # + Constitution-Check completeness validator (FR-001/002/003)
├── GeneratedProductContract.fs / .fsi   # NEW — schema_version, rule lifecycle, typed changelog (FR-004/005/006)
├── GeneratedProduct.fs / .fsi   # consult the versioned contract in runScanV3GeneratedProducts
├── Capabilities.fs              # surface-baseline gate — verify-only (Stage 6.1, shipped)
├── Evidence/Audit.fs            # [SEH] timing gate — verify-only (Stage 6.1, shipped)
├── Evidence/Engine.fs           # skill-id resolution gate — verify-only (Stage 6.1, shipped)
└── Front/Helpers.fs             # GeneratedGuidanceCheck dispatch (host, unchanged wiring)

tests/Governance.Tests/
├── ConstitutionCheckTests.fs            # NEW (FR-013)
├── GeneratedProductContractTests.fs     # NEW (FR-013)
└── fixtures/                            # plan.md fixtures (complete / missing-area / N-A / future-template)

.agents/skills/**                # prose trim (FR-007/008); .claude/** regenerated byte-identical (FR-009)
.gitignore                       # + scoped readiness logs/zips ignore (FR-011/012)
```

## Phase 0 — Research

Complete. See [research.md](./research.md). No NEEDS CLARIFICATION remain (spec
Clarifications + ADR 0003 resolved R1–R7: host gate, required-area source-of-truth,
unfilled detection, contract version/deprecation/changelog form, prose-trim ordering,
`.gitignore` scope, testing approach).

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md) — typed entities: `RequiredDecisionArea`,
  `ConstitutionCheckResult`, `RuleLifecycle`, `ContractSchemaVersion`, `ContractChangelogEntry`,
  `GeneratedProductContract`, `RuleOutcome`.
- [contracts/constitution-check.md](./contracts/constitution-check.md) — the gate's input
  (active feature `plan.md`), the 11 required identifiers, unfilled/N-A/unrecognized-revision
  semantics, and the build-failure behaviour.
- [contracts/generated-product-contract.md](./contracts/generated-product-contract.md) —
  `schema_version`, rule lifecycle states, deprecation-window warning→failure transition,
  and the typed changelog surfaced in `GeneratedProductCheck`.
- [quickstart.md](./quickstart.md) — the fail→fix→pass and warn→promote→fail walkthroughs,
  the prose-trim measurement commands, and the `.gitignore` check.
- **Agent context update**: `AGENTS.md` plan reference repointed to this plan.

## Phase 2 — (planned by `/speckit-tasks`, not generated here)

Tasks will be story-grouped (US1 Constitution-Check, US2 versioned contract, US3 prose
trim, US4 `.gitignore`) with `tasks.deps.yml` + `skillist` metadata, gate-before-prose
ordering enforced (FR-008: each prose-deletion task depends on its proven gate task).

# Implementation Plan: Single-Source the Duplicated Governance Corpus

**Branch**: `057-dedupe-governance-corpus` | **Date**: 2026-06-03 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/057-dedupe-governance-corpus/spec.md`

## Summary

Collapse the four structural-duplication classes in the governed corpus
(per-file contract-token carriage, per-file obligation anchors, in-file scanner
echoes, and constitution/template/fragment triple-maintenance) onto **single
canonical sources** whose per-file copies are **generated and currency-checked**,
extending the repository's existing single-source-and-generate machinery
(`RefreshSurfaceBaselines`, `ConstitutionFragments` splice, `SkillSyncCheck`,
`TargetMetadataDrift`) rather than inventing a new framework.

Technical approach (from [research.md](./research.md)):

1. **Catalogue** every duplication instance and trace it to the validator that
   requires it (FR-001) → `readiness/duplication-catalogue.md`.
2. **Generalize the splice**: a canonical `GovernedBlock` store renders
   token/obligation prose into its home files via `BEGIN/END GENERATED: gov/<id>`
   markers, **hybrid by consumer** — deleted where an in-repo scanner can read the
   canonical source, generated-and-checked where the consumer is a shipped/agent
   file (classes 1–3).
3. **Promote a placeholder-bearing constitution-principle source** to canonical;
   render `constitution.md` (placeholders substituted) and the two
   `constitution-template.md` twins (placeholders preserved), currency-checked
   (class 4) — extending `ConstitutionFragments` from first-sentence to full body.
4. **Fold the new currency check into `TargetMetadataDrift`** so every generated
   copy has a guard that fails on drift naming file + source (FR-003, SC-005), and
   preserve full 056 drift strength plus a **new** generated-copy-drift red→green
   case (FR-005, SC-004).
5. **Account honestly**: report the line delta vs 056's 6772 and the
   files-touched-per-rule-change (N→1), attributable to collapsed duplication, not
   dropped rules (FR-009, SC-002).

`build/Governance/Guidance.fs` stays the single home of the rule *set*; only the
*carriage* of tokens/phrases/principles changes. This is a Tier 2 internal
governance change that **escalates** on `Route` (governance paths touched).

## Technical Context

**Language/Version**: F# / .NET (`net10.0`), compiled `FS.Skia.UI.Build` front-end
**Primary Dependencies**: none new — Expecto, FAKE targets via the compiled
front-end, existing `build/Governance/**` modules (`Guidance.fs`,
`ConstitutionFragments.fs`, `ContractView.fs`, `SkillTreeGen.fs`, `TargetMetadata.fs`,
`Engine/{Model,Update,Interpret}.fs`)
**Testing**: Expecto unit tests in `tests/Governance.Tests/**`; FAKE governance
gates (`GeneratedGuidanceCheck`, `TargetMetadataDrift`, `SkillSyncCheck`,
`TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`);
red→green mutation transcripts as readiness evidence
**Target Platform**: Windows and Linux (governance file-scan + file-generation;
pure over the repo, no runtime/GPU surface)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Result: PASS** (initial and post-design). This is a governance-tooling and
governance-prose change with no product API, runtime, or package surface impact.
No principle is violated; no unjustified complexity is introduced (the change
*reduces* duplication and reuses the existing generation pattern, honoring
Principle III). Re-evaluated after Phase 1 design below — still PASS.

### Repository Governance Decisions

- **Template ownership**: Template-owned governance files **are** edited
  (`template/base/docs/product.md` echo removal; the constitution twins are
  template-owned). Each such change becomes a **generated** copy of a canonical
  source, so `.template.config/template.json` mappings and `TemplateDrift`
  alignment classes stay satisfied by the generation+currency machinery rather
  than by hand-sync. No new template profile or capability is added; no
  `template.json` schema change is required — confirm `TemplateCheck`/`TemplateDrift`
  green after regeneration.
- **Dependency impact**: None. No `Directory.Packages.props`, `docs/dependencies.md`,
  generated template inclusion, or `DependencyReport` change — no package is added,
  removed, or version-bumped (spec: "No version bump is implied"). N/A by design.
- **Command-surface impact**: `build.fsx`/`build/Governance/**` change:
  `RefreshSurfaceBaselines` gains the new generated artifacts (governed-block
  splice + full-body constitution render); `TargetMetadataDrift` gains the new
  currency fold; `GeneratedGuidanceCheck` re-validates the regenerated corpus;
  `SkillSyncCheck` continues to guard `.agents`→`.claude` peers. No new top-level
  FAKE target unless an artifact needs isolated failure ownership. FAKE-backed
  commands share `.fake` state — run sequentially in the deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: A generated `dotnet new fs-skia-ui` project must
  still receive correct, non-stale governance guidance (constitution + skills).
  The constitution twins and skill peers are regenerated, not hand-edited, so the
  generated product constitution/skills stay current — verified by
  `GeneratedProductCheck` and `SkillSyncCheck` (SC-007). No change to selected
  Controls guidance, validation logs, or generated `Dev` behavior.
- **Evidence paths**: All under `specs/057-dedupe-governance-corpus/readiness/` —
  `duplication-catalogue.md`, `single-source-demo.md`, `dedupe-red-green.md`,
  `silent-drift-audit.md`, `generated-consumer-currency.md`,
  `structural-reduction.md`, plus the standard escalated artifacts mirroring
  056 (`generated-guidance.md`, `target-metadata-drift.md`, `skill-sync-check.md`,
  `template-drift.md`, `prose-size-accounting.md`, `evidence-graph.md`,
  `evidence-audit.md`, `task-graph.{md,json}`, `validation-contract.md`,
  `focused-gates.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`,
  `runtime-limitations.md`, `skill-loading-evidence.md`, `logs/`, `package/`).
- **`.fsi` / contract impact**: **No product `.fsi`, public docs, surface baseline,
  or sample contract change.** The contract that changes is the *internal
  governance contract* (which files carry which tokens; how copies are generated),
  documented in `contracts/governance-generation-contract.md` and enforced by FAKE
  gates — not a public product API. New internal `build/Governance/**` modules have
  no signature files (build front-end convention). Tier 2 internal.
- **MVU/effect boundary**: The governance engine is already MVU/effect-shaped
  (`Engine/Model.fs`, pure `Engine/Update.fs`, I/O only at `Engine/Interpret.fs`).
  New generation/currency logic stays **pure** (block render + byte comparison) and
  emits `WriteFile`/regenerate effects from `update`; file I/O remains exclusively
  at the `Interpret.fs` edge. No new stateful product workflow.
- **Synthetic evidence**: None planned. All evidence is real file-scan +
  file-generation over the actual repository corpus and real FAKE gate runs. The
  red→green proofs are real mutations + `git checkout` reverts. No `[S]`/`[SEH]`
  task is anticipated; if a drift case can only be exercised by a contrived fixture
  it will carry full Principle V disclosure, but the default is real-repo evidence.
- **Test evidence**: Failing-first Expecto tests in `tests/Governance.Tests/**`:
  (a) `GovernedBlock` render + currency (new file, mirroring
  `ConstitutionFragmentsTests.fs`/`SkillSyncTests.fs`); (b) preserve every existing
  056 negative in `GuidanceValidatorTests.fs` (deleted obligation, removed token,
  reintroduced forbidden term); (c) a **new** generated-copy-drift test naming
  file + source; (d) a silent-drift-audit enumeration test (no generated artifact
  without a guard). Target-level evidence via the serialized gate order.
- **Observability**: Currency diagnostics must name the **drifted file and its
  canonical source** plus the repair command (`./fake.sh build -t
  RefreshSurfaceBaselines`), matching `SkillTreeGen.currencyDrift` /
  `ConstitutionFragments.currencyDrift`. Missing generated artifacts fail loudly via
  `RequireFiles`. Reports written to `readiness/target-metadata-drift.md`,
  `readiness/generated-guidance.md`, `readiness/skill-sync-check.md`.
- **Deferred scope**: No product features, visual-parity work, the Charts split,
  or package versioning (explicitly out of scope per spec). No new generation
  framework. If folding the new currency into `TargetMetadataDrift` proves
  unwieldy, a dedicated `GovernanceFragmentDrift` target is a bounded follow-up,
  noted but not in this feature's default scope.

## Project Structure

Source (compiled F# governance front-end — edited):

```
build/Governance/
  Guidance.fs                 # rule set unchanged; Files-list carriage updated
  ConstitutionFragments.fs    # generalized: first-sentence -> full principle body
  GovernedBlocks.fs           # NEW: canonical GovernedBlock store + render/currency
  ContractView.fs             # unchanged (Routing.fs not expected to change)
  TargetMetadata.fs           # currency fold for governed-block + constitution twins
  SkillTreeGen.fs             # unchanged (peers stay SkillSyncCheck-governed)
  Engine/Update.fs            # RefreshSurfaceBaselines + TargetMetadataDrift effects
  Engine/Interpret.fs         # I/O edge (unchanged shape)
```

Governed corpus (edited — copies become generated):

```
.specify/memory/constitution.md                              # render: substituted
.specify/templates/constitution-template.md                  # render: verbatim
.specify/presets/fsharp-opinionated/templates/constitution-template.md  # render: verbatim
.specify/templates/tasks-template.md                         # echo removed / gov-block
.specify/presets/.../templates/tasks-template.md             # echo removed / gov-block
.specify/presets/.../commands/speckit.tasks.md               # echo removed / gov-block
.agents/skills/speckit-tasks/SKILL.md                        # canonical; peer regen
.agents/skills/speckit-implement/SKILL.md                    # canonical; peer regen
.agents/skills/fs-skia-layout-evidence/SKILL.md              # echo removed / gov-block
template/base/docs/product.md                                # echo removed / gov-block
template/fragments/controls/**, src/Controls/skill/SKILL.md  # controls-token carriage
.claude/skills/**                                            # generated peers (SkillSync)
```

Tests (edited / added):

```
tests/Governance.Tests/
  GovernedBlocksTests.fs          # NEW: render + currency + drift naming
  ConstitutionFragmentsTests.fs   # extended: full-body + substitution
  GuidanceValidatorTests.fs       # preserve 056 negatives; carriage updates
  TargetMetadataTests.fs          # new currency fold
  SkillSyncTests.fs               # peers still current
```

## Phase 0 — Outline & Research

Complete. See [research.md](./research.md). All NEEDS CLARIFICATION resolved (both
spec clarifications already answer the two design forks: hybrid-by-consumer
end-state, and placeholder-bearing canonical constitution source). Decisions 1–6
cover the four duplication classes, currency placement, the new drift class,
measured (not targeted) reduction, and routing/escalation.

## Phase 1 — Design & Contracts

Complete. Artifacts:

- [data-model.md](./data-model.md) — `DuplicationInstance`, `GovernedBlock`,
  `GeneratedCopy`/`CurrencyGuard`, untouched governed types, lifecycle, validation
  rules.
- [contracts/governance-generation-contract.md](./contracts/governance-generation-contract.md)
  — C1 canonical-source, C2 generation, C3 no-silent-drift, C4 drift-strength,
  C5 generated-consumer, C6 routing invariants, each mapped to its verifying gate.
- [quickstart.md](./quickstart.md) — change-a-rule path, escalated verification
  order, red→green proof recipe, reduction measurement, readiness artifact list.
- Agent context — `AGENTS.md` SPECKIT marker updated to point at this plan.

**Post-design Constitution re-check: PASS.** No `.fsi`/surface obligation arises
(Tier 2 internal); MVU/effect boundary preserved (pure render + currency, I/O at
the interpret edge); no synthetic evidence; the design *reduces* complexity.

## Phase 2 — Next step

Run `/speckit-tasks` to break this plan into story-grouped tasks with
`tasks.deps.yml` + `skillist` metadata. Expected story order: US1 (single-source a
rule) → US2 (drift strength) → US3 (reduction accounting) → US4 (generated
consumers), with FR-001 catalogue as the foundational first task.

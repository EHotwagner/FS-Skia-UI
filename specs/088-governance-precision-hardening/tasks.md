# Tasks: Governance Precision Hardening

**Feature branch**: `088-governance-precision-hardening`
**Spec**: `specs/088-governance-precision-hardening/spec.md`
**Plan**: `specs/088-governance-precision-hardening/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]`/`[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view. `[SEH]`
is a design-time annotation, not a status — none is approved for this feature
(see Synthetic-Evidence Inventory).

## Scope & MVU note

All work is confined to the build-governance library `FS.Skia.UI.Build`
(`build/Governance/**`) and its tests (`tests/Governance.Tests/**`). No product
`src/**` runtime, `template/**` source, docs site, or package-identity change.
The **build engine is the MVU boundary** (`Engine/Model.fs[i]`, pure
`Engine/Update.fs`, `Engine/Interpret.fs`): US2 adds new
`StartTarget Targets.<Case>` arms that re-emit the **existing** effects
(`GenerateV3Products`/`ScanV3GeneratedProducts`/`ValidateGeneratedConsumer`) —
no new `Effect`/`Msg` constructor — with pure-transition tests asserting the
emitted-effect lists and unchanged-umbrella composition.

## Change classification ([T1]/[T2])

The feature is overall **internal (T2)** under the constitution. The
contract-touching US2 tasks that add build-target identities and intentionally
regenerate `validation.contract.yml` are **contracted (T1)** and annotated
`[T1]`; everything else inherits the overall T2 and is left unannotated.

## Governance risk levels

- **Small** — Tier 1/3 byte-identical change: focused validation is `Dev` +
  `TargetMetadataDrift` + the failing-first governance tests; no broad rerun.
- **Medium** — Tier 2 routing/target split: focused validation adds `Route`
  before/after captures, the `GeneratedProductCheck` umbrella/sub-target runs,
  and a regenerated-`validation.contract.yml` diff.
- **Broad** — required only when a Tier 2 change could alter effective gate
  coverage: run the escalated six-target order sequentially. Aggregate
  six-target results are recorded **non-authoritatively** in `readiness/logs/`
  with per-target verdicts; `GeneratedProductCheck` may fail locally for
  environment reasons (see `readiness/runtime-limitations.md`).

## Readiness scaffolds (authoritative command / artifact / failure class / next action)

Created in Phase 1 before implementation: `readiness/route-before.txt`,
`readiness/route-after-doconly.txt`, `readiness/route-after-source.txt`,
`readiness/route-after-structural.txt`, `readiness/logs/{dev,generated-guidance,template-check,generated-product-check,evidence-graph,evidence-audit}.txt`,
`readiness/target-metadata.md`, `readiness/skill-sync-check.md`,
`readiness/validation-contract-diff.md`, `readiness/behavior-preserving-baseline.md`,
`readiness/governance-risk-levels.md`, `readiness/runtime-limitations.md`,
`readiness/aggregate-hang-diagnostics.md`, `readiness/generated-validation-authority.md`,
and `readiness/agent-ready-verdict.md`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Create `specs/088-governance-precision-hardening/readiness/` (and `readiness/logs/`) with audit-enforced placeholder files — each naming its authoritative command, artifact path, failure class, and next action
- [X] T002 [P] [skillist: []] Record feature Tier (overall internal T2; contracted T1 only for US2 target/contract changes), affected layer (`build/Governance/**` only — no product surface), MVU applicability (build engine boundary reused, no new effect), and the real-evidence obligations in `readiness/`
- [X] T003 [P] [skillist: speckit-tasks] Author and validate `tasks.md` + `tasks.deps.yml` for this feature (DAG, skillist mirror, owns metadata)
- [X] T004 [skillist: []] Capture the pre-change `Route` baseline to `readiness/route-before.txt` and the Tier-3 pre-refactor scan-finding baseline artifacts (five generated file-lists + `GeneratedProductValidationPath`) under `readiness/behavior-preserving-baseline/`

---

## Phase 2: Foundation

- [X] T005 [skillist: []] Draft the governance-internal `.fsi` deltas: `Targets.fsi` (additive `GeneratedProductStructure`/`GeneratedConsumerValidation` cases + `routableGates`/`productCheckGates`/`isProductCheck` projection vals), `Front/Helpers` typed `focusedGateContract: BuildModel -> Targets.Target -> FocusedGateContract`, and the derived `AgentValidation.knownGates` note — no product `.fsi` touched
- [X] T006 [P] [skillist: []] Record FS.Skia.UI.Build build-tooling surface-baseline expectations (re-capture only if the build library's own per-package/aggregate baseline moves; product surface baselines stay frozen)
- [X] T007 [P] [skillist: []] Record unsupported-scope handling and non-authoritative aggregate reporting in `readiness/governance-risk-levels.md`, `readiness/runtime-limitations.md`, and `readiness/aggregate-hang-diagnostics.md`

**Checkpoint**: Foundation ready — tiers may proceed in priority order (US1 → US2 → US3), each independently shippable (SC-007).

---

## Phase 3: User Story 1 — Typed gate identity / single source (US1, Tier 1 / P1)

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US1] [skillist: fsharp-build-orchestration] Add a failing-first governance test that enumerates `Targets.routableGates` and asserts each resolves through `focusedGateContract` to a **non-`VerificationDegraded`** contract (SC-003)
- [X] T009 [P] [US1] [skillist: fsharp-build-orchestration] Add failing-first tests asserting `routableGates |> List.map name` set-equals the prior `AgentValidation.knownGates` literal, and `productCheckGates |> List.map name` equals the prior `Update.fs` `ProductChecksRun` list **byte-for-byte and in order** (SC-002)
- [X] T010 [P] [US1] [skillist: []] Document the compile-error proof for SC-001 (a throwaway `Target` case with no `focusedGateContract` arm fails to compile) and record the reverting walkthrough in `readiness/`

### Implementation

- [X] T011 [US1] [skillist: fsharp-code-generation, fsharp-graph-algorithms] Add the routable-gate projection to `Targets.fs`/`Targets.fsi`: `routableGates`, `isProductCheck`, and `productCheckGates` (Verify's prerequisites filtered) rendered in pinned registry order (FR-003/004)
- [X] T012 [US1] [skillist: []] Re-key `Front/Helpers.focusedGateContract` by `Targets.Target` with an **exhaustive, wildcard-free** match: re-key existing arms, add explicit arms for the previously-degraded gates (`ContrastCheck`, `ControlFidelityCheck`, `PerPackageSurfaceDiff`, `SkillContractPathCheck`, …), and route true non-routable targets through a named `internalTargetContract` helper reproducing the exact former wildcard value (FR-001/002)
- [X] T013 [US1] [skillist: fsharp-code-generation] Derive `AgentValidation.knownGates` from `routableGates` and `Verify`'s `ProductChecksRun` from `productCheckGates`, and convert `Update.fs` gate-name call sites (`focusedGateAssumptionCheck`/`focusedGateSummary`, `targetMetadata`) to pass the typed `Targets.<Case>` (FR-003/004/005)
- [X] T014 [US1] [skillist: fsharp-build-orchestration] Run `Dev` + `TargetMetadataDrift`; confirm `target-metadata.json` and `validation.contract.yml` byte-identical to baseline and the failing-first tests now pass; capture logs to `readiness/`

**Checkpoint**: US1 independently shippable — typed gate identity with compile-time enforcement, byte-identical contract.

---

## Phase 4: User Story 2 — Granular targets and precise routing (US2, Tier 2 / P2)

### Tests First

- [X] T015 [P] [US2] [skillist: fsharp-build-orchestration] Add failing-first pure-transition tests asserting the new `StartTarget GeneratedProductStructure` / `GeneratedConsumerValidation` arms emit the **existing** effects, and the `GeneratedProductCheck` umbrella composes both sub-targets with an identical emitted-effect/evidence/verdict set (SC-005)
- [X] T016 [P] [US2] [skillist: fsharp-build-orchestration] Add failing-first `Route` selection tests: doc-only vs. source vs. mixed diffs under `template/**` and `src/Controls/**` (doc-only resolves to the **exact pinned** `[ EvidenceGraph ]` set; mixed/source re-escalate to the full set), plus the structural-vs-consumer split classification (SC-004)

### Implementation

- [X] T017 [T1] [US2] [skillist: []] Add additive `GeneratedProductStructure` and `GeneratedConsumerValidation` cases to `Targets.fs`/`Targets.fsi` (`allTargets`, `spec`, `name`, `directPrerequisites`, `timeoutClass`/`cost`/`failureOwner`); make `GeneratedProductCheck`'s `directPrerequisites` compose both sub-targets while staying a resolvable umbrella (FR-006/007)
- [X] T018 [US2] [skillist: []] Add the new `StartTarget Targets.GeneratedProductStructure` / `…ConsumerValidation` arms in `Engine/Update.fs` that re-emit the existing `GenerateV3Products`/`ScanV3GeneratedProducts`/`ValidateGeneratedConsumer` effects + `RequireFiles`; keep `update` pure and the interpreter unchanged (FR-006)
- [X] T019 [T1] [US2] [skillist: fsharp-io-globbing] Refine `Routing.fs`: make the broad `controls-public-surface`/`generated-template` source rules match heavy gates only when the diff has a non-doc path, add `controls-docs` (`src/Controls/**/*.md`) and `template-docs` (`template/**/*.md`) rules with the **pinned** `RequiredGates = [ EvidenceGraph ]` (no heavy gates, no `Dev`), keep `build.fsx`/`scripts/build/**`/`validation.contract.yml`/`.specify/**`/`build/Governance/**` conservative (FR-008/009), and tighten only provably coverage-neutral dependency chains (FR-010)
- [X] T020 [T1] [US2] [skillist: fsharp-build-orchestration] Regenerate `validation.contract.yml`, run `TargetMetadataDrift`, and record the intentional contract diff with rationale in `readiness/validation-contract-diff.md`; capture `route-after-doconly.txt`, `route-after-source.txt`, `route-after-structural.txt`
- [X] T021 [US2] [skillist: fs-skia-template-update] Run `GeneratedProductStructure`, `GeneratedConsumerValidation`, and the `GeneratedProductCheck` umbrella; confirm the umbrella's evidence artifacts + verdict are byte-identical to the pre-split run and the structural target fails fast independently of/before consumer validation (SC-005)

**Checkpoint**: US2 independently shippable — split target behind a preserved umbrella plus doc-only routing relaxation; mixed/source changes unchanged.

---

## Phase 5: User Story 3 — Governance code health (US3, Tier 3 / P3, behavior-preserving)

### Tests First

- [X] T022 [US3] [skillist: fsharp-build-orchestration] Add failing-first byte-identical scan-findings tests for the extracted validators, comparing `scanGeneratedRow`/`scanV3GeneratedRow` output against the `readiness/behavior-preserving-baseline/` captured in T004 (SC-006/FR-013)

### Implementation

- [X] T023 [US3] [skillist: fsharp-io-globbing] Extract the shared validators (file enumeration with bin/obj/readiness filtering, forbidden-path/required-file validation) from `scanGeneratedRow` and `scanV3GeneratedRow` onto common helpers, keeping each caller's distinct row shape — no finding change (FR-011)
- [X] T024 [US3] [skillist: fsharp-code-generation] Consolidate the paired NuGet-config templates to one rendered source, behavior-preserving (FR-012)
- [X] T025 [US3] [skillist: fsharp-build-orchestration] Confirm byte-identical scan findings + governance goldens vs. the baseline and **no** `.fsi` / `validation.contract.yml` change; record the result in `readiness/behavior-preserving-baseline.md` (FR-013)

**Checkpoint**: US3 independently shippable — duplication extracted with byte-identical artifacts and no contract change.

---

## Phase 6: Integration & Evidence

- [X] T026 [skillist: fsharp-build-orchestration] Run the escalated six-target order **sequentially** (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`), recording per-target verdicts non-authoritatively to `readiness/logs/`
- [X] T027 [skillist: []] Run `SkillSyncCheck` and `TargetMetadataDrift` for currency; record clean results in `readiness/skill-sync-check.md` and `readiness/target-metadata.md`
- [X] T028 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises
- [X] T029 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (no `[S]`/diff-scan hits; no synthetic evidence planned)
- [X] T030 [skillist: []] Record the agent-ready verdict in `readiness/agent-ready-verdict.md`, including the **SC-007 independent-shippability argument** (verified as a design/ordering property, not an isolated per-tier gate run): the disjoint-file rationale (Tier 1/3 touch typed-keying/derived-lists/scan-helpers; only Tier 2 regenerates `validation.contract.yml`) plus the per-tier checkpoint evidence — US1 byte-identical contract (T014), US2 intentional contract diff + doc-only relaxation (T020/T021), US3 byte-identical artifacts (T025)

---

## Synthetic-Evidence Inventory

No synthetic evidence is planned. All evidence is real `Route`/target output and
real generated-product scans over real on-disk generated projects. No `[SEH]`
task is approved (no malformed-input/error-path obligation in this feature). If
synthetic evidence becomes necessary during implementation, Principle V `[S]`
disclosure applies and the audit hard-blocks; add the row below.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |

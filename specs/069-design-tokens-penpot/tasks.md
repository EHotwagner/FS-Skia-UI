# Tasks: Design Tokens + Penpot (DTCG → Generated F# + DesignTokenDrift)

**Feature branch**: `069-design-tokens-penpot`
**Spec**: `specs/069-design-tokens-penpot/spec.md`
**Plan**: `specs/069-design-tokens-penpot/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. It still remains `[S]` when completed
with synthetic-only malformed-input or explicit error-path evidence. This
classification is assigned **here, at task generation** (plan §"Synthetic
evidence"); implementation-time relabeling is forbidden.

## Vertical-slice rule (US phases)

A `[US*]` task may be marked `[X]` only when its change is reachable from a
user-facing entry point and that path was actually exercised — an FSI session
against the package surface, a regenerate-then-gate walkthrough, the parity
test re-rendering the gallery, or a captured transcript under `readiness/`.
Generator/core changes alone do **not** satisfy `[X]` for a `[US*]` task even
when their unit tests pass green. This feature is **not** a stateful/I/O MVU
workflow (plan Principle IV = N/A): the only effect is the build-side
`RegenerateDesignTokens` interpreted at `Engine/Interpret.fs`, mirroring
`RegenerateCatalog`; no product `Model`/`Msg`/`update` is added.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- **[T1]** — Tier 1 (contracted change); the whole feature is Tier 1, so per-task
  tier annotations are omitted (they all match the spec tier).
- **[SEH]** — design-approved synthetic error-handling task paired with
  `synthetic-error-handling-approved`

Every task line mirrors the structured `skillist` from `tasks.deps.yml` as
`[skillist: ...]` (`[skillist: []]` when none). `fs-skia-design-tokens` is the
**new** skill authored by T026 in this same branch; because it does not yet
resolve in the skill registry it is intentionally **absent** from every
`skillist` (it would fail `EvidenceGraph` skill resolution). Its guidance is
applied by hand during the relevant tasks and validated once it lands.

## Governance risk levels

- **Small (inner-loop)**: generator logic, tests, readiness prose — `Dev` only.
- **Medium**: `Theme.fs` re-expression, generated `DesignTokens.fs`,
  `Controls.fsproj` `<Compile>` insert — focused `Dev` + `DesignTokenDrift` +
  `PackageSurfaceCheck`.
- **Broad (escalated / consumer-contract)**: public `src/Controls/**/*.fsi`,
  the `DesignTokenDrift` target, the routing rule, and the new skill. `Route`
  escalates to the `controls-public-surface` gate set **plus** `DesignTokenDrift`
  and the governance/skill gates. Broad validation is required for T029–T031;
  run **only** the gates `Route` prints, FAKE-backed gates **sequentially**.
  Aggregate `Route`/multi-gate results are recorded as **non-authoritative**
  summaries in the readiness logs; the per-gate PASS lines are authoritative.

## Canonical Verification Targets

- `./fake.sh build -t Route` — print the authoritative tier + minimal gate list.
- `./fake.sh build -t Dev` — fast local verification.
- `./fake.sh build -t DesignTokenDrift` — the new currency gate (this feature).
- `./fake.sh build -t RefreshSurfaceBaselines` — single regenerate entry point
  (runs `RegenerateDesignTokens`); regenerates `validation.contract.yml` and the
  `.claude` skill peer.
- `./fake.sh build -t PackageSurfaceCheck` — additive surface review.
- `./fake.sh build -t GeneratedGuidanceCheck` / `SkillSyncCheck` /
  `SkillQualityCheck` — skill + generated-guidance governance.
- `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`.

FAKE-backed commands share `.fake` state and are **not** concurrency-safe; run
multiple FAKE targets sequentially in deterministic order.

## Success-criterion → assertion mapping

- **SC-002** (100% value parity) ← T019 (10×2 parity table, token-derived ≡
  pre-feature literal).
- **SC-003** (render parity) ← T021 (re-render controls gallery, node/visual diff = ∅).
- **SC-004** (single-edit value propagation) ← T018 value-edit walkthrough (DTCG
  value edit → `RefreshSurfaceBaselines` → generated module **and** resolved
  `Theme` field both update from one edit, no manual generated-file edit).
- **SC-005** (drift fails on stale/hand-edit, passes in sync) ← T010 drift-FAIL
  test + T018 gate transcript.
- **SC-006** (determinism) ← T010 (regenerate-twice byte-identity property).
- **SC-007** (no new Controls dependency) ← T020 (dependency-guard test).
- **SC-008** (additive-only surface) ← T024 (`PackageSurfaceCheck`) + T028 baseline.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold `specs/069-design-tokens-penpot/readiness/` with audit-discoverable placeholder files: `design-tokens.md`, `design-token-drift.md`, `theme-token-parity.md`, `package-surface-expectations.md`, plus `governance-risk-levels.md`, `runtime-limitations.md`, and the `fsi/` and `logs/` subfolders (each placeholder naming its authoritative command, artifact path, failure class, and next action)
- [X] T002 [P] [skillist: []] Record feature Tier (Tier 1 contracted), affected layer (`FS.Skia.UI.Controls` additive surface + `FS.Skia.UI.Build` generator), public-API impact (additive `DesignTokens` module only), MVU applicability (N/A — pure build transform, single `RegenerateDesignTokens` effect), and evidence obligations into `readiness/design-tokens.md`

---

## Phase 2: Foundation

- [X] T003 [skillist: []] Author the DTCG single-source document `src/Controls/design-tokens.tokens.json` — both `light` and `dark` groups, all 10 primitives, values reproducing today's `Theme.fs` literals exactly (data-model §1); include the worked `dark.danger` alias `"{light.danger}"`
- [X] T004 [P] [skillist: fsharp-code-generation] Draft curated public surface `src/Controls/DesignTokens.fsi` from `contracts/design-tokens.fsi` (`DesignTokens.Light.*` / `DesignTokens.Dark.*`; Principle II — sole public declaration, `.fs` carries no access modifiers)
- [X] T005 [P] [skillist: fsharp-code-generation, fsharp-parsing] Draft build-side generator surface `build/Governance/DesignTokenGen.fsi` from `contracts/design-token-gen.fsi` (`TokenKind`, `DesignTokenFact`, `RegionStatus`, `TokenCurrency`, `parse`/`renderValue`/`renderModule`/`splice`/`currency`/`isCurrent`/`currencyDrift`), mirroring `CatalogGen.fsi`
- [X] T006 [skillist: []] Exercise the draft `DesignTokens.fsi` from FSI against a hand-stubbed `.fs` (`DesignTokens.Light.foreground`, `= Theme.light.Foreground`) and capture the transcript to `readiness/fsi/design-tokens-surface.txt`
- [X] T007 [P] [skillist: []] Record the expected additive `FS.Skia.UI.Controls` surface delta and regenerated-baseline rationale into `readiness/package-surface-expectations.md` (new `DesignTokens` names only; `Theme`/`Control` signatures unchanged)
- [X] T008 [P] [skillist: []] Record unsupported-scope handling and loud failure diagnostics into `readiness/runtime-limitations.md`: no live Penpot/MCP, no remaining-41-controls migration, malformed/cyclic/missing DTCG fails loudly naming the offending token with no partial emit

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) — Theme primitives sourced from one DTCG document

### Tests First (Principle I, Principle VI)

- [X] T009 [P] [US1] [skillist: fsharp-build-orchestration] Failing-first contract tests committed red: assert `DesignTokenGen` exposes `parse`/`renderValue`/`renderModule`/`splice`/`currency`/`currencyDrift`, and that `DesignTokens.fsi` declares the `Light`/`Dark` token surface
- [X] T010 [P] [US1] [skillist: fsharp-build-orchestration, fsharp-code-generation] Generator semantic tests (mirror `CatalogTests` 066 block): byte-identity of `renderModule` vs. committed fixture, `currency` PASS on the committed tree, `splice` idempotency, **drift FAIL** on a hand-mutated generated file (diagnostic names the token + theme + `RefreshSurfaceBaselines`), missing/whole-file reported as all-`Missing` loudly, determinism (regenerate twice ⇒ byte-identical — SC-006), and deterministic alias resolution (`{light.danger}` ⇒ `#b91c1cff`)
- [S] T011 [P] [US1] [SEH] synthetic-error-handling-approved [skillist: fsharp-build-orchestration, fsharp-graph-algorithms] error-path tests: malformed DTCG JSON and cyclic/unresolvable alias each raise a generation failure naming the offending token and emit **no** F# (no partial module). Real input is infeasible to source for a malformed/cyclic document; these validate explicit failure behavior (plan §"Synthetic evidence")

### Implementation

- [X] T012 [US1] [skillist: fsharp-parsing, fsharp-graph-algorithms, fsharp-code-generation] Implement `build/Governance/DesignTokenGen.fs`: in-process DTCG JSON parse, deterministic alias resolution with cycle detection, `renderValue` (hex → `Colors.rgba R G B A`; dimension/number → float w/ decimal point; `fontFamily` null → `None`), `renderModule` (whole-file w/ GENERATED banner + regenerate command), `splice`, `currency`/`isCurrent`/`currencyDrift` — pure over in-memory text
- [X] T013 [US1] [skillist: fsharp-code-generation] Generate `src/Controls/DesignTokens.fs` from the DTCG source via the generator and insert its `<Compile>` (`.fsi` then `.fs`) after `Theme` in `src/Controls/Controls.fsproj`, adding **no** new package reference
- [X] T014 [US1] [skillist: fsharp-build-orchestration] Add the `DesignTokenDrift` target to `build/Governance/Targets.fs`/`Targets.fsi` (`Target` enum, `allTargets`, name map, `directPrerequisites`, `failureOwner`), mirroring `ControlsCatalogGenerationCheck`
- [X] T015 [US1] [skillist: fsharp-build-orchestration] Add the `RegenerateDesignTokens` model effect (`Engine/Model.fs`/`Model.fsi` next to `RegenerateCatalog`), dispatch it in `Engine/Interpret.fs`, and implement `regenerateDesignTokens` in `Front/Governance.fs` (mirrors `regenerateCatalog`; write is the only filesystem effect, at the interpreter edge)
- [X] T016 [US1] [skillist: fsharp-build-orchestration] Splice `RegenerateDesignTokens` into `RefreshSurfaceBaselines` and add the `DesignTokenDrift` arm in `Engine/Update.fs` so the DTCG document is the one edit point
- [X] T017 [US1] [skillist: fsharp-build-orchestration] Add the routing rule (`Targets.DesignTokenDrift` into the `controls-public-surface` gate list in `build/Governance/Routing.fs`) and regenerate `validation.contract.yml` from `Routing.fs` (no hand-sync)
- [X] T018 [US1] [skillist: fsharp-build-orchestration] Capture the `DesignTokenDrift` gate report to `readiness/design-token-drift.md` — currency PASS on the committed tree plus a hand-edit/stale FAIL transcript (under `readiness/logs/`) showing the named token + regenerate command. Also capture the **SC-004 value-edit propagation walkthrough** (US1 independent test): edit one DTCG token value → `./fake.sh build -t RefreshSurfaceBaselines` → show the generated `DesignTokens.*` value **and** the resolved `Theme.<field>` both updated from that **single** edit with no manual edit to the generated module, then revert the value

**Checkpoint**: User Story 1 — single-source pipeline + drift gate functional and independently testable.

---

## Phase 4: User Story 2 (US2) — Rendering behavior is unchanged

### Tests First

- [X] T019 [P] [US2] [skillist: fs-skia-ui-widgets, fs-skia-scene] Add the 10-field × 2-theme value-parity test (SC-002): each `Theme.light/dark.<Field>` equals its pre-feature literal from the frozen data-model §4 table; assert `DesignTokens.Light/Dark.*` resolve byte-identically
- [X] T020 [P] [US2] [skillist: fsharp-build-orchestration] Add the dependency-guard test (SC-007): `Controls.fsproj` gains **no** new package reference (in particular no `Fable.Elmish` and no JSON dependency), mirroring the `068` guard
- [X] T021 [P] [US2] [skillist: fs-skia-ui-widgets, fs-skia-scene] Add the render-parity check (SC-003): re-render the controls gallery against the token-derived themes and assert node/visual output is identical to the pre-feature themes

### Implementation

- [X] T022 [US2] [skillist: fs-skia-ui-widgets] Re-express `Theme.light` and `Theme.dark` in `src/Controls/Theme.fs` in terms of `DesignTokens.Light.*`/`DesignTokens.Dark.*` — value-identical, **zero** inline color/size/density/radius/contrast literals for the migrated fields; `Name` stays a code constant (`Types.fsi` signatures unchanged)
- [X] T023 [US2] [skillist: []] Record `readiness/theme-token-parity.md`: the 20-cell parity table (token-derived ≡ pre-feature literal) and the render-parity result

**Checkpoint**: User Story 2 — behavior-preservation proven; consumers recompile with no source edit.

---

## Phase 5: User Story 3 (US3) — Typed token surface for direct authoring

### Tests First

- [X] T024 [P] [US3] [skillist: fs-skia-ui-widgets] Add the consumer-reference test: a view/variant references a generated token by typed name (e.g. `DesignTokens.Light.accent`), compiles, and resolves to the DTCG value; assert `PackageSurfaceCheck`/`PerPackageSurfaceDiff` reports the `FS.Skia.UI.Controls` delta as **additive-only** (SC-008)

### Implementation

- [X] T025 [US3] [skillist: fs-skia-ui-widgets] Finalize the curated `src/Controls/DesignTokens.fsi`, add a small sample/FSI snippet demonstrating token-first authoring against a named token, and complete `readiness/package-surface-expectations.md` with the realized additive delta

**Checkpoint**: User Story 3 — typed token surface greppable and directly authorable.

---

## Phase 6: Skill, Integration & Polish

- [X] T026 [skillist: []] Author the new `fs-skia-design-tokens` capability skill at `.agents/skills/fs-skia-design-tokens/SKILL.md` (canonical source) — the DTCG → generated-F# flow, the `DesignTokenDrift` gate, and the tokens-first authoring flow (plan §16.4 / FR-010)
- [X] T027 [skillist: fsharp-build-orchestration] Run `./fake.sh build -t RefreshSurfaceBaselines` to regenerate the `.claude/skills/fs-skia-design-tokens/**` peer (and `validation.contract.yml`) from the canonical `.agents` tree, then confirm `SkillSyncCheck`/`SkillQualityCheck`/`GeneratedGuidanceCheck` pass
- [X] T028 [skillist: fsharp-build-orchestration] Refresh the surface baselines: regenerate the per-package snapshot `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt` via `PerPackageSurface.captureCurrent` (not produced by `RefreshSurfaceBaselines`) and the aggregate `readiness/surface-baselines/FS.Skia.UI.Controls.txt`; confirm the diff is additive-only
- [X] T029 [skillist: fsharp-build-orchestration] Run `./fake.sh build -t Route` over the branch diff, confirm it prints the escalated `controls-public-surface` set **including** `DesignTokenDrift`, then run **only** the printed gates sequentially (`Dev`, `DesignTokenDrift`, the public-surface gates, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`); record the non-authoritative aggregate summary plus authoritative per-gate PASS lines in `readiness/logs/`
- [X] T030 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm `feature-directory=specs/069-design-tokens-penpot`, no cycles, no dangling refs, every `skillist` resolves, and no `[S*]` surprises; refresh `readiness/task-graph.md`
- [X] T031 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with the only `[S]` being the approved `[SEH]` T011 row; no undisclosed synthetic propagation

---

## Synthetic-Evidence Inventory

List every `[S]`/`[SEH]` task here with its Principle V disclosures.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T011 | Malformed/cyclic DTCG documents cannot be sourced as real, representative input; the test validates explicit generation-failure behavior | readiness/logs/ drift/error-path transcript | n/a | synthetic-error-handling-approved | plan Synthetic evidence; spec Edge Cases (DTCG references/aliases, malformed/incomplete source); FR-006 | Malformed JSON document plus cyclic/unresolvable alias a-to-b-to-a | Generation raises a failure naming the offending token; emits no F# (no partial module) | accepted-seh |

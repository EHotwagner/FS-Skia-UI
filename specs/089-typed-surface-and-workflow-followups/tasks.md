# Tasks: Typed Front-Door Discoverability & Spec-Kit Workflow Followups

**Feature branch**: `089-typed-surface-and-workflow-followups`
**Spec**: `specs/089-typed-surface-and-workflow-followups/spec.md`
**Plan**: `specs/089-typed-surface-and-workflow-followups/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed by the evidence audit, never hand-written. None is
expected here — this feature plans **no** synthetic evidence (plan §Synthetic evidence).

`[SEH]` (design-approved synthetic error-handling) is not used in this feature; no
malformed-input / forced-error-path task applies to a governance/docs change.

## Vertical-slice rule (US phases)

A `[US*]` task is `[X]` only when the change is reachable from a user-facing entry
point and that path was actually exercised. For this feature the "user-facing
surface" per story is concrete and non-runtime:
- **US1** — the *published* `docs/api-surface/Controls/` tree + `catalog.yml` a
  consumer reads (proven by authoring a real typed `Props`+`view` from it, no DLL
  reflection), plus the currency gate failing on drift.
- **US2/US4** — the durable skill guidance present in **both** the `.agents` source
  and the regenerated `.claude` mirror (`SkillSyncCheck` byte-identity).
- **US3** — the `EvidenceGraph` gate output (`readiness/task-graph.md`) showing the
  resolution echo.

Principle IV (MVU/effect boundary) is **N/A**: no `Model`/`Msg`/`Effect`, no I/O —
all generation is pure render/splice/currency at the `build/Governance` edge.

## Success-criterion → assertion mapping

- **SC-001/SC-002** → T007 (CatalogGen `TypedModule` render + drift-fail + E1⟂E2
  cross-check) and T006 (ApiSurfaceGen/capabilities currency over the 14 typed `.fsi`).
- **SC-003** → T012 (failing-first guidance expectation) + T015 (gate present in both trees).
- **SC-004** → T016 (`skillistResolution` unit test) + T019 (gate output echo).
- **SC-005** → T020/T022 (clarify pre-check present + graceful no-op).
- **SC-006** → T023/T024/T025 (serialized order, graph, audit).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US4]** — user-story scope
- **[T1]** — Tier 1 (published-contract) change; the whole feature is Tier 1 /
  `maintainer-verify`, so per-task `[T1]`/`[T2]` is omitted (matches the spec tier).

FAKE-backed commands (`./fake.sh`) share `.fake` state and are **not** concurrency-safe.
Non-FAKE file edits/reads may be parallel; multiple FAKE targets run sequentially in the
deterministic order. Governance risk level for this change is **broad** (consumer-contract
+ governance-code + skill-tree): the focused validation is the serialized six-target order
(`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
`EvidenceGraph` → `EvidenceAudit`); aggregate target results are recorded as
**non-authoritative** when they fail only on the known-environment paths
(`GeneratedProductCheck` cannot resolve a feature locally — see readiness).

## Readiness scaffolds

T003 names each audit-discoverable readiness file before implementation:
`readiness/governance-risk-levels.md`, `readiness/generated-guidance-validation.md`,
`readiness/runtime-limitations.md`, `readiness/aggregate-hang-diagnostics.md`,
`readiness/visual-evidence-honesty.md`, `readiness/window-visibility.md`,
`readiness/real-image-evidence.md`, `readiness/evidence-graph.md`, and
`readiness/evidence-audit.md`. Each records the authoritative command, artifact path,
failure class, and next action. The visual/window/real-image files are **N/A for runtime**
here (no Skia/Vulkan/host change) and say so explicitly rather than claiming evidence.

Template source: `.specify/presets/fsharp-opinionated/templates/tasks-template.md`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory links spec + plan and that `./fake.sh build -t Route` escalates this change to `maintainer-verify` (Tier 1, published-contract)
- [X] T002 [P] [skillist: []] Record feature Tier 1 / published-contract layer, additive public-API impact, Principle IV (MVU) N/A, and the per-story evidence obligations
- [X] T003 [P] [skillist: []] Scaffold `readiness/` with the audit-discoverable placeholder files (authoritative command, artifact path, failure class, next action each); mark visual/window/real-image files N/A-for-runtime

---

## Phase 2: Foundation

- [X] T004 [P] [skillist: []] Record governance risk level **broad**, the focused serialized six-target validation, and how non-authoritative aggregate results (e.g. local `GeneratedProductCheck` feature-resolution failure) are reported
- [X] T005 [P] [skillist: []] Record unsupported-scope handling and currency-gate failure diagnostics (the `RefreshSurfaceBaselines` remedy named by `ApiSurfaceGen.currency` / `CatalogGen.currency` / `SkillSyncCheck`)

**Checkpoint**: Foundation ready — the four stories may proceed in parallel.

---

## Phase 3: User Story 1 — Published typed front-door surface (TYPED-SURFACE-1) (US1, P1)

### Tests First (Principle I, Principle VI)

- [X] T006 [P] [US1] [skillist: fsharp-code-generation] Failing-first governance test: capabilities/`ApiSurfaceGen` currency requires the 14 typed `src/Controls/Widgets/*.fsi` to emit byte-identically into `docs/api-surface/Controls/` (and the `template/base` mirror)
- [X] T007 [P] [US1] [skillist: fsharp-code-generation] Failing-first governance test: `CatalogGen` renders the `TypedModule` token into `catalog.yml`/`Catalog.fs`, currency fails on drift, and the E1⟂E2 cross-check holds (every `TypedModule` names a module declared in an enrolled `.fsi`) **and the coverage is total** — every one of the 52 `catalog.yml` control ids maps to a typed module that exposes a `view`, with the single bridge-typed `custom-control` (no `Props`/`view`) explicitly excepted — so the SC-001 "100%" claim is mechanically asserted, not spot-checked (SC-001, SC-002, FR-004)

### Implementation

- [X] T008 [US1] [skillist: fsharp-code-generation] Add the `TypedModule` field to `TypedCatalogFact` (`build/Governance/CatalogGen.fs` + `.fsi`), populate it per control in `catalogFacts`, and render it via `renderYamlRow` (and `renderFSharpRow`)
- [X] T009 [P] [US1] [skillist: []] Enroll the 14 `src/Controls/Widgets/*.fsi` rows into the `template/capabilities.yml` Controls `contracts:` (additive — keep the 14 legacy builder `.fsi`)
- [X] T010 [US1] [skillist: fs-skia-template-update] Regenerate the api-surface tree + `catalog.yml`/`Catalog.fs` via `RefreshSurfaceBaselines`; recapture the per-package `FS.Skia.UI.Controls.fsi.txt` and the emitted `template/base/docs/api-surface/Controls/` baselines
- [X] T011 [US1] [skillist: fs-skia-typed-controls] From the published surface alone (no reflection/decompilation), author a correct typed `Props` value + `view` call for three stateful `CollectionModel`/`TextInputModel`-backed controls; confirm whole-catalog coverage by relying on the T007 total-coverage cross-check (all 52 control ids → a typed module exposing `view`, `custom-control` excepted) rather than per-control spot checks, with the legacy `.fsi` still present (SC-001, SC-002, FR-003, FR-004)

**Checkpoint**: US1 — a consumer recovers typed `Props`/`view` from published docs without a DLL probe.

---

## Phase 4: User Story 2 — Verify-during-implement discipline (VERIFY-IMPL-1) (US2, P1)

### Tests First

- [X] T012 [P] [US2] [skillist: speckit-implement] Failing-first expectation (`GeneratedGuidanceCheck` / guidance review): `speckit-implement` is missing the interactive-UI run-and-use gate text — record the red state before editing

### Implementation

- [X] T013 [US2] [skillist: speckit-implement] Add the interactive-UI run-and-use gate to `.agents/skills/speckit-implement/SKILL.md` (after per-task Workflow step 6, before the status-write step): launch + interact via the `run`/`verify` skills, confirm the evidence exercised the **production render path** stated generically (the real user-reachable surface the feature drives — cite `controlsExampleView` → `Control.renderTree` only as an example, never as the rule, so the gate binds every future interactive-UI feature per FR-007), no-op for non-interactive stories, precondition of `[X]` on `[US*]`
- [X] T014 [US2] [skillist: fs-skia-template-update] Regenerate the `.claude/skills/speckit-implement/SKILL.md` mirror via `RefreshSurfaceBaselines`; confirm `SkillSyncCheck` byte-identity
- [X] T015 [US2] [skillist: speckit-implement] Confirm the run-and-use gate is present in **both** the `.agents` source and the `.claude` mirror, and that an interactive `[US*]` cannot be `[X]` without the recorded run-and-use step on the production path (SC-003)

**Checkpoint**: US2 — interactive-UI stories require a recorded run-and-use step before completion.

---

## Phase 5: User Story 3 — EvidenceGraph skillist echo (EVGRAPH-ECHO-1) (US3, P2)

### Tests First

- [X] T016 [P] [US3] [skillist: fsharp-parsing] Failing-first unit test for the pure `skillistResolution: SkillRegistry -> string list -> string` helper: resolved `id → path`, and alias / ambiguous / unresolved tokens each flagged distinctly (matching `Audit.fs` semantics)

### Implementation

- [X] T017 [US3] [skillist: fsharp-code-generation] Implement `skillistResolution` in `build/Governance/Evidence/Render.fs` (+ `.fsi`) and append the resolved section and the separate flagged section to `taskGraphMd`
- [X] T018 [US3] [skillist: []] Pass the existing `SkillRegistry` (already carried in `EvidenceInputs`, built in `Front/Governance.fs`) into the `taskGraphMd` call via `Engine` — reuse the registry already present, no parallel resolver
- [X] T019 [US3] [skillist: speckit-evidence-graph] Run `EvidenceGraph`; confirm `readiness/task-graph.md` shows the per-token `id → SKILL.md path` echo plus the distinct flagged section, agreeing with the `Audit` validator (SC-004)

**Checkpoint**: US3 — the gate output makes every skillist token's resolution visible.

---

## Phase 6: User Story 4 — Clarify source-spec pre-check (CLARIFY-SOURCE-1) (US4, P3)

### Implementation

- [X] T020 [P] [US4] [skillist: speckit-clarify] Add the `source-spec.md` pre-check step to `.agents/skills/speckit-clarify/SKILL.md` (after step 1): when a `source-spec.md` snapshot exists in `FEATURE_DIR`, consult it before forming questions; silent no-op when absent
- [X] T021 [US4] [skillist: fs-skia-template-update] Regenerate the `.claude/skills/speckit-clarify/SKILL.md` mirror via `RefreshSurfaceBaselines`; confirm `SkillSyncCheck` byte-identity
- [X] T022 [US4] [skillist: speckit-clarify] Confirm the pre-check step is present in **both** trees and degrades gracefully (no-op) when no `source-spec.md` is present (SC-005)

**Checkpoint**: US4 — clarify consults the snapshot before asking.

---

## Phase 7: Integration & Polish

- [X] T023 [skillist: fsharp-build-orchestration] Run the serialized order `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` (sequential, FAKE-backed); record non-authoritative aggregate results for any known-environment-only failure
- [X] T024 [skillist: speckit-evidence-graph] Run `EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises, and the skillist resolution echo is present (SC-006)
- [X] T025 [skillist: speckit-evidence-audit] Run `EvidenceAudit` — confirm verdict PASS (no `[S]`/`[S*]`, no diff-scan hits); no `--accept-synthetic` override expected (SC-006)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This feature plans none.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none planned)_ | | | | | | | | |

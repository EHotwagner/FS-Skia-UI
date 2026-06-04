# Tasks: Asteroids-Demo Consumer Friction Follow-ups & Template-Update Skill Currency

**Feature branch**: `060-asteroids-consumer-friction-followups`
**Spec**: `specs/060-asteroids-consumer-friction-followups/spec.md`
**Plan**: `specs/060-asteroids-consumer-friction-followups/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view. No `[SEH]`
synthetic error-handling tasks are approved for this feature — all FR-001/FR-003/
FR-005 evidence is real generated-project output (plan §Synthetic evidence: none).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US5]** — user-story scope
- **[T1]** — Tier-1 (consumer-contract) change; this whole feature is Tier-1
- Every task line mirrors its structured `skillist` as `[skillist: ...]`;
  `[skillist: []]` means no capability skill applies.

## Governance Risk Levels

- **Small**: a single skill-text or readiness-doc edit → focused validation is the
  one owning currency gate (`SkillSyncCheck` / `SkillQualityCheck`).
- **Medium**: a new governance check or generated-output change → focused validation
  is that gate plus `GeneratedProductCheck` / `TemplateCheck`.
- **Broad**: the routed `maintainer-verify` set (D10) → required before merge; run the
  six FAKE-backed targets **sequentially** (shared `.fake` state). Aggregate/headless
  results are recorded as non-authoritative in `readiness/aggregate-hang-diagnostics.md`;
  the authoritative merge verdict is `EvidenceAudit` `verdict=PASS`.

## Canonical Verification Targets

Run only the gates `./fake.sh build -t Route` prints (expected escalated set: `Dev`,
`GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, the new
api-surface / `SkillContractPathCheck` / `TemplateUpdateSkillPackageCheck` gates,
`SkillSyncCheck`, `TargetMetadataDrift`, `SkillQualityCheck`, `EvidenceGraph`,
`EvidenceAudit`). FAKE-backed targets share `.fake` state — run multiple sequentially:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

Template source: `.specify/presets/fsharp-opinionated/templates/tasks-template.md`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold the feature directory, link spec + plan, and repoint the `AGENTS.md` SPECKIT marker at this plan
- [X] T002 [P] [skillist: []] Create `readiness/` scaffolding with audit-enforced placeholder files discoverable before implementation: `target-metadata.md`, `agent-ready-verdict.md`, `skill-loading-evidence.md`, `aggregate-hang-diagnostics.md`, `governance-risk-levels.md`, `runtime-limitations.md`, `skill-quality-check.md`, `generated-project/{feature-resolution,api-surface,test-split}.log`, `template/{template-pack.log,template-package-contents.md}`
- [X] T003 [P] [skillist: []] Run `./fake.sh build -t Route` and capture the escalated `maintainer-verify` tier and authoritative gate list for this change
- [X] T004 [skillist: []] Record Tier-1 classification, affected layers (template / governance / skills), public-API impact (none — signatures unchanged), Elmish/MVU applicability (N/A — no stateful workflow), and the evidence obligations (FR-001/FR-003/FR-005 real generated-project logs)

---

## Phase 2: Foundation

- [X] T005 [skillist: []] Draft the `.fsi` signatures and module skeletons for the three new governance modules in `FS.Skia.UI.Build` (`ApiSurfaceGen`, `SkillContractPath`, `TemplateUpdatePackage`) over the entities in `data-model.md`
- [X] T006 [skillist: []] Wire the three new gates into `build/Governance/Routing.fs`, regenerate `validation.contract.yml`, and confirm `TargetMetadataDrift` currency for the routed globs
- [X] T007 [P] [skillist: []] Author the readiness contract docs (`governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`) naming the authoritative command, artifact path, failure class, and next action for each

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 — Trustworthy merge gate in generated projects (US1, P1)

### Tests First (Principle I, Principle VI)

- [X] T008 [US1] [skillist: fs-skia-template-update] Confirm/extend `GeneratedProjectValidationTests` asserting the generated `build.fsx` `resolveFeatureDir` echoes `feature-directory=`/`tasks=` for a multi-task feature and **fails loudly** for a missing `SPECKIT_FEATURE_DIR` / empty `feature.json`

### Implementation

- [X] T009 [US1] [skillist: fs-skia-template-update] Ensure the `0.1.63-preview.1` `FS.Skia.UI.*` packages are packed to the local feed; **bump the *template* package version only** (`.template.package/FS.Skia.UI.Template.fsproj`), run `TemplatePack`, and install so a freshly generated project carries the merged `0.1.63-preview.1` resolver (FR-002); capture `readiness/template/template-pack.log` + `template-package-contents.md`
- [X] T010 [US1] [skillist: fs-skia-template-update] Generate a project, run `EvidenceGraph`, and capture the echoed `feature-directory=`/`tasks=` and the loud-failure path into `readiness/generated-project/feature-resolution.log` (FR-001 proof, SC-001)

**Checkpoint**: US1 — generated projects audit the active feature and fail loudly; proven by log.

---

## Phase 4: User Story 2 — Authoritative API surface in generated projects (US2, P1)

### Tests First

- [X] T011 [P] [US2] [skillist: fsharp-io-globbing] Failing-first test: the api-surface currency generator emits `docs/api-surface/<Pkg>/<file>.fsi` **byte-identical** to each `capabilities.yml` `contracts:` source `.fsi`, and drift fails the currency gate
- [X] T012 [P] [US2] [skillist: fsharp-parsing] Failing-first test: `SkillContractPathCheck` fails when a capability/product skill names a `docs/api-surface/...fsi` path absent from the emitted tree, on an orphan emitted file no skill claims, and on a "no DLL reflection needed" claim against an absent path (FR-004)

### Implementation

- [X] T013 [US2] [skillist: fsharp-code-generation, fsharp-parsing] Implement `ApiSurfaceGen`: generate the `template/base/docs/api-surface/` tree single-source from `capabilities.yml` `contracts:`, regenerated via `RefreshSurfaceBaselines` and currency-enforced (FR-003)
- [X] T014 [US2] [skillist: fsharp-io-globbing] Implement `SkillContractPathCheck` and fold it into `GeneratedProductCheck`/`TemplateCheck` with diagnostics naming the skill and the missing/extra path (FR-004)
- [X] T015 [US2] [skillist: []] Update `.template.config/.../template.json` to include the emitted `docs/api-surface/**` content
- [X] T016 [US2] [skillist: fs-skia-template-update] Generate a project and prove each product-skill's named `docs/api-surface/<Pkg>/<Pkg>.fsi` exists and is byte-identical to source into `readiness/generated-project/api-surface.log` (SC-002)

**Checkpoint**: US2 — every skill-claimed contract path exists in a generated project; drift is a red build.

---

## Phase 5: User Story 3 — Generated tests separate governance from behavior (US3, P2)

### Tests First

- [X] T017 [US3] [skillist: fsharp-io-globbing] Update the generated-product source-structure assertions (`TemplateCheck`/`GeneratedProductCheck`) to require `GovernanceTests.fs` + `BehaviorTests.fs` in `Product.Tests` (failing-first against the single `Tests.fs`)

### Implementation

- [X] T018 [US3] [skillist: fs-skia-testing] Split `template/base/tests/Product.Tests/Tests.fs` into durable `GovernanceTests.fs` (model-agnostic source/structure/visual-evidence scans) and replaceable `BehaviorTests.fs`; update `Product.Tests.fsproj` compile order and `template.json` (FR-005)
- [X] T019 [US3] [skillist: fs-skia-template-update] In a generated project, swap the scaffold model and prove `GovernanceTests.fs` still compiles/runs while only `BehaviorTests.fs` needs rewriting into `readiness/generated-project/test-split.log` (SC-003)

**Checkpoint**: US3 — governance scans survive a model swap; only behavior tests are replaceable.

---

## Phase 6: User Story 4 — Capability skills match the real host contract, with pitfalls (US4, P2)

- [X] T020 [US4] [skillist: fs-skia-keyboard-input] Rewrite the `fs-skia-keyboard-input` skill (canonical `.agents`/template source) to show only the `mapKey : ViewerKey -> bool -> Msg option` boundary the `app` host threads, removing the `Keyboard.init bindings` / `KeyboardEffect` reducer flow as the consumer path (FR-006, SC-004); and add a "Common pitfalls" note covering duplicate DU case names across co-opened modules (`ViewerKey.Unknown` vs `ViewerRunBlockedStage.Unknown`) with the fully-qualified resolution, so the keyboard skill carries its half of the pitfall coverage (FR-007, SC-005)
- [X] T021 [P] [US4] [skillist: fs-skia-scene] Add a "Common pitfalls" section to `fs-skia-scene`: consumer geometry records (`Vec2`) colliding with framework `Point`/`Rect`, with the conversion note (the keyboard DU-case pitfall is owned by T020) (FR-007, SC-005)
- [X] T022 [P] [US4] [skillist: fs-skia-layout-readability] Document the intended HUD/gameplay-region pattern in `fs-skia-layout-readability` (reserve a HUD band; confine/clamp gameplay bounds to the gameplay region; overdraw the HUD) (FR-008, SC-005)

**Checkpoint**: US4 — capability skills compile verbatim against the real host and call out the sharp edges.

---

## Phase 7: User Story 5 — Template-update skill that cannot drift on package set (US5, P2)

### Tests First

- [X] T023 [US5] [skillist: fsharp-io-globbing] Failing-first test: `TemplateUpdateSkillPackageCheck` diffs the `fs-skia-template-update` enumerated package IDs against the packable `.fsproj` set (11 projects) and fails on any phantom or missing package (SC-006)

### Implementation

- [X] T024 [US5] [skillist: fs-skia-template-update] Correct the `fs-skia-template-update` skill (canonical `.agents`): remove the phantom bare-Lib `FS.Skia.UI` feed check, add `FS.Skia.UI.SkillSupport` and `FS.Skia.UI.Input` to the step-5 feed loop, and fix the "nine repo packages" count (FR-009)
- [X] T025 [US5] [skillist: fsharp-io-globbing] Implement `TemplateUpdateSkillPackageCheck` distinguishing the feed-loop enumeration (all packable, incl. non-pinned `Input`) from the props-pin enumeration (FR-009, SC-006)

**Checkpoint**: US5 — the template-update skill's package set exactly equals the packable set; drift is a red build.

---

## Phase 8: Integration & Polish

- [X] T026 [P] [skillist: []] FR-011: add an interacting/conflicting-requirement note to the spec-authoring guidance (entity-count bound vs. per-wave escalation — "count may cap; difficulty continues via speed") — authoring guidance, not a new gate
- [X] T027 [P] [skillist: []] FR-010: add SC→assertion mapping guidance to the tasks-authoring template, with the split governance test as the worked example of an enforcing assertion — authoring guidance, not a new gate
- [X] T028 [skillist: fs-skia-template-update] Run `./fake.sh build -t RefreshSurfaceBaselines` to regenerate the `.claude` tree from `.agents` and the api-surface tree; confirm `SkillSyncCheck` / `TargetMetadataDrift` / `SkillQualityCheck` green; capture `readiness/skill-quality-check.md` (FR-012)
- [X] T029 [skillist: []] Run the routed FAKE-backed gates sequentially (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`) and record any non-authoritative aggregate/headless results in `readiness/aggregate-hang-diagnostics.md`
- [X] T030 [skillist: []] Fill T002's scaffolded maintainer-verify readiness artifacts: `target-metadata.md`, `agent-ready-verdict.md`, and `skill-loading-evidence.md`. This only *aggregates* the pre-task skill loads that each skilled task (T008–T025) recorded with ISO-8601 timestamps **before** its code changes began; it does not originate them
- [X] T031 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises, and the echoed `feature-directory`/`tasks` match this feature
- [X] T032 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm `verdict=PASS` for `specs/060-asteroids-consumer-friction-followups` (SC-007)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section. None are planned —
all evidence is real generated-project output (plan §Synthetic evidence: none).

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |

# Tasks: Implement-Phase Feedback Hook Parity

**Feature branch**: `077-implement-feedback-hook-parity`
**Spec**: `specs/077-implement-feedback-hook-parity/spec.md`
**Plan**: `specs/077-implement-feedback-hook-parity/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. It still remains `[S]` when
completed with synthetic-only malformed-input or explicit error-path evidence.
The classification is assigned here (task generation); implementation-time
relabeling is forbidden.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- **[T1]** — Tier 1 (contracted) change; this feature is Tier 1 throughout
  (adds a routed governance gate to `validation.contract.yml` +
  `AgentValidation.knownGates` and changes consumer-facing phase-skill text)
- **[SEH]** — design-approved synthetic error-handling task paired with
  `synthetic-error-handling-approved`

Every task line mirrors its structured `skillist` value via `[skillist: ...]`
(`[skillist: []]` when none applies). Every task has a matching key in
`tasks.deps.yml`.

## Governance risk levels

- **Small** (skill-text edits to `.agents/skills/speckit-{implement,tasks,taskstoissues,constitution}/SKILL.md`):
  focused validation = `PhaseHookParityCheck` + `SkillSyncCheck`.
- **Medium** (new `PhaseHookParity` rule/gate + `Routing.fs`/`Targets.fs`/Engine
  wiring): focused validation = `Dev` (`Governance.Tests`) +
  `PhaseHookParityCheck` + `TargetMetadataDrift`.
- **Broad** (consumer propagation + contract currency): the full escalated serial
  order is required — `Dev` → `PhaseHookParityCheck` → `GeneratedGuidanceCheck`
  → `TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`.
  Broad validation is required once both the skills and the gate are in place
  (Phase 6). Non-authoritative aggregate results (notably the known
  `GeneratedProductCheck` local env failure) are recorded in
  `readiness/aggregate-hang-diagnostics.md` and
  `readiness/generated-product-check.md`, never used to claim a hard pass.

## Success-criterion → assertion mapping

- **SC-001** (100% of completed implement-phase runs in a feedback-enabled
  project produce a record / surfaced notice — zero silent omissions) → **no
  direct in-repo assertion is feasible**: this repository registers no feedback
  extension (only `git`/`evidence`), so an actual feedback record cannot be
  produced here. SC-001 is satisfied by **proxy** — the marker-presence checks
  (T012 for `implement`, T016 for the full corpus) prove the repaired skill
  performs `after_implement` discovery + the consolidated notice, and the
  `Breakout1` dogfood observation (spec Context) is the behavioral datum. This
  proxy is disclosed per Principle V; a real behavioral run belongs to a
  feedback-enabled consumer project (T022/T023 propagation), not this repo.
- **SC-002 / SC-003** (0 phase skills missing the block; guard fails when a block
  is removed) → the failing-first negative test (T006) + the positive full-corpus
  assertion (T016) in `Governance.Tests/PhaseHookParityTests.fs`.
- **SC-004** (`.agents`↔`.claude` in sync) → `SkillSyncCheck` after
  `RefreshSurfaceBaselines` (T014, T020).
- **SC-005** (generated projects carry the corrected skills) → `TemplateCheck` /
  `GeneratedGuidanceCheck` (T022).
- **SC-006** (no behavior change with no feedback hook registered) → the
  behavior-preservation check (T015).

---

## Phase 1: Setup

- [X] T001 [T1] [skillist: []] Confirm the feature directory and link `spec.md` ↔ `plan.md` ↔ this `tasks.md`; confirm `.specify/feature.json` resolves to `specs/077-implement-feedback-hook-parity`
- [X] T002 [P] [T1] [skillist: []] Add `readiness/` scaffolding with audit-enforced placeholders discoverable before implementation: `phase-hook-parity-check.md`, `skill-sync.md`, `template-check.md`, `generated-product-check.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md`, `evidence-graph.md`, `evidence-audit.md` — each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [P] [T1] [skillist: []] Record feature Tier (Tier 1 — new routed gate + consumer-facing skill text), affected layer (governance `build/Governance/**` + `.agents/skills/**`, no product `.fsi`), public-API impact (none on product libraries), Elmish/MVU applicability (reuse of the existing governance Engine boundary — a new `StartTarget PhaseHookParityCheck` `Msg` emits a `PhaseHookScan` effect; the check itself is the pure `checkCorpus`; interpreter evidence is the real guard run, T021), and evidence obligations (guard PASS on repaired tree + red→green guard test + `.agents`↔`.claude` sync + generated-output propagation)

---

## Phase 2: Foundation (the FR-006 anti-drift guard)

- [X] T004 [T1] [skillist: fsharp-code-generation] Draft the curated public surface `build/Governance/PhaseHookParity.fsi` (`roster : string list`; `type ParsedPhaseSkill = { Phase; RelPath; Body }`; `val checkCorpus : ParsedPhaseSkill list -> Findings.ValidationFinding list`; `val renderReport : ParsedPhaseSkill list -> string`) per `data-model.md`
- [X] T005 [T1] [skillist: []] Add the `PhaseHookParity.fsi` + `PhaseHookParity.fs` compile entries to `build/Governance/Governance.fsproj` (after `Findings`, before `Routing`)
- [S] T006 [SEH] synthetic-error-handling-approved [T1] [skillist: fsharp-build-orchestration] Add the failing-first **negative** test in `tests/Governance.Tests/PhaseHookParityTests.fs`: feed a block-stripped SKILL.md body (modern markers removed) and assert a `phase-hook-parity` finding is produced — red before the guard logic exists (`./fake.sh build -t Dev`)
- [X] T007 [T1] [skillist: fsharp-parsing] Implement the pure `build/Governance/PhaseHookParity.fs`: the fixed nine-phase `roster`, the three strict literal markers (`.specify/extensions/*/*.yml` ≥ 2×, `(extension, command)` dedupe language, `## Effective hooks for <phase>`), `checkCorpus` (one finding per missing marker; missing/unreadable roster skill ⇒ named failure), and `renderReport` — turning T006 green
- [X] T008 [T1] [skillist: fsharp-build-orchestration] Register the target in `build/Governance/Targets.fs`: `PhaseHookParityCheck` DU variant + `allTargets` + `name` + `directPrerequisites [Build]` + timeout/cost/owner metadata
- [X] T009 [T1] [skillist: []] Add `"PhaseHookParityCheck"` to `build/Governance/AgentValidation.fs` `knownGates`
- [X] T010 [T1] [skillist: fsharp-io-globbing] Wire the effect loop through the existing Engine boundary: `Engine/Update.fs` `StartTarget PhaseHookParityCheck` → `PhaseHookScan` effect; `Engine/Interpret.fs` `PhaseHookScan` handler (enumerate roster `.agents`/`.claude` SKILL.md → `checkCorpus` → `renderReport` → write `readiness/phase-hook-parity-check.md` → `failwith` on findings); `Front/Governance.fs` `runPhaseHookParityCheck` entry mirroring the `SkillQualityCheck` runner
- [X] T011 [T1] [skillist: fsharp-build-orchestration] Register the `PhaseHookParityCheck` FAKE target in `build.fsx`/`scripts/build/**` and add `Targets.PhaseHookParityCheck` to the `skill-quality` rule `RequiredGates` in `build/Governance/Routing.fs`

**Checkpoint**: Guard infrastructure exists; the pure logic is green on the
negative test; phase skills are not yet repaired (positive corpus still red).

---

## Phase 3: User Story 1 — feedback captured after implement (US1)

### Tests First (Principle I, Principle VI)

- [X] T012 [P] [US1] [T1] [skillist: fsharp-build-orchestration] Add the US1 assertion to `PhaseHookParityTests.fs`: the real `speckit-implement` SKILL.md (and its `.claude` mirror) passes all three markers — red until T013 repairs the skill (`./fake.sh build -t Dev`)

### Implementation

- [X] T013 [P] [US1] [T1] [skillist: []] Repair `.agents/skills/speckit-implement/SKILL.md`: add the `before_implement` pre-hook block and the `after_implement` post-hook block (multi-file discovery across central `extensions.yml` + every `.specify/extensions/*/*.yml`, dedupe by `(extension, command)`, optional/mandatory/condition/`enabled:false` precedence) plus the `## Effective hooks for implement` consolidated notice, mirroring `speckit-plan` (anchor = the implement workflow's first section)
- [X] T014 [US1] [T1] [skillist: []] Regenerate the `.claude` mirror via `./fake.sh build -t RefreshSurfaceBaselines` and confirm `SkillSyncCheck` reports no `.agents`↔`.claude` drift for `speckit-implement` (watch trailing-newline drift)
- [X] T015 [US1] [T1] [skillist: []] Behavior-preservation evidence (SC-006/FR-005/FR-009): run the implement phase / the guard in this repo (which registers only `git`/`evidence` hooks, no feedback) and confirm the new blocks are a silent no-op — no new error, prompt, or feedback file; record in `readiness/runtime-limitations.md`

**Checkpoint**: User Story 1 functional — implement honors its registered
`after_implement` hook and is silent when none is registered.

---

## Phase 4: User Story 2 — every phase honors its registered hooks (US2)

### Tests First

- [X] T016 [P] [US2] [T1] [skillist: fsharp-build-orchestration] Add the positive full-corpus assertion to `PhaseHookParityTests.fs` (SC-002/SC-003): every one of the nine roster phase skills — and each `.claude` mirror — passes all three markers; red until T017–T019 repair the remaining skills

### Implementation

- [X] T017 [P] [US2] [T1] [skillist: []] Repair `.agents/skills/speckit-tasks/SKILL.md` (none → modern): add the `before_tasks`/`after_tasks` discovery blocks + `## Effective hooks for tasks` notice (FR-004)
- [X] T018 [P] [US2] [T1] [skillist: []] Upgrade `.agents/skills/speckit-taskstoissues/SKILL.md` (legacy single-file → modern multi-file): replace the central-`extensions.yml`-only block with multi-file `.specify/extensions/*/*.yml` discovery + `(extension, command)` dedupe + `## Effective hooks for taskstoissues` notice
- [X] T019 [P] [US2] [T1] [skillist: []] Repair `.agents/skills/speckit-constitution/SKILL.md` (none → modern): add the `before_constitution`/`after_constitution` blocks so the **mandatory** `before_constitution` `git.initialize` hook is honored, plus the `## Effective hooks for constitution` notice
- [X] T020 [US2] [T1] [skillist: []] Regenerate the `.claude` mirrors for `tasks`/`taskstoissues`/`constitution` via `RefreshSurfaceBaselines` and confirm `SkillSyncCheck` reports no drift (SC-004)
- [X] T021 [US2] [T1] [skillist: fsharp-build-orchestration] Run `./fake.sh build -t PhaseHookParityCheck` against the fully repaired tree → all nine in-scope phase skills PASS; capture `readiness/phase-hook-parity-check.md` (the real interpreter-edge guard run is the emitted-effect evidence for the new `PhaseHookScan` effect)

**Checkpoint**: User Story 2 functional — zero phase skills missing the block;
the guard passes on the real tree and bites on a stripped body.

---

## Phase 5: User Story 3 — the fix reaches generated consumer projects (US3)

- [X] T022 [P] [US3] [T1] [skillist: fs-skia-template-update] Confirm propagation: `./fake.sh build -t GeneratedGuidanceCheck` then `./fake.sh build -t TemplateCheck` (`TemplateSmoke` asserts the corrected `speckit-implement`/`speckit-tasks` skills are present in generated `.agents` and `.claude` output); capture `readiness/template-check.md` + `readiness/generated-guidance-validation.md` (SC-005)
- [X] T023 [US3] [T1] [skillist: fs-skia-template-update] Run `./fake.sh build -t GeneratedProductCheck`; record the result in `readiness/generated-product-check.md` and treat the known local env failure (no template `feature.json` / `Map.empty` env) as **non-authoritative** in `readiness/aggregate-hang-diagnostics.md` — rely on `TemplateCheck`/CI for the propagation proof

**Checkpoint**: User Story 3 validated — generated projects carry the repaired
skills without manual patching.

---

## Phase 6: Integration & Polish (broad validation)

- [X] T024 [P] [T1] [skillist: []] Regenerate `validation.contract.yml` from `Routing.fs` via `RefreshSurfaceBaselines` (new `PhaseHookParityCheck` gate on the `skill-quality` rule) and confirm `TargetMetadataDrift` reports no contract drift
- [ ] T025 [T1] [skillist: []] Bump and pack `FS.Skia.UI.Build` (its assembly changed) per the build-package-version-drift guidance so the template-posture check stays green, even though no product src libs were touched
- [X] T026 [T1] [skillist: []] Run `./fake.sh build -t Route` and `./fake.sh build -t Route --enforce`: confirm a `.agents/skills/**` diff escalates to `FocusedAuthority` and that `PhaseHookParityCheck` appears in the printed `skill-quality` gate list with its required evidence artifact present
- [X] T027 [P] [T1] [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the echoed `feature-directory`/`tasks=<n>` match this feature, no cycles, no dangling refs, no `[S*]` surprises
- [X] T028 [T1] [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (synthetic-propagation + diff-scan); the single `[SEH]` row (T006) must remain `[S]`/`accepted-seh`, never `[X]`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T006 | Negative guard proof requires a deficient skill body; keeping a permanently-broken real SKILL.md in the tree is infeasible | Positive full-corpus assertion (T016) runs against the real repaired skills; real guard run T021 | — | synthetic-error-handling-approved | plan.md Constitution Check ("Synthetic evidence") + research.md D4 | block-stripped SKILL.md body (modern markers removed, in-memory string) | guard emits a `phase-hook-parity` finding / `PhaseHookParityCheck` fails | accepted-seh |

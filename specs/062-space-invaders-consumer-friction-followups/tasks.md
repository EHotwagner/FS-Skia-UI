# Tasks: Space-Invaders Consumer Friction Follow-ups

**Feature branch**: `062-space-invaders-consumer-friction-followups`
**Spec**: `specs/062-space-invaders-consumer-friction-followups/spec.md`
**Plan**: `specs/062-space-invaders-consumer-friction-followups/plan.md`

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
The classification must be assigned during design, planning, clarification, or
task generation. implementation-time relabeling is forbidden; newly discovered
needs go back to task/design review.

## Vertical-slice rule (US phases)

A task tagged `[US*]` may only be marked `[X]` when the change is
reachable from a user-facing entry point and that path was actually
exercised — an FSI session against the packed library, a smoke run of the
application, a manual walk-through with transcript, or a screenshot
captured under `readiness/`. Domain, model, or core-layer changes alone
do **not** satisfy `[X]` for a `[US*]` task, even if their unit tests
pass green. If the user-reachable surface is missing, stubbed, or not
yet wired, mark `[ ]` (work continues) or `[S]` with a disclosed reason
in the Synthetic-Evidence Inventory — never `[X]`.

For stateful or I/O-bearing stories, `[X]` also requires Elmish/MVU evidence:
the public `Model` / `Msg` / `Effect` or `Cmd<Msg>` contract was exercised,
pure `update` transitions were tested, emitted effects were asserted, and
the effect interpreter was run against real dependencies where safe. **This
feature adds no framework `Model`/`Msg`/`Effect` (Principle IV N/A — see
plan); the only new public surface is the FR-010 pure value-type helpers
exercised via FSI/unit tests.**

## Success-criterion → assertion mapping

Where a success criterion is mechanically testable (first-frame content, no-overlap,
determinism, a structural invariant), pair it with a concrete enforcing assertion so a
headline SC cannot be silently violated while every gate stays green. Note the mapping on
the task line or in the test name, e.g. `(SC-003)`. Worked mappings for this feature:
- **SC-001** → governance test that every `after_<phase>` feedback hook is
  `optional: false` (T010), enforced by `TemplateCheck`/`GeneratedGuidanceCheck` (T015).
- **SC-002** → diagnostics print the full per-file schema for every evidence-format
  class, single-sourced from the enforcing constants (T008/T019/T020/T021), proven
  recoverable-without-decompiling in a generated project (T018).
- **SC-005** → the compiled symbol set-difference (T029) with a seeded-drift
  assertion (T030/T032), plus the `Result.Ok`/`Result.Error` pitfall note (T033).
- **SC-006** → RNG determinism/replay and `reserveHudBand` clamp assertions
  (T038/T039) backing the shipped `FS.Skia.UI.SkillSupport` surface.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, … — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change
- **[SEH]** — design-approved synthetic error-handling task paired with
  `synthetic-error-handling-approved`

Every task must have a matching entry in `tasks.deps.yml` even if its
dependency list is empty. Every task line MUST mirror the structured
`skillist` value using `[skillist: ...]`; use `[skillist: []]` when no
capability skill applies. The `speckit.evidence.graph` command refuses to
proceed with dangling references or invalid task skill metadata.

## Canonical Verification Targets

Generated tasks call repository targets instead of duplicating raw
restore/build/test/package/evidence command order:

- `./fake.sh build -t Route` for the authoritative tier + minimal gate list of
  the actual diff (re-run after each change-set).
- `./fake.sh build -t Dev` for fast local verification.
- `./fake.sh build -t Verify` for the full governed workflow.
- `./fake.sh build -t PackLocal` for local package output.
- `./fake.sh build -t RefreshSurfaceBaselines` for intentional current surface
  baseline refreshes and `.agents` → `.claude` skill-tree regeneration.
- `./fake.sh build -t PackageSurfaceCheck` / `PerPackageSurfaceDiff` for package
  surface review.
- `./fake.sh build -t TemplateCheck` / `GeneratedProductCheck` for source/package
  generated project validation.
- `./fake.sh build -t GeneratedGuidanceCheck` for generated prompt, task-skill,
  and implementation guidance governance.
- `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`
  for graph and synthetic-evidence gates.

FAKE-backed commands (`./fake.sh`, `fake.cmd`, or `dotnet fake`) share
repository `.fake` state and are not safe to run concurrently. Run multiple
FAKE-backed tests or targets serially in deterministic order:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

Non-FAKE checks may be marked parallel-safe when they do not invoke FAKE or
depend on `.fake`. Race-like or unknown concurrent FAKE failures require a
sequential rerun order before product-regression claims.

## Governance risk levels

- **Small** — a single content/skill/doc change-set (e.g. a pitfalls note, a
  generated doc). Focused validation: the Route-printed gates for that diff only.
- **Medium** — the self-describing-diagnostics and symbol-diff change-sets
  (`build/Governance/**`). Focused validation: `Dev` + the Evidence gates +
  unit tests; broad validation when the schema constants or render output change.
- **Broad** — the FR-010 Tier-1 helper change-set (new `.fsi` surface + per-package
  baseline). Broad validation required: the full serialized six-target order plus
  `PackageSurfaceCheck`/`PerPackageSurfaceDiff`. Aggregate results from any broad
  run are recorded as **non-authoritative** in `readiness/aggregate-hang-diagnostics.md`;
  the authoritative verdict is the per-target gate, not the aggregate.

## Skill registry note

Declared `skillist` ids are the `name:` value from the owning `SKILL.md`
(`.agents/skills/*/SKILL.md`, `src/*/skill/SKILL.md`,
`template/fragments/*/skill/SKILL.md`), not the directory name. Evidence
ownership is declared via the optional `owns:` field in `tasks.deps.yml`
(closed vocabulary: `graph-validation`, `evidence-audit`, `task-generation`,
`implementation-loading`, `constitution`), never inferred from titles. Task
titles are free-form and never scanned for capability phrases. The visible
`[skillist: ...]` mirror must match the structured `skillist` exactly and in
order.

Template source: `.specify/presets/fsharp-opinionated/templates/tasks-template.md`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory and that spec.md, plan.md, research.md, data-model.md, contracts/, and quickstart.md are linked and current
- [X] T002 [P] [skillist: []] Scaffold `readiness/` audit-enforced placeholder files discoverable before implementation: `target-metadata.md`, `agent-ready-verdict.md`, `skill-loading-evidence.md`, `aggregate-hang-diagnostics.md`, `governance-risk-levels.md`, `runtime-limitations.md`, `readiness-recoverability.md`
- [X] T003 [P] [skillist: []] Record feature Tier (Tier 1, driven solely by FR-010), affected layers, public-API impact, Elmish/MVU applicability (N/A), and required evidence obligations to `readiness/agent-ready-verdict.md`
- [X] T004 [skillist: []] Run `./fake.sh build -t Route` against the working-tree diff and record the authoritative tier + minimal gate list to `readiness/target-metadata.md`

---

## Phase 2: Foundation

- [X] T005 [skillist: []] Draft the new public surface as `.fsi`: `src/SkillSupport/Random.fsi` (`RngState`, `seedRng`/`nextRng`/`nextBelow`) and `src/SkillSupport/Hud.fsi` (`BandEdge`, `Band`, `HudLayout`, `reserveHudBand`) per `contracts/skillsupport-api.md`
- [X] T006 [skillist: []] Exercise the drafted `.fsi` from FSI (seed-replay equality, `reserveHudBand` clamp/partition) and capture the session transcript to `readiness/fsi-session.txt`
- [X] T007 [P] [skillist: []] Create the new per-package surface baseline `readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt` from the drafted `.fsi` (authoritative `PerPackageSurfaceDiff` baseline; the reflected type-name `surface-baselines/` set does not include SkillSupport)
- [X] T008 [P] [skillist: fsharp-code-generation] Define the single-source `EvidenceFormatSchema` model/constants in `build/Governance/**` that both the FR-005 failing-class diagnostics and the generated `evidence-formats.md` derive from (so they cannot drift)
- [X] T009 [skillist: []] Record unsupported-scope handling, governance risk levels, and aggregate-hang diagnostics into `readiness/runtime-limitations.md` and `readiness/governance-risk-levels.md`

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) — Automatic feedback capture + deterministic hook precedence (P1)

### Tests First (Principle I, Principle VI)

- [X] T010 [P] [US1] [skillist: fs-skia-template-update] Failing-first governance test: every `after_<phase>` entry in `template/feedback/extensions/feedback.yml` registers `optional: false` (SC-001 regression guard)
- [X] T011 [P] [US1] [skillist: fs-skia-template-update] Generated-project harness verification: `dotnet new fs-skia-ui --feedback true`, confirm `.specify/extensions/feedback/feedback.yml` is `optional: false` and a completed phase auto-writes `specs/<feature>/feedback/<phase>-<date>.md` with no manual trigger (SC-001). Verified end-to-end against a real generated project (`SI062Probe`, template packed from current source): 6 `optional: false`, 0 `optional: true`; the precedence rule + effective-hooks notice ship in the generated phase skills.

### Implementation

- [X] T012 [US1] [skillist: []] Flip all six `optional: true` → `optional: false` (`after_specify/clarify/plan/tasks/analyze/implement`) in `template/feedback/extensions/feedback.yml` (FR-001)
- [X] T013 [US1] [skillist: []] Add the documented precedence rule (D1: `auto_execute_hooks` scopes the mandatory set; optionals always surfaced; `condition`-guarded deferred to executor) and the consolidated effective-hooks notice (D2, deduped by `(extension, command)`) to the phase skills that have a hook step (`speckit-{specify,clarify,plan,analyze,checklist}`; `tasks`/`implement` have no hook step, per 061) (FR-001/002)
- [X] T014 [US1] [skillist: fs-skia-template-update] Regenerate `.claude` from `.agents` (`RefreshSurfaceBaselines`) and confirm `SkillSyncCheck`/`TargetMetadataDrift`/`SkillQualityCheck` stay green (FR-012)
- [X] T015 [US1] [skillist: []] Fold the FR-001 `optional: false` regression assertion into `GeneratedGuidanceCheck` (`Guidance.validateFeedbackHookPolicy`, unit-tested) (low-cost executable check, D12)
- [X] T016 [US1] [skillist: []] Document the US1 independent validation path (auto-fire feedback + precedence/effective-hooks notice) and capture it under `readiness/feedback-hook-policy.md`

**Checkpoint**: User Story 1 is fully functional and testable independently.

---

## Phase 4: User Story 2 (US2) — Evidence formats fully recoverable without decompiling (P1)

### Tests First

- [X] T017 [P] [US2] [skillist: speckit-evidence-audit] Failing-first test: each evidence-format-class diagnostic prints its complete per-file schema (skill-loading-evidence 8-column table, window-visibility keys + `diagnostic-class` rows, SEH acceptance tokens) (SC-002)
- [X] T018 [P] [US2] [skillist: fs-skia-template-update] Generated-project verification: each evidence-format class is recoverable from the diagnostics and/or generated `docs/evidence-formats.md` — no `strings -el`, no sibling copy; logged to `readiness/readiness-recoverability.md` (SC-002). The real generated project ships `docs/evidence-formats.md` with all four classes (up-front recovery), and the per-class on-failure diagnostics are single-sourced + unit-proven.

### Implementation

- [X] T019 [US2] [skillist: fsharp-parsing] Add the `skill-loading-evidence.md` 8-column table schema print (one row per `(task,skill)`, `loaded_at < work_started_at`, resolved `.agents/skills/<id>/SKILL.md` path) to `build/Governance/Evidence/Audit.fs` (FR-005)
- [X] T020 [US2] [skillist: fsharp-parsing] Add the window-visibility `key=value` + `diagnostic-class=` value-row schema print to `build/Governance/Evidence/Scans.fs` (FR-005)
- [X] T021 [US2] [skillist: fsharp-parsing] Add the SEH acceptance token schema print (`accepted-seh`, `synthetic-error-handling-approved`, no backticks) to `build/Governance/Evidence/TaskParser.fs` (FR-005)
- [X] T022 [US2] [skillist: fsharp-code-generation] Generate `template/base/docs/evidence-formats.md` from the shared `EvidenceFormatSchema` constants and add its currency check (FR-005, D5)
- [X] T023 [US2] [skillist: []] Add `docs/evidence-formats.md` to the `.template.config/template.json` content map (verbatim/copyOnly, no `sourceName` substitution)

**Checkpoint**: User Story 2 is fully functional and testable independently.

---

## Phase 5: User Story 3 (US3) — Accurate build-target and task-graph visibility (P2)

- [X] T024 [P] [US3] [skillist: fsharp-build-orchestration] Add the "`Dev` writes logs/markers and does not compile; `Test`/`Verify` (`dotnet test`) is authoritative" line to `Dev`'s own emitted output and `dev-verdict.txt` in `build/Governance/Engine/Update.fs` (FR-004, SC-004)
- [X] T025 [P] [US3] [skillist: fsharp-graph-algorithms] Render the effective DAG — explicit deps plus the auto-injected Phase N+1 → Phase N checkpoint edges, distinctly labeled — and the resolved `skillist`-id set in `build/Governance/Evidence/Render.fs` (FR-007, SC-004)
- [X] T026 [P] [US3] [skillist: fsharp-code-generation] Generate `template/base/docs/skillist-reference.md` from the live `SkillRegistry` (directory-name-vs-`name:` resolved + closed `owns:`→implied-skill table) with a currency check (FR-006, SC-004)
- [X] T027 [US3] [skillist: []] Add `docs/skillist-reference.md` to the `.template.config/template.json` content map
- [X] T028 [US3] [skillist: fsharp-build-orchestration] Tests: `Dev` output caveat present; effective-DAG render shows injected edges + resolved skillist set; `skillist-reference.md` currency holds (SC-004)

**Checkpoint**: User Story 3 is fully functional and testable independently.

---

## Phase 6: User Story 4 (US4) — Mechanical cross-artifact symbol consistency (P2)

- [X] T029 [P] [US4] [skillist: fsharp-parsing] Implement the pure compiled symbol set-difference helper (extract `Msg` cases, union/`Screen` variants, entity record names, FR-/SC- IDs from `plan.md`/`data-model.md`/`tasks.md`; report proper-subset differences) (FR-008, D8)
- [X] T030 [P] [US4] [skillist: fsharp-parsing] Failing-first unit tests for the symbol-diff set algebra: proper-subset detection flagged; intentionally design-only symbol reported for human judgment, never hard-failed (SC-005)
- [X] T031 [US4] [skillist: speckit-analyze] Add analyze detection pass G to the `speckit-analyze` skill that runs/interprets the symbol-diff and reports set-differences as findings (FR-008)
- [X] T032 [US4] [skillist: speckit-analyze] Verification: seed a deliberate `Msg`-case drift (present in `data-model.md`/`tasks.md` but not `plan.md`) and confirm pass G reports the set-difference (SC-005)

**Checkpoint**: User Story 4 is fully functional and testable independently.

---

## Phase 7: User Story 5 (US5) — Pitfalls cover Result-shadowing + read-before-design source map (P3)

- [X] T033 [P] [US5] [skillist: fs-skia-skiaviewer] Add a "Common pitfalls" entry to the canonical `fs-skia-skiaviewer` skill: `open FS.Skia.UI.SkiaViewer` brings `ViewerDiagnosticLevel.Error` (and peers) into scope so bare `Ok`/`Error` bind to the union case — remedy: qualify as `Result.Ok`/`Result.Error`; cross-reference the existing `Unknown` note (FR-009, D9). Also added the companion `fs-skia-scene` record-label-collision Common-pitfalls note so the T034 pre-design pointer is non-dangling (SI-9 "already covered" assumption was not yet true in the skill).
- [X] T034 [P] [US5] [skillist: fs-skia-layout-readability] Author `template/base/docs/scaffold-map.md`: durable vs replaceable `src/**/*.fs`, the `GovernanceTests`-durable / `BehaviorTests`-replaceable split, the must-survive source-scan strings, and a pre-design pointer to the `fs-skia-scene` record-label-collision pitfall (FR-003, folds SI-9, D3)
- [X] T035 [US5] [skillist: []] Add `docs/scaffold-map.md` to the `.template.config/template.json` content map and add the one-line cross-reference from `fs-skia-layout-readability` so the map is reachable from an already-loaded skill
- [X] T036 [US5] [skillist: fs-skia-template-update] Regenerate `.claude` from `.agents` (`RefreshSurfaceBaselines`) and confirm `SkillSyncCheck`/`SkillQualityCheck` green after the FR-009/003 skill edits (FR-012) — done via the single post-T044 RefreshSurfaceBaselines; SkillSyncCheck/SkillQualityCheck/TargetMetadataDrift all green
- [X] T037 [US5] [skillist: []] Verification: the `fs-skia-skiaviewer` pitfalls note covers the `Result.Ok`/`Result.Error` case, and `scaffold-map.md` references the `fs-skia-scene` record-label pitfall as a pre-design step (SC-003)

**Checkpoint**: User Story 5 is fully functional and testable independently.

---

## Phase 8: User Story 6 (US6) — Recurring arcade helpers shipped, not re-documented (P3, Tier 1)

### Tests First

- [X] T038 [P] [US6] [T1] [skillist: fs-skia-elmish] Failing-first tests: RNG determinism / replay equality (same seed + sequence ⇒ identical stream) and `nextBelow n` bounds in `[0, n)` for `n > 0` (SC-006)
- [X] T039 [P] [US6] [T1] [skillist: fs-skia-layout-readability] Failing-first tests: `reserveHudBand` clamp/partition invariants — `HudBand.Size = min bandSize surface`, `Gameplay.Size = surface − HudBand.Size ≥ 0`, non-overlapping partition (SC-006)

### Implementation

- [X] T040 [US6] [T1] [skillist: fs-skia-elmish] Implement `src/SkillSupport/Random.fs` (splitmix64 seed → xorshift64 stream, pure `state -> (value, nextState)` threading, no ambient `System.Random`) against the drafted `.fsi` (FR-010)
- [X] T041 [US6] [T1] [skillist: fs-skia-layout-readability] Implement `src/SkillSupport/Hud.fs` (`reserveHudBand` plain-`float` API, no `Scene.Rect` dependency) against the drafted `.fsi` (FR-010)
- [X] T042 [US6] [T1] [skillist: []] Add `Random.fsi`/`.fs` and `Hud.fsi`/`.fs` `Compile` entries (`.fsi` before `.fs`) to `src/SkillSupport/SkillSupport.fsproj` (FR-010)
- [X] T043 [US6] [T1] [skillist: fs-skia-template-update] Finalize `readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt` against the built `.fsi` and confirm `PackageSurfaceCheck`/`PerPackageSurfaceDiff` (FR-012, Principle II)
- [X] T044 [US6] [T1] [skillist: fs-skia-elmish] Add the `Random` skill reference to `fs-skia-elmish` (pure-`update` threading owner) and the `Hud` reference to `fs-skia-layout-readability` (HUD/gameplay-region owner), then regenerate `.claude` (FR-010/011-#2, D11)
- [X] T045 [US6] [skillist: []] Record the FR-010 per-helper ship decisions and the FR-011 five-candidate dispositions (ship / fold / defer-with-rationale per D10/D11) so no candidate is silently dropped (SC-006)

**Checkpoint**: User Story 6 is fully functional and testable independently.

---

## Phase 9: Integration & Polish

- [X] T046 [skillist: []] Surface-area baseline refresh (Tier 1 only): confirmed `RefreshSurfaceBaselines` leaves the surface baselines and `.claude` tree clean (`PerPackageSurfaceDiff` zero-drift, `SkillSyncCheck` green, `TargetMetadataDrift` green)
- [X] T047 [skillist: fs-skia-template-update] Ran `TemplateCheck` (PASS — generated projects ship evidence-formats / skillist-reference / scaffold-map + flipped feedback.yml) + `GeneratedProductCheck` (EXPECTED-FAIL non-regression: feature-less scaffold has no `feature_directory`; aggregate is non-authoritative, the authoritative verdict is `EvidenceAudit verdict=PASS`); non-authoritative aggregate notes recorded in `readiness/target-metadata.md` (Dev regenerates the generic `aggregate-hang-diagnostics.md`)
- [X] T048 [skillist: speckit-evidence-graph] Ran `./fake.sh build -t EvidenceGraph` — no cycles, no dangling refs, no `[S*]`; the effective-DAG render (injected edges + resolved skillist set) is in `readiness/task-graph.md`
- [X] T049 [skillist: speckit-evidence-audit] Ran `./fake.sh build -t EvidenceAudit` — `verdict=PASS` (43 real tasks, 0 blockers) for `specs/062-space-invaders-consumer-friction-followups` (SC-007)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

No synthetic evidence is planned — all evidence is real (real gate runs, a real
generated project, real RNG/band/symbol-diff unit tests; see plan §Synthetic
evidence). `[S]` disclosure applies only if a real path proves infeasible
mid-implementation; none anticipated.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |

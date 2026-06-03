# Tasks: Decouple Author-Guidance Prose from Generation-Currency Anchors

**Feature branch**: `055-decouple-guidance-anchors`
**Spec**: `specs/055-decouple-guidance-anchors/spec.md`
**Plan**: `specs/055-decouple-guidance-anchors/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

This feature ships **zero** synthetic evidence (Constitution-check Principle V):
every proof is a real `evaluateGuidanceCheck` run over real governed-file
content, a real `wc -l` count, a real gate run, and a real diff. No `[S]` or
`[SEH]` rows are anticipated; the Synthetic-Evidence Inventory below stays empty.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- Tier annotation omitted: every task matches the spec's overall **Tier 2**
  (internal governance change — no public product `.fsi`, surface baseline,
  package identity, or runtime behavior change).

Every task has a matching entry in `tasks.deps.yml` with both `deps` and
`skillist` fields. Each line mirrors the structured `skillist` via
`[skillist: ...]` (`[skillist: []]` when no capability skill applies).

## Pitfall guidance (read before running `EvidenceGraph`)

- `tasks.deps.yml` uses **one object-shaped key per task id** with indented
  `deps` and `skillist` fields — never inline maps like
  `T001: { deps: [], skillist: [] }`.
- Every `Tnnn` in this file appears exactly once as a key in `tasks.deps.yml`;
  dependency lists use exact `Tnnn` ids; the visible `[skillist: ...]` mirror
  matches the structured list exactly and in order.
- Phase-checkpoint edges are auto-injected (every Phase N+1 task implicitly
  depends on the last task of Phase N) — only non-phase cross-edges are written
  in the yml.
- Setup/readiness tasks that merely cite required filenames use the
  `Complete readiness notes` prefix so they do not trip capability-trigger
  groups; the graph/audit tasks (T018/T019) legitimately own the
  `EvidenceGraph`/`EvidenceAudit` work and carry the matching skill ids.

## Governance risk level

**Medium → broad at integration.** The diff spans `build/Governance/**` +
`tests/**`, governed guidance under `.specify/**` (and possibly
`.agents/skills/**`), `docs/**` baseline records, and the `specs/047-*` peer
record, so `Route` **escalates** to the maintainer-verify path. Focused
validation = the new pure-core unit tests (US1 rewording PASS, US2 drift FAIL,
SC-004 token-removal FAIL) plus the real-repo `GeneratedGuidanceCheck`
regression. Broad validation (the full serialized six-target order) is required
at integration (Phase 6). Aggregate FAKE results are recorded as
**non-authoritative**; any race-like failure is rerun in focused isolation as
the authoritative result (FAKE shares `.fake` state — never run concurrently).

---

## Phase 1: Setup

- [X] T001 [skillist: []] Record feature scope and evidence obligations in the plan — Tier 2 internal governance change; affected layers are `build/Governance/Guidance.fs[i]`, `tests/Governance.Tests/**`, governed guidance under `.specify/**` (templates, `fsharp-opinionated` presets, command/memory copies) and possibly `.agents/skills/**`, and the baseline/goal records `docs/reports/_baselines/2026-06-02-foundations-after.md` + `specs/047-foundations-programme-closeout/contracts/after-baseline.md`; no public product `.fsi`/surface/package/runtime impact; Principle IV (Elmish/MVU) is **not applicable** (pure validation refactor, IO confined to the existing read-file wrapper); required real evidence = prose-size accounting, the rewording-passes / drift-fails red→green, the enumerated contract-token set, and the restated-goal record
- [X] T002 [P] [skillist: []] Complete readiness notes for the audit-required readiness files — create `specs/055-decouple-guidance-anchors/readiness/` and author `governance-risk-levels.md` (the small / medium / broad risk levels, the focused validation required for the selected level, when broad validation is required, and how non-authoritative aggregate FAKE results are recorded), `aggregate-hang-diagnostics.md` (verdict / stage / elapsed duration / last observed command / focused rerun / non-authoritative aggregate), and `runtime-limitations.md` (.NET 10 desktop / Vulkan / SkiaSharp preview / unsupported macOS/mobile/browser / no software-renderer fallback) so the unconditional readiness-contract scan passes
- [X] T003 [P] [skillist: []] Complete readiness notes for this feature's authored-evidence placeholders — create placeholder `readiness/prose-size-accounting.md`, `readiness/decoupling-red-green.md`, `readiness/contract-tokens.md`, `readiness/evidence-policy-separation.md` (the specify-catchall / generated-guidance artifact `Route` requires for `.specify/**` edits), `readiness/validation-contract.md` (docs-only single-source-generation currency note), and `readiness/skill-loading-evidence.md`, each naming its authoritative command, artifact path, failure class, and next action (regenerable logs land under `readiness/logs/**`, already gitignored)

---

## Phase 2: Foundation

- [X] T004 [skillist: []] Capture the pre-change baseline as failing-first evidence — record that under today's literal-substring table a reworded-but-concept-preserving edit to a governed file trips a `missing` term finding (the freeze SC-001 lifts), and capture the current measured guidance-prose line count (`find .agents/skills -name '*.md' | xargs wc -l | tail -1` + `find .specify -name '*.md' | xargs wc -l | tail -1`) into the readiness baselines (the before-state for SC-001 / SC-005)
- [X] T005 [P] [skillist: fsharp-parsing] Implement the decoupled model and pure evaluator in `build/Governance/Guidance.fs` — the `ContractToken`, `MatchMode` (`AnyOf` / `AllOf`), `GuidanceObligation`, and `GuidanceCheck` types and `evaluateGuidanceCheck : (string -> string option) -> GuidanceCheck -> string list` implementing the data-model rules (token-present case-insensitive substring; forbidden-token must-not-appear; obligation `AnyOf`/`AllOf` over short concept anchors; missing-file), reusing the existing `ValidationFinding` convention; keep the gate entry point `runGeneratedGuidanceScan` and its IO wrapper unchanged; add the new types to `Guidance.fsi` **only if** a unit test references them directly

**Checkpoint**: Foundation ready — the pure core exists and story implementation may begin.

---

## Phase 3: User Story 1 (US1) — tighten guidance prose without tripping the currency gate (P1)

### Tests First (Principle I, Principle VI)

<!-- Behavior-changing code MUST include automated tests that fail before the change and pass after (constitution Principle VI). -->

- [X] T006 [P] [US1] [skillist: fsharp-build-orchestration] Add the failing-first US1 test in `tests/Governance.Tests/GuidanceValidatorTests.fs` — feed `evaluateGuidanceCheck` an in-memory lookup whose content **shortens/rewords** a governed paragraph while preserving the obligation's concept anchor, assert it returns **no findings** (PASS), and assert the pre-055 literal-substring table would have returned a `missing` term finding for the same edit; confirm the new-evaluator assertion **fails-first** until T007 wires the obligations (SC-001 red→green)
### Implementation
- [X] T007 [US1] [skillist: fsharp-parsing] Build the `task-skillist-guidance` `GuidanceCheck` value and wire it — map the pre-055 literal table to `ContractToken`s (`[skillist: []]`, `skillist:`, `deps:`, `[SEH]`, `synthetic-error-handling-approved`, `loaded_at`, `work_started_at`, `readiness/skill-loading-evidence.md`) and `GuidanceObligation`s (`skillist-structured` `AnyOf`, `skillist-minimal-ordered` `AnyOf`, `skillist-confidence-fields` `AllOf`, `skill-breadth`, `aggregate-non-authoritative`, `graph-before-after`, `persistent-launch`, `seh-discipline`, `tasks-skill-gate`, `implement-skill-loading`, `tasks-post-gen-timing`, and the remaining concept obligations defined in `data-model.md`) per the data-model mapping, with **every twin** (template + `fsharp-opinionated` preset copy + command copy + memory copy) listed in each `Files`; redefine `validateTaskSkillistGuidance` as `evaluateGuidanceCheck (realLookup model)` over that value; confirm T006 now PASSes (SC-001)
- [X] T008 [US1] [skillist: []] Demonstrate the unlock live — tighten an actual governed paragraph (e.g. in `.specify/templates/tasks-template.md`) so it shortens/rewords while keeping its obligation concept, run `./fake.sh build -t GeneratedGuidanceCheck` and observe PASS where the pre-055 table failed; regenerate any touched `.agents`→`.claude`/preset twin via `./fake.sh build -t RefreshSurfaceBaselines`; record the red→green transcript to `readiness/decoupling-red-green.md` (SC-001)

**Checkpoint**: User Story 1 — reworded-but-faithful prose passes the currency gate.

---

## Phase 4: User Story 2 (US2) — currency still catches genuine drift (P1)

### Tests First (Principle VI)

- [X] T009 [US2] [skillist: fsharp-build-orchestration] Add the failing-first US2 drift test in `tests/Governance.Tests/GuidanceValidatorTests.fs` — feed `evaluateGuidanceCheck` a lookup with an obligation's concept anchor **removed while its sibling `ContractToken` remains present**, and assert it FAILs with the exact diagnostic shape `"{file}: obligation '{id}' ({source}) not reflected [{tag}]"` naming the file, obligation id, and source of truth (proving the obligation fails on prose-concept loss, not merely on token loss — the anchor-disjointness rule); include the twin-coverage case (drift in one twin file is still caught) (SC-002, FR-003)
- [X] T010 [US2] [skillist: fsharp-build-orchestration] Add the SC-004 / FR-006 retention test in `tests/Governance.Tests/GuidanceValidatorTests.fs` — assert that removing any machine-contract token (e.g. `[skillist: []]`, `synthetic-error-handling-approved`) from the lookup still FAILs with a `missing \`{token}\`` finding, and that reintroducing a forbidden/stale term still FAILs with the stale-term finding
### Implementation
- [X] T011 [US2] [skillist: fsharp-parsing] Convert `controls-boundary-guidance` to the model — `ContractToken`s (`FS.Skia.UI.Controls`, `Control<'msg>`, `DataGrid`, `FS.Skia.UI.Controls.Elmish`, `ControlsElmish.program`), obligations (`controls-skia-rendered` `AnyOf`, `controls-no-charts-shim` `AllOf`), and **every** `Forbidden` stale-term entry preserved verbatim (`FS.Skia.UI.Charts`, `fs-skia-charts`, `chart-only`, `DataGrid as chart`, `renderer neutral`, `host loop ownership`, …) so removed-Charts language cannot re-enter (FR-006); redefine `validateControlsBoundaryGuidance` over the new value
- [X] T012 [US2] [skillist: fsharp-parsing] Convert `sequential-fake-guidance` to the model — a `fake-sequential` obligation (`AllOf` over the four facets `FAKE-backed` / `.fake` / `sequential` / `not safe to run concurrently`, source `CLAUDE.md:FAKE concurrency rule`) while retaining the structural regex assertions (FAKE-command-present, numbered-order requirement, parallelism non-FAKE caveat) as unchanged machine logic; redefine `validateSerializedRunnerGuidance` over the new value
- [X] T013 [US2] [skillist: fsharp-build-orchestration] Verify no weakening of drift detection — run the new T009/T010 tests green, confirm the real-repository `runGeneratedGuidanceScan` still PASSes via `./fake.sh build -t GeneratedGuidanceCheck` (SC-006 regression), confirm all three mixed-purpose sites are now migrated with no list freezing prose as a pure currency proxy (SC-003), and record the US2 drift transcript plus the enumerated machine-contract-token set to `readiness/decoupling-red-green.md` and `readiness/contract-tokens.md` (SC-002, SC-004)

**Checkpoint**: User Story 2 — source-of-truth drift still fails, tokens stay literal, all three sites decoupled.

---

## Phase 5: User Story 3 (US3) — honest, measurable prose-size accounting (P2)

- [X] T014 [P] [US3] [skillist: fsharp-build-orchestration, fsharp-io-globbing] Implement prose-size accounting **test-first** — (a) add a failing-first byte-determinism test in `tests/Governance.Tests/ProseSizeAccountingTests.fs` feeding the pure render function a known `ProseSizeAccounting` record (baseline `6882`, fixed agents/specify counts) and asserting the rendered Markdown states the corrected baseline, the summed current count, the signed delta, and the restated target; then (b) implement the pure report-rendering function over a `ProseSizeAccounting` record (corrected baseline `6882`, the `.agents/skills/**/*.md` and `.specify/**/*.md` line counts gathered by the front-end IO enumeration, the summed current count, the delta, and the restated target) rendered to `readiness/prose-size-accounting.md` with the `find … | wc -l` reproduction commands, turning the test green (FR-007, SC-005)
- [X] T015 [US3] [skillist: []] Restate the size goal in the canonical baseline/goal records — edit `docs/reports/_baselines/2026-06-02-foundations-after.md` (row 5) and `specs/047-foundations-programme-closeout/contracts/after-baseline.md` so the discredited ~23,000-line / "low hundreds" figure is retired as the live target and tracking is stated against the corrected ≈6,882 baseline, with the actual large-scale reduction recorded as a bounded follow-up (FR-008, SC-005)

**Checkpoint**: User Story 3 — the goal is honest and the accounting is reproducible.

---

## Phase 6: Integration & gates (escalated maintainer-verify, serialized)

- [X] T016 [skillist: fsharp-build-orchestration] Keep single-source generation current (FR-010) — if any `.agents/skills/*/SKILL.md` prose was tightened, regenerate via `./fake.sh build -t RefreshSurfaceBaselines`, then confirm `SkillSyncCheck` (`.claude/skills/**` is a byte-identical reproduction of `.agents/skills/**`) and `TargetMetadataDrift` (`validation.contract.yml` generated from `Routing.fs` stays current; `Routing.fs` is not edited so it must not regenerate) stay **green**; record to `readiness/validation-contract.md`
- [X] T017 [skillist: fsharp-build-orchestration] Confirm `./fake.sh build -t Route --enforce` reports the escalated maintainer-verify tier with every required evidence artifact present, then run the escalated FAKE gate set **sequentially, never concurrently** — `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` — recording aggregate results as **non-authoritative** and rerunning any race-like failure in focused isolation as the authoritative result; logs under `readiness/logs/` (SC-006)
- [X] T018 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the acyclic DAG has no dangling refs, no `[S*]` surprises, and valid structured task metadata plus visible `skillist` mirrors (`verdict=ok`)
- [X] T019 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm `verdict=PASS` (0 unaccepted-synthetic, 0 auto-synthetic, 0 blocking diff-scan, 0 blocking readiness-contract) with zero synthetic evidence to accept; this feature ships no `[S]` task

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — this feature ships zero synthetic evidence; the red→green proofs feed real governed-file content with realistic reworded/drifted edits, and the size accounting is a real `wc -l` over the real corpus)_ | | | | | | | | |

# Tasks: Lunar-Lander Consumer Friction Follow-ups

**Feature branch**: `063-lunar-lander-consumer-friction-followups`
**Spec**: `specs/063-lunar-lander-consumer-friction-followups/spec.md`
**Plan**: `specs/063-lunar-lander-consumer-friction-followups/plan.md`

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
plan): the renderer fix is a pure draw walk and the only new public surface
is the FR-010 pure `Wrap.wrapDeltaX` helper, exercised via FSI/unit tests.**

## Success-criterion → assertion mapping

Where a success criterion is mechanically testable (first-frame content, no-overlap,
determinism, a structural invariant), pair it with a concrete enforcing assertion so a
headline SC cannot be silently violated while every gate stays green. Note the mapping on
the task line or in the test name, e.g. `(SC-003)`. Worked mappings for this feature:
- **SC-001** → before/after image-evidence golden/pixel assertion that `Line`/`Path`/`Text`
  render to actual pixels through the evidence path (T009/T015), backed by the shared
  exhaustive `SceneRenderer.paintNode` (T011/T012/T013).
- **SC-002** → the no-wildcard exhaustive `match` is a compile guard (T011), plus the test
  that a node-count assertion can no longer pass on an invisible placeholder scene (T010);
  the render-backed set is documented in `fs-skia-scene` (T014).
- **SC-003** → the `SymbolCrossCheck` target run from a read-only checkout over a seeded
  symbol drift (T020), backed by the target wiring + knownGates entry (T016/T017/T018).
- **SC-004** → the readiness-contract diagnostic relabel proven on a single-absent-token
  failure (T021/T022); the `speckit-implement` body names the reference + skill-loading
  location/timing (T023).
- **SC-005** → the `speckit-plan`/`scaffold-map` pointers (T025/T026), the URL-source
  snapshot step (T027), and the FR-008 template-scan disposition (T028).
- **SC-006** → `Wrap.wrapDeltaX` determinism/range/shortest-path assertions (T030) backing
  the shipped surface (T031/T032), and the recorded per-helper dispositions (T035).

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
- `./fake.sh build -t SymbolCrossCheck` for the FR-003 analyze pass-G symbol check
  (new this feature — reads plan/data-model/tasks from the feature dir).
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

- **Small** — a single content/skill/doc change-set (the FR-004/005/006/007/008
  skill + doc edits, the FR-009 summary-discipline note). Focused validation: the
  Route-printed gates for that diff only, plus `SkillSyncCheck`/`SkillQualityCheck`
  after `.agents` edits.
- **Medium** — the renderer refactor (`src/SkiaViewer/**`, FR-001/002) and the
  `SymbolCrossCheck` target (`build/Governance/**`, FR-003). Focused validation:
  `Dev` + the renderer golden/pixel tests + the Evidence gates + `TargetMetadataDrift`;
  broad validation when render output or the target contract changes.
- **Broad** — the FR-010 Tier-1 `Wrap` helper change-set (new `.fsi` surface +
  per-package baseline). Broad validation required: the full serialized six-target
  order plus `PackageSurfaceCheck`/`PerPackageSurfaceDiff`. Aggregate results from any
  broad run are recorded as **non-authoritative** in
  `readiness/aggregate-hang-diagnostics.md`; the authoritative verdict is the
  per-target gate, not the aggregate.

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

- [X] T001 [skillist: []] Confirm the feature directory and that spec.md, plan.md, research.md, data-model.md, contracts/ (renderer-parity, symbol-crosscheck-target, skillsupport-wrap-api, authoring-and-skill-edits), and quickstart.md are linked and current
- [X] T002 [P] [skillist: []] Scaffold `readiness/` audit-enforced placeholder files discoverable before implementation: `target-metadata.md`, `agent-ready-verdict.md`, `skill-loading-evidence.md`, `aggregate-hang-diagnostics.md`, `governance-risk-levels.md`, `runtime-limitations.md`, `renderer-image-evidence.md`, `symbol-cross-check.md`, `evidence-discoverability.md`, `evidence-path-token-scan.md`, `helper-dispositions.md`
- [X] T003 [P] [skillist: []] Record feature Tier (Tier 1, driven by the FR-010 `Wrap` surface + FR-003 new governance target; the renderer fix is Tier-2 internal but escalates the overall change), affected layers, public-API impact, Elmish/MVU applicability (N/A), and required evidence obligations to `readiness/agent-ready-verdict.md`
- [X] T004 [skillist: []] Run `./fake.sh build -t Route` against the working-tree diff and record the authoritative tier + minimal gate list to `readiness/target-metadata.md`

---

## Phase 2: Foundation

- [X] T005 [skillist: []] Draft the new public surface as `.fsi`: `src/SkillSupport/Wrap.fsi` (`module Wrap` with `val wrapDeltaX: worldWidth: float -> fromX: float -> toX: float -> float`) per `contracts/skillsupport-wrap-api.md`
- [X] T006 [skillist: []] Exercise the drafted `Wrap.fsi` from FSI (range `(-w/2, w/2]`, shortest-path examples `wrapDeltaX 100 90 10 = 20` / `wrapDeltaX 100 10 90 = -20`, identity `wrapDeltaX w a a = 0`) and capture the session transcript to `readiness/fsi-session.txt`
- [X] T007 [P] [skillist: []] Draft the `Wrap` addition to the existing per-package surface baseline `readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt` (authoritative `PerPackageSurfaceDiff` baseline; finalized against the built `.fsi` in T032)
- [X] T008 [skillist: []] Record unsupported-scope handling, governance risk levels (small/medium/broad), and aggregate-hang diagnostics into `readiness/runtime-limitations.md`, `readiness/governance-risk-levels.md`, and `readiness/aggregate-hang-diagnostics.md`

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) — Image evidence faithfully renders every scene primitive (P1)

### Tests First (Principle I, Principle VI)

- [X] T009 [P] [US1] [skillist: fs-skia-scene] Failing-first golden/pixel test: a scene containing `Line` + `Path` + `Text` rendered through the image-evidence path produces a decoded PNG with non-blank pixels in the expected regions (not a single 40×40 placeholder block), and `Text` produces glyph pixels (SC-001)
- [X] T010 [P] [US1] [skillist: fs-skia-evidence-mode] Failing-first test: a scene whose only content is a `Line` node can no longer pass a "scene is visible" check by node count alone via a placeholder substitution — node-count/`Scene.describe` is structural, the image is the visual proof (SC-002)

### Implementation

- [X] T011 [US1] [skillist: fs-skia-scene] Create the non-public `src/SkiaViewer/SceneRenderer.fs` shared painter — `paintNode: SKCanvas -> SceneNode -> unit` with an **exhaustive `match` over every `SceneNode` case (no wildcard)** — and move the paint helpers (`skColor`, `configurePaint`, `toSkPath`, `drawTextWithFallback`, support fns) out of `VulkanHost` into it (FR-001, D1; SC-002 compile guard)
- [X] T012 [US1] [skillist: fs-skia-scene] Render `Text`/`TextRun` as **real glyphs** via the moved `drawTextWithFallback` and delete the placeholder-rectangle substitution for `Text` (`SkiaViewer.fs:1796-1799`) (FR-001, D2)
- [X] T013 [US1] [skillist: fs-skia-skiaviewer] Add `SceneRenderer.fs` `Compile` entry to `SkiaViewer.fsproj` before its consumers; retype `drawScene` (`Host/Vulkan.fs:1005-1160`) and `drawScreenshotScene` (`SkiaViewer.fs:1771-1808`) to delegate to `SceneRenderer.paintNode`; delete the catch-all placeholder wildcard at `SkiaViewer.fs:1804-1806`. SkiaViewer per-package surface baseline is **unchanged** (shared module is non-public) (FR-001, D1/D11)
- [X] T014 [US1] [skillist: fs-skia-scene] Document the unified evidence/interactive renderer (one shared painter) and the render-backed primitive set in the `fs-skia-scene` skill (`src/Scene/skill/SKILL.md`), so node-count tests are understood as structural not visual proof (FR-002, D3)
- [X] T015 [US1] [skillist: fs-skia-skiaviewer] Capture the before/after image-evidence proof — before: `Line`/`Path` render blank/placeholder and `Text` is a box; after: terrain `Line` + filled-ground `Path` + real-glyph `Text` render to pixels and node-count no longer passes on an invisible scene — to `readiness/renderer-image-evidence.md` (SC-001/SC-002)

**Checkpoint**: User Story 1 is fully functional and testable independently.

---

## Phase 4: User Story 2 (US2) — Analyze can invoke the symbol cross-check deterministically (P1)

- [X] T016 [P] [US2] [skillist: fsharp-build-orchestration] Register the target: add `SymbolCrossCheck` to the `Target` DU, `allTargets`, `name`, and `directPrerequisites` in `build/Governance/Targets.fs`, and add `"SymbolCrossCheck"` to `ValidationContract.knownGates` in `build/Governance/AgentValidation.fs` (the separate allowlist — omitting it fails `Governance.Tests` with an unknown-gate diagnostic) (FR-003, D4)
- [X] T017 [US2] [skillist: fsharp-build-orchestration] Wire the effect: add `SymbolCrossCheckAnalyze` `BuildEffect` (`Engine/Model.fs`), `StartTarget Targets.SymbolCrossCheck` → effect + `RequireFiles` on the output (`Engine/Update.fs`), interpret it (`Engine/Interpret.fs`: resolve the feature dir, read `plan.md`/`data-model.md`/`tasks.md`, `SymbolCrossCheck.render (SymbolCrossCheck.diff …)`, print + write `readiness/symbol-cross-check.md`), and add the `focusedGateContract` case (`Front/Helpers.fs`). No new analyzer/renderer — reuse the existing `build/Governance/SymbolCrossCheck.fs` (FR-003, D4)
- [X] T018 [US2] [skillist: fsharp-build-orchestration] Regenerate `validation.contract.yml` via `./fake.sh build -t RefreshSurfaceBaselines` and confirm `TargetMetadataDrift` stays green and `Governance.Tests` reports no unknown-gate diagnostic (FR-011)
- [X] T019 [P] [US2] [skillist: speckit-analyze] Update analyze pass G in `.agents/skills/speckit-analyze/SKILL.md` to run `./fake.sh build -t SymbolCrossCheck` (consuming the compiled output) instead of "do not eyeball it" with no invocation path (FR-003, D4)
- [X] T020 [US2] [skillist: speckit-analyze] Verification: seed a deliberate `Msg`-case drift (present in `data-model.md` + `tasks.md` but absent from `plan.md`), run `./fake.sh build -t SymbolCrossCheck` from a read-only checkout, and confirm the proper-subset finding prints in the documented `## Symbol consistency (analyze pass G)` format and `readiness/symbol-cross-check.md` is written; confirm a no-drift run prints a well-formed empty section — no throwaway harness (SC-003)

**Checkpoint**: User Story 2 is fully functional and testable independently.

---

## Phase 5: User Story 3 (US3) — Evidence formats discoverable before authoring, diagnostics read clearly (P2)

### Tests First

- [X] T021 [P] [US3] [skillist: fsharp-parsing] Failing-first test: a readiness-contract failure with exactly **one** absent token prints the full required set and the absent subset under **distinct** labels (`full-required-set:` vs `absent-from-file:`), so one missing token does not read as "all missing" (SC-004)

### Implementation

- [X] T022 [US3] [skillist: fsharp-parsing] Relabel the readiness-contract diagnostic in `build/Governance/Evidence/Render.fs:471-480`: `required-tokens:` → `full-required-set:` and `missing:` → `absent-from-file:` (labels only — `Required = Some terms` and `MissingTerms` already exist in `Scans.fs:95-106`; no data shape change) (FR-004, D5)
- [X] T023 [P] [US3] [skillist: speckit-implement] Edit `.agents/skills/speckit-implement/SKILL.md`: add a pre-implementation pointer to read `docs/evidence-formats.md` **before** writing readiness/evidence files, and document that `skill-loading-evidence.md` is read from the **feature** readiness dir (`specs/<feature>/readiness/`, not repo-root), needs one row per (task, declared-skill) with `.agents/skills/<id>/SKILL.md` paths and `loaded_at < work_started_at`, and is **enforced only once tasks flip to `[X]`** (FR-004 / FR-005, D5)
- [X] T024 [US3] [skillist: []] Verification: trigger a one-absent-token readiness-contract failure and confirm the distinct labels print; confirm the regenerated `speckit-implement` skill body names `docs/evidence-formats.md` and the `skill-loading-evidence.md` location/timing — logged to `readiness/evidence-discoverability.md` (SC-004)

**Checkpoint**: User Story 3 is fully functional and testable independently.

---

## Phase 6: User Story 4 (US4) — Scaffold map and source spec are reproducible and discoverable (P2)

- [X] T025 [P] [US4] [skillist: speckit-plan] Add a pre-planning pointer in `.agents/skills/speckit-plan/SKILL.md` telling an author working on a generated product to read `docs/scaffold-map.md` before reconstructing the durable-vs-replaceable map by hand (FR-006, D6)
- [X] T026 [P] [US4] [skillist: fs-skia-layout-readability] Add an "API surface authority" note to `template/base/docs/scaffold-map.md`: the shipped `.fsi` surfaces / `docs/api-surface/` are the **authoritative** API reference and agent-generated API summaries (e.g. Explore output) are supporting reference only, never ground truth (FR-006, D6)
- [X] T027 [P] [US4] [skillist: speckit-specify] Extend `.agents/skills/speckit-specify/SKILL.md` step 3: when the feature input is an **external URL**, after fetching snapshot the source into `specs/<feature>/source-spec.md` (record the URL in a header) and reference the in-repo snapshot; for local-file or inline input the step is an explicit no-op (FR-007, D7)
- [X] T028 [US4] [skillist: fsharp-io-globbing] FR-008 disposition: run a template-wide scan confirming **no** generated artifact template seeds a divergent `evidence/` token (`.specify/templates/spec-template.md` references neither path; `tasks-template.md` uses `readiness/`; `template/base/docs/**` seeds no `specs/<feature>/evidence/`); record the consumer-authoring-only finding to `readiness/evidence-path-token-scan.md` and close with **no code change** (FR-008, D8)
- [X] T029 [US4] [skillist: []] Verification: the regenerated `speckit-plan` flow references `docs/scaffold-map.md` and the map carries the `.fsi`/`docs/api-surface`-authoritative note; specifying from a URL yields an in-repo `source-spec.md` snapshot while local input creates no redundant copy; the evidence-path token question is resolved (SC-005)

**Checkpoint**: User Story 4 is fully functional and testable independently.

---

## Phase 7: User Story 5 (US5) — Recurring game helpers dispositioned, not silently re-deferred (P3, Tier 1)

### Tests First

- [X] T030 [P] [US5] [T1] [skillist: fs-skia-layout-readability] Failing-first Expecto tests for `Wrap.wrapDeltaX`: range `result ∈ (-worldWidth/2, worldWidth/2]` for `worldWidth > 0`, shortest-path examples (`wrapDeltaX 100 90 10 = 20`, `wrapDeltaX 100 10 90 = -20`), identity (`wrapDeltaX w a a = 0`), and symmetry (`wrapDeltaX w a b = -(wrapDeltaX w b a)` except at the `+w/2` boundary) (SC-006)

### Implementation

- [X] T031 [US5] [T1] [skillist: fs-skia-layout-readability] Implement `src/SkillSupport/Wrap.fs` (pure, float-only, no `Scene`/`Layout` dependency) against the drafted `.fsi`, and add `Wrap.fsi`/`Wrap.fs` `Compile` entries (`.fsi` before `.fs`, after `Hud`) to `src/SkillSupport/SkillSupport.fsproj` (FR-010, D10)
- [X] T032 [US5] [T1] [skillist: fs-skia-template-update] Finalize `readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt` against the built `.fsi` (adds the `Wrap` module) and confirm `PackageSurfaceCheck`/`PerPackageSurfaceDiff` green (FR-011, Principle II)
- [X] T033 [US5] [skillist: fs-skia-layout-readability] Add the `wrapDeltaX` skill reference to `fs-skia-layout-readability` (alongside the existing `reserveHudBand` note) and document the **deferred** camera-centered projection (closure over per-game state, soft `Scene.Point` dependency, varies per game) with rationale + next-recurrence bar (FR-010, D10)
- [X] T034 [US5] [skillist: fs-skia-evidence-mode] Document the `--evidence-run` deterministic-summary **discipline** (pure model + per-frame held-input script + `InvariantCulture`/`F3` float formatting + `determinism=byte-identical` marker) in `fs-skia-evidence-mode` with the LunarLander1 / AsteroidsDemo3 functions as canonical examples, and record the deferral rationale (field set varies per game) + next-recurrence bar (a stable cross-game field set) (FR-009, D9)
- [X] T035 [US5] [skillist: []] Record the per-helper dispositions — **ship** `wrapDeltaX`; **document** the camera projection; **document + defer** the `--evidence-run` summary pattern — each with rationale and next-recurrence bar to `readiness/helper-dispositions.md`, so no candidate is silently dropped (SC-006)

**Checkpoint**: User Story 5 is fully functional and testable independently.

---

## Phase 8: Integration & Polish

- [X] T036 [skillist: fs-skia-template-update] Regenerate `.claude` from `.agents` (`./fake.sh build -t RefreshSurfaceBaselines`) after all skill/doc edits and confirm `SkillSyncCheck`/`SkillQualityCheck`/`TargetMetadataDrift` stay green; confirm `PerPackageSurfaceDiff` zero-drift for the finalized SkillSupport baseline (FR-011)
- [X] T037 [skillist: fs-skia-template-update] Run `TemplateCheck` (PASS — generated projects ship the faithful image-evidence renderer, the regenerated phase skills + `evidence-formats`/`scaffold-map` pointers, and the `wrapDeltaX` helper) + `GeneratedProductCheck` (EXPECTED-FAIL non-regression: a feature-less scaffold has no `feature_directory`; the aggregate is non-authoritative, the authoritative verdict is `EvidenceAudit verdict=PASS`); record the non-authoritative aggregate notes in `readiness/target-metadata.md`
- [X] T038 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]`; the effective-DAG render (explicit deps + auto-injected checkpoint edges + resolved skillist set) is written to `readiness/task-graph.md`
- [X] T039 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm `verdict=PASS` for `specs/063-lunar-lander-consumer-friction-followups` with no `[S]`/`[S*]` and no diff-scan hits, and that all Route-printed gates pass including the new `SymbolCrossCheck` wiring and the SkillSupport surface baseline (SC-007)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

No synthetic evidence is planned — all evidence is real (real before/after image
capture, real gate runs, real `Wrap`/symbol-diff/diagnostic unit tests; see plan
§Synthetic evidence). `[S]` disclosure applies only if a real path proves
infeasible mid-implementation; none anticipated, and no `[SEH]` cases are foreseen.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |

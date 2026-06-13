# Tasks: Layout Hot-Path Improvements

**Feature branch**: `117-layout-hot-path`
**Spec**: `specs/117-layout-hot-path/spec.md`
**Plan**: `specs/117-layout-hot-path/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]` or
`[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the evidence audit.
See `readiness/task-graph.md` for the propagated view.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- This whole feature is **Tier 1** (a breaking `ControlsElmish.fsi` `FrameMetrics`
  change — three new public fields: `TextMeasureCacheHitCount`,
  `TextMeasureCacheMissCount`, `LayoutInvalidatedNodeCount` — plus an internal
  `RetainedRender`/`WorkReductionRecord` text-measure-cache + dirty-set seam; the
  top-level surface baseline and per-package baselines move); per-task `[T1]/[T2]`
  annotations are omitted because every phase matches the feature tier.

## Elmish/MVU applicability

Principle IV's dedicated `Model`/`Msg`/`Effect`/`init`/`update`/interpreter tasks are
**N/A** for this feature: `Update`, effects, subscriptions, commands, and the interpreter
are unchanged (FR-004/state-workflow impact). The bounded text-measure cache + always-miss
flag, the hit/miss counts, and the layout-invalidated dirty-set size all live in the
retained render / measure path (interpreter-edge mutation confined to the step,
constitution III, exactly as the existing id/work counters, the 113 memo cache, and the
116 picture cache); `view`/`update` stay pure and dispatch outcomes are byte-identical
(FR-004). The interactive-UI run-and-use gate is **N/A** — the feature delivers an internal
text-measure-cache contract + deterministic metrics observable via
`ControlsElmish.Perf.runScript`, not a new interactive surface. Recorded in the
evidence-obligations task (T003 / T007).

## Governance risk level

**Medium** governance risk: the breaking `FrameMetrics` `.fsi` change (three new fields)
escalates `Route` to the **controls-public-surface** tier and moves the top-level +
per-package surface baselines, but there is **no new gate**, no dependency change, and no
template-content change. Focused validation = the escalated gate set `Route` prints (T023).
Broad validation (full `Verify`) is not required because the change set is two packages'
contents (`FS.Skia.UI.Controls`, `FS.Skia.UI.Controls.Elmish`) plus the regenerated
baselines + perf-corpus goldens. Non-authoritative aggregate results are recorded as
"focused rerun" notes in `readiness/aggregate-hang-diagnostics.md`.

## Success-criterion → assertion mapping

- **SC-001/SC-002** (warm hits / cold misses) → `Feature117MetricsTests` cold-frame-misses-
  then-warm-frame-hits golden over `Perf.runScript` (T018) + `Feature117TextCacheTests`
  cold→warm assertion (T008).
- **SC-003** (style-only zero work) → `Feature117LayoutInvalidatedTests` /
  `Feature117MetricsTests` style-only frame asserts `RemeasuredNodeCount = 0`,
  `LayoutInvalidatedNodeCount = 0`, text-cache misses `= 0` (T012/T015/T018).
- **SC-004** (byte-identity) → `Feature117TextCacheTests` always-miss oracle (cache-on ≡
  cache-off) + the standing Scene-parity golden suite under `Dev` (T008/T023).
- **SC-005** (bounded cap) → `Feature117CacheBoundTests` `Entries.Count <= cap` +
  deterministic eviction + evicted-entry re-miss (T009).
- **SC-006** (geometry `<= RemeasuredNodeCount`) → `Feature117LayoutInvalidatedTests`
  geometry-frame bounded-and-`>=` assertion (T012).
- **SC-007** (routed gate set + audit zero-synthetic) → T023/T024/T025.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm `specs/117-layout-hot-path/` carries spec + plan + research + data-model + contracts (`text-measure-cache-contract.md`, `layout-invalidated-metric-contract.md`) + quickstart + checklist (`checklists/requirements.md`) and that they are linked and current
- [X] T002 [P] [skillist: []] Create the `specs/117-layout-hot-path/readiness/` scaffolds discoverable before implementation — `evidence-audit.md`, `evidence-graph.md`, `skill-loading-evidence.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation.md`, `byte-identity-authority.md`, `text-cache-authority.md`, and the window-visibility not-applicable set — each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [skillist: []] Record feature Tier (Tier 1), affected packages (`FS.Skia.UI.Controls` — the internal bounded text-measure cache + `TextCacheEnabled` always-miss flag interposed over `Scene.measureText` at the six `Control.fs` call sites + `fittedFontSize`, the `WorkReductionRecord` hit/miss + layout-invalidated carriers, and the dirty-set-size threading; `FS.Skia.UI.Controls.Elmish` — the three public `FrameMetrics` fields), public-API impact (breaking `FrameMetrics` `.fsi` + internal `RetainedRender`/`WorkReductionRecord` seam reached via `InternalsVisibleTo`), Elmish/MVU + interactive-UI applicability (both N/A with the rationale above), and the required evidence obligations (cold-miss→warm-hit, per-keyed-input miss + hit byte-identity + always-miss oracle, bounded cap + deterministic eviction + evicted-entry re-miss, empty/whitespace + fitted-caption, layout-invalidated `<= RemeasuredNodeCount` + idle/style-only `= 0`, the three deterministic `FrameMetrics` goldens, at-rest byte-identity via the Scene-parity suite, baselines, XML-doc, drift-guard-still-empty, 113/114/116 composition)

---

## Phase 2: Foundation

- [X] T004 [skillist: fs-skia-controls-host, fs-skia-reconciliation] Draft the public + internal surfaces as `.fsi` signatures (XML-doc each, attribute-before-doc-before-type ordering): in `src/Controls.Elmish/ControlsElmish.fsi` add the three public `FrameMetrics` fields `TextMeasureCacheHitCount: int` / `TextMeasureCacheMissCount: int` / `LayoutInvalidatedNodeCount: int`; in `src/Controls/RetainedRender.fsi` add the internal `WorkReductionRecord` carriers `TextMeasureCacheHits` / `TextMeasureCacheMisses` / `LayoutInvalidatedNodeCount` plus the internal `TextMeasureKey` cache-key record, the internal bounded `TextMeasureCache` store, and the `TextCacheEnabled: bool` always-miss flag on `RetainedRender` (mirroring `MemoEnabled` `:81`/`.fsi:130` and `PictureCacheEnabled` `:87`/`.fsi:145`). Build compiles (signatures only)
- [X] T005 [skillist: fs-skia-controls-host] Exercise the drafted seam from FSI (`scripts/prelude.fsx` or ad-hoc): construct a `FrameMetrics` carrying the three new fields and show the `Perf.runScript` shape, toggle the `TextCacheEnabled` oracle, and print a `TextMeasureKey` round-trip; capture the session transcript to `readiness/fsi-session.txt`
- [X] T006 [P] [skillist: []] Capture the intended top-level (`FrameMetrics` three fields) + per-package (Controls `RetainedRender` internal cache/flag/carriers, Controls.Elmish `FrameMetrics`) surface baseline shape (the authoritative regen happens in T021) and note it in `readiness/`
- [X] T007 [P] [skillist: []] Record unsupported-scope handling and failure diagnostics: OUT this rung — structural-wrapper flattening (report task 4, semantic-change risk, no byte-identical guarantee); intrinsic / multi-pass layout introduction and any multi-pass metric (report optional task 5 — no such path exists, this rung must not create one, FR-009; the single-measure-pass contract is verified negatively — by the absence of any new multi-pass metric plus the still-empty layout drift guard asserted in T012); `SkiaViewer` frame-scheduling / readback separation / scene-submission / layer-skipping / render-thread / compositor split (Phase 9); GPU / layer caching; any timing-based pass/fail gate (the metrics are counts, not durations); the text-cache raw entry-count / byte-size is an internal invariant proven by test, not a public `FrameMetrics` field (unlike 116's `PictureCacheEntryCount`); features 109–116 unchanged; Principle IV + interactive-UI gate N/A

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — Text-heavy frames stop re-measuring identical text

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US1] [skillist: fs-skia-reconciliation, fs-skia-evidence-mode] Add a failing-first `Feature117TextCacheTests` in `tests/Controls.Tests` (reaching internal seams via `InternalsVisibleTo "Controls.Tests"`): a cold first measurement of a key is a miss, a second identical-key measurement is a hit served without re-invoking `Scene.measureText` (FR-001, SC-002→SC-001); perturbing **exactly one** keyed input in turn (text | font family | font size | font weight) each independently forces a miss with the correct fresh `TextMetrics`, proving no keyed input is omitted (FR-002); a hit's returned `TextMetrics` is byte-identical to the un-cached measure (FR-004, SC-004); the same scenarios run with the cache **disabled** (the `TextCacheEnabled` always-miss oracle) produce identical measured width/height and identical layout — cache-on ≡ cache-off (FR-004, SC-004); empty and whitespace text measure and cache without error and stay byte-identical (edge case); a `fittedFontSize` auto-fit caption's distinct candidate sizes are distinct keys, the cache helps across frames, and the chosen fitted size is unchanged (edge case)
- [X] T009 [P] [US1] [skillist: fs-skia-reconciliation, fs-skia-evidence-mode] Add a failing-first `Feature117CacheBoundTests` in `tests/Controls.Tests`: drive a scenario measuring more distinct strings than the cap and assert `Entries.Count <= cap` at all times (FR-003, SC-005); eviction is deterministic — the same input order yields the same surviving entries and the same hit/miss/eviction sequence (FR-003, SC-005); an evicted entry re-misses (fresh, correct measure) when next needed, never a stale hit (FR-003, SC-005)
- [X] T010 [US1] [skillist: fs-skia-reconciliation] Implement the bounded text-measure cache in `src/Controls/RetainedRender.fs` (+ wiring at the six `Control.fs` `measureText` call sites: `fittedFontSize` `:239`, `buttonGeom` `:786`, `badgeGeom` `:821`, `textFieldGeom` `:966`, `textAreaFieldGeom` `:996`, `richTextGeom` `:1017`): a `TextMeasureKey` ( text, family, size, weight ) keyed lookup — resident key returns the cached `TextMetrics` and counts a hit; an absent/evicted key calls `Scene.measureText`, inserts (evicting LRU at cap, deterministic traversal-ordered recency like 116), and counts a miss; populate the `WorkReductionRecord` hit/miss carriers; the `TextCacheEnabled = false` oracle forces every request to re-measure (hits = 0, byte-identical output/layout). Emitted `SubtreeScene`, layout boxes, and fitted font sizes byte-identical at rest. Make T008 + T009 pass (FR-001/FR-002/FR-003/FR-004)
- [X] T011 [US1] [skillist: []] Document the US1 independent validation path (drive a repeated-caption text-heavy layout cold→warm through `Perf.runScript`; assert warm-frame hits + zero misses, cold-frame misses; per-keyed-input miss matrix; always-miss oracle equivalence) in `readiness/us1-validation.md`

**Checkpoint**: User Story 1 is functional and independently testable.

---

## Phase 4: User Story 2 (US2) — Layout dirty propagation is observable by count

### Tests First

- [X] T012 [P] [US2] [skillist: fs-skia-reconciliation, fs-skia-evidence-mode] Add a failing-first `Feature117LayoutInvalidatedTests` in `tests/Controls.Tests`: an idle frame reports `LayoutInvalidatedNodeCount = 0` (FR-006); a style-only / visual-state-only frame reports `LayoutInvalidatedNodeCount = 0` and `RemeasuredNodeCount = 0` (FR-006/FR-007, SC-003); a geometry-changing frame (width/height/orientation) reports a bounded, explainable `LayoutInvalidatedNodeCount` that is `<= RemeasuredNodeCount` — fixed-size-ancestor propagation expands the pre-pinning dirty set into the re-measured boundary subtree (direction corrected 2026-06-13) (FR-006, SC-006); the feature 101 drift guard (`layoutDriftReport` over `layoutAffectingAttrNames` `Control.fs:1252`) still reports **empty** drift — this rung adds no new geometry-driving attribute (FR-008)
- [X] T013 [US2] [skillist: fs-skia-reconciliation] Implement `LayoutInvalidatedNodeCount` threading in `src/Controls/RetainedRender.fs`: surface `Set.count` of the dirty set produced by `layoutDirtySet` (`:497-504`) and fed to `evaluateIncremental` (`Control.fs:1307`), as a `WorkReductionRecord` carrier distinct from the post-pinning `RemeasuredNodeCount` (`layoutResult.Invalidated |> List.length` `:575`); `invalidated <= remeasured` always holds (the pre-pinning dirty set is a subset of the post-pinning re-measured boundary subtrees; direction corrected 2026-06-13). Reporting-only, no layout-box change, byte-identical at rest. Make T012 pass (FR-006/FR-007/FR-008)
- [X] T014 [US2] [skillist: []] Document the US2 independent validation path (style-only frame zero invalidated/zero remeasured; geometry frame bounded invalidated `<= RemeasuredNodeCount`; drift guard empty) in `readiness/us2-validation.md`

**Checkpoint**: User Story 2 is functional and independently testable.

---

## Phase 5: User Story 3 (US3) — Style-only and visual-state-only updates remeasure nothing

### Tests First

- [X] T015 [P] [US3] [skillist: fs-skia-reconciliation, fs-skia-evidence-mode] Add a failing-first style-only / visual-state-only zero-work assertion (in `Feature117LayoutInvalidatedTests` or a sibling in `tests/Controls.Tests`): a hover / focus / press / animation-tick frame over a text-bearing control re-measures **zero** layout nodes (`RemeasuredNodeCount = 0`), reports **zero** `LayoutInvalidatedNodeCount`, and produces **zero** text-measure cache misses for unchanged text (every measurement served from the warm cache), while remaining byte-identical at rest (FR-007, SC-003)
- [X] T016 [US3] [skillist: fs-skia-reconciliation] Wire / confirm the style-only zero-work guarantee on the retained step (`src/Controls/RetainedRender.fs`): a visual-state-only update routes through incremental layout with an empty dirty set (zero invalidated, zero remeasured) and unchanged text serves text-cache hits (zero misses); this is largely an assertion over behavior 096/112/113 already produce, formalized here as a deterministic gate. Make T015 pass (FR-007)
- [X] T017 [US3] [skillist: []] Document the US3 independent validation path (scripted hover/focus/visual-state frame asserts zero remeasure / zero invalidated / zero text-cache miss, byte-identical output) in `readiness/us3-validation.md`

**Checkpoint**: User Story 3 is functional and independently testable.

---

## Phase 6: Metrics surface & deterministic corpus

- [X] T018 [P] [skillist: fs-skia-controls-host, fs-skia-evidence-mode] Add a failing-first `Feature117MetricsTests` in `tests/Elmish.Tests` over `ControlsElmish.Perf.runScript`: the three `FrameMetrics` fields (`TextMeasureCacheHitCount`, `TextMeasureCacheMissCount`, `LayoutInvalidatedNodeCount`) are recorded deterministically and golden-asserted (FR-005/FR-006/FR-010); a cold text-heavy frame reports misses then a warm frame reports hits + zero misses (SC-001/SC-002); a style-only frame reports zero misses / zero invalidated / zero remeasured (FR-007, SC-003); an idle frame reports all three `= 0` (FR-005/FR-006); the counts aggregate correctly over the virtualized (114) row set (FR-008); a regression that re-shapes identical text or silently widens the dirty set fails a golden (FR-005/FR-006)
- [X] T019 [skillist: fs-skia-controls-host, fs-skia-reconciliation] Thread the three step carriers (`WorkReduction.{TextMeasureCacheHits, TextMeasureCacheMisses, LayoutInvalidatedNodeCount}`) into `FrameMetrics` in `src/Controls.Elmish/ControlsElmish.fs` exactly as `MemoHitCount`/`MemoMissCount` (113), `VirtualItems*` (114), and `PictureCache*` (116): the `zero` record (`:1366-1388`) carries `0` and **every** per-frame construction site (`:1421-1442` move, `:1478-1496` tick, key/pointer frames) lifts them from `lastWorkReduction`; surface through `Perf.runScript` and the live `OnFrameMetrics` sink (`:1003`); plumb the `TextCacheEnabled` oracle for the tests. Make T018 pass (FR-005/FR-006)
- [X] T020 [skillist: fs-skia-evidence-mode] Add the new corpus scenarios (a text-heavy repeated-caption cold→warm layout, a style-only / visual-state zero-work frame, and a cache-cap eviction layout) to the `Perf.runScript` corpus and regenerate the corpus goldens (`PERF_CORPUS_REGEN=1`) so they carry the three new metric fields; confirm the rendered scenes are otherwise unchanged (additive only) (FR-010, SC-006)

**Checkpoint**: The three `FrameMetrics` fields are observable and golden-asserted.

---

## Phase 7: Integration & Polish

- [X] T021 [skillist: fs-skia-ui-widgets] Run `./fake.sh build -t RefreshSurfaceBaselines` to regenerate the top-level public surface baseline (the three new `FrameMetrics` fields) and the per-package Controls/Controls.Elmish baselines (the internal `RetainedRender` text-measure-cache + `TextCacheEnabled` flag + `WorkReductionRecord` carriers); update any `FrameMetrics` construction sites or sample preludes it flags
- [X] T022 [skillist: []] Confirm the three new `FrameMetrics` fields satisfy the doc-preservation / XML-doc gate (`///` before each field, attribute-before-doc-before-type), and that no unrelated public function signature changed (additive `FrameMetrics` fields only; the `RetainedRender`/`WorkReductionRecord`/text-measure-cache additions stay internal)
- [X] T023 [skillist: fs-skia-template-update, fs-skia-controls-host] Run the escalated controls-public-surface gates sequentially as `Route` prints them — `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, the package/per-package surface diffs, `FsiTranscripts`, the controls catalog/doc/interaction/rendering checks, and `TemplateDrift` — confirming the standing Scene-parity golden suite (at-rest byte-identity, FR-004/SC-004) under `Dev` passes, and record the focused governance risk level + non-authoritative aggregate notes in `readiness/`
- [X] T024 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises, and the echoed `feature-directory`/`tasks=<n>` match this feature
- [X] T025 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with no remaining `[S]`/`[S*]` and no diff-scan hits, or document every `--accept-synthetic` override (SC-007)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the source
for the PR description's synthetic-evidence section. For `[SEH]` rows, include the
approval label, design-phase source, synthetic input class, expected error behavior, and
reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |

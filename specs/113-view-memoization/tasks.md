# Tasks: View Memoization and Stable Dependency Contracts

**Feature branch**: `113-view-memoization`
**Spec**: `specs/113-view-memoization/spec.md`
**Plan**: `specs/113-view-memoization/plan.md`

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
- **[US1]**, **[US2]**, **[US3]**, **[US4]** — user-story scope
- This whole feature is **Tier 1** (a breaking `ControlsElmish.fsi` `FrameMetrics`
  change + a new public `Controls` `Diagnostics` `val` + a new internal `Controls` memo
  seam — the top-level surface baseline and per-package baselines move); per-task
  `[T1]/[T2]` annotations are omitted because every phase matches the feature tier.

## Elmish/MVU applicability

Principle IV's dedicated `Model`/`Msg`/`Effect`/`init`/`update`/interpreter tasks are
**N/A** for this feature: it adds a pure-performance memoization boundary inside the
existing control-lowering / retained step plus two additive `FrameMetrics` fields and a
report-only diagnostic. `Update`, effects, subscriptions, commands, and the interpreter
are unchanged; the memo cache lives in the retained interpreter-edge state, not in
`update`; dispatch *outcomes* stay byte-identical (FR-014). The interactive-UI
run-and-use gate is also **N/A** — the feature delivers an internal seam + deterministic
metrics observable via `ControlsElmish.Perf.runScript`, not a new interactive surface.
Recorded in the evidence-obligations task (T003 / T008).

## Governance risk level

**Medium** governance risk: the breaking `FrameMetrics` `.fsi` change + the new public
`Diagnostics` `val` + the new internal memo seam escalate `Route` to the
**controls-public-surface** tier and move the top-level + per-package surface baselines,
but there is **no new gate** (the stability diagnostic is report-only, clarified
2026-06-12), no dependency change, and no template-content change. Focused validation =
the escalated gate set `Route` prints (T022). Broad validation (full `Verify`) is not
required because the change set is two packages' contents plus the regenerated
baselines + perf-corpus goldens. Non-authoritative aggregate results are recorded as
"focused rerun" notes in `readiness/aggregate-hang-diagnostics.md`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold `specs/113-view-memoization/` and confirm spec + plan + research + data-model + contracts (`memoization-seam.md`, `stability-diagnostic.md`) + quickstart + checklist are linked and current
- [X] T002 [P] [skillist: []] Create the `specs/113-view-memoization/readiness/` scaffolds discoverable before implementation — `evidence-audit.md`, `evidence-graph.md`, `skill-loading-evidence.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation.md`, `byte-identity-authority.md`, `memo-metrics-authority.md`, and the window-visibility not-applicable set — each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [skillist: []] Record feature Tier (Tier 1), affected packages (`FS.Skia.UI.Controls` — internal `RetainedRender` memo seam + cache types, the DataGrid projection site in `Control.fs`, the public `Diagnostics` stability-report `val`; `FS.Skia.UI.Controls.Elmish` — public `FrameMetrics` `MemoHitCount`/`MemoMissCount`), public-API impact (breaking `FrameMetrics` `.fsi` + new public `Diagnostics` `val` + internal memo seam reached via `InternalsVisibleTo`), Elmish/MVU + interactive-UI applicability (both N/A with the rationale above), and the required evidence obligations (memo hit/miss/cold, memo-on/memo-off scene parity + no-staleness, deterministic count goldens, stability-diagnostic flag/no-flag, stable-props page, baselines, XML-doc)

---

## Phase 2: Foundation

- [X] T004 [skillist: fs-skia-reconciliation] Draft the public + internal surfaces as `.fsi` signatures (XML-doc each): in `src/Controls/RetainedRender.fsi` add `type internal MemoEntry` (`Dependency: obj` — a **boxed** deterministic value compared by F# structural `=`, never object identity, FR-005; `Subtree: Scene list` — the lowered fragment, a reference type so a hit returns the **same instance**, specialized to `Scene list` this rung because the DataGrid projection is the sole memoized site; widening the stored subtree type travels with the deferred `Style.resolve` site), `type internal MemoCache` (`Map<ControlId, MemoEntry>`), `type internal MemoOutcome` (`Hit | Miss`), the memo slot on the retained per-identity state (or a sibling memo map on `RetainedRender`), `val internal memoize` (`ControlId -> dep -> thunk -> MemoCache -> subtree * MemoCache * MemoOutcome`), and the always-miss switch (FR-008); in `src/Controls.Elmish/ControlsElmish.fsi` add the public `MemoHitCount: int` / `MemoMissCount: int` `FrameMetrics` fields; in `src/Controls/Diagnostics.fsi` add the public stability-diagnostic `val` returning `ControlDiagnostic list`. Build compiles (signatures only)
- [X] T005 [skillist: fs-skia-reconciliation] Implement the `memoize` seam + always-miss switch in `src/Controls/RetainedRender.fs`: a `Hit` returns the stored `Subtree` instance without running the thunk and an `entry exists + dep equal`; an unequal/cold dep runs the thunk and stores `{ Dependency = dep; Subtree = result }` under the `ControlId` (C1–C4); thread the frame's aggregated `MemoHits`/`MemoMisses` onto the `step` result record alongside `WorkReductionRecord`/`RemeasuredNodeCount` (C7); always-miss mode forces every call to `Miss` with nothing reused (C4/FR-008). Build compiles
- [X] T006 [skillist: fs-skia-reconciliation] Exercise the drafted seam shape from FSI (build a tree + a `ControlId`, call `memoize` with an equal then a changed dependency over a thunk instrumented to record invocation, print the `MemoOutcome` and reuse) and capture the session transcript to `readiness/fsi-session.txt`
- [X] T007 [skillist: []] Capture the intended top-level (`FrameMetrics` fields) + per-package (`Diagnostics` `val`, internal memo seam/types) surface baseline shape (the authoritative regen happens in T020) and note it in `readiness/`
- [X] T008 [skillist: []] Record unsupported-scope handling and failure diagnostics: Phase 6+ is OUT (virtualization, paint/damage caches, layout caches, backend review); no public `Control.memo`/`Widget.memo` primitive (deferred); no enforced stability gate (report-only); only a representative memoized site (DataGrid projection), not the full 52-control migration; the seam misses (never reuses) on an unequal/unknown dependency, so a too-coarse dependency is caught by the memo-on/memo-off parity test, never a stale render (FR-007); features 110/111/112 unchanged (FR-015); Principle IV + interactive-UI gate N/A

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — An expensive control-internal transform is reused when its inputs are unchanged

### Tests First (Principle I, Principle VI)

- [X] T009 [P] [US1] [skillist: fs-skia-reconciliation, fs-skia-evidence-mode] Add a failing-first `Feature113MemoSeamTests` in `tests/Controls.Tests` (reaching the internal seam via `InternalsVisibleTo "Controls.Tests"`): a steady-state stable dependency → `Hit` with the subtree reference-reused and the thunk **not** run (instrument the thunk to assert non-invocation); a changed dependency → `Miss` + fresh subtree; a cold first frame (no prior entry) → `Miss`; the seam **never** reuses across an unequal/unknown dependency (FR-001/FR-004/FR-005, C1–C3, SC-001)
- [X] T010 [US1] [skillist: fs-skia-ui-widgets] Wrap the **DataGrid row/column projection** (`Control.fs` `gridGeom` / the `cells → Scene` projection, ~`Control.fs:550`) in the `memoize` seam keyed by the DataGrid's `ControlId` + a deterministic dependency value capturing every input that can change the projection (cell/column data + theme/geometry); a steady-state frame (unchanged data + theme) hits and reuses the prior projected subtree. Make T009 pass (FR-003/FR-004)
- [X] T011 [US1] [skillist: []] Document the US1 independent validation path (render the same model twice through `Perf.runScript` for a scenario with a memoizable DataGrid whose data + theme are unchanged; second frame records the hit and reuses the subtree) in `readiness/`

**Checkpoint**: User Story 1 is functional and independently testable.

---

## Phase 4: User Story 2 (US2) — Memoized output is byte-identical to the non-memoized build

### Tests First

- [X] T012 [P] [US2] [skillist: fs-skia-ui-widgets, fs-skia-evidence-mode] Add a failing-first `Feature113MemoParityTests` in `tests/Controls.Tests`: for representative scenarios and frame sequences, each frame's rendered scene built with memoization active equals the scene built memo-off (forced always-miss) — structural `Scene` equality (controls have no value equality) — and equals the pre-feature baseline (FR-006, C5, SC-002); include a scenario that mutates the memoized DataGrid's real inputs and assert the memoized build reflects the change (a `Miss` occurs; no stale subtree reused) (FR-007, C6, SC-003)
- [X] T013 [US2] [skillist: fs-skia-ui-widgets, fs-skia-reconciliation] Ensure the dependency value captures **every** input that can change the memoized subtree so memo-on ≡ memo-off for every frame and a real-input change produces a `Miss` and a fresh subtree (no staleness); confirm always-miss mode is byte-identical to the pre-feature baseline. Make T012 pass (FR-006/FR-007/FR-008, SC-002/SC-003)

**Checkpoint**: User Story 2 is functional and independently testable.

---

## Phase 5: User Story 3 (US3) — Memo work is observable as deterministic metrics

### Tests First

- [X] T014 [P] [US3] [skillist: fs-skia-controls-host, fs-skia-evidence-mode] Add a failing-first `Feature113MemoMetricsTests` in `tests/Elmish.Tests` over `ControlsElmish.Perf.runScript`: a steady-state scenario (memoized control's inputs unchanged across frames) accrues `MemoHitCount > 0` with `MemoMissCount = 0` for that site on the steady frames; a perturbed scenario (inputs changed each frame) and a cold first frame accrue `MemoMissCount`; an idle frame that evaluates no memoizable control reports both `0`. Counts are deterministic and golden-asserted (FR-009/FR-010, C7/C8, SC-004)
- [X] T015 [US3] [skillist: fs-skia-controls-host] Thread the retained step's `MemoHits`/`MemoMisses` into `FrameMetrics.MemoHitCount`/`MemoMissCount` in `src/Controls.Elmish/ControlsElmish.fs` — the `zero` record carries both `0` and **every** per-frame construction site (pointer-move, tick, key, idle, model branches) sets them from the last retained-step record; surface them through `Perf.runScript` and the live `OnFrameMetrics` sink. Make T014 pass (FR-009/FR-010)
- [X] T016 [US3] [skillist: fs-skia-evidence-mode] Regenerate the `Perf.runScript` corpus goldens to carry the two new metric fields (`PERF_CORPUS_REGEN=1 dotnet test tests/Elmish.Tests --filter Feature109CorpusTests`) and confirm the regenerated goldens show the expected hits/misses/idle-0/0 and the rendered scenes are otherwise unchanged (additive only)

**Checkpoint**: User Story 3 is functional and independently testable.

---

## Phase 6: User Story 4 (US4) — Unstable inputs that defeat reuse are diagnosable

### Tests First

- [X] T017 [P] [US4] [skillist: fs-skia-ui-widgets, fs-skia-evidence-mode] Add a failing-first `Feature113StabilityDiagTests` in `tests/Controls.Tests`: a fixture tree built twice with stable attributes/events → the stability-diagnostic report returns **no** findings; the same tree with an injected always-new attribute / per-frame closure → the report **flags** that input as a reuse-breaking instability, naming the control (`ControlId` + `ControlKind`) and the offending attribute/event (FR-011/FR-012, SC-005)
- [X] T018 [US4] [skillist: fs-skia-ui-widgets] Implement the public stability-diagnostic `val` in `src/Controls/Diagnostics.fs` — a two-build parallel walk of the same logical (sub)tree returning one `ControlDiagnostic` per attribute/event that compared **unequal** despite no semantic change (rebuilt `UntypedValue`, per-frame closure, rebuilt list, unstable key), reusing the existing `ControlDiagnostic` vocabulary (add a `ControlDiagnosticCode` for the instability class if needed); empty list ⇒ stable. Make T017 pass (FR-011/FR-012)
- [X] T019 [US4] [skillist: fs-skia-ui-widgets] Author the author-facing **stable-props guidance page** at `docs/controls/stable-props.md` naming the concrete reuse-breaking patterns (rebuilt `UntypedValue`, per-frame event closures, rebuilt lists, unstable keys) and how to make each input stable (FR-013/SC-005)

**Checkpoint**: User Story 4 is functional and independently testable.

---

## Phase 7: Integration & Polish

- [X] T020 [skillist: fs-skia-ui-widgets] Run `./fake.sh build -t RefreshSurfaceBaselines` to regenerate the top-level public surface baseline (the new `FrameMetrics.MemoHitCount`/`MemoMissCount` fields) and the per-package Controls/Controls.Elmish baselines (the public `Diagnostics` `val`; the internal memo seam + cache/entry types); update any construction sites or sample preludes it flags
- [X] T021 [skillist: fs-skia-ui-widgets] Confirm the new `FrameMetrics` fields, the `Diagnostics` `val`, and the internal memo seam/types satisfy the doc-preservation / XML-doc gate, and that no unrelated public function signature changed
- [X] T022 [skillist: fs-skia-template-update, fs-skia-controls-host] Run the escalated controls-public-surface gates sequentially as `Route` prints them — `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, the package/per-package surface diffs, `FsiTranscripts`, the controls catalog/doc/interaction/rendering checks, and `TemplateDrift` — and record the focused governance risk level + non-authoritative aggregate notes in `readiness/`
- [X] T023 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises, and the echoed `feature-directory`/`tasks=<n>` match this feature
- [X] T024 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with no remaining `[S]`/`[S*]` and no diff-scan hits, or document every `--accept-synthetic` override

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the source
for the PR description's synthetic-evidence section. For `[SEH]` rows, include the
approval label, design-phase source, synthetic input class, expected error behavior, and
reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |

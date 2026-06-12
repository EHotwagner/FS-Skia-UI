# Tasks: Frame Scheduler & Phase-Invalidation Model (Explain and Schedule Frames by Cause)

**Feature branch**: `111-frame-scheduler-invalidation`
**Spec**: `specs/111-frame-scheduler-invalidation/spec.md`
**Plan**: `specs/111-frame-scheduler-invalidation/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]` or
`[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the evidence
audit. See `readiness/task-graph.md` for the propagated view.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- This whole feature is **Tier 1** (the `ControlsElmish.fsi` `FrameCause` type +
  `FrameMetrics` fields are a breaking public surface change); per-task `[T1]/[T2]`
  annotations are omitted because every phase matches the feature tier.

## Elmish/MVU applicability

Principle IV's dedicated `Model`/`Msg`/`Effect`/`init`/`update`/interpreter tasks are
**N/A** for this feature: it is a per-frame scheduling/observability change inside an
existing MVU host. `Update`, effects, subscriptions, commands, and the interpreter are
unchanged; dispatch *outcomes* stay byte-identical (FR-008). This is recorded in the
evidence-obligations task (T003) rather than expanded into MVU contract tasks that
would have nothing to change.

## Governance risk level

**Medium** governance risk: a breaking public `.fsi` change (a new `FrameCause` type +
`FrameMetrics` fields) escalates `Route` to the **controls-public-surface** tier, but
there is no new gate, no dependency change, and no template-content change. Focused
validation = the escalated gate set Route prints (T021–T023). Broad validation (full
`Verify`) is not required because the change set is a single package's contents plus
its baselines. Non-authoritative aggregate results are recorded as "focused rerun"
notes in `readiness/aggregate-hang-diagnostics.md`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold `specs/111-frame-scheduler-invalidation/` and confirm spec + plan + research + data-model + contracts + quickstart are linked and current
- [X] T002 [P] [skillist: []] Create the `specs/111-frame-scheduler-invalidation/readiness/` scaffolds discoverable before implementation — `evidence-audit.md`, `evidence-graph.md`, `skill-loading-evidence.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation.md`, `byte-identity-authority.md`, `view-free-delta.md`, and the window-visibility not-applicable set — each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [skillist: []] Record feature Tier (Tier 1), affected package (`FS.Skia.UI.Controls.Elmish` + internal `FS.Skia.UI.Controls` retained surface), public-API impact (new `FrameCause` DU + `FrameMetrics` `DiffRan`/`LayoutRan`/`PaintRan` + narrowed `ViewCalled`), Elmish/MVU applicability (unchanged — N/A with the rationale above), and the required evidence obligations (cause classification, phase record, view-skip byte-identity, regenerated goldens, baselines, XML-doc)

---

## Phase 2: Foundation

- [X] T004 [skillist: fs-skia-controls-host] Add the public `[<RequireQualifiedAccess>] type FrameCause` DU and the `FrameCause` + `DiffRan` + `LayoutRan` + `PaintRan` fields to `FrameMetrics` in `ControlsElmish.fsi` (XML-doc each; narrow the `ViewCalled` doc), mirror them in the `.fs` definition, and thread them through **every** construction site so the build compiles — Perf `zero` (~`ControlsElmish.fs:1231`), coalesced move (~`1247`), tick (~`1273`), key (~`1307`), discrete (~`1325`), and live `emitFrameMetrics` (~`918`) — plus the test serializer `Feature109CorpusTests.fs:153` (cause classified per branch; phase bools per the CURRENT pipeline; the animation-tick view-skip is deferred to US3) (FR-001/FR-002/FR-007/FR-010)
- [X] T005 [skillist: fs-skia-controls-host] Exercise the drafted `FrameCause` + `FrameMetrics` shape from FSI (a move/idle frame through `Perf.runScript`), capturing the session transcript to `readiness/fsi-session.txt`
- [X] T006 [skillist: fs-skia-controls-host] Capture the intended top-level surface + per-package baseline shape for the new `FrameCause` type + fields (the authoritative regen happens in T019) and note it in `readiness/`
- [X] T007 [skillist: []] Record unsupported-scope handling and failure diagnostics: Phase 4+ is OUT; the full-tree runtime visual-state stamp is preserved (FR-009); the view-skip is gated on an unchanged `(model, size)` and degrades to a re-view fallback (never a stale/incorrect frame)

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — Every produced frame is explained by an explicit cause

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US1] [skillist: fs-skia-controls-host, fs-skia-evidence-mode] Add a failing-first `Feature111FrameCauseTests` through `Perf.runScript`: an idle / coalesced-move-burst / discrete-click / key / animation-only-tick script reports `FrameCause` `Idle` / `PointerMove` / `PointerDiscrete` / `Key` / `Tick` respectively, byte-stable across repeated runs (FR-001/SC-001, SC-005)

### Implementation

- [X] T009 [US1] [skillist: fs-skia-controls-host] Classify `FrameCause` at each `Perf.runScript` frame branch and at the live `mapPointer` (Moved → `PointerMove`; discrete → `PointerDiscrete`) and `wrappedTick` (`Tick`) seams; `Resize`/`Theme` remain live-only causes (no corpus frame produces them). Make T008 pass (FR-001)
- [X] T010 [US1] [skillist: []] Document the US1 independent validation path (run the mixed script; assert each frame's `FrameCause`) in `readiness/`

**Checkpoint**: User Story 1 is functional and independently testable.

---

## Phase 4: User Story 2 (US2) — The metrics identify which phases ran and which were skipped

### Tests First

- [X] T011 [P] [US2] [skillist: fs-skia-controls-host, fs-skia-evidence-mode] Add a failing-first `Feature111PhaseRecordTests`: an idle frame reports all four phase bools `false` (FR-005); an animation-only tick reports `ViewCalled = false` and `PaintRan = true`; a geometry-changing model frame reports `ViewCalled`/`DiffRan`/`LayoutRan`/`PaintRan` all `true`; a model frame with no visual diff reports `LayoutRan = false` (FR-002/SC-002, SC-004)

### Implementation

- [X] T012 [US2] [skillist: fs-skia-controls-host] Set `ViewCalled` (view) / `DiffRan` / `LayoutRan` / `PaintRan` explicitly per frame at every construction site — `DiffRan` = a new view tree was reconciled; `LayoutRan` = `RemeasuredNodeCount > 0` set at construction (not inferred at read time); `PaintRan` = a model render or animation overlay was assembled. Make T011 pass (FR-002)
- [X] T013 [US2] [skillist: []] Document the four phase-bool semantics + the hit-test-is-not-a-phase-field rationale (clarified 2026-06-12) in `readiness/`

**Checkpoint**: User Story 2 is functional and independently testable.

---

## Phase 5: User Story 3 (US3) — Frames run only the phases their cause requires (view-skip + byte-identity)

### Tests First

- [X] T014 [P] [US3] [skillist: fs-skia-controls-host, fs-skia-evidence-mode] Add a failing-first `Feature111ViewSkipTests`: an animation-only tick and a model-unchanged frame perform **no** `host.View` (`ViewCalled = false`, `FullRenderCount = 0`) while the rendered scene is **byte-identical** to the pre-feature output; a model-changing frame still runs the view (`ViewCalled = true`) (FR-003/FR-004/SC-003, SC-004, SC-007). **Also** assert the frame-rate-work clause (FR-006/SC-008): a continuous-drag burst and a continuous-animation tick sequence each report `PointerMovesProcessed <= 1` and zero per-sample `host.View` rebuilds (the move burst `FullRenderCount = 0`; every animation-only tick view-free), and no discrete press/release/click/scroll is dropped — the feature-108/110 coalescing fidelity is preserved through the scheduler

### Implementation

- [X] T015 [US3] [skillist: fs-skia-controls-host, fs-skia-reconciliation] Implement the Perf-driver view-skip: in the `[ FrameInput.Tick delta ]` branch, an animation-only tick (`hadAnimation && not hasMsgs`) re-samples the overlay by stepping `prev.Root.Control` (the retained tree = `host.View` of the unchanged model) with **no** `host.View` → `ViewCalled = false`, `FullRenderCount` loses the tick's `1`, `PaintRan = true`; a consumer `Tick` message stays a model frame (FR-003/FR-004)
- [X] T016 [US3] [skillist: fs-skia-controls-host] Implement the live-loop view-skip: `renderRetained` caches the un-stamped `host.View size model` output keyed by `(model-reference, size)` and reuses it when `obj.ReferenceEquals(model, cachedModel) && size = cachedSize`, still running `applyRuntimeVisualState` + `RetainedRender.step` and skipping only `host.View`; any key mismatch (incl. every value-type model) re-views (byte-identical fallback) (FR-003)
- [X] T017 [US3] [skillist: fs-skia-controls-host, fs-skia-evidence-mode] Update `Feature109MetricsHonestyTests`: the animation-only-tick assertion flips `ViewCalled` to `false` and asserts `PaintRan = true` + the new phase record (scope narrowed, not weakened); confirm the `ViewCalled = (FullRenderCount > 0)` invariant still holds (FR-011)
- [X] T018 [US3] [skillist: fs-skia-controls-host, fs-skia-evidence-mode] Regenerate the feature-109 corpus goldens (`PERF_CORPUS_REGEN=1`) so every line carries `FrameCause` + the three phase bools and the `text-entry-while-animating` tick frames are view-free (`ViewCalled false`, `FullRenderCount 0`, `PaintRan true`); record the before/after delta in `readiness/view-free-delta.md`; **also** confirm the at-rest rendered-output + geometry byte-identity clause (FR-008/SC-007) — assert no rendered-scene/geometry golden delta against the pre-feature state (the standing Scene-parity golden suite under `Dev`/T021 is the authority) and record that authority decision in `readiness/byte-identity-authority.md` (FR-008/FR-010/SC-006)

**Checkpoint**: User Story 3 is functional and independently testable.

---

## Phase 6: Integration & Polish

- [X] T019 [skillist: fs-skia-controls-host] Run `./fake.sh build -t RefreshSurfaceBaselines` to regenerate the top-level surface baseline (gains the `FrameCause` type + cases) and the per-package surface (FrameMetrics fields), and update any remaining `FrameMetrics` construction/read sites it flags (samples, FSI preludes)
- [X] T020 [skillist: fs-skia-controls-host] Confirm the new `FrameCause` + `DiffRan`/`LayoutRan`/`PaintRan` XML-doc satisfies the doc-preservation gate, the `ViewCalled` doc is narrowed, and no public function signature changed
- [X] T021 [skillist: fs-skia-template-update, fs-skia-controls-host] Run the escalated controls-public-surface gates sequentially as `Route` prints them — `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, the package/per-package surface diffs, and the controls catalog/doc/interaction/rendering checks — and record the focused governance risk level + non-authoritative aggregate notes in `readiness/`
- [X] T022 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises, and the echoed `feature-directory`/`tasks=<n>` match this feature
- [X] T023 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with no remaining `[S]`/`[S*]` and no diff-scan hits, or document every `--accept-synthetic` override

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section. For `[SEH]` rows, include
the approval label, design-phase source, synthetic input class, expected error
behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |

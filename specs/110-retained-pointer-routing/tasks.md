# Tasks: Retained-Frame Pointer Routing (Remove Full-Render Pointer Hot Path)

**Feature branch**: `110-retained-pointer-routing`
**Spec**: `specs/110-retained-pointer-routing/spec.md`
**Plan**: `specs/110-retained-pointer-routing/plan.md`

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
- This whole feature is **Tier 1** (the `ControlsElmish.fsi` `FrameMetrics` field
  is a breaking public surface change); per-task `[T1]/[T2]` annotations are
  omitted because every phase matches the feature tier.

## Elmish/MVU applicability

Principle IV's dedicated `Model`/`Msg`/`Effect`/`init`/`update`/interpreter tasks
are **N/A** for this feature: it is a hot-path routing *mechanism* change inside an
existing MVU host. `Update`, effects, subscriptions, commands, and the interpreter
are unchanged; dispatch *outcomes* stay byte-identical (FR-006/FR-011). This is
recorded in the evidence-obligations task (T003) rather than expanded into MVU
contract tasks that would have nothing to change.

## Governance risk level

**Medium** governance risk: a breaking public `.fsi` field on `FrameMetrics`
escalates `Route` to the **controls-public-surface** tier, but there is no new
gate, no dependency change, and no template-content change. Focused validation =
the escalated six-target set (T027–T029). Broad validation (full `Verify`) is not
required because the change set is a single package's contents plus its baselines.
Non-authoritative aggregate results are recorded as "focused rerun" notes in
`readiness/aggregate-hang-diagnostics.md`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold `specs/110-retained-pointer-routing/` and confirm spec + plan + research + data-model + contracts + quickstart are linked and current
- [X] T002 [P] [skillist: []] Create the `specs/110-retained-pointer-routing/readiness/` scaffolds discoverable before implementation — `evidence-audit.md`, `evidence-graph.md`, `skill-loading-evidence.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation.md`, and the window-visibility not-applicable set — each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [skillist: []] Record feature Tier (Tier 1), affected package (`FS.Skia.UI.Controls.Elmish` + internal `FS.Skia.UI.Controls` retained surface), public-API impact (`FrameMetrics` field), Elmish/MVU applicability (unchanged — N/A with rationale above), and the required evidence obligations (parity oracle, forced fallback, regenerated goldens, baselines, XML-doc)

---

## Phase 2: Foundation

- [X] T004 [skillist: fs-skia-controls-host] Add `FullRenderFallbackCount: int` to the `FrameMetrics` record in `ControlsElmish.fsi` with XML-doc, mirror it in the `.fs` definition, and update **every** construction site so the build compiles (`emitFrameMetrics` ~`ControlsElmish.fs:804`, `zero` ~`1076`, move ~`1107`, tick ~`1144`, key ~`1162`, discrete ~`1178`) plus the test serializer `Feature109CorpusTests.fs:153`
- [X] T005 [P] [skillist: fs-skia-reconciliation, fs-skia-ui-widgets] Add the internal retained-id → authored-control-id lookup to `RetainedRender` (`RetainedRender.fsi` internal seam + `RetainedRender.fs`), built from the step output and reproducing `Control.nearestAuthored`'s keyed-OR-in-`BoundIds` resolution from retained identity (feature 098 scheme)
- [X] T006 [skillist: fs-skia-controls-host] Retain the retained step's `ControlRenderResult` (`s.Render`) in a live-loop ref (seeded from `r0.Render` on first frame, `ControlsElmish.fs:763-773`) and carry it alongside the threaded retained value in `Perf.runScript` (`ControlsElmish.fs:1042-1053`) so routing reads `EventBindings`/`BoundIds`/`Bounds` without a fresh render
- [X] T007 [skillist: fs-skia-controls-host] Exercise the drafted `FrameMetrics` shape and the internal retained seam from FSI (prelude or ad-hoc), capturing the session transcript to `readiness/fsi-session.txt`
- [X] T008 [skillist: fs-skia-controls-host] Capture the intended surface + per-package baseline shape for the `FrameMetrics` change (the authoritative regen happens in T025) and note it in `readiness/`
- [X] T009 [skillist: []] Record unsupported-scope handling and failure diagnostics: Phase 3+ of the report is OUT; document that the full-render path is preserved as oracle/fallback and that the fallback degrades to correct dispatch (never silent mis-dispatch)

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — Pointer input routes from the retained frame, not a full render

### Tests First (Principle I, Principle VI)

- [X] T010 [P] [US1] [skillist: fs-skia-controls-host, fs-skia-evidence-mode] Add a failing-first metrics test through `Perf.runScript` asserting a routed move and a routed press/click each perform **zero** routing full renders — `FullRenderCount` is not incremented for routing and `ViewCalled` stays false on a pure routing frame (SC-001, SC-002)
- [X] T011 [P] [US1] [skillist: fs-skia-controls-host] Add a burst-coalescing test: N move samples in one frame report `PointerMovesProcessed <= 1` with zero routing full renders, and no discrete press/release/click/scroll is dropped; **also** assert drag/freehand **path fidelity** for a path-consuming consumer is retained through the retained route (the per-sample path the consumer observes is unchanged from the 108/109 baseline) (SC-009, FR-012)

### Implementation

- [X] T012 [US1] [skillist: fs-skia-controls-host, fs-skia-reconciliation] Implement the internal retained route: run `Pointer.update` over the retained frame's **cached** `LayoutResult`, resolve each interaction via `retainedHitTest` → the T005 lookup → the retained frame's `EventBindings`, with the unchanged `MapPointer` fallback for unbound interactions — performing no `host.View` + `Control.renderTree` for routing
- [X] T013 [US1] [skillist: fs-skia-controls-host] Wire `runInteractiveApp`'s `processInput` (`ControlsElmish.fs:816-837`) onto the retained route, keeping the already-retained focus-on-click `resolveFocus` path
- [X] T014 [US1] [skillist: fs-skia-controls-host] Wire `Perf.runScript`'s `routeInteraction` (`ControlsElmish.fs:1058-1066`) onto the retained route over the threaded retained frame instead of re-rendering
- [X] T015 [US1] [skillist: fs-skia-controls-host] Narrow `FullRenderCount`/`ViewCalled` so a retained routing frame increments neither, and thread the frame-local `FullRenderFallbackCount` accumulator through `emitFrameMetrics` and every `Perf.runScript` frame branch (FR-008)
- [X] T016 [US1] [skillist: []] Document the US1 independent validation path (run the move-then-click perf script; assert routing full renders are zero with correct hit + messages) in `readiness/`

**Checkpoint**: User Story 1 is functional and independently testable.

---

## Phase 4: User Story 2 (US2) — Retained routing is dispatch-identical to the full-render path

### Tests First

- [X] T017 [P] [US2] [skillist: fs-skia-controls-host, fs-skia-reconciliation] Add `Feature110RetainedRoutingParityTests` comparing the retained route against the preserved `routeInteractivePointer` oracle over keyed / unkeyed-same-kind-sibling / composite / nested scenes: dispatched message list, matched control identity, and focus outcome are equal (structural comparison, no value equality) (SC-003)
- [X] T018 [P] [US2] [skillist: fs-skia-controls-host] Add the targeted parity cases: an unkeyed same-kind sibling hit selects the same sibling and fires the same binding, and a composite control whose binding is authored above the hit node dispatches the same authored binding (SC-004, FR-003/FR-005)

### Implementation

- [X] T019 [US2] [skillist: fs-skia-reconciliation, fs-skia-ui-widgets] Make the T005 lookup resolve the exact authored id `nearestAuthored` would (composite-binding-above-hit climb; distinct retained ids for unkeyed siblings) so the parity tests pass
- [X] T020 [US2] [skillist: fs-skia-controls-host] Verify focus-outcome parity: a click that also moves focus yields the same focused identity via the retained path as the oracle (FR-006 focus clause)

**Checkpoint**: User Story 2 is functional and independently testable.

---

## Phase 5: User Story 3 (US3) — The fallback path is observable and stays off the hot path

### Tests First

- [X] T021 [P] [US3] [skillist: fs-skia-controls-host, fs-skia-evidence-mode] Add a test asserting every normal scripted pointer scenario in the corpus reports `FullRenderFallbackCount = 0` for every frame (SC-005)
- [X] T022 [P] [US3] [skillist: fs-skia-controls-host] Add `Feature110FallbackTests`: a deliberately constructed unroutable case increments `FullRenderFallbackCount` by one and the fallback dispatch still equals the oracle's (SC-006). Real evidence — the fallback runs the preserved oracle (real product code), so this is not synthetic

### Implementation

- [X] T023 [US3] [skillist: fs-skia-controls-host] Implement the counted fallback: when the retained route cannot resolve an event from the retained frame, fall back to the preserved `routeInteractivePointer` oracle and increment `FullRenderFallbackCount` (FR-007/FR-009)
- [X] T024 [US3] [skillist: fs-skia-controls-host, fs-skia-evidence-mode] Regenerate the feature-109 corpus pointer goldens (`PERF_CORPUS_REGEN=1`) so routing full-render counts drop to zero, and record the before/after delta in `readiness/`; **also** confirm the at-rest **rendered output + control geometry** byte-identity clause of FR-011/SC-008 — assert no rendered-scene/geometry golden delta against the pre-feature state (the standing Scene-parity golden suite run under `Dev`/T027 is the authority) and record that authority decision in `readiness/byte-identity-authority.md` (SC-007, SC-008, FR-010, FR-011)

**Checkpoint**: User Story 3 is functional and independently testable.

---

## Phase 6: Integration & Polish

- [X] T025 [skillist: fs-skia-controls-host] Run `./fake.sh build -t RefreshSurfaceBaselines` to regenerate the surface + per-package baselines for the `FrameMetrics` field, and update any remaining `FrameMetrics` construction/read sites it flags (samples, FSI preludes)
- [X] T026 [skillist: fs-skia-controls-host] Confirm the new field's XML-doc satisfies the doc-preservation gate and the public `routeInteractivePointer` signature is unchanged (oracle/fallback preserved)
- [X] T027 [skillist: fs-skia-template-update, fs-skia-controls-host] Run the escalated controls-public-surface gates sequentially — `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck` — and record the focused governance risk level + non-authoritative aggregate notes in `readiness/`
- [X] T028 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises, and the echoed `feature-directory`/`tasks=<n>` match this feature
- [X] T029 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with no remaining `[S]`/`[S*]` and no diff-scan hits, or document every `--accept-synthetic` override

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section. For `[SEH]` rows,
include the approval label, design-phase source, synthetic input class, expected
error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |

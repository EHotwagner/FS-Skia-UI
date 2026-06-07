# Tasks: Mouse Input & Pointer Events

**Feature branch**: `075-mouse-input-events`
**Spec**: `specs/075-mouse-input-events/spec.md`
**Plan**: `specs/075-mouse-input-events/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

`[SEH]` + `synthetic-error-handling-approved` is assigned only during design /
planning / clarification / task generation. **None are approved for this
feature**: every interaction test (including the FR-007 cancel path and the
FR-010 stale/miss diagnostics) feeds real, scripted `PointerMsg` values into the
pure `Pointer.update`/`replay` — genuine deterministic inputs, not synthetic
substitutes (see plan §"Synthetic evidence"). The Synthetic-Evidence Inventory
below is intentionally empty.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US5]** — user-story scope
- Feature tier is **Tier 1** (consumer-contract change) throughout; per-task
  `[T1]`/`[T2]` annotations are omitted because no phase differs from the
  feature tier.

Every task has a matching entry in `tasks.deps.yml`; every task line mirrors its
structured `skillist` as `[skillist: ...]` (use `[skillist: []]` when empty).
Phase-checkpoint edges (Phase N+1 → last task of Phase N) are auto-injected by
the graph compute and are NOT repeated in `tasks.deps.yml`.

## Governance risk & validation

This is a consumer-contract / **escalated maintainer-verify** change (public
`.fsi` in `Controls`, `Controls.Elmish`, `SkiaViewer`; `template/**`). Run
`./fake.sh build -t Route` first and run only the gates it prints. Expected
serialized FAKE-backed order (never concurrent — shared `.fake` state):
`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck`
→ `EvidenceGraph` → `EvidenceAudit`, plus `RefreshSurfaceBaselines` and
per-package `PerPackageSurface.captureCurrent` for the moved surfaces.
`GeneratedProductCheck` is a known local environment failure (record the
environment-failure classification, not a product defect). Risk levels:
small = single-package pure `update` change (focused per-package tests);
medium = cross-package `.fsi` surface move (surface baselines + FSI evidence);
broad = host contract + template fragment (full serialized order). Broad
validation is required because the host `ViewerEvent` arity changes and a
template fragment is added; non-authoritative aggregate results are recorded
under `readiness/logs/` and `readiness/generated-product-verify/`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the `075-mouse-input-events` feature directory and ensure `spec.md`/`plan.md`/`research.md`/`data-model.md`/`quickstart.md`/`contracts/` are linked from the task breakdown
- [X] T002 [P] [skillist: []] Scaffold `specs/075-mouse-input-events/readiness/` with the audit-enforced readiness-contract files this feature actually produces — `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `skill-loading-evidence.md`, `keyboard-regression.md`, `evidence-graph.md`, `evidence-audit.md` — plus the `fsi/`, `sample-smoke/`, `package-surfaces/`, `package/`, `logs/`, and `generated-product-verify/` subdirectories (each artifact names its authoritative command, artifact path, failure class, next action). NOTE: this feature delivers a pure pointer-coordination contract proven deterministically; per `contracts/sample-contract.md` the deterministic smoke — not a persistent visible GUI window — is the authoritative sample evidence, so the window-visibility evidence class (a persistent-visible-window deliverable) does not apply and its files are intentionally not scaffolded
- [X] T003 [P] [skillist: []] Record feature Tier (Tier 1), affected layer (Controls / Controls.Elmish / SkiaViewer host), public-API impact, Elmish/MVU applicability (**Principle IV applies** — `PointerState`/`PointerMsg`/`PointerInteraction`/`init`/`update`/`replay`), and required evidence obligations
- [X] T004 [P] [skillist: []] Record unsupported-scope handling and failure diagnostics design: `HitTestMiss`/`StaleTarget` diagnostics (FR-010), the `DragCancelled` window-exit/focus-loss cancel path (FR-007), and the SEH classification decision (none approved — all paths proven with real scripted messages)

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: fs-skia-skiaviewer] Draft the host `ViewerEvent` extension `.fsi` in `src/SkiaViewer/Host/Diagnostics.fsi` — `ViewerPointerButton`, `button` on `PointerPressed`/`PointerReleased`, new `PointerScrolled`/`PointerExited` cases — with the case-arity compatibility note (`contracts/viewer-event.host.fsi`)
- [X] T006 [P] [skillist: fs-skia-ui-widgets] Draft the `FS.Skia.UI.Controls` pointer front door `.fsi` in `src/Controls/Pointer.fsi` — `PointerButton`/`PointerOrigin`/`PointerPhase`/`PointerSample`/`PressCandidate`/`PointerState`/`PointerDiagnostic`/`PointerInteraction`/`PointerMsg` + `Pointer` module (`init`/`toMsg`/`update`/`replay`) — and add `Pointer.fs(i)` to `Controls.fsproj` compile order (`contracts/pointer.controls.fsi`)
- [X] T007 [skillist: fs-skia-elmish] Draft the `FS.Skia.UI.Controls.Elmish` bridge `.fsi` in `src/Controls.Elmish/ControlsElmish.fsi` — `interpretPointerEffect` + `interpretPointerOutcome`, reusing `ReportAdapterDiagnostic`/`DispatchControlRuntimeMessage` (add an `AdapterEffect` case only if needed) (`contracts/pointer.controls-elmish.fsi`)
- [X] T008 [skillist: fs-skia-ui-widgets, fs-skia-elmish] Exercise the draft `.fsi` shapes from FSI (`scripts/prelude.fsx` or ad-hoc) — `Pointer.init`, a scripted `replay`, and a `interpretPointerOutcome` lowering — capturing the session transcript to `readiness/fsi-session.txt` (Principle I: fix the surface if it reads awkwardly)
- [X] T009 [skillist: []] Record surface-area baselines for the new/changed public modules (`FS.Skia.UI.Controls`, `FS.Skia.UI.Controls.Elmish`, `FS.Skia.UI.SkiaViewer`); confirm `Layout`/`KeyboardInput` baselines are unchanged

**Checkpoint**: Foundation ready — user-story implementation may begin.

---

## Phase 3: User Story 1 — Hover feedback follows the pointer (US1, P1)

### Tests First (Principle I, Principle VI)

- [X] T010 [P] [US1] [skillist: fs-skia-ui-widgets] Add failing-first hover tests: ordered `HoverLeave(prior)` → `HoverEnter(next)`, no transition when the hit id is unchanged, leave-only on empty space / window exit (SC-001/FR-003), plus an FsCheck property "no duplicate or skipped hover transitions under random move bursts" (FR-003); also include one overlap case (topmost/front-most in paint order wins) and one hidden/collapsed-control case (never a hover target), asserting the pointer path honors the `Layout.hitTestComputed` paint-order/visibility contract (FR-002 edge cases)

### Implementation

- [X] T011 [US1] [skillist: fs-skia-ui-widgets] Create `src/Controls/Pointer.fs` — `init`/`toMsg` and the `Move` path of pure `update` (hit-test via `Layout.hitTestComputed`, derive ordered hover transitions, emit `ControlRuntimeMsg.HoverControl` to keep runtime hover consistent)
- [X] T012 [US1] [skillist: fs-skia-elmish] Implement `ControlsElmish.interpretPointerEffect`/`interpretPointerOutcome` in `src/Controls.Elmish/ControlsElmish.fs` (route meaningful interactions via `mapInteraction`, no-ops → `[]`, diagnostics → `ReportAdapterDiagnostic`, runtime messages → `DispatchControlRuntimeMessage`)
- [X] T013 [US1] [skillist: fs-skia-ui-widgets, fs-skia-elmish] Verify US1 end-to-end against the **packed** libraries from FSI: hover front door + Elmish bridge over a scripted move sequence; confirm every emitted effect is a `PointerInteraction` value carrying `PointerOrigin.Pointer` (type-distinct from keyboard/text effects), proving pointer-vs-keyboard origin discrimination (FR-011); capture transcript to `readiness/fsi/pointer-frontdoor.md`

**Checkpoint**: US1 hover is functional and independently testable.

---

## Phase 4: User Story 2 — Click activates a control (US2, P2)

### Tests First

- [X] T014 [P] [US2] [skillist: fs-skia-ui-widgets] Add failing-first click tests: click iff press+release over the **same** control, no click when released off the control (pressed state cleared), focus moves to a focusable pressed control (SC-002/FR-004/FR-005), plus an FsCheck property "press/release pair never dropped or reordered under interleaved moves" (FR-008)

### Implementation

- [X] T015 [US2] [skillist: fs-skia-skiaviewer] Implement the host `ViewerEvent` extension: mirror the type in `src/SkiaViewer/Host/Diagnostics.fs`, capture `MouseButton` in `src/SkiaViewer/Host/Vulkan.fs` (drop the `_` discard, map Silk.NET → `ViewerPointerButton`), subscribe/dispose `IMouse.Scroll`, wire a mouse-leave/blur → `PointerExited`, and update the sole `SkiaViewer.fs` matcher in lockstep
- [X] T016 [US2] [skillist: fs-skia-ui-widgets] Extend pure `update` with the `Down`/`Up` paths — record per-button `PressCandidate`, emit `PressedDown`/`ReleasedUp`/`Click` (click iff release over the press control), `FocusMovedByPointer` + `ControlRuntimeMsg.PressControl`/`FocusControl`, and `Diagnostic HitTestMiss` on a press miss
- [X] T017 [US2] [skillist: fs-skia-ui-widgets, fs-skia-elmish] Verify US2 against the packed libraries from FSI: same-control click dispatches once, off-control release dispatches nothing, focus moves to the pressed focusable control; append transcript to `readiness/fsi/pointer-frontdoor.md`

**Checkpoint**: US2 click + focus is functional and independently testable.

---

## Phase 5: User Story 3 — Drag interactions (US3, P3)

### Tests First

- [X] T018 [P] [US3] [skillist: fs-skia-ui-widgets] Add failing-first drag tests: press → move-past-threshold emits one `DragBegin`, ordered `DragMove`s, one `DragEnd` on release; sub-threshold press/release is a `Click` not a drag (Click XOR drag); `WindowExited`/`FocusLost` mid-press/drag yields `DragCancelled` with `Presses` empty and no active drag (SC-003/SC-004/FR-006/FR-007)

### Implementation

- [X] T019 [US3] [skillist: fs-skia-ui-widgets] Extend pure `update` with the held-`Move` drag path (`DragThreshold` distance test, `DragBegin`/`DragMove` + `ControlRuntimeMsg.StartDrag`/`MoveDrag`), the `Up` drag-end path (`DragEnd` + `EndDrag`), and the `WindowExited`/`FocusLost` cancel (`DragCancelled`, reset `Presses`/`Hover`, `CancelInteraction`)
- [X] T020 [US3] [skillist: fs-skia-ui-widgets] Verify US3 against the packed libraries from FSI: a scripted drag (begin/move/end) and a scripted cancel-on-exit; append transcript to `readiness/fsi/pointer-frontdoor.md`

**Checkpoint**: US3 drag + cancel is functional and independently testable.

---

## Phase 6: User Story 4 — Secondary-button (context) interaction (US4, P3)

### Tests First

- [X] T021 [P] [US4] [skillist: fs-skia-ui-widgets] Add failing-first per-button tests: a secondary press/release yields `Click(_, Secondary, …)` and no primary click (and converse); a middle press/release yields `Click(_, Middle, …)` distinct from both (FR-013 covers primary/secondary/middle); overlapping presses across buttons resolve independently with zero cross-button misattribution (SC-008/FR-013)

### Implementation

- [X] T022 [US4] [skillist: fs-skia-ui-widgets] Ensure `Down`/`Up` key the `Map<PointerButton, PressCandidate>` by button so each button's press resolves independently, and `Click`/drag effects carry the originating `PointerButton`
- [X] T023 [US4] [skillist: fs-skia-ui-widgets] Verify US4 against the packed libraries from FSI: distinct secondary click + an overlapping primary/secondary sequence; append transcript to `readiness/fsi/pointer-frontdoor.md`

**Checkpoint**: US4 secondary-button discrimination is functional and independently testable.

---

## Phase 7: User Story 5 — Wheel / scroll (US5, P3)

### Tests First

- [X] T024 [P] [US5] [skillist: fs-skia-ui-widgets] Add failing-first wheel tests: a `PointerMsg.Wheel` over a control emits `Scroll(control, dx, dy, x, y)` with the correct signed delta; a wheel over empty space emits no scroll interaction (SC-009/FR-014)

### Implementation

- [X] T025 [US5] [skillist: fs-skia-ui-widgets] Extend pure `update` with the `PointerMsg.Wheel` path (hit-test → `Scroll` to control-under-pointer; silent miss over empty space) consuming the host `PointerScrolled` wired in T015
- [X] T026 [US5] [skillist: fs-skia-ui-widgets] Verify US5 against the packed libraries from FSI: wheel-over-control vs wheel-over-empty; append transcript to `readiness/fsi/pointer-frontdoor.md`

**Checkpoint**: US5 wheel/scroll is functional and independently testable.

---

## Phase 8: Integration & Polish

- [X] T027 [P] [skillist: fs-skia-ui-widgets] Add the determinism test: `Pointer.replay` of the same `PointerMsg list` against the same `LayoutResult` yields identical effects on a re-run (SC-005/FR-009)
- [X] T028 [P] [skillist: fs-skia-keyboard-input] Re-run an existing keyboard-only sample unchanged and confirm no behavior change is forced (SC-006/FR-012); record the regression note under `readiness/`
- [X] T029 [P] [skillist: fs-skia-ui-widgets, fs-skia-elmish, fs-skia-skiaviewer] Build the `samples/PointerInteractionGallery` sample (`Program.fs`): `ViewerEvent.Pointer*` → `PointerSample` → `Pointer.update` → `interpretPointerOutcome` → Elmish `Cmd`, demonstrating hover/click/drag/secondary/scroll using **only** `ControlId`-level messages and no consumer-side hit-testing (SC-007)
- [X] T030 [skillist: fs-skia-skiaviewer, fs-skia-evidence-mode] Run the gallery sample from its default executable path through the deterministic contract smoke (the authoritative sample evidence per `contracts/sample-contract.md`), exercising hover/click/secondary/drag/scroll across the public front door, and capture the smoke log to `readiness/sample-smoke/PointerInteractionGallery.txt`; per `fs-skia-evidence-mode`, a persistent Vulkan window / render-only screenshot is unavailable under the headless validation host and is classified as an environment condition (see `readiness/runtime-limitations.md`), not a product defect — the deterministic smoke is the authoritative visual proof
- [X] T031 [skillist: fs-skia-samples, fs-skia-template-update] Add the pointer sample fragment under `template/fragments/samples/` so the generated Samples capability includes it, and add a short pointer-interaction paragraph to the selected Controls generated guidance
- [X] T032 [P] [skillist: []] Refresh surface-area baselines (`./fake.sh build -t RefreshSurfaceBaselines`) and regenerate the per-package `.fsi.txt` snapshots via `PerPackageSurface.captureCurrent` for `FS.Skia.UI.Controls`, `FS.Skia.UI.Controls.Elmish`, `FS.Skia.UI.SkiaViewer`; move snapshots into `readiness/package-surfaces/`
- [X] T033 [skillist: fs-skia-template-update] Run the serialized maintainer-verify gates sequentially (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck`), recording logs to `readiness/logs/` and the governance risk-level note; classify the known `GeneratedProductCheck` local failure as an environment failure under `readiness/generated-product-verify/`, not a product defect
- [X] T034 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises; write `readiness/evidence-graph.md` and `readiness/task-graph.{md,json}`
- [X] T035 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (or document every `--accept-synthetic` override); write `readiness/evidence-audit.md`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section. For `[SEH]` rows,
include the approval label, design-phase source, synthetic input class, expected
error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — all paths proven with real scripted pointer messages)_ | | | | | | | | |

# Feature Specification: Frame Scheduler & Phase-Invalidation Model (Explain and Schedule Frames by Cause)

**Feature Branch**: `111-frame-scheduler-invalidation`
**Created**: 2026-06-12
**Status**: Draft
**Input**: User description: "do the next part"

**Source report** (local in-repo report, not a remote URL — no `source-spec.md`
snapshot per the specify FR-016 no-op rule):
`docs/reports/2026-06-12-1422-controls-performance-framework-research.md`. This
feature implements the **next part** of that report's staged plan after feature
110 (which delivered Phase 2: retained pointer routing) — namely **Phase 3: Add a
Frame Scheduler and Phase Invalidation Model**, also the report's "Do next"
priority **#1**. Everything from Phase 4 onward (narrowed runtime visual-state
stamping, view memoization, viewport virtualization, paint/damage caches, layout
caches, backend review) remains **out of scope** — see *Unsupported scope*.

## Why this feature (context)

Features 108/109 made per-frame metrics truthful and built a reproducible corpus;
feature 110 removed the full-render pointer hot path. But the framework still
cannot **explain** a frame: the metrics report *what counts* changed, not *why the
frame ran* or *which phases it actually needed*. Worse, the live host's repaint
still calls `host.View size model` every frame even when the **product model did
not change** — a host-owned hover, focus, or animation change re-runs the consumer
view and re-stamps the whole tree (source report §"Gaps", "renderRetained still
calls host.View ... too broad for host-owned hover/focus/animation changes if the
consumer model did not change"). That is avoidable per-frame work riding on top of
an otherwise-retained pipeline.

This feature introduces an explicit **frame cause** for every produced frame and a
**phase-invalidation record** of which work phases (view, diff, layout, paint) the
frame actually ran versus skipped, and wires both into a frame
**scheduler** that runs *only the phases the cause requires*. Concretely, a frame
whose cause does **not** change the product model no longer re-runs `host.View`:
the framework reuses the view it already produced for the unchanged model, so
hover / focus / animation / input frames become **view-free** while at-rest
rendered output, geometry, focus/keyboard semantics, and every dispatch outcome
stay **byte-identical**. The only intended observable changes are the new
cause/phase observability and the elimination of redundant view work on
model-unchanged frames.

## Clarifications

### Session 2026-06-12

- Q: Should Phase 3 change behavior (skip the redundant `host.View` on
  model-unchanged frames) or be observability-only? → A: **Observability + view-skip**
  — add the `FrameCause` + phase record AND skip `host.View` on model-unchanged
  frames (hover/animation/input become view-free); `ViewCalled` flips `true → false`
  on animation ticks and the feature-109 SC-011 honesty test is updated to assert the
  new phase record. Rendered output stays byte-identical (`host.View` is pure).
- Q: Which phases should the deterministic `FrameMetrics` phase record expose as
  first-class fields, given `Perf.runScript` does not hit-test coalesced
  hover/drag moves (only discrete clicks hit-test)? → A: **view, diff/reconcile,
  layout, paint** (four boolean phase fields the deterministic path actually
  exercises and can byte-stably assert). **Hit-test is NOT a phase field** — it is a
  routing concern already covered by `PointerSamplesReceived` /
  `PointerMovesProcessed` / `FullRenderFallbackCount`, so it is not added to the
  deterministic record (no misleading always-false field on the move-burst corpus).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Every produced frame is explained by an explicit cause (Priority: P1)

A framework maintainer drives scripted interactions and needs to read,
deterministically, **why** each frame ran. Today the metrics say a full render
happened or a move was processed, but not the *cause*. After this feature, every
produced frame carries a closed `FrameCause` (idle, pointer-move, pointer-discrete,
key, tick, resize, theme), so the per-frame record answers "what triggered this
frame".

**Why this priority**: Causation is the foundation of the whole phase model — the
scheduler decides which phases to run *from the cause*, and every later phase
(narrowed stamping, virtualization, paint caches) is justified by cause/phase
evidence. Independently valuable: even with no scheduling change, naming the cause
makes the existing frame stream legible.

**Independent Test**: Run a scripted sequence (idle, a coalesced move burst, a
discrete click, a key, an animation-only tick, a model-changing input) through the
deterministic perf path and assert each produced frame reports the expected
`FrameCause`, byte-stable across repeated runs.

**Acceptance Scenarios**:

1. **Given** a no-input frame, **When** it is produced, **Then** its cause is
   `Idle`.
2. **Given** a coalesced pointer-move burst, **When** the frame is produced,
   **Then** its cause is `PointerMove`.
3. **Given** a discrete press/release/click/scroll, **When** the frame is
   produced, **Then** its cause is `PointerDiscrete`.
4. **Given** a tick that only advances animation clocks (no consumer message),
   **When** the frame is produced, **Then** its cause is `Tick`.
5. **Given** the same script run twice, **When** the causes are compared,
   **Then** the per-frame cause sequence is identical (deterministic).

---

### User Story 2 - The metrics identify which phases ran and which were skipped (Priority: P1)

A maintainer must be able to see, per frame, **which work phases happened** — view,
diff/reconcile, layout, paint — and which were **skipped**, so a frame's
work is explicit rather than inferred from a single render counter. An idle frame
must show *every* phase skipped (zero work); a hover frame must show the view phase
skipped; a model frame must show the view/diff/layout/paint phases ran.

**Why this priority**: "Metrics identify skipped phases explicitly" is the report's
Phase 3 acceptance gate. Without it, a regression that silently reintroduces a
skipped phase (e.g. a per-hover full view rebuild) is invisible. P1 because it is
the observability that proves US3's scheduling.

**Independent Test**: Run the corpus scenarios and assert the per-frame phase
record: an idle frame skips all phases; a pointer-move frame skips view + diff +
layout; an animation-only tick skips view but runs paint; a model-changing frame
runs view + diff + layout + paint. The record is byte-stable and golden-asserted.

**Acceptance Scenarios**:

1. **Given** an idle frame, **When** its phase record is read, **Then** view,
   diff, layout, and paint are all reported **not run** (zero work).
2. **Given** an animation-only tick, **When** its phase record is read, **Then**
   the view phase is **not run** and the paint phase **is run** (a paint-only
   frame).
3. **Given** a frame whose product message changed the model, **When** its phase
   record is read, **Then** view, diff, layout, and paint are reported **run**.
4. **Given** a regression that re-runs the view on a hover frame, **When** the
   golden is checked, **Then** the phase record differs and the deterministic
   golden fails.

---

### User Story 3 - Frames run only the phases their cause requires (Priority: P1)

A maintainer needs continuous host-owned change (hover sweeps, focus moves,
animation) to cost **frame-rate** work, not **sample-rate** view rebuilds. After
this feature the scheduler runs only the phases the cause requires: a frame whose
cause did **not** change the product model performs **no** `host.View` rebuild — the
framework reuses the view it already produced for the unchanged model — so an
animation tick paints without re-viewing and a pointer-move/hover frame neither
views nor diffs a fresh tree. At-rest rendered output and every dispatch outcome
stay byte-identical.

**Why this priority**: This is the report's Phase 3 goal ("Batch work at the frame
boundary and only run phases required by the cause"; "Make animation clocks request
paint-only frames while active"). It is the behavioral payoff US1/US2 observe. P1
because it removes the last broad per-frame view bypass the report names.

**Independent Test**: Run an animation-only tick and a pointer-move frame through
the deterministic perf path and assert `host.View` did **not** run (the view phase
is skipped) while the rendered output is byte-identical to the pre-feature output;
run a model-changing frame and assert the view phase **did** run as before.

**Acceptance Scenarios**:

1. **Given** an animation-only tick, **When** the frame is produced, **Then**
   `host.View` does not run and the painted output is byte-identical to the
   pre-feature animation frame.
2. **Given** a pointer-move or hover frame (no model change), **When** the frame
   is produced, **Then** `host.View` does not run and no fresh tree is diffed.
3. **Given** a frame whose dispatched message changed the model, **When** the
   frame is produced, **Then** `host.View` runs exactly once and the frame renders
   the updated model as before.
4. **Given** any of the above, **When** the rendered scene and control geometry
   are compared to the pre-feature output, **Then** they are byte-identical.

---

## Requirements *(mandatory)*

### Functional Requirements

**Cause & phase model (Phase 3 core)**

- **FR-001**: Every produced frame MUST carry a deterministic, closed `FrameCause`
  drawn from `{ Idle, PointerMove, PointerDiscrete, Key, Tick, Resize, Theme }` —
  the trigger that caused the frame. The cause MUST be byte-stable across repeated
  runs of the same script.
- **FR-002**: Every produced frame MUST report a deterministic **phase record** —
  four boolean phase fields — stating which work phases ran versus were skipped:
  **view**, **diff/reconcile**, **layout**, and **paint**. Skipped phases MUST be
  explicit (a `false`), not inferred from another counter. **Hit-test is NOT a phase
  field** (clarified 2026-06-12): the deterministic `Perf.runScript` path does not
  hit-test coalesced hover/drag moves (only discrete clicks hit-test, feature 110),
  so a hit-test field would read `false` across the move-burst corpus; hit-test work
  remains covered by `PointerSamplesReceived` / `PointerMovesProcessed` /
  `FullRenderFallbackCount`.
- **FR-003**: The scheduler MUST run **only the phases the cause requires**. A frame
  whose cause did **not** change the product model MUST NOT re-run `host.View`; the
  framework MUST reuse the view it already produced for the unchanged model
  (`host.View` is a pure function of the model + size, so the reused tree is the
  same tree a fresh call would produce). A model-changing frame MUST re-run
  `host.View` exactly as today.
- **FR-004**: An **animation-only tick** (a tick that advances animation clocks and
  produces no product message) MUST be a **paint-only** frame: it paints the
  animation overlay without re-running `host.View` (the paint phase runs; the view
  phase is skipped).
- **FR-005**: An **idle** frame MUST run **no** phase (no view, diff, layout, or
  paint) and report `FrameCause = Idle` with every phase field `false` (zero work).

**Scheduling & input (Phase 3 batching)**

- **FR-006**: Native input MUST be **enqueued and flushed at the frame/tick
  boundary** rather than each native sample synchronously rebuilding view, layout,
  paint, and dispatch; input handlers MUST enqueue and return. Pointer-move bursts
  MUST coalesce in the queue to at most one processed move per frame (feature 108
  fidelity) while the raw drag/freehand path remains available to path-consuming
  consumers (no discrete press/release/click/scroll is dropped).
- **FR-007**: The deterministic `Perf.runScript` driver MUST expose the `FrameCause`
  and phase record per frame so each non-idle frame is explained by cause and the
  skipped phases are visible, and these fields MUST re-run byte-identically (they
  join the byte-stable count/bool golden surface; timing stays excluded).

**Behaviour preservation & contract (cross-cutting)**

- **FR-008**: This feature is a **scheduling/observability change only**. At-rest
  rendered output, control geometry, focus/keyboard routing semantics, and every
  **dispatch outcome** MUST remain **byte-identical** to the pre-feature state. The
  only intended observable changes are (a) the new `FrameCause` + phase record and
  (b) the elimination of redundant `host.View` work on model-unchanged frames
  (which makes the *view phase* skip, not the *rendered output* change).
- **FR-009**: The full-tree runtime visual-state stamp is **preserved** — narrowing
  it to per-identity targeted stamping is **Phase 4 (deferred)**. Phase 3 reuses the
  already-produced view tree but still stamps/diffs it as today; it removes only the
  redundant `host.View` *call*, not the stamp.
- **FR-010**: Adding `FrameCause` and the phase record to `FrameMetrics` is a
  **breaking public `.fsi` change**; every `FrameMetrics` construction and read site
  (samples, FSI preludes, tests, surface / per-package baselines) MUST be updated in
  the same change, and the feature-109 corpus goldens MUST be regenerated to include
  the new fields and reflect the now view-free hover/animation frames, with the
  before/after delta recorded.
- **FR-011**: `ViewCalled` MUST keep its precise meaning ("`host.View` actually
  ran"). After this feature it is **false** on a model-unchanged frame (including an
  animation-only tick, which previously reported `ViewCalled = true`); the
  "work happened for the overlay" fact is re-expressed by the paint phase of the new
  record, so no field conflates "the view ran" with "the frame painted".

> Interacting / conflicting requirements:
> - **FR-003 (skip `host.View` when the model is unchanged) vs FR-008
>   (byte-identical output)** — resolution: `host.View` is a pure function of
>   `(model, size)`; when neither changed, a fresh call returns a tree equal to the
>   one already produced, so reusing it cannot change rendered output. The reuse is
>   gated on an exact unchanged-model + unchanged-size check; any model/size/theme
>   change re-runs the view. Byte-identity is preserved by construction.
> - **FR-011 (`ViewCalled` now false on an animation tick) vs feature-109 SC-011
>   (the animation tick reported `ViewCalled = true`)** — resolution: feature 109
>   used `ViewCalled = true` to mean "the frame did overlay work". Phase 3 makes that
>   precise: the view phase did **not** run (no `host.View`), the **paint** phase
>   did. `ViewCalled`'s definition is unchanged; its honest value flips, and the
>   overlay fact moves to the paint phase of the new record. The feature-109 honesty
>   test is updated to assert the new phase record, not weakened.
> - **FR-001 cause taxonomy vs a model-changing input** — resolution: `FrameCause`
>   names the **trigger** (e.g. `Key`), while whether the model changed is the
>   existing `ProductModelChanged`; a key that changes the model is
>   `FrameCause = Key` with `ProductModelChanged = true` and the view/diff/layout/
>   paint phases run. The cause and the effect are reported as separate facts.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Touches `FS.Skia.UI.Controls.Elmish` — the live
  `runInteractiveApp` scheduler/repaint path and the deterministic `Perf.runScript`
  driver, plus the `FrameMetrics` record (new `FrameCause` + phase fields) — and
  consumes the existing internal `FS.Skia.UI.Controls` retained surface
  (`RetainedRender.step`/`init`, the retained tree it already holds). No package
  identity changes; `Controls.Elmish` package **contents** change and its version
  bumps on merge. Any new scheduler/cause/phase seam consuming the internal
  `RetainedRender` is **internal** (tests reach it via `InternalsVisibleTo`).
- **Public contract impact**: `FrameMetrics` gains a closed `FrameCause` value and a
  per-phase invalidation record (deterministic) in `ControlsElmish.fsi` — a breaking
  public `.fsi` change, so surface + per-package baselines update and `Route`
  escalates to the **controls-public-surface** tier. XML-doc on the new fields is
  required (doc-preservation gate). `ViewCalled` semantics narrow (precise meaning
  retained; value flips on model-unchanged frames). No public function signature
  gains an internal-typed parameter.
- **State workflow impact**: None to MVU semantics — `Update`, effects,
  subscriptions, commands, and interpreter behaviour are unchanged. Dispatch
  *outcomes* are byte-identical (FR-008); only the **scheduling** of which phases run
  per frame, and the observability of cause/phase, change.
- **Layout/rendering impact**: None to rendered output — at-rest scene, geometry,
  and the retained step are byte-identical (FR-008). The scheduler removes redundant
  `host.View` calls on model-unchanged frames; this changes *whether the view phase
  runs*, not *what is drawn*. No Vulkan/Skia/visual-output change; no
  unsupported-environment diagnostic change.
- **Evidence obligations**: Per-frame `FrameCause` + phase-record evidence through
  `Perf.runScript` (US1/US2); a view-free-frame test proving `host.View` did not run
  on an animation tick / hover frame while output is byte-identical (US3);
  regenerated corpus goldens including the new fields with the before/after delta;
  byte-identity authority for at-rest output/geometry (the standing Scene-parity
  golden suite under `Dev`); skill-loading evidence; the window-visibility
  not-applicable set; `readiness/evidence-audit.md` with a verdict token; the
  generated-validation package-resolution tokens. The escalated `maintainer-verify`
  readiness set applies because of the `.fsi` change.
- **Unsupported scope**: This feature is **Phase 3 only**. Explicitly OUT: **Phase 4**
  narrowed per-identity runtime visual-state stamping (Phase 3 keeps the full-tree
  stamp, FR-009); view/control **memoization** (Phase 5); viewport **virtualization**
  (Phase 6); **damage rectangles / Skia picture / paint caches** (Phase 7); **text /
  layout-boundary caches** (Phase 8); **`SkiaViewer` backend / render-thread /
  compositor** review (Phase 9). No granular per-phase node-count fields beyond the
  ran/skipped record and the counts that already exist (`RemeasuredNodeCount`,
  `FullRenderCount`, `PointerSamplesReceived`, `PointerMovesProcessed`,
  `FullRenderFallbackCount`). No renderer rewrite, no Avalonia/WPF redesign, no new
  platform/release/distribution scope. The full-render path stays as feature 110's
  oracle/fallback (unchanged).
- **Build-target impact**: Escalated controls-public-surface set because of the
  `ControlsElmish.fsi` change: run `Route` first and obey its printed list
  (`Dev`, the package/per-package surface diffs, `FsiTranscripts`,
  `GeneratedProductCheck`, the controls catalog/doc/interaction/rendering checks,
  `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`).
  `RefreshSurfaceBaselines` must regenerate the surface + per-package baselines after
  the `FrameMetrics` field additions. No new gate is introduced.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of produced frames in the corpus report a `FrameCause` that
  matches their trigger class, byte-stable across repeated runs.
- **SC-002**: An idle frame reports `FrameCause = Idle` and **zero** work — every
  phase field (view, diff, layout, paint) reported not run.
- **SC-003**: A pointer-move frame and an animation-only tick each report the
  **view phase skipped** (`host.View` did not run) while the rendered output is
  byte-identical to the pre-feature output.
- **SC-004**: A frame whose product message changed the model reports the view,
  diff, layout, and paint phases **run** (`ViewCalled = true`), exactly as before.
- **SC-005**: The `FrameCause` + phase record is deterministic/byte-stable across
  repeated runs of the same script (golden-asserted); timing fields stay excluded
  from goldens.
- **SC-006**: The regenerated corpus goldens carry the new cause/phase fields and
  show the hover/animation frames as view-free, with the before/after delta recorded
  and **no** rendered-scene/geometry golden delta.
- **SC-007**: At-rest rendered output, control geometry, focus/keyboard routing
  semantics, and every dispatch outcome are byte-identical to the pre-feature state.
- **SC-008**: Continuous pointer movement and continuous animation produce at most
  **frame-rate** view/diff/paint work — zero per-sample `host.View` rebuilds — proven
  by the deterministic counts and phase records.

## Key Entities

- **FrameCause** (public, added to `FrameMetrics`): the closed trigger taxonomy
  `{ Idle, PointerMove, PointerDiscrete, Key, Tick, Resize, Theme }` naming why each
  frame ran. Deterministic, golden-asserted.
- **Phase-invalidation record** (public, added to `FrameMetrics`): four boolean
  fields stating which work phases ran versus were skipped (view, diff/reconcile,
  layout, paint). Makes "skipped phases" explicit (the Phase 3 acceptance gate).
  Hit-test is intentionally not a field (clarified 2026-06-12 — see FR-002).
  Deterministic, golden-asserted.
- **Frame scheduler** (internal, in `Controls.Elmish`): enqueues native input,
  flushes at the frame/tick boundary, classifies the cause, and runs only the phases
  the cause requires — reusing the already-produced view tree when the model is
  unchanged. Consumes the existing retained frame; no new consumer API.
- **Reused view tree** (internal): the most recent `host.View` output for the
  current `(model, size)`, reused on a model-unchanged frame so `host.View` is not
  re-run (FR-003). The mechanism that makes hover/animation/input frames view-free.
- **FrameMetrics record**: gains `FrameCause` + the phase record; `ViewCalled`
  narrows (precise meaning kept, value flips on model-unchanged frames). The
  observability contract this feature extends.
- **Perf script / corpus**: the feature-109 `FrameInput` scenarios whose goldens are
  regenerated to carry the cause/phase fields and evidence the view-free frames.

## Assumptions

- **"Next part" = Phase 3** (Frame Scheduler and Phase Invalidation Model). Feature
  110 delivered Phase 2; the report stages Phase 3 next and lists it as "Do next"
  #1. Phase 4+ is out of scope.
- Features 108/109/110's `FrameMetrics`, `Perf.runScript`, scenario corpus, retained
  routing, and retained render pipeline are merged and are the foundation this
  feature extends — not rebuilt.
- `host.View` is a **pure** function of `(model, size)` (the MVU contract), so
  reusing its output on an unchanged `(model, size)` is byte-identical to re-running
  it. If a consumer view were impure, the reuse would still match the pre-feature
  behaviour for that frame because the pre-feature path also called the pure view;
  the byte-identity claim is scoped to the documented pure-view contract.
- The full-tree runtime visual-state stamp stays in place (Phase 4 narrows it); a
  hover/focus change still re-stamps the reused tree — Phase 3 removes only the
  redundant `host.View` call, not the stamp.
- Dispatch and cause/phase parity are asserted by deterministic counts/booleans and
  structural comparison (controls have no general value equality), using the
  techniques established in features 109/110.
- Live `OnFrameMetrics` remains the best-effort sink; `Perf.runScript` remains the
  authoritative byte-stable surface for the cause/phase goldens.

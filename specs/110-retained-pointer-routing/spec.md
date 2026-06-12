# Feature Specification: Retained-Frame Pointer Routing (Remove Full-Render Pointer Hot Path)

**Feature Branch**: `110-retained-pointer-routing`
**Created**: 2026-06-12
**Status**: Draft
**Input**: User description: "@docs/reports/2026-06-12-1422-controls-performance-framework-research.md do next part."

**Source report** (local in-repo report, not a remote URL — no `source-spec.md`
snapshot per the specify FR-016 no-op rule):
`docs/reports/2026-06-12-1422-controls-performance-framework-research.md`. This
feature implements the **next part** of that report's staged implementation plan
after feature 109 (which delivered Phase 0 + Phase 1) — namely **Phase 2: Remove
Full-Render Pointer Routing from the Hot Path**, which is also the report's "Do
first" priority **#2** and the explicit subject of its *Final Recommendation*:
"The most important concrete follow-up after feature 108 is retained pointer
routing. As long as pointer input can call `host.View` plus `Control.renderTree`
for routing, controls will still feel slow under movement regardless of how good
`RetainedRender.step` becomes." Everything from Phase 3 onward (frame scheduler,
narrowed visual-state stamping, memoization, virtualization, paint/damage caches,
layout caches, backend review) remains **out of scope** — see *Unsupported
scope*.

## Why this feature (context)

Feature 109 (merged 2026-06-12) made the per-frame metrics truthful and built a
reproducible scenario corpus with honest baselines. Those baselines now make the
central hot-path defect *visible in counts*: on an ordinary pointer move or click
the live host still routes input by calling `host.View` and `Control.renderTree`
to materialize a brand-new control tree, hit-test it, and dispatch from it. That
is a full immediate-mode render **per routed pointer sample**, riding on top of an
otherwise-retained pipeline (`RetainedRender.step`, `retainedHitTest`, per-node
cached boxes, stable `RetainedId`). Under continuous movement this is the single
largest avoidable cost the report identifies, and it defeats the value of every
retained mechanism already shipped (features 091/092/096–103).

This feature routes pointer hit-testing and event-binding dispatch **from the
retained frame** instead of from a fresh full render. The retained frame already
carries everything required — stable per-node `RetainedId`, cached boxes for
`retainedHitTest`, and the frame's `ControlRenderResult` (`EventBindings`,
`BoundIds`, `Bounds`). The only missing link is a retained-id → authored-control
lookup so composite controls still dispatch their authored bindings. The result
must be **dispatch-identical** to the current full-render path (a parity oracle is
retained for exactly this proof) while performing **zero routing full-renders**.
It is a hot-path *mechanism* change, not a semantics change: at-rest rendered
output, control geometry, and dispatch outcomes stay byte-identical.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Pointer input routes from the retained frame, not a full render (Priority: P1)

A framework maintainer drives pointer moves and clicks through the live host
after the first frame. Today each routed sample triggers `host.View` +
`Control.renderTree` to produce a throwaway tree just to find what is under the
cursor and which binding to fire. After this feature, routing resolves the hit
and the binding from the already-built retained frame, so a pointer sample does
**no** full-tree rebuild for routing.

**Why this priority**: This is the report's headline follow-up and the largest
avoidable per-sample cost. Until it lands, continuous movement stays at
sample-rate full-render work no matter how good the retained step is.
Independently valuable: even with no other phase, removing routing full-renders
makes movement scale with the retained frame instead of the control count.

**Independent Test**: Run a scripted pointer-move-then-click sequence (after an
initial render) through the deterministic perf path and assert, via the metrics,
that the number of full renders performed *for routing* is zero — while the
move/click still produce the correct hit and the correct dispatched messages.

**Acceptance Scenarios**:

1. **Given** an initial render has produced a retained frame, **When** a pointer
   move is routed, **Then** routing performs **no** `host.View` +
   `Control.renderTree` full rebuild — the hit is resolved from the retained
   frame's cached boxes.
2. **Given** a retained frame, **When** a pointer press/click is routed over an
   authored, bound control, **Then** the binding is dispatched using the retained
   frame's event-binding/bound-id data, again with no routing full-render.
3. **Given** a click whose dispatched message changes the consumer model, **When**
   the next frame is produced, **Then** the model-driven re-render proceeds
   normally — that render is a legitimate model update, **not** a routing
   full-render, and is counted separately.
4. **Given** a burst of pointer-move samples in one frame, **When** the frame is
   processed, **Then** at most one move is processed (feature 108/109 coalescing)
   **and** that processing performs zero routing full-renders.

---

### User Story 2 - Retained routing is dispatch-identical to the full-render path (Priority: P1)

A maintainer must trust that switching the routing mechanism changed *nothing*
observable: the same pointer event over the same scene must resolve to the same
control and dispatch the same messages (and the same focus result) as the
previous full-render path — including for **unkeyed same-kind siblings**, which
only stay distinguishable through retained identity.

**Why this priority**: A faster-but-wrong router is unacceptable; correctness
parity is the gate that lets the optimization land. P1 because the optimization
(US1) cannot be accepted without it.

**Independent Test**: For a set of representative scenes (keyed controls, unkeyed
same-kind siblings, composite controls with authored bindings, nested
containers), route identical pointer events through both the retained path and the
preserved full-render oracle and assert the dispatched message lists, the matched
control identity, and the focus outcome are equal.

**Acceptance Scenarios**:

1. **Given** any pointer event and scene, **When** it is routed through the
   retained path and through the preserved full-render oracle, **Then** the
   dispatched messages are equal.
2. **Given** a scene with unkeyed same-kind siblings, **When** a pointer event
   hits one of them, **Then** retained identity selects the same sibling the
   full-render path would have, and the correct authored binding fires.
3. **Given** a composite control whose authored binding lives above the hit node,
   **When** the hit resolves to a retained id, **Then** the retained-id →
   authored-control lookup dispatches the same authored binding the full-render
   path would have dispatched.
4. **Given** a click that also moves focus, **When** routed via the retained path,
   **Then** the resulting focused identity equals the full-render path's result.

---

### User Story 3 - The fallback path is observable and stays off the normal hot path (Priority: P2)

A maintainer needs to know, in deterministic counts, whether retained routing ever
had to fall back to a full render to route — because such a fallback is a
correctness escape hatch, not the intended path. The corpus pointer scenarios
must show this fallback count at **zero**, and the full-render path must survive
only as a parity oracle / diagnostic fallback, never as the normal live route.

**Why this priority**: The report requires the full-render path be kept "only as a
test oracle or fallback diagnostic, not the normal live path," and asks for a
`FullRenderFallbackCount` metric required to be zero for normal scripted pointer
scenarios. P2 because it hardens and proves US1/US2 rather than delivering the
mechanism itself.

**Independent Test**: Run the corpus pointer scenarios through the deterministic
perf path and assert the new fallback counter is zero in every normal scenario;
separately, force the fallback (a deliberately unroutable case) and assert the
counter increments and the oracle still produces the correct dispatch.

**Acceptance Scenarios**:

1. **Given** any normal scripted pointer scenario in the corpus, **When** it runs,
   **Then** the routing-fallback count is **zero** for every frame.
2. **Given** a deliberately constructed unroutable case, **When** the retained
   router cannot resolve it and falls back, **Then** the fallback counter
   increments by one and the fallback dispatch still matches the oracle.
3. **Given** the feature-109 corpus pointer goldens, **When** they are
   regenerated after this feature, **Then** their routing full-render counts drop
   to zero (the recorded before/after delta the report's Phase 0 baseline exists
   to capture).

---

## Requirements *(mandatory)*

### Functional Requirements

**Retained routing (Phase 2 core)**

- **FR-001**: Pointer hit-testing on the live hot path MUST resolve the control
  under the cursor from the **retained frame** (stable `RetainedId` + per-node
  cached boxes via `retainedHitTest`), NOT by materializing a fresh tree with
  `host.View` + `Control.renderTree`.
- **FR-002**: Pointer event-binding dispatch on the live hot path MUST use the
  retained frame's event-binding / bound-id data (the frame's existing
  `EventBindings` / `BoundIds`), NOT a freshly rendered `ControlRenderResult`.
- **FR-003**: The framework MUST provide a retained-id → authored-control-id
  lookup so that a hit resolving to a retained node still dispatches the correct
  **authored** binding (including composite controls whose binding is authored
  above the hit node).
- **FR-004**: After an initial render, routing a pointer move or click MUST
  perform **zero** full `host.View` + `Control.renderTree` rebuilds *for the
  purpose of routing*. (A subsequent model-driven re-render caused by a dispatched
  message is a normal frame, not a routing render — see FR-008.)
- **FR-005**: Unkeyed same-kind siblings MUST remain distinguishable through
  retained identity: retained routing MUST select the same sibling, and fire the
  same binding, that the full-render path would have.

**Parity & fallback (Phase 2 correctness)**

- **FR-006**: For every pointer event over every scene, retained routing MUST
  produce dispatch results — the dispatched message list, the matched control
  identity, and the focus outcome — **equal** to the preserved full-render path.
  This parity MUST be proven by tests comparing the two paths directly.
- **FR-007**: The existing full-render routing path MUST be **preserved** as a
  parity oracle and a diagnostic fallback only; it MUST NOT be the normal live
  route. When retained routing cannot resolve an event, the system MAY fall back
  to the full-render path to preserve correctness, and MUST count that fallback
  (FR-009).

**Metrics (Phase 2 observability)**

- **FR-008**: The `FullRenderCount` field (feature 109) MUST continue to count
  only genuine `host.View` + `Control.renderTree` materializations; routing a
  pointer event via the retained path MUST NOT increment it. A model-driven
  re-render after a dispatched message MAY increment it as it does today.
- **FR-009**: `FrameMetrics` MUST gain a deterministic integer
  `FullRenderFallbackCount` that counts how many times retained routing fell back
  to a full render *to route an event* in the frame. For all normal scripted
  pointer scenarios this MUST be zero. Adding this field is a breaking public
  `.fsi` change; every `FrameMetrics` construction and read site (samples, FSI
  preludes, tests, surface / per-package baselines) MUST be updated in the same
  change.
- **FR-010**: The feature-109 corpus pointer scenario goldens MUST be regenerated
  so that their routing full-render counts reflect the new zero-full-render hot
  path, recording the before/after delta the Phase 0 baseline exists to capture.

**Behavior preservation (cross-cutting)**

- **FR-011**: This feature is a **hot-path mechanism change only**. At-rest
  rendered output, control geometry, focus/keyboard routing semantics, and every
  *dispatch outcome* MUST remain **byte-identical** to the pre-feature state. The
  only intended observable changes are (a) fewer full renders performed for
  routing and (b) the new `FullRenderFallbackCount` / changed routing counts in
  the observability surface. No pixel, layout box, or message-dispatch result may
  differ.
- **FR-012**: Coalescing fidelity from features 108/109 MUST be preserved:
  discrete press/release/click/scroll are never dropped, a move burst still
  collapses to ≤ 1 processed move per frame, and drag/freehand path fidelity for
  path-consuming consumers is retained — all now routed through the retained path.

> Interacting / conflicting requirements: FR-004 (zero routing full-renders) vs
> FR-007 (full-render path preserved as oracle/fallback) — resolution: the
> full-render path stays *in the codebase* and is reachable as a parity oracle and
> a counted last-resort fallback, but it is removed from the *normal* live route;
> "zero" in FR-004 is the normal-path requirement, while FR-009's
> `FullRenderFallbackCount` makes any deviation from it observable rather than
> silent. FR-009 (breaking `FrameMetrics` field addition) vs FR-011 (byte-identical
> behavior) — resolution: the *shape* of the observability contract changes (a new
> field, a surface/baseline update), but no rendered pixel, layout box, or dispatch
> outcome changes; an observability-surface change is not a behavior change.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Touches `FS.Skia.UI.Controls.Elmish` — the live pointer
  route (`routeInteractivePointer` and its call site inside `runInteractiveApp`),
  the `FrameMetrics` record, and the `Perf.runScript` corpus driver — and consumes
  the **internal** `FS.Skia.UI.Controls` retained surface (`RetainedRender`:
  `step` output, `retainedHitTest`, per-node `RetainedId`/cached boxes, and the
  frame `ControlRenderResult`'s `EventBindings`/`BoundIds`). A retained-id →
  authored-control-id lookup is added on the retained-frame output (internal). No
  package identity changes; `Controls.Elmish` package **contents** change and its
  version bumps on merge. `RetainedRender` stays `internal` (its `.fsi` hides the
  surface; tests reach it via `InternalsVisibleTo`); any new retained-routing seam
  added there is internal, not consumer API.
- **Public contract impact**: `FrameMetrics` gains a deterministic integer
  `FullRenderFallbackCount` (FR-009) — a breaking public `.fsi` change in
  `ControlsElmish.fsi`, so surface baselines and per-package baselines update and
  `Route` escalates to the **controls-public-surface** tier. XML-doc on the new
  field is required (doc-preservation gate). The public `routeInteractivePointer`
  signature is **retained** as the parity oracle / fallback; if the live host
  needs a retained-aware variant it is added as an **internal** seam (consuming the
  internal `RetainedRender`), so the public routing signature does not gain an
  internal-typed parameter. `Perf.runScript` corpus evidence gains the
  zero-routing-full-render goldens.
- **State workflow impact**: None to MVU semantics — `Update`, effects,
  subscriptions, commands, and interpreter behavior are unchanged. Dispatch
  *outcomes* are byte-identical (FR-006/FR-011); only the routing *mechanism* that
  produces the dispatched messages changes.
- **Layout/rendering impact**: None to rendered output — at-rest scene, geometry,
  and the retained step are byte-identical (FR-011). Hit-testing moves from a
  fresh full render to the retained frame's cached boxes; this changes *how* a hit
  is found, not *what* is hit. No Vulkan/Skia/visual-output change; no
  unsupported-environment diagnostic change.
- **Evidence obligations**: Parity evidence comparing retained routing to the
  full-render oracle (FR-006); regenerated corpus pointer goldens showing zero
  routing full-renders and the recorded before/after delta (FR-010); a
  forced-fallback test proving `FullRenderFallbackCount` increments and the oracle
  still dispatches correctly (FR-007/FR-009); skill-loading evidence; the
  window-visibility not-applicable set if the audit fires on literal filenames;
  `readiness/evidence-audit.md` with a verdict token; the generated-validation
  package-resolution tokens. The escalated `maintainer-verify` readiness set
  applies because of the `.fsi` change.
- **Unsupported scope**: This feature is **Phase 2 only**. Explicitly OUT: the
  `FrameCause`/`FrameInvalidation` frame scheduler (Phase 3), narrowed runtime
  visual-state stamping (Phase 4), view/control memoization (Phase 5), viewport
  virtualization for DataGrid/list (Phase 6), damage rectangles / Skia picture /
  paint caches (Phase 7), text-measurement / layout-boundary caches (Phase 8), and
  any `SkiaViewer` backend / render-thread / compositor review (Phase 9). No
  renderer rewrite, no Avalonia/WPF redesign, no new platform/release/distribution
  scope, and no removal of the full-render path (it is preserved as oracle/
  fallback).
- **Build-target impact**: Escalated controls-public-surface set because of the
  `ControlsElmish.fsi` change: `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`,
  `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit` (run `Route` first and
  obey its printed list). `RefreshSurfaceBaselines` must regenerate the surface +
  per-package baselines after the `FullRenderFallbackCount` addition, and every
  `FrameMetrics` construction site (samples, FSI preludes) must be updated or the
  build breaks. No new gate is introduced.

## Success Criteria *(mandatory)*

- **SC-001**: After an initial render, a routed pointer move performs **zero**
  full `host.View` + `Control.renderTree` rebuilds for routing.
- **SC-002**: A routed pointer press/click performs zero routing full-renders and
  dispatches the correct authored binding from the retained frame.
- **SC-003**: For **100%** of tested pointer events across representative scenes,
  retained routing's dispatched messages, matched identity, and focus outcome
  equal the preserved full-render oracle's.
- **SC-004**: Unkeyed same-kind siblings are routed to the same sibling (and fire
  the same binding) as the full-render path in **every** tested case.
- **SC-005**: Every normal scripted pointer scenario in the corpus reports
  `FullRenderFallbackCount = 0` for every frame.
- **SC-006**: A deliberately forced-fallback case increments
  `FullRenderFallbackCount` and still dispatches identically to the oracle.
- **SC-007**: The regenerated corpus pointer goldens show routing full-render
  counts dropping to zero relative to the feature-109 baseline (recorded
  before/after delta).
- **SC-008**: At-rest rendered output, control geometry, and all dispatch outcomes
  are byte-identical to the pre-feature state.
- **SC-009**: A burst of N pointer-move samples in one frame still reports
  processed-moves ≤ 1 with zero routing full-renders, and no discrete
  press/release/click/scroll is dropped.

## Key Entities

- **Retained frame** (`RetainedRender` step output, internal): stable per-node
  `RetainedId`, cached boxes, and the frame's `ControlRenderResult`
  (`EventBindings`, `BoundIds`, `Bounds`). The source of truth retained routing
  reads instead of a fresh render.
- **Retained-id → authored-control lookup**: the bridge (internal, on retained
  frame output) from a `retainedHitTest` result to the authored control id whose
  binding must fire — required for composite controls.
- **FrameMetrics record**: gains deterministic int `FullRenderFallbackCount`;
  `FullRenderCount` semantics narrow so routing never increments it. The
  observability contract this feature extends.
- **Full-render routing oracle**: the preserved `host.View` +
  `Control.renderTree` route, used only for parity proof and counted fallback —
  never the normal live path.
- **Perf script / corpus**: the feature-109 `FrameInput` pointer scenarios whose
  goldens are regenerated to evidence the zero-routing-full-render result.

## Assumptions

- **"Next part" = Phase 2** (Remove Full-Render Pointer Routing from the Hot
  Path). Feature 109 delivered Phase 0 + Phase 1 ("first part"); the report stages
  Phase 2 next, lists it as "Do first" #2, and names it the most important
  follow-up in its Final Recommendation. Phase 3+ is out of scope.
- Feature 109's truthful `FrameMetrics`, `Perf.runScript`, and scenario corpus are
  merged and are the foundation this feature extends — not rebuilt.
- The retained frame already carries enough data to route (stable `RetainedId`,
  cached boxes, `EventBindings`, `BoundIds`); the only genuinely new artifact is
  the retained-id → authored-control lookup. If a corner case cannot be resolved
  from the retained frame, the counted full-render fallback (FR-007/FR-009)
  preserves correctness rather than silently mis-dispatching.
- `RetainedRender` remains assembly-internal; any retained-aware routing seam is
  internal and reached by `Controls.Elmish` within the assembly boundary and by
  tests via `InternalsVisibleTo`.
- The public `routeInteractivePointer` function is retained (oracle/fallback) so
  consumers depending on it keep working; live routing is wired through the
  retained path inside `runInteractiveApp`.
- Dispatch parity is asserted by structural equality of message lists / identities
  / focus outcomes (controls have no general value equality, so comparisons use the
  same techniques established in prior controls features).

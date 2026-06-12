# Phase 0 Research: Honest Frame Metrics & Performance Baseline Corpus (feature 109)

All five spec ambiguities were resolved at `/speckit-clarify` (2026-06-12); no
`NEEDS CLARIFICATION` remain. This document records the design decisions that
turn those resolutions into a buildable plan, grounded in the **existing**
feature-108 code in `src/Controls.Elmish/ControlsElmish.fs`.

## D1 — How to split `ViewRebuilt` into `ProductModelChanged` + `ViewCalled`

- **Decision**: Replace the single `ViewRebuilt: bool` with two booleans.
  `ProductModelChanged` = a product message actually changed the model this frame;
  `ViewCalled` = `host.View size model` ran this frame to (re)produce a tree.
- **Rationale**: Today both branches of `Perf.runScript` and the live
  `emitFrameMetrics` derive a single `rebuilt = not (List.isEmpty msgs)` and then
  *also* gate `renderStep` on it (`let remeasured = if rebuilt then renderStep ()
  else 0`). So "model changed" and "view ran" are coincidentally equal in the
  current code — which is exactly why the conflated name is dangerous: a future
  optimization (e.g. host-state-only repaint, or skipping `View` when the model is
  reference-equal) will make them diverge, and the metric must already report the
  two facts separately so the optimization is *provable*. The split is therefore a
  prerequisite, not cosmetic (US1 / report Phase 1 task 3).
- **`ProductModelChanged` detection without a `'model` equality constraint**:
  `'model` has no `equality` constraint and `Control<'msg>` has no equality
  (memory notes the `%A`-compare workaround used elsewhere). Detect change by
  **reference identity** of the folded model: capture `model` before
  `applyMessages msgs`, compare `not (obj.ReferenceEquals(before, model))` after.
  This is honest for the metric's purpose ("did a product message change the
  model") — an `update` that returns a structurally-identical *new* record still
  counts as a change (it produced new state to view), and a no-op message list
  leaves the reference untouched → `false` (FR-003). No new constraint is added to
  the public host signature.
- **Alternatives considered**: (a) keep `ViewRebuilt` and just document it —
  rejected by the spec (SC-011 forbids any conflating name surviving; clarified
  "Replace it"). (b) Structural model equality via `=` — rejected: requires an
  `equality` constraint the public `InteractiveAppHost<'model,'msg>` does not
  impose and would be a breaking host-signature change far beyond scope.

## D2 — `ViewCalled` / `FullRenderCount` semantics

- **Decision**: `ViewCalled: bool` is true iff `host.View` ran for the frame.
  `FullRenderCount: int` counts **full `host.View` + `Control.renderTree` rebuilds**
  for the frame (FR-015).
- **Rationale & subtlety**: in the current `Perf.runScript`, `host.View` is
  invoked in **two** places per frame that produces messages: once inside
  `routeInteraction` (to build the rendered tree for pointer binding resolution —
  `Control.renderTree host.Theme size (host.View size model)`) and once inside
  `renderStep` (`host.View size model` → `RetainedRender.step`). A truthful
  full-render count must reflect the **render-pipeline** rebuilds, i.e. the
  `renderStep`/`RetainedRender` rebuilds (and, where pointer routing forces a
  `Control.renderTree`, that is itself a full materialization the report wants
  counted). **Resolution**: `FullRenderCount` counts each `host.View` +
  `Control.renderTree` materialization the frame performs (the routing render
  *and* the retained-step render when they occur), so the baseline answer to
  "how many full renders happened for this interaction" is honest and is exactly
  the number Phase 2 (retained pointer routing) will drive toward zero on the
  hot path. `ViewCalled` is `FullRenderCount > 0`. The animation-only `Tick`
  branch renders an **overlay step** (bounded remeasure, *not* a whole-tree
  rebuild) — it sets `ViewCalled = true` (View ran to assemble the overlay) but
  this is captured as a render with `RemeasuredNodeCount` bounded; the baseline
  notes animation-overlay frames distinctly so a later memoization phase can be
  measured against them.
- **Alternatives considered**: counting only `renderStep` and ignoring the
  routing `host.View` — rejected: it would under-report real full renders and hide
  exactly the cost Phase 2 removes (the report explicitly calls out
  "full-render pointer routing" as the Phase 2 hot path). Counting every internal
  `host.View` call indiscriminately (including any incidental) — accepted only for
  the *render+renderTree* materializations that actually rebuild a tree, not for
  cheap field reads, so the count maps to real work.

## D3 — Once-per-frame emission (FR-007 / SC-010)

- **Decision**: `OnFrameMetrics` fires exactly once per **produced frame**.
  `Perf.runScript` already yields one `FrameMetrics` per `toFrames` frame (a
  coalesced move-burst is a single frame), so its determinism surface is already
  correct; the verification is a count assertion. For the **live** loop, audit
  `runInteractiveApp` so `emitFrameMetrics` is called once per processed frame and
  never once-per-flush-boundary with ambiguous aggregated counts (the existing
  code paths at ~lines 856/873 each emit once; the test pins that a multi-sample
  burst frame emits a single metrics record with `PointerSamplesReceived = N`,
  not N records).
- **Rationale**: report Phase 1 task 6; SC-010. No behavior change is needed if
  the audit confirms single emission — this story *verifies and pins* rather than
  rebuilds (US3 is P2 precisely because the mechanism exists from 108).

## D4 — Coalescing fidelity verification (FR-008..FR-011)

- **Decision**: Add tests, not behavior. `PointerSamplesReceived` already counts
  raw samples (`k = List.length frame`); pin that it includes deferred/queued
  moves. `PointerMovesProcessed` is hard-coded to `1` on a coalesced frame and `0`
  on non-move frames — pin `≤ 1` for any burst. Discrete interactions
  (press/release/click/scroll/secondary/drag begin/end/cancel) are *never* in a
  coalesced frame (`isMoveInteraction` returns true only for
  `HoverEnter`/`HoverLeave`/`DragMove`), so `toFrames` already isolates them —
  pin that none is dropped. Drag **path fidelity**: `DragMove` carries its raw
  sample; the coalesced frame keeps the *latest* for routing/repaint while the raw
  path remains reconstructable from the script — pin that a path-consuming
  consumer can still obtain the full sample sequence.
- **Rationale**: report Phase 1 tasks 1–2; FR-008..FR-011 / SC-002/003. This is
  the "verify, don't assume" guardrail; the coalescing already shipped in 108.
- **Alternative**: re-implement coalescing — rejected, out of scope and would risk
  FR-020 byte-identity.

## D5 — `FrameDuration` real timing kept out of goldens (FR-012)

- **Decision**: In the **live** loop, `FrameDuration` becomes real wall-clock
  (measured around the frame's work) for diagnostics. In `Perf.runScript` (the
  deterministic golden driver) `FrameDuration` stays `TimeSpan.Zero` /
  unobserved, and the golden serializer **omits** `FrameDuration` (and allocation)
  entirely so goldens are byte-stable run-to-run (SC-005/009).
- **Note on the no-`Date.now` rule**: deterministic golden code never reads the
  clock; only the non-golden live/report path measures time. This keeps the golden
  path pure and the timing path honest, with no clock call inside anything golden.
- **Rationale**: FR-012; report Phase 1 task 5. Timing varies by machine and must
  never gate (counts gate, timing informs — FR-018).

## D6 — Scenario corpus shape & golden format

- **Decision**: The corpus is a list of named `PerformanceScenario` values
  (name, parameters, an ordered `FrameInput<'msg> list` script, and the host under
  test) defined in a **test/evidence fixture** (`tests/Elmish.Tests`), *not* a
  shipped `Controls.Elmish` helper (clarified 2026-06-12 — package surface stays
  minimal). Required scenarios (FR-013): hover sweep over 100 / 1000 / 5000 simple
  controls; DataGrid at 100 / 1000 / 10000 rows on the current fully-materialized
  path; deep nested layout of repeated labels+buttons; text entry in a focused
  field while unrelated controls animate; theme switch across a moderate dashboard;
  continuous drag/freehand of hundreds of raw samples.
- **Golden format**: each scenario serializes its `FrameMetrics list` to a
  committed text golden of **counts + booleans only** — one line per frame with
  `ProductModelChanged`, `ViewCalled`, `FullRenderCount`, `RemeasuredNodeCount`,
  `PointerSamplesReceived`, `PointerMovesProcessed`. `FrameDuration`/allocation are
  excluded by construction. Stored at
  `specs/109-perf-metrics-baseline/readiness/perf-corpus/<scenario>.golden.txt`.
  The test asserts the live run equals the committed golden and that a re-run is
  identical (SC-005).
- **Rationale**: report Phase 0 tasks 1–2; FR-013/FR-014; US2. The corpus is the
  yardstick every later phase is measured against; keeping it in evidence projects
  honours the "no new shipped API" clarification.
- **FR-015 missing-counter honesty**: the report's Phase 0 acceptance also lists
  *paint*, *layout*, and *hit-test* counts. Those phase counters do not exist
  until later (paint/damage Phase 7, retained hit-test Phase 2 — both out of
  scope). The baselines **state explicitly** which phase counters are not yet
  captured (FR-015 resolution: silent omission is not acceptable); the corpus is
  *extended* with them when those phases land.

## D7 — Non-golden report generator & before/after baselines (FR-016..FR-019)

- **Decision**: A **non-golden** report generator (an Expecto evidence test or a
  tiny evidence harness — **not** a FAKE gate, **not** a shipped command) runs the
  corpus and writes per-scenario **timing + allocation** to
  `docs/reports/_baselines/2026-06-12-controls-corpus-before.md` (and `…-after.md`
  for the post-feature state). It records the feature-108 hover-burst **before**
  (no coalescing) and **after** (coalescing) baselines so the benefit is evidenced
  (FR-019 / SC-007), and states regression thresholds **counts-first, timing-second**
  (FR-018).
- **Allocation measurement**: `GC.GetAllocatedBytesForCurrentThread()` deltas
  around a scenario run give a coarse, environment-dependent allocation figure —
  recorded as human-facing evidence only, never gating.
- **Rationale**: report Phase 0 tasks 3–5; FR-016/017/018/019; US4. Timing/
  allocation are environment-dependent and must live *outside* the goldens.

## D8 — Construction-site sweep (breaking field change blast radius)

- **Decision**: Inventory every `FrameMetrics` **record construction** and update
  it: `Perf.runScript` `zero` + 5 per-frame branches and `emitFrameMetrics` in
  `ControlsElmish.fs`; and the reading tests `Feature108MetricsTests.fs`,
  `Feature090DispatchTests.fs`, `Feature098DispatchTests.fs`. `OnFrameMetrics =
  ignore` sites (`template/base/src/Product/EvidenceCommands.fs`,
  `tests/SkiaViewer.Tests/Feature085InteractiveHostTests.fs`) set a *host field*,
  not a `FrameMetrics`, and need **no** change. No `.fsx` FSI prelude constructs a
  `FrameMetrics` (grep clean), so none needs updating — unlike feature 100's
  record-field blast radius. `RefreshSurfaceBaselines` regenerates the two surface
  baseline files.
- **Rationale**: FR-002 mandates same-change update of every construction/read
  site; the build (compile + `PackageSurfaceCheck` + baseline drift) enforces it.

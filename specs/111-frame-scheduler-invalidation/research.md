# Phase 0 Research: Frame Scheduler & Phase-Invalidation Model

All unknowns from the Technical Context are resolved below. Each decision names the
concrete in-repo anchor it relies on (post-feature-110 line numbers).

## R1 — The `FrameCause` taxonomy and per-frame mapping

**Decision**: Add a public closed DU
`[<RequireQualifiedAccess>] type FrameCause = Idle | PointerMove | PointerDiscrete | Key | Tick | Resize | Theme`
to `ControlsElmish.fsi`, and a `FrameCause: FrameCause` field on `FrameMetrics`. The
cause names the **trigger** of the frame, classified by the producing branch:

| Producing site | Cause |
|----------------|-------|
| `Perf` `[ FrameInput.Idle ]` (`ControlsElmish.fs:1272`) | `Idle` |
| `Perf` coalesced-move frame (`:1247`) | `PointerMove` |
| `Perf` `[ FrameInput.Pointer interaction ]` discrete (`:1325`) | `PointerDiscrete` |
| `Perf` `[ FrameInput.Key(k, mods) ]` (`:1307`) | `Key` |
| `Perf` `[ FrameInput.Tick delta ]` (`:1273`) | `Tick` |
| Live `mapPointer` Moved boundary (`:973`) | `PointerMove` |
| Live `mapPointer` discrete (`:973`) | `PointerDiscrete` |
| Live `wrappedTick` (`:1102`) | `Tick` |

`RequireQualifiedAccess` is required for the same reason `FrameInput` carries it
(`ControlsElmish.fsi:71`): the case names `Key`/`Tick`/`Idle` would shadow a
consumer's own `Msg` cases on `open FS.Skia.UI.Controls.Elmish`.

**`Resize` / `Theme`**: kept in the closed set for completeness (the live scheduler
can classify a size change or a theme change between paints), but the deterministic
`Perf.runScript` corpus does **not** produce them — its `FrameInput` script has no
resize input, and a theme switch in the corpus (`theme-switch-dashboard`) is a
**model-driven** theme change, so its frame is `FrameCause = Key` with the theme
changed as an effect (the existing `prev.Theme <> theme` invalidation in
`RetainedRender.step`, `ControlsElmish.fs` retained surface). Recording `Resize`/
`Theme` only on the live sink (best-effort) keeps the closed set honest without a
synthetic corpus frame. The cause is the trigger; whether the model changed is the
existing `ProductModelChanged` (so a model-changing `Key` is `FrameCause = Key` with
`ProductModelChanged = true`).

**Alternatives considered**:
- *A single "ModelMessage" cause distinct from Key/Pointer/Tick* — rejected: it
  conflates trigger with effect. The trigger taxonomy + `ProductModelChanged` reports
  the two facts separately and deterministically.
- *Reuse `FrameInput` as the cause* — rejected: `FrameInput` is generic over `'msg`
  and carries payloads; the cause is a payload-free, comparable classification.

## R2 — The four phase booleans and why `ViewCalled` is the view phase

**Decision**: The phase record is four booleans — `{ ViewCalled, DiffRan, LayoutRan,
PaintRan }` — where the existing `ViewCalled` (`ControlsElmish.fsi:54`) IS the **view
phase** (no duplicate `ViewRan`), and three new fields are added:

| Phase | Field | True when |
|-------|-------|-----------|
| View | `ViewCalled` (existing) | `host.View size model` ran this frame |
| Diff/reconcile | `DiffRan` (new) | a newly-produced view tree was reconciled against the retained tree this frame (in this pipeline view→diff are coupled, so `DiffRan ⟺ ViewCalled`; kept a distinct field per FR-002 explicitness and to stay meaningful when Phase 5 view-memoization decouples them) |
| Layout | `LayoutRan` (new) | ≥1 node was re-measured this frame (`WorkReductionRecord.RemeasuredNodeCount > 0`), set **explicitly** by the scheduler (not inferred at read time, FR-002) |
| Paint | `PaintRan` (new) | the painted scene (a model render) or the animation overlay was (re)assembled this frame |

Resulting per-frame record:

| Frame | View | Diff | Layout | Paint |
|-------|------|------|--------|-------|
| Idle | F | F | F | F |
| Pointer-move (no msg) | F | F | F | F |
| Animation-only tick | F | F | F | **T** |
| Model frame, geometry change (e.g. datagrid orient) | T | T | T | T |
| Model frame, no visual diff (constView) | T | T | F | T |

**Rationale**: `ViewCalled` already means exactly "the view phase ran"; adding a
fifth `ViewRan` would duplicate it. FR-002 requires skipped phases be **explicit**
booleans (not inferred from a counter), so `LayoutRan` is its own field even though
it tracks `RemeasuredNodeCount > 0`. The animation tick is the one paint-without-view
frame: `View=F, Diff=F` (no new tree reconciled — only the overlay is re-sampled),
`Paint=T`.

**Hit-test is NOT a phase field** (clarified 2026-06-12): post-110 the deterministic
`Perf.runScript` path routes a coalesced hover/drag move straight to `MapPointer`
with no hit-test (only a discrete `Click` hit-tests), so a hit-test bool would read
`false` across the entire move-burst corpus — a misleading always-false field.
Hit-test work stays covered by `PointerSamplesReceived` / `PointerMovesProcessed` /
`FullRenderFallbackCount`.

## R3 — Byte-identity of the view-skip (FR-003/FR-008)

**Decision**: Reusing the view output on a model-unchanged frame is byte-identical
because `host.View` is a pure function of `(model, size)` (the MVU contract): when
neither changed, a fresh call returns a tree **equal** to the one already produced.
Two mechanisms, one per driver:

- **Perf driver** (`renderStep`, `ControlsElmish.fs:1175`): an animation-only tick
  (`hadAnimation && not hasMsgs`) reuses `prev.Root.Control` — the tree the previous
  `host.View` produced for this same model — and `RetainedRender.step host.Theme size
  prev prev.Root.Control` produces the all-`Keep` diff + overlay. Because the model
  is unchanged, `host.View size model` would return a tree structurally equal to
  `prev.Root.Control`, so the step input is identical and the overlay is byte-
  identical. `host.View` is **not** called → `ViewCalled = false`, `FullRenderCount`
  drops the tick's `1` → `0`, `PaintRan = true`. (Perf carries no
  `applyRuntimeVisualState`; the corpus view stamps visual state inline via the
  model, so the unchanged-model tree already carries the correct visual state.)

- **Live loop** (`renderRetained`, `ControlsElmish.fs:867`): cache the **un-stamped**
  `host.View size model` output keyed by `(model-reference, size)`. On a paint where
  `obj.ReferenceEquals(model, cachedModel) && size = cachedSize`, reuse the cached
  tree, then **still** run the full-tree `ControlRuntime.applyRuntimeVisualState`
  stamp (hover/focus may have changed) and `RetainedRender.step`. Only `host.View`
  is skipped. Byte-identical: the stamp is `(cached-tree, runtime-state) → tree`, and
  `cached-tree` equals a fresh `host.View size model` (same model instance, same
  size, pure view). A reference-type model that did not change is the same instance →
  reuse; a value-type model (or any changed/uncertain case) re-runs `host.View`
  (safe fallback — byte-identical, no optimization). The deterministic golden surface
  is Perf, which uses the structural `hadAnimation/hasMsgs` signal, so the goldens
  reflect the view-skip without depending on reference identity.

**Open confirmation for implementation**: the live cache MUST invalidate on a theme
change too (`host.Theme` is per-loop and already a `step` reuse key); reuse is gated
on `(model-reference, size)` and the theme is uniform per loop, but if a theme switch
path mutates `host.Theme` between paints the cache key must include it. The view tree
itself is theme-independent (theme drives paint, not the lowered control tree), so
the un-stamped tree cache is theme-safe; only the paint (step) consumes the theme,
which always re-runs.

## R4 — `FullRenderCount` / `ViewCalled` drop and the feature-109 SC-011 contract

**Decision**: On an animation-only tick the view phase no longer runs, so the honest
values change: `ViewCalled : true → false` and `FullRenderCount : 1 → 0` (no
`host.View` + `Control.renderTree` materialization). `ViewCalled`'s **definition** is
unchanged ("`host.View` actually ran"); only its value flips. The feature-109 SC-011
fact — "an animation-only tick does overlay work with no product message" — is
**re-expressed**, not lost: it becomes `ProductModelChanged = false`, `ViewCalled =
false`, `PaintRan = true` (the paint phase did the overlay work). The feature-109
honesty test that asserted `ViewCalled = true` on the tick is **updated** to assert
the new phase record (the scope of the assertion narrows to the precise phase; no
assertion is weakened — Principle VI). The "`ViewCalled = (FullRenderCount > 0)`"
invariant still holds (`false = (0 > 0)`).

**Rationale**: this is the report's explicit Phase 3 goal ("make animation clocks
request paint-only frames"). Carrying it makes `ViewCalled` *more* precise, and the
new `PaintRan` field removes the prior overload of `ViewCalled` as a proxy for "the
frame painted".

## R5 — Scheduler shape and input queue (FR-006)

**Decision**: The feature-108 live coalescing (`pendingMove`/`mapPointer`,
`ControlsElmish.fs:973`) and the feature-110 retained routing already constitute the
"enqueue at the boundary, coalesce moves, process at the frame boundary" behaviour;
Phase 3 **formalizes** it as the cause-classifying scheduler and adds the
phase-skipping (R3) — it does not rebuild the input path. `Perf.runScript`'s
`toFrames` (`ControlsElmish.fs`) is the deterministic analog (it already groups
consecutive moves into one frame). No new live event loop is introduced; the move
burst still coalesces to `PointerMovesProcessed <= 1` and the raw drag path stays
recoverable from the driver script / the per-sample `pendingMove` (feature-108/110
fidelity, FR-006). The scheduler's new responsibility is purely: (1) tag each
produced frame with its `FrameCause`, (2) set the four phase booleans, (3) skip
`host.View` on a model-unchanged frame (R3).

**Rationale**: minimal, additive, and byte-identity-preserving; it reuses the queue/
coalescing the prior two features already shipped rather than re-architecting the
viewer loop (which is Phase 9, out of scope).

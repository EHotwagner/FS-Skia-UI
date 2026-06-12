# Contract: Frame scheduler — cause-driven phase execution

**Module**: `FS.Skia.UI.Controls.Elmish` (internal scheduling, no public function
signature change) consuming the internal `FS.Skia.UI.Controls.RetainedRender`. The
public surface delta is the `FrameCause` + phase fields (see
[frame-cause-phases.md](./frame-cause-phases.md)); the scheduler itself is internal.

## Behavioral contract

For each produced frame the scheduler MUST:

1. **Classify the cause** — assign a `FrameCause` from the producing branch (R1):
   `Idle` / `PointerMove` / `PointerDiscrete` / `Key` / `Tick` live + Perf;
   `Resize` / `Theme` live-only.
2. **Run only the phases the cause requires** — in particular, **skip `host.View`**
   when the cause did not change the product model:
   - **Perf** `[ FrameInput.Tick delta ]` (`ControlsElmish.fs:1273`): for an
     animation-only tick (`hadAnimation && not hasMsgs`), re-sample the overlay by
     stepping `prev.Root.Control` (the retained tree = `host.View` of the unchanged
     model) — **no `host.View`** — so `ViewCalled = false`, `FullRenderCount` loses
     the tick's `1`, `PaintRan = true`. A consumer `Tick` message (`hasMsgs`) is a
     model frame: `host.View` runs as today.
   - **Live** `renderRetained` (`ControlsElmish.fs:867`): reuse the cached un-stamped
     `host.View size model` output when `obj.ReferenceEquals(model, cachedModel) &&
     size = cachedSize`; still run `applyRuntimeVisualState` + `RetainedRender.step`;
     skip only `host.View` (`ViewCalled = false`). Any key mismatch (incl. every
     value-type model) re-runs `host.View` (byte-identical fallback).
3. **Set the phase record** — `ViewCalled` (view), `DiffRan`, `LayoutRan`, `PaintRan`
   per the truth table, deterministically.
4. **Preserve coalescing & routing** — the feature-108 move coalescing
   (`PointerMovesProcessed <= 1`, raw drag path retained) and the feature-110
   retained routing + oracle/fallback are unchanged (FR-006).

## Byte-identity obligation (FR-008 / SC-007)

The rendered scene, control geometry, focus/keyboard semantics, and every dispatch
outcome MUST be byte-identical to the pre-feature state. Proven by:
- **Perf**: stepping `prev.Root.Control` on an animation tick yields the same overlay
  as stepping a fresh `host.View` of the unchanged model (equal step input).
- **Live**: the reused un-stamped tree equals a fresh `host.View size model` (pure
  view in `(model, size)`); the stamp + step always re-run, so the painted scene is
  identical.
- A direct test renders the same animation/hover frame **with** and **without** the
  view-skip (or against the standing Scene-parity golden suite under `Dev`) and
  asserts structural scene equality.

## Determinism obligation (FR-007 / SC-005)

`FrameCause` and the phase booleans MUST re-run byte-identically across repeated runs
of the same `Perf.runScript` script — they join the byte-stable count/bool golden
surface. `FrameDuration` stays excluded.

## Metric-semantic changes (called out)

- An **animation-only tick**: `ViewCalled : true → false`, `FullRenderCount : 1 → 0`,
  `PaintRan : (new) true`. The feature-109 SC-011 "the tick did overlay work" fact is
  re-expressed as `ProductModelChanged = false ∧ PaintRan = true` (the
  `Feature109MetricsHonestyTests` assertion is updated, not weakened).
- The `ViewCalled = (FullRenderCount > 0)` invariant still holds.
- `ProductModelChanged` is unchanged; a model-changing `Key`/`Pointer`/`Tick` is
  `FrameCause = Key/PointerDiscrete/Tick` with `ProductModelChanged = true` and the
  view/diff/layout/paint phases run.

## Out of scope (Phase 4+, deferred)

The full-tree `applyRuntimeVisualState` stamp is **preserved** — narrowing it to
per-identity targeted stamping is Phase 4. The scheduler removes only the redundant
`host.View` *call*; it does not narrow the stamp, add memoization, virtualization,
paint/damage caches, or touch the `SkiaViewer` backend.

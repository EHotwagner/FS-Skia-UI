# Contract: Internal retained pointer route

**Module**: `FS.Skia.UI.Controls.Elmish` (internal seam) consuming the internal
`FS.Skia.UI.Controls.RetainedRender`. No public signature changes (the public
`routeInteractivePointer` is preserved as oracle/fallback).

## Behavioral contract

Given the retained frame for the current model (its cached `LayoutResult`,
`retainedHitTest`, the retained `ControlRenderResult` — `EventBindings`/`BoundIds`/
`Bounds` — and the `RetainedId → ControlId` lookup), the retained route MUST, for a
pointer sample:

1. Map the sample with `Pointer.toMsg` (unchanged fold input).
2. Run `Pointer.update policy retained.Layout pointerMsg state` over the retained
   frame's **cached** `LayoutResult` — performing **no** `host.View` /
   `Control.renderTree` (FR-001/FR-004).
3. For each emitted interaction, resolve the control via `retainedHitTest` →
   `RetainedId` → authored `ControlId` (the lookup) → an `EventBindings` entry on
   that id (FR-002/FR-003); dispatch the bound message. If no binding consumes the
   interaction, fall back to `MapPointer` with the raw interaction exactly as the
   oracle does (additive, order-preserving).
4. Return the advanced `PointerState` + product messages + a fallback flag.

## Parity obligation (FR-006)

For every pointer event over every representative scene, the retained route's
`(dispatched message list, matched control identity, focus outcome)` MUST equal the
preserved `routeInteractivePointer` oracle's. Proven by a direct two-path
comparison test (`Feature110RetainedRoutingParityTests.fs`) over: keyed controls,
**unkeyed same-kind siblings** (FR-005), composite controls whose binding is
authored above the hit node (FR-003), and nested containers. Controls have no
general value equality → compare structurally (the established technique).

## Fallback rule (FR-007 / FR-009)

The retained route MAY fall back to the oracle **only** when it cannot resolve an
event from the retained frame (hit-test `None` over a bindable point, or the lookup
yields no authored id for an interaction the oracle would bind). Each fallback:
- increments `FullRenderFallbackCount` by 1 (and, being a real full render, also
  `FullRenderCount`);
- MUST dispatch identically to the oracle (the fallback *is* the oracle).

For every normal scripted pointer scenario in the corpus,
`FullRenderFallbackCount = 0` for every frame (SC-005). The forced-fallback test
constructs a real unroutable case and asserts the counter increments while dispatch
still matches (SC-006).

## Wiring

- `runInteractiveApp` `processInput` (`ControlsElmish.fs:816-837`) calls the
  retained route instead of `routeInteractivePointer`; the focus-on-click
  `resolveFocus` path (`ControlsElmish.fs:822-833`) is already retained-aware and
  stays.
- `Perf.runScript` `routeInteraction` (`ControlsElmish.fs:1058-1066`) calls the
  retained route over the threaded retained frame instead of re-rendering.
- The step site (`ControlsElmish.fs:763-773`, `1042-1053`) retains `s.Render` so
  the route has the bindings without a fresh render.

## Coalescing fidelity (FR-012, preserved)

Discrete press/release/click/scroll are never dropped; a move burst collapses to
≤ 1 processed move per frame; drag/freehand path fidelity for path-consuming
consumers is retained — all now routed through the retained path. The coalescing
predicate (`mapPointer`, `ControlsElmish.fs:846`) is unchanged; only the per-sample
route it calls changes.

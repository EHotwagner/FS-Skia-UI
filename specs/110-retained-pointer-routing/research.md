# Phase 0 Research: Retained-Frame Pointer Routing

All unknowns from the Technical Context are resolved below. Each decision names
the concrete in-repo anchor it relies on.

## R1 — Bridging `RetainedId` → authored `ControlId`

**Decision**: Add an internal **retained-id → authored-control-id lookup** built
from the retained step output (`RetainedRender.fs`), exposed through
`RetainedRender.fsi` as an internal seam. The retained route hit-tests with
`retainedHitTest x y retained` → `RetainedId option`, then resolves that id to the
authored `ControlId` whose `EventBindings` entry must fire — reproducing, from
retained identity, the resolution `Control.nearestAuthored` performs over a
freshly rendered tree (`ControlsElmish.fs:175-199`).

**Rationale**: Feature 098 already unified the authored `ControlId` to
`Key ?? structural-path` and added `BoundIds: Set<ControlId>` + `boundIdsOf` on
`ControlRenderResult`, and widened `nearestAuthored` to a "keyed-OR-in-BoundIds"
climb. The retained route must land on the *same* authored id the full-render
path would, including the composite case where the binding is authored **above**
the hit node (US2 / FR-003). The retained node tree (`RetainedNode<'msg>` carries
`.Control` and `.Identity`) plus the frame's `BoundIds`/`EventBindings` carry
exactly the data needed; the only missing artifact is the explicit
`RetainedId → ControlId` association, which the lookup materializes during `step`
(when both the retained identity and the authored control are in hand).

**Alternatives considered**:
- *Re-derive the authored id from the hit node's structural path at route time* —
  rejected: it re-implements `nearestAuthored`'s climb at the call site and risks
  drifting from the oracle's scheme (the exact class of bug feature 098 fixed).
- *Store the authored id on each `RetainedNode`* — viable, but a side lookup map
  on the frame output keeps `RetainedNode` unchanged and matches how `BoundIds`
  is already a frame-level set rather than a per-node field.

**Fallback**: if `retainedHitTest` returns `None` over a point the oracle would
have bound, or the lookup cannot resolve an authored id that has a binding, the
route falls back to the preserved full-render oracle and increments
`FullRenderFallbackCount` (R3). Normal corpus scenarios must never hit this.

## R2 — `Pointer.update` over the retained frame's cached `LayoutResult`

**Decision**: Run the existing gesture pipeline
`Pointer.update policy retained.Layout pointerMsg state` against the retained
frame's **already-evaluated** `LayoutResult` (`RetainedRender<'msg>.Layout`),
instead of `Layout.evaluate available rendered.Layout` over a freshly rendered
`LayoutNode` (`ControlsElmish.fs:249`). `Pointer.toMsg` and the 4px click/drag
fold are unchanged; the `policy` is the same `Defaults.pixelSnapPolicy 1.0`.

**Rationale**: The retained step stores the evaluated `LayoutResult` on the
retained frame; the full-render route re-evaluates the identical layout from the
identical model/view/theme/size every sample. Reusing the cached `LayoutResult`
is the whole point of the optimization and is exactly what makes the gesture fold
byte-identical (R4) — the same `LayoutResult` in ⇒ the same interactions out.
Interaction *resolution* to a control then switches from `nearestAuthored` over
`rendered.Bounds` to `retainedHitTest` + the R1 lookup over the retained frame.

**Alternatives considered**:
- *Keep evaluating layout but skip only `host.View`* — rejected: `host.View` +
  `Control.renderTree` is the cost FR-004 targets; half-measures still rebuild the
  tree. The cached `LayoutResult` removes both the view call and the re-evaluate.

**Open confirmation for implementation**: confirm `Pointer.update`'s `layoutResult`
parameter and the retained frame's `Layout` are the same `LayoutResult` type and
coordinate space (the parity test in R4 is the proof obligation if they diverge).

## R3 — Fallback boundary and counter threading

**Decision**: The retained route returns its messages **and** a per-route fallback
flag. A route falls back (and counts +1) only when it cannot resolve an
event from the retained frame (R1 `None` cases). `FullRenderFallbackCount` is
summed per frame and emitted:
- Live loop: a frame-local accumulator threaded through `processInput` →
  `emitFrameMetrics` (`ControlsElmish.fs:804`), alongside the existing
  `fullRenderCount` argument.
- Corpus driver: accumulated in each frame branch of `Perf.runScript` and written
  into every `FrameMetrics` construction site (`ControlsElmish.fs:1076,1107,1144,
  1162,1178`).

`FullRenderCount` semantics **narrow** (FR-008): routing a pointer event via the
retained path increments **neither** `FullRenderCount` nor `ViewCalled`. A
model-driven re-render after a dispatched message still increments
`FullRenderCount` as today. A *fallback* route performs one oracle full-render —
that render is counted by `FullRenderFallbackCount` (its purpose) and, being a
genuine `host.View` + `Control.renderTree`, also by `FullRenderCount`; the spec's
"zero routing full-renders" (FR-004) is the **normal-path** requirement, and any
deviation is made visible by the fallback counter (spec §"Interacting /
conflicting requirements").

**Rationale**: keeps a single honest definition of "a full render happened"
(`FullRenderCount`) while adding a dedicated, golden-asserted "routing had to fall
back" signal (`FullRenderFallbackCount`) that must read zero on every normal
scenario.

## R4 — Byte-identity argument (FR-011)

**Decision**: The retained route is byte-identical in *dispatch* because it feeds
the *same* `LayoutResult` to the *same* `Pointer.update`/`Pointer.toMsg` gesture
fold (R2), and resolves interactions to the *same* authored `ControlId` the oracle
resolves (R1), reading bindings from the *same* `EventBindings`/`BoundIds` the
oracle's `ControlRenderResult` carries. At-rest rendered output and geometry are
untouched (no change to `step`, paint, or layout). The only intended observable
deltas are (a) fewer routing full-renders and (b) the new
`FullRenderFallbackCount` field — both observability, not behavior.

**Proof obligation**: FR-006 parity test compares, for keyed / unkeyed-same-kind
sibling / composite / nested scenes and a representative pointer event set, the
retained route's `(dispatched message list, matched control identity, focus
outcome)` against the preserved `routeInteractivePointer` oracle's, asserting
equality (controls have no general value equality → structural comparison, the
technique established in features 092/098/100). The forced-fallback test proves the
escape hatch dispatches identically too.

**Rationale**: parity-by-construction (shared fold + shared bindings) plus a direct
two-path oracle comparison is the same proof shape features 091/092 used to land
the reconciler on the render path; it is the strongest available evidence short of
pixel goldens, which are unaffected here.

## R5 — Surfacing the retained `ControlRenderResult`

**Decision**: At the step site (`ControlsElmish.fs:763-773`) store `s.Render` in a
new host-loop `ref` (e.g. `lastRender`/`lastBindings`) and seed it from
`r0.Render` on the first frame; in `Perf.runScript` carry the step's `Render`
alongside the retained value already threaded through `renderStep`
(`ControlsElmish.fs:1042-1053`). Today only `s.Render.Scene` is consumed and
`s.Render` is dropped; the routing read set (`EventBindings`, `BoundIds`, `Bounds`)
is already produced — it just is not kept.

**Rationale**: minimal, additive, and entirely internal; it reuses the value the
step already computes rather than recomputing anything, and it is the literal
"only missing link" the spec names.

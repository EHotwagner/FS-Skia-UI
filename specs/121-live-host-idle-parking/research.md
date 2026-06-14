# Phase 0 Research: Live Host Pacing, Surface Honesty & Viewer Ergonomics

All unknowns were resolved by reading the live path end-to-end. No `NEEDS CLARIFICATION`
remains.

## Decision 1 — Reconcile the report to shipped truth before specifying work

- **Decision**: Treat live paint-skip (feature 120) and quit-via-`CloseWindow` as
  **already shipped**; do not re-implement them. Scope the feature to the genuine gaps.
- **Rationale**: Tracing `runInteractiveViewerWithWindowBehavior`
  (`src/SkiaViewer/SkiaViewer.fs` ~2458) → `runPresentedPersistentWindow` (~1226) →
  `Host/OpenGl.fs` `renderFrame`/`shouldPresent` (~481–504) shows the live default
  (`DirectToSwapchain`) already skips clear/scene-walk/draw on an unchanged scene at an
  unchanged framebuffer size. `currentScene` is recomputed only on dispatch/resize, so a
  `host.Tick = None` frame reuses the same reference. `InteractiveAppHost.Update` returns
  `ViewerEffect list` and `CloseWindow` propagates to `AppRequestedClose`/`Shutdown`
  (`SkiaViewer.fs` ~1264–1273). Re-implementing these would create synthetic/duplicate-
  behavior evidence that `EvidenceAudit` blocks.
- **Alternatives considered**: (a) Implement all 10 original FRs literally — rejected:
  produces duplicate behavior + synthetic evidence, bumps a version for existing work.
  (b) Docs-only reconciliation — rejected by maintainer in favor of also closing the real
  gaps (frame-cap, no-alloc tick, api-surface).

## Decision 2 — Frame-cap as an additive, defaulted `ViewerOptions` field

- **Decision**: Add one field to `ViewerOptions` (default = current 60) that flows into
  `ViewerConfiguration.TargetFrameRate`, replacing the hard-coded `Some 60` at
  `SkiaViewer.fs:1234`. Validate it like `InitialSize` (`validateOptions`, ~826).
- **Rationale**: `ViewerConfiguration.TargetFrameRate : int option` already exists and is
  consumed by the loop; the only missing piece is a consumer lever. A defaulted field
  keeps existing construction compiling and at-rest output byte-identical (FR-001/FR-008).
- **Alternatives considered**: A separate `runInteractiveViewerPaced` overload — rejected:
  duplicates the public surface; the field is the smaller, discoverable change.
- **Open shape question** resolved in `contracts/viewer-options.md`: the field is modeled
  as `FrameRateCap: int option` (None ⇒ default 60), mirroring the native config's own
  `int option`, so "uncapped/default" and "explicit N" are both expressible and a record-
  copy default path (`ViewerOptions.defaults`/`withFrameRateCap`) preserves call sites.

## Decision 3 — Make the cap gate render cadence, not only update cadence

- **Decision**: In `Host/OpenGl.fs` `runEventLoop` (~858–885), gate the `DoRender()` call
  by the same `frameInterval` that already gates `DoUpdate()`, so the cap bounds render
  cadence (FR-002). Extract the per-iteration "should we render now?" decision as a pure
  function for unit testing.
- **Rationale**: Today `DoRender()` runs every poll iteration (~1 ms via `Thread.Sleep(1)`)
  regardless of `TargetFrameRate`; only `DoUpdate` is paced. Gating render by the interval
  is what a frame-cap is supposed to mean and is the measurable CPU reduction on a
  no-compositor host. Feature 120's paint-skip already makes each render cheap; pacing
  reduces how often it is even attempted.
- **Alternatives considered**: Event-driven "request redraw" (render only on
  invalidation) — rejected for this rung: a much larger native-loop rewrite, higher risk,
  and the persistent loop is not testable headless. The interval gate is minimal and
  composes with the existing structure.
- **Risk + mitigation**: The persistent loop is not drivable in headless CI. Mitigation:
  unit-test the **extracted pure pacing decision** (given last-render time, now, interval
  ⇒ render?), keep the native wiring a thin call to it, and record the live behavior as
  environment-bound in `readiness/runtime-limitations.md`. Offscreen/evidence paths
  (`runBounded`) do not use `runEventLoop`, so existing evidence is unaffected.

## Decision 4 — Allocation-free idle tick

- **Decision**: In `wrappedTick` (`ControlsElmish.fs` ~1221–1233), guard the
  `StateByIdentity |> Map.map (advance clocks)` with a `Map.exists (clock active)` check;
  when no clock is active, leave `retained.Value` unchanged (reference-equal) before
  calling `host.Tick`.
- **Rationale**: `Map.map` allocates a new map every tick even when every clock is `None`
  or settled — pure garbage on the idle path. `RetainedRender.advance` already no-ops on
  a settled/zero-delta clock, so the guard changes cost only, not behavior (FR-004/FR-008).
  Internal change — no `.fsi` impact.
- **Alternatives considered**: Mutating the map in place — rejected: `Map` is immutable and
  the `exists`-then-`map` guard is the plainest correct form (constitution III).

## Decision 5 — Publish the pointer/host surface

- **Decision**: Add `docs/api-surface/` entries for `PointerInteraction`
  (`src/Controls/Pointer.fsi` ~72–84), `PointerButton` (~9–13), and
  `ViewerPointerPhaseKind` (`src/SkiaViewer/SkiaViewer.fsi` ~539–544), plus a note on the
  `InteractiveAppHost` `MapPointer`/`MapKeyChord` folding contract. Guard against drift.
- **Rationale**: These are already public in their `.fsi` files; they were simply never
  surfaced under `docs/api-surface/`, forcing reflection downstream (FR-005). Publishing
  them is documentation of an existing public surface, not a new API.
- **Alternatives considered**: A bespoke per-control doc — rejected: `docs/api-surface/`
  is the established home and is drift-checked.

## Decision 6 — Viewer-host skill guidance

- **Decision**: Extend the canonical `.agents/skills/fs-skia-viewer-host` skill with
  present-mode selection, the new frame-cap lever, the env-limit free-run note, and the
  already-shipped paint-skip / quit-via-`CloseWindow` facts; regenerate the `.claude` peer
  via `RefreshSurfaceBaselines` (`SkillSyncCheck`-enforced).
- **Rationale**: FR-006/FR-007 are honesty/discoverability obligations; the skill is the
  consumer-facing home and is generated into projects.
- **Alternatives considered**: README-only — rejected: the skill is the discoverable,
  generated artifact and is currency-enforced.

## Cross-cutting: evidence honesty

The persistent interactive window cannot be exercised in headless CI. Every loop change
is therefore proven on extracted pure decisions + reasoning, and the live free-run is
disclosed as an environment limitation, never claimed as an interactive pass — consistent
with features 118/119/120 and `readiness/runtime-limitations.md`.
</content>

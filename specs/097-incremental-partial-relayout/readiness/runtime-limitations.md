# Runtime limitations & unsupported scope (feature 097, R2)

## Platform envelope (unchanged by R2)

target=.NET 10 desktop only.
graphics=Vulkan presentation; SkiaSharp preview rendering packages with explicit version pinning.
unsupported macOS/mobile/browser=not supported targets; R2 adds no new platform surface.
no software-renderer fallback=there is no software-renderer fallback; a missing GPU/window system is an
environment limitation, not a product defect.

## Feature-specific limitations & non-goals (FR-009)

- no virtualization / windowing of large collections (roadmap §6.2 deferred).
- no new layout algorithm and no new public layout type — R2 makes the EXISTING public
  `Layout.evaluateIncremental` do what its signature already promised, and wires it.
- no change to the `view : 'model -> Control<'msg>` consumer contract.
- no data-binding, observable, dependency/attached-property, lookless-template, or CSS-selector surface
  (permanent roadmap non-goals).

## Totality / failure behaviour (constitution VII)

- `evaluateIncremental` is TOTAL: a cache miss, an unrecognised dirty id, a Yoga failure, or duplicate
  `LayoutNodeId`s (Key collisions) all degrade to a full `evaluate` of the affected scope — conservative,
  never a silent divergence and never a throw (contract C1).
- `dirty` is a performance hint, never a correctness input: a wrong dirty set can only cost extra
  re-measure work, never wrong geometry.
- no new diagnostic class; existing layout `Diagnostic` surfacing is preserved verbatim.

## Scope note — Yoga rounding

R2 disables Yoga's INTERNAL pixel rounding (`pointScaleFactor 0`) so partial relayout is byte-identical;
explicit pixel snapping is unchanged via the separate `snapBounds`/`PixelSnapPolicy`. This changes
`Layout.evaluate` output only for fractional/overflow flex layouts (the Controls product path is integer
geometry and is unaffected — 277/277). Maintainer-approved.

# Phase 0 Research: Backend Paint Replay & Performance Honesty

All decisions below resolve the design unknowns; no NEEDS CLARIFICATION remain.

## R1 — Structural fingerprint algorithm (replaces `sprintf "%A"`)

- **Decision**: A total, recursive structural hash `hashScene : SceneNode -> uint64` (64-bit
  FNV-1a-style accumulator; widen to two lanes / 128-bit if collision headroom is wanted),
  exhaustive over all 24 `SceneNode` cases, mixing every render-affecting field: box geometry,
  every `Color`/`Paint` channel, `PathSpec` verbs+coords, text string + `FontSpec`
  (family/size/weight), opacity, transform matrices, and clip shape. Computed **on the
  fragment** in the repaint paths (`paintFresh`/`buildFresh`) and carried unchanged when a
  `Keep` fragment is reused — so its cost is proportional to the damage set (the nodes
  `RepaintedNodeCount` already counts), not to tree size. Stored as a `Fingerprint: uint64`
  field on the fragment record.
- **Rationale**: `%A` truncates past ~100 nodes (default print depth/width → `"..."`), so two
  structurally different deep subtrees can stringify identically → a false **hit**. That is
  harmless only because the current cache is advisory; making the cache load-bearing (US3)
  turns it into a stale-pixel bug. A real structural hash is collision-resistant and is the key
  the replay cache needs anyway, so "fix the key" and "enable replay" are one change. It is
  also far cheaper than allocating a large `%A` string each frame.
- **Backstop**: correctness does not rest on hash uniqueness — the always-direct oracle
  (FR-011) proves on/off byte-identity, so an (astronomically unlikely) collision degrades to a
  missed optimization, never a wrong pixel.
- **Alternatives considered**: (a) keep `%A` but lift the truncation depth — still a stringly
  key, still allocates, still fragile; rejected. (b) `GetHashCode` over the record — F#
  structural hash is not stable/collision-tuned for this and is unspecified across runtimes;
  rejected (goldens need determinism). (c) reference-equality of the reused `SubtreeScene`
  instance as the sole key — works for the common reuse case but cannot survive a rebuild that
  produces a structurally-identical new instance, and couples the IR boundary to object
  identity; kept only as an optional fast-path *before* the fingerprint check, not the key.

## R2 — `CachedSubtree` placement and byte-identity / golden impact

- **Decision**: Add an **additive** `SceneNode.CachedSubtree of CacheBoundary` case
  (`CacheBoundary = { CacheId: uint64; Fingerprint: uint64; Scene: Scene }`). RetainedRender
  splices a single `CachedSubtree` in place of a reuse-stable subtree's raw nodes when
  emitting `sceneList`. **Every existing Scene-IR consumer treats it transparently**:
  `Scene.describe`, `Scene.diagnostics`, `Scene.measure`, and RetainedRender's reduction /
  virtual-items / damage walks recurse straight into `.Scene`. The golden surface is therefore
  unchanged: the deterministic **count** goldens are derived from those walks (which see
  through the wrapper) and from `FrameMetrics`, not from a raw scene dump.
- **Byte-identity definition (explicit)**: "at-rest byte-identity" = (a) presented pixels via
  readback, and (b) the deterministic count/boolean goldens. The internal scene-list *structure*
  gains a transparent wrapper; pixels are identical because replay replays the exact recorded
  command stream and the oracle path recurses; counts are identical because all walks see
  through. New golden lines are only the **additive** replay counters (expected, not a churn).
- **Rationale**: Keeps the win at the backend (where the uncached cost is) while preserving the
  controls-level contracts. The transparent-fallback discipline matches the additive/oracle
  pattern features 112–117 used.
- **Alternatives considered**: (a) extend the existing `PictureNode`/`Picture` record with cache
  fields — touches every `PictureNode` construction site and overloads an existing semantic;
  rejected for a clean additive case. (b) Side-channel `(region, CacheId, Fingerprint)` list
  handed alongside the flat scene — requires fragile positional correspondence between the
  side-channel and the emitted nodes; rejected.

## R3 — Idle / unchanged-frame skip on a double-buffered GL surface

- **Decision**: Track a **frame-dirty signal** in the viewer model, set by: a product model
  message, a resize, a theme change, or an active animation clock (a tick that advances a live
  clock), and cleared after a present. `updateLegacy` emits `RenderFrame` only when dirty;
  otherwise it emits no scene work. Because the GL backend is double-buffered, a skipped frame
  must not leave a stale back buffer on screen: the loop either (i) does not swap (front buffer
  stays valid) or (ii) re-presents the last buffer with **no scene walk**. Either satisfies
  FR-004 ("no scene redraw / draw-call re-issue"); the chosen mechanism is whichever keeps the
  Mesa front buffer correct, verified by the idle-zero-redraw smoke.
- **Rationale**: This is "stop doing redundant work," the cheapest/largest/safest win, and it
  is independent of the cache. The existing `repaintCached`/animation-tick path (feature 111)
  already distinguishes paint-only frames, so the dirty signal slots into that machinery.
- **Alternatives considered**: comparing whole-scene structural equality each frame to detect
  "unchanged" — redundant work itself and defeats the point; the dirty signal is authoritative
  and already mostly present via 111's cause model. Rejected.

## R4 — Backend replay-enable seam for the parity oracle

- **Decision**: The `PictureReplayCache` honors an **enabled** flag (mirrors the controls-level
  `PictureCacheEnabled`/`TextCacheEnabled` oracles). When disabled, `paintNode` recurses into
  `CachedSubtree.Scene` exactly like the direct walk — no record, no replay. The parity test
  renders each corpus scene twice (enabled/disabled) and compares readback pixels (FR-009/011).
  The flag is reachable from tests via a SkiaViewer test seam; whether it is also surfaced on
  public `ViewerOptions` is decided in contracts (default enabled; a test-only internal seam is
  acceptable and preferred to widening the public surface unnecessarily).
- **Rationale**: The oracle is the project's established correctness proof for caches; reusing
  the pattern keeps the proof uniform and golden-checkable.
- **Alternatives considered**: a separate "reference renderer" — duplicate code, drift risk;
  rejected in favor of the same painter with replay short-circuited.

## R5 — `DirtyArea` union (replaces summed area)

- **Decision**: Compute `DirtyArea` as the integer area of the **union** of the distinct
  repainted boxes, not their sum. For the small box counts a frame produces, a simple
  sweep/merge of axis-aligned rectangles (or rectangle-union via inclusion-exclusion for tiny
  sets) is sufficient and deterministic. Result is clamped so it never exceeds the frame area.
- **Rationale**: The current sum double-counts overlapping damage (sum ≥ true area), so the
  "small dirty region" signal is dishonest under overlap and unusable as a future damage-clip
  input. The union is the honest region. Documented authority note updated.
- **Alternatives considered**: keep the sum and rename it "summed area" — preserves a misleading
  metric and blocks the damage-clip follow-up; rejected. A full spatial index — overkill for
  the handful of boxes per frame; rejected.

## R6 — Per-phase paint/compose timing

- **Decision**: Wrap the backend `drawScene` walk in a `Stopwatch` for `PaintDuration`, and the
  `Flush`+`SwapBuffers` present in a second for `ComposeDuration`; surface both as non-golden
  `FrameMetrics` fields (excluded from count goldens, per the 109 `FrameDuration` precedent),
  captured into a new `_baselines` report by the existing timing generator. On the deterministic
  `Perf.runScript` path (no real Skia), these stay `TimeSpan.Zero`, exactly like `FrameDuration`.
- **Rationale**: This is the report's missing precondition; it is cheap and makes US3's win
  measurable rather than asserted. Keeping it out of goldens preserves determinism.
- **Alternatives considered**: allocation accounting per phase (`GC.GetAllocatedBytesForCurrentThread`)
  — useful but noisier; deferred to the non-golden baseline report only, not a metric field.

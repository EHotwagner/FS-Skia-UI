# Phase 0 Research: Paint Cache, Damage Rectangles & Optional Skia Picture Boundaries

All decisions ground in the existing retained pipeline (features 109–114). No NEEDS
CLARIFICATION remains in the spec — its 2026-06-13 clarifications session resolved the
split, the metric publicity, the byte-identity strategy, the memory bound, and the
diagnostic semantics. This document fixes the remaining *implementation* choices.

## (a) Damage-set derivation + dirty rectangles

**Decision.** The damage set is the set of nodes whose `OwnScene` was repainted this frame
plus genuinely-shifted nodes — derived from the retained `step`'s **existing** repaint
decisions, not a new traversal. `paintFresh` (`RetainedRender.fs:494`), `buildFresh`
(`:500`), and the recompute branch of `carry`/`build` (`:515`/`:535-607`) each mark a node
as repainted; the existing `ShiftedNodeCount` accounting marks genuinely-shifted nodes.
Each repainted node contributes its evaluated `Fragment.Box` (`RenderFragment.Box`,
`RetainedRender.fsi:27`) as one axis-aligned damage rectangle.

- `RepaintedNodeCount` = count of nodes in the damage set.
- `DirtyRectCount` = count of **distinct** repainted boxes (default coalescing: one rect per
  repainted node's `Box`; identical boxes deduplicate; `None` boxes contribute nothing).
- `DirtyArea` = sum of `width * height` over the distinct rectangles, integer-rounded
  (geometry is already integer in this codebase — same source 097/101 rely on).

**Rationale.** Reusing the step's own decisions keeps damage **honest** (FR-002's
resolution: damage never under-reports actual repaint) and free of a second walk. Summed
per-node area (vs a unioned bounding region) is the simplest deterministic default and is
sufficient for the report's headline criterion (small vs frame-spanning); a union/merge
coalescer is a deterministic plan option but unnecessary this rung.

**Alternatives considered.** (i) A spatial union of overlapping rects into merged regions —
deterministic but more code; deferred (the count/area signal already distinguishes localized
from frame-spanning). (ii) Sub-pixel/float area — rejected, FR-016 keeps damage integer.

## (b) Picture-cache correctness key

**Decision.** A per-node **correctness key** record/tuple capturing every render-affecting
input the spec enumerates — theme, box, clip, opacity, transform, font/text, visual-state.
The key is built from the inputs the step already has at the reuse site: theme (the step's
`themeChanged`), box (`Fragment.Box`), and the remaining inputs (clip, opacity, transform,
font/text, visual-state) read from the **lowered `Control`/attrs already diffed** by
`Reconcile` at that node. A subtree is a **hit** only when its key equals the cached key in
**every** field; any single difference is a **miss**.

**Rationale.** Today's reuse key is `box + theme` only (`RetainedRender.fs:540`), narrower
than the report's required key — the spec calls a missing keyed input a defect (a stale hit
is "worse than a slow frame"). Building the key from the already-diffed attrs avoids a new
traversal and ties invalidation to the same inputs `Reconcile` tracks. Phase 1
(data-model.md) fixes the exact record shape; the per-keyed-input miss test
(`Feature116PictureCacheTests.fs`) is the proof that no input is omitted.

**Alternatives considered.** A single hash of the lowered subtree — rejected: a hash can
collide and obscures *which* input changed (the per-input test needs field-level
attribution). Comparing the full lowered `Control` structurally — correct but is what
`Reconcile` already does; the key records only the *render-affecting* inputs so an unchanged
subtree with a non-render attr delta still hits.

## (c) Hit/miss counting + always-miss oracle

**Decision.** The widened reuse condition increments `PictureCacheHits` on the reuse branch
(`{ pr with Control = nc }`) and `PictureCacheMisses` on every repaint branch
(`carry`/`paintFresh`/`buildFresh`), carried on `WorkReductionRecord` exactly as
`MemoHits`/`MemoMisses`. A new `PictureCacheEnabled: bool` flag on `RetainedRender`
(mirroring 113's `MemoEnabled`, the always-miss switch) forces every node down the repaint
branch when `false`. The cache-on ≡ cache-off proof renders the same script twice (flag on
vs off) and asserts byte-identical `SubtreeScene` output.

**Rationale.** Direct reuse of the 113 pattern — a proven seam, a proven oracle, zero new
mechanism. The flag is internal (declared in `RetainedRender.fsi`, reached via
`InternalsVisibleTo`), plumbed through `Perf.runScript` for the test.

**Alternatives considered.** A global mutable toggle — rejected, 113 already established the
per-`RetainedRender` field as the testable, thread-safe shape.

## (d) Bounded cross-frame picture cache

**Decision.** A cross-frame store keyed by stable identity (`RetainedId`) holding the cached
fragment + its correctness key, **capped by entry count** with **LRU eviction**. The cap is
a fixed constant (Phase 1 sets the value; a few hundred entries — large enough that the
corpus's stable subtrees never evict spuriously, small enough that the eviction scenario can
exceed it). Recency is tracked deterministically (access order from the frame's traversal
order, no wall-clock). On overflow the least-recently-used entries are dropped;
`PictureCacheEntryCount` = live entry count (`<= cap`). An evicted entry, when next needed,
finds no cache entry → miss → recompute fresh (FR-010). Raw byte size is computed only as a
non-golden diagnostic (excluded, like `FrameDuration`).

**Rationale.** A persistent paint cache that grows unbounded trades a CPU win for a memory
leak — FR-009 makes the bound part of the contract from creation. LRU-by-entry-count is the
simplest deterministic policy with an obviously golden-provable bound; eviction order is
deterministic because recency derives from deterministic traversal order, not time.

**Alternatives considered.** (i) Byte-size cap — rejected for goldens: raw bytes are
non-deterministic across platforms (the spec excludes them); entry count is the deterministic
bound. (ii) No cross-frame cache (per-frame only) — would not realize the report's
cross-frame picture reuse; FR-009 explicitly requires the cross-frame cache *and* its bound.

## (e) Byte-identity at rest + the deterministic/backend split

**Decision.** Accumulating the damage set, counting hits/misses, and maintaining the cache
**do not change the emitted flat `SubtreeScene`** — the step already reuses the *same
fragment instance* on a hit (`{ pr with Control = nc }`), so a hit emits the identical scene
list. The deterministic, golden-asserted contract is the **hit/miss counts + damage metrics
at the scene-list level**. The **SKPicture record/replay** lives in `SceneRenderer`
(backend), keyed off stable identity, with a **byte-identical raster** contract — it never
alters the golden scene list (FR-008/FR-014), exactly as 114's live `OnFrameMetrics`
reported the same fields the deterministic path asserted.

**Rationale.** This is the spec's explicit FR-005/FR-008-vs-FR-014 resolution. Keeping the
recording in the backend off the deterministic `Perf.runScript` path (which operates over
`Scene` lists, not raster) preserves every prior rung's byte-identical-at-rest goldens while
still delivering the optional real picture optimization.

**Alternatives considered.** Wrapping stable subtrees in a `PictureNode` on the
deterministic path — rejected: it would change the emitted scene structure and break every
prior byte-identical golden (the spec's clarification rules this out).

## (f) Offscreen-effect detection + diagnostic

**Decision.** A control's paint "requires offscreen composition" when its lowered scene
contains a **non-opaque opacity group over a multi-node subtree** (`withOpacity` with
alpha < 1 over >1 node, `SceneRenderer.fs:28-30`), a **clip** (`ClipNode`,
`SceneRenderer.fs:356-367`), or a **drop-shadow/image-filter** (`CreateDropShadow`,
`SceneRenderer.fs:125`). Detection happens at the Controls/lowering level (from the lowered
scene/attrs the step already has), emitting an advisory `ControlDiagnostic` with a new
`ControlDiagnosticCode` case naming the control + effect, surfaced through the existing
`Diagnostics` channel on the step result (`RetainedRender.fs:720`, precedent
`firstFrameCollisions` `:265-292`).

**Rationale.** The detection mirrors `KeyCollision` (`Types.fsi:154`) — a non-blocking
advisory through the established channel. Detecting from the lowered scene keeps it on the
existing diff path, advisory only (never fails a build, never alters output, FR-011).

**Alternatives considered.** Detecting in the backend (`SceneRenderer`) — rejected: the
diagnostic must reach the Controls `Diagnostics` channel consumers already read; the lowered-
scene site is where control identity + effect are both available.

## (g) Interaction with features 113 (memo) and 114 (virtualization)

**Decision.** The picture cache is **distinct from and complementary to** the 113 memo cache
(`MemoCache`, the DataGrid `gridGeom` memoization): the memo cache memoizes a projection
*input*; the picture cache reuses a painted *fragment*. They coexist on `RetainedRender`
unchanged. The damage/cache metrics **aggregate over the virtualized row set** (114): a
materialized row that scrolls out of the realized window simply stops contributing damage; a
cached picture entry for an offscreen row is a normal LRU candidate (it re-misses on
re-materialization). No 114 metric or 113 seam changes.

**Rationale.** FR-015 requires both to keep working; keeping the caches orthogonal (one
keyed by `ControlId`+dependency, the other by `RetainedId`+correctness-key) avoids any
interaction beyond the predictable LRU/virtualization composition the spec calls out.

## (h) Corpus scenarios + golden assertions

**Decision.** Extend the `Perf.runScript` corpus (`specs/109-perf-metrics-baseline/readiness/
perf-corpus/`) with scenarios proving each FR, regenerated with `PERF_CORPUS_REGEN=1`:
- **localized hover** → small `RepaintedNodeCount`/`DirtyRectCount`/`DirtyArea` (US1/FR-002);
- **theme switch** → frame-spanning damage (US1/FR-002);
- **idle frame** → `0/0/0` damage + `0` hit/miss (US1/US5/FR-003);
- **stable-subtree reuse** across two frames → `PictureCacheHitCount` hit, byte-identical
  output, + per-keyed-input miss perturbations (US2/FR-006);
- **cache-cap eviction** (more distinct cacheable subtrees than the cap, e.g. scrolling a
  large grid past many distinct row identities) → `PictureCacheEntryCount <= cap`,
  deterministic eviction, evicted-entry re-miss (US3/FR-009/FR-010).
Existing corpus goldens regenerate to carry the six new fields (idle/steady frames show the
new damage = 0 and hit/miss = 0; `PictureCacheEntryCount` reflects live size).

**Rationale.** This is the 109/113/114 evidence pattern — deterministic, golden-asserted,
regression-proof. The new scenarios map 1:1 to FR-013's enumerated proofs.

**Alternatives considered.** Asserting metrics only via unit tests on the internal step —
kept for the per-keyed-input attribution (`Feature116PictureCacheTests.fs`), but the
public-field regression proof must run through `Perf.runScript` (the consumer-facing path),
per FR-012.

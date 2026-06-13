# Feature Specification: Paint Cache, Damage Rectangles & Optional Skia Picture Boundaries (Observable Repaint Damage + Keyed Picture Cache + Offscreen-Effect Diagnostics)

**Feature Branch**: `116-paint-cache-damage-rects`
**Created**: 2026-06-13
**Status**: Draft
**Input**: User description: "do next part."

**Source report** (local in-repo report, not a remote URL — no `source-spec.md`
snapshot per the specify FR-016 no-op rule):
`docs/reports/2026-06-12-1422-controls-performance-framework-research.md`. This
feature implements the **next part** of that report's staged plan after feature 114
(which delivered Phase 6: viewport virtualization) — namely **Phase 7: Paint Cache,
Damage Rects, and Optional Picture Boundaries**. Per the maintainer's explicit
direction (2026-06-13), this rung delivers **all of Phase 7 in one feature** — the
damage-rectangle observability *and* the keyed picture/display-list cache + cache
boundaries + offscreen-effect diagnostics together — rather than splitting the
damage metrics from the cache. Phase 8 (layout hot-path / measurement caches) and
Phase 9 (backend/host review) remain **out of scope** — see *Unsupported scope*.

## Why this feature (context)

Features 109–114 hardened the retained hot path: honest frame metrics + a perf corpus
(109), retained pointer routing (110), a frame scheduler that skips `host.View` on a
model-unchanged frame (111), a targeted runtime visual-state stamp (112), a control-
internal memoization seam + stability diagnostics (113), and an observable viewport-
virtualization contract (114). The report's remaining performance frontier is **paint**:
the report's own gap list says "there is no backend layer, picture, damage-rect, or
draw-call batching layer", "reusing F# scene lists avoids some CPU paint construction; it
does not guarantee cheap backend presentation", and "there is no phase-complete profiler
record … so performance work can be misprioritized." The report's Phase 7 closes this:
damage rectangles, dirty-area/dirty-node metrics, explicit cache boundaries for stable
subtrees, an optional Skia picture/display-list cache, correctness keys covering every
render-affecting input, and diagnostics for hidden offscreen-layer costs.

The framework is **partway there already** — and that grounds an honest, additive rung:

1. **Repaint already happens but is unobservable as damage.** The retained step already
   decides, per node, whether to reuse the cached `OwnScene`/`SubtreeScene` or repaint
   (`RetainedRender.fs` `build`/`carry`/`paintFresh`); `WorkReductionRecord` counts
   *recomputed* nodes but there is **no spatial damage signal** — no count of repainted
   nodes surfaced per frame, no dirty-rectangle count, no dirty *area*. Nothing proves the
   report's headline acceptance criterion that "a hover state change reports a **small
   dirty region**." The report lists `RepaintedNodeCount`, `DirtyRectCount`, and an
   integer-rounded **dirty area** as exactly the deterministic fields this gap needs.

2. **The cache boundary exists but is unkeyed, uncounted, and not a real picture.** The
   step already reuses a whole cached fragment for an unchanged+unshifted+same-theme
   subtree (`RetainedRender.fs:540-543`, `{ pr with Control = nc }`) — that fragment **is**
   the cache boundary. But (a) its reuse key is only `box = pr.Fragment.Box && not
   themeChanged`, **narrower** than the report's required correctness key (theme, box,
   clip, opacity, transform, font/text, visual-state); (b) the reuse is **invisible** — no
   hit/miss count proves a stable subtree was reused vs repainted; and (c) it is a CPU
   scene-list reuse, **not** a recorded Skia picture — the existing `Scene.Picture` /
   `PictureNode` primitive is a passthrough (`SceneRenderer.fs:393` iterates
   `picture.Scene.Nodes`) and **no control emits one today**. The report wants the reuse
   named as an explicit boundary, keyed by every render-affecting input, counted, and —
   optionally, in the backend — recorded/replayed as a real Skia picture.

3. **Caches have no bound and no observability.** A picture/display-list cache that
   persists across frames can grow without limit. The report is blunt: "memory growth from
   caches is bounded and observable" and "track memory from the start." Today there is no
   cross-frame paint cache to bound; introducing one **requires** a cap + eviction and an
   observable entry count from day one.

4. **Offscreen-layer costs are silent.** The renderer already pays for expensive effects
   that force offscreen composition — non-opaque opacity groups, clips, drop-shadow image
   filters (`SceneRenderer.fs` `ClipNode`, `withOpacity`, `CreateDropShadow`). The report
   requires a **diagnostic** "when a control uses an expensive effect that requires
   offscreen composition", so consumers can see the cost rather than discover it as jank.

This feature turns the existing repaint decision + fragment reuse into an **observable,
keyed, bounded paint-cache contract with optional Skia picture boundaries**:

- (a) public, deterministic **damage metrics** — `RepaintedNodeCount`, `DirtyRectCount`,
  and integer-rounded **`DirtyArea`** — computed from the step's existing repaint
  decisions, golden-asserted via the `Perf.runScript` corpus (the 113/114 threading
  through `WorkReductionRecord` → `FrameMetrics`), so a localized hover reports a small
  dirty region and a whole-frame change reports a large one;
- (b) an **explicit, fully-keyed picture cache boundary**: the existing fragment reuse is
  named as the cache boundary, its correctness key **widened** to every render-affecting
  input (theme, box, clip, opacity, transform, font/text, visual-state), and its outcome
  **counted** as public `PictureCacheHitCount` / `PictureCacheMissCount` — reuse only when
  *every* keyed input is unchanged, so a cache hit is provably correct;
- (c) an **optional Skia picture/display-list recording** in the backend: stable cached
  subtrees are wrapped in the existing `Scene.Picture` boundary so `SceneRenderer` can
  record/replay a real Skia picture for an unchanged boundary, with a **byte-identical
  raster** contract;
- (d) **bounded, observable cache memory**: the cross-frame picture cache is capped with a
  deterministic eviction policy and an observable entry count (`PictureCacheEntryCount`),
  so cache memory cannot grow without limit and the bound is provable;
- (e) **offscreen-effect diagnostics**: a `ControlDiagnostic` flags a control that uses an
  effect requiring offscreen composition, so the cost is visible.

Per the report's staging, this rung deliberately keeps **damage rectangles axis-aligned
and integer-rounded** (deterministic), and **defers** all layout/text-measurement caches
and layout-boundary hints (Phase 8) and the `SkiaViewer` frame-scheduling / readback /
compositor review (Phase 9).

## Clarifications

### Session 2026-06-13

- Q: The report's Phase 7 bundles (a) damage-rectangle/dirty-region metrics with (b) an
  actual Skia picture/display-list cache, yet the report's own priority lists defer the
  picture cache ("Do later") and gate it on "after paint metrics prove benefit" / "do not
  do GPU/layer caching before paint metrics show it is the bottleneck". Should this rung be
  damage-metrics-only (deferring the picture cache to a follow-up) or full Phase 7? → A:
  **Full Phase 7 in one rung — do not split it up.** Deliver the damage observability *and*
  the keyed picture cache + cache boundaries + bounded/observable cache memory + offscreen-
  effect diagnostics together. (The maintainer was shown that the picture cache is **not**
  scheduled in any later report phase — Phase 8 is layout, Phase 9 is backend — so deferring
  it would require an unscheduled follow-up; they chose to land it now.)
- Q: Should the new damage/cache accounting be public golden-asserted `FrameMetrics` fields
  or internal-only counts? → A: **Public `FrameMetrics` fields** (informed default, matching
  109/110/111/113/114, and the report lists `RepaintedNodeCount` / `DirtyRectCount` / dirty
  area among its deterministic golden-friendly fields). This is a **breaking
  `ControlsElmish.fsi` `FrameMetrics` change** (new fields) and incurs corpus-golden churn,
  accepted for the same reason 113/114 made their counts public.
- Q: Wrapping stable subtrees in a `PictureNode` would change the emitted `Scene`
  structure and break the byte-identical-at-rest goldens every prior rung held. How is byte-
  identity preserved? → A: **The deterministic contract lives at the scene-list level and
  stays byte-identical.** The picture cache's deterministic, golden-asserted surface is the
  reuse *decision* (hit/miss counts) + the damage metrics, computed in the retained step
  **without changing the emitted flat `SubtreeScene`** at rest (the step already reuses the
  same fragment instance). The **Skia picture *recording*** is a backend (`SceneRenderer`)
  realization whose contract is **byte-identical raster** (covered by the existing Scene-
  parity / evidence path, not a new golden count) — exactly how 114's live `OnFrameMetrics`
  reported the same fields the deterministic path asserted. Any `PictureNode` boundary the
  backend records is keyed off stable identity and produces identical pixels.
- Q: A cross-frame picture cache can grow unbounded. What is the memory contract? → A:
  **Bounded + observable from day one.** The cross-frame picture cache is **capped** with a
  deterministic eviction policy (e.g. LRU to a fixed entry cap) and exposes a deterministic
  **`PictureCacheEntryCount`** (`<= cap`); raw byte size is a non-deterministic diagnostic
  (excluded from goldens, like `FrameDuration`). Memory cannot grow without limit and the
  entry bound is golden-provable.
- Q: What counts as an "expensive effect requiring offscreen composition" for the
  diagnostic, and is it blocking? → A: **Non-blocking advisory `ControlDiagnostic`.** A
  control whose paint requires offscreen composition — a non-opaque opacity group over a
  multi-node subtree, a clip, or a drop-shadow/image-filter effect — is flagged via the
  existing `Diagnostics` channel (like `KeyCollision`), advisory only; it never fails a
  build or changes rendered output.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A localized visual change reports a small dirty region (Priority: P1)

A framework maintainer moves the pointer so a single control's hover state changes. Today
the retained step repaints only that control's own scene (the reuse path keeps every other
fragment), but **nothing proves it** — there is no per-frame signal of how much was
repainted or over what area. After this feature, the frame records a small number of
repainted nodes, a small dirty-rectangle count, and a small integer dirty area — while a
whole-frame change (e.g. a theme switch) reports damage spanning the frame. Repaint cost
becomes an observable, regression-proof contract.

**Why this priority**: This is the report's Phase 7 headline acceptance criterion ("a
hover state change reports a small dirty region") and the prerequisite the report gates the
picture cache on ("after paint metrics prove benefit"). It is independently valuable — even
with no cache change, the damage metrics let a regression that repaints the whole tree on a
hover fail a golden.

**Independent Test**: Through the deterministic `Perf.runScript` path, drive a single-
control hover change and assert `RepaintedNodeCount`, `DirtyRectCount`, and `DirtyArea` are
small and bounded (proportional to the changed control, not the tree); drive a theme switch
and assert the damage spans the frame; drive an idle frame and assert all three are `0`.

**Acceptance Scenarios**:

1. **Given** a rendered tree where one control's visual state changes (hover), **When** the
   frame is built, **Then** `RepaintedNodeCount` counts only the changed node(s) (and any
   genuinely-shifted ancestors), `DirtyRectCount` is small, and `DirtyArea` covers only the
   changed control's box(es) — not the whole frame.
2. **Given** a theme switch that invalidates all cached paint, **When** the frame is built,
   **Then** the damage metrics report frame-spanning repaint (every node repainted, area ≈
   the frame).
3. **Given** an idle frame with no change, **When** the frame is built, **Then**
   `RepaintedNodeCount = 0`, `DirtyRectCount = 0`, and `DirtyArea = 0`.

---

### User Story 2 - Stable subtrees reuse a fully-keyed picture cache; reuse is provably correct (Priority: P1)

A maintainer needs the existing fragment reuse to be an **explicit, correctly-keyed cache
boundary**. A stable subtree (unchanged across the frame in *every* render-affecting input
— theme, box, clip, opacity, transform, font/text, visual-state) is reused from cache and
counted as a hit; a subtree any of whose keyed inputs changed is repainted and counted as a
miss. The cache key is **complete**: a change to any render-affecting input invalidates the
entry, so a cache hit can never show stale paint.

**Why this priority**: The report requires explicit cache boundaries for stable subtrees
with correctness keys covering "theme, box, clip, opacity, transform, font/text, and
visual-state values", and warns "a stale visual cache is worse than a slow frame." P1
because the picture cache is the rung's core deliverable and is unacceptable without
complete invalidation keys.

**Independent Test**: Render a tree with a stable subtree across two frames and assert
`PictureCacheHitCount` counts the reused subtree and rendered output is byte-identical;
then perturb **each** keyed input in turn (theme, box, clip, opacity, transform, text/font,
visual-state) and assert that input alone produces a `PictureCacheMissCount` (the subtree
repaints) and correct fresh output — proving no keyed input is missing from the key.

**Acceptance Scenarios**:

1. **Given** a subtree unchanged in every render-affecting input across two frames, **When**
   the second frame is built, **Then** it is a `PictureCacheHitCount` hit (reused from
   cache, not repainted) and its rendered scene is byte-identical to the first.
2. **Given** a subtree where exactly one keyed input changed (theme | box | clip | opacity |
   transform | font/text | visual-state), **When** the frame is built, **Then** the subtree
   is a `PictureCacheMissCount` miss (repainted) and the output reflects the change — for
   **every** keyed input independently.
3. **Given** the same scenarios run with the picture cache **disabled** (the always-miss
   oracle, mirroring 113's `MemoEnabled`), **When** each frame is built, **Then** the
   rendered scene is byte-identical to the cache-enabled build (cache-on ≡ cache-off output).

---

### User Story 3 - Picture-cache memory is bounded and observable (Priority: P1)

A maintainer must trust that the cross-frame picture cache cannot grow without limit. The
cache is capped with a deterministic eviction policy, and its live entry count is observable
per frame, so a leak (entries never evicted) or an unbounded cache shows up immediately.

**Why this priority**: The report's Phase 7 acceptance criterion is explicit — "memory
growth from caches is bounded and observable" and "track memory from the start." P1 because
an unbounded cache trades a CPU win for a memory leak; the bound is part of the contract,
not a follow-up.

**Independent Test**: Drive a scenario that would populate more distinct cacheable subtrees
than the cap (e.g. scrolling a large grid past many distinct row identities) and assert
`PictureCacheEntryCount` never exceeds the cap, that eviction is deterministic (same input
sequence → same surviving entries), and that output remains correct after eviction (an
evicted entry recomputes as a miss, not stale paint).

**Acceptance Scenarios**:

1. **Given** more distinct cacheable subtrees than the cache cap, **When** frames are built,
   **Then** `PictureCacheEntryCount <= cap` at all times and eviction follows the documented
   deterministic policy.
2. **Given** an entry was evicted, **When** its subtree is rendered again, **Then** it is a
   miss (recomputed fresh and correct), never a stale hit.
3. **Given** a frame, **When** it is recorded, **Then** `PictureCacheEntryCount` reflects the
   live cache size deterministically (raw byte size is available only as a non-golden
   diagnostic).

---

### User Story 4 - Controls using expensive offscreen effects are flagged (Priority: P2)

A control author uses an effect that forces offscreen composition — a non-opaque opacity
group over a multi-node subtree, a clip, or a drop-shadow/image-filter. The framework
surfaces an advisory diagnostic naming the control and the offscreen-forcing effect, so the
author can see the cost (and the caching consequence) rather than discover it as jank.

**Why this priority**: The report requires "diagnostics when a control uses an expensive
effect that requires offscreen composition." P2 because it hardens and informs the cache
work (offscreen effects interact with picture caching) rather than delivering the cache
itself; it is advisory and changes no rendered output.

**Independent Test**: Render a control whose paint requires offscreen composition (opacity
group / clip / drop-shadow) and assert a corresponding advisory `ControlDiagnostic` is
surfaced through the existing `Diagnostics` channel; render a control with no such effect
and assert none is surfaced; assert in both cases rendered output is unchanged.

**Acceptance Scenarios**:

1. **Given** a control whose paint requires offscreen composition (opacity group over a
   subtree | clip | drop-shadow/image-filter), **When** the frame is built, **Then** an
   advisory offscreen-effect `ControlDiagnostic` naming it is surfaced.
2. **Given** a control with no offscreen-forcing effect, **When** the frame is built,
   **Then** no offscreen-effect diagnostic is surfaced.
3. **Given** either case, **When** the frame is built, **Then** rendered output is byte-
   identical to the pre-feature state (the diagnostic is advisory, never altering paint).

---

### User Story 5 - The paint-cache contract is observable as deterministic metrics (Priority: P2)

A maintainer needs the damage and cache counts in the per-frame metrics so a regression that
defeats the cache (repainting stable subtrees, or blowing the cache cap) or a regression that
widens damage (repainting the whole tree on a localized change) shows up in the goldens
instead of silently costing CPU/memory.

**Why this priority**: The report lists `RepaintedNodeCount`, `DirtyRectCount`, dirty area,
and (by the 113 precedent) cache hit/miss + bounded size as the deterministic fields. P2
because it hardens and proves US1–US3 rather than delivering the mechanism itself.

**Independent Test**: Run corpus scenarios (idle, localized hover, theme switch, stable-
subtree reuse, cache-cap eviction) and assert every new metric is deterministic and golden-
asserted, and that all of them are `0`/empty on a frame that performs no paint work.

**Acceptance Scenarios**:

1. **Given** any corpus frame, **When** it is recorded, **Then** `RepaintedNodeCount`,
   `DirtyRectCount`, `DirtyArea`, `PictureCacheHitCount`, `PictureCacheMissCount`, and
   `PictureCacheEntryCount` are deterministic and golden-asserted.
2. **Given** an idle frame, **When** it is recorded, **Then** the damage metrics and cache
   hit/miss are `0` (a steady cache may retain entries, so `PictureCacheEntryCount` reflects
   live size, not necessarily 0).
3. **Given** a regression that repaints a stable subtree, **When** the corpus runs, **Then**
   the hit/miss counts and/or damage metrics change, failing the golden.

---

## Requirements *(mandatory)*

### Functional Requirements

**Damage tracking (Phase 7 core — metrics)**

- **FR-001**: The framework MUST compute, per frame, a **damage set** from the retained
  step's existing repaint decisions (the nodes whose `OwnScene` was repainted via
  `paintFresh`/`carry`/`buildFresh`, plus genuinely-shifted nodes), and surface it as:
  `RepaintedNodeCount` (count of repainted nodes), `DirtyRectCount` (count of distinct
  axis-aligned damage rectangles), and `DirtyArea` (integer-rounded total damaged area in
  px²). Damage rectangles derive from repainted nodes' evaluated boxes.
- **FR-002**: A **localized** visual change (e.g. a single control's hover/visual-state
  change) MUST report damage proportional to the changed control(s) — a small
  `RepaintedNodeCount`/`DirtyRectCount` and a `DirtyArea` covering only the changed box(es)
  — **not** frame-spanning damage. A change that genuinely invalidates all paint (theme
  switch) MUST report frame-spanning damage.
- **FR-003**: An **idle** frame (no change) MUST report `RepaintedNodeCount = 0`,
  `DirtyRectCount = 0`, and `DirtyArea = 0`.
- **FR-004**: `DirtyArea` and `DirtyRectCount` MUST be **deterministic** (integer-rounded,
  computed from integer control geometry), golden-assertable via `Perf.runScript`.

**Picture cache boundary + keys (Phase 7 core — cache)**

- **FR-005**: The framework MUST treat the existing reusable retained fragment as an
  **explicit picture-cache boundary** for a stable subtree, and MUST count its outcome per
  frame as `PictureCacheHitCount` (a stable subtree reused from cache without repaint) and
  `PictureCacheMissCount` (a subtree repainted/recorded fresh).
- **FR-006**: The cache **correctness key** MUST cover **every render-affecting input** —
  theme, box, clip, opacity, transform, font/text, and visual-state — widening today's
  `box + theme`-only reuse condition. A subtree MUST be reused (a hit) **only** when *all*
  keyed inputs are unchanged; a change to **any** keyed input MUST invalidate the entry (a
  miss, repainting it). No keyed input may be omitted (a stale hit is a defect, FR-conflict
  below).
- **FR-007**: A `PictureCacheHitCount` hit MUST produce **byte-identical** rendered output
  to repainting the subtree fresh. The cache MUST have an **always-miss oracle** mode
  (mirroring 113's `MemoEnabled`) under which every subtree repaints; the cache-on and
  cache-off rendered scenes MUST be byte-identical (cache correctness, FR-conflict below).
- **FR-008**: The **optional Skia picture recording** is the backend realization: stable
  cached boundaries MAY be wrapped in the existing `Scene.Picture` / `PictureNode` so
  `SceneRenderer` can record/replay a real Skia picture for an unchanged boundary. Its
  contract is **byte-identical raster**; it MUST NOT change the deterministic scene-list
  output asserted by the goldens (the deterministic contract is the hit/miss counts + damage
  metrics at the scene-list level; the SKPicture record/replay is a live backend
  optimization, like 114's live `OnFrameMetrics`).

**Bounded, observable cache memory (Phase 7 core — memory)**

- **FR-009**: Any **cross-frame** picture cache MUST be **bounded** by a fixed cap with a
  **deterministic eviction policy** (e.g. LRU to a fixed entry cap). It MUST expose a
  deterministic `PictureCacheEntryCount` (`<= cap`) per frame; raw byte size MAY be reported
  only as a **non-golden** diagnostic (excluded from goldens, like `FrameDuration`).
- **FR-010**: An **evicted** entry MUST recompute as a **miss** (fresh, correct paint) when
  next needed — never a stale hit. Eviction MUST be deterministic: the same input sequence
  yields the same surviving entries.

**Offscreen-effect diagnostics (Phase 7 — observability)**

- **FR-011**: The framework MUST surface an **advisory** `ControlDiagnostic` (through the
  existing `Diagnostics` channel, like `KeyCollision`) when a control's paint requires
  **offscreen composition** — a non-opaque opacity group over a multi-node subtree, a clip,
  or a drop-shadow/image-filter effect. The diagnostic MUST name the control/effect and MUST
  be advisory only: it never fails a build and never alters rendered output.

**Observability surface (Phase 7 — metrics publicity)**

- **FR-012**: `RepaintedNodeCount`, `DirtyRectCount`, `DirtyArea`, `PictureCacheHitCount`,
  `PictureCacheMissCount`, and `PictureCacheEntryCount` MUST be **public `FrameMetrics`
  fields** (clarified 2026-06-13), threaded through the existing `WorkReductionRecord` →
  `FrameMetrics` path (the 113/114 pattern), reproducible and **golden-asserted** via the
  `Perf.runScript` corpus. The metrics MUST make a regression that repaints a stable subtree,
  widens localized damage to the whole frame, or blows the cache cap, visible as a golden
  change.

**Evidence (Phase 7 — proof)**

- **FR-013**: The `Perf.runScript` corpus MUST include scenarios proving: a localized hover
  reports a small dirty region while a theme switch reports frame-spanning damage
  (FR-002/US1); a stable subtree is a cache hit with byte-identical output, and each keyed
  input independently forces a miss (FR-006/US2); the cache cap bounds `PictureCacheEntryCount`
  under eviction pressure (FR-009/US3); and the offscreen-effect diagnostic fires for an
  offscreen-forcing control and not otherwise (FR-011/US4).

**Behaviour preservation (cross-cutting)**

- **FR-014**: This feature is **additive observability + a correctly-keyed cache + bounded
  memory + advisory diagnostics only**. At-rest rendered output (the deterministic scene-list
  goldens), control geometry, focus/keyboard/pointer routing, and every existing dispatch
  outcome MUST remain **byte-identical** to the pre-feature state. The only intended
  observable changes are (a) the new `FrameMetrics` fields, (b) the advisory offscreen-effect
  diagnostic, and (c) the backend SKPicture record/replay whose contract is byte-identical
  raster.
- **FR-015**: Features 114 (virtualization counts — the damage/cache metrics MUST aggregate
  correctly over the virtualized row set; a cached row that scrolls out and an evicted picture
  entry interact predictably), 113 (memo seam — the picture cache is a distinct, complementary
  cache; the DataGrid `gridGeom` memoization continues to work), 112/111/110/109, and the
  retained render pipeline are otherwise unchanged.
- **FR-016**: This rung keeps damage rectangles **axis-aligned and integer-rounded**
  (deterministic). It **defers** all layout/text **measurement caches** and layout-boundary
  hints to **Phase 8**, and the `SkiaViewer` frame-scheduling / readback-separation /
  compositor review to **Phase 9** (clarified 2026-06-13).

> Interacting / conflicting requirements:
> - **FR-005/FR-008 (introduce a picture cache / record pictures) vs FR-014 (byte-identical
>   at rest)** — resolution: the **deterministic** contract is the hit/miss counts + damage
>   metrics computed in the retained step **without changing the emitted flat `SubtreeScene`**
>   (the step already reuses the same fragment instance, so a hit emits the identical scene
>   list). The **SKPicture recording** lives in `SceneRenderer` (backend), keyed off stable
>   identity, with a **byte-identical raster** contract — it does not alter the golden scene
>   list. Cache-on ≡ cache-off output (FR-007).
> - **FR-006 (widen the cache key to every render-affecting input) vs FR-005/SC (more hits)**
>   — resolution: correctness wins. A subtree is reused **only** when *all* keyed inputs are
>   unchanged; a complete key may yield fewer hits than an unsound narrow key, but "a stale
>   visual cache is worse than a slow frame." The always-miss oracle (FR-007) proves the key
>   never hides a change.
> - **FR-009 (bound the cache) vs FR-005 (cache to win)** — resolution: the cap is a hard
>   bound; under pressure, LRU eviction drops the least-recently-used entries (which simply
>   re-miss when next needed, FR-010). A bounded cache that occasionally re-misses is correct;
>   an unbounded cache that leaks is not.
> - **FR-002 (small dirty region for localized change) vs the existing shifted-node repaint**
>   — resolution: damage counts the **honest** repaint set (own-change + theme repaint +
>   genuinely-shifted nodes, mirroring `WorkReductionRecord`'s `ShiftedNodeCount`); a localized
>   change that shifts siblings reports those shifted boxes as damage too (still bounded, not
>   frame-spanning). Damage never under-reports actual repaint.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Touches `FS.Skia.UI.Controls` — the internal `RetainedRender` step
  gains damage-set computation, the widened picture-cache key + hit/miss counting, the
  bounded cross-frame picture cache (cap + eviction + entry count), and the offscreen-effect
  diagnostic; `WorkReductionRecord` gains the carrier fields. `FS.Skia.UI.Controls.Elmish` —
  the `FrameMetrics` record gains the six public fields, threaded from the retained step and
  surfaced through `Perf.runScript` and the live `OnFrameMetrics` sink. `FS.Skia.UI.SkiaViewer`
  — `SceneRenderer` optionally gains real Skia picture record/replay for an unchanged
  `PictureNode` boundary (byte-identical raster). No package identity changes; package
  **contents** change and versions bump on merge. DataGrid/Controls are the active authoring
  path (no legacy Charts migration).
- **Public contract impact**: **Breaking** `ControlsElmish.fsi` `FrameMetrics` change — six
  new public fields (`RepaintedNodeCount`, `DirtyRectCount`, `DirtyArea`,
  `PictureCacheHitCount`, `PictureCacheMissCount`, `PictureCacheEntryCount`), so the top-level
  surface baseline changes (precedent: 109/110/111/113/114 each added `FrameMetrics` fields).
  A new advisory `ControlDiagnostic` case (offscreen-effect) is additive public surface on the
  Controls diagnostics DU. The `RetainedRender`/`WorkReductionRecord` additions are
  assembly-internal (no public delta there). Possible additive `Scene`/`SceneRenderer`
  internals for picture record/replay (the public `Scene.Picture`/`PictureNode` already
  exists). `Route` is expected to escalate to the **controls-public-surface** tier because the
  Controls.Elmish `FrameMetrics` and the Controls diagnostics `.fsi` surfaces change; run
  `Route` first and obey its printed list.
- **State workflow impact**: None to MVU semantics — `Update`, effects, subscriptions,
  commands, and interpreter behaviour are unchanged. The picture cache is a render-side cache
  (interpreter-edge mutation confined to the retained step, constitution III, exactly as the
  existing id/work counters and the 113 memo cache are); `view`/`update` stay pure. Dispatch
  outcomes are byte-identical (FR-014).
- **Layout/rendering impact**: At rest, the deterministic scene-list output, geometry, and
  the retained step's emitted scene are byte-identical (FR-014); the picture cache changes
  *which work is done*, not *what is emitted* (cache-on ≡ cache-off, FR-007). The backend
  SKPicture record/replay is a live-only raster optimization with a byte-identical-pixels
  contract. Damage rectangles are axis-aligned, integer-rounded. No Vulkan change; no
  unsupported-environment diagnostic change. The new offscreen-effect diagnostic is advisory
  and alters no output.
- **Evidence obligations**: small-dirty-region vs frame-spanning damage evidence (localized
  hover vs theme switch, FR-002/US1); idle-frame zero-damage (FR-003); picture-cache hit with
  byte-identical output + per-keyed-input miss (theme/box/clip/opacity/transform/font-text/
  visual-state, FR-006/US2); cache-on ≡ cache-off (always-miss oracle, FR-007); bounded
  `PictureCacheEntryCount` under eviction pressure + deterministic eviction + evicted-entry
  re-miss (FR-009/FR-010/US3); offscreen-effect diagnostic fires/does-not-fire (FR-011/US4);
  the six new metrics deterministic + golden-asserted (FR-012/US5); the regenerated
  `Perf.runScript` corpus goldens carrying the new metric fields; at-rest byte-identity (the
  standing Scene-parity golden suite under `Dev`); skill-loading evidence; the window-
  visibility not-applicable set; `readiness/evidence-audit.md` with a verdict token; the
  generated-validation package-resolution tokens. The escalated `maintainer-verify` readiness
  set applies because of the Controls.Elmish/Controls `.fsi` change.
- **Unsupported scope**: This feature is **Phase 7 (full)** only. Explicitly OUT: **layout
  hot-path / text-measurement caches & layout-boundary hints / structural flattening**
  (Phase 8 — deferred); **`SkiaViewer` frame-scheduling, readback separation, scene-
  submission/layer-skipping, render-thread / compositor split** (Phase 9 — deferred, beyond
  the byte-identical SKPicture record/replay this rung adds); **non-axis-aligned or sub-pixel
  damage rectangles** (axis-aligned integer only this rung); **draw-call batching** (Qt-style,
  not this rung); **damage-driven partial-present** (the backend presents the whole frame;
  this rung adds the damage *signal*, not damage-scoped presentation). No renderer rewrite, no
  Avalonia/WPF redesign, no platform/release/distribution scope.
- **Build-target impact**: Escalation to the controls-public-surface set is expected because
  the Controls.Elmish `FrameMetrics` and the Controls diagnostics `.fsi` surfaces change; run
  `Route` first and obey its printed minimal list (`Dev`, the package/per-package surface
  diffs, `FsiTranscripts`, the controls catalog/doc/interaction/rendering checks,
  `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`).
  `RefreshSurfaceBaselines` must regenerate the top-level + per-package baselines after the
  `FrameMetrics` / diagnostics additions, and the `Perf.runScript` corpus goldens must be
  regenerated (`PERF_CORPUS_REGEN=1`) to carry the new metric fields. No new gate.

## Success Criteria *(mandatory)*

- **SC-001**: A single-control hover/visual-state change reports a **small** dirty region —
  `RepaintedNodeCount` proportional to the changed control(s), small `DirtyRectCount`, and a
  `DirtyArea` covering only the changed box(es) — while a theme switch reports frame-spanning
  damage; an idle frame reports all three as `0`.
- **SC-002**: A stable subtree (unchanged in **every** render-affecting input) is a
  `PictureCacheHitCount` hit producing **byte-identical** output; changing **any** one keyed
  input (theme, box, clip, opacity, transform, font/text, visual-state) independently forces a
  `PictureCacheMissCount` miss with correct fresh output — proving the key omits no input.
- **SC-003**: Rendered output with the picture cache enabled is **byte-identical** to the
  always-miss oracle (cache-on ≡ cache-off).
- **SC-004**: The cross-frame picture cache is bounded: `PictureCacheEntryCount <= cap` at all
  times under eviction pressure, eviction is deterministic, and an evicted entry re-misses
  (correct fresh paint), never a stale hit.
- **SC-005**: An advisory offscreen-effect `ControlDiagnostic` fires for a control whose paint
  requires offscreen composition (opacity group / clip / drop-shadow) and does not fire
  otherwise, altering no rendered output.
- **SC-006**: `RepaintedNodeCount`, `DirtyRectCount`, `DirtyArea`, `PictureCacheHitCount`,
  `PictureCacheMissCount`, and `PictureCacheEntryCount` are deterministic, golden-asserted
  `FrameMetrics` fields; a regression that repaints a stable subtree, widens localized damage
  to the whole frame, or blows the cache cap fails a golden.
- **SC-007**: At-rest rendered output (deterministic scene-list goldens), control geometry,
  pointer/keyboard/focus routing semantics, and all existing dispatch outcomes are byte-
  identical to the pre-feature state; features 113 (memo) and 114 (virtualization) continue to
  work, and the metrics aggregate correctly over the virtualized row set.

## Key Entities

- **Damage set / dirty rectangles** (new, internal `Controls`): the per-frame set of
  repainted nodes and their axis-aligned, integer-rounded boxes, computed from the retained
  step's existing repaint decisions. Surfaced as `RepaintedNodeCount` / `DirtyRectCount` /
  `DirtyArea`. Small for a localized change, frame-spanning for a theme switch, empty for idle.
- **Picture-cache boundary** (the existing retained fragment, now named + keyed): the reusable
  unit of cached paint for a stable subtree. Reused (a hit) only when its **complete**
  correctness key — theme, box, clip, opacity, transform, font/text, visual-state — is
  unchanged; otherwise repainted (a miss). Distinct from and complementary to the 113 memo
  cache.
- **Picture-cache correctness key** (new): the tuple of every render-affecting input that must
  be unchanged for a cache hit. Widens today's `box + theme` reuse condition so a hit can never
  show stale paint.
- **`Scene.Picture` / `PictureNode`** (existing, today a passthrough): the scene-list primitive
  the backend uses to record/replay a real Skia picture for an unchanged stable boundary
  (byte-identical raster). The optional backend realization of the cache boundary.
- **Bounded picture cache** (new, cross-frame): the capped, deterministically-evicted store of
  cached pictures. Observable via `PictureCacheEntryCount` (`<= cap`, deterministic); raw bytes
  are a non-golden diagnostic.
- **`RepaintedNodeCount` / `DirtyRectCount` / `DirtyArea` / `PictureCacheHitCount` /
  `PictureCacheMissCount` / `PictureCacheEntryCount`**: the six public, deterministic
  `FrameMetrics` fields this feature adds (the 113/114 publicity precedent), golden-asserted via
  `Perf.runScript`.
- **Offscreen-effect diagnostic** (new advisory `ControlDiagnostic`): flags a control whose
  paint requires offscreen composition (opacity group / clip / drop-shadow), advisory only.

## Assumptions

- The damage set is derived by extending the retained step's existing repaint accounting
  (`paintFresh`/`carry`/`buildFresh` and the `ShiftedNodeCount`/`ChangedSubtreeBound`
  bookkeeping in `RetainedRender.fs`); the plan picks the exact carrier fields on
  `WorkReductionRecord` and how `DirtyRectCount` coalesces overlapping/adjacent repainted
  boxes (default: one rect per repainted node's box, count = distinct boxes, area = summed
  integer area; coalescing strategy is a plan decision but MUST stay deterministic).
- The picture-cache hit/miss decision extends the existing fragment-reuse condition
  (`box = pr.Fragment.Box && not themeChanged`) by widening it to the full correctness key;
  the plan decides how each keyed input (clip, opacity, transform, font/text, visual-state) is
  detected for a node (e.g. via the lowered `Control`/attrs already diffed, or an added key
  hash on the fragment).
- The six new metrics thread through the same `WorkReductionRecord` → `FrameMetrics`
  mechanism features 113/114 used; the plan picks the exact carrier field names.
- The always-miss oracle mirrors 113's `MemoEnabled` (a per-`RetainedRender` flag forcing
  every subtree to repaint), used to prove cache-on ≡ cache-off output.
- The bounded picture cache is cross-frame and LRU-capped by **entry count** (a fixed cap the
  plan sets); raw byte accounting is a non-deterministic diagnostic excluded from goldens.
  Today there is no cross-frame paint cache, so this cache is new and bounded from creation.
- The backend SKPicture record/replay (`SceneRenderer`) is the optional realization and is
  **not** on the deterministic `Perf.runScript` path (which operates over `Scene` lists, not
  raster); its contract is byte-identical raster, covered by the existing Scene-parity /
  evidence path, exactly as 114's live `OnFrameMetrics` reported the same fields the
  deterministic path asserted.
- "Expensive effect requiring offscreen composition" means a non-opaque opacity group over a
  multi-node subtree, a clip (`ClipNode`), or a drop-shadow/image-filter — detectable from the
  lowered scene/attrs; the plan picks the exact detection site and diagnostic payload.
- Damage rectangles are axis-aligned and integer-rounded; non-axis-aligned/sub-pixel damage,
  draw-call batching, and damage-scoped partial presentation are out of scope.
- Layout/text-measurement caches and layout-boundary hints are Phase 8; `SkiaViewer`
  scheduling/readback/compositor work is Phase 9 (deferred).

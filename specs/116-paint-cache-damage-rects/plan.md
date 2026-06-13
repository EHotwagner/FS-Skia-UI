# Implementation Plan: Paint Cache, Damage Rectangles & Optional Skia Picture Boundaries

**Branch**: `116-paint-cache-damage-rects` | **Date**: 2026-06-13 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/116-paint-cache-damage-rects/spec.md`

## Summary

Features 109–114 hardened the retained hot path (honest frame metrics + a perf corpus,
retained pointer routing, a view-skip frame scheduler, a targeted runtime visual-state
stamp, a control-internal memoization seam + stability diagnostics, and an observable
viewport-virtualization contract). The performance report's remaining frontier is
**paint**: the report's gap list says there is "no backend layer, picture, damage-rect, or
draw-call batching layer," that reusing F# scene lists "does not guarantee cheap backend
presentation," and that hidden offscreen-layer costs are silent. This rung delivers the
report's **Phase 7 in full** (per the maintainer's 2026-06-13 direction — *not* split):
damage observability **and** the keyed picture cache + bounded cache memory + offscreen-
effect diagnostics, together. Phase 8 (layout/measurement caches) and Phase 9 (`SkiaViewer`
scheduling/readback/compositor) stay **out of scope**.

**Technical approach (Phase 7 of the performance report):**

1. **Damage set + dirty-rectangle metrics** (`FS.Skia.UI.Controls`, `RetainedRender`). The
   retained `step` already decides, per node, whether to reuse the cached fragment or
   repaint (`build`/`carry`/`paintFresh`/`buildFresh`, `RetainedRender.fs:494-607`). This
   feature accumulates those decisions into a **damage set** — the nodes whose `OwnScene`
   was repainted plus genuinely-shifted nodes (mirroring the existing
   `RecomputedNodeCount`/`ShiftedNodeCount` bookkeeping) — and surfaces it as three new
   carrier fields on `WorkReductionRecord`: `RepaintedNodeCount`, `DirtyRectCount` (distinct
   axis-aligned damage rectangles, derived from repainted nodes' evaluated `Fragment.Box`es),
   and `DirtyArea` (integer-rounded total damaged px²). A localized hover reports a small
   region; a theme switch reports frame-spanning damage; an idle frame reports `0/0/0`
   (FR-001/FR-002/FR-003/FR-004).

2. **An explicit, fully-keyed picture-cache boundary** (`FS.Skia.UI.Controls`,
   `RetainedRender`). The existing reuse condition `box = pr.Fragment.Box && not
   themeChanged` (`RetainedRender.fs:540`) **is** the cache boundary, but its key is too
   narrow. This feature names it the picture-cache boundary and **widens the correctness
   key to every render-affecting input** — theme, box, clip, opacity, transform, font/text,
   visual-state — so a hit can never show stale paint. The outcome is **counted** as
   `PictureCacheHitCount` (a stable subtree reused without repaint) and `PictureCacheMissCount`
   (a subtree repainted fresh), threaded on `WorkReductionRecord` (the 113
   `MemoHits`/`MemoMisses` pattern). An **always-miss oracle** mode (a per-`RetainedRender`
   flag mirroring 113's `MemoEnabled`) forces every subtree to repaint, proving cache-on ≡
   cache-off output (FR-005/FR-006/FR-007).

3. **A bounded, observable cross-frame picture cache** (`FS.Skia.UI.Controls`). Introducing
   a cache that persists keyed fragments across frames **requires** a bound from day one: a
   fixed entry cap with a **deterministic LRU eviction** policy and an observable
   `PictureCacheEntryCount` (`<= cap`). An evicted entry recomputes as a miss when next
   needed — never a stale hit. Raw byte size is a non-golden diagnostic (excluded from
   goldens, like `FrameDuration`) (FR-009/FR-010).

4. **Six public `FrameMetrics` fields** (breaking `ControlsElmish.fsi` change —
   `RepaintedNodeCount`, `DirtyRectCount`, `DirtyArea`, `PictureCacheHitCount`,
   `PictureCacheMissCount`, `PictureCacheEntryCount`). Threaded from the retained step /
   `WorkReductionRecord` (the 113/114 pattern), surfaced on the deterministic
   `Perf.runScript` path (golden-asserted) and through the live `OnFrameMetrics` sink. A
   regression that repaints a stable subtree, widens localized damage to the whole frame, or
   blows the cache cap becomes a golden change (FR-012/US5).

5. **An advisory offscreen-effect diagnostic** (`FS.Skia.UI.Controls`, `Types`). A new
   `ControlDiagnosticCode` case flags a control whose paint requires offscreen composition —
   a non-opaque opacity group over a multi-node subtree, a clip, or a drop-shadow/image-
   filter — surfaced through the existing `Diagnostics` channel like `KeyCollision`,
   advisory only: it never fails a build and never alters rendered output (FR-011/US4).

6. **Optional backend Skia picture record/replay** (`FS.Skia.UI.SkiaViewer`,
   `SceneRenderer`). Stable cached boundaries MAY be wrapped in the existing
   `Scene.Picture`/`PictureNode` (today a passthrough at `SceneRenderer.fs:393`) so the
   backend records/replays a real `SKPicture` for an unchanged boundary. Its contract is
   **byte-identical raster**; it does **not** change the deterministic flat `SubtreeScene`
   the goldens assert (the deterministic contract is the hit/miss counts + damage metrics at
   the scene-list level — exactly how 114's live `OnFrameMetrics` reported the same fields
   the deterministic path asserted) (FR-008).

This is **additive observability + a correctly-keyed cache + bounded memory + advisory
diagnostics only** (FR-014). At-rest rendered output (the deterministic scene-list
goldens), control geometry, focus/keyboard/pointer routing, and every dispatch outcome stay
**byte-identical**; cache-on ≡ cache-off (FR-007). The only intended observable deltas are
(a) the six new `FrameMetrics` fields, (b) the advisory offscreen-effect diagnostic, and
(c) the backend SKPicture record/replay whose contract is byte-identical raster. Features
113 (memo — a distinct complementary cache) and 114 (virtualization — the metrics aggregate
correctly over the virtualized row set) continue to work (FR-015). Damage rectangles stay
**axis-aligned and integer-rounded**; layout/text-measurement caches (Phase 8) and the
`SkiaViewer` scheduling/readback/compositor review (Phase 9) are **deferred** (FR-016).

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: No new dependencies. Edits `FS.Skia.UI.Controls`
(`RetainedRender` — the damage-set accumulation, the widened picture-cache key + hit/miss
counting, the bounded cross-frame picture cache with cap + LRU eviction + entry count, the
always-miss oracle flag, and the offscreen-effect diagnostic; `WorkReductionRecord` gains
the carrier fields; `Types` gains the diagnostic-code case), `FS.Skia.UI.Controls.Elmish`
(the `FrameMetrics` record + threading through the retained step / `Perf.runScript` /
`OnFrameMetrics`), and optionally `FS.Skia.UI.SkiaViewer` (`SceneRenderer` SKPicture
record/replay for an unchanged `PictureNode` boundary, byte-identical raster). Consumes
existing `RenderFragment`, `WorkReductionRecord`, `RetainedRender.step`, `MemoEnabled`
precedent, `ControlDiagnostic`/`ControlDiagnosticCode`, `Scene.Picture`/`PictureNode`.
**Testing**: Expecto + FsCheck. Damage / picture-cache key / always-miss-oracle / bounded-
eviction / offscreen-diagnostic tests in `tests/Controls.Tests` (reaching internal seams
via `InternalsVisibleTo "Controls.Tests"`); the six metric corpus goldens in
`tests/Elmish.Tests` over `ControlsElmish.Perf.runScript`; the standing Scene-parity golden
suite under `Dev` for at-rest byte-identity (FR-014) and for the SKPicture byte-identical-
raster path; FAKE targets.
**Target Platform**: Windows and Linux (no platform-specific code; no Vulkan change; the
optional SKPicture record/replay is a backend raster optimization with a byte-identical-
pixels contract).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification — Tier 1 (contracted change).** Six new **public** `FrameMetrics`
fields (`ControlsElmish.fsi`) and a new public `ControlDiagnosticCode` case (`Types.fsi`),
plus internal threading through `WorkReductionRecord`/`RetainedRender`. The top-level
surface baseline and per-package baselines move; the full artifact chain applies (`.fsi`
updates, baseline regeneration, test evidence, XML-doc). `Route` escalates to the
**controls-public-surface** tier.

**Principle compliance.**
- *I (Spec→FSI→Tests→Impl)*: the six `FrameMetrics` fields, the offscreen-effect diagnostic
  case, and the internal `WorkReductionRecord` carriers / always-miss flag are drafted in
  `.fsi` signature form first and exercised from FSI/tests; the small-vs-frame-spanning
  damage assertion, the per-keyed-input cache-miss proof, the cache-on ≡ cache-off oracle,
  the bounded-eviction proof, and the diagnostic fire/no-fire tests are the failing-first
  proofs.
- *II (Visibility in `.fsi`)*: the metric fields and the diagnostic case are public,
  declared in their `.fsi`; the damage-set accumulation, the picture-cache key + hit/miss
  counting, the bounded cache, and the always-miss flag are internal (declared in the owning
  `Controls` `RetainedRender.fsi`, reached via `InternalsVisibleTo`). No access modifiers in
  `.fs`.
- *III (Idiomatic simplicity)*: the damage set is a plain accumulation over the existing
  repaint decisions; `DirtyRectCount`/`DirtyArea` are deterministic integer computations
  over repainted boxes; the picture-cache key is a value tuple/record of render-affecting
  inputs compared structurally; the LRU cache is a bounded `Map` + recency list (a `mutable`
  accumulator disclosed at the use site, constitution III, exactly as the existing id/work
  counters and the 113 memo cache are interpreter-edge mutation confined to the step). No
  SRTP/reflection/type-providers; the offscreen-effect detection is a simple match over the
  lowered scene/attrs.
- *IV (Elmish/MVU boundary)*: unchanged — `Update`, effects, subscriptions, commands, and
  interpreter behaviour are untouched. The picture cache is a render-side cache (interpreter-
  edge mutation confined to the retained step, constitution III); `view`/`update` stay pure;
  dispatch outcomes are byte-identical (FR-014).
- *V (Synthetic disclosure)*: none expected — damage metrics, hit/miss, and bounded
  eviction are proven over real corpus scenarios on the real `Perf.runScript` path; byte-
  identity uses the real Scene-parity suite; the SKPicture record/replay is the real
  backend on the real raster path. Any unavoidable stub returns to task review for `[S]`
  disclosure.
- *VI (Test evidence)*: small-vs-frame-spanning damage; idle = 0/0/0; per-keyed-input
  independent miss (theme/box/clip/opacity/transform/font-text/visual-state); cache hit
  byte-identical output; cache-on ≡ cache-off (always-miss oracle); `PictureCacheEntryCount
  <= cap` under eviction pressure + deterministic eviction + evicted-entry re-miss;
  diagnostic fires/does-not-fire; the six metrics deterministic + golden-asserted — all fail
  before / pass after; no assertion weakening.
- *VII (Observability)*: the six metrics make a regression that repaints a stable subtree,
  widens localized damage to the whole frame, or blows the cache cap visible as a golden
  change; the offscreen-effect diagnostic surfaces a hidden compositor cost as actionable
  advisory context. No silent failure.

### Repository Governance Decisions

- **Template ownership**: N/A — no `template/**`, sample, Spec Kit asset, package-policy, or
  command-surface change; the metric fields and the diagnostic case do not alter
  `.template.config/template.json`. (The merge-time template package-pin bump is the
  standard post-merge step, not a content change in this feature.)
- **Dependency impact**: N/A — no new package; `Directory.Packages.props`,
  `docs/dependencies.md`, generated template inclusion, and `DependencyReport` coverage are
  unchanged (the SKPicture path uses the already-pinned SkiaSharp).
- **Command-surface impact**: No new gate. Escalated **controls-public-surface** set because
  the `Controls.Elmish` `FrameMetrics` and the `Controls` `Types` diagnostics `.fsi`
  surfaces change; run `Route` first and obey its printed minimal list.
  `RefreshSurfaceBaselines` regenerates the top-level + per-package baselines after the
  `FrameMetrics` / `ControlDiagnosticCode` additions; the `Perf.runScript` corpus goldens
  are regenerated (`PERF_CORPUS_REGEN=1`) to carry the six new metric fields and the new
  scenarios. FAKE-backed commands share `.fake` state and are not safe to run concurrently;
  run them sequentially in the deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: N/A to behaviour — generated default/minimal contents,
  selected Controls guidance, local skills, and generated `Dev` behaviour are unchanged.
  Generated projects gain the six new public `FrameMetrics` fields and the additive
  diagnostic case transitively (additive; `OnFrameMetrics` default stays `ignore`, the
  cache is byte-identical at rest, the diagnostic is advisory).
- **Evidence paths**: damage (small-region vs frame-spanning vs idle-zero) + picture-cache
  key (hit byte-identity + per-keyed-input miss) + always-miss-oracle + bounded-eviction +
  offscreen-diagnostic tests under `tests/Controls.Tests/Feature116*.fs`; the six-metric
  corpus goldens under `specs/109-perf-metrics-baseline/readiness/perf-corpus/*.golden.txt`
  (regenerated) + asserted in `tests/Elmish.Tests/Feature116*.fs`; the SKPicture byte-
  identical-raster + at-rest byte-identity via the standing Scene-parity suite under `Dev`;
  skill-loading evidence in `readiness/skill-loading-evidence.md`; the window-visibility
  not-applicable set; `readiness/evidence-audit.md` (verdict token); generated-validation
  package-resolution tokens; surface/per-package baselines under
  `readiness/surface-baselines/` + `readiness/per-package-surface/`.
- **`.fsi` / contract impact**: **Breaking** `ControlsElmish.fsi` `FrameMetrics` change —
  six new public fields (`RepaintedNodeCount`, `DirtyRectCount`, `DirtyArea`,
  `PictureCacheHitCount`, `PictureCacheMissCount`, `PictureCacheEntryCount`) with XML-doc
  (doc-preservation gate). Additive `Controls` `Types.fsi`: a new advisory
  `ControlDiagnosticCode` case for the offscreen-effect diagnostic (with XML-doc). The
  damage-set carriers + the picture-cache key/hit-miss + the bounded cache + the always-miss
  flag are internal on `RetainedRender.fsi`/`WorkReductionRecord` (no public delta there).
  Possible additive `Scene`/`SceneRenderer` internals for SKPicture record/replay (the
  public `Scene.Picture`/`PictureNode` already exists — no public delta). The top-level
  surface baseline changes (the `FrameMetrics` fields + the diagnostic case); per-package
  Controls + Controls.Elmish baselines regenerate. **Phase 0 decides** the exact shape of
  the picture-cache key carrier and how each keyed input is detected per node.
- **MVU/effect boundary**: Unchanged (preserved, not modified). `Model`/`Msg`/`Effect`/
  `init`/`update`/interpreter are untouched. The picture cache is a render-side cache
  (interpreter-edge mutation confined to the retained step, constitution III, exactly as
  the existing id/work counters and the 113 memo cache); `view`/`update` stay pure;
  dispatch outcomes are byte-identical (FR-014).
- **Synthetic evidence**: None planned. Damage / hit-miss / bounded eviction = real counts
  over the real `Perf.runScript` corpus; cache-on ≡ cache-off = the real always-miss oracle;
  byte-identity = the real Scene-parity suite; SKPicture record/replay = the real backend.
  Any unavoidable stub returns to task review for `[S]` disclosure under Principle V.
- **Test evidence**: failing-first — localized hover reports small `RepaintedNodeCount`/
  `DirtyRectCount`/`DirtyArea` while a theme switch reports frame-spanning damage; idle =
  `0/0/0`; a stable subtree is a `PictureCacheHitCount` hit with byte-identical output;
  changing each keyed input independently (theme/box/clip/opacity/transform/font-text/
  visual-state) forces a `PictureCacheMissCount` miss with correct fresh output; cache-on ≡
  cache-off (always-miss oracle); `PictureCacheEntryCount <= cap` under eviction pressure +
  deterministic eviction + evicted-entry re-miss; the offscreen-effect diagnostic fires for
  an offscreen-forcing control and not otherwise; all six metrics deterministic + golden-
  asserted.
- **Observability**: `RepaintedNodeCount`/`DirtyRectCount`/`DirtyArea`/`PictureCacheHitCount`/
  `PictureCacheMissCount`/`PictureCacheEntryCount` (public, deterministic, golden-asserted
  via `Perf.runScript`, plus live `OnFrameMetrics`); the advisory offscreen-effect
  `ControlDiagnostic` (through the existing `Diagnostics` channel, never failing a build).
  Raw cache byte size is a non-golden diagnostic. No unsupported-environment message change.
- **Deferred scope**: Phase 7 (full) only. OUT: **layout hot-path / text-measurement caches
  & layout-boundary hints / structural flattening** (Phase 8); **`SkiaViewer` frame-
  scheduling, readback separation, scene-submission / layer-skipping, render-thread /
  compositor split** (Phase 9 — beyond the byte-identical SKPicture record/replay this rung
  adds); **non-axis-aligned or sub-pixel damage rectangles** (axis-aligned integer only);
  **draw-call batching** (Qt-style); **damage-driven partial-present** (the backend presents
  the whole frame; this rung adds the damage *signal*, not damage-scoped presentation). No
  renderer rewrite, no Avalonia/WPF redesign, no platform/release/distribution scope.
  Features 109–114 are unchanged.

**Gate result: PASS.** No unjustified violations. Tier 1 obligations (`.fsi`, baselines,
tests, docs) are enumerated above and carried into Phase 1.

## Project Structure

Edited / added paths for this feature:

```
src/Controls/
  Types.fsi / Types.fs        # ControlDiagnosticCode gains an advisory offscreen-effect case
                              #   (additive; precedent KeyCollision Types.fsi:154) (+ XML-doc)
  RetainedRender.fsi          # WorkReductionRecord gains RepaintedNodeCount / DirtyRectCount /
                              #   DirtyArea / PictureCacheHits / PictureCacheMisses /
                              #   PictureCacheEntryCount (internal); RetainedRender gains the
                              #   bounded picture cache + a PictureCacheEnabled always-miss flag
                              #   (internal, mirroring MemoEnabled)
  RetainedRender.fs           # step accumulates the damage set from build/carry/paintFresh/
                              #   buildFresh decisions; widens the reuse key (box+theme ->
                              #   full correctness key); counts hits/misses; maintains the
                              #   bounded cross-frame LRU picture cache + entry count; emits the
                              #   offscreen-effect diagnostic. Byte-identical SubtreeScene at rest.

src/Controls.Elmish/
  ControlsElmish.fsi          # FrameMetrics gains six public fields (+ XML-doc)
  ControlsElmish.fs           # thread the six counts from the retained step into FrameMetrics
                              #   (zero record + every per-frame construction site); Perf.runScript
                              #   + OnFrameMetrics surface; PictureCacheEnabled oracle plumbed for tests

src/SkiaViewer/  (optional backend realization — FR-008)
  SceneRenderer.fs            # PictureNode boundary records/replays a real SKPicture for an
                              #   unchanged stable boundary (byte-identical raster); passthrough
                              #   at SceneRenderer.fs:393 stays the at-rest fallback

readiness/surface-baselines/  +  readiness/per-package-surface/
  FS.Skia.UI.Controls*.txt    # regenerated (RefreshSurfaceBaselines): top-level (FrameMetrics
                              #   fields + ControlDiagnosticCode case) + per-package

specs/109-perf-metrics-baseline/readiness/perf-corpus/
  *.golden.txt                # regenerated (PERF_CORPUS_REGEN=1) to carry the six metric fields;
                              #   new scenarios for stable-subtree reuse + cache-cap eviction

tests/Controls.Tests/
  Feature116DamageTests.fs          # FR-001/002/003/004 damage set: small hover vs frame-spanning
                                     #   theme vs idle-zero; deterministic integer DirtyArea/Count
  Feature116PictureCacheTests.fs    # FR-005/006/007 fully-keyed hit/miss: per-keyed-input miss +
                                     #   hit byte-identity + always-miss oracle (cache-on == cache-off)
  Feature116CacheBoundTests.fs      # FR-009/010 bounded LRU: EntryCount <= cap, deterministic
                                     #   eviction, evicted-entry re-miss (never stale)
  Feature116OffscreenDiagTests.fs   # FR-011 advisory offscreen-effect diagnostic fires/does-not-fire,
                                     #   output unchanged

tests/Elmish.Tests/
  Feature116MetricsTests.fs         # FR-012/013 six FrameMetrics goldens over Perf.runScript:
                                     #   idle 0s, localized vs frame-spanning, hit/miss, bounded cap

specs/116-paint-cache-damage-rects/
  spec.md  plan.md  research.md  data-model.md  quickstart.md
  contracts/damage-metrics-contract.md  contracts/picture-cache-contract.md
  contracts/offscreen-effect-diagnostic.md
  readiness/   # evidence-audit.md, skill-loading-evidence.md, byte-identity authority, window-visibility set
```

**Key seams (file:line anchors):**
- Repaint decisions to accumulate into the damage set: `paintFresh` `RetainedRender.fs:494`;
  `buildFresh` `RetainedRender.fs:500`; `carry` `RetainedRender.fs:515`; `build` reuse vs
  recompute `RetainedRender.fs:535-607`. The repainted node's box is `Fragment.Box`
  (`RenderFragment` `RetainedRender.fsi:27`).
- The reuse condition to widen into the full correctness key: `box = pr.Fragment.Box && not
  themeChanged` `RetainedRender.fs:540`; reuse branch `{ pr with Control = nc }`
  `RetainedRender.fs:543`.
- Count-threading precedent (how `MemoHits`/`MemoMisses` reach `FrameMetrics`):
  `WorkReductionRecord` `RetainedRender.fsi` (the 113 `MemoHits`/`MemoMisses` + 114
  `VirtualMaterialized`/`VirtualTotal` fields); construction `RetainedRender.fs:721-730`;
  threading `ControlsElmish.fs:1282-1283`.
- Always-miss-oracle precedent: `MemoEnabled` on `RetainedRender` (`RetainedRender.fsi`),
  the 113 always-miss switch — the `PictureCacheEnabled` flag mirrors it.
- Offscreen-effect diagnostic: `ControlDiagnosticCode` DU `Types.fsi:144-159` (precedent
  case `KeyCollision` `:154`, `UnstableReuseInput`); `ControlDiagnostic` record
  `Types.fsi:420-426`; emission precedent `firstFrameCollisions` `RetainedRender.fs:265-292`;
  diagnostics surfaced on the step result `RetainedRender.fs:720`.
- Offscreen-forcing effect detection sites (backend): `ClipNode` `SceneRenderer.fs:356-367`;
  `withOpacity` `SceneRenderer.fs:28-30`; `CreateDropShadow` `SceneRenderer.fs:125`.
- `Scene.Picture`/`PictureNode`: DU case `Scene.fsi:341`; `Picture` record `Scene.fsi:349-351`;
  `Scene.picture` constructor `Scene.fs:547-548`; backend passthrough
  `SceneRenderer.fs:393` (`| PictureNode picture -> picture.Scene.Nodes |> List.iter ...`).
- `FrameMetrics` type + threading: `ControlsElmish.fsi:68-142`; `lastMemo`/`lastVirtual`
  carriers `ControlsElmish.fs:1259/1263`; `runScript` step `ControlsElmish.fs:1245-1383`.
- Deterministic corpus driver + goldens: `ControlsElmish.Perf.runScript`
  `ControlsElmish.fsi:436`; corpus dir
  `specs/109-perf-metrics-baseline/readiness/perf-corpus/`; regen env `PERF_CORPUS_REGEN`.
- fsproj order: `Types.fsi/.fs` → ... → `RetainedRender.fsi/.fs` (Controls.fsproj);
  `ControlsElmish.fsi/.fs` (Controls.Elmish.fsproj, ProjectReference to Controls).

## Phase 0: Research

See [research.md](./research.md). Resolves: (a) the **damage-set derivation** — which
repaint decisions (`paintFresh`/`buildFresh`/`carry` + genuinely-shifted nodes) feed the
set, how `DirtyRectCount` coalesces (default: one rect per repainted node's box, count =
distinct boxes), and how `DirtyArea` sums integer px² deterministically (FR-001/FR-004);
(b) the **picture-cache correctness key** — the exact shape of the per-node key tuple
covering theme, box, clip, opacity, transform, font/text, visual-state, and how each input
is detected (from the lowered `Control`/attrs already diffed vs an added key hash on the
fragment) so a complete key never hides a change (FR-006); (c) the **hit/miss + always-miss
oracle** — how the widened reuse condition counts hits/misses and how the
`PictureCacheEnabled` flag forces every subtree to repaint, proving cache-on ≡ cache-off
(FR-005/FR-007); (d) the **bounded cross-frame cache** — the entry-count cap value, the LRU
recency structure, deterministic eviction order, and why an evicted entry re-misses correctly
(FR-009/FR-010); (e) **byte-identity** — why accumulating the damage set + counting hits/
misses + maintaining the cache does **not** change the emitted flat `SubtreeScene` at rest
(the step already reuses the same fragment instance) and how the SKPicture record/replay
stays a backend-only byte-identical-raster realization off the deterministic path (FR-008/
FR-014); (f) the **offscreen-effect detection** — the exact site + match (non-opaque opacity
group over a multi-node subtree | clip | drop-shadow/image-filter) and the advisory
diagnostic payload (FR-011); (g) **113/114 interaction** — the picture cache is distinct
from the memo cache, and the damage/cache metrics aggregate correctly over the virtualized
row set incl. a cached row that scrolls out + an evicted entry (FR-015); (h) the **corpus
scenarios** — localized hover, theme switch, idle, stable-subtree reuse, and cache-cap
eviction layouts and their golden assertions.

## Phase 1: Design & Contracts

- [data-model.md](./data-model.md): the damage set + `RepaintedNodeCount`/`DirtyRectCount`/
  `DirtyArea` carriers on `WorkReductionRecord`; the picture-cache correctness key record
  and per-node detection; the `PictureCacheHits`/`PictureCacheMisses`/`PictureCacheEntryCount`
  carriers + the bounded LRU cache state on `RetainedRender` and the `PictureCacheEnabled`
  flag; how the six counts aggregate into the `FrameMetrics` fields (the 113/114 path); the
  offscreen-effect diagnostic payload; and the idle = `0/0/0` + hit/miss = 0 rule.
- [contracts/damage-metrics-contract.md](./contracts/damage-metrics-contract.md): the
  damage contract — localized change → small `RepaintedNodeCount`/`DirtyRectCount`/`DirtyArea`
  proportional to the changed control(s); theme switch → frame-spanning; idle → `0/0/0`;
  deterministic integer rounding; damage never under-reports actual repaint (honest repaint
  set incl. genuinely-shifted nodes).
- [contracts/picture-cache-contract.md](./contracts/picture-cache-contract.md): the cache
  contract — reuse (a hit) **only** when *all* keyed inputs are unchanged; each keyed input
  independently forces a miss; a hit is byte-identical to repaint; cache-on ≡ cache-off
  (always-miss oracle); `PictureCacheEntryCount <= cap` under eviction pressure; deterministic
  LRU eviction; evicted entry re-misses (never stale); the optional SKPicture record/replay
  is byte-identical raster off the deterministic path.
- [contracts/offscreen-effect-diagnostic.md](./contracts/offscreen-effect-diagnostic.md):
  the diagnostic contract — fires for a control whose paint requires offscreen composition
  (non-opaque opacity group over a multi-node subtree | clip | drop-shadow/image-filter),
  does not fire otherwise, names the control/effect, is advisory only (never fails a build,
  never alters rendered output).
- [quickstart.md](./quickstart.md): how to run the damage / picture-cache / bound / diagnostic
  / metrics tests, regenerate the corpus goldens (`PERF_CORPUS_REGEN=1`, incl. the new
  reuse + eviction scenarios) and surface baselines (`RefreshSurfaceBaselines`), and run the
  escalated gate set.
- Agent context update: `AGENTS.md` SPECKIT marker repointed to this plan.

## Phase 2: Planning complete

Stop after design. `tasks.md` is produced by `/speckit.tasks`.

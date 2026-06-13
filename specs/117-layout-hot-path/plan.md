# Implementation Plan: Layout Hot-Path Improvements

**Branch**: `117-layout-hot-path` | **Date**: 2026-06-13 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/117-layout-hot-path/spec.md`

## Summary

Features 109–116 hardened the retained hot path (honest frame metrics + a perf corpus,
retained pointer routing, a view-skip frame scheduler, a targeted runtime visual-state
stamp, a control-internal memoization seam, observable viewport virtualization, and the
Phase 7 paint cache + damage rects + offscreen diagnostic). The performance report's next
rung is **Phase 8 (Layout Hot-Path Improvements)**: the single largest remaining avoidable
repeated cost on the layout hot path is **text measurement** — every label/caption leaf
re-runs `Scene.measureText` each frame, and the auto-fit caption path (`fittedFontSize`)
runs a binary search that calls `measureText` up to ~17 times for one caption. A consumer
scrolling, hovering, or animating over a text-dense surface pays full text-shaping cost on
every frame even when the measured text and font are identical. Phase 9 (`SkiaViewer`
scheduling / readback / compositor review) stays **out of scope**.

**Technical approach (Phase 8 of the performance report):**

1. **A deterministic, bounded text-measurement cache** (`FS.Skia.UI.Controls`). All text
   measurement on the layout path funnels through `Scene.measureText`
   (`Scene.fs:524-530`) — six call sites in `Control.fs` (`fittedFontSize` `:239`,
   `buttonGeom` `:786`, `badgeGeom` `:821`, `textFieldGeom` `:966`, `textAreaFieldGeom`
   `:996`, `richTextGeom` `:1017`), where `fittedFontSize` alone calls it 1 + up to 16
   times per caption. This feature interposes a cache keyed by **every input that affects
   the measured result** — text string, font family, font size, font weight — in front of
   that primitive. A resident key reuses the cached `TextMetrics` without re-invoking the
   underlying measurement; a new or evicted key measures once and stores. The cache is a
   **bounded** structure (fixed entry cap + deterministic LRU eviction, mirroring 116's
   `PictureCache`), so a many-distinct-string scenario cannot grow memory without bound and
   hit/miss/eviction outcomes are reproducible for goldens (FR-001/FR-002/FR-003).

2. **An always-miss oracle + byte-identity** (`FS.Skia.UI.Controls`). `Scene.measureText`
   is a pure function of its inputs (no hidden state); the cache returns the *same*
   `TextMetrics` value the un-cached call would return, so no measured width/height, layout
   box, fitted font size, or rendered scene changes value because the cache exists. A
   per-cache **always-miss flag** (mirroring 113 `MemoEnabled` / 116 `PictureCacheEnabled`)
   forces every request to re-measure, proving cache-on ≡ cache-off output and layout
   (FR-004). The cache is a transparent accelerator: the un-cached measurement is the
   oracle (spec interaction note: byte-identity wins over hit rate).

3. **Two public text-cache counters** (`FS.Skia.UI.Controls.Elmish`, `FrameMetrics`). Two
   new public integer fields — `TextMeasureCacheHitCount` and `TextMeasureCacheMissCount` —
   count cache hits and misses for the frame, `0` on a frame that measures no text,
   following the established hit/miss pattern (`MemoHitCount`/`MemoMissCount` `:88-98`,
   `PictureCacheHitCount`/`PictureCacheMissCount` `:129-143`). Threaded from the retained
   step via `WorkReductionRecord` (the 113/114/116 path) and golden-asserted on the
   deterministic `Perf.runScript` corpus (FR-005/FR-010).

4. **One public layout-invalidated counter** (`FS.Skia.UI.Controls.Elmish`,
   `FrameMetrics`). Today `RemeasuredNodeCount` reports the *post-pinning* re-measured set
   (`RetainedRender.fs:575` = `layoutResult.Invalidated |> List.length`), but the size of
   the **dirty set fed into incremental layout** (the layout-invalidated nodes, computed by
   feature 097/101's `layoutDirtySet` before fixed-size-ancestor pinning reduced the actual
   measure work) is not surfaced. This feature surfaces it as a new public integer field,
   `LayoutInvalidatedNodeCount`, distinct from `RemeasuredNodeCount`: `0` on idle and on
   style-only / visual-state-only frames, and `>= RemeasuredNodeCount` on a geometry frame
   (pinning can reduce re-measures below the invalidated set). Reporting-only; reduces no
   work itself but hardens 097/101 against a silently-widening dirty set (FR-006).

5. **Style-only / visual-state-only = zero work, made explicit** (assertion rung). A
   hover/focus/press/animation-tick frame over a text-bearing control re-measures zero
   layout nodes (`RemeasuredNodeCount = 0`), reports zero `LayoutInvalidatedNodeCount`, and
   produces zero text-cache misses for unchanged text (every measurement served from the
   warm cache), while staying byte-identical at rest. This is largely an assertion over
   behavior 096/112/113 already produce; this rung formalizes the report's Phase 8
   acceptance criterion as a deterministic gate (FR-007).

6. **Drift guard + no multi-pass introduction** (anti-drift rung). Feature 101's
   `layoutDriftReport` / single-sourced `layoutAffectingAttrNames` (`Control.fs:1252` =
   `{ AttrWidth; AttrHeight; AttrOrientation }`) MUST remain in force; this rung adds no new
   geometry-driving attribute, so the guard MUST still report empty drift. The codebase
   performs a single measure pass with no intrinsic-sizing path; this feature introduces
   no multi-pass / intrinsic path and adds no multi-pass metric (a no-op against the
   report's optional Phase 8 task 5) (FR-008/FR-009).

This is **additive observability + a correctly-keyed, bounded text-measure cache only**
(FR-004). At-rest rendered output (the deterministic scene-list goldens), control geometry,
fitted font sizes, DataGrid geometry, charts, screenshots, Vulkan/Skia output, focus /
keyboard / pointer routing, and every dispatch outcome stay **byte-identical**;
cache-on ≡ cache-off. The only intended observable deltas are the three new public
`FrameMetrics` fields (two text-cache counters + one layout-invalidated count). Damage rects
/ picture cache (116) and virtualization (114) are unaffected and continue to work (FR-008).
**Structural-wrapper flattening (report task 4), intrinsic/multi-pass layout, and Phase 9
(`SkiaViewer` scheduling / readback / compositor) are deferred** — flattening risks semantic
change and has no byte-identical guarantee, so it is not attempted here.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: No new dependencies. Edits `FS.Skia.UI.Controls`
(a bounded text-measure cache + always-miss flag interposed over `Scene.measureText` at the
six `Control.fs` call sites and `fittedFontSize`; `WorkReductionRecord`/`RetainedRender`
gain internal carriers for the hit/miss counts, the cache state, the always-miss flag, and
the dirty-set size) and `FS.Skia.UI.Controls.Elmish` (the `FrameMetrics` record + threading
through the retained step / `Perf.runScript` / `OnFrameMetrics`). Consumes existing
`Scene.measureText`/`TextMetrics`/`FontSpec`, `WorkReductionRecord`, `RetainedRender.step`,
`MemoEnabled`/`PictureCacheEnabled` precedent, `Layout.evaluateIncremental` /
`layoutDirtySet` / `layoutAffectingAttrNames`. **Phase 0 decides** where the cache lives
(in `Scene` as a measure-through wrapper vs. in `Controls` over the call sites) and how the
dirty-set size is threaded from the layout call into `WorkReductionRecord`.
**Testing**: Expecto + FsCheck. Text-cache key / always-miss-oracle / bounded-eviction /
empty-text / fitted-caption tests + the layout-invalidated-count and style-only-zero-work
tests in `tests/Controls.Tests` (reaching internal seams via
`InternalsVisibleTo "Controls.Tests"`); the three metric corpus goldens in
`tests/Elmish.Tests` over `ControlsElmish.Perf.runScript`; the standing Scene-parity golden
suite under `Dev` for at-rest byte-identity (FR-004).
**Target Platform**: Windows and Linux (no platform-specific code; no Vulkan change).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification — Tier 1 (contracted change).** Three new **public** `FrameMetrics`
fields (`ControlsElmish.fsi`: `TextMeasureCacheHitCount`, `TextMeasureCacheMissCount`,
`LayoutInvalidatedNodeCount`), plus internal threading through
`WorkReductionRecord`/`RetainedRender` and the internal text-measure cache. The top-level
surface baseline and per-package baselines move; the full artifact chain applies (`.fsi`
updates, baseline regeneration, test evidence, XML-doc). `Route` escalates to the
**controls-public-surface** tier (consistent with 109/110/113/114/116).

**Principle compliance.**
- *I (Spec→FSI→Tests→Impl)*: the three `FrameMetrics` fields and the internal cache /
  always-miss flag / dirty-set carrier are drafted in `.fsi` signature form first and
  exercised from FSI/tests; the cold-miss-then-warm-hit assertion, the per-keyed-input
  cache-miss proof, the cache-on ≡ cache-off oracle, the bounded-eviction proof, the
  empty/whitespace + fitted-caption proofs, the layout-invalidated `>= RemeasuredNodeCount`
  proof, and the style-only zero-work proof are the failing-first proofs.
- *II (Visibility in `.fsi`)*: the three metric fields are public, declared in
  `ControlsElmish.fsi`; the text-measure cache, the hit/miss counters on
  `WorkReductionRecord`, the always-miss flag, and the dirty-set-size carrier are internal
  (declared in the owning `Controls` `.fsi`, reached via `InternalsVisibleTo`). No access
  modifiers in `.fs`.
- *III (Idiomatic simplicity)*: the cache key is a value tuple/record of the four measured
  inputs compared structurally; the bounded LRU cache is a `Map` + recency structure with a
  `mutable` accumulator disclosed at the use site (constitution III, exactly as the 116
  picture cache and existing id/work counters — interpreter-edge mutation confined to the
  step / render path). No SRTP / reflection / type-providers; the layout-invalidated count
  is `Set.count` of the existing dirty set.
- *IV (Elmish/MVU boundary)*: unchanged — `Model`, `Msg`, `Effect`/`Cmd`, `init`, pure
  `update`, subscriptions, and interpreter behaviour are untouched. The text-measure cache
  is a render-time / measure-path cache (interpreter-edge mutation confined to the step);
  `view`/`update` stay pure; dispatch outcomes are byte-identical (FR-004).
- *V (Synthetic disclosure)*: none expected — cold/warm hit-miss, bounded eviction, and the
  layout-invalidated count are proven over real corpus scenarios on the real
  `Perf.runScript` path; byte-identity uses the real Scene-parity suite and the real
  always-miss oracle. Any unavoidable stub returns to task review for `[S]` disclosure.
- *VI (Test evidence)*: cold-frame misses then warm-frame hits; idle = 0 hits/0 misses;
  per-keyed-input independent miss (text / family / size / weight); empty + whitespace text;
  fitted-caption distinct-candidate keys + unchanged chosen size; cache hit byte-identical
  `TextMetrics`; cache-on ≡ cache-off (always-miss oracle over the whole corpus);
  bounded cap + deterministic eviction + evicted-entry re-miss; style-only frame
  `RemeasuredNodeCount = 0` / `LayoutInvalidatedNodeCount = 0` / `0` misses; geometry frame
  `LayoutInvalidatedNodeCount >= RemeasuredNodeCount`; all three metrics deterministic +
  golden-asserted — all fail before / pass after; no assertion weakening.
- *VII (Observability)*: the two text-cache counters make a regression that re-shapes
  identical text visible as a golden change; `LayoutInvalidatedNodeCount` makes a
  silently-widening dirty set visible before it surfaces as re-measures. No silent failure.

### Repository Governance Decisions

- **Template ownership**: N/A — no `template/**`, sample, Spec Kit asset, package-policy, or
  command-surface change; the three metric fields do not alter
  `.template.config/template.json`. (The merge-time template package-pin bump is the
  standard post-merge step, not a content change in this feature.)
- **Dependency impact**: N/A — no new package; `Directory.Packages.props`,
  `docs/dependencies.md`, generated template inclusion, and `DependencyReport` coverage are
  unchanged.
- **Command-surface impact**: No new gate. Escalated **controls-public-surface** set because
  the `Controls.Elmish` `FrameMetrics` `.fsi` surface changes; run `Route` first and obey
  its printed minimal list. `RefreshSurfaceBaselines` regenerates the top-level + per-package
  baselines after the three `FrameMetrics` additions; the `Perf.runScript` corpus goldens are
  regenerated (`PERF_CORPUS_REGEN=1`) to carry the three new metric fields and the new
  text-heavy + style-only scenarios. FAKE-backed commands share `.fake` state and are not
  safe to run concurrently; run them sequentially in the deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: N/A to behaviour — generated default/minimal contents,
  selected Controls guidance, local skills, and generated `Dev` behaviour are unchanged.
  Generated projects gain the three new public `FrameMetrics` fields transitively (additive;
  `OnFrameMetrics` default stays `ignore`; the cache is byte-identical at rest).
- **Evidence paths**: text-cache key (cold miss → warm hit, per-keyed-input miss, hit
  byte-identity) + always-miss-oracle + bounded-eviction + empty/whitespace + fitted-caption
  tests under `tests/Controls.Tests/Feature117*.fs`; the layout-invalidated-count +
  style-only-zero-work tests under `tests/Controls.Tests/Feature117*.fs`; the three-metric
  corpus goldens under `specs/109-perf-metrics-baseline/readiness/perf-corpus/*.golden.txt`
  (regenerated) + asserted in `tests/Elmish.Tests/Feature117*.fs`; at-rest byte-identity via
  the standing Scene-parity suite under `Dev`; skill-loading evidence in
  `specs/117-layout-hot-path/readiness/skill-loading-evidence.md`; the byte-identity
  authority note + the text-cache authority note; the window-visibility not-applicable set;
  `readiness/evidence-audit.md` (verdict token); `generated-validation.md`
  (`package-resolution=resolved`, `package-mismatch=false`); surface/per-package baselines
  under `readiness/surface-baselines/` + `readiness/per-package-surface/`.
- **`.fsi` / contract impact**: **Breaking** `ControlsElmish.fsi` `FrameMetrics` change —
  three new public fields (`TextMeasureCacheHitCount`, `TextMeasureCacheMissCount`,
  `LayoutInvalidatedNodeCount`) with XML-doc (doc-preservation gate; attribute-before-doc-
  before-type ordering preserved, `///` before each field). The text-measure cache + the
  hit/miss carriers on `WorkReductionRecord` + the always-miss flag + the dirty-set-size
  carrier are internal on `RetainedRender.fsi` / the owning Controls `.fsi` (no public delta
  there). The top-level surface baseline changes (the three `FrameMetrics` fields);
  per-package Controls + Controls.Elmish baselines regenerate. **Phase 0 decides** the exact
  cache key carrier shape, the cache home (`Scene` measure-through vs. `Controls` interpose),
  and the dirty-set-size threading.
- **MVU/effect boundary**: Unchanged (preserved, not modified). `Model`/`Msg`/`Effect`/
  `init`/`update`/interpreter are untouched. The text-measure cache is a measure-path /
  render-side cache (interpreter-edge mutation confined to the step, constitution III,
  exactly as the 116 picture cache and the existing id/work counters); `view`/`update` stay
  pure; dispatch outcomes are byte-identical (FR-004).
- **Synthetic evidence**: None planned. Hit/miss + bounded eviction = real counts over the
  real `Perf.runScript` corpus; cache-on ≡ cache-off = the real always-miss oracle;
  byte-identity = the real Scene-parity suite; the layout-invalidated count = the real dirty
  set size. Any unavoidable stub returns to task review for `[S]` disclosure under Principle V.
- **Test evidence**: failing-first — cold first frame reports text-cache misses, warm frame
  reports hits and zero misses for unchanged text; idle = 0 hits/0 misses; changing each
  keyed input independently (text / family / size / weight) forces a miss with the correct
  fresh `TextMetrics`; empty + whitespace text measures + caches without error; a fitted
  caption's distinct candidate sizes are distinct keys and the chosen size is unchanged;
  a hit is byte-identical to the un-cached measure; cache-on ≡ cache-off (always-miss oracle
  over the whole corpus); the cache never exceeds its cap, eviction is deterministic, an
  evicted entry re-misses (never stale); a style-only / visual-state-only frame reports
  `RemeasuredNodeCount = 0`, `LayoutInvalidatedNodeCount = 0`, and `0` text-cache misses;
  a geometry frame reports `LayoutInvalidatedNodeCount >= RemeasuredNodeCount` that is
  bounded and explainable; all three metrics deterministic + golden-asserted.
- **Observability**: `TextMeasureCacheHitCount` / `TextMeasureCacheMissCount` /
  `LayoutInvalidatedNodeCount` (public, deterministic, golden-asserted via `Perf.runScript`,
  plus live `OnFrameMetrics`). The text-cache raw entry count / byte size is not a golden
  field (no new public counter for it; the cap is an internal invariant proven by test, not
  a `FrameMetrics` field — unlike 116's `PictureCacheEntryCount`, which Phase 0 confirms is
  the precedent to follow or diverge from). No unsupported-environment message change.
- **Deferred scope**: Phase 8 (text-measure cache + dirty-propagation observability) only.
  OUT: **structural-wrapper flattening** (report task 4 — risks semantic change, no
  byte-identical guarantee); **intrinsic / multi-pass layout introduction** and any
  multi-pass metric (report optional task 5 — no such path exists; this rung must not create
  one); **`SkiaViewer` frame-scheduling, readback separation, scene-submission /
  layer-skipping, render-thread / compositor split** (Phase 9); **GPU / layer caching**; any
  **timing-based pass/fail gate** (the metrics are counts, not durations). No renderer
  rewrite, no platform/release/distribution scope. Features 109–116 are unchanged.

**Gate result: PASS.** No unjustified violations. Tier 1 obligations (`.fsi`, baselines,
tests, docs) are enumerated above and carried into Phase 1.

## Project Structure

Edited / added paths for this feature:

```
src/Scene/                          (Phase 0 decides: cache home option A)
  Scene.fsi / Scene.fs              # measureText (Scene.fs:524-530) MAY gain an internal
                                    #   measure-through cache wrapper + always-miss flag, or
                                    #   stay pure and be wrapped from Controls (option B).

src/Controls/
  Control.fs                        # the six measureText call sites (:239 fittedFontSize,
                                    #   :786 buttonGeom, :821 badgeGeom, :966 textFieldGeom,
                                    #   :996 textAreaFieldGeom, :1017 richTextGeom) route
                                    #   through the cache; layoutAffectingAttrNames (:1252)
                                    #   unchanged; evaluateIncremental call (:1307) surfaces
                                    #   the dirty-set size
  RetainedRender.fsi                # WorkReductionRecord gains TextMeasureCacheHits /
                                    #   TextMeasureCacheMisses / LayoutInvalidatedNodeCount
                                    #   (internal); RetainedRender gains the bounded text cache
                                    #   + a TextCacheEnabled always-miss flag (internal,
                                    #   mirroring MemoEnabled / PictureCacheEnabled)
  RetainedRender.fs                 # step routes measurement through the cache, counts
                                    #   hits/misses, threads the dirty-set size (Set.count of
                                    #   layoutDirtySet output, distinct from
                                    #   layoutResult.Invalidated at :575). Byte-identical
                                    #   SubtreeScene at rest.

src/Controls.Elmish/
  ControlsElmish.fsi                # FrameMetrics gains three public fields (+ XML-doc):
                                    #   TextMeasureCacheHitCount, TextMeasureCacheMissCount,
                                    #   LayoutInvalidatedNodeCount
  ControlsElmish.fs                 # thread the three counts from the retained step /
                                    #   WorkReductionRecord into FrameMetrics (zero record
                                    #   :1366-1388 + every per-frame construction site :1421,
                                    #   :1478, key/pointer frames); Perf.runScript +
                                    #   OnFrameMetrics (:1003) surface; TextCacheEnabled oracle
                                    #   plumbed for tests

readiness/surface-baselines/  +  readiness/per-package-surface/
  FS.Skia.UI.Controls*.txt          # regenerated (RefreshSurfaceBaselines): top-level
                                    #   (FrameMetrics fields) + per-package

specs/109-perf-metrics-baseline/readiness/perf-corpus/
  *.golden.txt                      # regenerated (PERF_CORPUS_REGEN=1) to carry the three
                                    #   metric fields; new text-heavy + style-only scenarios

tests/Controls.Tests/
  Feature117TextCacheTests.fs       # FR-001/002/004 fully-keyed hit/miss: per-keyed-input miss
                                    #   (text/family/size/weight) + hit byte-identity +
                                    #   always-miss oracle (cache-on == cache-off) + empty/
                                    #   whitespace + fitted-caption distinct keys / unchanged size
  Feature117CacheBoundTests.fs      # FR-003 bounded LRU: entry count <= cap, deterministic
                                    #   eviction, evicted-entry re-miss (never stale)
  Feature117LayoutInvalidatedTests.fs # FR-006/007 layout-invalidated count: idle = 0,
                                    #   style-only = 0 (>= RemeasuredNodeCount holds trivially),
                                    #   geometry frame bounded and >= RemeasuredNodeCount;
                                    #   FR-008 drift guard still empty

tests/Elmish.Tests/
  Feature117MetricsTests.fs         # FR-005/006/010 three FrameMetrics goldens over
                                    #   Perf.runScript: cold-miss → warm-hit text-heavy frame,
                                    #   style-only zero-miss / zero-invalidated frame, idle 0s

specs/117-layout-hot-path/
  spec.md  plan.md  research.md  data-model.md  quickstart.md
  contracts/text-measure-cache-contract.md
  contracts/layout-invalidated-metric-contract.md
  readiness/   # evidence-audit.md, skill-loading-evidence.md, byte-identity authority,
               #   text-cache authority, window-visibility not-applicable set
```

**Key seams (file:line anchors):**
- Text-measure primitive to cache: `Scene.measureText` body `Scene.fs:524-530`; signature
  `Scene.fsi:470`; `TextMetrics` (Width/Height/Baseline) + `FontSpec` (Family/Size/Weight)
  are the key inputs.
- Call sites to route through the cache (all `Control.fs`): `fittedFontSize` `:233-256`
  (calls `measureText` at `:239`, 1 + up to 16× per caption), `buttonGeom` `:786`,
  `badgeGeom` `:821`, `textFieldGeom` `:966`, `textAreaFieldGeom` `:996`, `richTextGeom`
  `:1017`.
- Count-threading precedent (how `MemoHits`/`MemoMisses` / `PictureCacheHits`/`Misses` reach
  `FrameMetrics`): `WorkReductionRecord` `RetainedRender.fsi:157-205`; construction
  `RetainedRender.fs:943`; live threading `ControlsElmish.fs:1003`; `runScript` threading
  `ControlsElmish.fs:1425/1482`.
- `RemeasuredNodeCount` (post-pinning, the existing field) vs. the new dirty-set size:
  `let remeasured = layoutResult.Invalidated |> List.length` `RetainedRender.fs:575`;
  `Layout.evaluateIncremental` returns `Invalidated` `Layout.fs:718`; dirty set produced by
  `layoutDirtySet` `RetainedRender.fs:497-504`; fed to `evaluateIncremental` at
  `Control.fs:1307` (`Set.toList dirty`). The new `LayoutInvalidatedNodeCount = Set.count
  dirty` (before pinning), so `>= RemeasuredNodeCount`.
- Drift guard (feature 101): `layoutAffectingAttrNames` `Control.fs:1252`
  (`{ AttrWidth; AttrHeight; AttrOrientation }`); `layoutDirtySet` `RetainedRender.fs:497`.
- Always-miss-oracle precedent: `MemoEnabled` `RetainedRender.fs:81`/`.fsi:130`,
  `PictureCacheEnabled` `RetainedRender.fs:87`/`.fsi:145`; init `:479/:482`; threaded
  frame-to-frame `:932/:935`. The `TextCacheEnabled` flag mirrors them.
- `FrameMetrics` type + threading: `ControlsElmish.fsi:68-174`; zero record
  `ControlsElmish.fs:1366-1388`; per-frame construction `:1421-1442` (move), `:1478-1496`
  (tick), key/pointer frames following the same `{ zero with … }` pattern; live
  `OnFrameMetrics` sink `:1003`.
- Deterministic corpus driver + goldens: `ControlsElmish.Perf.runScript`
  `ControlsElmish.fsi:472-476`; corpus dir
  `specs/109-perf-metrics-baseline/readiness/perf-corpus/`; regen env `PERF_CORPUS_REGEN`.
- fsproj order: `Scene.fsi/.fs` (Scene.fsproj) → … → `Control.fs` → `RetainedRender.fsi/.fs`
  (Controls.fsproj); `ControlsElmish.fsi/.fs` (Controls.Elmish.fsproj, ProjectReference to
  Controls). The cache type must be declared before its first use (Phase 0 confirms the home).

## Phase 0: Research

See [research.md](./research.md). Resolves: (a) the **cache home** — interpose inside
`Scene.measureText` as an internal measure-through wrapper (option A) vs. wrap from
`Controls` over the six call sites + `fittedFontSize` (option B), and the consequence for
hit/miss counting and the always-miss flag; (b) the **cache key shape** — the exact value
tuple/record over `(text, family, size, weight)` and whether any measurement constraint
(available width/height in `fittedFontSize`) belongs in the key (it does not change
`measureText`'s output, only the search path, so it is *not* keyed — each candidate size is
already a distinct key); (c) the **hit/miss + always-miss oracle** — how a resident key
counts a hit vs. a fresh measure counts a miss, and how the `TextCacheEnabled` flag forces
every request to re-measure, proving cache-on ≡ cache-off (FR-004); (d) the **bounded
cache** — the entry-cap value (align with 116's `PictureCacheCap = 256` LRU or justify a
different cap), the deterministic recency / eviction order, and why an evicted entry
re-misses correctly (FR-003); (e) **byte-identity** — why `Scene.measureText` is a pure
function so the cached value equals the un-cached value for every key, and why caching does
not change the emitted flat `SubtreeScene`, layout boxes, or fitted font sizes at rest
(FR-004); (f) the **layout-invalidated count** — that the dirty set fed to
`evaluateIncremental` (`Set.count` of `layoutDirtySet`) is the pre-pinning invalidated set,
how it is threaded into `WorkReductionRecord` distinct from the post-pinning
`RemeasuredNodeCount` at `:575`, and why `invalidated >= remeasured` always holds (FR-006);
(g) the **fitted-caption interaction** — the binary search's distinct candidate sizes are
distinct keys, the cache helps across frames (same caption + same box ⇒ same search path),
and the chosen fitted size is unchanged (edge case); (h) **113/114/116 interaction** — the
text cache is distinct from the memo cache (113) and the picture cache (116), and the
text-cache + layout-invalidated metrics aggregate correctly over the virtualized row set
(114) (FR-008); (i) the **corpus scenarios** — a text-heavy repeated-caption layout (cold →
warm), a style-only / visual-state frame (zero re-measure / zero invalidated / zero miss),
an idle frame (all zero), and a cache-cap eviction layout, and their golden assertions
(FR-010).

## Phase 1: Design & Contracts

- [data-model.md](./data-model.md): the text-measure cache key record + the bounded LRU
  cache state + the `TextCacheEnabled` flag on `RetainedRender`; the
  `TextMeasureCacheHits`/`TextMeasureCacheMisses`/`LayoutInvalidatedNodeCount` carriers on
  `WorkReductionRecord`; how the three counts aggregate into the `FrameMetrics` fields (the
  113/114/116 path); the dirty-set-size vs. post-pinning `RemeasuredNodeCount` distinction;
  and the idle / style-only = `0` rule.
- [contracts/text-measure-cache-contract.md](./contracts/text-measure-cache-contract.md):
  the cache contract — reuse (a hit) **only** when *all* keyed inputs (text, family, size,
  weight) are unchanged; each keyed input independently forces a miss; a hit is byte-identical
  to the un-cached measure; cache-on ≡ cache-off (always-miss oracle); the cache never
  exceeds its cap; deterministic LRU eviction; an evicted entry re-misses (never stale);
  empty/whitespace text caches without error; fitted-caption distinct candidate keys with an
  unchanged chosen size; theme switch legitimately misses.
- [contracts/layout-invalidated-metric-contract.md](./contracts/layout-invalidated-metric-contract.md):
  the metric contract — `LayoutInvalidatedNodeCount` is the size of the dirty set fed into
  incremental layout (pre-pinning), distinct from `RemeasuredNodeCount` (post-pinning); `0`
  on idle and on style-only / visual-state-only frames; `>= RemeasuredNodeCount` and bounded
  / explainable on a geometry-changing frame; the feature 101 drift guard stays in force and
  reports empty drift (no new geometry-driving attribute).
- [quickstart.md](./quickstart.md): how to run the text-cache / bound / fitted-caption /
  layout-invalidated / style-only / metrics tests, regenerate the corpus goldens
  (`PERF_CORPUS_REGEN=1`, incl. the new text-heavy + style-only + eviction scenarios) and
  surface baselines (`RefreshSurfaceBaselines`), and run the escalated gate set.
- Agent context update: `AGENTS.md` SPECKIT marker repointed to this plan.

## Phase 2: Planning complete

Stop after design. `tasks.md` is produced by `/speckit.tasks`.

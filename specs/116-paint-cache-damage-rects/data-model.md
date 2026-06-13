# Phase 1 Data Model: Paint Cache, Damage Rectangles & Optional Skia Picture Boundaries

All types below are **F# records/DUs**. Public surface is declared in the named `.fsi`;
internal carriers live on `RetainedRender.fsi` and are reached from tests via
`InternalsVisibleTo "Controls.Tests"`. Phase 0 (research.md) fixed the derivations; this
document fixes the shapes and the construction/threading sites.

## 1. Damage set + carriers (internal — `RetainedRender`)

The damage set is **not** a stored field; it is accumulated during `step` from the existing
repaint decisions and reduced to three integer carriers on `WorkReductionRecord`:

```
// WorkReductionRecord (RetainedRender.fsi) — new fields (additive; 113/114 precedent)
RepaintedNodeCount : int   // nodes whose OwnScene was repainted this frame + genuinely-shifted nodes
DirtyRectCount     : int   // distinct axis-aligned repainted boxes (one per repainted node's Fragment.Box; deduped)
DirtyArea          : int   // sum of (w*h) over the distinct rectangles, integer px²
```

**Derivation (per Phase 0a).** During the `step` walk, each repaint branch
(`paintFresh`/`buildFresh`/`carry`-recompute, `RetainedRender.fs:494/500/515`) and each
genuinely-shifted node contributes its `RenderFragment.Box` (`RetainedRender.fsi:27`). The
walk accumulates a list/set of repainted boxes; the three carriers are computed from it:
count of repainted nodes, count of distinct boxes, summed integer area.

**Invariants.**
- Idle frame (no repaint): `RepaintedNodeCount = 0`, `DirtyRectCount = 0`, `DirtyArea = 0`.
- Localized change: `RepaintedNodeCount <= 4` (changed node + immediate shifted neighbours;
  exactly `1` for a leaf hover with no sibling shift) **and** `< TotalNodeCount`;
  `DirtyArea < FrameArea`. Not frame-spanning. (Concrete predicates in
  `contracts/damage-metrics-contract.md` → *Concrete assertion bounds*.)
- Theme switch (all paint invalidated): frame-spanning — `RepaintedNodeCount = TotalNodeCount`,
  `DirtyArea = FrameArea`.
- Deterministic: integer geometry → integer rounding → reproducible across runs.
- Damage never under-reports actual repaint (honest set, FR-002 resolution).

## 2. Picture-cache correctness key (internal — `RetainedRender`)

```
// PictureCacheKey (RetainedRender.fs; shape declared internal in RetainedRender.fsi)
type internal PictureCacheKey =
    { Theme       : <theme identity>      // the step's themeChanged input
      Box         : Rect option           // Fragment.Box
      Clip        : <clip descriptor>     // from lowered attrs/scene
      Opacity     : <opacity value>       // from lowered attrs/scene
      Transform   : <transform descriptor>
      FontText    : <font/text descriptor>
      VisualState : <visual-state value> }
```

The exact field representations are read from the **lowered `Control`/attrs already diffed**
by `Reconcile` at the node (Phase 0b). Equality is structural; **a hit requires equality in
every field**. The per-keyed-input miss test perturbs one field at a time and asserts a miss
each time, proving no field is omitted (FR-006).

## 3. Bounded cross-frame picture cache + carriers (internal — `RetainedRender`)

```
// On RetainedRender<'msg> (RetainedRender.fsi) — new internal fields
PictureCache        : <bounded LRU store>   // RetainedId -> (cached fragment * PictureCacheKey), capped + recency-ordered
PictureCacheEnabled : bool                  // always-miss oracle (mirrors MemoEnabled); false => every node repaints

// WorkReductionRecord (RetainedRender.fsi) — new fields
PictureCacheHits       : int   // stable subtrees reused from cache without repaint this frame
PictureCacheMisses     : int   // subtrees repainted/recorded fresh this frame
PictureCacheEntryCount : int   // live cache entry count after this frame (<= cap)
```

**Cap + eviction (per Phase 0d).** A fixed entry cap, committed as a named constant in
`RetainedRender.fs`:

```
// RetainedRender.fs
[<Literal>] let internal PictureCacheCap = 256   // entries
```

`256` is chosen to sit **above** the corpus's stable-subtree count (every existing corpus
scene materializes well under 256 cacheable subtrees, so steady-state corpus frames never
evict — `PictureCacheEntryCount` reflects live size, FR-012) and **below** the eviction-
pressure scenario's distinct-identity count (T014/T015 drive **320** distinct cacheable
row identities = `1.25 × cap`, forcing ≥ 64 evictions so the bound is exercised, FR-009).
Recency order derives from the frame's deterministic traversal order (no wall-clock). On
overflow, least-recently-used entries are dropped. The cap and the `320`-identity eviction
scenario are the concrete values T014/T015 assert against.

**Invariants.**
- `PictureCacheEntryCount <= cap` at all times.
- Eviction is deterministic: same input sequence → same surviving entries.
- An evicted entry re-misses (recompute fresh) when next needed — never a stale hit (FR-010).
- `PictureCacheEnabled = false` ⇒ `PictureCacheHits = 0` and the rendered `SubtreeScene` is
  byte-identical to `PictureCacheEnabled = true` (cache-on ≡ cache-off, FR-007).
- A hit emits the identical fragment instance ⇒ byte-identical `SubtreeScene` at rest
  (FR-014).

## 4. Public `FrameMetrics` fields (public — `ControlsElmish.fsi`)

```
// FrameMetrics (ControlsElmish.fsi) — six new public fields (breaking; XML-doc each)
RepaintedNodeCount     : int
DirtyRectCount         : int
DirtyArea              : int
PictureCacheHitCount   : int
PictureCacheMissCount  : int
PictureCacheEntryCount : int
```

**Threading (the 113/114 path).** In `ControlsElmish.fs` `runScript`, carry the step's
`WorkReduction.{RepaintedNodeCount, DirtyRectCount, DirtyArea, PictureCacheHits,
PictureCacheMisses, PictureCacheEntryCount}` (alongside the existing `lastMemo`/`lastVirtual`
carriers, `:1259/1263/1282-1283`) into the `FrameMetrics` record at **every** construction
site (the `zero` record + each per-frame record) and through the live `OnFrameMetrics` sink.

**Invariants.**
- Deterministic + golden-asserted via `Perf.runScript` (FR-012).
- Idle frame: damage = `0/0/0`, hit/miss = `0` (a steady cache may retain entries, so
  `PictureCacheEntryCount` reflects live size, not necessarily 0).
- Aggregate correctly over multiple subtrees / the virtualized row set (114, FR-015).

## 5. Offscreen-effect diagnostic (public — `Types.fsi`)

```
// ControlDiagnosticCode (Types.fsi:144-159) — one new advisory case
| OffscreenComposition   // FR-011: control paint requires offscreen composition
```

Surfaced as a `ControlDiagnostic` (`Types.fsi:420-426`) through the existing `Diagnostics`
channel (`RetainedRender.fs:720`), `Severity` advisory, `Message` naming the control + the
offscreen-forcing effect (opacity group / clip / drop-shadow). Detection per Phase 0f.

**Invariants.**
- Fires for a control whose paint requires offscreen composition; does not fire otherwise.
- Advisory only: never fails a build, never alters rendered output (FR-011, output byte-
  identical to pre-feature).

## 6. Optional backend SKPicture (`SceneRenderer` — not on the deterministic path)

The existing `Scene.Picture`/`PictureNode` (`Scene.fsi:341-351`) is the boundary; the
backend MAY record/replay a real `SKPicture` for an unchanged stable boundary. Contract:
**byte-identical raster** (covered by the Scene-parity / evidence path, not a new golden
count). At-rest fallback stays the passthrough (`SceneRenderer.fs:393`). No public delta
(the `Scene.Picture`/`PictureNode` surface already exists).

## State transitions (frame to frame)

| Prior frame            | This frame's input              | Damage                     | Cache outcome                          |
|------------------------|---------------------------------|----------------------------|----------------------------------------|
| rendered tree          | no change (idle)                | `0/0/0`                    | hit (entry reused), entry count steady |
| rendered tree          | one control's visual state      | small (changed box[es])    | miss on that subtree; siblings hit     |
| rendered tree          | theme switch                    | frame-spanning             | every subtree miss (key.Theme changed) |
| rendered tree          | scroll past many row identities | per-row (materialized)     | misses; LRU evicts; entry count `<= cap` |
| entry evicted earlier  | subtree needed again            | repaint (miss)             | miss (recompute fresh, never stale)    |

# Byte-identity authority (feature 116)

The authoritative statement of WHY feature 116 is byte-identical at rest (FR-014) and how the
deterministic-vs-backend split is preserved (FR-008).

## At-rest byte-identity (FR-014)

Accumulating the damage set, counting picture-cache hits/misses, maintaining the bounded LRU, and
detecting offscreen effects are all **read-only observations** layered over the work the retained `step`
already performs:

- the damage set reads boxes the step already computed (no new paint);
- the picture cache is DECOUPLED from scene emission — the emitted `SubtreeScene` still comes from the
  unchanged 091–114 reuse/repaint logic (a cache hit/miss is a counting + bounded-store update, not a
  change to which fragment is emitted);
- the offscreen detector reads the lowered scene and appends an advisory `Diagnostics` entry — it never
  alters paint.

So the emitted flat `SubtreeScene` is unchanged from the pre-116 step. This is proven by: all 430
Controls + 148 Elmish tests green (incl. the 091/092 byte-identity + 113/114 parity suites), the Scene-
parity golden suite under `Dev`, and the cache-on ≡ cache-off always-miss oracle
(`Feature116PictureCacheTests`).

## cache-on ≡ cache-off (FR-007)

The `PictureCacheEnabled` flag (mirroring 113's `MemoEnabled`) forces every cacheable boundary to a miss
when `false`. Because the cache is decoupled from emission, the rendered scene is byte-identical whether
the cache is enabled or disabled — only the hit/miss counts differ (hits = 0 when disabled). Asserted in
`Feature116PictureCacheTests` ("cache-on ≡ cache-off").

## Deterministic / backend split (FR-008)

The deterministic, golden-asserted contract is the hit/miss counts + damage metrics at the SCENE-LIST
level (`Perf.runScript`). The optional SKPicture record/replay (FR-008) is a backend raster optimization
with a byte-identical-pixels contract that never alters the flat `SubtreeScene` the goldens assert; it is
DEFERRED this rung (T023 = `[-]`, the optional MAY), so no `SceneRenderer` source changed and the
deterministic path is the sole asserted surface.

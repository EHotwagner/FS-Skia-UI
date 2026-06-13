# US2 independent validation — stable subtrees reuse a fully-keyed picture cache; reuse is provably correct

**Story**: A subtree unchanged in every render-affecting input is reused (a hit, byte-identical);
changing any one keyed input forces a miss; cache-on ≡ cache-off.

## Path

Step a stack of cacheable `data-grid-row` boundaries twice; assert the second frame is all hits and
byte-identical. Perturb one keyed input at a time (theme / box / content / a paint-neutral attr) and
assert the miss/hit split. Re-run with the `PictureCacheEnabled` always-miss oracle and assert the scene
is byte-identical with zero hits.

## Evidence

- `tests/Controls.Tests/Feature116PictureCacheTests.fs` — three stable rows are 3 hits / 0 misses on the
  second frame; the hit scene is byte-identical to a fresh `Control.renderTree`; perturbing content or
  box forces a miss on exactly that row (2 hit / 1 miss); a theme switch misses every row (0 hit / 3
  miss); the correctness key is the painted-picture digest (a paint-affecting change misses, a
  paint-neutral one hits — never a stale hit); the `PictureCacheEnabled = false` oracle yields 0 hits and
  a byte-identical scene (cache-on ≡ cache-off).
- `tests/Elmish.Tests/Feature116MetricsTests.fs` — a stable grid reports `PictureCacheHitCount = rowCount`
  / `PictureCacheMissCount = 0` on the second frame; a localized change reports exactly one miss.
- 109 perf-corpus golden `picture-cache-reuse.golden.txt` (regenerated) — the second frame reuses every
  row picture as a hit.

Correctness key (pinned in [picture-cache-authority.md](./picture-cache-authority.md)): the box + a
structural digest of the row's painted subtree, which embeds EVERY render-affecting input by
construction, so no keyed input can be omitted (a hit is always byte-identical, FR-006).

Result: PASS — reuse is provably correct; cache-on ≡ cache-off (SC-002/SC-003).

# Picture-cache authority (feature 116)

The authoritative definition of the picture cache's boundary, correctness key, bound, and the offscreen
detector, pinned here.

## Cacheable boundary

A cacheable picture boundary is a materialized **`data-grid-row`** node — the row analog of feature
113's data-grid-only memo cache (`isMemoizable = childless data-grid`). This is the natural "stable
subtree" the report's picture cache targets and the unit the eviction scenario ("scrolling a large grid
past many distinct row identities") drives. Non-row nodes are not cached this rung (consistent with the
113 precedent of caching only the DataGrid family).

## Correctness key (complete by construction)

`PictureCacheKey = { Box: Rect option; Picture: string }` where `Picture` is a structural digest
(`sprintf "%A"`) of the row's painted subtree (`Fragment.SubtreeScene`). The painted subtree IS the
rendered output, so the key embeds EVERY render-affecting input (theme colours, clip, opacity, transform,
font/text, visual-state) by construction — no keyed input can be omitted (FR-006), and a hit is always
byte-identical to a fresh paint (FR-005). An input that does NOT alter the row's paint (e.g. a
visual-state a plain row's paint does not honour) correctly HITS, because the reused picture is genuinely
identical (never a stale hit). The per-keyed-input miss matrix asserts the inputs that affect a row's
paint (theme / box / content); inputs that don't are subsumed (they cannot change the digest, so they
cannot defeat a correct hit).

## Decoupling (byte-identity preserved)

The bounded LRU + hit/miss counting OBSERVE the row pictures the `step` already built — they are
DECOUPLED from scene emission. The emitted `SubtreeScene`, the 091–114 fragment-reuse behaviour, and
every prior work-reduction count are untouched (additive only). This is why all 430 Controls + 148 Elmish
tests (incl. the byte-identity / parity suites) stay green.

## Bound + eviction

`RetainedRender.PictureCacheCap = 256` (a plain named value). Recency = the frame's deterministic
traversal order (a monotonic `Clock`, no wall-clock). Over the cap the least-recently-accessed entry is
dropped; a dropped identity re-misses when next needed (never a stale hit, FR-010). The eviction
scenario drives 320 distinct rows (1.25 × cap). NOTE: large existing corpus grids (datagrid-1000/10000)
exceed the cap and therefore exercise the bound (`PictureCacheEntryCount = 256`) — this is honest and
deterministic; the cache decoupling means their rendered scenes and prior metrics are unchanged.

## Offscreen-effect detector (FR-011)

The genuinely offscreen-composition-forcing effects in THIS renderer are: a **drop-shadow / image
filter** (`DropShadow` → `SKImageFilter.CreateDropShadow`), a **`PathClip`**, and a **non-opaque paint
over a multi-node group** (which a layered backend composites via `SaveLayer`). A **`RectClip`** lowers
to the cheap `canvas.ClipRect` (no offscreen layer) and is the ubiquitous label clip every rich-family
control emits, so it is deliberately NOT flagged (flagging it would fire on nearly every control and be
pure noise). Baked-per-paint opacity over a single node is likewise not flagged. The diagnostic is
advisory (`Severity = Info`), emitted on `step.Diagnostics`, and never alters rendered output.

## Evidence

`tests/Controls.Tests/Feature116PictureCacheTests.fs`, `Feature116CacheBoundTests.fs`,
`Feature116OffscreenDiagTests.fs`, `tests/Elmish.Tests/Feature116MetricsTests.fs`, and the regenerated
109 perf-corpus goldens (`picture-cache-reuse`, `picture-cache-eviction`).

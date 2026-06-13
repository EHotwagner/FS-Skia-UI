# Byte-identity authority (feature 117)

The authoritative statement of WHY feature 117 is byte-identical at rest and for every scenario (FR-004).

## The cache is a transparent accelerator

`Scene.measureText` (`Scene.fs:524-530`) is a **pure deterministic function** of `(text, font)`:
`Width = max 1.0 (size*0.58) * text.Length`, `Height = max 1.0 font.Size`, `Baseline = size*0.8`. It
reads no hidden state. So the cached `TextMetrics` for a key EQUALS the un-cached value for that key **by
construction** — caching changes only *how fast* a measurement is produced, never *what* value. No layout
box, fitted font size, DataGrid geometry, chart, or emitted flat `SubtreeScene` changes value because the
cache exists.

The un-cached measurement is the oracle and the cache is a transparent accelerator (spec interaction
note: byte-identity wins over hit rate).

## Where the cache is interposed (decoupled from emission)

The cache lives on `RetainedRender` (the 113/116 cache home, research R1). `RetainedRender.step` installs
a per-pass closure (over its bounded working cache + hit/miss counters) into the `[<ThreadStatic>]`
`ControlInternals` measure hook around the frame's incremental-layout pass AND its paint walk, then clears
it. The six `Control.fs` text-measure call sites (`fittedFontSize`, `buttonGeom`, `badgeGeom`,
`textFieldGeom`, `textAreaFieldGeom`, `richTextGeom`) route through `ControlInternals.measureText`, which
consults the installed hook (or falls back to the direct `Scene.measureText` when none is installed —
e.g. the non-retained `Control.renderTree` path, byte-identical to pre-117). The cache OBSERVES the
measurements the step already performs; it never changes which scene is emitted.

## cache-on ≡ cache-off (FR-004)

The `TextCacheEnabled` flag (mirroring 113's `MemoEnabled` and 116's `PictureCacheEnabled`) forces every
measurement to re-measure via `Scene.measureText` and count a miss when `false`, never consulting or
populating the cache. Because the cached value equals the un-cached value, the emitted scene, the surfaced
bounds, and `RemeasuredNodeCount` are byte-identical whether the cache is enabled or disabled — only the
hit/miss counts differ (hits = 0 when disabled). Asserted directly in `Feature117TextCacheTests` ("the
always-miss oracle yields a byte-identical SCENE and layout").

## Proof

- all 445 Controls + 155 Elmish tests green (incl. the 091/092 byte-identity + 113/114/116 parity
  suites), unchanged from pre-117;
- the Scene-parity golden suite under `Dev` (at-rest byte-identity);
- the cache-on ≡ cache-off always-miss oracle (`Feature117TextCacheTests`);
- the per-keyed-input miss matrix (text/family/size/weight) proving no input is omitted (FR-002);
- the hit-equals-un-cached-measure assertion (FR-004).

# Quickstart: Paint Cache, Damage Rectangles & Optional Skia Picture Boundaries

This feature is **additive observability + a correctly-keyed bounded picture cache +
advisory diagnostics** on the retained render path. It adds six public `FrameMetrics` fields
and one advisory `ControlDiagnosticCode` case; at-rest rendered output stays byte-identical
(FR-014) and cache-on ≡ cache-off (FR-007).

## Run `Route` first

```
./fake.sh build -t Route
```

`Route` is expected to **escalate to the controls-public-surface tier** (the
`Controls.Elmish` `FrameMetrics` and the `Controls` `Types` diagnostics `.fsi` surfaces
change). Run only the gates it prints.

## The escalated maintainer-verify gate set (sequential — FAKE shares `.fake` state)

```
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

## Regenerate baselines + corpus goldens (after the `.fsi` additions)

```
# top-level + per-package surface baselines (FrameMetrics fields + ControlDiagnosticCode case)
./fake.sh build -t RefreshSurfaceBaselines

# perf corpus goldens carrying the six new metric fields + the new reuse/eviction scenarios
PERF_CORPUS_REGEN=1 ./fake.sh build -t Dev
```

## Targeted test runs

```
# damage set (small hover vs frame-spanning theme vs idle-zero; deterministic integer area)
dotnet test tests/Controls.Tests --filter Feature116Damage

# fully-keyed picture cache: per-keyed-input miss + hit byte-identity + always-miss oracle
dotnet test tests/Controls.Tests --filter Feature116PictureCache

# bounded LRU: EntryCount <= cap, deterministic eviction, evicted-entry re-miss
dotnet test tests/Controls.Tests --filter Feature116CacheBound

# advisory offscreen-effect diagnostic fires/does-not-fire, output unchanged
dotnet test tests/Controls.Tests --filter Feature116OffscreenDiag

# six FrameMetrics goldens over Perf.runScript
dotnet test tests/Elmish.Tests --filter Feature116Metrics
```

## What to verify

- **US1 / FR-001-004**: localized hover → small `RepaintedNodeCount`/`DirtyRectCount`/
  `DirtyArea`; theme switch → frame-spanning; idle → `0/0/0`.
- **US2 / FR-005-007**: stable subtree → `PictureCacheHitCount` hit + byte-identical output;
  each keyed input (theme/box/clip/opacity/transform/font-text/visual-state) independently →
  `PictureCacheMissCount` miss; cache-on ≡ cache-off (always-miss oracle).
- **US3 / FR-009-010**: `PictureCacheEntryCount <= cap` under eviction pressure; deterministic
  eviction; evicted entry re-misses (never stale).
- **US4 / FR-011**: advisory offscreen-effect diagnostic fires for opacity-group/clip/drop-
  shadow controls, not otherwise; rendered output unchanged.
- **US5 / FR-012**: all six metrics deterministic + golden-asserted; a regression that
  repaints a stable subtree, widens damage, or blows the cap fails a golden.
- **Byte-identity (FR-014)**: the standing Scene-parity golden suite under `Dev` (at rest +
  the SKPicture byte-identical-raster path) is unchanged.

## Evidence locations

- `specs/116-paint-cache-damage-rects/readiness/` — `evidence-audit.md` (verdict token),
  `skill-loading-evidence.md`, byte-identity authority note, window-visibility not-applicable
  set, generated-validation package-resolution tokens.
- `readiness/surface-baselines/` + `readiness/per-package-surface/` — regenerated baselines.
- `specs/109-perf-metrics-baseline/readiness/perf-corpus/*.golden.txt` — regenerated goldens.

# Quickstart — Layout Hot-Path Improvements (Feature 117)

How to build, test, regenerate evidence, and run the routed gate set for this feature.

## 1. Route first — run only the gates it prints

```bash
./fake.sh build -t Route            # prints the tier + minimal gate list for this diff
./fake.sh build -t Route --enforce  # additionally fails on a missing required evidence artifact
```

Expected: **controls-public-surface** escalation, because `ControlsElmish.fsi`'s
`FrameMetrics` gains three public fields (consistent with 109/110/113/114/116).

## 2. Targeted tests (Controls internal seams + Elmish goldens)

```bash
# Text-cache key / hit-miss / always-miss oracle / empty / fitted-caption
dotnet test tests/Controls.Tests --filter "FullyQualifiedName~Feature117TextCache"
# Bounded LRU: entry count <= cap, deterministic eviction, evicted-entry re-miss
dotnet test tests/Controls.Tests --filter "FullyQualifiedName~Feature117CacheBound"
# Layout-invalidated count + style-only zero-work + drift-guard-empty
dotnet test tests/Controls.Tests --filter "FullyQualifiedName~Feature117LayoutInvalidated"
# Three FrameMetrics goldens over Perf.runScript (cold->warm, style-only, idle)
dotnet test tests/Elmish.Tests --filter "FullyQualifiedName~Feature117Metrics"
```

Internal seams (the cache, `WorkReductionRecord` carriers, `TextCacheEnabled`) are reached via
`InternalsVisibleTo "Controls.Tests"`.

## 3. Regenerate evidence after a deliberate metric change

```bash
# Perf corpus goldens — carry the three new FrameMetrics fields + the new scenarios
PERF_CORPUS_REGEN=1 dotnet test tests/Elmish.Tests --filter "FullyQualifiedName~Perf"

# Surface baselines — top-level + per-package, after the FrameMetrics additions
./fake.sh build -t RefreshSurfaceBaselines
```

The corpus dir is `specs/109-perf-metrics-baseline/readiness/perf-corpus/`. Regenerate **only**
when a metric value change is intended; an unexpected golden diff is a regression signal.

## 4. Byte-identity check (FR-004)

```bash
# at-rest byte-identity (Scene-parity golden suite) + cache-on == cache-off oracle
./fake.sh build -t Dev
```

The always-miss oracle test (`TextCacheEnabled = false`) runs under `Dev`; it proves cache-on
output and layout equal cache-off output and layout over the corpus.

## 5. Escalated gate set (run sequentially — FAKE shares `.fake` state)

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Do **not** run FAKE-backed targets concurrently. If a failure looks race-like, rerun the
affected targets sequentially before product debugging.

## 6. Evidence readiness checklist

- `specs/117-layout-hot-path/readiness/evidence-audit.md` — verdict token, 0 synthetic.
- `specs/117-layout-hot-path/readiness/skill-loading-evidence.md` — per (task, skill) rows.
- `specs/117-layout-hot-path/readiness/generated-validation.md` —
  `package-resolution=resolved`, `package-mismatch=false`.
- byte-identity authority + text-cache authority notes; window-visibility not-applicable set.
- `readiness/surface-baselines/` + `readiness/per-package-surface/` regenerated.

## Key file anchors (for implementers)

- `Scene.measureText` — `Scene.fs:524-530` (pure; the cache wraps it from Controls).
- Call sites — `Control.fs:239` (fittedFontSize), `:786`, `:821`, `:966`, `:996`, `:1017`.
- `WorkReductionRecord` — `RetainedRender.fsi:157-205`; construction `RetainedRender.fs:943`.
- `RemeasuredNodeCount` (post-pinning) — `RetainedRender.fs:575`; dirty set (pre-pinning) —
  `layoutDirtySet` `RetainedRender.fs:497-504` → `Control.fs:1307`.
- Always-miss precedent — `MemoEnabled` `RetainedRender.fs:81`/`.fsi:130`, `PictureCacheEnabled`
  `:87`/`.fsi:145` (init `:479/:482`, carry `:932/:935`).
- `FrameMetrics` — `ControlsElmish.fsi:68-174`; `zero` `ControlsElmish.fs:1366-1388`; per-frame
  construction `:1421/:1478`; live sink `:1003`. `Perf.runScript` `ControlsElmish.fsi:472-476`.

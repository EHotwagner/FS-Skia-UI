# Quickstart: Backend Paint Replay & Performance Honesty

Local verification recipes for the four user stories. Run `./fake.sh build -t Route` first and
obey its printed gate list; FAKE-backed targets run **sequentially**.

## US1 — per-phase paint/compose timing

```sh
# Regenerate the non-golden live timing baseline (real Stopwatch/GC) on the corpus.
PERF_BASELINE_REGEN=1 ./fake.sh build -t Dev
# Inspect: each scenario reports PaintDuration + ComposeDuration distinctly.
cat docs/reports/_baselines/2026-06-13-paint-replay-after.md
```
Expected: paint and present/compose durations are separated; deterministic count goldens under
`specs/.../perf-corpus/*.golden.txt` are byte-identical to before the timing fields were added.

## US2 — idle / unchanged frames do no paint

```sh
# Pure-transition test: an unchanged frame with no cause emits no RenderFrame effect.
dotnet test tests/Elmish.Tests   # Feature120 idle-skip transition tests
# Live proof on Mesa: idle run reports zero paint work, forced repaint is byte-identical.
cat specs/120-backend-paint-replay-cache/readiness/smoke/idle-zero-redraw.md
```
Expected: after the first paint, unchanged frames show `PaintDuration ≈ 0` / zero draw-call
re-issue; a forced repaint afterward matches the first frame's readback pixels.

## US3 — replay cache + collision-free key

```sh
# Controls-side: structural fingerprint equality + the collision-miss the old %A would hit.
dotnet test tests/Controls.Tests       # Feature120 fingerprint tests
# Backend cache: LRU bound + native dispose.
dotnet test tests/SkiaViewer.Tests     # Feature120 PictureReplayCache tests
# Real-hardware parity (the load-bearing proof):
cat specs/120-backend-paint-replay-cache/readiness/smoke/replay-readback-parity.md
cat specs/120-backend-paint-replay-cache/readiness/smoke/forced-staleness.md
```
Expected: `render(replay=on)` and `render(replay=off)` readback byte-identical for every corpus
scene; a render-affecting change re-records (no stale pixels); on the 10000-row grid a small
change reports `ReplaySkippedNodeCount` = the large majority of subtree paint nodes and a lower
`PaintDuration` than the replay-off baseline.

## US4 — honesty / correctness cleanups

```sh
# DirtyArea union (overlapping damage no longer double-counts):
dotnet test tests/Controls.Tests       # Feature120 dirty-area-union test
# Present-mode docstring + sample:
grep -n "DirectToSwapchain" src/SkiaViewer/SkiaViewer.fsi samples/DemoReel/Program.fs
# Dead ref removed:
! grep -n "lastRuntimeStateTouched" src/Controls.Elmish/ControlsElmish.fs
```
Expected: docstring names `DirectToSwapchain` default; DemoReel live window uses it; dead ref
absent; `DirtyArea` for overlapping repaints equals the union area and never exceeds frame area.

## Merge-readiness gate order (escalated, `.fsi` changed)

```sh
./fake.sh build -t Route            # authoritative gate list for THIS diff
./fake.sh build -t Dev
# then the printed controls/package public-surface + generated/template + evidence gates,
# sequentially: GeneratedProductCheck, TemplateCheck, EvidenceGraph, EvidenceAudit
```
Target: full routed set + `EvidenceAudit` PASS, **0 synthetic**. Post-merge follow-up: version
bump all packable libs + `/fs-skia-template-update` re-pin (116–119 pattern).

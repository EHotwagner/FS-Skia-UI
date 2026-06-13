# Paint/Replay timing baseline — AFTER (feature 120)

Non-golden, live-only. Captured on the Linux/AMD Mesa OpenGL reference host (display :1) via the
production `Host.Viewer.run → GlHost.run → renderFrameDirect` path (bounded evidence harness,
`specs/120-backend-paint-replay-cache/readiness/sample-smoke/evidence-harness.fs`).

## Per-phase present timing is now observable (US1, FR-001/FR-003, SC-001)

A presented frame reports DISTINCT per-phase durations (`GlHost.lastPresentTiming`):

| phase | duration |
|-------|----------|
| PaintDuration (scene→canvas walk) | 00:00:00.0006318 (~0.63 ms) |
| ComposeDuration (flush + SwapBuffers) | 00:00:00.0074077 (~7.41 ms) |

The two phases are clearly separated, so the bottleneck is measured, not inferred. These are
live-only diagnostics and are EXCLUDED from the deterministic count goldens (SC-001/FR-002) —
`TimeSpan.Zero` on the `Perf.runScript` path.

## Unchanged-frame zero-redraw (US2, SC-002)

An unchanged frame is idle-skipped: `lastPresentTiming` reports `(00:00:00, 00:00:00)` — no surface
clear, no scene walk, no draw-call re-issue. The double-buffered front buffer holds the last
presented frame.

## Replay work-reduction (US3, SC-004) — deterministic signal

On a warm stable grid the deterministic model reports `ReplaySkippedNodeCount > 0` (the subtree paint
nodes a replay hit avoids) with `ReplayHitCount = PictureCacheHitCount`. The directional paint-duration
reduction from replay is observed here on the reference host (non-golden); byte-identity (SC-003) is
the gating correctness criterion, proven by the cache-on/off pixel parity test.

MissingCounters: none

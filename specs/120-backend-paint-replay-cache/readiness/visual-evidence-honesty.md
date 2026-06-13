# visual-evidence-honesty — feature 120 (US2/US3)

status=applicable

Feature 120 is explicit about what its evidence proves:

- **Pixel byte-identity (US3, SC-003)** is proven by a real render: a scene of `CachedSubtree`
  replay boundaries is rendered through the **production shared painter** `SceneRenderer.paintNode`
  to a real `SKSurface`, snapshotted, PNG-encoded, and the byte arrays compared — cache-OFF (direct
  walk) == disabled oracle == warmed replay (`Feature120ReplayCacheTests` cache-on/off parity). This
  proves a replayed boundary's pixels equal the direct walk; it does not by itself prove desktop
  visibility.
- **Live present / timing / idle-skip (US1/US2/US4)** is proven by a real bounded launch on the
  OpenGL backend on display `:1` (`sample-smoke/live-host-evidence.txt`): present diagnostic
  `readback=false`, a presented frame's distinct non-zero paint+compose durations, and an
  idle frame's `(0,0)` timing (idle-skipped — no clear/walk/swap).
- The **timing fields are honest and non-golden**: `PaintDuration`/`ComposeDuration` are live-only
  and `TimeSpan.Zero` on the deterministic path, so the count goldens stay byte-identical (SC-001).
- No visual claim overstates the artifacts. The replay cache is transparent (oracle-backstopped,
  FR-011): a fingerprint collision degrades to a missed optimization, never a wrong pixel.

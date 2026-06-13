# Paint/Replay timing baseline — BEFORE (feature 120 precondition)

Non-golden, live-only. Captured on the Linux/AMD Mesa OpenGL reference host (display :1).

Before feature 120 the host reported only a single whole-frame duration; paint (scene→canvas walk)
and compose (flush + buffer-swap) could not be isolated, and every present re-cleared and re-walked
the entire cached scene — even idle frames. There was no per-phase number to record here; this file
documents the precondition the report named (US1) so the AFTER baseline's split is measured, not
asserted.

MissingCounters: PaintDuration, ComposeDuration (not yet captured before this feature)

# window-visibility — feature 120 (US2)

status=applicable
mode=live-windowed
window-visible=observed
accessible-window=observed
first-frame-presented=observed
self-closed-for-evidence=observed

Feature 120's US2 (idle-skip) is exercised on a real persistent **OpenGL** window on display `:1`
through the production path `Host.Viewer.run → GlHost.run → renderFrame → renderFrameDirect`. The
bounded evidence harness opens the window (window-visible), presents its first frame and continues
through changed + unchanged (idle-skipped) frames, then self-closes after the bounded tick count
(self-closed-for-evidence) without a user action. The run is driven the same way a user-reachable
host runs — an Elmish `ViewerProgram` over `Viewer.run` — in the default `DirectToSwapchain` mode.
accessible-window=observed — a real native window the windowing system created and rendered into
(result `Ok ()`, present diagnostic emitted, frames presented).

Companion artifacts:

- [sample-smoke/live-host-evidence.txt](./sample-smoke/live-host-evidence.txt) — the live launch:
  present-mode readback=false, a presented frame's distinct paint/compose durations, and an idle
  frame's zero timing (idle-skip).
- [smoke/idle-zero-redraw.md](./smoke/idle-zero-redraw.md) — the zero-redraw decision + proof.
- [visual-evidence-honesty.md](./visual-evidence-honesty.md) — honest about what the captures prove.
- [real-image-evidence.md](./real-image-evidence.md) — production-painter pixel parity.

The interactive-UI run-and-use gate is satisfied: the windowed viewer was launched and driven
through the production `renderFrame`/`renderFrameDirect` path on a real OpenGL backend, exercising
the feature's idle-skip and present timing.

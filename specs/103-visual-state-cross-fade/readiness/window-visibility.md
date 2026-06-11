# Window visibility — applicability decision (feature 103, R6, T002/T003)

status=not-applicable
mode=render-only
window-obligation=none

The persistent-launch / viewer-launch task-generation rule does **not** apply to R6. R6 adds and
changes **no** default-executable, persistent-launch, or graphical entry point. The cross-fade is
GPU-free deterministic scene assembly exercised through `RetainedRender.step` with injected `TimeSpan`
deltas; there is no window, no screenshot, and no desktop-visibility claim. The two stable points
(at-rest, settled) stay byte-identical to the static render.

The full window-visibility evidence set records this not-applicable decision with honest values:

- [interactive-visible-window.md](./interactive-visible-window.md) — status=not-applicable, mode=render-only
- [close-reason-separation.md](./close-reason-separation.md) — no window close to classify
- [window-state-diagnostics.md](./window-state-diagnostics.md) — every diagnostic-class not-applicable
- [window-options.md](./window-options.md) — every option not-applicable
- [real-image-evidence.md](./real-image-evidence.md) — no image produced; proof is structural Scene assertion
- [generated-validation.md](./generated-validation.md) — nothing ships into the template/generated products

No live desktop window is involved at any point — R6 is a framework-internal render-path behavior change.

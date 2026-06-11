# Window visibility — applicability decision (feature 102, R8, T002/T003)

status=not-applicable
mode=render-only
window-obligation=none

## Visible decision (T003)

The persistent-launch / viewer-launch task-generation rule does **not** apply to R8. R8 adds and
changes **no** default-executable, persistent-launch, or graphical entry point and changes **no**
observable rendering output — every edit is roadmap report prose or a descriptive in-source comment.
There is no window, no screenshot, and no desktop-visibility claim. Rendering output is byte-identical
to pre-R8.

The full window-visibility evidence set records this not-applicable decision with honest values:

- [interactive-visible-window.md](./interactive-visible-window.md) — status=not-applicable, mode=render-only
- [close-reason-separation.md](./close-reason-separation.md) — no window close to classify
- [window-state-diagnostics.md](./window-state-diagnostics.md) — every diagnostic-class not-applicable
- [window-options.md](./window-options.md) — every option not-applicable
- [real-image-evidence.md](./real-image-evidence.md) — no image produced; rendering output byte-identical to pre-R8
- [generated-validation.md](./generated-validation.md) — nothing ships into the template/generated products

No live desktop window is involved at any point — R8 is a documentation/internal-comment honesty pass.

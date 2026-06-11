# Window visibility — applicability decision (feature 101, R7, T002/T003)

status=not-applicable
mode=render-only
window-obligation=none

## Visible decision (T003)

The persistent-launch / viewer-launch task-generation rule does **not** apply to R7. R7 adds and
changes **no** default-executable, persistent-launch, or graphical entry point and changes **no**
observable rendering output (R2 INV-1 is preserved byte-identically). The user-reachable surface for
this hardening feature is the **build/test gate itself**: a contributor who introduces dirty-set drift
gets a fast, explicit, named Expecto failure under `Dev`. There is no window, no screenshot, and no
desktop-visibility claim.

The full window-visibility evidence set records this not-applicable decision with honest values:

- [interactive-visible-window.md](./interactive-visible-window.md) — status=not-applicable, mode=render-only
- [close-reason-separation.md](./close-reason-separation.md) — no window close to classify
- [window-state-diagnostics.md](./window-state-diagnostics.md) — every diagnostic-class not-applicable
- [window-options.md](./window-options.md) — every option not-applicable
- [real-image-evidence.md](./real-image-evidence.md) — no image produced; rendering output is byte-identical to pre-R7
- [generated-validation.md](./generated-validation.md) — nothing ships into the template/generated products

No live desktop window is involved at any point — R7 is a framework-internal classifier guard exercised
entirely by in-process Expecto tests.
